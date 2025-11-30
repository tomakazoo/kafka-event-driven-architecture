# Kafka Event-Driven Architecture Series

Complete guide to building scalable event-driven systems with Apache Kafka. Learn from fundamental concepts to advanced production patterns.

## Quick Start

### Prerequisites

- Docker & Docker Compose
- Python 3.9+ OR .NET 8.0+
- Git

### Get Started (5 minutes)

1. Clone the repository
2. Start Kafka: `docker-compose up -d`
3. Explore examples in `examples/` directory
4. Run Python: `cd examples/01-fundamentals/python && python basic_producer.py`
5. Run C#: `cd examples/01-fundamentals/dotnet && dotnet build && dotnet run`

## 📚 Series Overview

| Chapter | Topic | Time | Difficulty |
|---------|-------|------|------------|
| 1 | Event-Driven Fundamentals | 8 min | ⭐ |
| 2 | Kafka Core Concepts | 10 min | ⭐⭐ |
| 3 | Building Producers | 12 min | ⭐⭐ |
| 4 | Consumer Patterns | 11 min | ⭐⭐ |
| 5 | Schema Registry | 9 min | ⭐⭐⭐ |
| 6 | Advanced Patterns | 13 min | ⭐⭐⭐⭐ |

[👉 Read the complete series](./SERIES_LANDING.md)

## 📁 Repository Structure

- `docs/` - Documentation and guides
- `examples/` - Code examples (Python & C#)
- `shared/` - Shared models and utilities
- `docker-compose.yml` - Local Kafka setup
- `docker/` - Docker images for examples

## 🚀 Running Examples

Each example includes both Python and C# implementations.

### Python Examples

```bash
cd examples/01-fundamentals/python
pip install -r requirements.txt
python basic_producer.py
```

### C# Examples

```bash
cd examples/01-fundamentals/dotnet
dotnet build
dotnet run
```

## 🤝 Contributing

Contributions welcome! See [docs/CONTRIBUTING.md](docs/CONTRIBUTING.md)

## 📄 License

MIT License - see [LICENSE](LICENSE)


