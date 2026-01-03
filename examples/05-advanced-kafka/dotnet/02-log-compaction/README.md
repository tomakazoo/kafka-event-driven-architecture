# Log Compaction: State Management with Compacted Topics

This example demonstrates Kafka's log compaction feature, which keeps only the latest state per key.

## 📚 Concept

**Log Compaction** is a retention mechanism that keeps only the latest value for each key in a topic. Older updates are removed, but the latest state is preserved indefinitely.

### How It Works

1. **Same Key**: All messages for the same key are stored
2. **Compaction**: Periodically, Kafka removes older values, keeping only the latest
3. **Result**: The log becomes a compacted state store

### Use Cases

- **User Profiles**: Latest user state per user ID
- **Configuration**: Latest config per configuration key
- **State Stores**: Latest state per entity ID
- **Change Logs**: Current state of entities

## 🏗️ Structure

```
02-log-compaction/
├── Domain/
│   ├── User.cs              # User aggregate with key
│   └── Domain.csproj
├── Producer/
│   ├── UserStateProducer.cs # Produces user updates with same key
│   └── Producer.csproj
├── Consumer/
│   ├── UserStateConsumer.cs # Reads compacted log
│   └── Consumer.csproj
├── Examples/
│   ├── CompactionDemo.cs
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

### Step 2: Run Producer

```bash
cd examples/05-advanced-kafka/dotnet/02-log-compaction
dotnet run --project Producer/Producer.csproj
```

**Expected:** Producer sends 5 updates for the same user key.

### Step 3: Run Consumer

```bash
dotnet run --project Consumer/Consumer.csproj
```

**Expected:** Consumer reads all messages (including historical).

### Step 4: Observe Compaction

1. Check Kafka UI: http://localhost:8080
2. Navigate to Topics → `user-state-compacted`
3. After compaction runs, older versions are removed
4. Only the latest version (v5) remains

## 🔍 What to Observe

1. **Before Compaction**: All 5 versions are visible
2. **After Compaction**: Only latest version remains
3. **Key Behavior**: Same key = same partition = compaction applies

## ⚙️ Configuration

The topic is created with:
- `cleanup.policy=compact` - Enable compaction
- `min.cleanable.dirty.ratio=0.5` - Compact when 50% dirty
- `segment.ms=10000` - Create new segment every 10 seconds

## 📖 Key Takeaways

- **Log Compaction**: Keeps latest state per key
- **Use Cases**: User profiles, configuration, state stores
- **Trade-off**: Storage efficiency vs. historical data

## 🔗 Related Concepts

- Topic configuration
- Retention policies
- State management
- Change data capture (CDC)

