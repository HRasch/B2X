#!/usr/bin/env bash

# B2Connect Deployment Validation
# Comprehensive validation of deployment health and functionality
#
# Usage: ./scripts/validate-deployment.sh [environment]
# Environment: blue, green, canary (default: current)

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"

ENVIRONMENT="${1:-current}"
TIMEOUT=60

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

log() {
    echo -e "${BLUE}[$(date +'%Y-%m-%d %H:%M:%S')] $1${NC}"
}

error() {
    echo -e "${RED}[ERROR] $1${NC}" >&2
}

success() {
    echo -e "${GREEN}[SUCCESS] $1${NC}"
}

# Get service URL
get_service_url() {
    local service=$1
    local env=$2

    case $env in
        blue)
            echo "http://blue-$service.b2connect.local"
            ;;
        green)
            echo "http://green-$service.b2connect.local"
            ;;
        canary)
            echo "http://canary-$service.b2connect.local"
            ;;
        current)
            echo "http://$service.b2connect.local"
            ;;
        *)
            echo "http://$service.b2connect.local"
            ;;
    esac
}

# Validate Kubernetes resources
validate_kubernetes() {
    local deployment="b2connect-$ENVIRONMENT"
    if [ "$ENVIRONMENT" = "current" ]; then
        deployment="b2connect"
    fi

    log "Validating Kubernetes resources for $deployment..."

    # Check deployment exists and is ready
    if ! kubectl get deployment $deployment -n b2connect >/dev/null 2>&1; then
        error "Deployment $deployment not found"
        return 1
    fi

    # Check rollout status
    if ! kubectl rollout status deployment/$deployment -n b2connect --timeout=60s >/dev/null 2>&1; then
        error "Deployment rollout failed"
        return 1
    fi

    # Check pod status
    local unhealthy_pods=$(kubectl get pods -n b2connect -l app=$deployment --no-headers | grep -v Running | wc -l)
    if [ "$unhealthy_pods" -gt 0 ]; then
        error "Found $unhealthy_pods unhealthy pods"
        kubectl get pods -n b2connect -l app=$deployment
        return 1
    fi

    success "Kubernetes resources validated"
    return 0
}

# Validate service health
validate_health() {
    local service=$1
    local url=$(get_service_url "$service" "$ENVIRONMENT")
    local endpoint="$url/health"

    log "Validating health for $service..."

    local response=$(curl -f -s --max-time $TIMEOUT -w "%{http_code}" "$endpoint" 2>/dev/null || echo "000")
    local status_code="${response: -3}"

    if [ "$status_code" != "200" ]; then
        error "$service health check failed (HTTP $status_code)"
        return 1
    fi

    success "$service health check passed"
    return 0
}

# Validate API functionality
validate_api() {
    local url=$(get_service_url "api-gateway" "$ENVIRONMENT")

    log "Validating API functionality..."

    # Test basic endpoints
    local endpoints=(
        "/api/v1/health"
        "/api/v1/system/info"
        "/api/v1/auth/status"
    )

    for endpoint in "${endpoints[@]}"; do
        local full_url="$url$endpoint"
        local response=$(curl -f -s --max-time $TIMEOUT -w "%{http_code}" "$full_url" 2>/dev/null || echo "000")
        local status_code="${response: -3}"

        if [ "$status_code" != "200" ] && [ "$status_code" != "401" ]; then
            error "API endpoint $endpoint failed (HTTP $status_code)"
            return 1
        fi
    done

    success "API functionality validated"
    return 0
}

# Validate database connectivity
validate_database() {
    local url=$(get_service_url "api-gateway" "$ENVIRONMENT")
    local endpoint="$url/api/v1/system/db-health"

    log "Validating database connectivity..."

    local response=$(curl -f -s --max-time $TIMEOUT -w "%{http_code}" "$endpoint" 2>/dev/null || echo "000")
    local status_code="${response: -3}"

    if [ "$status_code" != "200" ]; then
        error "Database connectivity check failed (HTTP $status_code)"
        return 1
    fi

    success "Database connectivity validated"
    return 0
}

# Validate frontend
validate_frontend() {
    local url=$(get_service_url "frontend" "$ENVIRONMENT")

    log "Validating frontend..."

    # Check HTTP response
    local response=$(curl -f -s --max-time $TIMEOUT -w "%{http_code}" "$url" 2>/dev/null || echo "000")
    local status_code="${response: -3}"

    if [ "$status_code" != "200" ]; then
        error "Frontend check failed (HTTP $status_code)"
        return 1
    fi

    # Check for basic HTML content
    local content=$(curl -s --max-time $TIMEOUT "$url" 2>/dev/null || echo "")
    if [[ ! "$content" =~ "<!DOCTYPE html>" ]]; then
        error "Frontend returned invalid HTML content"
        return 1
    fi

    success "Frontend validated"
    return 0
}

# Validate performance metrics
validate_performance() {
    local service="api-gateway"
    local url=$(get_service_url "$service" "$ENVIRONMENT")
    local endpoint="$url/health"

    log "Validating performance metrics..."

    # Measure response time
    local start_time=$(date +%s%N)
    local response=$(curl -f -s --max-time 10 "$endpoint" 2>/dev/null || echo "")
    local end_time=$(date +%s%N)

    if [ -z "$response" ]; then
        error "Performance test failed - no response"
        return 1
    fi

    local response_time=$(( (end_time - start_time) / 1000000 )) # milliseconds

    if [ "$response_time" -gt 2000 ]; then
        error "Performance test failed - response too slow (${response_time}ms > 2000ms)"
        return 1
    fi

    success "Performance validated (${response_time}ms)"
    return 0
}

# Validate monitoring integration
validate_monitoring() {
    log "Validating monitoring integration..."

    # Check if metrics endpoint is accessible
    local url=$(get_service_url "api-gateway" "$ENVIRONMENT")
    local endpoint="$url/metrics"

    local response=$(curl -f -s --max-time $TIMEOUT -w "%{http_code}" "$endpoint" 2>/dev/null || echo "000")
    local status_code="${response: -3}"

    if [ "$status_code" != "200" ]; then
        warn "Metrics endpoint not accessible (HTTP $status_code) - skipping"
        return 0
    fi

    success "Monitoring integration validated"
    return 0
}

# Main validation function
main() {
    log "Starting deployment validation for environment: $ENVIRONMENT"

    local failed=0

    # Validate infrastructure
    validate_kubernetes || ((failed++))

    # Validate services
    validate_health "api-gateway" || ((failed++))
    validate_health "frontend" || ((failed++))
    validate_api || ((failed++))
    validate_database || ((failed++))
    validate_frontend || ((failed++))
    validate_performance || ((failed++))
    validate_monitoring || ((failed++))

    if [ $failed -eq 0 ]; then
        success "🎉 All validation checks passed!"
        exit 0
    else
        error "❌ $failed validation check(s) failed"
        exit 1
    fi
}

# Run main function
main "$@"