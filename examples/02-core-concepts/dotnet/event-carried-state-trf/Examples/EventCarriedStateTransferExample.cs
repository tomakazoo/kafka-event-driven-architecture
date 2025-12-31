using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using EventCarriedStateTransfer.Domain;
using EventCarriedStateTransfer.Infrastructure;
using EventCarriedStateTransfer.Interfaces;
using EventCarriedStateTransfer.Services;
using System.Text.Json;

namespace EventCarriedStateTransfer.Examples;

class EventCarriedStateTransferExample
{
    static async Task Main()
    {
        Console.WriteLine("🚀 Event-Carried State Transfer Pattern Example");
        Console.WriteLine("📡 Connecting to Kafka: localhost:9092");
        Console.WriteLine();

        // Setup infrastructure
        IDatabase database = new InMemoryDatabase();
        IEventPublisher eventPublisher = new KafkaEventPublisher("localhost:9092");
        
        // Setup services
        var orderService = new OrderService(database, eventPublisher);
        var emailService = new EmailService();
        var inventoryService = new InventoryService();
        var analyticsService = new AnalyticsService();

        // Create Kafka topic first (consumers can't subscribe to non-existent topics)
        Console.WriteLine("📋 Creating topic 'orders' if it doesn't exist...");
        try
        {
            using var adminClient = new AdminClientBuilder(new AdminClientConfig
            {
                BootstrapServers = "localhost:9092"
            }).Build();

            try
            {
                await adminClient.CreateTopicsAsync(new[]
                {
                    new TopicSpecification
                    {
                        Name = "orders",
                        NumPartitions = 1,
                        ReplicationFactor = 1
                    }
                });
                Console.WriteLine("✅ Topic 'orders' created");
            }
            catch (CreateTopicsException ex) when (ex.Results[0].Error.Code == ErrorCode.TopicAlreadyExists)
            {
                Console.WriteLine("✅ Topic 'orders' already exists");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️  Could not create topic: {ex.Message}");
            Console.WriteLine("   Will try to publish first (auto-creates topic)...");
        }

        Console.WriteLine();

        // Setup Kafka consumer to demonstrate event consumption
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = "event-carried-state-transfer-consumers",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();

        // Create a sample order
        var customer = new Customer(
            id: "CUST-001",
            email: "john.doe@example.com",
            name: "John Doe",
            phone: "+1-555-0123"
        );

        var items = new List<OrderItem>
        {
            new OrderItem(
                productId: "PROD-001",
                productName: "Wireless Mouse",
                productSku: "WM-001",
                quantity: 2,
                unitPrice: 29.99m,
                totalPrice: 59.98m,
                productImageUrl: "https://example.com/images/wm-001.jpg"
            ),
            new OrderItem(
                productId: "PROD-002",
                productName: "Mechanical Keyboard",
                productSku: "KB-002",
                quantity: 1,
                unitPrice: 149.99m,
                totalPrice: 149.99m,
                productImageUrl: "https://example.com/images/kb-002.jpg"
            )
        };

        var shippingAddress = new ShippingAddress(
            street: "123 Main Street",
            city: "San Francisco",
            state: "CA",
            postalCode: "94102",
            country: "USA"
        );

        var order = new Order(
            id: "ORD-001",
            customerId: customer.Id,
            customer: customer,
            createdAt: DateTime.UtcNow,
            totalAmount: 209.97m,
            currency: "USD",
            status: "PLACED",
            items: items,
            shippingAddress: shippingAddress,
            paymentMethod: "credit_card",
            paymentLast4: "4242"
        );

        Console.WriteLine("📝 Placing order...");
        Console.WriteLine($"   Order ID: {order.Id}");
        Console.WriteLine($"   Customer: {customer.Name} ({customer.Email})");
        Console.WriteLine($"   Total: ${order.TotalAmount:F2}");
        Console.WriteLine($"   Items: {items.Count}");
        Console.WriteLine();

        // Subscribe to topic before publishing
        // (Topic should exist now from AdminClient creation above)
        consumer.Subscribe("orders");
        
        // Wait a moment for subscription to be ready
        await Task.Delay(500);

        // Place order (publishes event with full data)
        orderService.PlaceOrder(order);

        Console.WriteLine();
        Console.WriteLine("⏳ Waiting for event consumption...");
        Console.WriteLine();

        // Consume the event and demonstrate autonomous services
        var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(10));

        try
        {
            var result = consumer.Consume(cts.Token);
            var eventJson = JsonDocument.Parse(result.Value);

            Console.WriteLine("📥 Event received by consumers:");
            Console.WriteLine();

            // Each service handles the event autonomously
            Console.WriteLine("1️⃣ Email Service (Autonomous - no API calls needed):");
            emailService.HandleOrderPlaced(eventJson.RootElement);
            Console.WriteLine();

            Console.WriteLine("2️⃣ Inventory Service (Autonomous - all data in event):");
            inventoryService.HandleOrderPlaced(eventJson.RootElement);
            Console.WriteLine();

            Console.WriteLine("3️⃣ Analytics Service (Autonomous - complete metrics):");
            analyticsService.HandleOrderPlaced(eventJson.RootElement);
            Console.WriteLine();

            Console.WriteLine("✅ Event-Carried State Transfer Pattern demonstrated!");
            Console.WriteLine();
            Console.WriteLine("💡 Key Benefits:");
            Console.WriteLine("   • Services are fully autonomous");
            Console.WriteLine("   • No API calls needed between services");
            Console.WriteLine("   • All necessary data is in the event");
            Console.WriteLine("   • Services can operate independently");
            Console.WriteLine("   • Better resilience and scalability");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("⏱️  Timeout waiting for event. Make sure Kafka is running.");
            Console.WriteLine("   Run: ./scripts/start-kafka.sh");
        }
        finally
        {
            consumer.Close();
        }
    }
}

