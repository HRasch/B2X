---
docid: UNKNOWN-191
title: Start Feature.Prompt
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

# 🚀 START_FEATURE - New Feature Development Cycle

**Trigger**: User has a new feature request or user story
**Coordinator**: @SARAH
**Output**: Feature specification, task breakdown, team assignments

---

## Quick Start
```
@SARAH: /start-feature
Title: [Feature Title]
User Story: [As a..., I want..., so that...]
Priority: P0 | P1 | P2 | P3
```

---

## Process Flow

### 1️⃣ Requirements Gathering (@ProductOwner)
- Extract acceptance criteria from user story
- Define scope boundaries
- Identify dependencies
- Create initial requirements document in `.ai/requirements/`

### 2️⃣ Multi-Agent Analysis
- **@Backend**: Backend feasibility, API design, database impacts
- **@Frontend**: Frontend complexity, component requirements, UX implications
- **@Security**: Security considerations, auth requirements, compliance
- **@Architect**: Architectural impacts, service boundaries, design patterns

### 3️⃣ Consolidation (@SARAH)
- Resolve conflicts between agent analyses
- Create unified feature specification
- Generate task breakdown (backend tasks, frontend tasks, tests, docs)

### 4️⃣ Planning (@ScrumMaster)
- Estimate story points
- Assign team members
- Schedule into sprint
- Create GitHub issue

### 5️⃣ Implementation
- **@Backend** & **@Frontend** execute their assigned tasks
- **@QA** prepares test strategy
- **@Security** reviews security implementation

### 6️⃣ Delivery (@ScrumMaster)
- Code review (@TechLead)
- Testing verification (@QA)
- Documentation update
- Sprint closure

---

## Output Artifacts
- ✅ `.ai/requirements/{feature-id}.md` - Feature specification
- ✅ `.ai/sprint/{sprint-id}/tasks.md` - Task breakdown
- ✅ GitHub Issue with acceptance criteria
- ✅ Assignment to @Backend, @Frontend, @QA

---

## Next Steps
1. Define feature scope clearly
2. Identify dependencies early
3. Plan security review
4. Estimate effort accurately
5. Schedule implementation
