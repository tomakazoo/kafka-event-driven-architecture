using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared.Kafka;
using Shared.Kafka.Events;
using Shared.Observability;

namespace PricingService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PricingController : ControllerBase
{
    private readonly ILogger<PricingController> _logger;
    private readonly PricingEventProducer _producer;

    public PricingController(
        ILogger<PricingController> logger,
        PricingEventProducer producer)
    {
        _logger = logger;
        _producer = producer;
    }

    [HttpPost("trigger")]
    public async Task<IActionResult> TriggerPricing([FromBody] TriggerPricingRequest request)
    {
        using var activity = TracingSetup.ActivitySource.StartActivity("trigger_pricing");
        
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString() 
            ?? Guid.NewGuid().ToString();

        activity?.SetTag("correlation_id", correlationId);
        activity?.SetTag("fund.id", request.FundId);

        _logger.LogInformation(
            "📈 Triggering pricing for fund {FundId} (correlation: {CorrelationId})",
            request.FundId,
            correlationId);

        try
        {
            var prices = GenerateMockPrices();

            var pricingEvent = new PricingUpdateEvent
            {
                CorrelationId = correlationId,
                FundId = request.FundId,
                FundName = request.FundName ?? $"Fund {request.FundId}",
                Prices = prices,
                Timestamp = DateTime.UtcNow
            };

            await _producer.PublishPricingUpdateAsync(pricingEvent);

            return Ok(new
            {
                correlationId,
                fundId = request.FundId,
                fundName = pricingEvent.FundName,
                priceCount = prices.Count,
                timestamp = pricingEvent.Timestamp
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "❌ Error triggering pricing for {FundId}", 
                request.FundId);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    private List<PriceData> GenerateMockPrices()
    {
        var random = new Random();
        var securityCount = random.Next(5, 15);
        var prices = new List<PriceData>();

        for (int i = 0; i < securityCount; i++)
        {
            prices.Add(new PriceData
            {
                SecurityId = $"SEC-{i + 1:D3}",
                Price = (decimal)(random.NextDouble() * 1000 + 10),
                Quantity = random.Next(100, 10000)
            });
        }

        return prices;
    }
}

public record TriggerPricingRequest
{
    public string FundId { get; init; } = string.Empty;
    public string? FundName { get; init; }
}

public class PricingEventProducer
{
    private readonly ObservableKafkaProducer<string, string> _producer;
    private readonly ServiceMetrics _metrics;
    private readonly ILogger<PricingEventProducer> _logger;

    public PricingEventProducer(
        IConfiguration configuration,
        ServiceMetrics metrics,
        ILogger<PricingEventProducer> logger)
    {
        _metrics = metrics;
        _logger = logger;

        var config = new ProducerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"],
            Acks = Acks.All,
            MessageSendMaxRetries = 3,
            EnableIdempotence = true,
            CompressionType = CompressionType.Snappy
        };

        _producer = new ObservableKafkaProducer<string, string>(
            config,
            metrics,
            TracingSetup.ActivitySource);
    }

    public async Task PublishPricingUpdateAsync(PricingUpdateEvent pricingEvent)
    {
        var message = new Message<string, string>
        {
            Key = pricingEvent.FundId,
            Value = JsonSerializer.Serialize(pricingEvent)
        };

        var result = await _producer.ProduceAsync(
            "pricing-updates",
            message,
            pricingEvent.CorrelationId);

        _logger.LogInformation(
            "✅ Published pricing update for {FundId} to partition {Partition} at offset {Offset}",
            pricingEvent.FundId,
            result.Partition.Value,
            result.Offset.Value);
    }
}
