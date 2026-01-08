---
docid: PRM-036
title: Bug Quick Starter.Prompt
owner: @CopilotExpert
status: Active
created: 2026-01-08
---

---
docid: PRM-QUICK-BUG
title: Quick Bug-Fix Starter Prompt Set
owner: @TechLead
status: Active
---

# Quick Bug-Fix Starter Prompts

This document outlines 5 new quick-fix prompts designed for common bug patterns.
See ADR-052 for rationale and workflow integration.

---

## Overview

These prompts accelerate bugfixing for common patterns:

| Prompt | Pattern | MCP Chain | Time |
|--------|---------|-----------|------|
| `/bug-null-check` | Null reference errors | typescript-mcp | 5min |
| `/bug-async-race` | Missing awaits, race conditions | typescript-mcp | 5min |
| `/bug-type-mismatch` | TypeScript type errors | typescript-mcp | 5min |
| `/bug-i18n-missing` | Missing translation keys | typescript-mcp | 3min |
| `/bug-lint-fix` | ESLint violations | typescript-mcp | 2min |

---

## 1. `/bug-null-check` 

**When**: "Cannot read property X of undefined" or null reference errors

**What it does**:
```bash
# 1. Run type analysis on the file
typescript-mcp/analyze_types filePath="[path/to/file]"

# 2. Find symbol usages
typescript-mcp/find_symbol_usages symbol="[variableName]"

# 3. Generate defensive checks
# - Add v-if guards (Vue)
# - Add optional chaining (?.)
# - Add nullish coalescing (??)
# - Add type guards (is X defined)

# 4. Validate fix
typescript-mcp/validate_types filePath="[path/to/file]"
```

**Example Execution**:
```
@TechLead: /bug-null-check
File: src/components/ProductCard.vue
Variable: product.images[0].url
Error Location: Line 45
```

**Output**:
- ✅ `v-if="product?.images?.[0]?.url"` guard added
- ✅ Type safety restored
- ✅ Test updated

---

## 2. `/bug-async-race`

**When**: Data stale/undefined after async operation, race conditions

**What it does**:
```bash
# 1. Analyze async patterns
typescript-mcp/analyze_types filePath="[path/to/file]"

# 2. Find missing awaits
typescript-mcp/find_symbol_usages symbol="[asyncFunction]"

# 3. Detect race conditions
# - Check Promise.all usage
# - Validate await placement
# - Add abort signal handling

# 4. Generate fixes
# - Add await where missing
# - Wrap in try-catch
# - Add loading state guards

# 5. Validate
typescript-mcp/validate_types filePath="[path/to/file]"
```

**Example Execution**:
```
@TechLead: /bug-async-race
Component: src/components/SearchResults.vue
Function: fetchResults()
Symptom: Results sometimes undefined
```

**Output**:
- ✅ Added `const data = await fetchResults()`
- ✅ Added `v-if="loading === false"` guard
- ✅ Added AbortController for cancellation

---

## 3. `/bug-type-mismatch`

**When**: TypeScript type errors, type incompatibilities

**What it does**:
```bash
# 1. Get type error details
typescript-mcp/analyze_types filePath="[path/to/file]"

# 2. Analyze context
# - Inspect variable declarations
# - Check function signatures
# - Review type annotations

# 3. Identify type source
typescript-mcp/find_symbol_usages symbol="[typeName]"

# 4. Generate fix options
# - Type assertion (as Type)
# - Type guard (instanceof, type predicate)
# - Add proper typing
# - Update interface

# 5. Validate
typescript-mcp/validate_types filePath="[path/to/file]"
```

**Example Execution**:
```
@TechLead: /bug-type-mismatch
File: src/utils/parser.ts
Error: Type 'string | null' is not assignable to type 'string'
Line: 25
```

**Output**:
- ✅ Type guard added: `if (value !== null)`
- ✅ Type safety restored
- ✅ No type assertions (unsafe casts avoided)

---

## 4. `/bug-i18n-missing`

**When**: Translation keys not found, undefined i18n strings

**What it does**:
```bash
# 1. Validate translation keys
typescript-mcp/find_symbol_usages symbol="[i18nKey]"

# 2. Check locale files
# - en.json - key exists?
# - de.json - key exists?
# - fr.json - key exists? (all supported locales)

# 3. If missing, add to all locales
# - Find similar keys for context
# - Generate translation template
# - Mark for translator: [i18n-todo]

# 4. Update component
# - Link to correct key
# - Add default fallback

# 5. Validate
typescript-mcp/analyze_types filePath="[localesDir]"
```

**Example Execution**:
```
@TechLead: /bug-i18n-missing
Key: common.save_button
Languages: en, de, fr, es
```

**Output**:
- ✅ Added to `en.json`: `"save_button": "Save"`
- ✅ Added to `de.json`: `"save_button": "[i18n-todo]"`
- ✅ Added to `fr.json`: `"save_button": "[i18n-todo]"`
- ✅ Component references correct key

---

## 5. `/bug-lint-fix`

**When**: ESLint/StyleCop violations, formatting issues

**What it does**:
```bash
# 1. Get linting error details
get_errors filePath="[path/to/file]"

# 2. Identify rule violations
typescript-mcp/analyze_types filePath="[path/to/file]"

# 3. Apply auto-fixes
# - Run eslint --fix (Vue/TypeScript)
# - Run dotnet format (C#)
# - Remove unused imports
# - Fix indentation

# 4. Validate no new errors
get_errors filePath="[path/to/file]"

# 5. Review changes
# - Ensure no logic changes
# - Only formatting/cleanup
```

**Example Execution**:
```
@TechLead: /bug-lint-fix
File: src/components/FormField.vue
Rule: vue/multi-word-component-names
```

**Output**:
- ✅ File renamed to `FormFieldComponent.vue`
- ✅ All imports updated
- ✅ No linting violations

---

## Implementation Notes

### Pre-Flight Checks
Before running any `/bug-quick-*`:
1. ✅ Check `lessons.md` for similar issues
2. ✅ Verify bug hasn't been fixed before
3. ✅ Understand root cause category

### Post-Fix Validation
After running fix:
1. ✅ Run `get_errors` to verify no new issues
2. ✅ Run relevant test suite
3. ✅ Update `.ai/knowledgebase/lessons.md` with pattern

### Integration with `/bug-analysis`
- Use `/bug-quick-*` when bug pattern is clear
- Use `/bug-analysis` when root cause is unknown
- Use `/bug-analysis` first to identify bug category
- Then apply appropriate `/bug-quick-*`

---

## Prompt Templates

Create these files in `.github/prompts/`:

- `bug-quick-null-check.prompt.md`
- `bug-quick-async-race.prompt.md`
- `bug-quick-type-mismatch.prompt.md`
- `bug-quick-i18n-missing.prompt.md`
- `bug-quick-lint-fix.prompt.md`

Each inherits from this base pattern and adds specific MCP chains.

---

## Metrics & Success Criteria

| Metric | Target | Current |
|--------|--------|---------|
| Time to fix (null checks) | 5min | ~20min |
| Time to fix (async race) | 5min | ~30min |
| First-time fix rate | 90% | ~70% |
| Lessons captured | 80% | ~20% |

---

## References

- ADR-052: MCP-Enhanced Bugfixing Workflow
- PRM-008: Bug Analysis Prompt
- PRM-022: Auto-Lessons-Learned Prompt
- KB-053: TypeScript MCP Integration
- KB-055: Security MCP Best Practices
