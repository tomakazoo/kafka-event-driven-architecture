# How to Run: Kafka Streams Example

Step-by-step guide to running the Kafka Streams simulation example.

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
cd examples/05-advanced-kafka/dotnet/03-kafka-streams
dotnet build
```

**Expected:** Build succeeds with no errors.

## Step 3: Run Order Producer

**Terminal 1:**

```bash
dotnet run --project Examples/Examples.csproj
```

**Expected Output:**
```
📤 Order Producer for Stream Processing
📡 Connecting to: localhost:9092

✅ Producer created
📤 Producing to topic: orders-stream

📨 Sending order 0:
   Customer: customer-1
   Product: product-A
   Amount: $523.45
   Status: completed
✅ Delivered: orders-stream [[0]] @0

...

✅ All orders sent
💡 Run StreamsDemo to process the stream
```

**What happens:**
- Producer sends 20 orders
- Orders have random customers, products, amounts, and statuses
- Some orders are "completed", others are "pending" or "cancelled"

## Step 4: Run Stream Processing

**Terminal 2:**

**Modify `Examples.csproj` to use `StreamsDemo`:**

```xml
<StartupObject>KafkaStreams.Examples.StreamsDemo</StartupObject>
```

Then run:

```bash
dotnet run --project Examples/Examples.csproj
```

**Expected Output:**
```
═══════════════════════════════════════════════════════
   Kafka Streams Simulation Demo
═══════════════════════════════════════════════════════

✅ Created topic: orders-stream
✅ Created topic: customer-stats
✅ Created topic: product-stats

🔄 Starting stream processing topology...

✅ Stream processing started
💡 Waiting for orders to process...
💡 Press Ctrl+C to stop and see statistics

⏭️  Filtered out: Order order-0 (status: pending)
✅ Processed: Order order-1
   Customer: customer-2, Amount: $234.56
📊 Emitted customer stats: customer-2 (Count: 1, Total: $234.56)
📊 Emitted product stats: product-B (Count: 1, Total: $234.56)

...

🛑 Stopping stream processing...

═══════════════════════════════════════════════════════
📊 Current Statistics
═══════════════════════════════════════════════════════

👥 Customer Statistics:
   customer-2:
      Orders: 5
      Total: $1,234.56
      Average: $246.91
   customer-1:
      Orders: 3
      Total: $567.89
      Average: $189.30

📦 Product Statistics:
   product-B:
      Orders: 4
      Total: $890.12
      Average: $222.53
   product-A:
      Orders: 4
      Total: $912.33
      Average: $228.08
```

**What happens:**
- Stream processor reads from `orders-stream`
- Filters out non-completed orders
- Aggregates statistics by customer and product
- Emits aggregated results to `customer-stats` and `product-stats` topics
- Maintains stateful statistics in memory

## Step 5: Observe Results

### Check Kafka UI

1. Open http://localhost:8080
2. Navigate to Topics:
   - `orders-stream`: Source stream (all orders)
   - `customer-stats`: Aggregated customer statistics
   - `product-stats`: Aggregated product statistics

### Understanding the Processing

1. **Filtering**: Only "completed" orders are processed
2. **Aggregation**: Statistics computed per customer and product
3. **Stateful**: Statistics maintained across messages
4. **Real-time**: Processing happens as orders arrive

## 🔍 Stream Processing Concepts

### Filtering
```csharp
if (order.Status != "completed")
    continue; // Filter out
```

### Aggregation
```csharp
customerStat.Count++;
customerStat.TotalAmount += order.Amount;
customerStat.AverageAmount = customerStat.TotalAmount / customerStat.Count;
```

### Stateful Operations
- Statistics maintained in memory (`Dictionary`)
- Updated as new orders arrive
- Can be persisted to state store (not shown in this example)

## 🐛 Troubleshooting

### No orders processed
- Check producer ran first
- Verify orders have "completed" status
- Check consumer group offset

### Statistics not updating
- Verify stream processor is running
- Check for errors in processing
- Ensure orders are being consumed

### Topics not created
- Producer/processor creates topics automatically
- Check Kafka UI to verify topics exist

## 📚 Next Steps

- Explore performance tuning (04-performance-tuning)
- Understand security (05-security)
- Learn multi-DC replication (06-multi-dc-replication)

## 💡 Production Considerations

This example demonstrates concepts but uses manual processing. For production:

- **Use Kafka Streams** (Java) for native stream processing
- **Use ksqlDB** for SQL-like stream processing
- **Use Apache Flink** for advanced stream processing
- **Consider state stores** for persistent state
- **Handle failures** with checkpointing and recovery



