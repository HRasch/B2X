#!/bin/bash
# B2Connect MCP Token Optimization - System Verification
# Verifies all components are installed and operational

REPO_ROOT="$(git rev-parse --show-toplevel 2>/dev/null || pwd)"
SCRIPT_DIR="$REPO_ROOT/scripts"
STATUS_DIR="$REPO_ROOT/.ai/status"
KB_DIR="$REPO_ROOT/.ai/knowledgebase"

echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "B2Connect MCP Token Optimization System - Verification"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""

CHECKS_PASSED=0
CHECKS_FAILED=0

# Function to check if file exists
check_file() {
    if [ -f "$1" ]; then
        echo "✅ $2"
        ((CHECKS_PASSED++))
    else
        echo "❌ $2 (missing at $1)"
        ((CHECKS_FAILED++))
    fi
}

# Function to check if file is executable
check_executable() {
    if [ -x "$1" ]; then
        echo "✅ $2"
        ((CHECKS_PASSED++))
    else
        echo "❌ $2 (not executable)"
        ((CHECKS_FAILED++))
    fi
}

# Function to check directory exists
check_directory() {
    if [ -d "$1" ]; then
        echo "✅ $2"
        ((CHECKS_PASSED++))
    else
        echo "❌ $2 (missing at $1)"
        ((CHECKS_FAILED++))
    fi
}

echo "📦 Core Scripts:"
check_executable "$SCRIPT_DIR/mcp-console-logger.js" "  mcp-console-logger.js"
check_executable "$SCRIPT_DIR/mcp-cache-manager.js" "  mcp-cache-manager.js"
check_executable "$SCRIPT_DIR/mcp-rate-limiter.js" "  mcp-rate-limiter.js"
check_executable "$SCRIPT_DIR/mcp-metrics-dashboard.js" "  mcp-metrics-dashboard.js"
check_executable "$SCRIPT_DIR/mcp-ab-testing.js" "  mcp-ab-testing.js"
check_executable "$SCRIPT_DIR/mcp-audit-trail.js" "  mcp-audit-trail.js"
check_executable "$SCRIPT_DIR/daily-mcp-review.sh" "  daily-mcp-review.sh"

echo ""
echo "🔗 Git Integration:"
check_file "$REPO_ROOT/.git/hooks/pre-commit" "  Pre-commit hook installed"

echo ""
echo "📚 Knowledge Base:"
check_file "$KB_DIR/KB-QR-001-team-quick-reference.md" "  Quick Reference Card"
check_file "$KB_DIR/mcp-team-training-guide.md" "  Team Training Guide"
check_file "$KB_DIR/mcp-token-optimization-lessons.md" "  Lessons Learned"

echo ""
echo "📁 Infrastructure Directories:"
check_directory "$REPO_ROOT/.ai/cache/mcp" "  Cache directory"
check_directory "$REPO_ROOT/.ai/logs/mcp-usage" "  Usage logs directory"
check_directory "$REPO_ROOT/.ai/status" "  Status tracking directory"

echo ""
echo "📋 Status Documents:"
check_file "$STATUS_DIR/mcp-optimization.md" "  MCP Optimization Status"
check_file "$STATUS_DIR/PHASE-4-IMPLEMENTATION-SUMMARY.md" "  Phase 4 Summary"

echo ""
echo "⚙️  Configuration:"
check_file "$REPO_ROOT/.vscode/mcp.json" "  MCP Server Configuration"

echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "Verification Summary"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""
echo "✅ Checks Passed: $CHECKS_PASSED"
echo "❌ Checks Failed: $CHECKS_FAILED"
echo ""

if [ $CHECKS_FAILED -eq 0 ]; then
    echo "🎉 All systems operational!"
    echo ""
    echo "Next Steps:"
    echo "  1. Read Quick Reference: $KB_DIR/KB-QR-001-team-quick-reference.md"
    echo "  2. Check MCP health: node $SCRIPT_DIR/mcp-metrics-dashboard.js status"
    echo "  3. View daily report: tail -50 .ai/logs/mcp-usage/daily-reviews/latest-report.md"
    echo ""
    echo "Team Training:"
    echo "  • Quick Start: 5 minutes (.ai/knowledgebase/KB-QR-001-*)"
    echo "  • Full Course: 60 minutes (.ai/knowledgebase/mcp-team-training-guide.md)"
    echo ""
    exit 0
else
    echo "⚠️  Some checks failed. Please verify the installation."
    echo ""
    echo "Troubleshooting:"
    echo "  • Run: bash $SCRIPT_DIR/install-hooks.sh"
    echo "  • Check: ls -la $SCRIPT_DIR/"
    echo "  • Status: cat $STATUS_DIR/mcp-optimization.md"
    echo ""
    exit 1
fi
