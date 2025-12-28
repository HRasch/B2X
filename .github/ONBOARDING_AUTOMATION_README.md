# B2Connect: Team Onboarding Automation Scripts

**Date:** 28. Dezember 2025  
**Purpose:** Automate creation of 8 GitHub issues and project board for Q1 2026 team onboarding  
**Status:** Ready to Deploy ✅

---

## 📋 Overview

B2Connect has 8 role-specific team members with personalized onboarding issues:

1. **Backend Developer** - Wolverine, DDD, Onion Architecture (3 weeks)
2. **Frontend Developer** - Vue.js 3, Tailwind CSS, WCAG 2.1 AA (3 weeks, BITV deadline)
3. **Security Engineer** - P0.1-P0.5, P0.7, encryption, audit logging (3 weeks + ongoing)
4. **DevOps Engineer** - Aspire, P0.3-P0.5, infrastructure (3 weeks + ongoing)
5. **QA Engineer** - 52 compliance tests, automation framework (5 weeks)
6. **Tech Lead** - Code review standards, architecture oversight (3 weeks + ongoing)
7. **Product Owner** - 34-week roadmap, €428.5K budget, 7 FTE team (3 weeks + ongoing)
8. **Legal/Compliance Officer** - EU regulations, compliance review (3 weeks + ongoing)

---

## 🚀 Quick Start

### Option 1: Auto-Create Everything (Fastest)

```bash
# 1. Make scripts executable
chmod +x .github/CREATE_ONBOARDING_ISSUES.sh
chmod +x .github/CREATE_PROJECT_BOARD.sh

# 2. Create all 8 GitHub issues
./.github/CREATE_ONBOARDING_ISSUES.sh

# 3. Create GitHub Project board (manual setup with guidance)
./.github/CREATE_PROJECT_BOARD.sh

# 4. View all created issues
gh issue list --label "onboarding"
```

### Option 2: Manual Creation (Full Control)

1. Go to GitHub Issues tab
2. Copy issue markdown from `.github/ISSUES_ONBOARDING_QUICK_START_GUIDES.md`
3. Paste into "New Issue" form
4. Customize labels and assignee
5. Click "Create issue"

---

## 📝 Scripts Included

### 1. `CREATE_ONBOARDING_ISSUES.sh`

**Purpose:** Automatically create all 8 onboarding issues in GitHub

**What it does:**
- ✅ Creates 8 GitHub issues (one per role)
- ✅ Adds labels: `documentation`, `onboarding`, `[role]`, `team-setup`
- ✅ Assigns to milestone: `Q1 2026 - Team Setup`
- ✅ Includes full issue description with:
  - 3-week curriculum (day-by-day tasks)
  - Completion criteria
  - First task assignment
  - Resources and links
  - Critical notes and deadlines

**Requirements:**
- GitHub CLI (`gh`) installed: https://cli.github.com/
- Authenticated with GitHub: `gh auth login`
- Write access to repository

**Usage:**
```bash
./.github/CREATE_ONBOARDING_ISSUES.sh
```

**Output:**
```
✅ Issue 1 created
✅ Issue 2 created
✅ Issue 3 created
✅ Issue 4 created
✅ Issue 5 created
✅ Issue 6 created
✅ Issue 7 created
✅ Issue 8 created

✅ SUCCESS! Created all 8 Team Onboarding GitHub Issues
```

**Verify Creation:**
```bash
# View all onboarding issues
gh issue list --label "onboarding"

# View specific role's issue
gh issue list --label "onboarding,backend"
```

---

### 2. `CREATE_PROJECT_BOARD.sh`

**Purpose:** Create GitHub Project board to track all 8 onboarding issues

**What it does:**
- ✅ Creates GitHub Project board: "Team Onboarding Q1 2026"
- ✅ Provides Kanban structure (Not Started → In Progress → Completed)
- ✅ Suggests custom fields for tracking:
  - Owner (team member)
  - Duration (weeks)
  - P0 Component (if applicable)
  - Priority (Critical/High/Medium)
  - Start Date
  - Target Date

**Requirements:**
- GitHub CLI (`gh`) installed
- Authenticated with GitHub

**Usage:**
```bash
./.github/CREATE_PROJECT_BOARD.sh
```

**Manual Setup (if GitHub Projects API not available):**
1. Go to: https://github.com/[your-org]/B2Connect/projects
2. Click "New project"
3. Title: `Team Onboarding Q1 2026`
4. Template: Kanban (with table view)
5. Add custom fields (see instructions in script output)

---

## 📋 What Gets Created

### GitHub Issues (8 Total)

Each issue includes:

✅ **Title:** `[ONBOARDING] [Role]: [Task]`  
✅ **Labels:** `documentation`, `onboarding`, `[role]`, `team-setup`  
✅ **Milestone:** `Q1 2026 - Team Setup`  
✅ **Description:**
  - 🎯 Mission statement
  - ⏱️ Duration (3-5 weeks)
  - 📚 Week-by-week curriculum
  - 📊 Completion criteria
  - 📊 First task assignment
  - 📚 Resources & links
  - ⚠️ Critical notes & deadlines

### Project Board

**Columns:**
- Not Started (default for new issues)
- In Progress (drag when starting)
- Completed (drag when done)

**Custom Fields:**
- **Owner:** Select team member assigned
- **Duration (weeks):** 3, 4, or 5 weeks
- **P0 Component:** P0.1-P0.9 or N/A
- **Priority:** Critical, High, Medium, Low
- **Start Date:** Auto-populated
- **Target Date:** Auto-calculated

**Views:**
- Table view (all issues with custom fields)
- Board view (Kanban columns)
- Timeline view (Gantt chart)

---

## 🎯 Issue Breakdown

| # | Issue | Team | Time | P0 Comp | Status |
|---|-------|------|------|---------|--------|
| 1 | Backend Developer | Backend | 3w | P0.1, P0.6, P0.9 | Created |
| 2 | Frontend Developer | Frontend | 3w | P0.6, P0.8 | Created |
| 3 | Security Engineer | Security | 3w | P0.1, P0.2, P0.3, P0.5, P0.7 | Created |
| 4 | DevOps Engineer | DevOps | 3w | P0.3, P0.4, P0.5 | Created |
| 5 | QA Engineer | QA | 5w | All (52 tests) | Created |
| 6 | Tech Lead | Architecture | 3w | All (oversight) | Created |
| 7 | Product Owner | Product | 3w | All (roadmap) | Created |
| 8 | Legal/Compliance | Legal | 3w | P0.6, P0.7, P0.8, P0.9 | Created |

---

## 📚 Supporting Documentation

All onboarding issues reference these quick-start guides:

- `docs/by-role/BACKEND_DEVELOPER_QUICK_START.md`
- `docs/by-role/FRONTEND_DEVELOPER_QUICK_START.md`
- `docs/by-role/SECURITY_ENGINEER_QUICK_START.md`
- `docs/by-role/DEVOPS_ENGINEER_QUICK_START.md`
- `docs/by-role/QA_ENGINEER_QUICK_START.md`
- `docs/by-role/TECH_LEAD_QUICK_START.md`
- `docs/by-role/PRODUCT_OWNER_QUICK_START.md`
- `docs/by-role/LEGAL_COMPLIANCE_QUICK_START.md`

Plus complete issue specifications in:
- `.github/ISSUES_ONBOARDING_QUICK_START_GUIDES.md`

---

## 🔄 Workflow

### Step 1: Create Issues
```bash
./.github/CREATE_ONBOARDING_ISSUES.sh
```

### Step 2: Create Project Board
```bash
./.github/CREATE_PROJECT_BOARD.sh
```

### Step 3: Assign Issues to Team Members
```bash
# Assign Backend Developer issue to person@example.com
gh issue edit 1 --add-assignee person@example.com
```

### Step 4: Link Issues to Project (Manual in UI)
1. Open each issue
2. Click "Projects" on right sidebar
3. Select "Team Onboarding Q1 2026"

### Step 5: Add Custom Fields (Manual in UI)
1. Open project board
2. Click "+" next to each issue
3. Fill in: Owner, Duration, P0 Component, Priority

### Step 6: Start Tracking Progress
- Team members check in daily/weekly
- Update issue progress in comments
- Move cards across Kanban columns
- Use project board for executive visibility

---

## 📊 Tracking Progress

### View All Onboarding Issues
```bash
gh issue list --label "onboarding"
```

### View by Role
```bash
gh issue list --label "onboarding,backend"
gh issue list --label "onboarding,frontend"
gh issue list --label "onboarding,security"
# ... etc
```

### View by Milestone
```bash
gh issue list --milestone "Q1 2026 - Team Setup"
```

### View by Assignee
```bash
gh issue list --assignee "username"
```

### Export for Reporting
```bash
# Export as CSV for stakeholder reports
gh issue list --label "onboarding" --json "title,state,assignees,milestone" > onboarding-status.json
```

---

## ✅ Success Criteria

After running both scripts:

- ✅ 8 GitHub issues created (all with full descriptions)
- ✅ All issues labeled: `documentation`, `onboarding`, `team-setup`
- ✅ All issues assigned to milestone: `Q1 2026 - Team Setup`
- ✅ GitHub Project board created
- ✅ Project board has Kanban structure (Not Started → In Progress → Completed)
- ✅ Project board has custom fields defined
- ✅ Team members can view their onboarding issue
- ✅ Team members have quick-start guide links

---

## ⚠️ Critical Deadlines (Referenced in Issues)

Every issue includes reminders of these critical dates:

| Date | Regulation | Impact |
|------|-----------|--------|
| **28. Juni 2025** | BITV 2.0 (Accessibility) | €5K-100K penalties |
| **17. Okt 2025** | NIS2 Phase 1 (Cybersecurity) | Mandatory incident response |
| **1. Jan 2026** | E-Rechnung B2G (ZUGFeRD) | Contract termination |
| **12. Mai 2026** | EU AI Act (HIGH-RISK AI) | €30M penalties |
| **1. Jan 2027** | E-Rechnung B2B Receive | Market requirement |
| **1. Jan 2028** | E-Rechnung B2B Send | Market requirement |

---

## 🔗 Related Files

| File | Purpose |
|------|---------|
| `.github/ISSUES_ONBOARDING_QUICK_START_GUIDES.md` | Full issue descriptions |
| `docs/by-role/*_QUICK_START.md` | Quick-start guides (8 files) |
| `.github/copilot-instructions.md` | Architecture & patterns |
| `docs/EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md` | 34-week timeline |
| `ROLE_BASED_DOCUMENTATION_MAP.md` | Documentation index by role |

---

## 🎓 Learning Path

Each issue provides:

1. **Week 1:** Foundation (core concepts)
2. **Week 2:** Deep dive (specific domain knowledge)
3. **Week 3:** Implementation (first hands-on task)

All with:
- ✅ Day-by-day tasks
- ✅ Reading recommendations
- ✅ Resources and links
- ✅ Code examples (where applicable)
- ✅ Completion criteria

---

## 📞 Support

If issues don't create:

1. **Verify GitHub CLI is installed:**
   ```bash
   gh version
   ```

2. **Verify you're authenticated:**
   ```bash
   gh auth status
   ```

3. **Check repository access:**
   ```bash
   gh repo view
   ```

4. **Manual fallback:** Use `.github/ISSUES_ONBOARDING_QUICK_START_GUIDES.md` to create issues manually in GitHub UI

---

## 🚀 Ready to Deploy

Both scripts are ready to run immediately:

```bash
# Make executable
chmod +x .github/CREATE_ONBOARDING_ISSUES.sh
chmod +x .github/CREATE_PROJECT_BOARD.sh

# Run both scripts
./.github/CREATE_ONBOARDING_ISSUES.sh
./.github/CREATE_PROJECT_BOARD.sh

# Verify
gh issue list --label "onboarding"
```

---

**Created:** 28. Dezember 2025  
**Status:** ✅ Ready for Production  
**Questions?** See `.github/copilot-instructions.md` or `ROLE_BASED_DOCUMENTATION_MAP.md`
