#!/bin/bash

echo "📋 Creating Kafka Topics for Order System..."
echo ""

cd "$(dirname "$0")/../../.."

# Check if Kafka is running
if ! docker compose ps kafka | grep -q "Up"; then
    echo "❌ Kafka is not running!"
    echo "   Run: ./scripts/start-kafka.sh"
    exit 1
fi

echo "✅ Kafka is running"
echo ""

# Create topics
echo "Creating topics..."

docker compose exec -T kafka kafka-topics --create \
  --bootstrap-server localhost:9092 \
  --topic orders-created \
  --partitions 3 \
  --replication-factor 1 \
  --if-not-exists 2>/dev/null && echo "✅ Created orders-created" || echo "⚠️  orders-created already exists"

docker compose exec -T kafka kafka-topics --create \
  --bootstrap-server localhost:9092 \
  --topic orders-validated \
  --partitions 3 \
  --replication-factor 1 \
  --if-not-exists 2>/dev/null && echo "✅ Created orders-validated" || echo "⚠️  orders-validated already exists"

docker compose exec -T kafka kafka-topics --create \
  --bootstrap-server localhost:9092 \
  --topic orders-rejected \
  --partitions 1 \
  --replication-factor 1 \
  --if-not-exists 2>/dev/null && echo "✅ Created orders-rejected" || echo "⚠️  orders-rejected already exists"

docker compose exec -T kafka kafka-topics --create \
  --bootstrap-server localhost:9092 \
  --topic inventory-reserved \
  --partitions 3 \
  --replication-factor 1 \
  --if-not-exists 2>/dev/null && echo "✅ Created inventory-reserved" || echo "⚠️  inventory-reserved already exists"

docker compose exec -T kafka kafka-topics --create \
  --bootstrap-server localhost:9092 \
  --topic inventory-insufficient \
  --partitions 1 \
  --replication-factor 1 \
  --if-not-exists 2>/dev/null && echo "✅ Created inventory-insufficient" || echo "⚠️  inventory-insufficient already exists"

docker compose exec -T kafka kafka-topics --create \
  --bootstrap-server localhost:9092 \
  --topic orders-shipped \
  --partitions 3 \
  --replication-factor 1 \
  --if-not-exists 2>/dev/null && echo "✅ Created orders-shipped" || echo "⚠️  orders-shipped already exists"

echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "✅ All topics created!"
echo ""
echo "List topics:"
docker compose exec -T kafka kafka-topics --list --bootstrap-server localhost:9092

