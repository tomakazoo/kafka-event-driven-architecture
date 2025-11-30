#!/bin/bash

echo "Verifying Docker setup..."
echo ""

# Navigate to project root (parent of scripts directory)
cd "$(dirname "$0")/.."

# Check if Docker is installed
if ! command -v docker &> /dev/null; then
    echo "❌ Docker is not installed!"
    echo "Please run: ./scripts/install-docker.sh"
    exit 1
fi

# Check if Docker daemon is running
if ! docker info &> /dev/null; then
    echo "❌ Docker daemon is not running"
    echo "Run: ./scripts/start-docker.sh"
    exit 1
fi

# Check if docker compose services are running
echo "Checking Docker Compose services..."
docker compose ps

echo ""
echo "Checking Kafka connectivity..."
docker compose exec -T kafka kafka-broker-api-versions --bootstrap-server localhost:9092

if [ $? -eq 0 ]; then
    echo ""
    echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
    echo "✅ All services are running!"
    echo ""
    echo "Access Kafka UI: http://localhost:8080"
    echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
else
    echo ""
    echo "⚠️  Services are starting or not fully ready yet"
    echo "Try running this script again in a few seconds"
fi

