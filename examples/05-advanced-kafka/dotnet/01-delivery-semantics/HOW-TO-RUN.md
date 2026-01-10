# How to Run: Delivery Semantics Examples

Step-by-step guide to running all delivery semantics examples.

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
cd examples/05-advanced-kafka/dotnet/01-delivery-semantics
dotnet build
```

**Expected:** Build succeeds with no errors.

## Step 3: At-Most-Once Example

### 3.1 Run Producer

```bash
dotnet run --project AtMostOnce/AtMostOnce.csproj
```

**Expected Output:**
```
🔥 At-Most-Once Producer (Fire & Forget)
⚠️  WARNING: Messages may be lost!
📡 Connecting to: localhost:9092

✅ Producer created (Acks=None, Retries=0)
📤 Producing to topic: at-most-once-topic

📨 Queued message 0 (not waiting for ACK)
⚡ Sent (no ACK): at-most-once-topic [[0]] @0
...
✅ Producer finished (messages may or may not have been delivered)
💡 Check Kafka UI to see how many messages actually arrived
```

**What to observe:**
- Messages are queued without waiting for ACK
- Producer finishes quickly
- Some messages may be lost

### 3.2 Verify in Kafka UI

1. Open http://localhost:8080
2. Navigate to Topics → `at-most-once-topic`
3. Check message count (may be less than 10)

## Step 4: At-Least-Once Example

### 4.1 Run Producer

```bash
dotnet run --project AtLeastOnce/AtLeastOnce.csproj
```

**Expected Output:**
```
✅ At-Least-Once Producer (Default)
⚠️  Messages guaranteed to be written, but may duplicate
📡 Connecting to: localhost:9092

✅ Producer created (Acks=All, Retries=3)
📤 Producing to topic: at-least-once-topic

📨 Sending message 0 (event_id: ...)...
✅ Delivered to at-least-once-topic [[0]] @0
...
✅ All messages delivered (may be duplicated on retry)
```

**What to observe:**
- Producer waits for ACK
- Messages are guaranteed to be written
- May be duplicated on retry

### 4.2 Run Basic Consumer (Shows Duplicates)

**Modify `AtLeastOnce.csproj` to use `BasicConsumer`:**

```xml
<StartupObject>DeliverySemantics.AtLeastOnce.BasicConsumer</StartupObject>
```

Then run:

```bash
dotnet run --project AtLeastOnce/AtLeastOnce.csproj
```

**Expected Output:**
```
⚠️  Basic Consumer (Shows Duplicates)
📡 Connecting to: localhost:9092

✅ Consumer subscribed, waiting for messages...
💡 This consumer may process duplicates!

📥 Received: Message 0 (event_id: ...)
✅ Processing message 0...
✅ Committed offset: at-least-once-topic [[0]] @0

📥 Received: Message 0 (event_id: ...)  ← DUPLICATE!
⚠️  DUPLICATE DETECTED! Event ID: ...
```

**What to observe:**
- Same message may be processed multiple times
- Duplicates occur if commit fails after processing

### 4.3 Run Idempotent Consumer (Deduplicates)

**Modify `AtLeastOnce.csproj` to use `IdempotentConsumer`:**

```xml
<StartupObject>DeliverySemantics.AtLeastOnce.IdempotentConsumer</StartupObject>
```

Then run:

```bash
dotnet run --project AtLeastOnce/AtLeastOnce.csproj
```

**Expected Output:**
```
🛡️  Idempotent Consumer (Deduplication)
📡 Connecting to: localhost:9092

✅ Consumer subscribed with deduplication
💡 Duplicate messages will be skipped

📥 Received: Message 0 (event_id: ...)
✅ Processing message 0...
✅ Committed offset: at-least-once-topic [[0]] @0

📥 Received: Message 0 (event_id: ...)  ← DUPLICATE
⏭️  SKIPPING DUPLICATE: Event ID ... already processed
```

**What to observe:**
- Duplicates are detected and skipped
- Only unique messages are processed
- In production, use Redis or database for distributed deduplication

## Step 5: Exactly-Once Example

### 5.1 Run Transactional Producer

```bash
dotnet run --project ExactlyOnce/ExactlyOnce.csproj
```

**Expected Output:**
```
🎯 Exactly-Once Producer (Transactional)
✅ Guaranteed: No duplicates, no loss
📡 Connecting to: localhost:9092

🔄 Initializing transactions...
✅ Transactions initialized

🔄 Beginning transaction...
✅ Transaction started

📤 Producing messages to topic: exactly-once-topic

📨 Sending message 0 (event_id: ...)...
✅ Queued: exactly-once-topic [[0]] @0
...

🔄 Committing transaction...
✅ Transaction committed - all messages delivered exactly once
```

**What to observe:**
- Transaction is initialized
- All messages are produced within transaction
- Transaction is committed (all or nothing)

### 5.2 Run Read Committed Consumer

**Modify `ExactlyOnce.csproj` to use `ReadCommittedConsumer`:**

```xml
<StartupObject>DeliverySemantics.ExactlyOnce.ReadCommittedConsumer</StartupObject>
```

Then run:

```bash
dotnet run --project ExactlyOnce/ExactlyOnce.csproj
```

**Expected Output:**
```
🎯 Read Committed Consumer (Exactly-Once)
✅ Only reads committed transactions
📡 Connecting to: localhost:9092

✅ Consumer subscribed (ReadCommitted isolation level)
💡 Only committed messages will be read

📥 Received: Message 0 (event_id: ...)
✅ Processing message 0...
✅ Processed: exactly-once-topic [[0]] @0
...

🛑 Consumer stopped
📊 Total processed: 10
✅ No duplicates detected (exactly-once guarantee)
```

**What to observe:**
- Only committed messages are read
- No duplicates detected
- Exactly-once guarantee maintained

## 🔍 Comparison Summary

| Semantics | Speed | Safety | Duplicates | Use Case |
|-----------|-------|--------|------------|----------|
| At-Most-Once | ⚡ Fast | ❌ Unsafe | ❌ No | Metrics, logs |
| At-Least-Once | 🐢 Slower | ✅ Safe | ⚠️ Yes | Most applications |
| Exactly-Once | 🐢 Slowest | ✅ Safe | ✅ No | Critical operations |

## 🐛 Troubleshooting

### Producer hangs
- Check Kafka is running: `docker compose ps`
- Verify connection: `./scripts/verify-docker.sh`

### Consumer not receiving messages
- Check consumer group: `GroupId` must be unique
- Verify topic exists: Check Kafka UI
- Reset offset: Change `AutoOffsetReset` to `Earliest`

### Transaction errors
- Ensure `EnableIdempotence = true`
- Check `TransactionalId` is unique
- Verify `MaxInFlight <= 5`

### Duplicates in At-Least-Once
- This is expected behavior
- Use idempotent consumer to handle duplicates
- Check `event_id` in messages

## 📚 Next Steps

- Explore log compaction (02-log-compaction)
- Learn performance tuning (04-performance-tuning)
- Understand security (05-security)



