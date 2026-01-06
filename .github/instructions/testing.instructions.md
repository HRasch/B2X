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

## MCP-Enhanced Testing Strategy

### Type Safety Validation

**TypeScript Testing (Frontend)**:
```bash
# Before running tests - ensure zero type errors
typescript-mcp/analyze_types workspacePath="frontend/Store" filePath="src/components/ProductCard.vue"

# Type errors must be fixed before test execution
```

**C# Testing (Backend)** - Enable Roslyn MCP:
```bash
# Validate types before running tests
roslyn-mcp/analyze_types workspacePath="backend/Domain/Catalog"

# Ensure compilation succeeds
```

---

### Component Testing with Vue MCP

**Pre-Test Validation**:
```bash
# 1. Component structure validation
vue-mcp/analyze_vue_component filePath="src/components/LoginForm.vue"

# 2. i18n coverage check
vue-mcp/validate_i18n_keys componentPath="src/components/LoginForm.vue"
# Must return zero hardcoded strings

# 3. Responsive design validation
vue-mcp/check_responsive_design filePath="src/components/LoginForm.vue"

# 4. Accessibility pre-check
vue-mcp/check_accessibility filePath="src/components/LoginForm.vue"
```

**Integration**: Run MCP validations as part of test setup

---

### E2E Testing with Chrome DevTools MCP (Optional)

**Status**: Disabled by default (enable for advanced E2E testing)

**Enable Chrome DevTools MCP**:
```json
// .vscode/mcp.json
{
  "mcpServers": {
    "chrome-devtools": {
      "disabled": false  // Change from true
    }
  }
}
```

**Use Cases**:
- Visual regression testing with screenshot comparisons
- Performance profiling with Lighthouse
- Network monitoring and API validation
- Real-browser accessibility audits
- PDF generation for test reports

**Example Workflow**:
```bash
# 1. Launch browser with DevTools protocol
chrome-devtools-mcp/launch browser

# 2. Navigate and capture baseline
chrome-devtools-mcp/navigate url="http://localhost:3000"
chrome-devtools-mcp/screenshot path="tests/baseline/homepage.png"

# 3. Run Lighthouse audit
chrome-devtools-mcp/lighthouse url="http://localhost:3000"

# 4. Monitor network calls
chrome-devtools-mcp/network-monitor enable=true

# 5. Execute test actions
# [Playwright/Cypress test execution]

# 6. Visual regression comparison
# Compare screenshots against baseline
```

---

### Security Testing Integration

**Pre-Test Security Validation**:
```bash
# 1. Frontend security scan
security-mcp/scan_xss_vulnerabilities workspacePath="frontend/Store"

# 2. Backend security scan  
security-mcp/check_sql_injection workspacePath="backend"
security-mcp/validate_input_sanitization workspacePath="backend"

# 3. Dependency vulnerability check
security-mcp/scan_vulnerabilities workspacePath="."
```

**Policy**: Security MCP must pass GREEN before running test suite

---

### Accessibility Testing

**Automated Accessibility Validation**:
```bash
# HTML/CSS accessibility checks
htmlcss-mcp/check_html_accessibility \
  workspacePath="frontend/Store" \
  filePath="pages/product-detail.html"

# Vue component accessibility
vue-mcp/check_accessibility \
  filePath="src/components/ProductCard.vue"

# Target: WCAG 2.1 Level AAA compliance
```

**Integration with E2E Tests**:
- Run accessibility MCP before visual regression tests
- Validate WCAG compliance on critical user journeys
- Generate accessibility reports alongside test results

---

### MCP-Powered Test Workflow

**Complete Pre-Test Checklist**:
```bash
#!/bin/bash
echo "Running MCP pre-test validation..."

# 1. Type safety (choose based on stack)
typescript-mcp/analyze_types workspacePath="frontend/Store" || exit 1
# roslyn-mcp/analyze_types workspacePath="backend" || exit 1

# 2. Component validation (frontend)
vue-mcp/validate_i18n_keys workspacePath="frontend/Store" || exit 1
vue-mcp/check_responsive_design filePath="src/components/ProductCard.vue" || exit 1

# 3. Security validation
security-mcp/scan_xss_vulnerabilities workspacePath="frontend/Store" || exit 1
security-mcp/check_sql_injection workspacePath="backend" || exit 1

# 4. Accessibility validation
htmlcss-mcp/check_html_accessibility workspacePath="frontend/Store" filePath="pages/index.html" || exit 1

echo "✅ All MCP validations passed - proceeding with tests"
npm run test:e2e
```

---

### Visual Regression Testing

**Using Chrome DevTools MCP** (when enabled):
```bash
# Baseline creation (after intentional UI changes)
npm run test:e2e -- --update-snapshots

# Visual regression validation
chrome-devtools-mcp/screenshot path="tests/current/page.png"
# Compare against tests/baseline/page.png
# Threshold: 0.1 (10% pixel difference tolerance)
```

**Policy**:
- Update baselines only after design review
- Document visual changes in PR description
- Require @UX approval for visual regressions

---

### Performance Testing

**Chrome DevTools MCP Integration**:
```bash
# Lighthouse performance audit
chrome-devtools-mcp/lighthouse url="http://localhost:3000"

# Target metrics:
# - Performance Score: >90
# - First Contentful Paint: <1.8s
# - Largest Contentful Paint: <2.5s
# - Time to Interactive: <3.8s
# - Cumulative Layout Shift: <0.1
```

**Bundle Size Analysis**:
```bash
# Vite bundle analysis (Vue MCP)
vue-mcp/analyze_bundle workspacePath="frontend/Store"

# Target: Total bundle size <500KB gzipped
```

---

### Test Execution Best Practices

**MCP Integration Points**:
1. **Pre-test**: Run all MCP validators
2. **During test**: Use Chrome DevTools MCP for browser automation
3. **Post-test**: Generate MCP reports for accessibility, security, performance

**Resource Management**:
- Run MCP validations in parallel where possible
- Use targeted file paths instead of full workspace scans
- Cache MCP results during test runs
- Clean up Chrome DevTools MCP sessions after tests

**Failure Handling**:
- MCP validation failures should fail test suite
- Document MCP exceptions in test reports
- Re-run MCP after fixing issues to verify resolution

