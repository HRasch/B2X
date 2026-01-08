---
docid: TASK-002
title: BRIEF_TEMPLATE
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

---
docid: TASK-BRIEF-TEMPLATE
title: Task Brief Template
owner: "@SARAH"
status: Template
---

# Task Brief

**Task ID**: TASK-XXX  
**Agent**: @[AgentName]  
**Domain**: [backend|frontend|security|devops|testing|architecture]  
**Priority**: [P0|P1|P2|P3]  
**Start Date**: YYYY-MM-DD  
**Target Completion**: YYYY-MM-DD  

---

## Objective

[Single paragraph describing the concrete deliverable, not abstract.]

**Acceptance Criteria**:
1. [ ] Criterion 1
2. [ ] Criterion 2
3. [ ] Criterion 3

---

## Context

**Linked Issue**: [GitHub issue link]  
**Related ADR**: [ADR-XXX if applicable]  
**Depends On**: [TASK-XXX if applicable, or "None"]  
**Blockers**: [None, or external dependency description]

---

## Scope

**In Scope**:
- Item 1
- Item 2

**Out of Scope**:
- Item A (why: ...)
- Item B (why: ...)

---

## Success Criteria (Verification)

How will we know this task is done?

- [ ] Code compiles without errors
- [ ] Tests pass (coverage >80%)
- [ ] No new linting warnings
- [ ] PR linked in progress.md
- [ ] @[ReviewerAgent] approved

---

## Context Files to Load

**Do NOT load full project context. Load only what's needed:**

1. **Instructions**: 
   - Path pattern: `src/api/**` → Load `backend-essentials.instructions.md` only
   - Do NOT load: frontend, testing, devops instructions

2. **Knowledge Base**:
   - Query on-demand using KB-MCP instead of full articles
   - Example: `kb-mcp/search_knowledge_base query="Wolverine handler patterns"`

3. **Documents**:
   - ADR-XXX (specific decision)
   - Do NOT load: GL-* guidelines or KB index

---

## Estimation

**Complexity**: [Simple|Moderate|Complex]  
**Estimated Duration**: [2h|4h|1d|2d|1w]  
**Parallel OK**: [Yes|No] — Can run alongside other tasks?

---

## Notes for Agent

[Any special instructions, gotchas, or context that doesn't fit above]

---

**Prepared by**: @SARAH  
**Prepared on**: YYYY-MM-DD
