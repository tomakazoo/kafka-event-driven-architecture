using EventCarriedStateTransfer.Domain;

namespace EventCarriedStateTransfer.Interfaces;

public interface IDatabase
{
    void Save(Order order);
    Order? GetById(string orderId);
}

