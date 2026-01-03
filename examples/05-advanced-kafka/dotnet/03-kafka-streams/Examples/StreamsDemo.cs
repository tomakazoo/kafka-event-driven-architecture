using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using KafkaStreams.Domain;
using KafkaStreams.Streams;

namespace KafkaStreams.Examples
{
    /// <summary>
    /// Demo showing Kafka Streams concepts:
    /// - Real-time stream processing
    /// - Filtering
    /// - Aggregation
    /// - Stateful operations
    /// </summary>
    class StreamsDemo
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine("   Kafka Streams Simulation Demo");
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine();

            // Create topics
            await CreateTopicsIfNeeded();

            // Setup consumer for source stream
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "streams-processor-group",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true
            };

            var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
            consumer.Subscribe("orders-stream");

            // Setup producer for sink streams
            var producerConfig = new ProducerConfig
            {
                BootstrapServers = "localhost:9092",
                Acks = Acks.All
            };

            var producer = new ProducerBuilder<string, string>(producerConfig).Build();

            // Create processing topology
            var topology = new OrderProcessingTopology(consumer, producer);

            // Start processing in background
            var cts = new CancellationTokenSource();
            var processingTask = Task.Run(() => topology.ProcessStreamAsync(cts.Token));

            Console.WriteLine("✅ Stream processing started");
            Console.WriteLine("💡 Waiting for orders to process...");
            Console.WriteLine("💡 Press Ctrl+C to stop and see statistics");
            Console.WriteLine();

            // Wait for user to stop
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            try
            {
                await processingTask;
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine();
                Console.WriteLine("🛑 Stopping stream processing...");
            }
            finally
            {
                topology.PrintStats();
                consumer.Close();
                producer.Flush(TimeSpan.FromSeconds(5));
                producer.Dispose();
            }
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
                "orders-stream",
                "customer-stats",
                "product-stats"
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
                }
            }
        }
    }
}

