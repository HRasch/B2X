#!/bin/bash

################################################################################
# Script: Update GitHub Issues After Sprint Planning
# Purpose: Synchronize GitHub issue states with sprint planning decisions
# Author: @ScrumMaster
# Date: December 30, 2025
################################################################################

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}╔════════════════════════════════════════════════════════════╗${NC}"
echo -e "${BLUE}║   Updating GitHub Issues for Sprint 001 Planning           ║${NC}"
echo -e "${BLUE}╚════════════════════════════════════════════════════════════╝${NC}"
echo ""

# Check if gh CLI is installed
if ! command -v gh &> /dev/null; then
    echo -e "${RED}❌ Error: GitHub CLI (gh) is not installed${NC}"
    echo "Install from: https://cli.github.com/"
    exit 1
fi

# Check GitHub authentication
if ! gh auth status &> /dev/null; then
    echo -e "${RED}❌ Error: Not authenticated with GitHub${NC}"
    echo "Run: gh auth login"
    exit 1
fi

echo -e "${GREEN}✅ GitHub CLI authenticated${NC}"
echo ""

# Function to update an issue
update_issue() {
    local issue_num=$1
    local milestone=$2
    local add_labels=$3
    local remove_labels=$4
    local assignees=$5
    
    echo -e "${YELLOW}Updating Issue #${issue_num}...${NC}"
    
    # Build gh command
    local cmd="gh issue edit ${issue_num}"
    
    if [ -n "$milestone" ]; then
        cmd="$cmd --milestone \"$milestone\""
    fi
    
    if [ -n "$add_labels" ]; then
        cmd="$cmd --add-label $add_labels"
    fi
    
    if [ -n "$remove_labels" ]; then
        cmd="$cmd --remove-label $remove_labels"
    fi
    
    if [ -n "$assignees" ]; then
        cmd="$cmd --add-assignee $assignees"
    fi
    
    # Execute command
    eval $cmd && echo -e "${GREEN}  ✅ Updated${NC}" || echo -e "${RED}  ⚠️  Warning: Update had issues${NC}"
}

echo -e "${BLUE}Step 1: Updating Main Sprint 001 Issues${NC}"
echo "────────────────────────────────────────────────────────────"

# Issue #57: Dependencies
update_issue 57 \
    "Sprint 001" \
    "sprint/001,ready-to-start,week-1" \
    "backlog" \
    "holger"

# Issue #56: UI Modernization  
update_issue 56 \
    "Sprint 001" \
    "sprint/001,ready-to-start,week-2,has-conditions" \
    "backlog" \
    "holger"

# Issue #15: Compliance
update_issue 15 \
    "Sprint 001" \
    "sprint/001,specification-phase,p0-critical,awaiting-legal" \
    "" \
    "holger"

# Issue #48: Testing (deferred to Sprint 2)
update_issue 48 \
    "Sprint 002" \
    "sprint/002,deferred-approved" \
    "backlog" \
    "holger"

echo ""
echo -e "${BLUE}Step 2: Updating P0.6 Compliance Sub-Issues (#20-#28)${NC}"
echo "────────────────────────────────────────────────────────────"

for issue_num in {20..28}; do
    echo -e "${YELLOW}Updating Issue #${issue_num}...${NC}"
    gh issue edit $issue_num \
        --add-label "p0.6-compliance,blocked-by-#15" || true
    echo -e "${GREEN}  ✅ Updated${NC}"
done

echo ""
echo -e "${BLUE}Step 3: Verification${NC}"
echo "────────────────────────────────────────────────────────────"

echo ""
echo -e "${YELLOW}Sprint 001 Issues:${NC}"
gh issue list --milestone "Sprint 001" --json number,title,state --template '{{range .}}{{.number}} - {{.title}} ({{.state}}){{"\n"}}{{end}}'

echo ""
echo -e "${YELLOW}Sprint 002 Issues:${NC}"
gh issue list --milestone "Sprint 002" --json number,title,state --template '{{range .}}{{.number}} - {{.title}} ({{.state}}){{"\n"}}{{end}}'

echo ""
echo -e "${GREEN}╔════════════════════════════════════════════════════════════╗${NC}"
echo -e "${GREEN}║           ✅ Sprint 001 Issues Updated!                   ║${NC}"
echo -e "${GREEN}╚════════════════════════════════════════════════════════════╝${NC}"

echo ""
echo "Next Steps:"
echo "1. ✅ Review updated issues in GitHub"
echo "2. ✅ Verify milestone assignments"
echo "3. ✅ Check label assignments"
echo "4. ✅ Add status comments to issues"
echo "5. ✅ Notify team of sprint start"
echo ""
echo "See .github/instructions/update-github-issues-sprint.md for details"
