using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using OrderSystem.Shared;

class ShippingConsumer
{
    // Mock shipping addresses
    private static readonly Dictionary<string, string> CustomerAddresses = new()
    {
        { "CUST-001", "123 Main St, New York, NY 10001" },
        { "CUST-002", "456 Oak Ave, Los Angeles, CA 90001" },
        { "CUST-003", "789 Pine Rd, Chicago, IL 60601" }
    };
    
    private static int _trackingNumberCounter = 1000;
    
    static async Task Main()
    {
        Console.WriteLine("🚚 Starting Shipping Service...");
        Console.WriteLine($"📡 Connecting to Kafka: localhost:9092");
        
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = "shipping-service-group",
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
            Console.WriteLine("✅ Shipping Service connected successfully");
            Console.WriteLine("👂 Listening for validated orders and inventory reservations");
            Console.WriteLine("📤 Publishing shipping confirmations to 'orders-shipped' topic");
            Console.WriteLine();
            
            // Subscribe to both topics (will wait for topics to be created)
            try
            {
                consumer.Subscribe(new[] { "orders-validated", "inventory-reserved" });
                Console.WriteLine("✅ Subscribed to 'orders-validated' and 'inventory-reserved' topics");
                Console.WriteLine("⏳ Waiting for events... (topics will be created when services publish)");
                Console.WriteLine();
            }
            catch (Exception e)
            {
                Console.WriteLine($"⚠️  Warning: {e.Message}");
                Console.WriteLine("   Topics will be created when Customer/Inventory Services publish");
                Console.WriteLine();
            }
            
            // Track orders waiting for both events
            var pendingOrders = new Dictionary<string, OrderShippingState>();
            
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
                        Console.WriteLine("⏳ Topics not yet created. Waiting...");
                        Console.WriteLine("   Start Customer and Inventory Services to create topics.");
                        await Task.Delay(2000, cts.Token); // Wait 2 seconds before retrying
                        continue;
                    }
                    
                    if (result == null) continue;
                    
                    try
                    {
                        if (result!.Topic == "orders-validated")
                        {
                            var validated = EventSerializer.Deserialize<OrderValidatedEvent>(result.Message.Value);
                            if (validated != null && validated.IsValid)
                            {
                                Console.WriteLine($"📨 Received Order Validation: {validated.OrderId}");
                                
                                if (!pendingOrders.ContainsKey(validated.OrderId))
                                {
                                    pendingOrders[validated.OrderId] = new OrderShippingState
                                    {
                                        OrderId = validated.OrderId,
                                        CustomerId = validated.CustomerId,
                                        CustomerName = validated.CustomerName
                                    };
                                }
                                
                                pendingOrders[validated.OrderId].IsValidated = true;
                                Console.WriteLine($"   Customer: {validated.CustomerName}");
                                
                                await TryShipOrder(pendingOrders[validated.OrderId], producer);
                            }
                        }
                        else if (result.Topic == "inventory-reserved")
                        {
                            var reserved = EventSerializer.Deserialize<InventoryReservedEvent>(result.Message.Value);
                            if (reserved != null && reserved.Success)
                            {
                                Console.WriteLine($"📨 Received Inventory Reservation: {reserved.OrderId}");
                                
                                if (!pendingOrders.ContainsKey(reserved.OrderId))
                                {
                                    pendingOrders[reserved.OrderId] = new OrderShippingState
                                    {
                                        OrderId = reserved.OrderId
                                    };
                                }
                                
                                pendingOrders[reserved.OrderId].IsInventoryReserved = true;
                                pendingOrders[reserved.OrderId].ReservedItems = reserved.ReservedItems;
                                
                                Console.WriteLine($"   Reserved Items: {reserved.ReservedItems.Count}");
                                
                                await TryShipOrder(pendingOrders[reserved.OrderId], producer);
                            }
                        }
                        
                        Console.WriteLine();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"❌ Error processing event: {e.Message}");
                        Console.WriteLine();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("\n👋 Shipping Service shutting down...");
            }
            finally
            {
                consumer.Close();
                producer.Flush(TimeSpan.FromSeconds(5));
            }
        }
    }
    
    static async Task TryShipOrder(OrderShippingState orderState, IProducer<string, string> producer)
    {
        // Check if we have both validation and inventory reservation
        if (!orderState.IsValidated || !orderState.IsInventoryReserved)
        {
            return; // Wait for the other event
        }
        
        // Get shipping address
        if (!CustomerAddresses.TryGetValue(orderState.CustomerId ?? "", out var address))
        {
            address = "Address not found";
        }
        
        // Generate tracking number
        var trackingNumber = $"TRACK-{_trackingNumberCounter++}";
        
        // Create shipping event
        var shippedEvent = new OrderShippedEvent
        {
            OrderId = orderState.OrderId,
            TrackingNumber = trackingNumber,
            ShippingAddress = address,
            ShippedDate = DateTime.UtcNow
        };
        
        var eventJson = EventSerializer.Serialize(shippedEvent);
        
        var deliveryReport = await producer.ProduceAsync(
            "orders-shipped",
            new Message<string, string>
            {
                Key = shippedEvent.OrderId,
                Value = eventJson
            });
        
        Console.WriteLine($"🚚 Order {orderState.OrderId} shipped!");
        Console.WriteLine($"   Tracking Number: {trackingNumber}");
        Console.WriteLine($"   Address: {address}");
        Console.WriteLine($"   Published to orders-shipped @ {deliveryReport.TopicPartitionOffset}");
        
        // Remove from pending orders
        // (In production, you'd want to keep this for tracking)
    }
}

// Helper class to track order state
class OrderShippingState
{
    public string OrderId { get; set; } = string.Empty;
    public string? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public bool IsValidated { get; set; }
    public bool IsInventoryReserved { get; set; }
    public List<OrderItem> ReservedItems { get; set; } = new();
}

