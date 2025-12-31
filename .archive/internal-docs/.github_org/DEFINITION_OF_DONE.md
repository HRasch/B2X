# Definition of Done (DoD) - B2Connect

**Status**: ENFORCED by @team-assistant  
**Last Updated**: 30. Dezember 2025  
**Version**: 1.0

---

## 🛑 Rule: "Ready to Merge" = All DoD Items Complete

When a developer claims "Ready to Merge":

```
@team-assistant MUST verify all items below.
If ANY item is incomplete:
  → Comment: "🛑 BLOCKED - Cannot merge. Missing: [items]"
  → Status: "DoD Incomplete" (red)
  → Action: Developer fixes, resubmits
```

---

## ✅ Mandatory DoD Items (All 5 Categories)

### 1️⃣ CODE QUALITY

**Build & Compilation**
- [ ] `dotnet build B2Connect.slnx` → **0 errors** (warnings OK)
- [ ] All projects compile without breaking changes
- [ ] No pre-processor directives causing build failures

**Tests & Coverage**
- [ ] Unit tests: **100% passing** (0 failures, 0 skipped)
- [ ] Integration tests: **100% passing**
- [ ] Test coverage: **≥80%** for new/modified code
- [ ] No test timeouts or flaky tests

**Code Review**
- [ ] Code reviewed and approved by @tech-lead
- [ ] All PR comments resolved
- [ ] No "request changes" blocking merge

**Code Quality Standards**
- [ ] Follows Onion Architecture (Core → App → Infra)
- [ ] Follows Wolverine pattern (NOT MediatR)
- [ ] No hardcoded secrets or credentials
- [ ] No PII in plaintext (encrypted with `IEncryptionService`)
- [ ] SQL queries use parameterized queries (no SQL injection)
- [ ] All async methods include `CancellationToken`
- [ ] Follows naming conventions (PascalCase classes, camelCase fields)

**Code Style**
- [ ] Follows project patterns from `copilot-instructions-*.md`
- [ ] No console.log or Debug.WriteLine in production code
- [ ] Comments explain *why*, not *what*
- [ ] XML documentation on public methods (C#) or JSDoc (TypeScript)

**Checklist Verification**
```bash
# Developer runs before requesting merge:
dotnet build B2Connect.slnx -v minimal        # 0 errors
dotnet test B2Connect.slnx -v minimal         # 100% pass
npm run lint                                  # 0 errors (frontend)
npm run type-check                            # 0 errors (TypeScript)
```

**Sign-Off**: "✅ Code Quality verified"

---

### 2️⃣ QA TESTING

**Unit Test Execution**
- [ ] Unit tests pass locally
- [ ] Unit tests pass in CI pipeline (GitHub Actions)
- [ ] All test assertions are meaningful (not just checking for not-null)

**Integration Testing** (End-to-End Scenarios)
- [ ] Component integrates with dependent services
- [ ] Database operations verified (insert, read, update, delete)
- [ ] External API calls mocked or tested with staging environment
- [ ] Data validation works correctly (null checks, length, format)

**Edge Case & Error Handling**
- [ ] API failure scenarios tested (timeout, 500 error, invalid response)
- [ ] Invalid input handling tested (empty strings, null, wrong type)
- [ ] Concurrent operation handling tested (race conditions)
- [ ] Boundary values tested (min/max, empty collections)

**Manual Testing** (Browser/Device)
- [ ] Tested on Chrome (latest)
- [ ] Tested on Firefox (latest)
- [ ] Tested on Safari (latest)
- [ ] Tested on mobile (iOS Safari, Chrome Android)
- [ ] Tested on tablet (iPad)

**Accessibility** (WCAG 2.1 AA)
- [ ] Lighthouse accessibility score: **≥90**
- [ ] Keyboard navigation: 100% functional (no mouse required)
- [ ] Screen reader: Tested with NVDA (Windows) or VoiceOver (macOS)
- [ ] Color contrast: All text ≥4.5:1 ratio
- [ ] Form labels: All inputs have proper labels
- [ ] Error messages: Announced to screen readers

**Performance**
- [ ] API response time: <500ms (P95)
- [ ] UI render time: <100ms for interactions
- [ ] No memory leaks (check DevTools)
- [ ] No infinite loops or hanging processes
- [ ] Caching working correctly (if applicable)

**Regression Testing**
- [ ] Existing tests still pass (no breaks)
- [ ] Existing features still work (manual spot-check)
- [ ] No new bugs introduced

**Sign-Off**: "✅ QA Testing verified" (by @qa-engineer or QA team)

---

### 3️⃣ DOCUMENTATION

**Code Comments**
- [ ] Public classes have XML documentation (C#) or JSDoc (TypeScript)
- [ ] Complex algorithms documented with explanation
- [ ] Business logic commented with *why* (not *what*)
- [ ] No outdated comments

**Project Documentation**
- [ ] README updated if new architecture/patterns introduced
- [ ] README.md in project root current
- [ ] Architecture decision recorded (if significant change)

**API Documentation**
- [ ] Swagger/OpenAPI specs generated and accurate
- [ ] All endpoints documented (parameters, response codes, examples)
- [ ] Request/response schemas defined
- [ ] Error responses documented (400, 404, 500, etc.)

**User-Facing Documentation**
- [ ] Feature documentation written (if user-visible)
- [ ] User guide updated (if workflow change)
- [ ] Screenshots/diagrams updated (if UI change)
- [ ] Help text in UI is clear and actionable

**Changelog & Release Notes**
- [ ] Changelog entry added (if user-visible change)
- [ ] Format: "- **Feature**: Description for end users"
- [ ] Breaking changes clearly marked
- [ ] Migration steps documented (if database schema change)

**Examples & Troubleshooting**
- [ ] Working code examples provided for new APIs
- [ ] Common errors documented with resolution
- [ ] FAQ updated (if applicable)
- [ ] Troubleshooting guide updated (if new deployment steps)

**Checklist Verification**
```
Developer documents:
- [ ] XML/JSDoc added to public members
- [ ] README.md current
- [ ] Swagger specs complete
- [ ] User guide updated (if applicable)
- [ ] Changelog entry added
```

**Sign-Off**: "✅ Documentation verified"

---

### 4️⃣ COMPLIANCE & SECURITY

**Security Review** (If Authentication, Encryption, or PII)
- [ ] Security review completed by @security-engineer
- [ ] No hardcoded API keys or secrets
- [ ] Secrets stored in Azure KeyVault (production) or `appsettings.json` (dev)
- [ ] PII encrypted at rest and in transit
- [ ] TenantId filtering enforced on all queries (multi-tenant safety)
- [ ] OWASP Top 10 checked:
  - [ ] No injection vulnerabilities
  - [ ] No broken authentication
  - [ ] No sensitive data exposure
  - [ ] No XML external entities (XXE)
  - [ ] No broken access control
  - [ ] No security misconfiguration

**Compliance Testing** (If P0 Feature: P0.6-P0.9)
- [ ] **P0.6 E-Commerce**: All 15 tests PASS
- [ ] **P0.7 AI Act**: All 15 tests PASS
- [ ] **P0.8 Accessibility (BITV)**: All 12 tests PASS
- [ ] **P0.9 E-Rechnung**: All 10 tests PASS

**Legal & Regulatory** (If Applicable)
- [ ] Legal review completed by @tech-lead or legal team
- [ ] GDPR requirements met (if processing personal data)
- [ ] Terms of Service compliance verified
- [ ] Data retention policies documented
- [ ] Vendor agreements reviewed (if using external services)

**Accessibility Compliance** (WCAG 2.1 AA)
- [ ] UX review completed by @ux-expert
- [ ] Lighthouse score ≥90
- [ ] Keyboard navigation 100% functional
- [ ] Screen reader compatible
- [ ] Color contrast verified

**Sign-Off**: "✅ Compliance & Security verified" (by @security-engineer if applicable)

---

### 5️⃣ FINAL SIGN-OFF

**Team Lead Approval**
- [ ] @tech-lead: "Approved for merge"
- [ ] Architecture review completed
- [ ] Code quality acceptable
- [ ] No technical debt introduced

**QA Approval**
- [ ] QA Engineer: "Testing complete, no blockers"
- [ ] All scenarios tested
- [ ] Accessibility verified
- [ ] Performance acceptable

**Product Owner Approval** (If User-Visible Feature)
- [ ] @product-owner: "Feature meets requirements"
- [ ] Acceptance criteria met
- [ ] No scope creep
- [ ] Ready for production

---

## 🚫 Definition of BLOCKED (Cannot Merge If Any Present)

These items **BLOCK MERGE** immediately:

```
❌ BLOCKERS - Automatic Merge Prevention:
  1. Build fails (dotnet build errors)
  2. Tests fail (< 100% pass rate)
  3. No code review approval
  4. Hardcoded secrets or credentials found
  5. PII stored in plaintext
  6. SQL injection vulnerability detected
  7. Accessibility score < 90
  8. P0 compliance tests failing
  9. No documentation
  10. Critical security review items unaddressed
```

If ANY blocker present:
```
@team-assistant status: 🛑 BLOCKED
Merge approval: ❌ DENIED
Action: "Fix [blockers] and resubmit"
```

---

## 📋 Merge Checklist Template

Copy this to your PR before requesting merge approval:

```markdown
## Definition of Done Checklist

### Code Quality
- [ ] Build: `dotnet build` → 0 errors
- [ ] Tests: 100% passing
- [ ] Coverage: ≥80%
- [ ] Code review: Approved

### QA Testing
- [ ] Unit tests: ✓
- [ ] Integration tests: ✓
- [ ] Manual testing: ✓
- [ ] Accessibility: ✓
- [ ] No regressions: ✓

### Documentation
- [ ] Code comments: ✓
- [ ] API docs: ✓
- [ ] User guide: ✓
- [ ] Changelog: ✓

### Compliance & Security
- [ ] Security review: ✓
- [ ] P0 tests: ✓ (if applicable)
- [ ] No hardcoded secrets: ✓
- [ ] No PII in plaintext: ✓

### Sign-Offs
- [ ] @tech-lead approval
- [ ] @qa-engineer approval
- [ ] @product-owner approval (if user-facing)

**Ready for Merge**: YES ✅
```

---

## 🔄 Process Diagram

```
Feature Complete
    ↓
Developer: "Ready to Merge"
    ↓
@team-assistant Verifies DoD:
    ├─ Build check (0 errors?)
    ├─ Test check (100% pass?)
    ├─ Code review (approved?)
    ├─ QA testing (done?)
    ├─ Documentation (complete?)
    └─ Compliance (pass?)
    ↓
IF all items ✓:
    → Comment: "✅ DoD VERIFIED - APPROVED FOR MERGE"
    → Merge: NOW
    
IF any item ❌:
    → Comment: "🛑 BLOCKED - Missing: [list]"
    → Merge: DENIED
    → Action: Developer fixes, resubmits
    ↓
After resubmission:
    → @team-assistant re-verifies
    → All ✓: MERGE
    → Any ❌: Back to "BLOCKED"
```

---

## 📞 Questions About DoD?

| Question | Answer |
|----------|--------|
| What if I can't do accessibility testing? | You must. It's a legal requirement (WCAG 2.1 AA by 28. Juni 2025). Use Lighthouse + keyboard navigation. |
| What if documentation is complex? | Document what you can now, schedule detailed guide for later. But basic API docs and code comments required. |
| What if tests take too long? | Run locally first (fast), then CI pipeline (slower OK). Don't ship without tests passing. |
| What if compliance testing fails? | Fix the code, re-run tests, verify 100% pass before resubmitting. No exceptions for P0 features. |
| What if @tech-lead is unavailable? | Ask @software-architect or escalate. But code review approval is mandatory. |

---

## ✅ Verification Command

```bash
# Run this before requesting merge:

# 1. Build
dotnet build B2Connect.slnx -v minimal

# 2. Test
dotnet test B2Connect.slnx -v minimal

# 3. Frontend Build (if modified)
cd Frontend/Store && npm run build
cd Frontend/Admin && npm run build

# 4. Linting
npm run lint

# 5. Type Check
npm run type-check

# 6. Accessibility Audit
npx @axe-core/cli http://localhost:5173
npx lighthouse http://localhost:5173 --only-categories=accessibility

# If all green → Ready to request merge approval
```

---

**Enforced By**: @team-assistant (automatic merge blocking if DoD incomplete)  
**Authority**: @process-assistant (updates to this checklist)  
**Last Updated**: 30. Dezember 2025  
**Status**: ACTIVE - No exceptions

