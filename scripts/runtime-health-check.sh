#!/bin/bash

# Runtime Health Check Script for B2Connect
# Proof-of-Concept for MonitoringMCP integration in development workflows
# Focus: Backend services (e.g., Store Gateway on port 8000)
# Extended for Heartbeat System with Escalation

set -e

# Configuration
HEARTBEAT_INTERVAL=30  # seconds
SLACK_WEBHOOK_URL="${SLACK_WEBHOOK_URL:-}"  # Set via environment variable
MAX_RESTART_ATTEMPTS=3
RESTART_ATTEMPTS=0

# Parse arguments
HEARTBEAT_MODE=false
while [[ $# -gt 0 ]]; do
    case $1 in
        --heartbeat)
            HEARTBEAT_MODE=true
            shift
            ;;
        --slack-webhook)
            SLACK_WEBHOOK_URL="$2"
            shift 2
            ;;
        --help)
            echo "Usage: $0 [--heartbeat] [--slack-webhook URL]"
            echo "  --heartbeat: Run in continuous heartbeat mode (every 30s)"
            echo "  --slack-webhook: Slack webhook URL for alerts"
            exit 0
            ;;
        *)
            echo "Unknown option: $1"
            exit 1
            ;;
    esac
done

# Change to project root directory
cd "$(dirname "$0")/.."

# Function to send Slack alert
send_slack_alert() {
    local message="$1"
    if [ -n "$SLACK_WEBHOOK_URL" ]; then
        curl -X POST -H 'Content-type: application/json' --data "{\"text\":\"$message\"}" "$SLACK_WEBHOOK_URL" || true
    fi
}

# Function to attempt service restart
attempt_restart() {
    if [ $RESTART_ATTEMPTS -ge $MAX_RESTART_ATTEMPTS ]; then
        echo "❌ Max restart attempts reached. Manual intervention required."
        send_slack_alert "🚨 B2Connect: Max restart attempts reached. Manual intervention required."
        exit 1
    fi

    echo "🔄 Attempting service restart..."
    send_slack_alert "🔄 B2Connect: Attempting service restart (attempt $((RESTART_ATTEMPTS + 1)))"

    # Kill all services
    ./scripts/kill-all-services.sh

    # Start backend services
    echo "Starting backend services..."
    dotnet run --project AppHost/B2Connect.AppHost.csproj &
    BACKEND_PID=$!

    # Wait a bit for startup
    sleep 10

    ((RESTART_ATTEMPTS++))
}

# Function to perform health check
perform_health_check() {
    echo "$(date): Starting runtime health check..."

    # Call MonitoringMCP validate_health_checks for all services
    RESPONSE=$(echo '{"jsonrpc": "2.0", "id": 1, "method": "tools/call", "params": {"name": "validate_health_checks", "arguments": {}}}' | node tools/MonitoringMCP/dist/index.js 2>/dev/null)

    # Check response for errors
    if echo "$RESPONSE" | jq -e '.error' >/dev/null 2>&1; then
        echo "❌ Health check FAILED:"
        echo "$RESPONSE" | jq '.error'
        send_slack_alert "❌ B2Connect Health Check FAILED: $(echo "$RESPONSE" | jq -r '.error.message')"
        attempt_restart
        return 1
    elif echo "$RESPONSE" | jq -e '.result' >/dev/null 2>&1; then
        RESULT=$(echo "$RESPONSE" | jq -r '.result.content[0].text')
        echo "Health check results:"
        echo "$RESULT"
        
        # Check if any service is unhealthy
        if echo "$RESULT" | grep -q "❌ Unhealthy"; then
            echo "❌ Health check FAILED: Some services are unhealthy"
            send_slack_alert "❌ B2Connect Health Check FAILED: Some services are unhealthy\n$RESULT"
            attempt_restart
            return 1
        else
            echo "✅ Health check PASSED: All services are healthy"
            # Reset restart attempts on success
            RESTART_ATTEMPTS=0
            return 0
        fi
    else
        echo "⚠️  Unexpected response format:"
        echo "$RESPONSE"
        send_slack_alert "⚠️ B2Connect: Unexpected health check response format"
        return 1
    fi
}

# Main logic
if [ "$HEARTBEAT_MODE" = true ]; then
    echo "Starting heartbeat mode (interval: ${HEARTBEAT_INTERVAL}s)..."
    send_slack_alert "❤️ B2Connect Heartbeat System started"

    while true; do
        perform_health_check
        sleep $HEARTBEAT_INTERVAL
    done
else
    # Single check mode
    perform_health_check
fi