#!/bin/bash
# Cleanup script for macOS CDP port conflicts
# This script kills lingering dcpctrl processes and frees up ports

echo "🧹 Cleaning up CDP port conflicts..."

# Kill all dcpctrl processes
echo "Killing dcpctrl processes..."
pkill -9 -f "dcpctrl" 2>/dev/null || echo "  No dcpctrl processes found"

# Kill all B2X processes
echo "Killing all B2X services..."
pkill -9 -f "B2X" 2>/dev/null || echo "  No B2X processes found"

# Give the system a moment to release ports
sleep 2

# Check if ports are free
echo ""
echo "✅ Port status:"
for port in 5173 5174 7002 7003 7004 7005 7008 8000 8080; do
  if lsof -i :$port >/dev/null 2>&1; then
    echo "  ⚠️  Port $port: STILL IN USE"
  else
    echo "  ✓ Port $port: FREE"
  fi
done

echo ""
echo "✅ Cleanup complete. Ready to start fresh!"
