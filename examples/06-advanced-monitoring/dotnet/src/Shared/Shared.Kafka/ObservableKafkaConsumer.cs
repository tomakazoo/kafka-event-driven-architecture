using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Shared.Observability;

namespace Shared.Kafka;

public class ObservableKafkaConsumer<TKey, TValue> : IDisposable
{
    private readonly IConsumer<TKey, TValue> _consumer;
    private readonly ServiceMetrics _metrics;
    private readonly ActivitySource _activitySource;
    private readonly string _consumerGroup;

    public ObservableKafkaConsumer(
        ConsumerConfig config,
        ServiceMetrics metrics,
        ActivitySource activitySource)
    {
        _consumer = new ConsumerBuilder<TKey, TValue>(config).Build();
        _metrics = metrics;
        _activitySource = activitySource;
        _consumerGroup = config.GroupId ?? "unknown";
    }

    public void Subscribe(string topic)
    {
        _consumer.Subscribe(topic);
    }

    public async Task<ProcessingResult> ConsumeAsync(
        Func<ConsumeResult<TKey, TValue>, Task> processFunc,
        CancellationToken cancellationToken)
    {
        var consumeResult = _consumer.Consume(cancellationToken);
        
        var correlationId = ExtractHeader(consumeResult.Message, "correlation_id");
        var traceParent = ExtractHeader(consumeResult.Message, "traceparent");

        var parentContext = traceParent != null 
            ? ActivityContext.Parse(traceParent, null)
            : default(ActivityContext);

        using var activity = _activitySource.StartActivity(
            "process_event",
            ActivityKind.Consumer,
            parentContext);

        activity?.SetTag("messaging.system", "kafka");
        activity?.SetTag("messaging.source", consumeResult.Topic);
        activity?.SetTag("correlation_id", correlationId);

        using var tracker = _metrics.TrackEventProcessing(consumeResult.Topic);

        try
        {
            await processFunc(consumeResult);
            
            _consumer.Commit(consumeResult);
            _metrics.RecordEventProcessed(consumeResult.Topic, "success");
            
            activity?.SetStatus(ActivityStatusCode.Ok);
            
            return ProcessingResult.Success;
        }
        catch (Exception ex)
        {
            _metrics.RecordEventProcessed(consumeResult.Topic, "error");
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            
            return ProcessingResult.CreateError(ex);
        }
    }

    private string? ExtractHeader(Message<TKey, TValue> message, string headerName)
    {
        var header = message.Headers?.FirstOrDefault(h => h.Key == headerName);
        return header != null ? Encoding.UTF8.GetString(header.GetValueBytes()) : null;
    }

    public void Dispose()
    {
        _consumer?.Close();
        _consumer?.Dispose();
    }
}

public record ProcessingResult(bool IsSuccess, Exception? Error = null)
{
    public static ProcessingResult Success => new(true);
    public static ProcessingResult CreateError(Exception ex) => new(false, ex);
}
