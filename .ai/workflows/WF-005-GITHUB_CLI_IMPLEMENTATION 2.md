# GitHub CLI Sprint Management - Implementation Guide

**Created:** 2025-12-30  
**Status:** ✅ Ready for use

---

## Overview

Sprint planning instructions have been enhanced to integrate **GitHub CLI (gh)** for practical sprint management alongside GitHub Projects. This enables:

- **Automation**: Script-based sprint creation, issue tracking, and reporting
- **Consistency**: Standardized commands for all team members
- **Efficiency**: Quick status checks without leaving the terminal
- **Scalability**: Easy integration with CI/CD and monitoring systems

---

## What Was Created

### 1. **GitHub CLI How-To Guide**
📁 **File**: [GITHUB_CLI_SPRINT_HOWTO.md](GITHUB_CLI_SPRINT_HOWTO.md)

**Contents:**
- ✅ Setup & authentication instructions
- ✅ Sprint planning commands (create projects, issues, link dependencies)
- ✅ Sprint execution commands (daily standups, status updates, blockers)
- ✅ Sprint review & closure (metrics, retrospective)
- ✅ Useful queries & reporting patterns
- ✅ Daily standup workflow with sample script
- ✅ Common tasks cheat sheet
- ✅ Integration with GitHub Actions and shell aliases

**Key Features:**
- Complete command reference for all sprint operations
- Ready-to-use shell scripts for automation
- Real-world examples for daily standup generation
- Integration patterns for existing workflows

---

### 2. **Enhanced Sprint Cycle Prompt**
📁 **File**: [sprint-cycle.prompt.md](../../.github/prompts/sprint-cycle.prompt.md)

**Updates:**
- ✅ Quick start section with GitHub CLI examples
- ✅ Planning phase: `gh project create`, issue creation, linking
- ✅ Execution phase: Daily standup template with GitHub CLI commands
- ✅ Status tracking: `gh issue edit`, blocker creation, PR linking
- ✅ Review phase: Metric generation with GitHub CLI
- ✅ Retrospective: Issue creation for structured retro notes
- ✅ GitHub CLI integration guide with setup steps
- ✅ Cross-reference to how-to guide for detailed commands

---

## Quick Start

### For ScrumMaster

```bash
# Create a sprint project
gh project create --title "Sprint 12 (Jan 6-17, 2025)" \
  --owner yourorg \
  --format "table"

# List backlog items
gh issue list --label "backlog" --state open --limit 20

# Create sprint issues
gh issue create --title "FEAT-001: Feature Name" \
  --label "feature,sprint-12" \
  --assignee @dev-name \
  --project "Sprint 12"
```

### For Daily Standups

```bash
# Quick standup status
bash scripts/daily-standup.sh

# Or manually
gh issue list --label sprint-12 --state open --sort updated
gh pr list --label sprint-12 --state open
gh issue list --label sprint-12,blocker --state open
```

### For Closure

```bash
# Generate metrics
gh issue list --label sprint-12 --state closed --json number | wc -l

# Create retrospective issue
gh issue create \
  --title "Sprint 12 Retrospective" \
  --label "retrospective,sprint-12"
```

---

## CLI Command Categories

### Sprint Planning
| Operation | Command |
|-----------|---------|
| Create project | `gh project create --title "Sprint X"` |
| Create issue | `gh issue create --title "..." --label sprint-X` |
| Add to project | `gh project item-add PROJECT_ID --number ISSUE` |
| Link issues | `gh issue edit ISSUE --add-label "blocks-ISSUE2"` |
| Set points | `gh issue edit ISSUE --add-label "points-8"` |

### Daily Execution
| Operation | Command |
|-----------|---------|
| List sprint issues | `gh issue list --label sprint-X --state open` |
| Update status | `gh issue edit ISSUE --remove-label old --add-label new` |
| Create standup | `gh issue create --title "Standup - $(date)"` |
| Log blocker | `gh issue create --title "BLOCKER: ..."` |
| Close issue | `gh issue close ISSUE --comment "Fixed by PR #123"` |

### Sprint Review
| Operation | Command |
|-----------|---------|
| Get completed items | `gh issue list --label sprint-X --state closed` |
| View merged PRs | `gh pr list --label sprint-X --state merged` |
| Count points | `gh issue list --label sprint-X --state closed --json number` |
| Create retro notes | `gh issue create --title "Retrospective"` |

**Full command reference**: [GITHUB_CLI_SPRINT_HOWTO.md](GITHUB_CLI_SPRINT_HOWTO.md)

---

## Integration Points

### With GitHub Projects
- GitHub CLI creates issues that appear in Projects board
- Projects custom fields can be set via labels
- PR automation works alongside CLI updates
- Real-time synchronization between CLI and UI

### With CI/CD
- GitHub Actions can run `gh` commands
- Automated daily standup posts
- Automated metrics collection
- Integration with deployment pipelines

### With Team Workflows
- Shell aliases for quick access (`alias gh-standup=...`)
- Team scripts in `scripts/daily-standup.sh`
- Consistent commands across all team members
- Easy on-boarding with standard commands

---

## Sample Automation Scripts

### Daily Standup Script
```bash
#!/bin/bash
# scripts/daily-standup.sh

SPRINT="sprint-12"
REPO="owner/repo"

echo "📊 DAILY STANDUP - $(date +%Y-%m-%d)"

# List all team members' work
for member in @backend @frontend @qa; do
  echo "👤 $member"
  gh issue list --repo "$REPO" \
    --assignee "$member" \
    --label "$SPRINT" \
    --state open
done

# Show blockers
echo "🚨 BLOCKERS"
gh issue list --label "$SPRINT,blocker" --state open
```

### Velocity Reporting
```bash
# Get completed points per sprint
for sprint in sprint-10 sprint-11 sprint-12; do
  echo "$sprint: $(gh issue list --label $sprint --state closed --json number | wc -l)"
done
```

---

## Setup Instructions

### 1. Install GitHub CLI
```bash
# macOS
brew install gh

# Linux
sudo apt install gh

# Verify installation
gh --version
```

### 2. Authenticate
```bash
# Login to GitHub
gh auth login

# Follow prompts: GitHub.com → HTTPS → Generate token → Paste

# Verify authentication
gh auth status
```

### 3. Configure Repository
```bash
# Set default repo
gh repo set-default owner/repo

# Verify configuration
gh config list
```

### 4. Create Helper Scripts
```bash
# Copy daily standup script to scripts/
cp daily-standup.sh scripts/daily-standup.sh
chmod +x scripts/daily-standup.sh

# Test the script
bash scripts/daily-standup.sh
```

### 5. Setup Aliases (Optional)
```bash
# Add to ~/.zshrc or ~/.bashrc
alias gh-sprint='gh issue list --label sprint-12 --state open --sort updated'
alias gh-blockers='gh issue list --label sprint-12,blocker --state open'
alias gh-review='gh pr list --label sprint-12 --state open'
alias gh-standup='bash ./scripts/daily-standup.sh'

# Source the file
source ~/.zshrc
```

---

## Key Benefits

| Aspect | Benefit |
|--------|---------|
| **Speed** | Create issues, update status, check metrics in seconds |
| **Consistency** | Same commands work for all team members |
| **Automation** | Integrate with GitHub Actions, cron jobs, monitoring |
| **Transparency** | Everyone sees the same data from the CLI |
| **Scalability** | Easy to scale from 1 sprint to multiple concurrent sprints |
| **Integration** | Works seamlessly with GitHub Projects, PRs, CI/CD |

---

## Next Steps

1. **Install GitHub CLI** (see [Setup Instructions](GITHUB_CLI_IMPLEMENTATION.md))
2. **Review the How-To Guide** ([GITHUB_CLI_SPRINT_HOWTO.md](GITHUB_CLI_SPRINT_HOWTO.md))
3. **Create sample sprint project** using `gh project create`
4. **Test key commands** from the guide
5. **Set up helper scripts** in `scripts/` directory
6. **Train team** on basic commands and workflows
7. **Iterate** - add more automation as needed

---

## Documentation Map

```
.github/prompts/
├── sprint-cycle.prompt.md ........... Main sprint workflow
└── [other prompts]

.ai/workflows/
├── GITHUB_CLI_SPRINT_HOWTO.md ....... Complete CLI reference
├── GITHUB_CLI_IMPLEMENTATION.md ..... Implementation guide
├── GITHUB_CLI_QUICK_REFERENCE.md .... Quick reference card
└── [other workflows]

scripts/
├── daily-standup.sh ................ Sample automation script
├── sprint-velocity-report.sh ........ Metrics script
└── [other scripts]

.ai/sprint/
├── current.md ...................... Current sprint status
├── backlog.md ....................... Product backlog
└── GITHUB_PROJECTS_UPDATE.md ........ GitHub Projects setup guide
```

---

## Troubleshooting

### Issue: "Not authenticated"
```bash
gh auth login
```

### Issue: "Repository not found"
```bash
gh repo set-default owner/repo
```

### Issue: "Project not found"
```bash
gh project list --owner owner
```

---

**Ready to use!** Start with basic commands and grow your automation from there.  
📖 **Full GitHub CLI Docs**: https://cli.github.com/manual
