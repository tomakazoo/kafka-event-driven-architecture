# Quick Start Guide - Kafka Event-Driven Architecture

## 🎯 Complete Setup for WSL 2 (Without Docker Desktop)

This guide will get you from zero to running Kafka examples in minutes. All examples are organized by difficulty, from beginner to advanced.

---

## 📋 Prerequisites

- WSL 2 with Ubuntu 24.04 (or Docker Desktop)
- .NET 8.0 SDK (for C# examples)
- Git
- *Note: Python examples may be added in the future*

---

## 🚀 Initial Setup (One-Time)

### Step 1: Install Docker Engine

**For WSL 2 (without Docker Desktop):**
```bash
cd /home/babicto/projects/kafka-event-driven-architecture
./scripts/install-docker.sh
```

**After installation completes:**
```bash
# Activate docker group (so you don't need sudo)
newgrp docker

# Verify installation
docker --version
docker compose version
```

Expected output:
```
Docker version 24.x.x, build xxxxx
Docker Compose version v2.x.x
```

**For systems with Docker Desktop:**
- Just ensure Docker Desktop is running

---

### Step 2: Start Kafka Services

```bash
cd /home/babicto/projects/kafka-event-driven-architecture
./scripts/start-kafka.sh
./scripts/verify-docker.sh
```

This will start:
- ✅ Zookeeper (localhost:2181)
- ✅ Kafka Broker (localhost:9092)
- ✅ Schema Registry (localhost:8081)
- ✅ Kafka UI (http://localhost:8080)

**Verify everything is running:**
```bash
./scripts/verify-docker.sh
```

**Access Kafka UI:** Open http://localhost:8080 in your browser

---

## 📚 All Examples Overview

| Example | Difficulty | Description | Quick Link |
|---------|-----------|-------------|------------|
| **01-fundamentals** | ⭐ Beginner | Basic producer/consumer patterns | [Quick Start →](#1-fundamentals) |
| **02-core-concepts** | ⭐⭐ Intermediate | Event-carried state transfer | [Quick Start →](#2-core-concepts) |
| **03-producers** | ⭐⭐ Intermediate | Advanced producer patterns | [Quick Start →](#3-producers) |
| **04-build-e-commerce** | ⭐⭐⭐ Advanced | Complete e-commerce system | [Quick Start →](#4-build-e-commerce) |
| **05-advanced-kafka** | ⭐⭐⭐⭐ Expert | Production Kafka concepts | [Quick Start →](#5-advanced-kafka-concepts) |
| **06-event-sourcing** | ⭐⭐⭐⭐ Expert | Event sourcing with Kafka | [Quick Start →](#6-event-sourcing) |
| **06-saga** | ⭐⭐⭐⭐ Expert | Saga pattern (Choreography/Orchestration) | [Quick Start →](#7-saga-pattern) |
| **07-advanced-monitoring** | ⭐⭐⭐⭐ Expert | Observability & monitoring | [Quick Start →](#8-advanced-monitoring) |

---

## 🎓 Examples by Difficulty

### ⭐ Beginner Examples

---

### 1. Fundamentals

**📁 Location:** `examples/01-fundamentals/`

**What it demonstrates:**
- Basic Kafka producer/consumer patterns
- Message production and consumption
- Topic creation and message flow
- Kafka UI exploration

**Quick Start:**
```bash
cd examples/01-fundamentals/dotnet

# Run automated script (builds and runs producer then consumer)
./run-example.sh

# OR run manually:
dotnet run --project BasicProducer.csproj
# In another terminal:
dotnet run --project BasicConsumer.csproj
```

**Expected Output:**

Producer:
```
Delivered to my-topic [[0]] @0
Delivered to my-topic [[0]] @1
Delivered to my-topic [[0]] @2
```

Consumer:
```
Received: {"id":0,"value":"Message 0"}
Received: {"id":1,"value":"Message 1"}
```

**📖 Full Guide:** [examples/01-fundamentals/HOW-TO-RUN.md](../examples/01-fundamentals/HOW-TO-RUN.md)

**🎨 View in Kafka UI:** http://localhost:8080 → Topics → my-topic → Messages

---

### ⭐⭐ Intermediate Examples

---

### 2. Core Concepts

**📁 Location:** `examples/02-core-concepts/`

**What it demonstrates:**
- Event-Carried State Transfer pattern
- Autonomous services with full state in events
- Decoupled service communication

**Quick Start:**
```bash
cd examples/02-core-concepts/dotnet/event-carried-state-trf
dotnet build
dotnet run --project Examples/Examples.csproj
```

**📖 Full Guide:** [examples/02-core-concepts/dotnet/event-carried-state-trf/HOW-TO-RUN.md](../examples/02-core-concepts/dotnet/event-carried-state-trf/HOW-TO-RUN.md)

---

### 3. Producers

**📁 Location:** `examples/03-producers/`

**What it demonstrates:**
- Advanced producer patterns
- Different producer configurations
- Message batching and compression

**Quick Start:**
```bash
cd examples/03-producers/dotnet
dotnet build
dotnet run --project [ProjectName].csproj
```

**📖 Full Guide:** Check `examples/03-producers/README.md` for available examples

---

### ⭐⭐⭐ Advanced Examples

---

### 4. Build E-Commerce

**📁 Location:** `examples/04-build-e-commerce/`

**What it demonstrates:**
- Complete event-driven e-commerce system
- Multiple microservices (Order, Inventory, Payment, Shipping)
- Event-driven workflows
- Service-to-service communication via Kafka

**Quick Start:**
```bash
cd examples/04-build-e-commerce

# Build all services
dotnet build

# Run each service in separate terminals:
# Terminal 1: Order Service
dotnet run --project OrderService/OrderService.csproj

# Terminal 2: Inventory Service
dotnet run --project InventoryService/InventoryService.csproj

# Terminal 3: Payment Service
dotnet run --project PaymentService/PaymentService.csproj

# Terminal 4: Shipping Service
dotnet run --project ShippingService/ShippingService.csproj
```

**📖 Full Guide:** [examples/04-build-e-commerce/HOW-TO-RUN.md](../examples/04-build-e-commerce/HOW-TO-RUN.md)

**💡 Tip:** This example requires 4 terminal windows. See the HOW-TO-RUN guide for complete setup.

---

### ⭐⭐⭐⭐ Expert Examples

---

### 5. Advanced Kafka Concepts

**📁 Location:** `examples/05-advanced-kafka/dotnet/`

**What it demonstrates:**
- Delivery semantics (At-Most-Once, At-Least-Once, Exactly-Once)
- Log compaction for state management
- Kafka Streams simulation
- Performance tuning (batching, compression, parallel processing)
- Security (SSL, SASL)
- Multi-datacenter replication

**Structure:**
```
05-advanced-kafka/dotnet/
├── 01-delivery-semantics/      # Message delivery guarantees
├── 02-log-compaction/           # State management
├── 03-kafka-streams/           # Stream processing
├── 04-performance-tuning/      # Performance optimization
├── 05-security/                # SSL and SASL
└── 06-multi-dc-replication/    # Multi-datacenter replication
```

**Quick Start - Delivery Semantics:**
```bash
cd examples/05-advanced-kafka/dotnet/01-delivery-semantics
dotnet run --project AtMostOnce/AtMostOnce.csproj
```

**Quick Start - Log Compaction:**
```bash
cd examples/05-advanced-kafka/dotnet/02-log-compaction
dotnet run --project Producer/Producer.csproj
# In another terminal:
dotnet run --project Consumer/Consumer.csproj
```

**Quick Start - Performance Tuning:**
```bash
cd examples/05-advanced-kafka/dotnet/04-performance-tuning
dotnet run --project Producer/Producer.csproj
```

**📖 Full Guides:**
- [Main README](../examples/05-advanced-kafka/dotnet/README.md)
- [HOW-TO-RUN](../examples/05-advanced-kafka/dotnet/HOW-TO-RUN.md)
- [01-delivery-semantics](../examples/05-advanced-kafka/dotnet/01-delivery-semantics/HOW-TO-RUN.md)
- [02-log-compaction](../examples/05-advanced-kafka/dotnet/02-log-compaction/HOW-TO-RUN.md)
- [03-kafka-streams](../examples/05-advanced-kafka/dotnet/03-kafka-streams/HOW-TO-RUN.md)
- [04-performance-tuning](../examples/05-advanced-kafka/dotnet/04-performance-tuning/HOW-TO-RUN.md)
- [05-security](../examples/05-advanced-kafka/dotnet/05-security/HOW-TO-RUN.md)
- [06-multi-dc-replication](../examples/05-advanced-kafka/dotnet/06-multi-dc-replication/HOW-TO-RUN.md)

---

### 6. Event Sourcing

**📁 Location:** `examples/06-event-sourcing/dotnet/`

**What it demonstrates:**
- Event sourcing pattern with Kafka
- Domain events and aggregate root
- Event store implementation
- Event replay and time travel
- Optimistic locking and snapshots

**Quick Start:**
```bash
cd examples/06-event-sourcing/dotnet

# Run OrderService example
dotnet run --project Examples/OrderService.csproj

# Run EventReplay example
dotnet run --project Examples/EventReplay.csproj

# Run TimeTravel example
dotnet run --project Examples/TimeTravel.csproj
```

**📖 Full Guide:** [examples/06-event-sourcing/dotnet/HOW-TO-RUN.md](../examples/06-event-sourcing/dotnet/HOW-TO-RUN.md)

**💡 Tip:** See `STEP-BY-STEP.md` for a detailed walkthrough of the event sourcing pattern.

---

### 7. Saga Pattern

**📁 Location:** `examples/06-saga/dotnet/`

**What it demonstrates:**
- Saga pattern implementation
- Choreography pattern (event-driven)
- Orchestration pattern (centralized coordinator)
- Saga state persistence
- Compensating transactions

**Quick Start - Choreography:**
```bash
cd examples/06-saga/dotnet
dotnet run --project Choreography/ChoreographyExample.csproj
```

**Quick Start - Orchestration:**
```bash
cd examples/06-saga/dotnet
dotnet run --project Orchestration/OrchestrationExample.csproj
```

**Quick Start - Persistence:**
```bash
cd examples/06-saga/dotnet
dotnet run --project Orchestration/PersistenceExample.csproj
```

**📖 Full Guide:** [examples/06-saga/dotnet/HOW-TO-RUN.md](../examples/06-saga/dotnet/HOW-TO-RUN.md)

---

### 8. Advanced Monitoring

**📁 Location:** `examples/07-advanced-monitoring/`

**What it demonstrates:**
- Distributed tracing with OpenTelemetry
- Prometheus metrics
- Grafana dashboards
- Structured logging
- Observability best practices

**Quick Start:**
```bash
cd examples/07-advanced-monitoring

# Start monitoring stack (Prometheus, Grafana, Jaeger)
docker compose up -d

# Run services with observability
dotnet run --project [ServiceName]/[ServiceName].csproj
```

**📖 Full Guide:** Check `examples/07-advanced-monitoring/README.md` for setup instructions

**Access Dashboards:**
- Grafana: http://localhost:3000
- Prometheus: http://localhost:9090
- Jaeger: http://localhost:16686

---

## 🛠️ Common Commands

### Docker Management

```bash
# Start Kafka
./scripts/start-kafka.sh

# Stop Kafka
docker compose down

# Stop and remove all data
docker compose down -v

# View logs
docker compose logs -f kafka

# Restart a service
docker compose restart kafka

# Check status
docker compose ps
```

### Docker Service (WSL)

```bash
# Start Docker daemon
./scripts/start-docker.sh

# Check Docker status
docker info

# Start Docker manually
sudo service docker start
```

### .NET Commands

```bash
# Build a project
dotnet build

# Run a project
dotnet run --project [ProjectName].csproj

# Clean build artifacts
dotnet clean

# Restore packages
dotnet restore
```

---

## 🔧 Troubleshooting

### "Cannot connect to Docker daemon"

```bash
./scripts/start-docker.sh
```

### "Permission denied" when running Docker

```bash
# Add yourself to docker group
sudo usermod -aG docker $USER

# Then logout and login, or:
newgrp docker
```

### Port already in use

```bash
# Stop all containers
docker compose down

# Check what's using the port
sudo lsof -i :9092
```

### Kafka not responding

```bash
# Restart services
docker compose restart

# Check logs
docker compose logs kafka

# Fix Kafka issues
./scripts/fix-kafka.sh
```

### Producer hanging or timing out

See: [docs/PRODUCER-TROUBLESHOOTING.md](PRODUCER-TROUBLESHOOTING.md)

### Consumer not receiving messages

- Make sure producer ran first
- Check consumer is subscribed to the correct topic
- Verify Kafka is running: `docker compose ps`
- Check consumer group: http://localhost:8080 → Consumer Groups

---

## 📁 Project Structure

```
kafka-event-driven-architecture/
├── docker-compose.yml          # Kafka infrastructure definition
├── scripts/                    # Helper scripts
│   ├── install-docker.sh       # Docker Engine installation
│   ├── start-docker.sh         # Start Docker daemon
│   ├── start-kafka.sh          # Start Kafka services
│   ├── verify-docker.sh        # Verify Kafka is running
│   ├── fix-kafka.sh            # Fix Kafka issues
│   └── diagnose-kafka.sh      # Diagnostic tool
├── examples/
│   ├── 01-fundamentals/        # Basic producer/consumer
│   ├── 02-core-concepts/       # Event-carried state transfer
│   ├── 03-producers/           # Advanced producer patterns
│   ├── 04-build-e-commerce/    # Complete e-commerce system
│   ├── 05-advanced-kafka/      # Production Kafka concepts
│   ├── 06-event-sourcing/      # Event sourcing pattern
│   ├── 06-saga/                # Saga pattern
│   └── 07-advanced-monitoring/ # Observability & monitoring
└── docs/
    ├── QUICKSTART.md           # This file
    ├── SETUP.md                 # Original setup guide
    └── TROUBLESHOOTING.md      # Troubleshooting guide
```

---

## 🎯 Learning Path

### Recommended Order:

1. **Start Here:** [01-fundamentals](#1-fundamentals) - Learn basic producer/consumer
2. **Next:** [02-core-concepts](#2-core-concepts) - Understand event patterns
3. **Then:** [04-build-e-commerce](#4-build-e-commerce) - Build a complete system
4. **Advanced:** [05-advanced-kafka](#5-advanced-kafka-concepts) - Production concepts
5. **Expert:** [06-event-sourcing](#6-event-sourcing) - Event sourcing pattern
6. **Expert:** [06-saga](#7-saga-pattern) - Distributed transactions
7. **Expert:** [07-advanced-monitoring](#8-advanced-monitoring) - Observability

---

## 🎨 Explore Kafka UI

After starting Kafka, open **http://localhost:8080** to:

- **View Topics** - See all Kafka topics and their messages
- **Browse Messages** - Inspect message content and metadata
- **Monitor Consumer Groups** - Track consumer lag and offsets
- **See Broker Metrics** - Monitor Kafka cluster health
- **Explore Partitions** - Understand topic partitioning

---

## 📚 Additional Resources

### Documentation

- **[EXAMPLES-GUIDE.md](../EXAMPLES-GUIDE.md)** - Comprehensive guide for all examples
- **[docs/SETUP.md](SETUP.md)** - Original setup documentation
- **[docs/TROUBLESHOOTING.md](TROUBLESHOOTING.md)** - Troubleshooting guide
- **[docs/PRODUCER-TROUBLESHOOTING.md](PRODUCER-TROUBLESHOOTING.md)** - Producer-specific issues

### External Resources

- [Kafka Documentation](https://kafka.apache.org/documentation/)
- [Confluent Kafka .NET](https://docs.confluent.io/kafka-clients/dotnet/current/overview.html)
- [Event-Driven Architecture Patterns](https://martinfowler.com/articles/201701-event-driven.html)

---

## 🆘 Need Help?

### Check Logs
```bash
docker compose logs -f
```

### Restart Everything
```bash
docker compose down -v
./scripts/start-kafka.sh
```

### Diagnostic Tools
```bash
./scripts/diagnose-kafka.sh
```

### Still Stuck?

Make sure:
- ✅ Docker daemon is running: `docker info`
- ✅ No port conflicts: `docker compose ps`
- ✅ Sufficient disk space: `df -h`
- ✅ Kafka services are healthy: `./scripts/verify-docker.sh`

---

## 🎉 Next Steps

1. ✅ Complete the [01-fundamentals](#1-fundamentals) example
2. ✅ Explore Kafka UI at http://localhost:8080
3. ✅ Try modifying examples to send different messages
4. ✅ Experiment with consumer groups
5. ✅ Build your own event-driven application!

**Happy coding! 🚀**
