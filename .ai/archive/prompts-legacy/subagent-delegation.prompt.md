---
docid: UNKNOWN-029
title: Subagent Delegation.Prompt
owner: @DocMaintainer
status: Archived
created: 2026-01-08
---

# SubAgent Delegation Prompt

## Purpose
Guide agents to identify, delegate, and manage tasks using SubAgents to keep context lean and efficient.

## When to Use
- Context growing beyond 10 KB
- Task can be broken into research + implementation phases
- Multiple specialists needed (security, testing, optimization)
- Main task focus is decision-making, not task execution

## Quick Decision Tree

```
Does task involve:
├─ Research/Information Gathering?
│  └─ YES → Delegate to @SubAgent-Research
│
├─ Code or Design Review?
│  └─ YES → Delegate to @SubAgent-Review or @SubAgent-Security
│
├─ Writing Tests?
│  └─ YES → Delegate to @SubAgent-Testing
│
├─ Creating Documentation?
│  └─ YES → Delegate to @SubAgent-Documentation
│
├─ Performance Analysis?
│  └─ YES → Delegate to @SubAgent-Optimization
│
├─ Version Upgrade/Migration?
│  └─ YES → Delegate to @SubAgent-Research
│
└─ Core Decision Making?
   └─ KEEP IN MAIN CONTEXT
```

## Delegation Process

### Step 1: Identify Delegatable Work

**Ask yourself:**
```
- Will I need this for the next 10 min? → KEEP
- Is this reference material? → DELEGATE
- Am I an expert in this? → KEEP
- Does a specialist make sense? → DELEGATE
- Is this a blocking task? → DELEGATE
- Is this the critical path? → KEEP
```

**Examples:**

```markdown
❌ KEEP (Core to current task):
- "Implementing user authentication API"
- "Deciding between database options"
- "Architecting service boundaries"

✅ DELEGATE (Supporting work):
- "Research authentication best practices"
- "Security review of API endpoints"
- "Generate unit tests"
- "Analyze performance impact"
- "Document API endpoints"
```

### Step 2: Prepare Delegation Input

**For each delegation, provide:**

```json
{
  "task_type": "research|review|test|document|optimize",
  "subject": "What to analyze/research",
  "scope": "What areas to focus on",
  "criteria": "Success criteria",
  "context": "Background info",
  "output_location": ".ai/issues/{id}/{name}.md",
  "priority": "high|medium|low",
  "deadline": "YYYY-MM-DD HH:mm"
}
```

**Example - Research Delegation:**

```json
{
  "task_type": "research",
  "subject": "Node.js v20 migration impact",
  "scope": "Breaking changes, compatibility, performance",
  "criteria": "Summary of migration path, effort estimate",
  "context": "Current: v18.x, Target: v20.x, Using Express.js + PostgreSQL",
  "output_location": ".ai/knowledgebase/INDEX.md",
  "priority": "high",
  "deadline": "2025-12-31 17:00"
}
```

**Example - Security Review Delegation:**

```json
{
  "task_type": "review",
  "subject": "User authentication API endpoints",
  "scope": "Input validation, SQL injection risk, password handling, error messages",
  "criteria": "All security checklist items reviewed, findings documented",
  "context": "Code in src/api/auth.ts, PostgreSQL backend, bcrypt for passwords",
  "output_location": ".ai/issues/FEAT-456/security-review.md",
  "priority": "high",
  "deadline": "2025-12-31 12:00"
}
```

**Example - Test Generation Delegation:**

```json
{
  "task_type": "test",
  "subject": "UserAuthService class",
  "scope": "Login, registration, password reset flows",
  "criteria": "Unit tests with >90% coverage, all success/failure paths",
  "context": "TypeScript, Jest framework, PostgreSQL, mock database",
  "output_location": "src/services/UserAuthService.test.ts",
  "priority": "medium",
  "deadline": "2025-12-31 18:00"
}
```

### Step 3: Delegate

**Format for AI Agent:**

```
@SubAgent-{Type}
Delegation:
[Copy JSON or natural language from Step 2]
```

**Example:**

```
@SubAgent-Research
Delegation:
Please research Node.js v20 migration impact for our Express.js + PostgreSQL stack.
- Focus: Breaking changes, compatibility, performance gains
- Current version: 18.x, Target: 20.x
- Output: .ai/knowledgebase/INDEX.md
- Deadline: 2025-12-31 17:00
```

### Step 4: Receive Output

**SubAgent returns:**

```json
{
  "status": "completed",
  "output_file": ".ai/knowledgebase/INDEX.md",
  "summary": "Node.js v20 upgrade is straightforward for Express.js. 
             Main breaking change: dropped Node 14 support. 
             Performance gains: 15% faster request handling.",
  "key_findings": [
    "Node 16+ required (we're on 18, safe)",
    "Express.js compatible without changes",
    "15% performance improvement in benchmarks",
    "Migration effort: 2-4 hours for testing"
  ],
  "time_spent": 8,
  "confidence": "high"
}
```

### Step 5: Use Output

**In your main context:**

```markdown
## Upgrade Path: Node.js v18 → v20

See full analysis: [nodejs-migration-research.md](../../.ai/knowledgebase/INDEX.md)

**Key findings:**
- Safe upgrade (already Node 18)
- 15% performance improvement
- ~2-4 hours testing effort

**Decision:** Proceed with upgrade in next sprint.
```

**Result:**
- Keep only 2 KB summary in active context
- Link to 5 KB detailed analysis file
- Specialist handled complexity
- Main agent stays focused

## Delegation by Agent Type

### @Backend Delegations

**Research Delegations:**
```
"Research PostgreSQL v15 → v16 upgrade path"
"Analyze authentication library options"
"Research distributed cache strategies"
"Investigate message queue solutions"
```

**Review Delegations:**
```
"Security review of payment API endpoints"
"Code quality review of database queries"
"Performance audit of database indexes"
```

**Test Delegations:**
```
"Generate unit tests for UserService"
"Create integration tests for payment flow"
"Generate API endpoint tests"
```

### @Frontend Delegations

**Research Delegations:**
```
"Research React 19 new features impact"
"Analyze state management libraries"
"Research bundle optimization strategies"
```

**Review Delegations:**
```
"Accessibility audit of form components"
"Performance review of component tree"
"Design review against component library"
```

**Test Delegations:**
```
"Generate unit tests for LoginComponent"
"Create E2E tests for user flow"
"Generate accessibility tests"
```

### @Architect Delegations

**Research Delegations:**
```
"Research microservices vs monolith trade-offs"
"Analyze database architecture options"
"Research container orchestration solutions"
```

**Analysis Delegations:**
```
"Analyze scalability impact of API redesign"
"Evaluate cost/performance trade-offs"
"Impact analysis of authentication architecture change"
```

**Documentation Delegations:**
```
"Create ADR for database choice decision"
"Document API versioning strategy"
"Create architecture diagram documentation"
```

## Context Management During Delegation

### Before Delegation

```markdown
# @Backend - Full Context (25 KB)

## API Design Patterns
[3 KB of API design patterns]

## Database Schema
[4 KB of database schema reference]

## Error Handling
[3 KB of error handling standards]

## Security Checklist
[3 KB of security items]

## Test Framework
[3 KB of test patterns]

## Current Task
[2 KB of feature being built]
```

### After Delegation

```markdown
# @Backend - Lean Context (8 KB)

## Role
Design and implement backend APIs, services, databases.

## Current Task: User Authentication API
- Implement /auth/login endpoint
- Success criteria: Secure, validated inputs, proper error handling
- Deadline: 2025-12-31

## SubAgent Delegation Status
- @SubAgent-Security: API endpoints review (IN PROGRESS)
- @SubAgent-Testing: Unit tests generation (PENDING)
- [See delegations below]

## Delegations

| What | SubAgent | Status | Output |
|------|----------|--------|--------|
| API security review | @SubAgent-Security | IN PROGRESS | `.ai/issues/FEAT-123/security-review.md` |
| Unit tests | @SubAgent-Testing | PENDING | `src/auth.test.ts` |
| Error handling | Reference | READY | [error-patterns.md](../../.ai/knowledgebase/INDEX.md) |
| Database | Reference | READY | [schema-adr.md](../../.ai/decisions/INDEX.md) |

## Links (not inline)
- [API Design Patterns](../../.ai/knowledgebase/INDEX.md)
- [Security Best Practices](../instructions/security.instructions.md)
- [Testing Standards](../../.ai/knowledgebase/INDEX.md)
```

**Context reduction: 25 KB → 8 KB (68% smaller)**

## Monitoring Delegations

### Track Delegation Status

**File:** `.ai/issues/{id}/delegations.md`

```markdown
# Delegations for FEAT-123: User Authentication

## Open Delegations
- [ ] @SubAgent-Security - API security review (due 2025-12-30)
- [ ] @SubAgent-Testing - Unit tests (due 2025-12-31)

## Completed Delegations
- [x] @SubAgent-Research - Auth best practices (completed 2025-12-28)
- [x] @SubAgent-Documentation - API docs (completed 2025-12-29)

## Next Actions
1. Receive security review findings
2. Address any security issues
3. Receive test coverage report
4. Merge all outputs
```

### Success Criteria per SubAgent Type

**@SubAgent-Research:**
- [x] Sources cited
- [x] Multiple perspectives covered
- [x] Clear findings extracted
- [x] Next steps identified

**@SubAgent-Security:**
- [x] All security checklist items reviewed
- [x] Findings prioritized (critical/medium/low)
- [x] Remediation suggestions provided
- [x] Confidence level documented

**@SubAgent-Testing:**
- [x] >80% code coverage
- [x] Happy path tested
- [x] Edge cases covered
- [x] Tests documented

**@SubAgent-Documentation:**
- [x] Complete and accurate
- [x] Examples included
- [x] Accessible format
- [x] Properly formatted

## Token Budget

### Without Delegation

```
Task: Implement authentication feature
- Load full context: 25 KB = 6,250 tokens
- Keep active whole time: 5-8 hours
- Research phase (30 min): Full context loaded
- Design phase (20 min): Full context loaded
- Implementation phase (40 min): Full context loaded
- Testing phase (30 min): Full context loaded
Total tokens: 6,250 (whole task)
```

### With Delegation

```
Task: Implement authentication feature
- Load lean context: 8 KB = 2,000 tokens
- Research phase (5 min): Delegate to SubAgent (1,500 tokens, parallel)
- Design phase (20 min): Main context active (2,000 tokens)
- Implementation phase (40 min): Main context active (2,000 tokens)
- Testing phase (5 min): Delegate to SubAgent (1,500 tokens, parallel)
Total tokens: 3,500 (50% savings)
```

## Fallback Strategy

**If SubAgent unavailable:**
```
1. Main agent carries temporary context
2. Document what would have been delegated
3. Flag for future optimization
4. After task, delegate retroactively if needed
```

**Example:**
```
@Backend: "@SubAgent-Testing unavailable, writing tests myself"
→ Later, after feature complete:
User: @SubAgent-Testing "Add test coverage for auth feature"
→ SubAgent reviews & completes testing
```

## Anti-Patterns to Avoid

❌ **Over-delegation**
```
Don't delegate every little task
Keep decision-making in main agent
Keep critical path in main context
```

❌ **Under-delegation**
```
Don't keep all reference docs inline
Don't carry test frameworks in context
Don't include all security checklists
```

❌ **Poor communication**
```
Don't have vague output expectations
Don't forget to specify success criteria
Don't fail to document output location
```

❌ **Inconsistent results**
```
Don't trust SubAgent without review
Don't accept output without verification
Don't skip quality gates
```

## Best Practices

✅ **Clear input specification**
- What to analyze
- Success criteria
- Output format
- Deadline

✅ **Output file management**
- Consistent naming
- Organized in `.ai/issues/{id}/`
- Linked in main context
- Archived after task

✅ **Delegation chaining**
- Some SubAgents call other SubAgents
- Avoid infinite loops (max 2 levels)
- Document dependency chains

✅ **Quality assurance**
- Review SubAgent output
- Verify findings
- Test recommendations
- Update if needed

## Template: Delegation Record

```markdown
# Delegation: {Task Name}

**Date:** 2025-12-30  
**Delegated to:** @SubAgent-{Type}  
**Status:** Completed ✅ / In Progress 🔄 / Failed ❌  

## Input
[What was delegated]

## Output
[Result file and summary]

## Quality
- Completeness: ✅
- Accuracy: ✅
- Actionability: ✅
- On-time: ✅

## Key Findings
1. [Finding 1]
2. [Finding 2]
3. [Finding 3]

## Next Steps
1. [Action 1]
2. [Action 2]

---
Reviewed by: @Agent  
Date: 2025-12-31
```

---

**Related:**
- [SUBAGENT_DELEGATION.md](../../.ai/guidelines/SUBAGENT_DELEGATION.md) - Full strategy
- [subagent-delegation.workflow.md](../../.ai/workflows/subagent-delegation.workflow.md) - Implementation
- [CONTEXT_OPTIMIZATION.md](../../.ai/guidelines/CONTEXT_OPTIMIZATION.md) - Main agent optimization
