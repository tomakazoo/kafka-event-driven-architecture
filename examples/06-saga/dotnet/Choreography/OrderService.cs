using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Confluent.Kafka;
using SagaPattern.Shared.Domain;
using SagaPattern.Shared.Events;

namespace SagaPattern.Choreography;

public interface IOrderRepository
{
    void Save(Order order);
    Order Get(string orderId);
}

public class InMemoryOrderRepository : IOrderRepository
{
    private readonly Dictionary<string, Order> _orders = new();

    public void Save(Order order)
    {
        _orders[order.OrderId] = order;
    }

    public Order Get(string orderId)
    {
        return _orders.ContainsKey(orderId) ? _orders[orderId] : null;
    }
}

public interface IEventPublisher
{
    Task PublishAsync(string topic, object eventData);
}

public class KafkaEventPublisher : IEventPublisher
{
    private readonly IProducer<string, string> _producer;

    public KafkaEventPublisher(IProducer<string, string> producer)
    {
        _producer = producer;
    }

    public async Task PublishAsync(string topic, object eventData)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(eventData);
        var message = new Message<string, string>
        {
            Key = eventData switch
            {
                OrderPlacedEvent e => e.Order.OrderId,
                InventoryReservedEvent e => e.OrderId,
                InventoryReservationFailedEvent e => e.OrderId,
                PaymentReceivedEvent e => e.OrderId,
                PaymentFailedEvent e => e.OrderId,
                _ => Guid.NewGuid().ToString()
            },
            Value = json
        };

        await _producer.ProduceAsync(topic, message);
        _producer.Flush(TimeSpan.FromSeconds(5));
    }
}

public class OrderService
{
    private readonly IOrderRepository _repository;
    private readonly IEventPublisher _eventPublisher;

    public OrderService(IOrderRepository repository, IEventPublisher eventPublisher)
    {
        _repository = repository;
        _eventPublisher = eventPublisher;
    }

    public async Task PlaceOrderAsync(Order order)
    {
        Console.WriteLine($"📝 Order Service: Placing order {order.OrderId}...");
        
        // 1. Save order
        _repository.Save(order);
        Console.WriteLine($"   ✅ Order {order.OrderId} saved");
        
        // 2. Publish event
        await _eventPublisher.PublishAsync("order-events", new OrderPlacedEvent(order));
        Console.WriteLine($"   📤 Published OrderPlacedEvent for order {order.OrderId}");
    }
}

