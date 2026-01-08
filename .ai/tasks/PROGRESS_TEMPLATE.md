---
docid: TASK-003
title: PROGRESS_TEMPLATE
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

---
docid: TASK-PROGRESS-TEMPLATE
title: Task Progress Tracking Template
owner: "@SARAH"
status: Template
---

# Task Progress Tracking

**Task ID**: TASK-XXX  
**Agent**: @[AgentName]  
**Status**: [🟢 READY | 🟡 IN-PROGRESS | 🟠 BLOCKED | ✅ COMPLETED | 📦 CLEANUP]

---

## Quick Status

**One-liner**: [Current state in 1-2 sentences]

**Blockers**: [None] or [Description + action needed]

**Next Action**: [What happens next?]

---

## Timeline

| Date | Action | Status |
|------|--------|--------|
| 2026-01-08 10:00 | Task created | ✅ |
| 2026-01-08 10:15 | Chat dispatched to @Agent | ✅ |
| [Today] [Time] | [Action] | 🟡 |
| 2026-01-08 16:30 | Task completed, ready for cleanup | ✅ |

---

## Completion Checklist

From `brief.md` acceptance criteria:

- [ ] Acceptance criterion 1
- [ ] Acceptance criterion 2
- [ ] Acceptance criterion 3
- [ ] PR created & linked below
- [ ] Code review approved
- [ ] Tests passing
- [ ] No new warnings

---

## Artifacts

**Links to outputs**:

- **PR**: [Link to GitHub PR, or "Not yet"]
- **Branch**: `feature/TASK-XXX-{slug}`
- **Commits**: [Hash links or "TBD"]
- **Test Results**: [Link to CI run or "TBD"]

---

## Key Decisions Made

- **Decision 1**: [Why chosen over alternatives]
- **Decision 2**: [Design rationale]

---

## Chat Sessions Used

| Session # | Duration | Context Loaded | Output |
|-----------|----------|---|--------|
| 1 | [hh:mm] | brief.md + backend-essentials.instructions.md | Design + initial PR |
| 2 | [hh:mm] | previous PR feedback | Updates |

**Total Context**: [XXX KB] of [YYY KB] estimated

---

## Learning & Handoff Notes

[What should the next agent know if this task passes to someone else?]

[Any gotchas discovered?]

[Lessons for similar future tasks?]

---

## Cleanup Phase (After Completion)

**Cleanup Status**: [ ] Not Started | [x] In Progress | [ ] Complete

### Cleanup Checklist (for @SARAH)

- [ ] All PRs merged to main
- [ ] Tests passing in CI/CD
- [ ] Code review approved by @TechLead
- [ ] QA sign-off received
- [ ] Documentation updated
- [ ] Artifacts verified in `artifacts/` folder
- [ ] progress.md final version committed
- [ ] Metrics recorded in `.ai/logs/task-metrics.json`
- [ ] Lessons learned captured in `.ai/knowledgebase/lessons.md` (if applicable)
- [ ] Task directory cleaned (large files removed)
- [ ] ACTIVE_TASKS.md updated with ✅ ARCHIVED status
- [ ] Rate-limit dashboard updated (freed slot)

**Cleanup Completed By**: @SARAH  
**Cleanup Date**: YYYY-MM-DD HH:MM UTC

### Final Metrics

- **Estimated Tokens**: [XXX KB]
- **Actual Tokens Used**: [XXX KB]
- **Efficiency**: [XX%]
- **Estimated Duration**: [X hours]
- **Actual Duration**: [X hours]
- **Quality**: ✅ On target / ⚠️ Issues / ❌ Failed

### Archival Notes

- **Archive Location**: `.ai/tasks/archive/YYYY-MM/task-XXX-{slug}/`
- **Status in COMPLETED_TASKS.md**: ✅ Listed
- **GitHub PR Link**: [Keep for reference]
- **Lessons Documented**: [Yes/No]

---

## Updated by @Agent

**Agent**: @[Name]  
**Last Updated**: YYYY-MM-DD HH:MM UTC  
**Chat**: [Session link if available, or chat ID]
