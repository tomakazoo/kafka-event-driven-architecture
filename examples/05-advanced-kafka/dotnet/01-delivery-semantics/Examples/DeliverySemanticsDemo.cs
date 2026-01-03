using System;
using System.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Admin;

namespace DeliverySemantics.Examples
{
    /// <summary>
    /// Comprehensive demo showing all three delivery semantics
    /// </summary>
    class DeliverySemanticsDemo
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine("   Kafka Delivery Semantics Demo");
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine();

            // Create topics if they don't exist
            await CreateTopicsIfNeeded();

            Console.WriteLine("📚 This demo shows three delivery semantics:");
            Console.WriteLine();
            Console.WriteLine("1. 🔥 At-Most-Once: Fast but unsafe (messages may be lost)");
            Console.WriteLine("2. ✅ At-Least-Once: Safe but may duplicate (default)");
            Console.WriteLine("3. 🎯 Exactly-Once: Safe and no duplicates (transactions)");
            Console.WriteLine();
            Console.WriteLine("💡 Run each producer/consumer pair separately to see the differences");
            Console.WriteLine();
            Console.WriteLine("═══════════════════════════════════════════════════════");
        }

        static async Task CreateTopicsIfNeeded()
        {
            var adminConfig = new AdminClientConfig
            {
                BootstrapServers = "localhost:9092"
            };

            using var adminClient = new AdminClientBuilder(adminConfig).Build();

            var topics = new[]
            {
                "at-most-once-topic",
                "at-least-once-topic",
                "exactly-once-topic"
            };

            foreach (var topic in topics)
            {
                try
                {
                    var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(5));
                    var topicExists = metadata.Topics.Any(t => t.Topic == topic);

                    if (!topicExists)
                    {
                        var topicSpec = new TopicSpecification
                        {
                            Name = topic,
                            NumPartitions = 1,
                            ReplicationFactor = 1
                        };

                        await adminClient.CreateTopicsAsync(new[] { topicSpec });
                        Console.WriteLine($"✅ Created topic: {topic}");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"⚠️  Could not create topic {topic}: {e.Message}");
                    Console.WriteLine("💡 Topics will be auto-created by producers");
                }
            }
        }
    }
}

