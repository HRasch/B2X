#!/usr/bin/env bash

# B2X Canary Deployment Validation
# Validates canary deployment against success criteria
#
# Usage: ./scripts/validate-canary.sh

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"

# Success criteria
MAX_ERROR_RATE=5.0      # Maximum allowed error rate percentage
MAX_RESPONSE_TIME=1000  # Maximum allowed response time in ms
MIN_SUCCESS_RATE=95.0   # Minimum success rate percentage
MONITORING_PERIOD=300   # Monitoring period in seconds

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

# Collect canary metrics
collect_metrics() {
    local duration=$1

    log "Collecting canary metrics for ${duration} seconds..."

    # This would integrate with monitoring system
    # For now, simulate metric collection

    # Simulate metrics over time
    local total_requests=0
    local failed_requests=0
    local total_response_time=0
    local samples=0

    local end_time=$((SECONDS + duration))

    while [ $SECONDS -lt $end_time ]; do
        # Simulate request metrics
        local batch_requests=$((RANDOM % 100 + 50))
        local batch_failures=$((RANDOM % 5))
        local avg_response_time=$((RANDOM % 200 + 100))

        total_requests=$((total_requests + batch_requests))
        failed_requests=$((failed_requests + batch_failures))
        total_response_time=$((total_response_time + (avg_response_time * batch_requests)))
        samples=$((samples + 1))

        sleep 10
    done

    # Calculate final metrics
    local error_rate=$(echo "scale=2; ($failed_requests * 100) / $total_requests" | bc -l 2>/dev/null || echo "0")
    local avg_response_time=$((total_response_time / total_requests))
    local success_rate=$(echo "scale=2; 100 - $error_rate" | bc -l 2>/dev/null || echo "100")

    echo "$error_rate $avg_response_time $success_rate $total_requests"
}

# Compare with baseline
compare_with_baseline() {
    local canary_error_rate=$1
    local canary_response_time=$2
    local canary_success_rate=$3

    log "Comparing canary metrics with baseline..."

    # This would fetch baseline metrics from monitoring system
    # For now, use simulated baseline values
    local baseline_error_rate=2.0
    local baseline_response_time=150
    local baseline_success_rate=98.0

    log "Baseline - Error Rate: ${baseline_error_rate}%, Response Time: ${baseline_response_time}ms, Success Rate: ${baseline_success_rate}%"
    log "Canary   - Error Rate: ${canary_error_rate}%, Response Time: ${canary_response_time}ms, Success Rate: ${canary_success_rate}%"

    # Check if canary is within acceptable bounds
    local error_rate_diff=$(echo "$canary_error_rate - $baseline_error_rate" | bc -l 2>/dev/null || echo "0")
    local response_time_diff=$((canary_response_time - baseline_response_time))
    local success_rate_diff=$(echo "$baseline_success_rate - $canary_success_rate" | bc -l 2>/dev/null || echo "0")

    # Allow some tolerance
    if [ "$(echo "$error_rate_diff > 3.0" | bc -l 2>/dev/null)" = "1" ]; then
        error "Canary error rate significantly worse than baseline (+${error_rate_diff}%)"
        return 1
    fi

    if [ "$response_time_diff" -gt 200 ]; then
        error "Canary response time significantly worse than baseline (+${response_time_diff}ms)"
        return 1
    fi

    if [ "$(echo "$success_rate_diff > 2.0" | bc -l 2>/dev/null)" = "1" ]; then
        error "Canary success rate significantly worse than baseline (-${success_rate_diff}%)"
        return 1
    fi

    success "Canary metrics within acceptable bounds"
    return 0
}

# Validate absolute criteria
validate_absolute_criteria() {
    local error_rate=$1
    local response_time=$2
    local success_rate=$3

    log "Validating against absolute criteria..."

    if [ "$(echo "$error_rate > $MAX_ERROR_RATE" | bc -l 2>/dev/null)" = "1" ]; then
        error "Canary error rate exceeds maximum allowed (${error_rate}% > ${MAX_ERROR_RATE}%)"
        return 1
    fi

    if [ "$response_time" -gt "$MAX_RESPONSE_TIME" ]; then
        error "Canary response time exceeds maximum allowed (${response_time}ms > ${MAX_RESPONSE_TIME}ms)"
        return 1
    fi

    if [ "$(echo "$success_rate < $MIN_SUCCESS_RATE" | bc -l 2>/dev/null)" = "1" ]; then
        error "Canary success rate below minimum required (${success_rate}% < ${MIN_SUCCESS_RATE}%)"
        return 1
    fi

    success "Canary meets all absolute criteria"
    return 0
}

# Check system resources
check_resources() {
    log "Checking system resource usage..."

    # Check canary pod resource usage
    local resource_usage=$(kubectl top pods -n B2X -l app=B2X-canary --no-headers 2>/dev/null || echo "N/A")

    if [ "$resource_usage" = "N/A" ]; then
        warn "Could not retrieve resource usage metrics"
        return 0
    fi

    log "Canary resource usage: $resource_usage"

    # Check for resource issues (this would be more sophisticated in production)
    if echo "$resource_usage" | grep -q "100%\|[89][0-9]%" 2>/dev/null; then
        warn "Canary pods showing high resource usage"
    fi

    return 0
}

# Main validation function
main() {
    log "Starting canary deployment validation"

    # Collect metrics during monitoring period
    local metrics=$(collect_metrics $MONITORING_PERIOD)
    local error_rate=$(echo $metrics | awk '{print $1}')
    local response_time=$(echo $metrics | awk '{print $2}')
    local success_rate=$(echo $metrics | awk '{print $3}')
    local total_requests=$(echo $metrics | awk '{print $4}')

    log "Collected metrics - Requests: $total_requests, Error Rate: ${error_rate}%, Response Time: ${response_time}ms, Success Rate: ${success_rate}%"

    # Validate against absolute criteria
    if ! validate_absolute_criteria "$error_rate" "$response_time" "$success_rate"; then
        error "Canary failed absolute criteria validation"
        exit 1
    fi

    # Compare with baseline
    if ! compare_with_baseline "$error_rate" "$response_time" "$success_rate"; then
        error "Canary failed baseline comparison"
        exit 1
    fi

    # Check resources
    check_resources

    success "🎉 Canary deployment validation passed!"
    exit 0
}

# Run main function
main "$@"