<!--
Required fields for PRs: Plan link (ADR or issue), Acceptance criteria, Tests added.
-->

## Plan

- Plan / ADR: [link to ADR or issue]

## Acceptance Criteria

- [ ] Criterion 1
- [ ] Criterion 2

## Implementation notes

- Short summary of changes

## Tests

- Unit tests: [yes/no]
- Integration tests: [yes/no]
- E2E/visual tests: [yes/no]

## How to validate (Control steps)

- CI gates that must pass: build, analyzers, unit tests, smoke e2e, SCA
- Local quick-check: `scripts/run-local-checks.sh` (or equivalent)

## Checklist

- [ ] Linked Plan / ADR or Issue with acceptance criteria
- [ ] Small, focused commits
- [ ] Linters/formatters run and fixed
- [ ] Tests added and passing locally
- [ ] Security scan completed (if applicable)
<!-- Please review the checklist and fill the sections below before creating a PR -->

# Pull Request

## 🎯 Topic Declaration (REQUIRED)

**Primary Domain**: <!-- Check ONE -->
- [ ] Catalog (Product management)
- [ ] CMS (Content management)
- [ ] Identity (Auth/Users)
- [ ] Search (Elasticsearch)
- [ ] Localization (i18n/l10n)
- [ ] Infrastructure (DevOps/CI/CD)
- [ ] Documentation Only

**Change Type**: <!-- Check ONE -->
- [ ] Feature (new functionality)
- [ ] Fix (bug repair)
- [ ] Refactor (code improvement, no behavior change)
- [ ] Performance (optimization)
- [ ] Documentation (docs only)
- [ ] Infrastructure (build, deploy, monitoring)

**Mixed-Topic Check**: ⚠️ **CRITICAL QUALITY GATE**
- [ ] ✅ **Single cohesive topic** - All changes relate to ONE feature/fix/task
- [ ] ⚠️ **Multiple related components** - Justification provided below (e.g., feature + tests + docs)
- [ ] ❌ **Multiple unrelated topics** - **STOP! Split into separate PRs**

**PR Rule**: 🔒 **Nur eine gelöste Aufgabe pro PR** (Only one solved task per PR)
- [ ] ✅ **One task only** - This PR solves exactly ONE issue/feature/task
- [ ] ❌ **Multiple tasks** - **STOP! Create separate PRs for each task**

**If multiple components touched, explain why they're part of the same logical change:**
<!-- 
Example GOOD: "Add product search feature - includes API endpoint (backend), search UI (frontend), and integration tests"
Example BAD: "Fix login bug AND refactor catalog service AND update documentation"
-->

---

## 📋 Summary

**Issue**: Closes #<!-- issue number -->

<!-- Brief description of changes -->

---

## ✅ PR Quality Checklist (REQUIRED)

### Testing (MANDATORY)
- [ ] **Unit tests** added/updated for all new code
- [ ] **Integration tests** cover API/service changes (if applicable)
- [ ] **E2E tests** added for UI changes (if applicable)
- [ ] All tests pass locally (`npm run test`, `dotnet test`)
- [ ] **Coverage threshold met**: Backend ≥80%, Frontend ≥70%
- [ ] Coverage report reviewed (CI will auto-comment)

### Code Quality
- [ ] No linting errors (`npm run lint`, `dotnet format`)
- [ ] No compiler warnings
- [ ] No TypeScript errors (`npm run type-check`)
- [ ] Code follows project conventions (see `.ai/guidelines/`)
- [ ] Complex logic documented with comments
- [ ] Public APIs documented

### Security (MANDATORY for security-sensitive changes)
- [ ] **No secrets in code** (API keys, passwords, tokens)
- [ ] **Input validation** added for all user inputs
- [ ] **Authentication/Authorization** verified
- [ ] Security scan passed (GitHub CodeQL - auto-runs)
- [ ] Dependencies scanned for vulnerabilities (npm audit, dotnet vulnerable)

### Architecture & Design
- [ ] Follows existing architecture patterns
- [ ] ADR created (if architectural decision made)
- [ ] Service boundaries respected
- [ ] No circular dependencies introduced

### Database (if applicable)
- [ ] Database migrations included
- [ ] Migration tested locally
- [ ] Rollback migration provided
- [ ] No breaking schema changes (or migration plan documented)

### Documentation
- [ ] README updated (if public API changed)
- [ ] API documentation updated (OpenAPI/Swagger)
- [ ] Code comments added for complex logic
- [ ] CHANGELOG entry added (if user-visible change)

---

## 🧪 Test Evidence

### Manual Testing
<!-- Describe manual testing performed -->

### Coverage Impact
<!-- CI will auto-comment coverage report -->
<!-- If coverage decreased, explain why -->

### Screenshots/Videos
<!-- For UI changes, attach screenshots or screen recordings -->

---

## 🔐 Security Review

**Security Impact**: <!-- None | Low | Medium | High | Critical -->

**Changes**:
<!-- Describe any authentication, authorization, encryption, or security-related changes -->

---

## 🚀 Deployment Notes

### Breaking Changes
<!-- None OR detailed list -->

### Migration Steps
<!-- Database migrations, data migrations, manual steps -->

### Rollback Plan
<!-- How to rollback this change if issues occur -->

---

## 📊 CI Quality Gate Status

<!-- Auto-updated by GitHub Actions -->

| Check | Status | Details |
|-------|--------|---------|
| Fast Checks | ⏳ | Lint, Type, Secrets |
| Unit Tests | ⏳ | Coverage: Backend ≥80%, Frontend ≥70% |
| Integration Tests | ⏳ | API & Service Tests |
| E2E Tests | ⏳ | Critical User Flows |
| Security Scan | ⏳ | CodeQL + Dependency Scan |
| Code Quality | ⏳ | Mega-Linter (50+ linters) |

---

## 🔍 Reviewer Guidance

**Focus Areas**:
<!-- What should reviewers focus on? -->

**Testing Instructions**:
<!-- How can reviewers test this locally? -->

**Related PRs/Issues**:
<!-- Links to related PRs or issues -->

---

## 📝 Additional Context

<!-- Any additional context, design decisions, alternatives considered -->

---

## ⚠️ Important Reminders

- **All checkboxes must be checked** before requesting review
- **Coverage cannot decrease** without documented justification
- **Security-sensitive PRs** require @Security approval (auto-assigned via CODEOWNERS)
- **Breaking changes** require @Architect + @TechLead approval
- **Database changes** require @Backend + @Architect approval

---

**Auto-assigned reviewers**: See CODEOWNERS file  
**Quality gate**: See [ADR-020](.ai/decisions/ADR-020-pr-quality-gate.md)
## Summary

- **What**: Brief description of the changes.
- **Why**: Motivation and context (reference issue).

> Quick rules: open a PR ASAP (draft is fine). Branch must reference the related issue (e.g. `feature/123-desc`). Include tests for new features and document assumptions.

## Related Issue

- Closes: #<issue-number>

## Changes

- Bullet list of notable changes.

## Testing

- How to run tests and manual verification steps.

## Migration / Backwards Compatibility

- Any migration steps or breaking changes.

## Checklist

- [ ] Code builds locally and CI passes
- [ ] Tests added/updated and pass
- [ ] Documentation updated where relevant
- [ ] Security review (if applicable)
- [ ] Reviewers assigned: @TechLead, domain owner

Apply labels: `feature`, `bug`, `chore`, `security` as appropriate.
