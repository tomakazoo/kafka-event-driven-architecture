#!/bin/bash

echo "🔧 Fixing Docker Permissions..."
echo ""

# Check if user is in docker group
if groups | grep -q docker; then
    echo "✅ You are in the docker group"
    echo ""
    echo "To activate in this terminal, run:"
    echo "  newgrp docker"
    echo ""
    echo "Or restart your terminal."
else
    echo "❌ You are NOT in the docker group"
    echo ""
    echo "Adding you to the docker group..."
    sudo usermod -aG docker $USER
    
    echo ""
    echo "✅ Added to docker group!"
    echo ""
    echo "⚠️  IMPORTANT: You need to logout and login again for this to take effect."
    echo ""
    echo "OR activate in this terminal with:"
    echo "  newgrp docker"
    echo ""
    echo "Then verify with:"
    echo "  docker ps"
fi












