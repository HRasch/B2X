#!/bin/bash

# B2Connect Issue-Driven Copilot Context Setup
# One-time setup script

set -e

# Farben
RED='\033[0;31m'
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m'

REPO_ROOT=$(pwd)

print_header() {
  clear
  echo -e "${BLUE}╔═══════════════════════════════════════════════════╗${NC}"
  echo -e "${BLUE}║  B2Connect Copilot Context Setup                  ║${NC}"
  echo -e "${BLUE}╚═══════════════════════════════════════════════════╝${NC}"
  echo
}

check_prerequisites() {
  echo -e "${YELLOW}→ Prüfe Voraussetzungen...${NC}"
  echo
  
  # Git
  if ! command -v git &> /dev/null; then
    echo -e "${RED}❌ Git nicht installiert${NC}"
    exit 1
  fi
  echo -e "${GREEN}✓ Git${NC}"
  
  # GitHub CLI
  if ! command -v gh &> /dev/null; then
    echo -e "${YELLOW}⚠️  GitHub CLI nicht installiert${NC}"
    echo -e "${YELLOW}   Installiere: brew install gh${NC}"
    read -p "   Fortfahren ohne GitHub CLI? (y/n) " -n 1 -r
    echo
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
      exit 1
    fi
  else
    echo -e "${GREEN}✓ GitHub CLI${NC}"
  fi
  
  # jq (für JSON-Parsing)
  if ! command -v jq &> /dev/null; then
    echo -e "${YELLOW}⚠️  jq nicht installiert${NC}"
    echo -e "${YELLOW}   Installiere: brew install jq${NC}"
    read -p "   Fortfahren ohne jq? (y/n) " -n 1 -r
    echo
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
      exit 1
    fi
  else
    echo -e "${GREEN}✓ jq${NC}"
  fi
  
  echo
}

setup_git_hooks() {
  echo -e "${YELLOW}→ Richte Git Hooks ein...${NC}"
  
  # Prüfe ob Hook existiert
  if [ -f "$REPO_ROOT/.git/hooks/post-checkout" ]; then
    echo -e "${GREEN}✓ Post-checkout Hook existiert${NC}"
    chmod +x "$REPO_ROOT/.git/hooks/post-checkout"
    echo -e "${GREEN}✓ Hook ist ausführbar${NC}"
  else
    echo -e "${RED}❌ Post-checkout Hook nicht gefunden${NC}"
    echo -e "${YELLOW}   Erstelle: $REPO_ROOT/.git/hooks/post-checkout${NC}"
    exit 1
  fi
  
  echo
}

verify_config_files() {
  echo -e "${YELLOW}→ Verifiziere Konfigurationsdateien...${NC}"
  
  files=(
    ".github/ISSUE_TEMPLATE/smart-issue.yml"
    ".github/workflows/auto-label-issues.yml"
    "scripts/analyze-issue-roles.sh"
    "copilot-contexts.json"
  )
  
  for file in "${files[@]}"; do
    if [ -f "$REPO_ROOT/$file" ]; then
      echo -e "${GREEN}✓ $file${NC}"
    else
      echo -e "${RED}❌ $file nicht gefunden${NC}"
      exit 1
    fi
  done
  
  echo
}

test_analyzer() {
  echo -e "${YELLOW}→ Teste Issue Role Analyzer...${NC}"
  
  test_issue="Implementiere AES-256 Verschlüsselung für Benutzerdaten. Backend Encryption mit EF Core Value Converters. Tests für Round-trip."
  
  output=$(echo "$test_issue" | "$REPO_ROOT/scripts/analyze-issue-roles.sh" 2>&1)
  
  if echo "$output" | grep -q "ROLES="; then
    echo -e "${GREEN}✓ Role Detection funktioniert${NC}"
    echo "$output" | grep "ROLES="
  else
    echo -e "${RED}❌ Role Detection fehlerhat${NC}"
    exit 1
  fi
  
  echo
}

create_backups() {
  echo -e "${YELLOW}→ Erstelle Backups...${NC}"
  
  mkdir -p "$REPO_ROOT/.vscode/backups"
  
  if [ -f "$REPO_ROOT/.vscode/settings.json" ]; then
    cp "$REPO_ROOT/.vscode/settings.json" "$REPO_ROOT/.vscode/backups/settings.json.$(date +%s).bak"
    echo -e "${GREEN}✓ Alte settings.json gesichert${NC}"
  fi
  
  echo
}

print_final_steps() {
  echo -e "${BLUE}╔═══════════════════════════════════════════════════╗${NC}"
  echo -e "${BLUE}║  ✅ SETUP ABGESCHLOSSEN                           ║${NC}"
  echo -e "${BLUE}╚═══════════════════════════════════════════════════╝${NC}"
  echo
  
  echo -e "${GREEN}Nächste Schritte:${NC}"
  echo
  echo "1. ${YELLOW}Teste den Analyzer manuell:${NC}"
  echo "   echo 'API endpoint mit Wolverine Backend service' | ./scripts/analyze-issue-roles.sh"
  echo
  echo "2. ${YELLOW}Erstelle eine Test-Issue:${NC}"
  echo "   - Gehe zu GitHub Issues"
  echo "   - Klick 'New Issue'"
  echo "   - Wähle 'Smart Issue with Role Detection' Template"
  echo "   - Beschreibe mit Keywords (z.B. 'API', 'Backend', 'encryption')"
  echo
  echo "3. ${YELLOW}Checkout Test-Branch:${NC}"
  echo "   git checkout -b feature/issue-XXX-test"
  echo
  echo "4. ${YELLOW}Verifiziere Git Hook läuft:${NC}"
  echo "   - Output sollte Rollen anzeigen"
  echo "   - .vscode/settings.json sollte generiert sein"
  echo
  echo "5. ${YELLOW}Lade VS Code neu:${NC}"
  echo "   Cmd+Shift+P → 'Developer: Reload Window'"
  echo "   Cmd+Shift+P → 'Copilot: Rebuild Index'"
  echo
  echo -e "${GREEN}✅ Copilot wird jetzt automatisch optimiert!${NC}"
  echo
}

main() {
  print_header
  
  check_prerequisites
  setup_git_hooks
  verify_config_files
  test_analyzer
  create_backups
  
  print_final_steps
}

main
