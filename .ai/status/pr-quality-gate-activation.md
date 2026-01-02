# 🚀 PR Quality Gate - Activation Checklist

**Date**: 2. Januar 2026  
**Status**: ✅ Ready to Activate  
**Owner**: @DevOps

---

## ✅ Prerequisites (Completed)

- [x] GitHub Actions workflow created (`.github/workflows/pr-quality-gate.yml`)
- [x] Mega-Linter configured (`.mega-linter.yml`)
- [x] CodeQL configured (`.github/codeql-config.yml`)
- [x] CODEOWNERS file created (`.github/CODEOWNERS`)
- [x] PR template updated (`.github/pull_request_template.md`)
- [x] Roslynator added to backend (`backend/Directory.Build.props`)
- [x] Documentation created:
  - ADR-020 (`.ai/decisions/ADR-020-pr-quality-gate.md`)
  - Free tools guide (`.ai/knowledgebase/tools-and-tech/free-code-quality-tools.md`)
  - Developer guide (`docs/guides/PR_QUALITY_GATE_GUIDE.md`)
- [x] Activation scripts:
  - `scripts/enable-pr-quality-gate.sh`
  - `scripts/pr-preflight-check.sh`

---

## 🎯 Activation Steps

### Step 1: Update Repository Name (Required)

Edit `scripts/enable-pr-quality-gate.sh`:
```bash
REPO="YOUR_ORG/YOUR_REPO"  # Update this line
```

### Step 2: Install GitHub CLI (if not installed)

```bash
# macOS
brew install gh

# Authenticate
gh auth login
```

### Step 3: Enable Branch Protection

```bash
# Run the activation script
./scripts/enable-pr-quality-gate.sh
```

This configures GitHub branch protection with:
- ✅ Required status checks (all CI stages)
- ✅ Minimum 2 approvals
- ✅ Code owner review required
- ✅ Dismiss stale reviews
- ✅ Linear history required
- ✅ Conversation resolution required

### Step 4: Test with Sample PR

Create a test PR to verify all checks run:

```bash
# Create test branch
git checkout -b test/pr-quality-gate

# Make a small change
echo "# Test" >> TEST.md
git add TEST.md
git commit -m "test: verify PR quality gate"
git push origin test/pr-quality-gate

# Create PR via CLI
gh pr create --title "Test: PR Quality Gate" --body "Testing automated quality checks"

# Watch checks run
gh pr checks --watch
```

### Step 5: Verify CI Checks

Expected checks (should all run):
- ✅ fast-checks (~2 min)
- ✅ unit-tests (~5 min)
- ✅ integration-tests (~10 min)
- ✅ e2e-tests (~15 min)
- ✅ security-compliance (~10 min)
- ✅ quality-gate (final)

### Step 6: Review and Adjust

If any check fails:
1. Review the check details in GitHub Actions
2. Adjust configuration if needed:
   - Linter rules: `.mega-linter.yml`
   - Coverage thresholds: `.github/workflows/pr-quality-gate.yml`
   - CodeQL queries: `.github/codeql-config.yml`

### Step 7: Train the Team

Share with developers:
- 📖 [PR Quality Gate Guide](docs/guides/PR_QUALITY_GATE_GUIDE.md)
- 🔧 Pre-flight check: `./scripts/pr-preflight-check.sh`
- 📋 PR template checklist

### Step 8: Monitor Metrics

Track in weekly retrospectives:
- % PRs passing first CI run
- Average PR merge time
- Number of quality gate bypasses
- Test coverage trend

---

## 🛠️ Developer Quick Commands

### Before Creating PR
```bash
# Run pre-flight checks locally
./scripts/pr-preflight-check.sh

# Fix common issues
dotnet format
cd frontend/Store && npm run lint:fix
cd frontend/Management && npm run lint:fix
```

### During PR Review
```bash
# Watch CI checks
gh pr checks --watch

# Download CI artifacts
gh run download <run-id>
```

---

## 🔧 Configuration Tuning

### If Linter Too Strict

Edit `.mega-linter.yml`:
```yaml
DISABLE_LINTERS:
  - SPELL_CSPELL
  - COPYPASTE_JSCPD
```

### If Coverage Threshold Too High

Edit `.github/workflows/pr-quality-gate.yml`:
```yaml
# Adjust thresholds
Backend: 70% (instead of 80%)
Frontend: 60% (instead of 70%)
```

### If Checks Too Slow

Optimize stages:
- Increase parallel execution
- Cache more dependencies
- Reduce E2E test scope

---

## 🚨 Emergency Bypass Process

**Only for production hotfixes!**

1. Document reason in PR description
2. Get approval from:
   - @SARAH
   - @TechLead
   - Domain expert (@Backend/@Frontend/@Security)
3. Create follow-up issue to fix quality gaps
4. Admin can bypass branch protection (creates audit log)
5. **Fix quality issues within 24 hours**

---

## 📊 Success Metrics (Week 1)

Target after 1 week:
- [ ] >80% PRs pass first CI run
- [ ] <2 day average PR merge time
- [ ] 0 quality gate bypasses
- [ ] 0 production bugs from missing tests
- [ ] All developers trained

---

## 🎉 Activation Checklist

- [ ] Repository name updated in `enable-pr-quality-gate.sh`
- [ ] GitHub CLI installed and authenticated
- [ ] Branch protection enabled (run `enable-pr-quality-gate.sh`)
- [ ] Test PR created and verified
- [ ] All CI checks passing
- [ ] Team trained on workflow
- [ ] Metrics dashboard set up
- [ ] First real PR merged successfully

---

## 📞 Support

**Issues with CI**: @DevOps  
**Coverage questions**: @QA  
**Linter false positives**: @TechLead  
**Security concerns**: @Security  
**Process questions**: @SARAH

---

## 🔄 Rollback Plan

If major issues occur:

```bash
# Disable branch protection temporarily
gh api -X DELETE "/repos/YOUR_ORG/YOUR_REPO/branches/main/protection"

# Fix issues
# Re-enable when ready
./scripts/enable-pr-quality-gate.sh
```

---

**Status**: 🎯 Ready to activate  
**Total Setup Time**: ~30 minutes  
**Cost**: $0/month  
**Next Step**: Run `./scripts/enable-pr-quality-gate.sh`
