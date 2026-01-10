using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using System.Text.Json;

namespace MultiDCReplication.Producer
{
    /// <summary>
    /// Producer simulating DC1 (US-East)
    /// In production, this would connect to DC1 Kafka cluster
    /// </summary>
    class Dc1Producer
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("🌍 DC1 Producer (US-East)");
            Console.WriteLine("💡 Simulating production in DC1");
            Console.WriteLine("📡 Connecting to: localhost:9092 (DC1)");
            Console.WriteLine();

            var config = new ProducerConfig
            {
                BootstrapServers = "localhost:9092", // DC1 broker
                Acks = Acks.All,
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

            var topic = "orders-dc1"; // DC1 topic

            Console.WriteLine($"✅ Producer created (DC1)");
            Console.WriteLine($"📤 Producing to topic: {topic}");
            Console.WriteLine();

            try
            {
                for (int i = 0; i < 10; i++)
                {
                    var message = new
                    {
                        order_id = $"order-dc1-{i}",
                        customer_id = $"customer-{i % 3}",
                        amount = 100.0 + i * 10,
                        dc = "us-east",
                        timestamp = DateTime.UtcNow
                    };

                    Console.WriteLine($"📨 Sending order {i} from DC1:");
                    Console.WriteLine($"   Order ID: {message.order_id}");
                    Console.WriteLine($"   Customer: {message.customer_id}");
                    Console.WriteLine($"   Amount: ${message.amount:F2}");

                    var report = await producer.ProduceAsync(topic,
                        new Message<string, string>
                        {
                            Key = message.order_id,
                            Value = JsonSerializer.Serialize(message)
                        });

                    Console.WriteLine($"✅ Delivered to DC1: {report.TopicPartitionOffset}");
                    Console.WriteLine($"💡 This message will be replicated to DC2");
                    Console.WriteLine();

                    await Task.Delay(500);
                }

                producer.Flush(TimeSpan.FromSeconds(5));
                Console.WriteLine("✅ All messages sent from DC1");
                Console.WriteLine("💡 In production, MirrorMaker would replicate these to DC2");
            }
            catch (Exception e)
            {
                Console.WriteLine($"❌ Error: {e.Message}");
            }
        }
    }
}



