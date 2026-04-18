#!/usr/bin/env bash
# Comprehensive Test & Fix Script for B2X
# Runs all quality checks and fixes issues

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

log() { echo -e "${BLUE}[$(date +'%H:%M:%S')]${NC} $1"; }
success() { echo -e "${GREEN}✓${NC} $1"; }
error() { echo -e "${RED}✗${NC} $1" >&2; }
warn() { echo -e "${YELLOW}⚠${NC} $1"; }

ERRORS=0

# ============================================
# Phase 1: Backend Fixes
# ============================================
log "Phase 1: Backend Analysis & Fixes"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"

cd "$PROJECT_ROOT"

# 1.1: Fix StyleCop and Analyzer Warnings
log "Fixing code style and analyzer warnings..."
if dotnet format B2X.slnx --verify-no-changes; then
    success "Code formatting OK"
else
    warn "Applying automatic formatting..."
    dotnet format B2X.slnx
    success "Code formatted"
fi

# 1.2: Build Backend
log "Building backend..."
if dotnet build B2X.slnx --no-restore; then
    success "Backend build successful"
else
    error "Backend build failed"
    ERRORS=$((ERRORS + 1))
fi

# 1.3: Run Backend Tests
log "Running backend tests..."
if dotnet test B2X.slnx --no-build --verbosity minimal; then
    success "Backend tests passed"
else
    error "Backend tests failed"
    ERRORS=$((ERRORS + 1))
fi

# 1.4: Generate Coverage Report
log "Generating backend coverage report..."
dotnet test B2X.slnx --no-build --collect:"XPlat Code Coverage" --verbosity quiet
success "Coverage report generated"

# ============================================
# Phase 2: Frontend Store Fixes
# ============================================
log ""
log "Phase 2: Frontend Store Analysis & Fixes"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"

if [ -d "frontend/Store" ]; then
    cd "$PROJECT_ROOT/frontend/Store"
    
    # 2.1: Install Dependencies
    log "Installing dependencies..."
    npm ci --silent
    
    # 2.2: Fix ESLint Errors
    log "Fixing ESLint errors..."
    if npm run lint --silent; then
        success "No ESLint errors"
    else
        warn "Auto-fixing ESLint errors..."
        npm run lint -- --fix || true
        
        # Re-run to check remaining issues
        if npm run lint --silent; then
            success "All ESLint errors fixed"
        else
            warn "Some ESLint errors require manual intervention"
            npm run lint 2>&1 | head -50
        fi
    fi
    
    # 2.3: TypeScript Type Check
    log "Running TypeScript type check..."
    if npx vue-tsc --noEmit; then
        success "No TypeScript errors"
    else
        error "TypeScript errors found"
        ERRORS=$((ERRORS + 1))
    fi
    
    # 2.4: Run Unit Tests
    log "Running unit tests..."
    if npm run test --silent; then
        success "Unit tests passed"
    else
        error "Unit tests failed"
        ERRORS=$((ERRORS + 1))
    fi
    
    # 2.5: Build Application
    log "Building application..."
    if npm run build; then
        success "Build successful"
    else
        error "Build failed"
        ERRORS=$((ERRORS + 1))
    fi
fi

# ============================================
# Phase 3: Frontend Admin Fixes
# ============================================
log ""
log "Phase 3: Frontend Admin Analysis & Fixes"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"

if [ -d "frontend/Admin" ]; then
    cd "$PROJECT_ROOT/frontend/Admin"
    
    # 3.1: Install Dependencies
    log "Installing dependencies..."
    npm ci --silent
    
    # 3.2: Fix ESLint Errors
    log "Fixing ESLint errors..."
    npm run lint -- --fix || true
    
    # 3.3: TypeScript Type Check
    log "Running TypeScript type check..."
    if npx vue-tsc --noEmit; then
        success "No TypeScript errors"
    else
        error "TypeScript errors found"
        ERRORS=$((ERRORS + 1))
    fi
    
    # 3.4: Run Tests
    log "Running tests..."
    if npm run test --silent; then
        success "Tests passed"
    else
        error "Tests failed"
        ERRORS=$((ERRORS + 1))
    fi
fi

# ============================================
# Phase 4: Security & Compliance Checks
# ============================================
log ""
log "Phase 4: Security & Compliance Checks"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"

cd "$PROJECT_ROOT"

# 4.1: Dependency Vulnerability Scan
log "Scanning for vulnerable dependencies..."

# Backend
log "Checking .NET packages..."
if dotnet list package --vulnerable | grep -q "has the following vulnerable packages"; then
    error "Vulnerable .NET packages found"
    dotnet list package --vulnerable
    ERRORS=$((ERRORS + 1))
else
    success "No vulnerable .NET packages"
fi

# Frontend Store
if [ -d "frontend/Store" ]; then
    log "Checking Store npm packages..."
    cd "$PROJECT_ROOT/frontend/Store"
    if npm audit --audit-level=high 2>&1 | grep -q "found 0 vulnerabilities"; then
        success "No high-severity npm vulnerabilities in Store"
    else
        warn "Vulnerabilities found in Store - attempting fix..."
        npm audit fix || true
    fi
fi

# Frontend Admin
if [ -d "frontend/Admin" ]; then
    log "Checking Admin npm packages..."
    cd "$PROJECT_ROOT/frontend/Admin"
    if npm audit --audit-level=high 2>&1 | grep -q "found 0 vulnerabilities"; then
        success "No high-severity npm vulnerabilities in Admin"
    else
        warn "Vulnerabilities found in Admin - attempting fix..."
        npm audit fix || true
    fi
fi

# ============================================
# Phase 5: E2E Testing Setup
# ============================================
log ""
log "Phase 5: E2E Testing Setup & Validation"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"

cd "$PROJECT_ROOT"

# 5.1: Install Playwright
if [ -d "frontend/Store" ]; then
    cd "$PROJECT_ROOT/frontend/Store"
    log "Installing Playwright browsers..."
    npx playwright install --with-deps chromium
    success "Playwright installed"
fi

if [ -d "frontend/Admin" ]; then
    cd "$PROJECT_ROOT/frontend/Admin"
    npx playwright install --with-deps chromium
fi

# ============================================
# Phase 6: Generate Reports
# ============================================
log ""
log "Phase 6: Generating Quality Reports"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"

cd "$PROJECT_ROOT"

# Create reports directory
mkdir -p .ai/reports/$(date +%Y%m%d)
REPORT_DIR=".ai/reports/$(date +%Y%m%d)"

# Generate comprehensive report
cat > "$REPORT_DIR/quality-report.md" << EOF
# Quality Assurance Report
**Generated:** $(date)

## Backend Status
- Build: $(dotnet build B2X.slnx --no-restore > /dev/null 2>&1 && echo "✅ PASS" || echo "❌ FAIL")
- Tests: $(dotnet test B2X.slnx --no-build --verbosity quiet > /dev/null 2>&1 && echo "✅ PASS" || echo "❌ FAIL")
- Code Style: $(dotnet format --verify-no-changes > /dev/null 2>&1 && echo "✅ PASS" || echo "⚠️ WARNINGS")

## Frontend Store Status
EOF

if [ -d "frontend/Store" ]; then
    cd "$PROJECT_ROOT/frontend/Store"
    cat >> "$PROJECT_ROOT/$REPORT_DIR/quality-report.md" << EOF
- Build: $(npm run build > /dev/null 2>&1 && echo "✅ PASS" || echo "❌ FAIL")
- Lint: $(npm run lint --silent > /dev/null 2>&1 && echo "✅ PASS" || echo "⚠️ WARNINGS")
- Tests: $(npm run test --silent > /dev/null 2>&1 && echo "✅ PASS" || echo "❌ FAIL")
- TypeScript: $(npx vue-tsc --noEmit > /dev/null 2>&1 && echo "✅ PASS" || echo "❌ FAIL")
EOF
fi

cat >> "$PROJECT_ROOT/$REPORT_DIR/quality-report.md" << EOF

## Frontend Admin Status
EOF

if [ -d "frontend/Admin" ]; then
    cd "$PROJECT_ROOT/frontend/Admin"
    cat >> "$PROJECT_ROOT/$REPORT_DIR/quality-report.md" << EOF
- Build: $(npm run build > /dev/null 2>&1 && echo "✅ PASS" || echo "❌ FAIL")
- Lint: $(npm run lint --silent > /dev/null 2>&1 && echo "✅ PASS" || echo "⚠️ WARNINGS")
- Tests: $(npm run test --silent > /dev/null 2>&1 && echo "✅ PASS" || echo "❌ FAIL")
- TypeScript: $(npx vue-tsc --noEmit > /dev/null 2>&1 && echo "✅ PASS" || echo "❌ FAIL")
EOF
fi

cat >> "$PROJECT_ROOT/$REPORT_DIR/quality-report.md" << EOF

## Security Status
- Vulnerable Packages: $(cd "$PROJECT_ROOT" && dotnet list package --vulnerable | grep -q "has the following vulnerable packages" && echo "❌ FOUND" || echo "✅ NONE")
- npm Audit: Checked

## Next Steps
EOF

cd "$PROJECT_ROOT"

if [ $ERRORS -gt 0 ]; then
    cat >> "$REPORT_DIR/quality-report.md" << EOF
⚠️ **$ERRORS critical issue(s) found** - requires attention before merge

### Action Items:
1. Review error logs above
2. Fix critical issues
3. Re-run this script
4. Update tests if needed
EOF
else
    cat >> "$REPORT_DIR/quality-report.md" << EOF
✅ **All quality gates passed** - ready for code review

### Pre-Merge Checklist:
- [ ] All tests passing
- [ ] Code coverage >80%
- [ ] No security vulnerabilities
- [ ] Documentation updated
- [ ] Commit messages follow convention
EOF
fi

# ============================================
# Final Summary
# ============================================
log ""
log "Quality Check Summary"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"

if [ $ERRORS -eq 0 ]; then
    success "All checks passed! ✅"
    log "Report saved to: $REPORT_DIR/quality-report.md"
    exit 0
else
    error "$ERRORS critical issue(s) found ❌"
    warn "Review the report: $REPORT_DIR/quality-report.md"
    warn "Fix issues and re-run this script"
    exit 1
fi