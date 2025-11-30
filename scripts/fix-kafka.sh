#!/bin/bash

set -e

echo "🔧 Fixing Kafka Setup..."
echo ""

# Navigate to project root (parent of scripts directory)
cd "$(dirname "$0")/.."

# Stop all containers
echo "📦 Step 1: Stopping all containers..."
docker compose down

# Remove old data (if corrupted)
echo ""
echo "🧹 Step 2: Cleaning up old data..."
rm -rf ./kafka-data
mkdir -p ./kafka-data

# Pull latest images
echo ""
echo "📥 Step 3: Pulling latest images..."
docker compose pull

# Start services with logs
echo ""
echo "🚀 Step 4: Starting services..."
docker compose up -d

echo ""
echo "⏳ Step 5: Waiting for services to start (30 seconds)..."
for i in {30..1}; do
    echo -ne "\r⏱️  $i seconds remaining...  "
    sleep 1
done
echo ""

# Check status
echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "📊 Service Status:"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
docker compose ps

echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "📋 Recent Kafka Logs:"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
docker compose logs kafka --tail 30

echo ""
echo "✅ Fix complete! Run ./scripts/verify-docker.sh to verify."

