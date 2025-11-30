#!/bin/bash
# Helper script to start Kafka using Docker

set -e

echo "🚀 Starting Kafka infrastructure..."
echo ""

# Check if Docker is installed
if ! command -v docker &> /dev/null; then
    echo "❌ Docker is not installed!"
    echo ""
    echo "Please run: ./scripts/install-docker.sh"
    exit 1
fi

# Check if Docker daemon is running
if ! docker info &> /dev/null; then
    echo "⚠️  Docker daemon is not running"
    echo "Starting Docker..."
    sudo service docker start
    sleep 2
fi

# Navigate to project root (parent of scripts directory)
cd "$(dirname "$0")/.."

# Use modern docker compose syntax (V2)
docker compose up -d

if [ $? -eq 0 ]; then
    echo ""
    echo "✅ Kafka services started successfully!"
    echo ""
    echo "Services:"
    echo "  • Kafka Broker:      localhost:9092"
    echo "  • Zookeeper:         localhost:2181"
    echo "  • Schema Registry:   localhost:8081"
    echo "  • Kafka UI:          http://localhost:8080"
    echo ""
    echo "To verify: ./scripts/verify-docker.sh"
    echo "To stop:   docker compose down"
    echo ""
else
    echo ""
    echo "❌ Failed to start Kafka services"
    exit 1
fi

