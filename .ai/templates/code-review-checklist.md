# Simplified Code Review Checklist

**Purpose:** Streamlined checklist for faster, more focused reviews. Focus on critical issues only.

## 🚨 Critical (Blockers)
- [ ] No security vulnerabilities (SQL injection, XSS, auth bypass)
- [ ] No breaking API changes without migration plan
- [ ] Tests pass and cover new functionality
- [ ] No hardcoded secrets or credentials

## ⚠️ Important (Should Fix)
- [ ] Code follows established patterns and conventions
- [ ] Error handling is appropriate
- [ ] Performance considerations addressed
- [ ] Documentation updated for public APIs

## 💡 Optional (Nice to Have)
- [ ] Code could be simplified or refactored
- [ ] Additional test coverage possible
- [ ] Minor style improvements

## 📝 Review Guidelines
- **Low-risk changes** (bug fixes, refactoring): 1 reviewer required
- **High-risk changes** (new features, security): 2 reviewers required
- **Auto-approve** for urgent hotfixes (document reasoning)
- **Time limit:** Complete reviews within 24 hours
- **Focus on impact:** Prioritize bugs and security over style

## 🔧 Automation Handles
- Linting and formatting (ESLint, Stylelint, Roslyn)
- Basic security scans (CodeQL, detect-secrets)
- Test coverage (minimum thresholds enforced)
- Commit message standards (pre-commit hooks)

**Goal:** 30% faster reviews while maintaining quality. Use this checklist in PR templates.