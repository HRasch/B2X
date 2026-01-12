#!/bin/bash
# check-build-configs.sh
# Check build configurations and dependencies for refactoring impact

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"

cd "$PROJECT_ROOT"

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

print_header() {
    echo -e "${BLUE}========================================${NC}"
    echo -e "${BLUE}  Build Configuration Audit${NC}"
    echo -e "${BLUE}========================================${NC}"
}

print_section() {
    echo -e "${GREEN}[SECTION]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Function to check .csproj files for hardcoded paths
check_csproj_paths() {
    print_section "Checking .csproj Files for Hardcoded Paths"

    local csproj_files=$(find . -name "*.csproj" -not -path "./.git/*" -not -path "./node_modules/*")

    for file in $csproj_files; do
        echo "Checking $file..."

        # Check for hardcoded directory references
        local hardcoded=$(grep -E "(AppHost|Backend|Frontend)" "$file" | grep -v "<PackageReference" | grep -v "<ProjectReference")

        if [ -n "$hardcoded" ]; then
            print_warning "Found hardcoded paths in $file:"
            echo "$hardcoded"
            echo
        fi

        # Check for relative paths that might break
        local relative_paths=$(grep -E "\.\./\.\./" "$file")

        if [ -n "$relative_paths" ]; then
            print_warning "Found deep relative paths in $file:"
            echo "$relative_paths"
            echo
        fi
    done
}

# Function to check package.json files
check_package_json() {
    print_section "Checking package.json Files"

    local package_files=$(find . -name "package.json" -not -path "./.git/*" -not -path "./node_modules/*")

    for file in $package_files; do
        echo "Checking $file..."

        # Check for hardcoded paths in scripts
        local scripts=$(grep -A 20 '"scripts"' "$file" | grep -E "(AppHost|Backend|Frontend|docs|tests)" | grep -v "node_modules")

        if [ -n "$scripts" ]; then
            print_warning "Found hardcoded paths in scripts section of $file:"
            echo "$scripts"
            echo
        fi
    done
}

# Function to check Docker files
check_docker_files() {
    print_section "Checking Docker Files"

    local docker_files=$(find . -name "Dockerfile*" -o -name "docker-compose*.yml" -o -name "docker-compose*.yaml" | grep -v node_modules)

    for file in $docker_files; do
        echo "Checking $file..."

        # Check for COPY/ADD commands with hardcoded paths
        local copy_commands=$(grep -E "^(COPY|ADD)" "$file" | grep -E "(AppHost|Backend|Frontend)")

        if [ -n "$copy_commands" ]; then
            print_warning "Found hardcoded paths in COPY/ADD commands in $file:"
            echo "$copy_commands"
            echo
        fi

        # Check for WORKDIR with hardcoded paths
        local workdir=$(grep "^WORKDIR" "$file" | grep -E "(AppHost|Backend|Frontend)")

        if [ -n "$workdir" ]; then
            print_warning "Found hardcoded WORKDIR in $file:"
            echo "$workdir"
            echo
        fi
    done
}

# Function to check shell scripts
check_shell_scripts() {
    print_section "Checking Shell Scripts"

    local script_files=$(find . -name "*.sh" -not -path "./.git/*" -not -path "./node_modules/*")

    for file in $script_files; do
        echo "Checking $file..."

        # Check for hardcoded directory references
        local hardcoded=$(grep -E "(AppHost|Backend|Frontend)" "$file" | grep -v "#")

        if [ -n "$hardcoded" ]; then
            print_warning "Found hardcoded paths in $file:"
            echo "$hardcoded"
            echo
        fi
    done
}

# Function to check YAML configuration files
check_yaml_configs() {
    print_section "Checking YAML Configuration Files"

    local yaml_files=$(find . -name "*.yml" -o -name "*.yaml" | grep -v node_modules | grep -v ".git")

    for file in $yaml_files; do
        echo "Checking $file..."

        # Check for hardcoded paths
        local hardcoded=$(grep -E "(AppHost|Backend|Frontend)" "$file")

        if [ -n "$hardcoded" ]; then
            print_warning "Found hardcoded paths in $file:"
            echo "$hardcoded"
            echo
        fi
    done
}

# Function to check for build dependencies
check_build_dependencies() {
    print_section "Checking Build Dependencies"

    echo "Looking for build dependency patterns..."

    # Check for solution file references
    local sln_refs=$(grep -r "B2X.slnx" --include="*.sh" --include="*.ps1" --include="*.json" --include="*.yml" --include="*.yaml" --exclude-dir=".git" --exclude-dir="node_modules" . 2>/dev/null)

    if [ -n "$sln_refs" ]; then
        print_warning "Found references to B2X.slnx that may need updating:"
        echo "$sln_refs" | head -5
        echo
    fi

    # Check for project file references
    local proj_refs=$(grep -r "\.csproj" --include="*.sh" --include="*.ps1" --include="*.json" --include="*.yml" --include="*.yaml" --exclude-dir=".git" --exclude-dir="node_modules" . 2>/dev/null | grep -v "Directory.Build.props")

    if [ -n "$proj_refs" ]; then
        echo "Found .csproj references in config files (may need updating):"
        echo "$proj_refs" | head -5
        echo
    fi
}

# Function to check for test configurations
check_test_configs() {
    print_section "Checking Test Configurations"

    # Look for test project references
    local test_refs=$(find . -name "*.csproj" -exec grep -l "Microsoft.NET.Test.Sdk" {} \; 2>/dev/null)

    if [ -n "$test_refs" ]; then
        echo "Test projects found:"
        for test_proj in $test_refs; do
            echo "  - $test_proj"
        done
        echo

        # Check if test projects reference moved source files
        for test_proj in $test_refs; do
            local test_dir=$(dirname "$test_proj")
            local source_refs=$(grep -E "(AppHost|Backend|Frontend)" "$test_proj")

            if [ -n "$source_refs" ]; then
                print_warning "Test project $test_proj has hardcoded source references:"
                echo "$source_refs"
                echo
            fi
        done
    fi
}

# Function to generate build impact report
generate_build_report() {
    print_section "Build Impact Summary"

    echo "Key findings for refactoring impact:"
    echo
    echo "1. **Project References**: Update .csproj files to use new relative paths"
    echo "2. **Build Scripts**: Update shell scripts with new directory structure"
    echo "3. **Docker Files**: Update COPY/ADD commands and WORKDIR instructions"
    echo "4. **Package Scripts**: Update npm scripts in package.json files"
    echo "5. **Test Projects**: Ensure test projects can find moved source files"
    echo "6. **CI/CD Pipelines**: Update any hardcoded paths in GitHub Actions"
    echo
    print_warning "Run a full build test after path updates to validate changes!"
}

main() {
    print_header
    echo "Auditing build configurations for refactoring compatibility..."
    echo

    check_csproj_paths
    check_package_json
    check_docker_files
    check_shell_scripts
    check_yaml_configs
    check_build_dependencies
    check_test_configs

    generate_build_report
}

main "$@"