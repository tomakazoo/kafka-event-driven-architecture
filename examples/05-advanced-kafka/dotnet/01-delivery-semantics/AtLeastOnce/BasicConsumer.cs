using System;
using System.Threading;
using Confluent.Kafka;
using System.Text.Json;
using System.Collections.Generic;

namespace DeliverySemantics.AtLeastOnce
{
    /// <summary>
    /// Basic Consumer: Shows potential duplicates
    /// If processing succeeds but commit fails, message will be reprocessed
    /// </summary>
    class BasicConsumer
    {
        static void Main(string[] args)
        {
            Console.WriteLine("⚠️  Basic Consumer (Shows Duplicates)");
            Console.WriteLine("📡 Connecting to: localhost:9092");
            Console.WriteLine();

            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "at-least-once-consumer-group",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false // Manual commit to show the problem
            };

            using var consumer = new ConsumerBuilder<string, string>(config).Build();
            consumer.Subscribe("at-least-once-topic");

            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            Console.WriteLine("✅ Consumer subscribed, waiting for messages...");
            Console.WriteLine("💡 This consumer may process duplicates!");
            Console.WriteLine();

            var processedCount = 0;
            var duplicateCount = 0;
            var seenEventIds = new HashSet<string>();

            try
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    var result = consumer.Consume(cts.Token);
                    var message = JsonSerializer.Deserialize<Dictionary<string, object>>(result.Message.Value);
                    var eventId = message["event_id"].ToString();

                    // Simulate processing
                    Console.WriteLine($"📥 Received: Message {message["id"]} (event_id: {eventId})");

                    // Check for duplicate
                    if (seenEventIds.Contains(eventId))
                    {
                        duplicateCount++;
                        Console.WriteLine($"⚠️  DUPLICATE DETECTED! Event ID: {eventId}");
                    }
                    else
                    {
                        seenEventIds.Add(eventId);
                        processedCount++;
                        Console.WriteLine($"✅ Processing message {message["id"]}...");
                    }

                    // Simulate processing that might fail
                    Thread.Sleep(100);

                    // Commit offset (if this fails, message will be reprocessed)
                    try
                    {
                        consumer.Commit(result);
                        Console.WriteLine($"✅ Committed offset: {result.TopicPartitionOffset}");
                    }
                    catch (KafkaException e)
                    {
                        Console.WriteLine($"❌ Commit failed: {e.Message}");
                        Console.WriteLine("⚠️  Message will be reprocessed on restart!");
                    }

                    Console.WriteLine();
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine();
                Console.WriteLine("🛑 Consumer stopped");
                Console.WriteLine($"📊 Processed: {processedCount}, Duplicates: {duplicateCount}");
            }
            finally
            {
                consumer.Close();
            }
        }
    }
}

