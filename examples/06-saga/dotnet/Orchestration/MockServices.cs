using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SagaPattern.Shared.Domain;

namespace SagaPattern.Orchestration;

public class MockInventoryService : IInventoryService
{
    private readonly Dictionary<string, int> _inventory = new();
    private int _reservationCounter = 1000;

    public MockInventoryService()
    {
        _inventory["PROD-001"] = 10;
        _inventory["PROD-002"] = 5;
        _inventory["PROD-003"] = 8;
    }

    public Task<InventoryResult> ReserveAsync(List<OrderItem> items)
    {
        foreach (var item in items)
        {
            if (!_inventory.ContainsKey(item.ProductId) || _inventory[item.ProductId] < item.Quantity)
            {
                throw new InsufficientInventoryException(
                    $"Insufficient inventory for {item.ProductId}");
            }
            
            _inventory[item.ProductId] -= item.Quantity;
        }
        
        var reservationId = $"RES-{_reservationCounter++}";
        return Task.FromResult(new InventoryResult(reservationId));
    }

    public Task ReleaseAsync(string reservationId)
    {
        Console.WriteLine($"   🔄 Releasing inventory reservation {reservationId}");
        // In a real system, we'd track and release specific reservations
        return Task.CompletedTask;
    }
}

public class MockPaymentService : IPaymentService
{
    private readonly HashSet<string> _failedPayments = new();
    private int _paymentCounter = 2000;

    public MockPaymentService()
    {
        // Simulate some failed payments
        _failedPayments.Add("ORD-FAIL-001");
    }

    public Task<PaymentResult> ChargeAsync(string customerId, decimal amount)
    {
        // Simulate payment failure for specific orders
        if (_failedPayments.Contains(customerId))
        {
            throw new PaymentException($"Payment declined for customer {customerId}");
        }
        
        var paymentId = $"PAY-{_paymentCounter++}";
        return Task.FromResult(new PaymentResult(paymentId));
    }

    public Task RefundAsync(string paymentId)
    {
        Console.WriteLine($"   🔄 Refunding payment {paymentId}");
        return Task.CompletedTask;
    }
}

public class MockShippingService : IShippingService
{
    private int _trackingCounter = 3000;

    public Task<ShippingResult> ScheduleAsync(ShippingAddress address)
    {
        var trackingNumber = $"TRACK-{_trackingCounter++}";
        return Task.FromResult(new ShippingResult(trackingNumber));
    }
}

public class InsufficientInventoryException : Exception
{
    public InsufficientInventoryException(string message) : base(message) { }
}

public class PaymentException : Exception
{
    public PaymentException(string message) : base(message) { }
}


