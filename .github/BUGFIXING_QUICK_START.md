---
docid: DOC-012
title: BUGFIXING_QUICK_START
owner: @CopilotExpert
status: Active
created: 2026-01-08
---

---
docid: WF-QUICK-BUGFIX
title: MCP-Enhanced Bugfixing Quick Start
owner: @TechLead
status: Active
date: 2026-01-07
---

# MCP-Enhanced Bugfixing Quick Start

**Three major enhancements just deployed** to make bugfixing faster and more effective.

---

## ✅ What Just Went Live

### 1. ADR-052: MCP-Enhanced Bugfixing Framework
**File**: [.ai/decisions/ADR-052-mcp-enhanced-bugfixing.md](../../.ai/decisions/ADR-052-mcp-enhanced-bugfixing.md)

Strategic framework integrating MCP tools into bugfixing workflow:
- Diagnostic chains by bug category
- Auto-lessons-learned capture
- Enhanced `/bug-analysis` with MCP pre-checks
- Success metrics (target 30min → 10min RCA)

---

### 2. Five Quick Bug-Fix Prompts
**Location**: `.github/prompts/bug-quick-*.prompt.md`

| Prompt | Use When | Time | DocID |
|--------|----------|------|-------|
| `/bug-null-check` | Null/undefined reference errors | 5min | PRM-QBF-NULL |
| `/bug-async-race` | Missing awaits, race conditions | 5min | PRM-QBF-ASYNC |
| `/bug-type-mismatch` | TypeScript type errors | 5min | PRM-QBF-TYPE |
| `/bug-i18n-missing` | Missing translation keys | 3min | PRM-QBF-I18N |
| `/bug-lint-fix` | ESLint/StyleCop violations | 2min | PRM-QBF-LINT |

**How to use**:
```
@TechLead: /bug-null-check
File: src/components/MyComponent.vue
Variable: product.images
Line: 45
```

---

### 3. Chrome DevTools MCP Enabled
**File**: [.vscode/mcp.json](../../.vscode/mcp.json)

✅ **Now enabled** (previously disabled) for:
- Runtime debugging with browser console
- Network request inspection
- DOM state visualization
- Performance profiling (Lighthouse)
- Visual regression detection

**Use for**: `/debug-runtime` interactive debugging sessions

---

## 🎯 Quick Usage Examples

### Example 1: Null Reference Error
```
Error: Cannot read property 'images' of undefined

→ Use: @TechLead: /bug-null-check
→ Time: ~5 minutes
→ MCP Chain: typescript-mcp → type analysis & symbol tracing
→ Fix: Add optional chaining guards (product?.images?.[0])
```

### Example 2: Async Race Condition
```
Error: Data sometimes undefined after fetch

→ Use: @TechLead: /bug-async-race
→ Time: ~5 minutes
→ MCP Chain: typescript-mcp → find async functions & missing awaits
→ Fix: Add await, loading state guards, AbortController
```

### Example 3: Type Mismatch
```
Error: Type 'string | null' is not assignable to type 'string'

→ Use: @TechLead: /bug-type-mismatch
→ Time: ~5 minutes
→ MCP Chain: typescript-mcp → type guard analysis
→ Fix: Add type guard or optional coalescing
```

### Example 4: Missing Translation
```
Error: Translation key 'common.save_button' not found

→ Use: @TechLead: /bug-i18n-missing
→ Time: ~3 minutes
→ MCP Chain: typescript-mcp → validate keys in all locales
→ Fix: Add key to en.json, de.json, fr.json, etc.
```

### Example 5: Linting Violation
```
Error: ESLint rule 'no-unused-vars' violation

→ Use: @TechLead: /bug-lint-fix
→ Time: ~2 minutes
→ MCP Chain: get_errors → identify violation type
→ Fix: Run eslint --fix or apply manual formatting
```

---

## 📚 Documentation Structure

```
.ai/decisions/
└── ADR-052-mcp-enhanced-bugfixing.md          ← Strategic framework
    
.github/prompts/
├── bug-quick-starter.prompt.md                 ← Overview of all 5
├── bug-quick-null-check.prompt.md              ← Detailed guide
├── bug-quick-async-race.prompt.md              ← Detailed guide
├── bug-quick-type-mismatch.prompt.md           ← Detailed guide
├── bug-quick-i18n-missing.prompt.md            ← Detailed guide
└── bug-quick-lint-fix.prompt.md                ← Detailed guide

.ai/DOCUMENT_REGISTRY.md
└── Updated with ADR-052 and 6 new prompt DocIDs
```

---

## 🔗 Cross-References

| Document | Purpose |
|----------|---------|
| ADR-052 | Strategic framework for MCP-enhanced bugfixing |
| PRM-QUICK-BUG | Overview of all 5 quick-fix prompts |
| PRM-QBF-NULL through PRM-QBF-LINT | Individual prompt guides |
| PRM-008 | `/bug-analysis` prompt (enhanced with MCP) |
| PRM-022 | `/auto-lessons-learned` (captures patterns) |
| KB-053 | TypeScript MCP Integration guide |
| KB-055 | Security MCP Best Practices |
| KB-064 | Chrome DevTools MCP Server |

---

## 🚀 Getting Started (3 Steps)

### Step 1: Learn the Framework
Read: [ADR-052](../../.ai/decisions/ADR-052-mcp-enhanced-bugfixing.md) (5 min)

Understand:
- Bug category → MCP chain mapping
- When to use `/bug-quick-*` vs `/bug-analysis`
- How lessons are captured

### Step 2: Pick Your Quick Prompt
When you encounter a bug:
1. Identify bug category (null ref? async? type error? i18n? lint?)
2. Choose corresponding `/bug-quick-*` prompt
3. Follow the structured steps in that prompt

### Step 3: Document Your Learning
After fixing:
1. Add to `.ai/knowledgebase/lessons.md`
2. Include: root cause, fix pattern, prevention rule
3. Reference the MCP tools you used

---

## 📊 Success Metrics (After This Implementation)

| Metric | Before | Target | Tool |
|--------|--------|--------|------|
| **Time to RCA** | ~30 min | ~10 min | MCP diagnostics |
| **Duplicate bugs** | 20% recurrence | 6% recurrence | Lessons.md reference |
| **Lessons captured** | ~20% of bugs | >80% of bugs | Auto-generation |
| **Fix validation** | Manual | 100% automated | Post-MCP checks |

---

## ⚡ Pro Tips

### Tip 1: Use MCP Chains
Don't try to fix blindly. Let MCP analyze first:
```
typescript-mcp/analyze_types → identify issue → apply fix → validate
```

### Tip 2: Document Patterns
Similar bugs waste time. After fixing, document in lessons.md so the next developer learns:
```markdown
## [Pattern Name]
- **Root Cause**: [what went wrong]
- **Prevention**: [how to avoid]
- **Files**: [where it happened]
- **MCP Tools**: [what helped]
```

### Tip 3: Combine Prompts
Complex bugs may need both `/bug-analysis` AND a `/bug-quick-*`:
1. Use `/bug-analysis` to understand root cause
2. Use appropriate `/bug-quick-*` to apply fix
3. Run validation chains from `/bug-quick-*`

### Tip 4: Chrome DevTools MCP for Interactive Debugging
When `/bug-quick-*` needs runtime context:
```bash
@TechLead: /debug-runtime
Component: ProductCard.vue
Issue: State undefined after prop change
# Chrome DevTools MCP captures:
# - console errors
# - network requests
# - DOM state
# - performance metrics
```

---

## 🎓 Learning Resources

### Quick Reference
- [bug-quick-starter.prompt.md](../../.github/prompts/bug-quick-starter.prompt.md) - Overview of all 5 prompts

### Detailed Guides
- [bug-quick-null-check.prompt.md](../../.github/prompts/bug-quick-null-check.prompt.md)
- [bug-quick-async-race.prompt.md](../../.github/prompts/bug-quick-async-race.prompt.md)
- [bug-quick-type-mismatch.prompt.md](../../.github/prompts/bug-quick-type-mismatch.prompt.md)
- [bug-quick-i18n-missing.prompt.md](../../.github/prompts/bug-quick-i18n-missing.prompt.md)
- [bug-quick-lint-fix.prompt.md](../../.github/prompts/bug-quick-lint-fix.prompt.md)

### MCP Integration
- [KB-053] TypeScript MCP Integration
- [KB-055] Security MCP Best Practices
- [KB-064] Chrome DevTools MCP Server

---

## ✅ Deployment Checklist

- [x] ADR-052 created and documented
- [x] 5 quick-fix prompts created with detailed guides
- [x] Chrome DevTools MCP enabled in `.vscode/mcp.json`
- [x] DOCUMENT_REGISTRY.md updated with new DocIDs
- [x] This quick-start guide created
- [x] MCP chains validated
- [x] Documentation cross-referenced

---

## 📞 Questions?

| Question | Answer | Reference |
|----------|--------|-----------|
| When should I use `/bug-quick-*`? | When bug pattern is clear (null, async, type, i18n, lint) | [ADR-052](../../.ai/decisions/ADR-052-mcp-enhanced-bugfixing.md) |
| What if I don't know the bug category? | Use `/bug-analysis` first to identify root cause | [PRM-008] |
| How do I capture lessons learned? | Add to `.ai/knowledgebase/lessons.md` or use `/auto-lessons-learned` | [PRM-022] |
| Can I debug at runtime? | Yes! Enable Chrome DevTools MCP in `/debug-runtime` | [KB-064] |
| Where are all the prompts? | `.github/prompts/bug-quick-*.prompt.md` | [DOCUMENT_REGISTRY.md] |

---

**Status**: 🟢 DEPLOYED & READY TO USE  
**Date**: 2026-01-07  
**Owner**: @TechLead  
**Next Review**: 2026-02-07 (measure impact)
