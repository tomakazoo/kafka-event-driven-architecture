# Quick Start Guide - Kafka Event-Driven Architecture

## 🎯 Complete Setup for WSL 2 (Without Docker Desktop)

This guide will get you from zero to running Kafka examples in minutes.

---

## 📋 Prerequisites

- WSL 2 with Ubuntu 24.04 (you have this ✅)
- .NET 8.0 SDK (for C# examples)
- Python 3.9+ (for Python examples)

---

## 🚀 Step 1: Install Docker Engine

Run the installation script:

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

---

## 🐳 Step 2: Start Kafka Services

```bash
./scripts/start-kafka.sh
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

---

## 🎓 Step 3: Run Your First Example

### Option A: C# Example

```bash
cd examples/01-fundamentals/dotnet

# Run automated script (builds and runs producer then consumer)
./run-example.sh

# OR run manually:
dotnet build BasicProducer.csproj
dotnet run --project BasicProducer.csproj

# In another terminal:
dotnet run --project BasicConsumer.csproj
```

**Expected output:**

Producer:
```
Delivered to my-topic [[0]] @0
Delivered to my-topic [[0]] @1
Delivered to my-topic [[0]] @2
Delivered to my-topic [[0]] @3
Delivered to my-topic [[0]] @4
```

Consumer:
```
Received: {"id":0,"value":"Message 0"}
Received: {"id":1,"value":"Message 1"}
...
```

### Option B: Python Example

```bash
cd examples/01-fundamentals/python

# Install dependencies
pip install -r requirements.txt

# Run producer (in one terminal)
python basic_producer.py

# Run consumer (in another terminal)
python basic_consumer.py
```

---

## 🎨 Step 4: Explore Kafka UI

Open your browser to: **http://localhost:8080**

You can:
- View topics
- Browse messages
- Monitor consumer groups
- See broker metrics

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
```

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
│   └── diagnose-kafka.sh       # Diagnostic tool
├── examples/
│   └── 01-fundamentals/
│       ├── dotnet/
│       │   ├── BasicProducer.cs
│       │   ├── BasicProducer.csproj
│       │   ├── BasicConsumer.cs
│       │   ├── BasicConsumer.csproj
│       │   └── run-example.sh
│       └── python/
│           ├── requirements.txt
│           ├── basic_producer.py
│           └── basic_consumer.py
└── docs/
    └── SETUP.md
```

---

## 🎯 Next Steps

1. Explore the Kafka UI at http://localhost:8080
2. Modify the examples to send different messages
3. Create your own topics
4. Experiment with consumer groups
5. Try different partitioning strategies

---

## 📚 Helpful Resources

- [Kafka Documentation](https://kafka.apache.org/documentation/)
- [Confluent Kafka Python](https://docs.confluent.io/kafka-clients/python/current/overview.html)
- [Confluent Kafka .NET](https://docs.confluent.io/kafka-clients/dotnet/current/overview.html)

---

## 🆘 Need Help?

Check the logs:
```bash
docker compose logs -f
```

Restart everything:
```bash
docker compose down -v
./scripts/start-kafka.sh
```

Still stuck? Make sure:
- ✅ Docker daemon is running: `docker info`
- ✅ No port conflicts: `docker compose ps`
- ✅ Sufficient disk space: `df -h`

