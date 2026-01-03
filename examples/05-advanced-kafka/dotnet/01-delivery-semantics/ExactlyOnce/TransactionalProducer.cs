using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using System.Text.Json;
using System.Linq;

namespace DeliverySemantics.ExactlyOnce
{
    /// <summary>
    /// Exactly-Once Producer: Uses idempotence and transactions
    /// Guarantees no duplicates, even across retries
    /// </summary>
    class TransactionalProducer
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("🎯 Exactly-Once Producer (Transactional)");
            Console.WriteLine("✅ Guaranteed: No duplicates, no loss");
            Console.WriteLine("📡 Connecting to: localhost:9092");
            Console.WriteLine();

            var config = new ProducerConfig
            {
                BootstrapServers = "localhost:9092",
                EnableIdempotence = true,    // Enable idempotent producer
                Acks = Acks.All,             // Required for idempotence
                MaxInFlight = 5,              // Required: must be <= 5
                Retries = int.MaxValue,       // Required: must be > 0
                TransactionalId = "exactly-once-producer-1", // Unique transactional ID
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

            var topic = "exactly-once-topic";

            try
            {
                // Initialize transactions
                Console.WriteLine("🔄 Initializing transactions...");
                producer.InitTransactions(TimeSpan.FromSeconds(10));
                Console.WriteLine("✅ Transactions initialized");
                Console.WriteLine();

                // Begin transaction
                Console.WriteLine("🔄 Beginning transaction...");
                producer.BeginTransaction();
                Console.WriteLine("✅ Transaction started");
                Console.WriteLine();

                Console.WriteLine($"📤 Producing messages to topic: {topic}");
                Console.WriteLine();

                for (int i = 0; i < 10; i++)
                {
                    var message = new
                    {
                        id = i,
                        event_id = Guid.NewGuid().ToString(),
                        timestamp = DateTime.UtcNow,
                        value = $"Message {i} - Exactly once"
                    };

                    Console.WriteLine($"📨 Sending message {i} (event_id: {message.event_id})...");

                    // Produce within transaction
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
                                Console.WriteLine($"✅ Queued: {deliveryReport.TopicPartitionOffset}");
                            }
                        });

                    await Task.Delay(100);
                }

                // Commit transaction (all or nothing)
                Console.WriteLine();
                Console.WriteLine("🔄 Committing transaction...");
                producer.CommitTransaction(TimeSpan.FromSeconds(10));
                Console.WriteLine("✅ Transaction committed - all messages delivered exactly once");
                Console.WriteLine();

                producer.Flush(TimeSpan.FromSeconds(5));
                Console.WriteLine("✅ Producer finished");
            }
            catch (KafkaException e)
            {
                Console.WriteLine($"❌ Kafka error: {e.Message}");
                try
                {
                    producer.AbortTransaction(TimeSpan.FromSeconds(10));
                    Console.WriteLine("🔄 Transaction aborted - no messages delivered");
                }
                catch (Exception abortEx)
                {
                    Console.WriteLine($"❌ Error aborting transaction: {abortEx.Message}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"❌ Error: {e.Message}");
            }
        }
    }
}

