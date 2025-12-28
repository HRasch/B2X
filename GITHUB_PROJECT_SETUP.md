# 📊 GitHub Project Dashboard Setup Guide

**Purpose:** Create a single Kanban board to track all B2Connect work across Phases 0-3 organized by sprints.

---

## Quick Start

### Option 1: Automated Setup (Recommended)

```bash
# Run the automation script
./scripts/create-github-project.sh

# Follow the prompts to complete setup
```

### Option 2: Manual Setup via GitHub Web UI

1. **Create Project**
   - Navigate to: `https://github.com/your-org/B2Connect/projects`
   - Click "New project"
   - Name: `B2Connect Roadmap (Phases 0-3)`
   - Template: **Table**

2. **Configure Fields**
   - **Status** (Single Select): Backlog, In Progress, In Review, Done, Blocked
   - **Sprint** (Single Select): S1, S2, S3, S4, S5, S6, S7, S8
   - **Priority** (Single Select): P0 (Critical), P1 (High), P2 (Medium), P3 (Low)
   - **Phase** (Single Select): Phase 0, Phase 1, Phase 2, Phase 3
   - **Component** (Text): P0.1, P0.2, etc.

---

## Kanban Board Structure

### Status Column Values

```
📋 Backlog (Not started)
🔄 In Progress (Currently working)
👀 In Review (PR/Testing)
✅ Done (Complete)
🔴 Blocked (Can't proceed)
```

### Sprint Planning

**Phase 0: Compliance Foundation (Weeks 1-6)**
- Sprint 1: P0.1 (Audit) + P0.2 (Encryption) startup
- Sprint 2: P0.3 (Incident) + P0.4 (Network) startup  
- Sprint 3: P0.5 (Keys) completion
- Sprint 4: P0.6 (E-Commerce) + P0.7 (AI Act) + P0.9 (E-Rechnung)

**Phase 1: MVP + Compliance (Weeks 7-14)**
- Sprint 5: F1.1 (Auth) startup
- Sprint 6: F1.2 (Catalog) + F1.3 (Checkout)
- Sprint 7: F1.4 (Admin Dashboard) + testing
- Sprint 8: P1 testing & hardening

**Phase 2: Scale (Weeks 15-24)**
- Sprint 9+: P2.1-P2.4 components

---

## Issue Template for GitHub

When creating issues, use this format for consistency:

```markdown
## Title Format: [P0.1] Audit Logging - EF Core Interceptor

### Description
Brief description of the work

### User Story
As a [role], I want [feature], so that [benefit]

### Acceptance Criteria
- [ ] Criterion 1
- [ ] Criterion 2
- [ ] Tests: X+ test cases
- [ ] Code review approved
- [ ] Documentation updated

### Technical Details
- Component: P0.1 (Audit Logging)
- Sprint: S1
- Priority: P0 (Critical)
- Effort: 40 hours

### Related Issues
- Blocks: #123
- Depends on: #456
```

---

## Viewing the Dashboard

### Web URL
```
https://github.com/your-org/B2Connect/projects/[PROJECT-NUMBER]
```

### Quick Filters

```bash
# View issues for current sprint
gh issue list --label "sprint:S1" --state open

# View P0 critical issues
gh issue list --label "priority:P0" --state open

# View issues in progress
gh issue list --label "status:in-progress" --state open
```

---

## Setting Up Automations

### GitHub Actions Workflow

Create `.github/workflows/project-automation.yml`:

```yaml
name: Project Automation

on:
  issues:
    types: [opened, labeled, unlabeled]
  pull_request:
    types: [opened, synchronize, ready_for_review]

jobs:
  update-project:
    runs-on: ubuntu-latest
    steps:
      - name: Update project on PR
        if: github.event.pull_request
        uses: actions/github-script@v6
        with:
          script: |
            console.log('PR #' + context.payload.pull_request.number);
            // Add to project and move to "In Review"

      - name: Update project on issue open
        if: github.event.action == 'opened'
        uses: actions/github-script@v6
        with:
          script: |
            console.log('Issue #' + context.payload.issue.number);
            // Add to project and keep in "Backlog"
```

---

## Best Practices

### Labeling Strategy

Use consistent labels:

```
Component: p0.1, p0.2, p0.3, p0.4, p0.5, p0.6, p0.7, p0.8, p0.9
Type: feature, bug, improvement, documentation, security
Priority: p0-critical, p1-high, p2-medium, p3-low
Sprint: sprint-s1, sprint-s2, ..., sprint-s8
Status: backlog, in-progress, in-review, done, blocked
Phase: phase-0, phase-1, phase-2, phase-3
```

### Updating Status

Move issues through the kanban as work progresses:

```
Backlog → In Progress (assign to developer)
         ↓
      In Review (PR created)
         ↓
       Done (merged)
```

### Sprint Velocity Tracking

- Track burndown by sprint
- Record story points / effort estimates
- Review velocity at sprint retrospective

---

## Useful GitHub CLI Commands

```bash
# Create an issue and add to project
gh issue create \
  --title "[P0.1] Audit Logging - EF Core Interceptor" \
  --body "Implementation of audit logging..." \
  --label "p0.1,sprint-s1,type:feature" \
  --milestone "Phase 0"

# List all P0 issues
gh issue list --label "component:p0.1" --state open

# View project items
gh project item-list 1 --owner your-org

# Update issue
gh issue edit <issue-number> --add-label "status:in-progress"

# View sprint burndown (using labels)
gh issue list --label "sprint:s1" --state open --json title,state
```

---

## Integration with Sprints

### Sprint Kickoff
1. Create sprint-specific milestone in GitHub
2. Label all planned issues with sprint identifier
3. Move all sprint issues to "In Progress" column in project
4. Announce sprint in team chat

### Sprint Review
1. Filter project by sprint
2. Review all "Done" items
3. Verify acceptance criteria met
4. Update project documentation

### Sprint Retrospective
1. Analyze velocity (issues completed vs planned)
2. Identify blockers (review "Blocked" column)
3. Plan improvements for next sprint

---

## References

- [GitHub Projects Documentation](https://docs.github.com/en/issues/planning-and-tracking-with-projects)
- [gh CLI Project Commands](https://cli.github.com/manual/gh_project)
- [B2Connect Roadmap](./EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md)
- [Copilot Instructions](./github/copilot-instructions.md)

---

**Document Owner:** Architecture Team  
**Last Updated:** 28. Dezember 2025

