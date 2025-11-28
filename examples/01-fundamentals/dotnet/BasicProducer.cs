using Confluent.Kafka;
using System.Text.Json;

class BasicProducer
{
    static async Task Main()
    {
        var config = new ProducerConfig { BootstrapServers = "localhost:9092" };
        
        using (var producer = new ProducerBuilder<string, string>(config).Build())
        {
            var topic = "my-topic";
            
            for (int i = 0; i < 5; i++)
            {
                var message = new { id = i, value = $"Message {i}" };
                var report = await producer.ProduceAsync(
                    topic,
                    new Message<string, string>
                    {
                        Key = $"key-{i}",
                        Value = JsonSerializer.Serialize(message)
                    });
                Console.WriteLine($"Delivered to {report.TopicPartitionOffset}");
            }
            
            producer.Flush();
        }
    }
}

