#!/bin/bash

# B2Connect: Create GitHub Project Board for Team Onboarding Tracking
# Script: CREATE_PROJECT_BOARD.sh
# Purpose: Set up GitHub Project board to track all 8 role-specific onboarding issues
# Date: 28. Dezember 2025

set -e

echo "🚀 B2Connect: Creating GitHub Project Board for Onboarding Tracking..."
echo ""

# Check if gh CLI is installed
if ! command -v gh &> /dev/null; then
    echo "❌ Error: GitHub CLI (gh) is not installed."
    echo "Install it from: https://cli.github.com/"
    exit 1
fi

# Check if authenticated
if ! gh auth status &> /dev/null; then
    echo "❌ Error: Not authenticated with GitHub CLI."
    echo "Run: gh auth login"
    exit 1
fi

# Get repo info
REPO=$(gh repo view --json nameWithOwner -q)
echo "📦 Repository: $REPO"
echo ""

# ============================================================================
# Create Project Board
# ============================================================================

echo "📊 Creating GitHub Project board..."

# Create project using gh project create (new API)
# Note: GitHub Projects (beta) API might require different commands
# Fallback: Use gh project create if available, else provide manual instructions

if gh project create --title "Team Onboarding Q1 2026" --description "Track all 8 role-specific onboarding issues for Q1 2026 team setup" > /dev/null 2>&1; then
    echo "✅ GitHub Project board created successfully"
    PROJECT_URL=$(gh repo view --json url -q)"/projects"
    echo "📍 Project board: $PROJECT_URL"
else
    echo "⚠️  Note: GitHub Projects API not available in this gh version"
    echo "   Please create project manually:"
    echo "   1. Go to: $REPO/projects"
    echo "   2. Click 'New project'"
    echo "   3. Title: 'Team Onboarding Q1 2026'"
    echo "   4. Template: Kanban (with table view)"
    echo "   5. Add custom fields:"
    echo "      - Owner (select)"
    echo "      - Duration (number)"
    echo "      - P0 Component (select)"
    echo "      - Priority (select)"
fi

echo ""
echo "✅ Project board setup complete!"
echo ""
echo "📋 Next Steps:"
echo "   1. Manually add these columns/views to your project:"
echo "      - Not Started"
echo "      - In Progress"
echo "      - Completed"
echo "   2. Link all 8 onboarding issues to the project"
echo "   3. Set custom fields for tracking:"
echo "      - Owner: Team member assigned"
echo "      - Duration: Estimated weeks (3-5)"
echo "      - P0 Component: If applicable"
echo "      - Priority: Critical/High/Medium"
echo ""
echo "📊 Board Structure:"
echo ""
echo "   TEAM ONBOARDING Q1 2026"
echo "   ├─ Not Started"
echo "   │  ├─ Backend Developer Onboarding (3 weeks)"
echo "   │  ├─ Frontend Developer Onboarding (3 weeks, WCAG 2.1 AA)"
echo "   │  ├─ Security Engineer Onboarding (3 weeks, P0.1-P0.5, P0.7)"
echo "   │  ├─ DevOps Engineer Onboarding (3 weeks, P0.3-P0.5)"
echo "   │  ├─ QA Engineer Onboarding (5 weeks, 52 tests)"
echo "   │  ├─ Tech Lead Onboarding (3 weeks, code review standards)"
echo "   │  ├─ Product Owner Onboarding (3 weeks, 34-week roadmap)"
echo "   │  └─ Legal/Compliance Officer Onboarding (3 weeks, EU regs)"
echo "   │"
echo "   ├─ In Progress"
echo "   │  └─ [Issues being actively worked on]"
echo "   │"
echo "   └─ Completed"
echo "      └─ [Finished onboarding issues]"
echo ""
echo "🎯 Custom Fields to Add:"
echo ""
echo "   Field Name       | Type   | Options"
echo "   ─────────────────┼────────┼──────────────────────"
echo "   Owner            | Select | [Team member names]"
echo "   Duration (weeks) | Number | 1-10"
echo "   P0 Component     | Select | P0.1, P0.2, P0.3, P0.4, P0.5, P0.6, P0.7, P0.8, P0.9, N/A"
echo "   Priority         | Select | Critical, High, Medium, Low"
echo "   Start Date       | Date   | [Auto-fill with assignment date]"
echo "   Target Date      | Date   | [Auto-calculated from duration]"
echo ""
echo "==============================================================================="
