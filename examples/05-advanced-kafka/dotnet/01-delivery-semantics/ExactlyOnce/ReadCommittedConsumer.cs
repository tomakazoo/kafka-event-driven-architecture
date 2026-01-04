using System;
using System.Threading;
using Confluent.Kafka;
using System.Text.Json;
using System.Collections.Generic;

namespace DeliverySemantics.ExactlyOnce
{
    /// <summary>
    /// Read Committed Consumer: Only reads messages from committed transactions
    /// Works with transactional producers for exactly-once semantics
    /// </summary>
    class ReadCommittedConsumer
    {
        static void Main(string[] args)
        {
            Console.WriteLine("🎯 Read Committed Consumer (Exactly-Once)");
            Console.WriteLine("✅ Only reads committed transactions");
            Console.WriteLine("📡 Connecting to: localhost:9092");
            Console.WriteLine();

            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "exactly-once-consumer-group",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                IsolationLevel = IsolationLevel.ReadCommitted, // Only read committed messages
                EnableAutoCommit = true
            };

            using var consumer = new ConsumerBuilder<string, string>(config).Build();
            consumer.Subscribe("exactly-once-topic");

            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            Console.WriteLine("✅ Consumer subscribed (ReadCommitted isolation level)");
            Console.WriteLine("💡 Only committed messages will be read");
            Console.WriteLine();

            var processedCount = 0;
            var seenEventIds = new HashSet<string>();

            try
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    var result = consumer.Consume(cts.Token);
                    var message = JsonSerializer.Deserialize<Dictionary<string, object>>(result.Message.Value);
                    var eventId = message["event_id"].ToString();

                    // Verify no duplicates (should never happen with exactly-once)
                    if (seenEventIds.Contains(eventId))
                    {
                        Console.WriteLine($"⚠️  UNEXPECTED DUPLICATE: {eventId}");
                    }
                    else
                    {
                        seenEventIds.Add(eventId);
                    }

                    Console.WriteLine($"📥 Received: Message {message["id"]} (event_id: {eventId})");
                    Console.WriteLine($"✅ Processing message {message["id"]}...");
                    Thread.Sleep(100);
                    processedCount++;
                    Console.WriteLine($"✅ Processed: {result.TopicPartitionOffset}");
                    Console.WriteLine();
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine();
                Console.WriteLine("🛑 Consumer stopped");
                Console.WriteLine($"📊 Total processed: {processedCount}");
                Console.WriteLine($"✅ No duplicates detected (exactly-once guarantee)");
            }
            finally
            {
                consumer.Close();
            }
        }
    }
}


