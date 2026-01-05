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
- **Chunked Execution**: Run e2e tests in manageable chunks (build health → authentication → responsive → visual regression)
- **Server Management**: Ensure dev server is running and accessible before test execution
- **Configuration Consistency**: Use baseURL configuration instead of hardcoded localhost URLs
- **Cross-Browser Coverage**: Test critical user journeys across supported browsers
- **Responsive Validation**: Test key interactions at mobile, tablet, and desktop breakpoints

## Visual Regression Testing
- **Baseline Creation**: Run tests first to establish baseline screenshots after major UI changes
- **Screenshot Management**: Store snapshots in `tests/e2e/*.spec.ts-snapshots/` directory
- **Threshold Configuration**: Use appropriate pixel difference thresholds (0.1 = 10% tolerance)
- **Component Focus**: Capture screenshots of critical UI components and layouts
- **CI/CD Integration**: Include in automated pipelines to catch visual regressions

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
- **Systematic Chunking**: Execute tests in logical groups to isolate issues
- **Incremental Validation**: Fix problems in one chunk before proceeding to next
- **Baseline Management**: Update visual regression baselines after intentional UI changes
- **Parallel Execution**: Use single worker for e2e tests to avoid resource conflicts
- **Resource Cleanup**: Ensure proper cleanup between test runs and server restarts

## Debugging & Troubleshooting
- **Server Connectivity**: Verify dev server is running with `curl -I http://localhost:PORT`
- **Port Conflicts**: Check for conflicting processes with `lsof -i :PORT`
- **Process Cleanup**: Kill conflicting processes with `pkill -f "nuxt\|vite"`
- **Configuration Validation**: Ensure baseURL matches actual server port
- **Import Path Issues**: Use root-relative paths for shared dependencies (e.g., `../../../node_modules`)
- **Timeout Management**: Balance test timeouts with server startup time (30s recommended)
- **Background Execution**: Run servers with `npm run dev > /dev/null 2>&1 &` for stability

## Multilingual Testing
- Test translation keys exist in all supported languages
- Verify translation loading and switching functionality
- Test localization API endpoints for proper responses
- Include multilingual scenarios in E2E tests
- Validate date/number formatting across locales

