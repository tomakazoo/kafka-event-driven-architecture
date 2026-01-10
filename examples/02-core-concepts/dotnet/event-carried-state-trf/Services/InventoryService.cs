using System;
using System.Collections.Generic;
using System.Text.Json;

namespace EventCarriedStateTransfer.Services;

public class InventoryService
{
    private readonly Dictionary<string, int> _inventory = new();

    public InventoryService()
    {
        // Initialize some inventory
        _inventory["PROD-001"] = 100;
        _inventory["PROD-002"] = 50;
        _inventory["PROD-003"] = 75;
    }

    public void HandleOrderPlaced(JsonElement eventObj)
    {
        var data = eventObj.GetProperty("data");
        var orderData = data;
        var items = orderData.GetProperty("items");
        
        // Reduce stock for each item - all data is here
        foreach (var itemElement in items.EnumerateArray())
        {
            var productId = itemElement.GetProperty("productId").GetString() ?? "";
            var quantity = itemElement.GetProperty("quantity").GetInt32();
            var orderId = orderData.GetProperty("orderId").GetString() ?? "";
            
            ReduceStock(productId, quantity, orderId);
        }
        
        Console.WriteLine($"✅ Inventory updated for order {orderData.GetProperty("orderId").GetString()}");
    }
    
    private void ReduceStock(string productId, int quantity, string orderId)
    {
        if (_inventory.ContainsKey(productId))
        {
            _inventory[productId] -= quantity;
            Console.WriteLine($"   📦 Reduced stock: {productId} by {quantity} (remaining: {_inventory[productId]})");
        }
        else
        {
            Console.WriteLine($"   ⚠️  Product {productId} not found in inventory");
        }
    }
    
    public int GetStock(string productId)
    {
        return _inventory.TryGetValue(productId, out var stock) ? stock : 0;
    }
}




