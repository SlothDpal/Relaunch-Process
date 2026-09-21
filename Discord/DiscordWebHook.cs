using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Webhook
{
    // Результат одной попытки отправки. Позволяет разобраться в причине
    // неудачи (в т.ч. rate limit от Discord)
    public sealed class DiscordSendResult
    {
        public bool Success { get; private set; }
        public HttpStatusCode? StatusCode { get; private set; }
        public string ErrorBody { get; private set; }
        public TimeSpan? RetryAfter { get; private set; }
        public Exception Exception { get; private set; }

        public static DiscordSendResult Ok() => new DiscordSendResult { Success = true };
        public static DiscordSendResult Fail(HttpStatusCode? statusCode = null, string errorBody = null, TimeSpan? retryAfter = null, Exception exception = null)
            => new DiscordSendResult
            {
                Success = false,
                StatusCode = statusCode,
                ErrorBody = errorBody,
                RetryAfter = retryAfter,
                Exception = exception
            };
    }

    public class DiscordWebhook
    {
        public string Url { get; set; }
        public int queueRetryCount = 3;
        public int sendTimeoutSeconds = 5;
        // используем общий экземпляр HttpClient для всех запросов, чтобы избежать проблем с исчерпанием сокетов
        // HttpClient предназначен для повторного использования и потокобезопасен.
        private static readonly HttpClient _httpClient = new HttpClient();
        private UInt64 totalMessages = 0;
        private ConcurrentQueue<(UInt64 num, DiscordMessage message, FileInfo[] files)> _queue = new ConcurrentQueue<(UInt64 num, DiscordMessage, FileInfo[])>();
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private bool _isProcessing;
        private readonly object _isProcessingFlagLock = new object();
        private CancellationTokenSource _cts = new CancellationTokenSource();
        private int queueErrorCounter = 0;
        private int queueSuppressedCounter = 0;
        private HttpRequestException lastHpptEx = null;

        public UInt64 TotalMessages => totalMessages;
        public int QueueSize => _queue.Count;
        public void CancelProcessing() => _cts.Cancel();
        public int ErrorCount => queueErrorCounter;
        public bool IsProcessing => _isProcessing;
        public HttpRequestException LastHpptEx => lastHpptEx;

        public async Task<DiscordSendResult> SendAsync(DiscordMessage message, params FileInfo[] files)
        {
            if (string.IsNullOrEmpty(Url))
                throw new ArgumentNullException("Invalid Webhook URL.");

            string boundary = "------------------------" + DateTime.Now.Ticks.ToString("x");

            using (var content = new MultipartFormDataContent(boundary))
            {
                // Добавляем JSON payload
                var jsonContent = new StringContent(message.ToString(), Encoding.UTF8, "application/json");
                jsonContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
                {
                    Name = "\"payload_json\""
                };
                content.Add(jsonContent);

                // Добавляем файлы
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Exists)
                    {
                        var fileContent = new ByteArrayContent(File.ReadAllBytes(files[i].FullName));
                        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                        fileContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
                        {
                            Name = $"\"file_{i}\"",
                            FileName = $"\"{files[i].Name}\""
                        };
                        content.Add(fileContent);
                    }
                }

                // эмулируем таймаут соединения, используя CancellationTokenSource с заданным временем ожидания
                // если запрос не завершится за указанное время, будет выброшено исключение TaskCanceledException
                // так как HttpClient не имеет встроенного таймаута для отдельных запросов, мы используем
                // CancellationTokenSource для управления временем ожидания
                using (var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(sendTimeoutSeconds)))
                using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token, timeoutCts.Token))
                {
                    HttpResponseMessage response;
                    try
                    {
                        response = await _httpClient.PostAsync(Url, content, linkedCts.Token);
                    }
                    catch (HttpRequestException ex)
                    {
                        Debug.WriteLine($"SendAsync: Discord webhook request failed: {ex.Message}");
                        lastHpptEx = ex;
                        return DiscordSendResult.Fail(exception: ex);
                    }
                    catch (TaskCanceledException ex)
                    {
                        bool wasTimeout = timeoutCts.IsCancellationRequested && !_cts.IsCancellationRequested;
                        Debug.WriteLine(wasTimeout
                            ? $"SendAsync: Discord webhook request timed out after {sendTimeoutSeconds}s."
                            : $"SendAsync: Discord webhook request cancelled: {ex.Message}");
                        return DiscordSendResult.Fail(exception: ex);
                    }

                    using (response)
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            return DiscordSendResult.Ok();
                        }

                        // Discord возвращает 429 при превышении rate limit и указывает,
                        // сколько секунд подождать, в заголовке Retry-After.
                        if (response.StatusCode == (HttpStatusCode)429)
                        {
                            TimeSpan? retryAfter = response.Headers.RetryAfter?.Delta;
                            string body429 = await SafeReadBodyAsync(response);
                            Debug.WriteLine($"SendAsync: rate limited by Discord. Retry-After: {retryAfter?.TotalSeconds ?? -1}s. Body: {body429}");
                            return DiscordSendResult.Fail(statusCode: response.StatusCode, errorBody: body429, retryAfter: retryAfter);
                        }

                        string body = await SafeReadBodyAsync(response);
                        Debug.WriteLine($"SendAsync: Discord webhook request failed ({(int)response.StatusCode}): {body}");
                        return DiscordSendResult.Fail(statusCode: response.StatusCode, errorBody: body);
                    }
                }
            }
        }

        private static async Task<string> SafeReadBodyAsync(HttpResponseMessage response)
        {
            try
            {
                return await response.Content.ReadAsStringAsync();
            }
            catch
            {
                return string.Empty;
            }
        }

        private async Task ProcessQueueAsync()
        {
            queueErrorCounter = 0;

            while (_queue.TryPeek(out var queueItem))
            {
                if (_cts.Token.IsCancellationRequested)
                {
                    Debug.WriteLine("ProcessQueueAsync: Discord queue processing cancelled.");
                    break;
                }
                await _semaphore.WaitAsync();
                DiscordSendResult result;
                try
                {
                    Debug.WriteLine($"ProcessQueueAsync: Processing message {queueItem.num}. Queue size: {_queue.Count}");
                    result = await SendAsync(queueItem.message, queueItem.files);
                    if (result.Success)
                    {
                        _queue.TryDequeue(out var deqItem);
                        queueErrorCounter = 0;
                    }
                    else
                    {
                        queueErrorCounter++;
                        if (queueErrorCounter == queueRetryCount)
                        {
                            _queue.TryDequeue(out var deqItem);
                            queueErrorCounter = 0;
                            queueSuppressedCounter++;
                            Debug.WriteLine($"ProcessQueueAsync: Message dropped. Total messages dropped:{queueSuppressedCounter}. Queue size: {_queue.Count}.");
                        }
                    }
                }
                finally
                {
                    _semaphore.Release();
                }
                try
                {
                    // Если Discord вернул 429 с Retry-After — ждём именно столько,
                    // иначе используем стандартный лимит 1 сообщение в секунду.
                    TimeSpan delay = result.RetryAfter ?? TimeSpan.FromSeconds(1);
                    await Task.Delay(delay, _cts.Token);
                }
                catch (TaskCanceledException)
                {
                    Debug.WriteLine($"ProcessQueueAsync: Discord queue processing cancelled during delay. Was {_queue.Count} messages in queue. {totalMessages} messages in session. ");
                    break;
                }
            }
            if (_cts.IsCancellationRequested)
            {
                Debug.WriteLine($"ProcessQueueAsync: Discord queue processing cancelled. Was {_queue.Count} messages in queue.");
                Debug.WriteLine("Clearing queue.");
                var _newqueue = new ConcurrentQueue<(UInt64 num, DiscordMessage, FileInfo[])>();
                Interlocked.Exchange(ref _queue, _newqueue);
            }
            Debug.WriteLine($"ProcessQueueAsync: Discord queue processing finished.");
            lock (_isProcessingFlagLock)
            {
                _isProcessing = false;
            }
        }

        public void Send(DiscordMessage message, params FileInfo[] files)
        {
            _queue.Enqueue((totalMessages++, message, files));
            Debug.WriteLine($"Message {totalMessages - 1} added. Queue size: {_queue.Count}");

            lock (_isProcessingFlagLock)
            {
                if (_isProcessing)
                {
                    Debug.WriteLine("Already processing queue.");
                    return;
                }

                _isProcessing = true;
                _cts.Dispose();
                _cts = new CancellationTokenSource();
            }

            Debug.WriteLine("Run ProcessQueueAsync");
            Task.Run(ProcessQueueAsync);
            return;
        }
    }
}