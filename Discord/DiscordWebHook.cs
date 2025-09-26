using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Webhook
{
    public class DiscordWebhook
    {
        public string Url { get; set; }
        public int queueRetryCount = 3;
        public int sendTimeoutSeconds = 5;

        private UInt64 totalMessages = 0;
        private ConcurrentQueue<(UInt64 num, DiscordMessage message, FileInfo[] files)> _queue = new ConcurrentQueue<(UInt64 num, DiscordMessage, FileInfo[])>();
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private bool _isProcessing;
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

        public async Task<bool> SendAsync(DiscordMessage message, params FileInfo[] files)
        {
            if (string.IsNullOrEmpty(Url))
                throw new ArgumentNullException("Invalid Webhook URL.");

            string boundary = "------------------------" + DateTime.Now.Ticks.ToString("x");

            using (var client = new HttpClient() { Timeout = TimeSpan.FromSeconds(sendTimeoutSeconds) })
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

                try
                {
                    var response = await client.PostAsync(Url, content, _cts.Token);
                    response.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException ex)
                {
                    Debug.WriteLine($"SendAsync: Discord webhook request failed: {ex.Message}");
                    lastHpptEx = ex;
                    return false;
                }
                catch (TaskCanceledException ex)
                {
                    Debug.WriteLine($"SendAsync: Discord webhook request cancelled: {ex.Message}");
                    return false;
                }
            }
            return true;
        }

        private async Task ProcessQueueAsync()
        {
            queueErrorCounter = 0;

            while (_queue.TryPeek(out var item))
            {
                if (_cts.Token.IsCancellationRequested)
                {
                    Debug.WriteLine("ProcessQueueAsync: Discord queue processing cancelled.");
                    break;
                }
                await _semaphore.WaitAsync();
                try
                {
                    Debug.WriteLine($"ProcessQueueAsync: Processing message {item.num}. Queue size: {_queue.Count}");
                    if (await SendAsync(item.message, item.files))
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
                    await Task.Delay(1000, _cts.Token); // Discord rate limit: 1 message per second
                }
                catch (TaskCanceledException)
                {
                    Debug.WriteLine($"ProcessQueueAsync: Discord queue processing cancelled during delay. Was {_queue.Count} messages in queue. {totalMessages} messages in session. ");
                    break;
                }
            }
            if ( _cts.IsCancellationRequested)
            {
                Debug.WriteLine($"ProcessQueueAsync: Discord queue processing cancelled. Was {_queue.Count} messages in queue.");
                Debug.WriteLine("Clearing queue.");
                var _newqueue = new ConcurrentQueue<(UInt64 num, DiscordMessage, FileInfo[])>();
                Interlocked.Exchange(ref _queue, _newqueue);
            }
            Debug.WriteLine($"ProcessQueueAsync: Discord queue processing finished.");
            _isProcessing = false;
        }

        public void Send(DiscordMessage message, params FileInfo[] files)
        {
            _queue.Enqueue((totalMessages++, message, files));
            Debug.WriteLine($"Message {totalMessages-1} added. Queue size: {_queue.Count}");
            if (_isProcessing)
            {
                Debug.WriteLine("Already processing queue.");
                return;
            }
            Debug.WriteLine("Run ProcessQueueAsync");
            _cts.Dispose();
            _cts = new CancellationTokenSource();
            _isProcessing = true;
            Task.Run(ProcessQueueAsync);
            return;
        }
    }
}