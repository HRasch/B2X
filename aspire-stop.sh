#!/bin/bash

# B2Connect Aspire Hosting - Stop all services

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$SCRIPT_DIR"
PID_DIR="$PROJECT_ROOT/.pids"

# Color codes
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}═══════════════════════════════════════════════════════════════${NC}"
echo -e "${BLUE}B2Connect Aspire Hosting - Stopping Services${NC}"
echo -e "${BLUE}═══════════════════════════════════════════════════════════════${NC}"

if [ ! -d "$PID_DIR" ]; then
    echo -e "${YELLOW}[!] No PID directory found. Services may not be running.${NC}"
    exit 0
fi

services=(
    "AppHost"
    "ApiGateway"
    "AuthService"
    "TenantService"
    "LocalizationService"
)

for service in "${services[@]}"; do
    pid_file="$PID_DIR/${service}.pid"
    
    if [ -f "$pid_file" ]; then
        pid=$(cat "$pid_file")
        if kill -0 "$pid" 2>/dev/null; then
            echo -e "${YELLOW}[*] Stopping ${service} (PID: ${pid})...${NC}"
            kill "$pid" 2>/dev/null || true
            sleep 1
            
            # Force kill if still running
            if kill -0 "$pid" 2>/dev/null; then
                kill -9 "$pid" 2>/dev/null || true
                echo -e "${RED}[!] Force killed ${service}${NC}"
            else
                echo -e "${GREEN}[✓] ${service} stopped${NC}"
            fi
        else
            echo -e "${YELLOW}[!] Process ${pid} not found for ${service}${NC}"
        fi
        
        rm -f "$pid_file"
    else
        echo -e "${YELLOW}[!] No PID file for ${service}${NC}"
    fi
done

echo -e "\n${BLUE}═══════════════════════════════════════════════════════════════${NC}"
echo -e "${GREEN}✓ All services stopped${NC}"
echo -e "${BLUE}═══════════════════════════════════════════════════════════════${NC}"

# Cleanup
rmdir "$PID_DIR" 2>/dev/null || true

echo -e "\n${YELLOW}Logs are still available at:${NC}"
echo -e "  ${BLUE}$PROJECT_ROOT/logs/${NC}"
