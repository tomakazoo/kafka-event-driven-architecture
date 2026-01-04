# Kafka Streams: Real-Time Stream Processing

This example demonstrates Kafka Streams concepts through simulation. Since .NET doesn't have native Kafka Streams, this shows the concepts using Confluent.Kafka.

## 📚 Concept

**Kafka Streams** is a library for building real-time stream processing applications. It provides:

- **Filtering**: Select messages based on conditions
- **Mapping**: Transform messages
- **Aggregation**: Group and compute statistics
- **Windowing**: Time-based operations
- **Stateful Operations**: Maintain state across messages
- **Joins**: Combine multiple streams

### Stream Processing Topology

```
Source Stream (orders-stream)
    ↓
Filter (only completed orders)
    ↓
Map (extract key-value pairs)
    ↓
Aggregate (by customer/product)
    ↓
Sink Streams (customer-stats, product-stats)
```

## 🏗️ Structure

```
03-kafka-streams/
├── Domain/
│   ├── Order.cs              # Order domain model
│   ├── OrderStats.cs         # Aggregated statistics
│   └── Domain.csproj
├── Streams/
│   ├── OrderProcessingTopology.cs  # Stream processing logic
│   └── Streams.csproj
├── Examples/
│   ├── OrderProducer.cs      # Produces orders
│   ├── StreamsDemo.cs        # Runs stream processing
│   └── Examples.csproj
└── README.md
```

## 🚀 How to Run

### Prerequisites
- Kafka running
- .NET 8.0 SDK

### Step 1: Start Kafka
```bash
cd /home/babicto/projects/kafka-event-driven-architecture
./scripts/start-kafka.sh
./scripts/verify-docker.sh
```

### Step 2: Run Order Producer

**Terminal 1:**
```bash
cd examples/05-advanced-kafka/dotnet/03-kafka-streams
dotnet run --project Examples/Examples.csproj
```

**Expected:** Producer sends 20 orders with various statuses.

### Step 3: Run Stream Processing

**Terminal 2:**
```bash
# Modify Examples.csproj StartupObject to StreamsDemo
dotnet run --project Examples/Examples.csproj
```

**Expected:** Stream processor filters completed orders and aggregates statistics.

## 🔍 What to Observe

1. **Filtering**: Only completed orders are processed
2. **Aggregation**: Statistics computed per customer and product
3. **Real-time**: Processing happens as orders arrive
4. **Stateful**: Statistics maintained in memory

## 📖 Key Takeaways

- **Stream Processing**: Real-time transformation of data streams
- **Filtering**: Select relevant messages
- **Aggregation**: Compute statistics over time
- **Stateful**: Maintain state across messages

## 🔗 Related Concepts

- Event-driven architecture
- Real-time analytics
- State management
- Stream processing patterns

## 💡 Production Note

In production, use:
- **Kafka Streams** (Java) for native stream processing
- **ksqlDB** for SQL-like stream processing
- **Apache Flink** for advanced stream processing
- **Confluent Cloud** for managed stream processing

This example demonstrates concepts but uses manual processing. For production, consider Kafka Streams or ksqlDB.


