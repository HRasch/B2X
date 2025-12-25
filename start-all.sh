#!/bin/bash

# B2Connect - Start All Services Script

echo "🚀 B2Connect - Alle Services starten"
echo "===================================="
echo ""

# Get script directory
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
cd "$SCRIPT_DIR"

# Function to open new terminal tab and run command
run_in_terminal() {
    local title=$1
    local command=$2
    
    osascript <<EOF
tell application "Terminal"
    do script "cd '$SCRIPT_DIR' && $command"
    set current settings of window 1 to settings set "Basic"
end tell
EOF
    
    sleep 0.5
}

# Kill any existing processes
echo "🛑 Beende alte Prozesse..."
pkill -f "npm run dev" 2>/dev/null || true
pkill -f "dotnet run" 2>/dev/null || true
pkill -f "dotnet watch" 2>/dev/null || true
sleep 1

echo "✅ Alte Prozesse beendet"
echo ""

echo "📱 Starte Services in separaten Terminals..."
echo ""

# Terminal 1: Frontend
echo "1️⃣  Frontend wird gestartet (Port 3000)..."
run_in_terminal "Frontend" "cd frontend && npm run dev"
sleep 1

# Terminal 2: API Gateway
echo "2️⃣  API Gateway wird gestartet (Port 5000)..."
run_in_terminal "API Gateway" "cd backend/services/api-gateway && dotnet run"
sleep 1

# Terminal 3: Auth Service
echo "3️⃣  Auth Service wird gestartet (Port 5001)..."
run_in_terminal "Auth Service" "cd backend/services/auth-service && dotnet run"
sleep 1

# Terminal 4: Tenant Service
echo "4️⃣  Tenant Service wird gestartet (Port 5002)..."
run_in_terminal "Tenant Service" "cd backend/services/tenant-service && dotnet run"
sleep 1

# Terminal 5: AppHost
echo "5️⃣  AppHost wird gestartet (Port 15500)..."
run_in_terminal "AppHost" "cd backend/services/AppHost && dotnet run"

echo ""
echo "✅ Alle Services wurden gestartet!"
echo ""
echo "📍 Zugriffspunkte:"
echo "   Frontend:        http://localhost:3000"
echo "   API Gateway:     http://localhost:5000"
echo "   Auth Service:    http://localhost:5001"
echo "   Tenant Service:  http://localhost:5002"
echo "   AppHost:         http://localhost:15500"
echo ""
echo "💡 Tipp: Überprüfe den Health Status mit:"
echo "   ./health-check.sh"
echo ""
