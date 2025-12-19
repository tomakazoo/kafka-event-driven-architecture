# Event-Driven NAV Calculator - Production Operations Demo

A complete, working demonstration of event-driven architecture with Apache Kafka, built with C# (.NET 8) and React.

## 🎯 What This Demo Shows

- **3 Microservices** communicating asynchronously via Kafka
- **Full Observability Stack**: Prometheus, Grafana, Jaeger
- **Distributed Tracing** with correlation IDs across services
- **Horizontal Scaling** (2 NAV calculator instances)
- **Interactive UI** to trigger events and visualize the flow
- **Load Testing** to simulate month-end processing spikes

## �� Prerequisites

- Docker & Docker Compose
- .NET 8 SDK (for local development)
- Node.js 18+ (for UI development)

## 🚀 Quick Start

### 1. Clone and Start
```bash
git clone <your-repo>
cd event-driven-nav-poc

# Start everything
docker-compose up --build
```

Wait for all services to start (~2 minutes). You'll see:
```
✅ Kafka is healthy
✅ Services starting...
✅ UI available at http://localhost:3001
```

### 2. Access the Demo

| Service | URL | Description |
|---------|-----|-------------|
| **Demo UI** | http://localhost:3001 | Interactive interface |
| **Grafana** | http://localhost:3000 | Metrics dashboard (admin/admin) |
| **Jaeger** | http://localhost:16686 | Distributed traces |
| **Prometheus** | http://localhost:9090 | Raw metrics |

### 3. Try It Out

1. **Open the UI**: http://localhost:3001
2. **Click a fund** to trigger a single NAV calculation
3. **Watch the flow**:
   - Pricing Service → publishes event
   - NAV Calculator → calculates NAV
   - Notification Service → sends notification
4. **Check Grafana**: See metrics in real-time
5. **Check Jaeger**: Follow the trace through all services
6. **Simulate Month-End**: Click "Simulate Month-End" to process all funds at once

## 📊 Architecture
```
┌─────────────┐      pricing-updates       ┌──────────────┐
│   Pricing   ├───────────────────────────>│     NAV      │
│   Service   │        (Kafka)             │  Calculator  │
└─────────────┘                            └──────┬───────┘
                                                  │
                                                  │ nav-calculated
                                                  ↓
                                           ┌──────────────┐
                                           │ Notification │
                                           │   Service    │
                                           └──────────────┘
```

**Observable at every step:**
- Prometheus metrics
- Jaeger traces
- Structured logs

## 🧪 Load Testing

Run load tests to see how the system handles spikes:
```bash
# Build and run load tester
cd src/Tools/LoadTester
dotnet run -- --funds 28 --concurrent true --iterations 3

# Sequential processing
dotnet run -- --funds 28 --concurrent false

# Heavy load
dotnet run -- --funds 100 --concurrent true
```

## 🔍 Observability Features

### Metrics (Prometheus/Grafana)

- Events processed per second
- Processing duration (P50, P95, P99)
- Success/error rates
- Load distribution across instances

### Traces (Jaeger)

Every request gets a **correlation ID** that flows through all services. Search in Jaeger by correlation ID to see the entire flow.

### Logs

All services use structured logging with correlation IDs.

## 🏗️ Development

### Project Structure
```
src/
├── Shared/
│   ├── Shared.Observability/    # Metrics, tracing, middleware
│   └── Shared.Kafka/             # Kafka producers/consumers
├── Services/
│   ├── PricingService/           # Triggers pricing updates
│   ├── NavCalculator/            # Calculates NAV
│   └── NotificationService/      # Sends notifications
└── Tools/
    └── LoadTester/               # Performance testing
```

### Run Locally (without Docker)
```bash
# Terminal 1: Start Kafka
docker-compose up kafka jaeger prometheus grafana

# Terminal 2: Pricing Service
cd src/Services/PricingService
dotnet run

# Terminal 3: NAV Calculator
cd src/Services/NavCalculator
dotnet run

# Terminal 4: Notification Service
cd src/Services/NotificationService
dotnet run

# Terminal 5: UI
cd demo-ui
npm install
npm start
```

## 🛠️ Troubleshooting

### Kafka not starting
```bash
# Check logs
docker-compose logs kafka

# Restart Kafka
docker-compose restart kafka
```

### Services can't connect to Kafka
```bash
# Wait for Kafka to be healthy
docker-compose ps

# Check Kafka topics
docker-compose exec kafka kafka-topics --bootstrap-server localhost:9092 --list
```

### No metrics in Grafana
```bash
# Check Prometheus targets
http://localhost:9090/targets

# All should be "UP"
```

## 📚 Blog Series

This demo is Part 8 of the Event-Driven Architecture series.

## 🎓 Learning Objectives

By running this demo, you'll learn:

✅ How to instrument services for observability  
✅ How to use Prometheus + Grafana for monitoring  
✅ How to trace requests across services with Jaeger  
✅ How to design event-driven systems in C#  
✅ How Kafka handles load distribution  
✅ How to test event-driven systems  

## 📝 License

MIT

## 🤝 Contributing

Contributions welcome! Please open an issue or PR.

---

Built with ❤️ for the Event-Driven Architecture blog series
