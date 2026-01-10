using System;
using System.Text.Json;

namespace KafkaStreams.Domain
{
    /// <summary>
    /// Order domain model for stream processing
    /// </summary>
    public class Order
    {
        public string OrderId { get; set; }
        public string CustomerId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public DateTime Timestamp { get; set; }
        public string ProductId { get; set; }
        public int Quantity { get; set; }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }

        public static Order FromJson(string json)
        {
            return JsonSerializer.Deserialize<Order>(json) ?? new Order();
        }
    }

    /// <summary>
    /// Order statistics aggregated from stream
    /// </summary>
    public class OrderStats
    {
        public string Key { get; set; } // CustomerId or ProductId
        public int Count { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AverageAmount { get; set; }
        public DateTime LastUpdated { get; set; }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }

        public static OrderStats FromJson(string json)
        {
            return JsonSerializer.Deserialize<OrderStats>(json) ?? new OrderStats();
        }
    }
}



