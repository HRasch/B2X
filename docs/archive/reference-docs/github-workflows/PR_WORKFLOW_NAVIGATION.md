# Pull Request Workflow - Navigation & Quick Links

**Jump to what you need right now**

---

## 🏃 I want to...

### Open a PR in the next 5 minutes
→ [PR_WORKFLOW_QUICK_GUIDE.md](./docs/PR_WORKFLOW_QUICK_GUIDE.md) (read top section)

### Understand the complete PR workflow
→ [DEVELOPMENT_PROCESS_FRAMEWORK.md §7](./docs/DEVELOPMENT_PROCESS_FRAMEWORK.md#-pull-request-workflow) (detailed)

### Review a PR (as Lead Dev)
→ [DEVELOPMENT_PROCESS_FRAMEWORK.md §7 - Lead Developer](./docs/DEVELOPMENT_PROCESS_FRAMEWORK.md#1️⃣-lead-developer-always-required)

### Review architecture decisions
→ [DEVELOPMENT_PROCESS_FRAMEWORK.md §7 - Architecture Review](./docs/DEVELOPMENT_PROCESS_FRAMEWORK.md#2️⃣-architecture-review-if-new-pattern)

### Set up GitHub automation
→ [GITHUB_PR_SETUP.md](./docs/GITHUB_PR_SETUP.md) (Scrum Master task)

### Understand the 3-role approval process
→ [DEVELOPMENT_PROCESS_FRAMEWORK.md §7 - Code Review Roles](./docs/DEVELOPMENT_PROCESS_FRAMEWORK.md#code-review-roles--focus)

### Handle feedback from a reviewer
→ [PR_WORKFLOW_QUICK_GUIDE.md - Handling Feedback](./docs/PR_WORKFLOW_QUICK_GUIDE.md#-if-reviewer-says-request-changes)

### Disagree with code review feedback
→ [DEVELOPMENT_PROCESS_FRAMEWORK.md §7 - If you disagree](./docs/DEVELOPMENT_PROCESS_FRAMEWORK.md#if-you-disagree-with-feedback)

### Merge a PR
→ [PR_WORKFLOW_QUICK_GUIDE.md - Approval & Merging](./docs/PR_WORKFLOW_QUICK_GUIDE.md#-approval--merging)

### Fix a failed PR (build/test/coverage)
→ [PR_WORKFLOW_QUICK_GUIDE.md - PR Rejected](./docs/PR_WORKFLOW_QUICK_GUIDE.md#-pr-rejected---what-to-do)

### Understand branch protection rules
→ [GITHUB_PR_SETUP.md - Branch Protection Rules](./docs/GITHUB_PR_SETUP.md#-github-branch-protection-rules)

### Create GitHub Actions for PR checks
→ [GITHUB_PR_SETUP.md - GitHub Actions Workflows](./docs/GITHUB_PR_SETUP.md#-github-actions-workflows)

### Set up CODEOWNERS for auto-assignment
→ [GITHUB_PR_SETUP.md - GitHub CODEOWNERS File](./docs/GITHUB_PR_SETUP.md#-github-codeowners-file)

### See what's new in the framework
→ [PR_WORKFLOW_INTEGRATION_COMPLETE.md](./PR_WORKFLOW_INTEGRATION_COMPLETE.md)

---

## 📖 Document Quick Reference

| Document | Purpose | Read Time | For Whom |
|----------|---------|-----------|----------|
| **PR_WORKFLOW_QUICK_GUIDE.md** | Fast developer reference | 5 min | Developers |
| **DEVELOPMENT_PROCESS_FRAMEWORK.md §7** | Complete details | 30 min | Everyone |
| **GITHUB_PR_SETUP.md** | GitHub automation setup | 1 hour | Scrum Master |
| **PR_WORKFLOW_INTEGRATION_COMPLETE.md** | What was added & why | 15 min | Leadership |

---

## 🎯 Key Points to Remember

```
✅ Every commit goes through PR
✅ 3 roles must approve (Lead Dev + Architecture + Code Owner)
✅ All GitHub checks must pass (build, test, coverage)
✅ Code coverage must be >= 80%
✅ No hardcoded secrets allowed
✅ QA must test in staging before merge
✅ Squash merge to keep history clean
✅ Average PR time: 3-4 days
```

---

## 🚀 Getting Started

### If you're a Developer
1. Read [PR_WORKFLOW_QUICK_GUIDE.md](./docs/PR_WORKFLOW_QUICK_GUIDE.md) (5 min)
2. Use the checklist before opening PR
3. Reference quick guide for PR template
4. Ask in #dev-questions if confused

### If you're a Reviewer
1. Read [DEVELOPMENT_PROCESS_FRAMEWORK.md §7](./docs/DEVELOPMENT_PROCESS_FRAMEWORK.md#code-review-roles--focus)
2. Understand your role (Lead Dev / Architecture / Code Owner)
3. Use the role-specific checklist
4. Respond within 24 hours (SLA)

### If you're Scrum Master
1. Read [GITHUB_PR_SETUP.md](./docs/GITHUB_PR_SETUP.md) carefully
2. Execute the setup checklist
3. Test with a sample PR
4. Train the team
5. Track metrics weekly

---

## 📞 Need Help?

**About PR process**: Comment in PR or ask in #dev-questions

**About GitHub setup**: See [GITHUB_PR_SETUP.md](./docs/GITHUB_PR_SETUP.md) troubleshooting section

**About code review feedback**: Talk to reviewer in PR or escalate to Tech Lead

**About disagreement**: Explain reasoning, escalate if needed

**Emergency/Critical**: Ping @tech-lead or @scrum-master

---

**Version**: 1.0  
**Status**: ✅ Ready  
**Last Updated**: 29. Dezember 2025
