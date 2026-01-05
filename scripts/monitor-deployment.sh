#!/usr/bin/env bash

# B2Connect Deployment Monitor
# Monitors deployment health and performance metrics
#
# Usage: ./scripts/monitor-deployment.sh [environment] [duration]
# Environment: blue, green, canary (default: current)
# Duration: monitoring duration in seconds (default: 300)

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"

ENVIRONMENT="${1:-current}"
DURATION="${2:-300}"
INTERVAL=10

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

# Check response time
check_response_time() {
    local service=$1
    local url=$(get_service_url "$service" "$ENVIRONMENT")
    local endpoint="$url/health"

    local start_time=$(date +%s%N)
    if curl -f -s --max-time 5 "$endpoint" > /dev/null 2>&1; then
        local end_time=$(date +%s%N)
        local response_time=$(( (end_time - start_time) / 1000000 )) # Convert to milliseconds
        echo "$response_time"
        return 0
    else
        echo "-1"
        return 1
    fi
}

# Check error rate
check_error_rate() {
    local service=$1
    local url=$(get_service_url "$service" "$ENVIRONMENT")
    local endpoint="$url/metrics"

    # This would integrate with actual metrics endpoint
    # For now, simulate by checking health
    if curl -f -s --max-time 5 "$endpoint" > /dev/null 2>&1; then
        echo "0.00"
    else
        echo "100.00"
    fi
}

# Check CPU and memory usage via Kubernetes metrics
check_resource_usage() {
    local deployment="b2connect-$ENVIRONMENT"
    if [ "$ENVIRONMENT" = "current" ]; then
        deployment="b2connect"
    fi

    # Get pod resource usage
    kubectl top pods -n b2connect -l app=$deployment --no-headers 2>/dev/null || echo "N/A N/A"
}

# Monitor single service
monitor_service() {
    local service=$1
    local count=0
    local max_count=$((DURATION / INTERVAL))

    log "Monitoring $service for $DURATION seconds..."

    while [ $count -lt $max_count ]; do
        local response_time=$(check_response_time "$service")
        local error_rate=$(check_error_rate "$service")

        if [ "$response_time" = "-1" ]; then
            warn "$service: Unhealthy (no response)"
        else
            if [ "$response_time" -gt 1000 ]; then
                warn "$service: Slow response (${response_time}ms)"
            else
                log "$service: Healthy (${response_time}ms)"
            fi
        fi

        if [ "$(echo "$error_rate > 5.0" | bc -l 2>/dev/null)" = "1" ]; then
            warn "$service: High error rate (${error_rate}%)"
        fi

        sleep $INTERVAL
        ((count++))
    done
}

# Monitor resources
monitor_resources() {
    log "Monitoring resource usage..."

    local count=0
    local max_count=$((DURATION / INTERVAL))

    while [ $count -lt $max_count ]; do
        log "Resource usage check:"
        check_resource_usage

        sleep $INTERVAL
        ((count++))
    done
}

# Check deployment rollout status
check_rollout_status() {
    local deployment="b2connect-$ENVIRONMENT"
    if [ "$ENVIRONMENT" = "current" ]; then
        deployment="b2connect"
    fi

    kubectl rollout status deployment/$deployment -n b2connect --timeout=30s
}

# Main monitoring function
main() {
    log "Starting deployment monitoring for environment: $ENVIRONMENT"

    # Check initial rollout status
    log "Checking rollout status..."
    if ! check_rollout_status; then
        error "Deployment rollout failed"
        exit 1
    fi

    # Start monitoring in background
    monitor_service "api-gateway" &
    local gateway_pid=$!

    monitor_service "frontend" &
    local frontend_pid=$!

    monitor_resources &
    local resources_pid=$!

    # Wait for monitoring duration
    sleep $DURATION

    # Kill background processes
    kill $gateway_pid $frontend_pid $resources_pid 2>/dev/null || true

    log "Monitoring completed"
}

# Run main function
main "$@"