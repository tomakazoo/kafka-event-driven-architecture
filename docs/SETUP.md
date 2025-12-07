# Setup Guide

## Prerequisites

### All Examples

- Git
- Docker & Docker Compose
- 4GB free disk space

### C# Examples

- .NET 8.0 SDK
- Visual Studio Code or Visual Studio 2022

*Note: Python examples may be added in the future*

## Installation

### 1. Clone Repository

```bash
git clone https://github.com/yourusername/kafka-event-driven-architecture.git
cd kafka-event-driven-architecture
```

### 2. Start Kafka

```bash
docker-compose up -d
```

This starts:

- Kafka broker (localhost:9092)
- Zookeeper (localhost:2181)
- Schema Registry (localhost:8081)
- Kafka UI (localhost:8080)

### 3. Verify Setup

```bash
# Check Kafka is running
docker-compose ps

# Access Kafka UI: http://localhost:8080
```

### 4. Run First Example

**C#:**

```bash
cd examples/01-fundamentals/dotnet
dotnet build
dotnet run --project BasicProducer.csproj &
dotnet run --project BasicConsumer.csproj
```

## Verification

You should see messages flowing from producer to consumer.

## Troubleshooting

### Port Already in Use

```bash
# Stop existing containers
docker-compose down

# Remove volumes
docker-compose down -v

# Start fresh
docker-compose up -d
```

### Connection Refused

- Ensure docker-compose is running: `docker-compose ps`
- Check firewall settings
- Verify `BOOTSTRAP_SERVERS` in config

## IDE Setup

### Visual Studio (C#)

1. Open `.sln` file
2. NuGet packages auto-restore
3. Build solution
4. Set startup project to BasicProducer or BasicConsumer
5. Run

## Next Steps

[👉 Read Chapter 1](../blog/post-1-event-driven-fundamentals)


