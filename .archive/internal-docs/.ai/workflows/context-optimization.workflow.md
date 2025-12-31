# Context Optimization Workflow

**Version:** 1.0  
**Created:** 30.12.2025  
**Used by:** All Agents (coordinated by @SARAH)

## Quick Start

```
User: @Agent "Optimize your context for efficiency"
Agent: Runs context audit using /context-optimization prompt
Output: `.ai/logs/context-audit-{agent}-{date}.md`
Result: Reduced token usage, clearer information hierarchy
```

## When to Use

- **Monthly:** Regular hygiene (first of month)
- **Between tasks:** Clear temporary context
- **On handover:** Prepare clean context for next agent
- **On growth:** Context > 15 KB
- **On slowdown:** If agent seems unfocused

## Implementation for Each Agent

### Agent Inventory Check
```
Current context sources:
- .github/agents/{agent}.agent.md (FIXED, don't modify)
- .github/instructions/*.md (SHARED, use in-context)
- .ai/knowledgebase/* (REFERENCE, link to)
- .ai/guidelines/* (REFERENCE, link to)
- .ai/decisions/ADR-*.md (REFERENCE, link to)
- Task context (TEMPORARY, clear after task)
- Active issues in .ai/issues/ (TEMPORARY, link to)
```

### Tier Classification Example

**@Backend Agent Context Audit:**

```
TIER 1 - CRITICAL (Keep Inline, <2 KB)
✓ Role: Backend development, APIs, services, database
✓ Current task: Implement user authentication API
✓ Success criteria: GET /users/{id}, POST /login work
✓ Security rule: Always validate inputs
✓ Deadline: 31.12.2025

TIER 2 - IMPORTANT (Link)
- API Design patterns → [API Design](../../.ai/knowledgebase/patterns/api-design.md)
- Database schema → [Schema ADR](../../.ai/decisions/ADR-001-schema.md)
- Error handling → [Error Patterns](../../.ai/knowledgebase/patterns/error-handling.md)

TIER 3 - REFERENCE (Link)
- Code review checklist → [Code Review](../code-review.prompt.md)
- TypeScript guidelines → [TypeScript Best Practices](../../.ai/guidelines/typescript.md)
- Testing strategy → [Testing Guide](../../.ai/knowledgebase/best-practices/testing.md)

TIER 4 - ARCHIVE (Remove)
✗ Yesterday's debug logs
✗ Completed sprint retrospective
✗ Old API versions (kept only current)
✗ Duplicate explanations
```

### Context Reduction Example

**Before Optimization:**
```
Total context: 42 KB

Breakdown:
- Agent definition: 2 KB
- API specifications (full): 12 KB
- Database schema (full): 8 KB
- Error handling examples: 6 KB
- Historical decisions: 8 KB
- Current task: 3 KB
- Unused reference: 3 KB
```

**After Optimization:**
```
Total context: 8 KB (81% reduction)

Breakdown:
- Agent definition: 2 KB (unchanged)
- API link: 0.1 KB → [API Spec](docs/api.md)
- Database link: 0.1 KB → [Schema](decisions/schema-adr.md)
- Error handling link: 0.1 KB → [Patterns](knowledgebase/patterns/)
- Current task: 3 KB (focused)
- Decision references: 0.7 KB → [ADRs](decisions/)
- Links summary: 2 KB
```

## Step-by-Step Process

### Step 1: Collect (10 min)
```
List all context sources:
- Files open in editor
- Attached files to this conversation
- Knowledgebase files being referenced
- Guidelines being used
- Historical context in chat

Total: ___ KB
```

### Step 2: Categorize (15 min)
```
For each item:
□ Is this Tier 1 (critical right now)?
□ Is this Tier 2 (related, maybe needed)?
□ Is this Tier 3 (good to know)?
□ Is this Tier 4 (obsolete)?

Count items per tier
```

### Step 3: Convert to Links (20 min)
```
Tier 2-3 items:
□ Does this exist in .ai/knowledgebase/?
  → Yes: Create link
  → No: Move to knowledgebase first, then link

□ Does this exist in .ai/guidelines/?
  → Yes: Create link
  → No: Create guideline file

□ Is this a decision?
  → Yes: Should be ADR in .ai/decisions/
  → Create/link ADR
```

### Step 4: Consolidate (10 min)
```
Inline content:
□ Compress prose to bullets
□ Replace lists with tables
□ Remove redundant explanations
□ Keep only critical code samples
□ Mark templates as examples

Target: <3 KB inline content
```

### Step 5: Restructure (10 min)
```
Organize remaining context:
1. Agent definition (fixed)
2. Current task focus
3. Essential constraints
4. Key links section
5. Active decisions
```

### Step 6: Document (5 min)
```
Create summary:
- .ai/logs/context-audit-{agent}-{date}.md
- Include: Before/after sizes
- List: What was removed
- Explain: Why restructured
```

## Output Files

### Audit Log Example
```markdown
# Context Optimization Audit: @Backend

**Date:** 30.12.2025  
**Agent:** @Backend  
**Duration:** 60 min  

## Results

**Reduction:** 42 KB → 8 KB (81%)

### Tier Breakdown
| Tier | Items | Size | Action |
|------|-------|------|--------|
| 1 | 5 | 2 KB | Kept |
| 2 | 8 | 0.5 KB | Linked |
| 3 | 12 | 0.7 KB | Linked |
| 4 | 6 | - | Removed |

### Removed (Tier 4)
- Old API v1 spec (superseded by v2)
- Yesterday's debug output
- Completed feature branch details
- Duplicate error examples

### New Structure
```
Active Context (3 KB):
├── Authentication API implementation (current task)
├── Success criteria & deadline
└── Key constraints

Essential Links (5 KB):
├── API Design patterns
├── Database schema ADR
├── Error handling patterns
└── Code review checklist
```

### Next Steps
- [ ] Test task execution with new context
- [ ] Measure token usage next session
- [ ] Archive removed items in .ai/logs/
- [ ] Update knowledgebase if new patterns found

---
Optimized by: @Backend (assisted)
Reviewed by: @SARAH
```

## SARAH's Coordination Role

**Weekly Context Health Check:**
```
@SARAH monitors:
1. Agent context sizes (flagged if >15 KB)
2. Link validity in knowledgebase
3. Outdated references
4. Unused files in context
5. Duplication across agents

Output: `.ai/logs/context-health-{week}.md`
Action: Notify agents needing optimization
```

**Monthly Consolidation:**
```
@SARAH coordinates team:
1. Collect all audit logs
2. Identify common patterns
3. Update shared guidelines
4. Consolidate duplicates
5. Archive removed context

Output: `.ai/logs/monthly-context-report-{month}.md`
```

## Common Optimization Patterns

### Pattern 1: API Documentation
```
❌ BEFORE (15 KB inline):
Full API spec with all endpoints, parameters, response formats...

✅ AFTER (0.1 KB inline + link):
[See API Spec](docs/api.md) for endpoints. Key auth header: `Authorization: Bearer {token}`
```

### Pattern 2: Error Handling
```
❌ BEFORE (8 KB inline):
Try/catch blocks explained, error types listed, examples shown...

✅ AFTER (0.1 KB inline + link):
[See error patterns](patterns/error-handling.md). Always validate before processing.
```

### Pattern 3: Guidelines
```
❌ BEFORE (6 KB inline):
Full code style guide, naming conventions, formatting rules...

✅ AFTER (0.1 KB inline + link):
[Follow TypeScript guidelines](guidelines/typescript.md)
```

### Pattern 4: Decision History
```
❌ BEFORE (10 KB inline):
"We decided to use X because Y and Z. We considered A but it had problems. 
In the past we used B but migrated to X because..."

✅ AFTER (link to ADR):
[See ADR-007: Architecture Decision](decisions/ADR-007-chosen-approach.md)
```

## Metrics & Monitoring

### Track Per Agent
```
Weekly measurements:
- Context size (KB)
- Link validity (%)
- Tier 1 ratio (should be <30%)
- Setup time (min)
- Token reduction (%)
```

### Team-wide Metrics
```
Monthly tracking:
- Average context size
- Knowledgebase growth
- Link breakage rate
- Reusable patterns identified
- Time saved per agent
```

### Success Indicators
```
✅ All agents: <10 KB active context
✅ All links: 100% validity
✅ All decisions: Documented in ADRs
✅ Onboarding: New agents ready in 5 min
✅ Handover: Clean context transitions
✅ Reuse: Same patterns across team
```

## Tools & Integration

### Automation (Future)
```
Scripts to help:
- Check link validity
- Measure context size
- Identify duplicates
- Archive completed tasks
- Generate audit reports
```

### Related Documentation
- [CONTEXT_OPTIMIZATION.md](.ai/guidelines/CONTEXT_OPTIMIZATION.md)
- [context-optimization.prompt.md](.github/prompts/context-optimization.prompt.md)
- [Knowledgebase INDEX](.ai/knowledgebase/INDEX.md)
- [Agent Definitions](.github/agents/)

---

**Next Context Optimization:** 06.01.2026 (weekly)  
**Team Review:** 13.01.2026 (monthly)
