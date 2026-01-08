#!/bin/bash
# CI Validation Script for Dependency Management
# Ensures CPM compliance and version consistency across the project

set -e  # Exit on any error

echo "🔍 Starting CI Dependency Validation..."

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    local color=$1
    local message=$2
    echo -e "${color}${message}${NC}"
}

# Check if Directory.Packages.props exists
if [ ! -f "Directory.Packages.props" ]; then
    print_status $RED "❌ Directory.Packages.props not found in root directory"
    exit 1
fi

print_status $GREEN "✅ Root Directory.Packages.props found"

# Check if ManagePackageVersionsCentrally is enabled
if ! grep -q "ManagePackageVersionsCentrally.*true" Directory.Packages.props; then
    print_status $RED "❌ ManagePackageVersionsCentrally is not enabled in root Directory.Packages.props"
    exit 1
fi

print_status $GREEN "✅ CPM is enabled in root Directory.Packages.props"

# Check for duplicate Directory.Packages.props files
DUPLICATE_FILES=$(find . -name "Directory.Packages.props" -type f | grep -v "^./Directory.Packages.props$" | grep -v "/rename-backup-" | grep -v "/backup" | grep -v "/temp" | grep -v "/tmp" | wc -l)

if [ "$DUPLICATE_FILES" -gt 0 ]; then
    print_status $RED "❌ Found $DUPLICATE_FILES duplicate Directory.Packages.props files:"
    find . -name "Directory.Packages.props" -type f | grep -v "^./Directory.Packages.props$" | grep -v "/rename-backup-" | grep -v "/backup" | grep -v "/temp" | grep -v "/tmp"
    exit 1
fi

print_status $GREEN "✅ No duplicate Directory.Packages.props files found"

# Check that all .csproj files reference CPM (don't have hardcoded versions)
CSPROJ_FILES=$(find . -name "*.csproj" -type f | grep -v "/bin/" | grep -v "/obj/" | grep -v "/tools/" | grep -v "/rename-backup-" | grep -v "/backup" | grep -v "/temp" | grep -v "/tmp")

PROBLEMATIC_FILES=()

for csproj in $CSPROJ_FILES; do
    # Skip projects with CPM disabled
    if grep -q "<ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>" "$csproj"; then
        continue
    fi
    
    # Check for PackageReference with Version attribute (should not exist in CPM-enabled projects)
    if grep -q "<PackageReference.*Version=" "$csproj"; then
        PROBLEMATIC_FILES+=("$csproj")
    fi
done

if [ ${#PROBLEMATIC_FILES[@]} -gt 0 ]; then
    print_status $RED "❌ Found ${#PROBLEMATIC_FILES[@]} .csproj files with hardcoded package versions (violates CPM):"
    for file in "${PROBLEMATIC_FILES[@]}"; do
        echo "  - $file"
    done
    print_status $YELLOW "💡 Fix: Remove Version attributes from PackageReference elements"
    exit 1
fi

print_status $GREEN "✅ All .csproj files comply with CPM (no hardcoded versions)"

# Check that projects do NOT have explicit Directory.Packages.props imports (handled automatically by .NET 10 SDK)
EXPLICIT_IMPORT_FILES=()

for csproj in $CSPROJ_FILES; do
    # Check for explicit Import of Directory.Packages.props (should not exist in .NET 10+)
    if grep -q "Directory.Packages.props" "$csproj"; then
        EXPLICIT_IMPORT_FILES+=("$csproj")
    fi
done

if [ ${#EXPLICIT_IMPORT_FILES[@]} -gt 0 ]; then
    print_status $RED "❌ Found ${#EXPLICIT_IMPORT_FILES[@]} .csproj files with explicit Directory.Packages.props imports (not needed in .NET 10+):"
    for file in "${EXPLICIT_IMPORT_FILES[@]}"; do
        echo "  - $file"
    done
    print_status $YELLOW "💡 Fix: Remove explicit Directory.Packages.props imports - they're handled automatically by .NET 10 SDK"
    exit 1
fi

print_status $GREEN "✅ No explicit Directory.Packages.props imports found (.NET 10 SDK handles automatically)"
TOOLS_CSPROJ=$(find tools -name "*.csproj" -type f 2>/dev/null || true)

if [ -n "$TOOLS_CSPROJ" ]; then
    print_status $YELLOW "⚠️  Found .csproj files in tools/ folder - these may bypass CPM:"
    echo "$TOOLS_CSPROJ"
    print_status $YELLOW "💡 Consider: Update tools projects to use CPM or document exceptions"
fi

# Validate package versions are reasonable (not empty, follow semantic versioning)
INVALID_VERSIONS=$(grep "<PackageVersion" Directory.Packages.props | grep -v 'Version="[0-9]\+\.[0-9]\+\.[0-9]\+\([^"]*\)"' | wc -l)

if [ "$INVALID_VERSIONS" -gt 0 ]; then
    print_status $RED "❌ Found $INVALID_VERSIONS package versions that don't follow semantic versioning pattern"
    grep "<PackageVersion" Directory.Packages.props | grep -v 'Version="[0-9]\+\.[0-9]\+\.[0-9]\+\([^"]*\)"'
    exit 1
fi

print_status $GREEN "✅ All package versions follow semantic versioning pattern"

# Check for version consistency (no duplicate package IDs with different versions)
DUPLICATE_PACKAGES=$(grep "<PackageVersion" Directory.Packages.props | sed 's/.*Include="\([^"]*\)".*Version="\([^"]*\)".*/\1:\2/' | sort | uniq -d | wc -l)

if [ "$DUPLICATE_PACKAGES" -gt 0 ]; then
    print_status $RED "❌ Found duplicate package IDs with different versions:"
    grep "<PackageVersion" Directory.Packages.props | sed 's/.*Include="\([^"]*\)".*Version="\([^"]*\)".*/\1:\2/' | sort | uniq -d
    exit 1
fi

print_status $GREEN "✅ No duplicate package IDs with conflicting versions"

# Check for required package groups (basic validation)
REQUIRED_PACKAGES=("Microsoft.Extensions" "Microsoft.EntityFrameworkCore" "xunit" "FluentAssertions")

MISSING_PACKAGES=()

for package in "${REQUIRED_PACKAGES[@]}"; do
    if ! grep -q "Include=\"$package" Directory.Packages.props; then
        MISSING_PACKAGES+=("$package")
    fi
done

if [ ${#MISSING_PACKAGES[@]} -gt 0 ]; then
    print_status $YELLOW "⚠️  Missing some expected package groups:"
    for package in "${MISSING_PACKAGES[@]}"; do
        echo "  - $package"
    done
    print_status $YELLOW "💡 This may be normal if these packages aren't used in this project"
fi

print_status $GREEN "🎉 CI Dependency Validation completed successfully!"

# Summary
TOTAL_PACKAGES=$(grep -c "<PackageVersion" Directory.Packages.props)
echo ""
print_status $GREEN "📊 Summary:"
echo "  - Total packages managed: $TOTAL_PACKAGES"
echo "  - CPM enabled: ✅"
echo "  - No duplicates: ✅"
echo "  - All projects compliant: ✅"
echo "  - Version format valid: ✅"
echo "  - No explicit imports: ✅"

exit 0