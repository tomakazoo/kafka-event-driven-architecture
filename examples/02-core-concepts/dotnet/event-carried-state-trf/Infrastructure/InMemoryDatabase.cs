using System;
using System.Collections.Generic;
using EventCarriedStateTransfer.Domain;
using EventCarriedStateTransfer.Interfaces;

namespace EventCarriedStateTransfer.Infrastructure;

public class InMemoryDatabase : IDatabase
{
    private readonly Dictionary<string, Order> _orders = new();

    public void Save(Order order)
    {
        _orders[order.Id] = order;
        Console.WriteLine($"💾 Order {order.Id} saved to database");
    }

    public Order? GetById(string orderId)
    {
        return _orders.TryGetValue(orderId, out var order) ? order : null;
    }
}

