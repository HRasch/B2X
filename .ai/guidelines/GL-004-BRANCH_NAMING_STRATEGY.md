---
docid: GL-004
title: Branch Naming & Single-Topic Strategy
owner: "@GitManager + @SARAH"
status: Active
date: 2025-01-02
---

# Branch Naming & Single-Topic Strategy

**DocID**: `GL-004`  
**Purpose**: Enforce single-topic PRs through branch naming conventions and workflow  
**Owner**: @GitManager (workflow) + @SARAH (quality gate)

---

## Overview

**Problem**: Mixed-topic PRs are hard to review, difficult to rollback, and violate single-responsibility principle.

**Solution**: Enforce single-topic focus through:
1. Strict branch naming conventions
2. Automated validation
3. PR template topic declaration
4. SARAH quality gate enforcement

---

## Branch Naming Convention

### Format (REQUIRED)

```
<type>/<domain>/<issue-number>-<short-description>
```

### Components

| Component | Description | Examples |
|-----------|-------------|----------|
| **type** | Change category | `feature`, `fix`, `refactor`, `perf`, `docs`, `chore`, `infra` |
| **domain** | Primary service/area | `catalog`, `cms`, `identity`, `search`, `localization`, `shared`, `infra` |
| **issue-number** | GitHub issue ID | `57`, `123`, `456` |
| **short-description** | Kebab-case summary | `add-product-search`, `fix-login-timeout` |

### Valid Examples

```bash
feature/catalog/57-add-product-filtering
fix/identity/123-password-reset-bug
refactor/cms/89-extract-page-service
perf/search/145-optimize-elasticsearch-query
docs/shared/99-update-api-documentation
infra/devops/200-add-kubernetes-monitoring
```

### Invalid Examples (REJECTED)

```bash
❌ feature/catalog-and-identity/mixed-stuff
   Reason: Multiple domains

❌ fix/multiple-bugs
   Reason: No issue number, vague scope

❌ update-everything
   Reason: No type, no domain, no issue

❌ feature/catalog/add-search-and-fix-bugs
   Reason: Mixed change types

❌ john-dev-branch
   Reason: Doesn't follow convention
```

---

## Type Definitions

### `feature` - New Functionality
- Adds new capability
- Includes implementation + tests + docs
- **Scope**: Single feature only

### `fix` - Bug Repair
- Fixes identified bug
- Includes fix + tests + verification
- **Scope**: Single bug only (no "fix multiple bugs")

### `refactor` - Code Improvement
- Improves code structure without changing behavior
- No new features, no bug fixes
- **Scope**: Single module/service

### `perf` - Performance Optimization
- Improves performance
- Includes benchmarks/metrics
- **Scope**: Single optimization focus

### `docs` - Documentation Only
- Only documentation changes
- No code changes
- **Scope**: Related documentation area

### `chore` - Maintenance Tasks
- Dependency updates
- Build configuration
- **Scope**: Single maintenance task

### `infra` - Infrastructure Changes
- CI/CD, deployment, monitoring
- Kubernetes, Docker, GitHub Actions
- **Scope**: Single infrastructure concern

---

## Domain Boundaries

### Backend Domains
- `catalog` - Product management
- `cms` - Content management
- `identity` - Authentication, users, roles
- `search` - Elasticsearch integration
- `localization` - i18n, translations

### Cross-Cutting Domains
- `shared` - Shared libraries/utilities
- `infra` - Infrastructure, DevOps
- `docs` - Documentation

### Frontend Domains
- `store` - Store frontend
- `admin` - Admin frontend
- `management` - Management frontend

---

## What Counts as Single Topic?

### ✅ ALLOWED (Single Topic)

| Scenario | Rationale |
|----------|-----------|
| Feature + Unit Tests + Integration Tests | Tests are part of feature |
| Backend API + Frontend Integration | Same feature, related components |
| Database Schema + Migration Script | Cohesive database change |
| Feature + Documentation Update | Docs are part of feature delivery |
| Refactor + Updated Tests | Tests must reflect refactored code |
| Fix + Regression Test | Test prevents future regression |

### ⚠️ REQUIRES JUSTIFICATION

| Scenario | When Allowed |
|----------|--------------|
| Multiple Services in Same Domain | If tightly coupled for single feature |
| Backend + Frontend + Infrastructure | If ALL needed for single feature deployment |
| Refactor + Performance Improvement | If refactor enables the optimization |

### ❌ REJECTED (Multiple Topics)

| Scenario | What to Do Instead |
|----------|-------------------|
| Feature A + Feature B | Split into 2 PRs |
| Fix Bug X + Fix Bug Y | Separate PR for each bug |
| Catalog Feature + Identity Fix | Completely separate PRs |
| Refactor + New Feature | Refactor first (PR #1), feature second (PR #2) |
| Multiple unrelated dependency updates | Group related updates or separate PRs |

---

## Branch Lifecycle

### 1. Create Branch from Issue

```bash
# From GitHub issue #57 "Add product filtering"
git checkout main
git pull origin main
git checkout -b feature/catalog/57-add-product-filtering
```

### 2. Work on Branch (Single Topic Only!)

```bash
# Commit frequently, keep commits small
git commit -m "feat(catalog): add filter by brand"
git commit -m "test(catalog): add brand filter tests"
git commit -m "docs(catalog): document filter API"
```

### 3. Push and Create PR

```bash
git push origin feature/catalog/57-add-product-filtering
# Create PR, fill topic declaration in template
```

### 4. SARAH Quality Gate

SARAH verifies:
- ✅ Branch name follows convention
- ✅ Single domain touched
- ✅ Topic declaration matches changes
- ✅ No mixed topics

---

## Automation Support

### Pre-commit Hook (Recommended)

```bash
# .git/hooks/commit-msg
#!/bin/bash
# Validates commit message format
# See .ai/knowledgebase/git-commit-strategy.md
```

### GitHub Action (Auto-validates PRs)

```yaml
# .github/workflows/pr-topic-check.yml
name: PR Topic Validation
on: [pull_request]
jobs:
  validate-topic:
    runs-on: ubuntu-latest
    steps:
      - name: Check branch name
        run: |
          # Validate branch follows convention
          # Flag if multiple domains touched
      
      - name: Analyze changed files
        run: |
          # Detect if PR touches multiple unrelated domains
          # backend/Domain/Catalog + backend/Domain/Identity = FAIL
          
      - name: Comment on PR
        if: failure()
        run: |
          # Auto-comment with mixed-topic warning
```

---

## Enforcement Levels

### Level 1: Branch Name (Automated)
- GitHub Action validates branch name format
- Blocks merge if invalid

### Level 2: File Path Analysis (Automated)
- Analyzes changed files for domain violations
- Warns if multiple domains touched
- Requires justification in PR

### Level 3: Topic Declaration (Manual)
- PR template requires explicit topic declaration
- Developer must acknowledge single-topic commitment

### Level 4: SARAH Quality Gate (Manual)
- SARAH reviews topic coherence
- Rejects PRs with unjustified mixed topics
- Final authority on exceptions

---

## Exception Process

### When Mixed-Topic Might Be Justified

1. **Emergency hotfixes** - Multiple critical fixes for production incident
2. **Tightly coupled changes** - Impossible to separate without breaking system
3. **Migration work** - Large-scale refactoring across boundaries

### How to Request Exception

1. In PR description, add section:
   ```markdown
   ## 🚨 Mixed-Topic Exception Request
   
   **Reason**: [Detailed justification]
   **Alternative Considered**: [Why splitting isn't feasible]
   **Risk Assessment**: [Impact if rejected]
   **Reviewers**: @SARAH @Architect @TechLead
   ```

2. Tag @SARAH for review
3. SARAH evaluates with @Architect and @TechLead
4. Decision documented in PR comments

---

## Examples & Scenarios

### Example 1: Good Single-Topic PR

```
Branch: feature/catalog/57-add-product-search
Files Changed:
  ✅ backend/Domain/Catalog/Handlers/SearchProductsHandler.cs
  ✅ backend/Domain/Catalog/Tests/SearchProductsTests.cs
  ✅ frontend/Store/src/components/ProductSearch.vue
  ✅ frontend/Store/src/components/ProductSearch.test.ts
  ✅ docs/api/catalog-search.md

Topic Declaration: ✅ Single Topic
Rationale: All changes implement product search feature
```

### Example 2: Bad Mixed-Topic PR

```
Branch: fix/multiple-issues  ❌
Files Changed:
  ❌ backend/Domain/Catalog/Handlers/ProductHandler.cs (Fix price calc)
  ❌ backend/Domain/Identity/Services/AuthService.cs (Fix login bug)
  ❌ frontend/Store/src/pages/Checkout.vue (UI improvement)
  ❌ infrastructure/kubernetes/deployment.yml (Update resource limits)

Topic Declaration: ❌ Multiple unrelated topics
SARAH Decision: REJECT - Split into 4 separate PRs
```

### Example 3: Justified Multi-Component PR

```
Branch: feature/search/145-elasticsearch-integration
Files Changed:
  ✅ backend/Domain/Search/* (New search domain)
  ✅ backend/Domain/Catalog/Handlers/* (Add search capability)
  ✅ infrastructure/docker-compose.yml (Add Elasticsearch)
  ✅ AppHost/Program.cs (Wire Elasticsearch)
  ✅ Tests/* (All search tests)

Topic Declaration: ⚠️ Multiple components, justified
Rationale: Complete search feature requires Elasticsearch + domain integration
SARAH Decision: APPROVED - Components are tightly coupled for single feature
```

---

## Integration with Existing Practices

### Aligns With
- [GL-001] Communication Overview
- [KB-014] Git Commit Strategy
- [WF-005] GitHub CLI Implementation
- PR Quality Gate (ADR-020)

### Enhances
- Code review efficiency
- Rollback safety
- Feature traceability
- Team collaboration

---

## Metrics & Success Criteria

### Track These Metrics

| Metric | Target | How to Measure |
|--------|--------|----------------|
| % of PRs following branch naming | 95%+ | GitHub Action reports |
| % of PRs with single topic | 90%+ | SARAH quality gate stats |
| Average PR size | < 500 lines | GitHub analytics |
| PR review time | < 24 hours | GitHub analytics |
| Merge conflicts rate | < 10% | Git statistics |

### Success Indicators
- ✅ Faster code reviews
- ✅ Fewer merge conflicts
- ✅ Clearer git history
- ✅ Easier rollbacks
- ✅ Better traceability

---

## FAQ

### Q: Can I include tests and docs in same PR as feature?
**A**: Yes! Tests and docs are part of feature delivery, not separate topics.

### Q: What if my feature touches backend AND frontend?
**A**: That's fine if it's ONE feature. Use the most relevant domain (usually backend domain for the primary logic).

### Q: I need to update shared utilities for my feature. Separate PR?
**A**: If the utility change is ONLY needed for your feature and tightly coupled, include it. If it's a general improvement, separate PR.

### Q: Emergency production fix touching multiple services?
**A**: Request exception from @SARAH with clear justification. Emergency fixes may warrant exception.

### Q: How do I handle large refactoring?
**A**: Break into phases:
1. PR #1: Refactor module A (no behavior change)
2. PR #2: Refactor module B (no behavior change)
3. PR #3: Feature using refactored modules

---

## Related Documentation

- **[KB-014]** Git Commit Strategy
- **[GL-001]** Communication Overview
- **[WF-005]** GitHub CLI Implementation
- **[ADR-020]** PR Quality Gate (if exists)

---

## Changelog

| Date | Change | Author |
|------|--------|--------|
| 2025-01-02 | Initial version | @SARAH |

---

**Status**: ✅ Active  
**Next Review**: 2025-02-01  
**Feedback**: Open issue with label `process-improvement`
