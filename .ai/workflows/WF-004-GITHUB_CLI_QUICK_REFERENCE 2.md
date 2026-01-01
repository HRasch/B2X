# GitHub CLI Sprint Management - Quick Reference Card

**Print or bookmark this for quick access during sprint planning!**

---

## Essential Commands

### Setup
```bash
gh auth login                    # Authenticate with GitHub
gh repo set-default owner/repo   # Set your default repo
```

### Create Sprint Project & Issues
```bash
# Create project
gh project create --title "Sprint 12" --owner org

# Create single issue
gh issue create --title "FEAT-001" --label sprint-12

# Create multiple issues (batch)
for i in {1..5}; do
  gh issue create --title "FEAT-$i" --label sprint-12
done
```

### Daily Sprint Status
```bash
# List all sprint issues
gh issue list --label sprint-12 --state open

# List in-progress items
gh issue list --label "sprint-12,in-progress" --state open

# List blocked items
gh issue list --label "sprint-12,blocker" --state open

# List PRs in review
gh pr list --label sprint-12 --state open
```

### Update Issue Status
```bash
# Move to in-progress
gh issue edit 456 --remove-label backlog --add-label in-progress

# Move to review (when PR created)
gh issue edit 456 --remove-label in-progress --add-label in-review

# Mark done (close issue)
gh issue close 456 --comment "Merged in PR #789"
```

### Create Daily Notes
```bash
# Standup note
gh issue create \
  --title "Standup - 2025-12-30" \
  --body "## Status\n- @backend: ...\n- @frontend: ..." \
  --label standup,sprint-12

# Blocker
gh issue create \
  --title "BLOCKER: Description" \
  --label blocker,sprint-12 \
  --assignee @responsible-person

# Risk alert
gh issue create \
  --title "RISK: Description" \
  --label risk,sprint-12
```

### Sprint Review & Closure
```bash
# Count completed items
gh issue list --label sprint-12 --state closed --json number | wc -l

# List all completed items
gh issue list --label sprint-12 --state closed --json number,title

# Get merged PRs
gh pr list --label sprint-12 --state merged

# Create retrospective
gh issue create \
  --title "Sprint 12 Retrospective" \
  --body "## What went well\n...\n## What to improve\n..." \
  --label retrospective,sprint-12
```

---

## Common Label Conventions

```
✅ Status Labels
- backlog
- in-progress
- in-review
- done

🎯 Priority Labels
- priority-p0     (highest)
- priority-p1
- priority-p2
- priority-p3     (lowest)

📊 Estimation Labels
- points-1
- points-3
- points-5
- points-8
- points-13
- points-21

🏷️ Type Labels
- feature
- bugfix
- technical-debt
- documentation

🚨 Special Labels
- blocker
- risk
- standup
- retrospective
```

---

## Handy Aliases

Add to `~/.zshrc` or `~/.bashrc`:

```bash
# Sprint shortcuts
alias gh-sprint='gh issue list --label sprint-12 --state open'
alias gh-blockers='gh issue list --label sprint-12,blocker --state open'
alias gh-review='gh pr list --label sprint-12 --state open'
alias gh-done='gh issue list --label sprint-12 --state closed'
alias gh-standup='bash ./scripts/daily-standup.sh'

# Useful queries
alias gh-me='gh issue list --assignee @me --state open'
alias gh-assigned='gh pr list --assignee @me --state open'
```

---

## One-Liners

```bash
# Today's completed work
gh issue list --label sprint-12 --state closed --json closedAt,title --jq '.[] | select(.closedAt | startswith("2025-12-30"))'

# Team's capacity used
gh issue list --label sprint-12 --state open --json assignees

# Most active reviewer
gh pr list --label sprint-12 --state merged --json reviews

# Velocity trend
echo "Sprint 10: $(gh issue list --label sprint-10 --state closed --json number | wc -l)"
echo "Sprint 11: $(gh issue list --label sprint-11 --state closed --json number | wc -l)"
echo "Sprint 12: $(gh issue list --label sprint-12 --state closed --json number | wc -l)"
```

---

## Tips & Tricks

### View Detailed Issue
```bash
gh issue view 456  # Shows full details
```

### Edit Multiple Issues
```bash
# Add label to all backlog items
gh issue list --label backlog --json number -jq '.[] | .number' | \
  xargs -I {} gh issue edit {} --add-label priority-p0
```

### Create Issue Template
```bash
# From stdin
echo "## Description\nFix login bug" | \
  gh issue create --title "BUG-123" --body-file -
```

### Link Issues Automatically
```bash
# In PR description, mention: "Fixes #456"
# GitHub will auto-link and close when merged
```

---

## Before First Sprint

- [ ] Install GitHub CLI: `brew install gh`
- [ ] Authenticate: `gh auth login`
- [ ] Set default repo: `gh repo set-default owner/repo`
- [ ] Create sprint project: `gh project create`
- [ ] Add team members to project
- [ ] Set up custom fields in project
- [ ] Create label conventions (above)
- [ ] Set up helper scripts in `scripts/`

---

## Helpful Links

- 📖 Full Documentation: https://cli.github.com/manual
- 🛠️ How-To Guide: [GITHUB_CLI_SPRINT_HOWTO.md](GITHUB_CLI_SPRINT_HOWTO.md)
- 📋 Sprint Workflow: [sprint-cycle.prompt.md](../../.github/prompts/sprint-cycle.prompt.md)
- 📊 Implementation Guide: [GITHUB_CLI_IMPLEMENTATION.md](GITHUB_CLI_IMPLEMENTATION.md)

---

**Print this page and keep it handy during sprint planning!**
