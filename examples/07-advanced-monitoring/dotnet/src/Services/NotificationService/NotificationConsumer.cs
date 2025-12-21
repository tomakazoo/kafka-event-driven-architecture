using System;
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

namespace NotificationService;

public class NotificationConsumer : BackgroundService
{
    private readonly ILogger<NotificationConsumer> _logger;
    private readonly ServiceMetrics _metrics;
    private readonly IConfiguration _configuration;
    private ObservableKafkaConsumer<string, string>? _consumer;

    public NotificationConsumer(
        ILogger<NotificationConsumer> logger,
        ServiceMetrics metrics,
        IConfiguration configuration)
    {
        _logger = logger;
        _metrics = metrics;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = _configuration["Kafka:BootstrapServers"],
            GroupId = "notification-service-group",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
            SessionTimeoutMs = 30000,
            MaxPollIntervalMs = 300000
        };

        _consumer = new ObservableKafkaConsumer<string, string>(
            consumerConfig,
            _metrics,
            TracingSetup.ActivitySource);

        _consumer.Subscribe("nav-calculated");

        _logger.LogInformation("🔔 Starting to consume NAV calculated events...");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _consumer.ConsumeAsync(async (result) =>
                {
                    var navEvent = JsonSerializer.Deserialize<NavCalculatedEvent>(
                        result.Message.Value);

                    if (navEvent != null)
                    {
                        await ProcessNavCalculatedEvent(navEvent);
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

    private async Task ProcessNavCalculatedEvent(NavCalculatedEvent navEvent)
    {
        using var activity = TracingSetup.ActivitySource.StartActivity("send_notification");
        
        activity?.SetTag("fund.id", navEvent.FundId);
        activity?.SetTag("correlation_id", navEvent.CorrelationId);

        try
        {
            _logger.LogInformation(
                "📧 Sending notification for fund {FundId}: NAV = ${Nav:F4} (correlation: {CorrelationId}, calculated by: {CalculatedBy})",
                navEvent.FundId,
                navEvent.NavPerShare,
                navEvent.CorrelationId,
                navEvent.CalculatedBy);

            await Task.Delay(Random.Shared.Next(50, 200));

            _logger.LogInformation(
                "✅ Notification sent for fund {FundId}",
                navEvent.FundId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "❌ Error sending notification for {FundId}",
                navEvent.FundId);
            throw;
        }
    }

    public override void Dispose()
    {
        _consumer?.Dispose();
        base.Dispose();
    }
}
