using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using System.Text.Json;

namespace DeliverySemantics.AtMostOnce
{
    /// <summary>
    /// At-Most-Once Producer: Fire and forget
    /// Messages may be lost, but we don't wait for acknowledgment
    /// Use case: Metrics, logs where losing some data is acceptable
    /// </summary>
    class Producer
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("🔥 At-Most-Once Producer (Fire & Forget)");
            Console.WriteLine("⚠️  WARNING: Messages may be lost!");
            Console.WriteLine("📡 Connecting to: localhost:9092");
            Console.WriteLine();

            var config = new ProducerConfig
            {
                BootstrapServers = "localhost:9092",
                Acks = Acks.None,        // Don't wait for acknowledgment
                Retries = 0,             // Don't retry on failure
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

            Console.WriteLine("✅ Producer created (Acks=None, Retries=0)");
            var topic = "at-most-once-topic";
            Console.WriteLine($"📤 Producing to topic: {topic}");
            Console.WriteLine();

            try
            {
                for (int i = 0; i < 10; i++)
                {
                    var message = new
                    {
                        id = i,
                        timestamp = DateTime.UtcNow,
                        value = $"Message {i} - Fire and forget"
                    };

                    // Fire and forget - don't wait for result
                    producer.Produce(topic,
                        new Message<string, string>
                        {
                            Key = $"key-{i}",
                            Value = JsonSerializer.Serialize(message)
                        },
                        (deliveryReport) =>
                        {
                            if (deliveryReport.Error.IsError)
                            {
                                Console.WriteLine($"❌ Failed: {deliveryReport.Error.Reason}");
                            }
                            else
                            {
                                Console.WriteLine($"⚡ Sent (no ACK): {deliveryReport.TopicPartitionOffset}");
                            }
                        });

                    Console.WriteLine($"📨 Queued message {i} (not waiting for ACK)");
                    await Task.Delay(100); // Small delay to show async behavior
                }

                // Give some time for messages to be sent
                Console.WriteLine();
                Console.WriteLine("⏳ Waiting for messages to be sent (non-blocking)...");
                producer.Flush(TimeSpan.FromSeconds(3));
                Console.WriteLine();
                Console.WriteLine("✅ Producer finished (messages may or may not have been delivered)");
                Console.WriteLine("💡 Check Kafka UI to see how many messages actually arrived");
            }
            catch (Exception e)
            {
                Console.WriteLine($"❌ Error: {e.Message}");
            }
        }
    }
}


