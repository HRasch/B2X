---
docid: WF-011
title: Project Cleanup.Workflow
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

# Project Cleanup Workflow

**Version:** 1.0  
**Created:** 30.12.2025  
**Managed by:** @SARAH, @TechLead

## Overview

Systematische Bereinigung eines bestehenden Entwicklungsprojekts über multiple Dimensionen: Code, Dependencies, Documentation, Tests, Performance, Security, Technical Debt.

## Cleanup Dimensions

```
Existing Project Cleanup
├── 1. Code Quality (Dead Code, Duplication)
├── 2. Dependencies (Outdated, Unused, Vulnerable)
├── 3. Documentation (Outdated, Missing)
├── 4. Tests (Coverage, Flaky Tests)
├── 5. Performance (Bottlenecks, Optimization)
├── 6. Security (Vulnerabilities, Best Practices)
├── 7. Technical Debt (Refactoring, Architecture)
├── 8. Infrastructure (Unused Resources, Config)
├── 9. Git Hygiene (Old Branches, Commits)
└── 10. Team Knowledge (Sharing, Onboarding)
```

## Phase 1: Assessment (3-5 days)

### 1.1 Code Analysis

**Task:** @TechLead + @SubAgent-Analysis

```
Analyze:
- Total LOC (Lines of Code)
- Duplication ratio
- Cyclomatic complexity
- Coverage metrics
- Tech stack inventory
- Dead code estimate
- Architecture alignment

Output: `.ai/issues/CLEANUP-001/code-assessment.md`
```

**Checklist:**
```
- [ ] Run static analysis tools (ESLint, SonarQube, etc.)
- [ ] Identify duplicate code blocks
- [ ] Find unused imports/functions
- [ ] Check complexity hotspots
- [ ] Review architecture alignment
- [ ] Document findings with priorities
```

### 1.2 Dependency Audit

**Task:** @DevOps + @SubAgent-Research

```
Audit:
- Outdated packages
- Security vulnerabilities
- Unused dependencies
- Dependency conflicts
- License compliance
- Size impact

Output: `.ai/issues/CLEANUP-002/dependency-audit.md`
```

**Checklist:**
```
- [ ] npm audit (or equivalent)
- [ ] Check for security vulnerabilities
- [ ] Identify unused packages
- [ ] Check version update paths
- [ ] Document breaking changes
- [ ] Size analysis (bundle, install time)
```

### 1.3 Test Coverage Assessment

**Task:** @QA + @SubAgent-Analysis

```
Assess:
- Current coverage %
- Untested code blocks
- Flaky test detection
- Slow tests (>5s)
- Missing critical paths
- Test organization

Output: `.ai/issues/CLEANUP-003/test-assessment.md`
```

**Checklist:**
```
- [ ] Generate coverage report
- [ ] Identify gaps
- [ ] Find flaky tests
- [ ] Time slow tests
- [ ] Review test structure
- [ ] Document priorities
```

### 1.4 Documentation Audit

**Task:** @TechLead + @SubAgent-Documentation

```
Review:
- README completeness
- Architecture docs
- API documentation
- Setup guide accuracy
- Outdated sections
- Missing sections

Output: `.ai/issues/CLEANUP-004/docs-assessment.md`
```

**Checklist:**
```
- [ ] Check README vs reality
- [ ] Verify setup instructions work
- [ ] Review architecture docs
- [ ] API docs up-to-date?
- [ ] List missing docs
- [ ] Identify outdated info
```

### 1.5 Performance Profiling

**Task:** @Backend/Frontend + @SubAgent-Optimization

```
Profile:
- Load time (frontend)
- API response times
- Database queries
- Memory usage
- Bundle size
- Build time

Output: `.ai/issues/CLEANUP-005/performance-profile.md`
```

**Checklist:**
```
- [ ] Measure page load time
- [ ] Profile slow endpoints
- [ ] Check bundle size
- [ ] Analyze memory usage
- [ ] Measure build time
- [ ] Document baselines
```

### 1.6 Security Scan

**Task:** @Security + @SubAgent-Security

```
Scan:
- OWASP top 10
- Dependency vulnerabilities
- Hardcoded secrets
- Auth/authorization
- Data exposure
- API security

Output: `.ai/issues/CLEANUP-006/security-scan.md`
```

**Checklist:**
```
- [ ] Run OWASP scanning
- [ ] Check for hardcoded secrets
- [ ] Verify authentication
- [ ] Review authorization
- [ ] Check data handling
- [ ] API security review
```

### 1.7 Consolidation

**Task:** @SARAH

**File:** `.ai/issues/CLEANUP-000/assessment-summary.md`

```markdown
# Project Cleanup Assessment Summary

## Project Overview
- Name: [Project]
- Size: [LOC]
- Tech Stack: [Technologies]
- Team Size: [Team]
- Last Major Refactor: [When]

## Assessment Results

### Code Quality
- Duplication: 12%
- Complexity hotspots: 8 files
- Coverage: 65%
- Priority: HIGH

### Dependencies
- Outdated: 15 packages
- Vulnerabilities: 3 critical, 5 medium
- Unused: 8 packages
- Priority: CRITICAL

### Documentation
- Missing: Setup guide, API docs
- Outdated: Architecture, Configuration
- Coverage: 60%
- Priority: MEDIUM

### Tests
- Coverage: 65%
- Flaky: 2 tests
- Slow: 5 tests (>5s each)
- Priority: HIGH

### Performance
- Page load: 3.2s (target: 2s)
- API response: 150ms avg
- Bundle: 800KB (uncompressed)
- Priority: MEDIUM

### Security
- Critical vulns: 3
- Medium vulns: 5
- Issues: Hardcoded config in 2 places
- Priority: CRITICAL

### Technical Debt
- Estimated: 80-120 hours
- High impact areas: 5 modules
- Priority: HIGH

## Recommended Focus Areas
1. 🔴 Fix critical security issues (CRITICAL)
2. 🔴 Update vulnerable dependencies (CRITICAL)
3. 🟡 Improve test coverage to 80% (HIGH)
4. 🟡 Reduce code duplication (HIGH)
5. 🟡 Update documentation (MEDIUM)
6. 🟢 Performance optimization (MEDIUM)

## Next Steps
1. Create cleanup issues for each area
2. Prioritize by impact
3. Delegate to teams
4. Weekly progress tracking
5. Monthly reviews
```

## Phase 2: Planning (2-3 days)

### 2.1 Create Cleanup Sprints

**File:** `.ai/issues/CLEANUP-000/sprint-plan.md`

```markdown
# Cleanup Sprint Plan

## Sprint 1: Critical (Week 1)
Focus: Security & Critical Bugs

### Tasks
- [ ] Fix 3 critical security vulnerabilities
- [ ] Update vulnerable dependencies
- [ ] Remove hardcoded secrets
- [ ] Estimated effort: 40 hours

### Assigned
- @Security: Lead
- @Backend: Implementation
- @DevOps: Dependency updates

### Success Criteria
- All critical vulns fixed
- 0 secrets in code
- No dependency conflicts

---

## Sprint 2: Dependencies (Week 2)
Focus: Clean up package ecosystem

### Tasks
- [ ] Update outdated packages (non-breaking)
- [ ] Remove unused dependencies
- [ ] Document breaking changes
- [ ] Estimated effort: 30 hours

### Assigned
- @DevOps: Lead
- @Backend: Testing
- @Frontend: Testing

### Success Criteria
- All updates tested
- No breaking changes
- Bundle size optimized

---

## Sprint 3: Code Quality (Week 3-4)
Focus: Reduce duplication, increase coverage

### Tasks
- [ ] Extract duplicate code (3 areas)
- [ ] Improve test coverage (65% → 75%)
- [ ] Fix complexity hotspots
- [ ] Estimated effort: 50 hours

### Assigned
- @TechLead: Lead
- @Backend: Code refactoring
- @Frontend: Code refactoring
- @QA: Testing

### Success Criteria
- Duplication < 8%
- Coverage > 75%
- No complexity violations

---

## Sprint 4: Documentation (Week 5)
Focus: Update & create missing docs

### Tasks
- [ ] Update setup guide
- [ ] Create architecture documentation
- [ ] Complete API documentation
- [ ] Estimated effort: 25 hours

### Assigned
- @TechLead: Lead
- @Backend: API docs
- @Frontend: Component docs
- @DevOps: Deployment docs

### Success Criteria
- Setup guide accurate
- Architecture documented
- All APIs documented

---

## Sprint 5: Performance (Week 6)
Focus: Optimize load times & speed

### Tasks
- [ ] Reduce page load time
- [ ] Optimize slow endpoints
- [ ] Reduce bundle size
- [ ] Estimated effort: 35 hours

### Assigned
- @Frontend: Page optimization
- @Backend: API optimization
- @DevOps: Infrastructure tuning

### Success Criteria
- Page load < 2.5s
- API response < 100ms
- Bundle < 700KB
```

### 2.2 Risk Assessment

**Identify risks for each area:**

```markdown
## Risk Assessment

### Code Refactoring
- Risk: Breaking existing functionality
- Mitigation: Comprehensive testing, gradual rollout
- Priority: Must have full test suite coverage first

### Dependency Updates
- Risk: Breaking changes, compatibility issues
- Mitigation: Test in staging, monitor carefully
- Priority: Do non-breaking updates first

### Security Fixes
- Risk: Introducing new bugs, breaking changes
- Mitigation: Careful review, staged rollout
- Priority: ASAP (critical path)

### Performance Work
- Risk: Regression in other areas
- Mitigation: Performance testing, A/B testing
- Priority: Monitor closely
```

## Phase 3: Execution (4-6 weeks)

### 3.1 Weekly Execution Pattern

**Monday: Planning**
```
- @SARAH: Review previous week's progress
- Team: Plan current week tasks
- Assign: Work items to agents
- Output: `.ai/sprint/current.md`
```

**Tuesday-Thursday: Execution**
```
- Agents: Execute cleanup tasks
- @SubAgent-Review: Code reviews
- @SubAgent-Testing: Test generation/execution
- Track: Progress in `.ai/issues/CLEANUP-XXX/`
```

**Friday: Review & Consolidation**
```
- @SARAH: Collect progress
- Team: Review completed items
- @TechLead: Quality gate
- Output: `.ai/logs/cleanup-week-{date}.md`
```

### 3.2 Task Templates

**Security Fix Task**
```markdown
# CLEANUP-101: Fix SQL Injection in user.service.ts

## Issue
SQL queries concatenate user input without parameterization.

## Impact
Critical security vulnerability (OWASP A1)

## Solution
Use parameterized queries with prepared statements.

## Tasks
- [ ] Refactor 5 vulnerable queries
- [ ] Add tests for injection attempts
- [ ] Deploy to staging
- [ ] Verify fix

## Assigned
@Security (lead), @Backend (implementation)

## Deadline
2025-12-31

## Success Criteria
- Zero vulnerable queries
- All tests pass
- Security audit confirms fix
```

**Dependency Update Task**
```markdown
# CLEANUP-201: Update React 17 → 18

## Change
Major version update with breaking changes

## Impact
Improved performance, new features, but requires testing

## Migration
See [react-migration.md](../knowledgebase/INDEX.md)

## Tasks
- [ ] Update package.json
- [ ] Fix deprecation warnings
- [ ] Test all components
- [ ] Update documentation
- [ ] Deploy to staging

## Assigned
@Frontend (lead), @QA (testing)

## Deadline
2025-01-15

## Success Criteria
- All tests pass
- No console warnings
- Performance maintained or improved
```

**Code Duplication Task**
```markdown
# CLEANUP-301: Extract Common Utility Functions

## Issue
3 files have duplicate string handling logic (~50 LOC duplicated)

## Impact
Maintenance risk, inconsistency

## Solution
Create shared utility module

## Tasks
- [ ] Analyze duplicate code
- [ ] Create shared module
- [ ] Update all references (3 files)
- [ ] Add tests for utility
- [ ] Update imports

## Assigned
@Backend (lead), @TechLead (review)

## Deadline
2025-01-08

## Success Criteria
- No duplication remaining
- All tests pass
- Utility module well-tested
```

## Phase 4: Tracking & Monitoring

### 4.1 Daily Progress Log

**File:** `.ai/logs/cleanup-daily-{date}.md`

```markdown
# Cleanup Progress: 2025-12-30

## Completed Today
- [ ] Fix SQL injection in UserService (CLEANUP-101) ✅
- [ ] Update React dependencies (CLEANUP-201) 50% done
- [ ] Created string utilities module (CLEANUP-301) ✅

## In Progress
- React component migration (3 files remaining)
- Test coverage improvement (currently 68%)

## Blockers
- None

## Metrics
- Duplication: 11% (was 12%)
- Coverage: 68% (target: 75%)
- Security vulns: 1 fixed, 2 remaining

## Next Actions
1. Complete React migration
2. Address remaining security issue
3. Start performance optimization
```

### 4.2 Weekly Report

**File:** `.ai/logs/cleanup-week-{week}.md`

```markdown
# Cleanup Progress: Week of 2025-12-30

## Summary
- Tasks completed: 8/12 (67%)
- Issues resolved: 6
- Code quality: +5%
- Effort spent: 38/40 hours planned

## By Dimension

### Security (CRITICAL)
- Started: 3 vulns
- Fixed: 1 ✅
- In progress: 2
- Status: On track

### Dependencies (CRITICAL)
- Packages updated: 8/15
- Breaking changes addressed: 0 (so far)
- Status: On track

### Code Quality (HIGH)
- Duplication: 12% → 11%
- Coverage: 65% → 68%
- Hotspots analyzed: 8/8
- Status: Slightly ahead

### Tests (HIGH)
- Added tests: 25
- Fixed flaky tests: 1/2
- Status: On track

### Documentation (MEDIUM)
- Not started (planned for week 4)
- Status: On schedule

### Performance (MEDIUM)
- Not started (planned for week 5)
- Status: On schedule

## Team Velocity
- Planned: 40 hours
- Actual: 38 hours
- Efficiency: 95% ✅

## Risks & Mitigations
- Risk: Staging environment issues with new React version
- Mitigation: Testing locally first, then canary deploy
- Status: Monitoring

## Next Week Goals
- Complete 2 remaining security fixes
- Finish dependency updates
- Reach 75% test coverage
- Start documentation updates
```

### 4.3 Monthly Review

**File:** `.ai/logs/cleanup-month-{month}.md`

```markdown
# Project Cleanup: Monthly Review - December 2025

## Completion Status
- Sprint 1 (Security): 100% ✅
- Sprint 2 (Dependencies): 100% ✅
- Sprint 3 (Code Quality): 75% (on track for completion)
- Sprint 4 (Documentation): 0% (starting next week)
- Sprint 5 (Performance): 0% (starting next week)

Overall: 55% complete

## Metrics Improvement

| Metric | Start | Current | Target | Progress |
|--------|-------|---------|--------|----------|
| Security Vulns | 8 | 2 | 0 | 75% ✅ |
| Dependencies | 15 outdated | 2 outdated | 0 | 87% ✅ |
| Test Coverage | 65% | 72% | 85% | 27% |
| Duplication | 12% | 10% | <8% | 17% |
| Page Load | 3.2s | 3.0s | 2.0s | 6% |

## Issues Resolved
1. Critical SQL injection vulnerability ✅
2. Hardcoded database config ✅
3. 8/15 outdated dependencies ✅
4. 25 new unit tests added ✅
5. Code duplication in 2 modules ✅

## Team Effort
- Hours planned: 150
- Hours spent: 95
- Remaining: 55
- Efficiency: 95%

## Quality Impact
- Code complexity: Reduced 15%
- Maintainability: +20%
- Performance: +5%
- Security: Critical vulns eliminated

## Blockers Encountered
1. Staging environment issues with React 18 (RESOLVED)
2. Compatibility with legacy code (MITIGATED with wrapper)
3. Test flakiness (REDUCED from 2 to 1)

## Lessons Learned
1. Dependency updates should be batched by compatibility
2. Full test suite required before refactoring
3. Security fixes should be prioritized first
4. Documentation updates improve team velocity

## Next Month Focus
- Complete remaining security fixes
- Finish code quality improvements
- Update all documentation
- Performance optimization

## Team Feedback
- Positive: Clear priorities, good progress
- Challenges: Time management during optimization
- Suggestion: More staging environment capacity
```

## Phase 5: Completion & Handover

### 5.1 Final Verification

**Checklist:**
```
Code Quality:
- [ ] No critical issues
- [ ] Coverage >80%
- [ ] Duplication <8%
- [ ] Complexity acceptable

Dependencies:
- [ ] All critical vulns fixed
- [ ] Outdated packages updated
- [ ] Unused packages removed
- [ ] Tests pass

Tests:
- [ ] Coverage >80%
- [ ] No flaky tests
- [ ] Performance tests included

Documentation:
- [ ] README current
- [ ] Architecture documented
- [ ] API documented
- [ ] Setup guide works

Performance:
- [ ] Page load <2.5s
- [ ] API response <100ms
- [ ] Bundle optimized
- [ ] Build time acceptable

Security:
- [ ] All vulns fixed
- [ ] No hardcoded secrets
- [ ] Auth/auth verified
- [ ] Security scan passes
```

### 5.2 Knowledge Transfer

**File:** `.ai/issues/CLEANUP-000/handover.md`

```markdown
# Cleanup Project Handover

## What Changed
[Summary of all changes made]

## Architecture Changes
[If any architectural improvements were made]

## Dependency Updates
[List of updated/removed dependencies]

## Code Organization
[New structure/organization if changed]

## Testing
[New test coverage, testing approach]

## Documentation
[New/updated documentation]

## Performance Improvements
[Metrics before/after]

## Security Fixes
[Vulnerabilities addressed]

## Team Knowledge
- Key learnings documented in `.ai/knowledgebase/`
- Patterns established for maintaining cleanup
- New processes implemented

## Maintenance Going Forward
1. Keep dependencies updated (monthly)
2. Maintain test coverage >80%
3. Regular security scans (quarterly)
4. Documentation updates (with code)
5. Performance monitoring (continuous)

## Contact
For questions about cleanup decisions:
- Code quality: @TechLead
- Dependencies: @DevOps
- Security: @Security
- Performance: @Architect
```

## Anti-Patterns to Avoid

❌ **Doing too much at once**
```
Don't: Try to fix everything in parallel
Do: Focus areas sequentially, prioritize by impact
```

❌ **Skipping tests**
```
Don't: Cleanup without comprehensive test coverage
Do: Add tests first, then refactor
```

❌ **Ignoring team input**
```
Don't: Make decisions in isolation
Do: Get team consensus on approach
```

❌ **Not documenting decisions**
```
Don't: Change things without explanation
Do: Document why changes were made in ADRs
```

❌ **Breaking production**
```
Don't: Deploy directly to production
Do: Test in staging, gradual rollout
```

## Success Criteria

✅ **Security**
- All critical vulnerabilities fixed
- 0 hardcoded secrets
- Security scan passes

✅ **Code Quality**
- Duplication <8%
- Coverage >80%
- Complexity acceptable

✅ **Dependencies**
- All critical updates applied
- Unused packages removed
- No conflicts

✅ **Tests**
- Coverage >80%
- <1 flaky test
- Performance baseline established

✅ **Documentation**
- All docs current
- Setup guide works
- Architecture documented

✅ **Performance**
- Page load <2.5s
- API response <100ms
- Bundle size optimized

## Tools & Utilities

```
Code Analysis:
- ESLint / Pylint
- SonarQube
- Duplicate detection tools

Dependency Management:
- npm audit / pip audit
- OWASP Dependency-Check
- License scanning

Testing:
- Coverage tools (Istanbul, Coverage.py)
- Performance testing
- Security testing

Monitoring:
- Build times
- Bundle size
- Performance metrics
- Security findings
```

## Timeline Template

```
Week 1: Assessment (Phase 1)
- Code analysis
- Dependency audit
- Test coverage check
- Performance profiling
- Security scan

Week 2: Planning (Phase 2)
- Consolidate findings
- Create sprint plan
- Prioritize work
- Team kickoff

Weeks 3-7: Execution (Phase 3)
- Sprint 1: Critical fixes
- Sprint 2: Dependencies
- Sprint 3: Code quality
- Sprint 4: Documentation
- Sprint 5: Performance

Week 8: Verification (Phase 5)
- Final checks
- Knowledge transfer
- Handover
- Close project

Total: ~6-8 weeks
```

---

**Next Steps:**
1. Schedule kickoff meeting
2. Assign @TechLead as cleanup lead
3. Create assessment issues
4. Start Phase 1: Assessment
5. Weekly progress reviews

**Related Files:**
- [CONTEXT_OPTIMIZATION.md](.ai/guidelines/CONTEXT_OPTIMIZATION.md)
- [GL-002-SUBAGENT_DELEGATION.md](../../.ai/guidelines/GL-002-SUBAGENT_DELEGATION.md)
- [code-review.prompt.md](../../.github/prompts/code-review.prompt.md)
- [dependency-upgrade-research.prompt.md](../../.github/prompts/dependency-upgrade-research.prompt.md)
