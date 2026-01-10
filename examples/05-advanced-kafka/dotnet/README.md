# Advanced Kafka Concepts

Complete examples demonstrating advanced Kafka concepts for production systems.

## 📚 Overview

This folder contains comprehensive examples covering:

1. **Delivery Semantics** - At-most-once, at-least-once, exactly-once guarantees
2. **Log Compaction** - State management with compacted topics
3. **Kafka Streams** - Real-time stream processing (simulated)
4. **Performance Tuning** - Batching, compression, parallel processing
5. **Security** - SSL/TLS encryption and SASL authentication
6. **Multi-DC Replication** - MirrorMaker 2 concepts

## 🏗️ Structure

```
05-advanced-kafka/dotnet/
├── 01-delivery-semantics/      # Message delivery guarantees
├── 02-log-compaction/          # State management
├── 03-kafka-streams/           # Stream processing
├── 04-performance-tuning/      # Performance optimization
├── 05-security/                # SSL and SASL
├── 06-multi-dc-replication/    # Multi-datacenter replication
├── AdvancedKafkaConcepts.sln   # Solution file
└── README.md                    # This file
```

## 🚀 Quick Start

### Prerequisites

- .NET 8.0 SDK installed
- Docker and Docker Compose installed
- Kafka running (see setup below)

### Step 1: Start Kafka

```bash
cd /home/babicto/projects/kafka-event-driven-architecture
./scripts/start-kafka.sh
./scripts/verify-docker.sh
```

### Step 2: Run an Example

Each example has its own folder with detailed instructions:

```bash
cd examples/05-advanced-kafka/dotnet/01-delivery-semantics
dotnet run --project AtMostOnce/AtMostOnce.csproj
```

## 📖 Examples Guide

### 01. Delivery Semantics

**Location:** `01-delivery-semantics/`

**What it demonstrates:**
- At-Most-Once: Fast but unsafe (messages may be lost)
- At-Least-Once: Safe but may duplicate (default)
- Exactly-Once: Safe and no duplicates (transactions)

**Quick run:**
```bash
cd 01-delivery-semantics
dotnet run --project AtMostOnce/AtMostOnce.csproj
```

**See:** `01-delivery-semantics/README.md` for details

---

### 02. Log Compaction

**Location:** `02-log-compaction/`

**What it demonstrates:**
- Topic compaction configuration
- Latest state per key preservation
- State store reconstruction

**Quick run:**
```bash
cd 02-log-compaction
dotnet run --project Producer/Producer.csproj
dotnet run --project Consumer/Consumer.csproj
```

**See:** `02-log-compaction/README.md` for details

---

### 03. Kafka Streams

**Location:** `03-kafka-streams/`

**What it demonstrates:**
- Stream processing concepts
- Filtering and aggregation
- Stateful operations
- Real-time analytics

**Quick run:**
```bash
cd 03-kafka-streams
dotnet run --project Examples/Examples.csproj
```

**See:** `03-kafka-streams/README.md` for details

---

### 04. Performance Tuning

**Location:** `04-performance-tuning/`

**What it demonstrates:**
- Producer batching and compression
- Consumer parallel processing
- Performance comparisons

**Quick run:**
```bash
cd 04-performance-tuning
dotnet run --project Producer/Producer.csproj
dotnet run --project Consumer/Consumer.csproj
```

**See:** `04-performance-tuning/README.md` for details

---

### 05. Security

**Location:** `05-security/`

**What it demonstrates:**
- SSL/TLS encryption configuration
- SASL authentication (PLAIN)
- Secure client connections

**Note:** Requires broker-side SSL/SASL configuration

**Quick run:**
```bash
cd 05-security
dotnet run --project SSL/SSL.csproj
dotnet run --project SASL/SASL.csproj
```

**See:** `05-security/README.md` for details

---

### 06. Multi-DC Replication

**Location:** `06-multi-dc-replication/`

**What it demonstrates:**
- MirrorMaker 2 concepts
- Multi-datacenter replication
- Topic replication patterns

**Note:** Full replication requires multiple Kafka clusters

**Quick run:**
```bash
cd 06-multi-dc-replication
dotnet run --project Producer/Producer.csproj
dotnet run --project Consumer/Consumer.csproj
```

**See:** `06-multi-dc-replication/README.md` for details

---

## 🔍 Learning Path

**Recommended order:**

1. **Start with:** Delivery Semantics (foundational)
2. **Then:** Log Compaction (state management)
3. **Then:** Performance Tuning (optimization)
4. **Advanced:** Kafka Streams, Security, Multi-DC

## 📚 Key Concepts

### Delivery Semantics
- **At-Most-Once**: Fast but unsafe
- **At-Least-Once**: Default, requires idempotent consumers
- **Exactly-Once**: Transactions, guaranteed once

### Log Compaction
- Keeps latest state per key
- Removes older updates
- Creates state stores

### Kafka Streams
- Real-time stream processing
- Filtering, aggregation, joins
- Stateful operations

### Performance Tuning
- Batching for throughput
- Compression for efficiency
- Parallel processing

### Security
- SSL/TLS for encryption
- SASL for authentication
- ACLs for access control

### Multi-DC Replication
- MirrorMaker 2 for replication
- Disaster recovery
- Geographic distribution

## 🐛 Troubleshooting

### Common Issues

**Kafka not running:**
```bash
./scripts/start-kafka.sh
./scripts/verify-docker.sh
```

**Build errors:**
```bash
dotnet restore
dotnet build
```

**Port conflicts:**
```bash
docker compose down
./scripts/start-kafka.sh
```

**Consumer not receiving messages:**
- Check consumer group
- Verify topic exists
- Check offset reset policy

## 📖 Documentation

Each example includes:
- `README.md` - Concept explanation
- `HOW-TO-RUN.md` - Step-by-step guide
- Code comments - Inline documentation

## 🔗 Related Resources

- [Kafka Documentation](https://kafka.apache.org/documentation/)
- [Confluent Kafka .NET Client](https://github.com/confluentinc/confluent-kafka-dotnet)
- [Kafka Streams Documentation](https://kafka.apache.org/documentation/streams/)

## 💡 Production Notes

- **Always use SSL** in production
- **Use exactly-once** for critical operations
- **Enable log compaction** for state stores
- **Tune performance** based on workload
- **Set up replication** for disaster recovery

## 🤝 Contributing

Contributions welcome! See the main repository's contributing guide.

## 📄 License

MIT License - see LICENSE file in repository root.



