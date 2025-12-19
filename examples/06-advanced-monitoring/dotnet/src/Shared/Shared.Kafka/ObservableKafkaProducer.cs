using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using Shared.Observability;

namespace Shared.Kafka;

public class ObservableKafkaProducer<TKey, TValue> : IDisposable
{
    private readonly IProducer<TKey, TValue> _producer;
    private readonly ServiceMetrics _metrics;
    private readonly ActivitySource _activitySource;

    public ObservableKafkaProducer(
        ProducerConfig config,
        ServiceMetrics metrics,
        ActivitySource activitySource)
    {
        _producer = new ProducerBuilder<TKey, TValue>(config).Build();
        _metrics = metrics;
        _activitySource = activitySource;
    }

    public async Task<DeliveryResult<TKey, TValue>> ProduceAsync(
        string topic,
        Message<TKey, TValue> message,
        string? correlationId = null)
    {
        using var activity = _activitySource.StartActivity(
            "produce_event",
            ActivityKind.Producer);

        activity?.SetTag("messaging.system", "kafka");
        activity?.SetTag("messaging.destination", topic);
        activity?.SetTag("correlation_id", correlationId ?? Guid.NewGuid().ToString());

        try
        {
            message.Headers ??= new Headers();
            
            if (correlationId != null)
            {
                message.Headers.Add("correlation_id", 
                    Encoding.UTF8.GetBytes(correlationId));
            }

            if (activity != null)
            {
                message.Headers.Add("traceparent",
                    Encoding.UTF8.GetBytes(activity.Id ?? ""));
            }

            var result = await _producer.ProduceAsync(topic, message);
            
            activity?.SetTag("messaging.kafka.partition", result.Partition.Value);
            activity?.SetTag("messaging.kafka.offset", result.Offset.Value);
            
            return result;
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
    }

    public void Dispose()
    {
        _producer?.Dispose();
    }
}
