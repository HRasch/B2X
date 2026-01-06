---
applyTo: "**/*.test.*,**/*.spec.*,**/tests/**,**/__tests__/**"
---

# Testing Instructions

## Test Structure
- Use descriptive test names (describe what, not how)
- Follow Arrange-Act-Assert pattern
- One assertion per test (when practical)
- Group related tests with describe blocks

## Test Types
- **Unit Tests**: Test isolated functions/methods
- **Integration Tests**: Test component interactions
- **E2E Tests**: Test complete user flows
- **Visual Regression Tests**: Detect unintended UI changes with screenshot comparisons

## E2E Testing Strategy
- Run tests in chunks: build health → auth → responsive → visual
- Ensure dev server running before execution
- Use baseURL config, not hardcoded localhost
- Test critical journeys across browsers and breakpoints

## Visual Regression Testing
- Establish baselines after major UI changes
- Store snapshots in `tests/e2e/*.spec.ts-snapshots/`
- Use 0.1 (10%) pixel difference threshold
- Include in CI/CD pipelines

## Best Practices
- Test behavior, not implementation
- Use meaningful test data
- Avoid testing framework internals
- Mock external dependencies consistently
- **After fixing bugs**: Update `.ai/knowledgebase/lessons.md` with lessons learned to prevent future regressions

## Warning Policy
- **Fix all warnings** during test runs - treat warnings as errors
- If a warning cannot be fixed immediately, **whitelist it explicitly** with documented justification
- Common whitelisting methods:
  - ESLint: `// eslint-disable-next-line rule-name -- reason`
  - TypeScript: `// @ts-expect-error -- reason`
  - Playwright: Configure `expect.toPass()` options or test annotations
- **Never ignore warnings silently** - they indicate potential issues

## Coverage Goals
- Critical paths: 100% coverage
- Business logic: >80% coverage
- UI components: Test user interactions

## Test Execution Patterns
- Execute in logical groups, fix one chunk before next
- Update baselines after intentional UI changes
- Use single worker for e2e tests
- Ensure cleanup between test runs

## Debugging & Troubleshooting
- Server check: `curl -I http://localhost:PORT`
- Port conflicts: `lsof -i :PORT` then `pkill -f "nuxt\|vite"`
- Use root-relative import paths
- Recommended timeout: 30s for server startup

## Multilingual Testing
- Test translation keys exist in all supported languages
- Verify translation loading and switching functionality
- Test localization API endpoints for proper responses
- Include multilingual scenarios in E2E tests
- Validate date/number formatting across locales

