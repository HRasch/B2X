#!/bin/bash
# B2Connect Service Orchestrator for macOS
# Startet alle Services in separaten Terminals

set -e

PROJECT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

echo "🚀 B2Connect Service Orchestrator Starting..."
echo ""

# Colors
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Check if required commands exist
check_command() {
    if ! command -v $1 &> /dev/null; then
        echo "❌ $1 is not installed"
        exit 1
    fi
}

check_command "dotnet"
check_command "npm"

echo -e "${GREEN}✓ All dependencies found${NC}"
echo ""

# Function to run service
run_service() {
    local name=$1
    local path=$2
    local port=$3
    
    echo -e "${BLUE}▶ Starting $name (Port $port)...${NC}"
    cd "$path"
    dotnet run > /tmp/b2connect_${name}.log 2>&1 &
    local pid=$!
    echo "  PID: $pid"
    echo $pid > /tmp/b2connect_${name}.pid
    sleep 2
}

# Kill services on exit
cleanup() {
    echo ""
    echo -e "${YELLOW}🛑 Stopping all services...${NC}"
    
    for service in auth tenant localization; do
        if [ -f "/tmp/b2connect_${service}.pid" ]; then
            pid=$(cat /tmp/b2connect_${service}.pid)
            kill $pid 2>/dev/null || true
            rm /tmp/b2connect_${service}.pid
        fi
    done
    
    echo -e "${GREEN}✓ All services stopped${NC}"
}

trap cleanup EXIT

# Start services
echo -e "${YELLOW}📊 Services Starting:${NC}"
echo ""

run_service "auth" "$PROJECT_DIR/backend/services/auth-service" 9002
run_service "tenant" "$PROJECT_DIR/backend/services/tenant-service" 9003
run_service "localization" "$PROJECT_DIR/backend/services/LocalizationService" 9004

echo ""
echo -e "${GREEN}✅ All services started!${NC}"
echo ""
echo -e "${BLUE}Service URLs:${NC}"
echo "  • Auth Service:         http://localhost:9002"
echo "  • Tenant Service:       http://localhost:9003"
echo "  • Localization Service: http://localhost:9004"
echo ""
echo -e "${YELLOW}Frontend services:${NC}"
echo "  • Customer App:   Port 5173 (npm run dev)"
echo "  • Admin App:      Port 5174 (npm run dev -- --port 5174)"
echo ""
echo -e "${YELLOW}View logs:${NC}"
echo "  • tail -f /tmp/b2connect_auth.log"
echo "  • tail -f /tmp/b2connect_tenant.log"
echo "  • tail -f /tmp/b2connect_localization.log"
echo ""
echo -e "${YELLOW}Press Ctrl+C to stop all services${NC}"

# Keep script running
wait
