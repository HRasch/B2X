#!/bin/bash
# Get dynamic service ports from running Aspire services
# Usage: source scripts/get-aspire-ports.sh && npm run e2e

echo "🔍 Discovering Aspire service ports..."

# Parse listening ports from lsof output and match to processes
get_port_for_process() {
    local process_name="$1"
    local pid=$(pgrep -f "$process_name" | head -1)
    if [ -n "$pid" ]; then
        # Get first IPv4 listening port for this process
        lsof -i -P -n -a -p "$pid" 2>/dev/null | grep LISTEN | grep IPv4 | head -1 | awk '{print $9}' | cut -d: -f2
    fi
}

# Discover frontend ports from Vite processes
STORE_PORT=$(lsof -i -P -n | grep LISTEN | grep "node.*vite" | grep -v IPv6 | head -1 | awk '{print $9}' | cut -d: -f2)
ADMIN_PORT=$(lsof -i -P -n | grep LISTEN | grep "node.*vite" | grep -v IPv6 | tail -1 | awk '{print $9}' | cut -d: -f2)

# Check gateway ports (these are typically fixed in Aspire config)
if curl -s --max-time 1 -o /dev/null -w "%{http_code}" http://localhost:8000 2>/dev/null | grep -q "200\|404"; then
    STORE_GATEWAY_PORT=8000
fi
if curl -s --max-time 1 -o /dev/null -w "%{http_code}" http://localhost:8080 2>/dev/null | grep -q "200\|404"; then
    ADMIN_GATEWAY_PORT=8080
fi

# Export discovered ports (with defaults)
export STORE_PORT="${STORE_PORT:-5173}"
export ADMIN_PORT="${ADMIN_PORT:-5174}"
export STORE_GATEWAY_PORT="${STORE_GATEWAY_PORT:-8000}"
export ADMIN_GATEWAY_PORT="${ADMIN_GATEWAY_PORT:-8080}"

# Export URLs for Playwright
export PLAYWRIGHT_STORE_URL="http://localhost:${STORE_PORT}"
export PLAYWRIGHT_ADMIN_URL="http://localhost:${ADMIN_PORT}"

echo "✅ Service ports discovered:"
echo "   STORE_PORT=$STORE_PORT (Frontend Store)"
echo "   ADMIN_PORT=$ADMIN_PORT (Frontend Admin)"
echo "   STORE_GATEWAY_PORT=$STORE_GATEWAY_PORT (Store API)"
echo "   ADMIN_GATEWAY_PORT=$ADMIN_GATEWAY_PORT (Admin API)"
echo ""
echo "📝 Playwright URLs:"
echo "   PLAYWRIGHT_STORE_URL=$PLAYWRIGHT_STORE_URL"
echo "   PLAYWRIGHT_ADMIN_URL=$PLAYWRIGHT_ADMIN_URL"
