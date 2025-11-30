#!/bin/bash

set -e

echo "🐳 Installing Docker Engine in WSL 2"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""

# Check if running as root
if [ "$EUID" -ne 0 ]; then 
    echo "⚠️  This script requires sudo privileges."
    echo "You may be prompted for your password."
    echo ""
fi

# Remove old Docker installations
echo "📦 Step 1/6: Removing old Docker installations (if any)..."
sudo apt-get remove -y docker docker-engine docker.io containerd runc 2>/dev/null || true

# Update package index
echo ""
echo "📦 Step 2/6: Updating package index..."
sudo apt-get update

# Install prerequisites
echo ""
echo "📦 Step 3/6: Installing prerequisites..."
sudo apt-get install -y \
    ca-certificates \
    curl \
    gnupg \
    lsb-release

# Add Docker's official GPG key
echo ""
echo "🔑 Step 4/6: Adding Docker's GPG key..."
sudo mkdir -p /etc/apt/keyrings
curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo gpg --dearmor -o /etc/apt/keyrings/docker.gpg

# Set up the repository
echo ""
echo "📦 Step 5/6: Setting up Docker repository..."
echo \
  "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] https://download.docker.com/linux/ubuntu \
  $(lsb_release -cs) stable" | sudo tee /etc/apt/sources.list.d/docker.list > /dev/null

# Install Docker Engine
echo ""
echo "📦 Step 6/6: Installing Docker Engine..."
sudo apt-get update
sudo apt-get install -y docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin

# Add current user to docker group
echo ""
echo "👤 Adding $USER to docker group..."
sudo usermod -aG docker $USER

# Start Docker service
echo ""
echo "🚀 Starting Docker service..."
sudo service docker start

echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "✅ Docker Engine installed successfully!"
echo ""
echo "⚠️  IMPORTANT: To use Docker without sudo, you need to:"
echo "   1. Log out and log back in, OR"
echo "   2. Run: newgrp docker"
echo ""
echo "Verify installation:"
echo "   docker --version"
echo "   docker compose version"
echo ""

