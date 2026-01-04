# Delivery Semantics: At-Most-Once, At-Least-Once, Exactly-Once

This example demonstrates the three fundamental message delivery guarantees in Kafka.

## 📚 Concepts

### 1. At-Most-Once (Fire and Forget)
- **Configuration**: `Acks = None`, `Retries = 0`
- **Behavior**: Messages may be lost, but never duplicated
- **Use Case**: Metrics, logs where losing some data is acceptable
- **Trade-off**: ⚡ Fast but ❌ unsafe

### 2. At-Least-Once (Default)
- **Configuration**: `Acks = All`, `Retries > 0`
- **Behavior**: Messages guaranteed to be written, but may be duplicated
- **Use Case**: Most common pattern, requires idempotent consumers
- **Trade-off**: ✅ Safe but ⚠️ may duplicate

### 3. Exactly-Once (EOS)
- **Configuration**: `EnableIdempotence = true`, Transactions
- **Behavior**: Guaranteed exactly once, no duplicates, no loss
- **Use Case**: Financial transactions, critical operations
- **Trade-off**: ✅ Safe and ✅ no duplicates (more overhead)

## 🏗️ Structure

```
01-delivery-semantics/
├── AtMostOnce/
│   ├── Producer.cs          # Fire-and-forget producer
│   └── AtMostOnce.csproj
├── AtLeastOnce/
│   ├── Producer.cs          # Default producer with retries
│   ├── BasicConsumer.cs     # Shows duplicates
│   ├── IdempotentConsumer.cs # Deduplication example
│   └── AtLeastOnce.csproj
├── ExactlyOnce/
│   ├── TransactionalProducer.cs  # Idempotent + transactional
│   ├── ReadCommittedConsumer.cs  # Read committed only
│   └── ExactlyOnce.csproj
├── Examples/
│   ├── DeliverySemanticsDemo.cs
│   └── Examples.csproj
└── README.md
```

## 🚀 How to Run

### Prerequisites
- Kafka running (see main README)
- .NET 8.0 SDK

### Step 1: Start Kafka
```bash
cd /home/babicto/projects/kafka-event-driven-architecture
./scripts/start-kafka.sh
./scripts/verify-docker.sh
```

### Step 2: Run At-Most-Once Example

**Terminal 1 - Producer:**
```bash
cd examples/05-advanced-kafka/dotnet/01-delivery-semantics
dotnet run --project AtMostOnce/AtMostOnce.csproj
```

**Expected:** Messages sent without waiting for ACK. Some may be lost.

**Terminal 2 - Check Kafka UI:**
- Open http://localhost:8080
- Check `at-most-once-topic`
- Compare sent vs received messages

### Step 3: Run At-Least-Once Example

**Terminal 1 - Producer:**
```bash
dotnet run --project AtLeastOnce/AtLeastOnce.csproj
```

**Terminal 2 - Basic Consumer (shows duplicates):**
```bash
# Modify AtLeastOnce.csproj StartupObject to BasicConsumer
dotnet run --project AtLeastOnce/AtLeastOnce.csproj
```

**Terminal 3 - Idempotent Consumer (deduplicates):**
```bash
# Modify AtLeastOnce.csproj StartupObject to IdempotentConsumer
dotnet run --project AtLeastOnce/AtLeastOnce.csproj
```

**Expected:** Basic consumer may show duplicates. Idempotent consumer skips them.

### Step 4: Run Exactly-Once Example

**Terminal 1 - Transactional Producer:**
```bash
dotnet run --project ExactlyOnce/ExactlyOnce.csproj
```

**Terminal 2 - Read Committed Consumer:**
```bash
# Modify ExactlyOnce.csproj StartupObject to ReadCommittedConsumer
dotnet run --project ExactlyOnce/ExactlyOnce.csproj
```

**Expected:** No duplicates, guaranteed exactly once.

## 🔍 What to Observe

1. **At-Most-Once**: Fast but messages may be lost
2. **At-Least-Once**: Safe but duplicates possible (check event_id)
3. **Exactly-Once**: No duplicates, guaranteed delivery

## 📖 Key Takeaways

- **At-Most-Once**: Use for non-critical data
- **At-Least-Once**: Default, requires idempotent consumers
- **Exactly-Once**: Use for critical operations, requires transactions

## 🔗 Related Concepts

- Idempotent consumers
- Transactional producers
- Consumer offset management
- Message deduplication strategies


