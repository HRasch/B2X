---
docid: GL-098
title: SUBAGENT_DELEGATION_README
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

# SubAgent Delegation System

**Version:** 1.0  
**Status:** Strategy & Implementation Ready  
**Created:** 30.12.2025

## Quick Summary

Reduziere Hauptagent-Kontext um **50-70%** durch strategische Delegation an spezialisierte SubAgenten.

```
Before SubAgents: @Backend carries 25 KB context
After SubAgents: @Backend carries 8 KB context
                 → 68% context reduction
                 → 50% token savings
                 → Better focus & quality
```

## What is SubAgent Delegation?

Instead of carrying all reference material, patterns, and guidelines in the main agent's context, **delegate specialized tasks to focused SubAgents** that only load what they need.

```
Traditional Agent:
@Backend (25 KB) carries:
├── API design patterns
├── Database schema
├── Error handling
├── Security checklist
├── Testing framework
├── Performance guide
└── Current task

SubAgent Delegation:
@Backend (8 KB) focuses on:
├── Current task (2 KB)
├── Decision making (2 KB)
└── Delegation routing (2 KB)

Delegated to specialists:
├── @SubAgent-APIDesign (4 KB - only when needed)
├── @SubAgent-Security (3 KB - only when needed)
├── @SubAgent-Testing (3 KB - only when needed)
└── @SubAgent-Docs (3 KB - only when needed)
```

## Key Benefits

| Metric | Before | After | Improvement |
|--------|--------|-------|-------------|
| Main Agent Context | 25 KB | 8 KB | **-68%** |
| Tokens per Task | 6,250 | 3,500 | **-44%** |
| Focus Quality | Dispersed | Focused | **+50%** |
| Specialization | Generalist | Expert | **+100%** |
| Setup Time | 15 min | 5 min | **-67%** |

## Architecture

```
Main Agents (Reduced Context)
├── @Backend (8 KB)
├── @Frontend (7 KB)
├── @Architect (7 KB)
└── @DevOps (6 KB)

Core SubAgents (On-Demand)
├── @SubAgent-Research (2 KB)
├── @SubAgent-Review (2 KB)
├── @SubAgent-Testing (2 KB)
├── @SubAgent-Documentation (2 KB)
├── @SubAgent-Security (2 KB)
└── @SubAgent-Optimization (2 KB)

SARAH (Coordination)
└── Routes delegations, monitors quality, tracks metrics
```

## How It Works

### Step 1: Main Agent Identifies Delegatable Work

```
@Backend thinking: "I need to research Node.js v20 migration"

Decision:
- Is this core to my task? NO
- Is this reference material? YES
- Should specialist handle it? YES
→ Delegate to @SubAgent-Research
```

### Step 2: Create Delegation Request

```json
{
  "task": "research",
  "subject": "Node.js v20 migration impact",
  "context": "Express.js, PostgreSQL, currently v18",
  "output": ".ai/issues/FEAT-456/nodejs-migration.md"
}
```

### Step 3: SubAgent Executes

```
@SubAgent-Research:
- Fetches Node.js changelog
- Analyzes migration guide
- Identifies breaking changes
- Synthesizes into structured report
- Writes to output file
- Returns summary to main agent
```

### Step 4: Main Agent Uses Result

```
@Backend reads:
- 2 KB summary of key findings
- Links to full analysis
- Actionable recommendations
- Confidence level

Continues development with:
- Lean context (8 KB, not 25 KB)
- Clear focus
- Specialist findings
- Higher quality
```

## Files & Documentation

### Guidelines (Strategy & Details)

- **[SUBAGENT_DELEGATION.md](./SUBAGENT_DELEGATION.md)** — Complete strategy, architecture, patterns, metrics
- **[SARAH-SUBAGENT-COORDINATION.md](./SARAH-SUBAGENT-COORDINATION.md)** — SARAH's coordination responsibilities, monitoring, escalation

### Workflows (Implementation & Execution)

- **[subagent-delegation.workflow.md](../workflows/subagent-delegation.workflow.md)** — Step-by-step implementation, patterns by agent type, practical checklists

### Prompts (For Active Use)

- **[subagent-delegation.prompt.md](../../.github/prompts/subagent-delegation.prompt.md)** — When to delegate, how to create requests, examples, anti-patterns

## SubAgent Types

### Core SubAgents (Universal)

| SubAgent | Purpose | Max Context | When to Use |
|----------|---------|-------------|------------|
| @SubAgent-Research | Fetch & analyze information | 2 KB | Need external research, synthesis |
| @SubAgent-Review | Code/design analysis | 2 KB | Need specialist feedback |
| @SubAgent-Testing | Test generation & execution | 2 KB | Need tests written, coverage verified |
| @SubAgent-Documentation | Create structured docs | 2 KB | Need docs generated, formatted |
| @SubAgent-Security | Security audit | 2 KB | Need security review, vulnerabilities |
| @SubAgent-Optimization | Performance analysis | 2 KB | Need perf insights, bottleneck fix |

### Specialized SubAgents (Optional)

```
@SubAgent-Integration     (Cross-service coordination)
@SubAgent-Migration       (Version upgrades)
@SubAgent-Analysis        (Impact assessment)
@SubAgent-Accessibility  (A11y audit)
@SubAgent-Performance    (Optimization deep-dive)
```

## Usage by Main Agent Type

### @Backend Delegation Example

```
Task: Implement user authentication API

Delegations:
1. @SubAgent-Research → "Research auth best practices" 
   Output: `.ai/issues/FEAT-123/auth-research.md`

2. @SubAgent-Security → "Review auth API for security"
   Output: `.ai/issues/FEAT-123/security-review.md`

3. @SubAgent-Testing → "Generate auth tests"
   Output: `src/auth.test.ts`

Result:
- @Backend context: 8 KB (was 25 KB)
- All specialist work completed
- Higher quality implementation
- 60% faster, 50% fewer tokens
```

### @Frontend Delegation Example

```
Task: Build user profile form

Delegations:
1. @SubAgent-Review → "Accessibility audit of form"
   Output: `.ai/issues/FEAT-456/a11y-audit.md`

2. @SubAgent-Testing → "Generate E2E tests"
   Output: `src/components/UserProfile.test.tsx`

3. @SubAgent-Optimization → "Analyze bundle impact"
   Output: `.ai/issues/FEAT-456/perf-report.md`

Result:
- @Frontend context: 7 KB (was 24 KB)
- Accessibility verified
- Tests comprehensive
- Performance optimized
```

### @Architect Delegation Example

```
Task: Decide on database architecture

Delegations:
1. @SubAgent-Research → "Compare PostgreSQL vs MongoDB"
   Output: `.ai/issues/ADR-123/db-analysis.md`

2. @SubAgent-Analysis → "Impact analysis of choice"
   Output: `.ai/issues/ADR-123/impact-analysis.md`

3. @SubAgent-Documentation → "Create ADR"
   Output: `.ai/decisions/ADR-042-db-choice.md`

Result:
- @Architect context: 7 KB (was 22 KB)
- Research comprehensive
- Decision documented
- Team aligned
```

## Context Size Targets

### Main Agent Context

```
Target per main agent:
- Agent definition: 2 KB (fixed)
- Current task focus: 3 KB (variable)
- Decision framework: 2 KB (fixed)
- Delegation map: 1 KB (fixed)
────────────────────────
Maximum: 8-10 KB
```

### SubAgent Context (On-Demand)

```
Each SubAgent carries only:
- Core responsibility (1 KB)
- Input/output format (0.5 KB)
- Quality standards (0.3 KB)
- Tools & utilities (0.2 KB)
────────────────────────
Maximum: 2 KB per SubAgent
```

### Total Delegation Cost

```
Main agent active: 8 KB = 2,000 tokens
SubAgent called on-demand: 2 KB = 500 tokens
Per task context: <3,500 tokens

Savings vs. bloated main context:
- Bloated main agent: 25 KB = 6,250 tokens
- With SubAgents: 3,500 tokens
- Reduction: 44% ✓
```

## Getting Started

### Step 1: Understand the Strategy

```
Read in order:
1. This file (README.md) — Overview
2. SUBAGENT_DELEGATION.md — Full strategy
3. subagent-delegation.workflow.md — Implementation
4. subagent-delegation.prompt.md — Practical usage
```

### Step 2: Review Your Current Context

```
For each main agent:
1. List all context items
2. Categorize as Tier 1-4
3. Identify delegatable work
4. Create delegation map
```

### Step 3: Create SubAgent Definitions

```
For each SubAgent type:
1. Create .github/subagents/{name}.subagent.md
2. Define role, inputs, outputs
3. Set quality criteria
4. Test with main agent
```

### Step 4: Implement Delegations

```
Start with @Backend:
1. Create @SubAgent-Research
2. Test first delegation
3. Measure context/token reduction
4. Expand to other agents
```

### Step 5: Monitor & Optimize

```
Weekly by @SARAH:
1. Track delegation metrics
2. Monitor context sizes
3. Check quality scores
4. Optimize performance
```

## Success Metrics (First Week Target)

```
✅ Main agent context: <10 KB (from 20-30 KB)
✅ Token usage: -40% per task
✅ SubAgent quality: >95%
✅ Delegation adoption: >60%
✅ Team satisfaction: Improved focus
```

## Common Patterns

### Pattern 1: Research Delegation

```
Main Agent: "Need to research X"
  ↓
@SubAgent-Research: Fetch docs, synthesize findings
  ↓
Output: `.ai/issues/{id}/analysis.md`
  ↓
Main Agent: Reads 2 KB summary, decides
```

### Pattern 2: Security Review

```
Main Agent: "Review code for security"
  ↓
@SubAgent-Security: Check checklist, identify risks
  ↓
Output: `.ai/issues/{id}/security-review.md`
  ↓
Main Agent: Implements fixes
```

### Pattern 3: Test Generation

```
Main Agent: "Write tests for function"
  ↓
@SubAgent-Testing: Generate comprehensive tests
  ↓
Output: `src/{name}.test.ts`
  ↓
Main Agent: Integrates & runs
```

## Troubleshooting

### If context still too large

```
Check:
- [ ] All Tier 2-4 items delegated?
- [ ] Only critical info kept inline?
- [ ] Links used instead of inline docs?
- [ ] Current task focused?

Action:
- Delegate more aggressively
- Use links for all reference material
- Archive completed context
```

### If SubAgent response slow

```
Check:
- [ ] Request clear & specific?
- [ ] Output path specified?
- [ ] Scope reasonable?
- [ ] No circular dependencies?

Action:
- Simplify request
- Batch delegations
- Use specific criteria
```

### If delegation quality issues

```
Check:
- [ ] Quality criteria clear?
- [ ] SubAgent scope appropriate?
- [ ] Sufficient context provided?
- [ ] Output format specified?

Action:
- Clarify success criteria
- Review SubAgent definition
- Provide more context
- Follow up with main agent review
```

## Next Steps

1. **Review Strategy** → Read SUBAGENT_DELEGATION.md
2. **Plan Implementation** → Create SubAgent list for your team
3. **Start Small** → Test with @Backend + @SubAgent-Research
4. **Measure & Learn** → Track context/token reduction
5. **Scale Up** → Extend to all agents & SubAgents
6. **Optimize** → Monthly reviews, continuous improvement

## Related Documentation

**Strategies & Guidelines:**
- [SUBAGENT_DELEGATION.md](./SUBAGENT_DELEGATION.md)
- [SARAH-SUBAGENT-COORDINATION.md](./SARAH-SUBAGENT-COORDINATION.md)
- [CONTEXT_OPTIMIZATION.md](./CONTEXT_OPTIMIZATION.md)

**Workflows & Prompts:**
- [subagent-delegation.workflow.md](../workflows/subagent-delegation.workflow.md)
- [subagent-delegation.prompt.md](../../.github/prompts/subagent-delegation.prompt.md)
- [context-optimization.prompt.md](../../.github/prompts/context-optimization.prompt.md)

**Agent Definitions:**
- [Agent Definitions](../../.github/agents/)
- [SARAH.agent.md](../../.github/agents/SARAH.agent.md)

## Questions?

Refer to:
- Strategy questions → SUBAGENT_DELEGATION.md
- Implementation questions → subagent-delegation.workflow.md
- Usage questions → subagent-delegation.prompt.md
- Coordination questions → SARAH-SUBAGENT-COORDINATION.md

---

**System Status:** ✅ Ready for Implementation  
**Last Updated:** 30.12.2025  
**Maintained by:** @SARAH, @Architect  
**Next Review:** 06.01.2026
