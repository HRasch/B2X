# GitHub CLI (gh) Sprint Management How-To Guide

**Version:** 1.0  
**Last Updated:** 2025-12-30  
**Prerequisite:** GitHub CLI installed (`brew install gh` or download from [github.com/cli/cli](https://github.com/cli/cli))

---

## Table of Contents
1. [Setup & Authentication](GITHUB_CLI_SPRINT_HOWTO.md)
2. [Sprint Planning Commands](GITHUB_CLI_SPRINT_HOWTO.md)
3. [Sprint Execution Commands](GITHUB_CLI_SPRINT_HOWTO.md)
4. [Sprint Review & Closure](GITHUB_CLI_SPRINT_HOWTO.md)
5. [Useful Queries & Reports](GITHUB_CLI_SPRINT_HOWTO.md)
6. [Daily Standup Workflow](GITHUB_CLI_SPRINT_HOWTO.md)

---

## Setup & Authentication

### Initial Setup
```bash
# Install GitHub CLI
brew install gh

# Authenticate with GitHub
gh auth login
# Follow prompts: GitHub.com → HTTPS → Generate token → Paste token

# Verify authentication
gh auth status
```

### Configure for Project
```bash
# Set your default GitHub organization/repo
gh repo set-default owner/repo

# Verify configuration
gh config list
```

---

## Sprint Planning Commands

### 1. Create Sprint Project

```bash
# Create a new GitHub Projects board for the sprint
gh project create --title "Sprint 12 (Jan 6-17, 2025)" \
  --description "AI-DEV Framework Setup & Foundation" \
  --owner @yourorg \
  --format "table"
```

### 2. List Backlog Items

```bash
# View all items in backlog (not assigned to any sprint)
gh issue list --repo owner/repo \
  --label "backlog" \
  --state open \
  --sort created

# View prioritized items with story points
gh issue list --repo owner/repo \
  --label "backlog" \
  --sort reactions \
  --limit 20
```

### 3. Create Sprint Issues

```bash
# Create a new issue for sprint
gh issue create \
  --title "FEAT-001: User Authentication" \
  --body "## User Story
As a user, I want to authenticate with the system.

## Acceptance Criteria
- [ ] Login form displays correctly
- [ ] Password validation works
- [ ] JWT token issued on success

## Effort: 13 points" \
  --label "feature,sprint-12" \
  --assignee @backend-lead \
  --project "Sprint 12"

# Create multiple issues from template
for i in {1..5}; do
  gh issue create --title "FEAT-$i: Feature $i" \
    --label "feature,sprint-12" \
    --project "Sprint 12"
done
```

### 4. Add Issues to Sprint Project

```bash
# Add single issue to project
gh project item-add 123 --owner owner --number 456

# Add multiple issues to sprint (using query)
gh issue list --repo owner/repo \
  --label "sprint-12" \
  --state open \
  --json number \
  --jq '.[] | .number' | \
  xargs -I {} gh project item-add 123 --number {}
```

### 5. Set Story Points & Priority

```bash
# Using custom field update (requires GraphQL)
gh api graphql -f query='
  mutation {
    updateProjectV2ItemFieldValue(input: {
      projectId: "PVT_123"
      itemId: "PVTI_456"
      fieldId: "PVTF_789"
      value: {number: 13}
    }) {
      clientMutationId
    }
  }
'

# Or update via issue labels (points in label: "points-13")
gh issue edit 456 --add-label "points-13,priority-p0"
```

### 6. Link Dependencies

```bash
# Link two issues (issue #456 blocks issue #789)
gh issue edit 456 --add-label "blocks-789"
gh issue edit 789 --add-label "blocked-by-456"

# View issue with all links
gh issue view 456 --repo owner/repo
```

---

## Sprint Execution Commands

### Daily Standup

```bash
# Get today's status for all team members
gh issue list --repo owner/repo \
  --assignee "@me" \
  --label "sprint-12" \
  --state open \
  --sort updated

# Get team's in-progress items
gh issue list --repo owner/repo \
  --label "sprint-12,in-progress" \
  --state open

# Create standup note issue
gh issue create \
  --title "Daily Standup - 2025-12-31" \
  --body "## Team Status
- @backend: Completed FEAT-001, working on FEAT-002
- @frontend: Completed FEAT-003, blocked on API response
- @qa: Testing FEAT-001

## Blockers
- [ ] API endpoint response format unclear (Issue #456)

## Action Items
- [ ] @backend to clarify API contract by EOD" \
  --label "standup,sprint-12" \
  --assignee @scrummaster
```

### Update Issue Status

```bash
# Move issue from "In Progress" to "Review" (update via labels)
gh issue edit 456 \
  --remove-label "in-progress" \
  --add-label "in-review"

# Close issue (mark as done)
gh issue close 456 --comment "Merged in PR #789"

# View all issues by status
gh issue list --repo owner/repo \
  --label "sprint-12,in-progress" \
  --state open
```

### Track Blockers

```bash
# Create a blocker issue
gh issue create \
  --title "BLOCKER: S3 Configuration Not Complete" \
  --body "## Impact
Blocks FEAT-012 (Profile picture upload)

## Root Cause
Infrastructure team hasn't configured S3 bucket.

## Resolution
Target: Jan 2 by EOD
Owner: @devops

## Workaround
Use local file storage for now." \
  --label "blocker,sprint-12" \
  --milestone "Sprint 12"

# View all blockers in sprint
gh issue list --repo owner/repo \
  --label "blocker,sprint-12" \
  --state open
```

### Link PR to Issue

```bash
# Automatically link PR to issue (mention in PR description)
gh pr create \
  --title "Implement user authentication" \
  --body "Fixes #456

## Changes
- Added login endpoint
- Added JWT token generation
- Added password validation

## Testing
- [ ] Unit tests pass
- [ ] Integration tests pass" \
  --assignee @backend \
  --label "sprint-12"

# Or link existing PR to issue
gh issue comment 456 --body "Fixed by #789"
```

---

## Sprint Review & Closure

### Generate Sprint Metrics

```bash
# Count completed items in sprint
gh issue list --repo owner/repo \
  --label "sprint-12" \
  --state closed \
  --json number,title | wc -l

# Get velocity (sum of story points for closed issues)
gh api graphql -f query='
  query {
    repository(owner: "owner", name: "repo") {
      issues(first: 100, filterBy: {labels: "sprint-12", states: CLOSED}) {
        nodes {
          number
          title
        }
      }
    }
  }
'

# List all completed items with points
gh issue list --repo owner/repo \
  --label "sprint-12" \
  --state closed \
  --sort updated \
  --json number,title,labels
```

### Create Sprint Review Notes

```bash
gh issue create \
  --title "Sprint 12 Review - Jan 6-17, 2025" \
  --body "## Sprint Summary
- **Committed Points:** 120
- **Completed Points:** 95
- **Completion Rate:** 79%

## Completed Features
- FEAT-001: User Authentication
- FEAT-002: JWT Token Management
- FEAT-003: Login UI
- FEAT-010: Profile API

## Demo Items
- Link to PRs and merged commits

## Metrics
- Velocity: 95 points
- Defects found: 2 (low priority)
- Quality: Good

## Feedback
[Stakeholder feedback from review]" \
  --label "sprint-review,sprint-12" \
  --assignee @scrummaster
```

### Create Retrospective Issues

```bash
# Create retrospective issue
gh issue create \
  --title "Sprint 12 Retrospective" \
  --body "## What Went Well ✅
- Strong team collaboration
- Proactive testing
- Good documentation

## What Could Be Better ⚠️
- S3 setup delays
- Third-party API timeout issues
- Large PR reviews

## Action Items for Sprint 13
- [ ] Pre-plan infrastructure changes (@devops)
- [ ] Add load testing (@qa)
- [ ] Establish PR size guidelines (@techlead)

## Team Health
- Morale: Good
- Communication: Excellent
- Work-Life Balance: Good" \
  --label "retrospective,sprint-12" \
  --assignee @scrummaster
```

---

## Useful Queries & Reports

### Sprint Dashboard

```bash
# Get complete sprint status
echo "=== SPRINT 12 DASHBOARD ==="
echo "Total Issues:"
gh issue list --repo owner/repo \
  --label "sprint-12" --json number | wc -l

echo "Completed:"
gh issue list --repo owner/repo \
  --label "sprint-12" --state closed --json number | wc -l

echo "In Progress:"
gh issue list --repo owner/repo \
  --label "sprint-12,in-progress" --state open --json number | wc -l

echo "Blocked:"
gh issue list --repo owner/repo \
  --label "sprint-12,blocker" --state open --json number | wc -l
```

### Team Velocity Report

```bash
# Create velocity report for past 3 sprints
echo "=== VELOCITY REPORT ==="
for sprint in "sprint-10" "sprint-11" "sprint-12"; do
  echo "Sprint: $sprint"
  gh issue list --repo owner/repo \
    --label "$sprint" \
    --state closed \
    --json number,title,labels | \
    grep -i "points-" | wc -l
done
```

### Burndown Data

```bash
# Get daily progress (closed issues by day)
# This requires custom scripting, but shows the concept:
for day in {1..10}; do
  date="2025-01-0$day"
  echo "Day $day ($date):"
  gh api graphql -f query="
    query {
      search(query: \"repo:owner/repo label:sprint-12 closed:$date\", type: ISSUE, first: 100) {
        issueCount
      }
    }
  " --jq '.data.search.issueCount'
done
```

### Identify At-Risk Issues

```bash
# Issues not updated in 2+ days during sprint
gh issue list --repo owner/repo \
  --label "sprint-12,in-progress" \
  --state open \
  --sort updated \
  --json number,title,updatedAt

# Issues with no assignee
gh issue list --repo owner/repo \
  --label "sprint-12" \
  --state open \
  --json number,title,assignees | \
  grep '"assignees": \[\]'
```

---

## Daily Standup Workflow

### Quick Daily Status Check

```bash
#!/bin/bash
# save as: scripts/daily-standup.sh

SPRINT="sprint-12"
REPO="owner/repo"

echo "📊 DAILY STANDUP - $(date +%Y-%m-%d)"
echo ""

# Get all team members working on sprint
TEAM=("@backend" "@frontend" "@qa" "@devops" "@techlead")

for member in "${TEAM[@]}"; do
  echo "👤 $member"
  gh issue list --repo "$REPO" \
    --assignee "$member" \
    --label "$SPRINT" \
    --state open \
    --json number,title,labels \
    --template '{{range .}}  - #{{.number}}: {{.title}}{{"\n"}}{{end}}'
  echo ""
done

# Show blockers
echo "🚨 BLOCKERS"
gh issue list --repo "$REPO" \
  --label "$SPRINT,blocker" \
  --state open \
  --json number,title \
  --template '{{range .}}#{{.number}}: {{.title}}{{"\n"}}{{end}}'

# Show review queue
echo ""
echo "📋 IN REVIEW"
gh pr list --repo "$REPO" \
  --label "$SPRINT" \
  --state open \
  --json number,title,author \
  --template '{{range .}}#{{.number}}: {{.title}} by {{.author.login}}{{"\n"}}{{end}}'
```

Usage:
```bash
bash scripts/daily-standup.sh
```

### Automate Daily Standup Post

```bash
#!/bin/bash
# Schedule with cron: 0 9 * * 1-5 bash scripts/post-standup.sh

SPRINT="sprint-12"
REPO="owner/repo"
DATE=$(date +%Y-%m-%d)

# Generate standup report
REPORT="## Daily Standup - $DATE\n"
REPORT+="$(gh issue list --repo \"$REPO\" --label \"$SPRINT\" --state open --json number,title,assignees)"

# Post as issue comment or create new issue
gh issue create \
  --repo "$REPO" \
  --title "Standup - $DATE" \
  --body "$REPORT" \
  --label "standup,$SPRINT" \
  --assignee @scrummaster
```

---

## Integration Tips

### GitHub Actions Integration

```yaml
name: Sprint Status Check

on:
  schedule:
    - cron: '0 9 * * 1-5'  # 9 AM weekdays

jobs:
  standup:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Generate standup
        run: |
          gh issue list --repo owner/repo \
            --label "sprint-12" \
            --state open \
            --json number,title,assignees
        env:
          GH_TOKEN: ${{ secrets.GITHUB_TOKEN }}
```

### Shell Alias for Quick Access

```bash
# Add to ~/.zshrc or ~/.bashrc
alias gh-sprint='gh issue list --label sprint-12 --state open --sort updated'
alias gh-blockers='gh issue list --label sprint-12,blocker --state open'
alias gh-review='gh pr list --label sprint-12 --state open'
alias gh-standup='bash ./scripts/daily-standup.sh'
```

---

## Common Tasks Cheat Sheet

| Task | Command |
|------|---------|
| List sprint issues | `gh issue list --label sprint-12 --state open` |
| Create issue | `gh issue create --title "..." --label sprint-12` |
| Update status | `gh issue edit 456 --remove-label in-progress --add-label done` |
| View issue details | `gh issue view 456` |
| Add to project | `gh project item-add PROJECT_ID --number ISSUE_NUM` |
| Create PR | `gh pr create --title "..." --label sprint-12` |
| Link PR to issue | `gh issue comment 456 --body "Fixes #789"` |
| Close issue | `gh issue close 456` |
| Get metrics | `gh issue list --label sprint-12 --state closed --json number` |
| View blockers | `gh issue list --label sprint-12,blocker --state open` |

---

## Troubleshooting

### Authentication Issues
```bash
# Re-authenticate
gh auth logout
gh auth login

# Check token permissions
gh auth status
```

### Permission Denied
```bash
# Ensure you have access to the repo
gh repo view owner/repo

# Check if using correct org
gh config list
```

### Project Not Found
```bash
# List available projects
gh project list --owner owner

# Verify project ID
gh project list --owner owner --json id,title
```

---

**Ready to use!** Start with basic commands and build automation as needed.  
📖 Full GitHub CLI docs: https://cli.github.com/manual
