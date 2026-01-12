#!/bin/bash

# 🔍 Collaboration Monitor
# Watches collaboration folders and reports daily
# Usage: ./scripts/watch-collaboration.sh
# Or via VS Code: Run Debug Configuration "🔍 Collaboration Monitor"

set -e

COLLABORATE_DIR="collaborate/issue"
SUMMARY_FILE="/tmp/collaboration-summary.md"
TIMESTAMP=$(date '+%Y-%m-%d %H:%M:%S')

# Colors for output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

echo -e "${GREEN}🔍 Monitoring collaboration system...${NC}"
echo "Started: $TIMESTAMP"
echo ""

# Initialize summary
cat > "$SUMMARY_FILE" << EOF
# Daily Collaboration Summary
**Generated**: $TIMESTAMP

## Pending Requests

EOF

# Counters
TOTAL_PENDING=0
TOTAL_RESPONSES=0
TOTAL_OVERDUE=0

# Check all issues
echo -e "${YELLOW}Scanning issues...${NC}"
for issue_dir in "$COLLABORATE_DIR"/*; do
  if [ -d "$issue_dir" ]; then
    ISSUE_ID=$(basename "$issue_dir")
    
    # Count pending vs completed
    PENDING=$(find "$issue_dir" -maxdepth 2 -name "*.md" -type f \
             ! -name "*response*" \
             ! -name "*COORDINATION*" 2>/dev/null | wc -l || echo "0")
    
    RESPONSES=$(find "$issue_dir" -maxdepth 2 -name "*response*" -type f 2>/dev/null | wc -l || echo "0")
    
    TOTAL_PENDING=$((TOTAL_PENDING + PENDING))
    TOTAL_RESPONSES=$((TOTAL_RESPONSES + RESPONSES))
    
    if [ "$PENDING" -gt 0 ] || [ "$RESPONSES" -gt 0 ]; then
      echo "  Issue #$ISSUE_ID: $PENDING pending, $RESPONSES responses"
      
      echo "" >> "$SUMMARY_FILE"
      echo "### Issue #$ISSUE_ID" >> "$SUMMARY_FILE"
      echo "- **Pending Requests**: $PENDING" >> "$SUMMARY_FILE"
      echo "- **Responses**: $RESPONSES" >> "$SUMMARY_FILE"
      echo "" >> "$SUMMARY_FILE"
      
      # List pending requests
      echo "**Pending:**" >> "$SUMMARY_FILE"
      find "$issue_dir" -maxdepth 2 -name "*.md" -type f \
           ! -name "*response*" \
           ! -name "*COORDINATION*" \
           -printf "- \`%f\` (created: %Tc)\n" 2>/dev/null >> "$SUMMARY_FILE" || true
    fi
  fi
done

echo ""
echo -e "${GREEN}Summary:${NC}"
echo "  Total Pending: $TOTAL_PENDING"
echo "  Total Responses: $TOTAL_RESPONSES"
echo ""

# Check for overdue requests (> 48 hours)
echo -e "${YELLOW}Checking for overdue requests (> 48h)...${NC}"
OVERDUE_FILES=$(find "$COLLABORATE_DIR" -maxdepth 2 -name "*.md" -type f \
     ! -name "*response*" \
     ! -name "*COORDINATION*" \
     -mtime +2 2>/dev/null || echo "")

if [ ! -z "$OVERDUE_FILES" ]; then
  TOTAL_OVERDUE=$(echo "$OVERDUE_FILES" | wc -l)
  echo -e "${RED}  🔴 $TOTAL_OVERDUE overdue requests found!${NC}"
  
  echo "" >> "$SUMMARY_FILE"
  echo "## ⚠️ Overdue Requests (> 48h)" >> "$SUMMARY_FILE"
  echo "$OVERDUE_FILES" | while read file; do
    echo "- **$(basename "$file")**" >> "$SUMMARY_FILE"
    echo "  Location: $(dirname "$file")" >> "$SUMMARY_FILE"
  done
else
  echo -e "${GREEN}  ✅ No overdue requests${NC}"
  echo "" >> "$SUMMARY_FILE"
  echo "## ✅ Overdue Requests (> 48h)" >> "$SUMMARY_FILE"
  echo "None - all requests current" >> "$SUMMARY_FILE"
fi

echo ""
echo -e "${GREEN}📊 Summary saved to:${NC} $SUMMARY_FILE"
echo ""
echo "---"
cat "$SUMMARY_FILE"
echo "---"
echo ""
echo -e "${GREEN}✅ Collaboration monitoring complete${NC}"
echo ""
echo "Next Steps:"
echo "  1. Review summary above"
echo "  2. Update collaborate/issue/{ID}/COORDINATION_SUMMARY.md"
echo "  3. Post findings to GitHub issue comments"
echo "  4. Flag any overdue requests for escalation"
