---
docid: PRM-034
title: Bug Quick Lint Fix.Prompt
owner: @CopilotExpert
status: Active
created: 2026-01-08
---

---
docid: PRM-QBF-LINT
title: Quick Bug Fix - Linting and Formatting Violations
command: /bug-lint-fix
owner: @TechLead
status: Active
references: ["ADR-052", "KB-053", "INS-002"]
---

# /bug-lint-fix - Quick Linting Violation Fix

**Use when**: ESLint errors, StyleCop violations, formatting issues, unused imports

---

## Diagnostic MCP Chain

```bash
# 1. Get linting errors
get_errors filePath="[FILE_PATH]"

# 2. Analyze code structure
typescript-mcp/analyze_types filePath="[FILE_PATH]"

# 3. Validate fix
get_errors filePath="[FILE_PATH]"
```

---

## Execution Steps

### Step 1: Gather Context
Provide:
- **File**: Path to file with linting errors (e.g., `src/components/FormField.vue`)
- **Rule**: ESLint/StyleCop rule name (e.g., `vue/multi-word-component-names`)
- **Current**: Current error message

### Step 2: Identify Error Type

| Error Type | Fix Strategy | Time |
|-----------|--------------|------|
| **Unused imports** | Remove import statement | 1min |
| **Naming convention** | Rename to match convention | 2min |
| **Spacing/indentation** | Run formatter | 1min |
| **Missing semicolon** | Add semicolon | 1min |
| **Unused variables** | Remove or use variable | 2min |
| **Deprecated API** | Update to new API | 5min |

### Step 3: Apply Fix

**Fix Type 1: Unused Imports**
```typescript
// ❌ Linting error: 'unused-imports'
import { helpers } from './utils'; // Never used
import { getData } from './api';

export function process() {
  return getData();
}

// ✅ Fix: Remove unused import
import { getData } from './api';

export function process() {
  return getData();
}
```

**Fix Type 2: Component Naming**
```vue
<!-- ❌ Error: vue/multi-word-component-names -->
<!-- File: src/components/Button.vue -->

<!-- ✅ Fix: Rename to multi-word name -->
<!-- File: src/components/ButtonComponent.vue -->
```

**Fix Type 3: Unused Variables**
```typescript
// ❌ Error: '@typescript-eslint/no-unused-vars'
const unused = 'test';
const name = 'John';
console.log(name);

// ✅ Fix: Remove or use variable
const name = 'John';
console.log(name);
```

**Fix Type 4: Spacing and Indentation**
```typescript
// ❌ Formatting errors
const obj = {name:"John",age:25}

// ✅ Fix: Proper spacing and indentation
const obj = {
  name: 'John',
  age: 25
};
```

**Fix Type 5: Quote Style**
```typescript
// ❌ Error: quotes rule (wrong quote style)
const message = "Hello World"; // Double quotes

// ✅ Fix: Use consistent quotes (project uses single)
const message = 'Hello World'; // Single quotes
```

**Fix Type 6: Semicolons**
```typescript
// ❌ Error: no semicolon at end of statement
const value = 42

// ✅ Fix: Add semicolon
const value = 42;
```

### Step 4: Run Formatter (Optional)

For Vue/TypeScript:
```bash
# Auto-fix common linting issues
npm run lint -- --fix src/components/FormField.vue

# Or format with Prettier
npx prettier --write src/components/FormField.vue
```

For C#:
```bash
# Auto-format C# files
dotnet format backend/Domain/MyModule/Services/MyService.cs
```

### Step 5: Validate

```bash
# Check for remaining errors
get_errors filePath="[FILE_PATH]"

# Ensure no new errors introduced
# Ensure only formatting/style changed, not logic

# Run type check
typescript-mcp/validate_types filePath="[FILE_PATH]"
```

### Step 6: Document
Add to `.ai/knowledgebase/lessons.md`:
```markdown
## [Quick Fix] Linting Violation Prevention

**Pattern**: Run linter before committing; enable pre-commit hooks
**Files**: [list of files]
**Prevention**: 
  - Enable ESLint with `--fix` in pre-commit
  - Use Prettier for automatic formatting
  - Configure EditorConfig for consistent spacing
  - Treat new linting errors as CI blocking issues
**Tool**: typescript-mcp, ESLint, Prettier
```

---

## Common Linting Rules

### ESLint Rules (Frontend)

| Rule | Example | Fix |
|------|---------|-----|
| `no-unused-vars` | `const x = 5;` | Remove unused variable |
| `no-undef` | Use undefined variable | Import or define variable |
| `quotes` | Wrong quote style | Use consistent quotes |
| `semi` | Missing semicolon | Add semicolon |
| `comma-dangle` | Inconsistent commas | Add/remove trailing comma |
| `no-console` | `console.log()` | Remove debug logs |
| `no-var` | `var x = 1;` | Use `const` or `let` |

### Vue Rules

| Rule | Example | Fix |
|------|---------|-----|
| `vue/multi-word-component-names` | File: `Button.vue` | Rename: `ButtonComponent.vue` |
| `vue/no-unused-components` | Import unused component | Remove import |
| `vue/no-template-shadow` | Duplicate var in template | Rename one |
| `vue/html-indent` | Wrong indentation | Fix indentation |

### StyleCop Rules (.NET)

| Rule | Example | Fix |
|------|---------|-----|
| `SA1600` | Missing XML documentation | Add `///` comments |
| `SA1309` | Non-private field `_name` | Use correct naming |
| `SA1101` | Field not prefixed | Prefix with `this.` |
| `SA1503` | Opening brace on wrong line | Move brace to line end |

---

## Pre-Commit Hooks Setup

Configure automatic linting before commit:

**`.husky/pre-commit`**:
```bash
#!/bin/sh
. "$(dirname "$0")/_/husky.sh"

echo "Running pre-commit linting..."

# Lint staged files
npx lint-staged

# Check format
npm run lint -- --no-fix --max-warnings=0

exit $?
```

**`.lintstagedrc.json`**:
```json
{
  "src/**/*.{vue,ts,js}": [
    "eslint --fix",
    "prettier --write"
  ],
  "backend/**/*.cs": [
    "dotnet format"
  ]
}
```

---

## Quick Checklist

- [ ] Identified linting rule violation
- [ ] Applied appropriate fix
- [ ] No logic changes (only formatting/naming)
- [ ] No new errors introduced
- [ ] Type check passes (if TypeScript)
- [ ] Tests still pass
- [ ] Commit message mentions rule fixed
- [ ] Lesson documented in lessons.md

---

## Anti-Patterns to Avoid

❌ **Don't disable linting rules without reason**:
```typescript
// eslint-disable-next-line - WRONG! Hides real issues
console.log('debug');
```

✅ **Do disable only when justified**:
```typescript
// eslint-disable-next-line no-console -- Intentional logging for debugging
console.log('debug');
```

❌ **Don't use `// @ts-ignore` to silence errors**:
```typescript
// @ts-ignore
const value: string = maybeNull; // Hides type issue!
```

✅ **Do fix the underlying type issue**:
```typescript
const value: string = maybeNull ?? 'default'; // Fixes type properly
```

---

## See Also

- [ADR-052] MCP-Enhanced Bugfixing Workflow
- [KB-053] TypeScript MCP Integration
- [INS-002] Frontend Essentials - Linting Standards
- ESLint: https://eslint.org/
- Prettier: https://prettier.io/
- StyleCop: https://github.com/DotNetAnalyzers/StyleCopAnalyzers
