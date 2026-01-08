# Pull Request Workflow - Quick Guide

**Fast reference for developers opening pull requests**  
**For full details**: See [DEVELOPMENT_PROCESS_FRAMEWORK.md §7](../../../guides/DEVELOPMENT_PROCESS_FRAMEWORK.md#-pull-request-workflow)

---

## 📋 Before Opening PR (5 min checklist)

```bash
# 1. Build & test locally
dotnet build B2X.slnx
dotnet test B2X.slnx

# 2. Check coverage
dotnet test B2X.slnx --collect:"XPlat Code Coverage"
# Target: >= 80%

# 3. Code style
dotnet format

# 4. No secrets
grep -r "password\|secret\|api_key" backend/ --include="*.cs"
```

**Checklist**:
- [ ] Code compiles (no errors)
- [ ] All tests pass (100% of suite)
- [ ] Coverage >= 80%
- [ ] Code follows style guide
- [ ] No hardcoded secrets
- [ ] Agent changes marked with 🤖 (if applicable)
- [ ] Issue #XXX linked
- [ ] Documentation complete

---

## 🚀 Opening a PR (GitHub)

### PR Title Format

```
[TYPE](#scope): DESCRIPTION (#ISSUE)

Examples:
feat(catalog): implement VAT calculation service (#30)
fix(identity): prevent brute force attacks (#45)
test(catalog): add edge case tests (#31)
```

### PR Description Template

```markdown
### What
Brief description of changes

### Why
Reference the issue: Closes #30

### How
- Technical approach 1
- Technical approach 2

### Testing
- Unit tests: 5 new tests ✅
- Coverage: 85% ✅

### Documentation
- ✅ Swagger updated
- ✅ Changelog added

### Security
- ✅ No PII exposed
- ✅ Multi-tenant safe

### Checklist
- [x] Code compiles & tests pass
- [x] Coverage >= 80%
- [x] Documentation complete
- [x] Ready for review
```

---

## 👀 Code Review (What to Expect)

### 3 Roles Will Review

| Role | Checks | SLA |
|------|--------|-----|
| **Lead Developer** | Code quality, patterns, performance | < 24h |
| **Architecture** (if new pattern) | Design, scalability, security | < 24h |
| **Code Owner** (if different dev) | Business logic, acceptance criteria | < 24h |

### Review Comments

- ✅ **Approve**: Code is good to merge
- 📝 **Request Changes**: Must fix before merge
- 💬 **Comment**: Suggestions you can address

---

## 🔧 If Reviewer Says "Request Changes"

1. **Read the feedback** - Understand what needs fixing
2. **Fix your code** - Make the necessary changes
3. **Commit**: `git commit -m "fix: address reviewer feedback on X"`
4. **Push**: `git push`
5. **Reviewers notified automatically** ✅

---

## 🤖 If You Disagree with Feedback

1. Reply respectfully in the PR comment
2. Explain your reasoning with evidence
3. Link to docs, tests, or performance data
4. Let the reviewer decide
5. If still stuck, ask Tech Lead to mediate

---

## ✅ Approval & Merging

### When PR is Approved

```
✅ All 3 roles approved
✅ Build passing (GitHub CI)
✅ Tests passing (100%)
✅ Coverage >= 80%
✅ QA tested in staging
✅ No conflicts
```

### Click "Merge"

- Type: **Squash & Merge** (default)
- Message: GitHub auto-fills
- Branch: Auto-delete after merge ✅

**Done!** Issue auto-closes, deployment pipeline starts

---

## ❌ PR Rejected - What to Do

**If GitHub blocks PR**:

```
❌ Build failed      → Fix compilation errors
❌ Tests failed      → Fix failing tests
❌ Coverage too low  → Write more tests
❌ Secrets detected  → Remove hardcoded values
❌ Style violations  → Run dotnet format
```

**For each issue**:
1. Read the error message
2. Fix in your code
3. Re-push (`git push`)
4. Automated checks re-run
5. Loop until all green ✅

---

## 📊 Review Timeline

```
Day 1: Open PR
    ↓
Day 1-2: Lead Developer reviews
    ↓
Day 2: Architecture reviews (if needed)
    ↓
Day 2: Code Owner reviews (if different dev)
    ↓
Day 2-3: Developer addresses feedback (if any)
    ↓
Day 3: Final approval + QA testing
    ↓
Day 3-4: Merge to main
```

**SLA**: Each reviewer responds within 24 business hours

---

## 🎯 Common Mistakes

| Mistake | Fix |
|---------|-----|
| "Forgot to mark 🤖 AI code" | Go back, add marks, re-push |
| "Tests fail on CI but pass locally" | Check environment differences (DB, config) |
| "Merge conflicts" | Pull main, resolve locally, push |
| "Coverage dropped" | Write more tests, re-push |
| "Reviewer asked for changes 5 times" | Clarify requirements in issue next time |

---

## 🆘 Getting Help

**Question during review?**
→ Comment in PR (notify reviewer)

**Reviewer not responding?**
→ Mention in #dev-questions Slack channel

**Disagree with feedback?**
→ Ask Tech Lead to mediate

**Build broken?**
→ Check GitHub Actions logs, fix, re-push

---

## 📚 Full Documentation

See [DEVELOPMENT_PROCESS_FRAMEWORK.md §7](../../../guides/DEVELOPMENT_PROCESS_FRAMEWORK.md#-pull-request-workflow) for:
- Detailed role responsibilities
- Code review checklists
- Merge strategy options
- GitHub Actions automation
- PR rejection policies

---

**Remember**: 
- ✅ Build & test BEFORE opening PR
- ✅ Link to issue #XXX in description
- ✅ Respond to feedback respectfully
- ✅ Mark 🤖 AI-generated code
- ✅ Document APIs before merge

**Average PR time**: 3-4 days from opening to production  
**Fastest path**: Perfect PR on Day 1, merge Day 2, production Day 3
