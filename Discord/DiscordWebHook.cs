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

        private UInt64 queueNum = 0;
        private readonly ConcurrentQueue<(UInt64 num, DiscordMessage message, FileInfo[] files)> _queue = new ConcurrentQueue<(UInt64 num, DiscordMessage, FileInfo[])>();
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private bool _isProcessing;

        public async Task SendAsync(DiscordMessage message, params FileInfo[] files)
        {
            if (string.IsNullOrEmpty(Url))
                throw new ArgumentNullException("Invalid Webhook URL.");

            string boundary = "------------------------" + DateTime.Now.Ticks.ToString("x");

            using (var client = new HttpClient() { Timeout = TimeSpan.FromSeconds(30) })
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
                    var response = await client.PostAsync(Url, content);
                    response.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException ex)
                {
                    Debug.WriteLine($"Discord webhook request failed: {ex.Message}");
                }
            }
        }

        private async Task ProcessQueueAsync()
        {
            while (_queue.TryDequeue(out var item))
            {
                await _semaphore.WaitAsync();
                try
                {
                    Debug.WriteLine($"Processing message {item.num}. Queue size: {_queue.Count}");
                    await SendAsync(item.message, item.files);
                    Debug.WriteLine($"Message {item.num} sent.");
                }
                finally
                {
                    _semaphore.Release();
                }
                Task.Delay(500).Wait();
            }
            _isProcessing = false;
        }

        public void Send(DiscordMessage message, params FileInfo[] files)
        {
            _queue.Enqueue((queueNum++, message, files));
            Debug.WriteLine($"Message {queueNum-1} added. Queue size: {_queue.Count}");
            if (_isProcessing)
            {
                Debug.WriteLine("Already processing queue.");
                return;
            }
            Debug.WriteLine("Run ProcessQueueAsync");
            _isProcessing = true;
            Task.Run(ProcessQueueAsync);
        }
    }
}