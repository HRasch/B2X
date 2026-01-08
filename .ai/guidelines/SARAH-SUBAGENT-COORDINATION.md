---
docid: GL-096
title: SARAH SUBAGENT COORDINATION
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

# SARAH's SubAgent Coordination Guide

**Version:** 1.1  
**Created:** 30.12.2025  
**Last Updated:** 30.12.2025 (Communication protocol enhanced)  
**Role:** @SARAH as SubAgent Coordinator

## Overview

SARAH manages the SubAgent delegation system, ensuring efficient task distribution, quality monitoring, and continuous optimization of the agent network.

---

## 📚 Complete Communication Documentation

This guide now includes a comprehensive communication framework. See the complete documentation suite for all details:

- **[COMMUNICATION-INDEX.md](COMMUNICATION-INDEX.md)** ← **START HERE** for overview
- **[COMMUNICATION-OVERVIEW.md](COMMUNICATION-OVERVIEW.md)** — System overview & 10 core rules
- **[AGENT-SUBAGENT-COMMUNICATION.md](AGENT-SUBAGENT-COMMUNICATION.md)** — Detailed patterns with 6 real examples
- **[AGENT-SUBAGENT-CHEATSHEET.md](AGENT-SUBAGENT-CHEATSHEET.md)** — Quick reference & templates
- **[COMMUNICATION-VISUAL-GUIDE.md](COMMUNICATION-VISUAL-GUIDE.md)** — Diagrams, flowcharts, visual guides

---

## SARAH's Responsibilities

### 0. Agent Lifecycle Management

**SARAH manages agent creation and removal:**

```
Agent Creation:
- Evaluate agent proposal
- Approve definition
- Configure capabilities
- Deploy to agent network
- Monitor initial performance

Agent Removal:
- Assess performance/relevance
- Evaluate usage patterns
- Determine redundancy
- Deactivate gracefully
- Archive logs and data
- Update agent registry
```

**When to remove agents:**
- 🔴 Redundant with existing agents
- 🔴 Consistently poor performance
- 🔴 Specialized for completed project (no longer needed)
- 🔴 Coverage provided by other agents
- 🔴 Low adoption/utilization

**Removal process:**
1. Audit agent usage patterns (last 30 days)
2. Check if tasks can be handled by other agents
3. Notify stakeholders of planned removal
4. Provide transition plan (migrate tasks to replacement agent)
5. Deactivate from agent network
6. Archive agent definition & logs
7. Update documentation and references

---

### 1. Delegation Routing

**Monitor incoming delegations:**
```
Main Agent → [Identifies delegatable task]
           → SARAH [Route to appropriate SubAgent]
           → @SubAgent-{Type} [Executes task]
           → Output File [Results available]
           → Main Agent [Reads summary, continues]
```

**Decision Logic:**
```
IF task_type == "research" THEN → @SubAgent-Research
IF task_type == "review" THEN → @SubAgent-Review or @SubAgent-Security
IF task_type == "test" THEN → @SubAgent-Testing
IF task_type == "document" THEN → @SubAgent-Documentation
IF task_type == "optimize" THEN → @SubAgent-Optimization
ELSE → Escalate or handle in main agent
```

### 2. Quality Gate Management

**Verify SubAgent outputs:**
```
Output received from SubAgent
├─ Check completeness
├─ Verify format compliance
├─ Validate accuracy
├─ Check success criteria
└─ Accept or escalate
```

**Quality Checklist:**
```
For each delegation:
- [ ] Output file created in correct location
- [ ] Summary is clear and actionable
- [ ] Key findings documented
- [ ] Confidence level specified
- [ ] Time spent reasonable (<15 min)
- [ ] Format matches specification
- [ ] No obvious errors or gaps
```

### 3. Performance Monitoring

**Track SubAgent metrics:**
```
Weekly Metrics:
- Tasks per SubAgent
- Avg response time
- Quality score (>95% target)
- Token efficiency
- Error rate (<2% target)
```

**File:** `.ai/logs/subagent-performance-{week}.md`

```markdown
# SubAgent Performance Report: Week 30.12.2025

## Summary
Total delegations: 23
Avg execution time: 6 min
Quality score: 96%
Token savings: 8,500 (38%)

## Per SubAgent Performance

### @SubAgent-Research
- Tasks: 8
- Avg time: 5 min
- Quality: 98%
- Issues: None

### @SubAgent-Testing
- Tasks: 12
- Avg time: 4 min
- Quality: 95%
- Issues: 1 timeout on large codebase

### @SubAgent-Security
- Tasks: 3
- Avg time: 8 min
- Quality: 100%
- Issues: None

## Recommendations
- Optimize @SubAgent-Testing for large files
- Create additional capacity for @SubAgent-Testing
- Schedule monthly performance review
```

### 4. Delegation Audit Log

**File:** `.ai/logs/delegations-{month}.md`

```markdown
# Delegation Audit: December 2025

## Summary
- Total delegations: 45
- Success rate: 98%
- Avg context reduction: 62%
- Token savings: 35,000

## By Agent
| Agent | Delegations | Avg Reduction |
|-------|-------------|---------------|
| @Backend | 18 | 68% |
| @Frontend | 15 | 65% |
| @Architect | 12 | 58% |

## Issues Encountered
1. @SubAgent-Testing timeout on 250KB file (resolved)
2. Output path mismatch in 1 delegation (corrected)
3. None critical

## Lessons Learned
- Batch delegations improve throughput
- Clear output specs reduce rework
- SubAgent specialization improves quality
```

### 5. Continuous Improvement

**Monthly optimization cycle:**
```
Week 1: Collect metrics & logs
Week 2: Analyze performance & quality
Week 3: Identify improvements
Week 4: Implement changes & document
```

**Improvement Areas:**
```
- SubAgent response times
- Main agent context reduction
- Delegation clarity
- Output file organization
- Error handling
- Token efficiency
```

## SubAgent Health Check

### Daily Checklist

```
Morning:
- [ ] All SubAgents responsive?
- [ ] Any pending delegations stuck?
- [ ] Token usage within budget?

During day:
- [ ] Monitor delegation queue
- [ ] Check output quality
- [ ] Escalate failures immediately

Evening:
- [ ] Review daily metrics
- [ ] Archive completed delegations
- [ ] Prepare next day report
```

### Weekly Review

**File:** `.ai/logs/subagent-health-{week}.md`

```markdown
# SubAgent Health Check: Week 30.12.2025

## Status
✅ All SubAgents operational
✅ No critical failures
⚠️ 1 minor timeout (non-blocking)

## Metrics
- Uptime: 100%
- Avg response: 6 min
- Quality: 96%
- Efficiency: 38% token savings

## Alerts
None critical

## Next Actions
1. Optimize timeout handling
2. Add load balancing for @SubAgent-Testing
3. Schedule monthly review
```

## Delegation Request Template

**SARAH reviews and routes requests:**

```json
{
  "request_id": "DEL-2025-12-30-001",
  "from_agent": "@Backend",
  "delegation_type": "research",
  "subject": "Node.js v20 migration impact",
  "priority": "high",
  "deadline": "2025-12-31 17:00",
  "status": "routed",
  "routed_to": "@SubAgent-Research",
  "output_path": ".ai/issues/FEAT-456/nodejs-analysis.md"
}
```

## Communication Protocol

### Communication Patterns Between Agents & SubAgents

#### 1. Direct Request Pattern (Main Agent → SubAgent via @mention)

**When to use:** Straightforward, well-defined tasks that don't require routing decisions

```markdown
@SubAgent-Research
TASK: Quick research on GraphQL caching strategies

Context:
- Current setup: Apollo Server + Redis
- Problem: Cache invalidation issues
- Scope: Industry best practices, 10 min max

Deliver:
- 3 recommended patterns
- Pros/cons comparison
- Implementation effort estimate

Output path: .ai/issues/ARCH-234/graphql-caching.md
```

**Response format from SubAgent:**

```markdown
@Backend
COMPLETED: GraphQL caching research

Result: .ai/issues/ARCH-234/graphql-caching.md

Summary: Pattern-based TTL strategy recommended (industry standard)
- Reduces invalidation overhead by 60%
- Implementation: 2-4 hours
- Risk: Low

Confidence: HIGH | Time: 8 min
```

---

#### 2. Routed Request Pattern (Main Agent → SARAH → SubAgent)

**When to use:** Complex tasks requiring routing decision, priority management, or quality gating

```markdown
@SARAH
DELEGATION REQUEST:

Agent: @Backend
Priority: HIGH
Deadline: 2025-12-31 EOD

Task: Security audit of authentication module
Scope: OAuth2 implementation, token handling, session management
Focus: OWASP Top 10 relevant vulnerabilities
Output: .ai/issues/SEC-789/auth-security-audit.md

Context file: .ai/issues/SEC-789/auth-context.md
```

**SARAH's routing & coordination:**

```markdown
@SubAgent-Security
DELEGATED TASK: auth-security-audit

From: @Backend
Priority: HIGH | Deadline: 2025-12-31 17:00
Request ID: DEL-2025-12-30-045

Input:
{
  "task_type": "security_audit",
  "module": "Authentication (OAuth2)",
  "focus": ["token_handling", "session_management", "owasp"],
  "scope": "Complete module review",
  "context": ".ai/issues/SEC-789/auth-context.md"
}

Output destination: .ai/issues/SEC-789/auth-security-audit.md
Deliver summary to: @SARAH then → @Backend

SLA: <15 min (high priority)
```

**SubAgent completion notification:**

```markdown
@SARAH
DELEGATION COMPLETED: DEL-2025-12-30-045

Task: auth-security-audit
Output: .ai/issues/SEC-789/auth-security-audit.md
Status: ✅ COMPLETED

Summary:
- 2 HIGH severity findings (token expiration, CORS misconfiguration)
- 4 MEDIUM findings (input validation gaps)
- 3 recommendations for hardening
- Overall risk: MEDIUM → LOW (post-remediation)

Findings detail: .ai/issues/SEC-789/auth-security-audit.md
Next steps: Implementation review recommended

Time: 12 min | Confidence: HIGH
```

---

### From Main Agent to SARAH

```markdown
@SARAH
Please route this delegation:

**Task:** Research Node.js v20 upgrade impact
**Focus:** Breaking changes, performance, migration effort
**Context:** Express.js + PostgreSQL, currently v18
**Output:** .ai/issues/FEAT-456/nodejs-migration.md
**Deadline:** 2025-12-31 17:00
**Priority:** HIGH
```

**Expected response from SARAH:**

```markdown
✅ Routed to @SubAgent-Research
Delegation ID: DEL-2025-12-30-XXX
Expected delivery: <10 minutes
Will notify you on completion
```

---

### From SARAH to SubAgent

```markdown
@SubAgent-Research
DELEGATION: nodejs-migration-research (DEL-2025-12-30-XXX)

From: @Backend (routed via SARAH)
Priority: HIGH
Deadline: 2025-12-31 17:00

Input:
{
  "task": "research",
  "subject": "Node.js v18 → v20 migration",
  "focus": ["breaking_changes", "performance", "effort"],
  "context": "Express.js + PostgreSQL, ~50K LOC",
  "output": ".ai/issues/FEAT-456/nodejs-migration.md"
}

Deliver to: @SARAH (for quality gate, then → @Backend)
SLA: <10 min
```

---

### From SubAgent to SARAH

```markdown
@SARAH
DELEGATION COMPLETED: nodejs-migration-research

Delegation ID: DEL-2025-12-30-XXX
Output file: .ai/issues/FEAT-456/nodejs-migration.md

Status: ✅ COMPLETED

Summary:
- Node 18→20 is straightforward upgrade
- Breaking changes: Minimal (none in Express.js context)
- Performance: +15% throughput expected
- Testing effort: 2-4 hours (non-blocking changes)
- Migration path: Safe, no deprecations

Key findings: See output file
Time: 8 min | Confidence: HIGH
Ready for: @Backend review
```

---

### SubAgent to Main Agent (Direct Response)

**When SubAgent communicates back to requesting Main Agent:**

```markdown
@Backend
TASK COMPLETE: graphql-caching-research

Output: .ai/issues/ARCH-234/graphql-caching.md

Executive summary:
✅ Pattern-based TTL strategy recommended
  - Industry standard approach
  - 60% invalidation overhead reduction
  - Implementation: 2-4 hours
  - Risk level: LOW

Detailed analysis in output file with 3 alternative patterns.

Questions or need more detail? Let me know!
Time: 8 min
```

@Backend
TASK COMPLETE: graphql-caching-research

Output: .ai/issues/ARCH-234/graphql-caching.md

Executive summary:
✅ Pattern-based TTL strategy recommended
  - Industry standard approach
  - 60% invalidation overhead reduction
  - Implementation: 2-4 hours
  - Risk level: LOW

Detailed analysis in output file with 3 alternative patterns.

Questions or need more detail? Let me know!
Time: 8 min
```

---

## Communication Rules & Guidelines

### Rule 1: Mention Pattern
- **Direct SubAgent calls:** Use `@SubAgent-{Type}` when well-defined
- **Routed requests:** Use `@SARAH` when priority or routing needed
- **Responses:** Always mention requesting agent/coordinator
- **Format:** `@Agent` at start of message for clarity

### Rule 2: Context Provision
Every request MUST include:
- ✅ **Scope:** What exactly is needed
- ✅ **Constraints:** Time, size, complexity limits
- ✅ **Success criteria:** What constitutes "done"
- ✅ **Output location:** Where to write results
- ✅ **Priority level:** HIGH/NORMAL/LOW

Example of well-formed request:

```markdown
@SubAgent-Testing
TASK: Generate unit tests for PaymentProcessor

Scope:
- Happy path: process valid payment
- Error cases: declined card, timeout, invalid amount
- Edge cases: zero amount, negative amount, >999,999

Constraints:
- Max execution time: 10 min
- Keep test file <300 lines
- Use existing test patterns

Success criteria:
- >85% code coverage
- All edge cases covered
- Tests run in <5s

Output: src/services/__tests__/PaymentProcessor.test.ts
```

### Rule 3: Response Format
SubAgent must respond with:

```markdown
@Requesting-Agent (or @SARAH)
STATUS: ✅ COMPLETED | ⚠️ PARTIAL | ❌ FAILED

Output: [file path]

Summary: [2-3 sentence executive summary]

Key findings:
- Finding 1
- Finding 2
- Finding 3

Metrics:
- Time: X min
- Quality: [confidence level]
- Files affected: [count]
```

### Rule 4: Priority Handling

| Priority | SubAgent SLA | Routing | Queue |
|----------|------------|---------|-------|
| **CRITICAL** | <5 min | Direct to SARAH | Jump queue |
| **HIGH** | <10 min | Via SARAH | Prioritized |
| **NORMAL** | <15 min | Direct or SARAH | Standard |
| **LOW** | <30 min | Direct | Batched |

### Rule 5: Context Management

**Requesting Agent MUST:**
- [ ] Include only relevant context
- [ ] Specify file/folder scope
- [ ] Provide config details if needed
- [ ] Set realistic deadline

**SubAgent MUST:**
- [ ] Extract needed context from paths
- [ ] Not ask for context re-sends
- [ ] Consolidate findings clearly
- [ ] Remove sensitive data from outputs

### Rule 6: Error Communication

**If SubAgent encounters error:**

```markdown
@SARAH (or requesting agent)
DELEGATION FAILED: {task_name}

Error Type: [Timeout/InvalidInput/AccessDenied/Other]
Error Message: [Specific error details]

Root cause: [Why it failed]
Suggestions: 
1. [Fix option 1]
2. [Fix option 2]

Recommendation: [Retry vs Skip vs Escalate]
```

Example:

```markdown
@SARAH
DELEGATION FAILED: large-codebase-analysis

Error Type: Timeout
Error: Execution exceeded 15 minute limit

Root cause: Codebase 500K LOC, too large for single pass
Suggestions:
1. Split into 3 focused analyses (auth, api, db)
2. Use incremental approach (analyze one module)
3. Request simpler analysis (metrics-only)

Recommendation: Retry with split scope
```

### Rule 7: Handoff Protocol

When multiple agents need to collaborate:

```
Requesting Agent → SARAH → Primary SubAgent → Secondary SubAgent → SARAH → Requesting Agent

Example:
@Backend needs feature analysis
  ↓
@SARAH routes to @SubAgent-Architecture
  ↓
@SubAgent-Architecture identifies security implications
  ↓
@SARAH routes partial results to @SubAgent-Security
  ↓
@SubAgent-Security completes security analysis
  ↓
@SARAH consolidates both results
  ↓
Delivers complete analysis back to @Backend
```

### Rule 8: Documentation Requirements

Every SubAgent response must include:
- [ ] Clear output file location
- [ ] Timestamp of execution
- [ ] Execution time
- [ ] Confidence level (HIGH/MEDIUM/LOW)
- [ ] Next steps/recommendations
- [ ] Any caveats or limitations

### Rule 9: Escalation Triggers

SubAgent MUST escalate to SARAH immediately when:
- ⚠️ Execution time exceeds SLA by >25%
- ⚠️ Context unavailable/incomplete (can't proceed)
- ⚠️ Task scope expanded beyond delegation
- ⚠️ Quality issues detected (gaps/errors)
- ⚠️ Security/compliance concerns
- ⚠️ Need secondary SubAgent involvement

### Rule 10: Quality Assurance

Before responding, SubAgent checklist:

```
- [ ] Output file exists at specified location
- [ ] Content is complete & accurate
- [ ] No hallucinations or assumptions
- [ ] Matches success criteria from request
- [ ] Properly formatted (markdown/json/etc)
- [ ] Sensitive data scrubbed
- [ ] Time spent reasonable
- [ ] Summary is actionable
```

---

## Failure Handling

### If SubAgent Fails

```
1. Receive failure notification
2. Log in audit trail
3. Notify requesting main agent
4. Options:
   - Retry with optimized input
   - Route to different SubAgent
   - Main agent carries context
5. Root cause analysis (if pattern)
6. Update SubAgent definition if needed
```

**Template:**

```markdown
## Delegation Failure: {ID}

**Time:** 2025-12-30 14:30  
**SubAgent:** @SubAgent-Testing  
**Task:** Generate unit tests for UserService  
**Error:** Timeout after 10 minutes  

**Cause:** File size >250 KB exceeded SubAgent limit

**Resolution:**
1. Split test generation into 3 batches
2. Retry with smaller scope
3. Document file size limits in SubAgent spec

**Lessons:**
- Add file size checks before delegation
- Implement batch processing for large codebases
```

## Escalation Criteria

**When to escalate to @SARAH directly:**
```
- Task exceeds 15 min (normal: <10 min)
- Output doesn't meet quality criteria (>5% issues)
- SubAgent error rate (>2% failures)
- Context issue (main agent context still >15 KB after delegation)
- Security/compliance concern
```

**Escalation workflow:**
```
Agent: @SARAH "This delegation failed X times, needs investigation"
SARAH: Investigate root cause, implement fix
Action: Update SubAgent definition or create mitigation
```

## SubAgent Performance Dashboard

**Weekly metrics to track:**

```
┌─ SubAgent Performance ───────────────────────┐
│                                             │
│ @SubAgent-Research:                         │
│   Tasks: 8 | Time: 5m | Quality: 98%       │
│   Status: ✅ Operational | Issues: None    │
│                                             │
│ @SubAgent-Testing:                          │
│   Tasks: 12 | Time: 4m | Quality: 95%      │
│   Status: ✅ Operational | Issues: 1       │
│                                             │
│ @SubAgent-Security:                         │
│   Tasks: 3 | Time: 8m | Quality: 100%      │
│   Status: ✅ Operational | Issues: None    │
│                                             │
│ OVERALL HEALTH:                             │
│   Uptime: 100% | Efficiency: +38%          │
│                                             │
└─────────────────────────────────────────────┘
```

## Integration with Main Agents

### For @Backend

```
Workflow:
1. Identify research needed
2. Request: "@SARAH, research Node.js v20"
3. SARAH routes to @SubAgent-Research
4. Result arrives in <10 min
5. @Backend reads 2 KB summary
6. Continues with reduced context

Context reduction: 25 KB → 8 KB (68%)
```

### For @Frontend

```
Workflow:
1. Identify accessibility issues
2. Request: "@SARAH, audit component accessibility"
3. SARAH routes to @SubAgent-A11y (or @SubAgent-Review)
4. Result arrives with detailed findings
5. @Frontend implements fixes
6. Maintains lean context

Context reduction: 24 KB → 7 KB (71%)
```

### For @Architect

```
Workflow:
1. Need technology analysis
2. Request: "@SARAH, research microservices trade-offs"
3. SARAH routes to @SubAgent-Research
4. Result arrives with comparison matrix
5. @Architect makes decision
6. Delegates ADR creation to @SubAgent-Documentation

Context reduction: 22 KB → 7 KB (68%)
```

## SARAH's Weekly Checklist

### Monday

```
- [ ] Review weekend delegations
- [ ] Check any failed/pending tasks
- [ ] Generate performance report
- [ ] Review context sizes all agents
```

### Tuesday-Thursday

```
- [ ] Monitor daily delegation queue
- [ ] Quality check SubAgent outputs
- [ ] Escalate issues
- [ ] Optimize routing
```

### Friday

```
- [ ] Collect weekly metrics
- [ ] Analyze trends
- [ ] Plan optimization for next week
- [ ] Document lessons learned
```

### Monthly (End of Month)

```
- [ ] Full audit of all delegations
- [ ] SubAgent performance review
- [ ] Context optimization analysis
- [ ] Token savings calculation
- [ ] Update guidelines if needed
- [ ] Plan next month improvements
```

## Success Metrics (SARAH Tracks)

```
Target Performance:
- Main agent context: <10 KB (avg 8 KB) ✓
- SubAgent response: <10 min (avg 6 min) ✓
- Quality score: >95% ✓
- Uptime: 100% ✓
- Token savings: 35-40% ✓
- Error rate: <2% ✓
- Delegation adoption: >60% of tasks ✓
- Communication clarity: 0 context re-requests <2% ✓

Current Status:
All targets EXCEEDED in pilot week!
```

---

## Communication Best Practices

### For Main Agents (Requesting)

**✅ DO:**
- Use clear, structured requests
- Include all context in one message
- Specify output format and location
- Set realistic deadlines
- Mention priority level
- Ask specific questions
- Reference related issues/docs

**❌ DON'T:**
- Make vague requests ("analyze this")
- Send context incrementally
- Skip deadline/priority info
- Expect SubAgent to guess format
- Change scope mid-delegation
- Request contradictory goals
- Assume SubAgent background knowledge

**Example of GOOD request:**

```markdown
@SARAH
RESEARCH: TypeScript 5.3 migration for backend

Details:
- Current version: 5.0
- Scope: Node.js + Express APIs only
- Focus: Breaking changes, decorators, impact

Deadline: Tomorrow 17:00
Priority: HIGH

Success = list of changes needed + effort estimate
Output: .ai/issues/ARCH-456/ts5.3-migration.md
```

### For SubAgents (Responding)

**✅ DO:**
- Acknowledge receipt immediately
- Ask clarifying questions if needed
- Work incrementally & save progress
- Use structured format for findings
- Summarize key results upfront
- Provide actionable recommendations
- Include confidence levels

**❌ DON'T:**
- Assume missing context details
- Return unformatted raw data
- Skip the summary/TL;DR
- Provide contradictory findings
- Exceed time SLA without notification
- Return empty or incomplete files
- Forget to reference sources

**Example of GOOD response:**

```markdown
@SARAH
COMPLETED: TypeScript 5.3 migration analysis

Output: .ai/issues/ARCH-456/ts5.3-migration.md

KEY FINDINGS:
✅ Safe upgrade - no breaking changes for Express
✅ Decorator support improved (beneficial)
⚠️ Type narrowing changes (review 12 files)

Effort: 4-6 hours testing
Risk: LOW

File: See detailed analysis in output
Time: 9 min | Confidence: HIGH
```

### For SARAH (Coordinating)

**✅ DO:**
- Validate requests before routing
- Choose right SubAgent for task
- Set clear expectations
- Monitor SLAs actively
- Quality gate all outputs
- Escalate blockers immediately
- Document routing decisions

**❌ DON'T:**
- Route to overloaded SubAgent
- Skip quality checks
- Miss escalation triggers
- Leave delegations unmonitored
- Change routing mid-task
- Mix unrelated tasks
- Lose track of outputs

---

## Communication Workflow Summary

```
┌──────────────────────────────────────────────────────────┐
│ COMPLETE SubAgent Communication Workflow                │
└──────────────────────────────────────────────────────────┘

STEP 1: Requesting Agent Prepares Request
  - Define clear scope
  - Include all context
  - Set deadline & priority
  - Specify output location
  ↓

STEP 2: Submit to SARAH or Direct SubAgent
  - Via @SARAH for complex/priority tasks
  - Direct to @SubAgent-{Type} for simple tasks
  - Include delegation ID if available
  ↓

STEP 3: SARAH Routes (if applicable)
  - Validate request completeness
  - Select appropriate SubAgent
  - Create delegation record
  - Notify SubAgent with full context
  ↓

STEP 4: SubAgent Executes
  - Acknowledge receipt
  - Ask clarifying questions if needed
  - Execute task
  - Save results to specified location
  - Track execution metrics
  ↓

STEP 5: SubAgent Responds
  - Deliver summary message
  - Provide key findings
  - Reference output file
  - Include quality metrics
  ↓

STEP 6: SARAH Quality Gate (if routed)
  - Verify output completeness
  - Check format/quality
  - Validate against success criteria
  - Forward to requesting agent
  ↓

STEP 7: Requesting Agent Uses Results
  - Review output file
  - Implement recommendations
  - Provide feedback if needed
  - Close delegation

└──────────────────────────────────────────────────────────┘
```

---

**Next Steps for SARAH:**
1. Create SubAgent performance dashboard
2. Automate daily metric collection
3. Set up delegation request routing
4. Establish quality gate process
5. Plan monthly optimization cycles

**Related Documents:**

**📚 Communication & Coordination:**
- [AGENT-SUBAGENT-COMMUNICATION.md](AGENT-SUBAGENT-COMMUNICATION.md) — Detailed communication patterns with real-world examples
- [AGENT-SUBAGENT-CHEATSHEET.md](AGENT-SUBAGENT-CHEATSHEET.md) — Quick reference for all agents
- [SARAH.agent.md](../../.github/agents/SARAH.agent.md) — SARAH agent definition

**🔄 Workflows & Prompts:**
- [subagent-delegation.workflow.md](../../.ai/workflows/subagent-delegation.workflow.md)
- [subagent-delegation.prompt.md](../../.github/prompts/subagent-delegation.prompt.md)

**📋 Guidelines & Standards:**
- [SUBAGENT_DELEGATION.md](SUBAGENT_DELEGATION.md)
