# How to Run: Performance Tuning Examples

Step-by-step guide to running performance tuning examples.

## Prerequisites

- .NET 8.0 SDK installed
- Docker and Docker Compose installed
- Kafka running (see setup below)

## Step 1: Start Kafka

Make sure Kafka is running:

```bash
cd /home/babicto/projects/kafka-event-driven-architecture
./scripts/start-kafka.sh
```

Wait for Kafka to be ready:

```bash
./scripts/verify-docker.sh
```

**Expected:** All Kafka services running and accessible.

## Step 2: Build the Solution

```bash
cd examples/05-advanced-kafka/dotnet/04-performance-tuning
dotnet build
```

**Expected:** Build succeeds with no errors.

## Step 3: Run Baseline Producer

**Modify `Producer.csproj` to use `BaselineProducer`:**

```xml
<StartupObject>PerformanceTuning.Producer.BaselineProducer</StartupObject>
```

Then run:

```bash
dotnet run --project Producer/Producer.csproj
```

**Expected Output:**
```
🐌 Baseline Producer (No Tuning)
📡 Connecting to: localhost:9092

✅ Producer created (baseline - no tuning):
   Batch Size: 16384 bytes
   Linger: 0 ms
   Compression: None

📤 Producing 1000 messages to: performance-topic

📨 Queued 100 messages...
📨 Queued 200 messages...
...

═══════════════════════════════════════════════════════
📊 Performance Results (Baseline)
═══════════════════════════════════════════════════════
Total messages: 1000
Delivered: 1000
Errors: 0
Time: 5234 ms
Throughput: 191.05 msg/s

💡 Compare with TunedProducer to see performance improvement!
```

**What happens:**
- Producer sends messages without batching optimization
- No compression
- Baseline performance measurement

## Step 4: Run Tuned Producer

**Modify `Producer.csproj` to use `TunedProducer`:**

```xml
<StartupObject>PerformanceTuning.Producer.TunedProducer</StartupObject>
```

Then run:

```bash
dotnet run --project Producer/Producer.csproj
```

**Expected Output:**
```
⚡ Performance-Tuned Producer
📡 Connecting to: localhost:9092

✅ Producer created with performance tuning:
   Batch Size: 32768 bytes
   Linger: 10 ms
   Compression: Snappy
   Max In-Flight: 5

📤 Producing 1000 messages to: performance-topic

📨 Queued 100 messages...
📨 Queued 200 messages...
...

═══════════════════════════════════════════════════════
📊 Performance Results
═══════════════════════════════════════════════════════
Total messages: 1000
Delivered: 1000
Errors: 0
Time: 2341 ms
Throughput: 427.17 msg/s

💡 Batching and compression improve throughput!
```

**What happens:**
- Producer batches messages (32KB batches, 10ms linger)
- Uses Snappy compression
- Improved throughput compared to baseline

## Step 5: Run Parallel Consumer

**Terminal 2:**

```bash
dotnet run --project Consumer/Consumer.csproj
```

**Expected Output:**
```
⚡ Parallel Consumer
💡 Processing messages concurrently
📡 Connecting to: localhost:9092

✅ Consumer subscribed
💡 Max concurrent workers: 10

📊 Processed: 100, Errors: 0, Active: 8
📊 Processed: 200, Errors: 0, Active: 9
...

🛑 Stopping consumer...
⏳ Waiting for all tasks to complete...

═══════════════════════════════════════════════════════
📊 Performance Results
═══════════════════════════════════════════════════════
Processed: 1000
Errors: 0
Time: 1234 ms
Throughput: 810.37 msg/s

💡 Parallel processing improves consumer throughput!
```

**What happens:**
- Consumer processes messages in parallel (up to 10 concurrent)
- Uses `SemaphoreSlim` to limit concurrency
- High throughput with parallel processing

## 🔍 Performance Comparison

### Producer Comparison

| Metric | Baseline | Tuned | Improvement |
|--------|----------|-------|-------------|
| Batch Size | 16KB | 32KB | 2x |
| Linger | 0ms | 10ms | Batched |
| Compression | None | Snappy | Reduced bandwidth |
| Throughput | ~191 msg/s | ~427 msg/s | **2.2x faster** |

### Consumer Comparison

| Metric | Sequential | Parallel | Improvement |
|--------|------------|----------|-------------|
| Concurrency | 1 | 10 | 10x |
| Throughput | ~100 msg/s | ~810 msg/s | **8x faster** |

## 📊 Compression Comparison

| Algorithm | Speed | Compression | Use Case |
|-----------|-------|-------------|----------|
| None | Fastest | 1.0x | Low bandwidth, fast processing |
| Snappy | Fast | 2.5x | **Recommended default** |
| Lz4 | Very Fast | 2.6x | High throughput |
| Gzip | Slow | 3.3x | High compression needed |
| Zstd | Medium | 3.6x | Best compression |

## 🐛 Troubleshooting

### Low throughput
- Check network latency
- Verify Kafka broker performance
- Increase batch size and linger time
- Enable compression

### High memory usage
- Reduce `MaxInFlight` requests
- Reduce batch size
- Limit parallel consumer workers

### Consumer lag
- Increase `MaxPollRecords`
- Increase parallel workers
- Optimize processing logic

## 📚 Next Steps

- Explore security (05-security)
- Learn multi-DC replication (06-multi-dc-replication)
- Understand delivery semantics (01-delivery-semantics)



