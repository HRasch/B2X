#!/bin/bash

# B2Connect Aspire Hosting Setup
# Orchestrates all services for centralized hosting

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$SCRIPT_DIR/.."
SERVICES_DIR="$PROJECT_ROOT/backend/services"

# Color codes
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Configuration
ENVIRONMENT="${1:-Development}"
BUILD_CONFIG="${2:-Debug}"

echo -e "${BLUE}═══════════════════════════════════════════════════════════════${NC}"
echo -e "${BLUE}B2Connect Aspire Hosting - Central Orchestration${NC}"
echo -e "${BLUE}Environment: ${ENVIRONMENT} | Build: ${BUILD_CONFIG}${NC}"
echo -e "${BLUE}═══════════════════════════════════════════════════════════════${NC}"

# Function to start a service
start_service() {
    local service_name=$1
    local service_path=$2
    local port=$3
    local log_file="$PROJECT_ROOT/logs/${service_name}.log"

    echo -e "${YELLOW}[*] Starting ${service_name} on port ${port}...${NC}"

    mkdir -p "$PROJECT_ROOT/logs"
    
    cd "$service_path"
    
    # Build if needed
    if [ "$BUILD_CONFIG" = "Release" ]; then
        dotnet build --configuration Release --no-restore > /dev/null 2>&1
    fi

    # Start service in background
    ASPNETCORE_ENVIRONMENT=$ENVIRONMENT \
    ASPNETCORE_URLS="http://+:${port}" \
    dotnet run --configuration "$BUILD_CONFIG" --no-build > "$log_file" 2>&1 &
    
    local PID=$!
    echo -e "${GREEN}[+] ${service_name} started (PID: ${PID})${NC}"
    echo "$PID" > "$PROJECT_ROOT/.pids/${service_name}.pid"
}

# Create PID directory
mkdir -p "$PROJECT_ROOT/.pids"

# Start all services
echo -e "\n${YELLOW}[*] Starting all services...${NC}\n"

start_service "LocalizationService" "$SERVICES_DIR/LocalizationService" "5003"
sleep 2

start_service "TenantService" "$SERVICES_DIR/tenant-service" "5002"
sleep 2

start_service "AuthService" "$SERVICES_DIR/auth-service" "5001"
sleep 2

start_service "ApiGateway" "$SERVICES_DIR/api-gateway" "5000"
sleep 2

start_service "AppHost" "$SERVICES_DIR/AppHost" "9000"
sleep 3

# Health check
echo -e "\n${YELLOW}[*] Performing health checks...${NC}\n"

check_service() {
    local name=$1
    local port=$2
    local max_attempts=5
    local attempt=0

    while [ $attempt -lt $max_attempts ]; do
        if curl -s "http://localhost:${port}/health" > /dev/null 2>&1; then
            echo -e "${GREEN}[✓] ${name} is healthy${NC}"
            return 0
        fi
        attempt=$((attempt + 1))
        sleep 1
    done

    echo -e "${RED}[✗] ${name} health check failed${NC}"
    return 1
}

check_service "LocalizationService" 5003
check_service "TenantService" 5002
check_service "AuthService" 5001
check_service "ApiGateway" 5000
check_service "AppHost" 9000

echo -e "\n${BLUE}═══════════════════════════════════════════════════════════════${NC}"
echo -e "${GREEN}✓ All services started successfully!${NC}"
echo -e "${BLUE}═══════════════════════════════════════════════════════════════${NC}"

echo -e "\n${YELLOW}Service URLs:${NC}"
echo -e "  AppHost Dashboard: ${BLUE}http://localhost:9000${NC}"
echo -e "  API Gateway:       ${BLUE}http://localhost:5000${NC}"
echo -e "  Auth Service:      ${BLUE}http://localhost:5001${NC}"
echo -e "  Tenant Service:    ${BLUE}http://localhost:5002${NC}"
echo -e "  Localization:      ${BLUE}http://localhost:5003${NC}"

echo -e "\n${YELLOW}Health Endpoints:${NC}"
echo -e "  Overall Health:    ${BLUE}http://localhost:9000/health${NC}"
echo -e "  Service Status:    ${BLUE}http://localhost:9000/api/health${NC}"

echo -e "\n${YELLOW}Logs are available at:${NC}"
echo -e "  ${BLUE}$PROJECT_ROOT/logs/${NC}"

echo -e "\n${YELLOW}To stop all services, run:${NC}"
echo -e "  ${BLUE}./aspire-stop.sh${NC}"

# Keep script running
wait
