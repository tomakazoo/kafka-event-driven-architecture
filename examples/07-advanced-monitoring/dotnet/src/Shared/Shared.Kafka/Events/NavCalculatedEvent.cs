using System;

namespace Shared.Kafka.Events;

public record NavCalculatedEvent
{
    public string EventType { get; init; } = "NAVCalculated";
    public string CorrelationId { get; init; } = string.Empty;
    public string FundId { get; init; } = string.Empty;
    public string FundName { get; init; } = string.Empty;
    public decimal NavPerShare { get; init; }
    public decimal TotalAssets { get; init; }
    public int SharesOutstanding { get; init; }
    public DateTime CalculatedAt { get; init; } = DateTime.UtcNow;
    public string CalculatedBy { get; init; } = string.Empty;
}
