# 🚀 Quick Reference: Update Sprint Issues on GitHub

**When:** After sprint planning is complete and approved  
**Who:** @ScrumMaster  
**How Long:** 10-15 minutes

---

## ⚡ Quick Start

### Option 1: Automated Script (Recommended)
```bash
chmod +x scripts/update-sprint-issues.sh
./scripts/update-sprint-issues.sh
```

### Option 2: Manual GitHub CLI Commands

```bash
# Issue #57: Dependencies (8 SP) → Ready to Start
gh issue edit 57 --milestone "Sprint 001" \
  --add-label sprint/001,ready-to-start,week-1

# Issue #56: UI Modernization (13 SP) → Ready to Start
gh issue edit 56 --milestone "Sprint 001" \
  --add-label sprint/001,ready-to-start,week-2,has-conditions

# Issue #15: Compliance (21 SP) → Specification Phase
gh issue edit 15 --milestone "Sprint 001" \
  --add-label sprint/001,specification-phase,p0-critical,awaiting-legal

# Issue #48: Testing (13 SP) → Deferred to Sprint 2
gh issue edit 48 --milestone "Sprint 002" \
  --add-label sprint/002,deferred-approved
```

### Option 3: Manual via GitHub Web UI

**For each issue (#57, #56, #15, #48):**
1. Open issue on GitHub
2. Click Milestone → Select appropriate sprint
3. Click Labels → Add appropriate labels
4. Add comment with status update
5. Verify assignee

---

## 📋 Issue Status Quick Map

| Issue | Status | Labels | Milestone |
|-------|--------|--------|-----------|
| #57 | Ready to Start | sprint/001, ready-to-start, week-1 | Sprint 001 |
| #56 | Ready to Start | sprint/001, ready-to-start, week-2, has-conditions | Sprint 001 |
| #15 | Specification | sprint/001, specification-phase, p0-critical, awaiting-legal | Sprint 001 |
| #48 | Deferred | sprint/002, deferred-approved | Sprint 002 |
| #20-#28 | Backlog | p0.6-compliance, blocked-by-#15 | Phase 1 |

---

## ✅ Verification Commands

```bash
# View all Sprint 001 issues
gh issue list --milestone "Sprint 001" -L 100

# View all Sprint 002 issues
gh issue list --milestone "Sprint 002" -L 100

# View issues with specific label
gh issue list --label sprint/001 -L 100

# View issues awaiting legal sign-off
gh issue list --label awaiting-legal -L 100
```

---

## 💡 Common Tasks

### Add Status Comment to Issue
```bash
gh issue comment 57 -b "## Sprint 001 Update

Status: Ready to Start (Jan 2, 2026)
Owner: @Backend
Risk: LOW

See SPRINT_001_PLAN.md for details."
```

### Add Multiple Labels at Once
```bash
gh issue edit 56 \
  --add-label label1,label2,label3,label4
```

### Update Milestone
```bash
gh issue edit 57 --milestone "Sprint 001"
```

### Assign Issue to User
```bash
gh issue edit 57 --add-assignee holger
```

---

## 📞 Troubleshooting

**"Not authenticated with GitHub"**
```bash
gh auth login
# Follow prompts to authenticate
```

**"Issue not found"**
- Check issue number is correct
- Check you have access to the repository

**"gh command not found"**
```bash
# Install GitHub CLI from https://cli.github.com/
brew install gh  # macOS
apt install gh   # Linux
```

**"Permission denied"**
- Check you have write access to repository
- Check GitHub token has correct scopes

---

## 📚 Full Documentation

See: `.ai/workflows/update-github-issues-sprint.md`

For complete step-by-step instructions with screenshots and templates.

---

**Status:** ✅ Ready to Use  
**Last Updated:** December 30, 2025  
**Next Use:** After Sprint 2 Planning
