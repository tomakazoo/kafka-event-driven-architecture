using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using System.Text.Json;

namespace PerformanceTuning.Consumer
{
    /// <summary>
    /// Parallel consumer demonstrating concurrent message processing
    /// Uses SemaphoreSlim to limit concurrent workers
    /// </summary>
    class ParallelConsumer
    {
        private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(10); // Max 10 concurrent workers
        private static int ProcessedCount = 0;
        private static int ErrorCount = 0;

        static void Main(string[] args)
        {
            Console.WriteLine("⚡ Parallel Consumer");
            Console.WriteLine("💡 Processing messages concurrently");
            Console.WriteLine("📡 Connecting to: localhost:9092");
            Console.WriteLine();

            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "parallel-consumer-group",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                
                // Fetch settings for throughput
                FetchMinBytes = 1024,           // Wait for 1KB minimum
                FetchMaxWaitMs = 500,           // But no more than 500ms
                MaxPartitionFetchBytes = 1048576,  // 1MB per partition
                
                // Processing
                MaxPollRecords = 500,           // Fetch 500 records per poll
                MaxPollIntervalMs = 300000,     // 5 minutes max processing time
                
                // Session management
                SessionTimeoutMs = 10000,       // 10 second timeout
                HeartbeatIntervalMs = 3000,     // Heartbeat every 3 seconds
                
                EnableAutoCommit = true,
                AutoCommitIntervalMs = 5000
            };

            using var consumer = new ConsumerBuilder<string, string>(config).Build();
            consumer.Subscribe("performance-topic");

            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            Console.WriteLine("✅ Consumer subscribed");
            Console.WriteLine($"💡 Max concurrent workers: {Semaphore.CurrentCount}");
            Console.WriteLine();

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var tasks = new ConcurrentBag<Task>();

            try
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    var result = consumer.Consume(cts.Token);

                    // Limit concurrent processing
                    await Semaphore.WaitAsync(cts.Token);

                    var task = Task.Run(async () =>
                    {
                        try
                        {
                            await ProcessMessageAsync(result);
                        }
                        finally
                        {
                            Semaphore.Release();
                        }
                    }, cts.Token);

                    tasks.Add(task);

                    // Show progress
                    if (ProcessedCount % 100 == 0 && ProcessedCount > 0)
                    {
                        Console.WriteLine($"📊 Processed: {ProcessedCount}, Errors: {ErrorCount}, Active: {10 - Semaphore.CurrentCount}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine();
                Console.WriteLine("🛑 Stopping consumer...");
            }
            finally
            {
                // Wait for all tasks to complete
                Console.WriteLine("⏳ Waiting for all tasks to complete...");
                await Task.WhenAll(tasks);
                stopwatch.Stop();

                Console.WriteLine();
                Console.WriteLine("═══════════════════════════════════════════════════════");
                Console.WriteLine("📊 Performance Results");
                Console.WriteLine("═══════════════════════════════════════════════════════");
                Console.WriteLine($"Processed: {ProcessedCount}");
                Console.WriteLine($"Errors: {ErrorCount}");
                Console.WriteLine($"Time: {stopwatch.ElapsedMilliseconds} ms");
                Console.WriteLine($"Throughput: {ProcessedCount * 1000.0 / stopwatch.ElapsedMilliseconds:F2} msg/s");
                Console.WriteLine();
                Console.WriteLine("💡 Parallel processing improves consumer throughput!");
            }
        }

        static async Task ProcessMessageAsync(ConsumeResult<string, string> result)
        {
            try
            {
                var message = JsonSerializer.Deserialize<JsonElement>(result.Message.Value);
                
                // Simulate processing work
                await Task.Delay(10); // 10ms processing time

                Interlocked.Increment(ref ProcessedCount);
            }
            catch (Exception e)
            {
                Interlocked.Increment(ref ErrorCount);
                Console.WriteLine($"❌ Error processing message: {e.Message}");
            }
        }
    }
}



