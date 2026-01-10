# How to Run: Multi-Datacenter Replication Example

Step-by-step guide to running the multi-DC replication example.

## ⚠️ Important Note

**Full MirrorMaker replication requires multiple Kafka clusters.**

This example demonstrates concepts but uses a single cluster for simplicity. For production, configure MirrorMaker 2 with multiple clusters.

## Prerequisites

- Kafka running (single cluster for demo, multiple for production)
- .NET 8.0 SDK
- MirrorMaker 2 (for production replication)

## Step 1: Start Kafka

**For local demo:**

```bash
cd /home/babicto/projects/kafka-event-driven-architecture
./scripts/start-kafka.sh
./scripts/verify-docker.sh
```

**For production (multiple clusters):**
- Start DC1 cluster (US-East)
- Start DC2 cluster (US-West)

## Step 2: Build the Solution

```bash
cd examples/05-advanced-kafka/dotnet/06-multi-dc-replication
dotnet build
```

**Expected:** Build succeeds with no errors.

## Step 3: Run Replication Demo

```bash
dotnet run --project Examples/Examples.csproj
```

**Expected Output:**
```
═══════════════════════════════════════════════════════
   Multi-Datacenter Replication Demo
═══════════════════════════════════════════════════════

📚 Multi-DC Replication Concepts:

1. **MirrorMaker 2**: Replicates topics between clusters
2. **Active-Passive**: One DC active, others replicate
3. **Active-Active**: Both DCs produce and consume
4. **Topic Prefixing**: Prevents topic name conflicts

🌍 Architecture:
  DC1 (US-East)                    DC2 (US-West)
  ┌─────────────┐                  ┌─────────────┐
  │   Kafka     │                  │   Kafka     │
  │   Cluster   │                  │   Cluster   │
  └──────┬──────┘                  └──────┬──────┘
         │                                  │
         │        MirrorMaker 2            │
         └──────────────┬───────────────────┘
                        │
                  Replication
```

## Step 4: Run DC1 Producer

**Terminal 1:**

```bash
dotnet run --project Producer/Producer.csproj
```

**Expected Output:**
```
🌍 DC1 Producer (US-East)
💡 Simulating production in DC1
📡 Connecting to: localhost:9092 (DC1)

✅ Producer created (DC1)
📤 Producing to topic: orders-dc1

📨 Sending order 0 from DC1:
   Order ID: order-dc1-0
   Customer: customer-0
   Amount: $100.00
✅ Delivered to DC1: orders-dc1 [[0]] @0
💡 This message will be replicated to DC2

...

✅ All messages sent from DC1
💡 In production, MirrorMaker would replicate these to DC2
```

**What happens:**
- Producer sends messages to DC1 cluster
- In production, MirrorMaker replicates to DC2

## Step 5: Run DC2 Consumer

**Terminal 2:**

```bash
dotnet run --project Consumer/Consumer.csproj
```

**Expected Output:**
```
🌍 DC2 Consumer (US-West)
💡 Simulating consumption from DC2
📡 Connecting to: localhost:9092 (DC2)

✅ Consumer subscribed
💡 Waiting for replicated messages from DC1...

📥 Received replicated message #1:
   Order ID: order-dc1-0
   Customer: customer-0
   Amount: $100.00
   Source DC: us-east
   Offset: 0
💡 This message was replicated from DC1 to DC2

...
```

**What happens:**
- Consumer receives messages from DC2 cluster
- In production, these are replicated from DC1 via MirrorMaker

## 🔍 Understanding Replication

### MirrorMaker 2 Configuration

**mm2.properties:**
```properties
clusters = us-east, us-west

us-east.bootstrap.servers = east-kafka:9092
us-west.bootstrap.servers = west-kafka:9092

# Replication flows
us-east->us-west.enabled = true
us-west->us-east.enabled = true

# Topics to replicate
us-east->us-west.topics = orders-dc1
us-west->us-east.topics = orders-dc2

# Heartbeats and checkpoints
emit.heartbeats.enabled = true
emit.checkpoints.enabled = true
```

### Starting MirrorMaker 2

```bash
connect-mirror-maker.sh mm2.properties
```

### Replication Features

1. **Topic Replication**: Replicates topics between clusters
2. **Consumer Group Replication**: Replicates consumer group offsets
3. **Topic Prefixing**: Adds prefix to prevent conflicts
4. **Offset Translation**: Maps offsets between clusters

## 🐛 Troubleshooting

### Messages not replicating

- Verify MirrorMaker is running
- Check replication flow configuration
- Verify topics exist in source cluster
- Check network connectivity between clusters

### Consumer not receiving messages

- Verify MirrorMaker replicated topics
- Check consumer group configuration
- Verify topic names (may have prefixes)
- Check consumer offset

### Replication lag

- Monitor replication lag metrics
- Check network latency between clusters
- Verify MirrorMaker performance
- Consider increasing MirrorMaker resources

## 📚 Next Steps

- Explore delivery semantics (01-delivery-semantics)
- Understand performance tuning (04-performance-tuning)
- Learn security (05-security)

## 💡 Production Checklist

- [ ] Multiple Kafka clusters configured
- [ ] MirrorMaker 2 installed and configured
- [ ] Replication flows defined
- [ ] Monitoring and alerting set up
- [ ] Disaster recovery plan documented
- [ ] Replication lag monitored
- [ ] Failover procedures tested



