#!/bin/bash
# Project Cleanup Continuation Script
# Run this on a new machine to check cleanup status and next steps

echo "🔍 B2X Project Cleanup Status Check"
echo "===================================="

# Check if cleanup directory exists
if [ -d ".ai/issues/CLEANUP-001" ]; then
    echo "✅ Cleanup directory found"
    echo "📁 Files present:"
    ls -1 .ai/issues/CLEANUP-001/
else
    echo "❌ Cleanup directory missing - run '/project-cleanup' first"
    exit 1
fi

echo ""
echo "📦 Dependency Status:"
if command -v npm &> /dev/null; then
    if npm list js-yaml &>/dev/null; then
        echo "✅ js-yaml installed"
    else
        echo "❌ js-yaml missing - run: npm install js-yaml"
    fi

    echo "🔒 Security check:"
    if npm audit --audit-level moderate | grep -q "found 0 vulnerabilities"; then
        echo "✅ No security vulnerabilities"
    else
        echo "⚠️  Security issues found - run: npm audit fix"
    fi
else
    echo "❌ npm not available"
fi

echo ""
echo "🔧 Next Priority Actions:"
echo "1. Fix frontend workspaces: npm run install:all"
echo "2. Run tests: npm run test:backend"
echo "3. Check coverage: dotnet test with --collect:'XPlat Code Coverage'"
echo "4. Start refactoring: Review code-assessment.md"
echo "5. Update docs: Check README.md version badges"

echo ""
echo "📋 Quick Commands:"
echo "• Status check: ./scripts/cleanup-continuation.sh"
echo "• Full cleanup: npm run legacy-cleanup"
echo "• Format check: npm run check:all"
echo "• Test run: dotnet test B2X.slnx"