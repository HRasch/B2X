---
docid: TASKS-001
title: Active Task Registry & Dispatch Board
owner: "@SARAH"
status: Active
last-updated: 2026-01-08
---

# 📋 Active Task Registry

**Purpose**: Central coordination point for multi-chat workload distribution. Tasks routed to specialized agent chats to minimize token waste and enable parallelization.

**Maintenance**: Updated by @SARAH during task dispatch and completion.

---

## Task Dispatch Rules

### When to Create New Task
- **Independent work**: Frontend and backend can run in parallel
- **Domain-specific**: Task requires KB/instruction loading for one domain
- **Rate-limit protection**: Sequential scheduling to prevent API throttling

### When to Continue in Current Chat
- **Blocker dependencies**: Waiting for previous task output
- **Same domain**: Backend→Backend, Frontend→Frontend
- **Token-efficient**: <5KB context reuse

---

## 🟢 ACTIVE TASKS

### Format
```
## [TASK-ID] [STATUS] [Agent] — [Brief Description]

**Assigned To**: @Agent  
**Domain**: backend|frontend|security|devops|testing|architecture  
**Priority**: P0|P1|P2|P3  
**Started**: YYYY-MM-DD  
**Expected Completion**: YYYY-MM-DD  
**Status File**: `.ai/tasks/task-{id}/progress.md`  
**Blockers**: [None] or [Link to blocker task]

**Brief**: Single sentence describing deliverable
```

### Example (Template)
```
## TASK-001 🟡 IN-PROGRESS @Backend — Refactor Catalog Domain

**Assigned To**: @Backend  
**Domain**: backend  
**Priority**: P1  
**Started**: 2026-01-08  
**Expected Completion**: 2026-01-10  
**Status File**: `.ai/tasks/task-001-catalog-refactor/progress.md`  
**Blockers**: None

**Brief**: Extract Wolverine handlers from Catalog domain into separate modules per [ADR-021].

**Decision**: Create dedicated Backend chat, load KB-006 (Wolverine patterns) + INS-001 (backend essentials).
```

---

## Current Active Tasks

### [NONE]
*No active tasks at this time. Ready to dispatch.*

---

## Task Status Legend

| Icon | Status | Meaning |
|------|--------|---------|
| 🟢 | READY | Waiting for dispatch |
| 🟡 | IN-PROGRESS | Actively being worked |
| 🟠 | BLOCKED | Waiting on dependency |
| 🔴 | CRITICAL | P0, needs immediate attention |
| ✅ | COMPLETED | Done, ready for QA/merge |

---

## Rate-Limit Schedule Template

**SARAH manages this schedule to prevent concurrent intensive operations:**

```
Monday:
  09:00-10:00  → @Backend chat batch (30 min cooldown after)
  10:30-11:30  → @Frontend chat batch (parallel OK)
  12:00-13:00  → @Testing validation (parallel OK)
  
  (Cooldown 14:00-14:15)
  
  14:15-15:00  → @Security audit
  15:15-16:00  → @Architecture review
  
Tuesday-Friday: [Similar staggered schedule]
```

**Cooldown Rules**:
- 10-15 minutes between high-intensity agent switches
- Maximum 2 agents active simultaneously
- Text-based status updates only (no interactive chats during cooldown)

---

## Cleanup & Archival Process

**See [WF-012] Task Cleanup & Archival Procedure for detailed steps.**

### Timeline
```
✅ Task Complete (agent signals)
  ↓ (1 hour: verify completion)
Verify QA Sign-Off
  ↓ (1-7 days: wait for approvals)
Consolidate Artifacts
  ↓ (1 day: gather all outputs)
Record Metrics
  ↓ (1 day: log to task-metrics.json)
Archive Task
  ↓ (move to .ai/tasks/archive/YYYY-MM/)
Retain for 12 Months
```

### Weekly Cleanup (@SARAH Fridays)

- [ ] Review completed tasks from past week
- [ ] Verify all PRs merged to main
- [ ] Confirm QA approvals
- [ ] Archive tasks (all checks passed)
- [ ] Record metrics in weekly report
- [ ] Update COMPLETED_TASKS.md
- [ ] Generate lessons learned (if any)
- [ ] Publish weekly summary

### Archive Structure

```
.ai/tasks/
├── ACTIVE_TASKS.md              ← Current tasks only
├── COMPLETED_TASKS.md           ← Summary index
├── archive/
│   ├── 2026-01/                ← Monthly folders
│   │   ├── task-001-{slug}/
│   │   ├── task-002-{slug}/
│   │   └── ARCHIVE_INDEX.md
│   └── 2025-12/
└── ARCHIVE_RETENTION.md         ← Policy (12 months retention)
```

Completed Tasks Archive
→ Moved to `.ai/tasks/archive/YYYY-MM/` after 7-14 days  
→ Summarized in `.ai/tasks/COMPLETED_TASKS.md`  
→ Searchable via archive index
→ Retained for 12 months

---

## Integration with Issues

**Linking**: Each task maps to GitHub issue(s):
```
Task → GitHub Issue → `.ai/issues/{id}/progress.md` (agent writes updates)
     ↓
    ACTIVE_TASKS.md (SARAH aggregates status)
     ↓
    PR/Release notes
```

---

## Template: Create New Task

When dispatching new work:

1. **Generate task ID**: Next sequential number (TASK-001, TASK-002, etc.)
2. **Create subdirectory**: `.ai/tasks/task-{id}-{slug}/`
3. **Initialize files**:
   ```
   task-{id}-{slug}/
   ├── brief.md           (Task overview & acceptance criteria)
   ├── instructions.md    (Path-specific rules for domain)
   ├── progress.md        (Agent updates this in real-time)
   └── artifacts/         (Generated files, links to PRs)
   ```
4. **Update ACTIVE_TASKS.md**: Add entry to Current Active Tasks
5. **Dispatch chat**: Route @Agent with minimal context

---

## Token Efficiency Metrics

**Track per task**:
- Initial context size (KB)
- Final deliverable size (KB)
- Total tokens consumed
- Rate-limit headroom used

**Target**: <15KB context per chat session (vs. 45KB single-chat baseline)

---

## How Agents Use This

### Agent Workflow:
1. **Receive dispatch from @SARAH** with link to `.ai/tasks/task-{id}/brief.md`
2. **Load minimal context**: Path-specific instructions only (GL-044)
3. **Query KB on-demand**: Use KB-MCP instead of full article embeds
4. **Update progress.md** after each significant action
5. **Link artifacts**: PRs, commits, generated files

### Agent Does NOT Do:
- ❌ Load full project context
- ❌ Re-read all KB articles
- ❌ Upload full instruction files
- ❌ Recreate context from previous chats

---

## SARAH Coordination Checklist

- [ ] Task identified & prioritized
- [ ] Dependency chain verified (no circular blocks)
- [ ] Domain assigned (backend/frontend/security/etc.)
- [ ] Rate-limit slot available in schedule
- [ ] Task subdirectory created (`.ai/tasks/task-{id}-{slug}/`)
- [ ] Brief.md written with acceptance criteria
- [ ] Relevant ADR/issue linked
- [ ] @Agent notified with dispatch message
- [ ] ACTIVE_TASKS.md updated
- [ ] Task marked IN-PROGRESS

---

## Emergency Protocols

### Rate Limit Hit
1. Pause new chat dispatches immediately
2. Current active agents finish current action only (no new tasks)
3. 30-minute cooldown before resuming
4. Log incident: `.ai/logs/rate-limits/incident-{date}.md`

### Task Blocked
1. Update ACTIVE_TASKS.md with 🟠 BLOCKED status
2. Document blocker & dependency in task's progress.md
3. Re-prioritize other tasks during wait time
4. Notify dependent agents when blocker resolves

### Agent Overload
1. Split large task into smaller sub-tasks
2. Create task chain: TASK-001 → TASK-002 → TASK-003
3. Complete & merge one task before starting next (sequential)
4. Document task dependencies in ACTIVE_TASKS.md

---

**Owned by**: @SARAH  
**Last Review**: 2026-01-08  
**Next Review**: 2026-01-22
