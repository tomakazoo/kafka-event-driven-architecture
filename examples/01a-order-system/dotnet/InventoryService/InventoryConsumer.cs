using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using OrderSystem.Shared;

class InventoryConsumer
{
    // Mock inventory database
    private static readonly Dictionary<string, Product> Inventory = new()
    {
        { "PROD-001", new Product { ProductId = "PROD-001", Name = "Laptop", Stock = 10, Price = 999.99m } },
        { "PROD-002", new Product { ProductId = "PROD-002", Name = "Mouse", Stock = 50, Price = 29.99m } },
        { "PROD-003", new Product { ProductId = "PROD-003", Name = "Keyboard", Stock = 25, Price = 79.99m } },
        { "PROD-004", new Product { ProductId = "PROD-004", Name = "Monitor", Stock = 5, Price = 299.99m } }
    };
    
    static async Task Main()
    {
        Console.WriteLine("📦 Starting Inventory Service...");
        Console.WriteLine($"📡 Connecting to Kafka: localhost:9092");
        
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = "inventory-service-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = "localhost:9092",
            RequestTimeoutMs = 5000,
            MessageTimeoutMs = 5000,
            SocketTimeoutMs = 5000
        };
        
        using (var consumer = new ConsumerBuilder<string, string>(consumerConfig)
            .SetErrorHandler((c, e) => Console.WriteLine($"❌ Consumer Error: {e.Reason}"))
            .Build())
        using (var producer = new ProducerBuilder<string, string>(producerConfig)
            .SetErrorHandler((p, e) => Console.WriteLine($"❌ Producer Error: {e.Reason}"))
            .Build())
        {
            Console.WriteLine("✅ Inventory Service connected successfully");
            Console.WriteLine("👂 Listening for orders on 'orders-created' topic");
            Console.WriteLine("📤 Publishing reservation results to 'inventory-reserved' or 'inventory-insufficient' topics");
            Console.WriteLine();
            
            // Subscribe to topic (will wait for topic to be created)
            try
            {
                consumer.Subscribe("orders-created");
                Console.WriteLine("✅ Subscribed to 'orders-created' topic");
                Console.WriteLine("⏳ Waiting for orders... (topic will be created when Order Service publishes)");
                Console.WriteLine();
            }
            catch (Exception e)
            {
                Console.WriteLine($"⚠️  Warning: {e.Message}");
                Console.WriteLine("   Topic will be created when Order Service publishes first message");
                Console.WriteLine();
            }
            
            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };
            
            try
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    ConsumeResult<string, string>? result = null;
                    
                    try
                    {
                        result = consumer.Consume(cts.Token);
                    }
                    catch (ConsumeException e) when (e.Error.Code == ErrorCode.UnknownTopicOrPart || e.Error.Reason.Contains("Unknown topic"))
                    {
                        Console.WriteLine("⏳ Topic 'orders-created' not yet created. Waiting...");
                        Console.WriteLine("   Start Order Service to create the topic.");
                        await Task.Delay(2000, cts.Token); // Wait 2 seconds before retrying
                        continue;
                    }
                    
                    if (result == null) continue;
                    
                    try
                    {
                        var orderCreated = EventSerializer.Deserialize<OrderCreatedEvent>(result!.Message.Value);
                        
                        if (orderCreated == null)
                        {
                            Console.WriteLine("⚠️  Failed to deserialize order event");
                            continue;
                        }
                        
                        Console.WriteLine($"📨 Received Order: {orderCreated.OrderId}");
                        Console.WriteLine($"   Items: {orderCreated.Items.Count}");
                        
                        // Check and reserve inventory
                        var reservationResult = ReserveInventory(orderCreated);
                        
                        // Publish reservation result
                        var reservedEvent = new InventoryReservedEvent
                        {
                            OrderId = orderCreated.OrderId,
                            ReservedItems = reservationResult.ReservedItems,
                            Success = reservationResult.Success,
                            FailureReason = reservationResult.FailureReason
                        };
                        
                        var topic = reservationResult.Success ? "inventory-reserved" : "inventory-insufficient";
                        var eventJson = EventSerializer.Serialize(reservedEvent);
                        
                        var deliveryReport = await producer.ProduceAsync(
                            topic,
                            new Message<string, string>
                            {
                                Key = reservedEvent.OrderId,
                                Value = eventJson
                            });
                        
                        if (reservationResult.Success)
                        {
                            Console.WriteLine($"✅ Order {orderCreated.OrderId} - Inventory reserved");
                            foreach (var item in reservationResult.ReservedItems)
                            {
                                Console.WriteLine($"   - {item.ProductName}: {item.Quantity} units");
                            }
                            Console.WriteLine($"   Published to inventory-reserved @ {deliveryReport.TopicPartitionOffset}");
                        }
                        else
                        {
                            Console.WriteLine($"❌ Order {orderCreated.OrderId} - Insufficient inventory");
                            Console.WriteLine($"   Reason: {reservationResult.FailureReason}");
                            Console.WriteLine($"   Published to inventory-insufficient @ {deliveryReport.TopicPartitionOffset}");
                        }
                        
                        Console.WriteLine();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"❌ Error processing order: {e.Message}");
                        Console.WriteLine();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("\n👋 Inventory Service shutting down...");
            }
            finally
            {
                consumer.Close();
                producer.Flush(TimeSpan.FromSeconds(5));
            }
        }
    }
    
    static (bool Success, List<OrderItem> ReservedItems, string? FailureReason) ReserveInventory(OrderCreatedEvent order)
    {
        var reservedItems = new List<OrderItem>();
        
        foreach (var orderItem in order.Items)
        {
            if (!Inventory.TryGetValue(orderItem.ProductId, out var product))
            {
                return (false, new List<OrderItem>(), $"Product {orderItem.ProductId} not found");
            }
            
            if (product.Stock < orderItem.Quantity)
            {
                return (false, new List<OrderItem>(), 
                    $"Insufficient stock for {product.Name}. Available: {product.Stock}, Requested: {orderItem.Quantity}");
            }
            
            // Reserve inventory (in real system, this would update database)
            product.Stock -= orderItem.Quantity;
            
            reservedItems.Add(new OrderItem
            {
                ProductId = product.ProductId,
                ProductName = product.Name,
                Quantity = orderItem.Quantity,
                Price = product.Price
            });
        }
        
        return (true, reservedItems, null);
    }
}

// Simple Product model
class Product
{
    public string ProductId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Stock { get; set; }
    public decimal Price { get; set; }
}

