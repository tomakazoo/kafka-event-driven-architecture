using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Confluent.Kafka;
using OrderSystem.Shared;

class OrderProducer
{
    static async Task Main()
    {
        Console.WriteLine("🛒 Starting Order Service...");
        Console.WriteLine($"📡 Connecting to Kafka: localhost:9092");
        
        var config = new ProducerConfig 
        { 
            BootstrapServers = "localhost:9092",
            RequestTimeoutMs = 5000,
            MessageTimeoutMs = 5000,
            SocketTimeoutMs = 5000
        };
        
        using (var producer = new ProducerBuilder<string, string>(config)
            .SetErrorHandler((p, e) => 
            {
                Console.WriteLine($"❌ Producer Error: {e.Reason}");
            })
            .Build())
        {
            Console.WriteLine("✅ Order Service connected successfully");
            Console.WriteLine("📤 Publishing orders to 'orders-created' topic");
            Console.WriteLine();
            
            var topic = "orders-created";
            
            try
            {
                // Create sample orders
                var orders = CreateSampleOrders();
                
                foreach (var order in orders)
                {
                    Console.WriteLine($"📨 Creating Order: {order.OrderId}");
                    Console.WriteLine($"   Customer: {order.CustomerId}");
                    Console.WriteLine($"   Items: {order.Items.Count}");
                    Console.WriteLine($"   Total: ${order.Total:F2}");
                    
                    var eventJson = EventSerializer.Serialize(order);
                    
                    var report = await producer.ProduceAsync(
                        topic,
                        new Message<string, string>
                        {
                            Key = order.OrderId,
                            Value = eventJson
                        });
                    
                    Console.WriteLine($"✅ Order {order.OrderId} published to {report.TopicPartitionOffset}");
                    Console.WriteLine();
                    
                    // Small delay between orders
                    await Task.Delay(1000);
                }
                
                producer.Flush(TimeSpan.FromSeconds(5));
                Console.WriteLine("✅ All orders published successfully!");
            }
            catch (ProduceException<string, string> e)
            {
                Console.WriteLine($"❌ Failed to publish order: {e.Error.Reason}");
                Console.WriteLine($"   Error Code: {e.Error.Code}");
                Environment.Exit(1);
            }
            catch (KafkaException e)
            {
                Console.WriteLine($"❌ Kafka error: {e.Message}");
                Environment.Exit(1);
            }
            catch (Exception e)
            {
                Console.WriteLine($"❌ Unexpected error: {e.Message}");
                Environment.Exit(1);
            }
        }
    }
    
    static List<OrderCreatedEvent> CreateSampleOrders()
    {
        return new List<OrderCreatedEvent>
        {
            new OrderCreatedEvent
            {
                OrderId = "ORD-001",
                CustomerId = "CUST-001",
                Items = new List<OrderItem>
                {
                    new OrderItem { ProductId = "PROD-001", ProductName = "Laptop", Quantity = 1, Price = 999.99m },
                    new OrderItem { ProductId = "PROD-002", ProductName = "Mouse", Quantity = 1, Price = 29.99m }
                },
                Total = 1029.98m
            },
            new OrderCreatedEvent
            {
                OrderId = "ORD-002",
                CustomerId = "CUST-002",
                Items = new List<OrderItem>
                {
                    new OrderItem { ProductId = "PROD-003", ProductName = "Keyboard", Quantity = 2, Price = 79.99m }
                },
                Total = 159.98m
            },
            new OrderCreatedEvent
            {
                OrderId = "ORD-003",
                CustomerId = "CUST-999", // Invalid customer for testing
                Items = new List<OrderItem>
                {
                    new OrderItem { ProductId = "PROD-001", ProductName = "Laptop", Quantity = 1, Price = 999.99m }
                },
                Total = 999.99m
            },
            new OrderCreatedEvent
            {
                OrderId = "ORD-004",
                CustomerId = "CUST-001",
                Items = new List<OrderItem>
                {
                    new OrderItem { ProductId = "PROD-999", ProductName = "Out of Stock Item", Quantity = 10, Price = 49.99m }
                },
                Total = 499.90m
            }
        };
    }
}

