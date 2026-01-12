#!/usr/bin/env bash

# B2X Aspire Hosting - Stop all services
# Gracefully stops the .NET Aspire orchestration

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="${SCRIPT_DIR%/scripts}"
PID_DIR="$PROJECT_ROOT/.pids"
LOGS_DIR="$PROJECT_ROOT/logs"

# Color codes
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

echo -e "${BLUE}═══════════════════════════════════════════════════════════════${NC}"
echo -e "${BLUE}  B2X - .NET Aspire - Stopping Services${NC}"
echo -e "${BLUE}═══════════════════════════════════════════════════════════════${NC}"
echo ""

# Kill AppHost
stop_service() {
    local service=$1
    local pid_file="$PID_DIR/${service}.pid"
    
    if [ -f "$pid_file" ]; then
        local pid=$(cat "$pid_file")
        if kill -0 "$pid" 2>/dev/null; then
            echo -e "${YELLOW}[*] Stopping ${service} (PID: ${pid})...${NC}"
            
            # Graceful shutdown with SIGTERM
            kill -TERM "$pid" 2>/dev/null || true
            
            # Wait for graceful shutdown
            local wait_count=0
            while kill -0 "$pid" 2>/dev/null && [ $wait_count -lt 5 ]; do
                sleep 1
                wait_count=$((wait_count + 1))
            done
            
            # Force kill if still running
            if kill -0 "$pid" 2>/dev/null; then
                echo -e "${RED}[!] Force killing ${service}...${NC}"
                kill -9 "$pid" 2>/dev/null || true
                echo -e "${RED}[!] ${service} force killed${NC}"
            else
                echo -e "${GREEN}[✓] ${service} stopped gracefully${NC}"
            fi
            
            rm -f "$pid_file"
        else
            echo -e "${YELLOW}[!] ${service} not running (PID file exists)${NC}"
            rm -f "$pid_file"
        fi
    else
        echo -e "${YELLOW}[!] ${service} PID file not found${NC}"
    fi
}

# Stop AppHost
if [ -d "$PID_DIR" ]; then
    stop_service "AppHost"
else
    echo -e "${YELLOW}[!] No PID directory found. Services may not be running.${NC}"
fi

# Cleanup
echo ""
echo -e "${BLUE}═══════════════════════════════════════════════════════════════${NC}"
echo -e "${GREEN}[✓] All services stopped${NC}"
echo -e "${BLUE}═══════════════════════════════════════════════════════════════${NC}"
echo ""

# Kill any lingering processes
echo -e "${YELLOW}[*] Cleaning up lingering processes...${NC}"
pkill -f "dotnet run" 2>/dev/null || true
pkill -f "dotnet watch" 2>/dev/null || true
pkill -f "npm run dev" 2>/dev/null || true

echo -e "${GREEN}[✓] Cleanup complete${NC}"
echo ""

# Cleanup PID directory
rmdir "$PID_DIR" 2>/dev/null || true

echo -e "${CYAN}Logs available at:${NC}"
echo -e "  ${BLUE}$LOGS_DIR/${NC}"
echo ""
