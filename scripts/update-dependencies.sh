#!/bin/bash
# Automated Dependency Update Script
# Updates Directory.Packages.props and package.json with latest versions

set -e

echo "🔄 Starting Automated Dependency Updates..."

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m'

print_status() {
    local color=$1
    local message=$2
    echo -e "${color}${message}${NC}"
}

# Function to update NuGet packages in Directory.Packages.props
update_nuget_packages() {
    print_status $GREEN "📦 Updating NuGet packages..."

    # Get outdated packages
    "/mnt/c/Program Files/dotnet/dotnet.exe" list package --outdated --format json > outdated.json

    # Extract package updates
    jq -r '.projects[0].frameworks[0].topLevelPackages[]? | select(.latestVersion != .resolvedVersion) | "\(.id)|\(.resolvedVersion)|\(.latestVersion)"' outdated.json > updates.txt

    if [ ! -s updates.txt ]; then
        print_status $GREEN "✅ No NuGet package updates needed"
        return 0
    fi

    # Update Directory.Packages.props
    while IFS='|' read -r package_id current_version latest_version; do
        if grep -q "Include=\"$package_id\"" Directory.Packages.props; then
            print_status $YELLOW "  Updating $package_id: $current_version → $latest_version"

            # Use sed to update the version
            sed -i "s|Include=\"$package_id\" Version=\"$current_version\"|Include=\"$package_id\" Version=\"$latest_version\"|g" Directory.Packages.props
        fi
    done < updates.txt

    print_status $GREEN "✅ NuGet packages updated"
}

# Function to update NPM packages
update_npm_packages() {
    print_status $GREEN "📦 Updating NPM packages..."

    cd frontend/Store

    # Get outdated packages
    npm outdated --json > ../../npm-outdated.json

    # Check if there are updates
    if [ "$(jq 'keys | length' ../../npm-outdated.json)" -eq 0 ]; then
        print_status $GREEN "✅ No NPM package updates needed"
        cd ../..
        return 0
    fi

    # Update packages
    npm update

    # Install to update package-lock.json
    npm install

    cd ../..

    print_status $GREEN "✅ NPM packages updated"
}

# Function to validate updates
validate_updates() {
    print_status $GREEN "🔍 Validating updates..."

    # Run CI validation script
    if ./scripts/ci-validate-dependencies.sh; then
        print_status $GREEN "✅ CI validation passed"
    else
        print_status $RED "❌ CI validation failed"
        exit 1
    fi

    # Try to build
    if "/mnt/c/Program Files/dotnet/dotnet.exe" build --configuration Release --verbosity minimal; then
        print_status $GREEN "✅ Build succeeded"
    else
        print_status $RED "❌ Build failed"
        exit 1
    fi

    # Run tests
    if "/mnt/c/Program Files/dotnet/dotnet.exe" test --configuration Release --verbosity minimal --no-build; then
        print_status $GREEN "✅ Tests passed"
    else
        print_status $RED "❌ Tests failed"
        exit 1
    fi
}

# Main execution
main() {
    print_status $GREEN "🚀 Starting dependency update process..."

    # Backup original files
    cp Directory.Packages.props Directory.Packages.props.backup
    cp frontend/Store/package.json frontend/Store/package.json.backup
    cp frontend/Store/package-lock.json frontend/Store/package-lock.json.backup

    # Update packages
    update_nuget_packages
    update_npm_packages

    # Validate
    validate_updates

    print_status $GREEN "🎉 All dependency updates completed successfully!"

    # Clean up backup files on success
    rm -f Directory.Packages.props.backup
    rm -f frontend/Store/package.json.backup
    rm -f frontend/Store/package-lock.json.backup
    rm -f outdated.json updates.txt npm-outdated.json
}

# Error handling
error_handler() {
    print_status $RED "❌ Error occurred during dependency updates"

    # Restore backups
    if [ -f Directory.Packages.props.backup ]; then
        mv Directory.Packages.props.backup Directory.Packages.props
        print_status $YELLOW "  Restored Directory.Packages.props from backup"
    fi

    if [ -f frontend/Store/package.json.backup ]; then
        mv frontend/Store/package.json.backup frontend/Store/package.json
        print_status $YELLOW "  Restored package.json from backup"
    fi

    if [ -f frontend/Store/package-lock.json.backup ]; then
        mv frontend/Store/package-lock.json.backup frontend/Store/package-lock.json
        print_status $YELLOW "  Restored package-lock.json from backup"
    fi

    exit 1
}

# Set error handler
trap error_handler ERR

# Run main function
main