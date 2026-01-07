#!/bin/bash
# B2X MCP Automated Daily Review Script
# Runs every morning at 09:00 to validate MCP health and generate reports
# Setup: crontab -e -> 0 9 * * * /path/to/B2X/scripts/daily-mcp-review.sh

set -e

REPO_ROOT="$(git rev-parse --show-toplevel)"
cd "$REPO_ROOT"

TIMESTAMP=$(date +%Y-%m-%d_%H%M%S)
REPORT_DIR=".ai/logs/mcp-usage/daily-reviews"
REPORT_FILE="$REPORT_DIR/daily-review-$TIMESTAMP.md"

# Create report directory
mkdir -p "$REPORT_DIR"

echo "# Daily MCP Review Report" > "$REPORT_FILE"
echo "**Generated:** $TIMESTAMP" >> "$REPORT_FILE"
echo "" >> "$REPORT_FILE"

# 1. Metrics Dashboard
echo "## 📊 Metrics Dashboard" >> "$REPORT_FILE"
echo "" >> "$REPORT_FILE"
echo '```' >> "$REPORT_FILE"
node scripts/mcp-metrics-dashboard.js print >> "$REPORT_FILE" 2>&1 || echo "Dashboard generation failed" >> "$REPORT_FILE"
echo '```' >> "$REPORT_FILE"
echo "" >> "$REPORT_FILE"

# 2. Rate Limits Summary
echo "## ⚙️ Rate Limits Summary" >> "$REPORT_FILE"
echo "" >> "$REPORT_FILE"
echo '```' >> "$REPORT_FILE"
node scripts/mcp-rate-limiter.js summary >> "$REPORT_FILE" 2>&1 || echo "Rate limit check failed" >> "$REPORT_FILE"
echo '```' >> "$REPORT_FILE"
echo "" >> "$REPORT_FILE"

# 3. Cache Statistics
echo "## 🗄️ Cache Statistics" >> "$REPORT_FILE"
echo "" >> "$REPORT_FILE"
echo '```' >> "$REPORT_FILE"
node scripts/mcp-cache-manager.js stats >> "$REPORT_FILE" 2>&1 || echo "Cache stats unavailable" >> "$REPORT_FILE"
echo '```' >> "$REPORT_FILE"
echo "" >> "$REPORT_FILE"

# 4. Audit Trail Report (daily)
echo "## 📋 Audit Trail (24h)" >> "$REPORT_FILE"
echo "" >> "$REPORT_FILE"
echo '```' >> "$REPORT_FILE"
node scripts/mcp-audit-trail.js report daily >> "$REPORT_FILE" 2>&1 || echo "Audit trail unavailable" >> "$REPORT_FILE"
echo '```' >> "$REPORT_FILE"
echo "" >> "$REPORT_FILE"

# 5. Health Status
echo "## 🏥 System Health" >> "$REPORT_FILE"
echo "" >> "$REPORT_FILE"

ISSUES=0

# Check for rate limit violations
if node scripts/mcp-rate-limiter.js summary 2>/dev/null | grep -E "9[0-9]\.[0-9]|100\.0" > /dev/null; then
    echo "- ⚠️ **WARNING**: One or more servers approaching rate limits" >> "$REPORT_FILE"
    ISSUES=$((ISSUES + 1))
else
    echo "- ✓ All servers within rate limits" >> "$REPORT_FILE"
fi

# Check cache hit rate
if [ -f ".ai/logs/mcp-usage/metrics.json" ]; then
    HIT_RATE=$(grep -o '"hitRate": "[^"]*"' ".ai/logs/mcp-usage/metrics.json" | head -1 | cut -d'"' -f4)
    if [ -n "$HIT_RATE" ]; then
        echo "- 📊 Cache hit rate: $HIT_RATE" >> "$REPORT_FILE"
    fi
fi

# Check for errors in recent logs
if [ -f ".ai/logs/mcp-usage/rate-limit-alerts.log" ]; then
    ERROR_COUNT=$(wc -l < ".ai/logs/mcp-usage/rate-limit-alerts.log")
    if [ "$ERROR_COUNT" -gt 0 ]; then
        echo "- ⚠️ **ALERTS**: $ERROR_COUNT recent alerts (see alerts log)" >> "$REPORT_FILE"
        ISSUES=$((ISSUES + 1))
    fi
fi

echo "" >> "$REPORT_FILE"

# 6. Recommendations
echo "## 💡 Recommendations" >> "$REPORT_FILE"
echo "" >> "$REPORT_FILE"

if [ $ISSUES -eq 0 ]; then
    echo "- ✅ System operating normally" >> "$REPORT_FILE"
    echo "- Continue monitoring daily" >> "$REPORT_FILE"
else
    echo "- ⚠️ **ACTION REQUIRED**: Review alerts and take corrective action" >> "$REPORT_FILE"
    echo "- Contact @Security or @CopilotExpert for assistance" >> "$REPORT_FILE"
fi

echo "" >> "$REPORT_FILE"
echo "---" >> "$REPORT_FILE"
echo "*Report Location:* \`$REPORT_FILE\`" >> "$REPORT_FILE"
echo "*Next Review:* $(date -d '+1 day' '+%Y-%m-%d 09:00')" >> "$REPORT_FILE"

# Output summary
echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "✅ Daily MCP Review Complete"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""
echo "📄 Report saved to: $REPORT_FILE"
echo ""

if [ $ISSUES -gt 0 ]; then
    echo "⚠️  $ISSUES ISSUE(S) DETECTED - Review report above"
    exit 1
else
    echo "✓ All systems healthy"
    exit 0
fi
