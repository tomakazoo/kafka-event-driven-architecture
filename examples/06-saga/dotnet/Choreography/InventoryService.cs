using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using SagaPattern.Shared.Domain;
using SagaPattern.Shared.Events;

namespace SagaPattern.Choreography;

public class InsufficientInventoryException : Exception
{
    public InsufficientInventoryException(string message) : base(message) { }
}

public class InventoryService
{
    private readonly IConsumer<string, string> _consumer;
    private readonly IEventPublisher _eventPublisher;
    private readonly Dictionary<string, int> _inventory = new();

    public InventoryService(IConsumer<string, string> consumer, IEventPublisher eventPublisher)
    {
        _consumer = consumer;
        _consumer.Subscribe("order-events");
        _eventPublisher = eventPublisher;
        
        // Initialize mock inventory
        _inventory["PROD-001"] = 10;
        _inventory["PROD-002"] = 5;
        _inventory["PROD-003"] = 8;
    }

    public async Task ConsumeEventsAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("📦 Inventory Service: Listening for order events...");
        
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var result = _consumer.Consume(cancellationToken);
                var eventData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(result.Message.Value);
                
                if (!eventData.ContainsKey("Order"))
                {
                    var eventType = eventData.ContainsKey("OrderId") 
                        ? GetEventTypeFromData(eventData)
                        : "Unknown";
                    
                    if (eventType == "PaymentFailed")
                    {
                        await HandlePaymentFailedAsync(eventData);
                    }
                    continue;
                }
                
                var orderJson = eventData["Order"].GetRawText();
                var order = JsonSerializer.Deserialize<Order>(orderJson, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });
                
                if (order != null)
                {
                    await HandleOrderPlacedAsync(order);
                }
            }
            catch (ConsumeException ex)
            {
                Console.WriteLine($"❌ Inventory Service Error: {ex.Error.Reason}");
            }
        }
    }

    private string GetEventTypeFromData(Dictionary<string, JsonElement> eventData)
    {
        if (eventData.ContainsKey("OrderId"))
        {
            // Check if it's a failure event
            if (eventData.ContainsKey("Reason"))
                return "OrderFailed";
            if (eventData.ContainsKey("OrderId") && !eventData.ContainsKey("ReservationId"))
                return "PaymentFailed";
        }
        return "Unknown";
    }

    private async Task HandleOrderPlacedAsync(Order order)
    {
        Console.WriteLine($"📦 Inventory Service: Received OrderPlacedEvent for order {order.OrderId}");
        
        try
        {
            // Reserve inventory
            await ReserveInventoryAsync(order.OrderId, order.Items);
            
            // Publish success
            await _eventPublisher.PublishAsync("order-events", new InventoryReservedEvent(order.OrderId));
            Console.WriteLine($"   ✅ Inventory reserved for order {order.OrderId}");
        }
        catch (InsufficientInventoryException ex)
        {
            // Publish failure
            await _eventPublisher.PublishAsync("order-events", 
                new InventoryReservationFailedEvent(order.OrderId));
            Console.WriteLine($"   ❌ Inventory reservation failed for order {order.OrderId}: {ex.Message}");
        }
    }

    private async Task HandlePaymentFailedAsync(Dictionary<string, JsonElement> eventData)
    {
        var orderId = eventData["OrderId"].GetString();
        Console.WriteLine($"📦 Inventory Service: Received PaymentFailedEvent for order {orderId}");
        
        // Compensating transaction
        await ReleaseInventoryAsync(orderId);
        Console.WriteLine($"   🔄 Released inventory for order {orderId}");
    }

    private Task ReserveInventoryAsync(string orderId, List<OrderItem> items)
    {
        foreach (var item in items)
        {
            if (!_inventory.ContainsKey(item.ProductId) || _inventory[item.ProductId] < item.Quantity)
            {
                throw new InsufficientInventoryException(
                    $"Insufficient inventory for {item.ProductId}. Available: {(_inventory.ContainsKey(item.ProductId) ? _inventory[item.ProductId] : 0)}, Required: {item.Quantity}");
            }
            
            _inventory[item.ProductId] -= item.Quantity;
            Console.WriteLine($"   📉 Reserved {item.Quantity}x {item.ProductId}. Remaining: {_inventory[item.ProductId]}");
        }
        
        return Task.CompletedTask;
    }

    private Task ReleaseInventoryAsync(string orderId)
    {
        // In a real system, we'd track reservations and release them
        // For this demo, we'll just log it
        Console.WriteLine($"   🔄 Releasing inventory for order {orderId}");
        return Task.CompletedTask;
    }
}


