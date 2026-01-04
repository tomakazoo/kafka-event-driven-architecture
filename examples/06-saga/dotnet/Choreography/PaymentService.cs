using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using SagaPattern.Shared.Events;

namespace SagaPattern.Choreography;

public class PaymentException : Exception
{
    public PaymentException(string message) : base(message) { }
}

public class PaymentService
{
    private readonly IConsumer<string, string> _consumer;
    private readonly IEventPublisher _eventPublisher;
    private readonly HashSet<string> _failedPayments = new();

    public PaymentService(IConsumer<string, string> consumer, IEventPublisher eventPublisher)
    {
        _consumer = consumer;
        _consumer.Subscribe("order-events");
        _eventPublisher = eventPublisher;
        
        // Simulate some failed payments for demo
        _failedPayments.Add("ORD-FAIL-001");
    }

    public async Task ConsumeEventsAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("💳 Payment Service: Listening for order events...");
        
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var result = _consumer.Consume(cancellationToken);
                var eventData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(result.Message.Value);
                
                if (eventData.ContainsKey("OrderId"))
                {
                    var orderId = eventData["OrderId"].GetString();
                    
                    // Check if it's InventoryReserved event
                    if (!eventData.ContainsKey("Reason") && !eventData.ContainsKey("Order"))
                    {
                        await HandleInventoryReservedAsync(orderId);
                    }
                }
            }
            catch (ConsumeException ex)
            {
                Console.WriteLine($"❌ Payment Service Error: {ex.Error.Reason}");
            }
        }
    }

    private async Task HandleInventoryReservedAsync(string orderId)
    {
        Console.WriteLine($"💳 Payment Service: Received InventoryReservedEvent for order {orderId}");
        
        try
        {
            // Process payment
            await ChargeCustomerAsync(orderId);
            
            // Publish success
            await _eventPublisher.PublishAsync("order-events", new PaymentReceivedEvent(orderId));
            Console.WriteLine($"   ✅ Payment processed for order {orderId}");
        }
        catch (PaymentException ex)
        {
            // Publish failure (triggers inventory rollback)
            await _eventPublisher.PublishAsync("order-events", 
                new PaymentFailedEvent(orderId));
            Console.WriteLine($"   ❌ Payment failed for order {orderId}: {ex.Message}");
        }
    }

    private Task ChargeCustomerAsync(string orderId)
    {
        // Simulate payment failure for specific orders
        if (_failedPayments.Contains(orderId))
        {
            throw new PaymentException($"Payment declined for order {orderId}");
        }
        
        Console.WriteLine($"   💰 Charging customer for order {orderId}");
        return Task.CompletedTask;
    }
}




