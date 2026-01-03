using System;
using System.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Admin;

namespace MultiDCReplication.Examples
{
    /// <summary>
    /// Demo showing multi-datacenter replication concepts
    /// 
    /// NOTE: Full MirrorMaker requires multiple Kafka clusters.
    /// This demo shows the concepts and configuration needed.
    /// </summary>
    class ReplicationDemo
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine("   Multi-Datacenter Replication Demo");
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine();

            Console.WriteLine("📚 Multi-DC Replication Concepts:");
            Console.WriteLine();
            Console.WriteLine("1. **MirrorMaker 2**: Replicates topics between clusters");
            Console.WriteLine("2. **Active-Passive**: One DC active, others replicate");
            Console.WriteLine("3. **Active-Active**: Both DCs produce and consume");
            Console.WriteLine("4. **Topic Prefixing**: Prevents topic name conflicts");
            Console.WriteLine();

            Console.WriteLine("🌍 Architecture:");
            Console.WriteLine();
            Console.WriteLine("  DC1 (US-East)                    DC2 (US-West)");
            Console.WriteLine("  ┌─────────────┐                  ┌─────────────┐");
            Console.WriteLine("  │   Kafka     │                  │   Kafka     │");
            Console.WriteLine("  │   Cluster   │                  │   Cluster   │");
            Console.WriteLine("  └──────┬──────┘                  └──────┬──────┘");
            Console.WriteLine("         │                                  │");
            Console.WriteLine("         │        MirrorMaker 2            │");
            Console.WriteLine("         └──────────────┬───────────────────┘");
            Console.WriteLine("                        │");
            Console.WriteLine("                  Replication");
            Console.WriteLine();

            Console.WriteLine("💡 This demo simulates replication concepts");
            Console.WriteLine("💡 For full replication, configure MirrorMaker 2");
            Console.WriteLine();
            Console.WriteLine("📚 See README.md for MirrorMaker 2 configuration");
            Console.WriteLine();

            // Create topic for demo
            await CreateTopicIfNeeded();
        }

        static async Task CreateTopicIfNeeded()
        {
            var adminConfig = new AdminClientConfig
            {
                BootstrapServers = "localhost:9092"
            };

            using var adminClient = new AdminClientBuilder(adminConfig).Build();

            var topic = "orders-dc1";

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
                Console.WriteLine($"⚠️  Could not create topic: {e.Message}");
            }
        }
    }
}

