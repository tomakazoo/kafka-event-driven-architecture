using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using System.Text.Json;

namespace PerformanceTuning.Producer
{
    /// <summary>
    /// Performance-tuned producer demonstrating:
    /// - Batching for throughput
    /// - Compression for efficiency
    /// - Optimized configuration
    /// </summary>
    class TunedProducer
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("⚡ Performance-Tuned Producer");
            Console.WriteLine("📡 Connecting to: localhost:9092");
            Console.WriteLine();

            var config = new ProducerConfig
            {
                BootstrapServers = "localhost:9092",
                
                // Batching for throughput
                BatchSize = 32768,              // 32KB batches
                LingerMs = 10,                  // Wait up to 10ms to fill batch
                
                // Compression
                CompressionType = CompressionType.Snappy,  // Fast compression
                // Options: CompressionType.Gzip, Snappy, Lz4, Zstd
                
                // Buffer size
                MessageMaxBytes = 1000000,      // 1MB max message size
                
                // Network
                MaxInFlight = 5,                // Allow 5 in-flight requests
                
                // Reliability
                Acks = Acks.All,
                Retries = 3,
                
                // Timeouts
                RequestTimeoutMs = 5000,
                MessageTimeoutMs = 5000,
                SocketTimeoutMs = 5000
            };

            using var producer = new ProducerBuilder<string, string>(config)
                .SetErrorHandler((p, e) =>
                {
                    Console.WriteLine($"❌ Producer Error: {e.Reason}");
                })
                .Build();

            var topic = "performance-topic";
            var messageCount = 1000;

            Console.WriteLine("✅ Producer created with performance tuning:");
            Console.WriteLine($"   Batch Size: {config.BatchSize} bytes");
            Console.WriteLine($"   Linger: {config.LingerMs} ms");
            Console.WriteLine($"   Compression: {config.CompressionType}");
            Console.WriteLine($"   Max In-Flight: {config.MaxInFlight}");
            Console.WriteLine();
            Console.WriteLine($"📤 Producing {messageCount} messages to: {topic}");
            Console.WriteLine();

            var stopwatch = Stopwatch.StartNew();
            var deliveredCount = 0;
            var errorCount = 0;

            try
            {
                for (int i = 0; i < messageCount; i++)
                {
                    var message = new
                    {
                        id = i,
                        timestamp = DateTime.UtcNow,
                        data = $"Message {i} - Performance test with batching and compression"
                    };

                    producer.Produce(topic,
                        new Message<string, string>
                        {
                            Key = $"key-{i % 10}", // Reuse keys for better batching
                            Value = JsonSerializer.Serialize(message)
                        },
                        (deliveryReport) =>
                        {
                            if (deliveryReport.Error.IsError)
                            {
                                Interlocked.Increment(ref errorCount);
                            }
                            else
                            {
                                Interlocked.Increment(ref deliveredCount);
                            }
                        });

                    // Show progress every 100 messages
                    if ((i + 1) % 100 == 0)
                    {
                        Console.WriteLine($"📨 Queued {i + 1} messages...");
                    }
                }

                // Flush all pending messages
                Console.WriteLine();
                Console.WriteLine("🔄 Flushing producer...");
                producer.Flush(TimeSpan.FromSeconds(30));
                stopwatch.Stop();

                Console.WriteLine();
                Console.WriteLine("═══════════════════════════════════════════════════════");
                Console.WriteLine("📊 Performance Results");
                Console.WriteLine("═══════════════════════════════════════════════════════");
                Console.WriteLine($"Total messages: {messageCount}");
                Console.WriteLine($"Delivered: {deliveredCount}");
                Console.WriteLine($"Errors: {errorCount}");
                Console.WriteLine($"Time: {stopwatch.ElapsedMilliseconds} ms");
                Console.WriteLine($"Throughput: {deliveredCount * 1000.0 / stopwatch.ElapsedMilliseconds:F2} msg/s");
                Console.WriteLine();
                Console.WriteLine("💡 Batching and compression improve throughput!");
            }
            catch (Exception e)
            {
                Console.WriteLine($"❌ Error: {e.Message}");
            }
        }
    }
}

