using System;
using System.Diagnostics;
using Prometheus;

namespace Shared.Observability;

public class ServiceMetrics
{
    private readonly string _serviceName;
    private readonly Counter _eventsProcessed;
    private readonly Histogram _processingDuration;

    public ServiceMetrics(string serviceName)
    {
        _serviceName = serviceName;
        
        _eventsProcessed = Metrics.CreateCounter(
            "events_processed_total",
            "Total events processed",
            new CounterConfiguration
            {
                LabelNames = new[] { "service", "event_type", "status" }
            });

        _processingDuration = Metrics.CreateHistogram(
            "event_processing_duration_seconds",
            "Time to process event",
            new HistogramConfiguration
            {
                LabelNames = new[] { "service", "event_type" },
                Buckets = Histogram.ExponentialBuckets(0.001, 2, 10)
            });
    }

    public IDisposable TrackEventProcessing(string eventType)
    {
        return new EventProcessingTracker(this, eventType);
    }

    public void RecordEventProcessed(string eventType, string status)
    {
        _eventsProcessed
            .WithLabels(_serviceName, eventType, status)
            .Inc();
    }

    private class EventProcessingTracker : IDisposable
    {
        private readonly ServiceMetrics _metrics;
        private readonly string _eventType;
        private readonly Stopwatch _stopwatch;

        public EventProcessingTracker(ServiceMetrics metrics, string eventType)
        {
            _metrics = metrics;
            _eventType = eventType;
            _stopwatch = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            var duration = _stopwatch.Elapsed.TotalSeconds;
            
            _metrics._processingDuration
                .WithLabels(_metrics._serviceName, _eventType)
                .Observe(duration);
        }
    }
}
