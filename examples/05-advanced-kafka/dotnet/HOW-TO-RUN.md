# How to Run: Advanced Kafka Concepts Examples

Complete guide for running all advanced Kafka concept examples.

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

## Step 2: Build All Examples

```bash
cd examples/05-advanced-kafka/dotnet
dotnet build AdvancedKafkaConcepts.sln
```

**Expected:** All projects build successfully.

## Step 3: Run Examples

Each example can be run independently. See individual `HOW-TO-RUN.md` files for detailed instructions.

### Example 1: Delivery Semantics

```bash
cd 01-delivery-semantics
dotnet run --project AtMostOnce/AtMostOnce.csproj
dotnet run --project AtLeastOnce/AtLeastOnce.csproj
dotnet run --project ExactlyOnce/ExactlyOnce.csproj
```

**See:** `01-delivery-semantics/HOW-TO-RUN.md`

---

### Example 2: Log Compaction

```bash
cd 02-log-compaction
dotnet run --project Producer/Producer.csproj
dotnet run --project Consumer/Consumer.csproj
```

**See:** `02-log-compaction/HOW-TO-RUN.md`

---

### Example 3: Kafka Streams

```bash
cd 03-kafka-streams
# Terminal 1: Producer
dotnet run --project Examples/Examples.csproj
# Terminal 2: Stream processor
# Modify Examples.csproj StartupObject to StreamsDemo
dotnet run --project Examples/Examples.csproj
```

**See:** `03-kafka-streams/HOW-TO-RUN.md`

---

### Example 4: Performance Tuning

```bash
cd 04-performance-tuning
# Baseline
dotnet run --project Producer/Producer.csproj
# Tuned (modify StartupObject)
dotnet run --project Producer/Producer.csproj
dotnet run --project Consumer/Consumer.csproj
```

**See:** `04-performance-tuning/HOW-TO-RUN.md`

---

### Example 5: Security

```bash
cd 05-security
# Note: Requires broker SSL/SASL configuration
dotnet run --project SSL/SSL.csproj
dotnet run --project SASL/SASL.csproj
```

**See:** `05-security/HOW-TO-RUN.md`

---

### Example 6: Multi-DC Replication

```bash
cd 06-multi-dc-replication
dotnet run --project Producer/Producer.csproj
dotnet run --project Consumer/Consumer.csproj
```

**See:** `06-multi-dc-replication/HOW-TO-RUN.md`

---

## 🔍 Quick Reference

### Running Order

1. **Delivery Semantics** - Understand message guarantees
2. **Log Compaction** - Learn state management
3. **Performance Tuning** - Optimize throughput
4. **Kafka Streams** - Real-time processing
5. **Security** - Secure connections (requires setup)
6. **Multi-DC** - Replication concepts (requires setup)

### Common Commands

**Build specific example:**
```bash
cd 01-delivery-semantics
dotnet build
```

**Run specific example:**
```bash
cd 01-delivery-semantics
dotnet run --project AtMostOnce/AtMostOnce.csproj
```

**Clean and rebuild:**
```bash
dotnet clean
dotnet build
```

## 🐛 Troubleshooting

### Kafka Not Running

```bash
./scripts/start-kafka.sh
./scripts/verify-docker.sh
```

### Build Errors

```bash
dotnet restore
dotnet clean
dotnet build
```

### Port Conflicts

```bash
docker compose down
./scripts/start-kafka.sh
```

### Consumer Not Receiving Messages

- Check consumer group is unique
- Verify topic exists
- Check `AutoOffsetReset` setting
- Verify Kafka is running

### Examples Not Working

- Ensure Kafka is running
- Check individual example README.md
- Verify prerequisites are met
- Check for port conflicts

## 📚 Next Steps

After running examples:

1. **Read documentation** in each example's README.md
2. **Experiment** with different configurations
3. **Compare** baseline vs tuned performance
4. **Explore** Kafka UI to see messages
5. **Try** combining concepts

## 💡 Tips

- **Start simple**: Begin with delivery semantics
- **Use Kafka UI**: Visualize messages at http://localhost:8080
- **Read logs**: Check console output for details
- **Experiment**: Modify configurations and observe changes
- **Document**: Take notes on what you learn

## 🔗 Related Examples

- `01-fundamentals` - Basic producer/consumer
- `02-core-concepts` - Core Kafka concepts
- `04-build-e-commerce` - Complete microservices example
- `06-event-sourcing` - Event sourcing patterns
- `06-saga` - Saga patterns

## 📖 Documentation

Each example includes:
- `README.md` - Concept explanation and overview
- `HOW-TO-RUN.md` - Detailed step-by-step instructions
- Code comments - Inline documentation

## 🤝 Getting Help

- Check individual example README.md files
- Review HOW-TO-RUN.md for detailed instructions
- Check Kafka UI for message visualization
- Review console output for errors

## 📄 License

MIT License - see LICENSE file in repository root.

