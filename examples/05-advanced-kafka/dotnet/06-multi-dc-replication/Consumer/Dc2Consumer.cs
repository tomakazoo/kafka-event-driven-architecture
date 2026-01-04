using System;
using System.Threading;
using Confluent.Kafka;
using System.Text.Json;

namespace MultiDCReplication.Consumer
{
    /// <summary>
    /// Consumer simulating DC2 (US-West)
    /// In production, this would consume from DC2 Kafka cluster
    /// which receives replicated messages from DC1 via MirrorMaker
    /// </summary>
    class Dc2Consumer
    {
        static void Main(string[] args)
        {
            Console.WriteLine("🌍 DC2 Consumer (US-West)");
            Console.WriteLine("💡 Simulating consumption from DC2");
            Console.WriteLine("📡 Connecting to: localhost:9092 (DC2)");
            Console.WriteLine();

            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092", // DC2 broker (in production)
                GroupId = "dc2-consumer-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<string, string>(config).Build();
            
            // In production, MirrorMaker replicates topics with prefix
            // e.g., "orders-dc1" from DC1 becomes "orders-dc1" in DC2
            consumer.Subscribe("orders-dc1");

            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            Console.WriteLine("✅ Consumer subscribed");
            Console.WriteLine("💡 Waiting for replicated messages from DC1...");
            Console.WriteLine();

            var receivedCount = 0;

            try
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    var result = consumer.Consume(cts.Token);
                    var message = JsonSerializer.Deserialize<JsonElement>(result.Message.Value);

                    receivedCount++;
                    Console.WriteLine($"📥 Received replicated message #{receivedCount}:");
                    Console.WriteLine($"   Order ID: {message.GetProperty("order_id")}");
                    Console.WriteLine($"   Customer: {message.GetProperty("customer_id")}");
                    Console.WriteLine($"   Amount: ${message.GetProperty("amount")}");
                    Console.WriteLine($"   Source DC: {message.GetProperty("dc")}");
                    Console.WriteLine($"   Offset: {result.Offset}");
                    Console.WriteLine($"💡 This message was replicated from DC1 to DC2");
                    Console.WriteLine();
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine();
                Console.WriteLine("🛑 Consumer stopped");
                Console.WriteLine($"📊 Total replicated messages received: {receivedCount}");
            }
            finally
            {
                consumer.Close();
            }
        }
    }
}


