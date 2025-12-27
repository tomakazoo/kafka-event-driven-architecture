using System;
using System.Collections.Generic;

namespace SagaPattern.Shared.Domain;

public class Order
{
    public string OrderId { get; set; }
    public string CustomerId { get; set; }
    public List<OrderItem> Items { get; set; }
    public decimal TotalAmount { get; set; }
    public ShippingAddress ShippingAddress { get; set; }
    public string Status { get; set; }

    public Order(string orderId, string customerId, List<OrderItem> items, decimal totalAmount, ShippingAddress shippingAddress)
    {
        OrderId = orderId;
        CustomerId = customerId;
        Items = items;
        TotalAmount = totalAmount;
        ShippingAddress = shippingAddress;
        Status = "PENDING";
    }
}

public class OrderItem
{
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }

    public OrderItem(string productId, string productName, int quantity, decimal price)
    {
        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        Price = price;
    }
}

public class ShippingAddress
{
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string ZipCode { get; set; }

    public ShippingAddress(string street, string city, string state, string zipCode)
    {
        Street = street;
        City = city;
        State = state;
        ZipCode = zipCode;
    }
}

public class OrderRequest
{
    public string OrderId { get; set; }
    public string CustomerId { get; set; }
    public List<OrderItem> Items { get; set; }
    public decimal Amount { get; set; }
    public ShippingAddress ShippingAddress { get; set; }

    public OrderRequest(string orderId, string customerId, List<OrderItem> items, decimal amount, ShippingAddress shippingAddress)
    {
        OrderId = orderId;
        CustomerId = customerId;
        Items = items;
        Amount = amount;
        ShippingAddress = shippingAddress;
    }
}

