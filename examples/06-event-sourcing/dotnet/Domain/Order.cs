using System;
using System.Collections.Generic;
using System.Linq;

namespace EventSourcing.Domain;

public class OrderItem
{
    public string ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }

    public OrderItem(string productId, int quantity, decimal price)
    {
        ProductId = productId;
        Quantity = quantity;
        Price = price;
    }
}

// Aggregate Root
public class Order
{
    public string OrderId { get; private set; }
    public string CustomerId { get; private set; } = string.Empty;
    public List<OrderItem> Items { get; private set; }
    public Dictionary<string, string>? ShippingAddress { get; private set; }
    public string Status { get; private set; } = string.Empty;
    public int Version { get; private set; }
    public List<DomainEvent> UncommittedEvents { get; private set; }

    private Order(string orderId)
    {
        OrderId = orderId;
        Items = new List<OrderItem>();
        UncommittedEvents = new List<DomainEvent>();
        Version = 0;
    }

    // Factory method for loading from snapshot
    public static Order CreateEmpty(string orderId)
    {
        return new Order(orderId);
    }

    // Commands
    public static Order Create(string orderId, string customerId)
    {
        var order = new Order(orderId);
        var evt = new OrderCreated(
            EventId: Guid.NewGuid().ToString(),
            AggregateId: orderId,
            EventType: "OrderCreated",
            Timestamp: DateTime.UtcNow,
            Version: 1,
            CustomerId: customerId
        );
        
        order.Apply(evt);
        order.UncommittedEvents.Add(evt);
        
        return order;
    }

    public void AddItem(string productId, int quantity, decimal price)
    {
        if (Status == "SUBMITTED")
            throw new InvalidOperationException("Cannot modify submitted order");
        
        var evt = new ItemAdded(
            EventId: Guid.NewGuid().ToString(),
            AggregateId: OrderId,
            EventType: "ItemAdded",
            Timestamp: DateTime.UtcNow,
            Version: Version + 1,
            ProductId: productId,
            Quantity: quantity,
            Price: price
        );
        
        Apply(evt);
        UncommittedEvents.Add(evt);
    }

    public void RemoveItem(string productId)
    {
        if (Status == "SUBMITTED")
            throw new InvalidOperationException("Cannot modify submitted order");
        
        var evt = new ItemRemoved(
            EventId: Guid.NewGuid().ToString(),
            AggregateId: OrderId,
            EventType: "ItemRemoved",
            Timestamp: DateTime.UtcNow,
            Version: Version + 1,
            ProductId: productId
        );
        
        Apply(evt);
        UncommittedEvents.Add(evt);
    }

    public void SetShippingAddress(Dictionary<string, string> address)
    {
        var evt = new ShippingAddressSet(
            EventId: Guid.NewGuid().ToString(),
            AggregateId: OrderId,
            EventType: "ShippingAddressSet",
            Timestamp: DateTime.UtcNow,
            Version: Version + 1,
            Address: address
        );
        
        Apply(evt);
        UncommittedEvents.Add(evt);
    }

    public void Submit()
    {
        if (!Items.Any())
            throw new InvalidOperationException("Cannot submit empty order");
        if (ShippingAddress == null)
            throw new InvalidOperationException("Shipping address required");
        
        var evt = new OrderSubmitted(
            EventId: Guid.NewGuid().ToString(),
            AggregateId: OrderId,
            EventType: "OrderSubmitted",
            Timestamp: DateTime.UtcNow,
            Version: Version + 1
        );
        
        Apply(evt);
        UncommittedEvents.Add(evt);
    }

    public void RecordPayment(string paymentId, decimal amount)
    {
        var evt = new PaymentReceived(
            EventId: Guid.NewGuid().ToString(),
            AggregateId: OrderId,
            EventType: "PaymentReceived",
            Timestamp: DateTime.UtcNow,
            Version: Version + 1,
            PaymentId: paymentId,
            Amount: amount
        );
        
        Apply(evt);
        UncommittedEvents.Add(evt);
    }

    public void Ship(string trackingNumber)
    {
        if (Status != "PAID")
            throw new InvalidOperationException("Order must be paid before shipping");
        
        var evt = new OrderShipped(
            EventId: Guid.NewGuid().ToString(),
            AggregateId: OrderId,
            EventType: "OrderShipped",
            Timestamp: DateTime.UtcNow,
            Version: Version + 1,
            TrackingNumber: trackingNumber
        );
        
        Apply(evt);
        UncommittedEvents.Add(evt);
    }

    // Event Handlers (apply events to rebuild state)
    internal void Apply(DomainEvent evt)
    {
        switch (evt)
        {
            case OrderCreated e:
                CustomerId = e.CustomerId;
                Status = "CREATED";
                break;
            
            case ItemAdded e:
                Items.Add(new OrderItem(e.ProductId, e.Quantity, e.Price));
                break;
            
            case ItemRemoved e:
                Items.RemoveAll(i => i.ProductId == e.ProductId);
                break;
            
            case ShippingAddressSet e:
                ShippingAddress = e.Address;
                break;
            
            case OrderSubmitted:
                Status = "SUBMITTED";
                break;
            
            case PaymentReceived:
                Status = "PAID";
                break;
            
            case OrderShipped:
                Status = "SHIPPED";
                break;
        }
        
        Version = evt.Version;
    }

    public void LoadFromHistory(IEnumerable<DomainEvent> events)
    {
        foreach (var evt in events)
        {
            Apply(evt);
        }
    }
}

public class ConcurrencyException : Exception
{
    public ConcurrencyException(string message) : base(message) { }
}

