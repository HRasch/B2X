# 🧪 RUN_TESTS - Quality Assurance & Testing Cycle

**Trigger**: Feature ready for testing or before merge
**Coordinator**: @QA
**Output**: Test report, defect list, sign-off

---

## Quick Start
```
@QA: /run-tests
Component: backend | frontend | all
Scope: unit | integration | e2e | all
Environment: development | staging | production
```

---

## Testing Strategy

### Phase 0: MCP Pre-Test Validation (MANDATORY)

**Run before executing test suite to ensure code quality:**

#### Frontend Pre-Test Checks
```bash
echo "🔍 Running Frontend MCP Pre-Test Validation..."

# 1. Type safety (CRITICAL - must pass)
typescript-mcp/analyze_types workspacePath="frontend/Store"
if [ $? -ne 0 ]; then 
  echo "❌ Type errors detected - fix before running tests"
  exit 1
fi

# 2. i18n compliance (CRITICAL - zero hardcoded strings)
vue-mcp/validate_i18n_keys workspacePath="frontend/Store"
if [ $? -ne 0 ]; then 
  echo "❌ Hardcoded strings detected - fix before running tests"
  exit 1
fi

# 3. Component structure validation
vue-mcp/analyze_vue_component filePath="src/components/[TestTarget].vue"

# 4. Accessibility pre-check
vue-mcp/check_accessibility filePath="src/components/[TestTarget].vue"

# 5. Responsive design validation
vue-mcp/check_responsive_design filePath="src/components/[TestTarget].vue"

echo "✅ Frontend MCP pre-test validation passed"
```

#### Backend Pre-Test Checks
```bash
echo "🔍 Running Backend MCP Pre-Test Validation..."

# Optional: Enable Roslyn MCP for type safety
# roslyn-mcp/analyze_types workspacePath="backend"

# 1. SQL injection check (CRITICAL)
security-mcp/check_sql_injection workspacePath="backend"
if [ $? -ne 0 ]; then 
  echo "❌ SQL injection vulnerabilities detected"
  exit 1
fi

# 2. Input validation
security-mcp/validate_input_sanitization workspacePath="backend"

# 3. Authentication patterns
security-mcp/check_authentication workspacePath="backend"

echo "✅ Backend MCP pre-test validation passed"
```

#### Security Pre-Test Checks
```bash
echo "🔍 Running Security MCP Pre-Test Validation..."

# 1. Dependency vulnerabilities
security-mcp/scan_vulnerabilities workspacePath="."

# 2. XSS scan (frontend)
security-mcp/scan_xss_vulnerabilities workspacePath="frontend"

echo "✅ Security MCP pre-test validation passed"
```

**Policy**: 
- ❌ **DO NOT RUN TESTS** if MCP validation fails
- ✅ **FIX ISSUES FIRST**, then re-run MCP validation
- ✅ **DOCUMENT** any MCP exceptions with justification

---

### Unit Tests (@QA)
**Command**: `dotnet test` or `npm test`
- **Backend**: All domain logic tested
- **Frontend**: Component logic tested
- **Coverage**: Minimum 80% for critical paths
- **Framework**: xUnit (backend), Vitest (frontend)

### Integration Tests (@QA)
**Command**: `dotnet test --filter Integration`
- Database interactions
- Service composition
- API endpoint behavior
- Event handling
- External service mocking

### Runtime Health Check (@QA)
**Command**: `scripts/runtime-health-check.sh`
- Execute after Unit/Integration tests
- Validates backend services runtime health via MonitoringMCP
- Focus: Store Gateway (port 8000) and other critical services
- **Policy**: Build blocks on health check failure (exit code 1)
- Simulates error scenarios for validation

**Integration Steps**:
```bash
# After unit/integration tests pass
echo "🔍 Running Runtime Health Check..."
./scripts/runtime-health-check.sh
if [ $? -ne 0 ]; then
  echo "❌ Runtime health check failed - blocking build"
  exit 1
fi
echo "✅ Runtime health check passed"
```

**Error Simulation for Testing**:
- Stop backend services to simulate downtime
- Modify health endpoints to return errors
- Test network isolation scenarios

### E2E Tests (Frontend)
**Scope**: Critical user journeys

**Optional: Chrome DevTools MCP Integration**
```bash
# Enable in .vscode/mcp.json for advanced E2E testing
# "chrome-devtools": { "disabled": false }

# Example: Visual regression testing
chrome-devtools-mcp/launch browser
chrome-devtools-mcp/navigate url="http://localhost:3000"
chrome-devtools-mcp/screenshot path="tests/baseline/homepage.png"

# Example: Performance profiling
chrome-devtools-mcp/lighthouse url="http://localhost:3000"
# Target: Performance Score >90

# Example: Network monitoring
chrome-devtools-mcp/network-monitor enable=true
# [Run E2E tests]
# Validate API call patterns
```

**Critical User Journeys**:
- Login flow
- Product browsing
- Shopping cart
- Checkout process
- Payment integration

**E2E Pre-Test MCP Validation**:
```bash
# Accessibility validation before E2E
htmlcss-mcp/check_html_accessibility \
  workspacePath="frontend/Store" \
  filePath="pages/index.html"

# Component validation
vue-mcp/check_accessibility filePath="src/components/LoginForm.vue"
```

### Security Tests (@Security)
**MCP-Powered Security Testing**:
```bash
# Automated security validation during test execution
security-mcp/check_sql_injection workspacePath="backend"
security-mcp/scan_xss_vulnerabilities workspacePath="frontend"
security-mcp/validate_input_sanitization workspacePath="."
security-mcp/check_authentication workspacePath="backend"
```

**Manual Security Tests**:
- SQL injection attempts
- XSS payloads
- CSRF token validation
- Authentication bypass
- Authorization checks

**Policy**: Security MCP must pass GREEN before manual security testing

### Performance Tests (@QA)
**MCP-Powered Performance Testing**:
```bash
# Chrome DevTools MCP Lighthouse audit
chrome-devtools-mcp/lighthouse url="http://localhost:3000"

# Target Metrics:
# - Performance Score: >90
# - First Contentful Paint: <1.8s
# - Largest Contentful Paint: <2.5s
# - Time to Interactive: <3.8s
# - Cumulative Layout Shift: <0.1

# Bundle size analysis (Vue MCP)
vue-mcp/analyze_bundle workspacePath="frontend/Store"
# Target: <500KB gzipped
```

**Manual Performance Tests**:
- Response time thresholds
- Database query performance
- Load testing (under 100 concurrent users)
- Memory profiling

---

## Test Environment Setup

### Development Environment
```bash
# Backend tests
cd backend
dotnet test B2Connect.slnx -v minimal

# Frontend tests
cd frontend/{Management|Store|Admin}
npm test
```

### Staging Environment
- Deploy feature branch
- Run full test suite
- Performance baseline verification
- UAT preparation

---

## Test Report Template

```markdown
## Test Execution Report

### Component: [Backend/Frontend/Integration]
### Environment: [Development/Staging]
### Date: [YYYY-MM-DD]
### Tester: @QA

---

### MCP Pre-Test Validation Results

#### Type Safety
- **Frontend**: `typescript-mcp/analyze_types` - [✅ PASS | ❌ FAIL]
  - Type errors: [count]
- **Backend**: `roslyn-mcp/analyze_types` (if enabled) - [✅ PASS | N/A]

#### i18n Compliance
- `vue-mcp/validate_i18n_keys` - [✅ PASS | ❌ FAIL]
  - Hardcoded strings: [count]

#### Accessibility
- `htmlcss-mcp/check_html_accessibility` - [✅ PASS | ⚠️ WARNINGS]
  - CRITICAL violations: [count]
  - SERIOUS violations: [count]

#### Security
- `security-mcp/check_sql_injection` - [✅ PASS | ❌ FAIL]
- `security-mcp/scan_xss_vulnerabilities` - [✅ PASS | ❌ FAIL]
- `security-mcp/scan_vulnerabilities` - [✅ PASS | ⚠️ WARNINGS]

#### MCP Pre-Test Result
[✅ ALL PASSED - Tests can proceed | ❌ FAILED - Fix issues before testing]

---

### Summary
- **Total Tests**: X
- **Passed**: X ✅
- **Failed**: X ❌
- **Skipped**: X ⏭️
- **Coverage**: X%

---

### Test Execution Details

#### Unit Tests
- Status: [PASS/FAIL]
- Issues: [None / Listed below]

#### Integration Tests
- Status: [PASS/FAIL]
- Issues: [None / Listed below]

#### E2E Tests (if applicable)
- Status: [PASS/FAIL]
- Chrome DevTools MCP Used: [YES/NO]
- Visual Regression: [PASS/FAIL/N/A]
- Performance Score: [score/N/A]
- Issues: [None / Listed below]

---

### MCP-Enhanced Test Results

#### Performance (Chrome DevTools MCP)
- Lighthouse Score: [score/100]
- Bundle Size: [size] KB gzipped
- Target: <500KB ✅/❌

#### Accessibility (HTML/CSS MCP + Vue MCP)
- WCAG Compliance: [AAA/AA/A]
- Critical Violations: [count]
- Serious Violations: [count]

---

### Defects Found

| ID | Severity | Source | Title | Description | Status |
|---|---|---|---|---|---|
| B-001 | P1 | MCP | [Title] | [Description] | New |
| B-002 | P2 | Manual | [Title] | [Description] | New |

---

### Sign-Off

- [ ] MCP pre-test validation passed
- [ ] All critical tests passing
- [ ] Coverage acceptable (>80%)
- [ ] Performance acceptable (score >90)
- [ ] Security MCP checks passed
- [ ] Accessibility validated (WCAG AAA target)
- [ ] Ready for production

**QA Lead**: [Signature]
**Date**: [YYYY-MM-DD]
```

---

## Test Execution Checklist

### Before Testing
- [ ] MCP pre-test validation completed and passed
- [ ] Environment configured
- [ ] Test data prepared
- [ ] Dependencies installed
- [ ] Previous test artifacts cleaned

### During Testing
- [ ] MCP tools monitoring (if applicable):
  - [ ] Chrome DevTools MCP for E2E performance
  - [ ] Security MCP for real-time validation
- [ ] Logs captured
- [ ] Screenshots/videos of failures
- [ ] Performance metrics recorded
- [ ] Issues documented immediately

### After Testing
- [ ] Test report generated (including MCP results)
- [ ] Defects triaged
- [ ] Coverage metrics analyzed
- [ ] MCP baselines updated (visual regression, performance)
- [ ] Retrospective notes collected

---

## Defect Severity Levels

| Level | Impact | Example | Source | Response |
|---|---|---|---|---|
| **P0 - Critical** | System down | Login broken, MCP CRITICAL security issue | MCP/Manual | Immediate fix required |
| **P1 - High** | Major feature broken | Payment fails, MCP HIGH vulnerability | MCP/Manual | Fix before merge |
| **P2 - Medium** | Workaround exists | UI glitch, MCP MEDIUM warning | MCP/Manual | Fix in next sprint |
| **P3 - Low** | Minor issue | Typo, MCP MINOR suggestion | MCP/Manual | Nice-to-have |

---

## Approval Criteria

- ✅ MCP pre-test validation passed
- ✅ All P0 defects resolved (including MCP CRITICAL)
- ✅ P1 defects assigned to sprint (including MCP HIGH)
- ✅ Test coverage ≥ 80%
- ✅ Performance benchmarks met (Lighthouse >90)
- ✅ Security MCP checks passed GREEN
- ✅ Accessibility validated (WCAG AAA target)
- ✅ @QA sign-off obtained

---

## References

- [KB-053] TypeScript MCP Integration Guide
- [KB-054] Vue MCP Integration Guide  
- [KB-055] Security MCP Best Practices
- [KB-056] HTML/CSS MCP Usage Guide
- [mcp-operations.instructions.md] MCP Operations Guide
- [INS-003] Testing Instructions
