---
docid: GL-053
title: AGENT REMOVAL GUIDE
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

# Agent Removal Quick Guide

**For:** @SARAH managing agent lifecycle  
**Prompt:** `/agent-removal`  
**Process Duration:** 4-5 weeks (includes process redesign)

---

## ⚠️ CRITICAL PRINCIPLE

**Remove agents AFTER redesigning processes, NOT before**

Before removing an agent:
1. ✅ Analyze which processes use it
2. ✅ Assess process criticality
3. ✅ Design agent-free alternatives
4. ✅ Update all documentation
5. ✅ Pilot test new workflows
6. ✅ Full rollout new processes
7. ✅ THEN remove agent

---

## When to Remove Agents

Remove agents when:
- ❌ **Redundant** — Another agent does the same thing better
- ❌ **Poor Performance** — Success rate < 85% for 30+ days
- ❌ **Project-Specific** — Specialized for completed project
- ❌ **Low Adoption** — Used < 10% of capacity
- ❌ **Resource Optimization** — Need to consolidate portfolio

**BUT ONLY if processes can function without it**

---

## Phase 0: Process Impact Analysis (REQUIRED FIRST)

**Duration:** 3-5 days  
**Output:** Decision to proceed or halt removal

### 1. Identify Process Dependencies
- What processes use this agent?
- How critical is it to each?
- What breaks if it's removed?

### 2. Assess Criticality
- CRITICAL = Cannot remove without process redesign
- HIGH/MEDIUM/LOW = Can redesign and remove

### 3. Design Alternatives
- Option A: Manual process
- Option B: Different agent
- Option C: Process restructure
- Option D: Automation/tooling

**Decision Logic:**
```
CRITICAL processes → CANNOT remove (redesign first)
HIGH/MEDIUM/LOW → CAN remove (after redesign)
```

---

## Phase 1: Process Redesign (MANDATORY BEFORE REMOVAL)

**Duration:** 5-10 days  
**Critical:** All processes must work without agent first

### 1. Redesign Workflows
- Document new process without agent
- Define roles/responsibilities
- Establish quality gates
- Plan for edge cases

### 2. Update Documentation
- Update .ai/workflows/
- Update .ai/guidelines/
- Update .ai/collaboration/
- Update .github/prompts/
- Update .github/instructions/
- Update any ADRs

### 3. Pilot Test
- Test with 1-2 volunteer teams
- Measure: time, quality, effort
- Collect feedback
- Refine if needed

### 4. Full Rollout
- Activate new processes
- Train all users
- Monitor closely (1-2 weeks)
- Ensure stability

### 5. Stabilization
- Minimum 5-day stability period
- No quality issues
- Team comfortable with new process
- Then proceed with agent removal

---

## Phase 2: Agent Removal (ONLY AFTER PROCESS REDESIGN)

Once processes are redesigned and stable:

### 7-Step Removal Process

1. **APPROVAL & DECISION** (1-2 days)
   - Evaluate removal criteria
   - Document decision
   - Identify replacement agent

2. **STAKEHOLDER NOTIFICATION** (2-3 days)
   - Notify all users
   - Provide transition plan
   - Open Q&A window

3. **TRANSITION PLANNING** (5-7 days)
   - Create migration guide
   - Map old tasks → new agent
   - Prepare replacement agent

4. **ACTIVE DELEGATION HANDLING** (During transition)
   - Complete in-progress work
   - Redirect new requests
   - Monitor for issues

5. **DATA & LOGS ARCHIVAL** (Deactivation day)
   - Archive agent definition
   - Save performance logs
   - Create removal report

6. **AGENT DEACTIVATION** (Deactivation day)
   - Remove from network
   - Update all documentation
   - Verify clean removal

7. **POST-REMOVAL VERIFICATION** (3-5 days)
   - Monitor replacement agent
   - Gather user feedback
   - Publish final report

---

## Complete Pre-Removal Audit

Before ANY removal step, verify:

```
PROCESS ANALYSIS:
□ All processes identified
□ Criticality assessed
□ Alternatives designed
□ Documentation updated
□ Pilot testing complete
□ Full rollout successful
□ Stability period (5+ days) met

PERFORMANCE:
□ Success rate (30d avg)
□ Response times
□ Quality score
□ Error rate

REPLACEMENT:
□ Identified alternative agent
□ Capability coverage confirmed
□ Performance acceptable
□ User acceptance expected

RISK:
□ Dependencies identified
□ Disruption level assessed
□ Rollback plan ready
□ Stakeholder communication ready
```

---

## Key Decision Point

```
Before Agent Removal:
  Can all processes function without this agent?
  
  YES → Proceed with removal (after redesign)
  NO → Either redesign OR don't remove
```

---

## Timeline Overview

```
PHASE 0: Process Analysis (3-5 days)
├─ Identify dependencies
├─ Assess criticality
├─ Design alternatives
└─ Decision: Proceed or Stop

PHASE 1: Process Redesign (5-10 days)
├─ Redesign workflows
├─ Update documentation
├─ Pilot test
├─ Full rollout
└─ Stabilization (5+ days)

PHASE 2: Agent Removal (7-10 days)
├─ Approval & notification
├─ Transition planning
├─ Delegation handling
├─ Deactivation
└─ Post-removal verification

TOTAL: 4-5 WEEKS
```

---

## Critical Checkpoints

| Checkpoint | Status | Proceed? |
|-----------|--------|----------|
| Processes analyzed | ✅ | YES |
| Criticality assessed | ✅ | YES |
| Alternatives designed | ✅ | YES |
| Documentation updated | ✅ | YES |
| Pilot testing passed | ✅ | YES |
| Full rollout successful | ✅ | YES |
| Stability period met (5+ days) | ✅ | YES |
| → Proceed to Phase 2 | ✅ | GO |

---

## Key Documents

| Task | Document |
|------|----------|
| **Full process (Phase 0+1+2)** | [agent-removal.prompt.md](.github/prompts/agent-removal.prompt.md) |
| **Phase 0 details** | Included in removal prompt |
| **Phase 1 details** | Included in removal prompt |
| **Agent lifecycle** | [SARAH-SUBAGENT-COORDINATION.md](.ai/guidelines/SARAH-SUBAGENT-COORDINATION.md) |

---

## Common Mistakes to Avoid

❌ Removing without analyzing processes  
❌ Ignoring "critical" process dependencies  
❌ Skipping process redesign  
❌ Removing before processes stable  
❌ Not testing new workflows  
❌ Removing without updating documentation  
❌ Rushing through phase 1  

---

## Success Indicators

✅ All processes function without agent  
✅ 100% of active delegations completed/migrated  
✅ Zero failed migrations  
✅ Replacement agent handles load  
✅ User adoption of new process  
✅ Quality maintained or improved  
✅ All documentation updated  

---

**REMEMBER:** Processes must be redesigned BEFORE agent removal.  
**Start:** Use prompt `/agent-removal` to execute full process  
**Questions:** Refer to [agent-removal.prompt.md](.github/prompts/agent-removal.prompt.md)
