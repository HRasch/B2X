# SubAgent Delegation Implementation Workflow

**Version:** 1.0  
**Created:** 30.12.2025  
**Coordinated by:** @SARAH, @Architect

## Quick Start

```
User: @Agent "Delegate research to SubAgent for efficiency"

Step 1: Agent identifies delegatable task
Step 2: Calls @SubAgent-Research with inputs
Step 3: Receives structured output file
Step 4: Continues main task with reduced context

Result: 50-70% context reduction, 40% token savings
```

## When to Delegate

### Perfect for Delegation ✅

```
✓ Research tasks (fetch docs, synthesis)
✓ Code reviews (security, quality analysis)
✓ Test generation (unit tests, E2E)
✓ Documentation (API docs, guides)
✓ Performance analysis
✓ Version migration analysis
✓ Integration planning
```

### Keep in Main Context ❌

```
✗ Active decision-making
✗ Core business logic
✗ Current task focus
✗ Real-time feedback loops
✗ Critical constraints
```

## Implementation by Agent Type

### Pattern 1: @Backend with SubAgents

#### Before Delegation

**@Backend Context: 28 KB**
```
- API design patterns (3 KB)
- Database schema reference (4 KB)
- Error handling guidelines (3 KB)
- Security checklist (3 KB)
- Test requirements (3 KB)
- Performance optimization (2 KB)
- Documentation template (2 KB)
- Integration points (3 KB)
- Current task (2 KB)
```

#### After Delegation

**@Backend Context: 8 KB (Reduced 71%)**
```
# Backend Agent - Optimized

## Role
API design, services, database implementation.

## Current Task
[Feature being implemented]

## Decision Framework
- Input validation always
- Use parameterized queries
- Proper error handling
- Log security events

## SubAgent Delegation Map

| Need | SubAgent | Output |
|------|----------|--------|
| API patterns | @SubAgent-APIDesign | `.ai/issues/{id}/api-design.md` |
| DB schema | @SubAgent-DBDesign | `.ai/issues/{id}/schema-design.md` |
| Security review | @SubAgent-Security | `.ai/issues/{id}/security-review.md` |
| Test framework | @SubAgent-Testing | `.tests/`, `.ai/issues/{id}/test-report.md` |
| Performance | @SubAgent-Optimization | `.ai/issues/{id}/perf-analysis.md` |
| Integration | @SubAgent-Integration | `.ai/issues/{id}/integration-plan.md` |

## When to Delegate
- "Need API design review" → @SubAgent-APIDesign
- "Analyze security implications" → @SubAgent-Security
- "Generate unit tests" → @SubAgent-Testing
- "Research PostgreSQL v16 upgrade" → @SubAgent-Research
```

#### Workflow: Implement User Registration API

```
Step 1: @Backend identifies task
"I need to implement user registration endpoint"

Step 2: @Backend delegates research
User: @SubAgent-Research
- Topic: User authentication best practices
- Context: Node.js, Express, PostgreSQL
- Output: `.ai/issues/FEAT-456/auth-research.md`

Step 3: @Backend delegates security review
User: @SubAgent-Security
- Code: src/api/register.ts
- Focus: Input validation, password security, rate limiting
- Output: `.ai/issues/FEAT-456/security-review.md`

Step 4: @Backend delegates test generation
User: @SubAgent-Testing
- Code: src/api/register.ts
- Spec: Success/failure cases, edge cases
- Output: `src/api/register.test.ts`, `.ai/issues/FEAT-456/test-results.md`

Step 5: @Backend implements
- Reads research summary (2 KB)
- Implements per security guidelines (0.5 KB)
- Integrates tests (reference only)
- Main context used: <3 KB

Total context: 8 KB (was 28 KB)
```

### Pattern 2: @Frontend with SubAgents

#### Before Delegation

**@Frontend Context: 24 KB**
```
- React hooks guide (3 KB)
- Component patterns (3 KB)
- State management (3 KB)
- Accessibility standards (3 KB)
- Performance optimization (2 KB)
- Testing patterns (2 KB)
- CSS/styling guide (2 KB)
- Responsive design (2 KB)
- Current task (2 KB)
```

#### After Delegation

**@Frontend Context: 7 KB (Reduced 71%)**
```
# Frontend Agent - Optimized

## Role
UI components, state management, user experience.

## Current Task
[Component being built]

## Component Standards
- Use React hooks
- Accessible by default (WCAG 2.1)
- Mobile-first responsive
- Performance: <100ms render

## SubAgent Delegation Map

| Need | SubAgent | Output |
|------|----------|--------|
| Component patterns | @SubAgent-Components | `.ai/issues/{id}/design.md` |
| Accessibility | @SubAgent-A11y | `.ai/issues/{id}/a11y-audit.md` |
| Performance | @SubAgent-Performance | `.ai/issues/{id}/perf-report.md` |
| Testing | @SubAgent-Testing | `.tests/`, `.ai/issues/{id}/test-results.md` |
| Design review | @SubAgent-Design | `.ai/issues/{id}/design-review.md` |

## Delegation Examples
- "Review component for accessibility" → @SubAgent-A11y
- "Optimize bundle size" → @SubAgent-Performance
- "Generate component tests" → @SubAgent-Testing
```

#### Workflow: Build User Profile Form

```
Step 1: @Frontend delegates design/patterns
User: @SubAgent-Components
- Component: UserProfileForm
- Fields: name, email, avatar, bio
- Output: `.ai/issues/FEAT-789/component-design.md`

Step 2: @Frontend delegates accessibility review
User: @SubAgent-A11y
- Code: src/components/UserProfileForm.tsx
- Check: Form labels, error messages, keyboard navigation
- Output: `.ai/issues/FEAT-789/a11y-audit.md`

Step 3: @Frontend delegates performance check
User: @SubAgent-Performance
- Code: src/components/UserProfileForm.tsx
- Targets: Bundle size, re-render optimization
- Output: `.ai/issues/FEAT-789/perf-report.md`

Step 4: @Frontend delegates test generation
User: @SubAgent-Testing
- Component: UserProfileForm
- Cases: Form submission, validation, error states
- Output: `src/components/UserProfileForm.test.tsx`

Step 5: @Frontend implements
- Main context: 7 KB (decision framework only)
- Reads summaries from outputs
- Implements component
- All checks automated
```

### Pattern 3: @Architect with SubAgents

#### Before Delegation

**@Architect Context: 22 KB**
```
- System design patterns (4 KB)
- Architectural styles (3 KB)
- Technology comparisons (4 KB)
- Scalability considerations (3 KB)
- Security architecture (2 KB)
- Integration patterns (2 KB)
- ADR process (2 KB)
- Current decision (2 KB)
```

#### After Delegation

**@Architect Context: 7 KB (Reduced 68%)**
```
# Architect Agent - Optimized

## Role
System design, architectural decisions, scalability planning.

## Current Decision
[Architecture being evaluated]

## Decision Framework
- Cost/Benefit analysis required
- Security implications documented
- Scalability verified at 10x load
- Team capacity assessed

## SubAgent Delegation Map

| Need | SubAgent | Output |
|------|----------|--------|
| Technology research | @SubAgent-Research | `.ai/issues/{id}/tech-analysis.md` |
| Impact analysis | @SubAgent-Analysis | `.ai/issues/{id}/impact-analysis.md` |
| Trade-off evaluation | @SubAgent-Comparison | `.ai/issues/{id}/trade-offs.md` |
| ADR documentation | @SubAgent-Documentation | `.ai/decisions/ADR-XXX.md` |
| Architecture review | @SubAgent-Review | `.ai/issues/{id}/arch-review.md` |

## Delegation Examples
- "Research microservices vs monolith" → @SubAgent-Research
- "Analyze scalability impact" → @SubAgent-Analysis
- "Compare database options" → @SubAgent-Comparison
```

#### Workflow: Choose Database Architecture

```
Step 1: @Architect delegates technology research
User: @SubAgent-Research
- Decision: PostgreSQL vs MongoDB for user data
- Context: 50M users, real-time queries, relational data
- Output: `.ai/issues/ADR-123/db-analysis.md`

Step 2: @Architect delegates impact analysis
User: @SubAgent-Analysis
- Question: Impact on API, scalability, operations
- Scope: Schema design, query patterns, backups
- Output: `.ai/issues/ADR-123/impact-analysis.md`

Step 3: @Architect delegates trade-off evaluation
User: @SubAgent-Comparison
- Options: PostgreSQL, MongoDB, DynamoDB
- Criteria: Cost, scalability, team expertise, performance
- Output: `.ai/issues/ADR-123/trade-offs.md`

Step 4: @Architect delegates ADR documentation
User: @SubAgent-Documentation
- Decision: PostgreSQL for normalized data
- Rationale: From trade-off analysis
- Output: `.ai/decisions/ADR-042-database-choice.md`

Step 5: @Architect decides
- Reads summaries: 5 KB total
- Makes final decision: PostgreSQL
- Documents rationale: Auto-generated by SubAgent
- Team can review in ADR

Total context: 7 KB (was 22 KB)
```

## SubAgent Library Implementation

### Core SubAgents (Ready to Create)

```
.github/subagents/
├── Research.subagent.md      (Fetch & synthesize info)
├── Review.subagent.md         (Code/design analysis)
├── Testing.subagent.md        (Test generation)
├── Documentation.subagent.md  (Doc creation)
├── Security.subagent.md       (Security audit)
├── Optimization.subagent.md   (Performance analysis)
├── Integration.subagent.md    (Cross-service coordination)
└── Analysis.subagent.md       (Impact/trade-off analysis)
```

### SubAgent Definition Template

```markdown
# @SubAgent-{Name}

## Role
Clear, single responsibility

## Context Size
<2 KB (absolute maximum)

## Input Format
```json
{
  "task": "what to do",
  "subject": "what to analyze",
  "context": "background",
  "output_path": "where to write"
}
```

## Output Format
```json
{
  "status": "completed|failed",
  "output_file": "path",
  "summary": "2-3 lines",
  "key_findings": [],
  "time_spent": "minutes"
}
```

## Success Criteria
- High quality
- Fast execution
- Clear output
- Actionable findings

## When to Call
- Specific task types
- Overloaded main context
- Specialized expertise needed
```

## SARAH's Coordination Role

### Delegation Routing

```
@SARAH monitors:
1. Task queue from main agents
2. SubAgent availability
3. Output file generation
4. Quality of delegations

Responsibilities:
- Route delegations efficiently
- Track SubAgent performance
- Escalate failures
- Maintain delegation audit log
```

### Weekly Delegation Report

**File:** `.ai/logs/delegation-report-{week}.md`

```markdown
# Delegation Report: Week of 30.12.2025

## Delegations Completed
- @Backend → @SubAgent-Security: 5 tasks
- @Frontend → @SubAgent-Testing: 8 tasks
- @Architect → @SubAgent-Research: 3 tasks

## Token Savings
- Total tokens saved: 12,500
- Avg per task: 2,100
- Efficiency improvement: 42%

## SubAgent Performance
| SubAgent | Tasks | Avg Time | Quality |
|----------|-------|----------|---------|
| Research | 8 | 4 min | 95% |
| Testing | 12 | 3 min | 98% |
| Security | 5 | 5 min | 100% |

## Issues
- None critical
- 1 timeout (Optimization on large codebase)

## Next Week
- Optimize Research timeout handling
- Add @SubAgent-APIDesign
```

## Token Impact Measurement

### Before Delegation

```
@Backend working on authentication feature:
- Context loaded: 28 KB
- Token cost: 7,000 tokens
- Context refresh rate: Every 30 min
- Inactive tokens: 40% (research docs, old patterns)

Task execution: Research (30min) + Design (20min) + Implement (40min)
Inefficiency: Carrying docs whole time
```

### After Delegation

```
@Backend working on same feature:
- Context loaded: 8 KB
- Token cost: 2,000 tokens
- SubAgent called as needed: 1,500 tokens (research)
- Active token ratio: 90% (focused only)

Task execution:
1. Delegate research to SubAgent (3 min)
2. Continue design with reduced context (20 min)
3. Read research summary (2 min)
4. Implement with SubAgent security review (40 min)
Total context tokens: 3,500 (50% savings)
```

## Practical Checklist

### For Main Agent

```
Before starting task:
- [ ] Identify if research needed → Delegate to @SubAgent-Research
- [ ] Identify if review needed → Delegate to @SubAgent-Review
- [ ] Identify if tests needed → Delegate to @SubAgent-Testing
- [ ] Identify if docs needed → Delegate to @SubAgent-Documentation

During task:
- [ ] Keep context <10 KB (link all reference docs)
- [ ] Use SubAgent outputs instead of inline docs
- [ ] Archive completed delegations in .ai/logs/

After task:
- [ ] Document learnings in .ai/issues/{id}/
- [ ] Update SubAgent guidelines if new patterns
- [ ] Log context usage & savings
```

### For SARAH

```
Daily:
- [ ] Monitor delegation queue
- [ ] Check SubAgent outputs
- [ ] Escalate any failures

Weekly:
- [ ] Generate delegation report
- [ ] Measure token savings
- [ ] Review SubAgent performance

Monthly:
- [ ] Audit context sizes
- [ ] Optimize delegation patterns
- [ ] Update SubAgent definitions
```

## Success Metrics (Targets)

```
Main Agent Context:
- Size: <10 KB (was 20-30 KB)
- Setup time: <5 min
- Focus: Current task only

SubAgent Usage:
- Delegation rate: >60% of tasks
- Response time: <10 min
- Quality: >95%

Team Impact:
- Tokens saved: 40%
- Task completion: 15% faster
- Context switches: 50% fewer
- Team satisfaction: Significantly improved
```

## Rollout Timeline

### Phase 1: Foundation (Week 1)
```
- [ ] Create @SubAgent-Research
- [ ] Create @SubAgent-Testing
- [ ] Test with @Backend
- [ ] Measure initial savings
```

### Phase 2: Expansion (Week 2-3)
```
- [ ] Create @SubAgent-Security
- [ ] Create @SubAgent-Optimization
- [ ] Extend to @Frontend, @Architect
- [ ] Refine delegation patterns
```

### Phase 3: Scale (Week 4)
```
- [ ] Create remaining SubAgents
- [ ] Automate routing (SARAH)
- [ ] All agents using delegation
- [ ] Measure 40% overall savings
```

### Phase 4: Optimize (Ongoing)
```
- [ ] Weekly performance reviews
- [ ] Continuous pattern improvement
- [ ] New SubAgents as needed
- [ ] Team training & best practices
```

---

**Next Steps:**
1. Create first 5 SubAgent definitions
2. Test delegation with @Backend
3. Measure token usage before/after
4. Document learnings
5. Expand to other agents

**Related Files:**
- [GL-002-SUBAGENT_DELEGATION.md](../guidelines/GL-002-SUBAGENT_DELEGATION.md) - Strategy & architecture
- [CONTEXT_OPTIMIZATION.md](../guidelines/CONTEXT_OPTIMIZATION.md) - Main agent optimization
- [Agent Definitions](.github/agents/)
