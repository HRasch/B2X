---
docid: QS-002
title: Multi-Chat Task Management Quick Start
owner: "@SARAH"
status: Active
last-updated: 2026-01-08
---

# ⚡ Multi-Chat Task Management Quick Start

**DocID**: `QS-002`  
**Target**: 5-minute read  
**For**: @SARAH (coordinator), Agent teams

---

## Problem Solved

❌ **Old**: Single chat → 45KB context per task → Token waste → Rate limits  
✅ **New**: Multiple specialized chats → 12KB context per task → 73% savings → Parallelization

---

## 5-Minute Overview

### 1. Task Arrives
→ @SARAH analyzes: **Is this 1 task or many tasks?**

```
Feature request
├─ Backend work (TASK-001)
├─ Frontend work (TASK-002)  ← Can run in parallel
├─ Tests (TASK-003)          ← Depends on 1 & 2
└─ Security audit (TASK-004) ← Can run with 1 & 2
```

### 2. @SARAH Creates Task Files
→ For each task:

```
.ai/tasks/task-001-{name}/
├─ brief.md       (What to do, acceptance criteria, context to load)
├─ progress.md    (Updated by agent during work)
└─ artifacts/     (Links to PRs, commits)
```

### 3. @SARAH Dispatches to Agent Chat
→ Minimal context:
```
✅ Load: brief.md + path-specific instructions + KB-MCP queries on-demand
❌ Don't load: Full project context, KB articles, frontend instructions
```

### 4. Agent Works & Updates progress.md
→ After each step:
```
- What was done
- PR link (if applicable)
- Next action
- Any blockers
```

### 5. Task Complete
→ Agent marks ✅ COMPLETED in progress.md

### 6. Cleanup & Archive (1-7 Days Later)
→ @SARAH handles cleanup:
```
- Verify QA sign-off
- Consolidate artifacts
- Record metrics
- Move to .ai/tasks/archive/YYYY-MM/
- Update COMPLETED_TASKS.md
```

**See [WF-012] for detailed cleanup procedure.**

---

## Key Files

| File | Purpose | Owner |
|------|---------|-------|
| `.ai/tasks/ACTIVE_TASKS.md` | Dispatch board | @SARAH |
| `.ai/tasks/task-{id}/brief.md` | Task spec | @SARAH writes |
| `.ai/tasks/task-{id}/progress.md` | Execution tracking | Agent updates |
| `.ai/workflows/WF-011-TASK-DISPATCH.md` | Full dispatch workflow | Reference |
| `.ai/guidelines/GL-052-RATE-LIMIT-COORDINATION.md` | Rate-limit safety | Reference |

---

## Rate-Limit Safety (3-Point Rule)

1. **Max 2 chats at a time** (different domains only)
2. **10-15 min cooldown** between high-intensity tasks
3. **Check headroom** before dispatch: tokens/min < 50K

```
✅ Safe: Backend chat + Frontend chat (parallel)
❌ Risky: Backend chat + Backend chat (conflicts)
⛔ Blocked: 3+ chats (rate limit risk)
```

---

## @SARAH Dispatch Checklist

Before sending task to agent:

- [ ] Brief.md completed (acceptance criteria clear)?
- [ ] Active chats < 2?
- [ ] Different domain than existing tasks?
- [ ] Last dispatch > 10 min ago?
- [ ] Rate-limit < 50K tokens/min?

✅ All yes → Dispatch  
❌ Any no → Wait or reschedule

---

## Agent Workflow (When You Receive Task)

1. **Read**: `.ai/tasks/task-{id}/brief.md`
2. **Load context**:
   - ✅ Path-specific instructions only
   - ✅ KB-MCP queries on-demand
   - ❌ Don't embed full KB articles
3. **Work on task**
4. **Update progress.md** after each major step
5. **Complete** when all ✅ in brief.md

---

## Expected Efficiency Gains

| Metric | Before | After |
|--------|--------|-------|
| Tokens/task | 45KB | 12KB |
| Token savings | — | 73% |
| Tasks in parallel | 1 | 2-3 |
| Rate-limit incidents | 2-3/sprint | <1 |
| Context setup time | 10 min | 1 min |

---

## Example: Real Task Flow

**Monday 10:00 — Feature request arrives**

```
@SARAH: Decompose into 4 tasks
  TASK-001 @Backend — API endpoints (P1, 2h, 15K tokens)
  TASK-002 @Frontend — UI components (P1, 2h, 12K tokens)
  TASK-003 @Testing — Integration tests (P1, 1h, 8K tokens)
  TASK-004 @Security — Vulnerability scan (P0, 1h, 6K tokens)
```

**10:15 — Dispatch Wave 1 (parallel)**
```
Create task directories + brief.md for TASK-001 & 002
@SARAH: "TASK-001 ready" → @Backend chat
@SARAH: "TASK-002 ready" → @Frontend chat
Estimated parallel time: 2 hours, 27K tokens total
(vs. 4 hours sequential, 67K tokens in old model)
```

**10:30-10:45 — Cooldown**
```
No new task dispatches
@SARAH aggregates progress from agents
Rate-limit check: 27K of 95K budget used ✅
```

**10:45 — Dispatch Wave 2 (parallel with Wave 1)**
```
@SARAH: "TASK-004 ready" → @Security chat
Note: Different domain (security audit), low API conflict
Estimated time: 1 hour, 6K tokens
```

**11:45 — Check Wave 1 Complete?**
```
YES: TASK-001 ✅ (Backend done)
     TASK-002 ✅ (Frontend done)
→ Dispatch TASK-003 @Testing (depends on 1 & 2)
```

**12:45 — All Tasks Complete**
```
Total: ~3 hours (vs. 6-7 hours sequential)
Total tokens: 41K (vs. 67K old model)
Savings: 39% faster, 39% fewer tokens
```

---

## When Things Go Wrong

### Task Blocked (waiting for dependency)
→ Update ACTIVE_TASKS.md: 🟠 BLOCKED  
→ Re-prioritize other tasks during wait

### Rate-Limit Alert (>75K tokens/min)
→ @SARAH: Pause new chats  
→ Wait 30 minutes  
→ Resume with reduced load

### Agent Needs Clarification
→ Reply in task dispatch thread  
→ @SARAH updates brief.md if needed  
→ Continue work

---

## Tips for Success

1. **Be specific in brief.md**: "Extract 3 Wolverine handlers" not "Refactor catalog"
2. **Load minimal context**: If in doubt, query KB-MCP instead of embedding
3. **Update progress.md frequently**: After each major output
4. **Notify @SARAH on completion**: Don't assume they know
5. **Document learnings**: Add to `.ai/knowledgebase/lessons.md`

---

## Next Steps

- [ ] Familiarize yourself with WF-011 (detailed dispatch workflow)
- [ ] Review GL-052 (rate-limit safety protocol)
- [ ] Bookmark `.ai/tasks/ACTIVE_TASKS.md` (your task dashboard)
- [ ] Bookmark `.ai/tasks/BRIEF_TEMPLATE.md` (use when creating tasks)

---

## FAQ

**Q: Can I work on 2 tasks in one chat?**  
A: No. One task per chat. Reduces context bloat and token waste.

**Q: What if I finish early?**  
A: Mark complete in progress.md. @SARAH will dispatch next task from queue.

**Q: What if a task is bigger than estimated?**  
A: Update brief.md with new estimate. @SARAH adjusts schedule.

**Q: Can I query KB articles directly?**  
A: Yes, use `kb-mcp/search_knowledge_base query="..."` instead of asking for full articles.

**Q: How do I track rate limits?**  
A: @SARAH maintains `.ai/logs/rate-limits/current-status.md`. Check before dispatch.

---

**Read More**: [WF-011] Task Dispatch Workflow | [GL-052] Rate-Limit Coordination

---

*Last Updated: 2026-01-08*
