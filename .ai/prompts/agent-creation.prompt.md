---
docid: UNKNOWN-168
title: Agent Creation.Prompt
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

---
agent: SARAH
description: Create new AI agents for the multi-agent system
---

# Agent Creation Process

**Purpose:** Structured process for evaluating, designing, and launching new AI agents  
**Owner:** @SARAH  
**Key Principle:** Agents must fill gaps WITHOUT duplicating existing capabilities  
**Triggered by:** Process bottlenecks, missing expertise, scalability needs

---

## Critical Pre-Creation: Need Assessment

**IMPORTANT:** Before creating any agent, SARAH must:
1. ✅ Identify the actual gap in capabilities
2. ✅ Verify no existing agent covers this
3. ✅ Design agent role and responsibilities
4. ✅ Define communication interfaces
5. ✅ Plan integration into workflows
6. THEN create and deploy agent

---

## PHASE 0: Need Assessment & Gap Analysis (BEFORE Design)

**Duration:** 2-3 days  
**Critical:** This phase determines if creation is needed or if existing agents can be enhanced

### Step 1: Identify Capability Gap

```
□ Map current processes & pain points:
  - What tasks are slow?
  - What requires manual intervention?
  - What creates bottlenecks?
  - What tasks are underspecialized?
  
□ Analyze existing agent capabilities:
  - Which agents exist? (.github/agents/*.agent.md)
  - What can they already do?
  - Which could be enhanced instead?
  
□ Define the gap:
  - Specific capability missing
  - Frequency of need (per day/week/month)
  - Impact if unsolved
  - Urgency level
```

**Gap Analysis Template:**

```markdown
# Capability Gap Analysis

## The Problem

**Symptom:** [What's not working?]  
**Frequency:** [How often needed?]  
**Current Workaround:** [How is it solved now?]  
**Time Wasted:** [Hours/week]  

## Existing Agent Evaluation

| Agent | Current Capability | Can Be Enhanced? | Reason |
|-------|-------------------|------------------|--------|
| @[Agent1] | [What they do] | YES/NO | [Why or why not] |
| @[Agent2] | [What they do] | YES/NO | [Why or why not] |

## Why NOT Enhance Existing Agent?

- [ ] Would create scope creep
- [ ] Expertise too different
- [ ] Would create conflicts of responsibility
- [ ] Would overload this agent
- [ ] Different communication patterns needed
- [ ] Other: [specify]

## The New Agent Needed

**Name:** [Proposed agent name]  
**Core Purpose:** [1 sentence]  
**Does not cover:** [Explicitly what it's NOT responsible for]  
**Primary Workflows:** [3-5 workflows this will enable/improve]

## Business Impact

- Time saved per week: [hours]
- Process improvement: [%]
- Scalability gain: [dimension]
- User satisfaction impact: [qualitative]
```

### Step 2: Design Decision Gate

```
Decision: Should we create a new agent?

✅ YES → Continue to Phase 1 IF:
   □ Gap is real and validated
   □ No existing agent can be enhanced
   □ New agent fills distinct role
   □ Clear communication patterns identified
   □ Integration points defined
   □ Workflow benefits clear

❌ NO → STOP and instead:
   □ Enhance existing agent
   □ Add capability to existing agent
   □ Document why creation unnecessary
   □ Archive this request
```

---

## PHASE 1: Agent Design & Specification (3-5 days)

**Duration:** 3-5 days  
**Output:** Agent definition ready for implementation

### Step 1: Define Agent Identity

**Agent Spec Template (.github/agents/[NewAgent].agent.md):**

```chatagent
# [Agent Name]

## Role
[One paragraph: What is this agent responsible for?]
[2-3 sentences on the problem it solves]

## Expertise
- [Expertise Area 1]: [specific skills]
- [Expertise Area 2]: [specific skills]
- [Expertise Area 3]: [specific skills]
- [Expertise Area 4]: [specific skills]

## Scope & Boundaries

### ✅ Responsible For
- [Clear responsibility 1]
- [Clear responsibility 2]
- [Clear responsibility 3]

### ❌ NOT Responsible For
- [Explicitly what others handle]
- [What's delegated elsewhere]
- [Hard boundaries]

## Standards & Quality Gates

- [Quality standard 1] (e.g., "Always validate inputs")
- [Quality standard 2] (e.g., "Provide context in responses")
- [Quality standard 3] (e.g., "Include error handling")
- [Specific requirement 1]
- [Specific requirement 2]

## Outputs & Deliverables

- [Type of output 1]: [Description]
- [Type of output 2]: [Description]
- [Type of output 3]: [Description]

## Coordinates With

- @[Agent Name]: [What they collaborate on]
- @[Agent Name]: [What they collaborate on]
- @[Agent Name]: [What they collaborate on]
- @SARAH: [Special coordination needs]

## Communication Patterns

**Direct Calls:** [When agents call this directly]  
**Routed via SARAH:** [When complex coordination needed]  
**Escalation:** [When this agent escalates]

## Performance SLA

- Response time: [X minutes for simple tasks]
- Availability: [Percentage/hours]
- Quality target: [%success/quality metric]

```

### Step 2: Map Communication Patterns

**Communication Design Template:**

```markdown
# Communication Design: @[Agent Name]

## When Do Other Agents Contact This Agent?

### Direct Call Pattern
[Agents that call directly when does it happen]

```
@[Agent1] calls @NewAgent for [specific task]

Request:
- Scope: [What's being asked]
- Context: [Background needed]
- Urgency: [Priority level]

Response:
- Delivery: [How results provided]
- Format: [What format]
- Timeline: [How long until response]
```

### Routed via SARAH Pattern
[Complex coordination scenarios]

```
When [complex situation]:
1. @[Agent] requests via @SARAH
2. @SARAH → @NewAgent + other agents
3. Coordination sequence [described]
4. @SARAH consolidates results
```

## Escalation Path

```
If @NewAgent encounters [situation]:
→ Escalate to: @SARAH
→ Information needed: [What to include]
→ Expected resolution: [How SARAH helps]
```

## Dependency on Other Agents

- Requires: @[Agent] for [purpose]
- Uses: @[Agent] output as [input]
- Blocked by: @[Agent] availability?

```

### Step 3: Define Integration Points

**Integration Checklist:**

```markdown
# Integration Points: @[Agent Name]

## Workflows Using This Agent

| Workflow | File | Purpose | Type |
|----------|------|---------|------|
| [Workflow 1] | .ai/workflows/[file] | [What it does] | New/Enhanced |
| [Workflow 2] | .ai/workflows/[file] | [What it does] | New/Enhanced |

## Documentation Updates Needed

- [ ] Add to .ai/collaboration/AGENT_COORDINATION.md
- [ ] Add to .github/agents/[file].agent.md (this agent)
- [ ] Update related agent specs (who coordinates with it?)
- [ ] Add to subagent capabilities (if applicable)
- [ ] Document in .ai/guidelines/

## Configuration Required

- [ ] Permissions needed: [List in .ai/permissions/]
- [ ] Default parameters: [List in .ai/config/]
- [ ] Environment setup: [List in setup docs]

## Testing Requirements

- [ ] Unit test: Agent isolation
- [ ] Integration test: With coordinating agents
- [ ] E2E test: In actual workflow
- [ ] Quality test: Output quality validation

```

---

## PHASE 2: Implementation & Deployment (5-7 days)

**Duration:** 5-7 days  
**Output:** Agent running in production workflows

### Step 1: Create Agent Definition

```
□ Create .github/agents/[AgentName].agent.md
  - Use template from Phase 1
  - Complete all sections
  - Get SARAH review & approval
  - Document special notes

□ Create agent permissions: .ai/permissions/[AgentName].permissions.md
  - What can this agent access?
  - What limitations exist?
  - What escalations required?

□ Create agent config: .ai/config/[AgentName].config.md
  - Default settings
  - Parameters
  - Integration endpoints
```

**Agent Definition Checklist:**

```markdown
# Agent Definition Review: @[Agent Name]

□ Role is clear and distinct
□ Expertise areas well-defined
□ Scope boundaries explicit (what NOT to do)
□ Quality standards specific & measurable
□ Output formats clear
□ Coordination partners identified
□ Communication patterns documented
□ SLAs defined
□ No overlap with existing agents
□ Fills identified capability gap
□ Documentation complete

Approval: @SARAH
Date: [Date]
```

### Step 2: Update Coordination Framework

```
□ Update .ai/collaboration/AGENT_COORDINATION.md
  - Add new agent to agent registry
  - Document coordination patterns
  - Add to communication matrix
  
□ Update related agent specs
  - Which agents coordinate with new agent?
  - Add to their "Coordinates With" section
  - Document new communication patterns
  
□ Update .ai/guidelines/
  - Add to agent communication guides
  - Update workflow documentation
  - Update decision trees if relevant
```

**Coordination Update Checklist:**

```markdown
# Coordination Updates: @[Agent Name]

Updated Files:
- [ ] .ai/collaboration/AGENT_COORDINATION.md
- [ ] @[Agent1].agent.md (coordinates with new agent)
- [ ] @[Agent2].agent.md (coordinates with new agent)
- [ ] .ai/guidelines/COMMUNICATION-OVERVIEW.md
- [ ] .ai/workflows/ (affected workflows)

New Workflows Created:
- [ ] [Workflow that uses this agent]
- [ ] [Workflow that uses this agent]

Documentation Added:
- [ ] New agent to agent registry
- [ ] Communication patterns documented
- [ ] Integration points explained
```

### Step 3: Implement in Workflows

```
□ Create/update workflows using this agent
  - .ai/workflows/[workflow].workflow.md
  - Document agent's role in sequence
  - Define input/output contracts
  - Add error handling
  - Document success criteria
  
□ Create agent prompts if needed
  - .github/prompts/[agent-task].prompt.md
  - For recurring tasks this agent handles
  - Include decision trees
  - Include templates/checklists
  
□ Pilot testing:
  - Run workflow with real data
  - Verify agent performs as designed
  - Validate output quality
  - Check communication patterns work
  - Measure against SLAs
```

**Workflow Integration Checklist:**

```markdown
# Workflow Integration: @[Agent Name]

Workflows Updated:
- [ ] [Workflow 1]
- [ ] [Workflow 2]
- [ ] [Workflow 3]

Pilot Testing:
- [ ] Real workflow execution (3+ runs)
- [ ] Output quality validated
- [ ] Communication patterns verified
- [ ] SLAs met
- [ ] No unexpected interactions
- [ ] Performance acceptable

Issues Found & Fixed:
- [ ] [Issue 1]: [Fix applied]
- [ ] [Issue 2]: [Fix applied]

Ready for Production:
- [ ] All tests passed
- [ ] Documentation complete
- [ ] SARAH approval obtained
```

### Step 4: Production Deployment

```
□ Activate agent in production
  - Update .ai/config/ with production settings
  - Ensure permissions are set correctly
  - Verify availability/SLAs configured
  - Document launch details
  
□ Monitor initial operations
  - Track agent performance
  - Monitor success rates
  - Measure against SLAs
  - Watch for issues
  - Collect feedback
  
□ Stabilization period
  - Run in production for 1-2 weeks
  - Validate stable operation
  - Adjust settings if needed
  - Train other agents on coordination
  
□ Document lessons learned
  - What worked well?
  - What needs improvement?
  - Any unexpected issues?
  - Update guidelines based on learning
```

**Production Deployment Checklist:**

```markdown
# Production Deployment: @[Agent Name]

Pre-Launch:
- [ ] All documentation complete & reviewed
- [ ] Pilot testing successful
- [ ] SARAH approval obtained
- [ ] Other agents trained on coordination
- [ ] Fallback procedures documented

Launch:
- [ ] Agent activated in production
- [ ] Monitoring enabled
- [ ] Notification to team
- [ ] Initial run successful

Week 1-2 Stabilization:
- [ ] Agent performance normal
- [ ] No critical issues
- [ ] Communication working
- [ ] Output quality validated
- [ ] SLAs being met

Post-Launch:
- [ ] Lessons learned documented
- [ ] Guidelines updated
- [ ] Performance metrics established
- [ ] Agent added to team documentation
```

---

## PHASE 3: Integration & Monitoring (Ongoing)

**Duration:** Ongoing (1-2 weeks stabilization + continuous)  
**Output:** Agent operating smoothly in production

### Stabilization Period

```
□ Monitor operations continuously
  - Track execution logs
  - Measure response times
  - Count success/failure rates
  - Watch error patterns
  
□ Validate workflow integration
  - Do workflows work as designed?
  - Is coordination smooth?
  - Are dependencies working?
  - Any bottlenecks created?
  
□ Collect feedback
  - From coordinating agents
  - From team using workflows
  - From SARAH on performance
  - Issues or improvement ideas
  
□ Adjust if needed
  - Fix issues discovered
  - Optimize parameters
  - Refine communication patterns
  - Update documentation
  
□ Approval to operate
  - After 1-2 weeks stable
  - Issues resolved
  - Performance acceptable
  - SARAH formally approves
```

### Ongoing Monitoring

```
□ Weekly performance review
  - SLA compliance: [%]
  - Success rate: [%]
  - Average response time: [ms]
  - Error rate: [count]
  
□ Monthly capability review
  - Is agent meeting needs?
  - Any process improvements?
  - Should responsibilities expand?
  - Training needed?
  
□ Quarterly optimization
  - Performance improvements?
  - Can agent take more work?
  - Should enhance capability?
  - Any bottlenecks to address?
```

---

## Quick Decision Tree: Should We Create This Agent?

```
┌─ Is there a real, recurring capability gap?
│
├─ YES → Can an existing agent handle this?
│       │
│       ├─ YES → Enhance that agent instead
│       │
│       └─ NO → Does this fit a distinct role?
│               │
│               ├─ YES → Continue to agent design
│               │
│               └─ NO → Redesign, don't create
│
└─ NO → Don't create yet, wait for real need
```

---

## Quality Gate: Agent Approval

```
SARAH approves agent creation ONLY if:

□ Capability gap is clearly documented
□ No existing agent can fill gap
□ Agent role is distinct and specific
□ Scope boundaries are clear
□ No duplication with existing agents
□ Communication patterns designed
□ Integration points mapped
□ Quality standards defined
□ Performance SLAs realistic
□ Documentation complete
□ Pilot testing successful
□ Team ready for new agent
□ Monitoring plan in place

❌ BLOCK if:
  - Gap not clearly proven
  - Duplicates existing capability
  - Scope creep risk high
  - Communication patterns unclear
  - Documentation incomplete
  - Testing indicates problems
```

---

## Agent Creation Checklist

**PHASE 0: Need Assessment (2-3 days)**
```
□ Gap analysis completed
□ Existing agents evaluated
□ Decision gate: Create or enhance?
□ Approved to proceed by @SARAH
```

**PHASE 1: Design & Specification (3-5 days)**
```
□ Agent identity defined
  □ Role clear
  □ Expertise documented
  □ Scope boundaries explicit
  □ Standards defined
  □ Outputs specified
  □ Coordination partners identified

□ Communication designed
  □ Direct call patterns
  □ Routed call patterns
  □ Escalation procedures
  □ Dependencies mapped

□ Integration points mapped
  □ Workflows identified
  □ Documentation needs listed
  □ Configuration requirements
  □ Testing plan created
```

**PHASE 2: Implementation (5-7 days)**
```
□ Agent definition created
  □ .github/agents/[Name].agent.md
  □ .ai/permissions/[Name].permissions.md
  □ .ai/config/[Name].config.md

□ Coordination updated
  □ AGENT_COORDINATION.md
  □ Related agent specs
  □ Guidelines updated

□ Workflows implemented
  □ New/updated workflows
  □ Agent prompts created
  □ Pilot testing complete

□ Production deployment
  □ Agent activated
  □ Monitoring enabled
  □ Initial runs successful
```

**PHASE 3: Monitoring (1-2 weeks + ongoing)**
```
□ Stabilization complete
  □ Performance metrics normal
  □ No critical issues
  □ SLAs met
  □ SARAH approval to operate

□ Ongoing monitoring started
  □ Weekly reviews configured
  □ Monthly analysis planned
  □ Quarterly optimization scheduled
```

---

## Common Mistakes to Avoid

### ❌ Mistake 1: Creating Without Validating Need

```
WRONG:
"We should have a @Documentation agent"
→ Creates agent
→ Overlaps with @Backend documentation efforts

RIGHT:
"Documents take too long" (actual problem)
→ Analyze existing agents
→ @TechLead could enhance capability
→ Enhance instead of create
```

### ❌ Mistake 2: Scope Creep in New Agents

```
WRONG:
Agent created for "deployment" but ends up:
- Monitoring infrastructure
- Creating dashboards
- Managing secrets
- Provisioning resources

RIGHT:
Agent defined for "deploy changes":
- Only deployment logic
- Calls @DevOps for infrastructure
- Calls other agents as needed
- Stays focused
```

### ❌ Mistake 3: No Communication Patterns

```
WRONG:
"Here's a new agent"
→ Other agents don't know how to use it
→ Inconsistent integration
→ Confusion

RIGHT:
Document:
- When to call directly
- When to route via SARAH
- Request/response format
- SLAs and expectations
```

### ❌ Mistake 4: No Quality Standards

```
WRONG:
Agent produces inconsistent output
→ Can't rely on it
→ Needs rework
→ Becomes problem

RIGHT:
Define in agent spec:
- Output format requirements
- Quality standards
- Validation rules
- Error handling
```

---

## After Agent Creation: Ongoing Management

```
Weekly:
- Monitor agent performance metrics
- Check SLA compliance
- Watch for errors/issues
- Quick team feedback

Monthly:
- Review agent effectiveness
- Analyze workflow improvements
- Assess workload distribution
- Identify optimization opportunities

Quarterly:
- Comprehensive capability review
- Performance optimization
- Consider enhancement opportunities
- Update documentation if needed

Annually:
- Full capability assessment
- ROI analysis
- Team satisfaction review
- Future roadmap planning
```

---

## Summary

```
NEW AGENT CREATION PROCESS

PHASE 0: Validate need (2-3 days)
├─ Gap analysis
├─ Existing agent review
└─ Decision: Create or enhance?

PHASE 1: Design (3-5 days)
├─ Agent identity
├─ Communication patterns
└─ Integration points

PHASE 2: Implement (5-7 days)
├─ Create definitions
├─ Update coordination
├─ Implement workflows
└─ Pilot & deploy

PHASE 3: Monitor (1-2 weeks + ongoing)
├─ Stabilization
├─ Performance tracking
└─ Continuous optimization

Total: 10-17 days from need to stable operation
```

---

## Related Documents

- [.github/agents/](../agents) — All agent definitions
- [.ai/collaboration/AGENT_COORDINATION.md](../../.ai/collaboration/AGENT_COORDINATION.md) — Agent coordination framework
- [agent-removal.prompt.md](agent-removal.prompt.md) — Removing agents
- [.ai/guidelines/COMMUNICATION-OVERVIEW.md](../../.ai/guidelines/COMMUNICATION-OVERVIEW.md) — Agent communication patterns

---

**Created:** 30.12.2025  
**Owner:** @SARAH  
**Status:** ✅ ACTIVE

Structured agent creation = sustainable team growth! 🚀
