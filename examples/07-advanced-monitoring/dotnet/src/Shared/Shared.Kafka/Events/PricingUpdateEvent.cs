using System;
using System.Collections.Generic;

namespace Shared.Kafka.Events;

public record PricingUpdateEvent
{
    public string EventType { get; init; } = "PricingUpdate";
    public string CorrelationId { get; init; } = string.Empty;
    public string FundId { get; init; } = string.Empty;
    public string FundName { get; init; } = string.Empty;
    public List<PriceData> Prices { get; init; } = new();
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}

public record PriceData
{
    public string SecurityId { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int Quantity { get; init; }
}
