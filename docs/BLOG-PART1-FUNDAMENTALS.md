# Part 1 Blog Content: Try It Yourself - Your First Event-Driven Application

**Location in blog:** After "When NOT to Use EDA" section, before "Getting Started: A Practical Roadmap"

**Word count:** ~800 words + 3-4 screenshots

---

## Try It Yourself: Your First Event-Driven Application

Now that you understand the theory, let's see Event-Driven Architecture in action. We'll build a simple producer-consumer application that demonstrates the core concepts we've discussed.

### What We'll Build

A simple message producer that sends events to Kafka, and a consumer that reads them. This demonstrates:

- **Event Production** - Publishing events without knowing who consumes them
- **Event Consumption** - Reacting to events independently
- **Decoupling** - Producer and consumer don't know about each other
- **Asynchronous Communication** - Messages flow through Kafka broker

This is the simplest possible EDA example, perfect for understanding the fundamentals.

### Prerequisites

Before we start, make sure you have:

- **Docker** installed (for running Kafka locally)
- **.NET 8.0 SDK**
- **Git** (to clone the repository)

If you don't have Docker yet, don't worry - we'll guide you through the setup.

### Step 1: Quick Setup

First, let's get Kafka running locally. We'll use Docker Compose to spin up a complete Kafka environment in minutes.

```bash
# Clone the repository
git clone https://github.com/your-username/kafka-event-driven-architecture.git
cd kafka-event-driven-architecture

# Start Kafka infrastructure
./scripts/start-kafka.sh

# Verify everything is running
./scripts/verify-docker.sh
```

**What just happened?**

Docker Compose started four services:
- **Zookeeper** - Coordinates the Kafka cluster
- **Kafka Broker** - The event broker (port 9092)
- **Schema Registry** - Manages data schemas
- **Kafka UI** - Web interface at http://localhost:8080

Your Kafka cluster is now running locally! 🎉

### Step 2: Create a Producer

A producer publishes events to Kafka. Let's create a simple one in C#:

```csharp
using Confluent.Kafka;
using System.Text.Json;

class BasicProducer
{
    static async Task Main()
    {
        var config = new ProducerConfig
        {
            BootstrapServers = "localhost:9092"
        };

        using var producer = new ProducerBuilder<string, string>(config).Build();
        var topic = "my-topic";

        for (int i = 0; i < 5; i++)
        {
            var message = new { id = i, value = $"Message {i}" };
            var result = await producer.ProduceAsync(
                topic,
                new Message<string, string>
                {
                    Key = $"key-{i}",
                    Value = JsonSerializer.Serialize(message)
                });
            
            Console.WriteLine($"✅ Delivered to {result.TopicPartitionOffset}");
        }
        
        producer.Flush(TimeSpan.FromSeconds(5));
    }
}
```

**What this code does:**

1. Creates a producer connected to `localhost:9092`
2. Publishes 5 messages to topic `my-topic`
3. Each message has a key and JSON value
4. Kafka auto-creates the topic when the first message arrives

**Run it:**

```bash
cd examples/01-fundamentals/dotnet
dotnet run --project BasicProducer.csproj
```

**Expected output:**

```
🚀 Starting Kafka Producer...
📡 Connecting to: localhost:9092
✅ Producer created successfully
📤 Producing to topic: my-topic
✅ Delivered to my-topic [[0]] @0
✅ Delivered to my-topic [[0]] @1
✅ Delivered to my-topic [[0]] @2
✅ Delivered to my-topic [[0]] @3
✅ Delivered to my-topic [[0]] @4
✅ All messages delivered successfully!
```

**What happened?**

- The producer sent 5 messages to Kafka
- Kafka stored them in partition 0 of `my-topic`
- Each message got an offset (0, 1, 2, 3, 4)
- The messages are now **persisted** in Kafka, waiting to be consumed

![Producer Success](images/producer-success.png)
*Producer successfully delivering messages to Kafka*

### Step 3: Create a Consumer

Now let's create a consumer that reads these messages:

```csharp
using Confluent.Kafka;

class BasicConsumer
{
    static void Main()
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = "dotnet-consumer-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<string, string>(config).Build();
        consumer.Subscribe("my-topic");

        var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            cts.Cancel();
        };

        try
        {
            while (!cts.Token.IsCancellationRequested)
            {
                var result = consumer.Consume(cts.Token);
                Console.WriteLine($"Received: {result.Message.Value}");
            }
        }
        finally
        {
            consumer.Close();
        }
    }
}
```

**What this code does:**

1. Creates a consumer in group `dotnet-consumer-group`
2. Subscribes to `my-topic`
3. Reads from the **earliest** offset (gets all messages)
4. Continuously polls for new messages
5. Prints each message to console

**Run it:**

```bash
# In a new terminal
cd examples/01-fundamentals/dotnet
dotnet run --project BasicConsumer.csproj
```

**Expected output:**

```
Received: {"id":0,"value":"Message 0"}
Received: {"id":1,"value":"Message 1"}
Received: {"id":2,"value":"Message 2"}
Received: {"id":3,"value":"Message 3"}
Received: {"id":4,"value":"Message 4"}
(waiting for more messages...)
```

**What happened?**

- The consumer read all 5 messages we produced earlier
- Messages were delivered in order
- The consumer is now waiting for new messages
- Press Ctrl+C to stop it

![Consumer Success](images/consumer-success.png)
*Consumer successfully receiving messages from Kafka*

### Step 4: View Events in Kafka UI

Kafka UI provides a visual interface to explore your events. Open **http://localhost:8080** in your browser.

**Navigate to:** Topics → my-topic → Messages

![Kafka UI Messages](images/kafka-ui-messages.png)
*Viewing messages in Kafka UI - showing all messages with keys, values, and timestamps*

**What you can see:**

- **All messages** with their keys and values
- **JSON formatted** nicely for readability
- **Timestamps** showing when each message was produced
- **Partition and offset** information
- **Message metadata** (headers, size, etc.)

**Try this:**

1. Keep the consumer running
2. Run the producer again in another terminal
3. Watch the consumer **immediately** display the new messages
4. See the new messages appear in Kafka UI

This demonstrates Kafka's **real-time streaming** capability!

### What You Just Learned

Congratulations! You've just built your first event-driven application. Here's what happened:

✅ **Events are immutable** - Once published, they're stored permanently in Kafka

✅ **Producers don't know consumers** - The producer just publishes events. It doesn't know who (or if anyone) is listening.

✅ **Consumers react independently** - The consumer reads events at its own pace, independently of the producer.

✅ **Decoupling through events** - Producer and consumer are completely decoupled. They only know about Kafka, not each other.

✅ **Asynchronous by default** - Messages flow through Kafka asynchronously. The producer doesn't wait for consumers.

✅ **Events persist** - Messages are stored on disk. Consumers can re-read them, and new consumers can read historical events.

This simple example demonstrates the core principles of Event-Driven Architecture. In Part 4, we'll build a complete microservices system with multiple services communicating through events.

### Next Steps

You've seen EDA in action! Now you're ready to:

- **Part 2:** Learn how to design events properly (event patterns, schema evolution, granularity)
- **Part 3:** Deep dive into Apache Kafka (topics, partitions, consumer groups, how Kafka works internally)
- **Part 4:** Build a complete e-commerce order system with multiple microservices

**Full code and detailed guide:**  
[GitHub Repository](https://github.com/your-username/kafka-event-driven-architecture) → `examples/01-fundamentals/`

**Complete setup guide:**  
[QUICKSTART.md](https://github.com/your-username/kafka-event-driven-architecture/blob/main/docs/QUICKSTART.md)

---

**Key Takeaway:** Event-Driven Architecture isn't just theory - it's a practical pattern you can use today. This simple producer-consumer example demonstrates the core concepts: decoupling, asynchronous communication, and event persistence. In the next parts, we'll explore event design, Kafka internals, and build real-world systems.

