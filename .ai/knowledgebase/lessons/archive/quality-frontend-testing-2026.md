---
docid: KB-131
title: Quality Frontend Testing 2026
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

---
docid: quality-frontend-testing-2026
title: 7. Januar 2026 - Comprehensive Frontend Quality Assurance & Test Infrastructure Fixes
category: quality
migrated: 2026-01-08
---
### Nuxt Composables Mocking Strategy for Vitest

**Issue**: `useCookie` composable returning plain objects instead of Vue refs, causing JSON parsing errors in auth store tests.

**Root Cause**: Incorrect mocking approach using `vi.mock()` which doesn't handle global composables properly.

**Lesson**: Use `vi.stubGlobal()` for Nuxt auto-imported composables to ensure proper Vue reactivity.

**Solution**: Replace `vi.mock()` with `vi.stubGlobal()` for composables:
```typescript
// tests/setup.ts - BEFORE (causing JSON parsing errors)
vi.mock('#app', () => ({
  useCookie: vi.fn(() => ({ value: null })),
}));

// tests/setup.ts - AFTER (proper Vue ref behavior)
vi.stubGlobal('useCookie', vi.fn(() => ref(null)));
```

**Benefits**:
- **Proper Reactivity**: Returns Vue refs that work with `.value` access
- **Type Safety**: Maintains TypeScript compatibility
- **Test Reliability**: Eliminates JSON parsing errors in auth store
- **Framework Alignment**: Matches Nuxt's actual composable behavior

### Auth Store Test Environment Compatibility

**Issue**: Auth store using `useCookie` in production but tests failing due to cookie unavailability.

**Root Cause**: Single persistence strategy (cookies only) incompatible with test environment.

**Lesson**: Implement dual persistence strategy for production/test compatibility.

**Solution**: Add localStorage fallback for test environments:
```typescript
// src/stores/auth.ts
const useAuthPersistence = () => {
  if (process.env.NODE_ENV === 'test') {
    // Test environment: use localStorage
    return {
      get: () => localStorage.getItem('auth'),
      set: (value: string) => localStorage.setItem('auth', value),
      remove: () => localStorage.removeItem('auth'),
    };
  } else {
    // Production: use cookies
    const authCookie = useCookie('auth', { 
      default: () => null,
      encode: JSON.stringify,
      decode: JSON.parse,
    });
    return {
      get: () => authCookie.value,
      set: (value: any) => authCookie.value = value,
      remove: () => authCookie.value = null,
    };
  }
};
```

**Key Benefits**:
- **Environment Agnostic**: Works in both production and test environments
- **Zero Breaking Changes**: Production behavior unchanged
- **Test Reliability**: Eliminates cookie-related test failures
- **SSR Compatible**: Maintains server-side rendering support

### PostCSS ES Module Configuration for Tailwind v4

**Issue**: PostCSS configuration failing with CommonJS syntax in ES module environment.

**Root Cause**: Using `module.exports` instead of ES module `export default`.

**Lesson**: Tailwind CSS v4 requires ES module syntax in PostCSS configuration.

**Solution**: Convert to ES module exports:
```javascript
// postcss.config.js - BEFORE (CommonJS - fails in v4)
module.exports = {
  plugins: [tailwindcss, autoprefixer]
};

// postcss.config.js - AFTER (ES modules - works with v4)
export default {
  plugins: [tailwindcss, autoprefixer]
};
```

**Migration Impact**:
- **Framework Alignment**: Matches Tailwind v4's ES module requirements
- **Build Stability**: Eliminates PostCSS configuration errors
- **Future Proofing**: Compatible with modern build tools
- **Type Safety**: Better IDE support and error detection

### i18n Plugin Setup for Component Testing

**Issue**: Internationalized components failing in tests due to missing i18n context.

**Root Cause**: Test setup missing Vue i18n plugin configuration.

**Lesson**: Component tests require explicit i18n plugin setup for proper translation mocking.

**Solution**: Add i18n plugin to test component mounting:
```typescript
// tests/components/cms/Testimonials.spec.ts
import { createI18n } from 'vue-i18n';

const i18n = createI18n({
  legacy: false,
  locale: 'en',
  messages: {
    en: { /* mock translations */ }
  }
});

const wrapper = mount(Testimonials, {
  global: {
    plugins: [i18n], // Required for i18n-enabled components
  }
});
```

**Testing Benefits**:
- **Translation Resolution**: Eliminates "key not found" warnings
- **Component Stability**: Prevents i18n-related test failures
- **Realistic Testing**: Mimics production i18n setup
- **Error Prevention**: Catches missing translation keys early

### ESLint Unused Parameter Warnings in Test Files

**Issue**: ESLint flagging unused parameters in test mock functions.

**Root Cause**: Mock functions with required parameter signatures but unused in test context.

**Lesson**: Use ESLint disable comments for intentionally unused parameters in test mocks.

**Solution**: Add targeted ESLint disable comments:
```typescript
// BEFORE - ESLint error
vi.mock('#app', () => ({
  useCookie: vi.fn((name) => ref(null)), // 'name' parameter unused
}));

// AFTER - ESLint compliant
vi.mock('#app', () => ({
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  useCookie: vi.fn((name) => ref(null)), // 'name' parameter intentionally unused
}));
```

**Code Quality Balance**:
- **Clean Tests**: No ESLint violations in test files
- **Intent Clarity**: Comments explain why parameters are unused
- **Maintainability**: Prevents future accidental usage
- **CI Compliance**: Tests pass linting gates

### Systematic Quality Check Pipeline Debugging

**Issue**: Quality check failing on coverage phase despite direct test execution succeeding.

**Root Cause**: Vitest coverage configuration not finding test files in quality-check context.

**Lesson**: Coverage configuration requires explicit file pattern matching for monorepo structures.

**Investigation Process**:
1. **Isolate Issue**: Direct `npm run test` works, `npm run test:coverage` fails
2. **Check Configuration**: Vitest config missing coverage file patterns
3. **Root Cause**: Coverage provider not finding test files in nested directories
4. **Solution**: Add explicit include patterns to vitest.config.ts

**Solution**: Configure coverage file patterns:
```typescript
// vitest.config.ts
export default defineConfig({
  test: {
    coverage: {
      include: ['**/*.{test,spec}.{js,mjs,cjs,ts,mts,cts,jsx,tsx}'],
      exclude: ['node_modules/**', '.nuxt/**', 'dist/**'],
    },
  },
});
```

**Debugging Benefits**:
- **Systematic Approach**: Step-by-step issue isolation
- **Configuration Awareness**: Understanding tool-specific requirements
- **Pattern Recognition**: Identifying monorepo-specific issues
- **Documentation Value**: Creating reusable troubleshooting guides

### Vue Component Asset Import Path Resolution

**Issue**: CSS imports failing due to incorrect relative paths in monorepo structure.

**Root Cause**: Asset paths not accounting for nested frontend directory structure.

**Lesson**: Use root-relative paths for shared assets in monorepo frontend projects.

**Solution**: Update CSS import paths:
```css
/* BEFORE - Incorrect relative path */
@import "../../../assets/css/main.css";

/* AFTER - Root-relative path */
@import "~/assets/css/main.css";
```

**Path Resolution Benefits**:
- **Monorepo Compatibility**: Works regardless of nesting depth
- **Build Tool Integration**: Leverages Nuxt/Vite path resolution
- **Maintainability**: No path updates needed when moving files
- **Convention Alignment**: Follows Nuxt path alias conventions

### Test Setup File Organization Best Practices

**Issue**: Test setup becoming complex with multiple mocking strategies.

**Root Cause**: Scattered mocking logic across different test files.

**Lesson**: Centralize test setup with clear separation of concerns.

**Solution**: Organize setup.ts with clear sections:
```typescript
// tests/setup.ts
import { vi } from 'vitest';

// 1. Global test utilities
global.testUtils = { /* shared utilities */ };

// 2. Nuxt composables mocking
vi.stubGlobal('useCookie', vi.fn(() => ref(null)));
vi.stubGlobal('useHead', vi.fn());

// 3. Vue plugins setup
global.createTestWrapper = (component, options = {}) => {
  return mount(component, {
    global: {
      plugins: [createI18n({ /* config */ })],
      ...options.global,
    },
  });
};

// 4. Cleanup utilities
afterEach(() => {
  vi.clearAllMocks();
  localStorage.clear();
});
```

**Organization Benefits**:
- **Maintainability**: Single source of truth for test setup
- **Consistency**: Standardized mocking across all tests
- **Reusability**: Shared utilities reduce code duplication
- **Debugging**: Centralized setup easier to troubleshoot

### Quality Gate Integration Testing

**Issue**: Quality check pipeline not reflecting actual development workflow.

**Root Cause**: Quality checks running in isolation without integration testing.

**Lesson**: Quality gates should validate complete development workflow, not just individual tools.

**Solution**: Implement comprehensive quality validation:
```json
// package.json scripts
{
  "quality-check": "npm run type-check && npm run lint-check && npm run test:coverage",
  "type-check": "nuxt typecheck",
  "lint-check": "eslint . --ext .ts,.vue",
  "test:coverage": "vitest run --coverage"
}
```

**Quality Assurance Benefits**:
- **Workflow Validation**: Tests complete development pipeline
- **Early Detection**: Catches integration issues before merge
- **CI/CD Reliability**: Ensures consistent quality across environments
- **Developer Confidence**: Clear pass/fail indicators for code readiness

---
