#!/bin/bash

echo "🔨 Building C# Kafka Examples..."
echo ""

# Build both projects
echo "Building BasicProducer..."
dotnet build BasicProducer.csproj --nologo -v quiet

echo "Building BasicConsumer..."
dotnet build BasicConsumer.csproj --nologo -v quiet

echo ""
echo "✅ Build complete!"
echo ""
echo "📤 Running Producer (will send 5 messages)..."
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
dotnet run --project BasicProducer.csproj --no-build

echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "📥 Running Consumer (press Ctrl+C to stop)..."
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
dotnet run --project BasicConsumer.csproj --no-build


