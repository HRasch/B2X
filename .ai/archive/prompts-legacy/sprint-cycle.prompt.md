---
docid: UNKNOWN-027
title: Sprint Cycle.Prompt
owner: @DocMaintainer
status: Archived
created: 2026-01-08
---

# 🔄 SPRINT_CYCLE - Sprint Planning, Execution & Retrospective

**Trigger**: Sprint planning, sprint execution, sprint retrospective
**Coordinator**: @ScrumMaster
**Output**: GitHub Projects (Planner) configuration, daily standups, retrospective notes
**Integration**: GitHub Projects Planner + GitHub CLI (gh)
**CLI Guide**: See [WF-006] WF-006-GITHUB_CLI_SPRINT_HOWTO.md for detailed commands

---

## Quick Start

### Using GitHub CLI
```bash
# Create sprint project
gh project create --title "Sprint 12 (Jan 6-17, 2025)" \
  --description "Sprint Goal: Feature Development" \
  --owner @yourorg

# List sprint issues
gh issue list --label sprint-12 --state open --sort updated

# Create sprint issue
gh issue create --title "FEAT-001: Feature Name" \
  --label "feature,sprint-12" \
  --assignee @backend-dev \
  --project "Sprint 12"

# See full guide: [WF-006] .ai/workflows/WF-006-GITHUB_CLI_SPRINT_HOWTO.md
```

### Manual Start
```
@ScrumMaster: /sprint-cycle
Phase: [planning | execution | retrospective]
Sprint: [Sprint number and dates]
Team: [Team members]
Capacity: [Available story points]
GitHub Project: [Project name/URL]
```

---

## Sprint Phases

### Phase 1️⃣: Sprint Planning (1 day)

**Objective**: Define sprint goals, select work, estimate effort using GitHub Projects (Planner) + GitHub CLI

**GitHub CLI Quick Commands:**
```bash
# Step 1: Create sprint project
gh project create --title "Sprint 12 (Jan 6-17, 2025)" \
  --owner yourorg --format "table"

# Step 2: List backlog items
gh issue list --repo owner/repo --label "backlog" --state open

# Step 3: Create sprint issue
gh issue create --title "FEAT-001: Feature Name" \
  --label "feature,sprint-12" --assignee @dev --project "Sprint 12"

# Step 4: Add issues to project
gh issue list --label sprint-12 --json number --jq '.[] | .number' | \
  xargs -I {} gh project item-add PROJECT_ID --number {}

# Step 5: Set priority labels
gh issue edit 456 --add-label "points-8,priority-p0"

# See full guide: [WF-006] .ai/workflows/WF-006-GITHUB_CLI_SPRINT_HOWTO.md
```

#### Pre-Planning Meeting
```markdown
## Sprint Planning - Pre-Meeting

### Backlog Refinement Review
- [ ] GitHub Project backlog prioritized
- [ ] User stories have acceptance criteria
- [ ] Stories properly formatted in GitHub Issues
- [ ] Estimates added to issues (via custom field)
- [ ] Dependencies identified and linked
- [ ] Blocks identified and resolved

### Capacity Planning
- Team members: [Names]
- Available capacity: [Story points]
- Vacation/PTO: [Dates and impact]
- Buffer for support: [%]
- Realistic capacity: [Story points]

### GitHub Project Setup
- [ ] Sprint project created in GitHub Projects
- [ ] Custom fields configured (Points, Status, Priority)
- [ ] Views created (Backlog, In Progress, Review, Done)
- [ ] Team access configured
- [ ] Automation rules enabled
```

#### Sprint Planning Meeting (2-3 hours)
```markdown
## Sprint Planning Meeting Notes

### Sprint Goal
[1-2 sentences describing the sprint's main objective]

### Sprint Theme
[Category of work: feature, bugfix, technical debt, compliance, etc.]

### Compliance Requirements
Review compliance documents before planning:
- [ACCESSIBILITY_COMPLIANCE_REPORT.md](../../ACCESSIBILITY_COMPLIANCE_REPORT.md) - WCAG 2.1
- [ISSUE_TEMPLATE_STORE_LEGAL_COMPLIANCE.md](../../ISSUE_TEMPLATE_STORE_LEGAL_COMPLIANCE.md) - EU legal
- [.ai/knowledgebase/governance.md](../../.ai/knowledgebase/governance.md) - Requirements governance
- [compliance-integration.prompt.md](compliance-integration.prompt.md) - Implementation guidance

**Check**: Which compliance categories apply to this sprint?
- [ ] WCAG 2.1 AA (accessibility)
- [ ] GDPR (data protection)
- [ ] PAngV (store/pricing)
- [ ] Security standards
- [ ] Audit logging

### GitHub Project Configuration
- **Project URL**: [Link to GitHub Projects Planner board]
- **Sprint Dates**: [Start date] - [End date]
- **Status Workflow**: Backlog → In Progress → Review → Done
- **Custom Fields**: Points, Priority, Effort, Epic, Compliance Status
- **Automation**: Auto-move on PR review, close on merge

### Selected User Stories (from GitHub Project)
All items added to the sprint project with:
- Story title and description (GitHub Issue)
- Estimated story points (custom field)
- Priority level
- Assigned team member
- **Compliance category** (if applicable: wcag, gdpr, pangv, security, audit)
- Links to dependencies and related issues

### Known Risks
[Potential blockers and mitigation]
**Include**: Compliance blockers (legal review, security audit, accessibility testing)

### Success Criteria
[How we measure sprint success]
- All issues in Done column
- All PRs merged and passing compliance gates
- Test coverage ≥ 80%
- Zero critical security findings
- Compliance requirements verified (see COMPLIANCE_INTEGRATION.md)
```

#### Sprint Backlog (GitHub Projects Planner View)
```markdown
## Sprint Backlog - Sprint 12 (Jan 6-17, 2025)

**GitHub Project Link**: [https://github.com/orgs/org/projects/XX]

### Sprint Configuration
- **Total Capacity**: 120 story points
- **Allocated**: 115 story points
- **Buffer**: 5 points
- **Team Size**: 5 members
- **Sprint Duration**: 10 days
- **Compliance Reviews**: Assigned to @Security, @Legal, @TechLead

### Backlog Items (Managed in GitHub Projects)

All items tracked in the GitHub Projects board with:
- **Issue**: Unique GitHub issue number
- **Title**: Clear, actionable story title
- **Points**: Estimated story points
- **Compliance Status**: (Needs Review → In Review → Approved → Done)
- **Priority**: P0, P1, P2, P3
- **Status**: Backlog, In Progress, In Review, Done
- **Assignee**: Team member responsible
- **Labels**: feature, bugfix, technical-debt, documentation
- **Epic**: Related epic (if applicable)
- **Dependencies**: Linked to blocking/blocked issues

### Example Structure in GitHub Projects:
```
Issues in Sprint View:
├── Feature Epic 1
│   ├── FEAT-001: Login API (8 pts) - @Backend - P0
│   ├── FEAT-002: JWT tokens (8 pts) - @Backend - P0
│   ├── FEAT-003: Login UI (5 pts) - @Frontend - P0
│   └── TEST-001: Auth tests (5 pts) - @QA - P1
├── Feature Epic 2
│   ├── FEAT-010: Profile API (8 pts) - @Backend - P0
│   ├── FEAT-011: Profile UI (8 pts) - @Frontend - P0
│   └── TEST-010: Profile tests (8 pts) - @QA - P1
├── Technical Debt
│   ├── TECH-001: Refactor auth (12 pts) - @Backend - P2
│   └── TECH-002: Update deps (8 pts) - @DevOps - P2
└── Bug Fixes
    ├── BUG-001: Login timeout (5 pts) - @Backend - P1
    └── BUG-002: Mobile layout (5 pts) - @Frontend - P1
```

### Team Assignments (in GitHub Projects)
Assignments made directly on issues via GitHub Projects:
- [ ] All issues assigned to responsible team members
- [ ] Capacity verified (no person over 100%)
- [ ] Dependencies resolved
- [ ] Labels applied appropriately
```

---

### Phase 2️⃣: Sprint Execution (Daily)

#### Daily Standup (15 minutes)
**Time**: 9:30 AM daily
**Participants**: Entire team
**Format**: What I did, what I'm doing, what blocks me
**Tool**: GitHub CLI + GitHub Projects board

**GitHub CLI Daily Standup:**
```bash
# Run daily standup script (see [WF-006] .ai/workflows/WF-006-GITHUB_CLI_SPRINT_HOWTO.md)
bash scripts/daily-standup.sh

# Or get quick status:
echo "📊 Sprint 12 Status"
gh issue list --repo owner/repo --label sprint-12 --state open --sort updated
gh pr list --repo owner/repo --label sprint-12 --state open
gh issue list --repo owner/repo --label sprint-12,blocker --state open
```

**Manual Standup Template:**

```markdown
## Daily Standup Notes - [Date]

### Attendees
@Backend, @Frontend, @QA, @DevOps, @TechLead

### GitHub Projects Board Status
- **Link**: [GitHub Projects Planner URL]
- **Current Sprint Progress**: [X points completed / Y total points]
- **Issues In Progress**: [Count from "In Progress" column]
- **Issues In Review**: [Count from "In Review" column]

### Status Updates (Reference GitHub Projects Issues)

#### @Backend
- **Yesterday**: [Issue(s) completed] - Points: [X]
- **Today**: [Issue(s) in progress] - Points: [Y]
- **Blockers**: [Any obstacles, linked to blocking issues]
- **GitHub Issues**: [Links to assigned open issues]

#### @Frontend
- **Yesterday**: [Issue(s) completed] - Points: [X]
- **Today**: [Issue(s) in progress] - Points: [Y]
- **Blockers**: [Any obstacles, linked to blocking issues]
- **GitHub Issues**: [Links to assigned open issues]

#### @QA
- **Yesterday**: [Issue(s) completed] - Points: [X]
- **Today**: [Issue(s) in progress] - Points: [Y]
- **Blockers**: [Any obstacles]
- **GitHub Issues**: [Links to test/validation issues]

### Team Blockers Identified (Log in GitHub Issues)
1. [Issue #123: Blocker description] - Owner: [@assigned] - Target: [date]
2. [Issue #124: Blocker description] - Owner: [@assigned] - Target: [date]

### Action Items (Create GitHub Issues as needed)
- [ ] [Action with owner and deadline] (Issue #XXX)
- [ ] [Action with owner and deadline] (Issue #XXX)
```

#### Update Issue Status (GitHub CLI)

```bash
# Move issue from "In Progress" to "Review" using labels
gh issue edit 456 --remove-label "in-progress" --add-label "in-review"

# Close issue (mark as done)
gh issue close 456 --comment "Merged in PR #789"

# Link PR to issue
gh pr create --title "Implement feature" \
  --body "Fixes #456" \
  --label sprint-12

# View all issues by status
gh issue list --repo owner/repo \
  --label "sprint-12,in-progress" \
  --state open
```

#### Track Blockers (GitHub CLI)

```bash
# Create a blocker issue
gh issue create \
  --title "BLOCKER: S3 Configuration Not Complete" \
  --body "## Impact
Blocks FEAT-012 (Profile picture upload)

## Owner: @devops
## Target Resolution: Jan 2 by EOD
## Workaround: Use local file storage" \
  --label "blocker,sprint-12"

# View all blockers in sprint
gh issue list --repo owner/repo \
  --label "sprint-12,blocker" \
  --state open
```
```markdown
## Sprint Progress - Sprint 12

### GitHub Projects Dashboard
**Primary View**: GitHub Projects → [Sprint Name] → Table/Board view
- Monitor points in each status column
- Track velocity automatically
- View upcoming blockers via linked issues

### Daily Status via GitHub Projects
| Date | Completed Pts | In Progress Pts | Remaining Pts | Velocity Trend |
|---|---|---|---|---|
| Day 1 (Mon) | 0 | 0 | 115 | On track |
| Day 2 (Tue) | 8 | 15 | 92 | On track |
| Day 3 (Wed) | 13 | 20 | 82 | On track |

### Completed Items
- ✅ FEAT-001: Login API (8 pts)
- ✅ FEAT-003: Login UI (5 pts)

### In Progress
- 🔄 FEAT-002: JWT token management (8 pts) - 50% complete
- 🔄 FEAT-010: Profile API (8 pts) - 25% complete

### At Risk
- ⚠️ FEAT-012: Profile picture upload (8 pts) - Blocked on S3 setup
- ⚠️ BUG-001: Login timeout (5 pts) - Root cause still investigating

### Not Started
- FEAT-011: Profile UI
- TECH-001: Refactor auth service
```

#### Blocker Resolution
```markdown
## Blocker Log

### Active Blockers

**Blocker 1**: S3 Configuration Not Complete
- Identified: Day 2
- Impact: Blocks FEAT-012 (Profile picture upload)
- Owner: @DevOps
- Target Resolution: Day 3 by EOD
- Status: In Progress
- Workaround: [Temp workaround if applicable]

**Blocker 2**: Third-party API Latency
- Identified: Day 3
- Impact: Affecting BUG-001 root cause analysis
- Owner: @Backend + @TechLead
- Target Resolution: Day 4
- Status: Researching
- Workaround: Increase timeout threshold temporarily
```

---

### Phase 3️⃣: Sprint Closure

#### Sprint Review (Demonstration + Compliance Verification)
**Duration**: 1-2 hours
**Audience**: Stakeholders, Product Owner, Team, @Security, @Legal (if compliance items)
**Reference**: GitHub Projects completed items + GitHub CLI reporting + Compliance checklist

**Pre-Review Compliance Gate (CRITICAL):**
```bash
# VERIFY: All compliance reviews completed
echo "🔒 Compliance Verification"
echo "WCAG 2.1 AA issues closed:"
gh issue list --label "sprint-12,wcag-2.1-review" --state closed | wc -l

echo "GDPR issues closed:"
gh issue list --label "sprint-12,gdpr" --state closed | wc -l

echo "Security reviews completed:"
gh issue list --label "sprint-12,security" --state closed | wc -l

echo "PAngV compliance verified:"
gh issue list --label "sprint-12,pangv" --state closed | wc -l

# FAIL: If any compliance issues still open
gh issue list --label "sprint-12" --label "wcag-2.1-review,gdpr,security,pangv" --state open
# Result should be: ZERO

# Get all completed issues in sprint
gh issue list --repo owner/repo \
  --label "sprint-12" \
  --state closed \
  --sort updated \
  --json number,title,labels

# Count completed points
gh issue list --repo owner/repo \
  --label "sprint-12" \
  --state closed \
  --json number | wc -l

# Get merged PRs for sprint
gh pr list --repo owner/repo \
  --label "sprint-12" \
  --state merged \
  --sort updated

# See full reporting in [WF-006] .ai/workflows/WF-006-GITHUB_CLI_SPRINT_HOWTO.md: "Generate Sprint Metrics"
```

**Sprint Review Markdown Template:**

### Metrics Presented (from GitHub Projects)
- **Committed Points**: 120
- **Completed Points**: 80 ✅
- **Velocity**: 80 points (target: 80)
- **Completion Rate**: 67%
- **Bugs Found in Demo**: 2 (low priority, added to next sprint backlog)
- **Feature Quality**: Good

### Sprint Data from GitHub Projects
- Filter Issues by Sprint label
- Show completion timeline (Issue created → PR → merged)
- Display burndown data if available via GitHub Projects views
```

#### Sprint Retrospective (Blameless)
**Duration**: 1 hour
**Reference**: GitHub Projects issues and PR history
**Participants**: Entire team
**Format**: What went well, what didn't, what to improve

**Create Retrospective Issue (GitHub CLI):**
```bash
# Create retrospective notes issue
gh issue create \
  --title "Sprint 12 Retrospective" \
  --body "## What Went Well ✅
- Strong team collaboration
- Proactive testing
- Good issue linking

## What Could Be Better ⚠️
- S3 setup delays (Issue #115)
- Third-party API latency (Issue #118)
- Large PR reviews

## Action Items for Sprint 13
- [ ] Pre-plan infrastructure (@devops)
- [ ] Add load testing (@qa)
- [ ] Establish PR size guidelines (@techlead)" \
  --label "retrospective,sprint-12" \
  --assignee @scrummaster

# See full reporting in [WF-006] .ai/workflows/WF-006-GITHUB_CLI_SPRINT_HOWTO.md: "Create Retrospective Issues"
```

**Retrospective Markdown Template:**

```markdown
## Sprint Retrospective - Sprint 12

**GitHub Project**: [Link to project board]
**Sprint Duration**: [Date] - [Date]
**Total Points Completed**: [X points]

### What Went Well ✅
1. Strong team collaboration on GitHub discussions
2. Good use of issue linking for dependencies
3. Proactive testing with automated PR checks
4. [Reference specific GitHub issues/PRs]

### What Could Be Better ⚠️
1. S3 Setup planning (Issue #115 - blocked)
2. Third-party API timeout handling (Issue #118 - took extra days)
3. Large PRs difficult to review (multiple issues >400 lines)
4. [Reference specific GitHub issues/PRs with difficulty]

### Blockers & Root Causes
- Infrastructure decisions delayed sprint progress (Issue #115)
- External API limitations not discovered during planning
- PR review bottleneck due to size

### Action Items (Logged as GitHub Issues for Sprint 13)
| Issue | Action | Owner | Sprint | Priority |
|-------|--------|-------|--------|----------|
| #130 | Pre-plan infrastructure changes with DevOps | @DevOps | Sprint 13 | P1 |
| #131 | Add load testing to test strategy | @QA | Sprint 13 | P1 |
| #132 | Establish PR size guidelines & automate checks | @TechLead | Sprint 13 | P2 |

### Process Improvements
- [ ] Create GitHub discussion for next sprint blockers
- [ ] Set up GitHub automation for PR size warnings
- [ ] Add infrastructure planning checklist to sprint planning
- [ ] Schedule pre-sprint infrastructure review

### Team Health
- Morale: Good
- Skill Development: Team learned S3 integration patterns
- Communication: Excellent (GitHub discussions)
- Work-Life Balance: Good
- Collaboration Quality: High
```

---

## GitHub Projects Integration Guide

### Setting Up GitHub Projects for Sprint Planning

#### 1. Create Sprint Project
```
Name: Sprint 12 (Jan 6-17, 2025)
Type: Table view (for sprint tracking) + Board view (for workflow)
Visibility: Internal
```

#### 2. Configure Custom Fields
- **Story Points**: Number field (0, 3, 5, 8, 13, 21)
- **Priority**: Single select (P0, P1, P2, P3)
- **Epic**: Single select (Feature, Bugfix, Technical Debt, etc.)
- **Sprint**: Single select (Sprint 12, Sprint 13, etc.)
- **Status**: Workflow field (Backlog → In Progress → In Review → Done)

#### 3. Views for Sprint Management
- **Table View**: All issues sorted by priority and assignee
- **Board View**: Kanban columns (Backlog, In Progress, Review, Done)
- **Roadmap View**: Timeline view for dependencies
- **Burndown View**: Points remaining (if available via GitHub Projects)

#### 4. Automation Rules
- Auto-move issues when PR is opened (→ In Review)
- Auto-move issues when PR is merged (→ Done)
- Auto-add linked PRs to issue
- Auto-update parent epic status based on children

---

## GitHub CLI Integration Guide

### Quick Reference Commands

**Sprint Planning:**
```bash
# Create sprint project
gh project create --title "Sprint 12" --owner yourorg

# Create issues for sprint
gh issue create --title "FEAT-001: ..." --label sprint-12 --project "Sprint 12"

# Add issues to project
gh project item-add PROJECT_ID --number ISSUE_NUM
```

**Daily Execution:**
```bash
# Get sprint status
gh issue list --label sprint-12 --state open

# View blockers
gh issue list --label sprint-12,blocker --state open

# Create standup note
gh issue create --title "Standup - $(date)" --label standup,sprint-12
```

**Sprint Closure:**
```bash
# Generate metrics
gh issue list --label sprint-12 --state closed --json number

# Create retrospective
gh issue create --title "Retrospective - Sprint 12" --label retrospective,sprint-12
```

**Full command reference: [WF-006-GITHUB_CLI_SPRINT_HOWTO.md](../../.ai/workflows/WF-006-GITHUB_CLI_SPRINT_HOWTO.md)**

---

## GitHub Projects Setup (UI)

#### 1. Create Sprint Project
```
Name: Sprint 12 (Jan 6-17, 2025)
Type: Table view (for sprint tracking) + Board view (for workflow)
Visibility: Internal
```

#### 2. Configure Custom Fields
- **Story Points**: Number field (0, 3, 5, 8, 13, 21)
- **Priority**: Single select (P0, P1, P2, P3)
- **Epic**: Single select (Feature, Bugfix, Technical Debt, etc.)
- **Sprint**: Single select (Sprint 12, Sprint 13, etc.)
- **Status**: Workflow field (Backlog → In Progress → In Review → Done)

#### 3. Views for Sprint Management
- **Table View**: All issues sorted by priority and assignee
- **Board View**: Kanban columns (Backlog, In Progress, Review, Done)
- **Roadmap View**: Timeline view for dependencies
- **Burndown View**: Points remaining (if available via GitHub Projects)

#### 4. Automation Rules
- Auto-move issues when PR is opened (→ In Review)
- Auto-move issues when PR is merged (→ Done)
- Auto-add linked PRs to issue
- Auto-update parent epic status based on children

#### 5. Team Access & Permissions
- All team members: Read + Update permissions
- @ScrumMaster: Admin (manage sprint, close issues)
- @ProductOwner: Write (add/prioritize items)

#### 6. GitHub CLI Integration
- Use `gh` commands for automation and scripting
- Automate daily standups with `scripts/daily-standup.sh`
- Generate reports programmatically
- See [WF-006-GITHUB_CLI_SPRINT_HOWTO.md](../../.ai/workflows/WF-006-GITHUB_CLI_SPRINT_HOWTO.md) for examples

---

## Sprint Documentation Templates

### Sprint Planning Output (GitHub Projects Board + GitHub CLI)
- [ ] Sprint project created in GitHub Projects
- [ ] Sprint goal documented in project description
- [ ] Custom fields configured (Points, Priority, Status, Epic, Sprint)
- [ ] All backlog items added as GitHub issues
- [ ] Issues assigned to team members
- [ ] Estimates (points) added to all issues
- [ ] GitHub CLI scripts ready (`scripts/daily-standup.sh`, reporting, etc.)
- [ ] Dependencies linked (GitHub issue links)
- [ ] Risks documented in issue descriptions
- [ ] Success criteria added to project description
- [ ] Automation rules enabled

### Sprint Execution Artifacts (GitHub Projects + GitHub CLI)
- [ ] Daily standup notes created via GitHub CLI (`gh issue create`)
- [ ] Sprint board with active status updates
- [ ] Blocker issues created with "blocker" label
- [ ] Pull requests linked to issues
- [ ] Code review queue monitored (In Review column)
- [ ] Testing status in issue comments/PR checks
- [ ] Deployment status tracked (Done column)
- [ ] GitHub CLI used for daily status checks and updates

### Sprint Closure Artifacts (GitHub Projects + GitHub CLI)
- [ ] Sprint review feedback (GitHub Discussions or issue comments)
- [ ] Retrospective issue created via GitHub CLI with structured notes
- [ ] Metrics generated via GitHub CLI (`gh issue list`, `gh pr list`)
- [ ] Action items created as GitHub issues for next sprint
- [ ] Velocity trending calculated from closed issues
- [ ] Team health assessment in retrospective issue

---

**Sprint Cycle Complete** ✅  
*All artifacts managed through GitHub Projects + GitHub CLI automation*

**Essential Documentation:**
- 📖 [Sprint Cycle Prompt](sprint-cycle.prompt.md) - Main sprint workflow
- 🛠️ [GitHub CLI How-To Guide](../../.ai/workflows/WF-006-GITHUB_CLI_SPRINT_HOWTO.md) - CLI command reference & examples
- 📊 Sample Scripts - See `scripts/` folder in project root
