# Sprint Planning Update - GitHub Projects Integration

**Date:** 2025-12-30  
**Updated By:** @SARAH  
**Change Type:** Instructions Modification

---

## Summary

The sprint planning instructions have been modified to use **GitHub Projects (Planner)** as the primary sprint management system instead of standalone markdown documents.

### Key Changes

#### 1. **Planning Phase**
- ✅ Sprint projects created in GitHub Projects with custom fields
- ✅ User stories tracked as GitHub Issues (single source of truth)
- ✅ Custom fields: Points, Priority, Status, Epic, Sprint
- ✅ Views: Table (prioritization), Board (workflow), Roadmap (dependencies)

#### 2. **Execution Phase**
- ✅ Daily standups reference GitHub Projects board status
- ✅ Team members report on assigned GitHub issues
- ✅ Blockers tracked as GitHub issues with "blocker" label
- ✅ Pull requests linked to issues for traceability
- ✅ Status automation: PR opened → In Review, PR merged → Done

#### 3. **Closure Phase**
- ✅ Sprint review pulls data from GitHub Projects "Done" column
- ✅ Retrospective notes linked to specific issues and PRs
- ✅ Action items created as new GitHub issues for next sprint
- ✅ Metrics calculated from GitHub Projects (velocity, completion rate)

### GitHub Projects Integration Benefits

| Aspect | Benefit |
|--------|---------|
| **Single Source of Truth** | All sprint data in one system (issues, PRs, discussion) |
| **Automation** | Auto-move issues based on PR lifecycle |
| **Traceability** | Full history of changes, discussions, decisions |
| **Team Collaboration** | GitHub Discussions for standup notes and retros |
| **Visibility** | Real-time board views for team and stakeholders |
| **Scalability** | Handles multiple sprints and backlogs |
| **Integration** | Works with CI/CD, code reviews, merges |

### Configuration Steps

1. **Create Sprint Project** in GitHub Projects
2. **Add Custom Fields**: Points, Priority, Status, Epic, Sprint
3. **Create Views**: Table, Board, Roadmap
4. **Configure Automation**: PR-based status transitions
5. **Set Team Permissions**: Admin, Write, Read access levels

### What Changed

- ❌ Removed standalone sprint backlog markdown files
- ❌ Removed manual burndown chart tracking
- ✅ Added GitHub Projects configuration guide
- ✅ Added GitHub Issues linking strategy
- ✅ Added PR-to-issue automation rules
- ✅ Added GitHub Discussions for retrospectives

### Implementation Notes

- **Sprint planning prompt**: Updated with GitHub Projects workflows
- **Current sprint file** (`.ai/sprint/current.md`): Can reference GitHub Projects link
- **Daily standups**: Can pull data from GitHub Projects API/UI
- **Metrics**: Use GitHub Projects native metrics + custom reporting

---

## Next Steps

1. Set up first GitHub Projects sprint board
2. Configure custom fields and automation rules
3. Migrate current sprint items to GitHub Issues
4. Train team on using GitHub Projects for sprint tracking
5. Update CI/CD pipelines to link with issues/PRs

---

**Status:** ✅ Instructions Updated  
**Implementation:** Pending team setup  
**Reference:** [sprint-cycle.prompt.md](.github/prompts/sprint-cycle.prompt.md)
