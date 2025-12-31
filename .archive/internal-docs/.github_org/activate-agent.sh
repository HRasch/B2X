#!/bin/bash
# activate-agent.sh - Switch Copilot agent context by role
# Usage: ./activate-agent.sh <role>

set -e

ROLE=$1
CONTEXT_DIR=".github/role-contexts"
GLOBAL_CONTEXT=".github/copilot-instructions.md"
GLOBAL_CONTEXT_BACKUP=".github/copilot-instructions.md.bak"

# Color codes for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

print_header() {
    echo -e "${BLUE}========================================${NC}"
    echo -e "${BLUE}$1${NC}"
    echo -e "${BLUE}========================================${NC}"
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

# Validate input
if [[ -z "$ROLE" ]]; then
    print_header "Copilot Agent Activation Tool"
    echo ""
    echo "Usage: ./activate-agent.sh <role>"
    echo ""
    echo "Available roles:"
    echo "  security    - 🔐 Security Engineer (P0.1-P0.5)"
    echo "  backend     - 💻 Backend Developer (Features + Wolverine)"
    echo "  qa          - 🧪 QA Engineer (52 Compliance Tests)"
    echo "  frontend    - 🎨 Frontend Developer (Vue.js + WCAG)"
    echo "  devops      - ⚙️ DevOps Engineer (Infrastructure)"
    echo "  product     - 📋 Product Owner (Prioritization)"
    echo "  legal       - ⚖️ Legal/Compliance (Regulations)"
    echo "  tech-lead   - 👔 Tech Lead (Architecture)"
    echo ""
    echo "Example: ./activate-agent.sh backend"
    echo ""
    exit 1
fi

# Map role to context file
case $ROLE in
    security)
        CONTEXT_FILE="$CONTEXT_DIR/security-engineer-context.md"
        ROLE_NAME="🔐 Security Engineer"
        ;;
    backend)
        CONTEXT_FILE="$CONTEXT_DIR/backend-developer-context.md"
        ROLE_NAME="💻 Backend Developer"
        ;;
    qa)
        CONTEXT_FILE="$CONTEXT_DIR/qa-engineer-context.md"
        ROLE_NAME="🧪 QA Engineer"
        ;;
    frontend)
        CONTEXT_FILE="$CONTEXT_DIR/frontend-developer-context.md"
        ROLE_NAME="🎨 Frontend Developer"
        ;;
    devops)
        CONTEXT_FILE="$CONTEXT_DIR/devops-engineer-context.md"
        ROLE_NAME="⚙️ DevOps Engineer"
        ;;
    product)
        CONTEXT_FILE="$CONTEXT_DIR/product-owner-context.md"
        ROLE_NAME="📋 Product Owner"
        ;;
    legal)
        CONTEXT_FILE="$CONTEXT_DIR/legal-compliance-context.md"
        ROLE_NAME="⚖️ Legal/Compliance"
        ;;
    tech-lead)
        CONTEXT_FILE="$CONTEXT_DIR/tech-lead-context.md"
        ROLE_NAME="👔 Tech Lead"
        ;;
    *)
        print_error "Unknown role: $ROLE"
        echo "Valid roles: security, backend, qa, frontend, devops, product, legal, tech-lead"
        exit 1
        ;;
esac

# Verify context file exists
if [[ ! -f "$CONTEXT_FILE" ]]; then
    print_error "Context file not found: $CONTEXT_FILE"
    exit 1
fi

# Backup existing context (if not already backed up)
if [[ ! -f "$GLOBAL_CONTEXT_BACKUP" && -f "$GLOBAL_CONTEXT" ]]; then
    cp "$GLOBAL_CONTEXT" "$GLOBAL_CONTEXT_BACKUP"
    print_info "Backed up original context to $GLOBAL_CONTEXT_BACKUP"
fi

# Copy role-specific context to global location
cp "$CONTEXT_FILE" "$GLOBAL_CONTEXT"

print_header "Copilot Agent Activated"
echo ""
print_success "Role: $ROLE_NAME"
print_success "Context: $CONTEXT_FILE"
echo ""
print_info "Reloading VS Code Copilot to apply new context..."
echo ""
echo "To apply changes in VS Code:"
echo "  1. Close and reopen Copilot Chat"
echo "  2. Or reload VS Code window (Cmd+Shift+P → Developer: Reload Window)"
echo ""
print_success "Agent context loaded!"
echo ""
