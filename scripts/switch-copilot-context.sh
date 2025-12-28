#!/bin/bash

# Copilot Context Switcher für B2Connect
# Automatischer Wechsel zwischen Backend/Frontend Kontexten
# Verwendet: ./scripts/switch-copilot-context.sh [backend|frontend]

set -e

ROLE="${1:-backend}"
VSCODE_DIR=".vscode"
SETTINGS_FILE="$VSCODE_DIR/settings.json"
BACKUP_DIR="$VSCODE_DIR/backups"
TIMESTAMP=$(date +%Y%m%d_%H%M%S)

# Farben für Output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Funktionen
print_header() {
  echo -e "${BLUE}╔════════════════════════════════════════╗${NC}"
  echo -e "${BLUE}║ Copilot Context Switcher for B2Connect ║${NC}"
  echo -e "${BLUE}╚════════════════════════════════════════╝${NC}"
  echo
}

print_success() {
  echo -e "${GREEN}✅ $1${NC}"
}

print_error() {
  echo -e "${RED}❌ $1${NC}"
}

print_info() {
  echo -e "${YELLOW}ℹ️  $1${NC}"
}

print_step() {
  echo -e "${BLUE}→ $1${NC}"
}

# Backup-Ordner erstellen
create_backup_dir() {
  if [ ! -d "$BACKUP_DIR" ]; then
    mkdir -p "$BACKUP_DIR"
    print_info "Backup-Ordner erstellt: $BACKUP_DIR"
  fi
}

# Settings.json sichern
backup_settings() {
  local backup_file="$BACKUP_DIR/settings_${TIMESTAMP}.json"
  cp "$SETTINGS_FILE" "$backup_file"
  print_info "Backup gespeichert: $backup_file"
}

# Zu Backend-Kontext wechseln
switch_to_backend() {
  print_step "Wechsel zu Backend-Developer Kontext..."
  
  if [ ! -f "$VSCODE_DIR/settings-backend.json" ]; then
    print_error "Datei nicht gefunden: $VSCODE_DIR/settings-backend.json"
    echo "Stelle sicher, dass beide Konfigurationsdateien existieren:"
    echo "  - .vscode/settings-backend.json"
    echo "  - .vscode/settings-frontend.json"
    exit 1
  fi
  
  backup_settings
  cp "$VSCODE_DIR/settings-backend.json" "$SETTINGS_FILE"
  print_success "Backend-Kontext ist jetzt aktiv"
  
  print_info "Ausgeschlossene Bereiche:"
  echo "  🚫 frontend-store/ (Vue.js Store)"
  echo "  🚫 frontend-admin/ (Vue.js Admin)"
  echo "  🚫 Frontend/ (Shared Frontend Components)"
  echo "  🚫 frontend/ (Root Frontend)"
  echo "  ✅ Backend-Fokus: /backend, /AppHost, /ServiceDefaults"
  
  echo
  print_step "Größe der analysierten Codebase: ~8,000 Dateien"
  
  rebuild_hint
}

# Zu Frontend-Kontext wechseln
switch_to_frontend() {
  print_step "Wechsel zu Frontend-Developer Kontext..."
  
  if [ ! -f "$VSCODE_DIR/settings-frontend.json" ]; then
    print_error "Datei nicht gefunden: $VSCODE_DIR/settings-frontend.json"
    echo "Stelle sicher, dass beide Konfigurationsdateien existieren:"
    echo "  - .vscode/settings-backend.json"
    echo "  - .vscode/settings-frontend.json"
    exit 1
  fi
  
  backup_settings
  cp "$VSCODE_DIR/settings-frontend.json" "$SETTINGS_FILE"
  print_success "Frontend-Kontext ist jetzt aktiv"
  
  print_info "Ausgeschlossene Bereiche:"
  echo "  🚫 backend/ (.NET Services)"
  echo "  🚫 AppHost/ (Orchestration)"
  echo "  🚫 ServiceDefaults/ (.NET Shared)"
  echo "  ✅ Frontend-Fokus: /frontend-store, /frontend-admin, /Frontend"
  
  echo
  print_step "Größe der analysierten Codebase: ~4,500 Dateien"
  
  rebuild_hint
}

# Rebuild-Hinweis
rebuild_hint() {
  echo
  print_step "🔧 KRITISCH: Copilot Index Rebuild erforderlich"
  echo
  echo "1️⃣  Öffne VS Code Command Palette:"
  echo "   Cmd+Shift+P"
  echo
  echo "2️⃣  Gib ein und wähle:"
  echo "   'Developer: Reload Window'"
  echo
  echo "3️⃣  Danach nochmal:"
  echo "   'Copilot: Rebuild Index'"
  echo
  echo "⏱️  Warten Sie 30-60 Sekunden bis der Index aktualisiert ist"
  echo
  print_info "Danach sollte Copilot 2-5x schneller sein!"
}

# Status anzeigen
show_status() {
  echo
  print_step "Aktueller Kontext-Status:"
  echo
  
  if grep -q "frontend-store/\*\*" "$SETTINGS_FILE" 2>/dev/null; then
    echo "   📍 Backend-Kontext ist aktiv"
    print_info "Copilot fokussiert auf .NET/C#-Dateien"
  elif grep -q "backend/\*\*" "$SETTINGS_FILE" 2>/dev/null; then
    echo "   📍 Frontend-Kontext ist aktiv"
    print_info "Copilot fokussiert auf TypeScript/Vue-Dateien"
  else
    echo "   ❌ Unbekannter oder Standard-Kontext"
    print_info "Könnte Original settings.json sein"
  fi
  echo
}

# Backup auflisten
list_backups() {
  echo
  print_step "Verfügbare Backups:"
  echo
  
  if [ ! -d "$BACKUP_DIR" ]; then
    print_info "Keine Backups vorhanden"
    return
  fi
  
  if [ "$(ls -A "$BACKUP_DIR")" ]; then
    ls -1 "$BACKUP_DIR" | nl
  else
    print_info "Backup-Ordner ist leer"
  fi
  echo
}

# Hilfe anzeigen
show_help() {
  print_header
  echo "Verwendung:"
  echo "  ./scripts/switch-copilot-context.sh [BEFEHL]"
  echo
  echo "Befehle:"
  echo "  backend      Wechsel zu Backend-Developer Kontext"
  echo "  frontend     Wechsel zu Frontend-Developer Kontext"
  echo "  status       Zeige aktuellen Kontext"
  echo "  backups      Liste verfügbare Backups"
  echo "  help         Zeige diese Hilfe"
  echo
  echo "Beispiele:"
  echo "  ./scripts/switch-copilot-context.sh backend"
  echo "  ./scripts/switch-copilot-context.sh frontend"
  echo "  ./scripts/switch-copilot-context.sh status"
  echo
}

# Hauptprogramm
main() {
  print_header
  
  create_backup_dir
  
  case "${ROLE,,}" in
    backend)
      switch_to_backend
      ;;
    frontend)
      switch_to_frontend
      ;;
    status)
      show_status
      list_backups
      ;;
    backups)
      list_backups
      ;;
    help|-h|--help)
      show_help
      ;;
    *)
      print_error "Unbekannter Befehl: $ROLE"
      echo
      show_help
      exit 1
      ;;
  esac
}

# Starte Hauptprogramm
main
