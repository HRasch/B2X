#!/bin/bash

# B2X Aspire Start Script
# Führt Pre-Flight Checks durch und startet dann Aspire
# Usage: ./start-aspire.sh [--force]

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
ORCHESTRATION_DIR="$PROJECT_ROOT/backend/AppHost"

# Farben
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

# Parameter
FORCE_START=${1:-}

echo -e "${BLUE}════════════════════════════════════════════════════════════════${NC}"
echo -e "${BLUE}  B2X Aspire Launcher${NC}"
echo -e "${BLUE}════════════════════════════════════════════════════════════════${NC}"
echo ""

# Kritische Ports
CRITICAL_PORTS=(7002 7003 7004 7005 7008 8000 8080 15500)
OPTIONAL_PORTS=(5173 5174 6379 5432 9200 5672)

# ═══════════════════════════════════════════════════════════════
# Pre-Flight Check 1: Prüfe ob bereits eine Instanz läuft
# ═══════════════════════════════════════════════════════════════
echo -e "${YELLOW}[1/4] Prüfe laufende Instanzen...${NC}"

ORCHESTRATION_RUNNING=$(pgrep -f "B2X.AppHost" 2>/dev/null | wc -l | tr -d ' ')
DCP_RUNNING=$(pgrep -f "dcpctrl" 2>/dev/null | wc -l | tr -d ' ')

if [ "$ORCHESTRATION_RUNNING" -gt 0 ] || [ "$DCP_RUNNING" -gt 0 ]; then
    echo -e "${RED}  ⚠️  Aspire/DCP Prozesse bereits aktiv!${NC}"
    
    if [ "$FORCE_START" = "--force" ]; then
        echo -e "${YELLOW}  → Force-Mode: Stoppe vorhandene Prozesse...${NC}"
        "$SCRIPT_DIR/kill-all-services.sh"
        sleep 2
    else
        echo ""
        echo -e "${RED}  Optionen:${NC}"
        echo -e "    1. Stoppen: ${BLUE}./scripts/kill-all-services.sh${NC}"
        echo -e "    2. Force-Start: ${BLUE}./scripts/start-aspire.sh --force${NC}"
        echo ""
        exit 1
    fi
else
    echo -e "${GREEN}  ✅ Keine laufenden Instanzen${NC}"
fi

# ═══════════════════════════════════════════════════════════════
# Pre-Flight Check 2: Prüfe kritische Ports
# ═══════════════════════════════════════════════════════════════
echo -e "${YELLOW}[2/4] Prüfe kritische Ports...${NC}"

BLOCKED_PORTS=()
for port in "${CRITICAL_PORTS[@]}"; do
    if lsof -i :$port -P -n 2>/dev/null | grep -q LISTEN; then
        BLOCKED_PORTS+=($port)
        process=$(lsof -i :$port -P -n 2>/dev/null | grep LISTEN | head -1 | awk '{print $1}')
        echo -e "${RED}  ❌ Port $port blockiert von: $process${NC}"
    fi
done

if [ ${#BLOCKED_PORTS[@]} -gt 0 ]; then
    if [ "$FORCE_START" = "--force" ]; then
        echo -e "${YELLOW}  → Force-Mode: Gebe Ports frei...${NC}"
        for port in "${BLOCKED_PORTS[@]}"; do
            lsof -ti:$port 2>/dev/null | xargs kill -9 2>/dev/null || true
        done
        sleep 1
    else
        echo ""
        echo -e "${RED}  ${#BLOCKED_PORTS[@]} kritische Port(s) blockiert!${NC}"
        echo -e "  Nutze ${BLUE}--force${NC} oder führe manuell aus:"
        echo -e "    ${BLUE}./scripts/kill-all-services.sh${NC}"
        exit 1
    fi
else
    echo -e "${GREEN}  ✅ Alle kritischen Ports frei${NC}"
fi

# ═══════════════════════════════════════════════════════════════
# Pre-Flight Check 3: Prüfe .NET SDK
# ═══════════════════════════════════════════════════════════════
echo -e "${YELLOW}[3/4] Prüfe .NET SDK...${NC}"

if ! command -v dotnet &> /dev/null; then
    echo -e "${RED}  ❌ .NET SDK nicht gefunden!${NC}"
    exit 1
fi

DOTNET_VERSION=$(dotnet --version)
echo -e "${GREEN}  ✅ .NET SDK $DOTNET_VERSION${NC}"

# ═══════════════════════════════════════════════════════════════
# Pre-Flight Check 4: Prüfe Projekt-Build
# ═══════════════════════════════════════════════════════════════
echo -e "${YELLOW}[4/4] Prüfe Orchestration Build...${NC}"

if [ ! -f "$ORCHESTRATION_DIR/bin/Debug/net10.0/B2X.AppHost.dll" ]; then
    echo -e "${YELLOW}  → Build erforderlich...${NC}"
    cd "$ORCHESTRATION_DIR"
    dotnet build -c Debug --nologo -v q
    if [ $? -ne 0 ]; then
        echo -e "${RED}  ❌ Build fehlgeschlagen!${NC}"
        exit 1
    fi
fi
echo -e "${GREEN}  ✅ Build OK${NC}"

# ═══════════════════════════════════════════════════════════════
# Start Aspire
# ═══════════════════════════════════════════════════════════════
echo ""
echo -e "${BLUE}════════════════════════════════════════════════════════════════${NC}"
echo -e "${GREEN}  🚀 Starte Aspire Orchestration...${NC}"
echo -e "${BLUE}════════════════════════════════════════════════════════════════${NC}"
echo ""

cd "$ORCHESTRATION_DIR"

# Umgebungsvariablen setzen
export ASPNETCORE_ENVIRONMENT=Development
export Database__Provider=inmemory
export DOTNET_DASHBOARD_UNSECURED_ALLOW_ANONYMOUS=true
export ASPIRE_ALLOW_UNSECURED_TRANSPORT=true

# Starte mit detaillierter Ausgabe
echo -e "${YELLOW}Dashboard URL: ${BLUE}http://localhost:15500${NC}"
echo ""
echo -e "${YELLOW}Services werden gestartet:${NC}"
echo -e "  • Auth Service        → http://localhost:7002"
echo -e "  • Tenant Service      → http://localhost:7003"
echo -e "  • Localization        → http://localhost:7004"
echo -e "  • Catalog Service     → http://localhost:7005"
echo -e "  • Theming Service     → http://localhost:7008"
echo -e "  • Store Gateway       → http://localhost:8000"
echo -e "  • Admin Gateway       → http://localhost:8080"
echo -e "  • Frontend Store      → http://localhost:5173"
echo -e "  • Frontend Admin      → http://localhost:5174"
echo ""
echo -e "${YELLOW}Logs werden nach /tmp/aspire-startup.log geschrieben${NC}"
echo -e "${YELLOW}Drücke Ctrl+C zum Beenden${NC}"
echo ""

# Starte Aspire
dotnet run --configuration Debug 2>&1 | tee /tmp/aspire-startup.log
