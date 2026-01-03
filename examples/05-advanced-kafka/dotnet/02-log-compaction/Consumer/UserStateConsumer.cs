using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Confluent.Kafka;
using LogCompaction.Domain;

namespace LogCompaction.Consumer
{
    /// <summary>
    /// Consumer that reads from compacted topic
    /// After compaction, only latest state per key remains
    /// </summary>
    class UserStateConsumer
    {
        static void Main(string[] args)
        {
            Console.WriteLine("📦 Log Compaction Consumer");
            Console.WriteLine("💡 Reading from compacted topic");
            Console.WriteLine("📡 Connecting to: localhost:9092");
            Console.WriteLine();

            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "compaction-consumer-group",
                AutoOffsetReset = AutoOffsetReset.Earliest, // Read from beginning
                EnableAutoCommit = true
            };

            using var consumer = new ConsumerBuilder<string, string>(config).Build();
            consumer.Subscribe("user-state-compacted");

            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            Console.WriteLine("✅ Consumer subscribed");
            Console.WriteLine("💡 Reading all messages (including historical updates)");
            Console.WriteLine();

            var messages = new List<(string key, User user, long offset)>();

            try
            {
                var timeout = TimeSpan.FromSeconds(10);
                var startTime = DateTime.Now;

                while (DateTime.Now - startTime < timeout)
                {
                    var result = consumer.Consume(timeout);

                    if (result == null)
                        break;

                    var user = User.FromJson(result.Message.Value);
                    messages.Add((result.Message.Key, user, result.Offset));

                    Console.WriteLine($"📥 Received: Offset {result.Offset}");
                    Console.WriteLine($"   User: {user.UserId}");
                    Console.WriteLine($"   Version: {user.Version}");
                    Console.WriteLine($"   Name: {user.Name}");
                    Console.WriteLine($"   Email: {user.Email}");
                    Console.WriteLine();
                }

                Console.WriteLine("═══════════════════════════════════════════════════════");
                Console.WriteLine("📊 Summary");
                Console.WriteLine("═══════════════════════════════════════════════════════");
                Console.WriteLine($"Total messages received: {messages.Count}");

                if (messages.Count > 0)
                {
                    var latest = messages.OrderByDescending(m => m.offset).First();
                    Console.WriteLine();
                    Console.WriteLine("📌 Latest State (after compaction):");
                    Console.WriteLine($"   User ID: {latest.user.UserId}");
                    Console.WriteLine($"   Version: {latest.user.Version}");
                    Console.WriteLine($"   Name: {latest.user.Name}");
                    Console.WriteLine($"   Email: {latest.user.Email}");
                    Console.WriteLine($"   Offset: {latest.offset}");
                }

                Console.WriteLine();
                Console.WriteLine("💡 Note: After compaction runs, older versions are removed");
                Console.WriteLine("💡 Only the latest state per key remains in the log");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("🛑 Consumer stopped");
            }
            finally
            {
                consumer.Close();
            }
        }
    }
}

