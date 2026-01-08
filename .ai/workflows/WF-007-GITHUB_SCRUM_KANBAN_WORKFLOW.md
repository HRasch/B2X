---
docid: WF-018
title: WF 007 GITHUB_SCRUM_KANBAN_WORKFLOW
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

# GitHub-Based Development Workflow

## Überblick

Ein integrierter Entwicklungs-Workflow, der SCRUM/KANBAN Prinzipien mit GitHub-nativen Features kombiniert. Dieser Workflow definiert:
- Sprint-Struktur (2-Wochen SCRUM)
- Backlog-Management über GitHub Issues
- Kanban Board für kontinuierliche Workflows
- PR-basierte Code Review & Integration
- GitHub Projects für Tracking

## Tool-Stack

- **GitHub Issues**: Backlog, User Stories, Bugs, Tasks
- **GitHub Projects**: Sprint Boards, Kanban Boards
- **GitHub Pull Requests**: Code Review, Integration
- **GitHub Discussions**: Team Collaboration
- **GitHub Milestones**: Sprint Planning
- **GitHub Labels**: Kategorisierung, Prioritäten

## Rollen & Responsibilities

### Product Owner (ProductOwner.agent.md)
- Backlog-Priorisierung in GitHub Issues
- Sprint Planning (mit Team & ScrumMaster)
- Acceptance Criteria in Issues definieren
- Review in Sprint Review

### Scrum Master (ScrumMaster.agent.md)
- Sprint-Verwaltung in GitHub Projects
- Daily Standup Koordination
- Impediment-Tracking
- Board-Maintenance

### Development Team (DevelopmentTeam.agent.md + Spezialisten)
- Sprint-Planung, Schätzung
- Issue-Implementation via PRs
- Code-Review durchführen
- Deployment

### Spezialisten (Backend, Frontend, DevOps, QA, Security, TechLead)
- Expertise-spezifische Tasks
- Fachliche Code Reviews
- Domain-spezifische Checklisten

## Workflow Phases

### Phase 1: Product Backlog Management

**GitHub Issues Setup:**
```
Each User Story / Task / Bug:
- Title: Clear, actionable
- Description: 
  * As a [role]
  * I want [feature]
  * So that [benefit]
- Acceptance Criteria (Checklist)
- Labels: type, priority, domain
- Milestone: (empty or future sprint)
- Assignee: (none, awaiting sprint planning)
```

**Labels Taxonomy:**
```
Type: feature, bug, enhancement, documentation, technical-debt
Priority: P0-critical, P1-high, P2-medium, P3-low
Domain: backend, frontend, devops, security, infra
Status: backlog, ready-for-sprint, in-progress, in-review, done
Sprint: s1-2024, s2-2024, etc.
```

**Product Owner Responsibilities:**
- Regular backlog refinement
- Prioritization (drag in GitHub Projects)
- Add acceptance criteria
- Remove obsolete items

### Phase 2: Sprint Planning

**Sprint Cadence:**
- **Duration**: 2 weeks
- **Planning**: Monday 10am (1h Planning, 30min Buffer)
- **Daily Standup**: 9:30am (15 min)
- **Review**: Friday 4pm (2h)
- **Retrospective**: Friday 5pm (1.5h)

**Sprint Planning Process:**

1. **Product Owner Presents** (30min)
   - Top-prioritized backlog items
   - Context & business value
   - Any dependencies

2. **Team Estimates** (20min)
   - Story Points using t-shirt sizes: XS(1), S(2), M(3), L(5), XL(8)
   - Raise concerns early
   - Ask clarifying questions

3. **Team Commits** (10min)
   - Team agrees on realistic sprint goal
   - Issues moved to Sprint Milestone
   - Issues assigned to team members

**GitHub Setup für Sprint:**
```
GitHub Milestone: "Sprint [Number] - [Dates]"
Issues mit Label "Sprint: s1-2024"
GitHub Project: Sprint Board created
Board Columns: Backlog, Ready, In Progress, In Review, Done
```

### Phase 3: Sprint Execution

**Daily Standup (15 min):**
- What did I do yesterday?
- What do I do today?
- What blocks me?
- GitHub Project serves as reference

**Work Process:**

1. **Pick Issue from Board**
   - Move from "Ready" to "In Progress"
   - Self-assign
   - Create branch: `feature/issue-123-short-name`

2. **Develop & Test**
   - Follow coding standards from `.ai/guidelines/`
   - Write tests alongside code
   - Commit messages: `fix(#123): description`

3. **Push & Create PR**
   - PR Title: `[Domain] Fix/Feature description (Closes #123)`
   - PR Description:
     ```
     ## What
     Brief description
     
     ## Why
     Context & motivation
     
     ## How
     Technical approach
     
     ## Testing
     How was this tested?
     
     Closes #123
     ```

4. **Code Review (Async)**
   - Assignee: Domain-appropriate reviewer (Backend → Backend Agent)
   - Reviewers check:
     - Coding standards
     - Tests present & passing
     - No security issues
     - Performance implications
     - Documentation updated

5. **Address Feedback**
   - Pushes address feedback
   - Conversations resolved
   - Re-request review

6. **Merge & Deploy**
   - Squash-and-merge (clean history)
   - Automated tests must pass
   - Issue auto-closes via "Closes #123"
   - Move Issue to "Done" (auto or manual)

**GitHub Project Board:**
```
Backlog     → Ready     → In Progress → In Review → Done
  ↓           ↓           ↓              ↓          ↓
(prioritized) (estimated) (assigned)  (under review) (complete)
```

### Phase 4: Sprint Review (Friday 4pm)

**Process:**
1. **Demo Completed Work** (30min)
   - Each issue demo'd
   - Acceptance criteria verified
   - PO accepts or requests changes

2. **Backlog Refinement** (30min)
   - Review upcoming sprint candidates
   - Break down large items
   - Add acceptance criteria
   - Re-prioritize if needed

3. **Metrics & Retrospective Prep** (30min)
   - Velocity calculation (Story Points completed)
   - Burndown review
   - Issues carried over noted

**GitHub Automation:**
- Generate Sprint Report from closed issues
- Calculate velocity
- Identify blockers

### Phase 5: Sprint Retrospective (Friday 5pm)

**Process (1.5h):**
1. **What went well?** (20min)
   - Celebrate wins
   - Identify patterns

2. **What could improve?** (20min)
   - Identify issues
   - Root cause analysis

3. **Action Items** (20min)
   - Pick 1-3 improvements for next sprint
   - Assign owners
   - Create GitHub issues for process improvements

4. **Demo learnings** (10min)
   - How will we apply learnings?

**GitHub Issues für Process Improvements:**
```
Label: "process-improvement"
Milestone: "Sprint [N+1]"
Description: Experiment & success criteria
```

## Kanban Mode (Parallel to SCRUM)

For continuous workflows (hotfixes, urgent bugs):

**Kanban Board:**
```
Backlog → Prioritized → In Progress → In Review → Done
(all)      (urgent)      (assigned)   (review)   (released)
```

**Rules:**
- WIP Limit: 3 items in progress max
- Hotfixes bypass sprint, go direct
- Must not exceed 20% of team capacity
- Backfill sprint when Kanban items done

## Branch Strategy

```
main (production)
  ↓
develop (staging)
  ↑
feature/issue-123-name
feature/issue-124-name
bugfix/issue-125-name
```

**Process:**
1. Create branch from `develop`
2. PR targets `develop`
3. After sprint, `develop` merged to `main` (release)

## Definition of Done

An issue is "Done" when:
- [ ] Code written and tests passing
- [ ] Code reviewed and approved (domain-specific)
- [ ] Tests > 80% coverage
- [ ] Security review passed (if applicable)
- [ ] Documentation updated
- [ ] Acceptance criteria met
- [ ] No open conversations in PR
- [ ] Merged to develop

## Metrics & Tracking

SARAH tracks:
- **Velocity**: Story Points completed per sprint
- **Burndown**: Issues completed vs sprint goal
- **Cycle Time**: Time from issue created to done
- **Code Review Time**: Time in review before merge
- **Lead Time**: Time from PR creation to merge
- **Defect Rate**: Bugs found in production

**GitHub Queries for Metrics:**
```
Velocity: closed:>=SPRINT_START closed:<=SPRINT_END
Burndown: milestone:S1-2024 is:closed
```

## Integration Points

### Automation (GitHub Actions)
- [ ] Run tests on PR
- [ ] Lint check
- [ ] Security scan (SAST)
- [ ] Coverage report
- [ ] Auto-close stale issues
- [ ] Generate sprint report

### Issue Templates
```
Feature Template:
- User Story
- Acceptance Criteria
- Definition of Done

Bug Template:
- Reproduction steps
- Expected vs actual
- Environment

Task Template:
- Description
- Acceptance Criteria
```

## Role-Specific Workflows

### Backend Agent Workflow
```
1. Pick Backend issue from Sprint Board
2. Create feature branch
3. Implement API/Database changes
4. Write integration tests
5. PR → Self-review → Backend Agent review
6. Deploy to staging
7. Merge after approval
```

### Frontend Agent Workflow
```
1. Pick Frontend issue from Sprint Board
2. Wait for API ready (Backend)
3. Implement UI components
4. Write component tests
5. PR → Self-review → Frontend Agent review
6. Visual QA (screenshots)
7. Merge after approval
```

### QA Agent Workflow
```
1. Review completed issues
2. Execute test plan from acceptance criteria
3. Create bugs if issues found (link to original)
4. Verify fixes
5. Sign-off for done
```

### Security Agent Workflow
```
1. Review PRs labeled "security"
2. Run security scans
3. Check for common vulnerabilities
4. Add security checklist to code review
5. Approve/reject with findings
```

## Escalation & Exceptions

### Blocked Issues
```
If issue blocked:
1. Add "blocked" label
2. Comment: "Blocked by #[issue_number]: [reason]"
3. Notify ScrumMaster
4. ScrumMaster finds resolution
```

### Urgent Hotfixes
```
1. Create issue labeled "hotfix"
2. Skip to top of Kanban board
3. Fast-track code review
4. Bypass sprint if critical
```

### Scope Creep
```
If during dev: more work discovered
1. Create separate issue
2. Link as "related to"
3. Don't expand current issue scope
4. Decide in next planning
```

## Integration with AI Team

### SARAH's Role
- Monitors workflow adherence
- Ensures Scrum ceremonies happen
- Coordinates cross-team dependencies
- Reviews process for improvements

### Agent Coordination
- Backend/Frontend coordinate via PR comments
- Dependencies tracked in issues ("blocks", "relates")
- ScrumMaster escalates conflicts
- TechLead reviews architecture questions

## Anti-Patterns to Avoid

⚠️ **Common Issues:**
- ❌ Issues too large (break down)
- ❌ Acceptance criteria unclear (define clearly)
- ❌ PRs without context (link to issues)
- ❌ Skipping code review (quality gate)
- ❌ Issues left in "In Progress" at sprint end (close or carry over)
- ❌ No retrospectives (continuous improvement)
- ❌ Ignoring blockers (escalate)

## Getting Started

1. Create GitHub Project "Development Workflow"
2. Configure board columns
3. Add issue templates
4. Set up GitHub Actions for automation
5. Run first sprint planning
6. Execute first sprint
7. Do retrospective, iterate

## References

- `.github/agents/ProductOwner.agent.md` - PO responsibilities
- `.github/agents/ScrumMaster.agent.md` - SM responsibilities
- `.github/agents/DevelopmentTeam.agent.md` - Team responsibilities
- `.ai/guidelines/process/` - Process guidelines
- `.ai/workflows/` - Workflow implementations
