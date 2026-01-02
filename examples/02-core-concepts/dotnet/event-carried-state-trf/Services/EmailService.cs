using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace EventCarriedStateTransfer.Services;

// Consumer - Fully autonomous
public class EmailService
{
    public void HandleOrderPlaced(JsonElement eventObj)
    {
        var data = eventObj.GetProperty("data");
        var orderData = data;
        
        // All data is in the event - NO API call needed!
        var customer = orderData.GetProperty("customer");
        var items = orderData.GetProperty("items");
        var shippingAddress = orderData.GetProperty("shippingAddress");
        
        var emailContent = RenderEmailTemplate(
            template: "order_confirmation",
            customerName: customer.GetProperty("name").GetString() ?? "",
            orderId: orderData.GetProperty("orderId").GetString() ?? "",
            items: items.EnumerateArray().ToList(),
            total: orderData.GetProperty("totalAmount").GetDouble(),
            shippingAddress: shippingAddress
        );
        
        SendEmail(
            to: customer.GetProperty("email").GetString() ?? "",
            subject: $"Order Confirmation - {orderData.GetProperty("orderId").GetString()}",
            content: emailContent
        );
        
        // Service is fully autonomous!
    }
    
    private string RenderEmailTemplate(string template, string customerName, string orderId, List<JsonElement> items, double total, JsonElement shippingAddress)
    {
        var itemsList = string.Join("\n", items.Select(item => 
            $"  - {item.GetProperty("productName").GetString()} x{item.GetProperty("quantity").GetInt32()} = ${item.GetProperty("totalPrice").GetDouble():F2}"));
        
        return $@"
========================================
ORDER CONFIRMATION EMAIL
========================================
Template: {template}

Dear {customerName},

Thank you for your order!

Order ID: {orderId}
Total Amount: ${total:F2}

Items:
{itemsList}

Shipping Address:
{shippingAddress.GetProperty("street").GetString()}
{shippingAddress.GetProperty("city").GetString()}, {shippingAddress.GetProperty("state").GetString()} {shippingAddress.GetProperty("postalCode").GetString()}
{shippingAddress.GetProperty("country").GetString()}

We'll send you a tracking number once your order ships.

Best regards,
The Store Team
========================================
";
    }
    
    private void SendEmail(string to, string subject, string content)
    {
        Console.WriteLine($"📧 EMAIL SENT");
        Console.WriteLine($"   To: {to}");
        Console.WriteLine($"   Subject: {subject}");
        Console.WriteLine($"   Content:\n{content}");
    }
}


