using System;
using System.Collections.Generic;
using System.Linq;
using EventCarriedStateTransfer.Domain;
using EventCarriedStateTransfer.Interfaces;

namespace EventCarriedStateTransfer.Services;

// Producer - Full data in event
public class OrderService
{
    private readonly IDatabase _db;
    private readonly IEventPublisher _eventPublisher;

    public OrderService(IDatabase db, IEventPublisher eventPublisher)
    {
        _db = db;
        _eventPublisher = eventPublisher;
    }

    public void PlaceOrder(Order order)
    {
        // Save order to database
        _db.Save(order);
        
        // Publish event with ALL necessary data
        var eventObj = new
        {
            eventType = "OrderPlaced",
            eventId = Guid.NewGuid().ToString(),
            eventVersion = "1.0",
            timestamp = DateTime.UtcNow.ToString("o"),
            source = "order-service",
            correlationId = Guid.NewGuid().ToString(),
            data = new
            {
                // Identifiers
                orderId = order.Id,
                customerId = order.CustomerId,
                
                // Customer info (denormalized)
                customer = new
                {
                    id = order.Customer.Id,
                    email = order.Customer.Email,
                    name = order.Customer.Name,
                    phone = order.Customer.Phone
                },
                
                // Order details
                orderDate = order.CreatedAt.ToString("o"),
                totalAmount = (double)order.TotalAmount,
                currency = order.Currency,
                status = order.Status,
                
                // Line items (complete info)
                items = order.Items.Select(item => new
                {
                    productId = item.ProductId,
                    productName = item.ProductName,
                    productSku = item.ProductSku,
                    quantity = item.Quantity,
                    unitPrice = (double)item.UnitPrice,
                    totalPrice = (double)item.TotalPrice,
                    imageUrl = item.ProductImageUrl
                }).ToArray(),
                
                // Shipping info
                shippingAddress = new
                {
                    street = order.ShippingAddress.Street,
                    city = order.ShippingAddress.City,
                    state = order.ShippingAddress.State,
                    postalCode = order.ShippingAddress.PostalCode,
                    country = order.ShippingAddress.Country
                },
                
                // Payment info (safe subset)
                payment = new
                {
                    method = order.PaymentMethod,
                    last4 = order.PaymentLast4,
                    status = "completed"
                }
            }
        };
        
        _eventPublisher.Publish("orders", eventObj);
    }
}




