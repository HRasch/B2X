#!/bin/bash
# move-files-phase2.sh
# Phase 2: Move source code (Backend, Frontend, Tests)
# Usage: ./move-files-phase2.sh

set -e

echo "Phase 2: Moving source code..."
echo "⚠️  This phase moves critical source files. Make sure you have committed Phase 1!"
echo ""

# Confirm before proceeding
read -p "Have you committed Phase 1 changes? (y/N): " -n 1 -r
echo
if [[ ! $REPLY =~ ^[Yy]$ ]]; then
    echo "Please commit Phase 1 changes first, then run this script again."
    exit 1
fi

# Move Backend code
echo "Moving Backend code..."
if [ -d "src/" ]; then
    mv src/* src/ 2>/dev/null || true
    # Move specific subdirectories
    for subdir in Api BoundedContexts CLI Connectors Domain Gateway services shared Tests; do
        if [ -d "src/$subdir" ]; then
            mv "src/$subdir" "src/"
            echo "  ✓ src/$subdir → src/"
        fi
    done
    # Remove empty Backend directory
    rmdir Backend 2>/dev/null || echo "  ⚠️  Backend directory not empty, keeping it"
else
    echo "  ⚠️  Backend directory not found"
fi

# Move Frontend code
echo "Moving Frontend code..."
if [ -d "src/" ]; then
    # Move Admin frontend
    if [ -d "src/Admin" ]; then
        mkdir -p src/admin
        mv src/Admin/* src/admin/ 2>/dev/null || true
        echo "  ✓ src/Admin → src/admin/"
    fi

    # Move Store frontend
    if [ -d "src/Store" ]; then
        mkdir -p src/store
        mv src/Store/* src/store/ 2>/dev/null || true
        echo "  ✓ src/Store → src/store/"
    fi

    # Move Management frontend
    if [ -d "src/Management" ]; then
        mkdir -p src/management
        mv src/Management/* src/management/ 2>/dev/null || true
        echo "  ✓ src/Management → src/management/"
    fi

    # Remove empty Frontend directory
    rmdir Frontend 2>/dev/null || echo "  ⚠️  Frontend directory not empty, keeping it"
else
    echo "  ⚠️  Frontend directory not found"
fi

# Move test projects
echo "Moving test projects..."
if [ -d "tests/tests/tests/AppHost.Tests/" ]; then
    mv tests/tests/tests/AppHost.Tests tests/
    echo "  ✓ tests/tests/tests/AppHost.Tests → tests/"
fi

# Move B2X.Seeding.API
echo "Moving seeding API..."
if [ -d "B2X.Seeding.API/" ]; then
    mkdir -p src/seeding
    mv B2X.Seeding.API/* src/seeding/ 2>/dev/null || true
    echo "  ✓ B2X.Seeding.API → src/seeding/"
fi

# Move IdsConnectAdapter
echo "Moving IdsConnectAdapter..."
if [ -d "IdsConnectAdapter/" ]; then
    mv IdsConnectAdapter src/
    echo "  ✓ IdsConnectAdapter → src/"
fi

# Move erp-connector
echo "Moving erp-connector..."
if [ -d "erp-connector/" ]; then
    mv erp-connector src/
    echo "  ✓ erp-connector → src/"
fi

echo ""
echo "Phase 2 complete! Source code moved to src/"
echo ""
echo "Next steps:"
echo "  1. Run validate-moves.sh to check all moves"
echo "  2. Run update-references.sh to fix import paths"
echo "  3. Test builds with test-builds.sh"