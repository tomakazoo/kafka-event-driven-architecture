using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using KafkaStreams.Domain;

namespace KafkaStreams.Examples
{
    /// <summary>
    /// Producer that sends orders to the stream
    /// </summary>
    class OrderProducer
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("📤 Order Producer for Stream Processing");
            Console.WriteLine("📡 Connecting to: localhost:9092");
            Console.WriteLine();

            var config = new ProducerConfig
            {
                BootstrapServers = "localhost:9092",
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

            var topic = "orders-stream";

            Console.WriteLine($"✅ Producer created");
            Console.WriteLine($"📤 Producing to topic: {topic}");
            Console.WriteLine();

            var random = new Random();
            var customers = new[] { "customer-1", "customer-2", "customer-3" };
            var products = new[] { "product-A", "product-B", "product-C" };
            var statuses = new[] { "pending", "completed", "cancelled" };

            try
            {
                for (int i = 0; i < 20; i++)
                {
                    var order = new Order
                    {
                        OrderId = $"order-{i}",
                        CustomerId = customers[random.Next(customers.Length)],
                        ProductId = products[random.Next(products.Length)],
                        Amount = (decimal)(random.NextDouble() * 1000 + 10),
                        Quantity = random.Next(1, 5),
                        Status = statuses[random.Next(statuses.Length)],
                        Timestamp = DateTime.UtcNow
                    };

                    Console.WriteLine($"📨 Sending order {i}:");
                    Console.WriteLine($"   Customer: {order.CustomerId}");
                    Console.WriteLine($"   Product: {order.ProductId}");
                    Console.WriteLine($"   Amount: ${order.Amount:F2}");
                    Console.WriteLine($"   Status: {order.Status}");

                    var report = await producer.ProduceAsync(topic,
                        new Message<string, string>
                        {
                            Key = order.OrderId,
                            Value = order.ToJson()
                        });

                    Console.WriteLine($"✅ Delivered: {report.TopicPartitionOffset}");
                    Console.WriteLine();

                    await Task.Delay(500);
                }

                producer.Flush(TimeSpan.FromSeconds(5));
                Console.WriteLine("✅ All orders sent");
                Console.WriteLine("💡 Run StreamsDemo to process the stream");
            }
            catch (Exception e)
            {
                Console.WriteLine($"❌ Error: {e.Message}");
            }
        }
    }
}


