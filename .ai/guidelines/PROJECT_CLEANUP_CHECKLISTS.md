---
docid: GL-091
title: PROJECT_CLEANUP_CHECKLISTS
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

# Project Cleanup Checklists

**Quick Reference for Cleanup Tasks**

## Pre-Cleanup Checklist

### Prerequisites
- [ ] Team aligned on cleanup goals
- [ ] Timeline and sprints planned
- [ ] Agents assigned
- [ ] All assessment reports completed
- [ ] Risk assessment done
- [ ] Staging environment ready
- [ ] Backup/rollback plan documented

### Tools & Setup
- [ ] Code analysis tools installed (ESLint, SonarQube, etc.)
- [ ] Dependency audit tools ready (npm audit, etc.)
- [ ] Performance profiling tools available
- [ ] Security scanning tools configured
- [ ] CI/CD pipelines ready for testing
- [ ] Monitoring/metrics collection enabled

### Team Preparation
- [ ] Team trained on cleanup process
- [ ] Responsibilities assigned
- [ ] Communication channels established
- [ ] Escalation paths defined
- [ ] Handoff procedures documented

---

## Dimension Checklists

### 1. Security Cleanup

**Assessment:**
- [ ] OWASP scan completed
- [ ] Dependency vulnerabilities identified
- [ ] Hardcoded secrets found and documented
- [ ] Auth/authz reviewed
- [ ] CVE list created with severity levels

**Fixes (Priority Order):**
- [ ] Critical vulnerabilities fixed
- [ ] Hardcoded secrets removed
- [ ] Dependency security updates applied
- [ ] Authentication hardened
- [ ] Security testing added

**Verification:**
- [ ] Security scan re-run (0 critical)
- [ ] Penetration test (if applicable)
- [ ] Security team sign-off
- [ ] Deployment monitoring

**Documentation:**
- [ ] Security fixes documented in ADRs
- [ ] Security testing guidelines created
- [ ] Monitoring/alerting in place

---

### 2. Dependency Cleanup

**Assessment:**
- [ ] All dependencies listed and versioned
- [ ] Outdated packages identified
- [ ] Unused dependencies found
- [ ] Vulnerability scan completed
- [ ] Breaking change analysis for major updates

**Updates (Priority Order):**
- [ ] Critical security updates
- [ ] Non-breaking patch/minor updates (in batch)
- [ ] Breaking major updates (with migration plan)
- [ ] Unused dependencies removed

**Verification:**
- [ ] Build succeeds
- [ ] All tests pass
- [ ] Staging deploy successful
- [ ] Performance metrics stable
- [ ] No console warnings

**Documentation:**
- [ ] Update `package.json`/`requirements.txt` clean
- [ ] Migration path documented for major updates
- [ ] Dependency policy documented

**Maintenance:**
- [ ] Set up dependency update automation (e.g., Dependabot)
- [ ] Schedule monthly reviews
- [ ] Alert on security advisories

---

### 3. Code Quality Cleanup

**Assessment:**
- [ ] Duplication analysis completed
- [ ] Complexity hotspots identified
- [ ] Dead code found
- [ ] Architectural violations noted
- [ ] Tech debt items catalogued

**Refactoring (Priority Order):**
- [ ] Extract duplicate code to utilities
- [ ] Reduce complexity in hotspots
- [ ] Remove dead code
- [ ] Fix architecture violations
- [ ] Consolidate similar functions

**Verification:**
- [ ] All tests pass
- [ ] Duplication % reduced
- [ ] Complexity metrics improved
- [ ] Code review approval
- [ ] No performance regressions

**Documentation:**
- [ ] New utilities documented
- [ ] Architectural improvements in ADR
- [ ] Refactoring decisions captured

**Maintenance:**
- [ ] Complexity rules enforced in CI
- [ ] Dead code detection in CI
- [ ] Regular duplication analysis

---

### 4. Test Coverage Cleanup

**Assessment:**
- [ ] Coverage % measured
- [ ] Critical untested paths identified
- [ ] Flaky tests catalogued
- [ ] Slow tests identified
- [ ] Test organization reviewed

**Improvements (Priority Order):**
- [ ] Fix flaky tests
- [ ] Add tests for critical paths
- [ ] Improve slow tests
- [ ] Reorganize test structure
- [ ] Add missing test types (integration, E2E)

**Verification:**
- [ ] Coverage >80%
- [ ] No flaky tests
- [ ] Test execution <acceptable time
- [ ] All tests have descriptive names
- [ ] Test organization logical

**Documentation:**
- [ ] Testing guidelines documented
- [ ] New test patterns in knowledgebase
- [ ] Coverage requirements in CI

**Maintenance:**
- [ ] Enforce coverage thresholds in CI
- [ ] Monitor test flakiness
- [ ] Regular coverage reports

---

### 5. Documentation Cleanup

**Assessment:**
- [ ] README completeness checked
- [ ] Setup guide accuracy tested
- [ ] Architecture docs status reviewed
- [ ] API docs coverage verified
- [ ] Outdated sections identified

**Updates (Priority Order):**
- [ ] Fix setup guide (make it work)
- [ ] Update README with current info
- [ ] Document architecture
- [ ] Complete API documentation
- [ ] Add deployment docs

**Verification:**
- [ ] Setup guide works end-to-end
- [ ] All code examples tested
- [ ] No broken links
- [ ] Screenshots/diagrams current
- [ ] Team review completed

**Documentation:**
- [ ] Create doc maintenance process
- [ ] Assign doc ownership
- [ ] Set update frequency

**Maintenance:**
- [ ] Docs updated with code changes
- [ ] Quarterly review cycle
- [ ] Link validation in CI

---

### 6. Performance Cleanup

**Assessment:**
- [ ] Baseline metrics established
- [ ] Profiling completed
- [ ] Bottlenecks identified
- [ ] Comparison to targets done
- [ ] Root causes documented

**Optimizations (Priority Order):**
- [ ] Fix most impactful bottlenecks
- [ ] Reduce bundle size
- [ ] Optimize database queries
- [ ] Improve rendering/API response
- [ ] Reduce build time

**Verification:**
- [ ] Metrics improved
- [ ] No regressions elsewhere
- [ ] Staging performance stable
- [ ] Monitoring alerting added

**Documentation:**
- [ ] Performance optimizations documented
- [ ] New targets established
- [ ] Monitoring dashboard created

**Maintenance:**
- [ ] Continuous performance monitoring
- [ ] Monthly performance reviews
- [ ] Performance regression tests

---

### 7. Technical Debt

**Assessment:**
- [ ] Tech debt items listed
- [ ] Effort estimated
- [ ] Impact evaluated
- [ ] Prioritized
- [ ] Dependencies mapped

**Reduction (Select High-Impact Items):**
- [ ] Refactor high-complexity functions
- [ ] Consolidate similar implementations
- [ ] Update legacy patterns
- [ ] Improve error handling
- [ ] Enhance logging

**Verification:**
- [ ] Functionality preserved
- [ ] Tests pass
- [ ] Code review approved
- [ ] No performance impact

**Documentation:**
- [ ] Tech debt decisions in ADRs
- [ ] Remaining tech debt tracked
- [ ] Prevention patterns documented

**Maintenance:**
- [ ] Prevent new tech debt
- [ ] Regular tech debt reviews
- [ ] Allocate time quarterly

---

### 8. Git Repository Cleanup

**Assessment:**
- [ ] Old branches identified
- [ ] Abandoned PRs reviewed
- [ ] Large files detected
- [ ] Commit history analyzed
- [ ] Protected branch rules reviewed

**Cleanup:**
- [ ] Delete old/merged branches
- [ ] Close abandoned PRs
- [ ] Document branch strategy
- [ ] Remove large binary files
- [ ] Update protected branch rules

**Verification:**
- [ ] Repository size reduced
- [ ] Branch list clean
- [ ] CI rules enforced
- [ ] Access controls reviewed

**Documentation:**
- [ ] Git workflow documented
- [ ] Branch naming conventions
- [ ] PR template established

**Maintenance:**
- [ ] Regular branch cleanup
- [ ] PR/issue archival process
- [ ] Repository size monitoring

---

### 9. Infrastructure Cleanup

**Assessment:**
- [ ] Unused resources identified
- [ ] Configuration reviewed
- [ ] Secrets management checked
- [ ] Resource costs analyzed
- [ ] Compliance reviewed

**Cleanup:**
- [ ] Terminate unused resources
- [ ] Remove unused config
- [ ] Migrate secrets to vault
- [ ] Document resource allocation
- [ ] Update IAM policies

**Verification:**
- [ ] Services still functioning
- [ ] Cost reduced
- [ ] Compliance maintained
- [ ] Security improved

**Documentation:**
- [ ] Infrastructure as code updated
- [ ] Resource allocation documented
- [ ] Cost monitoring enabled

**Maintenance:**
- [ ] Regular resource reviews
- [ ] Cost optimization
- [ ] Security compliance monitoring

---

### 10. Team Knowledge

**Assessment:**
- [ ] Knowledge gaps identified
- [ ] Tribal knowledge documented
- [ ] Architecture understood by team
- [ ] Deployment process known
- [ ] Emergency procedures documented

**Knowledge Transfer:**
- [ ] Create onboarding guide
- [ ] Document key decisions (ADRs)
- [ ] Record architecture explanations
- [ ] Capture troubleshooting guides
- [ ] Document common tasks

**Verification:**
- [ ] New team member can onboard
- [ ] Key decisions explained
- [ ] Runbooks available
- [ ] Emergency procedures clear

**Documentation:**
- [ ] Knowledge base in `.ai/knowledgebase/`
- [ ] Onboarding checklist
- [ ] FAQ for common issues

**Maintenance:**
- [ ] Update knowledge docs with changes
- [ ] Monthly knowledge refresh
- [ ] Team feedback on gaps

---

## Weekly Tracking Checklist

### Monday
- [ ] Review previous week results
- [ ] Plan current week tasks
- [ ] Identify any blockers
- [ ] Adjust timeline if needed
- [ ] Update team on priorities

### Tuesday-Thursday
- [ ] Execute assigned cleanup tasks
- [ ] Code review cleanup work
- [ ] Run tests after changes
- [ ] Update task progress
- [ ] Flag blockers early

### Friday
- [ ] Complete all planned tasks
- [ ] Consolidate progress
- [ ] Generate weekly report
- [ ] Identify next week's work
- [ ] Team sync on status

---

## Final Verification Checklist

### Before Going Live

**Security:**
- [ ] All critical vulns fixed
- [ ] No hardcoded secrets
- [ ] Security tests pass
- [ ] Security team approved

**Code:**
- [ ] No critical issues
- [ ] Coverage >80%
- [ ] Tests all pass
- [ ] Code review approved

**Dependencies:**
- [ ] All updates tested
- [ ] No conflicts
- [ ] Security scan clean
- [ ] Compatibility verified

**Performance:**
- [ ] Metrics meet targets
- [ ] No regressions
- [ ] Monitoring in place
- [ ] Alerts configured

**Documentation:**
- [ ] Current and accurate
- [ ] All examples work
- [ ] No broken links
- [ ] Team reviewed

**Operations:**
- [ ] Deployment tested in staging
- [ ] Rollback plan ready
- [ ] Monitoring dashboard ready
- [ ] Team trained on changes

---

## Post-Cleanup Checklist

### Handoff & Monitoring

**Deployment:**
- [ ] Deploy to production
- [ ] Monitor metrics closely
- [ ] Be ready to rollback
- [ ] Document any issues

**Verification:**
- [ ] Services operational
- [ ] Metrics as expected
- [ ] No error spikes
- [ ] User feedback positive

**Documentation:**
- [ ] Create handover document
- [ ] Update runbooks
- [ ] Record lessons learned
- [ ] Archive assessment reports

**Maintenance Plan:**
- [ ] Establish monitoring
- [ ] Schedule future reviews
- [ ] Prevent regression
- [ ] Allocate maintenance time

**Team Feedback:**
- [ ] Retrospective meeting
- [ ] Capture learnings
- [ ] Identify improvements
- [ ] Update processes

---

## Emergency Procedures

**If Something Breaks:**

```
1. STOP: Do not proceed
2. ASSESS: What's broken?
3. NOTIFY: Alert team immediately
4. INVESTIGATE: Root cause analysis
5. ROLLBACK: If deployment cause
   - Revert changes
   - Restore from backup
   - Verify stability
6. COMMUNICATE: Update stakeholders
7. DOCUMENT: Incident report
8. PREVENT: Add safeguards
```

**Rollback Procedure:**

```
If recent cleanup caused issues:
1. Identify specific change that caused issue
2. Revert that change locally
3. Test reversal
4. Deploy revert to staging
5. Verify in staging
6. Deploy to production
7. Monitor metrics
8. Document root cause
9. Plan safer approach
```

---

## Success Metrics Template

Track these during cleanup:

```
Weekly Metrics:
├── Security
│   ├── Vulnerabilities: [Start] → [Current] → [Target]
│   └── Hardcoded secrets: [Start] → [Current] → [Target]
├── Code Quality
│   ├── Duplication: [Start]% → [Current]% → [Target]%
│   └── Complexity: [Start] → [Current] → [Target]
├── Tests
│   ├── Coverage: [Start]% → [Current]% → [Target]%
│   └── Flaky tests: [Start] → [Current] → [Target]
├── Dependencies
│   ├── Outdated: [Start] → [Current] → [Target]
│   └── Vulnerabilities: [Start] → [Current] → [Target]
└── Performance
    ├── Load time: [Start]ms → [Current]ms → [Target]ms
    └── Bundle size: [Start]KB → [Current]KB → [Target]KB
```

---

**Use these checklists with:**
- [project-cleanup.workflow.md](.ai/workflows/project-cleanup.workflow.md)
- [project-cleanup.prompt.md](.github/prompts/project-cleanup.prompt.md)
