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
if command -v npm &> /dev/null; then
    # Test admin frontend
    if [ -d "src/admin" ] && [ -f "src/admin/package.json" ]; then
        echo "  Building admin frontend..."
        cd src/admin
        if npm run build 2>/dev/null; then
            echo "  ✓ Admin frontend build successful"
        else
            echo "  ❌ Admin frontend build failed"
            errors=$((errors + 1))
        fi
        cd ../..
    fi

    # Test store frontend
    if [ -d "src/store" ] && [ -f "src/store/package.json" ]; then
        echo "  Building store frontend..."
        cd src/store
        if npm run build 2>/dev/null; then
            echo "  ✓ Store frontend build successful"
        else
            echo "  ❌ Store frontend build failed"
            errors=$((errors + 1))
        fi
        cd ../..
    fi

    # Test management frontend
    if [ -d "src/management" ] && [ -f "src/management/package.json" ]; then
        echo "  Building management frontend..."
        cd src/management
        if npm run build 2>/dev/null; then
            echo "  ✓ Management frontend build successful"
        else
            echo "  ❌ Management frontend build failed"
            errors=$((errors + 1))
        fi
        cd ../..
    fi
else
    echo "  ⚠️  npm not found, skipping Node.js tests"
fi

# Test that key files exist
echo ""
echo "Testing key files exist..."
key_files=(
    "src/Api/B2X.Gateway/B2X.Gateway.csproj"
    "src/Domain/Catalog/B2X.Catalog.csproj"
    "src/Gateway/Store/API/B2X.Store.csproj"
    "tests/tests/tests/tests/AppHost.Tests/tests/tests/tests/AppHost.Tests.csproj"
    "docs/project/README.md"
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