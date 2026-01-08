---
docid: GL-052
title: Rate-Limit Coordination & Multi-Chat Safety Protocol
owner: "@SARAH"
status: Active
last-updated: 2026-01-08
---

# 🛡️ Rate-Limit Coordination & Multi-Chat Safety Protocol

**Purpose**: Prevent API rate limits when running parallel agent chats while maximizing throughput.

**Document ID**: `GL-052`

---

## The Problem

- **Old approach**: Single chat, high token per task, predictable rate limits
- **New approach**: Multiple parallel chats, lower tokens per task, but higher risk of rate-limit triggers
- **Goal**: Parallelization WITHOUT hitting rate limits

---

## Rate-Limit Thresholds (GitHub Copilot)

| Tier | Requests/Min | Tokens/Min | Risk Level |
|------|--------------|-----------|-----------|
| Safe Zone | <30 | <50K | 🟢 |
| Caution | 30-50 | 50-75K | 🟡 |
| Danger | 50-80 | 75-95K | 🔴 |
| Exceeded | >80 | >95K | ⛔ BLOCKED |

**Our Strategy**: Stay in Safe Zone with headroom (target: <20 req/min, <30K tokens/min)

---

## Coordination Rules

### Rule 1: Maximum Concurrency

**Never exceed**:
- 2 agent chats running simultaneously
- Exception: If both domains are independent (backend + frontend OK, but not backend + backend)

```
✅ ALLOWED:
  - @Backend chat
  - @Frontend chat
  (different domains, low conflict)

❌ NOT ALLOWED:
  - @Backend chat #1
  - @Backend chat #2
  (same domain, will conflict on repository access)
```

### Rule 2: Sequential Scheduling

Schedule intensive operations in **time slots**:

```
MONDAY (Rate-Limit Budget: 95K tokens)
  09:00-09:30  @Backend (intensive refactor) — 30K tokens
  09:30-09:45  COOLDOWN — no new chats
  09:45-10:15  @Frontend (UI refactor) — 25K tokens
  10:15-10:30  COOLDOWN — no new chats
  10:30-11:00  @Testing (full test suite) — 20K tokens
  11:00-12:00  Recovery window (no tasks)
```

**Cooldown Rules**:
- 10-15 minutes between high-intensity tasks
- No new interactive chat sessions during cooldown
- OK: Text-based progress file updates
- OK: Aggregating status from previous agents

### Rule 3: Task Classification by Load

**High Intensity** (20-30K tokens, sequential only):
- Full codebase refactoring
- Complete test suite runs
- Security audit scans
- Architecture redesign
- Database schema changes

**Medium Intensity** (10-15K tokens, sequential with cooldown):
- Feature implementation
- Component redesign
- Bug fixes
- API endpoint creation
- Documentation updates

**Low Intensity** (<5K tokens, can parallel):
- Code review feedback
- Linting fixes
- Small UI tweaks
- Documentation typos
- Configuration changes

### Rule 4: Domain-Based Parallelization

**Can run in parallel** (different domains):
```
✅ Backend + Frontend (no conflicts)
✅ Backend + Security audit (different APIs)
✅ Frontend + DevOps (independent)
```

**Sequential only** (same domain):
```
❌ Backend Task 1 + Backend Task 2
❌ Frontend Task 1 + Frontend Task 2
❌ Security Audit 1 + Security Audit 2
```

**Why**: Same domain = likely hitting same KB articles, instructions, code paths → throttling risk

---

## Pre-Task Rate-Limit Check

**Before dispatching new task, @SARAH verifies**:

```python
def can_dispatch_task(new_task):
    # 1. Count active chats
    active_chats = count_active_agent_chats()
    if active_chats >= 2:
        return False  # Already at max concurrency
    
    # 2. Check domain conflicts
    for active_task in ACTIVE_TASKS:
        if active_task.domain == new_task.domain and active_chats > 0:
            return False  # Same domain, must be sequential
    
    # 3. Verify rate-limit headroom
    current_usage = get_rate_limit_status()
    if current_usage > 50000_tokens_per_minute:
        return False  # Too close to limit
    
    # 4. Check cooldown timer
    time_since_last_dispatch = now() - LAST_DISPATCH_TIME
    if time_since_last_dispatch < 10_minutes:
        return False  # In cooldown window
    
    return True  # Safe to dispatch
```

---

## Real-Time Rate-Limit Monitoring

**@SARAH monitors continuously**:

1. **GitHub API rate limit endpoint**: Check every 5 minutes
2. **Token usage tracking**: Log tokens per chat session
3. **Incident detection**: If approaching 75K tokens/min, trigger alert

### Alert Levels

| Level | Action | Response |
|-------|--------|----------|
| 🟢 GREEN | Normal ops | Continue scheduling |
| 🟡 YELLOW | >50K tokens/min | Reduce new dispatches, start cooldown |
| 🔴 RED | >75K tokens/min | Pause new chats, 15-min cooldown |
| ⛔ BLOCKED | >95K tokens/min | Emergency protocol (see below) |

---

## Emergency Protocol (Rate Limit Exceeded)

**If API returns 429 (rate limited)**:

### Immediate (0-5 min)
1. **PAUSE all new chat dispatches** immediately
2. **Notify all active agents**: "Stop new operations, finish current action only"
3. **Wait time**: 30 minutes (GitHub standard reset)
4. **Document incident**: Create `.ai/logs/rate-limits/incident-{timestamp}.md`

### During Wait (5-30 min)
- Agents can finish *current* work item only (no new tasks)
- OK: Completing a PR, finishing a test run, completing a decision
- NOT OK: Starting new file edits, launching new analysis
- Text updates: Agents can update progress.md files without chat interaction

### Recovery (30+ min)
1. **@SARAH verifies rate limit reset** (check GitHub API)
2. **Resume with reduced load**: Single task only for first 30 minutes
3. **Gradual ramp**: Add second task after 30 min if safe

### Post-Incident Review
```markdown
## Rate-Limit Incident Report

**Date/Time**: YYYY-MM-DD HH:MM UTC
**Duration**: X minutes
**Peak Usage**: XXXK tokens/min
**Root Cause**: [What triggered the overload?]
**Resolution**: [What was done?]
**Prevention**: [How do we prevent this?]

Action Items:
- [ ] Adjust concurrent task limit
- [ ] Add cooldowns between specific task types
- [ ] Update rate-limit monitoring thresholds
```

---

## Status File: Rate-Limit Dashboard

Location: `.ai/logs/rate-limits/current-status.md`

**@SARAH updates this every hour**:

```markdown
# Rate-Limit Status Dashboard

**Last Updated**: 2026-01-08 14:00 UTC  
**Status**: 🟢 GREEN

## Current Usage
- Requests/min: 15 of 80 (18%)
- Tokens/min: 22K of 95K (23%)
- Headroom: 73K tokens remaining

## Active Chats
- @Backend (Task-001) — Started 13:30, ~8K tokens so far
- @Frontend (Task-002) — Started 13:35, ~6K tokens so far

## Next Scheduled Tasks
- TASK-003 @Testing — 14:30 (after 10-min cooldown)
- TASK-004 @Security — 15:00 (sequential, high intensity)

## Cooldown Schedule
- 14:00-14:10: COOLDOWN (from TASK-002)
- 14:10: Check if TASK-001 complete
- 14:30: Dispatch TASK-003 if ready

## Incidents This Week
- None
```

---

## Agent Chat Guidelines

### What Agents Should Do
- ✅ Work on tasks within task scope
- ✅ Update progress.md with status
- ✅ Query KB-MCP on-demand (don't embed full articles)
- ✅ Use path-specific instructions only
- ✅ Link PRs and commits to task

### What Agents Should NOT Do
- ❌ Work across multiple tasks in parallel
- ❌ Load full project context
- ❌ Recreate context from previous chat
- ❌ Start new tasks during @SARAH-announced cooldown
- ❌ Ignore progress.md updates from @SARAH

---

## Estimation Template for Rate-Limit Planning

**For each task brief, include**:

```markdown
## Estimation

**Complexity**: [Simple|Moderate|Complex]
**Estimated Duration**: [30 min|1 hour|2 hours|4 hours|1 day|2 days]
**Estimated Tokens**: [<5K|5-10K|10-20K|20-30K]
**High Intensity?**: [Yes|No]

**Parallel OK?**: [Yes|No] — Can run alongside other tasks?
**Domain**: [backend|frontend|security|devops|testing|architecture]
```

**Example**:
```markdown
## Estimation

**Complexity**: Moderate
**Estimated Duration**: 2 hours
**Estimated Tokens**: 12-15K
**High Intensity?**: No

**Parallel OK?**: Yes — Can run with @Frontend work
**Domain**: backend
```

---

## Weekly Rate-Limit Review

**Every Friday, @SARAH publishes**:

```markdown
# Weekly Rate-Limit Report

**Week of**: 2026-01-06 to 2026-01-12
**Peak Daily Usage**: 65K tokens (Tuesday)
**Incidents**: None
**Efficiency Gain**: 62% token reduction vs. single-chat baseline

## Task Dispatch Summary
- Total Tasks: 8
- Completed: 8
- Avg Tokens per Task: 12KB (target: <15KB) ✅
- Avg Task Duration: 3.5 hours
- Parallelization Rate: 45% (of available time, tasks ran in parallel)

## Cooldown Effectiveness
- Intended Cooldowns: 12
- Actual Cooldowns Used: 10
- Rate-Limit Triggers Prevented: ~3 estimated
- Cost Savings: ~15K tokens equivalent

## Recommendations for Next Week
- Increase backend task scheduling (lower token load observed)
- Monitor security audits (highest load type)
- Test 3-concurrent-task model if headroom stays >50K

---
```

---

## Integration with ACTIVE_TASKS.md

Rate-limit status feeds into task dispatch decisions:

```markdown
## TASK-003 🟠 WAITING-FOR-COOLDOWN @Testing

**Assigned To**: @Testing  
**Status**: Waiting for rate-limit cooldown  
**Next Available Dispatch**: 2026-01-08 14:45 UTC  
**Why**: TASK-001 & TASK-002 completed 14:35, 10-min cooldown until 14:45
```

---

## Tools & Automation (Future)

**Automation opportunities** (Phase 3):
- [ ] GitHub Actions: Monitor rate-limit API every 5 min
- [ ] Slack bot: Alert @SARAH when approaching limits
- [ ] Dashboard: Real-time usage graph
- [ ] Auto-queue: ACTIVE_TASKS.md auto-orders by rate-limit impact

---

## Quick Reference: Dispatch Checklist

Before sending task to agent, @SARAH verifies:

- [ ] Active chats < 2?
- [ ] No domain conflicts with existing task?
- [ ] Rate-limit < 50K tokens/min?
- [ ] Last dispatch > 10 min ago?
- [ ] Brief.md complete (acceptance criteria clear)?
- [ ] Progress.md initialized (status = 🟢 READY)?
- [ ] Context files specified (what NOT to load)?
- [ ] Estimated tokens < 20K (or scheduled for high-intensity slot)?

✅ All checked → **Safe to dispatch**

---

**Owned by**: @SARAH (Coordinator)  
**Last Updated**: 2026-01-08  
**Next Review**: 2026-01-22  
**Related**: [WF-011] Task Dispatch Workflow, [ACTIVE_TASKS.md] Registry
