#!/usr/bin/env bash

# B2X Local Service Discovery Startup Script
# Starts all backend services with proper service discovery configuration

set -euo pipefail

SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
BACKEND_DIR="$SCRIPT_DIR/backend/services"

# Color codes
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${BLUE}═══════════════════════════════════════════════════════════════${NC}"
echo -e "${BLUE}  B2X Local Service Startup (Service Discovery Mode)${NC}"
echo -e "${BLUE}═══════════════════════════════════════════════════════════════${NC}"

# Kill any existing processes on our ports
echo -e "${YELLOW}[1/4] Cleaning up existing processes...${NC}"
pkill -f "dotnet run" || true
sleep 1

# Define services with service path and port
declare -a SERVICE_NAMES=("api-gateway" "auth-service" "tenant-service" "localization-service")
declare -a SERVICE_PATHS=("$BACKEND_DIR/api-gateway" "$BACKEND_DIR/auth-service" "$BACKEND_DIR/tenant-service" "$BACKEND_DIR/localization-service")
declare -a SERVICE_PORTS=(6000 5001 5002 5003)
declare -a SERVICE_DISPLAY=("API Gateway" "Auth Service" "Tenant Service" "Localization Service")

# Start services
echo -e "${YELLOW}[2/4] Starting backend services...${NC}"

for i in "${!SERVICE_NAMES[@]}"; do
    service_name="${SERVICE_NAMES[$i]}"
    service_path="${SERVICE_PATHS[$i]}"
    service_port="${SERVICE_PORTS[$i]}"
    service_display="${SERVICE_DISPLAY[$i]}"
    
    if [ -d "$service_path" ]; then
        echo -e "${GREEN}  ✓${NC} Starting $service_display (port $service_port)..."
        (cd "$service_path" && dotnet run > /tmp/B2X-${service_name}.log 2>&1 &)
    fi
done

sleep 3

# Verify services are running
echo -e "${YELLOW}[3/4] Verifying services...${NC}"

for i in "${!SERVICE_NAMES[@]}"; do
    service_port="${SERVICE_PORTS[$i]}"
    service_display="${SERVICE_DISPLAY[$i]}"
    
    # Simple check without timeout
    if curl -s http://localhost:$service_port/health > /dev/null 2>&1; then
        echo -e "${GREEN}  ✓${NC} $service_display is running on port $service_port"
    else
        echo -e "${YELLOW}  ⚠${NC} $service_display may still be starting (port $service_port)"
    fi
done

# Environment configuration
echo -e "${YELLOW}[4/4] Configuration${NC}"
echo -e "${GREEN}  ✓${NC} API Gateway:       http://localhost:6000"
echo -e "${GREEN}  ✓${NC} Auth Service:      http://localhost:5001"
echo -e "${GREEN}  ✓${NC} Tenant Service:    http://localhost:5002"
echo -e "${GREEN}  ✓${NC} Localization Svc:  http://localhost:5003"
echo ""

# Frontend startup instructions
echo -e "${BLUE}───────────────────────────────────────────────────────────────${NC}"
echo -e "${GREEN}To start the frontend:${NC}"
echo ""
echo -e "  ${YELLOW}Frontend (port 5173):${NC}"
echo -e "    cd frontend"
echo -e "    VITE_API_GATEWAY_URL=http://localhost:6000 npm run dev"
echo ""
echo -e "  ${YELLOW}Admin Frontend (port 5174):${NC}"
echo -e "    cd src/Admin"
echo -e "    VITE_API_GATEWAY_URL=http://localhost:6000 npm run dev"
echo ""
echo -e "${BLUE}───────────────────────────────────────────────────────────────${NC}"
echo -e "${GREEN}Service logs:${NC}"
echo -e "  tail -f /tmp/B2X-*.log"
echo ""
echo -e "${BLUE}═══════════════════════════════════════════════════════════════${NC}"

# Keep the script running and show logs
echo -e "${BLUE}Monitoring services... (Press Ctrl+C to stop)${NC}"
while true; do
    sleep 1
    for i in "${!SERVICE_NAMES[@]}"; do
        service_port="${SERVICE_PORTS[$i]}"
        
        if ! lsof -i :$service_port > /dev/null 2>&1; then
            echo -e "${YELLOW}[WARNING]${NC} Service on port $service_port may have crashed. Check logs."
        fi
    done
done
