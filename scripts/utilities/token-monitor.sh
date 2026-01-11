#!/bin/bash
# Token Optimization Monitor
# Usage: ./scripts/token-monitor.sh [period]
# Period: daily | weekly | monthly (default: daily)

PERIOD=${1:-daily}
LOG_FILE=".ai/logs/token-optimization-${PERIOD}.log"
TEMP_DIR=".ai/temp"

mkdir -p "$(dirname "$LOG_FILE")"

echo "=== Token Optimization Monitor: $PERIOD ===" >> "$LOG_FILE"
echo "Date: $(date)" >> "$LOG_FILE"

# Count temp files created in period
case $PERIOD in
    daily)
        SINCE="1 day ago"
        ;;
    weekly)
        SINCE="7 days ago"
        ;;
    monthly)
        SINCE="30 days ago"
        ;;
    *)
        SINCE="1 day ago"
        ;;
esac

TEMP_FILES=$(find "$TEMP_DIR" -name "task-*.txt" -o -name "task-*.log" -o -name "task-*.json" -newermt "$SINCE" 2>/dev/null | wc -l)
TOTAL_SIZE=$(find "$TEMP_DIR" -name "task-*.txt" -o -name "task-*.log" -o -name "task-*.json" -newermt "$SINCE" -exec wc -c {} \; 2>/dev/null | awk '{sum += $1} END {print sum}')

echo "Temp files created: $TEMP_FILES" >> "$LOG_FILE"
echo "Total size saved: ${TOTAL_SIZE:-0} bytes" >> "$LOG_FILE"

# Estimate token savings (rough: 1 token ≈ 4 bytes)
ESTIMATED_TOKENS=$((TOTAL_SIZE / 4))
echo "Estimated tokens saved: $ESTIMATED_TOKENS" >> "$LOG_FILE"

# Check for rate limit indicators (placeholder)
echo "Rate limit incidents: 0 (monitoring not implemented)" >> "$LOG_FILE"

echo "Monitor complete. See $LOG_FILE" >> "$LOG_FILE"
echo "" >> "$LOG_FILE"

echo "Token optimization monitoring complete for $PERIOD period."
echo "Log saved to: $LOG_FILE"