using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SagaPattern.Choreography;
using SagaPattern.Shared.Domain;
using SagaPattern.Shared.Events;

namespace SagaPattern.Orchestration;

public interface IInventoryService
{
    Task<InventoryResult> ReserveAsync(List<OrderItem> items);
    Task ReleaseAsync(string reservationId);
}

public interface IPaymentService
{
    Task<PaymentResult> ChargeAsync(string customerId, decimal amount);
    Task RefundAsync(string paymentId);
}

public interface IShippingService
{
    Task<ShippingResult> ScheduleAsync(ShippingAddress address);
}

public class InventoryResult
{
    public string ReservationId { get; set; }
    
    public InventoryResult(string reservationId)
    {
        ReservationId = reservationId;
    }
}

public class PaymentResult
{
    public string PaymentId { get; set; }
    
    public PaymentResult(string paymentId)
    {
        PaymentId = paymentId;
    }
}

public class ShippingResult
{
    public string TrackingNumber { get; set; }
    
    public ShippingResult(string trackingNumber)
    {
        TrackingNumber = trackingNumber;
    }
}

public class OrderFulfillmentSaga
{
    private readonly string _orderId;
    private readonly IInventoryService _inventoryService;
    private readonly IPaymentService _paymentService;
    private readonly IShippingService _shippingService;
    private readonly IEventPublisher _eventPublisher;
    
    private string _state = "STARTED";
    private readonly List<Func<Task>> _compensations = new();

    public OrderFulfillmentSaga(
        string orderId,
        IInventoryService inventoryService,
        IPaymentService paymentService,
        IShippingService shippingService,
        IEventPublisher eventPublisher)
    {
        _orderId = orderId;
        _inventoryService = inventoryService;
        _paymentService = paymentService;
        _shippingService = shippingService;
        _eventPublisher = eventPublisher;
    }

    public async Task<ShippingResult> ExecuteAsync(OrderRequest orderRequest)
    {
        Console.WriteLine($"🎭 Saga {_orderId}: Starting execution...");
        
        try
        {
            // Step 1: Reserve Inventory
            Console.WriteLine($"   Step 1: Reserving inventory...");
            var inventoryResult = await _inventoryService.ReserveAsync(orderRequest.Items);
            _compensations.Add(() => _inventoryService.ReleaseAsync(inventoryResult.ReservationId));
            Console.WriteLine($"   ✅ Inventory reserved: {inventoryResult.ReservationId}");
            
            // Step 2: Process Payment
            Console.WriteLine($"   Step 2: Processing payment...");
            var paymentResult = await _paymentService.ChargeAsync(
                orderRequest.CustomerId,
                orderRequest.Amount
            );
            _compensations.Add(() => _paymentService.RefundAsync(paymentResult.PaymentId));
            Console.WriteLine($"   ✅ Payment processed: {paymentResult.PaymentId}");
            
            // Step 3: Schedule Shipping
            Console.WriteLine($"   Step 3: Scheduling shipping...");
            var shippingResult = await _shippingService.ScheduleAsync(orderRequest.ShippingAddress);
            // No compensation for shipping (can't un-ship)
            Console.WriteLine($"   ✅ Shipping scheduled: {shippingResult.TrackingNumber}");
            
            // Saga succeeded
            _state = "COMPLETED";
            await _eventPublisher.PublishAsync("order-events", new OrderFulfilledEvent(_orderId));
            Console.WriteLine($"   🎉 Saga {_orderId} completed successfully!");
            
            return shippingResult;
        }
        catch (Exception ex)
        {
            // Saga failed - run compensations
            Console.WriteLine($"   ❌ Saga {_orderId} failed: {ex.Message}");
            Console.WriteLine($"   🔄 Running compensations...");
            await CompensateAsync();
            _state = "FAILED";
            await _eventPublisher.PublishAsync("order-events", 
                new OrderFailedEvent(_orderId, ex.Message));
            throw;
        }
    }

    private async Task CompensateAsync()
    {
        // Run compensating transactions in reverse order
        for (int i = _compensations.Count - 1; i >= 0; i--)
        {
            try
            {
                Console.WriteLine($"   🔄 Running compensation {_compensations.Count - i}...");
                await _compensations[i]();
                Console.WriteLine($"   ✅ Compensation {_compensations.Count - i} completed");
            }
            catch (Exception ex)
            {
                // Log and continue with other compensations
                Console.WriteLine($"   ⚠️  Compensation {_compensations.Count - i} failed: {ex.Message}");
            }
        }
    }
}

// Saga Orchestrator
public class SagaOrchestrator
{
    private readonly Dictionary<string, OrderFulfillmentSaga> _activeSagas = new();
    private readonly IInventoryService _inventoryService;
    private readonly IPaymentService _paymentService;
    private readonly IShippingService _shippingService;
    private readonly IEventPublisher _eventPublisher;

    public SagaOrchestrator(
        IInventoryService inventoryService,
        IPaymentService paymentService,
        IShippingService shippingService,
        IEventPublisher eventPublisher)
    {
        _inventoryService = inventoryService;
        _paymentService = paymentService;
        _shippingService = shippingService;
        _eventPublisher = eventPublisher;
    }

    public async Task<ShippingResult> StartOrderSagaAsync(OrderRequest orderRequest)
    {
        var saga = new OrderFulfillmentSaga(
            orderRequest.OrderId,
            _inventoryService,
            _paymentService,
            _shippingService,
            _eventPublisher
        );
        
        _activeSagas[orderRequest.OrderId] = saga;
        
        try
        {
            return await saga.ExecuteAsync(orderRequest);
        }
        finally
        {
            _activeSagas.Remove(orderRequest.OrderId);
        }
    }
}

