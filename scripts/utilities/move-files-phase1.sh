#!/bin/bash
# move-files-phase1.sh
# Phase 1: Move low-risk files (data, config, docs)
# Usage: ./move-files-phase1.sh

set -e

echo "Phase 1: Moving low-risk files..."

# Move data files
echo "Moving data files..."
if [ -f "mock-db.json" ]; then
    mv mock-db.json data/
    echo "  ✓ mock-db.json → data/"
fi

if [ -f "mock-db 2.json" ]; then
    mv "mock-db 2.json" data/
    echo "  ✓ mock-db 2.json → data/"
fi

if [ -f "test-data/" ]; then
    mv test-data/* data/ 2>/dev/null || true
    rmdir test-data 2>/dev/null || true
    echo "  ✓ test-data/ → data/"
fi

# Move documentation files
echo "Moving documentation files..."
mkdir -p docs/project
for doc in *.md; do
    if [[ "$doc" != "README.md" && "$doc" != "GOVERNANCE.md" ]]; then
        mv "$doc" docs/project/
        echo "  ✓ $doc → docs/project/"
    fi
done

# Move config files
echo "Moving configuration files..."
mkdir -p config/ci
for config in docker-compose*.yml test-compose.yml; do
    if [ -f "$config" ]; then
        mv "$config" config/
        echo "  ✓ $config → config/"
    fi
done

# Move build artifacts
echo "Moving build artifacts..."
if [ -d "artifacts/" ]; then
    mv artifacts/* build/ 2>/dev/null || true
    rmdir artifacts 2>/dev/null || true
    echo "  ✓ artifacts/ → build/"
fi

echo ""
echo "Phase 1 complete! Files moved:"
echo "  - Data files → data/"
echo "  - Documentation → docs/project/"
echo "  - Config files → config/"
echo "  - Build artifacts → build/"
echo ""
echo "Next: Run validate-moves.sh to check the moves"
echo "Then: Run move-files-phase2.sh for source code moves"