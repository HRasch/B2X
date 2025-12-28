#!/bin/bash

# Issue Role Detector & Auto-Labeler
# Analysiert Issue-Beschreibung und weist automatisch Rollen zu

set -e

ISSUE_FILE="${1:-.github/issue_template.md}"
CONTEXTS_CONFIG="copilot-contexts.json"

# Farben
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m'

# Role Detection Keywords
declare -A ROLE_KEYWORDS=(
  ["backend"]="API|database|microservice|handler|service|entity|repository|EF Core|Wolverine|controller|endpoint|async|await"
  ["frontend"]="Vue|component|CSS|Tailwind|TypeScript|UI|UX|component|layout|store|pinia|responsive|accessibility|WCAG"
  ["security"]="encryption|audit|compliance|NIS2|GDPR|secret|vault|password|hash|JWT|authentication|authorization|certificate"
  ["devops"]="docker|kubernetes|infrastructure|terraform|CI/CD|deploy|monitoring|orchestration|container|ansible|cloud"
  ["qa"]="test|automation|junit|xunit|mock|fixture|scenario|acceptance|regression|coverage|bug|defect|test case"
)

# Compliance Keywords (für P0 Components)
declare -A COMPLIANCE_KEYWORDS=(
  ["p0-1-audit"]="audit|logging|immutable|tamper|trail|record"
  ["p0-2-encryption"]="encryption|AES|encrypt|decrypt|cipher|secret"
  ["p0-3-incident"]="incident|response|detection|alert|breach|security"
  ["p0-4-network"]="segmentation|network|firewall|security group|isolation"
  ["p0-5-keys"]="key management|vault|rotation|secret|KeyVault"
  ["p0-6-ecommerce"]="e-commerce|VAT|invoice|withdrawal|B2B|B2C|legal|price"
  ["p0-7-ai"]="AI Act|decision|bias|transparency|model|fraud detection|high-risk"
  ["p0-8-bitv"]="accessibility|WCAG|BITV|barrierefreiheit|keyboard|screen reader"
  ["p0-9-erechnung"]="ZUGFeRD|e-invoice|UBL|digital signature|XML"
)

print_header() {
  echo -e "${BLUE}╔════════════════════════════════════════╗${NC}"
  echo -e "${BLUE}║   Issue Role Detector & Auto-Labeler   ║${NC}"
  echo -e "${BLUE}╚════════════════════════════════════════╝${NC}"
  echo
}

# Zähle Keyword-Matches pro Rolle
detect_roles() {
  local content="$1"
  local content_lower=$(echo "$content" | tr '[:upper:]' '[:lower:]')
  
  declare -A role_scores
  
  # Initialisiere alle Rollen mit 0
  for role in backend frontend security devops qa; do
    role_scores[$role]=0
  done
  
  # Zähle Matches
  for role in "${!ROLE_KEYWORDS[@]}"; do
    IFS='|' read -ra keywords <<< "${ROLE_KEYWORDS[$role]}"
    for keyword in "${keywords[@]}"; do
      count=$(echo "$content_lower" | grep -io "$keyword" | wc -l)
      role_scores[$role]=$((role_scores[$role] + count))
    done
  done
  
  # Sortiere und gebe Rollen mit Score > 0 aus
  local detected_roles=()
  for role in "${!role_scores[@]}"; do
    if [ "${role_scores[$role]}" -gt 0 ]; then
      detected_roles+=("$role:${role_scores[$role]}")
    fi
  done
  
  # Sortiere by score (descending)
  IFS=$'\n' sorted=($(sort -t: -k2 -rn <<<"${detected_roles[*]}"))
  unset IFS
  
  # Gebe nur Rollennamen aus (top 3)
  for i in {0..2}; do
    if [ -n "${sorted[$i]}" ]; then
      echo "${sorted[$i]%:*}"
    fi
  done
}

# Erkenne Compliance Focus (P0 Components)
detect_compliance_focus() {
  local content="$1"
  local content_lower=$(echo "$content" | tr '[:upper:]' '[:lower:]')
  
  for focus in "${!COMPLIANCE_KEYWORDS[@]}"; do
    if echo "$content_lower" | grep -q "${COMPLIANCE_KEYWORDS[$focus]}"; then
      echo "$focus"
    fi
  done
}

# Hauptprogramm
main() {
  print_header
  
  # Lese Issue-Inhalt
  echo -e "${YELLOW}→ Analysiere Issue-Inhalt...${NC}"
  
  # Von stdin oder File
  if [ -t 0 ]; then
    # Interaktiv: Benutzer gibt Inhalt ein
    echo "Gib Issue-Beschreibung ein (Ctrl+D zum Beenden):"
    issue_content=$(cat)
  else
    # Aus Pipe/Datei
    issue_content=$(cat)
  fi
  
  if [ -z "$issue_content" ]; then
    echo -e "${RED}❌ Keine Eingabe${NC}"
    exit 1
  fi
  
  # Erkenne Rollen
  echo
  echo -e "${YELLOW}→ Erkenne benötigte Rollen...${NC}"
  roles=($(detect_roles "$issue_content"))
  
  if [ ${#roles[@]} -eq 0 ]; then
    echo -e "${YELLOW}⚠️  Keine Rollen erkannt - setze auf 'backend' als default${NC}"
    roles=("backend")
  fi
  
  echo
  echo -e "${GREEN}✅ Erkannte Rollen:${NC}"
  for role in "${roles[@]}"; do
    echo "   • $role"
  done
  
  # Erkenne Compliance Focus
  echo
  echo -e "${YELLOW}→ Erkenne Compliance-Focus...${NC}"
  compliance_focuses=($(detect_compliance_focus "$issue_content"))
  
  if [ ${#compliance_focuses[@]} -gt 0 ]; then
    echo -e "${GREEN}✅ Erkannte P0 Components:${NC}"
    for focus in "${compliance_focuses[@]}"; do
      echo "   • $focus"
    done
  else
    echo -e "${YELLOW}ℹ️  Kein Compliance-Focus erkannt${NC}"
  fi
  
  # Ausgabe für weitereVerarbeitung
  echo
  echo -e "${BLUE}→ Ausgabe für Skript-Verarbeitung:${NC}"
  
  # Rollen als komma-separierte Liste
  ROLES_STR=$(IFS=,; echo "${roles[*]}")
  echo "ROLES=$ROLES_STR"
  
  # Compliance Focus als komma-separierte Liste
  if [ ${#compliance_focuses[@]} -gt 0 ]; then
    FOCUS_STR=$(IFS=,; echo "${compliance_focuses[*]}")
    echo "FOCUS=$FOCUS_STR"
  else
    echo "FOCUS="
  fi
  
  # GitHub Label-Format
  echo
  echo -e "${BLUE}→ Vorgeschlagene GitHub Labels:${NC}"
  for role in "${roles[@]}"; do
    echo "role:$role"
  done
  for focus in "${compliance_focuses[@]}"; do
    echo "focus:$focus"
  done
}

# Starten
main
