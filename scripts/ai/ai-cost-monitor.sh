#!/bin/bash

# AI Cost Monitoring and Optimization Script
# Tracks GitHub Copilot usage, monitors budgets, and provides optimization recommendations
# Part of the Agent Escalation System for cost-effective AI usage

set -euo pipefail

# Configuration
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"
LOG_DIR="$PROJECT_ROOT/logs"
AI_LOG_DIR="$LOG_DIR/ai-usage"
CONFIG_FILE="$PROJECT_ROOT/.ai/config/ai-budget.json"

# Default configuration
DEFAULT_BUDGET_MONTHLY=500  # $500 monthly budget
DEFAULT_BUDGET_DAILY=15     # $15 daily budget
ALERT_THRESHOLD_80=80       # Alert at 80% usage
ALERT_THRESHOLD_95=95       # Critical alert at 95% usage

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Logging functions
log_info() {
    echo -e "${BLUE}[INFO]${NC} $(date '+%Y-%m-%d %H:%M:%S') - $1" >&2
}

log_warn() {
    echo -e "${YELLOW}[WARN]${NC} $(date '+%Y-%m-%d %H:%M:%S') - $1" >&2
}

log_error() {
    echo -e "${RED}[ERROR]${NC} $(date '+%Y-%m-%d %H:%M:%S') - $1" >&2
}

log_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $(date '+%Y-%m-%d %H:%M:%S') - $1" >&2
}

# Ensure directories exist
ensure_directories() {
    mkdir -p "$AI_LOG_DIR"
    mkdir -p "$LOG_DIR"
}

# Initialize configuration if it doesn't exist
init_config() {
    if [[ ! -f "$CONFIG_FILE" ]]; then
        log_info "Creating default AI budget configuration..."
        cat > "$CONFIG_FILE" << EOF
{
  "budget": {
    "monthly": $DEFAULT_BUDGET_MONTHLY,
    "daily": $DEFAULT_BUDGET_DAILY
  },
  "alerts": {
    "threshold_80_percent": $ALERT_THRESHOLD_80,
    "threshold_95_percent": $ALERT_THRESHOLD_95,
    "email_notifications": false,
    "slack_webhook": null
  },
  "monitoring": {
    "enabled": true,
    "log_requests": true,
    "track_models": true,
    "track_agents": true
  },
  "optimization": {
    "auto_escalation": false,
    "cache_enabled": true,
    "batch_processing": false
  }
}
EOF
        log_success "AI budget configuration created at $CONFIG_FILE"
    fi
}

# Load configuration
load_config() {
    if [[ ! -f "$CONFIG_FILE" ]]; then
        log_error "Configuration file not found: $CONFIG_FILE"
        return 1
    fi

    # Simple JSON parsing for bash (limited)
    MONTHLY_BUDGET=$(grep -o '"monthly":\s*[0-9]*' "$CONFIG_FILE" | grep -o '[0-9]*' | head -1)
    DAILY_BUDGET=$(grep -o '"daily":\s*[0-9]*' "$CONFIG_FILE" | grep -o '[0-9]*' | tail -1)

    MONTHLY_BUDGET=${MONTHLY_BUDGET:-$DEFAULT_BUDGET_MONTHLY}
    DAILY_BUDGET=${DAILY_BUDGET:-$DEFAULT_BUDGET_DAILY}
}

# Get current month for tracking
get_current_month() {
    date '+%Y-%m'
}

# Get current date for daily tracking
get_current_date() {
    date '+%Y-%m-%d'
}

# Calculate cost based on model and tokens (simplified estimation)
calculate_cost() {
    local model="$1"
    local tokens="$2"
    local cost_per_token=0

    case "$model" in
        "gpt-4o")
            cost_per_token=0.00003  # $0.03 per 1K tokens
            ;;
        "gpt-4o-mini")
            cost_per_token=0.0000015  # $0.0015 per 1K tokens
            ;;
        "claude-3-5-sonnet")
            cost_per_token=0.000015  # $0.015 per 1K tokens
            ;;
        "claude-3-haiku")
            cost_per_token=0.0000005  # $0.0005 per 1K tokens
            ;;
        *)
            cost_per_token=0.00001  # Default estimate
            ;;
    esac

    # Calculate cost for tokens (assuming 1K token chunks)
    echo "scale=6; ($tokens / 1000) * $cost_per_token" | bc -l
}

# Log AI usage (simulated - in real implementation, this would integrate with GitHub Copilot API)
log_ai_usage() {
    local agent="$1"
    local model="$2"
    local tokens="$3"
    local task_type="$4"
    local timestamp
    local cost

    timestamp=$(date '+%Y-%m-%d %H:%M:%S')
    cost=$(calculate_cost "$model" "$tokens")

    # Log to monthly file
    local monthly_file="$AI_LOG_DIR/$(get_current_month).log"
    echo "$timestamp|$agent|$model|$tokens|$cost|$task_type" >> "$monthly_file"

    # Log to daily file
    local daily_file="$AI_LOG_DIR/$(get_current_date).log"
    echo "$timestamp|$agent|$model|$tokens|$cost|$task_type" >> "$daily_file"

    log_info "Logged AI usage: Agent=$agent, Model=$model, Tokens=$tokens, Cost=\$$cost, Task=$task_type"
}

# Calculate usage for current month
calculate_monthly_usage() {
    local monthly_file="$AI_LOG_DIR/$(get_current_month).log"
    local total_cost=0
    local total_tokens=0

    if [[ -f "$monthly_file" ]]; then
        while IFS='|' read -r timestamp agent model tokens cost task_type; do
            total_cost=$(echo "scale=6; $total_cost + $cost" | bc -l)
            total_tokens=$((total_tokens + tokens))
        done < "$monthly_file"
    fi

    echo "$total_cost|$total_tokens"
}

# Calculate usage for current day
calculate_daily_usage() {
    local daily_file="$AI_LOG_DIR/$(get_current_date).log"
    local total_cost=0
    local total_tokens=0

    if [[ -f "$daily_file" ]]; then
        while IFS='|' read -r timestamp agent model tokens cost task_type; do
            total_cost=$(echo "scale=6; $total_cost + $cost" | bc -l)
            total_tokens=$((total_tokens + tokens))
        done < "$daily_file"
    fi

    echo "$total_cost|$total_tokens"
}

# Check budget thresholds and send alerts
check_budget_alerts() {
    local monthly_usage
    local daily_usage
    local monthly_cost
    local daily_cost
    local monthly_percent
    local daily_percent

    monthly_usage=$(calculate_monthly_usage)
    daily_usage=$(calculate_daily_usage)

    monthly_cost=$(echo "$monthly_usage" | cut -d'|' -f1)
    daily_cost=$(echo "$daily_usage" | cut -d'|' -f1)

    # Calculate percentages
    monthly_percent=$(echo "scale=2; ($monthly_cost / $MONTHLY_BUDGET) * 100" | bc -l 2>/dev/null || echo "0")
    daily_percent=$(echo "scale=2; ($daily_cost / $DAILY_BUDGET) * 100" | bc -l 2>/dev/null || echo "0")

    # Check monthly alerts
    if (( $(echo "$monthly_percent >= $ALERT_THRESHOLD_95" | bc -l) )); then
        log_error "🚨 CRITICAL: Monthly AI budget at ${monthly_percent}% ($${monthly_cost}/$${MONTHLY_BUDGET})"
        echo "CRITICAL: Monthly AI budget exceeded ${ALERT_THRESHOLD_95}% threshold"
    elif (( $(echo "$monthly_percent >= $ALERT_THRESHOLD_80" | bc -l) )); then
        log_warn "⚠️  WARNING: Monthly AI budget at ${monthly_percent}% ($${monthly_cost}/$${MONTHLY_BUDGET})"
        echo "WARNING: Monthly AI budget exceeded ${ALERT_THRESHOLD_80}% threshold"
    fi

    # Check daily alerts
    if (( $(echo "$daily_percent >= $ALERT_THRESHOLD_95" | bc -l) )); then
        log_error "🚨 CRITICAL: Daily AI budget at ${daily_percent}% ($${daily_cost}/$${DAILY_BUDGET})"
        echo "CRITICAL: Daily AI budget exceeded ${ALERT_THRESHOLD_95}% threshold"
    elif (( $(echo "$daily_percent >= $ALERT_THRESHOLD_80" | bc -l) )); then
        log_warn "⚠️  WARNING: Daily AI budget at ${daily_percent}% ($${daily_cost}/$${DAILY_BUDGET})"
        echo "WARNING: Daily AI budget exceeded ${ALERT_THRESHOLD_80}% threshold"
    fi
}

# Generate usage report
generate_report() {
    local monthly_usage
    local daily_usage
    local monthly_cost
    local daily_cost
    local monthly_tokens
    local daily_tokens

    monthly_usage=$(calculate_monthly_usage)
    daily_usage=$(calculate_daily_usage)

    monthly_cost=$(echo "$monthly_usage" | cut -d'|' -f1)
    monthly_tokens=$(echo "$monthly_usage" | cut -d'|' -f2)
    daily_cost=$(echo "$daily_usage" | cut -d'|' -f1)
    daily_tokens=$(echo "$daily_usage" | cut -d'|' -f2)

    cat << EOF
🤖 AI Cost Monitoring Report - $(date '+%Y-%m-%d %H:%M:%S')

📊 Current Month ($(get_current_month)):
   • Cost: \$${monthly_cost} / \$${MONTHLY_BUDGET} ($(echo "scale=2; ($monthly_cost / $MONTHLY_BUDGET) * 100" | bc -l 2>/dev/null || echo "0")%)
   • Tokens: ${monthly_tokens}

📅 Today ($(get_current_date)):
   • Cost: \$${daily_cost} / \$${DAILY_BUDGET} ($(echo "scale=2; ($daily_cost / $DAILY_BUDGET) * 100" | bc -l 2>/dev/null || echo "0")%)
   • Tokens: ${daily_tokens}

⚙️  Configuration:
   • Monthly Budget: \$${MONTHLY_BUDGET}
   • Daily Budget: \$${DAILY_BUDGET}
   • Alert Thresholds: ${ALERT_THRESHOLD_80}% / ${ALERT_THRESHOLD_95}%

📁 Log Files:
   • Monthly: $AI_LOG_DIR/$(get_current_month).log
   • Daily: $AI_LOG_DIR/$(get_current_date).log
   • Config: $CONFIG_FILE
EOF
}

# Simulate AI usage for testing (remove in production)
simulate_usage() {
    local agents=("Backend" "Frontend" "QA" "Architect" "TechLead" "Security" "DevOps")
    local models=("gpt-4o" "gpt-4o-mini" "claude-3-5-sonnet" "claude-3-haiku")
    local task_types=("code-review" "implementation" "debugging" "documentation" "testing")

    # Random agent, model, and task
    local agent=${agents[$RANDOM % ${#agents[@]}]}
    local model=${models[$RANDOM % ${#models[@]}]}
    local task_type=${task_types[$RANDOM % ${#task_types[@]}]}

    # Random token count (realistic range)
    local tokens=$((RANDOM % 2000 + 500))

    log_ai_usage "$agent" "$model" "$tokens" "$task_type"
}

# Main command processing
main() {
    local command="${1:-status}"

    ensure_directories
    init_config
    load_config

    case "$command" in
        "status"|"report")
            generate_report
            check_budget_alerts
            ;;
        "log")
            if [[ $# -lt 5 ]]; then
                log_error "Usage: $0 log <agent> <model> <tokens> <task_type>"
                exit 1
            fi
            log_ai_usage "$2" "$3" "$4" "$5"
            ;;
        "alerts")
            check_budget_alerts
            ;;
        "simulate")
            local count=${2:-1}
            log_info "Simulating $count AI usage events..."
            for ((i=1; i<=count; i++)); do
                simulate_usage
            done
            log_success "Simulation complete"
            ;;
        "config")
            if [[ -f "$CONFIG_FILE" ]]; then
                cat "$CONFIG_FILE"
            else
                log_error "Configuration file not found: $CONFIG_FILE"
                exit 1
            fi
            ;;
        "help"|"-h"|"--help")
            cat << EOF
AI Cost Monitoring Script

USAGE:
  $0 [command] [options]

COMMANDS:
  status     Show current usage report and check alerts (default)
  report     Same as status
  log        Log AI usage: agent model tokens task_type
  alerts     Check budget alerts only
  simulate   Generate simulated usage data for testing
  config     Show current configuration
  help       Show this help message

EXAMPLES:
  $0                          # Show status report
  $0 log Backend gpt-4o 1500 code-review
  $0 simulate 10             # Generate 10 test entries
  $0 alerts                  # Check budget alerts

CONFIGURATION:
  Edit $CONFIG_FILE to modify budgets and thresholds

LOGS:
  Monthly logs: $AI_LOG_DIR/YYYY-MM.log
  Daily logs: $AI_LOG_DIR/YYYY-MM-DD.log
EOF
            ;;
        *)
            log_error "Unknown command: $command"
            echo "Run '$0 help' for usage information"
            exit 1
            ;;
    esac
}

# Run main function with all arguments
main "$@"