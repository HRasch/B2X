#!/usr/bin/env bash

# B2X Rollback Validation
# Validates rollback deployment and ensures system stability
#
# Usage: ./scripts/validate-rollback.sh

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"

TIMEOUT=120
RETRIES=5

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

warn() {
    echo -e "${YELLOW}[WARN] $1${NC}"
}

# Validate rollback deployment
validate_rollback_deployment() {
    log "Validating rollback deployment..."

    # Check if stable deployment exists
    if ! kubectl get deployment B2X -n B2X >/dev/null 2>&1; then
        error "Stable deployment not found after rollback"
        return 1
    fi

    # Check rollout status
    if ! kubectl rollout status deployment/B2X -n B2X --timeout=120s >/dev/null 2>&1; then
        error "Rollback deployment rollout failed"
        return 1
    fi

    # Check pod health
    local unhealthy_pods=$(kubectl get pods -n B2X -l app=B2X --no-headers | grep -v Running | wc -l)
    if [ "$unhealthy_pods" -gt 0 ]; then
        error "Found $unhealthy_pods unhealthy pods after rollback"
        kubectl get pods -n B2X -l app=B2X
        return 1
    fi

    success "Rollback deployment validated"
    return 0
}

# Validate service endpoints
validate_service_endpoints() {
    log "Validating service endpoints after rollback..."

    local services=("api-gateway" "frontend")
    local failed=0

    for service in "${services[@]}"; do
        local url="http://$service.B2X.local/health"

        for i in $(seq 1 $RETRIES); do
            if curl -f -s --max-time $TIMEOUT "$url" > /dev/null 2>&1; then
                log "✓ $service endpoint accessible"
                break
            else
                if [ $i -eq $RETRIES ]; then
                    error "✗ $service endpoint failed after $RETRIES attempts"
                    ((failed++))
                else
                    warn "$service endpoint attempt $i failed, retrying..."
                    sleep 5
                fi
            fi
        done
    done

    if [ $failed -gt 0 ]; then
        return 1
    fi

    return 0
}

# Validate data consistency
validate_data_consistency() {
    log "Validating data consistency after rollback..."

    # This would check database consistency, cache invalidation, etc.
    # For now, perform basic connectivity check

    local url="http://api-gateway.B2X.local/api/v1/system/db-health"

    for i in $(seq 1 $RETRIES); do
        local response=$(curl -f -s --max-time $TIMEOUT -w "%{http_code}" "$url" 2>/dev/null || echo "000")
        local status_code="${response: -3}"

        if [ "$status_code" = "200" ]; then
            success "Database connectivity validated"
            return 0
        else
            if [ $i -eq $RETRIES ]; then
                error "Database connectivity check failed after $RETRIES attempts"
                return 1
            else
                warn "Database check attempt $i failed, retrying..."
                sleep 5
            fi
        fi
    done
}

# Validate performance after rollback
validate_performance() {
    log "Validating performance after rollback..."

    local service="api-gateway"
    local url="http://$service.B2X.local/health"

    # Measure response times
    local total_time=0
    local samples=5

    for i in $(seq 1 $samples); do
        local start_time=$(date +%s%N)
        if curl -f -s --max-time 10 "$url" > /dev/null 2>&1; then
            local end_time=$(date +%s%N)
            local response_time=$(( (end_time - start_time) / 1000000 ))
            total_time=$((total_time + response_time))
        else
            error "Performance test failed on sample $i"
            return 1
        fi
        sleep 1
    done

    local avg_response_time=$((total_time / samples))

    if [ "$avg_response_time" -gt 1000 ]; then
        error "Average response time too high after rollback (${avg_response_time}ms > 1000ms)"
        return 1
    fi

    success "Performance validated (avg: ${avg_response_time}ms)"
    return 0
}

# Check for rollback artifacts cleanup
validate_cleanup() {
    log "Validating rollback cleanup..."

    # Check for leftover canary deployments
    if kubectl get deployment B2X-canary -n B2X >/dev/null 2>&1; then
        warn "Canary deployment still exists, should be cleaned up"
    fi

    # Check for failed/old pods
    local failed_pods=$(kubectl get pods -n B2X --field-selector=status.phase!=Running,status.phase!=Succeeded --no-headers | wc -l)
    if [ "$failed_pods" -gt 0 ]; then
        warn "Found $failed_pods failed/old pods that should be cleaned up"
        kubectl get pods -n B2X --field-selector=status.phase!=Running,status.phase!=Succeeded
    fi

    success "Cleanup validation completed"
    return 0
}

# Send rollback notification
send_notification() {
    local success=$1

    log "Sending rollback validation notification..."

    # This would integrate with notification system (Slack, Teams, etc.)
    if [ "$success" = "true" ]; then
        echo "✅ Rollback validation successful"
    else
        echo "❌ Rollback validation failed"
    fi
}

# Main validation function
main() {
    log "Starting rollback validation"

    local validation_success=true

    # Validate deployment
    if ! validate_rollback_deployment; then
        validation_success=false
    fi

    # Validate service endpoints
    if ! validate_service_endpoints; then
        validation_success=false
    fi

    # Validate data consistency
    if ! validate_data_consistency; then
        validation_success=false
    fi

    # Validate performance
    if ! validate_performance; then
        validation_success=false
    fi

    # Validate cleanup
    validate_cleanup

    # Send notification
    send_notification "$validation_success"

    if [ "$validation_success" = "true" ]; then
        success "🎉 Rollback validation completed successfully!"
        exit 0
    else
        error "❌ Rollback validation failed"
        exit 1
    fi
}

# Run main function
main "$@"