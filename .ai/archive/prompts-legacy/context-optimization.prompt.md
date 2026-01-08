---
docid: UNKNOWN-015
title: Context Optimization.Prompt
owner: @DocMaintainer
status: Archived
created: 2026-01-08
---

# Agent Context Optimization Prompt

## Purpose
Guide agents to maintain efficient, high-value context without token bloat. Optimize for relevance, not volume.

## When to Use
- Agent context is growing too large (>50 KB)
- Agent's knowledge base needs consolidation
- Unclear what information is actually used vs. unused
- Moving between different tasks/contexts
- Preparing handover to another agent

## Context Audit Process

[React patterns](../../.ai/knowledgebase/INDEX.md)
```
Current context inventory:
- Agent instructions: [X lines]
- Task context: [X lines]
- Reference files: [X files]
- Knowledgebase links: [X files]
- Historical context: [X lines]

Total active context: [X KB]
Target: <10 KB for focused tasks
```

### Step 2: Categorize by Relevance

**Tier 1 - CRITICAL (Must Keep)**
- Current task requirements
- Agent role definition
- Security/compliance rules
- Essential API signatures
- Critical constraints

**Tier 2 - IMPORTANT (Keep with Links)**
See [API Spec](../../docs/) for details.
- Common edge cases
- Integration points
- Reference documentation

**Tier 3 - REFERENCE (Link Only)**
- General guidelines
- Historical decisions
- Examples
- Background information

**Tier 4 - ARCHIVE (Remove)**
- Completed tasks
- Obsolete information
- Redundant explanations
- Old decision logs

### Step 3: Consolidation Strategy

#### A. Replace Prose with Links
```
❌ BEFORE (250 tokens):
"The system uses React hooks for state management. 
We prefer functional components over class components. 
Always use useState for local state, useContext for global state, 
and custom hooks for shared logic. See the component patterns 
for more details on how to structure hooks properly..."
[Code Review Checklist](code-review.prompt.md)
✅ AFTER (5 tokens):
"[React patterns](../../.ai/knowledgebase/INDEX.md)"
```

#### B. Compress Documentation
```
❌ BEFORE (150 tokens):
## API Response Format
The API returns JSON responses with the following structure:
{
[Code Review Checklist](code-review.prompt.md)
  "meta": {
    "timestamp": "ISO-8601",
    "version": "1.0"
  },
  "errors": [...]
 [CONTEXT_OPTIMIZATION.md](../../.ai/guidelines/CONTEXT_OPTIMIZATION.md)
 [Knowledgebase INDEX](../../.ai/knowledgebase/INDEX.md)
 [Agent Definitions](../agents/)
## API Response
```json
{ "data": {...}, "meta": {...}, "errors": [...] }
```
See [API Spec](../../docs/) for details.
```

#### C. Use Tables Instead of Lists
```
❌ BEFORE (80 tokens):
The Backend agent is responsible for APIs, handles service logic, 
manages databases, and coordinates with DevOps. The Frontend agent 
focuses on UI components, manages state, and handles UX. The DevOps 
agent manages CI/CD, infrastructure...

✅ AFTER (30 tokens):
| Agent | Focus |
|-------|-------|
| @Backend | APIs, Services, DB |
| @Frontend | UI, State, UX |
| @DevOps | CI/CD, Infra |
```

#### D. Reference Standard Locations
```
❌ BEFORE (repeated):
For code reviews, check error handling, type safety, 
performance considerations...

✅ AFTER (5 tokens):
[Code Review Checklist](code-review.prompt.md)
```

### Step 4: Implementation Rules

#### Rule 1: Single Source of Truth
```
If information exists in:
- .github/instructions/*
- .ai/knowledgebase/*
- .ai/guidelines/*
- .ai/decisions/ADRs

→ LINK TO IT, don't duplicate
```

#### Rule 2: Inline Only Critical Information
```
KEEP INLINE:
✓ Current task details
✓ Immediate success criteria
✓ Security constraints
✓ Critical dependencies

MOVE TO FILES:
✗ General patterns
✗ Historical context
✗ Examples
✗ Tutorials
✗ Reference docs
```

#### Rule 3: Structured Context Hierarchy
```
Agent Context Structure (Target: <10 KB total)
├── Agent Definition (2 KB)
│   ├── Role & Expertise
│   ├── Key Responsibilities
│   └── Coordination Partners
├── Current Task Context (5 KB)
│   ├── Immediate Goal
│   ├── Success Criteria
│   └── Constraints
├── Essential References (2 KB)
│   ├── Links to patterns
│   ├── API specifications
│   └── Key Guidelines
└── Active Decisions (1 KB)
    ├── Recent choices
    └── Related issues
```

#### Rule 4: Remove After Using
```
When task completes:
1. Extract learnings to knowledgebase
2. Archive decision in .ai/decisions/
3. Document in .ai/logs/
4. Clear temporary context
5. Prepare for next task
```

## Optimization Checklist

### Context Audit
- [ ] List all files/references in current context
- [ ] Identify Tier 1-4 items
- [ ] Mark duplicated information
- [ ] Find broken/outdated links
- [ ] Calculate total context size

### Consolidation
- [ ] Replace 5+ line explanations with links
- [ ] Convert lists to tables (if >5 items)
- [ ] Archive completed tasks
- [ ] Remove redundant examples
- [ ] Keep only critical code samples

### Documentation
- [ ] Verify all links point to current versions
- [ ] Ensure knowledgebase has linked content
- [ ] Update .ai/guidelines/ if new patterns emerge
- [ ] Create ADR for major decisions
- [ ] Document context rationale

### Handover
- [ ] Create summary for next agent
- [ ] Mark essential vs. reference info
- [ ] Prepare context files in .ai/issues/{id}/
- [ ] Document discontinuities/gaps
- [ ] List open questions

## Token Budget Guidelines

### Per Agent
```
Agent Instructions: <2 KB
Task Context: <5 KB
Reference Links: <3 KB
Active Decisions: <1 KB
─────────────────────────
TOTAL TARGET: <10 KB
```

### Per Task
```
Critical Info: <3 KB
Supporting Context: <2 KB
References: <1 KB
─────────────────────────
TOTAL: <5 KB
```

### Knowledgebase
```
Each entry: <5 KB focused content
Multiple references: Link to one source
Redundant info: Consolidate or remove
```

## Common Optimization Patterns

### Pattern 1: Remove Repetition
```
❌ BEFORE:
"Use the @Backend agent for backend work, backend issues, 
backend implementation, backend testing..."

✅ AFTER:
"Use @Backend for backend work"
```

### Pattern 2: Collapse Examples
```
❌ BEFORE (8 code examples showing variations)
✅ AFTER (1 template example + link to variations)
```

### Pattern 3: Replace Explanations with Titles
```
❌ BEFORE:
"This is the error handling best practice where you validate 
inputs at the function boundary and throw descriptive errors..."

✅ AFTER:
"[Error Handling](../../.ai/knowledgebase/INDEX.md)"
```

### Pattern 4: Metadata Only
```
❌ BEFORE (full status history):
"Started on Monday 5 AM, paused at 11 AM due to build failure, 
resumed after 2-hour investigation, completed Tuesday 3 PM..."

✅ AFTER:
"Status: Completed | Date: 30.12.2025 | Issues: 1 resolved"
```

## Measurement & Monitoring

### Metrics
```
- Context size: Tokens used per agent session
- Refresh rate: How often context is updated
- Link validity: % of working references
- Reusability: How often same context used
- Hit rate: % relevant to current task
```

### Targets
```
Average task context: <5 KB
Reference hit rate: >80%
Link validity: 100%
Context reuse: >60%
Setup time: <5 min
```

### Review Frequency
```
Monthly: Full context audit per agent
Weekly: Check broken links
Per-task: Handover context optimization
```

## Implementation Steps

### For Individual Agent
1. Collect all current context
2. Categorize items Tier 1-4
3. Remove Tier 4 items
4. Convert Tier 2-3 to links
5. Restructure into hierarchy
6. Test task execution
7. Measure new token count

### For Team
1. @Architect: Review agent definitions
2. @Backend: Review task context standards
3. @TechLead: Review code patterns link structure
4. @SARAH: Consolidate and standardize
5. @DevOps: Automate context cleanup

### For Knowledgebase
1. Audit all `.ai/knowledgebase/` entries
2. Identify duplicates
3. Consolidate into single sources
4. Create comprehensive INDEX
5. Verify all links active
6. Set update schedule

## Anti-Patterns (Avoid)

❌ Keeping "just in case" information
❌ Duplicating info across multiple files
❌ Broken or outdated links
❌ Unused historical context
❌ Verbose explanations instead of links
❌ Code samples not marked as templates
❌ Decision logs without consolidation
❌ Context that grows indefinitely

## Success Indicators

✅ Consistent <10 KB context per agent
✅ Clear separation of Tier 1-4 information
✅ Single source of truth for each topic
✅ <1 min to find needed information
✅ New agents can onboard in <5 min
✅ Zero broken links in context
✅ All decisions documented in .ai/decisions/
✅ Regular context hygiene (monthly)

## Template: Context Summary

```markdown
# Agent Context Optimization Summary

## Audit Results
- Current size: [X KB]
- Target size: [Y KB]
- Reduction: [Z%]

## Tier Analysis
| Tier | Items | Action |
|------|-------|--------|
| 1 | [N] | Keep inline |
| 2 | [N] | Link |
| 3 | [N] | Link |
| 4 | [N] | Remove |

## Key Changes
- Removed: [Items]
- Linked: [Items]
- Consolidated: [Items]

## New Context Structure
[Hierarchy of organized context]

## Handover Notes
- Essential for @Agent: [Items]
- Good to know: [Items]
- See also: [Links]

## Metrics
- Tokens saved: [N]
- Refresh time: [Time]
- Link validity: [%]

---
Optimized: [Date] | By: [Agent]
```

---

**Next Steps:**
1. Run audit on current agent context
2. Use this prompt iteratively
3. Document findings in `.ai/logs/context-audit.md`
4. Update `.ai/guidelines/CONTEXT_OPTIMIZATION.md` with learnings
5. Schedule monthly review

**Related:**
- [CONTEXT_OPTIMIZATION.md](../../.ai/guidelines/CONTEXT_OPTIMIZATION.md)
- [Knowledgebase INDEX](../../.ai/knowledgebase/INDEX.md)
- [Agent Definitions](../agents)
