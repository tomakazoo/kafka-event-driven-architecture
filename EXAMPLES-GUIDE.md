# 📚 Complete Examples Guide

Quick reference guide for running all Kafka event-driven architecture examples.

---

## 🎯 Prerequisites

### System Requirements
- Docker & Docker Compose (or Docker Engine for WSL 2)
- .NET 8.0+ SDK
- Git

### Initial Setup (One-Time)

**For WSL 2 (without Docker Desktop):**
```bash
# 1. Install Docker Engine
./scripts/install-docker.sh

# 2. Activate docker group
newgrp docker

# 3. Start Docker daemon
./scripts/start-docker.sh
```

**For systems with Docker Desktop:**
- Just ensure Docker Desktop is running

---

## 📋 All Examples Overview

| Example | Difficulty | Description | Quick Link |
|---------|-----------|-------------|------------|
| **01-fundamentals** | ⭐ Beginner | Basic producer/consumer patterns | [→ Example 1](#example-1-fundamentals) |
| **02-core-concepts** | ⭐⭐ Intermediate | Event-carried state transfer | [→ Example 2](#example-2-core-concepts-event-carried-state-transfer) |
| **03-producers** | ⭐⭐ Intermediate | Advanced producer patterns | [→ Example 3](#example-3-producers) |
| **04-build-e-commerce** | ⭐⭐⭐ Advanced | Complete e-commerce system | [→ Example 4](#example-4-build-e-commerce) |
| **05-advanced-kafka** | ⭐⭐⭐⭐ Expert | Production Kafka concepts (6 sub-examples) | [→ Example 5](#example-5-advanced-kafka-concepts) |
| **06-event-sourcing** | ⭐⭐⭐⭐ Expert | Event sourcing with Kafka | [→ Example 6](#example-6-event-sourcing) |
| **06-saga** | ⭐⭐⭐⭐ Expert | Saga pattern (Choreography/Orchestration) | [→ Example 7](#example-7-saga-pattern) |
| **07-advanced-monitoring** | ⭐⭐⭐⭐ Expert | Observability & monitoring | [→ Example 8](#example-8-advanced-monitoring) |

---

## 📁 Example 1: Fundamentals (`examples/01-fundamentals`)

### What It Demonstrates
- Basic Kafka producer/consumer patterns
- Message production and consumption
- Topic creation and message flow
- Kafka UI exploration

### Quick Start

**Step 1: Start Kafka Infrastructure**
```bash
cd /home/babicto/projects/kafka-event-driven-architecture
./scripts/start-kafka.sh
./scripts/verify-docker.sh
```

**Step 2: Run Producer**
```bash
cd examples/01-fundamentals/dotnet
dotnet run --project BasicProducer.csproj
```

**Expected Output:**
```
🚀 Starting Kafka Producer...
📡 Connecting to: localhost:9092
✅ Producer created successfully
📤 Producing to topic: my-topic
✅ Delivered to my-topic [[0]] @0
...
✅ All messages delivered successfully!
```

**Step 3: Run Consumer** (in a new terminal)
```bash
cd examples/01-fundamentals/dotnet
dotnet run --project BasicConsumer.csproj
```

**Expected Output:**
```
Received: {"id":0,"value":"Message 0"}
Received: {"id":1,"value":"Message 1"}
...
```

**Step 4: View in Kafka UI**
- Open: http://localhost:8080
- Navigate to: Topics → my-topic → Messages

### Architecture
```
Producer → Kafka Broker → Consumer
```

### Troubleshooting

**Kafka won't start?**
```bash
./scripts/fix-kafka.sh
```

**Consumer not receiving messages?**
- Make sure producer ran first
- Check consumer is subscribed to `my-topic`
- Verify Kafka is running: `docker compose ps`

**Port conflicts?**
```bash
docker compose down
./scripts/start-kafka.sh
```

**View logs:**
```bash
docker compose logs kafka
docker compose logs zookeeper
```

**📖 Full Guide:** [examples/01-fundamentals/HOW-TO-RUN.md](examples/01-fundamentals/HOW-TO-RUN.md)

---

## 📁 Example 2: Core Concepts - Event-Carried State Transfer (`examples/02-core-concepts`)

### What It Demonstrates
- Event-Carried State Transfer pattern
- Autonomous services with full state in events
- Decoupled service communication
- No API calls between services

### Quick Start

**Step 1: Start Kafka Infrastructure**
```bash
cd /home/babicto/projects/kafka-event-driven-architecture
./scripts/start-kafka.sh
./scripts/verify-docker.sh
```

**Step 2: Build Solution**
```bash
cd examples/02-core-concepts/dotnet/event-carried-state-trf
dotnet build EventCarriedStateTransfer.sln
```

**Step 3: Run Example**
```bash
dotnet run --project Examples/Examples.csproj
```

**Expected Flow:**
1. OrderService creates an order with full data
2. Publishes `OrderPlaced` event containing:
   - Order details
   - Customer info (denormalized)
   - Line items (complete)
   - Shipping address
3. Three autonomous consumers process the event:
   - **EmailService** - Sends confirmation email (no API calls!)
   - **InventoryService** - Updates inventory (no API calls!)
   - **AnalyticsService** - Records analytics (no API calls!)

**Step 4: View in Kafka UI**
- Open: http://localhost:8080
- Navigate to: Topics → orders → Messages
- See the rich event payload with all data

### Architecture
```
OrderService (Producer)
    ↓ (OrderPlaced event with FULL data)
Kafka Topic: "orders"
    ↓
┌─────────────────┬─────────────────┬─────────────────┐
│ EmailService    │ InventoryService │ AnalyticsService │
│ (Autonomous)    │ (Autonomous)     │ (Autonomous)     │
│                 │                  │                  │
│ No API calls!   │ No API calls!    │ No API calls!    │
└─────────────────┴─────────────────┴─────────────────┘
```

### Key Concepts
- **Full Data in Events**: Events contain denormalized, complete data
- **Autonomous Consumers**: Services don't need to call other services
- **Decoupling**: Services operate independently
- **Resilience**: If one service is down, others continue working

### Troubleshooting

**Build errors?**
```bash
cd examples/02-core-concepts/dotnet/event-carried-state-trf
dotnet clean
dotnet restore
dotnet build
```

**Topic not available?**
- The example creates the topic automatically
- If it fails, check Kafka is running: `docker compose ps`

**📖 Full Guide:** [examples/02-core-concepts/dotnet/event-carried-state-trf/HOW-TO-RUN.md](examples/02-core-concepts/dotnet/event-carried-state-trf/HOW-TO-RUN.md)

---

## 📁 Example 3: Producers (`examples/03-producers`)

### What It Demonstrates
- Advanced producer patterns
- Different producer configurations
- Message batching and compression
- Producer best practices

### Quick Start

**Step 1: Start Kafka Infrastructure**
```bash
cd /home/babicto/projects/kafka-event-driven-architecture
./scripts/start-kafka.sh
./scripts/verify-docker.sh
```

**Step 2: Check Available Examples**
```bash
cd examples/03-producers/dotnet
ls -la
```

**Note:** This folder structure is prepared for future producer pattern examples. Check the folder for available examples.

### Troubleshooting

**No examples found?**
- This folder may be under development
- Check for updates or see other examples for producer patterns

---

## 📁 Example 4: Build E-Commerce (`examples/04-build-e-commerce`)

### What It Demonstrates
- Multi-service event-driven architecture
- Service decoupling via Kafka events
- Event dependencies (Shipping waits for multiple events)
- Error handling and rejection flows

### Quick Start

**Step 1: Start Kafka Infrastructure**
```bash
cd /home/babicto/projects/kafka-event-driven-architecture
./scripts/start-kafka.sh
./scripts/verify-docker.sh
```

**Step 2: Create Topics** (Recommended)
```bash
cd examples/04-build-e-commerce/dotnet
./create-topics.sh
```

**Step 3: Start Services** (4 separate terminals)

**Terminal 1 - Customer Service:**
```bash
cd examples/04-build-e-commerce/dotnet
dotnet run --project CustomerService/CustomerService.csproj
```

**Terminal 2 - Inventory Service:**
```bash
cd examples/04-build-e-commerce/dotnet
dotnet run --project InventoryService/InventoryService.csproj
```

**Terminal 3 - Shipping Service:**
```bash
cd examples/04-build-e-commerce/dotnet
dotnet run --project ShippingService/ShippingService.csproj
```

**Terminal 4 - Order Service** (creates orders):
```bash
cd examples/04-build-e-commerce/dotnet
dotnet run --project OrderService/OrderService.csproj
```

**Expected Flow:**
1. Order Service creates orders → `orders-created` topic
2. Customer Service validates → `orders-validated` or `orders-rejected`
3. Inventory Service reserves → `inventory-reserved` or `inventory-insufficient`
4. Shipping Service ships when BOTH validated AND reserved → `orders-shipped`

**Step 4: View in Kafka UI**
- Open: http://localhost:8080
- Navigate to Topics to see all event topics
- Check Consumers to see consumer groups

### Architecture
```
Order Service
    ↓ (orders-created)
┌─────────────────────────┐
│ Customer Service        │ → orders-validated / orders-rejected
│ Inventory Service       │ → inventory-reserved / inventory-insufficient
└─────────────────────────┘
    ↓
Shipping Service (waits for both)
    ↓ (orders-shipped)
```

### Topics Created
- `orders-created` - New orders
- `orders-validated` - Validated orders
- `orders-rejected` - Rejected orders
- `inventory-reserved` - Reserved inventory
- `inventory-insufficient` - Insufficient inventory
- `orders-shipped` - Shipped orders

### Troubleshooting

**Services not receiving messages?**
1. Check Kafka is running: `docker compose ps`
2. Verify topics exist:
   ```bash
   docker compose exec kafka kafka-topics --list --bootstrap-server localhost:9092
   ```
3. Run `./create-topics.sh` if topics are missing
4. Restart services in order: Customer → Inventory → Shipping → Order

**"Topic not available" errors?**
```bash
# Create topics manually
cd examples/04-build-e-commerce/dotnet
./create-topics.sh

# Or start Order Service first to create orders-created topic
```

**Build errors?**
```bash
cd examples/04-build-e-commerce/dotnet
dotnet clean
dotnet build
```

**Services hanging?**
- Check Kafka connectivity: `./scripts/verify-docker.sh`
- Check service logs in each terminal
- Verify all services are connected to same Kafka broker

**📖 Full Guide:** [examples/04-build-e-commerce/HOW-TO-RUN.md](examples/04-build-e-commerce/HOW-TO-RUN.md)

---

## 📁 Example 5: Advanced Kafka Concepts (`examples/05-advanced-kafka/dotnet`)

### What It Demonstrates
This folder contains **6 comprehensive sub-examples** covering production Kafka concepts:

1. **Delivery Semantics** - At-most-once, at-least-once, exactly-once guarantees
2. **Log Compaction** - State management with compacted topics
3. **Kafka Streams** - Real-time stream processing (simulated)
4. **Performance Tuning** - Batching, compression, parallel processing
5. **Security** - SSL/TLS encryption and SASL authentication
6. **Multi-DC Replication** - MirrorMaker 2 concepts

### Structure
```
05-advanced-kafka/dotnet/
├── 01-delivery-semantics/      # Message delivery guarantees
├── 02-log-compaction/          # State management
├── 03-kafka-streams/           # Stream processing
├── 04-performance-tuning/      # Performance optimization
├── 05-security/                # SSL and SASL
├── 06-multi-dc-replication/    # Multi-datacenter replication
├── AdvancedKafkaConcepts.sln   # Solution file
└── README.md                    # Main README
```

### Quick Start

**Step 1: Start Kafka Infrastructure**
```bash
cd /home/babicto/projects/kafka-event-driven-architecture
./scripts/start-kafka.sh
./scripts/verify-docker.sh
```

**Step 2: Run a Sub-Example**

Each sub-example has its own folder with detailed instructions. See below for each one.

---

### 5.1 Delivery Semantics (`01-delivery-semantics`)

**What it demonstrates:**
- At-Most-Once: Fast but unsafe (messages may be lost)
- At-Least-Once: Safe but may duplicate (default)
- Exactly-Once: Safe and no duplicates (transactions)

**Quick Start:**
```bash
cd examples/05-advanced-kafka/dotnet/01-delivery-semantics

# At-Most-Once
dotnet run --project AtMostOnce/AtMostOnce.csproj

# At-Least-Once (with Redis deduplication)
dotnet run --project AtLeastOnce/AtLeastOnce.csproj

# Exactly-Once (transactional)
dotnet run --project ExactlyOnce/ExactlyOnce.csproj
```

**📖 Full Guide:** [examples/05-advanced-kafka/dotnet/01-delivery-semantics/HOW-TO-RUN.md](examples/05-advanced-kafka/dotnet/01-delivery-semantics/HOW-TO-RUN.md)

---

### 5.2 Log Compaction (`02-log-compaction`)

**What it demonstrates:**
- Topic compaction configuration (`cleanup.policy=compact`)
- Latest state per key preservation
- State store reconstruction
- Tombstone messages for deletion

**Quick Start:**
```bash
cd examples/05-advanced-kafka/dotnet/02-log-compaction

# Terminal 1: Producer
dotnet run --project Producer/Producer.csproj

# Terminal 2: Consumer
dotnet run --project Consumer/Consumer.csproj
```

**📖 Full Guide:** [examples/05-advanced-kafka/dotnet/02-log-compaction/HOW-TO-RUN.md](examples/05-advanced-kafka/dotnet/02-log-compaction/HOW-TO-RUN.md)

---

### 5.3 Kafka Streams (`03-kafka-streams`)

**What it demonstrates:**
- Stream processing topology simulation
- Filtering and aggregation
- Stateful operations
- Real-time analytics

**Quick Start:**
```bash
cd examples/05-advanced-kafka/dotnet/03-kafka-streams

# Run the streams demo
dotnet run --project Examples/Examples.csproj
```

**Note:** .NET doesn't have native Kafka Streams. This example simulates stream processing concepts. For production, consider Kafka Streams (Java) or ksqlDB.

**📖 Full Guide:** [examples/05-advanced-kafka/dotnet/03-kafka-streams/HOW-TO-RUN.md](examples/05-advanced-kafka/dotnet/03-kafka-streams/HOW-TO-RUN.md)

---

### 5.4 Performance Tuning (`04-performance-tuning`)

**What it demonstrates:**
- Producer batching and compression
- Parallel consumer processing
- Performance optimization techniques
- Throughput comparison (baseline vs tuned)

**Quick Start:**
```bash
cd examples/05-advanced-kafka/dotnet/04-performance-tuning

# Baseline producer
dotnet run --project Producer/Producer.csproj

# Tuned producer (modify StartupObject)
dotnet run --project Producer/Producer.csproj

# Parallel consumer
dotnet run --project Consumer/Consumer.csproj
```

**📖 Full Guide:** [examples/05-advanced-kafka/dotnet/04-performance-tuning/HOW-TO-RUN.md](examples/05-advanced-kafka/dotnet/04-performance-tuning/HOW-TO-RUN.md)

---

### 5.5 Security (`05-security`)

**What it demonstrates:**
- SSL/TLS encryption for Kafka
- SASL authentication (PLAIN mechanism)
- Secure producer/consumer configuration

**Quick Start:**
```bash
cd examples/05-advanced-kafka/dotnet/05-security

# SSL example
dotnet run --project SSL/SSL.csproj

# SASL example
dotnet run --project SASL/SASL.csproj
```

**⚠️ Note:** Requires broker-side SSL/SASL configuration. See the README for setup instructions.

**📖 Full Guide:** [examples/05-advanced-kafka/dotnet/05-security/HOW-TO-RUN.md](examples/05-advanced-kafka/dotnet/05-security/HOW-TO-RUN.md)

---

### 5.6 Multi-DC Replication (`06-multi-dc-replication`)

**What it demonstrates:**
- MirrorMaker 2 concepts
- Multi-datacenter replication patterns
- Topic replication configuration

**Quick Start:**
```bash
cd examples/05-advanced-kafka/dotnet/06-multi-dc-replication

# DC1 Producer
dotnet run --project Producer/Producer.csproj

# DC2 Consumer
dotnet run --project Consumer/Consumer.csproj
```

**⚠️ Note:** Full replication requires multiple Kafka clusters. This example demonstrates concepts with a single cluster.

**📖 Full Guide:** [examples/05-advanced-kafka/dotnet/06-multi-dc-replication/HOW-TO-RUN.md](examples/05-advanced-kafka/dotnet/06-multi-dc-replication/HOW-TO-RUN.md)

**📖 Main Guide:** [examples/05-advanced-kafka/dotnet/HOW-TO-RUN.md](examples/05-advanced-kafka/dotnet/HOW-TO-RUN.md) | [README.md](examples/05-advanced-kafka/dotnet/README.md)

---

## 📁 Example 6: Event Sourcing (`examples/06-event-sourcing/dotnet`)

### What It Demonstrates
- Event sourcing pattern with Kafka as event store
- Aggregate pattern with domain events
- Event replay to rebuild state
- Time travel queries (view state at any point in time)
- Optimistic locking for concurrency control
- Snapshots for performance optimization

### Quick Start

**Step 1: Start Kafka Infrastructure**
```bash
cd /home/babicto/projects/kafka-event-driven-architecture
./scripts/start-kafka.sh
./scripts/verify-docker.sh
```

**Step 2: Build Projects**
```bash
cd examples/06-event-sourcing/dotnet
dotnet build EventSourcing.sln
```

**Step 3: Run Examples**

**Order Service** (Basic event sourcing):
```bash
dotnet run --project Examples/OrderService.csproj
```

**Event Replay** (Rebuild state from events):
```bash
dotnet run --project Examples/EventReplay.csproj
```

**Time Travel** (View state at different points in time):
```bash
dotnet run --project Examples/TimeTravel.csproj
```

**Expected Flow:**
1. Order created → `OrderCreated` event
2. Items added → `ItemAdded` events
3. Address set → `ShippingAddressSet` event
4. Order submitted → `OrderSubmitted` event
5. Payment received → `PaymentReceived` event
6. All events stored in Kafka topic `order-events`
7. State rebuilt by replaying events

**Step 4: View Events in Kafka UI**
- Open: http://localhost:8080
- Navigate to: Topics → order-events → Messages
- See all domain events with versions and timestamps

### Architecture
```
Order Aggregate
    ↓ (Commands)
Domain Events
    ↓
Kafka Event Store (order-events topic)
    ↓
Event Replay → Rebuild State
```

### Key Concepts
- **Event Sourcing**: Store events instead of current state
- **Aggregate**: Order aggregate with business logic
- **Event Store**: Kafka topic stores all events
- **Event Replay**: Rebuild state by applying events
- **Time Travel**: Query state at any timestamp
- **Optimistic Locking**: Version-based concurrency control

### Troubleshooting

**Kafka not running?**
```bash
./scripts/start-kafka.sh
./scripts/verify-docker.sh
```

**Build errors?**
```bash
cd examples/06-event-sourcing/dotnet
dotnet clean
dotnet restore
dotnet build
```

**Events not appearing?**
- Check Kafka topic: `docker compose exec kafka kafka-topics --list --bootstrap-server localhost:9092`
- View in Kafka UI: http://localhost:8080
- Verify events are being published

**ConcurrencyException?**
- Expected when two processes modify same aggregate
- Retry with latest version

**📖 Full Guide:** [examples/06-event-sourcing/dotnet/HOW-TO-RUN.md](examples/06-event-sourcing/dotnet/HOW-TO-RUN.md) | [README.md](examples/06-event-sourcing/dotnet/README.md)

---

## 📁 Example 7: Saga Pattern (`examples/06-saga/dotnet`)

### What It Demonstrates
- Saga pattern implementation
- Choreography pattern (event-driven)
- Orchestration pattern (centralized coordinator)
- Saga state persistence
- Compensating transactions

### Quick Start

**Step 1: Start Kafka Infrastructure**
```bash
cd /home/babicto/projects/kafka-event-driven-architecture
./scripts/start-kafka.sh
./scripts/verify-docker.sh
```

**Step 2: Build Projects**
```bash
cd examples/06-saga/dotnet
dotnet build SagaPattern.sln
```

**Step 3: Run Examples**

**Choreography Example:**
```bash
dotnet run --project Examples/ChoreographyExample.csproj
```

**Orchestration Example:**
```bash
dotnet run --project Examples/OrchestrationExample.csproj
```

**Persistence Example:**
```bash
dotnet run --project Orchestration/PersistenceExample.csproj
```

**Expected Flow:**

**Choreography:**
1. Order Service publishes `OrderPlacedEvent`
2. Inventory Service reacts → `InventoryReservedEvent` or `InventoryInsufficientEvent`
3. Payment Service reacts → `PaymentReceivedEvent` or `PaymentFailedEvent`
4. Services coordinate via events (no central orchestrator)

**Orchestration:**
1. Saga Orchestrator coordinates all steps
2. Step 1: Reserve Inventory
3. Step 2: Process Payment
4. Step 3: Schedule Shipping
5. If any step fails, compensations run automatically

### Architecture

**Choreography Pattern:**
```
Order Service
    ↓ (OrderPlacedEvent)
Inventory Service → (InventoryReservedEvent)
    ↓
Payment Service → (PaymentReceivedEvent)
    ↓
Order Fulfilled
```

**Orchestration Pattern:**
```
Saga Orchestrator
    ↓
Step 1: Reserve Inventory
    ↓
Step 2: Process Payment
    ↓
Step 3: Schedule Shipping
    ↓
Order Fulfilled
```

### Key Concepts
- **Saga Pattern**: Distributed transaction management across multiple services
- **Choreography**: Services coordinate via events (no central orchestrator)
- **Orchestration**: Central orchestrator coordinates all steps
- **Compensating Transactions**: Rollback mechanism for failed sagas
- **Saga State Persistence**: Recover sagas after failures

### Troubleshooting

**Kafka not running?**
```bash
./scripts/start-kafka.sh
./scripts/verify-docker.sh
```

**Build errors?**
```bash
cd examples/06-saga/dotnet
dotnet clean
dotnet restore
dotnet build
```

**Events not flowing?**
- Check Kafka topics: `docker compose exec kafka kafka-topics --list --bootstrap-server localhost:9092`
- View in Kafka UI: http://localhost:8080
- Verify services are subscribed to correct topics

**📖 Full Guide:** [examples/06-saga/dotnet/HOW-TO-RUN.md](examples/06-saga/dotnet/HOW-TO-RUN.md) | [README.md](examples/06-saga/dotnet/README.md)

---

## 📁 Example 8: Advanced Monitoring (`examples/07-advanced-monitoring`)

### What It Demonstrates
- Full observability stack (Prometheus, Grafana, Jaeger)
- Distributed tracing with correlation IDs
- Horizontal scaling (2 NAV calculator instances)
- Interactive UI for triggering events
- Load testing capabilities

### Quick Start

**Step 1: Start All Services**
```bash
cd examples/07-advanced-monitoring/dotnet
./start.sh
```

**Or manually:**
```bash
cd examples/07-advanced-monitoring/dotnet
docker compose up --build -d
```

**Wait for services to be healthy** (~2 minutes)

**Step 2: Access Dashboards**

| Service | URL | Credentials |
|---------|-----|-------------|
| **Demo UI** | http://localhost:3001 | None |
| **Grafana** | http://localhost:3000 | admin/admin |
| **Jaeger** | http://localhost:16686 | None |
| **Prometheus** | http://localhost:9090 | None |

**Step 3: Generate Data**

**Option A: Use Demo UI**
1. Open http://localhost:3001
2. Click any fund card to trigger NAV calculation
3. Click "Simulate Month-End" to process all 28 funds

**Option B: Use Load Tester**
```bash
cd examples/07-advanced-monitoring/dotnet/src/Tools/LoadTester
dotnet run -- --funds 28 --concurrent true --iterations 3
```

**Option C: Use API**
```bash
curl -X POST http://localhost:5001/api/pricing/trigger \
  -H "Content-Type: application/json" \
  -d '{"fundId":"LUX-001","fundName":"Test Fund"}'
```

**Step 4: View Monitoring Data**

**Grafana:**
- Login: admin/admin
- Navigate to: Dashboards → Browse
- Look for "NAV Calculator Dashboard"
- Shows: Events/sec, processing duration, success rates

**Jaeger:**
- Open http://localhost:16686
- Select service: `pricing-service` or `nav-calculator-1`
- Click "Find Traces"
- Click any trace to see full request flow

**Prometheus:**
- Open http://localhost:9090
- Try queries:
  ```
  events_processed_total
  event_processing_duration_seconds
  events_processed_total{status="success"}
  ```

### Architecture
```
Pricing Service
    ↓ (pricing-updates)
NAV Calculator (2 instances)
    ↓ (nav-calculated)
Notification Service
```

**Observability:**
- Prometheus metrics at `/metrics` endpoint
- Jaeger traces with correlation IDs
- Grafana dashboards for visualization

### Service Endpoints

| Service | Health Check | Metrics |
|---------|-------------|---------|
| Pricing Service | http://localhost:5001/health | http://localhost:5001/metrics |
| NAV Calculator 1 | http://localhost:5002/health | http://localhost:5002/metrics |
| NAV Calculator 2 | http://localhost:5003/health | http://localhost:5003/metrics |
| Notification Service | http://localhost:5004/health | http://localhost:5004/metrics |

### Troubleshooting

**Services not starting?**
```bash
# Check status
docker compose ps

# View logs
docker compose logs pricing-service
docker compose logs nav-calculator-1
docker compose logs notification-service

# Restart
docker compose restart
```

**No data in Grafana?**
1. Check Prometheus targets: http://localhost:9090/targets
2. Verify services expose metrics: http://localhost:5001/metrics
3. Check Prometheus config: `monitoring/prometheus.yml`

**No traces in Jaeger?**
1. Verify services are sending traces (check logs)
2. Check Jaeger is running: `docker compose ps jaeger`
3. Look for correlation IDs in service logs

**Demo UI not working?**
1. Check pricing service: http://localhost:5001/health
2. Check browser console for errors
3. Verify demo-ui container: `docker compose ps demo-ui`

**Kafka connection issues?**
```bash
# Check Kafka is healthy
docker compose ps kafka

# View Kafka logs
docker compose logs kafka

# Restart Kafka
docker compose restart kafka
```

**Port conflicts?**
```bash
# Stop all services
docker compose down

# Check what's using ports
netstat -tulpn | grep -E "(3000|3001|5001|5002|5003|5004|9090|16686)"

# Restart
./start.sh
```

**Build errors?**
```bash
# Clean and rebuild
docker compose down
docker compose build --no-cache
docker compose up -d
```

**📖 Full Guide:** Check `examples/07-advanced-monitoring/dotnet/README.md` for setup instructions

---

## 🔧 Common Troubleshooting (All Examples)

### Docker Issues

**Docker daemon not running (WSL 2):**
```bash
./scripts/start-docker.sh
```

**Permission denied:**
```bash
# Add user to docker group
sudo usermod -aG docker $USER
newgrp docker
```

**Docker compose not found:**
- Use `docker compose` (space, not hyphen) for Docker Compose V2
- Or install Docker Compose separately

### Kafka Issues

**Kafka won't start:**
```bash
./scripts/fix-kafka.sh
```

**Kafka unhealthy:**
```bash
# Check logs
docker compose logs kafka

# Restart
docker compose restart kafka

# Full reset
docker compose down -v
./scripts/start-kafka.sh
```

**Can't connect to Kafka:**
```bash
# Verify Kafka is running
./scripts/verify-docker.sh

# Check port 9092 is accessible
nc -zv localhost 9092
```

**Topics not created:**
```bash
# List topics
docker compose exec kafka kafka-topics --list --bootstrap-server localhost:9092

# Create manually (if needed)
docker compose exec kafka kafka-topics --create \
  --bootstrap-server localhost:9092 \
  --topic my-topic \
  --partitions 1 \
  --replication-factor 1
```

### .NET Issues

**Build errors:**
```bash
# Clean and rebuild
dotnet clean
dotnet restore
dotnet build
```

**Missing dependencies:**
```bash
# Restore packages
dotnet restore

# Update packages
dotnet add package <package-name>
```

**Runtime errors:**
- Check .NET version: `dotnet --version` (should be 8.0+)
- Verify project files are correct
- Check service logs for detailed errors

### Network Issues

**Port already in use:**
```bash
# Find process using port
sudo lsof -i :9092
sudo lsof -i :8080
sudo lsof -i :3000

# Kill process (if needed)
sudo kill -9 <PID>
```

**Services can't communicate:**
- Verify all services use same Kafka broker: `localhost:9092`
- Check Docker network: `docker network ls`
- Ensure services are on same network

---

## 📊 Quick Reference

### Start Kafka (for examples 1-6)
```bash
./scripts/start-kafka.sh
./scripts/verify-docker.sh
```

### Stop Kafka
```bash
docker compose down
```

### Stop and Remove Data
```bash
docker compose down -v
```

### View Kafka UI
- http://localhost:8080

### View Logs
```bash
# All services
docker compose logs -f

# Specific service
docker compose logs -f kafka
docker compose logs -f pricing-service
```

### Check Service Health
```bash
# Docker services
docker compose ps

# HTTP endpoints (example 7 & 8)
curl http://localhost:5001/health
curl http://localhost:5002/health
curl http://localhost:5003/health
curl http://localhost:5004/health
```

---

## 🎯 Example Comparison

| Feature | 01-Fundamentals | 02-Core-Concepts | 04-E-Commerce | 05-Advanced-Kafka | 06-Event-Sourcing | 06-Saga | 07-Monitoring |
|---------|----------------|------------------|---------------|-------------------|-------------------|---------|---------------|
| **Services** | 2 (Producer, Consumer) | 4 (Order, Email, Inventory, Analytics) | 4 (Order, Customer, Inventory, Shipping) | 6 sub-examples | 3 Examples | 3 Examples | 3 (Pricing, NAV Calc, Notification) |
| **Kafka Setup** | Main docker-compose.yml | Main docker-compose.yml | Main docker-compose.yml | Main docker-compose.yml | Main docker-compose.yml | Main docker-compose.yml | Own docker-compose.yml |
| **Topics** | 1 (my-topic) | 1 (orders) | 6 topics | Varies by sub-example | 1 (order-events) | Multiple | 2 topics |
| **Monitoring** | Kafka UI only | Kafka UI only | Kafka UI only | Kafka UI only | Kafka UI only | Kafka UI only | Prometheus, Grafana, Jaeger |
| **UI** | None | None | None | None | None | None | React Demo UI |
| **Load Testing** | None | None | None | None | None | None | LoadTester tool |
| **Pattern** | Producer/Consumer | Event-Carried State Transfer | Event-Driven | Production Kafka | Event Sourcing | Saga | Observability |
| **Complexity** | ⭐ Basic | ⭐⭐ Intermediate | ⭐⭐⭐ Advanced | ⭐⭐⭐⭐ Expert | ⭐⭐⭐⭐ Expert | ⭐⭐⭐⭐ Expert | ⭐⭐⭐⭐ Expert |

---

## 📚 Additional Resources

- **Main README**: [README.md](README.md)
- **Quick Start Guide**: [docs/QUICKSTART.md](docs/QUICKSTART.md)
- **Troubleshooting**: [docs/TROUBLESHOOTING.md](docs/TROUBLESHOOTING.md)
- **Scripts Documentation**: [scripts/README.md](scripts/README.md)
- **Installation Notes**: [INSTALL-NOTES.txt](INSTALL-NOTES.txt)

---

## 🚀 Getting Help

1. **Check logs**: `docker compose logs <service-name>`
2. **Run diagnostics**: `./scripts/diagnose-kafka.sh`
3. **Fix common issues**: `./scripts/fix-kafka.sh`
4. **Verify setup**: `./scripts/verify-docker.sh`
5. **Read troubleshooting guide**: [docs/TROUBLESHOOTING.md](docs/TROUBLESHOOTING.md)

---

**Happy Event Streaming! 🎉**
