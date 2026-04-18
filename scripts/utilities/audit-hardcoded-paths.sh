#!/bin/bash
# audit-hardcoded-paths.sh
# Audit for hardcoded paths that may break during refactoring

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
    echo -e "${BLUE}  Hardcoded Paths Audit${NC}"
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

# Function to check for hardcoded directory references
check_hardcoded_dirs() {
    local dir="$1"
    local new_path="$2"
    local risk="$3"

    echo -n "Checking hardcoded '$dir' references... "

    # Look for absolute path patterns
    local count=$(grep -r "$dir" --include="*" --exclude-dir=".git" --exclude-dir="node_modules" --exclude-dir="bin" --exclude-dir="obj" . 2>/dev/null | grep -v "^\.\./" | grep -v "^\./" | wc -l)

    if [ "$count" -gt 0 ]; then
        case "$risk" in
            "HIGH")
                echo -e "${RED}$count found (HIGH RISK)${NC}"
                ;;
            "MEDIUM")
                echo -e "${YELLOW}$count found (MEDIUM RISK)${NC}"
                ;;
            "LOW")
                echo -e "${GREEN}$count found (LOW RISK)${NC}"
                ;;
        esac

        echo "Potential issues:"
        grep -r "$dir" --include="*" --exclude-dir=".git" --exclude-dir="node_modules" --exclude-dir="bin" --exclude-dir="obj" . 2>/dev/null | grep -v "^\.\./" | grep -v "^\./" | head -5
        echo
    else
        echo -e "${GREEN}none found${NC}"
    fi
}

# Function to check for absolute paths
check_absolute_paths() {
    print_section "Checking for Absolute Paths"

    echo "Looking for absolute path patterns..."
    local abs_paths=$(grep -r "^/" --include="*.sh" --include="*.ps1" --include="*.json" --include="*.yml" --include="*.yaml" --exclude-dir=".git" --exclude-dir="node_modules" . 2>/dev/null | wc -l)

    if [ "$abs_paths" -gt 0 ]; then
        print_warning "Found $abs_paths absolute paths in config/script files"
        echo "These may need updating for different environments:"
        grep -r "^/" --include="*.sh" --include="*.ps1" --include="*.json" --include="*.yml" --include="*.yaml" --exclude-dir=".git" --exclude-dir="node_modules" . 2>/dev/null | head -5
        echo
    else
        echo -e "${GREEN}No absolute paths found in config files${NC}"
    fi
}

# Function to check for Windows vs Unix path separators
check_path_separators() {
    print_section "Checking Path Separators"

    echo "Looking for Windows-style backslashes..."
    local backslashes=$(grep -r "\\\\" --include="*.sh" --include="*.ps1" --include="*.json" --include="*.yml" --include="*.yaml" --exclude-dir=".git" --exclude-dir="node_modules" . 2>/dev/null | wc -l)

    if [ "$backslashes" -gt 0 ]; then
        print_warning "Found $backslashes Windows-style paths (may cause issues on Unix)"
        echo "Consider using forward slashes or cross-platform path handling:"
        grep -r "\\\\" --include="*.sh" --include="*.ps1" --include="*.json" --include="*.yml" --include="*.yaml" --exclude-dir=".git" --exclude-dir="node_modules" . 2>/dev/null | head -3
        echo
    fi
}

# Function to check for hardcoded localhost URLs
check_localhost_refs() {
    print_section "Checking Localhost References"

    local localhost_count=$(grep -r "localhost" --include="*.json" --include="*.yml" --include="*.yaml" --include="*.config" --exclude-dir=".git" --exclude-dir="node_modules" . 2>/dev/null | wc -l)

    if [ "$localhost_count" -gt 0 ]; then
        print_warning "Found $localhost_count localhost references in config files"
        echo "These may need environment-specific overrides:"
        grep -r "localhost" --include="*.json" --include="*.yml" --include="*.yaml" --include="*.config" --exclude-dir=".git" --exclude-dir="node_modules" . 2>/dev/null | head -3
        echo
    fi
}

# Function to check for hardcoded ports
check_hardcoded_ports() {
    print_section "Checking Hardcoded Ports"

    # Look for common development ports
    local ports=("3000" "5000" "5001" "8000" "8080" "9000")
    local found_ports=()

    for port in "${ports[@]}"; do
        local count=$(grep -r ":$port" --include="*.json" --include="*.yml" --include="*.yaml" --include="*.config" --exclude-dir=".git" --exclude-dir="node_modules" . 2>/dev/null | wc -l)
        if [ "$count" -gt 0 ]; then
            found_ports+=("$port ($count)")
        fi
    done

    if [ ${#found_ports[@]} -gt 0 ]; then
        print_warning "Found hardcoded development ports:"
        printf '%s\n' "${found_ports[@]}"
        echo "Consider using environment variables for port configuration."
        echo
    fi
}

# Function to check for file extension dependencies
check_file_extensions() {
    print_section "Checking File Extension Dependencies"

    local extensions=("*.cs" "*.csproj" "*.ts" "*.js" "*.vue" "*.json" "*.yml" "*.yaml")
    local found_deps=()

    for ext in "${extensions[@]}"; do
        local count=$(grep -r "$ext" --include="*.sh" --include="*.ps1" --include="*.json" --include="*.yml" --include="*.yaml" --exclude-dir=".git" --exclude-dir="node_modules" . 2>/dev/null | wc -l)
        if [ "$count" -gt 0 ]; then
            found_deps+=("$ext ($count)")
        fi
    done

    if [ ${#found_deps[@]} -gt 0 ]; then
        echo "File extension references found in scripts/configs:"
        printf '%s\n' "${found_deps[@]}"
        echo "These may need updating if file locations change."
        echo
    fi
}

# Function to check for relative path assumptions
check_relative_paths() {
    print_section "Checking Relative Path Assumptions"

    # Look for ../ patterns that might break
    local relative_count=$(grep -r "\.\./\.\./" --include="*.sh" --include="*.ps1" --include="*.json" --include="*.yml" --include="*.yaml" --exclude-dir=".git" --exclude-dir="node_modules" . 2>/dev/null | wc -l)

    if [ "$relative_count" -gt 0 ]; then
        print_warning "Found $relative_count deep relative path references (../../..)"
        echo "These may break if directory structure changes:"
        grep -r "\.\./\.\./" --include="*.sh" --include="*.ps1" --include="*.json" --include="*.yml" --include="*.yaml" --exclude-dir=".git" --exclude-dir="node_modules" . 2>/dev/null | head -3
        echo
    fi
}

# Function to generate recommendations
generate_recommendations() {
    print_section "Recommendations"

    echo "Based on this audit, consider these improvements:"
    echo
    echo "1. **Environment Variables**: Replace hardcoded paths with environment variables"
    echo "2. **Configuration Files**: Use different configs for different environments"
    echo "3. **Path Resolution**: Implement dynamic path resolution in scripts"
    echo "4. **Cross-Platform**: Use forward slashes and path.join() for cross-platform compatibility"
    echo "5. **Validation**: Add path validation in startup scripts"
    echo
    print_warning "Address HIGH RISK items before proceeding with refactoring!"
}

main() {
    print_header
    echo "Auditing for hardcoded paths and environment assumptions..."
    echo

    # Check specific directories that will move
    check_hardcoded_dirs "AppHost" "src/AppHost" "HIGH"
    check_hardcoded_dirs "Backend" "src/Backend" "HIGH"
    check_hardcoded_dirs "Frontend" "src/Frontend" "HIGH"
    check_hardcoded_dirs "docs" "docs" "MEDIUM"
    check_hardcoded_dirs "tests" "tests" "MEDIUM"

    check_absolute_paths
    check_path_separators
    check_localhost_refs
    check_hardcoded_ports
    check_file_extensions
    check_relative_paths

    generate_recommendations
}

main "$@"