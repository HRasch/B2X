#!/bin/bash
# rollback-refactor.sh
# Rolls back the directory restructuring if needed
# Usage: ./rollback-refactor.sh

set -e

echo "Rolling back directory restructuring..."
echo "⚠️  This will move files back to their original locations!"
echo ""

# Confirm before proceeding
read -p "Are you sure you want to rollback? This will undo all moves! (y/N): " -n 1 -r
echo
if [[ ! $REPLY =~ ^[Yy]$ ]]; then
    echo "Rollback cancelled."
    exit 0
fi

# Rollback source code moves
echo "Rolling back source code moves..."

# Move Backend back
if [ -d "src/Api" ] || [ -d "src/Domain" ] || [ -d "src/Gateway" ]; then
    mkdir -p Backend
    mv src/Api src/ 2>/dev/null || true
    mv src/BoundedContexts src/ 2>/dev/null || true
    mv src/CLI src/ 2>/dev/null || true
    mv src/Connectors src/ 2>/dev/null || true
    mv src/Domain src/ 2>/dev/null || true
    mv src/Gateway src/ 2>/dev/null || true
    mv src/services src/ 2>/dev/null || true
    mv src/shared src/ 2>/dev/null || true
    mv src/Tests src/ 2>/dev/null || true
    echo "  ✓ Moved Backend code back"
fi

# Move Frontend back
if [ -d "src/admin" ] || [ -d "src/store" ] || [ -d "src/management" ]; then
    mkdir -p Frontend
    mv src/admin src/Admin 2>/dev/null || true
    mv src/store src/Store 2>/dev/null || true
    mv src/management src/Management 2>/dev/null || true
    echo "  ✓ Moved Frontend code back"
fi

# Move other components back
if [ -d "src/IdsConnectAdapter" ]; then
    mv src/IdsConnectAdapter ./
    echo "  ✓ Moved IdsConnectAdapter back"
fi

if [ -d "src/erp-connector" ]; then
    mv src/erp-connector ./
    echo "  ✓ Moved erp-connector back"
fi

if [ -d "src/seeding" ]; then
    mv src/seeding B2X.Seeding.API 2>/dev/null || true
    echo "  ✓ Moved seeding API back"
fi

# Move tests back
if [ -d "tests/tests/tests/tests/AppHost.Tests" ]; then
    mv tests/tests/tests/tests/AppHost.Tests ./
    echo "  ✓ Moved tests/tests/tests/AppHost.Tests back"
fi

# Move data files back
if [ -f "data/mock-db.json" ]; then
    mv data/mock-db.json ./
    echo "  ✓ Moved mock-db.json back"
fi

if [ -f "data/mock-db 2.json" ]; then
    mv "data/mock-db 2.json" ./
    echo "  ✓ Moved mock-db 2.json back"
fi

# Move docs back
if [ -d "docs/project" ]; then
    mv docs/project/*.md ./ 2>/dev/null || true
    echo "  ✓ Moved documentation back"
fi

# Move config files back
if [ -f "config/docker-compose.yml" ]; then
    mv config/docker-compose.yml ./
    echo "  ✓ Moved docker-compose.yml back"
fi

# Clean up empty directories
echo "Cleaning up empty directories..."
rmdir src/* 2>/dev/null || true
rmdir src 2>/dev/null || true
rmdir tests/* 2>/dev/null || true
rmdir tests 2>/dev/null || true
rmdir docs/* 2>/dev/null || true
rmdir docs 2>/dev/null || true
rmdir build/* 2>/dev/null || true
rmdir build 2>/dev/null || true
rmdir config/* 2>/dev/null || true
rmdir config 2>/dev/null || true
rmdir data/* 2>/dev/null || true
rmdir data 2>/dev/null || true
rmdir archive/* 2>/dev/null || true
rmdir archive 2>/dev/null || true

# Restore backup files
echo "Restoring backup files..."
find . -name "*.bak" | while read -r backup; do
    original="${backup%.bak}"
    if [ -f "$backup" ]; then
        mv "$backup" "$original"
        echo "  ✓ Restored $original"
    fi
done

echo ""
echo "Rollback complete!"
echo ""
echo "The project structure has been restored to its pre-refactoring state."
echo "You can now:"
echo "  - Review what went wrong"
echo "  - Fix issues and try again"
echo "  - Or keep the current state if preferred"