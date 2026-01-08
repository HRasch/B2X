---
docid: GL-061
title: GL 002 SUBAGENT_DELEGATION
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

# SubAgent Context Delegation Strategy

**Version:** 1.0  
**Created:** 30.12.2025  
**Managed by:** @SARAH, @Architect

## Executive Summary

Reduziere Hauptagent-Kontext um 50-70% durch strategische Delegation an spezialisierte SubAgenten. Jeder SubAgent hat einen fokussierten Kontext für eine spezifische Aufgabe, während der Hauptagent nur essenzielle Information behält.

## Core Principle

```
Hauptagent (10 KB max)
├── Role & Essentials (2 KB)
├── Current Task Context (3 KB)
├── Decision Making Rules (2 KB)
└── SubAgent Delegations (3 KB)
    ├── @SubAgent-Research
    ├── @SubAgent-Review
    ├── @SubAgent-Documentation
    └── @SubAgent-Testing
```

## SubAgent Architecture

### SubAgent Categories

| Category | Purpose | Lifespan | Context |
|----------|---------|----------|---------|
| **Research** | Gather & synthesize information | Task | <5 KB |
| **Review** | Analyze & provide feedback | Task | <3 KB |
| **Documentation** | Create & maintain docs | Task | <4 KB |
| **Testing** | Execute & verify quality | Task | <3 KB |
| **Integration** | Coordinate between systems | Task | <2 KB |
| **Optimization** | Performance & efficiency | Task | <2 KB |

### Example: @Backend Context Optimization

**Before (Full Context: 25 KB)**
```
@Backend (25 KB)
├── API design patterns (3 KB)
├── Database schema (4 KB)
├── Error handling rules (3 KB)
├── Testing requirements (3 KB)
├── Performance guidelines (2 KB)
├── Security requirements (2 KB)
├── Integration points (3 KB)
└── Current task (2 KB)
```

**After (With SubAgents: 8 KB)**
```
@Backend (8 KB) - MAIN
├── Role & coordination (2 KB)
├── Current task focus (3 KB)
├── Decision framework (2 KB)
└── SubAgent delegation map (1 KB)

@SubAgent-APIDesign (5 KB) - DELEGATE
├── API patterns
├── Error handling
└── Design decisions

@SubAgent-DatabaseDesign (4 KB) - DELEGATE
├── Schema reference
├── Optimization tips
└── Performance rules

@SubAgent-Testing (3 KB) - DELEGATE
├── Test requirements
├── Quality criteria
└── Verification checklist

@SubAgent-Integration (2 KB) - DELEGATE
├── Integration points
└── Interface contracts
```

**Reduction: 25 KB → 8 KB (68% less context)**

## Delegation Patterns

### Pattern 1: Research & Synthesis

**Use Case:** Agent needs to research a topic without carrying full documentation

```
@Backend (Main): "Research Node.js v20 migration impact for our APIs"
↓
@SubAgent-Research (Specialized):
- Fetches changelogs, migration guides, breaking changes
- Synthesizes into structured report
- Returns: `.ai/issues/{id}/analysis-nodejs-migration.md`
↓
@Backend (Main): Reads 2 KB summary from file, makes decisions
```

**Savings:**
- Keeps Node.js docs out of @Backend context
- @SubAgent-Research carries only research tools/process
- @Backend stays focused on decision-making

### Pattern 2: Code Review & Analysis

**Use Case:** Agent needs feedback without review criteria inline

```
@Backend (Main): "Review this API design for security issues"
↓
@SubAgent-SecurityReview (Specialized):
- Loads security guidelines (from .ai/guidelines/)
- Analyzes code against checklist
- Returns: `.ai/issues/{id}/security-review.md` with findings
↓
@Backend (Main): Sees review without carrying guidelines
```

**Savings:**
- Security guidelines stay in SubAgent, not main context
- Main agent gets actionable feedback, not documentation
- Specialization improves review quality

### Pattern 3: Documentation Generation

**Use Case:** Agent needs docs created without doc templates inline

```
@Backend (Main): "Document this API endpoint"
↓
@SubAgent-Documentation (Specialized):
- Loads API doc template
- Structures content
- Returns: `.docs/api/endpoints.md` (auto-formatted)
↓
@Backend (Main): Task complete, docs generated
```

**Savings:**
- API templates out of main context
- Format & structure automated
- Main agent stays focused on feature, not formatting

### Pattern 4: Test Development & Verification

**Use Case:** Agent needs tests without test framework inline

```
@Backend (Main): "Write tests for authentication flow"
↓
@SubAgent-Testing (Specialized):
- Loads test framework & patterns
- Generates test cases
- Returns: `src/auth.test.ts` + `.ai/issues/{id}/test-results.md`
↓
@Backend (Main): Tests written & verified
```

**Savings:**
- Test frameworks out of main context
- Pattern consistency automated
- Main agent delegates & integrates

## Implementation Strategy

### Phase 1: Identify Delegation Opportunities

**For each main agent, list:**
1. Context items >2 KB
2. Items only used for specific task types
3. Items referenced from knowledgebase
4. Repetitive processes (testing, docs, reviews)

**Example for @Backend:**
```
Large Context Items (>2 KB):
✓ API design patterns → Delegate to @SubAgent-APIDesign
✓ Database schema guide → Delegate to @SubAgent-DBDesign
✓ Test templates → Delegate to @SubAgent-Testing
✓ Security checklist → Delegate to @SubAgent-Security
```

### Phase 2: Create SubAgent Definitions

**Each SubAgent:**
- Clear, focused role
- Specific input requirements
- Output format & location
- When called & by whom
- <2 KB context size

**Template:**
```markdown
# @SubAgent-{Name}

## Role
Single responsibility focus

## Input Format
What the main agent provides

## Output Format
File location & structure

## Constraints
Speed, size, quality limits

## When to Use
Triggering conditions
```

### Phase 3: Define Delegation Interfaces

**Main Agent needs simple checklist:**

```
When I need to:
- ✓ Research topics → Call @SubAgent-Research
- ✓ Review code → Call @SubAgent-Review
- ✓ Generate docs → Call @SubAgent-Documentation
- ✓ Write tests → Call @SubAgent-Testing
- ✓ Analyze performance → Call @SubAgent-Optimization
```

**Each delegation:**
- Clear input format
- Expected output file
- Success criteria
- Fallback if SubAgent unavailable

### Phase 4: Documentation Flow

**Main agent context includes:**
```
## SubAgent Delegation Map

| Task | SubAgent | Input | Output |
|------|----------|-------|--------|
| Research | @SubAgent-Research | Topic + questions | `.ai/issues/{id}/analysis.md` |
| Review | @SubAgent-Review | Code + criteria | `.ai/issues/{id}/review.md` |
| Documentation | @SubAgent-Documentation | Details + template | `.docs/` + reference |
| Testing | @SubAgent-Testing | Code + spec | `.test.ts` + `.ai/issues/{id}/test-results.md` |
```

## Context Size Targets

### Before Delegation
```
@Backend (Main): 25-30 KB
- API patterns (5 KB)
- DB schema (5 KB)
- Testing guide (4 KB)
- Documentation (3 KB)
- Security (3 KB)
- Performance (2 KB)
- Current task (3 KB)
```

### After Delegation
```
@Backend (Main): 8-10 KB
- Role & responsibilities (2 KB)
- Current task (3 KB)
- Decision framework (2 KB)
- SubAgent delegation map (2 KB)

@SubAgent-APIDesign: 4 KB (only loaded when needed)
@SubAgent-DBDesign: 4 KB (only loaded when needed)
@SubAgent-Testing: 3 KB (only loaded when needed)
@SubAgent-Security: 3 KB (only loaded when needed)
@SubAgent-Docs: 3 KB (only loaded when needed)
```

**Result: 65-70% context reduction for main agent**

## Delegation Workflows

### Workflow 1: Research Task Delegation

```
@Backend initiates:
1. Identifies need: "Need to research React v19 upgrade impact"
2. Calls: @SubAgent-Research
3. Provides:
   - Topic: "React v19 migration"
   - Questions: Breaking changes, new features, upgrade path
   - Output path: `.ai/issues/FEAT-123/analysis-react-v19.md`

@SubAgent-Research executes:
1. Fetches official docs
2. Analyzes changelog
3. Identifies breaking changes
4. Structures findings

@Backend receives:
1. File: `.ai/issues/FEAT-123/analysis-react-v19.md`
2. Reads 2 KB summary
3. Makes decisions
4. Continues work
```

**Time Saved:** 
- No research tools in @Backend context
- No keeping research docs open
- Focus on decision-making only

### Workflow 2: Code Review Delegation

```
@Backend initiates:
1. Identifies review need: "Review API endpoint for security"
2. Calls: @SubAgent-SecurityReview
3. Provides:
   - Code location: `src/api/auth.ts`
   - Review type: "Security audit"
   - Output path: `.ai/issues/FEAT-123/security-review.md`

@SubAgent-SecurityReview executes:
1. Loads security guidelines
2. Reviews code
3. Checks against:
   - Input validation
   - SQL injection risks
   - Auth logic
   - Error handling
   - Logging/secrets

@Backend receives:
1. File: `.ai/issues/FEAT-123/security-review.md`
2. Reads findings
3. Fixes issues
4. Continues implementation
```

**Time Saved:**
- Security guidelines out of context
- Structured feedback
- Specialized expertise

### Workflow 3: Documentation Generation

```
@Backend initiates:
1. Task: "Document API endpoint"
2. Calls: @SubAgent-Documentation
3. Provides:
   - Function: auth.login()
   - Description: "User authentication endpoint"
   - Output: `.docs/api/login.md`

@SubAgent-Documentation executes:
1. Loads API doc template
2. Structures: Purpose, Parameters, Response, Examples
3. Generates markdown
4. Writes to `.docs/`

@Backend receives:
1. Confirmation: Docs created
2. File path: `.docs/api/login.md`
3. Continues development
```

**Time Saved:**
- No doc templates in context
- Consistent formatting
- Faster documentation

## SubAgent Library

### Core SubAgents (All Teams)

```
@SubAgent-Research
├── Role: Fetch, analyze, synthesize information
├── Context: 2 KB (research patterns only)
├── Inputs: Topic, questions, sources
├── Outputs: `.ai/issues/{id}/analysis.md`

@SubAgent-Review
├── Role: Code analysis & feedback
├── Context: 2 KB (review checklist)
├── Inputs: Code, review type
├── Outputs: `.ai/issues/{id}/review.md`

@SubAgent-Documentation
├── Role: Create structured docs
├── Context: 2 KB (doc templates)
├── Inputs: Content, doc type
├── Outputs: `.docs/...`

@SubAgent-Testing
├── Role: Test generation & execution
├── Context: 2 KB (test patterns)
├── Inputs: Code, test specs
├── Outputs: `.test.ts`, `.ai/issues/{id}/test-results.md`
```

### Optional Specialized SubAgents

```
@SubAgent-Optimization
├── Role: Performance analysis
├── Inputs: Code/system, metrics
├── Outputs: Performance report, recommendations

@SubAgent-Integration
├── Role: Cross-service coordination
├── Inputs: Integration points, contracts
├── Outputs: Integration plan, validation

@SubAgent-Migration
├── Role: Version upgrades, migrations
├── Inputs: From version, to version, codebase
├── Outputs: Migration plan, breaking changes summary

@SubAgent-Security
├── Role: Security analysis, vulnerability scanning
├── Inputs: Code, security policy
├── Outputs: Security review, vulnerability list
```

## Communication Protocol

### Main Agent → SubAgent

```json
{
  "task": "research|review|document|test|optimize",
  "subject": "Topic or code to process",
  "context": "Background information",
  "criteria": "Success criteria / checklist",
  "output_path": ".ai/issues/{id}/analysis.md",
  "deadline": "YYYY-MM-DD HH:mm"
}
```

### SubAgent → Main Agent

```json
{
  "status": "completed|failed",
  "output_file": ".ai/issues/{id}/analysis.md",
  "summary": "2-3 line summary",
  "key_findings": ["Finding 1", "Finding 2"],
  "time_spent": "minutes",
  "confidence": "high|medium|low"
}
```

## Token Impact Analysis

### Context Tokens Saved

```
Typical Main Agent (without delegation):
- 30 KB context = ~7,500 tokens

With SubAgent Delegation:
- 8 KB main context = ~2,000 tokens
- SubAgent called on-demand: ~1,500 tokens (temporary)
- Total per task: ~3,500 tokens

Savings per task: 54% (7,500 → 3,500)
```

### Token Budget Redistribution

```
BEFORE (all in main context):
Total: 10,000 tokens
- Main agent: 7,500 (bloated)
- Unused during task: 2,500 (waste)

AFTER (delegation):
Total: 5,000 tokens per task
- Main agent active: 2,000 (focused)
- SubAgent active: 1,500 (when called)
- Efficiency: +100% (same work, half tokens)
```

## Implementation Checklist

### For Each Main Agent

- [ ] Identify context items >2 KB
- [ ] Categorize as Tier 1-4 (critical to reference)
- [ ] Create SubAgent for Tier 2-4 items
- [ ] Define delegation interface
- [ ] Test delegation workflow
- [ ] Document in agent definition
- [ ] Update `.ai/guidelines/` with examples
- [ ] Measure token reduction
- [ ] Archive old full-context version

### For SARAH

- [ ] Inventory all SubAgents
- [ ] Create SubAgent registry in `.ai/`
- [ ] Set up delegation routing
- [ ] Monitor SubAgent calls
- [ ] Track token savings
- [ ] Regular optimization reviews

## Examples by Agent Type

### @Backend Delegation

```
REDUCED CONTEXT (8 KB):
- Role & current task (3 KB)
- Decision rules (2 KB)
- SubAgent map (3 KB)

DELEGATED TO SUBAGENTS:
@SubAgent-APIDesign:
  - API patterns, design guidelines
  - Error handling standards

@SubAgent-DBDesign:
  - Schema reference, optimization
  - Data migration patterns

@SubAgent-Security:
  - Security checklist, auth patterns
  - Vulnerability prevention

@SubAgent-Testing:
  - Test frameworks, patterns
  - Quality criteria
```

### @Frontend Delegation

```
REDUCED CONTEXT (8 KB):
- Role & current task (3 KB)
- Component design (2 KB)
- SubAgent map (3 KB)

DELEGATED TO SUBAGENTS:
@SubAgent-ComponentLibrary:
  - Reusable components, patterns

@SubAgent-Accessibility:
  - A11y guidelines, testing

@SubAgent-Performance:
  - Bundle optimization, rendering

@SubAgent-Testing:
  - Unit/E2E test patterns
```

### @Architect Delegation

```
REDUCED CONTEXT (8 KB):
- Role & current decision (3 KB)
- ADR process (2 KB)
- SubAgent map (3 KB)

DELEGATED TO SUBAGENTS:
@SubAgent-Research:
  - Technology research, comparison

@SubAgent-Analysis:
  - Impact analysis, trade-off evaluation

@SubAgent-Documentation:
  - ADR templates, rationale documentation

@SubAgent-Review:
  - Architecture review, validation
```

## Risks & Mitigations

| Risk | Mitigation |
|------|-----------|
| SubAgent unavailable | Fallback to main agent carrying context |
| Communication overhead | Batch delegations, async updates |
| Inconsistent responses | SubAgent context must be authoritative |
| Context duplication | Shared files in `.ai/`, not copied |
| Stale information | Regular SubAgent context updates |

## Success Metrics

### Main Agent Metrics
```
- Context size: <10 KB (was 25-30 KB)
- Setup time: <5 min per task
- Context reuse: >80%
- Link validity: 100%
```

### SubAgent Metrics
```
- Delegation rate: >50% of tasks
- Response time: <5 min per delegation
- Quality score: >90%
- Token efficiency: 50-70% savings
```

### Team Metrics
```
- Avg task context: <8 KB (was 15-20 KB)
- Total token usage: -40%
- Task completion time: -15% (less context switching)
- Team satisfaction: Improved focus
```

## Rollout Plan

### Week 1: Foundation
- [ ] Create 5 core SubAgents
- [ ] Define delegation interfaces
- [ ] Test with @Backend

### Week 2: Expansion
- [ ] Extend to @Frontend, @DevOps
- [ ] Refine SubAgent definitions
- [ ] Measure token savings

### Week 3: Optimization
- [ ] Fine-tune delegation timing
- [ ] Create SubAgent registry
- [ ] Document best practices

### Week 4: Operationalization
- [ ] All agents using SubAgents
- [ ] Automated routing (@SARAH)
- [ ] Regular reviews scheduled

---

**Next:** Create first SubAgent definitions in `.github/subagents/`  
**Related:** [CONTEXT_OPTIMIZATION.md](CONTEXT_OPTIMIZATION.md), [Agent Definitions](.github/agents/)
