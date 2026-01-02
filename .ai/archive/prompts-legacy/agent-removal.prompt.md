---
agent: SARAH
description: Agent removal with process impact analysis and redesign
---

# Agent Removal & Deactivation Process

**Purpose:** Structured process for evaluating agents against process requirements, redesigning workflows, and then safely removing agents  
**Owner:** @SARAH  
**Key Principle:** Adapt processes BEFORE removing agents, not after
**Triggered by:** Performance review, redundancy detection, project completion, or resource optimization

---

## Critical Pre-Removal: Process Impact Analysis

**IMPORTANT:** Before any removal decision, SARAH must:
1. ✅ Identify ALL processes using this agent
2. ✅ Assess process criticality
3. ✅ Design agent-free alternatives
4. ✅ Adapt processes
5. ✅ Validate new workflow
6. THEN remove agent

---

## PHASE 0: Process Impact Analysis (BEFORE Removal Decision)

**Duration:** 3-5 days  
**Critical:** This phase determines if removal is even possible

### Step 1: Identify All Process Dependencies

```
□ Scan all process documentation:
  - .ai/workflows/
  - .ai/collaboration/
  - .ai/guidelines/
  - .github/instructions/
  - .github/prompts/
  
□ Document agent usage in:
  - Delegation workflows
  - Agent-to-agent handoff paths
  - Coordination sequences
  - Quality gates
  - Escalation paths
  
□ Create dependency map:
  - Which processes use this agent?
  - How critical is this agent to each?
  - What happens if agent unavailable?
  - Any single-point-of-failure situations?
```

**Dependency Mapping Template:**

```markdown
# Process Dependencies: [Agent Name]

## Direct Usage

| Process | Workflow | Purpose | Criticality |
|---------|----------|---------|------------|
| [Process 1] | [File path] | [What it does] | CRITICAL / HIGH / MEDIUM / LOW |
| [Process 2] | [File path] | [What it does] | CRITICAL / HIGH / MEDIUM / LOW |

## Indirect Dependencies

- [Process] depends on [This Agent] via @[Other Agent]
- [Coordination] requires [This Agent] in sequence

## Single Points of Failure

- [If agent unavailable, X process blocked]
- [If agent fails, Y workflow interrupted]

## Cascading Effects

If this agent removed:
→ [Process 1] becomes [State]
→ [Process 2] becomes [State]
→ [Coordination] becomes [State]
```

---

### Step 2: Assess Process Criticality

**For EACH process using the agent:**

```
CRITICALITY ASSESSMENT:

Critical (Must Have)
❌ Process cannot function without agent
❌ No workaround available
❌ Blocking production workflows
❌ User-facing impact immediate

High (Should Have)
⚠️  Process degraded without agent
⚠️  Workaround exists but complex
⚠️  Affects team productivity
⚠️  Requires manual effort

Medium (Nice to Have)
⚪ Process has alternatives
⚪ Workaround simple/quick
⚪ Low-frequency use
⚪ Limited impact

Low (Optional)
✅ Process not essential
✅ Agent is convenience feature
✅ Easy to work around
✅ Minimal disruption

---

DECISION LOGIC:

If CRITICAL processes exist:
→ Agent CANNOT be safely removed
→ Must redesign processes first
→ OR find higher-value alternative

If only HIGH/MEDIUM/LOW:
→ Proceed with process redesign
→ Plan migration strategy
→ Then remove agent
```

**Criticality Matrix:**

```markdown
# Process Criticality Assessment: [Agent Name]

| Process | Current Role | Criticality | Blockers | Redesign? |
|---------|--------------|------------|----------|-----------|
| [Proc 1] | [Role] | CRITICAL | [Blocker] | ❌ NO |
| [Proc 2] | [Role] | HIGH | [Blocker] | ✅ YES |
| [Proc 3] | [Role] | MEDIUM | [Blocker] | ✅ YES |
| [Proc 4] | [Role] | LOW | None | ✅ YES |

**Conclusion:** [Can proceed with redesign / Cannot proceed / Need to fix critical first]
```

---

### Step 3: Design Agent-Free Alternative Workflows

**For each process, answer:**

```
CURRENT STATE (WITH AGENT):
□ How does process work now?
□ What exactly does agent do?
□ Why is agent in this process?
□ What value does agent add?

ALTERNATIVE PATHS:

Option A: Manual Process
├─ Steps to execute manually?
├─ Who performs each step?
├─ How long does it take?
├─ What's the quality impact?
├─ Is this sustainable?

Option B: Different Agent
├─ Which agent could do this?
├─ Does it have required capabilities?
├─ Performance vs current agent?
├─ Adoption effort?
├─ Any gaps or limitations?

Option C: Process Redesign
├─ Restructure to not need agent?
├─ Combine with other processes?
├─ Automate certain steps?
├─ Distribute work differently?
├─ Quality/efficiency impact?

Option D: Tool/Automation
├─ Could tooling replace agent?
├─ Build effort?
├─ Maintenance overhead?
├─ Team capability?
├─ Long-term viability?

RECOMMENDATION:
Best path: [Option A/B/C/D]
Rationale: [Why this is best]
Risks: [What could go wrong]
Mitigation: [How to handle risks]
Effort: [Implementation time]
```

**Workflow Redesign Template:**

```markdown
# Process Redesign: [Process Name] (Without [Agent Name])

## Current Workflow (WITH Agent)

```
Input → @Agent → Quality Gate → Output
Time: 10 min | Success: 95% | Manual effort: 0
```

## Option 1: Manual Process

```
Input → Manual Review → Approve → Output
Time: 30 min | Success: 92% | Manual effort: 100%

Steps:
1. [Step 1]
2. [Step 2]
3. [Step 3]

Owner: [Role]
Frequency: [How often]
Blockers: [Any issues]
```

## Option 2: Different Agent (@[Other Agent])

```
Input → @[Agent] → Quality Gate → Output
Time: 12 min | Success: 93% | Manual effort: 5%

Capability match:
- [Capability 1]: ✅ YES / ⚠️ PARTIAL / ❌ NO
- [Capability 2]: ✅ YES / ⚠️ PARTIAL / ❌ NO

Gaps: [List any gaps]
Workarounds: [How to handle gaps]
```

## Option 3: Process Restructure

```
[New workflow structure]

Changes required:
- [Change 1]
- [Change 2]

Benefits:
- [Benefit 1]
- [Benefit 2]

Risks:
- [Risk 1]
- [Risk 2]
```

## Recommended Approach

**Selected:** [Option A/B/C]

**Why:**
[Justification for selection]

**Implementation Plan:**
1. [Step 1]
2. [Step 2]
3. [Step 3]

**Timeline:** [Days/weeks]
**Effort:** [Team effort]
**Quality:** [Expected quality vs current]
```

---

### Step 4: Identify Affected Documentation & Processes

```
□ Find all documents mentioning agent:
  - .ai/workflows/
  - .ai/guidelines/
  - .ai/collaboration/
  - .github/prompts/
  - .github/instructions/
  - README files
  - ADRs
  - Any custom docs
  
□ List all locations to update:
  [Location 1]: [Change needed]
  [Location 2]: [Change needed]
  [Location 3]: [Change needed]
  
□ Identify affected processes:
  - [Process 1]: Update to [new workflow]
  - [Process 2]: Update to [new workflow]
  - [Process 3]: Remove references
  
□ Determine update sequence:
  - Which docs must update first?
  - Any dependencies between updates?
  - Rollback capability for each?
```

---

## Phase 1: Process & Workflow Redesign

**Duration:** 5-10 days  
**Critical:** Complete before removal

### Step 1: Redesign & Document New Workflows

```
For each affected process:

□ Create new workflow documentation:
  - Updated .ai/workflows/ files
  - New collaboration paths
  - Updated prompts if needed
  - Revised guidelines
  
□ Document changes clearly:
  - What changed from old workflow
  - Why it changed (agent removed)
  - New responsibilities/roles
  - Any quality/timeline impacts
  
□ Provide detailed instructions:
  - Step-by-step process
  - New agent assignments (if any)
  - Escalation paths
  - Quality gates
  - Error handling
```

**Updated Workflow Template:**

```markdown
# REDESIGNED PROCESS: [Name]

**Status:** READY FOR IMPLEMENTATION (Agent removal: [Agent Name])  
**Effective Date:** [Date]  
**Owner:** [Role]

## OLD WORKFLOW (With [Agent Name])

```
Input → @Agent → Output
```

**Issues:** [Why we're removing the agent]

---

## NEW WORKFLOW (Without [Agent Name])

```
Input → [Step 1] → [Step 2] → Output
```

## Process Steps

1. **[Step 1]: [Action]**
   - Owner: [Role]
   - Input: [What]
   - Output: [What]
   - Time: [Duration]
   - Success criteria: [Criteria]

2. **[Step 2]: [Action]**
   - Owner: [Role]
   - Input: [What]
   - Output: [What]
   - Time: [Duration]
   - Success criteria: [Criteria]

3. **[Quality Gate]: [Check]**
   - Who approves: [Role]
   - Acceptance criteria: [Criteria]
   - What if fails: [Escalation]

## Quality & Performance

| Metric | Old Process | New Process | Delta |
|--------|------------|------------|-------|
| Execution Time | 10 min | 25 min | +15 min |
| Success Rate | 95% | 92% | -3% |
| Manual Effort | 0 | 40% | +40% |
| Cost | Low | High | +? |

**Acceptable?** [YES / NO / WITH MITIGATIONS]

## Mitigation Strategies

If new process introduces gaps:

- [Gap 1] → Mitigated by: [Mitigation]
- [Gap 2] → Mitigated by: [Mitigation]
- [Gap 3] → Mitigated by: [Mitigation]

## Rollback Plan

If new process fails:
1. Restore old documentation
2. Temporarily re-enable agent
3. Root cause analysis
4. Redesign iteration

---

**Approved by:** @SARAH  
**Date:** [Date]  
**Implementation date:** [Date]
```

---

### Step 2: Update All Affected Documentation

**For EACH file/process that mentions the agent:**

```
□ Locate document/file
□ Identify all references to agent
□ Replace with new workflow instructions
□ Update examples (if any)
□ Update diagrams/flowcharts
□ Update any templated prompts
□ Add note: "Updated due to agent removal"
□ Review for any missed references
□ Test for broken cross-references
```

**Documentation Update Checklist:**

```markdown
# Documentation Updates for Agent Removal: [Agent Name]

## Workflows (in .ai/workflows/)

| File | Changes | Status | Updated |
|------|---------|--------|---------|
| [workflow-1.md] | Updated steps 1-3, removed step 4 | ✅ | [Date] |
| [workflow-2.md] | Replaced agent with manual process | ✅ | [Date] |
| [workflow-3.md] | Removed agent reference | ✅ | [Date] |

## Guidelines (in .ai/guidelines/)

| File | Changes | Status | Updated |
|------|---------|--------|---------|
| [guideline-1.md] | Updated process section | ✅ | [Date] |
| [guideline-2.md] | Added mitigation strategies | ✅ | [Date] |

## Collaboration (in .ai/collaboration/)

| File | Changes | Status | Updated |
|------|---------|--------|---------|
| [collab-1.md] | Removed from agent list | ✅ | [Date] |
| [collab-2.md] | Updated agent matrix | ✅ | [Date] |

## Prompts (in .github/prompts/)

| File | Changes | Status | Updated |
|------|---------|--------|---------|
| [prompt-1.md] | Removed delegation to agent | ✅ | [Date] |
| [prompt-2.md] | Updated workflow references | ✅ | [Date] |

## Instructions (in .github/instructions/)

| File | Changes | Status | Updated |
|------|---------|--------|---------|
| [instruction-1.md] | Removed process section | ✅ | [Date] |
| [instruction-2.md] | Updated steps | ✅ | [Date] |

## Other References

| Location | Changes | Status | Updated |
|----------|---------|--------|---------|
| README.md | Removed from agent list | ✅ | [Date] |
| .github/copilot-instructions.md | Removed from agents | ✅ | [Date] |
| Any ADRs | Updated context | ✅ | [Date] |

---

**Total files updated:** X  
**Status:** ✅ ALL COMPLETE
```

---

### Step 3: Pilot Test New Workflows

**Before full rollout:**

```
□ Select a pilot group:
  - 1-2 agents who use this process
  - Volunteer participants
  - Low-risk scenarios first
  
□ Execute new workflow:
  - Follow new process exactly
  - Document any issues
  - Track metrics (time, quality, errors)
  - Collect feedback
  
□ Measure outcomes:
  - Success rate
  - Execution time
  - Manual effort required
  - User satisfaction
  - Any blockers
  
□ Compare vs old:
  - Better/worse/same?
  - Acceptable quality loss?
  - Time trade-off worth it?
  - Any critical issues?
  
□ Decision:
  - ✅ Ready for full rollout
  - ⚠️ Ready with mitigations
  - ❌ Not ready, needs redesign
```

**Pilot Testing Report Template:**

```markdown
# Process Pilot Testing: [Process Name]

**Period:** [Dates]  
**Participants:** [Names/teams]  
**Process Tested:** [Process]  

## Test Results

| Metric | Target | Actual | Status |
|--------|--------|--------|--------|
| Execution time | < 20 min | 22 min | ⚠️ SLIGHTLY OVER |
| Success rate | > 90% | 91% | ✅ OK |
| Manual effort | < 30% | 35% | ⚠️ SLIGHTLY OVER |
| User satisfaction | > 4/5 | 4.2/5 | ✅ OK |
| Blockers | 0 | 1 | ⚠️ FOUND |

## Issues Found

1. **Issue:** [Description]
   - Impact: [Impact]
   - Workaround: [Workaround]
   - Fix: [Fix planned]

2. **Issue:** [Description]
   - Impact: [Impact]
   - Workaround: [Workaround]
   - Fix: [Fix planned]

## Feedback

**Positive:**
- [Feedback 1]
- [Feedback 2]

**Concerns:**
- [Concern 1]
- [Concern 2]

**Suggestions:**
- [Suggestion 1]
- [Suggestion 2]

## Decision

✅ **APPROVED FOR ROLLOUT**

Conditions:
- [Condition 1]
- [Condition 2]

OR

❌ **NEEDS ITERATION**

Next steps:
- [Action 1]
- [Action 2]

---

**Report prepared by:** [Name]  
**Date:** [Date]  
**Approved by:** @SARAH
```

---

### Step 4: Communicate Process Changes

```
□ Notify all affected agents/teams:
  - What process is changing
  - Why it's changing (agent removal)
  - What new workflow looks like
  - How it affects them
  - When it takes effect
  
□ Provide training materials:
  - Updated process documentation
  - Step-by-step guides
  - Video walkthrough (if complex)
  - Examples and templates
  - FAQ with answers
  
□ Open feedback window:
  - Concerns/questions?
  - Collected and addressed
  - Documentation improved based on feedback
  
□ Set support channel:
  - Questions during transition?
  - Contact @SARAH
  - Issues escalation path
```

**Change Notification Template:**

```markdown
# PROCESS CHANGE NOTIFICATION

**Effective Date:** [Date]  
**Reason:** Agent [Agent Name] removal  

## What's Changing

Process: **[Process Name]**

**Before:**
```
[Old workflow]
```

**After:**
```
[New workflow]
```

## What This Means For You

If you use this process:
- New steps: [Step 1], [Step 2], [Step 3]
- Manual effort increases from X% to Y%
- Timeline changes from 10 min to 20 min
- [Positive impact]: [Impact]
- [Negative impact]: [Impact]

## Timeline

- **[Date]:** Notification (TODAY)
- **[Date]:** Training available
- **[Date]:** Process change effective
- **[Date]:** Old process unavailable

## Support

- Questions? Ask @SARAH
- Issues? Report immediately
- Feedback? Always welcome

---

**This change was tested with pilot group. Feedback incorporated.**
```

---

### Step 5: Full Rollout of New Workflows

```
□ Set effective date:
  - When new process becomes standard
  - All teams switch simultaneously
  - Monitor closely for issues
  
□ Activate new process:
  - All documentation live
  - Updated prompts active
  - Guidelines follow new path
  - Old references removed
  
□ Monitor transition:
  - First week: Daily checks
  - Second week: Every other day
  - Third week: Weekly review
  - Track metrics carefully
  
□ Handle issues:
  - Issues arise? Quick mitigation
  - Blockers? Escalate to @SARAH
  - Quality drops? Re-examine approach
  - Rollback if critical failure
```

---

## ONLY AFTER Process Redesign: Agent Removal

Once new workflows are live and stable (3-5 days minimum):

### Phase 2: Pre-Removal Audit (Existing)

See previous section "When to Remove Agents"

---

### Phase 3: Stakeholder Notification (Existing)

See previous section

---

### Phase 4: Agent Deactivation (Existing)

See previous section

---

### Phase 5: Post-Removal Verification (Existing)

See previous section

---

## Complete Timeline (Process + Removal)

```
PHASE 0: PROCESS ANALYSIS & REDESIGN (7-15 days)
├─ Days 1-3: Identify all process dependencies
├─ Days 2-5: Design agent-free workflows
├─ Days 4-7: Update all documentation
├─ Days 7-10: Pilot test new workflows
├─ Days 10-15: Full rollout of new processes
└─ Days 13-15: Stabilization & monitoring

PHASE 1: AGENT REMOVAL (7-10 days)
├─ Days 1-2: Approval & decision
├─ Days 3-4: Stakeholder notification
├─ Days 5-10: Transition & deactivation
└─ Days 11-15: Post-removal verification

TOTAL TIMELINE: 4-5 WEEKS (Process redesign + Agent removal)
```

---

## Removal Decision Logic

```
┌────────────────────────────────────┐
│ Should we remove this agent?       │
└────────────────────────────────────┘
                │
                ▼
    ┌───────────────────────┐
    │ What processes use it?│
    └───────────────────────┘
                │
    ┌───────────┼───────────┐
    │           │           │
    ▼           ▼           ▼
  NONE        SOME       MANY
    │           │           │
    │      ┌────▼────┐      │
    │      │ Critical?     │
    │      └────┬────┘      │
    │           │           │
    │      YES  │  NO       │
    │      │    │    │      │
    ▼      ▼    ▼    ▼      ▼
  ✅      ❌   ✅   ✅     ✅
 SAFE   BLOCK SAFE SAFE   SAFE
 REMOVE

  If BLOCK: Cannot remove without process redesign
  If SAFE:  Can proceed with removal (redesign optional)
```

---

## Critical Success Factors

✅ **Processes redesigned BEFORE removal**  
✅ **All documentation updated BEFORE removal**  
✅ **New workflows tested & approved**  
✅ **Team trained on new processes**  
✅ **Minimum 5-day stability period**  
✅ **No quality degradation**  
✅ **Clear rollback capability**  

---

## Common Mistakes to Avoid

❌ Removing agent without analyzing processes  
❌ Ignoring "critical" process dependencies  
❌ Updating documentation AFTER removal  
❌ Skipping pilot testing of new workflows  
❌ Rushing rollout of new processes  
❌ Not communicating process changes  
❌ Removing without stabilization period  

---

## When to Stop (Red Flags)

🚨 **Stop removal if:**
- Process analysis reveals blocking dependencies
- New workflow pilot shows quality drop > 10%
- Critical issues found during testing
- Team unable to adopt new process
- Replacement agent capacity insufficient
- Business impact too high

**Action:** Redesign or cancel removal

---

## Process Change Impact Assessment

```markdown
# Impact Assessment: [Process Name] Without [Agent Name]

## Affected Entities

- [Team 1]: [Impact]
- [Team 2]: [Impact]
- [Workflow 1]: [Impact]
- [Workflow 2]: [Impact]

## Quality Impact

- Current quality: 95%
- New quality: 90%
- Acceptable? [YES / WITH MITIGATION / NO]

## Efficiency Impact

- Current time: 10 min
- New time: 20 min
- Acceptable? [YES / WITH MITIGATION / NO]

## Effort Impact

- Current manual: 0%
- New manual: 40%
- Sustainable? [YES / NO / SHORT-TERM ONLY]

## Risk Assessment

- Risk level: [LOW / MEDIUM / HIGH]
- Mitigations in place: [List]
- Rollback capability: [YES / NO]
- Go/No-Go: [GO / NO-GO]
```

---

## Related Documents

- [SARAH-SUBAGENT-COORDINATION.md](../../.ai/guidelines/SARAH-SUBAGENT-COORDINATION.md) — Agent lifecycle
- [.ai/workflows/](../../.ai/workflows) — All process workflows
- [.ai/collaboration/](../../.ai/collaboration) — Collaboration patterns
- [.github/agents/SARAH.agent.md](../agents/SARAH.agent.md) — SARAH definition

---

**REMEMBER:** Remove agents only after processes can run without them.

```
REMOVAL CRITERIA:
🔴 Redundant Agent
   - Another agent provides same capabilities
   - Coverage by existing agent is sufficient
   - No unique specialization

🔴 Poor Performance
   - Consistently <85% success rate
   - Response times > SLA by 50%+
   - Error rate > 5% for extended period
   - Quality score < 85% for 30+ days

🔴 Project-Specific Agent
   - Specialized for completed project
   - No ongoing need for capabilities
   - Project entered maintenance mode
   - Can migrate tasks to general agents

🔴 Low Adoption
   - Delegation rate < 10% of capacity
   - No usage for 30+ days
   - Minimal user base
   - Tasks easily handled elsewhere

🔴 Resource Optimization
   - Specialized niche no longer needed
   - Team reorganization
   - Portfolio consolidation
   - Budget constraints
```

---

## Pre-Removal Audit Checklist

Before removing an agent, complete this audit:

### 1. Performance Analysis

```
□ Review last 30 days metrics:
  □ Total delegations received
  □ Success rate (target: >95%)
  □ Average response time
  □ Quality score
  □ Error rate
  
□ Identify performance issues:
  □ Is performance issue systemic?
  □ Can it be fixed? (resources, training)
  □ Is removal the right solution?
  
□ Compare vs other agents:
  □ Similar agents in network?
  □ Overlapping capabilities?
  □ Performance comparison
  □ Utilization comparison
```

### 2. Usage Pattern Analysis

```
□ Analyze delegation patterns:
  □ Which agents delegate to it?
  □ Task types primarily handled
  □ Request sources
  □ Frequency trends (increasing/decreasing)
  
□ Identify critical tasks:
  □ Unique capabilities provided?
  □ Irreplaceable functionality?
  □ Can tasks migrate elsewhere?
  □ Dependencies on this agent?
  
□ Assess adoption:
  □ Days since last delegation
  □ Adoption curve
  □ User satisfaction scores
  □ Feedback patterns
```

### 3. Capability Coverage Check

```
□ Document current capabilities:
  - [Capability 1]
  - [Capability 2]
  - [Capability 3]
  
□ Identify replacement paths:
  □ Which agent provides similar capability?
  □ Coverage gap analysis
  □ Performance of replacement
  □ User acceptance of migration
  
□ List affected workflows:
  - [Workflow 1] → [New agent]
  - [Workflow 2] → [New agent]
  - [Workflow 3] → [New agent]
```

### 4. Risk Assessment

```
□ Identify dependencies:
  □ Other agents depending on it?
  □ Critical workflows using it?
  □ User groups heavily relying on it?
  □ Potential disruption level?
  
□ Communication readiness:
  □ Stakeholder list prepared?
  □ Replacement solution identified?
  □ Transition plan created?
  □ Documentation updated?
```

---

## Removal Process (7 Steps)

### STEP 1: Approval & Decision

**Duration:** 1-2 days

```
□ @SARAH evaluates removal criteria
  - Performance data reviewed
  - Redundancy confirmed
  - Alternatives identified

□ Approval documented:
  - Decision: APPROVED / REJECTED
  - Reason for removal
  - Date of decision
  - Alternative agents identified

□ If REJECTED:
  - Document improvement plan
  - Set 30-day review schedule
  - Communicate to team
  - → STOP PROCESS

□ If APPROVED:
  - Proceed to STEP 2
```

**Decision Template:**

```markdown
# Agent Removal Decision: [Agent Name]

**Decision:** APPROVED for removal
**Date:** [Date]
**Reason:** [Reason for removal]

**Supporting Data:**
- Performance Score: X%
- Usage (last 30d): X delegations
- Error Rate: X%
- Coverage: [Agent handling same tasks]

**Replacement Plan:**
- [Task type 1] → [Replacement agent]
- [Task type 2] → [Replacement agent]

**Stakeholders Affected:**
- [Agent/Team 1]
- [Agent/Team 2]

**Timeline:**
- Notification: [Date]
- Transition Period: [Duration]
- Deactivation: [Date]
```

---

### STEP 2: Stakeholder Notification

**Duration:** 2-3 days before deactivation

```
□ Notify all stakeholders:
  - Agents using this agent
  - Users/teams relying on it
  - Project owners
  - Product managers
  
□ Send notification message:
  - Clear removal date
  - Reason for removal
  - Transition instructions
  - New agent/solution to use
  - Q&A window (24-48 hours)
  
□ Document questions & answers:
  - Common concerns addressed
  - FAQs created
  - Feedback collected
```

**Notification Template:**

```markdown
# NOTICE: Agent Deprecation

**Agent:** [Agent Name]
**Removal Date:** [Date and time]
**Reason:** [Brief reason]

## What's Changing

This agent will be deactivated on [date] due to [reason].

## What You Should Do

Transition your tasks to: **@[Replacement Agent]**

Recommended migration path:
- [Task 1] → Use [Replacement Agent]
- [Task 2] → Use [Replacement Agent]

## Transition Support

- Transition period: [Start] to [End]
- Both agents available during this time
- Questions? Ask @SARAH

## Timeline

- **Today:** Notification sent
- **[Date]:** Last day to start new delegations
- **[Date]:** Both agents available
- **[Date]:** Original agent deactivated
- **[Date]:** Data archived

---

For questions or concerns, ping @SARAH.
```

---

### STEP 3: Transition Planning

**Duration:** 5-7 days

```
□ Create migration plan:
  - Document all active delegations
  - List pending tasks
  - Identify in-progress work
  - Plan handoff timing
  
□ Prepare replacement agent:
  - Verify capacity for migrated work
  - Test capability coverage
  - Alert replacement agent of incoming load
  - Set up monitoring
  
□ Document migration guide:
  - Old format → New format mapping
  - Request template changes
  - Response format expectations
  - Any workarounds needed
  
□ Set up feedback mechanism:
  - Monitoring during transition
  - Issue escalation process
  - Performance metrics tracked
  - User feedback collection
```

**Migration Plan Template:**

```markdown
# Agent Removal Migration Plan

**Agent Being Removed:** [Agent Name]
**Removal Date:** [Date]
**Replacement Agent:** [Agent Name]

## Migration Mapping

| Original Task Type | Removal Agent | → | Replacement Agent | Notes |
|---|---|---|---|---|
| [Task 1] | @Old-Agent | → | @New-Agent | [Any notes] |
| [Task 2] | @Old-Agent | → | @New-Agent | [Any notes] |
| [Task 3] | @Old-Agent | → | @New-Agent | [Any notes] |

## Active Work Handling

**In Progress Delegations:**
- [Delegation 1] - Complete before [date]
- [Delegation 2] - Migrate to [agent]
- [Delegation 3] - Cancel and notify user

## User Communication

- Notification sent: [Date]
- Q&A window: [Dates]
- Documentation updated: [Date]
- Training provided: [Method]

## Monitoring Plan

- Daily check-ins during transition
- Performance metrics: [Metrics tracked]
- Escalation process: Contact @SARAH
- Rollback trigger: [If X happens, rollback]
```

---

### STEP 4: Active Delegation Handling

**Duration:** Throughout transition period

```
□ Handle active delegations:
  - Complete high-priority work
  - Gracefully finish in-progress tasks
  - Document partial results
  - Notify users of completion status
  
□ Prevent new delegations:
  - Update agent status to "deprecated"
  - Redirect new requests to replacement
  - Log any attempts to use removed agent
  - Provide helpful error message
  
□ Monitor for issues:
  - Track delegation success rate
  - Monitor replacement agent load
  - Check for errors/escalations
  - Maintain quality standards
```

**Status Message (When Deprecated):**

```markdown
⚠️ AGENT DEPRECATED

[Agent Name] will be removed on [Date].

Please use @[Replacement Agent] instead.

Active work will be completed before removal date.
New requests will be automatically routed to the replacement.
```

---

### STEP 5: Data & Logs Archival

**Duration:** Same day as deactivation

```
□ Archive agent definition:
  - Save full agent.md to archive
  - Location: .ai/archive/agents/[agent-name]-[date].md
  - Include version history if available
  
□ Archive performance logs:
  - 30-day performance metrics
  - Delegation history
  - Quality scores
  - Error logs
  - Location: .ai/archive/logs/[agent-name]-[date].md
  
□ Archive task history:
  - Completed delegations (summary)
  - Success patterns
  - Common issues
  - User feedback
  - Location: .ai/archive/usage/[agent-name]-[date].md
  
□ Create removal report:
  - Why removed
  - Performance summary
  - Replacement agent
  - Key learnings
  - Future recommendations
  - Location: .ai/archive/removal-reports/[agent-name]-[date].md
```

**Archive Structure:**

```
.ai/archive/
├── agents/
│   └── [agent-name]-2025-12-30.md
├── logs/
│   └── [agent-name]-performance-2025-12-30.md
├── usage/
│   └── [agent-name]-delegations-2025-12-30.md
└── removal-reports/
    └── [agent-name]-removal-report-2025-12-30.md
```

---

### STEP 6: Agent Deactivation

**Duration:** Single action

```
□ Deactivate from agent network:
  - Update .github/agents/ registry
  - Mark agent as "inactive"
  - Remove from active agent list
  - Update agent descriptions
  
□ Update documentation:
  - Remove from copilot-instructions.md agents list
  - Update AGENT_COORDINATION.md
  - Remove from any workflow references
  - Update capability matrix
  
□ Verify deactivation:
  - Agent no longer appears in suggestions
  - @Agent-Name mentions fail gracefully
  - Monitoring shows zero delegations
  - No active processes running
```

**Documentation Updates Checklist:**

```markdown
# Documentation Updates for [Agent Removal]

□ .github/copilot-instructions.md
  - Remove from agent table
  - Update agent list count
  
□ .ai/collaboration/AGENT_COORDINATION.md
  - Remove from agent definitions
  - Update coordination matrix
  
□ .ai/guidelines/COMMUNICATION-OVERVIEW.md
  - Remove agent examples
  - Update SubAgent types
  
□ .github/agents/SARAH.agent.md
  - Remove agent from managed list
  - Update status section
  
□ README.md files
  - Remove from project overview
  - Update agent count
  
□ Any project-specific docs
  - Search for agent references
  - Update or remove mentions
```

---

### STEP 7: Post-Removal Verification & Reporting

**Duration:** 3-5 days after deactivation

```
□ Verify clean deactivation:
  - No delegation attempts received
  - Replacement agent handling all requests
  - Performance metrics normal
  - No errors from removed agent
  
□ Monitor replacement agent:
  - Performance maintained
  - Response times acceptable
  - Quality scores stable
  - User satisfaction confirmed
  
□ Gather feedback:
  - User feedback on transition
  - Any issues or gaps?
  - Suggestions for improvements
  - Document lessons learned
  
□ Create final removal report:
  - Removal successful? YES / NO
  - Issues encountered: [List]
  - Lessons learned: [List]
  - Time to completion: [Duration]
  - Cost savings: [If applicable]
  - Recommendations for future removals
  
□ Publish report:
  - Location: .ai/archive/removal-reports/
  - Share with team
  - Update guidelines if needed
```

**Final Removal Report Template:**

```markdown
# Removal Report: [Agent Name]

**Removal Date:** [Date]
**Status:** ✅ COMPLETED

## Summary

[Agent Name] has been successfully removed from the Copilot network.
Replacement tasks handled by @[Replacement Agent].

## By The Numbers

- **Removal Date:** [Date]
- **Total Active Delegations:** X (all completed)
- **Migration Success Rate:** 100%
- **Transition Time:** X days
- **Issues Encountered:** [Count] (all resolved)

## Transition Metrics

| Metric | Before | After | Status |
|--------|--------|-------|--------|
| Task Coverage | [Agent] | @[New Agent] | ✅ OK |
| Response Time | Xm | Xm | ✅ Maintained |
| Quality Score | X% | X% | ✅ Maintained |
| Error Rate | X% | X% | ✅ Improved |

## Issues & Resolutions

| Issue | Impact | Resolution | Status |
|-------|--------|-----------|--------|
| [Issue 1] | Minor | [Resolution] | ✅ Resolved |
| [Issue 2] | Medium | [Resolution] | ✅ Resolved |

## Lessons Learned

**What went well:**
- [Learning 1]
- [Learning 2]

**What could improve:**
- [Improvement 1]
- [Improvement 2]

**Recommendations for future removals:**
- [Recommendation 1]
- [Recommendation 2]

## Archived Data

All agent data archived in:
- `.ai/archive/agents/[agent-name]-[date].md`
- `.ai/archive/logs/[agent-name]-[date].md`
- `.ai/archive/usage/[agent-name]-[date].md`

---

**Report Prepared By:** @SARAH  
**Date:** [Date]  
**Approval:** @[Authority]  
```

---

## Removal Timeline Example

```
DAY 1:
├─ Morning: @SARAH evaluates removal criteria
├─ Midday: Removal decision documented
└─ Evening: Audit checklist completed

DAY 2-3:
├─ Stakeholder notifications sent
├─ Q&A window opens
└─ Transition plan created

DAY 4-7:
├─ Migration guide published
├─ Replacement agent prepared
├─ Active delegations managed
└─ User feedback collected

DAY 8:
├─ Final delegations completed
├─ Data archived
├─ Agent deactivated
├─ Documentation updated
└─ Monitoring activated

DAY 9-13:
├─ Transition monitoring
├─ Performance verification
├─ Feedback collection
└─ Final report created

TOTAL: ~2 weeks from decision to full completion
```

---

## Rollback Procedure

If removal should be reversed:

```
ROLLBACK TRIGGERS:
🔴 Replacement agent cannot handle load
🔴 Quality drops significantly (>10%)
🔴 Critical issues discovered
🔴 Unanticipated use case found

ROLLBACK STEPS:
1. @SARAH decides to rollback
2. Reactivate agent from archive
3. Restore agent definition
4. Update documentation
5. Notify users of reactivation
6. Monitor performance
7. Create incident report
```

---

## Quick Checklist

**Print this for quick reference:**

```
AGENT REMOVAL CHECKLIST

PRE-REMOVAL:
□ Performance audit complete
□ Usage analysis done
□ Replacement identified
□ Risk assessment complete

APPROVAL:
□ Removal decision documented
□ Stakeholders identified
□ Timeline established

NOTIFICATION:
□ Notifications sent
□ Q&A window open
□ Feedback collected

TRANSITION:
□ Migration plan created
□ Replacement agent ready
□ Active work managed
□ New requests redirected

DEACTIVATION:
□ Agent definitions archived
□ Logs archived
□ Documentation updated
□ Agent deactivated

VERIFICATION:
□ Clean deactivation confirmed
□ Replacement agent stable
□ Feedback collected
□ Report created
□ Lessons documented
```

---

## Common Issues & Solutions

| Issue | Symptom | Solution |
|-------|---------|----------|
| **High migration burden** | Too many active delegations | Extend transition period |
| **Replacement overloaded** | New agent response time > SLA | Distribute across agents |
| **Data loss risk** | Archives incomplete | Pause removal, audit logs |
| **User resistance** | Refusal to migrate | Extra training, gradual transition |
| **Performance drop** | New agent worse than old | Rollback if necessary |
| **Incomplete migration** | Some users still using old | Forced redirect + support |

---

## Related Documents

- [SARAH-SUBAGENT-COORDINATION.md](../../.ai/guidelines/SARAH-SUBAGENT-COORDINATION.md) — Agent lifecycle management
- [COMMUNICATION-OVERVIEW.md](../../.ai/guidelines/COMMUNICATION-OVERVIEW.md) — Agent communication
- [.github/agents/SARAH.agent.md](../agents/SARAH.agent.md) — SARAH definition
- [copilot-instructions.md](../copilot-instructions.md) — Agent registry

---

**Usage:** When @SARAH needs to remove an agent from the network, follow this prompt to ensure smooth, documented deactivation with minimal disruption.
