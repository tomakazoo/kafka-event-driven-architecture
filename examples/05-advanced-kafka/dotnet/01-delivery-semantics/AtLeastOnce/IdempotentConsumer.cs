using System;
using System.Threading;
using Confluent.Kafka;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;

namespace DeliverySemantics.AtLeastOnce
{
    /// <summary>
    /// Idempotent Consumer: Uses in-memory deduplication
    /// In production, use Redis or database for distributed deduplication
    /// </summary>
    class IdempotentConsumer
    {
        // In-memory deduplication (use Redis in production)
        private static readonly HashSet<string> ProcessedEventIds = new HashSet<string>();

        static void Main(string[] args)
        {
            Console.WriteLine("🛡️  Idempotent Consumer (Deduplication)");
            Console.WriteLine("📡 Connecting to: localhost:9092");
            Console.WriteLine();

            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "idempotent-consumer-group",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };

            using var consumer = new ConsumerBuilder<string, string>(config).Build();
            consumer.Subscribe("at-least-once-topic");

            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            Console.WriteLine("✅ Consumer subscribed with deduplication");
            Console.WriteLine("💡 Duplicate messages will be skipped");
            Console.WriteLine();

            var processedCount = 0;
            var skippedCount = 0;

            try
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    var result = consumer.Consume(cts.Token);
                    var message = JsonSerializer.Deserialize<Dictionary<string, object>>(result.Message.Value);
                    var eventId = message["event_id"].ToString();

                    Console.WriteLine($"📥 Received: Message {message["id"]} (event_id: {eventId})");

                    // Check if already processed
                    lock (ProcessedEventIds)
                    {
                        if (ProcessedEventIds.Contains(eventId))
                        {
                            skippedCount++;
                            Console.WriteLine($"⏭️  SKIPPING DUPLICATE: Event ID {eventId} already processed");
                            consumer.Commit(result); // Commit even duplicates to advance offset
                            Console.WriteLine();
                            continue;
                        }

                        // Mark as processed
                        ProcessedEventIds.Add(eventId);
                    }

                    // Process the message
                    Console.WriteLine($"✅ Processing message {message["id"]}...");
                    Thread.Sleep(100);
                    processedCount++;

                    // Commit offset
                    consumer.Commit(result);
                    Console.WriteLine($"✅ Committed offset: {result.TopicPartitionOffset}");
                    Console.WriteLine();
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine();
                Console.WriteLine("🛑 Consumer stopped");
                Console.WriteLine($"📊 Processed: {processedCount}, Skipped (duplicates): {skippedCount}");
            }
            finally
            {
                consumer.Close();
            }
        }
    }
}


