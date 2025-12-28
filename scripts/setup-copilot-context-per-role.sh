#!/bin/bash

##############################################################################
# Setup Copilot Context per Role
# Automatisiert Kontext-Umschaltung basierend auf Branch oder Rolle
# Usage: ./setup-copilot-context-per-role.sh
##############################################################################

set -e

WORKSPACE="/Users/holger/Documents/Projekte/B2Connect"
COLORS_GREEN='\033[0;32m'
COLORS_BLUE='\033[0;34m'
COLORS_YELLOW='\033[1;33m'
COLORS_RED='\033[0;31m'
COLORS_NC='\033[0m'

##############################################################################
# 1. Prüfe GitHub CLI Installation
##############################################################################
check_gh_cli() {
    if ! command -v gh &> /dev/null; then
        echo -e "${COLORS_RED}❌ GitHub CLI nicht gefunden${COLORS_NC}"
        exit 1
    fi
    echo -e "${COLORS_GREEN}✅ GitHub CLI gefunden${COLORS_NC}"
}

##############################################################################
# 2. Erstelle Role-basierte Issue Labels
##############################################################################
create_issue_labels() {
    echo -e "${COLORS_BLUE}Creating role-based issue labels...${COLORS_NC}"
    
    # Backend Labels
    gh label create "backend" --description "Backend implementation" --color "2CA02C" || true
    gh label create "api" --description "API endpoints" --color "1F77B4" || true
    gh label create "database" --description "Database related" --color "D62728" || true
    
    # Frontend Labels
    gh label create "frontend" --description "Frontend implementation" --color "FF7F0E" || true
    gh label create "ui" --description "UI/UX" --color "E377C2" || true
    gh label create "accessibility" --description "WCAG/BITV compliance" --color "7F7F7F" || true
    
    # Security Labels
    gh label create "security" --description "Security & compliance" --color "C7254E" || true
    gh label create "encryption" --description "Encryption related" --color "BD3039" || true
    gh label create "audit" --description "Audit logging" --color "D32F2F" || true
    gh label create "compliance" --description "Regulatory compliance" --color "E64A19" || true
    
    # DevOps Labels
    gh label create "infrastructure" --description "Infrastructure & deployment" --color "6F42C1" || true
    gh label create "ci-cd" --description "CI/CD pipeline" --color "5E35B1" || true
    gh label create "devops" --description "DevOps tasks" --color "512DA8" || true
    
    # QA/Testing Labels
    gh label create "testing" --description "Testing & QA" --color "00BCD4" || true
    gh label create "test" --description "Test code" --color "0097A7" || true
    gh label create "qa" --description "Quality assurance" --color "00838F" || true
    
    # P0 Component Labels
    gh label create "P0.1" --description "P0.1: Audit Logging" --color "FF5733" || true
    gh label create "P0.2" --description "P0.2: Encryption at Rest" --color "FF5733" || true
    gh label create "P0.3" --description "P0.3: Incident Response" --color "FF5733" || true
    gh label create "P0.4" --description "P0.4: Network Segmentation" --color "FF5733" || true
    gh label create "P0.5" --description "P0.5: Key Management" --color "FF5733" || true
    gh label create "P0.6" --description "P0.6: E-Commerce Legal" --color "FF5733" || true
    gh label create "P0.7" --description "P0.7: AI Act Compliance" --color "FF5733" || true
    gh label create "P0.8" --description "P0.8: BITV Accessibility" --color "FF5733" || true
    gh label create "P0.9" --description "P0.9: E-Rechnung" --color "FF5733" || true
    
    echo -e "${COLORS_GREEN}✅ Labels created${COLORS_NC}"
}

##############################################################################
# 3. Konfiguriere Git Hooks für automatisches Context-Switching
##############################################################################
setup_git_hooks() {
    echo -e "${COLORS_BLUE}Setting up git hooks...${COLORS_NC}"
    
    mkdir -p "$WORKSPACE/.git/hooks"
    
    # post-checkout hook: Wechsele Role basierend auf Branch-Name
    cat > "$WORKSPACE/.git/hooks/post-checkout" << 'EOF'
#!/bin/bash

# Automatisches Context-Switching basierend auf Branch-Name
BRANCH=$(git rev-parse --abbrev-ref HEAD)

# Erkenne Rolle aus Branch-Name
if [[ $BRANCH == feature/backend* ]] || [[ $BRANCH == fix/backend* ]]; then
    ROLE="backend"
elif [[ $BRANCH == feature/frontend* ]] || [[ $BRANCH == fix/frontend* ]]; then
    ROLE="frontend"
elif [[ $BRANCH == feature/security* ]] || [[ $BRANCH == fix/security* ]]; then
    ROLE="security"
elif [[ $BRANCH == feature/devops* ]] || [[ $BRANCH == fix/devops* ]]; then
    ROLE="devops"
elif [[ $BRANCH == feature/test* ]] || [[ $BRANCH == fix/test* ]]; then
    ROLE="qa"
elif [[ $BRANCH == chore/docs* ]]; then
    ROLE="product"
else
    ROLE="backend"  # Default to backend
fi

# Optional: Trigger context switch
# echo "🔄 Switching copilot context to: $ROLE"
# cd "$(git rev-parse --show-toplevel)"
# ./scripts/role-based-issue-filter.sh "$ROLE"

exit 0
EOF
    
    chmod +x "$WORKSPACE/.git/hooks/post-checkout"
    echo -e "${COLORS_GREEN}✅ Git hooks configured${COLORS_NC}"
}

##############################################################################
# 4. Erstelle .vscode/settings.json für Copilot-Kontext
##############################################################################
setup_vscode_settings() {
    echo -e "${COLORS_BLUE}Setting up VS Code settings...${COLORS_NC}"
    
    mkdir -p "$WORKSPACE/.vscode"
    
    # Nur Datei updaten, wenn sie existiert und keine korrekte Struktur hat
    if [ ! -f "$WORKSPACE/.vscode/settings.json" ]; then
        cat > "$WORKSPACE/.vscode/settings.json" << 'EOF'
{
  "copilot.context": {
    "enable": true,
    "scope": "role-based",
    "roles": {
      "backend": {
        "maxTokens": 3500,
        "searchPaths": ["backend/Domain/**", "backend/Orchestration/**"],
        "excludePaths": ["frontend/**", "node_modules/**"]
      },
      "frontend": {
        "maxTokens": 2500,
        "searchPaths": ["frontend-store/**", "frontend-admin/**", "Frontend/**"],
        "excludePaths": ["backend/**", "node_modules/**"]
      },
      "security": {
        "maxTokens": 2000,
        "searchPaths": ["backend/Domain/Identity/**", "docs/compliance/**"],
        "excludePaths": ["frontend/**"]
      },
      "devops": {
        "maxTokens": 2000,
        "searchPaths": ["backend/Orchestration/**", "backend/infrastructure/**"],
        "excludePaths": ["frontend/**", "backend/Domain/**"]
      },
      "qa": {
        "maxTokens": 2500,
        "searchPaths": ["**/tests/**", "docs/compliance/**"],
        "excludePaths": ["**/src/**", "node_modules/**"]
      }
    }
  },
  "github.copilot.enable": {
    "*": true,
    "plaintext": false,
    "markdown": true
  }
}
EOF
    fi
    echo -e "${COLORS_GREEN}✅ VS Code settings created${COLORS_NC}"
}

##############################################################################
# 5. Erstelle Role-basierte Kontext-Vorlage
##############################################################################
create_role_templates() {
    echo -e "${COLORS_BLUE}Creating role-based context templates...${COLORS_NC}"
    
    mkdir -p "$WORKSPACE/.github/copilot-contexts"
    
    # Backend Context Template
    cat > "$WORKSPACE/.github/copilot-contexts/backend-context.md" << 'EOF'
# 💻 Backend Developer Context

## Focus Areas
- C#/.NET microservices
- API endpoints (Wolverine HTTP)
- Database persistence (EF Core)
- Business logic & validation

## P0 Components
- P0.1: Audit Logging
- P0.6: E-Commerce Legal (VAT, Invoices, Returns)
- P0.7: AI Act Compliance
- P0.9: E-Rechnung (ZUGFeRD)

## Key Files to Review
- `backend/Domain/Identity/` - Authentication handlers
- `backend/Domain/Catalog/` - Product management
- `docs/WOLVERINE_HTTP_ENDPOINTS.md` - Handler pattern
- `.github/copilot-instructions.md` § Wolverine Pattern

## Recent Issues
Run: `./scripts/role-based-issue-filter.sh backend`

## Token Budget: ~3,500 tokens
EOF

    # Frontend Context Template
    cat > "$WORKSPACE/.github/copilot-contexts/frontend-context.md" << 'EOF'
# 🎨 Frontend Developer Context

## Focus Areas
- Vue.js 3 components
- Tailwind CSS styling
- TypeScript/reactive state
- User experience & accessibility

## P0 Components
- P0.6: E-Commerce UI (Price display, checkout flow)
- P0.8: BITV Accessibility (WCAG 2.1 AA)

## Key Files to Review
- `frontend-store/src/components/` - Reusable components
- `frontend-admin/src/components/` - Admin UI
- `docs/FRONTEND_FEATURE_INTEGRATION_GUIDE.md`
- `docs/P0.8_BARRIEREFREIHEIT_BITV_TESTS.md`

## Recent Issues
Run: `./scripts/role-based-issue-filter.sh frontend`

## Token Budget: ~2,500 tokens
EOF

    # Security Context Template
    cat > "$WORKSPACE/.github/copilot-contexts/security-context.md" << 'EOF'
# 🔐 Security Engineer Context

## Focus Areas
- Encryption (AES-256)
- Audit logging
- Compliance (NIS2, GDPR, AI Act)
- Key management

## P0 Components
- P0.1: Audit Logging (immutable trail)
- P0.2: Encryption at Rest
- P0.3: Incident Response (< 24h notification)
- P0.5: Key Management (KeyVault)
- P0.7: AI Act Risk Assessment

## Key Files to Review
- `.github/copilot-instructions.md` § Security Checklist
- `docs/EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md`
- `docs/compliance/P0.7_AI_ACT_TESTS.md`

## Recent Issues
Run: `./scripts/role-based-issue-filter.sh security`

## Token Budget: ~2,000 tokens
EOF

    # DevOps Context Template
    cat > "$WORKSPACE/.github/copilot-contexts/devops-context.md" << 'EOF'
# ⚙️ DevOps Engineer Context

## Focus Areas
- Infrastructure (VPC, security groups)
- CI/CD pipelines (GitHub Actions)
- Orchestration (Aspire, K8s)
- Monitoring & logging

## P0 Components
- P0.3: Incident Response Infrastructure
- P0.4: Network Segmentation
- P0.5: Key Management (KeyVault)

## Key Files to Review
- `backend/Orchestration/` - Aspire config
- `.github/workflows/` - CI/CD pipelines
- `docker-compose.yml` - Local development
- `docs/architecture/ASPIRE_GUIDE.md`

## Recent Issues
Run: `./scripts/role-based-issue-filter.sh devops`

## Token Budget: ~2,000 tokens
EOF

    # QA Context Template
    cat > "$WORKSPACE/.github/copilot-contexts/qa-context.md" << 'EOF'
# 🧪 QA Engineer Context

## Focus Areas
- Test automation (xUnit, Playwright)
- Compliance testing (52 P0 tests)
- Performance & load testing
- Bug tracking & regression

## P0 Components
- P0.6: E-Commerce Legal Tests (15 tests)
- P0.7: AI Act Tests (15 tests)
- P0.8: BITV Accessibility Tests (12 tests)
- P0.9: E-Rechnung Tests (10 tests)

## Key Files to Review
- `docs/compliance/P0.6_ECOMMERCE_LEGAL_TESTS.md`
- `docs/compliance/P0.7_AI_ACT_TESTS.md`
- `docs/TESTING_FRAMEWORK_GUIDE.md`
- `backend/Domain/**/tests/` - Test patterns

## Recent Issues
Run: `./scripts/role-based-issue-filter.sh qa`

## Token Budget: ~2,500 tokens
EOF

    echo -e "${COLORS_GREEN}✅ Context templates created in .github/copilot-contexts/${COLORS_NC}"
}

##############################################################################
# 6. Mache Skripte ausführbar
##############################################################################
make_executable() {
    chmod +x "$WORKSPACE/scripts/role-based-issue-filter.sh"
    chmod +x "$WORKSPACE/.git/hooks/post-checkout"
    echo -e "${COLORS_GREEN}✅ Scripts are executable${COLORS_NC}"
}

##############################################################################
# MAIN
##############################################################################

main() {
    echo -e "${COLORS_BLUE}=== Setting up Role-Based Copilot Context ===${COLORS_NC}"
    echo ""
    
    check_gh_cli
    create_issue_labels
    setup_git_hooks
    setup_vscode_settings
    create_role_templates
    make_executable
    
    echo ""
    echo -e "${COLORS_GREEN}=== Setup Complete! ===${COLORS_NC}"
    echo ""
    echo -e "${COLORS_YELLOW}Next steps:${COLORS_NC}"
    echo "1. Assign your role: export COPILOT_ROLE=backend"
    echo "2. Load role context: ./scripts/role-based-issue-filter.sh backend"
    echo "3. View all issues per role: ./scripts/role-based-issue-filter.sh all"
    echo ""
    echo -e "${COLORS_YELLOW}Branch naming convention:${COLORS_NC}"
    echo "  Backend:  feature/backend-xxx, fix/backend-xxx"
    echo "  Frontend: feature/frontend-xxx, fix/frontend-xxx"
    echo "  Security: feature/security-xxx, fix/security-xxx"
    echo "  DevOps:   feature/devops-xxx, fix/devops-xxx"
    echo "  Testing:  feature/test-xxx, fix/test-xxx"
    echo "  Docs:     chore/docs-xxx"
}

main "$@"
