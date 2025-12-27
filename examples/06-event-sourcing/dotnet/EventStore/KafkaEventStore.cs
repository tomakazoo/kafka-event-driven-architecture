using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;
using EventSourcing.Domain;

namespace EventSourcing.EventStore;

public interface IEventPublisher
{
    Task PublishAsync(string topic, DomainEvent evt);
}

public class KafkaEventPublisher : IEventPublisher
{
    private readonly IProducer<string, string> _producer;
    private readonly string _topic;

    public KafkaEventPublisher(IProducer<string, string> producer, string topic)
    {
        _producer = producer;
        _topic = topic;
    }

    public async Task PublishAsync(string topic, DomainEvent evt)
    {
        var json = JsonSerializer.Serialize(evt, new JsonSerializerOptions 
        { 
            WriteIndented = false 
        });
        
        var message = new Message<string, string>
        {
            Key = evt.AggregateId,
            Value = json,
            Headers = new Headers
            {
                { "event-type", Encoding.UTF8.GetBytes(evt.EventType) },
                { "event-id", Encoding.UTF8.GetBytes(evt.EventId) },
                { "version", Encoding.UTF8.GetBytes(evt.Version.ToString()) }
            }
        };

        await _producer.ProduceAsync(topic, message);
    }
}

public class KafkaEventStore
{
    private readonly IProducer<string, string> _producer;
    private readonly IConsumer<string, string> _consumer;
    private readonly string _topic;
    private readonly Dictionary<string, List<DomainEvent>> _cache = new();
    private readonly Dictionary<string, Snapshot> _snapshots = new();

    public KafkaEventStore(
        IProducer<string, string> producer,
        IConsumer<string, string> consumer,
        string topic)
    {
        _producer = producer;
        _consumer = consumer;
        _topic = topic;
        
        // Subscribe to topic to build cache
        _consumer.Subscribe(topic);
    }

    public async Task<int> SaveEventsAsync(
        string aggregateId, 
        List<DomainEvent> events, 
        int expectedVersion)
    {
        // Check version (optimistic locking)
        var currentVersion = GetCurrentVersion(aggregateId);
        if (currentVersion != expectedVersion)
        {
            throw new ConcurrencyException(
                $"Expected version {expectedVersion}, but current is {currentVersion}"
            );
        }

        // Publish events to Kafka
        foreach (var evt in events)
        {
            var json = JsonSerializer.Serialize(evt, new JsonSerializerOptions 
            { 
                WriteIndented = false 
            });
            
            var message = new Message<string, string>
            {
                Key = aggregateId,
                Value = json,
                Headers = new Headers
                {
                    { "event-type", Encoding.UTF8.GetBytes(evt.EventType) },
                    { "event-id", Encoding.UTF8.GetBytes(evt.EventId) },
                    { "version", Encoding.UTF8.GetBytes(evt.Version.ToString()) }
                }
            };

            await _producer.ProduceAsync(_topic, message);
            
            // Update cache
            if (!_cache.ContainsKey(aggregateId))
                _cache[aggregateId] = new List<DomainEvent>();
            
            _cache[aggregateId].Add(evt);
        }

        _producer.Flush(TimeSpan.FromSeconds(5));
        
        return _cache.ContainsKey(aggregateId) ? _cache[aggregateId].Count : 0;
    }

    public Task<List<DomainEvent>> GetEventsAsync(
        string aggregateId, 
        int fromVersion = 0)
    {
        // Try cache first
        if (_cache.ContainsKey(aggregateId))
        {
            return Task.FromResult(_cache[aggregateId]
                .Where(e => e.Version > fromVersion)
                .OrderBy(e => e.Version)
                .ToList());
        }

        // Load from Kafka (in a real implementation, you'd use a proper event store)
        // For this example, we'll use the cache after initial load
        return Task.FromResult(new List<DomainEvent>());
    }

    public void SaveSnapshot(string aggregateId, Dictionary<string, object> snapshot, int version)
    {
        _snapshots[aggregateId] = new Snapshot
        {
            State = snapshot,
            Version = version,
            Timestamp = DateTime.UtcNow
        };
    }

    public Snapshot? GetSnapshot(string aggregateId)
    {
        return _snapshots.ContainsKey(aggregateId) ? _snapshots[aggregateId] : null;
    }

    private int GetCurrentVersion(string aggregateId)
    {
        if (!_cache.ContainsKey(aggregateId) || !_cache[aggregateId].Any())
            return 0;
        
        return _cache[aggregateId].Max(e => e.Version);
    }

    public void LoadEventsFromKafka(string aggregateId)
    {
        // In a production system, you'd read from Kafka partitions
        // For this example, we'll use the cache populated by SaveEventsAsync
        // In a real implementation, you'd:
        // 1. Read from Kafka topic partition for this aggregate
        // 2. Deserialize events
        // 3. Build cache
    }
}

public class Snapshot
{
    public Dictionary<string, object> State { get; set; } = new();
    public int Version { get; set; }
    public DateTime Timestamp { get; set; }
}

