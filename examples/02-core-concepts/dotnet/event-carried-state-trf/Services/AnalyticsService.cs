using System;
using System.Collections.Generic;
using System.Text.Json;

namespace EventCarriedStateTransfer.Services;

public class AnalyticsService
{
    private readonly List<Dictionary<string, object>> _metrics = new();

    public void HandleOrderPlaced(JsonElement eventObj)
    {
        var data = eventObj.GetProperty("data");
        var orderData = data;
        var items = orderData.GetProperty("items");
        var shippingAddress = orderData.GetProperty("shippingAddress");
        
        // Record comprehensive analytics - all data available
        var metrics = new Dictionary<string, object>
        {
            ["event_type"] = "order_placed",
            ["order_id"] = orderData.GetProperty("orderId").GetString() ?? "",
            ["customer_id"] = orderData.GetProperty("customerId").GetString() ?? "",
            ["amount"] = orderData.GetProperty("totalAmount").GetDouble(),
            ["currency"] = orderData.GetProperty("currency").GetString() ?? "",
            ["item_count"] = items.GetArrayLength(),
            ["country"] = shippingAddress.GetProperty("country").GetString() ?? "",
            ["timestamp"] = eventObj.GetProperty("timestamp").GetString() ?? ""
        };
        
        RecordMetrics(metrics);
    }
    
    private void RecordMetrics(Dictionary<string, object> metrics)
    {
        _metrics.Add(metrics);
        Console.WriteLine($"📊 ANALYTICS RECORDED:");
        Console.WriteLine($"   Event Type: {metrics["event_type"]}");
        Console.WriteLine($"   Order ID: {metrics["order_id"]}");
        Console.WriteLine($"   Customer ID: {metrics["customer_id"]}");
        Console.WriteLine($"   Amount: {metrics["currency"]} {metrics["amount"]}");
        Console.WriteLine($"   Item Count: {metrics["item_count"]}");
        Console.WriteLine($"   Country: {metrics["country"]}");
    }
    
    public int GetTotalOrders()
    {
        return _metrics.Count;
    }
}




