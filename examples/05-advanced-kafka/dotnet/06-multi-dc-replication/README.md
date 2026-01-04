# Multi-Datacenter Replication: MirrorMaker 2

This example demonstrates multi-datacenter replication concepts using MirrorMaker 2.

## 📚 Concept

**Multi-Datacenter Replication** replicates Kafka topics between clusters in different datacenters.

### MirrorMaker 2

MirrorMaker 2 is a tool for replicating data between Kafka clusters:

- **Topic Replication**: Replicates topics from source to target cluster
- **Consumer Group Replication**: Replicates consumer group offsets
- **Topic Prefixing**: Adds prefix to prevent conflicts
- **Offset Translation**: Maps offsets between clusters

### Replication Patterns

1. **Active-Passive**: One DC active, others replicate
2. **Active-Active**: Both DCs produce and consume
3. **Hub-and-Spoke**: Central hub replicates to multiple spokes

## 🏗️ Structure

```
06-multi-dc-replication/
├── Domain/
│   ├── ReplicationEvent.cs   # Event model
│   └── Domain.csproj
├── Producer/
│   ├── Dc1Producer.cs        # Simulates DC1 producer
│   └── Producer.csproj
├── Consumer/
│   ├── Dc2Consumer.cs        # Simulates DC2 consumer
│   └── Consumer.csproj
├── Examples/
│   ├── ReplicationDemo.cs    # Concepts demo
│   └── Examples.csproj
└── README.md
```

## 🚀 How to Run

### Prerequisites
- Multiple Kafka clusters (for full replication)
- MirrorMaker 2 configured
- .NET 8.0 SDK

### Step 1: Start Kafka Clusters

**For local demo (single cluster):**
```bash
cd /home/babicto/projects/kafka-event-driven-architecture
./scripts/start-kafka.sh
```

**For production (multiple clusters):**
- Start DC1 cluster (US-East)
- Start DC2 cluster (US-West)
- Configure MirrorMaker 2

### Step 2: Run DC1 Producer

```bash
cd examples/05-advanced-kafka/dotnet/06-multi-dc-replication
dotnet run --project Producer/Producer.csproj
```

**Expected:** Producer sends messages to DC1 cluster.

### Step 3: Run DC2 Consumer

```bash
dotnet run --project Consumer/Consumer.csproj
```

**Expected:** Consumer receives replicated messages from DC2 cluster.

## 🔍 What to Observe

1. **Replication**: Messages replicated from DC1 to DC2
2. **Topic Prefixing**: Topics may have prefixes in target cluster
3. **Offset Mapping**: Offsets translated between clusters
4. **Consumer Groups**: Consumer group offsets replicated

## 📖 Key Takeaways

- **MirrorMaker 2**: Tool for cluster replication
- **Multi-DC**: Replicate data across datacenters
- **Disaster Recovery**: Backup cluster for failover
- **Geographic Distribution**: Serve users from nearby DCs

## 🔗 Related Concepts

- Disaster recovery
- Geographic distribution
- Cluster replication
- Topic prefixing

## 💡 Production Setup

For production, configure MirrorMaker 2:

1. **Install MirrorMaker 2**
2. **Configure clusters** (source and target)
3. **Create replication flows**
4. **Monitor replication lag**

See `HOW-TO-RUN.md` for detailed setup instructions.


