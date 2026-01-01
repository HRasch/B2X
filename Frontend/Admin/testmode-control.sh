#!/bin/bash

# TestMode Control Script
# Einfache Möglichkeit, den TestMode zu aktivieren/deaktivieren

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

log_info() {
    echo -e "${BLUE}ℹ️  $1${NC}"
}

log_success() {
    echo -e "${GREEN}✅ $1${NC}"
}

log_warning() {
    echo -e "${YELLOW}⚠️  $1${NC}"
}

log_error() {
    echo -e "${RED}❌ $1${NC}"
}

show_help() {
    echo "TestMode Control Script"
    echo ""
    echo "Usage: $0 [command]"
    echo ""
    echo "Commands:"
    echo "  enable     - Aktiviert TestMode für die nächste Session"
    echo "  disable    - Deaktiviert TestMode"
    echo "  status     - Zeigt aktuellen TestMode Status"
    echo "  demo       - Führt TestMode Demo aus (benötigt laufenden Dev Server)"
    echo "  help       - Zeigt diese Hilfe"
    echo ""
    echo "Beispiele:"
    echo "  $0 enable"
    echo "  $0 demo"
    echo ""
    echo "TestMode Features:"
    echo "  • Automatische Fehlerbehebung"
    echo "  • Echtzeit Action-Monitoring"
    echo "  • Debug Panel (Ctrl+Shift+T)"
    echo "  • Performance-Monitoring"
    echo "  • Log-Export Funktion"
}

enable_testmode() {
    log_info "Aktiviere TestMode..."

    # Set localStorage flag (funktioniert nur wenn Browser offen ist)
    # Für globale Aktivierung verwenden wir eine andere Methode

    # Erstelle eine kleine HTML-Datei zum Aktivieren
    cat > "$PROJECT_ROOT/frontend/Admin/enable-testmode.html" << 'EOF'
<!DOCTYPE html>
<html>
<head>
    <title>TestMode Aktivierung</title>
    <style>
        body { font-family: Arial, sans-serif; padding: 20px; }
        .success { color: green; }
        .info { color: blue; }
    </style>
</head>
<body>
    <h1>TestMode Aktivierung</h1>
    <p class="info">Öffne diese Seite in einem neuen Tab und der TestMode wird aktiviert.</p>

    <script>
        // Aktiviere TestMode
        localStorage.setItem('testModeEnabled', 'true');

        // Zeige Erfolgsmeldung
        document.body.innerHTML += '<p class="success">✅ TestMode wurde aktiviert!</p>';
        document.body.innerHTML += '<p>Sie können jetzt zur Admin-Anwendung zurückkehren.</p>';
        document.body.innerHTML += '<p><a href="http://localhost:5174">→ Zur Admin-Anwendung</a></p>';

        // Zeige aktuellen Status
        setTimeout(() => {
            const status = localStorage.getItem('testModeEnabled');
            console.log('TestMode Status:', status);
        }, 100);
    </script>
</body>
</html>
EOF

    log_success "TestMode wurde aktiviert!"
    log_info "Öffnen Sie $PROJECT_ROOT/frontend/Admin/enable-testmode.html in Ihrem Browser"
    log_info "Alternativ: Fügen Sie ?testmode=true zur URL hinzu"
}

disable_testmode() {
    log_info "Deaktiviere TestMode..."

    # Erstelle eine kleine HTML-Datei zum Deaktivieren
    cat > "$PROJECT_ROOT/frontend/Admin/disable-testmode.html" << 'EOF'
<!DOCTYPE html>
<html>
<head>
    <title>TestMode Deaktivierung</title>
    <style>
        body { font-family: Arial, sans-serif; padding: 20px; }
        .success { color: green; }
        .info { color: blue; }
    </style>
</head>
<body>
    <h1>TestMode Deaktivierung</h1>
    <p class="info">Öffne diese Seite in einem neuen Tab und der TestMode wird deaktiviert.</p>

    <script>
        // Deaktiviere TestMode
        localStorage.removeItem('testModeEnabled');

        // Zeige Erfolgsmeldung
        document.body.innerHTML += '<p class="success">✅ TestMode wurde deaktiviert!</p>';
        document.body.innerHTML += '<p>Sie können jetzt zur Admin-Anwendung zurückkehren.</p>';
        document.body.innerHTML += '<p><a href="http://localhost:5174">→ Zur Admin-Anwendung</a></p>';

        // Zeige aktuellen Status
        setTimeout(() => {
            const status = localStorage.getItem('testModeEnabled');
            console.log('TestMode Status:', status);
        }, 100);
    </script>
</body>
</html>
EOF

    log_success "TestMode wurde deaktiviert!"
    log_info "Öffnen Sie $PROJECT_ROOT/frontend/Admin/disable-testmode.html in Ihrem Browser"
}

show_status() {
    log_info "TestMode Status:"

    # Prüfe ob Dev Server läuft
    if curl -s http://localhost:5174 > /dev/null 2>&1; then
        log_success "Dev Server läuft auf http://localhost:5174"
    else
        log_warning "Dev Server läuft nicht - starten Sie mit: npm run dev"
    fi

    echo ""
    log_info "TestMode wird automatisch aktiviert wenn:"
    echo "  • URL Parameter ?testmode=true gesetzt ist"
    echo "  • localStorage.testModeEnabled = 'true' ist"
    echo "  • Anwendung im Development-Modus läuft"
    echo ""
    log_info "Debug Panel öffnen: Ctrl + Shift + T im Browser"
    log_info "Globale API: window.getTestMode(), window.enableTestMode(), window.disableTestMode()"
}

run_demo() {
    log_info "Starte TestMode Demo..."

    # Prüfe ob Dev Server läuft
    if ! curl -s http://localhost:5174 > /dev/null 2>&1; then
        log_error "Dev Server läuft nicht auf http://localhost:5174"
        log_info "Starten Sie zuerst: cd frontend/Admin && npm run dev"
        exit 1
    fi

    # Prüfe ob Node.js verfügbar ist
    if ! command -v node &> /dev/null; then
        log_error "Node.js ist nicht installiert"
        exit 1
    fi

    # Führe Demo aus
    cd "$PROJECT_ROOT/frontend/Admin"
    node testmode-demo.js
}

# Main script logic
case "${1:-help}" in
    "enable")
        enable_testmode
        ;;
    "disable")
        disable_testmode
        ;;
    "status")
        show_status
        ;;
    "demo")
        run_demo
        ;;
    "help"|*)
        show_help
        ;;
esac