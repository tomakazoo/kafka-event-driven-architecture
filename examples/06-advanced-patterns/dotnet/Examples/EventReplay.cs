using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Confluent.Kafka;
using EventSourcing.Domain;
using EventSourcing.EventStore;
using EventSourcing.Repository;

class EventReplay
{
    static async Task Main()
    {
        Console.WriteLine("🔄 Event Replay Demo - Event Sourcing");
        Console.WriteLine("📡 Connecting to Kafka: localhost:9092");
        Console.WriteLine();

        var producerConfig = new ProducerConfig
        {
            BootstrapServers = "localhost:9092",
            RequestTimeoutMs = 5000,
            MessageTimeoutMs = 5000,
            SocketTimeoutMs = 5000
        };

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = "event-replay-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var producer = new ProducerBuilder<string, string>(producerConfig)
            .SetErrorHandler((p, e) => Console.WriteLine($"❌ Producer Error: {e.Reason}"))
            .Build();

        using var consumer = new ConsumerBuilder<string, string>(consumerConfig)
            .SetErrorHandler((c, e) => Console.WriteLine($"❌ Consumer Error: {e.Reason}"))
            .Build();

        var eventStore = new KafkaEventStore(producer, consumer, "order-events");
        var repository = new OrderRepository(eventStore);

        Console.WriteLine("✅ Connected successfully");
        Console.WriteLine();

        try
        {
            // Create an order with multiple changes
            Console.WriteLine("📝 Creating order with multiple changes...");
            var order = Order.Create("ORD-REPLAY-001", "CUST-999");
            
            Console.WriteLine($"   Step 1: Created - Status: {order.Status}, Version: {order.Version}");
            
            order.AddItem("PROD-X", 1, 25m);
            Console.WriteLine($"   Step 2: Added item - Items: {order.Items.Count}, Version: {order.Version}");
            
            order.AddItem("PROD-Y", 2, 15m);
            Console.WriteLine($"   Step 3: Added another item - Items: {order.Items.Count}, Version: {order.Version}");
            
            order.RemoveItem("PROD-X");
            Console.WriteLine($"   Step 4: Removed item - Items: {order.Items.Count}, Version: {order.Version}");
            
            order.SetShippingAddress(new Dictionary<string, string>
            {
                ["street"] = "789 Replay Ave",
                ["city"] = "Boston",
                ["state"] = "MA"
            });
            Console.WriteLine($"   Step 5: Set address - Has Address: {order.ShippingAddress != null}, Version: {order.Version}");
            
            order.Submit();
            Console.WriteLine($"   Step 6: Submitted - Status: {order.Status}, Version: {order.Version}");
            
            await repository.SaveAsync(order);
            Console.WriteLine();

            // Now replay events to rebuild state
            Console.WriteLine("🔄 Replaying events to rebuild order state...");
            Console.WriteLine();

            var events = await eventStore.GetEventsAsync("ORD-REPLAY-001");
            
            Console.WriteLine($"   Found {events.Count} events to replay:");
            Console.WriteLine();

            // Create a new order and replay events
            var replayedOrder = Order.CreateEmpty("ORD-REPLAY-001");
            
            foreach (var evt in events)
            {
                Console.WriteLine($"   Replaying: {evt.EventType} (v{evt.Version})");
                replayedOrder.LoadFromHistory(new[] { evt });
                
                Console.WriteLine($"      → Status: {replayedOrder.Status}");
                Console.WriteLine($"      → Items: {replayedOrder.Items.Count}");
                Console.WriteLine($"      → Has Address: {replayedOrder.ShippingAddress != null}");
                Console.WriteLine();
            }

            Console.WriteLine("✅ Final State After Replay:");
            Console.WriteLine($"   Order ID: {replayedOrder.OrderId}");
            Console.WriteLine($"   Customer: {replayedOrder.CustomerId}");
            Console.WriteLine($"   Status: {replayedOrder.Status}");
            Console.WriteLine($"   Items: {replayedOrder.Items.Count}");
            foreach (var item in replayedOrder.Items)
            {
                Console.WriteLine($"      - {item.ProductId}: {item.Quantity}x ${item.Price}");
            }
            Console.WriteLine($"   Version: {replayedOrder.Version}");
            Console.WriteLine();

            Console.WriteLine("✅ Event Replay Demo Complete!");
            Console.WriteLine();
            Console.WriteLine("💡 Key Points:");
            Console.WriteLine("   • Order state is rebuilt by replaying events");
            Console.WriteLine("   • Events are applied in order (by version)");
            Console.WriteLine("   • Each event transforms the state");
            Console.WriteLine("   • Can rebuild state from scratch using only events");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }
}

