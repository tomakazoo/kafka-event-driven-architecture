using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using System.Text.Json;

namespace DeliverySemantics.AtLeastOnce
{
    /// <summary>
    /// At-Least-Once Producer: Default Kafka behavior
    /// Messages are guaranteed to be written, but may be duplicated
    /// Use case: Most common pattern, requires idempotent consumers
    /// </summary>
    class Producer
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("✅ At-Least-Once Producer (Default)");
            Console.WriteLine("⚠️  Messages guaranteed to be written, but may duplicate");
            Console.WriteLine("📡 Connecting to: localhost:9092");
            Console.WriteLine();

            var config = new ProducerConfig
            {
                BootstrapServers = "localhost:9092",
                Acks = Acks.All,         // Wait for all replicas
                Retries = 3,             // Retry on failure
                MaxInFlight = 5,         // Allow up to 5 in-flight requests
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

            Console.WriteLine("✅ Producer created (Acks=All, Retries=3)");
            var topic = "at-least-once-topic";
            Console.WriteLine($"📤 Producing to topic: {topic}");
            Console.WriteLine();

            try
            {
                for (int i = 0; i < 10; i++)
                {
                    var message = new
                    {
                        id = i,
                        event_id = Guid.NewGuid().ToString(), // Unique event ID for deduplication
                        timestamp = DateTime.UtcNow,
                        value = $"Message {i} - At least once"
                    };

                    Console.WriteLine($"📨 Sending message {i} (event_id: {message.event_id})...");

                    var report = await producer.ProduceAsync(topic,
                        new Message<string, string>
                        {
                            Key = $"key-{i}",
                            Value = JsonSerializer.Serialize(message)
                        });

                    Console.WriteLine($"✅ Delivered to {report.TopicPartitionOffset}");
                    await Task.Delay(200);
                }

                producer.Flush(TimeSpan.FromSeconds(5));
                Console.WriteLine();
                Console.WriteLine("✅ All messages delivered (may be duplicated on retry)");
            }
            catch (Exception e)
            {
                Console.WriteLine($"❌ Error: {e.Message}");
            }
        }
    }
}


