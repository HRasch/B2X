#!/bin/bash

# B2Connect Service Cleanup Script v2.0
# Stoppt alle laufenden B2Connect Services und gibt Ports frei
# Verbessert: Robustere DCP-Terminierung, Port-Validierung pro Service

set -o pipefail

# Konfiguration
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
MAX_RETRIES=10
RETRY_DELAY=0.5

# Alle B2Connect Ports (einfache Arrays für macOS bash 3.x Kompatibilität)
SERVICE_NAMES=("Auth_Service" "Tenant_Service" "Localization_Service" "Catalog_Service" "Theming_Service" "Store_Gateway" "Admin_Gateway" "Frontend_Store" "Frontend_Admin" "Aspire_Dashboard" "Redis")
SERVICE_PORTS=(7002 7003 7004 7005 7008 8000 8080 5173 5174 15500 6379)

# Funktion: Port freigeben mit Retry
free_port() {
    local port=$1
    local name=$2
    
    for i in $(seq 1 $MAX_RETRIES); do
        local pid=$(lsof -ti:$port 2>/dev/null)
        if [ -z "$pid" ]; then
            return 0
        fi
        
        # Erst SIGTERM, dann SIGKILL
        if [ $i -le 3 ]; then
            kill $pid 2>/dev/null || true
        else
            kill -9 $pid 2>/dev/null || true
        fi
        sleep $RETRY_DELAY
    done
    
    # Finale Prüfung
    if lsof -ti:$port >/dev/null 2>&1; then
        return 1
    fi
    return 0
}

# Funktion: Prozesse nach Pattern beenden
kill_processes() {
    local pattern=$1
    local use_sigkill=${2:-false}
    
    local pids=$(pgrep -f "$pattern" 2>/dev/null || true)
    if [ -n "$pids" ]; then
        if [ "$use_sigkill" = true ]; then
            echo "$pids" | xargs kill -9 2>/dev/null || true
        else
            echo "$pids" | xargs kill 2>/dev/null || true
        fi
    fi
}

echo "🛑 B2Connect Service Cleanup v2.0"
echo "=================================="
echo ""

# PHASE 1: Graceful Shutdown (SIGTERM)
echo "📤 Phase 1: Graceful shutdown..."

# 1.1 Orchestration zuerst (kontrolliert Child-Prozesse)
echo "  → Stopping Orchestration..."
kill_processes "B2Connect.Orchestration" false
sleep 1

# 1.2 DCP Controller (hält alle Ports)
echo "  → Stopping DCP Controller..."
kill_processes "dcpctrl" false
kill_processes "dcpproc" false
sleep 1

# 1.3 Aspire Dashboard
echo "  → Stopping Aspire Dashboard..."
kill_processes "Aspire.Dashboard" false
sleep 1

# PHASE 2: Force Kill (SIGKILL)
echo ""
echo "💀 Phase 2: Force termination..."

# 2.1 DCP MUSS zuerst sterben (hält Port-Handles)
echo "  → Force killing DCP..."
for i in {1..5}; do
    kill_processes "dcpctrl" true
    kill_processes "dcpproc" true
    
    DCP_COUNT=$(pgrep -f "dcpctrl|dcpproc" 2>/dev/null | wc -l | tr -d ' ')
    if [ "$DCP_COUNT" -eq 0 ]; then
        echo "  ✅ DCP terminated"
        break
    fi
    sleep 0.5
done

# 2.2 Alle B2Connect .NET Prozesse
echo "  → Force killing .NET services..."
kill_processes "B2Connect.Orchestration" true
kill_processes "B2Connect.Identity" true
kill_processes "B2Connect.Tenancy" true
kill_processes "B2Connect.Localization" true
kill_processes "B2Connect.Catalog" true
kill_processes "B2Connect.Theming" true
kill_processes "B2Connect.Store" true
kill_processes "B2Connect.Admin" true
kill_processes "Aspire.Dashboard" true
sleep 1

# 2.3 Node/Vite Prozesse (Frontend)
echo "  → Force killing Node/Vite..."
kill_processes "vite" true
kill_processes "esbuild" true
# Nicht killall node - könnte andere Prozesse treffen
sleep 1

# 2.4 MSBuild Workers
echo "  → Stopping MSBuild workers..."
kill_processes "MSBuild.dll" true
kill_processes "VBCSCompiler" true

# PHASE 3: Port-basierte Bereinigung
echo ""
echo "🔌 Phase 3: Port cleanup..."

FAILED_PORTS=()

# Iteriere über alle Ports (macOS bash 3.x kompatibel)
for i in "${!SERVICE_PORTS[@]}"; do
    port=${SERVICE_PORTS[$i]}
    service=${SERVICE_NAMES[$i]}
    
    if free_port $port "$service"; then
        echo "  ✅ Port $port ($service) - frei"
    else
        echo "  ❌ Port $port ($service) - BLOCKIERT"
        FAILED_PORTS+=("$port:$service")
    fi
done

# PHASE 4: Finale Validierung
echo ""
echo "🔍 Phase 4: Validierung..."

# Warte kurz für OS-Bereinigung
sleep 2

# Prüfe alle Ports nochmal
ALL_PORTS=$(echo "${SERVICE_PORTS[@]}" | tr ' ' ',')
STILL_BLOCKED=$(lsof -ti:$ALL_PORTS 2>/dev/null | wc -l | tr -d ' ')

if [ "$STILL_BLOCKED" -gt 0 ]; then
    echo ""
    echo "⚠️  $STILL_BLOCKED Port(s) noch blockiert. Aggressive Bereinigung..."
    
    # Letzte Chance: Alle PIDs auf diesen Ports killen
    lsof -ti:$ALL_PORTS 2>/dev/null | xargs kill -9 2>/dev/null || true
    sleep 2
    
    # Finale Prüfung
    FINAL_BLOCKED=$(lsof -ti:$ALL_PORTS 2>/dev/null | wc -l | tr -d ' ')
    
    if [ "$FINAL_BLOCKED" -gt 0 ]; then
        echo ""
        echo "❌ FEHLER: $FINAL_BLOCKED Port(s) konnten nicht freigegeben werden!"
        echo ""
        echo "Blockierte Ports:"
        lsof -i -P -n | grep -E ":($ALL_PORTS)" | grep LISTEN
        echo ""
        echo "Manuelle Bereinigung erforderlich:"
        echo "  sudo lsof -ti:PORT | xargs sudo kill -9"
        exit 1
    fi
fi

# Erfolg
echo ""
echo "════════════════════════════════════════"
echo "✅ Alle Services gestoppt!"
echo ""
echo "Freigegebene Ports:"
for i in "${!SERVICE_PORTS[@]}"; do
    printf "  • %-20s : %s\n" "${SERVICE_NAMES[$i]}" "${SERVICE_PORTS[$i]}"
done
echo ""
echo "🚀 Bereit zum Starten:"
echo "   cd backend/Orchestration && dotnet run"
echo "════════════════════════════════════════"
