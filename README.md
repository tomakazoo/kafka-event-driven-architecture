# Kafka Event-Driven Architecture Series

Complete guide to building scalable event-driven systems with Apache Kafka. Learn from fundamental concepts to advanced production patterns.

## Quick Start

### Prerequisites

- Docker & Docker Compose (or Docker Engine for WSL 2)
- Python 3.9+ OR .NET 8.0+
- Git

### Get Started (5 minutes)

1. Clone the repository
2. Start Kafka: `./scripts/start-kafka.sh` (or `docker-compose up -d`)
3. Verify setup: `./scripts/verify-docker.sh`
4. Explore examples in `examples/` directory
5. Run Python: `cd examples/01-fundamentals/python && python basic_producer.py`
6. Run C#: `cd examples/01-fundamentals/dotnet && dotnet run --project BasicProducer.csproj`

**📖 For detailed setup instructions, see [docs/QUICKSTART.md](docs/QUICKSTART.md)**

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
- `scripts/` - Helper scripts for Docker and Kafka management
- `shared/` - Shared models and utilities
- `docker-compose.yml` - Local Kafka setup
- `docker/` - Docker images for examples

---

## 📚 Documentation

Complete documentation is available in the `docs/` folder. Here's a quick guide:

### 🚀 Getting Started

- **[docs/QUICKSTART.md](docs/QUICKSTART.md)** - Complete setup guide for WSL 2 (without Docker Desktop). Step-by-step instructions to get Kafka running and run your first example.
  
  **Start here if:** You're new to the project or setting up for the first time.

- **[docs/SETUP.md](docs/SETUP.md)** - Original setup documentation with prerequisites, installation steps, and IDE setup instructions.
  
  **Start here if:** You want the original setup guide or need IDE-specific instructions.

### 🔧 Troubleshooting & Support

- **[docs/TROUBLESHOOTING.md](docs/TROUBLESHOOTING.md)** - Comprehensive troubleshooting guide covering common issues, error messages, and solutions.
  
  **Use this when:** You encounter errors, Kafka won't start, or services aren't responding.

- **[docs/PRODUCER-TROUBLESHOOTING.md](docs/PRODUCER-TROUBLESHOOTING.md)** - Comprehensive guide for troubleshooting producer hanging issues, including diagnostic steps and solutions.
  
  **Use this when:** Your producer hangs and doesn't produce messages, or times out unexpectedly.

- **[docs/DOCKER-PERMISSIONS-FIX.md](docs/DOCKER-PERMISSIONS-FIX.md)** - Guide to fixing Docker permission issues in WSL 2, including quick fixes and permanent solutions.
  
  **Use this when:** You see "permission denied" errors when running Docker commands.

### 📖 Additional Documentation

- **[examples/01-fundamentals/HOW-TO-RUN.md](examples/01-fundamentals/HOW-TO-RUN.md)** - Detailed guide for running the fundamentals example with all components explained.

- **[scripts/README.md](scripts/README.md)** - Documentation for all helper scripts (Docker management, Kafka setup, diagnostics).

### 🗺️ Documentation Map

```
docs/
├── QUICKSTART.md               ← Start here (WSL 2 setup)
├── SETUP.md                    ← Original setup guide
├── TROUBLESHOOTING.md          ← Fix common issues
├── PRODUCER-TROUBLESHOOTING.md ← Fix producer hanging
├── DOCKER-PERMISSIONS-FIX.md   ← Fix Docker permissions
└── CONTRIBUTING.md             ← Contribute to project
```

### 🚀 Quick Links

- **New to the project?** → Start with [docs/QUICKSTART.md](docs/QUICKSTART.md)
- **Having issues?** → Check [docs/TROUBLESHOOTING.md](docs/TROUBLESHOOTING.md)
- **Producer hanging?** → See [docs/PRODUCER-TROUBLESHOOTING.md](docs/PRODUCER-TROUBLESHOOTING.md)
- **Docker permission errors?** → See [docs/DOCKER-PERMISSIONS-FIX.md](docs/DOCKER-PERMISSIONS-FIX.md)
- **Want to contribute?** → Read [docs/CONTRIBUTING.md](docs/CONTRIBUTING.md)

---

## 🚀 Running Examples

Each example includes both Python and C# implementations. For detailed step-by-step instructions, see [examples/01-fundamentals/HOW-TO-RUN.md](examples/01-fundamentals/HOW-TO-RUN.md).

### Quick Start

**Prerequisites:** Make sure Kafka is running first:
```bash
./scripts/start-kafka.sh
./scripts/verify-docker.sh
```

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
dotnet run --project BasicProducer.csproj
```

For complete setup instructions, see [docs/QUICKSTART.md](docs/QUICKSTART.md).

## 🤝 Contributing

Contributions welcome! See [docs/CONTRIBUTING.md](docs/CONTRIBUTING.md)

## 📄 License

MIT License - see [LICENSE](LICENSE)


