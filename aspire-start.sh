#!/bin/bash

# B2Connect Aspire Hosting Setup
# Orchestrates all microservices with centralized .NET Aspire orchestration
# Usage: ./aspire-start.sh [Environment] [BuildConfig] [Port]
# Example: ./aspire-start.sh Development Debug 5200

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$SCRIPT_DIR"
SERVICES_DIR="$PROJECT_ROOT/backend/services"
APPHOST_DIR="$SERVICES_DIR/AppHost"
LOGS_DIR="$PROJECT_ROOT/logs"
PID_DIR="$PROJECT_ROOT/.pids"

# Color codes
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Configuration
ENVIRONMENT="${1:-Development}"
BUILD_CONFIG="${2:-Debug}"
APPHOST_PORT="${3:-5200}"
DASHBOARD_PORT="5500"

echo -e "${BLUE}═══════════════════════════════════════════════════════════════${NC}"
echo -e "${BLUE}  B2Connect - .NET Aspire Central Orchestration${NC}"
echo -e "${BLUE}═══════════════════════════════════════════════════════════════${NC}"
echo -e "${CYAN}Environment:${NC}     $ENVIRONMENT"
echo -e "${CYAN}Build Config:${NC}    $BUILD_CONFIG"
echo -e "${CYAN}AppHost Port:${NC}    $APPHOST_PORT"
echo -e "${CYAN}Dashboard Port:${NC}  $DASHBOARD_PORT"
echo -e "${BLUE}═══════════════════════════════════════════════════════════════${NC}"
echo ""

# Setup directories
setup_directories() {
    mkdir -p "$LOGS_DIR"
    mkdir -p "$PID_DIR"
    echo -e "${GREEN}[✓] Directories created${NC}"
}

# Check prerequisites
check_prerequisites() {
    echo -e "${YELLOW}[*] Checking prerequisites...${NC}"
    
    if ! command -v dotnet &> /dev/null; then
        echo -e "${RED}[✗] .NET SDK not found${NC}"
        exit 1
    fi
    
    if [ ! -d "$APPHOST_DIR" ]; then
        echo -e "${RED}[✗] AppHost directory not found: $APPHOST_DIR${NC}"
        exit 1
    fi
    
    echo -e "${GREEN}[✓] All prerequisites met${NC}"
}

# Start Aspire AppHost
start_apphost() {
    echo -e "${YELLOW}[*] Starting .NET Aspire AppHost on port $APPHOST_PORT...${NC}"
    
    cd "$APPHOST_DIR"
    
    # Restore dependencies
    echo -e "${CYAN}    Restoring dependencies...${NC}"
    dotnet restore > /dev/null 2>&1 || true
    
    # Build project
    echo -e "${CYAN}    Building AppHost ($BUILD_CONFIG)...${NC}"
    dotnet build --configuration "$BUILD_CONFIG" --no-restore > /dev/null 2>&1
    
    # Prepare environment
    local log_file="$LOGS_DIR/apphost.log"
    
    # Start AppHost in background
    ASPNETCORE_ENVIRONMENT=$ENVIRONMENT \
    ASPNETCORE_URLS="http://+:$APPHOST_PORT" \
    DOTNET_DASHBOARD_UNSECURED_ALLOW_ANONYMOUS=true \
    dotnet run --configuration "$BUILD_CONFIG" --no-build > "$log_file" 2>&1 &
    
    local pid=$!
    echo $pid > "$PID_DIR/AppHost.pid"
    
    echo -e "${GREEN}[✓] AppHost started (PID: $pid, Port: $APPHOST_PORT)${NC}"
    sleep 2
}

# Health check function
check_health() {
    local url=$1
    local service=$2
    local max_attempts=10
    local attempt=0

    echo -e "${YELLOW}[*] Checking $service health...${NC}"
    
    while [ $attempt -lt $max_attempts ]; do
        if curl -sf "$url/health" > /dev/null 2>&1 || curl -sf "$url" > /dev/null 2>&1; then
            echo -e "${GREEN}[✓] $service is healthy${NC}"
            return 0
        fi
        echo -e "${CYAN}    Attempt $((attempt + 1))/$max_attempts...${NC}"
        attempt=$((attempt + 1))
        sleep 1
    done

    echo -e "${YELLOW}[!] $service health check timeout (may still be starting)${NC}"
    return 0
}

# Cleanup on exit
cleanup() {
    echo -e "${YELLOW}[*] Cleaning up...${NC}"
    # Add cleanup logic if needed
}

trap cleanup EXIT

# Main execution
echo ""
setup_directories
check_prerequisites

echo ""
start_apphost

echo ""
echo -e "${BLUE}═══════════════════════════════════════════════════════════════${NC}"
echo -e "${GREEN}[✓] .NET Aspire Orchestration Started Successfully${NC}"
echo -e "${BLUE}═══════════════════════════════════════════════════════════════${NC}"

echo ""
echo -e "${CYAN}Service URLs:${NC}"
echo -e "  ${BLUE}AppHost Dashboard${NC}   → http://localhost:$APPHOST_PORT"
echo -e "  ${BLUE}Aspire Dashboard${NC}    → http://localhost:$DASHBOARD_PORT"
echo -e ""
echo -e "${CYAN}Logs:${NC}"
echo -e "  ${BLUE}AppHost Log${NC}         → $LOGS_DIR/apphost.log"
echo -e ""
echo -e "${YELLOW}[*] Press Ctrl+C to stop all services${NC}"
echo ""

# Keep script running
wait


echo -e "\n${YELLOW}Health Endpoints:${NC}"
echo -e "  Overall Health:    ${BLUE}http://localhost:9000/health${NC}"
echo -e "  Service Status:    ${BLUE}http://localhost:9000/api/health${NC}"

echo -e "\n${YELLOW}Logs are available at:${NC}"
echo -e "  ${BLUE}$PROJECT_ROOT/logs/${NC}"

echo -e "\n${YELLOW}To stop all services, run:${NC}"
echo -e "  ${BLUE}./aspire-stop.sh${NC}"

# Keep script running
wait
