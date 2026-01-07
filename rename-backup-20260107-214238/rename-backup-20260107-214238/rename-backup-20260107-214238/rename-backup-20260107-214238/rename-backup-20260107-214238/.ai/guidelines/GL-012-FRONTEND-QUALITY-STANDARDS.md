---
docid: GL-012
title: Frontend Quality Standards
owner: "@TechLead"
status: Active
---

# GL-012: Frontend Quality Standards

**DocID**: `GL-012`  
**Last Updated**: 5. Januar 2026  
**Owner**: @TechLead

---

## Overview

This guideline establishes quality standards for all B2Connect frontend projects (Store, Admin, Management). These standards ensure consistent code quality, type safety, and maintainability across the codebase.

---

## 1. TypeScript Configuration Standards

### Required Compiler Options (Phase 1 - Active)

All frontend projects MUST enable these compiler options immediately:

```typescript
{
  "compilerOptions": {
    "strict": true,
    "noImplicitAny": true,
    "noImplicitReturns": true,
    "noFallthroughCasesInSwitch": true,
    "noUnusedLocals": true,
    "noUnusedParameters": true
  }
}
```

### Future Options (Phase 2 - After Type Cleanup)

These stricter options will be enabled after existing type issues are resolved:

```typescript
{
  "compilerOptions": {
    "exactOptionalPropertyTypes": true,  // Enable after interface cleanup
    "noUncheckedIndexedAccess": true     // Enable after array access patterns reviewed
  }
}
```

### Rationale

| Option | Purpose |
|--------|---------|
| `noImplicitAny` | Prevents implicit `any` types, ensuring explicit type annotations |
| `exactOptionalPropertyTypes` | Requires exact types for optional properties |
| `noImplicitReturns` | Ensures all code paths return a value |
| `noFallthroughCasesInSwitch` | Prevents fallthrough in switch statements |
| `noUncheckedIndexedAccess` | Adds `undefined` to indexed access results |

---

## 2. ESLint Rules

### Required Rules (Error Level)

```javascript
{
  rules: {
    '@typescript-eslint/no-explicit-any': 'error',
    '@typescript-eslint/prefer-nullish-coalescing': 'error',
    '@typescript-eslint/prefer-optional-chain': 'error',
    'vue/multi-word-component-names': 'off',
    '@typescript-eslint/no-unused-vars': ['warn', { argsIgnorePattern: '^_' }],
  }
}
```

### Migration Strategy

For existing code with `any` types:
1. Create a tracking issue for each file with `any` types
2. Gradually replace `any` with proper types
3. Use `// eslint-disable-next-line @typescript-eslint/no-explicit-any -- reason` for unavoidable cases
4. Document justification in code comments

---

## 3. Test Coverage Requirements

### Coverage Thresholds

All frontend projects MUST maintain these minimum thresholds:

| Metric | Minimum |
|--------|---------|
| Branches | 75% |
| Functions | 80% |
| Lines | 80% |
| Statements | 80% |

### Configuration

```typescript
// vitest.config.ts
{
  coverage: {
    thresholds: {
      global: {
        branches: 75,
        functions: 80,
        lines: 80,
        statements: 80,
      },
    },
  },
}
```

### Coverage Failures

- CI/CD pipeline MUST fail if coverage thresholds are not met
- Coverage reports are generated and uploaded as artifacts
- Use `/* istanbul ignore next */` sparingly with documented justification

---

## 4. Quality Check Scripts

All frontend projects MUST include these npm scripts:

```json
{
  "scripts": {
    "quality-check": "npm run type-check && npm run lint:check && npm run test:coverage",
    "pre-commit": "npm run quality-check",
    "type-check": "vue-tsc --noEmit",
    "lint": "eslint . --fix",
    "lint:check": "eslint .",
    "test:coverage": "vitest --coverage"
  }
}
```

### Usage

- Run `npm run quality-check` before committing
- CI/CD runs quality-check on all PRs
- Pre-commit hooks should execute quality-check

---

## 5. Bundle Size Monitoring

### Configuration

All projects include rollup-plugin-visualizer for bundle analysis:

```typescript
// vite.config.ts
import { visualizer } from 'rollup-plugin-visualizer';

export default defineConfig({
  plugins: [
    vue(),
    visualizer({
      filename: 'dist/bundle-analysis.html',
      gzipSize: true,
      brotliSize: true,
    }),
  ],
  build: {
    reportCompressedSize: true,
    chunkSizeWarningLimit: 1000, // 1MB warning threshold
  },
});
```

### Bundle Size Limits

| Chunk | Warning | Error |
|-------|---------|-------|
| Main bundle | 500 KB | 1 MB |
| Vendor chunks | 250 KB | 500 KB |
| Individual chunks | 100 KB | 250 KB |

---

## 6. Code Review Checklist

PRs affecting frontend code MUST verify:

- [ ] TypeScript types are correct (no implicit any)
- [ ] ESLint rules pass without disabled rules
- [ ] Test coverage meets thresholds
- [ ] No console.log statements in production code
- [ ] Bundle size impact assessed
- [ ] Accessibility requirements met
- [ ] Documentation updated if API changed

---

## 7. Performance Budgets

### Web Vitals Targets

| Metric | Target | Maximum |
|--------|--------|---------|
| First Contentful Paint | 1.8s | 2.5s |
| Largest Contentful Paint | 2.5s | 4.0s |
| First Input Delay | 100ms | 200ms |
| Cumulative Layout Shift | 0.1 | 0.25 |
| Time to Interactive | 3.0s | 5.0s |

### Monitoring

- Lighthouse audits run on every PR
- Performance regression alerts configured
- Bundle analysis reports stored as artifacts

---

## 8. Dependency Management

### Automated Updates

- Weekly dependency updates via GitHub Actions
- Security patches applied immediately
- Major version updates require manual review

### Approval Requirements

| Update Type | Review Required |
|-------------|-----------------|
| Security patch | Auto-merge allowed |
| Minor version | QA review |
| Major version | TechLead + QA review |

---

## 9. Accessibility Standards

### Requirements

- WCAG 2.1 AA compliance required
- Automated axe-core testing in CI/CD
- Manual accessibility review for complex components
- Screen reader testing for critical flows

### Testing

```bash
# Run accessibility tests
npm run e2e:accessibility

# Run all quality E2E tests
npm run e2e:quality
```

---

## 10. Enforcement

### CI/CD Gates

All quality checks are enforced in CI/CD:

1. Type checking must pass
2. ESLint must pass (no errors)
3. Test coverage thresholds met
4. Bundle size within limits
5. Accessibility tests pass

### Exceptions

Exceptions require:
1. Written justification in PR description
2. TechLead approval
3. Tracking issue for resolution
4. Timeline for compliance

---

## Related Documents

- [ADR-020] PR Quality Gate
- [ADR-021] ArchUnitNET Architecture Testing
- [GL-005] SARAH Quality Gate Criteria
- [INS-002] Frontend Instructions

---

**Maintained by**: @TechLead  
**Review Schedule**: Monthly
