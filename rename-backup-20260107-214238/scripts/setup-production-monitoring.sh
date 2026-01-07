#!/bin/bash
# B2Connect MCP Production Monitoring Setup
# Enables automated daily monitoring and reporting

REPO_ROOT="$(git rev-parse --show-toplevel 2>/dev/null || pwd)"
SCRIPT_DIR="$REPO_ROOT/scripts"

echo "📊 Setting up B2Connect MCP Production Monitoring..."
echo ""

# Check if crontab exists
if ! command -v crontab &> /dev/null; then
    echo "⚠️  crontab not available on this system"
    echo "Manual setup required:"
    echo "  Run daily-mcp-review.sh manually or via your scheduler"
    exit 0
fi

# Get current crontab (if exists)
CURRENT_CRON=$(crontab -l 2>/dev/null || echo "")

# Check if daily review is already scheduled
if echo "$CURRENT_CRON" | grep -q "daily-mcp-review.sh"; then
    echo "✓ Daily MCP review already scheduled in crontab"
else
    # Create new cron entry
    NEW_CRON="$CURRENT_CRON"$'\n'"# B2Connect MCP Daily Review - 09:00 UTC"$'\n'"0 9 * * * $SCRIPT_DIR/daily-mcp-review.sh >> $REPO_ROOT/.ai/logs/mcp-usage/cron.log 2>&1"
    
    # Install cron entry
    echo "$NEW_CRON" | crontab -
    echo "✓ Daily MCP review scheduled for 09:00 UTC"
fi

echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "✅ Production Monitoring Setup Complete"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""
echo "Automated Tasks Enabled:"
echo "  • Daily MCP Review: 09:00 UTC (via crontab)"
echo "  • Pre-Commit Validation: On every git commit"
echo ""
echo "Monitor these files for reports:"
echo "  • .ai/logs/mcp-usage/daily-reviews/ (daily reports)"
echo "  • .ai/logs/mcp-usage/cron.log (scheduling logs)"
echo ""
echo "To verify cron is active:"
echo "  crontab -l | grep mcp"
echo ""
