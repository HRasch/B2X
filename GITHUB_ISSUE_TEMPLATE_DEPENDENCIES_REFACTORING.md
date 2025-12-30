# GitHub Issue Template: Update Dependencies & Code Refactoring

**Copy this content into a new GitHub issue**

---

## Title
`chore(dependencies): update to latest stable versions and reduce technical debt`

## Description

Update all project dependencies to the latest stable versions and perform comprehensive code refactoring to keep technical debt low across the B2Connect platform.

### Rationale
- Reduce security vulnerabilities by using patched versions
- Improve code quality and maintainability
- Ensure framework/library compatibility going forward
- Prevent cumulative technical debt

### Scope

#### Part 1: Dependency Updates
- [ ] **Backend (.NET 10)**:
  - Update all NuGet packages in `Directory.Packages.props` to latest stable
  - Review and update critical dependencies: Wolverine, EF Core, Serilog, FluentValidation
  - Test compatibility with existing code
  
- [ ] **Frontend (Vue.js 3)**:
  - Update npm dependencies in `package.json` (Store, Admin, Management)
  - Review: Vue 3, Vite, Tailwind, daisyUI
  - Run dependency audit: `npm audit`
  - Fix security vulnerabilities found
  
- [ ] **DevOps/Infrastructure**:
  - Update Docker base images
  - Update GitHub Actions versions
  - Update Kubernetes manifest versions

#### Part 2: Code Refactoring
- [ ] **Backend Code Quality**:
  - Run static analysis: `dotnet analyze`
  - Fix all compiler warnings (target: 0 warnings)
  - Remove unused imports, dead code
  - Consolidate duplicate utilities
  - Update deprecated API usage
  - Improve error handling patterns
  
- [ ] **Frontend Code Quality**:
  - Run ESLint: `npm run lint`
  - Fix all linting issues
  - Remove unused components/utilities
  - Update deprecated Vue 3 patterns
  - Modernize prop definitions (use `defineProps<T>()`)
  
- [ ] **Test Quality**:
  - Verify all tests still pass (target: 100%)
  - Update test patterns to match latest frameworks
  - Remove deprecated test utilities
  - Consolidate duplicate test helpers

### Acceptance Criteria

**Dependency Updates:**
- [ ] All NuGet packages updated to latest stable versions
- [ ] All npm packages updated to latest stable versions
- [ ] No security vulnerabilities reported by `npm audit`
- [ ] No deprecated dependencies remain
- [ ] `Directory.Packages.props` reflects latest versions
- [ ] `package.json` in all frontend projects reflects latest versions
- [ ] Docker images use latest stable base versions

**Code Refactoring:**
- [ ] 0 compiler warnings in entire solution
- [ ] 0 ESLint violations in frontend code
- [ ] All tests passing (204+ tests, 100% pass rate)
- [ ] Code coverage maintained ≥ 80%
- [ ] Removed 50+ lines of dead/duplicate code
- [ ] Updated 10+ deprecated API usages

**Quality Metrics:**
- [ ] Build time: < 10 seconds
- [ ] Test execution time: < 30 seconds
- [ ] No new critical issues introduced
- [ ] Code maintainability improved (measured by complexity metrics)

### Definition of Done

- [ ] All dependencies updated and compatible
- [ ] All refactoring changes completed
- [ ] Full test suite passing (unit, integration, E2E)
- [ ] No breaking changes to public APIs
- [ ] Documentation updated for any changed patterns
- [ ] Code review approved by @tech-lead
- [ ] Performance benchmarks show no regression
- [ ] Build/test pipeline green

### Out of Scope

- Adding new features or functionality
- Major architectural changes
- Database schema modifications
- API contract changes (breaking changes)

### Dependencies

- None (can be parallel with other features)

### Effort Estimate

- **Backend Dependencies**: 4 hours
- **Frontend Dependencies**: 3 hours
- **Code Refactoring**: 8 hours
- **Testing & Verification**: 3 hours
- **Total**: ~18 hours (2-3 days for dedicated developer)

### Owner

- **Backend Refactoring**: @backend-developer
- **Frontend Refactoring**: @frontend-developer
- **Code Quality Review**: @tech-lead
- **Testing Verification**: @qa-engineer

### Labels

`chore`, `technical-debt`, `dependencies`, `refactoring`, `quality`

### Related Issues

- Related to: P0 Compliance Foundation (overall quality improvement)
- Blocked by: None
- Blocks: None (improvements only)

### Priority

**Medium** - Improves code health but not blocking features

### Timeline

Recommended: **After Phase 1 MVP** (post Sprint 3)

---

## Steps to Create This Issue

1. Go to: https://github.com/HRasch/B2Connect/issues/new
2. Copy the title and description above
3. Paste into the issue form
4. Add labels: `chore`, `technical-debt`, `dependencies`, `refactoring`
5. Assign to: @backend-developer (lead), @frontend-developer, @tech-lead
6. Set milestone: Backlog or next appropriate sprint
7. Create issue

---

## After Issue Creation

Once created, the team should:

1. **Backend Lead** (@backend-developer):
   ```bash
   # Check for outdated packages
   dotnet outdated --include-prerelease=false
   
   # Update packages
   dotnet package update --include-prerelease=false
   ```

2. **Frontend Lead** (@frontend-developer):
   ```bash
   cd Frontend/Store && npm update
   cd Frontend/Admin && npm update
   cd Frontend/Management && npm update
   npm audit fix
   ```

3. **Tech Lead** (@tech-lead):
   - Review all changes
   - Verify no breaking changes
   - Approve refactoring patterns

4. **QA** (@qa-engineer):
   - Run full test suite
   - Verify metrics maintained
   - Sign off on quality

---

**Status**: Ready to Create  
**Created**: 30. Dezember 2025
