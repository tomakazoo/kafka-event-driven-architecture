# 🚀 Complete Setup and Run Guide

This guide provides step-by-step instructions for setting up and running each example in the Kafka Event-Driven Architecture project. Follow each section sequentially to test all examples.

---

## 📋 Table of Contents

1. [Prerequisites](#prerequisites)
2. [Initial Setup (One-Time)](#initial-setup-one-time)
3. [Example 1: Fundamentals](#example-1-fundamentals)
4. [Example 2: Core Concepts - Event-Carried State Transfer](#example-2-core-concepts---event-carried-state-transfer)
5. [Example 3: Producers](#example-3-producers)
6. [Example 4: Build E-Commerce](#example-4-build-e-commerce)
7. [Example 5: Advanced Kafka Concepts](#example-5-advanced-kafka-concepts)
8. [Example 6: Event Sourcing](#example-6-event-sourcing)
9. [Example 7: Saga Pattern](#example-7-saga-pattern)
10. [Example 8: Advanced Monitoring](#example-8-advanced-monitoring)
11. [Common Troubleshooting](#common-troubleshooting)

---

## Prerequisites

### System Requirements

- **Docker & Docker Compose** (or Docker Engine for WSL 2)
- **.NET 8.0+ SDK**
- **Git**
- **Terminal/Command Line Access**

### Verify Prerequisites

```bash
# Check Docker
docker --version
docker compose version

# Check .NET SDK
dotnet --version  # Should be 8.0 or higher

# Check Git
git --version
```

---

## Initial Setup (One-Time)

### For WSL 2 (without Docker Desktop)

If you're on WSL 2 without Docker Desktop, you need to set up Docker Engine:

```bash
cd /home/babicto/projects/kafka-event-driven-architecture

# 1. Install Docker Engine (if not already installed)
./scripts/install-docker.sh

# 2. Activate docker group
newgrp docker

# 3. Start Docker daemon
./scripts/start-docker.sh
```

### For Systems with Docker Desktop

Just ensure Docker Desktop is running.

### Verify Docker Setup

```bash
./scripts/verify-docker.sh
```

**Expected output:**
```
✅ Docker is running
✅ Docker Compose is available
```

---

## Example 1: Fundamentals

**Difficulty:** ⭐ Beginner  
**What it demonstrates:** Basic Kafka producer/consumer patterns

### Step 1: Start Kafka Infrastructure

```bash
cd /home/babicto/projects/kafka-event-driven-architecture
./scripts/start-kafka.sh
./scripts/verify-docker.sh
```

**Wait for:** All services to show as "Up" (about 30 seconds)

### Step 2: Verify Kafka is Running

```bash
docker compose ps
```

**Expected:** All containers (zookeeper, kafka, schema-registry, kafka-ui) should be "Up"

### Step 3: Run Producer

```bash
cd examples/01-fundamentals/dotnet
dotnet run --project BasicProducer.csproj
```

**Expected output:**
```
🚀 Starting Kafka Producer...
📡 Connecting to: localhost:9092
✅ Producer created successfully
📤 Producing to topic: my-topic
✅ Delivered to my-topic [[0]] @0
✅ Delivered to my-topic [[0]] @1
...
✅ All messages delivered successfully!
```

### Step 4: Run Consumer (New Terminal)

```bash
cd /home/babicto/projects/kafka-event-driven-architecture/examples/01-fundamentals/dotnet
dotnet run --project BasicConsumer.csproj
```

**Expected output:**
```
Received: {"id":0,"value":"Message 0"}
Received: {"id":1,"value":"Message 1"}
...
```

**Note:** Consumer will keep running. Press `Ctrl+C` to stop.

### Step 5: View in Kafka UI (Optional)

1. Open browser: http://localhost:8080
2. Navigate to: **Topics** → **my-topic** → **Messages**
3. See all messages with keys, values, and timestamps

### Verification Checklist

- [ ] Producer successfully sends messages
- [ ] Consumer receives all messages
- [ ] Messages visible in Kafka UI
- [ ] No errors in console output

### Troubleshooting

**Kafka won't start?**
```bash
./scripts/fix-kafka.sh
```

**Consumer not receiving messages?**
- Make sure producer ran first
- Check consumer is subscribed to `my-topic`
- Verify Kafka is running: `docker compose ps`

---

## Example 2: Core Concepts - Event-Carried State Transfer

**Difficulty:** ⭐⭐ Intermediate  
**What it demonstrates:** Event-Carried State Transfer pattern with autonomous services

### Step 1: Start Kafka Infrastructure

```bash
cd /home/babicto/projects/kafka-event-driven-architecture
./scripts/start-kafka.sh
./scripts/verify-docker.sh
```

### Step 2: Build Solution

```bash
cd examples/02-core-concepts/dotnet/event-carried-state-trf
dotnet build EventCarriedStateTransfer.sln
```

**Expected:** Build succeeds with no errors

### Step 3: Run Example

```bash
dotnet run --project Examples/Examples.csproj
```

**Expected output:**
```
🛒 Order Service: Creating order...
   Order ID: ORD-001
   Customer: John Doe
   Items: 2
   Total: $109.98

📧 Email Service: Received OrderPlaced event
   ✅ Sending confirmation email to John Doe
   Order: ORD-001, Total: $109.98

📦 Inventory Service: Received OrderPlaced event
   ✅ Updating inventory for order ORD-001
   Items: 2

📊 Analytics Service: Received OrderPlaced event
   ✅ Recording analytics for order ORD-001
   Customer: John Doe, Total: $109.98

✅ Event-Carried State Transfer Demo Complete!
```

### Step 4: View in Kafka UI

1. Open: http://localhost:8080
2. Navigate to: **Topics** → **orders** → **Messages**
3. See the rich event payload with all data (order details, customer info, line items)

### Verification Checklist

- [ ] Order service creates order successfully
- [ ] All three services (Email, Inventory, Analytics) receive the event
- [ ] Event contains full data (no API calls needed)
- [ ] Messages visible in Kafka UI

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

---

## Example 3: Producers

**Difficulty:** ⭐⭐ Intermediate  
**What it demonstrates:** Advanced producer patterns

### Step 1: Start Kafka Infrastructure

```bash
cd /home/babicto/projects/kafka-event-driven-architecture
./scripts/start-kafka.sh
./scripts/verify-docker.sh
```

### Step 2: Check Available Examples

```bash
cd examples/03-producers/dotnet
ls -la
```

**Note:** This folder structure is prepared for future producer pattern examples. Check the folder for available examples.

### Verification Checklist

- [ ] Folder structure exists
- [ ] Ready for future examples

**Note:** If no examples are present, this folder may be under development. Check other examples for producer patterns.

---

## Example 4: Build E-Commerce

**Difficulty:** ⭐⭐⭐ Advanced  
**What it demonstrates:** Multi-service event-driven architecture with event dependencies

### Step 1: Start Kafka Infrastructure

```bash
cd /home/babicto/projects/kafka-event-driven-architecture
./scripts/start-kafka.sh
./scripts/verify-docker.sh
```

### Step 2: Create Topics (Recommended)

```bash
cd examples/04-build-e-commerce/dotnet
./create-topics.sh
```

**Expected output:**
```
Creating topics...
✅ Topic orders-created created
✅ Topic orders-validated created
✅ Topic inventory-reserved created
...
```

### Step 3: Build All Services

```bash
cd examples/04-build-e-commerce/dotnet
dotnet build OrderService/OrderService.csproj
dotnet build CustomerService/CustomerService.csproj
dotnet build InventoryService/InventoryService.csproj
dotnet build ShippingService/ShippingService.csproj
```

### Step 4: Start Services (4 Separate Terminals)

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

**Terminal 4 - Order Service:**
```bash
cd examples/04-build-e-commerce/dotnet
dotnet run --project OrderService/OrderService.csproj
```

### Step 5: Observe Event Flow

Watch all terminals to see events flow through the system:

1. **Order Service** creates orders → `orders-created` topic
2. **Customer Service** validates → `orders-validated` or `orders-rejected`
3. **Inventory Service** reserves → `inventory-reserved` or `inventory-insufficient`
4. **Shipping Service** ships when BOTH validated AND reserved → `orders-shipped`

### Step 6: View in Kafka UI

1. Open: http://localhost:8080
2. Navigate to: **Topics** to see all event topics
3. Check: **Consumers** to see consumer groups

### Verification Checklist

- [ ] All 4 services start successfully
- [ ] Order Service creates orders
- [ ] Customer Service validates/rejects orders
- [ ] Inventory Service reserves/fails inventory
- [ ] Shipping Service ships when both conditions met
- [ ] All topics visible in Kafka UI
- [ ] Consumer groups visible in Kafka UI

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
cd examples/04-build-e-commerce/dotnet
./create-topics.sh
```

**Build errors?**
```bash
cd examples/04-build-e-commerce/dotnet
dotnet clean
dotnet build
```

---

## Example 5: Advanced Kafka Concepts

**Difficulty:** ⭐⭐⭐⭐ Expert  
**What it demonstrates:** 6 production Kafka concepts (delivery semantics, log compaction, streams, performance, security, replication)

### Step 1: Start Kafka Infrastructure

```bash
cd /home/babicto/projects/kafka-event-driven-architecture
./scripts/start-kafka.sh
./scripts/verify-docker.sh
```

### Step 2: Build All Examples

```bash
cd examples/05-advanced-kafka/dotnet
dotnet build AdvancedKafkaConcepts.sln
```

### Step 3: Run Individual Sub-Examples

Each sub-example can be run independently. Navigate to each folder and follow the instructions below.

---

### 5.1 Delivery Semantics

**Location:** `examples/05-advanced-kafka/dotnet/01-delivery-semantics`

**What it demonstrates:**
- At-Most-Once: Fast but unsafe (messages may be lost)
- At-Least-Once: Safe but may duplicate (default)
- Exactly-Once: Safe and no duplicates (transactions)

**Run:**
```bash
cd examples/05-advanced-kafka/dotnet/01-delivery-semantics

# At-Most-Once
dotnet run --project AtMostOnce/AtMostOnce.csproj

# At-Least-Once (with Redis deduplication)
dotnet run --project AtLeastOnce/AtLeastOnce.csproj

# Exactly-Once (transactional)
dotnet run --project ExactlyOnce/ExactlyOnce.csproj
```

**Verification:**
- [ ] Each example runs successfully
- [ ] Observe different delivery guarantees
- [ ] Check messages in Kafka UI

---

### 5.2 Log Compaction

**Location:** `examples/05-advanced-kafka/dotnet/02-log-compaction`

**What it demonstrates:**
- Topic compaction configuration
- Latest state per key preservation
- State store reconstruction

**Run:**
```bash
cd examples/05-advanced-kafka/dotnet/02-log-compaction

# Terminal 1: Producer
dotnet run --project Producer/Producer.csproj

# Terminal 2: Consumer
dotnet run --project Consumer/Consumer.csproj
```

**Verification:**
- [ ] Producer sends messages with same keys
- [ ] Consumer receives only latest state per key
- [ ] Check compacted topic in Kafka UI

---

### 5.3 Kafka Streams

**Location:** `examples/05-advanced-kafka/dotnet/03-kafka-streams`

**What it demonstrates:**
- Stream processing topology simulation
- Filtering and aggregation
- Real-time analytics

**Run:**
```bash
cd examples/05-advanced-kafka/dotnet/03-kafka-streams
dotnet run --project Examples/Examples.csproj
```

**Note:** .NET doesn't have native Kafka Streams. This example simulates stream processing concepts.

**Verification:**
- [ ] Stream processing demo runs
- [ ] Observe filtering and aggregation
- [ ] Check output for processed streams

---

### 5.4 Performance Tuning

**Location:** `examples/05-advanced-kafka/dotnet/04-performance-tuning`

**What it demonstrates:**
- Producer batching and compression
- Parallel consumer processing
- Performance optimization techniques

**Run:**
```bash
cd examples/05-advanced-kafka/dotnet/04-performance-tuning

# Baseline producer
dotnet run --project Producer/Producer.csproj

# Tuned producer (modify StartupObject if needed)
dotnet run --project Producer/Producer.csproj

# Parallel consumer
dotnet run --project Consumer/Consumer.csproj
```

**Verification:**
- [ ] Compare baseline vs tuned performance
- [ ] Observe throughput differences
- [ ] Check processing times

---

### 5.5 Security

**Location:** `examples/05-advanced-kafka/dotnet/05-security`

**What it demonstrates:**
- SSL/TLS encryption for Kafka
- SASL authentication (PLAIN mechanism)
- Secure producer/consumer configuration

**Run:**
```bash
cd examples/05-advanced-kafka/dotnet/05-security

# SSL example
dotnet run --project SSL/SSL.csproj

# SASL example
dotnet run --project SASL/SASL.csproj
```

**⚠️ Note:** Requires broker-side SSL/SASL configuration. See the README for setup instructions.

**Verification:**
- [ ] SSL example runs (if configured)
- [ ] SASL example runs (if configured)
- [ ] Check secure connections

---

### 5.6 Multi-DC Replication

**Location:** `examples/05-advanced-kafka/dotnet/06-multi-dc-replication`

**What it demonstrates:**
- MirrorMaker 2 concepts
- Multi-datacenter replication patterns
- Topic replication configuration

**Run:**
```bash
cd examples/05-advanced-kafka/dotnet/06-multi-dc-replication

# DC1 Producer
dotnet run --project Producer/Producer.csproj

# DC2 Consumer
dotnet run --project Consumer/Consumer.csproj
```

**⚠️ Note:** Full replication requires multiple Kafka clusters. This example demonstrates concepts with a single cluster.

**Verification:**
- [ ] Producer sends messages
- [ ] Consumer receives messages
- [ ] Understand replication concepts

---

### Overall Verification Checklist for Example 5

- [ ] All 6 sub-examples can be built
- [ ] Each sub-example runs successfully
- [ ] Concepts are understood
- [ ] No build errors

### Troubleshooting

**Build errors?**
```bash
cd examples/05-advanced-kafka/dotnet
dotnet clean
dotnet restore
dotnet build
```

**Kafka not running?**
```bash
./scripts/start-kafka.sh
./scripts/verify-docker.sh
```

---

## Example 6: Event Sourcing

**Difficulty:** ⭐⭐⭐⭐ Expert  
**What it demonstrates:** Event sourcing pattern with Kafka as event store

### Step 1: Start Kafka Infrastructure

```bash
cd /home/babicto/projects/kafka-event-driven-architecture
./scripts/start-kafka.sh
./scripts/verify-docker.sh
```

### Step 2: Build Projects

```bash
cd examples/06-event-sourcing/dotnet
dotnet build EventSourcing.sln
```

### Step 3: Run Order Service Example

```bash
dotnet run --project Examples/OrderService.csproj
```

**Expected output:**
```
🛒 Starting Event Sourcing Order Service...
📝 Creating Order ORD-001...
   Status: CREATED
   Version: 1

➕ Adding items...
   Added: PROD-001 (2x $29.99)
   Version: 3

💾 Saving to event store...
   ✅ Saved 6 uncommitted events

🔄 Loading order from event store...
   Order ID: ORD-001
   Status: PAID
   Version: 6

✅ Event Sourcing Demo Complete!
```

### Step 4: Run Event Replay Example

```bash
dotnet run --project Examples/EventReplay.csproj
```

**Expected:** Shows how state is rebuilt by replaying events

### Step 5: Run Time Travel Example

```bash
dotnet run --project Examples/TimeTravel.csproj
```

**Expected:** Shows viewing order state at different points in time

### Step 6: View Events in Kafka UI

1. Open: http://localhost:8080
2. Navigate to: **Topics** → **order-events** → **Messages**
3. See all domain events with versions and timestamps

### Verification Checklist

- [ ] Order Service creates order with events
- [ ] Event Replay rebuilds state correctly
- [ ] Time Travel shows state at different times
- [ ] All events visible in Kafka UI
- [ ] Events have proper versioning

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

---

## Example 7: Saga Pattern

**Difficulty:** ⭐⭐⭐⭐ Expert  
**What it demonstrates:** Saga pattern (Choreography and Orchestration)

### Step 1: Start Kafka Infrastructure

```bash
cd /home/babicto/projects/kafka-event-driven-architecture
./scripts/start-kafka.sh
./scripts/verify-docker.sh
```

### Step 2: Build Projects

```bash
cd examples/06-saga/dotnet
dotnet build SagaPattern.sln
```

### Step 3: Run Choreography Example

```bash
dotnet run --project Examples/ChoreographyExample.csproj
```

**Expected output:**
```
🎭 Saga Pattern - Choreography Example
📝 Order Service: Placing order ORD-001...
   ✅ Order ORD-001 saved
   📤 Published OrderPlacedEvent

📦 Inventory Service: Received OrderPlacedEvent
   ✅ Inventory reserved for order ORD-001

💳 Payment Service: Received InventoryReservedEvent
   ✅ Payment processed for order ORD-001

✅ Choreography Saga Demo Complete!
```

### Step 4: Run Orchestration Example

```bash
dotnet run --project Examples/OrchestrationExample.csproj
```

**Expected output:**
```
🎭 Saga Pattern - Orchestration Example
🎭 Saga ORD-SAGA-001: Starting execution...
   Step 1: Reserving inventory...
   ✅ Inventory reserved: RES-1000
   Step 2: Processing payment...
   ✅ Payment processed: PAY-2000
   Step 3: Scheduling shipping...
   ✅ Shipping scheduled: TRACK-3000
   🎉 Saga ORD-SAGA-001 completed successfully!
```

### Step 5: Run Persistence Example

```bash
dotnet run --project Examples/PersistenceExample.csproj
```

**Expected:** Shows saga state persistence for recovery

### Verification Checklist

- [ ] Choreography example runs successfully
- [ ] Orchestration example runs successfully
- [ ] Persistence example shows state recovery
- [ ] Events flow correctly between services
- [ ] Compensating transactions work

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

---

## Example 8: Advanced Monitoring

**Difficulty:** ⭐⭐⭐⭐ Expert  
**What it demonstrates:** Full observability stack (Prometheus, Grafana, Jaeger)

### Step 1: Start All Services

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

### Step 2: Verify Services are Running

```bash
docker compose ps
```

**Expected:** All services should be "Up" (pricing-service, nav-calculator-1, nav-calculator-2, notification-service, prometheus, grafana, jaeger, demo-ui)

### Step 3: Access Dashboards

| Service | URL | Credentials |
|---------|-----|-------------|
| **Demo UI** | http://localhost:3001 | None |
| **Grafana** | http://localhost:3000 | admin/admin |
| **Jaeger** | http://localhost:16686 | None |
| **Prometheus** | http://localhost:9090 | None |

### Step 4: Generate Data

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

### Step 5: View Monitoring Data

**Grafana:**
1. Login: admin/admin
2. Navigate to: **Dashboards** → **Browse**
3. Look for: "NAV Calculator Dashboard"
4. See: Events/sec, processing duration, success rates

**Jaeger:**
1. Open http://localhost:16686
2. Select service: `pricing-service` or `nav-calculator-1`
3. Click "Find Traces"
4. Click any trace to see full request flow

**Prometheus:**
1. Open http://localhost:9090
2. Try queries:
   ```
   events_processed_total
   event_processing_duration_seconds
   events_processed_total{status="success"}
   ```

### Verification Checklist

- [ ] All services start successfully
- [ ] Demo UI is accessible
- [ ] Grafana shows metrics
- [ ] Jaeger shows traces
- [ ] Prometheus collects metrics
- [ ] Load tester generates data
- [ ] Events flow through all services

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

**To stop all services:**
```bash
cd examples/07-advanced-monitoring/dotnet
./stop.sh
# Or
docker compose down
```

---

## Common Troubleshooting

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

## Quick Reference Commands

### Start Kafka (for examples 1-7)
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

# HTTP endpoints (example 8)
curl http://localhost:5001/health
curl http://localhost:5002/health
curl http://localhost:5003/health
curl http://localhost:5004/health
```

---

## Testing Checklist

Use this checklist to verify all examples are working:

- [ ] **Example 1:** Fundamentals - Producer and Consumer work
- [ ] **Example 2:** Core Concepts - Event-Carried State Transfer works
- [ ] **Example 3:** Producers - Folder structure exists
- [ ] **Example 4:** E-Commerce - All 4 services communicate correctly
- [ ] **Example 5:** Advanced Kafka - All 6 sub-examples run
- [ ] **Example 6:** Event Sourcing - Order service, replay, and time travel work
- [ ] **Example 7:** Saga Pattern - Choreography and orchestration work
- [ ] **Example 8:** Advanced Monitoring - All services, Grafana, Jaeger work

---

## Next Steps

After testing all examples:

1. **Experiment** with different configurations
2. **Modify** code to understand how it works
3. **Combine** concepts from different examples
4. **Read** the detailed HOW-TO-RUN.md files in each example folder
5. **Explore** Kafka UI to visualize message flow
6. **Review** the EXAMPLES-GUIDE.md for conceptual explanations

---

## Getting Help

1. **Check logs**: `docker compose logs <service-name>`
2. **Run diagnostics**: `./scripts/diagnose-kafka.sh`
3. **Fix common issues**: `./scripts/fix-kafka.sh`
4. **Verify setup**: `./scripts/verify-docker.sh`
5. **Read troubleshooting guide**: [docs/TROUBLESHOOTING.md](docs/TROUBLESHOOTING.md)

---

**Happy Event Streaming! 🎉**
