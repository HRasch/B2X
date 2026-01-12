#!/bin/bash
# check-external-deps.sh
# Check for external dependencies and integrations that may be affected

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
    echo -e "${BLUE}  External Dependencies Check${NC}"
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

# Function to check for patterns in specific contexts
check_context() {
    local pattern="$1"
    local context="$2"
    local risk="$3"

    echo -n "Checking $context... "
    local count=$(grep -r "$pattern" --include="*" --exclude-dir=".git" --exclude-dir="node_modules" --exclude-dir="bin" --exclude-dir="obj" . 2>/dev/null | wc -l)

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

        echo "Locations:"
        grep -r "$pattern" --include="*" --exclude-dir=".git" --exclude-dir="node_modules" --exclude-dir="bin" --exclude-dir="obj" . 2>/dev/null | head -5
        if [ "$count" -gt 5 ]; then
            echo "... and $(($count - 5)) more"
        fi
        echo
    else
        echo -e "${GREEN}none found${NC}"
    fi
}

# Function to check URLs and endpoints
check_urls() {
    print_section "Checking URLs and API Endpoints"

    check_context "https*://.*B2X" "External URLs" "HIGH"
    check_context "api\.B2X" "API endpoints" "HIGH"
    check_context "webhook.*B2X" "Webhook URLs" "HIGH"
    check_context "callback.*B2X" "Callback URLs" "MEDIUM"
    check_context "\.B2X\." "Domain references" "HIGH"
}

# Function to check database configurations
check_database() {
    print_section "Checking Database Configurations"

    check_context "Server=.*B2X" "SQL Server connections" "HIGH"
    check_context "Database=.*B2X" "Database names" "HIGH"
    check_context "schema.*B2X" "Schema references" "MEDIUM"
    check_context "table.*B2X" "Table prefixes" "LOW"
}

# Function to check container and orchestration
check_containers() {
    print_section "Checking Container & Orchestration"

    check_context "image:.*B2X" "Docker images" "HIGH"
    check_context "container_name:.*B2X" "Container names" "HIGH"
    check_context "service:.*B2X" "Kubernetes services" "HIGH"
    check_context "namespace:.*B2X" "Kubernetes namespaces" "HIGH"
    check_context "deployment:.*B2X" "Kubernetes deployments" "MEDIUM"
}

# Function to check monitoring and logging
check_monitoring() {
    print_section "Checking Monitoring & Logging"

    check_context "service:.*B2X" "APM service names" "MEDIUM"
    check_context "index.*B2X" "Log indices" "MEDIUM"
    check_context "metric.*B2X" "Metrics names" "LOW"
    check_context "alert.*B2X" "Alert rule names" "LOW"
    check_context "dashboard.*B2X" "Dashboard names" "LOW"
}

# Function to check security configurations
check_security() {
    print_section "Checking Security Configurations"

    check_context "cert.*B2X" "Certificate references" "HIGH"
    check_context "ssl.*B2X" "SSL configurations" "HIGH"
    check_context "secret.*B2X" "Secret references" "HIGH"
    check_context "key.*B2X" "Key references" "MEDIUM"
    check_context "vault.*B2X" "Vault paths" "HIGH"
}

# Function to check CI/CD configurations
check_cicd() {
    print_section "Checking CI/CD Configurations"

    check_context "workflow.*B2X" "GitHub Actions workflows" "MEDIUM"
    check_context "job.*B2X" "CI/CD job names" "LOW"
    check_context "artifact.*B2X" "Build artifacts" "MEDIUM"
    check_context "registry.*B2X" "Container registries" "HIGH"
}

# Function to check development tools
check_devtools() {
    print_section "Checking Development Tools"

    check_context "workspace.*B2X" "IDE workspace names" "LOW"
    check_context "launch.*B2X" "Debug configurations" "LOW"
    check_context "task.*B2X" "Task definitions" "LOW"
    check_context "extension.*B2X" "Extension settings" "LOW"
}

# Function to check package configurations
check_packages() {
    print_section "Checking Package Configurations"

    check_context "\"name\":.*B2X" "npm package names" "HIGH"
    check_context "<PackageId>.*B2X" "NuGet package IDs" "HIGH"
    check_context "description.*B2X" "Package descriptions" "LOW"
    check_context "registry.*B2X" "Package registries" "MEDIUM"
}

# Function to generate summary report
generate_report() {
    print_section "Summary Report"

    echo "This check has identified potential external dependencies that may be affected"
    echo "by the B2X project refactoring. Review each HIGH RISK item carefully."
    echo
    echo "Next steps:"
    echo "1. Update external service configurations"
    echo "2. Coordinate with third-party providers"
    echo "3. Update monitoring and alerting systems"
    echo "4. Test all external integrations"
    echo "5. Update security configurations"
    echo
    print_warning "Do not proceed with refactoring until all HIGH RISK items are addressed!"
}

main() {
    print_header
    echo "Checking for external dependencies and integrations..."
    echo "This may take a few minutes depending on codebase size."
    echo

    check_urls
    check_database
    check_containers
    check_monitoring
    check_security
    check_cicd
    check_devtools
    check_packages

    generate_report
}

main "$@"