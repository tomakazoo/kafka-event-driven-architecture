# Helper Scripts

This directory contains utility scripts for managing Docker and Kafka infrastructure.

## 📋 Script Overview

### Installation & Setup

#### `install-docker.sh`
Installs Docker Engine and Docker Compose on WSL 2 (Ubuntu).

**Usage:**
```bash
./scripts/install-docker.sh
```

**What it does:**
- Removes old Docker installations
- Adds Docker's official repository
- Installs Docker Engine and Docker Compose V2
- Adds current user to docker group
- Starts Docker service

**After running:**
- Run `newgrp docker` to activate group membership
- Or logout and login again

---

### Docker Management

#### `start-docker.sh`
Starts the Docker daemon in WSL 2.

**Usage:**
```bash
./scripts/start-docker.sh
```

**What it does:**
- Checks if Docker is installed
- Starts Docker daemon if not running
- Verifies Docker is working

---

### Kafka Management

#### `start-kafka.sh`
Starts all Kafka infrastructure services.

**Usage:**
```bash
./scripts/start-kafka.sh
```

**What it does:**
- Checks Docker is installed and running
- Starts Zookeeper, Kafka, Schema Registry, and Kafka UI
- Shows service URLs

**Services started:**
- Kafka Broker: `localhost:9092`
- Zookeeper: `localhost:2181`
- Schema Registry: `localhost:8081`
- Kafka UI: `http://localhost:8080`

---

#### `verify-docker.sh`
Verifies all Kafka services are running correctly.

**Usage:**
```bash
./scripts/verify-docker.sh
```

**What it does:**
- Checks Docker is installed and running
- Lists all running containers
- Tests Kafka connectivity
- Confirms all services are healthy

---

#### `fix-kafka.sh`
Fixes common Kafka startup issues.

**Usage:**
```bash
./scripts/fix-kafka.sh
```

**What it does:**
- Stops all containers
- Cleans up corrupted data
- Pulls fresh images
- Restarts all services
- Shows detailed logs

**Use when:**
- Kafka fails to start
- Services are in error state
- Need a clean restart

---

#### `diagnose-kafka.sh`
Diagnoses Kafka issues with detailed information.

**Usage:**
```bash
./scripts/diagnose-kafka.sh
```

**What it does:**
- Shows container status
- Displays Kafka logs
- Displays Zookeeper logs
- Checks port conflicts
- Shows disk space

**Use when:**
- Troubleshooting issues
- Understanding what's wrong
- Before asking for help

---

### Utility

#### `docker-helper.sh`
Generic Docker wrapper (legacy - not typically needed).

**Usage:**
```bash
./scripts/docker-helper.sh <docker-command>
```

---

## 🚀 Common Workflows

### First Time Setup
```bash
# 1. Install Docker
./scripts/install-docker.sh

# 2. Activate docker group
newgrp docker

# 3. Start Kafka
./scripts/start-kafka.sh

# 4. Verify
./scripts/verify-docker.sh
```

### Daily Usage
```bash
# Start Kafka (if not running)
./scripts/start-kafka.sh

# Verify status
./scripts/verify-docker.sh

# Run your examples
cd examples/01-fundamentals/dotnet && ./run-example.sh
```

### Troubleshooting
```bash
# Diagnose issues
./scripts/diagnose-kafka.sh

# Fix common problems
./scripts/fix-kafka.sh

# Verify after fix
./scripts/verify-docker.sh
```

### Cleanup
```bash
# Stop all services
docker compose down

# Stop and remove data
docker compose down -v
```

---

## 📖 Additional Documentation

- **[QUICKSTART.md](../docs/QUICKSTART.md)** - Complete setup guide
- **[TROUBLESHOOTING.md](../docs/TROUBLESHOOTING.md)** - Troubleshooting guide
- **[INSTALL-NOTES.txt](../INSTALL-NOTES.txt)** - Quick reference

---

## ⚙️ Script Requirements

All scripts require:
- **Bash shell** (available by default in WSL/Linux)
- **Execute permissions** (already set)
- **Run from project root** - Scripts handle navigation automatically

Scripts will check for required tools and provide clear error messages if anything is missing.

