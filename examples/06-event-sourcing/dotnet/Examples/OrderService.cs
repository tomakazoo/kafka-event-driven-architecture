using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;
using EventSourcing.Domain;
using EventSourcing.EventStore;
using EventSourcing.Repository;

class OrderService
{
    static async Task Main()
    {
        Console.WriteLine("🛒 Starting Event Sourcing Order Service...");
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
            GroupId = "event-store-group",
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
            // Create an order
            Console.WriteLine("📝 Creating Order ORD-001...");
            var order = Order.Create("ORD-001", "CUST-456");
            Console.WriteLine($"   Status: {order.Status}");
            Console.WriteLine($"   Version: {order.Version}");
            Console.WriteLine();

            // Add items
            Console.WriteLine("➕ Adding items...");
            order.AddItem("PROD-001", 2, 29.99m);
            Console.WriteLine($"   Added: PROD-001 (2x $29.99)");
            order.AddItem("PROD-002", 1, 49.99m);
            Console.WriteLine($"   Added: PROD-002 (1x $49.99)");
            Console.WriteLine($"   Total Items: {order.Items.Count}");
            Console.WriteLine($"   Version: {order.Version}");
            Console.WriteLine();

            // Set shipping address
            Console.WriteLine("📍 Setting shipping address...");
            order.SetShippingAddress(new Dictionary<string, string>
            {
                ["street"] = "123 Main St",
                ["city"] = "Boston",
                ["state"] = "MA",
                ["zip"] = "02101"
            });
            Console.WriteLine($"   Address set: {order.ShippingAddress["street"]}, {order.ShippingAddress["city"]}");
            Console.WriteLine($"   Version: {order.Version}");
            Console.WriteLine();

            // Submit order
            Console.WriteLine("✅ Submitting order...");
            order.Submit();
            Console.WriteLine($"   Status: {order.Status}");
            Console.WriteLine($"   Version: {order.Version}");
            Console.WriteLine();

            // Record payment
            Console.WriteLine("💳 Recording payment...");
            order.RecordPayment("PAY-789", 109.97m);
            Console.WriteLine($"   Status: {order.Status}");
            Console.WriteLine($"   Version: {order.Version}");
            Console.WriteLine();

            // Save to event store
            var uncommittedCount = order.UncommittedEvents.Count;
            Console.WriteLine("💾 Saving to event store...");
            await repository.SaveAsync(order);
            Console.WriteLine($"   ✅ Saved {uncommittedCount} uncommitted events");
            Console.WriteLine($"   Total events: {order.Version}");
            Console.WriteLine();

            // Load order from event store (rebuild from events)
            Console.WriteLine("🔄 Loading order from event store...");
            var loadedOrder = await repository.GetAsync("ORD-001");
            Console.WriteLine($"   Order ID: {loadedOrder.OrderId}");
            Console.WriteLine($"   Customer: {loadedOrder.CustomerId}");
            Console.WriteLine($"   Status: {loadedOrder.Status}");
            Console.WriteLine($"   Items: {loadedOrder.Items.Count}");
            Console.WriteLine($"   Version: {loadedOrder.Version}");
            Console.WriteLine();

            // Show all events
            Console.WriteLine("📋 Event History:");
            var events = await eventStore.GetEventsAsync("ORD-001");
            foreach (var evt in events)
            {
                Console.WriteLine($"   [{evt.Version}] {evt.EventType} @ {evt.Timestamp:HH:mm:ss}");
            }
            Console.WriteLine();

            Console.WriteLine("✅ Event Sourcing Demo Complete!");
            Console.WriteLine();
            Console.WriteLine("💡 Key Points:");
            Console.WriteLine("   • Order state is rebuilt from events");
            Console.WriteLine("   • Events are stored in Kafka");
            Console.WriteLine("   • Version tracking prevents concurrency issues");
            Console.WriteLine("   • Full audit trail of all changes");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }
}

