using System;
using System.Collections.Generic;
using System.Text.Json;

namespace OrderSystem.Shared;

// Base event class
public abstract class OrderEvent
{
    public string EventId { get; set; } = Guid.NewGuid().ToString();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string OrderId { get; set; } = string.Empty;
}

// Order Created Event
public class OrderCreatedEvent : OrderEvent
{
    public string CustomerId { get; set; } = string.Empty;
    public List<OrderItem> Items { get; set; } = new();
    public decimal Total { get; set; }
    public string Status { get; set; } = "created";
}

// Order Item
public class OrderItem
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

// Order Validated Event
public class OrderValidatedEvent : OrderEvent
{
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public bool IsValid { get; set; }
    public string? RejectionReason { get; set; }
}

// Inventory Reserved Event
public class InventoryReservedEvent : OrderEvent
{
    public List<OrderItem> ReservedItems { get; set; } = new();
    public bool Success { get; set; }
    public string? FailureReason { get; set; }
}

// Order Shipped Event
public class OrderShippedEvent : OrderEvent
{
    public string TrackingNumber { get; set; } = string.Empty;
    public string ShippingAddress { get; set; } = string.Empty;
    public DateTime ShippedDate { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "shipped";
}

// Helper for JSON serialization
public static class EventSerializer
{
    public static string Serialize<T>(T eventObj) where T : OrderEvent
    {
        return JsonSerializer.Serialize(eventObj, new JsonSerializerOptions 
        { 
            WriteIndented = false 
        });
    }

    public static T? Deserialize<T>(string json) where T : OrderEvent
    {
        return JsonSerializer.Deserialize<T>(json);
    }
}

