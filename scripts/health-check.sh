#!/usr/bin/env bash

# B2X Health Check Script

set -euo pipefail

echo "🏥 B2X Health Check"
echo "=========================="
echo ""

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

check_port() {
    local port=$1
    local name=$2
    
    if lsof -Pi :$port -sTCP:LISTEN -t >/dev/null 2>&1 ; then
        echo -e "${GREEN}✅${NC} $name (Port $port): RUNNING"
        return 0
    else
        echo -e "${RED}❌${NC} $name (Port $port): DOWN"
        return 1
    fi
}

check_health_endpoint() {
    local url=$1
    local name=$2
    
    if response=$(curl -s -m 2 "$url" 2>/dev/null); then
        echo -e "${GREEN}✅${NC} $name Health: HEALTHY"
        return 0
    else
        echo -e "${RED}❌${NC} $name Health: UNREACHABLE"
        return 1
    fi
}

echo "📡 Port Status:"
echo "---------------"
check_port 3000 "Frontend (Vue.js)"
check_port 5000 "API Gateway"
check_port 5001 "Auth Service"
check_port 5002 "Tenant Service"
check_port 15500 "Aspire Dashboard"
echo ""

echo "🏥 Service Health:"
echo "------------------"
check_health_endpoint "http://localhost:5000/health" "API Gateway"
check_health_endpoint "http://localhost:15500/api/health" "AppHost"
echo ""

echo "🌐 Frontend Connectivity:"
echo "------------------------"
check_port 3000 "Dev Server"
echo ""

echo "📊 Full System Status:"
echo "---------------------"

all_good=true

for port in 3000 5000 5001 5002 15500; do
    if ! lsof -Pi :$port -sTCP:LISTEN -t >/dev/null 2>&1 ; then
        all_good=false
        break
    fi
done

if [ "$all_good" = true ]; then
    echo -e "${GREEN}✅ Alle Services sind ONLINE${NC}"
    echo ""
    echo "📍 Zugriffspunkte:"
    echo "   Frontend:        http://localhost:3000"
    echo "   Aspire Dashboard: http://localhost:15500"
    echo "   API Gateway:     http://localhost:5000"
else
    echo -e "${RED}❌ Einige Services sind offline${NC}"
    echo ""
    echo "💡 Tipps:"
    echo "   1. Starte das Projekt: Cmd+Shift+D → F5"
    echo "   2. Überprüfe Logs in VS Code Terminal"
    echo "   3. Führe aus: ./start-vscode.sh"
    echo "   4. Siehe: SOCKET_ERRORS.md für Diagnose"
fi

echo ""
