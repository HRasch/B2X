---
docid: INS-014
title: Testing.Instructions
owner: @CopilotExpert
status: Active
created: 2026-01-08
---

﻿---
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

## Contract Testing (Proposed)
- Require consumer-driven contract tests for gateway APIs; run in CI on integration lanes
- Store contract artifacts/versioning in `.ai/contracts/`

## Visual Regression Policy (Proposed)
- PRs run focused visual smoke tests; full visual suite runs nightly
- Baseline updates must reference a review comment and acceptance criteria

## Flaky Test Triage (Proposed)
- Mark flaky tests, quarantine them, and create remediation tasks with owners
- Track flaky-test counts and surface in weekly test-health reports

## Metrics & Control (Proposed)
- Track test pass rates, coverage, and time-to-fix
- Publish weekly to a testing dashboard or PR checks summary
- Document `scripts/run-local-checks.sh` usage and troubleshooting tips

## MCP-Enhanced Testing Strategy

### runSubagent for Pre-Test Validation (Token-Optimized)

For comprehensive pre-test checks, use `#runSubagent` to execute all validations in isolation:

```text
Validate test readiness with #runSubagent:
- TypeScript type checking for frontend/Store
- Roslyn analysis for backend/Domain
- Vue component validation (structure, i18n, accessibility)
- Database schema validation for test DB
- API contract validation

Return ONLY: type_errors + blocking_issues + test_readiness_score
```

**Benefits**:
- ~6000 Token savings for full pre-test validation
- All domain checks (frontend, backend, DB) run in isolated context
- Only actionable blockers returned to main context
- Prevents test runs with known type/schema errors

**When to use**: Before any test suite execution, especially E2E

---

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

**Reference**: See [KB-064] Chrome DevTools MCP Server.

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

### Database MCP for Data Integrity Testing

**Reference**: See [KB-057] Database MCP Usage Guide.

**Database Test Validation**:
```bash
# Validate test database schema
database-mcp/validate_schema connectionString="Server=localhost;Database=B2X_test"

# Check test data migrations
database-mcp/check_migrations workspacePath="backend" migrationPath="TestMigrations"

# Analyze test query performance
database-mcp/analyze_queries workspacePath="backend/Domain" queryFile="TestQueries.cs"

# Validate multi-tenant test setup
database-mcp/validate_multitenancy workspacePath="backend" tenantConfig="appsettings.Testing.json"
```

### API Documentation MCP for Contract Testing

**Reference**: See [KB-059] API Documentation MCP Usage Guide.

**API Test Validation**:
```bash
# Validate OpenAPI specs for testing
api-mcp/validate_openapi filePath="backend/Gateway/Store/openapi.yaml"

# Check API contracts in tests
api-mcp/validate_contracts workspacePath="backend/Gateway"

# Detect breaking changes in test APIs
api-mcp/check_breaking_changes oldSpec="v1.0/openapi.yaml" newSpec="v2.0/openapi.yaml"

# Validate test API schemas
api-mcp/validate_schemas workspacePath="backend"
```

### i18n MCP for Localization Testing

**Reference**: See [KB-060] i18n MCP Usage Guide.

**Localization Test Validation**:
```bash
# Validate translation keys in tests
i18n-mcp/validate_translation_keys workspacePath="frontend/Store" localePath="locales"

# Check missing translations in test data
i18n-mcp/check_missing_translations workspacePath="frontend/Store" baseLocale="en"

# Validate translation consistency in tests
i18n-mcp/validate_consistency workspacePath="frontend/Store"

# Check pluralization in test scenarios
i18n-mcp/check_pluralization workspacePath="frontend/Store" locale="de"
```

### Monitoring MCP for Test Observability

**Reference**: See [KB-061] Monitoring MCP Usage Guide.

**Test Monitoring Setup**:
```bash
# Collect test execution metrics
monitoring-mcp/collect_application_metrics serviceName="test-runner"

# Monitor test environment performance
monitoring-mcp/monitor_system_performance hostName="test-server-01"

# Track test failures and errors
monitoring-mcp/track_errors serviceName="test-suite"

# Analyze test logs
monitoring-mcp/analyze_logs filePath="logs/test-results.log"

# Validate test health checks
monitoring-mcp/validate_health_checks serviceName="test-services"
```

### Documentation MCP for Test Documentation

**Reference**: See [KB-062] Documentation MCP Usage Guide.

**Test Documentation Validation**:
```bash
# Validate test documentation
docs-mcp/validate_documentation filePath="docs/testing/test-strategy.md"

# Check links in test docs
docs-mcp/check_links workspacePath="docs/testing"

# Analyze test documentation quality
docs-mcp/analyze_content_quality filePath="README.md"

# Validate test documentation structure
docs-mcp/validate_structure workspacePath="docs/testing"
```

### Docker MCP for Container Testing

**Reference**: See Docker MCP tools in MCP Operations Guide.

**Container Test Validation**:
```bash
# Validate test container images
docker-mcp/check_container_security imageName="B2X/test-runner:latest"

# Analyze test Dockerfiles
docker-mcp/analyze_dockerfile filePath="Dockerfile.test"

# Check test Kubernetes manifests
docker-mcp/validate_kubernetes_manifests filePath="k8s/test-deployment.yaml"

# Monitor test container health
docker-mcp/monitor_container_health containerName="test-runner"
```

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

# 5. Performance validation
performance-mcp/analyze_code_performance workspacePath="frontend/Store" || exit 1

# 6. Git validation
git-mcp/validate_commit_messages workspacePath="." count=10 || exit 1

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

## Large File Editing Strategy ([GL-043])

When editing large test files (>200 lines), use the Multi-Language Fragment Editing approach with Testing MCP integration:

### Pre-Edit Analysis
```bash
# Test structure analysis
testing-mcp/validate_test_coverage workspacePath="backend" testFile="ServiceTests.cs"

# Mock validation
testing-mcp/analyze_mocks workspacePath="backend" testFile="ServiceTests.cs"

# Test data integrity check
testing-mcp/validate_test_data workspacePath="backend" testFile="IntegrationTests.cs"
```

### Fragment-Based Editing Patterns
```csharp
// Fragment: Test method (82% token savings)
[Fact]
public async Task ProcessOrder_ValidRequest_ReturnsSuccess()
{
    // Arrange - edit only test setup
    var request = new OrderRequest { /* test data */ };
    var mockValidator = new Mock<IOrderValidator>();
    
    // Act - single test action
    var result = await _sut.ProcessOrderAsync(request);
    
    // Assert - focused assertions
    result.IsSuccess.Should().BeTrue();
}
```

**Testing MCP Workflows**:
```bash
# 1. Test coverage validation
testing-mcp/validate_test_coverage workspacePath="backend" testFile="ServiceTests.cs"

# 2. Mock analysis and validation
testing-mcp/analyze_mocks workspacePath="backend" testFile="ServiceTests.cs"
testing-mcp/validate_mock_setup testFile="ServiceTests.cs"

# 3. Test data integrity
testing-mcp/validate_test_data workspacePath="backend" testFile="IntegrationTests.cs"

# 4. Performance test validation
testing-mcp/analyze_performance_tests workspacePath="backend" testFile="LoadTests.cs"

# 5. Contract test validation
testing-mcp/validate_contract_tests workspacePath="backend/Gateway" testFile="ApiContractTests.cs"
```

### Integration with Other MCP Tools
```bash
# Database test validation
database-mcp/validate_schema connectionString="Server=localhost;Database=B2X_test"

# API documentation validation
api-mcp/validate_openapi filePath="backend/Gateway/Store/openapi.yaml"

# i18n test validation
i18n-mcp/validate_translation_keys workspacePath="frontend/Store" localePath="locales"
```

### Quality Gates
- Always run `runTests()` after edits
- Use `testing-mcp/validate_test_coverage` for test validation
- Ensure no regression in coverage metrics (>80% maintained)
- Validate mock integrity and test data consistency

**Token Savings**: 82% vs. reading entire test files | **Quality**: Test integrity validation with coverage enforcement

## Temp-File Usage for Token Optimization

For large test outputs (e.g., coverage reports, logs >1KB), save to temp files to reduce token consumption:

```bash
# Auto-save large test output
OUTPUT=$(dotnet test --verbosity minimal 2>&1)
if [ $(echo "$OUTPUT" | wc -c) -gt 1024 ]; then
  bash scripts/temp-file-manager.sh create "$OUTPUT" txt
else
  echo "$OUTPUT"
fi

# Reference in prompts/responses
"See temp file: .ai/temp/task-uuid.json (5KB saved)"
```

- Auto-cleanup after 1 hour or task completion.
- Complements [GL-006] token optimization strategy.

