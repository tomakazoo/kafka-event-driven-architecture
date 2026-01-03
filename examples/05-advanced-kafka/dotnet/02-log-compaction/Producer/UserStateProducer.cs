using System;
using System.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using LogCompaction.Domain;

namespace LogCompaction.Producer
{
    /// <summary>
    /// Producer that sends user state updates with the same key
    /// Log compaction will keep only the latest state per key
    /// </summary>
    class UserStateProducer
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("📦 Log Compaction Producer");
            Console.WriteLine("💡 Sending multiple updates for same user key");
            Console.WriteLine("📡 Connecting to: localhost:9092");
            Console.WriteLine();

            // Create compacted topic
            await CreateCompactedTopic();

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

            var topic = "user-state-compacted";
            var userId = "user-123";

            Console.WriteLine($"✅ Producer created");
            Console.WriteLine($"📤 Producing to topic: {topic}");
            Console.WriteLine($"👤 User ID: {userId}");
            Console.WriteLine();

            try
            {
                // Send multiple updates for the same user
                var updates = new[]
                {
                    new { Name = "John Doe", Email = "john@example.com", Version = 1 },
                    new { Name = "John Doe", Email = "john.doe@example.com", Version = 2 },
                    new { Name = "John D. Doe", Email = "john.doe@example.com", Version = 3 },
                    new { Name = "John D. Doe", Email = "j.doe@example.com", Version = 4 },
                    new { Name = "John D. Doe", Email = "j.doe@example.com", Version = 5 }
                };

                foreach (var update in updates)
                {
                    var user = new User
                    {
                        UserId = userId,
                        Name = update.Name,
                        Email = update.Email,
                        UpdatedAt = DateTime.UtcNow,
                        Version = update.Version
                    };

                    Console.WriteLine($"📨 Sending update v{update.Version}:");
                    Console.WriteLine($"   Name: {update.Name}");
                    Console.WriteLine($"   Email: {update.Email}");

                    var report = await producer.ProduceAsync(topic,
                        new Message<string, string>
                        {
                            Key = userId, // Same key for all updates
                            Value = user.ToJson()
                        });

                    Console.WriteLine($"✅ Delivered: {report.TopicPartitionOffset}");
                    Console.WriteLine();
                    await Task.Delay(500);
                }

                producer.Flush(TimeSpan.FromSeconds(5));
                Console.WriteLine("✅ All updates sent");
                Console.WriteLine();
                Console.WriteLine("💡 After compaction, only the latest version (v5) will remain");
                Console.WriteLine("💡 Check Kafka UI to see compaction in action");
            }
            catch (Exception e)
            {
                Console.WriteLine($"❌ Error: {e.Message}");
            }
        }

        static async Task CreateCompactedTopic()
        {
            var adminConfig = new AdminClientConfig
            {
                BootstrapServers = "localhost:9092"
            };

            using var adminClient = new AdminClientBuilder(adminConfig).Build();

            var topicName = "user-state-compacted";

            try
            {
                var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(5));
                var topicExists = metadata.Topics.Any(t => t.Topic == topicName);

                if (!topicExists)
                {
                    var topicSpec = new TopicSpecification
                    {
                        Name = topicName,
                        NumPartitions = 1,
                        ReplicationFactor = 1,
                        Configs = new System.Collections.Generic.Dictionary<string, string>
                        {
                            { "cleanup.policy", "compact" }, // Enable log compaction
                            { "min.cleanable.dirty.ratio", "0.5" }, // Compact when 50% dirty
                            { "segment.ms", "10000" } // Create new segment every 10 seconds
                        }
                    };

                    await adminClient.CreateTopicsAsync(new[] { topicSpec });
                    Console.WriteLine($"✅ Created compacted topic: {topicName}");
                    Console.WriteLine($"   cleanup.policy=compact");
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine($"ℹ️  Topic already exists: {topicName}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"⚠️  Could not create topic: {e.Message}");
                Console.WriteLine("💡 Topic will be auto-created (may not have compaction enabled)");
            }
        }
    }
}

