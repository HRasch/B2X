#!/bin/bash
# discover-b2x-references.sh
# Comprehensive discovery of all B2X references in the codebase

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
    echo -e "${BLUE}  B2X Reference Discovery Tool${NC}"
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

# Function to count occurrences
count_occurrences() {
    local pattern="$1"
    local description="$2"

    echo -n "Searching for '$pattern'... "
    local count=$(grep -r "$pattern" --include="*" --exclude-dir=".git" --exclude-dir="node_modules" --exclude-dir="bin" --exclude-dir="obj" . 2>/dev/null | wc -l)
    echo "$count occurrences"

    if [ "$count" -gt 0 ]; then
        echo "Files containing '$description':"
        grep -r "$pattern" --include="*" --exclude-dir=".git" --exclude-dir="node_modules" --exclude-dir="bin" --exclude-dir="obj" . 2>/dev/null | head -10
        if [ "$count" -gt 10 ]; then
            echo "... and $(($count - 10)) more files"
        fi
        echo
    fi
}

# Function to search specific file types
search_file_type() {
    local pattern="$1"
    local file_pattern="$2"
    local description="$3"

    echo -n "Searching $file_pattern files for '$pattern'... "
    local count=$(find . -name "$file_pattern" -type f -exec grep -l "$pattern" {} \; 2>/dev/null | wc -l)
    echo "$count files"

    if [ "$count" -gt 0 ]; then
        echo "$description files:"
        find . -name "$file_pattern" -type f -exec grep -l "$pattern" {} \; 2>/dev/null | head -5
        if [ "$count" -gt 5 ]; then
            echo "... and $(($count - 5)) more files"
        fi
        echo
    fi
}

# Main discovery
main() {
    print_header
    echo "Discovering all B2X references that may need updating..."
    echo

    print_section "1. Basic B2X Pattern Search"
    count_occurrences "B2X" "basic B2X references"

    print_section "2. File Type Specific Searches"

    # Code files
    search_file_type "B2X" "*.cs" "C# source"
    search_file_type "B2X" "*.csproj" "C# project"
    search_file_type "B2X" "*.ts" "TypeScript"
    search_file_type "B2X" "*.js" "JavaScript"
    search_file_type "B2X" "*.vue" "Vue.js"

    # Configuration files
    search_file_type "B2X" "*.json" "JSON config"
    search_file_type "B2X" "*.yml" "YAML config"
    search_file_type "B2X" "*.yaml" "YAML config"
    search_file_type "B2X" "*.xml" "XML config"
    search_file_type "B2X" "*.config" "Config files"

    # Scripts and docs
    search_file_type "B2X" "*.sh" "Shell scripts"
    search_file_type "B2X" "*.ps1" "PowerShell scripts"
    search_file_type "B2X" "*.md" "Markdown docs"
    search_file_type "B2X" "*.txt" "Text files"

    print_section "3. High-Risk Pattern Searches"

    # Docker and containerization
    count_occurrences "image.*B2X" "Docker image references"
    count_occurrences "container.*B2X" "Container names"

    # Kubernetes
    count_occurrences "namespace.*B2X" "Kubernetes namespaces"
    count_occurrences "service.*B2X" "Kubernetes services"
    count_occurrences "deployment.*B2X" "Kubernetes deployments"

    # Environment variables
    count_occurrences "B2X_" "Environment variables"

    # URLs and endpoints
    count_occurrences "api.*B2X" "API endpoints"
    count_occurrences "url.*B2X" "URLs with B2X"

    # Database
    count_occurrences "database.*B2X" "Database names"
    count_occurrences "schema.*B2X" "Database schemas"

    print_section "4. Security & Certificates"
    count_occurrences "cert.*B2X" "Certificate references"
    count_occurrences "ssl.*B2X" "SSL/TLS references"
    count_occurrences "secret.*B2X" "Secret references"

    print_section "5. Monitoring & Logging"
    count_occurrences "log.*B2X" "Log references"
    count_occurrences "metric.*B2X" "Metrics references"
    count_occurrences "monitor.*B2X" "Monitoring references"

    print_section "6. External Services"
    count_occurrences "webhook.*B2X" "Webhook URLs"
    count_occurrences "callback.*B2X" "Callback URLs"
    count_occurrences "endpoint.*B2X" "External endpoints"

    print_section "7. Development Tools"
    count_occurrences "workspace.*B2X" "IDE workspace references"
    count_occurrences "launch.*B2X" "Debug launch configs"
    count_occurrences "task.*B2X" "Task configurations"

    echo
    print_warning "Review the findings above and update REFACTORING_GAP_ANALYSIS.md"
    echo "Add any discovered patterns to the refactoring scripts."
    echo
    echo "Next steps:"
    echo "1. Review high-count patterns for update strategies"
    echo "2. Check external systems for B2X dependencies"
    echo "3. Update monitoring and alerting configurations"
    echo "4. Coordinate with security team for certificates"
}

main "$@"