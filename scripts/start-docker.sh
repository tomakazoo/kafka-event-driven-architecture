#!/bin/bash

# Helper script to start Docker service in WSL 2

echo "🐳 Starting Docker service..."

# Check if Docker is installed
if ! command -v docker &> /dev/null; then
    echo "❌ Docker is not installed!"
    echo ""
    echo "Please run: ./install-docker.sh"
    exit 1
fi

# Check if Docker daemon is running
if sudo service docker status &> /dev/null; then
    echo "✅ Docker is already running"
else
    echo "Starting Docker daemon..."
    sudo service docker start
    
    if [ $? -eq 0 ]; then
        echo "✅ Docker started successfully"
    else
        echo "❌ Failed to start Docker"
        exit 1
    fi
fi

# Verify Docker is working
echo ""
echo "Verifying Docker installation..."
docker --version
docker compose version

echo ""
echo "✅ Docker is ready to use!"

