using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Confluent.Kafka;
using SagaPattern.Choreography;
using SagaPattern.Orchestration;
using SagaPattern.Shared.Domain;

class OrchestrationExample
{
    static async Task Main()
    {
        Console.WriteLine("🎭 Saga Pattern - Orchestration Example");
        Console.WriteLine("📡 Connecting to Kafka: localhost:9092");
        Console.WriteLine();

        var producerConfig = new ProducerConfig
        {
            BootstrapServers = "localhost:9092",
            RequestTimeoutMs = 5000,
            MessageTimeoutMs = 5000,
            SocketTimeoutMs = 5000
        };

        using var producer = new ProducerBuilder<string, string>(producerConfig)
            .SetErrorHandler((p, e) => Console.WriteLine($"❌ Producer Error: {e.Reason}"))
            .Build();

        var eventPublisher = new KafkaEventPublisher(producer);
        var inventoryService = new MockInventoryService();
        var paymentService = new MockPaymentService();
        var shippingService = new MockShippingService();
        var orchestrator = new SagaOrchestrator(
            inventoryService,
            paymentService,
            shippingService,
            eventPublisher
        );

        Console.WriteLine("✅ Connected successfully");
        Console.WriteLine();

        try
        {
            // Create order request
            Console.WriteLine("📝 Creating order request...");
            var orderRequest = new OrderRequest(
                "ORD-SAGA-001",
                "CUST-789",
                new List<OrderItem>
                {
                    new OrderItem("PROD-001", "Laptop", 1, 999.99m),
                    new OrderItem("PROD-002", "Mouse", 2, 29.99m)
                },
                1059.97m,
                new ShippingAddress("456 Saga St", "Boston", "MA", "02101")
            );

            Console.WriteLine($"   Order ID: {orderRequest.OrderId}");
            Console.WriteLine($"   Customer: {orderRequest.CustomerId}");
            Console.WriteLine($"   Amount: ${orderRequest.Amount}");
            Console.WriteLine($"   Items: {orderRequest.Items.Count}");
            Console.WriteLine();

            // Execute saga
            var result = await orchestrator.StartOrderSagaAsync(orderRequest);

            Console.WriteLine();
            Console.WriteLine($"✅ Saga completed successfully!");
            Console.WriteLine($"   Tracking Number: {result.TrackingNumber}");
            Console.WriteLine();
            Console.WriteLine("💡 Key Points:");
            Console.WriteLine("   • Central orchestrator coordinates all steps");
            Console.WriteLine("   • Compensations stored and executed in reverse order");
            Console.WriteLine("   • If any step fails, compensations run automatically");
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine($"❌ Saga failed: {ex.Message}");
            Console.WriteLine("   (Compensations were executed)");
        }
    }
}




