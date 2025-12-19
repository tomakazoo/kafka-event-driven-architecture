using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Kafka;
using Shared.Kafka.Events;
using Shared.Observability;

namespace NavCalculator;

public class NavCalculatorConsumer : BackgroundService
{
    private readonly ILogger<NavCalculatorConsumer> _logger;
    private readonly ServiceMetrics _metrics;
    private readonly IConfiguration _configuration;
    private readonly string _instanceId;
    private ObservableKafkaConsumer<string, string>? _consumer;
    private ObservableKafkaProducer<string, string>? _producer;

    public NavCalculatorConsumer(
        ILogger<NavCalculatorConsumer> logger,
        ServiceMetrics metrics,
        IConfiguration configuration)
    {
        _logger = logger;
        _metrics = metrics;
        _configuration = configuration;
        _instanceId = Environment.GetEnvironmentVariable("INSTANCE_ID") ?? "nav-calculator";
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = _configuration["Kafka:BootstrapServers"],
            GroupId = "nav-calculator-group",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
            SessionTimeoutMs = 30000,
            MaxPollIntervalMs = 300000
        };

        var producerConfig = new ProducerConfig
        {
            BootstrapServers = _configuration["Kafka:BootstrapServers"],
            Acks = Acks.All,
            MessageSendMaxRetries = 3,
            EnableIdempotence = true,
            CompressionType = CompressionType.Snappy
        };

        _consumer = new ObservableKafkaConsumer<string, string>(
            consumerConfig,
            _metrics,
            TracingSetup.ActivitySource);

        _producer = new ObservableKafkaProducer<string, string>(
            producerConfig,
            _metrics,
            TracingSetup.ActivitySource);

        _consumer.Subscribe("pricing-updates");

        _logger.LogInformation(
            "🎧 [{InstanceId}] Starting to consume pricing updates...", 
            _instanceId);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _consumer.ConsumeAsync(async (result) =>
                {
                    var pricingEvent = JsonSerializer.Deserialize<PricingUpdateEvent>(
                        result.Message.Value);

                    if (pricingEvent != null)
                    {
                        await ProcessPricingUpdate(pricingEvent);
                    }
                }, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in consume loop");
                await Task.Delay(1000, stoppingToken);
            }
        }
    }

    private async Task ProcessPricingUpdate(PricingUpdateEvent pricingEvent)
    {
        using var activity = TracingSetup.ActivitySource.StartActivity("calculate_nav");
        
        activity?.SetTag("fund.id", pricingEvent.FundId);
        activity?.SetTag("correlation_id", pricingEvent.CorrelationId);
        activity?.SetTag("instance_id", _instanceId);

        try
        {
            _logger.LogInformation(
                "📊 [{InstanceId}] Calculating NAV for fund {FundId} (correlation: {CorrelationId})",
                _instanceId,
                pricingEvent.FundId,
                pricingEvent.CorrelationId);

            var calculationTime = Random.Shared.Next(100, 500);
            await Task.Delay(calculationTime);

            var totalValue = pricingEvent.Prices
                .Sum(p => p.Price * p.Quantity);
            
            var sharesOutstanding = Random.Shared.Next(100000, 1000000);
            var navPerShare = totalValue / sharesOutstanding;

            activity?.SetTag("nav.value", (double)navPerShare);
            activity?.SetTag("calculation_time_ms", calculationTime);

            var navEvent = new NavCalculatedEvent
            {
                CorrelationId = pricingEvent.CorrelationId,
                FundId = pricingEvent.FundId,
                FundName = pricingEvent.FundName,
                NavPerShare = Math.Round(navPerShare, 4),
                TotalAssets = totalValue,
                SharesOutstanding = sharesOutstanding,
                CalculatedAt = DateTime.UtcNow,
                CalculatedBy = _instanceId
            };

            var message = new Message<string, string>
            {
                Key = pricingEvent.FundId,
                Value = JsonSerializer.Serialize(navEvent)
            };

            await _producer!.ProduceAsync(
                "nav-calculated",
                message,
                pricingEvent.CorrelationId);

            _logger.LogInformation(
                "✅ [{InstanceId}] Calculated NAV for {FundId}: ${Nav:F4} (took {Ms}ms)",
                _instanceId,
                pricingEvent.FundId,
                navPerShare,
                calculationTime);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "❌ [{InstanceId}] Error calculating NAV for {FundId}",
                _instanceId,
                pricingEvent.FundId);
            throw;
        }
    }

    public override void Dispose()
    {
        _consumer?.Dispose();
        _producer?.Dispose();
        base.Dispose();
    }
}
