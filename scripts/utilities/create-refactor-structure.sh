#!/bin/bash
# create-refactor-structure.sh
# Creates the new directory structure for B2X project refactoring
# Usage: ./create-refactor-structure.sh

set -e

echo "Creating new directory structure for B2X refactoring..."

# Create main directories
mkdir -p src/{api,services,models,repositories,shared}
mkdir -p src/{components,pages,hooks,ui,stores}
mkdir -p tests/{unit,integration,e2e,performance}
mkdir -p docs/{developer,user,api,architecture}
mkdir -p build/{artifacts,packages,temp}
mkdir -p config/{ci,deployment,environments}
mkdir -p data/{migrations,seeds,fixtures}
mkdir -p archive/{old-code,deprecated,backups}

echo "Directory structure created successfully!"
echo ""
echo "Created directories:"
echo "  src/           - Source code (Backend, Frontend)"
echo "  tests/         - All test files"
echo "  docs/          - Documentation"
echo "  build/         - Build artifacts"
echo "  config/        - Configuration files"
echo "  data/          - Data files and migrations"
echo "  archive/       - Archived files"
echo ""
echo "Next: Run move-files-phase1.sh to start moving files"