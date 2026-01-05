#!/usr/bin/env bash

# B2Connect Smoke Tests for Deployment Validation
# Runs basic functionality tests against deployed services
#
# Usage: ./scripts/smoke-test.sh [environment]
# Environment: blue, green, canary (default: current)

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"

ENVIRONMENT="${1:-current}"
TIMEOUT=30
RETRIES=3

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m'

log() {
    echo -e "${GREEN}[$(date +'%Y-%m-%d %H:%M:%S')] $1${NC}"
}

error() {
    echo -e "${RED}[ERROR] $1${NC}" >&2
}

warn() {
    echo -e "${YELLOW}[WARN] $1${NC}"
}

# Get service URL based on environment
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

# Test health endpoint
test_health() {
    local service=$1
    local url=$(get_service_url "$service" "$ENVIRONMENT")
    local endpoint="$url/health"

    log "Testing health endpoint: $endpoint"

    for i in $(seq 1 $RETRIES); do
        if curl -f -s --max-time $TIMEOUT "$endpoint" > /dev/null 2>&1; then
            log "✓ $service health check passed"
            return 0
        fi
        warn "Health check attempt $i failed for $service, retrying..."
        sleep 2
    done

    error "✗ $service health check failed after $RETRIES attempts"
    return 1
}

# Test API gateway
test_api_gateway() {
    local url=$(get_service_url "api-gateway" "$ENVIRONMENT")
    local endpoint="$url/api/v1/health"

    log "Testing API gateway: $endpoint"

    for i in $(seq 1 $RETRIES); do
        if curl -f -s --max-time $TIMEOUT "$endpoint" > /dev/null 2>&1; then
            log "✓ API Gateway check passed"
            return 0
        fi
        warn "API Gateway check attempt $i failed, retrying..."
        sleep 2
    done

    error "✗ API Gateway check failed after $RETRIES attempts"
    return 1
}

# Test database connectivity
test_database() {
    local url=$(get_service_url "api-gateway" "$ENVIRONMENT")
    local endpoint="$url/api/v1/system/db-health"

    log "Testing database connectivity: $endpoint"

    for i in $(seq 1 $RETRIES); do
        if curl -f -s --max-time $TIMEOUT "$endpoint" > /dev/null 2>&1; then
            log "✓ Database connectivity check passed"
            return 0
        fi
        warn "Database check attempt $i failed, retrying..."
        sleep 2
    done

    error "✗ Database connectivity check failed after $RETRIES attempts"
    return 1
}

# Test frontend
test_frontend() {
    local url=$(get_service_url "frontend" "$ENVIRONMENT")

    log "Testing frontend: $url"

    for i in $(seq 1 $RETRIES); do
        if curl -f -s --max-time $TIMEOUT -I "$url" | grep -q "200 OK"; then
            log "✓ Frontend check passed"
            return 0
        fi
        warn "Frontend check attempt $i failed, retrying..."
        sleep 2
    done

    error "✗ Frontend check failed after $RETRIES attempts"
    return 1
}

# Test authentication flow (smoke test)
test_auth_smoke() {
    local url=$(get_service_url "api-gateway" "$ENVIRONMENT")
    local endpoint="$url/api/v1/auth/status"

    log "Testing auth smoke test: $endpoint"

    for i in $(seq 1 $RETRIES); do
        if curl -f -s --max-time $TIMEOUT "$endpoint" > /dev/null 2>&1; then
            log "✓ Auth smoke test passed"
            return 0
        fi
        warn "Auth smoke test attempt $i failed, retrying..."
        sleep 2
    done

    error "✗ Auth smoke test failed after $RETRIES attempts"
    return 1
}

# Main smoke test execution
main() {
    log "Starting smoke tests for environment: $ENVIRONMENT"

    local failed=0

    # Test core services
    test_api_gateway || ((failed++))
    test_health "api-gateway" || ((failed++))
    test_database || ((failed++))
    test_frontend || ((failed++))

    # Test key business services
    test_auth_smoke || ((failed++))

    if [ $failed -eq 0 ]; then
        log "🎉 All smoke tests passed!"
        exit 0
    else
        error "❌ $failed smoke test(s) failed"
        exit 1
    fi
}

# Run main function
main "$@"