# Performance Tuning: Optimizing Kafka Producers and Consumers

This example demonstrates performance optimization techniques for Kafka producers and consumers.

## 📚 Concepts

### Producer Tuning

1. **Batching**: Group messages together for better throughput
   - `BatchSize`: Size of batches (32KB recommended)
   - `LingerMs`: Wait time to fill batches (10ms recommended)

2. **Compression**: Reduce network bandwidth
   - `Snappy`: Fast compression, good balance
   - `Gzip`: Better compression, slower
   - `Lz4`: Very fast, less compression
   - `Zstd`: Best compression, medium speed

3. **In-Flight Requests**: Allow multiple requests in parallel
   - `MaxInFlight`: Number of concurrent requests (5 recommended)

### Consumer Tuning

1. **Fetch Settings**: Optimize how much data to fetch
   - `FetchMinBytes`: Minimum bytes to wait for
   - `FetchMaxWaitMs`: Maximum wait time
   - `MaxPartitionFetchBytes`: Max bytes per partition

2. **Parallel Processing**: Process messages concurrently
   - Use `SemaphoreSlim` to limit concurrency
   - Process messages in background tasks

3. **Poll Settings**: Optimize polling behavior
   - `MaxPollRecords`: Records per poll
   - `MaxPollIntervalMs`: Max time between polls

## 🏗️ Structure

```
04-performance-tuning/
├── Producer/
│   ├── TunedProducer.cs      # Optimized producer
│   ├── BaselineProducer.cs   # Baseline for comparison
│   └── Producer.csproj
├── Consumer/
│   ├── ParallelConsumer.cs   # Parallel processing consumer
│   └── Consumer.csproj
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

### Step 2: Run Baseline Producer

```bash
cd examples/05-advanced-kafka/dotnet/04-performance-tuning
dotnet run --project Producer/Producer.csproj
# Modify StartupObject to BaselineProducer
```

**Expected:** Baseline throughput (no tuning).

### Step 3: Run Tuned Producer

```bash
# Modify StartupObject to TunedProducer
dotnet run --project Producer/Producer.csproj
```

**Expected:** Improved throughput with batching and compression.

### Step 4: Run Parallel Consumer

```bash
dotnet run --project Consumer/Consumer.csproj
```

**Expected:** High throughput with parallel processing.

## 🔍 What to Observe

1. **Batching**: Tuned producer batches messages for better throughput
2. **Compression**: Reduced network bandwidth
3. **Parallel Processing**: Consumer processes multiple messages concurrently
4. **Performance**: Compare baseline vs tuned throughput

## 📖 Key Takeaways

- **Batching**: Improves throughput by grouping messages
- **Compression**: Reduces network bandwidth
- **Parallel Processing**: Increases consumer throughput
- **Tuning**: Balance between latency and throughput

## 🔗 Related Concepts

- Producer configuration
- Consumer configuration
- Network optimization
- Throughput vs latency trade-offs


