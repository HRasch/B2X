#!/usr/bin/env bash

# B2Connect - Start All Services Script
# Orchestrates Backend (Aspire) and Frontend development servers

echo "🚀 B2Connect - Complete Environment Startup"
echo "═════════════════════════════════════════════"
echo ""

# Get script directory
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
cd "$SCRIPT_DIR"

# Configuration
ENVIRONMENT="${1:-Development}"
BUILD_CONFIG="${2:-Debug}"
BACKEND_PORT="${3:-5200}"

# Color codes
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

echo -e "${CYAN}Configuration:${NC}"
echo -e "  Environment:  $ENVIRONMENT"
echo -e "  Build Config: $BUILD_CONFIG"
echo -e "  Backend Port: $BACKEND_PORT"
echo ""

# Cleanup function
cleanup() {
    echo -e "${YELLOW}[*] Shutting down services...${NC}"
    pkill -f "aspire-start.sh" 2>/dev/null || true
    pkill -f "npm run dev" 2>/dev/null || true
    pkill -f "dotnet run" 2>/dev/null || true
    pkill -f "dotnet watch" 2>/dev/null || true
    sleep 1
    echo -e "${GREEN}[✓] Services stopped${NC}"
}

trap cleanup EXIT INT TERM

# Kill any existing processes
echo -e "${YELLOW}[*] Cleaning up old processes...${NC}"
pkill -f "aspire-start.sh" 2>/dev/null || true
pkill -f "npm run dev" 2>/dev/null || true
pkill -f "dotnet run" 2>/dev/null || true
pkill -f "dotnet watch" 2>/dev/null || true
sleep 1

echo -e "${GREEN}[✓] Old processes cleaned${NC}"
echo ""

# Start Backend (Aspire)
echo -e "${BLUE}═════════════════════════════════════════════${NC}"
echo -e "${YELLOW}[1/2] Starting Backend Services (.NET Aspire)...${NC}"
echo -e "${BLUE}═════════════════════════════════════════════${NC}"
echo ""

./aspire-start.sh "$ENVIRONMENT" "$BUILD_CONFIG" "$BACKEND_PORT" &
BACKEND_PID=$!
sleep 5

# Start Frontend
echo ""
echo -e "${BLUE}═════════════════════════════════════════════${NC}"
echo -e "${YELLOW}[2/2] Starting Frontend (Vite Dev Server)...${NC}"
echo -e "${BLUE}═════════════════════════════════════════════${NC}"
echo ""

cd "$SCRIPT_DIR/frontend-admin"
npm run dev &
FRONTEND_PID=$!

echo ""
echo -e "${BLUE}═════════════════════════════════════════════${NC}"
echo -e "${GREEN}[✓] All services started successfully!${NC}"
echo -e "${BLUE}═════════════════════════════════════════════${NC}"
echo ""

echo -e "${CYAN}Service URLs:${NC}"
echo -e "  ${BLUE}Backend Aspire${NC}     → http://localhost:$BACKEND_PORT"
echo -e "  ${BLUE}Frontend (Vite)${NC}    → http://localhost:5173"
echo -e "  ${BLUE}Aspire Dashboard${NC}   → http://localhost:5500"
echo ""

echo -e "${CYAN}Logs:${NC}"
echo -e "  ${BLUE}Backend${NC}            → logs/apphost.log"
echo -e "  ${BLUE}Frontend${NC}           → Terminal output"
echo ""

echo -e "${YELLOW}[*] Press Ctrl+C to stop all services${NC}"
echo ""

# Wait for both processes
wait $BACKEND_PID $FRONTEND_PID 2>/dev/null || true

