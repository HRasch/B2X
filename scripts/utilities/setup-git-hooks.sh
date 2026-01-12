#!/bin/bash
# Setup script for Git hooks
# Configures pre-commit hooks for dependency management

set -e

echo "🔧 Setting up Git hooks for dependency management..."

# Colors for output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m'

print_status() {
    local color=$1
    local message=$2
    echo -e "${color}${message}${NC}"
}

# Create .githooks directory if it doesn't exist
mkdir -p .githooks

# Make pre-commit hook executable
if [ -f ".githooks/pre-commit" ]; then
    chmod +x .githooks/pre-commit
    print_status $GREEN "✅ Pre-commit hook is executable"
else
    print_status $YELLOW "⚠️  Pre-commit hook not found, run this script after creating the hook"
fi

# Configure git to use custom hooks path
git config core.hooksPath .githooks
print_status $GREEN "✅ Git configured to use .githooks directory"

# Test the hook
print_status $YELLOW "🧪 Testing pre-commit hook..."
if .githooks/pre-commit; then
    print_status $GREEN "✅ Pre-commit hook validation passed"
else
    print_status $YELLOW "⚠️  Pre-commit hook found violations (expected if projects need migration)"
fi

print_status $GREEN "🎉 Git hooks setup complete!"
print_status $YELLOW "   Hooks will run automatically on commit"
print_status $YELLOW "   To skip hooks: git commit --no-verify"</content>
<parameter name="filePath">c:\Users\Holge\repos\B2Connect\scripts\setup-git-hooks.sh