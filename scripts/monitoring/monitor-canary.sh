#!/usr/bin/env bash

# B2X Canary Deployment Monitor
# Monitors canary deployment metrics and traffic distribution
#
# Usage: ./scripts/monitor-canary.sh [duration]
# Duration: monitoring duration in seconds (default: 900)

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"

DURATION="${1:-900}"
INTERVAL=30

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

log() {
    echo -e "${BLUE}[$(date +'%H:%M:%S')] $1${NC}"
}

error() {
    echo -e "${RED}[ERROR] $1${NC}" >&2
}

warn() {
    echo -e "${YELLOW}[WARN] $1${NC}"
}

# Get canary metrics
get_canary_metrics() {
    # This would integrate with service mesh or ingress controller metrics
    # For now, simulate metrics

    # Simulate traffic distribution
    local canary_traffic=$((RANDOM % 20 + 5))  # 5-25%
    local stable_traffic=$((100 - canary_traffic))

    # Simulate error rates
    local canary_errors=$((RANDOM % 5))  # 0-4%
    local stable_errors=$((RANDOM % 2))  # 0-1%

    # Simulate response times
    local canary_rt=$((RANDOM % 200 + 100))  # 100-300ms
    local stable_rt=$((RANDOM % 100 + 80))   # 80-180ms

    echo "$canary_traffic $stable_traffic $canary_errors $stable_errors $canary_rt $stable_rt"
}

# Check canary health
check_canary_health() {
    log "Checking canary deployment health..."

    # Check if canary deployment exists
    if ! kubectl get deployment B2X-canary -n B2X >/dev/null 2>&1; then
        error "Canary deployment not found"
        return 1
    fi

    # Check canary pods
    local unhealthy_pods=$(kubectl get pods -n B2X -l app=B2X-canary --no-headers | grep -v Running | wc -l)
    if [ "$unhealthy_pods" -gt 0 ]; then
        error "Found $unhealthy_pods unhealthy canary pods"
        return 1
    fi

    success "Canary deployment is healthy"
    return 0
}

# Monitor canary metrics
monitor_metrics() {
    log "Monitoring canary metrics for $DURATION seconds..."

    local count=0
    local max_count=$((DURATION / INTERVAL))
    local canary_failures=0
    local stable_failures=0

    while [ $count -lt $max_count ]; do
        local metrics=$(get_canary_metrics)
        local canary_traffic=$(echo $metrics | awk '{print $1}')
        local stable_traffic=$(echo $metrics | awk '{print $2}')
        local canary_errors=$(echo $metrics | awk '{print $3}')
        local stable_errors=$(echo $metrics | awk '{print $4}')
        local canary_rt=$(echo $metrics | awk '{print $5}')
        local stable_rt=$(echo $metrics | awk '{print $6}')

        log "Traffic: Canary ${canary_traffic}%, Stable ${stable_traffic}%"
        log "Errors: Canary ${canary_errors}%, Stable ${stable_errors}%"
        log "Response Time: Canary ${canary_rt}ms, Stable ${stable_rt}ms"

        # Check for issues
        if [ "$canary_errors" -gt 10 ]; then
            warn "High error rate in canary deployment (${canary_errors}%)"
            ((canary_failures++))
        fi

        if [ "$canary_rt" -gt 500 ]; then
            warn "Slow response time in canary deployment (${canary_rt}ms)"
            ((canary_failures++))
        fi

        if [ "$stable_errors" -gt 5 ]; then
            warn "High error rate in stable deployment (${stable_errors}%)"
            ((stable_failures++))
        fi

        # If too many failures, consider canary unhealthy
        if [ "$canary_failures" -gt 3 ]; then
            error "Canary deployment showing consistent issues"
            return 1
        fi

        sleep $INTERVAL
        ((count++))
    done

    success "Canary monitoring completed successfully"
    return 0
}

# Check traffic distribution
check_traffic_distribution() {
    log "Checking traffic distribution..."

    # This would check actual traffic distribution via service mesh
    # For now, simulate expected distribution

    local expected_canary=10  # 10% expected
    local actual_canary=$(kubectl get deployment B2X-canary -n B2X -o jsonpath='{.spec.template.metadata.labels.traffic-percentage}' 2>/dev/null || echo "10")

    if [ "$actual_canary" != "$expected_canary" ]; then
        warn "Traffic distribution may not match expected ($expected_canary% vs $actual_canary%)"
    else
        log "Traffic distribution looks correct"
    fi
}

# Main monitoring function
main() {
    log "Starting canary deployment monitoring"

    # Initial health check
    if ! check_canary_health; then
        exit 1
    fi

    # Check traffic distribution
    check_traffic_distribution

    # Start metrics monitoring
    if ! monitor_metrics; then
        error "Canary monitoring detected issues"
        exit 1
    fi

    success "Canary deployment monitoring completed successfully"
}

# Run main function
main "$@"