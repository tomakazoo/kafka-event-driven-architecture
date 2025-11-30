#!/bin/bash
# Temporary helper to run Docker from Windows Docker Desktop

DOCKER="/mnt/c/Program Files/Docker/Docker/resources/bin/docker.exe"

if [ ! -f "$DOCKER" ]; then
    echo "❌ Docker Desktop not found at expected location"
    echo "Please enable WSL Integration in Docker Desktop Settings"
    exit 1
fi

# Run docker with all passed arguments
"$DOCKER" "$@"

