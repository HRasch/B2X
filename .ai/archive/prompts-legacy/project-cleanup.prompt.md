# Project Cleanup Prompt

## Purpose
Guide team through systematic project cleanup covering code quality, dependencies, documentation, tests, performance, and security.

## When to Use

**Start cleanup when project has:**
- Accumulating technical debt
- Outdated dependencies
- Low test coverage
- Documentation gaps
- Performance issues
- Security vulnerabilities
- Code duplication
- Aging codebase (>2 years)

## Quick Assessment

### Ask Yourself

```
Code Quality:
- [ ] Are there obvious duplications?
- [ ] Is code hard to understand?
- [ ] Do we have technical debt?
→ If YES: Include in cleanup

Dependencies:
- [ ] Are dependencies outdated?
- [ ] Are there security vulnerabilities?
- [ ] Do we have unused packages?
→ If YES: Include in cleanup

Testing:
- [ ] Is coverage below 75%?
- [ ] Are there flaky tests?
- [ ] Are there untested critical paths?
→ If YES: Include in cleanup

Documentation:
- [ ] Is README outdated?
- [ ] Are setup instructions broken?
- [ ] Is architecture not documented?
→ If YES: Include in cleanup

Performance:
- [ ] Is page load slow (>3s)?
- [ ] Are API responses slow (>200ms)?
- [ ] Is bundle size large (>1MB)?
→ If YES: Include in cleanup

Security:
- [ ] Are there known vulnerabilities?
- [ ] Is there hardcoded config/secrets?
- [ ] Are best practices not followed?
→ If YES: Include in cleanup
```

## Phase 1: Assessment Questions

### Code Analysis

**Ask @TechLead with @SubAgent-Analysis:**

```
Please analyze our codebase for:
1. Dead code (unused imports, functions, classes)
2. Code duplication (identify duplicate blocks >20 LOC)
3. Complexity hotspots (cyclomatic complexity >10)
4. Architecture violations
5. Tech debt areas (comments, TODOs, FIXMEs)

Provide:
- Total LOC
- Duplication percentage
- Number of hotspots
- Priority list (critical → low)
- Effort estimate for each area

Output: `.ai/issues/CLEANUP-001/code-assessment.md`
```

### Dependency Assessment

**Ask @DevOps with @SubAgent-Research:**

```
Please audit our dependencies for:
1. Outdated packages (compare to latest)
2. Security vulnerabilities (critical/medium/low)
3. Unused dependencies (not imported anywhere)
4. Dependency conflicts
5. Breaking changes for major updates

Provide:
- List of outdated packages with versions
- CVE list for vulnerabilities
- Unused dependencies to remove
- Migration path for major updates
- Recommended update order

Output: `.ai/issues/CLEANUP-002/dependency-audit.md`
```

### Test Coverage Assessment

**Ask @QA with @SubAgent-Analysis:**

```
Please assess our test coverage:
1. Current coverage percentage
2. Untested critical code paths
3. Flaky tests (identify and characterize)
4. Slow tests (>5s execution time)
5. Missing test types (unit, integration, E2E)

Provide:
- Coverage report (by file/module)
- List of 5-10 critical untested paths
- Flaky test list with causes
- Slow test list with bottlenecks
- Test structure recommendations

Output: `.ai/issues/CLEANUP-003/test-assessment.md`
```

### Documentation Assessment

**Ask @TechLead:**

```
Please audit documentation for:
1. README completeness vs. reality
2. Setup guide accuracy (is it still valid?)
3. Architecture documentation (exists? up-to-date?)
4. API documentation (complete? examples work?)
5. Configuration documentation
6. Deployment documentation

Provide:
- README checklist (what's missing)
- Setup guide issues
- Architecture docs status
- API docs coverage
- Deployment docs status
- Prioritized list of updates needed

Output: `.ai/issues/CLEANUP-004/docs-assessment.md`
```

### Performance Assessment

**Ask @Backend/@Frontend with @SubAgent-Optimization:**

```
Please profile our application for:
1. Frontend load time (measure with real traffic if possible)
2. API response times (by endpoint)
3. Database query performance (slow queries)
4. Bundle size (JavaScript, CSS, assets)
5. Build time (current vs. acceptable)
6. Memory usage (leaks? abnormal patterns?)

Provide:
- Baseline metrics for all areas
- Hotspot identification
- Comparison to industry standards
- Quick wins for optimization
- Estimated effort for major improvements

Output: `.ai/issues/CLEANUP-005/performance-profile.md`
```

### Security Assessment

**Ask @Security with @SubAgent-Security:**

```
Please scan codebase for:
1. OWASP top 10 vulnerabilities
2. Hardcoded secrets (API keys, passwords, config)
3. Dependency vulnerabilities (run npm audit equivalent)
4. Authentication/authorization issues
5. Data exposure risks
6. SQL injection / XSS / CSRF vulnerabilities

Provide:
- OWASP mapping (which vulnerabilities found)
- Critical findings (immediate action needed)
- Medium findings (should fix soon)
- Low findings (good to fix)
- Hardcoded secrets locations
- Remediation steps for each

Output: `.ai/issues/CLEANUP-006/security-scan.md`
```

## Phase 2: Planning Questions

### Consolidate Findings

**Ask @SARAH:**

```
Based on these assessment files:
- .ai/issues/CLEANUP-001/code-assessment.md
- .ai/issues/CLEANUP-002/dependency-audit.md
- .ai/issues/CLEANUP-003/test-assessment.md
- .ai/issues/CLEANUP-004/docs-assessment.md
- .ai/issues/CLEANUP-005/performance-profile.md
- .ai/issues/CLEANUP-006/security-scan.md

Please consolidate into a cleanup plan:

1. Identify all issues
2. Prioritize by:
   - Impact (critical/high/medium/low)
   - Effort (hours)
   - Risk
3. Create cleanup sprints (1-2 weeks each)
4. Assign areas to agents
5. Create Gantt/timeline
6. Identify dependencies between tasks
7. Risk assessment for each sprint

Output: `.ai/issues/CLEANUP-000/cleanup-plan.md`
```

### Risk Assessment

**For each major area:**

```
For [Area]:
- What could go wrong?
- What's the blast radius?
- How can we mitigate?
- What testing is needed?
- How do we rollback if needed?
- What's the communication plan?

Example for "Update React 17→18":
- Risk: Component breaking changes, styling issues
- Blast: Affects all frontend components
- Mitigation: Stage in dev→staging→production, comprehensive tests
- Testing: All components must pass E2E tests
- Rollback: Keep v17 branch, can quickly switch
- Communication: Update frontend team, planning meeting
```

## Phase 3: Execution Questions

### Weekly Planning

**Monday of each week:**

```
For the current cleanup sprint:
- What are the top 3 priorities for this week?
- Which tasks should be done in parallel?
- Who is assigned to each?
- What are the blockers?
- What should be completed by Friday?

Output: Update `.ai/sprint/current.md`
```

### Daily Standups

**Each morning (async):**

```
For each agent working on cleanup:
- What did you complete yesterday?
- What are you working on today?
- Any blockers?
- Any help needed?
- Estimated completion?

Update: `.ai/logs/cleanup-daily-{date}.md`
```

### Code Review During Cleanup

**For each cleanup commit:**

```
@TechLead / @SubAgent-Review:
- Does this fix the identified issue?
- Are tests added/updated?
- Does it break anything?
- Are there side effects?
- Is it deployment-safe?

Criteria:
- ✅ Fixes the issue
- ✅ Tests pass
- ✅ No regressions
- ✅ Documented
- ✅ Safe to deploy
```

## Phase 4: Tracking Questions

### Daily Tracking

**End of day:**

```
Update progress in `.ai/logs/cleanup-daily-{date}.md`:
- Completed items (with ✅)
- In-progress items
- Blockers (if any)
- Tomorrow's plan
- Any risks emerged?
```

### Weekly Review

**Friday afternoon:**

```
@SARAH consolidates:
- How many tasks completed?
- Are we on pace?
- What's the blockers?
- Any team feedback?
- Adjust plan if needed?

Output: `.ai/logs/cleanup-week-{week}.md`
```

### Metrics to Track

```
Weekly metrics:
- Duplication ratio (target: <8%)
- Test coverage (target: >80%)
- Security vulns (target: 0 critical)
- Outdated packages (target: 0)
- Page load time (target: <2.5s)
- API response time (target: <100ms)
- Build time (target: <5 min)
- Code complexity (trending down)
```

## Phase 5: Completion Questions

### Final Verification

**Before marking complete:**

```
For each dimension, verify:

Code Quality:
- [ ] No critical issues?
- [ ] Duplication <8%?
- [ ] All tests pass?

Dependencies:
- [ ] All critical vulns fixed?
- [ ] Packages updated?
- [ ] Unused removed?
- [ ] Tests pass?

Tests:
- [ ] Coverage >80%?
- [ ] No flaky tests?
- [ ] Performance tests added?

Documentation:
- [ ] README current?
- [ ] Setup works?
- [ ] Architecture documented?

Performance:
- [ ] Measured improvement?
- [ ] Baselines established?
- [ ] Monitoring in place?

Security:
- [ ] All vulns fixed?
- [ ] No secrets in code?
- [ ] Security scan passes?
```

### Knowledge Transfer

**Before closing:**

```
Document for team:
1. What changed and why?
2. New processes/standards established?
3. Architecture improvements made?
4. Tools/scripts for maintaining cleanup?
5. Monitoring approach?
6. Escalation points?

Create: `.ai/issues/CLEANUP-000/handover.md`
```

## Common Cleanup Scenarios

### Scenario 1: High Duplication

```
Issue: 15% code duplication in business logic

Approach:
1. Identify duplicate blocks
2. Extract to shared utilities
3. Add tests for utilities
4. Update all references
5. Verify no regressions

Agents: @Backend, @TechLead, @QA
Effort: 20-30 hours
Risk: Low (with good tests)
```

### Scenario 2: Outdated Dependencies

```
Issue: 25 packages outdated, 5 with security vulns

Approach:
1. Fix critical security issues first
2. Update non-breaking packages in batch
3. Test thoroughly
4. Handle breaking changes
5. Update documentation

Agents: @DevOps, @Backend, @Frontend, @QA
Effort: 40-50 hours
Risk: Medium (compatibility issues possible)
```

### Scenario 3: Low Test Coverage

```
Issue: 55% coverage, critical paths untested

Approach:
1. Identify untested critical paths
2. Write integration tests first
3. Add unit tests
4. Fix flaky tests
5. Establish coverage CI gates

Agents: @QA, @Backend, @Frontend
Effort: 50-60 hours
Risk: Low (additions only)
```

### Scenario 4: Security Vulnerabilities

```
Issue: 8 CVEs found, 3 critical

Approach:
1. Fix critical vulns immediately
2. Develop security test suite
3. Establish security scanning
4. Update dependencies
5. Security review process

Agents: @Security, @Backend, @DevOps
Effort: 25-35 hours
Risk: High (depends on vulns), requires careful testing
```

### Scenario 5: Performance Issues

```
Issue: Page load 4s (target 2s), API slow

Approach:
1. Profile to find bottlenecks
2. Prioritize by impact
3. Optimize database queries
4. Reduce bundle size
5. Implement monitoring

Agents: @Frontend, @Backend, @DevOps
Effort: 40-50 hours
Risk: Medium (performance regressions possible)
```

## Decision Trees

### Dependency Update Decision

```
Found outdated package:
├─ Is it a major version?
│  ├─ YES: Check breaking changes
│  │        ├─ Breaking changes?
│  │        │  ├─ YES: Plan migration sprint
│  │        │  └─ NO: Update & test
│  │        └─ Output: migration plan
│  └─ NO: Check for vulnerabilities
│         ├─ Critical/Medium?
│         │  ├─ YES: Update immediately
│         │  └─ NO: Update in batch
│         └─ Output: update list

Result: Prioritized update plan
```

### Code Refactoring Decision

```
Found duplication or complexity:
├─ Is it critical path?
│  ├─ YES: Must refactor
│  └─ NO: Check effort/benefit
├─ Check test coverage
│  ├─ <80%: Add tests first
│  └─ >80%: Can refactor safely
├─ Risk assessment
│  ├─ High risk: Smaller changes
│  └─ Low risk: Larger refactoring
└─ Result: Refactoring approach
```

## Documentation Template

```markdown
# Cleanup Task: {Name}

## Issue Description
[What's wrong/outdated]

## Impact
- Affected files: {X}
- Risk level: {Low/Medium/High}
- Business impact: {e.g., security, performance, quality}
- Effort: {X hours}

## Current State
[Before metrics/description]

## Target State
[After metrics/description]

## Approach
1. [Step 1]
2. [Step 2]
3. [Step 3]

## Testing Strategy
[How to verify fix is good]

## Rollback Plan
[How to undo if problems]

## Assigned To
@Agent (lead)

## Success Criteria
- [ ] Criteria 1
- [ ] Criteria 2
- [ ] Criteria 3

## Status
In Progress / Completed / Blocked

---
Tracked in: `.ai/issues/CLEANUP-XXX/`
```

---

**Related:**
- [project-cleanup.workflow.md](../../.ai/workflows/project-cleanup.workflow.md) - Full workflow with examples
- [CONTEXT_OPTIMIZATION.md](../../.ai/guidelines/CONTEXT_OPTIMIZATION.md) - Code cleanup principles
- [code-review.prompt.md](code-review.prompt.md) - Quality review checklist
- [dependency-upgrade-research.prompt.md](dependency-upgrade-research.prompt.md) - Dependency analysis
