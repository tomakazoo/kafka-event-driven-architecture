#!/bin/bash
# start.sh - Quick start script

echo "🚀 Starting Event-Driven NAV Calculator Demo..."
echo ""

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    echo "❌ Docker is not running. Please start Docker and try again."
    exit 1
fi

echo "✅ Docker is running"
echo ""

# Build and start services
echo "📦 Building and starting services..."
docker compose up --build -d

echo ""
echo "⏳ Waiting for services to be healthy..."
sleep 10

# Check service health
echo ""
echo "🔍 Checking service health..."

services=("pricing-service:5001" "nav-calculator-1:5002" "nav-calculator-2:5003" "notification-service:5004")
all_healthy=true

for service in "${services[@]}"; do
    IFS=':' read -r name port <<< "$service"
    if curl -sf "http://localhost:$port/health" > /dev/null; then
        echo "  ✅ $name is healthy"
    else
        echo "  ❌ $name is not responding"
        all_healthy=false
    fi
done

echo ""
if [ "$all_healthy" = true ]; then
    echo "🎉 All services are healthy!"
    echo ""
    echo "📊 Access the demo:"
    echo "  Demo UI:    http://localhost:3001"
    echo "  Grafana:    http://localhost:3000 (admin/admin)"
    echo "  Jaeger:     http://localhost:16686"
    echo "  Prometheus: http://localhost:9090"
    echo ""
    echo "🧪 Run load test:"
    echo "  cd src/Tools/LoadTester && dotnet run"
    echo ""
    echo "📜 View logs:"
    echo "  docker compose logs -f [service-name]"
else
    echo "⚠️  Some services are not healthy. Check logs with:"
    echo "  docker compose logs"
fi
