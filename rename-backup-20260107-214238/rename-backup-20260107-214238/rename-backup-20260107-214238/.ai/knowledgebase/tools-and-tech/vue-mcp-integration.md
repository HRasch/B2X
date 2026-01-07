---
docid: KB-054
title: Vue MCP Integration Guide
owner: @CopilotExpert
status: Active
---

# Vue MCP Integration Guide

**DocID**: `KB-054`  
**Last Updated**: 6. Januar 2026  
**Owner**: @CopilotExpert

## Overview

The Vue MCP server provides AI-assisted analysis tools for Vue 3 development, including component analysis, i18n validation, responsive design checks, Pinia store analysis, and accessibility validation.

## MCP Server Configuration

**Location**: `tools/VueMCP/`  
**MCP Config**: `.vscode/mcp.json` (enabled by default)

```json
{
  "mcpServers": {
    "vue-mcp": {
      "command": "node",
      "args": ["tools/VueMCP/dist/index.js"],
      "env": { "NODE_ENV": "production" },
      "disabled": false
    }
  }
}
```

## Available Tools

### 1. analyze_vue_component
**Purpose**: Analyze Vue SFC structure and composition  
**Use Cases**:
- Study component patterns before creating new components
- Validate component structure
- Extract composition API usage
- Analyze reactive variables, computed properties, methods

**Example**:
```typescript
{
  filePath: "src/components/UserProfile.vue",
  workspacePath: "frontend/Store"
}
```

**Returns**:
- Script setup analysis (reactive vars, computed, methods)
- Template structure with directives
- Component props, emits, and slots
- Style scoped information

---

### 2. find_component_usage
**Purpose**: Find all usages of a Vue component across the project  
**Use Cases**:
- Check component dependencies before refactoring
- Track component usage patterns
- Identify orphaned components

**Example**:
```typescript
{
  componentName: "UserProfile",
  workspacePath: "frontend/Store"
}
```

**Returns**:
- List of files using the component
- Import statements
- Component registration locations

---

### 3. validate_i18n_keys
**Purpose**: Validate i18n key usage and detect hardcoded strings  
**Use Cases**:
- **MANDATORY pre-commit check** - enforce zero hardcoded strings
- Validate translation key patterns
- Check for missing translations across locales
- Ensure [GL-042] i18n strategy compliance

**Example**:
```typescript
{
  workspacePath: "frontend/Store",
  componentPath: "src/components/LoginForm.vue" // optional
}
```

**Returns**:
- Hardcoded strings detected (MUST be zero)
- i18n key usage patterns
- Missing translation keys
- Locale coverage report

**Policy**:
- ❌ **ZERO hardcoded strings allowed** in any Vue component
- ✅ All user-facing text must use `$t()` or `useI18n()`
- ✅ Run before every commit
- ✅ PR blocked if hardcoded strings detected

---

### 4. check_responsive_design
**Purpose**: Analyze responsive design implementation with Tailwind CSS  
**Use Cases**:
- Validate breakpoint usage
- Ensure mobile-first design patterns
- Check responsive class consistency

**Example**:
```typescript
{
  filePath: "src/components/ProductCard.vue",
  workspacePath: "frontend/Store"
}
```

**Returns**:
- Tailwind responsive classes used (sm:, md:, lg:, xl:, 2xl:)
- Breakpoint consistency analysis
- Mobile-first pattern validation
- Recommendations for improvement

**Breakpoints**:
- `sm`: 640px (mobile landscape)
- `md`: 768px (tablet)
- `lg`: 1024px (desktop)
- `xl`: 1280px (large desktop)
- `2xl`: 1536px (extra large)

---

### 5. analyze_pinia_store
**Purpose**: Analyze Pinia store structure and patterns  
**Use Cases**:
- Validate store organization
- Check composition patterns
- Ensure state management best practices

**Example**:
```typescript
{
  filePath: "src/stores/user.ts",
  workspacePath: "frontend/Store"
}
```

**Returns**:
- State structure analysis
- Getters and computed properties
- Actions and mutations
- Store composition patterns
- Best practice recommendations

---

### 6. check_accessibility
**Purpose**: Run axe-core accessibility audits  
**Use Cases**:
- WCAG compliance validation
- Automated accessibility scoring
- Violation detection before PR

**Example**:
```typescript
{
  filePath: "src/components/LoginForm.vue",
  workspacePath: "frontend/Store"
}
```

**Returns**:
- WCAG violations with severity
- Accessibility score
- Remediation suggestions
- ARIA attribute validation

**Target**: WCAG 2.1 Level AAA compliance

---

## Development Workflows

### Component Creation Workflow

```bash
# Step 1: Study similar components
vue-mcp/analyze_vue_component filePath="src/components/ExistingComponent.vue"

# Step 2: Create new component
# [Implement component code]

# Step 3: Validate structure
vue-mcp/analyze_vue_component filePath="src/components/NewComponent.vue"

# Step 4: Check dependencies
vue-mcp/find_component_usage componentName="NewComponent"

# Step 5: Validate i18n (MANDATORY)
vue-mcp/validate_i18n_keys componentPath="src/components/NewComponent.vue"
# MUST return zero hardcoded strings

# Step 6: Check responsive design
vue-mcp/check_responsive_design filePath="src/components/NewComponent.vue"

# Step 7: Accessibility validation
vue-mcp/check_accessibility filePath="src/components/NewComponent.vue"
```

---

### Pre-Commit Checklist

Before committing any Vue component changes:

```bash
# 1. i18n validation (CRITICAL)
vue-mcp/validate_i18n_keys componentPath="[your-component].vue"
# ✅ Must return: "No hardcoded strings found"

# 2. Responsive design
vue-mcp/check_responsive_design filePath="[your-component].vue"
# ✅ Must validate: Mobile-first patterns

# 3. Accessibility
vue-mcp/check_accessibility filePath="[your-component].vue"
# ✅ Must pass: Zero critical violations

# 4. Component structure
vue-mcp/analyze_vue_component filePath="[your-component].vue"
# ✅ Must validate: Proper composition API usage
```

---

### Refactoring Workflow

```bash
# Step 1: Find all usages
vue-mcp/find_component_usage componentName="ComponentToRefactor"

# Step 2: Analyze current structure
vue-mcp/analyze_vue_component filePath="src/components/ComponentToRefactor.vue"

# Step 3: Make changes
# [Refactor code]

# Step 4: Re-validate
vue-mcp/analyze_vue_component filePath="src/components/ComponentToRefactor.vue"

# Step 5: Check for regressions
vue-mcp/validate_i18n_keys componentPath="src/components/ComponentToRefactor.vue"
vue-mcp/check_responsive_design filePath="src/components/ComponentToRefactor.vue"
vue-mcp/check_accessibility filePath="src/components/ComponentToRefactor.vue"
```

---

### Pinia Store Development

```bash
# Step 1: Analyze existing store patterns
vue-mcp/analyze_pinia_store filePath="src/stores/existingStore.ts"

# Step 2: Create new store
# [Implement store]

# Step 3: Validate structure
vue-mcp/analyze_pinia_store filePath="src/stores/newStore.ts"

# Step 4: Check i18n in store (if applicable)
vue-mcp/validate_i18n_keys workspacePath="frontend/Store"
```

---

## Integration with Development Process

### Code Review Integration (PRM-002)

**Automated gates before human review**:
1. `vue-mcp/validate_i18n_keys` → Must pass (zero hardcoded strings)
2. `vue-mcp/check_responsive_design` → Must validate mobile-first
3. `vue-mcp/check_accessibility` → Must pass critical violations
4. `vue-mcp/analyze_vue_component` → Structure must be valid

**Policy**: MCP validation must pass GREEN before PR approval

---

### i18n Strategy Compliance (GL-042)

The Vue MCP server enforces [GL-042] token-optimized i18n strategy:

```typescript
// ❌ WRONG - Hardcoded string
<template>
  <h1>Welcome to B2X</h1>
</template>

// ✅ CORRECT - Translation key
<template>
  <h1>{{ $t('welcome.title') }}</h1>
</template>
```

**Validation Command**:
```bash
vue-mcp/validate_i18n_keys workspacePath="frontend/Store"
```

**Supported Languages**: en, de, fr, es, it, pt, nl, pl  
**Source of Truth**: English (`en.json`)

---

### Responsive Design Patterns

```vue
<!-- ✅ CORRECT - Mobile-first approach -->
<div class="w-full md:w-1/2 lg:w-1/3">
  <!-- Mobile: 100% width, Tablet: 50%, Desktop: 33% -->
</div>

<!-- ✅ CORRECT - Responsive spacing -->
<div class="p-4 md:p-6 lg:p-8">
  <!-- Padding scales with breakpoints -->
</div>

<!-- ❌ WRONG - Desktop-first -->
<div class="w-1/3 md:w-1/2 sm:w-full">
  <!-- Should be mobile-first: w-full md:w-1/2 lg:w-1/3 -->
</div>
```

**Validation Command**:
```bash
vue-mcp/check_responsive_design filePath="src/components/MyComponent.vue"
```

---

## Common Issues and Solutions

### Issue: Hardcoded strings detected
```bash
vue-mcp/validate_i18n_keys componentPath="src/components/LoginForm.vue"
# Result: "3 hardcoded strings found"
```

**Solution**:
1. Extract hardcoded strings to locale files
2. Replace with `$t()` calls
3. Re-run validation until zero hardcoded strings

---

### Issue: Accessibility violations
```bash
vue-mcp/check_accessibility filePath="src/components/LoginForm.vue"
# Result: "5 critical violations"
```

**Solution**:
1. Review violation details
2. Add ARIA labels, roles, and semantic HTML
3. Re-run until zero critical violations
4. See [KB-027] for email/form accessibility patterns

---

### Issue: Non-mobile-first responsive classes
```bash
vue-mcp/check_responsive_design filePath="src/components/ProductCard.vue"
# Result: "Desktop-first pattern detected"
```

**Solution**:
1. Reorder Tailwind classes: smallest breakpoint first
2. Use base classes without prefix for mobile
3. Add breakpoint prefixes for larger screens
4. Re-validate

---

## Performance Considerations

### Targeted Analysis
```typescript
// ✅ GOOD - Analyze specific component
vue-mcp/analyze_vue_component filePath="src/components/UserProfile.vue"

// ❌ AVOID - Full workspace scan (slow)
// Only use full scan when necessary
```

### Incremental Validation
```bash
# During development - validate individual files
vue-mcp/validate_i18n_keys componentPath="src/components/NewComponent.vue"

# Pre-PR - validate all changes
vue-mcp/validate_i18n_keys workspacePath="frontend/Store"
```

---

## Best Practices

1. **Run MCP tools early and often** - Don't wait until PR time
2. **Fix issues immediately** - Smaller corrections are easier
3. **Use MCP for discovery** - Study patterns before implementing
4. **Automate with scripts** - Create npm scripts for common MCP chains
5. **Document exceptions** - If MCP flags false positives, document why

---

## Related Documentation

- [GL-042] - Token-Optimized i18n Strategy
- [KB-053] - TypeScript MCP Integration Guide
- [KB-055] - Security MCP Best Practices
- [KB-056] - HTML/CSS MCP Usage Guide
- [ADR-042] - Internationalization Strategy

---

## Troubleshooting

### MCP Server Not Responding
```bash
# Check MCP server status
cd tools/VueMCP
npm run build
npm start

# Verify in .vscode/mcp.json
# "disabled": false
```

### False Positives in i18n Validation
```typescript
// If code comments or console.logs flagged as hardcoded strings:
// Document in component with special comment
// <!-- MCP-EXCEPTION: Console logs for debugging only -->
console.log('Debug info')
```

### Performance Issues
```bash
# Clear MCP cache (if implemented)
# Restart VS Code
# Use targeted file paths instead of full workspace scans
```

---

**Maintained by**: @CopilotExpert  
**Last Review**: 6. Januar 2026  
**Next Review**: 6. Februar 2026
