using System;
using System.Collections.Generic;

namespace EventCarriedStateTransfer.Domain;

public class Order
{
    public string Id { get; set; }
    public string CustomerId { get; set; }
    public Customer Customer { get; set; }
    public DateTime CreatedAt { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; }
    public string Status { get; set; }
    public List<OrderItem> Items { get; set; }
    public ShippingAddress ShippingAddress { get; set; }
    public string PaymentMethod { get; set; }
    public string PaymentLast4 { get; set; }

    public Order(
        string id,
        string customerId,
        Customer customer,
        DateTime createdAt,
        decimal totalAmount,
        string currency,
        string status,
        List<OrderItem> items,
        ShippingAddress shippingAddress,
        string paymentMethod,
        string paymentLast4)
    {
        Id = id;
        CustomerId = customerId;
        Customer = customer;
        CreatedAt = createdAt;
        TotalAmount = totalAmount;
        Currency = currency;
        Status = status;
        Items = items;
        ShippingAddress = shippingAddress;
        PaymentMethod = paymentMethod;
        PaymentLast4 = paymentLast4;
    }
}

public class Customer
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }

    public Customer(string id, string email, string name, string phone)
    {
        Id = id;
        Email = email;
        Name = name;
        Phone = phone;
    }
}

public class OrderItem
{
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public string ProductSku { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public string ProductImageUrl { get; set; }

    public OrderItem(
        string productId,
        string productName,
        string productSku,
        int quantity,
        decimal unitPrice,
        decimal totalPrice,
        string productImageUrl)
    {
        ProductId = productId;
        ProductName = productName;
        ProductSku = productSku;
        Quantity = quantity;
        UnitPrice = unitPrice;
        TotalPrice = totalPrice;
        ProductImageUrl = productImageUrl;
    }
}

public class ShippingAddress
{
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }

    public ShippingAddress(string street, string city, string state, string postalCode, string country)
    {
        Street = street;
        City = city;
        State = state;
        PostalCode = postalCode;
        Country = country;
    }
}




