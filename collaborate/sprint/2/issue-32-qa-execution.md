# Issue #32 - QA Phase Execution Instructions

**Status**: Warning Fixes Complete → QA Testing Phase  
**Date**: 30. Dezember 2025  
**Executor**: @team-assistant (automatic execution)  
**Authority**: @process-assistant (workflow authorization)

---

## 🎯 Objectives

Execute QA phase for Issue #32 Invoice feature:
1. ✅ Verify build succeeds with clean state
2. ✅ Run unit/integration test suite
3. ✅ Confirm all tests pass (100% success rate)
4. ✅ Report status and metrics

---

## 📋 Automatic Execution Steps

### Step 1: Clean Build (Clear Cache)

**Command**:
```bash
cd /Users/holger/Documents/Projekte/B2Connect
dotnet clean B2Connect.slnx
dotnet build B2Connect.slnx -v minimal
```

**Success Criteria**:
- Exit code: 0
- Output: "Build succeeded"
- Errors: 0
- Warnings: 39 or fewer (Issue #32 code should be clean)

**Action if Failed**:
- Stop and report specific errors to @process-assistant
- Do NOT proceed to testing

---

### Step 2: Run Invoice Test Suite

**Command**:
```bash
cd /Users/holger/Documents/Projekte/B2Connect
dotnet test backend/Domain/Customer/tests/B2Connect.Customer.Tests.csproj -v minimal --logger "console;verbosity=normal"
```

**Success Criteria**:
- Exit code: 0
- Output contains: "Passed" with count (e.g., "Passed: 18")
- No failed tests
- Test count: 18+ tests (Invoice-related)

**Action if Failed**:
- Report specific test failures
- Note which tests failed and why
- Do NOT mark as complete

---

### Step 3: Verify Specific Invoice Functionality

**Test Classes to Verify Passed**:
1. `InvoiceServiceTests` - Business logic
2. `InvoiceRepositoryTests` - Data access
3. `InvoiceHandlerTests` - Integration
4. `InvoiceValidatorTests` - Validation

**Action**:
- Confirm all 4 test classes show green status
- If any red, report which class and test name failed

---

### Step 4: Report Results

**Success Report Template**:
```
✅ Issue #32 QA Phase Complete

Build Status:
- Clean build: SUCCESS (0 errors)
- Warnings: [count] (cached only, code is correct)

Test Results:
- Test suite: PASS (18/18 or actual count)
- All Invoice tests: GREEN
- No regressions detected

Status: READY FOR CODE REVIEW

Next: Move to PR creation and code review (when user resumes)
```

**Failure Report Template**:
```
❌ Issue #32 QA Phase - Blockers Found

Failure Point: [Build or Testing]
Error Details: [Specific error message]
Affected File: [Path]
Line Number: [If applicable]

Action Required: @process-assistant review and advise
```

---

## ⏱️ Execution Timeline

- **Build**: ~10 seconds
- **Test Execution**: ~30-60 seconds
- **Total**: ~90 seconds

---

## ✅ Definition of Done (QA Phase)

Before marking complete, verify:
- [ ] Build: 0 errors (warnings cached, not a blocker)
- [ ] Tests: 100% pass rate (18/18 or more)
- [ ] No new test failures introduced
- [ ] Code compiles cleanly
- [ ] All Invoice handlers tested
- [ ] All Invoice validators tested
- [ ] All Invoice repository methods tested

---

## 🚀 Post-QA Actions (Conditional)

**IF Tests Pass**:
1. Document results in GitHub issue #32
2. Update issue status: "Ready for Code Review"
3. Notify @process-assistant of completion

**IF Tests Fail**:
1. Document failure details
2. Stop execution
3. Notify @process-assistant with error report
4. Wait for investigation and fixes

---

## 📞 Contact & Escalation

- **Executor**: @team-assistant
- **Authority**: @process-assistant
- **On Failure**: Escalate to @tech-lead or @qa-engineer

---

## 🔄 Automatic Execution Signal

**PROCEED WITH AUTOMATIC EXECUTION**: ✅ AUTHORIZED

This workflow is approved by @process-assistant for automatic execution by @team-assistant.

Status: READY TO EXECUTE  
Timestamp: 30. Dezember 2025, ~12:00 UTC  
Authorization: Process workflow for Issue #32 QA phase

---

**Next Phase After QA**: Code review + API documentation (upon completion)
