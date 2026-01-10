---
docid: WF-023
title: WF 012 TASK CLEANUP
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

---
docid: WF-012
title: Task Cleanup & Archival Procedure
owner: "@SARAH"
status: Active
last-updated: 2026-01-08
---

# 🧹 Task Cleanup & Archival Procedure

**Document ID**: `WF-012`  
**Purpose**: Ensure completed tasks are properly archived, metrics captured, and knowledge preserved.  
**Owner**: @SARAH (Coordinator)

---

## Overview

Task cleanup is the final phase of the task lifecycle. After completion & QA sign-off, tasks are archived with full history preserved for future reference and metrics analysis.

**Goals**:
- ✅ Preserve all artifacts and learnings
- ✅ Capture metrics for process improvement
- ✅ Free up rate-limit slots by archiving
- ✅ Enable searchability for similar future tasks
- ✅ Document lessons learned

---

## Cleanup Timeline

```
Task Completion (✅ COMPLETED)
    ↓ (immediate: 1 hour)
Verify QA Sign-Off
    ↓ (1-7 days: wait for all sign-offs)
Consolidate Artifacts
    ↓ (1 day)
Record Metrics
    ↓ (1 day)
Archive Task
    ↓ (retain for 12 months)
Move to Historical Archive
```

---

## Phase 1: Immediate Completion (1 Hour)

When agent signals task complete:

### 1. Mark in progress.md

```markdown
**Status**: ✅ COMPLETED  
**Completed**: 2026-01-08 16:30 UTC  
**Duration**: 4.5 hours (vs. 4 hours estimated)
**Tokens Used**: 14KB (vs. 15KB estimated)
**Quality**: High ✅
```

### 2. Link Final Artifacts

Ensure `artifacts/` folder contains:

```
task-001/artifacts/
├── PR-LINK.txt           ← GitHub PR URL
├── FINAL-PR-#1234.md     ← PR description (copy)
├── COMMITS.txt           ← List of commit SHAs
├── TEST-RESULTS.json     ← Final test run results
└── COVERAGE.json         ← Code coverage snapshot
```

### 3. Update ACTIVE_TASKS.md

```markdown
## TASK-001 ✅ COMPLETED @Backend — Catalog Domain Refactor

**Status**: ✅ COMPLETED  
**Completed**: 2026-01-08 16:30 UTC  
**Duration**: 4.5 hours  
**Tokens**: 14KB / 15KB estimated (93% accuracy) ✅  
**PR**: https://github.com/B2Connect/B2X/pull/1234  
**Next Phase**: Awaiting QA sign-off (7 days max)
```

---

## Phase 2: Consolidation (1-7 Days)

While waiting for QA and merge approvals:

### 1. Gather All Outputs

Collect from various locations:
- ✅ GitHub PR (link)
- ✅ Commit messages (link)
- ✅ Test results (from CI/CD)
- ✅ Code coverage (from CI/CD)
- ✅ Performance metrics (if applicable)

```bash
# Example: Link GitHub artifacts
echo "https://github.com/B2Connect/B2X/pull/1234" > task-001/artifacts/PR-LINK.txt
echo "14abc1d,24def2e,34ghi3f" > task-001/artifacts/COMMITS.txt
```

### 2. Create Cleanup Notes

In `task-001/CLEANUP_NOTES.md`:

```markdown
# Task Cleanup Notes — TASK-001

**Task**: Catalog Domain Refactor  
**Agent**: @Backend  
**Status**: Completed & waiting QA approval  
**Cleanup Date**: 2026-01-08

## Completion Quality

**Acceptance Criteria**: ✅ All met
- ✅ All Wolverine handlers extracted
- ✅ Naming convention followed (see KB-006)
- ✅ No service methods with business logic
- ✅ Tests passing (94% coverage)

**Code Quality**: ✅ Excellent
- No new linting warnings
- StyleCop: All rules satisfied
- Performance: +15% faster vs. before

## Key Learnings

1. **Wolverine Pattern Clarity**: KB-006 needs clarification on handler naming
   - Action: Update KB-006 with new examples
   
2. **Test Performance**: Integration test discovery took 3x expected time
   - Action: Profile test suite, create bottleneck issue
   
3. **Estimation Accuracy**: 4.5h actual vs. 4h estimated (112%)
   - Action: Add +0.5h buffer for Wolverine refactors

## Artifacts Preserved

- PR: #1234
- Commits: 14abc1d..34ghi3f
- Test Results: 94% coverage
- Performance: +15% improvement

## Metrics Summary

| Metric | Value | Target | Status |
|--------|-------|--------|--------|
| Tokens | 14KB | 15KB | ✅ 93% |
| Duration | 4.5h | 4h | ✅ 112% |
| Coverage | 94% | >80% | ✅ |
| Performance | +15% | Neutral | ✅ Bonus |

---

## Suggestions for Future Tasks

- Allocate +0.5h for Wolverine refactors
- Pre-load KB-006 + KB-020 (ArchUnitNET patterns)
- Schedule integration test profiling separately
- Monitor test suite performance trend
```

### 3. Document Exceptions (if any)

If task differed from plan:

```markdown
## Deviations from Original Plan

**Change**: Added performance optimization as bonus work
**Reason**: Discovered low-hanging fruit during refactor
**Impact**: Took 30 min extra, but delivered 15% perf gain
**Decision**: Worth it — improved user experience
**Approval**: @TechLead approved in PR review

---

**Change**: KB-006 needed clarification mid-task
**Reason**: Naming convention ambiguous for edge cases
**Impact**: 1h investigation + doc update
**Decision**: Proactively fixed KB to prevent future confusion
**Outcome**: KB-006 improved for next agents
```

---

## Phase 3: Archive (7-14 Days)

After all QA & merge approvals complete:

### 1. Finalize progress.md

Add final status:

```markdown
---

## Final Status: ✅ ARCHIVED

**Merged**: 2026-01-10 14:00 UTC (merged to main)  
**QA Approval**: ✅ Approved by @QA  
**Code Review**: ✅ Approved by @TechLead  
**Archive Date**: 2026-01-15 10:00 UTC  
**Archive Location**: `.ai/tasks/archive/2026-01/task-001-catalog-refactor/`

---
```

### 2. Move Task Directory

```bash
# Archive to monthly folder
mkdir -p .ai/tasks/archive/2026-01/
mv .ai/tasks/task-001-catalog-refactor/ .ai/tasks/archive/2026-01/
```

### 3. Update ACTIVE_TASKS.md

Move from "Current Active Tasks" → move to end with archive note:

```markdown
---

## ARCHIVED TASKS (Last 30 Days)

### TASK-001 ✅ ARCHIVED — Catalog Domain Refactor
- **Completed**: 2026-01-08
- **Archived**: 2026-01-15
- **Domain**: @Backend
- **Duration**: 4.5 hours
- **Tokens**: 14KB / 15KB (93% accuracy)
- **PR**: #1234
- **Quality**: ✅ Excellent
- **Archive**: `.ai/tasks/archive/2026-01/task-001-catalog-refactor/`
```

### 4. Record in COMPLETED_TASKS.md

```markdown
## TASK-001 (Completed & Archived)

**Name**: Catalog Domain Refactor  
**Agent**: @Backend  
**Date**: 2026-01-08  
**Duration**: 4.5 hours  
**Tokens**: 14KB / 15KB (93%)  
**PR**: https://github.com/B2Connect/B2X/pull/1234  
**Status**: ✅ Merged to main  
**Quality**: ✅ High  

**Summary**: Successfully extracted Wolverine handlers from Catalog domain. Added performance optimization (+15% improvement). Updated KB-006 with clarifications.

**Archive**: `.ai/tasks/archive/2026-01/task-001-catalog-refactor/`

[📋 Full progress.md](./archive/2026-01/task-001-catalog-refactor/progress.md)
```

---

## Phase 4: Metrics Recording (1 Day)

### 1. Log to task-metrics.json

Location: `.ai/logs/task-metrics.json`

```json
{
  "task_id": "TASK-001",
  "name": "Catalog Domain Refactor",
  "agent": "@Backend",
  "domain": "backend",
  "priority": "P1",
  "status": "completed",
  "dates": {
    "created": "2026-01-08T10:00Z",
    "started": "2026-01-08T10:15Z",
    "completed": "2026-01-08T16:30Z",
    "archived": "2026-01-15T10:00Z"
  },
  "duration": {
    "hours": 4.5,
    "estimated_hours": 4.0,
    "accuracy_percent": 112
  },
  "tokens": {
    "estimated": 15000,
    "actual": 14000,
    "efficiency_percent": 93
  },
  "quality": {
    "test_coverage": 94,
    "coverage_target": 80,
    "code_quality": "excellent",
    "performance_impact": "+15%"
  },
  "deliverables": {
    "pr_number": 1234,
    "pr_link": "https://github.com/B2Connect/B2X/pull/1234",
    "commits": 3,
    "coverage_percent": 94
  },
  "meta": {
    "learned": true,
    "lessons_count": 3,
    "deviations": 2,
    "bonus_work": true
  }
}
```

### 2. Aggregate Weekly Report

Create `.ai/logs/task-metrics-weekly-2026-01-08.json`:

```json
{
  "week": "2026-01-08 to 2026-01-14",
  "tasks_completed": 4,
  "total_duration_hours": 18.5,
  "avg_duration_hours": 4.6,
  "total_tokens": 52000,
  "avg_tokens_per_task": 13000,
  "token_target": 15000,
  "efficiency_percent": 87,
  "parallelization_percent": 45,
  "incidents": 0,
  "tasks": [
    "TASK-001", "TASK-002", "TASK-003", "TASK-004"
  ],
  "insights": {
    "slowest_domain": "backend",
    "most_efficient": "frontend",
    "highest_quality": "TASK-001"
  }
}
```

---

## Phase 5: Lessons Learned (1-2 Days)

If task revealed new patterns or challenges:

### 1. Update .ai/knowledgebase/lessons.md

```markdown
## Lesson: Wolverine Handler Pattern Clarification

**Date**: 2026-01-08 (TASK-001 cleanup)  
**Source**: Catalog Domain Refactor (@Backend)  
**Category**: Patterns  
**Priority**: Medium

### The Issue
KB-006 (Wolverine Patterns) lacked clarity on handler naming convention for edge cases. Caused 1h investigation during TASK-001.

### The Solution
Updated KB-006 with concrete examples:
- Query handlers: `[Command]Query` (e.g., `GetProductsQuery`)
- Command handlers: `[Action]Command` (e.g., `CreateProductCommand`)
- Event handlers: `On[Event]` (e.g., `OnProductCreated`)

### Impact
- Future Wolverine refactors save ~1h on pattern clarification
- Estimated savings: 5-10h per sprint (5-10 similar tasks)

### Action Items
- [x] KB-006 updated
- [x] Example PR linked (#1234)
- [ ] Mention in next backend standup
```

### 2. Update Estimation Models

If pattern emerges:

```markdown
## Estimation Update: Wolverine Refactors

**Date**: 2026-01-15  
**Source**: TASK-001 completion analysis

### Previous Estimate
- Duration: 4 hours
- Tokens: 15KB

### New Estimate (based on TASK-001 actual)
- Duration: 4.5 hours (**+0.5h buffer**)
  - Add time for KB clarification (avg 1h per task)
  - Add time for integration testing (avg 0.5h)
- Tokens: 14KB (actually **lower**, keep at 15KB for safety)

### Why Changed
- Wolverine patterns need pre-research
- Integration tests often reveal edge cases
- Real usage: 4.5h (vs. 4h estimated)

### Future Wolverine Tasks
- Allocate: 4.5 hours minimum
- Pre-load: KB-006, KB-020, ADR-001
- Budget: +1h for KB clarification
```

---

## Weekly Cleanup Schedule (@SARAH)

Every Friday at 17:00 UTC:

### Checklist

- [ ] Review completed tasks from past week
- [ ] Verify all PRs merged to main
- [ ] Confirm QA approvals received
- [ ] Archive tasks ready (all checks passed)
- [ ] Move to `.ai/tasks/archive/YYYY-MM/`
- [ ] Record metrics in `.ai/logs/task-metrics-weekly-{date}.json`
- [ ] Update COMPLETED_TASKS.md with this week's completions
- [ ] Generate lessons learned (if any)
- [ ] Publish weekly summary to status page
- [ ] Verify rate-limit dashboard updated (freed slots)

### Output: Weekly Summary

```markdown
# Weekly Cleanup Report — Week of 2026-01-08

**Completed & Archived**: 4 tasks  
**Total Duration**: 18.5 hours  
**Total Tokens**: 52KB (avg 13KB/task, target 15KB) ✅  
**Efficiency**: 87%  
**Quality**: All high ✅  

## Tasks Archived
- TASK-001: Catalog Refactor (14KB, 4.5h) ✅
- TASK-002: UI Components (12KB, 4h) ✅
- TASK-003: Integration Tests (15KB, 5h) ✅
- TASK-004: Security Audit (11KB, 5.5h) ✅

## Insights
- Backend domain: 1h slower than estimate (factor in future)
- Frontend domain: 20% faster than estimate ⚡
- Parallelization rate: 45% (2-3 tasks in parallel)
- Rate-limit incidents: 0 ✅

## Next Week Recommendations
- Increase frontend task allocation (more efficient)
- Add +1h buffer to backend estimates
- Try 3-task parallelization (rate limit has headroom)
```

---

## Archive Retention Policy

### Keep (12 Months)
- ✅ progress.md (history)
- ✅ brief.md (reference)
- ✅ PR links
- ✅ Commit hashes
- ✅ Test results

### Move to Historical (12+ Months)
After 12 months, move to:
```
.ai/archive/historical/2025/task-001-{slug}/
```

(Searchable but not in active task directory)

### Purge (After 24 Months)
Remove if:
- No ongoing learnings
- Superseded by new patterns
- Storage space needed

Otherwise: Keep indefinitely (reference value)

---

## Troubleshooting

### Task Stuck in Cleanup

**Problem**: Task ready to archive but QA approval delayed  
**Solution**: Create temporary "pending-qa" status:
```
TASK-001 ✅ COMPLETED (pending QA approval)
Archive: After QA approval (expected 2026-01-20)
```

### Missing Artifacts

**Problem**: PR deleted or CI results disappeared  
**Solution**: Document what's missing & why:
```
TASK-001/artifacts/
├── PR-LINK.txt (DELETED — PR merged & cleaned up)
├── COMMITS.txt (RESTORED from git log)
└── TEST-RESULTS.json (ARCHIVED from CI job #5432)
```

### Metrics Inconsistency

**Problem**: Token usage doesn't match estimate  
**Solution**: Document discrepancy:
```json
{
  "tokens_estimated": 15000,
  "tokens_actual": 14000,
  "note": "KB queries more efficient than expected"
}
```

---

## Automation Opportunities (Phase 2+)

- [ ] Auto-move completed tasks to archive after 7 days
- [ ] Auto-generate metrics reports
- [ ] Auto-update lessons.md with patterns
- [ ] Archive cleanup reminder (Slack bot)
- [ ] Monthly dashboard showing archived tasks

---

**Owner**: @SARAH  
**Related**: [WF-011] Task Dispatch | [WF-013] Agent Support & Escalation | [WF-014] Deployment Guide | [QS-002] Quick Start | [PROGRESS_TEMPLATE.md]
**Next Review**: 2026-01-22
