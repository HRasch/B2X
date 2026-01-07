#!/bin/bash
# B2Connect Git Hooks Installation Script
# Installs pre-commit hook for MCP token optimization

REPO_ROOT="$(git rev-parse --show-toplevel 2>/dev/null || pwd)"
HOOKS_DIR="$REPO_ROOT/.git/hooks"
SCRIPT_DIR="$REPO_ROOT/scripts"

echo "🔧 Installing B2Connect MCP Git Hooks..."
echo ""

# Create hooks directory if it doesn't exist
mkdir -p "$HOOKS_DIR"

# Install pre-commit hook
if [ -f "$SCRIPT_DIR/pre-commit" ]; then
    cp "$SCRIPT_DIR/pre-commit" "$HOOKS_DIR/pre-commit"
    chmod +x "$HOOKS_DIR/pre-commit"
    echo "✓ Pre-commit hook installed: $HOOKS_DIR/pre-commit"
else
    echo "✗ ERROR: $SCRIPT_DIR/pre-commit not found"
    exit 1
fi

# Make hook executable
chmod +x "$HOOKS_DIR/pre-commit"

echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "✅ Git Hooks Installation Complete"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""
echo "Installed hooks:"
echo "  • pre-commit: Validates MCP rate limits and caching before commits"
echo ""
echo "To test the hook, make a commit:"
echo "  git commit -m 'test: verify hooks are working'"
echo ""
echo "To disable hook temporarily (NOT RECOMMENDED):"
echo "  git commit --no-verify -m 'message'"
echo ""
