#!/bin/bash
# update-references.sh
# Updates file references after directory restructuring
# Usage: ./update-references.sh

set -e

echo "Updating file references after restructuring..."

# Update .slnx file references
echo "Updating .slnx file..."
if [ -f "B2X.slnx" ]; then
    # Update project paths in .slnx
    sed -i.bak 's|src/|src/|g' B2X.slnx
    sed -i.bak 's|src/|src/|g' B2X.slnx
    sed -i.bak 's|AppHost\.Tests|tests/tests/tests/tests/AppHost.Tests|g' B2X.slnx
    echo "  ✓ Updated B2X.slnx"
fi

# Update package.json references
echo "Updating package.json..."
if [ -f "package.json" ]; then
    # Update workspace paths
    sed -i.bak 's|"src/|"src/|g' package.json
    echo "  ✓ Updated package.json"
fi

# Update csproj files
echo "Updating .csproj files..."
find src/ -name "*.csproj" -type f | while read -r file; do
    echo "  Updating $file..."
    # Update project references
    sed -i.bak 's|..\\Backend\\|..\\..\\src\\|g' "$file"
    sed -i.bak 's|..\\Frontend\\|..\\..\\src\\|g' "$file"
    sed -i.bak 's|..\\AppHost|..\\..\\AppHost|g' "$file"
    sed -i.bak 's|..\\ServiceDefaults|..\\..\\ServiceDefaults|g' "$file"
done

# Update TypeScript/JavaScript imports
echo "Updating TypeScript/JavaScript imports..."
find src/ -name "*.ts" -o -name "*.js" -o -name "*.vue" | while read -r file; do
    # Skip node_modules and build outputs
    if [[ "$file" == *"node_modules"* ]] || [[ "$file" == *"dist"* ]] || [[ "$file" == *"build"* ]]; then
        continue
    fi

    echo "  Updating imports in $file..."
    # Update relative imports that reference moved directories
    sed -i.bak 's|from '\''\.\./\.\./src/|from '\''\.\./\.\./src/|g' "$file"
    sed -i.bak 's|from '\''\.\./\.\./src/|from '\''\.\./\.\./src/|g' "$file"
    sed -i.bak 's|import.*'\''\.\./\.\./src/|import '\''\.\./\.\./src/|g' "$file"
    sed -i.bak 's|import.*'\''\.\./\.\./src/|import '\''\.\./\.\./src/|g' "$file"
done

# Update C# using statements
echo "Updating C# using statements..."
find src/ -name "*.cs" | while read -r file; do
    echo "  Updating using statements in $file..."
    # Update namespace references
    sed -i.bak 's|using B2X\.Backend\.|using B2X\.src\.|g' "$file"
    sed -i.bak 's|using B2X\.Frontend\.|using B2X\.src\.|g' "$file"
done

# Update Docker compose files
echo "Updating Docker compose files..."
find config/ -name "docker-compose*.yml" | while read -r file; do
    echo "  Updating $file..."
    sed -i.bak 's|./src/|./src/|g' "$file"
    sed -i.bak 's|./src/|./src/|g' "$file"
done

# Update scripts
echo "Updating script references..."
find scripts/ -name "*.sh" | while read -r file; do
    echo "  Updating $file..."
    sed -i.bak 's|src/|src/|g' "$file"
    sed -i.bak 's|src/|src/|g' "$file"
    sed -i.bak 's|AppHost\.Tests|tests/tests/tests/tests/AppHost.Tests|g' "$file"
done

echo ""
echo "Reference updates complete!"
echo ""
echo "Backup files created with .bak extension"
echo "Review changes and remove .bak files when satisfied"
echo ""
echo "Next: Run test-builds.sh to verify builds still work"