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

## 📁 Example 1: Fundamentals (`examples/01-fundamentals`)

### What It Demonstrates
- Basic Kafka producer/consumer patterns
- Message production and consumption
- Topic creation and message flow

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

---

## 📁 Example 2: E-Commerce Order System (`examples/04-build-e-commerce`)

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

---

## 📁 Example 3: Advanced Monitoring (`examples/07-advanced-monitoring/dotnet`)

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

### Start Kafka (for examples 1 & 2)
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

# HTTP endpoints (example 3)
curl http://localhost:5001/health
curl http://localhost:5002/health
curl http://localhost:5003/health
curl http://localhost:5004/health
```

---

## 🎯 Example Comparison

| Feature | 01-Fundamentals | 04-E-Commerce | 07-Monitoring |
|---------|----------------|---------------|---------------|
| **Services** | 2 (Producer, Consumer) | 4 (Order, Customer, Inventory, Shipping) | 3 (Pricing, NAV Calc, Notification) |
| **Kafka Setup** | Main docker-compose.yml | Main docker-compose.yml | Own docker-compose.yml |
| **Topics** | 1 (my-topic) | 6 topics | 2 topics |
| **Monitoring** | Kafka UI only | Kafka UI only | Prometheus, Grafana, Jaeger |
| **UI** | None | None | React Demo UI |
| **Load Testing** | None | None | LoadTester tool |
| **Complexity** | ⭐ Basic | ⭐⭐ Intermediate | ⭐⭐⭐ Advanced |

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

