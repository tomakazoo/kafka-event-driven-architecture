using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using System.Text.Json;

class BasicProducer
{
    static async Task Main()
    {
        Console.WriteLine("🚀 Starting Kafka Producer...");
        Console.WriteLine($"📡 Connecting to: localhost:9092");
        
        var config = new ProducerConfig 
        { 
            BootstrapServers = "localhost:9092",
            // Timeout settings to prevent hanging
            RequestTimeoutMs = 5000,           // 5 seconds timeout for requests
            MessageTimeoutMs = 5000,           // 5 seconds timeout for message delivery
            SocketTimeoutMs = 5000              // 5 seconds socket timeout
        };
        
        using (var producer = new ProducerBuilder<string, string>(config)
            .SetErrorHandler((p, e) => 
            {
                Console.WriteLine($"❌ Producer Error: {e.Reason}");
            })
            .Build())
        {
            Console.WriteLine("✅ Producer created successfully");
            var topic = "my-topic";
            Console.WriteLine($"📤 Producing to topic: {topic}");
            
            try
            {
                for (int i = 0; i < 5; i++)
                {
                    Console.WriteLine($"📨 Sending message {i}...");
                    var message = new { id = i, value = $"Message {i}" };
                    
                    try
                    {
                        var report = await producer.ProduceAsync(
                            topic,
                            new Message<string, string>
                            {
                                Key = $"key-{i}",
                                Value = JsonSerializer.Serialize(message)
                            });
                        Console.WriteLine($"✅ Delivered to {report.TopicPartitionOffset}");
                    }
                    catch (ProduceException<string, string> e)
                    {
                        Console.WriteLine($"❌ Failed to deliver message {i}: {e.Error.Reason}");
                        Console.WriteLine($"   Error Code: {e.Error.Code}");
                        throw;
                    }
                }
                
                // Flush with timeout to prevent hanging
                Console.WriteLine("🔄 Flushing producer...");
                producer.Flush(TimeSpan.FromSeconds(5));
                Console.WriteLine("✅ All messages delivered successfully!");
            }
            catch (ProduceException<string, string> e)
            {
                Console.WriteLine($"❌ Failed to deliver message: {e.Error.Reason}");
                Console.WriteLine($"   Error Code: {e.Error.Code}");
                throw;
            }
            catch (KafkaException e)
            {
                Console.WriteLine($"❌ Kafka error: {e.Message}");
                if (e.InnerException != null)
                {
                    Console.WriteLine($"   Inner Exception: {e.InnerException.Message}");
                }
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine($"❌ Unexpected error: {e.Message}");
                throw;
            }
        }
    }
}

