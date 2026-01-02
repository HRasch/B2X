#!/bin/bash
# PR Quality Gate - Local Pre-flight Check
# Run this before creating a PR to catch issues early

set -e

echo "🚀 Running PR Quality Pre-flight Checks..."
echo ""

BACKEND_DIR="/Users/holger/Documents/Projekte/B2Connect"
FRONTEND_STORE="${BACKEND_DIR}/frontend/Store"
FRONTEND_MGMT="${BACKEND_DIR}/frontend/Management"

ERRORS=0

# ============================================
# Stage 1: Fast Checks
# ============================================
echo "📋 Stage 1: Fast Checks (Lint, Format, Type)"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"

# Backend format check
echo "  → Checking .NET format..."
if dotnet format --verify-no-changes --verbosity quiet; then
    echo "    ✅ Backend format OK"
else
    echo "    ❌ Backend format issues found. Run: dotnet format"
    ERRORS=$((ERRORS + 1))
fi

# Frontend lint
if [ -d "$FRONTEND_STORE" ]; then
    echo "  → Checking Store lint..."
    cd "$FRONTEND_STORE"
    if npm run lint --silent; then
        echo "    ✅ Store lint OK"
    else
        echo "    ❌ Store lint issues. Run: npm run lint:fix"
        ERRORS=$((ERRORS + 1))
    fi
    cd "$BACKEND_DIR"
fi

if [ -d "$FRONTEND_MGMT" ]; then
    echo "  → Checking Management lint..."
    cd "$FRONTEND_MGMT"
    if npm run lint --silent; then
        echo "    ✅ Management lint OK"
    else
        echo "    ❌ Management lint issues. Run: npm run lint:fix"
        ERRORS=$((ERRORS + 1))
    fi
    cd "$BACKEND_DIR"
fi

# TypeScript check
if [ -d "$FRONTEND_STORE" ]; then
    echo "  → Checking Store types..."
    cd "$FRONTEND_STORE"
    if npm run type-check --silent; then
        echo "    ✅ Store types OK"
    else
        echo "    ❌ Store TypeScript errors found"
        ERRORS=$((ERRORS + 1))
    fi
    cd "$BACKEND_DIR"
fi

# ============================================
# Stage 2: Tests
# ============================================
echo ""
echo "🧪 Stage 2: Tests + Coverage"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"

# Backend tests
echo "  → Running backend tests..."
if dotnet test B2Connect.slnx --verbosity quiet --no-build; then
    echo "    ✅ Backend tests passed"
else
    echo "    ❌ Backend tests failed"
    ERRORS=$((ERRORS + 1))
fi

# Frontend tests
if [ -d "$FRONTEND_STORE" ]; then
    echo "  → Running Store tests..."
    cd "$FRONTEND_STORE"
    if npm run test --silent; then
        echo "    ✅ Store tests passed"
    else
        echo "    ❌ Store tests failed"
        ERRORS=$((ERRORS + 1))
    fi
    cd "$BACKEND_DIR"
fi

# ============================================
# Stage 3: Security
# ============================================
echo ""
echo "🔐 Stage 3: Security Checks"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"

# Check for secrets (simple grep)
echo "  → Scanning for secrets..."
if git grep -i -E "(api[_-]?key|password|secret|token)\s*=\s*['\"][^'\"]+['\"]" -- '*.cs' '*.ts' '*.js' '*.json' 2>/dev/null; then
    echo "    ⚠️  Possible secrets detected! Review above matches."
    ERRORS=$((ERRORS + 1))
else
    echo "    ✅ No obvious secrets found"
fi

# Check dependencies
echo "  → Checking backend dependencies..."
if dotnet list package --vulnerable --include-transitive 2>&1 | grep -q "has the following vulnerable packages"; then
    echo "    ❌ Vulnerable backend packages found"
    dotnet list package --vulnerable --include-transitive
    ERRORS=$((ERRORS + 1))
else
    echo "    ✅ No vulnerable backend packages"
fi

# ============================================
# Summary
# ============================================
echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
if [ $ERRORS -eq 0 ]; then
    echo "✅ All pre-flight checks passed!"
    echo ""
    echo "You're ready to create a PR. CI will run additional checks:"
    echo "  - Coverage validation (≥80% backend, ≥70% frontend)"
    echo "  - Integration tests"
    echo "  - E2E tests"
    echo "  - Mega-Linter (50+ linters)"
    echo "  - GitHub CodeQL (security)"
    echo ""
    exit 0
else
    echo "❌ $ERRORS check(s) failed"
    echo ""
    echo "Fix the issues above before creating a PR."
    echo "CI will fail with these issues."
    echo ""
    exit 1
fi
