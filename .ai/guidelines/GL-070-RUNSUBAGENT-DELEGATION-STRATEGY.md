---
docid: GL-070
title: runSubagent Delegation Strategy
owner: @SARAH
status: Active
created: 2026-01-11
---

# GL-070: runSubagent Delegation Strategy

## Purpose

Define when and how to use `runSubagent` for token-optimized task delegation, including the new Knowledge-Expert agents pattern.

---

## Core Principle

> **Delegate to isolate**: Use `runSubagent` when task output should NOT persist in main conversation context.

---

## When to Use runSubagent

### ✅ USE runSubagent For:

| Scenario | Token Savings | Example |
|----------|---------------|---------|
| **Knowledge queries** | ~90% | Ask @KBWolverine for Wolverine patterns |
| **Multi-file analysis** | ~70% | Scan codebase for pattern violations |
| **Validation suites** | ~80% | Run security/accessibility checks |
| **Code generation** | ~60% | Generate boilerplate from templates |
| **Research tasks** | ~75% | Gather context from multiple sources |

### ❌ DON'T USE runSubagent For:

- Simple file reads (use `read_file`)
- Single MCP tool calls (call directly)
- Tasks requiring conversation memory
- Interactive debugging sessions

---

## Knowledge-Expert Agents Pattern

### Available KB-Experts

| Agent | Domain | DocID |
|-------|--------|-------|
| `@KBWolverine` | Wolverine/CQRS patterns | AGT-KB-001 |
| `@KBVue` | Vue.js 3/Frontend patterns | AGT-KB-002 |

### Usage Pattern

```text
#runSubagent @KBWolverine: [specific question]
Return: [specify output format]
```

### Example Queries

**Backend Pattern Query:**
```text
#runSubagent @KBWolverine: 
What's the saga pattern for order processing with compensation?
Return: code example (max 25 lines) + pattern name
```

**Frontend Pattern Query:**
```text
#runSubagent @KBVue:
Show Pinia store pattern with async actions and error handling
Return: complete store code + usage example
```

**Validation Query:**
```text
#runSubagent @KBWolverine:
Is this handler pattern correct? [paste code]
Return: valid/invalid + specific corrections
```

---

## Delegation Templates

### 1. Security Audit Delegation
```text
#runSubagent Security Audit:
- Scan dependencies for HIGH/CRITICAL CVEs
- Check SQL injection in backend/
- Scan XSS in frontend/Store
- Validate input sanitization

Return ONLY: blocking_issues + CVE_list + fix_recommendations
```

### 2. Backend Validation Delegation
```text
#runSubagent Backend Analysis:
- Roslyn type analysis for {path}
- Check breaking changes in public APIs
- Validate Wolverine CQRS patterns
- Analyze DI configuration

Return ONLY: type_errors + breaking_changes + pattern_violations
```

### 3. Frontend Validation Delegation
```text
#runSubagent Frontend Analysis:
- TypeScript strict mode validation
- i18n key coverage (zero hardcoded strings)
- Accessibility pre-check (WCAG 2.1)
- Component structure validation

Return ONLY: violations + fix_suggestions + accessibility_score
```

### 4. Knowledge Query Delegation
```text
#runSubagent @KB{Expert}:
[Specific question about patterns/best practices]
Return: code example + source DocID + brief explanation
```

---

## Response Format Contract

### Subagent responses MUST:
1. Be **concise** (max 500 tokens for KB queries)
2. Include **actionable items only**
3. **Cite sources** (DocID references)
4. Follow **requested format** exactly

### Response Template:
```
[Requested content: code/analysis/findings]

📚 Source: [DocID] | Status: [pass/fail/warnings]
```

---

## Token Savings Reference

| Task Type | Without runSubagent | With runSubagent | Savings |
|-----------|---------------------|------------------|---------|
| KB article lookup | 3000 tokens | 300 tokens | 90% |
| Multi-file security scan | 8000 tokens | 1500 tokens | 81% |
| Full validation suite | 12000 tokens | 2500 tokens | 79% |
| Code generation | 5000 tokens | 2000 tokens | 60% |

---

## Integration with Quality Gates

### Pre-Commit
```text
#runSubagent Pre-commit validation:
- Type checking (TS/Roslyn)
- Lint validation
- Security quick scan
Return: pass/fail + blockers
```

### Pre-PR
```text
#runSubagent PR readiness:
- Full test suite status
- Coverage delta
- Breaking change detection
- Documentation completeness
Return: ready/not-ready + required_actions
```

### Pre-Deploy
```text
#runSubagent Deployment validation:
- Container security scan
- Health check verification
- Configuration validation
Return: deploy_ready + risk_assessment
```

---

## Best Practices

### DO:
- ✅ Specify exact output format in prompt
- ✅ Request "ONLY" specific fields to limit response
- ✅ Use KB-Expert agents for pattern questions
- ✅ Chain subagents for complex workflows

### DON'T:
- ❌ Ask open-ended questions without format spec
- ❌ Request "everything you know about X"
- ❌ Use for simple single-tool operations
- ❌ Expect subagent to remember previous queries

---

## Future KB-Expert Agents (Planned)

| Agent | Domain | Priority |
|-------|--------|----------|
| `@KBSecurity` | OWASP, auth patterns | Phase 2 |
| `@KBDotNet` | .NET 10, C# 14 features | Phase 2 |
| `@KBInfra` | Docker, K8s, Aspire | Phase 2 |
| `@KBi18n` | Localization patterns | Phase 3 |

---

**Related Documents:**
- [GL-002] Subagent Delegation (general)
- [GL-006] Token Optimization Strategy
- [AGT-KB-001] @KBWolverine Agent
- [AGT-KB-002] @KBVue Agent

---

**Maintained by**: @SARAH  
**Size**: 2.5 KB
