#!/usr/bin/env bash

# B2Connect Service Cleanup Script
# Stops all running B2Connect services

echo "Stopping B2Connect services..."

# Kill all dotnet processes
pkill -f "dotnet run" || echo "No dotnet processes found"

# Kill specific services if still running
for port in 5001 5002 5003 6000 15500; do
    if lsof -i :$port > /dev/null 2>&1; then
        echo "Cleaning up port $port..."
        lsof -i :$port | awk 'NR!=1 {print $2}' | xargs kill -9 2>/dev/null || true
    fi
done

echo "✓ All services stopped"
echo "Logs saved in /tmp/b2connect-*.log"
