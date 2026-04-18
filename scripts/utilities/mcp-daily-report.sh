#!/bin/bash
# MCP Token Monitoring Script
# Tracks and reports MCP token usage daily

STATS_FILE=".ai/logs/mcp-usage/mcp-stats.json"
REPORT_FILE=".ai/logs/mcp-usage/daily-report-$(date +%Y-%m-%d).md"

echo "# MCP Token Usage Report - $(date)" > "$REPORT_FILE"
echo "" >> "$REPORT_FILE"

if [ -f "$STATS_FILE" ]; then
  echo "## Current Statistics" >> "$REPORT_FILE"
  echo "" >> "$REPORT_FILE"
  echo "\`\`\`json" >> "$REPORT_FILE"
  cat "$STATS_FILE" >> "$REPORT_FILE"
  echo "\`\`\`" >> "$REPORT_FILE"
  echo "" >> "$REPORT_FILE"
fi

# Get cache statistics
node scripts/mcp-cache-manager.js stats >> "$REPORT_FILE" 2>&1

echo "" >> "$REPORT_FILE"
echo "## Cache Status" >> "$REPORT_FILE"
echo "- Cache directory: \`.ai/cache/mcp/\`" >> "$REPORT_FILE"
echo "- Active servers: TypeScript MCP, Vue MCP, Security MCP, Performance MCP" >> "$REPORT_FILE"
echo "- Disabled servers: Roslyn MCP, Wolverine MCP, Chrome DevTools" >> "$REPORT_FILE"

echo "✅ Report generated: $REPORT_FILE"
