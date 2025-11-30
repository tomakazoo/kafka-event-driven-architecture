using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using OrderSystem.Shared;

class CustomerConsumer
{
    // Mock customer database
    private static readonly Dictionary<string, Customer> Customers = new()
    {
        { "CUST-001", new Customer { CustomerId = "CUST-001", Name = "John Doe", Email = "john@example.com", IsActive = true } },
        { "CUST-002", new Customer { CustomerId = "CUST-002", Name = "Jane Smith", Email = "jane@example.com", IsActive = true } },
        { "CUST-003", new Customer { CustomerId = "CUST-003", Name = "Bob Johnson", Email = "bob@example.com", IsActive = false } } // Inactive
    };
    
    static async Task Main()
    {
        Console.WriteLine("👤 Starting Customer Service...");
        Console.WriteLine($"📡 Connecting to Kafka: localhost:9092");
        
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = "customer-service-group",
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
            Console.WriteLine("✅ Customer Service connected successfully");
            Console.WriteLine("👂 Listening for orders on 'orders-created' topic");
            Console.WriteLine("📤 Publishing validation results to 'orders-validated' topic");
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
                        Console.WriteLine($"   Customer ID: {orderCreated.CustomerId}");
                        
                        // Validate customer
                        var validationResult = ValidateCustomer(orderCreated);
                        
                        // Publish validation result
                        var validatedEvent = new OrderValidatedEvent
                        {
                            OrderId = orderCreated.OrderId,
                            CustomerId = orderCreated.CustomerId,
                            CustomerName = validationResult.CustomerName ?? "Unknown",
                            IsValid = validationResult.IsValid,
                            RejectionReason = validationResult.RejectionReason
                        };
                        
                        var topic = validationResult.IsValid ? "orders-validated" : "orders-rejected";
                        var eventJson = EventSerializer.Serialize(validatedEvent);
                        
                        var deliveryReport = await producer.ProduceAsync(
                            topic,
                            new Message<string, string>
                            {
                                Key = validatedEvent.OrderId,
                                Value = eventJson
                            });
                        
                        if (validationResult.IsValid)
                        {
                            Console.WriteLine($"✅ Order {orderCreated.OrderId} validated - Customer: {validationResult.CustomerName}");
                            Console.WriteLine($"   Published to orders-validated @ {deliveryReport.TopicPartitionOffset}");
                        }
                        else
                        {
                            Console.WriteLine($"❌ Order {orderCreated.OrderId} rejected - Reason: {validationResult.RejectionReason}");
                            Console.WriteLine($"   Published to orders-rejected @ {deliveryReport.TopicPartitionOffset}");
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
                Console.WriteLine("\n👋 Customer Service shutting down...");
            }
            finally
            {
                consumer.Close();
                producer.Flush(TimeSpan.FromSeconds(5));
            }
        }
    }
    
    static (bool IsValid, string? CustomerName, string? RejectionReason) ValidateCustomer(OrderCreatedEvent order)
    {
        if (!Customers.TryGetValue(order.CustomerId, out var customer))
        {
            return (false, null, $"Customer {order.CustomerId} not found");
        }
        
        if (!customer.IsActive)
        {
            return (false, customer.Name, $"Customer {customer.Name} is inactive");
        }
        
        return (true, customer.Name, null);
    }
}

// Simple Customer model
class Customer
{
    public string CustomerId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

