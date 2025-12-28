#!/bin/bash

# Admin Frontend Login E2E Test Runner
# =====================================
# Schnell-Start-Script für Login-Tests

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
FRONTEND_DIR="${SCRIPT_DIR}/../frontend/Admin"
BACKEND_DIR="${SCRIPT_DIR}/../backend/Orchestration"

echo "🚀 B2Connect Admin Frontend - Login E2E Tests"
echo "=============================================="
echo ""

# Farben
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Funktion zum Prüfen ob Service läuft
check_service() {
    local name=$1
    local url=$2
    
    echo -n "Prüfe ${name}... "
    
    if curl -s -f "${url}" > /dev/null 2>&1; then
        echo -e "${GREEN}✓ läuft${NC}"
        return 0
    else
        echo -e "${RED}✗ nicht erreichbar${NC}"
        return 1
    fi
}

# Step 1: Services überprüfen
echo "📋 Schritt 1: Service-Status überprüfen"
echo "---------------------------------------"

SERVICES_OK=true

if ! check_service "Admin Frontend" "http://localhost:5174"; then
    SERVICES_OK=false
    echo -e "${YELLOW}→ Starte mit: cd frontend/Admin && npm run dev${NC}"
fi

if ! check_service "Admin Gateway" "http://localhost:8080/health"; then
    SERVICES_OK=false
    echo -e "${YELLOW}→ Starte Backend mit: cd backend/Orchestration && dotnet run${NC}"
fi

if ! check_service "Identity Service" "http://localhost:7002/health"; then
    SERVICES_OK=false
fi

echo ""

if [ "$SERVICES_OK" = false ]; then
    echo -e "${RED}❌ Nicht alle Services laufen!${NC}"
    echo ""
    echo "Starte die Services mit:"
    echo "  1. Backend:  cd backend/Orchestration && dotnet run"
    echo "  2. Frontend: cd frontend/Admin && npm run dev"
    echo ""
    exit 1
fi

# Step 2: Dependencies prüfen
echo "📦 Schritt 2: Dependencies prüfen"
echo "-----------------------------------"

if [ ! -d "${FRONTEND_DIR}/node_modules" ]; then
    echo -e "${YELLOW}⚠️  node_modules fehlt. Installiere Dependencies...${NC}"
    cd "${FRONTEND_DIR}"
    npm install
    echo -e "${GREEN}✓ Dependencies installiert${NC}"
else
    echo -e "${GREEN}✓ node_modules vorhanden${NC}"
fi

echo ""

# Step 3: Test-Modus auswählen
echo "🧪 Schritt 3: Test-Modus auswählen"
echo "-----------------------------------"
echo "1) Alle Login-Tests ausführen (Standard)"
echo "2) Login-Tests mit UI (interaktiv)"
echo "3) Login-Tests im Debug-Modus"
echo "4) Nur Security-Tests"
echo "5) Playwright Report anzeigen"
echo "6) Alle E2E-Tests ausführen"
echo ""

read -p "Auswahl (1-6, Enter für Standard): " choice
choice=${choice:-1}

echo ""

cd "${FRONTEND_DIR}"

case $choice in
    1)
        echo -e "${GREEN}▶ Starte alle Login-Tests...${NC}"
        npm run e2e:auth
        ;;
    2)
        echo -e "${GREEN}▶ Starte Login-Tests mit UI...${NC}"
        npx playwright test tests/e2e/auth.spec.ts --ui
        ;;
    3)
        echo -e "${GREEN}▶ Starte Login-Tests im Debug-Modus...${NC}"
        npx playwright test tests/e2e/auth.spec.ts --debug
        ;;
    4)
        echo -e "${GREEN}▶ Starte nur Security-Tests...${NC}"
        npx playwright test tests/e2e/auth.spec.ts -g "spoofing|unauthorized|sensitive"
        ;;
    5)
        echo -e "${GREEN}▶ Öffne Playwright Report...${NC}"
        npm run e2e:report
        ;;
    6)
        echo -e "${GREEN}▶ Starte alle E2E-Tests...${NC}"
        npm run e2e
        ;;
    *)
        echo -e "${RED}Ungültige Auswahl!${NC}"
        exit 1
        ;;
esac

echo ""
echo -e "${GREEN}✅ Tests abgeschlossen!${NC}"
echo ""
echo "📊 Weitere Optionen:"
echo "  • HTML-Report: npm run e2e:report"
echo "  • Mit Trace:   npx playwright test --trace on"
echo "  • Headed Mode: npx playwright test --headed"
echo ""
