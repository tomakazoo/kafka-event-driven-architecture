#!/bin/bash

echo "🚀 Starting Order System Services..."
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""
echo "This script will start all 4 services in separate terminals."
echo "Make sure Kafka is running: ./scripts/start-kafka.sh"
echo ""
read -p "Press Enter to continue..."

# Get the directory of this script
SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
cd "$SCRIPT_DIR"

echo ""
echo "🔨 Building all services..."
echo ""

# Build all projects
dotnet build OrderService/OrderService.csproj --nologo -v quiet
dotnet build CustomerService/CustomerService.csproj --nologo -v quiet
dotnet build InventoryService/InventoryService.csproj --nologo -v quiet
dotnet build ShippingService/ShippingService.csproj --nologo -v quiet

if [ $? -ne 0 ]; then
    echo "❌ Build failed!"
    exit 1
fi

echo "✅ All services built successfully!"
echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "📋 Starting Services:"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""
echo "1. Customer Service (validates customers)"
echo "2. Inventory Service (reserves inventory)"
echo "3. Shipping Service (creates shipments)"
echo "4. Order Service (creates orders - run this last)"
echo ""
echo "⚠️  Note: Start services 1-3 first, then run Order Service"
echo "    to see the complete event flow."
echo ""
echo "Run services manually:"
echo "  Terminal 1: dotnet run --project CustomerService/CustomerService.csproj"
echo "  Terminal 2: dotnet run --project InventoryService/InventoryService.csproj"
echo "  Terminal 3: dotnet run --project ShippingService/ShippingService.csproj"
echo "  Terminal 4: dotnet run --project OrderService/OrderService.csproj"
echo ""


