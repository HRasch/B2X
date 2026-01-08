---
docid: WF-024
title: WF 013 AGENT SUPPORT
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

---
docid: WF-013
title: Agent Support & Escalation Instructions
owner: "@SARAH"
status: Active
last-updated: 2026-01-08
---

# 📖 Agent Support & Escalation Instructions

**Document ID**: `WF-013`  
**Purpose**: Self-service troubleshooting + clear escalation paths for agents when blocked.  
**Principle**: Agents work autonomously; get help only when truly blocked.

---

## Overview

This is **self-service support documentation** for agents. When you (agent) get stuck:

1. **Check this guide first** (most issues self-resolvable)
2. **Follow troubleshooting steps**
3. **If still blocked**: Escalate with specific details
4. **Minimal @SARAH involvement** needed

---

## 🚀 Quick Troubleshooting Index

| Problem | Instructions | Escalation |
|---------|--------------|------------|
| Task is unclear | [Read here](#task-unclear) | Ask @SARAH via thread |
| Need KB article | [Query KB](#kb-queries) | Use KB-MCP, don't ask |
| Rate-limit hit | [See procedures](#rate-limits) | @SARAH notified auto |
| Need design input | [Propose in PR](#design-review) | @TechLead reviews |
| Blocked on another task | [Document blocker](#task-blocked) | Update ACTIVE_TASKS.md |
| Can't proceed without context | [Load fragments](#context-loading) | Follow GL-044 rules |

---

## Task Clarity Issues

### Problem: "Brief.md is unclear"

**Instructions**:

1. **Re-read brief.md** carefully:
   - [ ] Objective clear? (1 sentence)
   - [ ] Acceptance criteria specific? (checkboxes)
   - [ ] In-scope items listed?
   - [ ] Out-of-scope items listed?

2. **Document what's unclear**:
   ```markdown
   # Clarification Request
   
   **Issue**: Objective says "refactor" but unclear if includes tests
   **Question**: Are integration tests in scope?
   **Impact**: Affects time estimate (4h vs. 6h)
   **Default Assumption**: [Propose your assumption]
   ```

3. **Reply in dispatch thread** with clarification request

4. **@SARAH will**:
   - Respond within 1 hour
   - Update brief.md if needed
   - Confirm you can proceed

**You decide**: Make reasonable assumption or wait for clarification?

---

## KB & Knowledge Queries

### Problem: "I need to understand [pattern]"

**Instructions** (DO NOT ask for full articles):

1. **Use KB-MCP query** instead:
   ```
   kb-mcp/search_knowledge_base query="Wolverine handler patterns for commands"
   ```

2. **What you get**: Targeted excerpt + link to full article

3. **If excerpt insufficient**: 
   - Read full article via link
   - Don't ask agent chat to embed it (wastes tokens)

4. **If KB article missing pattern**:
   - Document in `.ai/knowledgebase/lessons.md`
   - Link to GitHub PR showing pattern
   - Next agent will benefit

**Never ask**: "Can you tell me about Vue.js patterns?"  
**Instead do**: Use KB-MCP or read KB-007 directly

---

## Context Loading

### Problem: "I don't have the right context"

**Instructions**:

1. **Check what loaded**:
   - ✅ brief.md (your task spec)
   - ✅ Path-specific instructions (e.g., backend-essentials.instructions.md)
   - ✅ Security instructions (always loaded)
   - ❌ Full KB articles (use KB-MCP instead)

2. **If instructions missing**:
   - Domain is `backend` → Should have `backend-essentials.instructions.md`
   - Domain is `frontend` → Should have `frontend-essentials.instructions.md`
   - Missing? See [GL-044] Fragment-Based Access

3. **If pattern unclear**:
   - Use KB-MCP queries (don't ask for embeddings)
   - Check relevant ADR (linked in brief.md)
   - Look at similar PR from git history

4. **Still stuck**?
   - Document specifically what's needed
   - Reply: "@SARAH, need [specific KB article] for pattern clarity"
   - Proceed with best guess while waiting

---

## Rate-Limit Handling

### Problem: "I got rate-limited (429 error)"

**Instructions**:

1. **What happened**: API hit limit, automatic 30-min cooldown starting
   
2. **Your action NOW**:
   - Stop new operations immediately
   - Finish current work item only (if almost done)
   - DO NOT start new analysis/refactoring
   - Update progress.md: "Rate-limit hit, waiting cooldown"

3. **During cooldown** (30 minutes):
   - ✅ Update progress.md with status
   - ✅ Document what you've accomplished so far
   - ✅ Prepare next steps (no execution)
   - ❌ Don't continue coding

4. **After cooldown** (30 min later):
   - @SARAH checks if limit reset
   - You'll get notification when safe to resume
   - Continue with reduced load (single task focus)

5. **Prevention** (for next time):
   - Batch operations (fewer chat requests)
   - Use file-based communication instead of chat
   - Query KB-MCP instead of full embeds

**@SARAH handles**: Monitoring, escalation, emergency decisions  
**You handle**: Awareness, communication, resume signals

---

## Task Dependencies & Blockers

### Problem: "I'm blocked waiting for another task"

**Instructions**:

1. **Document blocker** in your progress.md:
   ```markdown
   ## Blocker: Waiting on TASK-001

   **Why blocked**: TASK-001 (Backend API) must complete before tests
   **Expected unblock**: 2026-01-09 14:00 UTC
   **What I'm doing meanwhile**: Preparing test fixtures, documentation
   ```

2. **Update ACTIVE_TASKS.md**:
   ```markdown
   ## TASK-003 🟠 BLOCKED @Testing — Integration Tests
   **Blocked by**: TASK-001 (backend complete needed)
   ```

3. **Continue with non-blocked work**:
   - Write documentation
   - Prepare fixtures
   - Design test scenarios
   - Review related code

4. **@SARAH will**:
   - See BLOCKED status
   - Monitor TASK-001 completion
   - Notify you when blocker resolves
   - You resume immediately

**Key**: Block is documented, work continues on adjacent tasks

---

## Design Review & Feedback

### Problem: "Need architecture/design input"

**Instructions**:

1. **Propose your design** in PR description:
   ```markdown
   ## Design Proposal
   
   ### Problem
   Need to refactor Catalog handlers.
   
   ### Proposed Solution
   [Your design here]
   
   ### Alternatives Considered
   - Option A: [rejected because...]
   - Option B: [rejected because...]
   
   ### @TechLead Review Requested
   Please confirm approach before implementation.
   ```

2. **Tag reviewers** for feedback:
   - `@TechLead` for architecture
   - `@Backend` for domain expertise
   - `@Security` for security concerns

3. **Wait for review** (typically 2-4 hours):
   - Continue parallel work if possible
   - Update progress.md with design review status
   - Don't implement until approved

4. **Decision received**:
   - Approved → Proceed with implementation
   - Rejected → Return to step 1 (propose alternative)
   - Changes requested → Update design, resubmit

**Key**: Design review happens early, saves rework later

---

## Common Questions & Answers

### "Can I create a sub-chat to explore something?"
**No.** One task per chat. If you need exploration:
- [ ] Document in progress.md: "Exploring [pattern]"
- [ ] Findings go in progress.md
- [ ] When complete, update brief.md if needed
- [ ] Stay in same chat

### "What if my estimate was wrong?"
**Update immediately**:
- [ ] Update progress.md: "Revised estimate: 6h (was 4h)"
- [ ] Explain: "Found edge case in X, needs Y time"
- [ ] Continue working (no need to stop)
- [ ] @SARAH adjusts schedule automatically

### "Can I take on an additional small task?"
**No.** One task at a time. When current task complete:
- [ ] Mark ✅ COMPLETED in progress.md
- [ ] Notify @SARAH (just say "TASK-001 complete")
- [ ] @SARAH dispatches next task
- [ ] You start new task in fresh context

### "What if I discover a bug in the framework?"
**Create GitHub issue**:
- [ ] Document the bug
- [ ] Provide reproduction steps
- [ ] Link in progress.md: "Found bug #[issue], pausing work"
- [ ] Check if workaround exists (continue if yes)
- [ ] If no workaround: Document as blocker, proceed with next task

### "Need to split my task into smaller pieces?"
**Ask @SARAH**:
- [ ] Reply: "Task too large, propose splitting into TASK-XXX-A and TASK-XXX-B"
- [ ] @SARAH evaluates
- [ ] If approved: Complete TASK-XXX-A, dispatch TASK-XXX-B
- [ ] If rejected: Continue with original scope

---

## Escalation Procedures

### Level 1: Self-Service (You First)
**Check these before asking**:
- [ ] Read brief.md entirely
- [ ] Search KB for pattern (KB-MCP)
- [ ] Check relevant ADR (linked in brief)
- [ ] Look at git history for similar PRs
- [ ] Review path-specific instructions
- [ ] Check progress.md for updates

✅ **Resolved?** → Continue work  
❌ **Still stuck?** → Escalate to Level 2

### Level 2: Dispatch Thread
**Reply in task dispatch thread**:
```
@SARAH, need clarification on [specific thing]:

Current Understanding: [What you think]
Question: [What's unclear]
Impact: [Why it matters]
Proposed Decision: [Your best guess if must proceed]
```

**@SARAH response time**: 1 hour  
**Decision**: Clarify, approve assumption, or update brief

✅ **Unblocked** → Continue work  
❌ **Need more?** → Escalate to Level 3

### Level 3: Domain Expert
**If blocking design decision**:
```
@TechLead, need architecture guidance:

Context: [Situation from brief.md]
Options:
  A) [Choice 1 - pros/cons]
  B) [Choice 2 - pros/cons]
Recommendation: [Your best judgment]

Needed by: [When decision needed]
```

**Response time**: 2-4 hours  
**Decision**: Expert guidance or approval

✅ **Unblocked** → Continue work  
❌ **Still stuck?** → [Rare] @SARAH intervention

### Level 4: Emergency (@SARAH Only)
**Only if**:
- Task completely blocked (no workaround)
- Waiting extends beyond 4 hours
- Affects downstream tasks
- Rate-limit crisis

**Escalate**:
```
@SARAH, EMERGENCY UNBLOCK NEEDED:

Current Status: [What you're stuck on]
Duration: [How long stuck]
Impact: [What's blocked]
Downstream: [What depends on this]
```

**@SARAH action**: Immediate investigation + decision

---

## Update Frequency

**When to update progress.md**:
- ✅ After each major output (PR pushed, tests passing)
- ✅ When blockers discovered
- ✅ When estimates change
- ✅ When escalation needed
- ❌ NOT every 5 minutes (no spam)

**Update template**:
```markdown
---

**[Time]**: [What happened]
- [ ] Completed: [Deliverable]
- [ ] Next: [What's next]
- [ ] Blockers: [None] or [Blocker description]
- [ ] Questions: [If escalating]
```

---

## Communication Norms

### When Replying to @SARAH
- ✅ **Specific**: "Need clarification on acceptance criterion #2"
- ❌ **Vague**: "This is confusing"

- ✅ **Context**: "Wolverine pattern for X unclear, see KB-006"
- ❌ **Context-free**: "How do handlers work?"

- ✅ **Proposed resolution**: "Can I assume handlers are [pattern]?"
- ❌ **No proposal**: "What should I do?"

### When Blocked
- ✅ **Document**: Update progress.md first
- ✅ **Notify**: Reply in thread if urgent
- ✅ **Continue**: Work on non-blocked items
- ❌ **Wait passively**: Don't just sit idle

### When Complete
- ✅ **Signal**: "TASK-001 complete, ready for QA"
- ✅ **Link**: "PR #1234, all criteria met"
- ❌ **Assume**: Don't assume @SARAH will notice

---

## Success Patterns

### What Works Well
- ✅ Clear, specific questions (not vague)
- ✅ Proposing solutions before asking for help
- ✅ Updating progress.md regularly
- ✅ Documenting blockers clearly
- ✅ Using KB-MCP for patterns
- ✅ Preparing next steps while waiting

### What Doesn't Work
- ❌ Asking for KB articles to be embedded
- ❌ Vague status updates
- ❌ Waiting passively without updates
- ❌ Asking for help without proposing solution
- ❌ Creating sub-chats for exploration
- ❌ Working on unstated tasks

---

## Quick Reference: When to Escalate

| Situation | First Try | Then | Then |
|-----------|-----------|------|------|
| Unclear task | Re-read brief | Ask @SARAH | Propose assumption |
| Missing pattern | KB-MCP query | Check ADR | Ask @TechLead |
| Rate-limited | Stop new ops | Update progress | Wait 30 min |
| Need design | Propose design | PR review | @TechLead feedback |
| Blocked on task | Document | Work non-blocked | @SARAH monitors |
| Estimate wrong | Update progress | Explain change | Continue |

---

## Emergency Contact

**@SARAH is available for**:
- Genuine blockers (4+ hours, no workaround)
- Policy questions (can I...?)
- Rate-limit emergencies
- Escalations from @TechLead

**@SARAH is NOT for**:
- Clarifying brief (check first)
- KB article requests (use KB-MCP)
- Design help (ask @TechLead, propose first)
- Every status update (use progress.md)

---

## Your Workflow (Summary)

```
1. Receive task dispatch
   ↓
2. Read brief.md entirely
   ↓
3. Load minimal context (instructions + KB-MCP)
   ↓
4. Understand constraints (in-scope, out-of-scope)
   ↓
5. Start work
   ↓
6. Update progress.md after each major step
   ↓
7. Hit blocker?
   → Document in progress.md
   → Try self-service first
   → Escalate only if truly stuck
   ↓
8. Task complete
   → Mark ✅ COMPLETED
   → Signal @SARAH
   → Await next task
```

---

**Owner**: @SARAH (maintains policy)  
**For**: All agents (@Backend, @Frontend, @Testing, @Security, etc.)  
**When confused**: Start here, then escalate if needed  
**Key principle**: Self-service first, help when truly needed

---

*Last Updated: 2026-01-08*
