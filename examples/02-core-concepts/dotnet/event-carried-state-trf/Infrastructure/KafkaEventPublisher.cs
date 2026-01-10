using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;
using EventCarriedStateTransfer.Interfaces;

namespace EventCarriedStateTransfer.Infrastructure;

public class KafkaEventPublisher : IEventPublisher
{
    private readonly IProducer<string, string> _producer;

    public KafkaEventPublisher(string bootstrapServers = "localhost:9092")
    {
        var config = new ProducerConfig
        {
            BootstrapServers = bootstrapServers
        };
        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task PublishAsync(string topic, object eventData)
    {
        var json = JsonSerializer.Serialize(eventData, new JsonSerializerOptions
        {
            WriteIndented = false
        });

        var message = new Message<string, string>
        {
            Key = Guid.NewGuid().ToString(),
            Value = json
        };

        await _producer.ProduceAsync(topic, message);
        Console.WriteLine($"📤 Event published to topic '{topic}'");
    }

    public void Publish(string topic, object eventData)
    {
        PublishAsync(topic, eventData).GetAwaiter().GetResult();
    }

    public void Dispose()
    {
        _producer?.Dispose();
    }
}




