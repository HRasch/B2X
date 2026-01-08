---
docid: GL-097
title: SUBAGENT_DELEGATION_PATTERNS
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

# Agent-to-Agent Delegation Patterns & Framework

**Purpose**: Enable efficient, auditable delegation between SubAgents  
**Scope**: All 33+ SubAgents across Phases 1-4  
**Framework**: Explicit, asynchronous, task-scoped  
**Governance**: @SARAH coordinates complex delegations

---

## Core Principles

### 1. Explicit Intent
Every delegation must state clearly:
- **What**: Specific task/decision needed
- **Why**: How it serves the overall goal
- **Success**: What constitutes successful completion
- **Context**: Relevant background info

```markdown
Delegation Example:
TO: @SubAgent-SearchPerformance
TASK: "Optimize product search index for 10k+ products"
WHY: Store pages load 500ms; target <200ms
SUCCESS: Index <50MB, queries <100ms P99
CONTEXT: Current Elasticsearch config in .github/agents/SubAgent-SearchPerformance.agent.md
```

### 2. Clear Ownership
One agent owns final output; others provide input

```
Primary Owner: @SubAgent-SearchDDD
  └─ Designs search domain model
     ├─ Input from: @SubAgent-SearchPerformance (latency constraints)
     ├─ Input from: @SubAgent-APISecurity (security constraints)
     └─ Input from: @SubAgent-Instrumentation (metrics needs)
  └─ Deliverable: Search domain design (.ai/issues/{id}/search-domain.md)
```

### 3. Scope Boundaries
Each agent works within their domain; escalate for conflicts

```
Conflict Example:
@SubAgent-SearchPerformance: "Use aggressive caching (1h TTL)"
@SubAgent-CMSSecurity: "Content changes must appear immediately"

Resolution:
→ Escalate to @Architect for trade-off decision
→ Decision: Cache CMS for 5 minutes (compromise)
→ Document decision in ADR
```

---

## Delegation Patterns

### Pattern 1: Sequential (Waterfall)

**Use when**: Output of one feeds into next, single-path workflow

**Flow**:
```
Agent 1           Agent 2           Agent 3
  ↓                 ↓                 ↓
Design    →    Optimize    →    Secure    →    Complete
  ↓                 ↓                 ↓
Output1          Output2          Output3
```

**Example: Implementing new Product attribute**
```
Step 1: @SubAgent-CatalogDDD
  Task: "Design product attribute domain model"
  Output: Product aggregate with attributes, events, constraints
  Time: 30 min

Step 2: @SubAgent-CatalogPerformance
  Task: "Optimize attribute querying for 10k+ products"
  Input: Domain model from Step 1
  Output: Database schema, indexes, query patterns
  Time: 20 min

Step 3: @SubAgent-CatalogSecurity
  Task: "Add access control for sensitive attributes"
  Input: Domain model + schema from Steps 1-2
  Output: Security rules, access control implementation
  Time: 15 min

Step 4: Merge into Feature Implementation
  Final: Complete attribute system ready for coding
  Time: 5 min (merge)
```

**When to use**:
- ✅ Clear dependencies (must complete A before B)
- ✅ Single path to solution
- ✅ Sequential expertise needed
- ❌ Don't use if: can work in parallel

---

### Pattern 2: Parallel (Concurrent)

**Use when**: Multiple agents can work independently on same task

**Flow**:
```
        ┌─ Agent 1 ─┐
        │           │
Task ──┤─ Agent 2   ├─→ Merge ─→ Complete
        │           │
        └─ Agent 3 ─┘
```

**Example: Complete Feature Implementation**
```
Feature: "Product Variant System"

Parallel Tasks:
├─ @SubAgent-CatalogDDD
   "Design variant domain model"
   → Product Aggregate with variants, SKUs, attributes
   
├─ @SubAgent-CatalogPerformance
   "Design variant query performance"
   → Indexes, cache strategy, query patterns
   
├─ @SubAgent-CatalogSecurity
   "Design variant access control"
   → Who can see/edit variants, pricing rules
   
└─ @SubAgent-Instrumentation
    "Design variant metrics"
    → What to track, dashboards, alerts

Timeline: All 4 work simultaneously (4 hours total vs. 8 hours sequential)

Merge Point (1 hour):
- Review all 4 outputs
- Resolve conflicts (if any)
- Create unified Feature Implementation Plan
```

**When to use**:
- ✅ Independent concerns (can be solved separately)
- ✅ Time-critical (parallelization speeds delivery)
- ✅ Multiple perspectives needed
- ❌ Don't use if: strong dependencies between agents

---

### Pattern 3: Validation (Review Chain)

**Use when**: Need quality validation from multiple experts

**Flow**:
```
Agent 1: Create
  ↓
Agent 2: Validate Domain
  ↓
Agent 3: Validate Performance
  ↓
Agent 4: Validate Security
  ↓
Approved / Rejected
```

**Example: Security Review of Payment Feature**
```
Step 1: @Backend implements payment module
  Code: Stripe integration, payment processing

Step 2: @SubAgent-IdentitySecurity validates
  Question: "Is user authentication verified before payment?"
  Check: User tokens valid, session not expired
  Result: ✅ Pass

Step 3: @SubAgent-APISecurity validates
  Question: "Is API endpoint rate-limited?"
  Check: 10 req/min per user, no bulk payment attacks
  Result: ✅ Pass

Step 4: @SubAgent-Encryption validates
  Question: "Are payment details encrypted?"
  Check: PCI compliance, TLS 1.3, key rotation
  Result: ⚠️ Flag - Missing key rotation

Step 5: Remediation
  Fix: Implement key rotation (weekly)
  Re-validate: @SubAgent-Encryption confirms
  Result: ✅ Pass

Step 6: Approval
  All validators approve → Feature ready for production
```

**When to use**:
- ✅ Quality gates needed (security, compliance, performance)
- ✅ Multiple validators required
- ✅ Mistakes are costly
- ❌ Don't use if: only one expert opinion needed

---

### Pattern 4: Escalation (Decision Making)

**Use when**: SubAgents have conflicting recommendations

**Flow**:
```
Agent 1: Proposes A
  ↓
Agent 2: Proposes B (conflicts with A)
  ↓
Escalate to Main Agent
  ↓
Main Agent: Makes decision
  ↓
Resolution: Proceed with decided approach
```

**Example: Cache TTL Trade-off**
```
Task: "Set cache TTL for product prices"

@SubAgent-CatalogPerformance proposes:
  "Cache for 1 hour to minimize database queries"
  Benefit: 95% query cache hit rate
  Risk: Prices can be 60 minutes stale

@SubAgent-CatalogSecurity proposes:
  "Cache for 5 minutes to prevent stale prices"
  Benefit: Price accuracy (5 min max latency)
  Risk: 30% increase in database queries

Conflict: Performance vs. Accuracy

Escalation to @Architect:
  Decision: 15-minute cache
    - Good performance (70% cache hit)
    - Acceptable price freshness
    - Reasonable DB load

Resolution:
  - Implement 15-min cache
  - Document decision in ADR (Architecture Decision Record)
  - Both agents confirm acceptable
```

**When to use**:
- ✅ SubAgents disagree
- ✅ Trade-offs between competing concerns
- ✅ High-impact decisions
- ❌ Don't use if: clear single best approach exists

---

### Pattern 5: Cascading (Progressive Refinement)

**Use when**: Task requires multiple rounds of refinement

**Flow**:
```
Agent 1: Initial design
  ↓
Agent 2: Add constraints
  ↓
Agent 3: Optimize
  ↓
Agent 4: Validate
  ↓
Agent 1: Final version
```

**Example: Building Search Feature**
```
Round 1: @SubAgent-SearchDDD
  Design: Search domain model, events, repositories
  Output: Initial search design

Round 2: @SubAgent-SearchPerformance
  Refinement: Apply performance constraints
  - Elasticsearch index structure
  - Query optimization
  - Facet aggregation strategy
  Output: Performance-optimized design

Round 3: @SubAgent-APISecurity
  Refinement: Add security constraints
  - Rate limiting per user
  - Query complexity limits
  - Access control
  Output: Security-hardened design

Round 4: @SubAgent-Instrumentation
  Validation: Add observability
  - Search latency metrics
  - Query volume tracking
  - Error rate monitoring
  Output: Observable design

Round 5: @SubAgent-SearchDDD (final review)
  Finalization: Incorporate all feedback
  Output: Complete, production-ready search design
```

**When to use**:
- ✅ Complex features requiring multiple expertise areas
- ✅ Feedback loops necessary
- ✅ Iterative refinement valuable
- ❌ Don't use if: simple enough for single agent

---

## Delegation Execution Framework

### Step 1: Request Formation

```markdown
[DELEGATION REQUEST]

TO: @SubAgent-CatalogPerformance
FROM: @Backend (main agent)
TASK_ID: issue-42-variant-performance

---

## Task
Optimize product variant queries for E-commerce platform.

## Requirements
- Handle 50k+ products with 5-10 variants each
- Query <100ms for listing page (10 variants visible)
- Support filtering by variant attributes (size, color)
- Work with existing Catalog schema

## Constraints
- Must use existing Elasticsearch setup
- PostgreSQL queries must be optimized (index on sku, category)
- Cache TTL max 1 hour (from @CatalogSecurity)

## Success Criteria
- ✅ Query latency <100ms P99
- ✅ Database CPU <70%
- ✅ No N+1 queries
- ✅ Variant filtering working

## Context
- Current schema: `.github/agents/SubAgent-CatalogDDD.agent.md` (section: Product Aggregate)
- Current performance issues: Variant queries hitting DB 5 times per product
- Related decision: ADR-012-variant-architecture.md

## Deliverable Format
`.ai/issues/issue-42/catalog-variant-performance.md`
```

### Step 2: Agent Receives & Confirms

Agent acknowledges receipt:
```markdown
[DELEGATION ACK]

FROM: @SubAgent-CatalogPerformance
TO: @Backend
TASK_ID: issue-42-variant-performance

Status: ✅ ACCEPTED

Understanding:
- Optimize 50k+ products x 5-10 variants
- Target: <100ms query, <70% CPU
- Constraints: ES + Postgres, 1h cache max
- Output: Design doc with schema/indexes/queries

Estimated Time: 45 minutes
Risk: None identified
```

### Step 3: Agent Executes

Agent works on task, may ask for clarification:
```markdown
[CLARIFICATION REQUEST]

FROM: @SubAgent-CatalogPerformance
TO: @Backend
TASK_ID: issue-42-variant-performance

Question: "Should variant attributes be searchable?"
Current design assumes: Attributes (size, color) filterable via facets
But if full-text search needed on attribute values, changes query design.

Waiting for: Answer to proceed
```

### Step 4: Agent Delivers

Agent completes and submits output:
```markdown
[DELEGATION COMPLETE]

FROM: @SubAgent-CatalogPerformance
TO: @Backend
TASK_ID: issue-42-variant-performance

Status: ✅ COMPLETED

Deliverable: `.ai/issues/issue-42/catalog-variant-performance.md`

Summary:
- Schema changes: Add (product_id, sku) composite index
- Query patterns: Use prepared statements for variant listing
- Cache strategy: Redis cache variant facets (1h TTL)
- Performance: Achieves <80ms P99, <50% DB CPU

Time spent: 40 minutes
Quality: All success criteria met ✅

Next steps: Ready for @Backend to code implementation
```

### Step 5: Primary Agent Reviews & Integrates

Main agent verifies output and integrates:
```markdown
[INTEGRATION COMPLETE]

FROM: @Backend
TASK_ID: issue-42-variant-performance

Status: ✅ INTEGRATED

Review of @SubAgent-CatalogPerformance output:
✅ All recommendations align with requirements
✅ No conflicts with other constraints
✅ Performance projections validated
✅ Implementation plan clear

Next: Implementation work begins
Timeline: 2 days coding + 1 day testing

Thank you @SubAgent-CatalogPerformance!
```

---

## Conflict Resolution Framework

### Scenario 1: Performance vs. Freshness

```
Conflict:
@SubAgent-CatalogPerformance: "Cache product list 1 hour"
@SubAgent-CMSSecurity: "Update within 5 minutes when price changes"

Resolution Steps:
1. Identify root cause: Performance vs. Data accuracy trade-off
2. Find middle ground: 15-minute cache
3. Implement hybrid: Longer cache + event-driven invalidation
4. Document: ADR-015-product-cache-strategy.md
5. Validate: Both agents confirm acceptable

Result: 15-min cache, instant invalidation on price change events
```

### Scenario 2: Security vs. Usability

```
Conflict:
@SubAgent-IdentitySecurity: "Require MFA for all admin operations"
@SubAgent-AdminWorkflowDDD: "MFA per-action too annoying for workflow"

Resolution Steps:
1. Identify: Security risk vs. User friction
2. Compromise: Session-based MFA (once per login, not per action)
3. Escalate: @Security approves risk level
4. Implement: MFA at login, not per operation
5. Monitor: Track unauthorized access attempts

Result: Balanced security + usability
```

### Scenario 3: Scalability vs. Consistency

```
Conflict:
@SubAgent-Scalability: "Use eventual consistency for faster writes"
@SubAgent-CatalogDDD: "Product inventory must be immediately consistent"

Resolution Steps:
1. Identify: Consistency model trade-off
2. Distinguish: Some data can be eventually consistent (search index)
                Some must be immediate (inventory count)
3. Implement: Hybrid approach
   - Inventory: Immediate (transactional)
   - Search index: Eventual (event-driven, 1-5s lag)
4. Document: Which data uses which model
5. Validate: Both agents confirm

Result: Different consistency models for different data
```

---

## Monitoring Delegations

### Metrics per Delegation

Track for continuous improvement:

```
Delegation: @SubAgent-CatalogPerformance
TASK_ID: issue-42-variant-performance

Metrics:
├─ Time to Complete: 40 min (est. 45 min) ✅ On time
├─ Quality: All criteria met ✅
├─ Clarity: No clarification questions needed ✅
├─ Usefulness: Implementer saved 2 hours vs. self-research ✅
├─ Adoption: Code follows recommendations 100% ✅
└─ Issue: None

Feedback:
"Excellent, detailed performance analysis with concrete
 schema changes. Saved us multiple iteration cycles."

Conclusion: High quality delegation
```

### Patterns to Track

```
Over time, monitor:

✅ Delegation success rate (% meeting all criteria)
✅ Time estimates accuracy
✅ Clarification needs (should decrease)
✅ Integration friction (rework needed)
✅ Team satisfaction with outputs
✅ Which agent combinations work best
❌ Failed delegations (require rework)
❌ Conflicts requiring escalation
❌ Misunderstandings
```

---

## Best Practices

### DO ✅

- ✅ **Be explicit**: State what, why, success criteria
- ✅ **Provide context**: Link to relevant docs/schemas
- ✅ **Set time bounds**: Expected completion time
- ✅ **Specify format**: Exactly what deliverable looks like
- ✅ **Acknowledge completion**: Thank agent, confirm value
- ✅ **Document decisions**: Capture why delegation happened
- ✅ **Track metrics**: Learn what works best

### DON'T ❌

- ❌ **Be vague**: "Optimize this" without specifics
- ❌ **Surprise dependencies**: Mention constraints upfront
- ❌ **Neglect context**: Assume agent knows your code
- ❌ **Ignore output**: Read the deliverable, provide feedback
- ❌ **Create long chains**: >3-4 sequential steps breaks down
- ❌ **Delegate unclear tasks**: Clarify with main agent first
- ❌ **Forget documentation**: Log decisions & rationale

---

## Governance

### Authority Levels

```
Delegation Level 1: Team Lead → SubAgent
Authority: Full (no approval needed)
Example: @Backend → @SubAgent-CatalogDDD

Delegation Level 2: SubAgent → SubAgent (same domain)
Authority: Full (peer experts)
Example: @SubAgent-CatalogDDD → @SubAgent-CatalogPerformance

Delegation Level 3: SubAgent → SubAgent (different domain)
Authority: Full (if clear & scoped)
Example: @SubAgent-SearchDDD → @SubAgent-APISecurity

Delegation Level 4: Complex cross-domain
Authority: Requires @SARAH coordination
Example: Trade-off between performance & security
```

### Escalation Criteria

Escalate to @SARAH when:
1. **Conflicting recommendations** from multiple agents
2. **Major trade-offs** (security vs. performance, consistency vs. scale)
3. **Architectural decisions** affecting multiple contexts
4. **Precedent needed** (first time doing something)
5. **Team consensus** required before proceeding

---

## Templates

### Delegation Request Template

```markdown
[DELEGATION REQUEST]

TO: @SubAgent-{Name}
FROM: @{MainAgent}
TASK_ID: {issue-number}

## What
{Specific task description}

## Why
{How this serves overall goal}

## Requirements
- Requirement 1
- Requirement 2
- Requirement 3

## Constraints
- Constraint 1
- Constraint 2

## Success Criteria
- ✅ Criterion 1
- ✅ Criterion 2
- ✅ Criterion 3

## Context
- Link to relevant docs
- Link to current implementation
- Link to related decisions

## Deliverable
`.ai/issues/{issue-id}/{filename}.md`

## Timeline
Estimated completion: {time estimate}
```

### Delegation Completion Template

```markdown
[DELEGATION COMPLETE]

FROM: @SubAgent-{Name}
TO: @{MainAgent}
TASK_ID: {issue-number}

Status: ✅ COMPLETED

## Summary
{2-3 sentence summary of what was delivered}

## Key Findings
- Finding 1
- Finding 2
- Finding 3

## Recommendations
- Recommendation 1
- Recommendation 2

## Deliverable Location
`.ai/issues/{issue-id}/{filename}.md`

## Quality Checklist
✅ All success criteria met
✅ No outstanding questions
✅ Ready for implementation

Time spent: X minutes (vs. Y estimated)
```

---

## Phase 4+ Evolution

### Phase 4: Foundation
- Basic sequential & parallel patterns
- Manual delegation coordination
- Async feedback loops

### Phase 5: Optimization
- Conflict resolution automation
- Smart agent routing (which agent best for this task)
- Learned preferences (agents work better together)

### Phase 5+: Autonomy
- Self-delegating agents (request help automatically)
- Predictive routing (anticipate needs)
- Continuous optimization (learn from outcomes)

---

**Status**: READY FOR PHASE 4 IMPLEMENTATION  
**Next Review**: End of Phase 3 (Early Feb 2026)  
**Owner**: @SARAH (governance)  
**Prepared by**: AI Agent Architecture Team
