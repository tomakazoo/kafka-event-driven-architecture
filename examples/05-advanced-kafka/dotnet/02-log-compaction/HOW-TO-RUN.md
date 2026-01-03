# How to Run: Log Compaction Example

Step-by-step guide to running the log compaction example.

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
cd examples/05-advanced-kafka/dotnet/02-log-compaction
dotnet build
```

**Expected:** Build succeeds with no errors.

## Step 3: Run Producer

```bash
dotnet run --project Producer/Producer.csproj
```

**Expected Output:**
```
📦 Log Compaction Producer
💡 Sending multiple updates for same user key
📡 Connecting to: localhost:9092

✅ Created compacted topic: user-state-compacted
   cleanup.policy=compact

✅ Producer created
📤 Producing to topic: user-state-compacted
👤 User ID: user-123

📨 Sending update v1:
   Name: John Doe
   Email: john@example.com
✅ Delivered: user-state-compacted [[0]] @0

📨 Sending update v2:
   Name: John Doe
   Email: john.doe@example.com
✅ Delivered: user-state-compacted [[0]] @1

...

✅ All updates sent
💡 After compaction, only the latest version (v5) will remain
```

**What happens:**
- Producer creates a compacted topic
- Sends 5 updates for the same user key (`user-123`)
- Each update has a different version number
- All messages go to the same partition (same key)

## Step 4: Run Consumer

```bash
dotnet run --project Consumer/Consumer.csproj
```

**Expected Output:**
```
📦 Log Compaction Consumer
💡 Reading from compacted topic
📡 Connecting to: localhost:9092

✅ Consumer subscribed
💡 Reading all messages (including historical updates)

📥 Received: Offset 0
   User: user-123
   Version: 1
   Name: John Doe
   Email: john@example.com

📥 Received: Offset 1
   User: user-123
   Version: 2
   Name: John Doe
   Email: john.doe@example.com

...

═══════════════════════════════════════════════════════
📊 Summary
═══════════════════════════════════════════════════════
Total messages received: 5

📌 Latest State (after compaction):
   User ID: user-123
   Version: 5
   Name: John D. Doe
   Email: j.doe@example.com
   Offset: 4

💡 Note: After compaction runs, older versions are removed
💡 Only the latest state per key remains in the log
```

**What happens:**
- Consumer reads from the beginning (`AutoOffsetReset.Earliest`)
- Receives all 5 versions
- Shows the latest state summary

## Step 5: Observe Compaction

### Check Kafka UI

1. Open http://localhost:8080
2. Navigate to Topics → `user-state-compacted`
3. View messages

**Before Compaction:**
- All 5 messages are visible
- Each has a different version

**After Compaction (runs periodically):**
- Older versions are removed
- Only the latest version (v5) remains
- The log becomes a state store

### Trigger Compaction Manually

Compaction runs automatically, but you can check compaction status:

```bash
# Check topic configuration
docker compose exec kafka kafka-configs --bootstrap-server localhost:9092 \
  --entity-type topics --entity-name user-state-compacted --describe
```

## 🔍 Understanding Compaction

### How Compaction Works

1. **Segments**: Kafka stores messages in segments
2. **Dirty Ratio**: When 50% of a segment is "dirty" (has newer values), compaction runs
3. **Key-Based**: Compaction keeps only the latest value per key
4. **Non-Blocking**: Compaction doesn't block reads/writes

### What Gets Compacted

- ✅ Messages with the same key (older removed)
- ✅ Null values (tombstones - delete markers)
- ❌ Messages with different keys (all kept)

### Compaction Timing

- Runs periodically (configurable)
- Triggered by `min.cleanable.dirty.ratio`
- Can be triggered manually via admin API

## 🐛 Troubleshooting

### Compaction not running
- Check topic config: `cleanup.policy=compact`
- Wait for compaction to trigger (runs periodically)
- Check segment size and dirty ratio

### Consumer sees all versions
- This is expected before compaction runs
- Compaction runs asynchronously
- Wait for compaction to complete

### Topic not created with compaction
- Producer creates topic with compaction enabled
- Verify in Kafka UI: Topic → Configuration
- Check `cleanup.policy` is set to `compact`

## 📚 Next Steps

- Explore Kafka Streams (03-kafka-streams)
- Learn performance tuning (04-performance-tuning)
- Understand security (05-security)

