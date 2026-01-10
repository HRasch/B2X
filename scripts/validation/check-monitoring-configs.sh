#!/bin/bash
# check-monitoring-configs.sh
# Check monitoring, logging, and observability configurations for refactoring impact

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
    echo -e "${BLUE}  Monitoring Configuration Audit${NC}"
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

# Function to check appsettings.json files for monitoring configs
check_appsettings_monitoring() {
    print_section "Checking Application Settings for Monitoring"

    local appsettings_files=$(find . -name "appsettings*.json" -not -path "./.git/*" -not -path "./node_modules/*")

    for file in $appsettings_files; do
        echo "Checking $file..."

        # Check for logging configuration
        local logging_config=$(grep -A 10 '"Logging"' "$file")

        if [ -n "$logging_config" ]; then
            echo "Found logging configuration in $file"
        fi

        # Check for Application Insights or other monitoring
        local monitoring_config=$(grep -i -E "(applicationinsights|appinsights|monitoring|telemetry)" "$file")

        if [ -n "$monitoring_config" ]; then
            print_warning "Found monitoring configuration in $file:"
            echo "$monitoring_config"
            echo
        fi

        # Check for hardcoded paths in connection strings
        local conn_strings=$(grep -A 5 '"ConnectionStrings"' "$file" | grep -E "(AppHost|Backend|Frontend)")

        if [ -n "$conn_strings" ]; then
            print_warning "Found hardcoded paths in connection strings in $file:"
            echo "$conn_strings"
            echo
        fi
    done
}

# Function to check for log file paths
check_log_paths() {
    print_section "Checking Log File Paths"

    # Look for log directory references
    local log_refs=$(grep -r -i "logs" --include="*.json" --include="*.config" --include="*.yml" --include="*.yaml" --exclude-dir=".git" --exclude-dir="node_modules" . 2>/dev/null)

    if [ -n "$log_refs" ]; then
        echo "Found log directory references:"
        echo "$log_refs" | head -10
        echo
    fi

    # Check for hardcoded log paths
    local hardcoded_logs=$(grep -r "/logs" --include="*.sh" --include="*.ps1" --include="*.json" --include="*.yml" --include="*.yaml" --exclude-dir=".git" --exclude-dir="node_modules" . 2>/dev/null)

    if [ -n "$hardcoded_logs" ]; then
        print_warning "Found hardcoded log paths:"
        echo "$hardcoded_logs"
        echo
    fi
}

# Function to check health check endpoints
check_health_checks() {
    print_section "Checking Health Check Configurations"

    # Look for health check endpoints in code
    local health_checks=$(grep -r -i "health" --include="*.cs" --include="*.ts" --include="*.js" --exclude-dir=".git" --exclude-dir="node_modules" --exclude-dir="bin" --exclude-dir="obj" . 2>/dev/null | grep -i -E "(endpoint|route|path)")

    if [ -n "$health_checks" ]; then
        echo "Found health check endpoint references:"
        echo "$health_checks" | head -5
        echo
    fi

    # Check for health check middleware configuration
    local health_middleware=$(grep -r "UseHealthChecks" --include="*.cs" --exclude-dir=".git" --exclude-dir="node_modules" --exclude-dir="bin" --exclude-dir="obj" . 2>/dev/null)

    if [ -n "$health_middleware" ]; then
        echo "Found health check middleware usage:"
        echo "$health_middleware"
        echo
    fi
}

# Function to check metrics and telemetry
check_metrics_telemetry() {
    print_section "Checking Metrics and Telemetry"

    # Look for metrics collection
    local metrics=$(grep -r -i -E "(metrics|telemetry|prometheus|grafana)" --include="*.cs" --include="*.json" --include="*.yml" --include="*.yaml" --exclude-dir=".git" --exclude-dir="node_modules" . 2>/dev/null)

    if [ -n "$metrics" ]; then
        echo "Found metrics/telemetry configuration:"
        echo "$metrics" | head -10
        echo
    fi

    # Check for Application Insights setup
    local app_insights=$(grep -r -i "applicationinsights" --include="*.cs" --include="*.json" --include="*.config" --exclude-dir=".git" --exclude-dir="node_modules" . 2>/dev/null)

    if [ -n "$app_insights" ]; then
        print_warning "Found Application Insights configuration:"
        echo "$app_insights"
        echo
    fi
}

# Function to check for monitoring dashboards
check_dashboards() {
    print_section "Checking Monitoring Dashboards"

    # Look for dashboard configurations
    local dashboards=$(grep -r -i -E "(dashboard|grafana|kibana)" --include="*.json" --include="*.yml" --include="*.yaml" --include="*.md" --exclude-dir=".git" --exclude-dir="node_modules" . 2>/dev/null)

    if [ -n "$dashboards" ]; then
        echo "Found dashboard references:"
        echo "$dashboards" | head -5
        echo
    fi
}

# Function to check for alerting configurations
check_alerting() {
    print_section "Checking Alerting Configurations"

    # Look for alert configurations
    local alerts=$(grep -r -i -E "(alert|notification|webhook)" --include="*.json" --include="*.yml" --include="*.yaml" --exclude-dir=".git" --exclude-dir="node_modules" . 2>/dev/null | grep -v "package")

    if [ -n "$alerts" ]; then
        echo "Found alerting configuration:"
        echo "$alerts" | head -5
        echo
    fi
}

# Function to check for APM/tracing
check_apm_tracing() {
    print_section "Checking APM and Tracing"

    # Look for tracing configurations
    local tracing=$(grep -r -i -E "(tracing|opentelemetry|jaeger|zipkin)" --include="*.cs" --include="*.json" --include="*.yml" --include="*.yaml" --exclude-dir=".git" --exclude-dir="node_modules" . 2>/dev/null)

    if [ -n "$tracing" ]; then
        echo "Found tracing configuration:"
        echo "$tracing" | head -5
        echo
    fi
}

# Function to check for log aggregation
check_log_aggregation() {
    print_section "Checking Log Aggregation"

    # Look for ELK stack, Splunk, etc.
    local aggregation=$(grep -r -i -E "(elasticsearch|logstash|kibana|splunk|datadog|newrelic)" --include="*.json" --include="*.yml" --include="*.yaml" --include="*.config" --exclude-dir=".git" --exclude-dir="node_modules" . 2>/dev/null)

    if [ -n "$aggregation" ]; then
        echo "Found log aggregation configuration:"
        echo "$aggregation" | head -5
        echo
    fi
}

# Function to generate monitoring impact report
generate_monitoring_report() {
    print_section "Monitoring Impact Summary"

    echo "Key findings for monitoring during refactoring:"
    echo
    echo "1. **Application Settings**: Update appsettings.json files with new paths"
    echo "2. **Log Paths**: Update log directory references in configurations"
    echo "3. **Health Checks**: Verify health check endpoints still work"
    echo "4. **Metrics Collection**: Ensure metrics collection continues after moves"
    echo "5. **Dashboards**: Update dashboard configurations if they reference specific paths"
    echo "6. **Alerting**: Test alerting rules after refactoring"
    echo "7. **Tracing**: Verify distributed tracing configuration"
    echo "8. **Log Aggregation**: Update log shipper configurations"
    echo
    print_warning "Monitor application behavior closely during and after refactoring!"
    echo
    echo "Recommended monitoring during refactoring:"
    echo "- Set up additional logging for path resolution"
    echo "- Monitor for increased error rates"
    echo "- Watch health check endpoints"
    echo "- Check metrics collection continuity"
}

main() {
    print_header
    echo "Auditing monitoring and logging configurations..."
    echo

    check_appsettings_monitoring
    check_log_paths
    check_health_checks
    check_metrics_telemetry
    check_dashboards
    check_alerting
    check_apm_tracing
    check_log_aggregation

    generate_monitoring_report
}

main "$@"