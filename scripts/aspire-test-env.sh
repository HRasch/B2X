#!/bin/bash
# =============================================================================
# Aspire Test Environment Helper
# =============================================================================
# This script starts Aspire services and exports the endpoint URLs as
# environment variables for Playwright E2E tests.
#
# Usage:
#   source scripts/aspire-test-env.sh
#   npm run test:e2e
#
# Or in CI:
#   . scripts/aspire-test-env.sh && npm run test:e2e
# =============================================================================

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"

echo "🚀 Starting Aspire test environment..."

# Check if Aspire is already running
if lsof -i :15500 >/dev/null 2>&1; then
    echo "✅ Aspire Dashboard already running on port 15500"
else
    echo "⏳ Starting Aspire AppHost..."
    cd "$PROJECT_ROOT/AppHost"
    
    # Start Aspire in background with test-friendly settings
    ASPNETCORE_ENVIRONMENT=Development \
    DOTNET_DASHBOARD_UNSECURED_ALLOW_ANONYMOUS=true \
    ASPIRE_ALLOW_UNSECURED_TRANSPORT=true \
    dotnet run &
    
    ASPIRE_PID=$!
    echo "Started Aspire with PID: $ASPIRE_PID"
    
    # Wait for Aspire Dashboard to be ready
    echo "⏳ Waiting for Aspire Dashboard (up to 60s)..."
    for i in {1..60}; do
        if curl -s http://localhost:15500/api/v0/resources >/dev/null 2>&1; then
            echo "✅ Aspire Dashboard ready!"
            break
        fi
        if ! kill -0 $ASPIRE_PID 2>/dev/null; then
            echo "❌ Aspire process died unexpectedly"
            exit 1
        fi
        sleep 1
    done
fi

# Wait for gateways to be healthy
echo "⏳ Waiting for Store Gateway (port 8000)..."
for i in {1..30}; do
    if curl -s http://localhost:8000/health >/dev/null 2>&1; then
        echo "✅ Store Gateway healthy!"
        break
    fi
    sleep 1
done

echo "⏳ Waiting for Admin Gateway (port 8080)..."
for i in {1..30}; do
    if curl -s http://localhost:8080/health >/dev/null 2>&1; then
        echo "✅ Admin Gateway healthy!"
        break
    fi
    sleep 1
done

# Export environment variables for Playwright
# Gateways have fixed ports
export PLAYWRIGHT_STORE_GATEWAY_URL="http://localhost:8000"
export PLAYWRIGHT_ADMIN_GATEWAY_URL="http://localhost:8080"

# Frontends have fixed ports via WithEndpoint configuration
export PLAYWRIGHT_STORE_URL="http://localhost:5173"
export PLAYWRIGHT_ADMIN_URL="http://localhost:5174"

# For backward compatibility
export STORE_PORT="5173"
export ADMIN_PORT="5174"
export PLAYWRIGHT_BASE_URL="http://localhost:5173"

echo ""
echo "📋 Environment variables exported:"
echo "   PLAYWRIGHT_STORE_URL=$PLAYWRIGHT_STORE_URL"
echo "   PLAYWRIGHT_ADMIN_URL=$PLAYWRIGHT_ADMIN_URL"
echo "   PLAYWRIGHT_STORE_GATEWAY_URL=$PLAYWRIGHT_STORE_GATEWAY_URL"
echo "   PLAYWRIGHT_ADMIN_GATEWAY_URL=$PLAYWRIGHT_ADMIN_GATEWAY_URL"
echo ""
echo "🎭 Ready to run Playwright tests!"
echo "   cd frontend/Store && npm run test:e2e"
