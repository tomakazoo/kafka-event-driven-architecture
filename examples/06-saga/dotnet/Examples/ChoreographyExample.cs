using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using SagaPattern.Choreography;
using SagaPattern.Shared.Domain;

class ChoreographyExample
{
    static async Task Main()
    {
        Console.WriteLine("🎭 Saga Pattern - Choreography Example");
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
            GroupId = "saga-choreography-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var producer = new ProducerBuilder<string, string>(producerConfig)
            .SetErrorHandler((p, e) => Console.WriteLine($"❌ Producer Error: {e.Reason}"))
            .Build();

        using var inventoryConsumer = new ConsumerBuilder<string, string>(consumerConfig)
            .SetErrorHandler((c, e) => Console.WriteLine($"❌ Consumer Error: {e.Reason}"))
            .Build();

        using var paymentConsumer = new ConsumerBuilder<string, string>(new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = "payment-service-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        })
            .SetErrorHandler((c, e) => Console.WriteLine($"❌ Consumer Error: {e.Reason}"))
            .Build();

        var eventPublisher = new KafkaEventPublisher(producer);
        var orderRepository = new InMemoryOrderRepository();
        var orderService = new OrderService(orderRepository, eventPublisher);
        var inventoryService = new InventoryService(inventoryConsumer, eventPublisher);
        var paymentService = new PaymentService(paymentConsumer, eventPublisher);

        Console.WriteLine("✅ Connected successfully");
        Console.WriteLine();

        // Start consumers in background
        var cts = new CancellationTokenSource();
        var inventoryTask = Task.Run(() => inventoryService.ConsumeEventsAsync(cts.Token));
        var paymentTask = Task.Run(() => paymentService.ConsumeEventsAsync(cts.Token));

        await Task.Delay(2000); // Give consumers time to subscribe

        try
        {
            // Create and place an order
            Console.WriteLine("📝 Creating order...");
            var order = new Order(
                "ORD-001",
                "CUST-456",
                new List<OrderItem>
                {
                    new OrderItem("PROD-001", "Laptop", 2, 999.99m),
                    new OrderItem("PROD-002", "Mouse", 1, 29.99m)
                },
                2029.97m,
                new ShippingAddress("123 Main St", "Boston", "MA", "02101")
            );

            await orderService.PlaceOrderAsync(order);
            Console.WriteLine();

            Console.WriteLine("⏳ Waiting for saga to complete...");
            Console.WriteLine("   (Watch how services react to events)");
            Console.WriteLine();

            // Wait for saga to complete
            await Task.Delay(5000);

            Console.WriteLine();
            Console.WriteLine("✅ Choreography Saga Demo Complete!");
            Console.WriteLine();
            Console.WriteLine("💡 Key Points:");
            Console.WriteLine("   • Services react to events autonomously");
            Console.WriteLine("   • No central orchestrator");
            Console.WriteLine("   • Compensating transactions handle failures");
            Console.WriteLine("   • Each service knows what to do based on event type");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
        }
        finally
        {
            cts.Cancel();
            await Task.WhenAll(inventoryTask, paymentTask);
        }
    }
}


