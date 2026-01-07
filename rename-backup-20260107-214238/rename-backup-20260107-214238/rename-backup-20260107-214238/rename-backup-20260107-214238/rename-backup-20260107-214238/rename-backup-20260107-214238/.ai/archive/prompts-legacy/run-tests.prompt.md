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

### E2E Tests (Frontend)
**Scope**: Critical user journeys
- Login flow
- Product browsing
- Shopping cart
- Checkout process
- Payment integration

### Security Tests (@Security)
- SQL injection attempts
- XSS payloads
- CSRF token validation
- Authentication bypass
- Authorization checks

### Performance Tests (@QA)
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
- Issues: [None / Listed below]

---

### Defects Found

| ID | Severity | Title | Description | Status |
|---|---|---|---|---|
| B-001 | P1 | [Title] | [Description] | New |

---

### Sign-Off

- [ ] All critical tests passing
- [ ] Coverage acceptable
- [ ] Performance acceptable
- [ ] Security checks passed
- [ ] Ready for production

**QA Lead**: [Signature]
**Date**: [YYYY-MM-DD]
```

---

## Test Execution Checklist

### Before Testing
- [ ] Environment configured
- [ ] Test data prepared
- [ ] Dependencies installed
- [ ] Previous test artifacts cleaned

### During Testing
- [ ] Logs captured
- [ ] Screenshots/videos of failures
- [ ] Performance metrics recorded
- [ ] Issues documented immediately

### After Testing
- [ ] Test report generated
- [ ] Defects triaged
- [ ] Coverage metrics analyzed
- [ ] Retrospective notes collected

---

## Defect Severity Levels

| Level | Impact | Example | Response |
|---|---|---|---|
| **P0 - Critical** | System down | Login broken | Immediate fix required |
| **P1 - High** | Major feature broken | Payment fails | Fix before merge |
| **P2 - Medium** | Workaround exists | UI glitch | Fix in next sprint |
| **P3 - Low** | Minor issue | Typo in label | Nice-to-have |

---

## Approval Criteria

- ✅ All P0 defects resolved
- ✅ P1 defects assigned to sprint
- ✅ Test coverage ≥ 80%
- ✅ Performance benchmarks met
- ✅ Security checks passed
- ✅ @QA sign-off obtained
