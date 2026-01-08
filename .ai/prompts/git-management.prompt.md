---
docid: UNKNOWN-184
title: Git Management.Prompt
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

# 🔀 GIT_MANAGEMENT - Branching Strategy & Workflow Design

**Trigger**: Git workflow design, branching strategy, code review process setup
**Coordinator**: @GitManager
**Output**: Git workflow documentation, branch protection rules, code review standards
**Integration**: GitHub, GitHub CLI, branch protection rules, CI/CD

---

## Quick Start

### Workflow Design
```
@GitManager: /git-management
Type: [workflow-design | branch-protection | code-review | troubleshooting]
Current Model: [current branching strategy or "new"]
Team Size: [number of developers]
Release Cadence: [continuous | weekly | monthly]
Compliance: [GDPR | SOC2 | PCI | none]
```

### GitHub CLI Integration
```bash
# Get current branch info
gh repo view owner/repo

# View branch protection rules
gh api repos/owner/repo/branches/main/protection

# Update branch protection
gh api -X PUT repos/owner/repo/branches/main/protection \
  -f required_status_checks='{"strict": true}'
```

---

## Git Workflow Phases

### Phase 1️⃣: Workflow Design (1 day)

**Objective**: Design Git workflow aligned with team processes, release cadence, and compliance

#### Analysis Meeting

```markdown
## Git Workflow Design - Analysis

### Team Assessment
- Team size: [number]
- Distribution: [co-located / distributed / hybrid]
- Time zones: [relevant zones]
- Git experience: [beginner / intermediate / expert]

### Project Assessment
- Repository type: [monorepo / polyrepo / multi-service]
- Release frequency: [continuous / daily / weekly / monthly]
- Production criticality: [high / medium / low]
- Deployment targets: [dev / staging / production]

### Current State
- Existing branching model: [if applicable]
- Current pain points: [conflicts, slow reviews, etc.]
- Compliance requirements: [GDPR, SOC2, PCI, ISO, audit trail]
- Integration with CI/CD: [GitHub Actions, Jenkins, etc.]

### Goals for New Workflow
1. [Goal 1 - e.g., reduce merge conflicts]
2. [Goal 2 - e.g., faster code review cycle]
3. [Goal 3 - e.g., safer deployments]
```

#### Workflow Design Document

**Output**: `.ai/decisions/{sprint}/git-workflow-design.md`

```markdown
# Git Workflow Design - [Project/Team Name]

## 1. Branching Strategy

### Model Selection: [Git Flow / GitHub Flow / Trunk-Based Development]

**Git Flow** (For complex releases)
- Main branch: `main` (production)
- Development branch: `develop` (integration)
- Feature branches: `feature/*`
- Release branches: `release/*`
- Hotfix branches: `hotfix/*`

**GitHub Flow** (For continuous deployment)
- Main branch: `main` (always deployable)
- Feature branches: `feature/*`, `bugfix/*`
- Short-lived PR-based workflow
- Direct merge to main

**Trunk-Based Development** (For high-performing teams)
- Single main branch
- Very short-lived feature branches (< 1 day)
- Frequent merges to main
- Feature flags for incomplete features

### Branch Naming Convention
- `feature/[issue-id]-[description]`
  - Example: `feature/AUTH-101-jwt-validation`
- `bugfix/[issue-id]-[description]`
  - Example: `bugfix/API-234-null-pointer-exception`
- `hotfix/[issue-id]-[description]`
  - Example: `hotfix/PROD-456-database-connection-leak`
- `docs/[description]`
  - Example: `docs/add-authentication-guide`
- `refactor/[description]`
  - Example: `refactor/simplify-auth-service`

### Branch Lifespan
- Feature branches: < 1 week (prefer < 3 days)
- Release branches: < 2 weeks
- Hotfix branches: < 1 day
- Stale branches: Delete after merge (auto-cleanup)

## 2. Commit Message Convention

### Format: Conventional Commits
```
<type>(<scope>): <subject>

<body>

<footer>
```

### Type Prefixes
- `feat` - New feature
- `fix` - Bug fix
- `docs` - Documentation
- `style` - Code style (formatting, missing semicolons)
- `refactor` - Code refactor (no feature/fix)
- `perf` - Performance improvement
- `test` - Test additions or updates
- `chore` - Build, dependencies, tooling

### Scope (Optional)
- Module/component name: `auth`, `api`, `ui-button`
- Or area: `backend`, `frontend`, `database`

### Examples
```
feat(auth): add JWT token refresh mechanism
fix(api): resolve null pointer in user endpoint
docs(readme): update installation instructions
test(auth): add test coverage for token validation
refactor(api): simplify request validation logic
perf(ui): optimize component re-render
```

### Best Practices
- Imperative mood: "add feature", not "added feature"
- Lowercase subject line
- No period at end of subject
- Separate subject from body with blank line
- Limit subject to 50 characters
- Wrap body at 72 characters
- Explain WHAT and WHY, not HOW

## 3. Code Review Process

### Pull Request (PR) Requirements

#### Pre-submission Checklist
- [ ] Branch created from latest `main` or `develop`
- [ ] All commits follow Conventional Commits
- [ ] No WIP or temporary commits
- [ ] Local tests pass
- [ ] Code follows project style guide
- [ ] No hardcoded secrets or credentials
- [ ] Documentation updated if needed
- [ ] Changelog entry added (if applicable)

#### PR Description Template
```markdown
## Description
Brief description of changes and why they're needed.

## Related Issue
Fixes #123

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] Documentation update

## Testing
- [ ] Unit tests added/updated
- [ ] Integration tests added/updated
- [ ] Manual testing completed

## Screenshots (if UI changes)
[Add screenshots of UI changes]

## Checklist
- [ ] Code follows style guidelines
- [ ] Self-review completed
- [ ] Comments added for complex logic
- [ ] No new warnings generated
- [ ] Tests pass locally
```

### Review Standards

#### Minimum Requirements
- Minimum reviewers: **2 for main/release branches**
- Minimum reviewers: **1 for feature branches**
- Approval from: Code owners (CODEOWNERS file)
- Status checks: All must pass (CI, linting, tests)
- Branch up to date: Yes (before merge)
- Dismiss stale reviews: Yes

#### Review Checklist (for Reviewers)
- [ ] **Functionality**: Does the change do what it claims?
- [ ] **Correctness**: Is the logic correct? Any edge cases missed?
- [ ] **Security**: No vulnerabilities, hardcoded secrets, or auth bypass? (@Security review required)
- [ ] **Testing**: Are tests adequate? Coverage sufficient?
- [ ] **Performance**: Any performance impact? Acceptable trade-offs?
- [ ] **Style**: Follows project conventions? Readable?
- [ ] **Documentation**: Code comments? Docs updated?
- [ ] **Maintainability**: Easy to understand? Follow SOLID principles?
- [ ] **Compliance**: WCAG/GDPR/PAngV/Audit logging requirements met? (@Legal/@UI review if needed)

**Compliance Gate** (See [compliance-integration.prompt.md](compliance-integration.prompt.md)):
- [ ] WCAG 2.1 AA (if UI change)
- [ ] GDPR data protection (if personal data)
- [ ] PAngV price transparency (if pricing/store)
- [ ] Security standards (all code)
- [ ] Audit logging (sensitive operations)

#### Approval Process
1. Author submits PR with description
2. Automated checks run (lint, tests, security scan)
3. Minimum number of reviewers review code
4. Approval required from code owners
5. All conversations resolved
6. All checks passing
7. Merge (squash, rebase, or merge commit - see below)

## 4. Merge Strategy

### Method Selection

**Merge Commit** (Preserves full history)
```
git merge --no-ff feature/branch
```
- Keeps full feature history
- Creates merge commit
- Good for audit trail
- Best for: Release branches, hotfixes

**Squash & Merge** (Cleans up history)
```
git merge --squash feature/branch
```
- Combines all commits into one
- Linear, clean history
- Easier to revert if needed
- Best for: Feature branches, small changes

**Rebase & Merge** (Linear history)
```
git rebase main
git merge --ff-only feature/branch
```
- Replays commits on top of main
- Linear, chronological history
- Easy to understand flow
- Best for: Short-lived branches, frequent merges

### Recommended Strategy
| Branch Type | Method | Reason |
|------------|--------|--------|
| `feature/*` | Squash | Clean commits into logical unit |
| `bugfix/*` | Squash | Single clear fix commit |
| `release/*` | Merge commit | Preserve release history |
| `hotfix/*` | Merge commit | Audit trail for production fixes |
| `docs/*` | Squash | Combine doc changes |

## 5. Branch Protection Rules

### GitHub Settings (Repository → Settings → Branches)

#### main Branch Protection
```yaml
Require pull request reviews:
  - Require 2 approvals
  - Dismiss stale PR approvals: Yes
  - Require review from code owners: Yes

Require status checks:
  - Require branches to be up to date: Yes
  - Required checks:
    - CI / build
    - CI / test
    - CI / lint
    - Security / scan
    - Accessibility / a11y-check

Allow force pushes: No
Allow deletions: No
```

#### develop Branch Protection (if using Git Flow)
```yaml
Require pull request reviews:
  - Require 1 approval
  - Require review from code owners: Yes

Require status checks:
  - Require branches to be up to date: Yes
  - Required checks:
    - CI / build
    - CI / test

Allow force pushes: No
Allow deletions: No
```

#### Feature Branch Rules (if needed)
```yaml
Require pull request reviews:
  - Require 1 approval
  - Dismiss stale PR approvals: Yes

Require status checks:
  - Require branches to be up to date: Yes
  - Required checks: All CI checks

Allow force pushes: Yes (for team collaboration)
Allow deletions: Yes (after merge)
```

## 6. GitHub CLI Integration

### Branch Operations
```bash
# Create and switch to feature branch
gh repo clone owner/repo
cd repo
git switch -c feature/AUTH-123-jwt-validation
git push -u origin feature/AUTH-123-jwt-validation

# List branches
gh repo view --web  # Show repo, then navigate to branches

# Delete branch
gh api -X DELETE repos/owner/repo/git/refs/heads/branch-name
```

### Pull Request Management
```bash
# Create PR
gh pr create --title "feat(auth): add JWT validation" \
  --body "Fixes #123" \
  --base main \
  --draft false

# Check PR status
gh pr view 456
gh pr checks 456

# Request review
gh pr edit 456 --add-reviewer @team-member

# Merge PR
gh pr merge 456 --squash --delete-branch
```

### Automation
```bash
# Auto-merge on passing checks
gh pr merge 456 --auto --squash

# Enable auto-merge for PR
gh pr merge 456 --merge --auto
```

## 7. Troubleshooting Guide

### Merge Conflicts

**Prevention:**
- Keep branches short-lived (< 3 days)
- Merge main into feature branch regularly
- Communicate parallel changes to team
- Review before pushing

**Resolution:**
```bash
# During rebase
git fetch origin main
git rebase origin/main
# Resolve conflicts in your editor
git add resolved-files
git rebase --continue
git push -f origin feature/branch

# During merge
git merge origin/main
# Resolve conflicts
git add resolved-files
git commit -m "Merge main into feature-branch"
git push origin feature/branch
```

### Accidentally Committed to Wrong Branch

```bash
# Get the commit hash from main
git log main --oneline | head -1

# Reset current branch to before commit
git reset --soft HEAD~1

# Switch to correct branch
git switch feature/correct-branch
git commit -m "correct commit message"
git push origin feature/correct-branch

# Fix main if needed
git switch main
git reset --hard origin/main
```

### Undo Last Commit (before push)
```bash
# Keep changes, undo commit
git reset --soft HEAD~1

# Discard commit and changes
git reset --hard HEAD~1
```

### Lost Commits
```bash
# Find the commit hash
git reflog | grep "your message"

# Recover the commit
git cherry-pick <commit-hash>
```

---

## 8. Release Process

### Release Branch Workflow (Git Flow)

```bash
# 1. Create release branch from develop
git switch -c release/1.0.0 develop

# 2. Update version numbers
# - package.json, VERSION file, etc.
git add .
git commit -m "chore(release): bump version to 1.0.0"

# 3. Create changelog
# - Summarize changes since last release
git add CHANGELOG.md
git commit -m "docs(release): add changelog for 1.0.0"

# 4. Create PR to main
gh pr create --title "release: 1.0.0" \
  --base main \
  --body "Release 1.0.0"

# 5. Get approval, merge to main
gh pr merge <pr-number> --merge

# 6. Tag the release
git switch main
git pull origin main
git tag -a v1.0.0 -m "Release version 1.0.0"
git push origin v1.0.0

# 7. Merge back to develop
git switch develop
git pull origin develop
git merge main
git push origin develop
```

### Hotfix Process (Git Flow)

```bash
# 1. Create hotfix branch from main
git switch -c hotfix/1.0.1 main

# 2. Fix the bug
# ... make changes ...
git add .
git commit -m "fix(auth): resolve token expiration bug"

# 3. Update version to 1.0.1
git add .
git commit -m "chore(release): bump version to 1.0.1"

# 4. Create PR to main
gh pr create --title "hotfix: 1.0.1" --base main

# 5. Merge to main and tag
gh pr merge <pr-number> --merge
git switch main
git pull origin main
git tag -a v1.0.1 -m "Hotfix 1.0.1"
git push origin v1.0.1

# 6. Backport to develop
git switch develop
git merge main
git push origin develop
```

---

## 9. Team Guidelines

### Expectations
- All code goes through PR review (no direct pushes to main)
- PRs should be focused (one feature/fix per PR)
- Review turnaround: 24 hours maximum
- Keep commits focused and well-described
- Communicate blockers early

### CI/CD Integration
- All PRs must pass automated checks
- Tests must pass before merge
- Code coverage must meet threshold (80%+)
- Linting must pass with no errors
- Security scanning must pass

### Rollback Procedure
```bash
# If deployment has critical issues
git revert <commit-hash>
# or
git reset --hard <previous-good-commit>
git push -f origin main  # Only if necessary!
```

---

## 10. Success Metrics

Track these metrics to evaluate workflow effectiveness:

- **Merge Conflict Frequency**: Target < 5% of merges
- **Code Review Time**: Target 24-48 hours response
- **PR Time to Merge**: Target < 3 days
- **Build Success Rate**: Target > 95%
- **Commit Quality**: 100% follow Conventional Commits
- **Deploy Frequency**: [As per team cadence]
- **Deployment Success**: Target > 99%
- **Rollback Rate**: Target < 1%

---

## Implementation Checklist

- [ ] Team aligned on branching model
- [ ] Naming conventions documented
- [ ] Branch protection rules configured
- [ ] CI/CD checks integrated
- [ ] Code review process established
- [ ] Merge strategy decided
- [ ] Commit message template added
- [ ] CODEOWNERS file created
- [ ] Team trained on workflow
- [ ] Runbooks created for common issues
- [ ] GitHub Actions configured
- [ ] Release automation set up (optional)

---

**When workflow questions arise, @GitManager provides guidance aligned with team needs.**
```

