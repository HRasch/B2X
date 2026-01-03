#!/bin/bash

# Rate Limit Monitor Script - Simplified Version
# Monitors GitHub Copilot usage and agent activity to prevent rate limit hits

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"
LOG_DIR="$PROJECT_ROOT/.ai/logs"
RATE_LOG="$LOG_DIR/rate-limit-monitoring.log"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Create log directory if it doesn't exist
mkdir -p "$LOG_DIR"

# Function to log messages
log() {
    local level="$1"
    local message="$2"
    local timestamp=$(date '+%Y-%m-%d %H:%M:%S')
    echo "[$timestamp] [$level] $message" >> "$RATE_LOG"
    echo "[$timestamp] [$level] $message"
}

# Function to check current agent activity (returns count only)
get_agent_activity() {
    local recent_files=$(find "$PROJECT_ROOT/.ai" -name "*.md" -newermt "5 minutes ago" 2>/dev/null | wc -l)
    echo "$recent_files"
}

# Function to check rate limit status
check_rate_limit_status() {
    log "INFO" "Checking rate limit status..."

    local activity_level=$(get_agent_activity)
    local status="UNKNOWN"

    if [ "$activity_level" -gt 5 ]; then
        status="${RED}HIGH ACTIVITY${NC} - Consider cooldown"
        log "WARNING" "High agent activity detected: $activity_level recent files"
    elif [ "$activity_level" -gt 2 ]; then
        status="${YELLOW}MODERATE ACTIVITY${NC} - Monitor closely"
        log "INFO" "Moderate agent activity: $activity_level recent files"
    else
        status="${GREEN}LOW ACTIVITY${NC} - Safe to continue"
        log "INFO" "Low agent activity: $activity_level recent files"
    fi

    echo "Rate Limit Status: $status"
}

# Function to show usage statistics
show_usage_stats() {
    echo -e "\n${BLUE}=== GitHub Copilot Usage Statistics ===${NC}"

    # Count agent interactions (rough estimate based on file modifications)
    local today_files=$(find "$PROJECT_ROOT/.ai" -name "*.md" -newermt "1 day ago" 2>/dev/null | wc -l)
    local week_files=$(find "$PROJECT_ROOT/.ai" -name "*.md" -newermt "7 days ago" 2>/dev/null | wc -l)

    echo "Files modified today: $today_files"
    echo "Files modified this week: $week_files"

    # Estimate API calls (rough heuristic)
    local estimated_daily_calls=$((today_files * 3))  # Rough estimate
    local estimated_weekly_calls=$((week_files * 3))

    echo "Estimated daily API calls: $estimated_daily_calls"
    echo "Estimated weekly API calls: $estimated_weekly_calls"

    # GitHub Copilot Pro+ limit is 1,500/month
    local monthly_limit=1500
    local projected_monthly=$((estimated_weekly_calls * 4))

    if [ "$projected_monthly" -gt $((monthly_limit * 80 / 100)) ]; then
        echo -e "${RED}Projected monthly usage: $projected_monthly / $monthly_limit (WARNING: >80%)${NC}"
    elif [ "$projected_monthly" -gt $((monthly_limit * 50 / 100)) ]; then
        echo -e "${YELLOW}Projected monthly usage: $projected_monthly / $monthly_limit${NC}"
    else
        echo -e "${GREEN}Projected monthly usage: $projected_monthly / $monthly_limit${NC}"
    fi
}

# Function to show optimization recommendations
show_recommendations() {
    echo -e "\n${BLUE}=== Rate Limit Optimization Recommendations ===${NC}"

    local activity_level=$(get_agent_activity)

    if [ "$activity_level" -gt 5 ]; then
        echo -e "${RED}🚨 CRITICAL: High concurrent activity detected${NC}"
        echo "  → Stop all agent work immediately"
        echo "  → Wait 15-30 minutes before resuming"
        echo "  → Switch to single-agent sequential workflow"
    elif [ "$activity_level" -gt 2 ]; then
        echo -e "${YELLOW}⚠️  WARNING: Moderate concurrent activity${NC}"
        echo "  → Complete current tasks, then take 10-minute cooldown"
        echo "  → Batch remaining work into single sessions"
        echo "  → Use text-based reviews instead of live agent coordination"
    else
        echo -e "${GREEN}✅ LOW ACTIVITY: Safe to continue${NC}"
        echo "  → Continue with sequential workflow"
        echo "  → Monitor activity levels"
    fi

    echo -e "\n${BLUE}General Recommendations:${NC}"
    echo "  • Use single-agent sessions (45 min work + 10 min cooldown)"
    echo "  • Batch file operations instead of individual edits"
    echo "  • Archive old files to reduce context size"
    echo "  • Use text-based agent coordination via .ai/ files"
    echo "  • Monitor this script regularly during development"
}

# Function to run continuous monitoring
monitor_continuous() {
    local interval="${1:-300}"  # Default 5 minutes
    log "INFO" "Starting continuous monitoring (interval: ${interval}s)"

    while true; do
        echo -e "\n$(date '+%Y-%m-%d %H:%M:%S') - Rate Limit Monitor Check"
        echo "=================================================="

        check_rate_limit_status
        show_usage_stats
        show_recommendations

        echo -e "\nNext check in ${interval} seconds... (Ctrl+C to stop)"
        sleep "$interval"
    done
}

# Main script logic
case "${1:-status}" in
    "status")
        log "INFO" "Running rate limit status check"
        check_rate_limit_status
        show_usage_stats
        show_recommendations
        ;;
    "monitor")
        monitor_continuous "${2:-300}"
        ;;
    "stats")
        show_usage_stats
        ;;
    "log")
        echo "Rate limit monitoring log:"
        echo "=========================="
        if [ -f "$RATE_LOG" ]; then
            tail -20 "$RATE_LOG"
        else
            echo "No log file found at $RATE_LOG"
        fi
        ;;
    "help"|"-h"|"--help")
        echo "Usage: $0 [command] [options]"
        echo ""
        echo "Commands:"
        echo "  status          Show current rate limit status (default)"
        echo "  monitor [sec]   Continuous monitoring (default 300s interval)"
        echo "  stats           Show usage statistics only"
        echo "  log             Show recent log entries"
        echo "  help            Show this help"
        echo ""
        echo "Examples:"
        echo "  $0 status"
        echo "  $0 monitor 600    # Check every 10 minutes"
        echo "  $0 stats"
        ;;
    *)
        echo "Unknown command: $1"
        echo "Run '$0 help' for usage information"
        exit 1
        ;;
esac

log "INFO" "Rate limit monitoring script completed"