# PR Quality Gate - Quick Start Guide

**Date**: 2. Januar 2026  
**Status**: ✅ Ready for Use  
**Owner**: @DevOps + @TechLead

---

## 🎯 What Changed?

We now have **automated PR quality gates** that ensure:
- ✅ All code is tested (80%+ backend, 70%+ frontend)
- ✅ Security vulnerabilities are caught
- ✅ Code quality standards are met
- ✅ No secrets committed

**All checks are FREE** - no SonarQube costs!

---

## 🚀 For Developers

### Before Creating PR

**1. Run tests locally**:
```bash
# Backend
dotnet test B2X.slnx

# Frontend
cd frontend/Store && npm run test
cd frontend/Management && npm run test
```

**2. Check linting**:
```bash
# Backend
dotnet format --verify-no-changes

# Frontend
cd frontend/Store && npm run lint
cd frontend/Management && npm run lint
```

**3. Verify coverage**:
```bash
# Backend
dotnet test --collect:"XPlat Code Coverage"
# Look for coverage > 80%

# Frontend
npm run test:coverage
# Look for coverage > 70%
```

---

### Creating the PR

**1. Use the PR template** (auto-populated)
- Fill in all sections
- Check all boxes in the checklist
- Attach test evidence (screenshots for UI changes)

**2. Wait for CI checks** (automatic)
```
⏳ Fast Checks (2 min)
⏳ Unit Tests + Coverage (5 min)
⏳ Integration Tests (10 min)
⏳ E2E Tests (15 min)
⏳ Security Scan (10 min)
```

**3. Fix any failures**
- CI will comment with specific errors
- Fix and push again
- CI re-runs automatically

**4. Request review**
- Only after ALL checks pass ✅
- Reviewers auto-assigned via CODEOWNERS

---

## ✅ What the CI Checks

### Stage 1: Fast Checks (fails fast)
- ESLint (frontend)
- dotnet format (backend)
- TypeScript type checking
- **Secret detection** (TruffleHog)

### Stage 2: Unit Tests
- Backend tests with coverage ≥80%
- Frontend tests with coverage ≥70%
- **Fails if coverage below threshold**

### Stage 3: Integration Tests
- API integration tests
- Database integration tests
- Service integration tests

### Stage 4: E2E Tests
- Critical user flows (Playwright)
- Smoke tests

### Stage 5: Security & Quality
- **Mega-Linter**: 50+ linters checking code quality
- **GitHub CodeQL**: Advanced security scanning
- **npm audit**: Frontend dependency vulnerabilities
- **dotnet vulnerable**: Backend dependency vulnerabilities
- **License check**: No GPL/AGPL licenses

---

## 🔴 If CI Fails

### Linting Errors
```bash
# Auto-fix most issues
dotnet format
npm run lint:fix

# Check again
dotnet format --verify-no-changes
npm run lint
```

### Test Failures
```bash
# Run specific test
dotnet test --filter "TestName"
npm run test -- -t "test name"

# Debug locally
# Fix the issue
# Add/update tests
```

### Coverage Too Low
```bash
# Check coverage report
dotnet test --collect:"XPlat Code Coverage"
open test-results/coverage-report/index.html

# Add missing tests
# Especially for new code!
```

### Security Vulnerabilities
```bash
# Frontend
npm audit
npm audit fix  # Auto-fix if possible

# Backend
dotnet list package --vulnerable
# Update vulnerable packages
```

### Secrets Detected
```bash
# Remove the secret
# Add to .gitignore
# Rotate the secret immediately!
# Use environment variables instead
```

---

## 🎨 Tools You'll See

### Mega-Linter (Code Quality)
- Runs 50+ linters on your code
- Comments on PR with issues
- Provides auto-fix suggestions

**Configuration**: `.mega-linter.yml`

### GitHub CodeQL (Security)
- Scans for security vulnerabilities
- Detects: SQL injection, XSS, hardcoded secrets, etc.
- Results visible in Security tab

**Configuration**: `.github/codeql-config.yml`

### Coverage Comments
- CI will comment on your PR with coverage report
- Shows coverage increase/decrease
- Highlights uncovered lines

---

## 💡 Pro Tips

### Tip 1: Test Locally First
```bash
# Run the same checks CI will run
npm run lint
npm run type-check
npm run test
dotnet format --verify-no-changes
dotnet test
```

### Tip 2: Use Draft PRs
- Mark PR as "Draft" while working
- CI still runs (great for early feedback)
- Convert to "Ready for Review" when done

### Tip 3: Review CI Artifacts
- Click "Details" on failed checks
- Download coverage reports
- Download Mega-Linter reports

### Tip 4: Learn from Mega-Linter
- Mega-Linter explains WHY each issue is a problem
- Great learning opportunity
- Documentation links provided

### Tip 5: Commit Often
- Smaller commits = faster CI
- Easier to find what broke
- Better git history

---

## 🚫 What Will Block Your PR

| Issue | Solution |
|-------|----------|
| Test failures | Fix tests or implementation |
| Coverage < threshold | Add more tests |
| Linting errors | Run `dotnet format`, `npm run lint:fix` |
| TypeScript errors | Fix type issues |
| Security vulnerabilities | Update dependencies or fix code |
| Secrets detected | Remove secrets, use env vars |
| License violations | Remove incompatible dependencies |

---

## 🆘 Getting Help

### CI Stuck or Broken?
1. Check [GitHub Actions](../../actions) status
2. Ask @DevOps in PR comments
3. Check [ADR-020](../../.ai/decisions/ADR-020-pr-quality-gate.md)

### Coverage Calculation Wrong?
1. Download coverage artifact from CI
2. Review locally
3. Ask @QA in PR comments

### False Positive from Linter?
1. Document why it's a false positive
2. Ask @TechLead in PR comments
3. We can suppress with justification

### Emergency Merge Needed?
1. Document reason in PR
2. Get approval from @SARAH + @TechLead + domain expert
3. Create follow-up issue
4. Fix within 24 hours

---

## 📚 Additional Resources

- **PR Template**: `.github/pull_request_template.md`
- **Quality Gate ADR**: `.ai/decisions/ADR-020-pr-quality-gate.md`
- **Free Tools Guide**: `.ai/knowledgebase/tools-and-tech/free-code-quality-tools.md`
- **CODEOWNERS**: `.github/CODEOWNERS`
- **Mega-Linter Docs**: https://megalinter.io/
- **CodeQL Docs**: https://codeql.github.com/

---

## 🎯 Success Criteria

Your PR is ready to merge when:
- ✅ All CI checks pass (green)
- ✅ Coverage meets threshold
- ✅ All checklist items checked
- ✅ 2+ approvals from code owners
- ✅ All conversations resolved

---

## 📊 Metrics We Track

We monitor:
- % PRs passing first CI run
- Average PR merge time
- Test coverage trend
- Production bugs from missing tests
- Quality gate overrides (should be 0)

**Goal**: Ship high-quality code fast, with confidence

---

**Questions?** Ask @TechLead or @DevOps  
**Status**: ✅ Active and enforced  
**Cost**: $0/month (all free tools!)
