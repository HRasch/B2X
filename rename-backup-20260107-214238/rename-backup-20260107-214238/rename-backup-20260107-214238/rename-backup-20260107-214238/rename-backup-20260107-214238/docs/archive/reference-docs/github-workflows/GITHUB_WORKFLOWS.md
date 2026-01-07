# GitHub & Development Workflows

**Version:** 1.0  
**Last Updated:** 27. Dezember 2025  
**Status:** Active

---

## Table of Contents

1. [GitHub Project Management](#github-project-management)
2. [Branch Strategy](#branch-strategy)
3. [Commit Strategy](#commit-strategy)
4. [Pull Request Workflow](#pull-request-workflow)
5. [Code Review Process](#code-review-process)
6. [Release Management](#release-management)
7. [Issue Management](#issue-management)
8. [GitHub Actions CI/CD](#github-actions-cicd)

---

## GitHub Project Management

### Repository Structure

```
B2Connect/
├── .github/
│   ├── ISSUE_TEMPLATE/
│   │   ├── p0-security-issue.md
│   │   ├── feature-request.md
│   │   └── bug-report.md
│   ├── pull_request_template.md
│   ├── workflows/
│   │   ├── ci-backend.yml
│   │   ├── ci-frontend.yml
│   │   ├── security-scan.yml
│   │   └── deploy.yml
│   └── CONTRIBUTING.md
├── docs/
│   ├── APPLICATION_SPECIFICATIONS.md ← You are here
│   ├── GITHUB_WORKFLOWS.md
│   ├── COMPREHENSIVE_REVIEW.md
│   └── ...
├── backend/
├── frontend-admin/
├── frontend-store/
└── README.md
```

### Project Board Setup

**Three GitHub Projects:**

#### 1. **P0 Critical Week** (Dec 30 - Jan 3)
```
Status: In Progress
Priority: CRITICAL

Issues:
- P0.1: Remove hardcoded JWT secrets
- P0.2: Fix CORS configuration
- P0.3: Implement encryption at rest
- P0.4: Add audit logging

View: Table (grouped by status)
Automation: Auto-move to "Done" when PR merged
```

#### 2. **Sprint Backlog** (Rolling 2-week sprints)
```
Status: Backlog → In Progress → Review → Done
Priority: P1, P2, P3

Issues: Features, bugs, improvements
View: Board or Table
Automation: Auto-assign, auto-label
```

#### 3. **Roadmap** (Quarterly planning)
```
Status: Planned → In Sprint → Done
Timeline: 3-month planning
View: Table (grouped by quarter)
```

---

## Branch Strategy

### Git Flow with Staging

```
main
├── (production, protected)
├── hotfix/p0-security-issues ← Current week
│   ├── Contains: P0.1, P0.2, P0.3, P0.4
│   └── Merges to: main + develop
│
develop
├── (staging preparation)
├── feature/gdpr-compliance
├── feature/testing-framework
└── bugfix/cors-configuration

Release branches (temporary):
└── release/v1.1.0
    ├── Version bumps
    ├── Documentation
    └── Merges to: main + develop
```

### Naming Conventions

```
Format: <type>/<scope>-<description>

Types:
  - feature/     ← New features
  - bugfix/      ← Bug fixes
  - hotfix/      ← Critical fixes (from main)
  - refactor/    ← Code refactoring
  - docs/        ← Documentation
  - test/        ← Test additions
  - perf/        ← Performance improvements
  - sec/         ← Security improvements

Examples:
  ✅ feature/gdpr-data-export
  ✅ hotfix/p0-jwt-secrets
  ✅ bugfix/cors-validation
  ✅ refactor/service-layer
  ❌ fix-cors (too vague)
  ❌ Update_Config (wrong format)
```

### Branch Protection Rules

**Main Branch Protection:**
```
✅ Require pull request reviews (2 required)
✅ Dismiss stale pull request approvals
✅ Require branches to be up to date
✅ Require status checks to pass:
   - CI Backend
   - CI Frontend
   - Security Scan
   - Code Coverage > 80%
✅ Require code quality review (CodeQL)
✅ Lock branch after merge until CI passes
```

**Develop Branch Protection:**
```
✅ Require pull request reviews (1 required)
✅ Require status checks to pass:
   - CI Backend
   - CI Frontend
   - Code Coverage > 40% (minimum)
```

---

## Commit Strategy

### Commit Message Format

**Conventional Commits Standard:**

```
<type>(<scope>): <subject>

<body>

<footer>
```

### Type Reference

| Type | Purpose | Example |
|------|---------|---------|
| feat | New feature | `feat(auth): add JWT refresh token` |
| fix | Bug fix | `fix(cors): allow production domain` |
| docs | Documentation | `docs: update API specification` |
| style | Code style (no logic change) | `style: format code` |
| refactor | Code refactoring | `refactor(auth): extract token logic` |
| perf | Performance improvement | `perf(db): add index to user email` |
| test | Test additions | `test(auth): add JWT validation tests` |
| chore | Build/config/deps | `chore: update .NET to 10.0.1` |
| ci | CI/CD changes | `ci: add security scanning workflow` |

### Subject Line Rules

- ✅ Imperative mood ("add" not "added" or "adds")
- ✅ Lowercase first letter
- ✅ No period at end
- ✅ Maximum 50 characters
- ✅ Specific to changes

```
✅ fix(jwt): remove hardcoded secret
❌ FIX: removed hardcoded secret.
❌ Fixed stuff
❌ fix: make JWT work properly everywhere
```

### Body Rules

- ✅ Explain **what** and **why**, not **how**
- ✅ Wrap at 72 characters
- ✅ Separate from subject with blank line
- ✅ Use bullet points for multiple items
- ✅ Reference issues: "Fixes #123"

```
fix(cors): load origins from configuration

CORS origins were hardcoded, preventing production deployment.
Moved to configuration-driven approach:
- Separate config per environment
- Development: localhost:5174
- Production: admin.b2connect.com
- Validation on startup

This allows secure production configuration without code changes.

Fixes #45
Related to #40 #41
BREAKING: Requires CORS__ALLOWEDORIGINS environment variable
```

### Footer Rules

```
Fixes: #123 (closes issue on merge)
Related: #124, #125
BREAKING CHANGE: Description of breaking change
Co-authored-by: Name <email@example.com>
```

### Example Commits

**Feature Commit:**
```
feat(auth): implement encryption service

Add new EncryptionService for field-level encryption:
- AES-256 encryption with IV
- Key management from configuration
- Value converters for EF Core
- Support for nullable fields

Fixes #34
```

**Fix Commit:**
```
fix(jwt): remove hardcoded secret

Move JWT secret from hardcoded value to environment variable:
- Remove default fallback in production
- Load from IConfiguration
- Validate minimum 32-character length
- Add .env.example documentation

Fixes #23
BREAKING: ASPNETCORE_JWT_SECRET environment variable required
```

**Security Fix Commit:**
```
fix(security): add rate limiting to auth endpoints

Prevent brute force attacks:
- 5 failed login attempts = 15 min lockout
- Rate limit: 100 requests/min per IP
- Store rate limit state in Redis
- Log all rate limit violations

Fixes #18
```

### Commit Atomicity

Each commit should be:
- ✅ **Atomic:** Single logical change
- ✅ **Testable:** Tests pass for that commit
- ✅ **Independent:** Can be reverted without affecting others
- ✅ **Minimal:** Only necessary changes

```
BAD: One commit with everything
  - Added JWT encryption
  - Fixed CORS
  - Updated tests
  - Added documentation
  - Changed database schema

GOOD: Four separate commits
  - fix(jwt): implement encryption
  - fix(cors): environment configuration
  - test(jwt): add encryption tests
  - docs(security): update security guide
```

---

## Pull Request Workflow

### PR Creation Checklist

**Before Creating PR:**

```
Code Changes:
- [ ] Code written and locally tested
- [ ] All tests passing locally
- [ ] No merge conflicts with develop
- [ ] Code formatted consistently
- [ ] No console.log/Debug.WriteLine left in
- [ ] No commented-out code

Documentation:
- [ ] Commit messages follow convention
- [ ] PR description filled out
- [ ] Related issues linked
- [ ] Breaking changes documented
- [ ] Update CHANGELOG if applicable

Branch:
- [ ] Created from develop (not main)
- [ ] Branch name follows convention
- [ ] Push all commits to remote
```

### PR Template Completion

**Use the PR Template (.github/pull_request_template.md):**

```markdown
## What does this PR do?
Clear description of changes and rationale

## Type of Change
- [x] Bug fix
- [ ] Feature
- [ ] Security fix

## Testing
- [x] Unit tests added
- [x] Manual testing completed

## Security Checklist
- [x] No hardcoded secrets
- [x] Input validation present
- [x] Authentication required

## Checklist
- [x] Tests passing
- [x] Documentation updated
- [x] No breaking changes (or documented)
```

### PR Size Guidelines

| Size | Scope | Review Time | Merge Criteria |
|------|-------|-------------|----------------|
| XS (< 100 lines) | Single fix | 15 min | 1 approval |
| S (100-300 lines) | Single feature | 30 min | 1 approval |
| M (300-600 lines) | Feature + tests | 1 hour | 2 approvals |
| L (600-1000 lines) | Complex feature | 2+ hours | 2 approvals + Lead |
| XL (> 1000 lines) | Major refactor | 3+ hours | 2+ approvals + Architecture Review |

**Guidelines:**
- ✅ Keep PRs < 400 lines when possible
- ✅ Break large changes into smaller PRs
- ✅ Link related PRs together

### PR Naming Convention

```
Format: [TYPE] Short description

Examples:
✅ [FEATURE] GDPR data export endpoint
✅ [BUGFIX] CORS configuration loading
✅ [SECURITY] P0.1 - Remove JWT secrets
✅ [TEST] Encryption service tests
✅ [DOCS] Update API specifications
❌ Fix stuff
❌ random changes
```

---

## Code Review Process

### Review Checklist

**Reviewer Responsibilities:**

```
Functionality:
- [ ] Code does what PR describes
- [ ] Logic is correct
- [ ] Edge cases handled
- [ ] Error handling present

Quality:
- [ ] Code is readable/maintainable
- [ ] Follows project conventions
- [ ] No code duplication
- [ ] Performance acceptable

Security:
- [ ] No hardcoded secrets
- [ ] Input validation present
- [ ] Authorization checks
- [ ] No SQL injection risks
- [ ] Sensitive data encrypted

Testing:
- [ ] Tests added/updated
- [ ] Test coverage adequate
- [ ] Tests are meaningful
- [ ] Manual testing verified

Documentation:
- [ ] Code comments for complex logic
- [ ] README/docs updated
- [ ] API docs updated
- [ ] Breaking changes documented
```

### Review Comments

**Good Review Comment:**
```
## Code Style
In `UserService.cs` line 45, the variable name `usr` is unclear.
Consider `user` or `currentUser` for better readability.

Reference: Project style guide - Use full names for variables
```

**Bad Review Comment:**
```
This is bad code.
```

### Approval Requirements

- **For main branch:** 2 approvals minimum
- **For develop branch:** 1 approval minimum
- **Special cases:**
  - Architecture changes: Lead architect approval required
  - Security changes: Security reviewer approval required
  - Database changes: DB expert approval required

### Addressing Review Comments

**Required Actions:**
1. Respond to every comment
2. Make requested changes
3. Re-request review
4. Don't dismiss feedback

**Options for Response:**
```
✅ "Done" - Made the change
✅ "Good idea, updated in <commit>" - Linked change
✅ "Disagree because..." - Explain your perspective
✅ "Resolved by commit abc123" - Link to change
❌ "Will do later" - No, fix before merge
❌ Dismiss without response - Don't do this
```

---

## Release Management

### Version Strategy

**Semantic Versioning: MAJOR.MINOR.PATCH**

```
1.2.3
│ │ └─ PATCH (hotfixes, security patches)
│ └─── MINOR (new features, non-breaking)
└───── MAJOR (breaking changes, major refactors)

Examples:
1.0.0 ← Initial release
1.0.1 ← Hotfix/patch
1.1.0 ← New features added
2.0.0 ← Breaking changes
```

### Release Process

**For Monthly Release (e.g., v1.1.0):**

```
Step 1: Create Release Branch (Day 1)
  git checkout -b release/v1.1.0
  - Update version in package.json, .csproj files
  - Update CHANGELOG.md
  - Commit: "chore(release): prepare v1.1.0"
  - Push to remote
  - Create PR to main

Step 2: Final Testing (Day 1-2)
  - Run full test suite
  - Deploy to staging
  - Smoke tests
  - Performance tests

Step 3: Approval & Merge (Day 2)
  - PR approval from 2+ reviewers
  - Merge to main
  - Create GitHub Release with changelog
  - Tag: git tag v1.1.0

Step 4: Deploy to Production (Day 2-3)
  - Manual approval required
  - Deploy to production
  - Monitor error rates
  - Notify stakeholders

Step 5: Merge back to develop
  - git checkout develop
  - git merge main
  - Resolve any conflicts
  - Push to remote
```

### Hotfix Process (for critical production issues)

```
Step 1: Create Hotfix Branch
  git checkout -b hotfix/v1.0.1
  git checkout main (branch from main, not develop)

Step 2: Fix Issue
  - Minimal changes only
  - Add test for issue
  - Update patch version

Step 3: Test & Deploy
  - Full test suite passing
  - Deploy to production
  - Monitor error rates

Step 4: Merge Back
  - Merge to main
  - Merge to develop
  - Tag: v1.0.1
```

### CHANGELOG Format

```markdown
# Changelog

All notable changes to this project will be documented in this file.

## [1.1.0] - 2026-01-03

### Added
- GDPR data export endpoint
- User consent tracking
- Email verification workflow

### Fixed
- CORS configuration loading
- JWT token refresh logic
- Database encryption for PII

### Changed
- API response format (see migration guide)
- Configuration structure

### Security
- Hardcoded JWT secrets removed
- Rate limiting implemented
- Audit logging added

### Deprecated
- /v1/auth/login endpoint (use /v2/auth/login)

### Removed
- Legacy password reset flow

## [1.0.0] - 2025-12-01

Initial release
```

---

## Issue Management

### Issue Lifecycle

```
Triage → Backlog → In Progress → Review → Done

Triage:
- All new issues get reviewed
- Assigned priority (P0, P1, P2, P3)
- Assigned to developer or backlog
- Estimated effort

Backlog:
- Ready for development
- Clear acceptance criteria
- Linked to related issues

In Progress:
- Assigned to developer
- PR created and linked
- Status updates as needed

Review:
- PR under review
- Code review comments addressed
- Tests verified

Done:
- PR merged to develop/main
- Issue closed
- Released in next version
```

### Priority & Effort Labels

**Priority Labels:**
```
P0 - CRITICAL
  - Security issues
  - Data loss/corruption
  - Complete outages
  - GDPR violations
  Example: "Hardcoded JWT secret in production"

P1 - HIGH
  - Major functionality broken
  - Significant performance degradation
  - Important feature missing
  Example: "CORS prevents production API access"

P2 - MEDIUM
  - Minor functionality issue
  - Nice-to-have improvements
  Example: "API response format inconsistency"

P3 - LOW
  - Polish/cosmetic
  - Technical debt
  - Documentation improvements
  Example: "Update README with new setup steps"
```

**Effort Estimate Labels:**
```
estimate/xs   - < 2 hours
estimate/s    - 2-4 hours
estimate/m    - 4-8 hours
estimate/l    - 8-16 hours
estimate/xl   - 16+ hours (break down into smaller issues)
```

### Example Issues

**P0 Issue:**
```
Title: [P0] Hardcoded JWT secrets exposed in production

Priority: P0 - CRITICAL
Labels: security, p0-critical-week
Effort: estimate/m

Description:
JWT secrets are hardcoded in Program.cs files and exposed
in version control. This could lead to token forgery.

Security Impact:
- Any developer with access can forge tokens
- Secret could be exposed via GitHub history
- Violates OWASP Top 10 (#5 Broken Access Control)

Acceptance Criteria:
- [ ] All hardcoded secrets removed from code
- [ ] Secrets loaded from environment variables
- [ ] Production configuration requires env vars
- [ ] All tests passing
- [ ] Documentation updated with setup steps

Related: #23, #24, #25

Links to: CRITICAL_ISSUES_ROADMAP.md#p01
```

**P1 Issue:**
```
Title: CORS configuration blocks production deployment

Priority: P1 - HIGH
Labels: bugfix, api, production-blocker
Effort: estimate/m

Description:
CORS is hardcoded to localhost domains. Production API
cannot be called from frontend due to CORS restrictions.

Current Configuration:
- http://localhost:5174
- http://localhost:5173
- https://localhost:5173

Required:
- Production domain: https://admin.b2connect.com
- Dynamic configuration per environment

Acceptance Criteria:
- [ ] CORS origins loaded from configuration
- [ ] appsettings.Development/Production separate
- [ ] No hardcoded origins in code
- [ ] Tests verify configuration loading
- [ ] Documentation updated

Steps to Test:
1. Deploy to production
2. Call API from https://admin.b2connect.com
3. Verify request succeeds

Related: #24, #25
```

---

## GitHub Actions CI/CD

### Workflows Overview

```
On every commit to any branch:
  → lint-backend.yml (quick checks)
  → unit-tests-backend.yml
  → security-scan.yml (CodeQL)

On every PR:
  → All above workflows
  → Build & test reporting
  → Comment results on PR

On merge to develop:
  → Deploy to staging
  → Run integration tests
  → Smoke tests

On merge to main (release):
  → Build release artifacts
  → Deploy to production
  → Notify stakeholders
```

### CI/CD Checklists

**Backend CI/CD Workflow:**

```yaml
name: CI Backend
on: [push, pull_request]

jobs:
  lint:
    - Run: dotnet format --verify-no-changes
    
  build:
    - Run: dotnet build backend/B2Connect.slnx
    - Report: Build status on PR
    
  test:
    - Run: dotnet test backend/B2Connect.slnx
    - Report: Test results + coverage
    - Fail if: Coverage < 40%
    
  security:
    - Run: CodeQL analysis
    - Run: SAST scanning
    - Report: Security issues on PR
    
  analysis:
    - Run: SonarQube analysis (optional)
    - Report: Code quality metrics
```

**Frontend CI/CD Workflow:**

```yaml
name: CI Frontend
on: [push, pull_request]

jobs:
  lint:
    - Run: npm run lint --prefix frontend-admin
    
  build:
    - Run: npm run build --prefix frontend-admin
    - Report: Build status on PR
    
  test:
    - Run: npm run test --prefix frontend-admin
    - Report: Test results + coverage
    
  e2e:
    - Run: npm run test:e2e --prefix frontend-admin
    - Report: E2E test results
```

---

## Development Best Practices

### Daily Development Workflow

```
Morning (09:00):
1. Pull latest from develop
2. Create new feature branch
3. Review day's tasks

During Day:
1. Commit regularly with clear messages
2. Run tests locally before pushing
3. Push to remote daily
4. Create PR early (mark as draft)

End of Day (17:00):
1. Push all changes
2. Request review
3. Update issue status
4. Plan next day
```

### Before Creating PR

```bash
# 1. Update from develop
git fetch origin
git rebase origin/develop

# 2. Run all tests locally
dotnet test backend/B2Connect.slnx
npm run test --prefix frontend-admin

# 3. Run linting
dotnet format
npm run lint --prefix frontend-admin

# 4. Verify no secrets exposed
git grep -i 'secret\|password\|api.?key' | grep -v '.example'

# 5. Check code quality
dotnet build backend/B2Connect.slnx --no-warnings

# 6. Commit and push
git push origin feature/your-branch

# 7. Create PR on GitHub
```

### Handling Merge Conflicts

```bash
# 1. Update develop (remote)
git fetch origin

# 2. Rebase on latest develop
git rebase origin/develop

# 3. Fix conflicts in editor
# - Mark sections with <<<<<<, ======, >>>>>>
# - Choose desired version
# - Delete conflict markers

# 4. Resume rebase
git add .
git rebase --continue

# 5. Force push (only on your feature branch!)
git push origin feature/your-branch --force

# 6. PR should auto-update
```

---

## Repository Guidelines

### DO ✅

```
✅ Write clear commit messages
✅ Create small, focused PRs
✅ Test locally before pushing
✅ Request review early and often
✅ Respond to review comments quickly
✅ Link related issues in PR
✅ Update documentation
✅ Follow naming conventions
✅ Keep branches up to date with develop
✅ Delete merged branches
```

### DON'T ❌

```
❌ Commit directly to main
❌ Force push to develop/main
❌ Leave PRs unreviewed for > 1 day
❌ Ignore failing tests
❌ Commit secrets or credentials
❌ Use vague commit messages ("fix stuff")
❌ Create mega-PRs with 10+ files
❌ Leave commented-out code
❌ Dismiss review feedback
❌ Keep old branches cluttering the repo
```

---

## Emergency Procedures

### Production Issue Hotfix

```
Step 1: Create hotfix branch
  git checkout -b hotfix/critical-issue

Step 2: Fix with minimal changes
  - Change only what's necessary
  - Add test case
  - Update commit message with impact

Step 3: Deploy to production
  - Approval from lead developer
  - Manual deployment
  - Monitor error rates

Step 4: Create PR
  - PR to both main AND develop
  - Mark as URGENT
  - Request immediate review

Step 5: Merge & Tag
  - Merge to main
  - Create v1.0.1 tag
  - Merge to develop
  - Close related issues
```

### Rollback Procedure

```
If production deploy fails:

Step 1: Identify issue
  - Check error logs
  - Review recent deployments
  - Identify bad commit/release

Step 2: Rollback
  git revert <bad-commit>
  OR
  git checkout <previous-tag>

Step 3: Deploy previous version
  - Deploy stable version
  - Verify errors resolved
  - Notify stakeholders

Step 4: Investigation
  - Debug issue offline
  - Create fix in feature branch
  - Test thoroughly
  - Re-deploy with fix
```

---

## References

- [GitHub Flow Guide](https://guides.github.com/introduction/flow/)
- [Conventional Commits](https://www.conventionalcommits.org/)
- [Semantic Versioning](https://semver.org/)
- [CONTRIBUTING.md](../../../../CONTRIBUTING.md)
- [APPLICATION_SPECIFICATIONS.md](./APPLICATION_SPECIFICATIONS.md)

---

**Document Owner:** DevOps & Engineering Team  
**Last Reviewed:** 27. Dezember 2025  
**Next Review:** 10. Januar 2026
