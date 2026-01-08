---
docid: WF-022
title: WF 011 TASK DISPATCH
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

---
docid: WF-011
title: Task Dispatch & Multi-Chat Coordination Workflow
owner: "@SARAH"
status: Active
last-updated: 2026-01-08
---

# 🎯 Task Dispatch & Multi-Chat Coordination Workflow

**Purpose**: Guide @SARAH in splitting workloads across specialized Copilot chats to minimize token waste and enable parallelization.

**Document ID**: `WF-011`

---

## Overview

Instead of one monolithic chat handling all work, we dispatch independent tasks to **specialized agent chats** that load only domain-specific context.

**Expected Result**:
- 73% reduction in tokens per task (45KB → 12KB)
- Parallel execution of independent work
- Predictable rate-limit management

---

## Decision Tree: When to Create New Chat

```
START: New work arrives
  ↓
Is this dependent on current chat's output?
  YES → Continue in current chat
  NO  → Is it a different domain?
        YES → Create new specialized chat
        NO  → Continue in current chat
        
Current chat context >20KB?
  YES → Create new chat to reset context
  NO  → Can continue
```

---

## Phase 1: Task Analysis (@SARAH)

When new work arrives (from GitHub issue, user request, etc.):

### 1. Decompose into Tasks

**Ask**:
- What's the single deliverable?
- What domain: backend, frontend, security, testing, architecture, devops?
- Can this run in parallel with other work?
- What's the dependency chain?

**Output**: Task decomposition map

```
Feature Request
├── TASK-001 @Backend  — Domain model design (P1, 2d)
├── TASK-002 @Frontend — Component design (P1, 2d) ← Can parallel with TASK-001
├── TASK-003 @Testing  — Integration tests (P1, 1d) ← Depends on TASK-001 & TASK-002
└── TASK-004 @Security — Vulnerability audit (P0, 4h) ← Can parallel with TASK-001-002
```

### 2. Identify Context Requirements

For **each task**, list minimal context:

```
TASK-001 (Backend):
  - ADR-001 (Wolverine decision)
  - KB-006 (Wolverine patterns)
  - INS-001 (Backend essentials)
  - INS-005 (Security - ALL tasks)
  - NOT: KB-007 (Vue.js), frontend instructions
```

### 3. Determine Rate-Limit Scheduling

Group tasks into **time slots** to avoid concurrent API saturation:

```
10:00-10:30  → Dispatch TASK-001 @Backend + TASK-002 @Frontend (different domains, parallel OK)
10:30-10:45  → COOLDOWN (no new intensive tasks)
10:45-11:15  → Dispatch TASK-004 @Security (parallel with TASK-001 backend work)
11:15-11:30  → COOLDOWN
11:30-12:00  → Dispatch TASK-003 @Testing (depends on TASK-001/002 complete)
```

---

## Phase 2: Task Creation (@SARAH)

### 1. Create Task Directory

```bash
mkdir -p .ai/tasks/task-001-backend-domain-model/
```

### 2. Create `brief.md`

Copy [TASK-BRIEF-TEMPLATE.md](BRIEF_TEMPLATE.md), fill in:
- Objective & acceptance criteria
- Scope (in/out)
- Context files to load (minimal)
- Domain & priority
- Depends on what?

**Critical**: Be specific about what NOT to load.

```markdown
# Task Brief

**Task ID**: TASK-001  
**Agent**: @Backend  
**Domain**: backend  
**Priority**: P1  

## Objective
Refactor Catalog domain model to follow [ADR-001] Wolverine handlers pattern.

## Acceptance Criteria
1. [ ] All command/query logic extracted from services
2. [ ] Handlers follow Wolverine naming convention
3. [ ] No service methods containing business logic
4. [ ] Tests passing, coverage >80%

## Context Files to Load
- ADR-001 (Wolverine decision rationale)
- KB-006 (Wolverine handler patterns) — use KB-MCP query instead of full embed
- INS-001 (Backend essentials)
- INS-005 (Security instructions)

## Do NOT Load
- KB-002 (dotnet features) — not relevant
- Frontend instructions
- Project structure docs
```

### 3. Create `progress.md`

Copy [TASK-PROGRESS-TEMPLATE.md](PROGRESS_TEMPLATE.md), initialize with 🟢 READY status.

### 4. Update `ACTIVE_TASKS.md`

Add task to "Current Active Tasks" section:

```markdown
## TASK-001 🟢 READY @Backend — Catalog Domain Refactor

**Assigned To**: @Backend  
**Domain**: backend  
**Priority**: P1  
**Started**: 2026-01-08  
**Expected Completion**: 2026-01-10  
**Status File**: `.ai/tasks/task-001-catalog-refactor/progress.md`  
**Blockers**: None

**Brief**: Refactor Catalog domain to use Wolverine handlers per [ADR-001].

**Decision**: Create dedicated @Backend chat. Load KB-006 + INS-001 only (via KB-MCP queries).
```

---

## Phase 3: Chat Dispatch (@SARAH)

### 1. Prepare Dispatch Message

```
Subject: [TASK-001] Catalog Domain Refactor — 2d estimate

@Backend, new task assigned:

Brief: c:\Users\Holge\repos\B2Connect\.ai\tasks\task-001-catalog-refactor\brief.md

Minimal Context to Load:
✓ ADR-001 (Wolverine decision)
✓ KB-006 via kb-mcp/search_knowledge_base (don't embed full article)
✓ INS-001 (backend-essentials.instructions.md)
✓ INS-005 (security.instructions.md)

What NOT to Load:
✗ Frontend instructions
✗ Full KB articles (query on-demand)
✗ Project structure docs

Acceptance Criteria (from brief):
1. All command/query logic extracted from services
2. Handlers follow Wolverine naming convention
3. No service methods containing business logic
4. Tests passing, coverage >80%

Update `.ai/tasks/task-001-catalog-refactor/progress.md` after each major step.

Questions? Reply in thread.
```

### 2. Update Task Status

Change ACTIVE_TASKS.md: 🟢 READY → 🟡 IN-PROGRESS

Mark progress.md: Update "Chat Sessions Used" with session details

### 3. Coordinate Rate-Limit Schedule

If dispatching parallel tasks:

```
@Backend: Dispatch TASK-001 (start 10:00)
@Frontend: Dispatch TASK-002 (start 10:00 — different domain, parallel OK)

COOLDOWN 10:30-10:45 — no new task dispatches

@Security: Dispatch TASK-004 (start 10:45 — audit task, lower API load)
```

---

## Phase 4: Agent Execution (Agent Chat)

### Agent Workflow

1. **Receive dispatch** with `.ai/tasks/task-{id}/brief.md` link
2. **Load minimal context**:
   ```
   ✓ Read brief.md (context already prepared by @SARAH)
   ✓ Load INS-00X (path-specific instructions from instructions/ folder)
   ✓ Query KB-MCP on-demand: kb-mcp/search_knowledge_base query="..."
   ✗ Don't load full KB articles
   ✗ Don't recreate context from previous chats
   ```
3. **Work on task** (design, code, tests)
4. **Update progress.md** after each significant output:
   - What was done
   - PR link (if applicable)
   - Next action
   - Any blockers discovered
5. **Complete task** when acceptance criteria ✅
6. **Notify @SARAH** with completion signal

---

## Phase 5: Task Completion (@SARAH)

When agent signals task complete:

### 1. Verify Acceptance Criteria
- [ ] All checkboxes in brief.md marked?
- [ ] Tests passing?
- [ ] PR approved?

### 2. Update ACTIVE_TASKS.md
```markdown
## TASK-001 ✅ COMPLETED @Backend — Catalog Domain Refactor

**Completed**: 2026-01-08 16:30 UTC
**Duration**: 4.5 hours
**PR**: https://github.com/...
**Tokens Used**: 14KB (estimated 15KB) ✅
```

### 3. Archive Task
Move to completed folder after 7 days:
```
.ai/tasks/COMPLETED_TASKS.md (append summary)
```

### 4. Trigger Next Task (if dependent)
If TASK-003 was blocked on TASK-001, dispatch it now.

---

## Phase 6: Task Cleanup & Archival (@SARAH)

**When**: After task completion + all QA sign-off complete (typically 1-7 days)

### 1. Final Artifact Consolidation

Before archiving, ensure all deliverables are captured:

```
task-001-catalog-refactor/
├── brief.md                 ✅ Keep (reference)
├── progress.md              ✅ Keep (history)
├── artifacts/
│   ├── PR-LINK.txt         ✅ Link to GitHub PR
│   ├── COMMITS.txt         ✅ Commit SHAs
│   ├── TEST-RESULTS.json   ✅ Final test results
│   └── COVERAGE.json       ✅ Code coverage report
└── CLEANUP_DATE.txt         ← Add this (when archived)
```

**Task**: Verify all artifacts linked in `artifacts/` folder

### 2. Update ACTIVE_TASKS.md

Change status to archived:

```markdown
## TASK-001 ✅ ARCHIVED @Backend — Catalog Domain Refactor

**Completed**: 2026-01-08 16:30 UTC  
**Archived**: 2026-01-15 09:00 UTC  
**Duration**: 4.5 hours  
**Tokens**: 14KB / 15KB estimated (94% accuracy) ✅  
**PR**: https://github.com/...

**Summary**: Successfully refactored Catalog domain to use Wolverine handlers.
See `.ai/tasks/task-001-catalog-refactor/` for details.
```

### 3. Move Task to Completed Archive

**Option A: Lightweight Archive (Recommended)**
```bash
# Keep task directory in .ai/tasks/ for 7 days, then move to archive
mv .ai/tasks/task-001-catalog-refactor .ai/tasks/archive/task-001-catalog-refactor-2026-01-08
```

**Option B: Summary Only**
```bash
# For older tasks (>30 days), create summary in COMPLETED_TASKS.md:
## TASK-001 (Archived) — Catalog Domain Refactor
- **Date**: 2026-01-08
- **Agent**: @Backend
- **Duration**: 4.5 hours
- **Outcome**: ✅ Complete
- **PR**: #1234
- **Tokens**: 14KB

[Full directory available in .ai/tasks/archive/]
```

### 4. Cleanup Task Directory

**What to Keep** (in archive):
- ✅ brief.md (reference for similar tasks)
- ✅ progress.md (learning & history)
- ✅ artifacts/ (links to final deliverables)
- ✅ CLEANUP_NOTES.md (lessons learned, if any)

**What to Remove**:
- ❌ Temporary exploration files
- ❌ Drafts not in final deliverable
- ❌ Large debug logs (>10MB)
- ❌ Intermediate work products

```bash
# Example cleanup
rm task-001-catalog-refactor/debug-logs.txt    # Large log file
rm task-001-catalog-refactor/scratch-notes.md  # Exploration notes
# Keep everything else
```

### 5. Record Metrics

Add to `.ai/logs/task-metrics.json`:

```json
{
  "task_id": "TASK-001",
  "name": "Catalog Domain Refactor",
  "domain": "backend",
  "start_date": "2026-01-08T10:15Z",
  "completion_date": "2026-01-08T16:30Z",
  "duration_hours": 4.5,
  "tokens_estimated": 15000,
  "tokens_actual": 14000,
  "efficiency": 0.93,
  "status": "completed",
  "pr_link": "https://github.com/...",
  "notes": "On budget, high quality"
}
```

**Metrics tracked**:
- Actual vs. estimated tokens (efficiency)
- Duration accuracy
- Domain-specific patterns
- Quality indicators

### 6. Update Rate-Limit Dashboard

Remove completed task from `.ai/logs/rate-limits/current-status.md`:

```markdown
## Previously Active Tasks (Archived)
- TASK-001 @Backend (completed, 14KB tokens) ✅

## New Capacity Available
- Rate-limit headroom: +15K tokens (from freed parallelization slot)
- Next task ready to dispatch: TASK-004 @Security
```

### 7. Generate Lessons Learned Summary

**If task had learnings or challenges**:

```markdown
## CLEANUP NOTE: TASK-001 Catalog Refactor

**Completed**: 2026-01-08  
**Lessons Captured**: YES

### Key Learnings
- Wolverine handler naming convention (see KB-006 update needed?)
- Test performance improved 20% after refactoring
- Integration test discovery time increased — need optimization

### For Future Similar Tasks
- Allocate extra time for integration test debugging
- Pre-load KB-006 (Wolverine patterns) — very relevant
- Monitor test execution time during refactoring

**Action Items**:
- [ ] Update KB-006 with new handler naming pattern
- [ ] Create performance baseline for Catalog tests
- [ ] Document integration test bottleneck
```

Update `.ai/knowledgebase/lessons.md` with any new learnings.

### 8. Cleanup Checklist

Before marking task complete:

- [ ] All PRs merged to main
- [ ] Tests passing in CI/CD
- [ ] Code review approved by @TechLead
- [ ] QA sign-off received
- [ ] Documentation updated
- [ ] Artifacts linked in `artifacts/` folder
- [ ] progress.md final version committed
- [ ] Metrics recorded in task-metrics.json
- [ ] Lessons learned captured (if any)
- [ ] Task directory cleaned (large files removed)
- [ ] ACTIVE_TASKS.md updated with ✅ ARCHIVED status
- [ ] Rate-limit dashboard updated (freed slot)

✅ All checked → Task cleanup complete

---

## Cleanup Schedule

**Daily**: None (automated)

**Weekly** (Friday):
- Review completed tasks from past week
- Archive tasks ready for archival (>7 days, all QA complete)
- Update COMPLETED_TASKS.md summary
- Verify metrics recorded

**Monthly** (Last Friday):
- Archive tasks >30 days old (move to `.ai/tasks/archive/2026-01/`)
- Generate monthly metrics report
- Identify process improvements from task data
- Update rate-limit forecasting model

---

## Archive Structure

```
.ai/tasks/
├── ACTIVE_TASKS.md              ← Current active tasks
├── COMPLETED_TASKS.md           ← Summary of all completed tasks
├── archive/
│   ├── 2026-01/                ← Monthly folders
│   │   ├── task-001-catalog-refactor/
│   │   ├── task-002-ui-components/
│   │   └── task-003-tests/
│   ├── 2025-12/
│   │   └── [previous month tasks]
│   └── ARCHIVE_INDEX.md         ← Search index for archives
```

Archives are searchable by:
- Task ID
- Domain (backend/frontend/security/etc.)
- Date range
- Status (completed/failed/archived)
- Keywords

---

## Emergency Cleanup (If Rate-Limit Crisis)

**If rate-limit exceeded, cleanup active tasks for recovery**:

```bash
# Immediate: Archive any completed-but-not-archived tasks
.ai/tasks/task-001/ → .ai/tasks/archive/task-001/

# This frees slots for new high-priority work without losing history
```

---

## Troubleshooting

### Task Cleanup Stuck
- Issue: Task marked complete but QA hasn't approved
- Solution: Add to `PENDING_QA` section in ACTIVE_TASKS.md
- Action: Don't archive until QA approves

### Missing Artifacts
- Issue: PR link not saved before cleanup
- Solution: Link to GitHub from COMPLETED_TASKS.md
- Action: Check git history if needed

### Metrics Inconsistency
- Issue: Actual tokens differ significantly from estimate
- Solution: Review token usage in chat transcripts
- Action: Update estimation model for that domain

---

**Cleanup Owner**: @SARAH  
**Automation**: Manual weekly (Phase 2 will automate)  
**Retention**: 12 months in `.ai/tasks/archive/`, then moved to `.ai/archive/historical/`

---

## Multi-Task Coordination: Full Example

**Scenario**: New feature request arrives

```
FRIDAY 10:00 — @SARAH Analysis
├─ Decompose into 4 tasks
├─ Plan rate-limit schedule
├─ Create TASK-001 through TASK-004 directories
└─ Create ACTIVE_TASKS.md entries

FRIDAY 10:15 — @SARAH Dispatch Wave 1
├─ Dispatch TASK-001 @Backend (can run 10:15-11:00)
├─ Dispatch TASK-002 @Frontend (parallel, 10:15-11:00)
└─ Update ACTIVE_TASKS.md: Both 🟡 IN-PROGRESS

FRIDAY 10:30-10:45 — COOLDOWN
└─ No new task dispatches
└─ Text-based status updates only

FRIDAY 10:45 — @SARAH Dispatch Wave 2
├─ Dispatch TASK-004 @Security (parallel with backend/frontend, 10:45-11:15)
└─ Update ACTIVE_TASKS.md: TASK-004 🟡 IN-PROGRESS

FRIDAY 11:15-11:30 — COOLDOWN
└─ Aggregating progress updates from agents

FRIDAY 11:30 — Check: TASK-001 & TASK-002 Complete?
└─ YES → Dispatch TASK-003 @Testing (depends on 1 & 2)
└─ NO → Wait, continue cooldown

FRIDAY 12:00 — Task Completion
├─ TASK-001 ✅, TASK-002 ✅, TASK-003 ✅, TASK-004 ✅
├─ @SARAH consolidates into PR summary
└─ Feature ready for merge
```

**Token Usage**:
- TASK-001 @Backend: 12KB (vs. 45KB in single chat)
- TASK-002 @Frontend: 11KB
- TASK-004 @Security: 8KB
- Total: 31KB across 3 parallel agents = ~60% savings vs. sequential

---

## Rate-Limit Safety Rules

### Maximum Concurrency
- **Level 1**: 2 agents (different domains) running simultaneously
- **Level 2**: If rate limit detected, reduce to 1 agent, 30-min cooldown

### Intensive Operations
High token load → schedule sequentially:
- Database refactoring
- Large file rewrites
- Security audits
- Full test suites

### Between-Task Cooldowns
- 10-15 minutes after high-intensity task
- No new chat dispatches during cooldown
- Text-based progress updates only (no interactive chats)

---

## Token Efficiency Checklist

For each task, verify:

- [ ] Brief.md specifies what NOT to load
- [ ] Agent using KB-MCP queries instead of full article embeds
- [ ] Path-specific instructions loaded only (GL-044)
- [ ] Initial context <15KB
- [ ] Artifacts linked (no context duplication in deliverables)

---

## Troubleshooting

### Task Blocked
1. Update progress.md: 🟠 BLOCKED
2. Document blocker & dependency
3. Re-prioritize other tasks during wait
4. Notify @SARAH in ACTIVE_TASKS.md

### Rate Limit Hit
1. Pause new chat dispatches immediately
2. Current agents finish current action only
3. 30-minute cooldown
4. Log incident: `.ai/logs/rate-limits/incident-{date}.md`

### Agent Needs Clarification
1. Reply in task dispatch thread
2. @SARAH updates brief.md if clarification affects acceptance criteria
3. Continue work

---

## Files Reference

| File | Purpose | Owner |
|------|---------|-------|
| `.ai/tasks/ACTIVE_TASKS.md` | Central dispatch board | @SARAH |
| `.ai/tasks/task-{id}/brief.md` | Task spec & acceptance criteria | @SARAH |
| `.ai/tasks/task-{id}/progress.md` | Real-time execution tracking | Agent |
| `.ai/tasks/task-{id}/artifacts/` | Generated files, PR links | Agent |
| `BRIEF_TEMPLATE.md` | Template for new briefs | Reference |
| `PROGRESS_TEMPLATE.md` | Template for progress tracking | Reference |

---

**Owned by**: @SARAH (Coordinator)  
**Last Updated**: 2026-01-08  
**Next Review**: 2026-01-22
