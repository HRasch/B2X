#!/bin/bash
# validate-moves.sh
# Validates that file moves were successful and structure is correct
# Usage: ./validate-moves.sh

set -e

echo "Validating file moves and directory structure..."

errors=0

# Check main directories exist
echo "Checking main directories..."
for dir in src tests docs build config data archive; do
    if [ ! -d "$dir" ]; then
        echo "  ❌ Missing directory: $dir"
        errors=$((errors + 1))
    else
        echo "  ✓ $dir exists"
    fi
done

# Check subdirectories
echo ""
echo "Checking subdirectories..."
subdirs=(
    "src/api"
    "src/services"
    "src/models"
    "src/components"
    "src/pages"
    "tests/unit"
    "tests/integration"
    "tests/e2e"
    "docs/developer"
    "docs/user"
    "docs/api"
    "build/artifacts"
    "config/ci"
    "data/migrations"
    "archive/old-code"
)

for subdir in "${subdirs[@]}"; do
    if [ ! -d "$subdir" ]; then
        echo "  ❌ Missing subdirectory: $subdir"
        errors=$((errors + 1))
    else
        echo "  ✓ $subdir exists"
    fi
done

# Check moved files
echo ""
echo "Checking moved files..."
moved_files=(
    "data/mock-db.json"
    "data/mock-db 2.json"
    "docs/project/PROJECT_DASHBOARD.md"
    "docs/project/QUICK_START_GUIDE.md"
    "config/docker-compose.yml"
)

for file in "${moved_files[@]}"; do
    if [ -f "$file" ]; then
        echo "  ✓ $file exists"
    else
        echo "  ⚠️  $file not found (might not have existed)"
    fi
done

# Check that old files are gone
echo ""
echo "Checking old files are moved..."
old_files=(
    "mock-db.json"
    "mock-db 2.json"
    "PROJECT_DASHBOARD.md"
    "QUICK_START_GUIDE.md"
)

for file in "${old_files[@]}"; do
    if [ -f "$file" ]; then
        echo "  ❌ Old file still exists: $file"
        errors=$((errors + 1))
    fi
done

echo ""
if [ $errors -eq 0 ]; then
    echo "✅ Validation passed! All directories and moves look good."
    echo ""
    echo "Next steps:"
    echo "  1. Commit these changes: git add -A && git commit -m 'refactor: create new directory structure'"
    echo "  2. Run move-files-phase2.sh for source code moves"
else
    echo "❌ Validation failed with $errors errors. Please check the issues above."
    exit 1
fi