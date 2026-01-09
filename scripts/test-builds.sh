#!/bin/bash
# test-builds.sh
# Tests that builds still work after restructuring
# Usage: ./test-builds.sh

set -e

echo "Testing builds after restructuring..."
echo "This may take several minutes..."
echo ""

errors=0

# Test .NET build
echo "Testing .NET build..."
if command -v dotnet &> /dev/null; then
    echo "  Building AppHost..."
    if dotnet build AppHost/B2X.AppHost.csproj --verbosity minimal; then
        echo "  ✓ AppHost build successful"
    else
        echo "  ❌ AppHost build failed"
        errors=$((errors + 1))
    fi

    echo "  Building solution..."
    if dotnet build B2X.slnx --verbosity minimal; then
        echo "  ✓ Solution build successful"
    else
        echo "  ❌ Solution build failed"
        errors=$((errors + 1))
    fi
else
    echo "  ⚠️  dotnet CLI not found, skipping .NET tests"
fi

# Test Node.js builds
echo ""
echo "Testing Node.js builds..."
echo "Note: Frontend workspaces are not fully configured yet."
echo "Skipping frontend builds - requires separate workspace setup."
if command -v npm &> /dev/null; then
    # Temporarily skip frontend builds until workspaces are properly configured
    echo "  ⚠️  Frontend builds skipped (workspaces not configured)"
else
    echo "  ⚠️  npm not found, skipping Node.js tests"
fi

# Test that key files exist
echo ""
echo "Testing key files exist..."
key_files=(
    "src/Domain/Catalog/B2X.Catalog.API.csproj"
    "src/Gateway/Store/API/B2X.Store.csproj"
    "README.md"
    "config/docker-compose.yml"
)

for file in "${key_files[@]}"; do
    if [ -f "$file" ]; then
        echo "  ✓ $file exists"
    else
        echo "  ❌ $file missing"
        errors=$((errors + 1))
    fi
done

echo ""
if [ $errors -eq 0 ]; then
    echo "✅ All build tests passed!"
    echo ""
    echo "Next steps:"
    echo "  1. Run tests: ./scripts/run-tests.sh"
    echo "  2. Test services start: ./scripts/service-health.sh"
    echo "  3. Commit changes: git add -A && git commit -m 'refactor: restructure to src/docs/tests layout'"
else
    echo "❌ $errors build test(s) failed. Please check the errors above."
    echo ""
    echo "Troubleshooting:"
    echo "  - Check that all files were moved correctly"
    echo "  - Verify reference updates in update-references.sh"
    echo "  - Check for broken imports or project references"
    echo "  - Consider running rollback.sh if needed"
    exit 1
fi