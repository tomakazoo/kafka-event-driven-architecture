using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Confluent.Kafka;
using EventSourcing.Domain;
using EventSourcing.EventStore;
using EventSourcing.Repository;

class TimeTravel
{
    static async Task Main()
    {
        Console.WriteLine("⏰ Time Travel Demo - Event Sourcing");
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
            GroupId = "time-travel-group",
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
            // First, create an order with some history
            Console.WriteLine("📝 Creating order with history...");
            var order = Order.Create("ORD-TIME-001", "CUST-789");
            Console.WriteLine($"   Step 1: Created - Status: {order.Status}, Version: {order.Version}");
            order.AddItem("PROD-A", 1, 100m);
            Console.WriteLine($"   Step 2: Added PROD-A - Items: {order.Items.Count}, Version: {order.Version}");
            await repository.SaveAsync(order);
            
            await Task.Delay(1000); // Wait 1 second
            var timestamp1 = DateTime.UtcNow;
            
            order.AddItem("PROD-B", 2, 50m);
            Console.WriteLine($"   Step 3: Added PROD-B - Items: {order.Items.Count}, Version: {order.Version}");
            await repository.SaveAsync(order);
            
            await Task.Delay(1000); // Wait 1 second
            var timestamp2 = DateTime.UtcNow;
            
            order.SetShippingAddress(new Dictionary<string, string>
            {
                ["street"] = "456 Time St",
                ["city"] = "Boston",
                ["state"] = "MA"
            });
            Console.WriteLine($"   Step 4: Set address - Has Address: {order.ShippingAddress != null}, Version: {order.Version}");
            await repository.SaveAsync(order);
            
            await Task.Delay(1000); // Wait 1 second
            var timestamp3 = DateTime.UtcNow;
            
            order.Submit();
            Console.WriteLine($"   Step 5: Submitted - Status: {order.Status}, Version: {order.Version}");
            await repository.SaveAsync(order);
            
            await Task.Delay(1000); // Wait 1 second
            var timestamp4 = DateTime.UtcNow;
            
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
            
            var finalTimestamp = DateTime.UtcNow;

            Console.WriteLine($"   Order created with {order.Version} events");
            Console.WriteLine();

            // Now, travel back in time!
            Console.WriteLine("⏰ Time Travel - Viewing order at different points in time:");
            Console.WriteLine();

            // Point 1: Just created
            Console.WriteLine($"📅 Point 1: {timestamp1:HH:mm:ss} (just created)");
            var orderAtTime1 = await repository.GetOrderAtTimestampAsync("ORD-TIME-001", timestamp1);
            Console.WriteLine($"   Status: {orderAtTime1.Status}");
            Console.WriteLine($"   Items: {orderAtTime1.Items.Count}");
            Console.WriteLine($"   Version: {orderAtTime1.Version}");
            Console.WriteLine();

            // Point 2: After adding second item
            Console.WriteLine($"📅 Point 2: {timestamp2:HH:mm:ss} (after adding PROD-B)");
            var orderAtTime2 = await repository.GetOrderAtTimestampAsync("ORD-TIME-001", timestamp2);
            Console.WriteLine($"   Status: {orderAtTime2.Status}");
            Console.WriteLine($"   Items: {orderAtTime2.Items.Count}");
            Console.WriteLine($"   Version: {orderAtTime2.Version}");
            Console.WriteLine();

            // Point 3: After setting address
            Console.WriteLine($"📅 Point 3: {timestamp3:HH:mm:ss} (after setting address)");
            var orderAtTime3 = await repository.GetOrderAtTimestampAsync("ORD-TIME-001", timestamp3);
            Console.WriteLine($"   Status: {orderAtTime3.Status}");
            Console.WriteLine($"   Items: {orderAtTime3.Items.Count}");
            Console.WriteLine($"   Has Address: {orderAtTime3.ShippingAddress != null}");
            Console.WriteLine($"   Version: {orderAtTime3.Version}");
            Console.WriteLine();

            // Point 4: After payment received
            Console.WriteLine($"📅 Point 4: {timestamp4:HH:mm:ss} (after payment received)");
            var orderAtTime4 = await repository.GetOrderAtTimestampAsync("ORD-TIME-001", timestamp4);
            Console.WriteLine($"   Status: {orderAtTime4.Status}");
            Console.WriteLine($"   Items: {orderAtTime4.Items.Count}");
            Console.WriteLine($"   Version: {orderAtTime4.Version}");
            Console.WriteLine();

            // Current state
            Console.WriteLine($"📅 Current: {finalTimestamp:HH:mm:ss} (all events applied)");
            var currentOrder = await repository.GetAsync("ORD-TIME-001");
            Console.WriteLine($"   Status: {currentOrder.Status}");
            Console.WriteLine($"   Items: {currentOrder.Items.Count}");
            Console.WriteLine($"   Version: {currentOrder.Version}");
            Console.WriteLine();

            Console.WriteLine("✅ Time Travel Demo Complete!");
            Console.WriteLine();
            Console.WriteLine("💡 Key Points:");
            Console.WriteLine("   • Can view order state at any point in time");
            Console.WriteLine("   • Events are immutable - perfect audit trail");
            Console.WriteLine("   • Rebuild state by replaying events up to timestamp");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }
}

