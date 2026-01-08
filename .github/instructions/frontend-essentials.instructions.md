---
docid: INS-011
title: Frontend Essentials.Instructions
owner: @CopilotExpert
status: Active
created: 2026-01-08
---

---
applyTo: "src/components/**,src/pages/**,src/hooks/**,src/ui/**,**/frontend/**"
---

# Frontend Development Instructions (Essentials)

## Component Design
- Use functional components with hooks
- Keep components small and focused (single responsibility)
- Extract reusable logic into custom hooks
- Use proper TypeScript types for props

## State Management
- Use local state for component-specific data
- Use context/global state sparingly
- Avoid prop drilling beyond 2 levels
- Implement proper loading and error states

## Styling
- Follow project's styling conventions
- Use consistent spacing and sizing
- Ensure responsive design (mobile-first)
- Maintain accessibility standards (WCAG)

## Performance
- Implement lazy loading for routes/components
- Memoize expensive computations
- Optimize re-renders with React.memo/useMemo
- Use proper image optimization

## UX
- Provide immediate feedback on user actions
- Handle loading states gracefully
- Display meaningful error messages
- Implement proper form validation

## i18n (MANDATORY)
- **ZERO hardcoded strings** - all text must use translation keys
- Use `$t()` in templates or `useI18n()` in scripts
- Key pattern: `module.section.key` (e.g., `auth.login.title`)
- English (`en`) is source of truth
- Supported languages: en, de, fr, es, it, pt, nl, pl

## TypeScript & Linting (Quality Gate)
- Enforce `compilerOptions.strict = true` across frontend projects
- Use ESLint with TypeScript rules; run `eslint --max-warnings=0` in CI
- Maintain a shared ESLint config in `frontend/.eslintrc.js`

## Formatting & Pre-commit Hooks
- Add Prettier config and Husky pre-commit hooks
- Run `npm run lint --fix` and `npm test` on pre-commit
- Provide `npm run format` commands for local use
- **MANDATORY**: Run TypeScript MCP validation before editing Vue/TypeScript files to ensure type safety and reduce iterations

## Visual Regression Policy
- PRs run focused visual smoke tests; full visual suite runs nightly
- Baseline updates must reference a review comment and acceptance criteria
- Document baseline update procedure and acceptable thresholds

## Control Evidence
- Publish lint and visual test outputs to PR checks
- Include instructions for local quick-checks (`scripts/run-local-checks.sh`)
- Include quick accessibility checks in CI for critical pages
- Document Lighthouse/perf thresholds for release

## MCP Tools

For detailed frontend-specific MCP tools documentation:

```
kb-mcp/search_knowledge_base
  query: "TypeScript MCP" OR "Vue MCP" OR "Accessibility"
```

See [KB-053] TypeScript MCP, [KB-054] Vue MCP, [KB-056] HTML/CSS MCP

## Large File Editing Strategy ([GL-043])

When editing large frontend files (>200 lines), use the Multi-Language Fragment Editing approach with TypeScript MCP and Vue MCP integration:

### Pre-Edit Analysis
```bash
# TypeScript analysis
typescript-mcp/analyze_types workspacePath="frontend/src"

# Vue component analysis
vue-mcp/analyze_vue_component filePath="src/components/Component.vue"

# i18n validation
vue-mcp/validate_i18n_keys componentPath="src/components/Component.vue"
```

### Fragment-Based Editing Patterns
```typescript
// Fragment: React component method (75% token savings)
const handleSubmit = async (formData: FormData) => {
  try {
    const response = await apiClient.post('/orders', formData);
    setOrders(prev => [...prev, response.data]);
  } catch (error) {
    setError('Submission failed');
  }
};
```

```vue
<!-- Fragment: Vue template section (78% token savings) -->
<template>
  <div class="order-form">
    <form @submit.prevent="handleSubmit">
      <!-- Edit only form fields -->
    </form>
  </div>
</template>
```

**TypeScript MCP Workflows**:
```bash
# 1. Type safety validation
typescript-mcp/analyze_types workspacePath="frontend/Store" filePath="src/components/ProductCard.vue"

# 2. Fragment refactoring
typescript-mcp/invoke_refactoring fileUri="Component.ts" name="source.addTypeAnnotation"
typescript-mcp/invoke_refactoring fileUri="Component.ts" name="source.convertImportFormat"

# 3. Interface validation
typescript-mcp/validate_interfaces workspacePath="frontend" filePath="types/api.ts"

# 4. Module dependency analysis
typescript-mcp/analyze_dependencies workspacePath="frontend/src"
```

**Vue MCP Integration**:
```bash
# Component structure validation
vue-mcp/analyze_vue_component filePath="src/components/LoginForm.vue"

# i18n coverage check (must return zero hardcoded strings)
vue-mcp/validate_i18n_keys componentPath="src/components/LoginForm.vue"

# Responsive design validation
vue-mcp/check_responsive_design filePath="src/components/LoginForm.vue"

# Accessibility pre-check
vue-mcp/check_accessibility filePath="src/components/LoginForm.vue"

# Composition API validation
vue-mcp/validate_composition_api filePath="src/components/ProductCard.vue"
```

### Quality Gates
- Always run `get_errors()` after edits
- Execute related tests with `runTests()`
- Use language-specific MCP validation
- Validate i18n compliance and accessibility

**Token Savings**: 75-78% vs. reading entire files | **Quality**: Type-safe validation with accessibility enforcement

## Temp-File Usage for Token Optimization

For large outputs during task execution (e.g., build logs, test results >1KB), save to temp files to reduce token consumption:

```bash
# Auto-save large build output
OUTPUT=$(npm run build 2>&1)
if [ $(echo "$OUTPUT" | wc -c) -gt 1024 ]; then
  bash scripts/temp-file-manager.sh create "$OUTPUT" log
else
  echo "$OUTPUT"
fi

# Reference in prompts/responses
"See temp file: .ai/temp/task-uuid.json (5KB saved)"
```

- Auto-cleanup after 1 hour or task completion.
- Complements [GL-006] token optimization strategy.

---

**Full documentation**: Use `kb-mcp/get_article` or search Knowledge Base  
**Size**: 1.9 KB (with temp-file additions)
