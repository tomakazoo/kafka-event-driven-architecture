#!/bin/bash

echo "🔍 Troubleshooting Producer Hanging Issue..."
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""

# Navigate to project root
cd "$(dirname "$0")/.."

# Step 1: Check Kafka is running
echo "📦 Step 1: Checking Kafka services..."
docker compose ps | grep -E "kafka|zookeeper" || {
    echo "❌ Kafka services not found!"
    echo "   Run: ./scripts/start-kafka.sh"
    exit 1
}

echo "✅ Kafka services found"
echo ""

# Step 2: Check Kafka container status
echo "📊 Step 2: Checking Kafka container status..."
KAFKA_STATUS=$(docker compose ps kafka --format json 2>/dev/null | grep -o '"State":"[^"]*"' | cut -d'"' -f4)

if [ "$KAFKA_STATUS" != "running" ]; then
    echo "❌ Kafka is not running! Status: $KAFKA_STATUS"
    echo "   Run: docker compose restart kafka"
    exit 1
fi

echo "✅ Kafka is running"
echo ""

# Step 3: Test connectivity
echo "🔌 Step 3: Testing Kafka connectivity..."
docker compose exec -T kafka kafka-broker-api-versions --bootstrap-server localhost:9092 > /dev/null 2>&1

if [ $? -eq 0 ]; then
    echo "✅ Kafka is responding"
else
    echo "❌ Kafka is not responding!"
    echo "   Check logs: docker compose logs kafka --tail 50"
    exit 1
fi
echo ""

# Step 4: Check port accessibility
echo "🌐 Step 4: Checking port 9092 accessibility..."
if command -v nc > /dev/null; then
    nc -zv localhost 9092 > /dev/null 2>&1
    if [ $? -eq 0 ]; then
        echo "✅ Port 9092 is accessible"
    else
        echo "⚠️  Port 9092 might not be accessible from host"
        echo "   Check: docker compose ps (verify port mapping)"
    fi
else
    echo "⚠️  'nc' not available, skipping port check"
fi
echo ""

# Step 5: Check recent Kafka logs for errors
echo "📋 Step 5: Checking recent Kafka logs..."
RECENT_ERRORS=$(docker compose logs kafka --tail 50 2>/dev/null | grep -i error | head -5)

if [ -n "$RECENT_ERRORS" ]; then
    echo "⚠️  Recent errors found in Kafka logs:"
    echo "$RECENT_ERRORS"
    echo ""
    echo "   View full logs: docker compose logs kafka --tail 100"
else
    echo "✅ No recent errors in Kafka logs"
fi
echo ""

# Step 6: Check topic exists
echo "📝 Step 6: Checking if topic 'my-topic' exists..."
TOPIC_EXISTS=$(docker compose exec -T kafka kafka-topics --list --bootstrap-server localhost:9092 2>/dev/null | grep -q "my-topic" && echo "yes" || echo "no")

if [ "$TOPIC_EXISTS" = "yes" ]; then
    echo "✅ Topic 'my-topic' exists"
else
    echo "⚠️  Topic 'my-topic' does not exist (will be auto-created)"
fi
echo ""

# Summary
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "📊 Summary:"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""
echo "If all checks passed, try running the producer:"
echo "  cd examples/01-fundamentals/dotnet"
echo "  dotnet run --project BasicProducer.csproj"
echo ""
echo "If producer still hangs:"
echo "  1. Wait 30 seconds after starting Kafka"
echo "  2. Check: docker compose logs -f kafka (in another terminal)"
echo "  3. See: docs/PRODUCER-TROUBLESHOOTING.md"
echo ""
echo "Quick fixes:"
echo "  • Restart Kafka: docker compose restart kafka"
echo "  • Restart all: docker compose restart"
echo "  • Fresh start: ./scripts/fix-kafka.sh"
echo ""



