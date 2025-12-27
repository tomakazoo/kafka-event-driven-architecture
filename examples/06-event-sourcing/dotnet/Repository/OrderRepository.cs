using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventSourcing.Domain;
using EventSourcing.EventStore;

namespace EventSourcing.Repository;

public class OrderRepository
{
    private readonly KafkaEventStore _eventStore;

    public OrderRepository(KafkaEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    public async Task SaveAsync(Order order)
    {
        if (!order.UncommittedEvents.Any())
            return;
        
        var expectedVersion = order.Version - order.UncommittedEvents.Count;
        
        await _eventStore.SaveEventsAsync(
            order.OrderId,
            order.UncommittedEvents,
            expectedVersion
        );
        
        order.UncommittedEvents.Clear();
        
        // Save snapshot every 10 events (for demo purposes)
        if (order.Version % 10 == 0)
        {
            _eventStore.SaveSnapshot(
                order.OrderId,
                new Dictionary<string, object>
                {
                    ["customer_id"] = order.CustomerId,
                    ["items"] = order.Items.Select(i => new Dictionary<string, object>
                    {
                        ["product_id"] = i.ProductId,
                        ["quantity"] = i.Quantity,
                        ["price"] = i.Price
                    }).ToList(),
                    ["shipping_address"] = order.ShippingAddress ?? new Dictionary<string, string>(),
                    ["status"] = order.Status
                },
                order.Version
            );
        }
    }

    public async Task<Order> GetAsync(string orderId)
    {
        // Try to load from snapshot
        var snapshot = _eventStore.GetSnapshot(orderId);
        
        List<DomainEvent> events;
        Order order;
        
        if (snapshot != null)
        {
            // Create order and replay events from snapshot version
            order = Order.CreateEmpty(orderId);
            events = await _eventStore.GetEventsAsync(orderId, snapshot.Version);
            
            // Apply snapshot state by creating events (simplified approach)
            // In production, you'd restore state directly
            order.LoadFromHistory(events);
        }
        else
        {
            // Load all events
            order = Order.CreateEmpty(orderId);
            events = await _eventStore.GetEventsAsync(orderId);
        }
        
        // Replay events
        order.LoadFromHistory(events);
        
        return order;
    }

    public async Task<Order> GetOrderAtTimestampAsync(string orderId, DateTime timestamp)
    {
        // See order state at specific point in time
        var allEvents = await _eventStore.GetEventsAsync(orderId);
        
        // Filter events before timestamp
        var historicalEvents = allEvents
            .Where(e => e.Timestamp <= timestamp)
            .OrderBy(e => e.Version)
            .ToList();
        
        // Rebuild historical state
        var order = Order.CreateEmpty(orderId);
        order.LoadFromHistory(historicalEvents);
        
        return order;
    }
}

