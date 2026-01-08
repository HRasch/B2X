# B2X Development Process Framework

**Version**: 1.0  
**Last Updated**: 29. Dezember 2025  
**Purpose**: Structured, collaborative development workflow with agent coordination, quality gates, and continuous improvement

---

## 📋 Table of Contents

1. [Overview & Principles](#overview--principles)
2. [Definition of Ready (DoR)](#definition-of-ready-dor)
3. [Team Roles & Collaboration Network](#team-roles--collaboration-network)
4. [Buildability & Code Compilation Gate](#buildability--code-compilation-gate)
5. [Code Ownership & Responsibility](#code-ownership--responsibility)
6. [Development Workflow](#development-workflow)
7. [Pull Request Workflow](#pull-request-workflow) ✨ NEW
8. [Agent-Change Marking System](#agent-change-marking-system)
9. [Inter-Role Question & Support Protocol](#inter-role-question--support-protocol)
10. [QA Bug Loop & Issue Reporting](#qa-bug-loop--issue-reporting)
11. [Quality Gates & Testing Requirements](#quality-gates--testing-requirements)
12. [Documentation & API Standards](#documentation--api-standards)
13. [Retrospective & Continuous Improvement](#retrospective--continuous-improvement)
14. [Critical Issues Escalation](#critical-issues-escalation)
15. [Tools & Templates](#tools--templates)

---

## 🎯 Overview & Principles

### Core Principles

```
1. COLLABORATIVE: All roles work together, no silos
2. TRANSPARENT: Every change marked and traceable
3. QUALITY-FIRST: Testing before commit
4. ASYNC-CAPABLE: Support requests answered by other roles
5. CONTINUOUS-IMPROVEMENT: Retrospectives drive process evolution
6. CRITICAL-ONLY-STOP: Continue unless critical issue blocks
7. DOCUMENTED: Every API/Interface documented before merge
```

### Success Metrics

- ✅ Zero untracked AI agent changes
- ✅ 100% inter-role question resolution (avg 24h)
- ✅ 80%+ code coverage on all features
- ✅ 100% API/Interface documentation
- ✅ <5 critical issues per sprint
- ✅ Retrospectives drive 10%+ automation gain/sprint
- ✅ **100% build success before phase transitions**
- ✅ **100% DoR compliance (no work without DoR)**
- ✅ **Zero code changes outside responsible developer**
- ✅ **100% QA bugs returned to original developer**

---

## 📋 Definition of Ready (DoR)

### What is Definition of Ready?

Every issue **MUST** be marked as "Ready for Development" before ANY developer pulls it from GitHub. This is the **mandatory first step** of the development process.

### DoR Checklist (Before Development Starts)

**Product Owner Responsibility:**
```
✅ ACCEPTANCE CRITERIA
   [ ] Feature description is clear and unambiguous
   [ ] Acceptance criteria: At least 3 specific, testable criteria
   [ ] User story format: "As a [role], I want [feature], so that [value]"
   [ ] Example: "As a customer, I want to see VAT in pricing, so that I understand total cost"

✅ DEPENDENCIES
   [ ] No blocking issues (must be resolved first)
   [ ] Required APIs/services identified
   [ ] Database schema changes documented (if needed)
   [ ] External service integrations documented

✅ TESTING REQUIREMENTS
   [ ] Test scenarios documented (at least 3)
   [ ] Edge cases identified (null, empty, invalid inputs)
   [ ] Performance requirements stated (if applicable)
   [ ] Compliance tests identified (if P0.x feature)

✅ TECHNICAL SPECIFICATIONS
   [ ] Architecture decision recorded (if new pattern)
   [ ] Database schema provided (if data layer change)
   [ ] API contract defined (if new endpoint)
   [ ] Security requirements listed (PII, encryption, etc.)

✅ DOCUMENTATION PLAN
   [ ] API documentation needed? (Yes/No)
   [ ] Code examples required? (Yes/No)
   [ ] Changelog entry needed? (Yes/No)
   [ ] Runbook needed? (Yes/No)

✅ ESTIMATION
   [ ] Story points assigned (1-13)
   [ ] Effort breakdown provided (design, dev, test, docs)
   [ ] No story > 13 points (break down if needed)

✅ APPROVAL
   [ ] Product Owner approved: @name
   [ ] Tech Lead reviewed: @name
   [ ] Architecture validated (if needed): @architect
   [ ] Marked as "Ready for Development"
```

### DoR Validation Process

```
┌───────────────────────────────────────────────────────────────┐
│ ISSUE CREATED BY PRODUCT OWNER                                │
└───────────────────────────┬──────────────────────────────────┘
                            ↓
                   TECH LEAD REVIEW (< 24h)
                   - Is acceptance criteria clear?
                   - Are dependencies documented?
                   - Can this be estimated accurately?
                       ↓
                   PASS or FAIL?
                   ↙            ↘
                FAIL           PASS
                 ↓             ↓
            Return to      Architect Review
            Product        (if new pattern)
            Owner          └─ Approve or Return
            (refine)          ↓
                 ↓          Approved?
                 └─────┬─ YES → Mark "Ready"
                       └─ NO → Architect feedback
                              to Product Owner
                       ↓
                ┌──────────────────────────┐
                │ PULL READY ISSUE         │
                │ (Developer starts)       │
                └──────────────────────────┘
```

### DoR Labels in GitHub

```yaml
labels:
  - "status: ready-for-dev"     # Issue is ready to be pulled
  - "status: waiting-approval"  # Waiting for DoR review
  - "status: needs-clarification" # Needs more details
  - "status: in-progress"       # Developer working on it
```

### DoR Issue Template

```markdown
## [FEATURE] Brief Title

**Issue Type**: Feature / Bug / Tech Debt  
**Assigned To**: @developer-name  
**Story Points**: 5  
**Status**: ⏳ Waiting for DoR Approval

### User Story
As a [role], I want [feature], so that [value]

### Acceptance Criteria
- [ ] **Criterion 1**: Specific, measurable, testable
- [ ] **Criterion 2**: ...
- [ ] **Criterion 3**: ...

### Dependencies
- [ ] No dependencies OR
- [ ] Depends on: #XXX (blocked until merged)
- [ ] Requires: Service API documented
- [ ] Requires: Database schema: [link]

### Test Scenarios
1. **Happy Path**: User does [action] → [expected result]
2. **Error Case**: User provides invalid [input] → [error handling]
3. **Edge Case**: [scenario] → [expected behavior]

### Technical Notes
- Architecture: [pattern name or ADR link]
- Database: [schema changes]
- API: [endpoint definition]
- Security: [PII handling, encryption, auth]

### Definition of Ready
- [ ] Product Owner approved
- [ ] Tech Lead reviewed
- [ ] Architecture validated (if new pattern)
- [ ] All criteria above complete

---

**Status**: ⏳ Waiting DoR Review  
**Ready For Dev**: [Will be marked when ready]
```

---

## 🏗️ Buildability & Code Compilation Gate

### Core Rule: "Software Must Compile Before Next Phase"

**Every transition to a new development phase requires:**

```
┌──────────────────────────────────────────────────────────────┐
│ BUILDABILITY IS A MANDATORY GATE                              │
│                                                               │
│ Before Phase Transition:                                     │
│ ✅ Code MUST compile without errors                          │
│ ✅ ALL tests MUST pass                                       │
│ ✅ NO compiler warnings (treat as errors)                    │
│ ✅ Coverage >= 80% (on changed files)                        │
└──────────────────────────────────────────────────────────────┘
```

### Build Gates by Phase

#### Gate: Before Commit
```bash
# MANDATORY: Developer must run BEFORE committing
dotnet build B2X.slnx
  → If FAIL: Fix immediately, cannot commit
  → If PASS: Continue to testing
  
dotnet test B2X.slnx -v minimal
  → If FAIL: Fix immediately, cannot commit
  → If PASS: Can commit
```

#### Gate: Before Push to GitHub
```bash
# Run full suite locally
dotnet build B2X.slnx    # Must succeed
dotnet test B2X.slnx --collect:"XPlat Code Coverage"
  → Coverage report shows >= 80% on changed code
  → If < 80%: Add tests, cannot push
  
# Check for agent changes marked
grep -r "// 🤖" backend/Domain/[Service]/src
  → If agent changes NOT marked: Mark them, cannot push
```

#### Gate: Before PR Merge (CI Pipeline)
```yaml
# GitHub Actions MUST all pass
✅ Build: dotnet build B2X.slnx
✅ Test: dotnet test B2X.slnx
✅ Coverage: >= 75% (GitHub Actions enforces)
✅ Lint: StyleCop, ESLint (no warnings as errors)
✅ Security: SonarQube scan (no critical issues)

# If ANY fail: PR cannot be merged
# Developer MUST fix and push again
```

#### Gate: Before Staging Deployment
```bash
# Full build on staging environment
dotnet build -c Release B2X.slnx
  → If FAIL: Deployment blocked
  
# All integration tests
dotnet test B2X.slnx --filter "Category=Integration"
  → If FAIL: Deployment blocked
```

#### Gate: Before Production Deployment
```bash
# Same as staging + production verification
✅ Build successful (Release config)
✅ All tests passing
✅ Smoke tests on staging passed
✅ Product Owner sign-off received
✅ Rollback plan documented
```

### Handling Build Failures

```
Build Fails at Any Gate
    ↓
Responsible Developer Notified (GitHub)
    ↓
Developer Fixes Immediately
    (Max 2h response time)
    ↓
Developer Pushes Fix
    ↓
CI Re-runs Automatically
    ↓
Build Succeeds? → Proceed to Next Phase
Build Fails Again? → Escalate to Lead Developer
    ↓
[Lead Developer assists in root cause]
```

---

## 👤 Code Ownership & Responsibility

### Principle: "Only Responsible Developer Modifies Code"

**This is a CRITICAL quality rule** to prevent tangled ownership and accountability issues.

### Code Assignment Rules

```
1. ISSUE PULLED BY DEVELOPER
   └─ That developer is the OWNER
   └─ Only that developer modifies the code
   └─ Only that developer fixes bugs found in testing

2. CODE REVIEW FEEDBACK
   └─ Reviewer provides feedback in PR comments
   └─ Reviewer does NOT push changes
   └─ Owner makes changes requested
   └─ Reviewer approves modified code

3. CONCURRENT CHANGES FORBIDDEN
   ❌ Two developers on same feature = NOT ALLOWED
   ❌ Reviewer pushing fixes = NOT ALLOWED
   ❌ QA modifying code = NOT ALLOWED
   ✅ Owner modifies based on feedback only

4. HANDOFF ONLY IF ESSENTIAL
   If owner must hand off (vacation, emergency):
   ├─ Create new issue for handoff
   ├─ Original owner: @john-dev → Current work summary
   ├─ New owner: @jane-dev → Pulls handoff issue
   ├─ Lead Developer: Approves handoff
   └─ Document reason + handoff notes in issue
```

### Code Review Process (No Code Changes by Reviewer)

```
Owner → Pushes PR with Code
        ↓
Lead Developer → Reviews Code
    ├─ Comment: "Use async/await here"
    ├─ Comment: "Add validation for null input"
    ├─ Comment: "Missing unit test for error case"
    └─ Does NOT push changes
        ↓
Owner → Reads feedback
    ├─ Makes code changes requested
    ├─ Adds requested validation
    ├─ Writes missing unit test
    └─ Pushes updated code
        ↓
Lead Developer → Re-reviews
    ├─ Code looks good
    └─ Approves PR
```

### Breaking the Rule: Escalation

**If reviewer finds critical issues that owner won't fix:**

```
Reviewer Rejects PR + Escalates to Lead Developer
    ↓
Lead Developer Contacts Owner
    ├─ Option 1: Owner fixes (preferred)
    ├─ Option 2: Pair programming session
    ├─ Option 3: Issue reassigned to new developer
    └─ Last Resort: Code revert
```

### Responsibility Matrix

| Task | Responsible | Can Assist |
|------|-------------|-----------|
| Write code | Owner | None |
| Fix code review feedback | Owner | None |
| Write tests | Owner | QA (test design) |
| Fix test failures | Owner | Lead Dev (strategy) |
| Fix bugs found in QA | Owner | QA (documentation) |
| Fix performance issues | Owner | Architect (design) |
| Merge PR | Lead Developer | None |
| Deploy code | DevOps | Lead Developer |

---

## 👥 Team Roles & Collaboration Network

### Seven Core Roles

```
                    ┌──────────────────────┐
                    │  PRODUCT OWNER       │
                    │  (Vision & Backlog)  │
                    └──────────┬───────────┘
                               │
        ┌──────────────────────┼──────────────────────┐
        │                      │                      │
        ▼                      ▼                      ▼
    ┌─────────┐         ┌──────────────┐       ┌──────────────┐
    │SOFTWARE │         │LEAD          │       │DOCUMENTATION│
    │ARCHITECT│         │DEVELOPER     │       │ENGINEER      │
    │         │         │(Tech Lead)   │       │              │
    └────┬────┘         └──────┬───────┘       └──────┬───────┘
         │                     │                      │
         └─────────────────────┼──────────────────────┘
                               │
         ┌─────────────────────┼──────────────────────┐
         │                     │                      │
         ▼                     ▼                      ▼
    ┌──────────┐         ┌──────────┐         ┌──────────┐
    │BACKEND   │         │FRONTEND  │         │QA        │
    │DEVELOPER │         │DEVELOPER │         │ENGINEER  │
    │          │         │          │         │          │
    └──────────┘         └──────────┘         └──────────┘
```

### Role Definitions & Responsibilities

#### 1. **Product Owner** 🎯
- **Responsibility**: Vision, backlog prioritization, acceptance criteria
- **Questions to Ask**: Architecture, Lead Dev, Documentation
- **Collaboration**: Weekly backlog refinement, sprint planning
- **Escalates**: Conflicting requirements, business blocking issues

#### 2. **Software Architect** 🏗️
- **Responsibility**: System design, service boundaries, patterns
- **Questions to Ask**: All roles for technical feasibility
- **Collaboration**: Design reviews, tech debt assessment
- **Escalates**: Architectural conflicts, scalability issues

#### 3. **Lead Developer** 👨‍💼
- **Responsibility**: Code quality, PR reviews, technical decisions
- **Questions to Ask**: All developers, QA for test coverage needs
- **Collaboration**: Sprint planning, mentoring, code review
- **Escalates**: Code quality violations, performance bottlenecks

#### 4. **Backend Developer** 🔧
- **Responsibility**: Wolverine services, database, APIs
- **Questions to Ask**: Architect (design), Lead Dev (review), QA (test scenarios), Frontend (API contract)
- **Collaboration**: Service implementation, integration testing
- **Escalates**: Database design issues, external API problems

#### 5. **Frontend Developer** 🎨
- **Responsibility**: Vue.js, accessibility, UI/UX
- **Questions to Ask**: Backend (API), Architect (patterns), QA (accessibility tests), Documentation (component docs)
- **Collaboration**: Component library, design system, theme
- **Escalates**: Accessibility failures, cross-browser issues

#### 6. **QA Engineer** 🧪
- **Responsibility**: Testing strategy, compliance, automation
- **Questions to Ask**: Developers (test scenarios), Architect (test design), Documentation (test docs)
- **Collaboration**: Test case creation, compliance verification
- **Escalates**: Compliance failures, critical regressions

#### 7. **Documentation Engineer** 📚
- **Responsibility**: API docs, architecture docs, guides
- **Questions to Ask**: All roles for technical details
- **Collaboration**: Living documentation, changelog management
- **Escalates**: Documentation gaps before release

---

## 🔄 Development Workflow

### Phase 0: Pull DoR Issue from GitHub (Day 0)

**MANDATORY FIRST STEP - Every feature starts here!**

```
┌──────────────────────────────────────────────────────────────┐
│ DEVELOPER GOES TO GITHUB                                     │
│ Filters: status:ready-for-dev                                │
└──────────────────────────┬─────────────────────────────────┘
                           ↓
                   Developer Pulls Issue
                   ├─ Reads acceptance criteria
                   ├─ Checks dependencies (all resolved?)
                   ├─ Reads test scenarios
                   ├─ Reviews technical specifications
                   └─ If questions: Ask in issue comments
                                   (Lead Dev responds < 4h)
                           ↓
                   Assign to Self
                   └─ Issue status: "in-progress"
                           ↓
                   [READY TO CODE]
```

**Before accepting issue, verify:**
- ✅ Status label: "ready-for-dev"
- ✅ Acceptance criteria: Clear and specific
- ✅ Dependencies: All resolved or documented
- ✅ Story points: Estimated
- ✅ Architecture: If new pattern, ADR linked

### Phase 1: Planning (Day 1)

```
Product Owner
    ├─ Creates Issue with Acceptance Criteria
    ├─ Marks as "status: waiting-approval"
    └─ Questions → Software Architect
                   (Is this architecturally sound?)
                   ↓
Software Architect
    ├─ Reviews Issue Design
    └─ Questions → Lead Developer
                   (Are we following patterns?)
                   ↓
Lead Developer
    ├─ Reviews DoR completeness
    ├─ Validates technical feasibility
    ├─ Approves: Marks "status: ready-for-dev"
    └─ Notifies developers (issue ready to pull)
        ↓
Developer (When Ready)
    ├─ Pulls issue from GitHub
    ├─ Confirms understanding
    └─ Asks questions (if any)
        ↓
[READY TO START DEVELOPMENT]
```

### Phase 2: Development (Days 2-3)

```
Responsible Developer (Only)
    ├─ Create Feature Branch: feature/#<issue>-<name>
    ├─ Implement with Test-Driven Development (TDD)
    │
    ├─ BUILD GATE #1: Compile Before Continuing
    │   └─ dotnet build B2X.slnx
    │   └─ If FAIL: Fix immediately, cannot proceed
    │
    ├─ Questions During Development:
    │   ├─ Lead Dev → "Is this approach correct?" (async in comments)
    │   ├─ Backend → Frontend → "What's the API contract?" (negotiate)
    │   ├─ → QA → "What test scenarios should I cover?" (test planning)
    │   └─ → Documentation → "How should this be documented?" (doc review)
    │
    ├─ Write Unit & Integration Tests (80%+ coverage minimum)
    │   └─ BUILD GATE #2: All tests pass locally
    │   └─ dotnet test / npm test
    │   └─ If FAIL: Fix immediately
    │
    ├─ Mark ALL AI Agent Changes in RED (see § Agent-Change Marking)
    │
    ├─ Run Local Tests with Coverage
    │   └─ dotnet test --collect:"XPlat Code Coverage"
    │   └─ Coverage >= 80% minimum
    │   └─ If < 80%: Write more tests
    │
    ├─ Code Quality Check
    │   ├─ No hardcoded secrets/config
    │   ├─ Agent changes all marked with 🤖
    │   ├─ API/Interface documented
    │   ├─ No critical warnings
    │   └─ dotnet format (fix formatting)
    │
    └─ Self-Review Checklist
        ├─ [ ] ✅ Code compiles (BUILD GATE #1)
        ├─ [ ] ✅ All tests pass (BUILD GATE #2)
        ├─ [ ] ✅ Coverage >= 80%
        ├─ [ ] ✅ No hardcoded secrets/config
        ├─ [ ] ✅ Agent changes marked in RED
        ├─ [ ] ✅ API/Interface documented
        └─ [ ] ✅ No compiler warnings

[READY FOR REVIEW]
```

**RULE: Only the original responsible developer can modify this code.**
**NO other developer can push changes to this branch (except owner)**

### Phase 3: Testing (Day 3-4)

```
Responsible Developer
    └─ Push to feature branch (auto-trigger CI)
       ↓
[GITHUB ACTIONS CI - BUILD GATE #3]
    ├─ Build: dotnet build B2X.slnx
    │  └─ If FAIL: Developer fixes immediately, CI re-runs
    ├─ Test: dotnet test (all services)
    │  └─ If FAIL: Developer fixes immediately, CI re-runs
    ├─ Coverage: Report coverage (must be >= 75%)
    │  └─ If < 75%: Developer adds tests, CI re-runs
    ├─ Lint: StyleCop for backend, ESLint for frontend
    │  └─ If FAIL: Developer fixes, CI re-runs
    └─ All checks PASS → Comments on PR ✅
       ↓
[REVIEWS START - CODE CANNOT BE MODIFIED EXCEPT BY OWNER]
    │
    ├─ Lead Developer → Code Review (< 24h)
    │   ├─ [ ] Builds without warnings (already verified by CI)
    │   ├─ [ ] Follows Wolverine pattern
    │   ├─ [ ] No security violations
    │   ├─ [ ] Performance acceptable
    │   ├─ [ ] Tests adequate
    │   ├─ [ ] Agent changes marked & explained
    │   └─ Decision: "APPROVED" or "REQUEST CHANGES"
    │       (Reviewer does NOT push code changes)
    │       If changes needed:
    │       └─ Developer reads comments → Implements changes
    │       └─ Developer pushes updated code
    │       └─ Lead Dev re-reviews
    │
    ├─ QA Engineer → Test Coverage Review (< 24h)
    │   ├─ [ ] 80%+ coverage met
    │   ├─ [ ] Compliance tests included (if P0.x)
    │   ├─ [ ] Integration tests pass
    │   ├─ [ ] Test scenarios cover edge cases
    │   └─ Decision: "APPROVED" or "REQUEST CHANGES"
    │       If issues found:
    │       └─ QA documents specific test cases needed
    │       └─ Developer adds tests and fixes
    │
    └─ Documentation Engineer → Doc Review (< 24h)
        ├─ [ ] API documented (Swagger/XML comments)
        ├─ [ ] Architecture clear (ADR if new pattern)
        ├─ [ ] Examples provided (≥2 per service)
        ├─ [ ] Changelog updated (if user-facing)
        └─ Decision: "APPROVED" or "REQUEST CHANGES"
            If docs needed:
            └─ Developer writes docs and pushes

[ALL APPROVALS RECEIVED → READY TO MERGE]
```

**CRITICAL RULE:**
- ✅ Developer makes ALL changes based on review feedback
- ❌ Reviewers do NOT push code changes
- ❌ No code merged without explicit approval from all 3 roles

### Phase 4: Deployment (Day 4-5)

```
Lead Developer
    ├─ Merge to main (squash or conventional commits)
    ├─ Tag: v<major>.<minor>.<patch>
    └─ Deploy to staging
       ↓
QA Engineer
    ├─ Smoke testing on staging
    ├─ Compliance test verification
    └─ Sign-off or Reject
       ↓
Product Owner
    ├─ Final acceptance testing
    └─ Approve deployment to production
       ↓
DevOps/Lead Developer
    ├─ Deploy to production
    ├─ Monitor health checks
    └─ Rollback plan ready
       ↓
Documentation Engineer
    └─ Update changelog + release notes
```

### Phase 5: Retrospective (End of Sprint)

```
Scrum Master → Facilitate Retrospective
    ├─ Celebrate wins (fast deployments, 0 critical issues, etc.)
    ├─ Discuss blockers (questions not answered in 24h, etc.)
    ├─ Identify automation opportunities
    └─ 3 Action Items for next sprint
       ↓
All Roles → Vote & Commit
    └─ Report: Efficiency gains (10%+), Issue resolution time, etc.
```

---

## 🔀 Pull Request Workflow

### Purpose
Pull requests are the **formal code review gate** before merging to `main`. Every PR must satisfy:
1. **Build gates** (code compiles, tests pass)
2. **Code ownership** (only owner modifies)
3. **Role-based approvals** (3 roles required)
4. **Documentation** (APIs documented)
5. **QA readiness** (testable, no blockers)

### PR Lifecycle

```
┌────────────────────────────────────────────────────────────────────────┐
│ DEVELOPER READY TO MERGE                                              │
│ (Phase 2 complete, code tested locally)                               │
└────────────────────────┬─────────────────────────────────────────────┘
                         ↓
    ┌──────────────────────────────────────────────────────────────┐
    │ CREATE PULL REQUEST                    │
    │ ✅ Title                               │
    │ ✅ Issue link (#XXX)                   │
    │ ✅ Checklist completed                 │
    │ ✅ Tests pass locally                  │
    │ ✅ Docs complete                       │
    └──────────────────┬───────────────────────────────────────────┘
                         ↓
    GitHub Automatically:
    ├─ Runs build (must PASS)
    ├─ Runs tests (must PASS)
    ├─ Checks code coverage (must be >= target)
    └─ Requests reviewers
                         ↓
    ┌──────────────────────────────────────────────────────────────┐
    │ ROLE-BASED CODE REVIEW (3 roles must approve)               │
    └──────────────────┬───────────────────────────────────────────┘
                         ↓
    1️⃣  LEAD DEVELOPER REVIEW (required)
       • Code quality
       • Follows patterns
       • No technical debt
       • Build gates passing
       └─ Approve/Request Changes
                         ↓
    2️⃣  ARCHITECTURE/TECH LEAD REVIEW (if new pattern)
       • Design decisions
       • Scalability
       • Security implications
       └─ Approve/Request Changes
                         ↓
    3️⃣  CODE OWNER REVIEW (if different developer)
       • Business logic correct
       • Matches specification
       • Tests adequate
       └─ Approve/Request Changes
                         ↓
    All 3 Approved? → Ready to Merge
    Any Request Changes? → Developer refines → Re-review
                         ↓
    ┌──────────────────────────────────────────────────────────────┐
    │ QA TESTING (Phase 4)                                         │
    │ Deploy to staging for validation                             │
    └──────────────────┬───────────────────────────────────────────┘
                         ↓
    QA Tests acceptance criteria
    ├─ All tests pass? → QA Approves
    └─ Bugs found? → Create bug issue, return to dev
                         ↓
    ┌──────────────────────────────────────────────────────────────┐
    │ MERGE TO MAIN                                                │
    │ • Squash commit (see below)                                 │
    │ • Auto-link issue (Closes #XXX)                             │
    │ • Auto-delete branch                                        │
    │ • Trigger deployment pipeline                               │
    └──────────────────┬───────────────────────────────────────────┘
                         ↓
    ✅ Phase 5: Production deployed
```

### PR Checklist (Before Opening)

**MANDATORY**: Developer must verify ALL before opening PR:

```
✅ CODE QUALITY
   [ ] Code compiles: `dotnet build`
   [ ] All tests pass: `dotnet test` (100% of test suite)
   [ ] Code follows style guide (StyleCop passing)
   [ ] No dead code or commented-out code
   [ ] No console.log/Debug output left
   [ ] No hardcoded secrets or credentials
   [ ] No TODO comments (use issues instead)

✅ TESTING
   [ ] New tests added for feature
   [ ] Edge cases tested (null, empty, invalid)
   [ ] Code coverage >= target (80%+)
   [ ] All existing tests still pass
   [ ] Integration tests passing
   [ ] Manual testing done locally

✅ CODE OWNERSHIP
   [ ] Only I modified my code (no other devs touched)
   [ ] No concurrent changes from other developers
   [ ] Code is directly related to issue #XXX
   [ ] No unrelated changes (keep PRs focused)

✅ DOCUMENTATION
   [ ] API documentation complete (Swagger/OpenAPI)
   [ ] Code comments for complex logic
   [ ] Changelog entry added
   [ ] README updated (if needed)
   [ ] Runbook/deployment notes (if needed)

✅ SECURITY & COMPLIANCE
   [ ] No PII in logs or comments
   [ ] Encryption used for sensitive data
   [ ] Database changes safe (migrations reversible)
   [ ] Multi-tenant isolation validated
   [ ] Audit logging for data changes

✅ DATABASE
   [ ] Migrations created (if schema change)
   [ ] Migrations tested locally
   [ ] Data loss prevented (no dropped columns)
   [ ] Backward compatible (if needed)
   [ ] Rollback plan documented

✅ AGENT CHANGES (if applicable)
   [ ] All AI-generated code marked with 🤖
   [ ] Each mark includes:
      - What it does
      - Issue reference (#XXX)
      - Contact person for modifications
   [ ] Code is tested (not speculative)

✅ LINKED TO ISSUE
   [ ] Issue #XXX linked in PR description
   [ ] Closes/Relates keywords used correctly
   [ ] Related PRs mentioned (if dependencies)
   [ ] Blocks/Blocked by relationships noted

✅ READY FOR REVIEW
   [ ] PR title clear and descriptive
   [ ] PR description complete
   [ ] No draft status (ready to review)
   [ ] All conversation resolved
   [ ] No merge conflicts
```

### PR Title Format

```
[TYPE](#scope): DESCRIPTION (#ISSUE)

Examples:
feat(catalog): implement VAT calculation service (#30)
fix(identity): prevent brute force attacks (#45)
docs(price): update VAT rate documentation (#30)
test(catalog): add edge case tests for VAT validation (#31)

Types: feat, fix, docs, style, refactor, perf, test, chore
```

### PR Description Template

```markdown
### 🎯 What
[Brief 1-2 sentence description of changes]

### 📋 Why
[Why this change was needed - reference issue]

### 🔍 How
[Technical approach taken]
- Point 1
- Point 2
- Point 3

### ✅ Testing
- Unit tests: X new tests, all passing
- Integration tests: [what tested]
- Manual testing: [what tested]
- Coverage: X% (target: 80%+)

### 📚 Documentation
- ✅ Swagger/OpenAPI updated
- ✅ Changelog entry added
- ✅ Code comments for complex logic

### 🔐 Security & Compliance
- ✅ No PII exposed
- ✅ Credentials in vault
- ✅ Multi-tenant isolation verified

### 🤖 Agent Changes (if applicable)
None / [List any AI-generated sections with issue links]

### 🔗 Related Issues
Closes #30
Related #25, #26
Blocks #35
```

### Code Review Roles & Focus

#### 1️⃣ Lead Developer (ALWAYS REQUIRED)
- Code quality, patterns, technical debt
- Checks: Style, SOLID principles, error handling, performance, tests adequate

#### 2️⃣ Architecture (IF new pattern)
- Design decisions, scalability, security
- Checks: Design sound, scalable, secure, compliant

#### 3️⃣ Code Owner (IF different developer)
- Business logic, specification match
- Checks: Implements acceptance criteria, all test scenarios, error handling

### Approval Decision

- ✅ **Approve**: Code quality acceptable, meets requirements
- 📝 **Request Changes**: Issues that MUST be fixed before merge
- 💬 **Comment**: Questions/suggestions, developer can address or argue

### Handling Feedback

**If "Request Changes":**
1. Read feedback carefully
2. Fix the issue in code
3. Commit with: `git commit -m "fix: address reviewer feedback on X"`
4. Push - reviewer notified automatically
5. Re-request review

**If you disagree:**
1. Reply respectfully with your reasoning
2. Provide evidence (docs, tests, performance data)
3. Let reviewer decide
4. Escalate to Tech Lead if needed

### Merge Strategy

#### Default: Squash & Merge

```bash
# Why squash?
✅ Clean git history (1 commit per feature)
✅ Easier to revert if needed
✅ No messy merge commits
✅ Easier to bisect for bug origins
```

#### Merge Checklist

Before clicking "Merge":
```
✅ All 3 approvals received
✅ All builds passing
✅ All tests passing
✅ Code coverage acceptable
✅ QA has tested (Phase 4 complete)
✅ No merge conflicts
✅ Issue linked correctly
```

### GitHub Automation

**Automatic PR Checks** (triggered on every push):
```
✅ Build: Passes / ❌ Fails
✅ Tests: Pass / ❌ Fail
✅ Coverage: 92% (>= 80%) / ❌ 72% (< 80%)
✅ Code Style: Pass / ❌ Fail
✅ Security: No secrets found / ❌ Secrets detected
```

**If ANY check fails**: PR cannot be merged (GitHub blocks it)

**Developer responsibility**:
1. Read rejection reason
2. Fix the issue
3. Re-push
4. Automated checks re-run
5. Loop until all green ✅

### PR Rejection Policy

**PR will be blocked if**:
```
❌ Build fails
❌ Any test fails
❌ Code coverage below 80%
❌ Code style violations
❌ Hardcoded secrets detected
❌ Missing approvals (3 roles)
❌ QA has not tested
❌ Issue not linked
❌ Documentation incomplete
```

---

## 🎨 Agent-Change Marking System

### Purpose
Clearly distinguish AI-generated code from human-written code for transparency and accountability.

### How to Mark Agent Changes

#### Rule 1: Code Comments (Inline)
```csharp
// 🤖 AGENT-GENERATED: Price calculation service using IServiceProvider pattern
public class PriceCalculationService : IPriceCalculationService
{
    // 🤖 AGENT: Inject Redis cache via DI
    private readonly IDistributedCache _cache;
    
    // 🤖 AGENT: Custom business logic comment
    public async Task<decimal> CalculateVatAmount(decimal netAmount, string countryCode)
    {
        // Human: We need this for German B2B customers
        if (countryCode == "DE")
        {
            // 🤖 AGENT: Standard VAT calculation (19% for Germany)
            return netAmount * 0.19m;
        }
    }
}
```

#### Rule 2: Documentation Comments (XML Doc)
```csharp
/// <summary>
/// 🤖 AGENT-GENERATED: Validates VAT ID against VIES API
/// 
/// This service was auto-generated with human review.
/// Contact: Lead Developer for modifications
/// </summary>
public interface IVatIdValidationService
{
    Task<bool> ValidateAsync(string vatId, CancellationToken ct);
}
```

#### Rule 3: Test Methods
```csharp
public class PriceCalculationTests
{
    /// <summary>
    /// 🤖 AGENT-GENERATED: Test for standard VAT calculation
    /// </summary>
    [Fact]
    public async Task CalculateVat_GermanCustomer_Returns19Percent()
    {
        // Arrange
        var service = new PriceCalculationService(_cache);
        
        // Act
        var result = await service.CalculateVatAmount(100m, "DE");
        
        // Assert
        Assert.Equal(19m, result);
    }
}
```

#### Rule 4: Markdown Documentation
```markdown
## Price Calculation Service

### 🤖 AGENT-GENERATED SECTION

The following code was auto-generated by AI agent on 29. Dezember 2025:
- PriceCalculationService.cs (whole file)
- PriceCalculationTests.cs (test cases 1-5)

### Human-Modified Section

The following changes were made by Lead Developer:
- Added caching strategy
- Performance optimization for bulk calculations

### Review Status

- ✅ Code Review: @LeadDeveloper
- ✅ Test Coverage: 87% (target: 80%+)
- ✅ Documentation: Complete
```

### AI Agent Accountability

When creating/modifying files, agent MUST:

1. ✅ Mark all generated code with `// 🤖 AGENT-GENERATED:` comment
2. ✅ Explain WHY code was written (business logic comment)
3. ✅ Link to GitHub issue: `// Issue #30: Price calculation`
4. ✅ Include contact info: `// Contact: Lead Developer for changes`
5. ✅ Add human review stamp: `// ✅ Reviewed by: @LeadDeveloper`

### PR Check

Before approving PR with agent changes:

- [ ] All agent changes marked with 🤖
- [ ] Explanation comments present
- [ ] Issue references included
- [ ] Contact info for modifications clear
- [ ] Code review stamp added

---

## 🐛 QA Bug Loop & Issue Reporting

### Rule: "QA Finds Bugs → Returns to Developer"

When QA finds bugs during testing, they do NOT fix the code. They report the issue back to the **original responsible developer**.

### Bug Finding & Reporting Workflow

```
┌──────────────────────────────────────┐
│ QA TESTING IN STAGING                │
└────────────────┬─────────────────────┘
                 ↓
          QA Finds Bug/Issue
          (Feature doesn't work as accepted)
                 ↓
    ┌──────────────────────────────────────────────┐
    │ CREATE BUG ISSUE IN GITHUB                   │
    │                                              │
    │ Title: [BUG] Description                     │
    │ Assign To: @original-developer               │
    │ Link: "Related to #XXX"                      │
    │ Priority: Critical/High/Medium/Low           │
    │                                              │
    │ Description:                                 │
    │ - Expected: [from acceptance]                │
    │ - Actual: [what happened]                    │
    │ - Steps to reproduce:                        │
    │   1. [step 1]                                │
    │   2. [step 2]                                │
    │ - Screenshots/logs attached                  │
    │                                              │
    │ Labels:                                      │
    │ - "type: bug"                                │
    │ - "status: needs-fix"                        │
    │ - "priority: critical/high/medium"           │
    └──────────────────────────────────────────────┘
                 ↓
    QA Comments in Original Issue:
    └─ "Found bug, created #XXX for fix"
                 ↓
    Original Developer Notified
    (GitHub assigns + email)
                 ↓
┌──────────────────────────────────────────────────┐
│ DEVELOPER FIXES BUG                              │
│                                                  │
│ Process:                                         │
│ 1. Reads bug report                              │
│ 2. Reproduces locally                            │
│ 3. Writes test for bug                           │
│ 4. Fixes the code                                │
│ 5. Verifies test now passes                      │
│ 6. Pushes fix                                    │
│ 7. Creates PR (if needed)                        │
│ 8. Notifies QA in issue                          │
└────────────────┬────────────────────────────────┘
                 ↓
    QA Re-Tests in Staging
    ├─ Bug fixed? → Closes issue ✅
    └─ Bug still exists? → Updates issue with details
                          Developer re-fixes
```

### Bug Priority & Response SLA

| Priority | Definition | Developer SLA | QA Retest | Status |
|----------|-----------|---|---|---|
| **Critical** 🔴 | Feature broken, user-facing, 0 workaround | 1h response, same-day fix | Immediate | P0 |
| **High** 🟠 | Major functionality degraded | 4h response, next-day fix | Within 24h | P1 |
| **Medium** 🟡 | Minor functionality issue | 8h response, within 3 days | Within 48h | P2 |
| **Low** 🟢 | UI issue, cosmetic, edge case | 24h response | Within 1 week | P3 |

### QA Cannot:

```
❌ Modify the developer's code directly
❌ Commit fixes to the developer's branch
❌ Merge PRs
❌ Close bug issues without developer confirmation
❌ Change requirements on the fly

✅ Document issues clearly
✅ Provide steps to reproduce
✅ Suggest fixes (in comments, not code)
✅ Ask clarifying questions
✅ Verify fixes when developer provides them
```

### Bug Issue Template

```markdown
## [BUG] Feature Title - Issue Description

**Related to Issue**: #XXX (Original feature issue)  
**Assigned to**: @original-developer  
**Priority**: Critical / High / Medium / Low  
**Found During**: Staging Testing  
**Date**: YYYY-MM-DD  

### Expected Behavior
[From acceptance criteria of #XXX]

### Actual Behavior
[What really happened]

### Steps to Reproduce
1. [Step 1]
2. [Step 2]
3. [Step 3]

### Environment
- Service: [Service name]
- URL: [staging URL]
- Browser: [if frontend]
- Build Version: [git commit or version]

### Logs / Screenshots
[Attach relevant logs, screenshots, error messages]

### Root Cause (if known)
[QA guess, helps developer]

### Acceptance Criteria for Fix
- [ ] Bug reproduces before fix
- [ ] Bug does NOT reproduce after fix
- [ ] New test added to prevent regression
- [ ] All other tests still pass

---

**Status**: 🔴 Needs Fix  
**Assigned**: @developer-name  
**Fixed**: [Will update when developer provides fix]
```

### Bug Verification Checklist (QA)

Before closing bug issue:

```
✅ Original bug reproduced (before developer's fix)
✅ Followed developer's fix instructions
✅ Bug no longer occurs (after fix)
✅ All acceptance criteria from #XXX met
✅ No new bugs introduced
✅ Related tests pass
✅ Developer's response acceptable

If ALL checked → Close issue ✅
If ANY unchecked → Re-open, ask developer to refine
```

---

### Problem Statement
- Features often block on unclear requirements
- Developers wait 5+ days for architect feedback
- No formal "ask for help" mechanism

### Solution: Question & Answer Board

#### Question Template

```markdown
## [QUESTION] Can I use Strategy Pattern here?

**From**: Backend Developer (@john-dev)  
**To**: Software Architect (@architect)  
**Issue**: #30 (Price Calculation)  
**Priority**: Medium (blocks dev, not critical)  
**Created**: 29. Dezember 2025, 10:30 CET  

### Context
Implementing VAT calculation with multiple country rules.

### Question
Should I use Strategy pattern (separate class per country) 
or a single service with if/else logic?

### What I've Tried
- Reviewed existing patterns in Catalog service
- Looked at CQRS handlers for inspiration

### Acceptance Criteria for Answer
- [ ] Recommended pattern explained
- [ ] Code example provided
- [ ] Performance implications noted
- [ ] Refactoring risk assessed

---

**Status**: ⏳ Waiting (SLA: 4 business hours)  
**Answered By**: [Will be filled by responder]  
**Resolution Time**: [Will be calculated]
```

#### Response Template

```markdown
## ✅ ANSWER: Strategy Pattern Recommended

**From**: Software Architect (@architect)  
**Response Time**: 2h 15m (SLA: 4h ✅)  

### Recommendation
Use **Strategy Pattern** with factory method.

### Code Example
[Example code provided]

### Rationale
1. Extensible for new countries
2. Testable (mock each strategy)
3. Follows DDD principle (bounded contexts by country)
4. Performance: No measurable impact (< 1ms per call)

### Implementation Steps
1. Create IVatStrategy interface
2. Implement per-country strategy
3. Register in DI with factory
4. Add unit tests (5 per strategy)

### Next Steps
- [ ] Developer implements
- [ ] Lead Dev reviews
- [ ] QA tests all countries
- [ ] Closes this question

---

**Answer Marked As**: Helpful ✅  
**Developer Feedback**: "Thanks! Clear and actionable"
```

### SLA for Questions

| Priority | Responder | SLA | Escalation |
|----------|-----------|-----|-----------|
| **Critical** | Any available role | 1h | Tech Lead |
| **High** | Role expert | 4h | Lead Developer |
| **Medium** | Role expert | 8h | Lead Developer |
| **Low** | Role expert | 24h | End of sprint |

### Tracking Questions

Store questions in GitHub Discussions:
- Channel: `#development-questions`
- Tag: `@role-name` (e.g., `@architect`, `@qa-engineer`)
- Auto-reminder: If unanswered after 24h, notify on-call role

---

## 🛡️ Quality Gates & Testing Requirements

### Gate 0: Definition of Ready (Before Code Starts)

```
✅ Issue pulled from GitHub with status: "ready-for-dev"
✅ Acceptance criteria clear
✅ Dependencies documented & resolved
✅ Test scenarios defined
✅ Story points assigned
✅ Architecture (if new pattern) documented
✅ Developer assigned to self in GitHub
```

### Gate 1: Pre-Commit (Developer Machine)

```bash
# MANDATORY: Run before ANY commit
dotnet build B2X.slnx
  → If FAIL: Fix immediately, cannot commit
  
dotnet test B2X.slnx -v minimal
  → If FAIL: Fix immediately, cannot commit

# Code quality check
npm run lint (frontend)
dotnet format (backend)

# Check for agent changes marked
grep -r "// 🤖" backend/Domain/[Service]/src
  → All agent changes must be marked
```

### Gate 2: Pre-Push (Local Verification)

```bash
# Frontend coverage
cd Frontend/Store && npm run test -- --coverage

# Backend coverage
dotnet test B2X.slnx --collect:"XPlat Code Coverage"
# Must show: >= 80% for new code
```

### Gate 3: CI Pipeline (GitHub Actions)

```yaml
# .github/workflows/ci.yml
- Build: dotnet build B2X.slnx
- Test: dotnet test B2X.slnx
- Coverage Report: Show coverage per service
- Security: SonarQube analysis
- Lint: StyleCop + ESLint
- Documentation: Check API docs exist

# FAIL if:
- Coverage < 75% for changed files
- Build warnings > 0 (treat as error)
- Security issues found
- No API documentation
```

### Gate 4: Code Review & QA Coordination (Before Merge)

**Lead Developer Must Verify:**
- ✅ 80%+ test coverage
- ✅ All tests pass locally + CI
- ✅ Security: No hardcoded secrets, encryption used for PII
- ✅ Performance: No N+1 queries, caching strategy clear
- ✅ Documentation: API/Interface documented
- ✅ Agent changes marked: All code with 🤖 has explanation

**QA Coordinator (@qa-engineer) Must:**
- ✅ Plan test strategy: Unit, Integration, E2E, Security, Performance, Compliance
- ✅ Delegate specialized tests:
  - **E2E/Frontend**: Assign to @qa-frontend (user workflows, forms, accessibility)
  - **Security**: Assign to @qa-pentesting (vulnerabilities, API security, OWASP)
  - **Performance**: Assign to @qa-performance (load testing, metrics, bottlenecks)
- ✅ Own verification of:
  - ✅ Unit tests pass (backend business logic)
  - ✅ Integration tests pass (API endpoints, database)
  - ✅ Compliance tests pass (P0.6-P0.9 if applicable)
  - ✅ No regressions on existing functionality
- ✅ Coordinate specialist results:
  - ✅ @qa-frontend report: E2E workflows, accessibility
  - ✅ @qa-pentesting report: Security findings, fixes applied
  - ✅ @qa-performance report: Load test results, acceptable baseline
- ✅ Create overall quality sign-off comment in PR

**QA Specialist Responsibilities:**

| Specialist | Verifies | Reports to | Decision |
|-----------|----------|-----------|----------|
| @qa-frontend | E2E workflows, forms, accessibility, responsive | @qa-engineer | Approve/Reject |
| @qa-pentesting | OWASP Top 10, auth/authz, encryption | @qa-engineer | Approve/Reject |
| @qa-performance | Load tests, response time, bottlenecks | @qa-engineer | Approve/Reject |

**Documentation Engineer Must Verify:**
- ✅ API documented: Swagger/OpenAPI, code comments, examples
- ✅ Architecture decisions recorded
- ✅ Changelog updated (if user-facing)
- ✅ No internal jargon without explanation

### Gate 5: Staging Deployment

```bash
# Smoke tests
curl http://staging:7002/health  # All services must respond
npm run test:e2e                  # E2E tests on staging

# Compliance verification (if P0.x)
dotnet test --filter "Category=Compliance"

# Performance baseline
lighthouse http://staging:5173 --only-categories=performance
# Target: >= 85 on mobile, >= 95 on desktop
```

### Gate 6: Production Deployment

**All Previous Gates + Production Check:**
- ✅ Backup created
- ✅ Rollback plan documented
- ✅ Monitoring alerts configured
- ✅ Change log published
- ✅ Product Owner sign-off received

---

## 📖 Documentation & API Standards

### Mandatory Documentation Before Merge

#### 1. API Endpoints (Swagger/OpenAPI)

```csharp
/// <summary>
/// 🤖 AGENT-GENERATED: Get product pricing with VAT
/// </summary>
/// <param name="productId">SKU of the product</param>
/// <param name="countryCode">ISO 3166-1 alpha-2 (e.g., "DE")</param>
/// <returns>Pricing including VAT breakdown</returns>
/// <response code="200">Price calculated successfully</response>
/// <response code="404">Product not found</response>
/// <response code="400">Invalid country code</response>
[HttpGet("/api/products/{productId}/pricing")]
public async Task<GetProductPricingResponse> GetPricing(
    [FromRoute] string productId,
    [FromQuery] string countryCode,
    CancellationToken cancellationToken)
{
    // Implementation
}
```

#### 2. Service Interfaces

```csharp
/// <summary>
/// 🤖 AGENT-GENERATED: Calculates product prices with VAT
/// 
/// Implementation Pattern: Wolverine Service
/// Issue: #30 (Price Calculation)
/// 
/// Example Usage:
/// <code>
/// var priceService = serviceProvider.GetRequiredService<IPriceCalculationService>();
/// var result = await priceService.CalculatePriceAsync(productId, "DE");
/// // Returns: { NetAmount: 100m, VatAmount: 19m, GrossAmount: 119m }
/// </code>
/// </summary>
public interface IPriceCalculationService
{
    /// <summary>
    /// Calculate price including VAT for product in specific country
    /// </summary>
    /// <param name="productId">Unique product identifier (SKU)</param>
    /// <param name="countryCode">ISO 3166-1 alpha-2 country code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Price breakdown (net, VAT, gross)</returns>
    /// <exception cref="InvalidOperationException">Product not found</exception>
    /// <exception cref="ArgumentException">Invalid country code</exception>
    Task<PriceCalculationResult> CalculatePriceAsync(
        string productId,
        string countryCode,
        CancellationToken cancellationToken);
}
```

#### 3. Architecture Decision Record (ADR)

```markdown
## ADR: Why Strategy Pattern for VAT Calculation

**Status**: Accepted  
**Date**: 29. Dezember 2025  
**Author**: Software Architect  
**Issue**: #30

### Problem
VAT rates vary by country (Germany 19%, UK 20%, EU average 17%).
Need extensible, testable approach.

### Options Considered
1. ❌ If/else chains → Not extensible, hard to test
2. ✅ Strategy Pattern → Extensible, DDD-aligned
3. ❌ Database lookup → Too slow, maintenance overhead

### Decision
Implement Strategy Pattern with factory method.

### Consequences
- ✅ New countries addable without code changes
- ✅ Each country independently testable
- ✅ Follows DDD principle (country = bounded context)
- ❌ Slight performance overhead (negligible: < 1ms)

### Reference
- Code: backend/Domain/Catalog/src/Services/VatStrategy/
- Tests: backend/Domain/Catalog/tests/VatStrategyTests.cs
```

#### 4. Changelog Entry

```markdown
## Version 1.2.0 (Released 29. Dezember 2025)

### New Features
- ✨ **Price Calculation with VAT** (#30)
  - GET /api/products/{id}/pricing
  - Supports all EU countries
  - Caching with Redis
  - Contact: John (Backend Lead)

- ✨ **VAT ID Validation** (#31)
  - POST /api/validate/vat-id
  - VIES API integration
  - Rate-limiting (100 req/min)
  - Contact: Jane (Backend Dev)

### Breaking Changes
- 🔴 Removed legacy `GetPriceAsync()` endpoint
  - Use `/api/products/{id}/pricing` instead
  - Deprecated on: 15. Dezember 2025
  - Removed on: 29. Dezember 2025

### Bug Fixes
- 🐛 Fixed decimal separator for German locale (#29)
  - Was: 19.99 (wrong format)
  - Now: 19,99 (correct German format)
```

### Documentation Checklist (Before PR)

- [ ] **APIs**: All endpoints documented in Swagger
- [ ] **Interfaces**: All public methods have XML comments
- [ ] **Examples**: At least 2 code examples per service
- [ ] **Architecture**: ADR written for design decisions
- [ ] **Changelog**: Entry written for user-facing changes
- [ ] **Runbook**: If operational (e.g., cache invalidation)

---

## 🔍 Retrospective & Continuous Improvement

### Sprint Retrospective Format

**Timing**: Last 2h of sprint  
**Facilitator**: Scrum Master  
**Attendees**: All 7 roles  
**Output**: 3 action items for next sprint

### Structure (90 minutes)

#### Part 1: Celebration (15 min)

```
What Went GREAT This Sprint?

Product Owner: "Delivered 5 features, 0 critical bugs"
Lead Developer: "All PRs merged same-day, avg review: 2h"
QA Engineer: "Compliance tests: 52/52 passed"
Documentation: "API docs 100% complete"
Backend Dev: "Service latency: 45ms (target: <50ms)"
Frontend Dev: "Lighthouse score: 92"

Wins to Celebrate:
✅ Zero post-deploy hotfixes
✅ 100% test coverage on P0.6 features
✅ Question response SLA: 100% met
✅ Accessibility: WCAG AA on all pages
```

#### Part 2: Obstacles & Blockers (20 min)

```
What Blocked or Slowed Us?

Backend Dev: "Waited 6h for architect feedback on API design"
  → Question unanswered quickly → SLA miss
  → Action: Assign backup architect

Frontend Dev: "API contract changed 3 times mid-sprint"
  → No upfront API design review
  → Action: Mandatory design review before dev starts

QA: "E2E tests flaky on macOS CI"
  → Platform-specific issues
  → Action: Add macOS runner to CI pipeline

Lead Dev: "Onboarding new dev → 3 days to first commit"
  → Documentation unclear
  → Action: Create "First Commit" guide

Blockers Log:
❌ Architect SLA miss: 2 instances (avg 8h, target 4h)
❌ API contract changes: 3 instances mid-sprint
❌ E2E test flakiness: 5% of runs
❌ Onboarding time: 3 days → target 1 day
```

#### Part 3: Metrics Review (15 min)

```
Key Performance Indicators

VELOCITY: 34 story points (target: 30-40)  ✅
  ├─ Consistent delivery
  └─ No major scope creep

CODE QUALITY:
  ├─ Test coverage: 84% (target: 80%+)  ✅
  ├─ Build success rate: 98% (target: >95%)  ✅
  ├─ Critical bugs: 0 (target: <1)  ✅
  └─ Regressions: 1 (target: 0)  ⚠️

TEAM EFFICIENCY:
  ├─ PR review time: 2.1h (target: <3h)  ✅
  ├─ Question response SLA: 95% (target: 100%)  ⚠️
  ├─ Deployment success: 100% (target: >98%)  ✅
  └─ Production incidents: 0 (target: 0)  ✅

DOCUMENTATION:
  ├─ API documentation: 100% (target: 100%)  ✅
  ├─ Architecture decisions: 5 ADRs (target: 3+)  ✅
  └─ Missing docs: 0 PRs (target: 0)  ✅

AUTOMATION WINS:
  ├─ Time saved: 8h/sprint (CI automation)  ✅
  ├─ Reduced manual testing: 20%  ✅
  └─ New test scenarios automated: 15  ✅
```

#### Part 4: Action Items (20 min)

```
3 Concrete Action Items for Next Sprint

PRIORITY 1: Architect Response SLA
├─ Problem: 95% SLA (target: 100%)
├─ Root cause: Only 1 architect, high demand
├─ Action: Train 2nd architect, document patterns
├─ Metric: Measure SLA weekly
├─ Owner: Lead Developer
├─ Deadline: End of sprint 2
└─ Success: SLA = 100%

PRIORITY 2: API Contract Changes
├─ Problem: 3 mid-sprint changes (expensive)
├─ Root cause: No upfront design review
├─ Action: Mandatory API design review BEFORE dev starts
├─ Metric: Count breaking changes (target: 0)
├─ Owner: Product Owner + Architect
├─ Deadline: Sprint planning
└─ Success: 0 breaking changes mid-sprint

PRIORITY 3: Onboarding Acceleration
├─ Problem: 3 days to first commit (target: 1 day)
├─ Root cause: Documentation unclear, setup complex
├─ Action: Create "First Commit" guide + automate setup
├─ Metric: Measure onboarding time
├─ Owner: Documentation + Lead Developer
├─ Deadline: Before next hire
└─ Success: < 1 day to first commit
```

#### Part 5: Feedback to Scrum Master (20 min)

```
Recommendations for Process Improvement

FEEDBACK 1: Async Question Protocol Working ✅
├─ Before: Decisions blocked, waiting 5+ days
├─ After: Question-answer board, 4h SLA
├─ Benefit: 8h saved/sprint, unblocked 10 decisions
├─ Recommendation: Keep, expand to other teams

FEEDBACK 2: Agent-Change Marking Effective ✅
├─ Before: Unclear which code was AI-generated
├─ After: 100% of changes marked with 🤖
├─ Benefit: Transparency, easier code review
├─ Recommendation: Mandatory for all agent work

FEEDBACK 3: Compliance Testing Gate Too Strict ⚠️
├─ Before: Not verified
├─ After: 52 compliance tests required before deploy
├─ Current: Takes 4h to run all tests
├─ Recommendation: Parallelize tests, run in 1h
├─ Action: DevOps to optimize test pipeline

FEEDBACK 4: Documentation Before Merge Is Working ✅
├─ Before: Docs added later, often incomplete
├─ After: 100% of PRs have docs before review
├─ Benefit: No "documentation debt", easier onboarding
├─ Recommendation: Continue as mandatory

FEEDBACK 5: Need Better Runbook Automation ❌
├─ Problem: Manual deployments, 30 min per environment
├─ Recommendation: GitOps + auto-deployment
├─ Owner: DevOps
├─ Benefit: < 5 min deployments, zero-touch
├─ Deadline: Sprint 3

OVERALL ASSESSMENT:
├─ Team collaboration: 👍 Excellent (question protocol works)
├─ Code quality: 👍 Excellent (84% coverage, 0 critical bugs)
├─ Deployment safety: 👍 Excellent (100% success rate)
├─ Automation: 👎 Needs improvement (manual steps remain)
└─ 10% Efficiency Gain Achieved ✅
    → Saved 8h in automation
    → Reduced decision time from 5d to 4h
    → Decreased onboarding from 3d to waiting (next hire test)
```

### Feedback Template for Scrum Master

```markdown
## Retrospective Report - Sprint [#]

**Date**: 29. Dezember 2025  
**Duration**: 90 minutes  
**Attendees**: 7 roles (100% attendance)

### Wins 🎉
- [ ] Zero critical issues
- [ ] 100% compliance tests passing
- [ ] API documentation complete
- [ ] Question SLA met
- [ ] **Efficiency gain: X% (vs previous sprint)**

### Pain Points 😣
- [ ] Issue 1: Description + Root cause + Action + Owner
- [ ] Issue 2: ...

### Action Items 📝
1. **Priority 1**: [Action] → Owner: [Role] → Deadline: [Date] → Metric: [KPI]
2. **Priority 2**: [Action] → Owner: [Role] → Deadline: [Date] → Metric: [KPI]
3. **Priority 3**: [Action] → Owner: [Role] → Deadline: [Date] → Metric: [KPI]

### Key Metrics 📊
| Metric | This Sprint | Target | Status |
|--------|------------|--------|--------|
| Velocity | X points | 30-40 | ✅ |
| Test Coverage | X% | 80%+ | ✅ |
| Regressions | X | 0 | ⚠️ |
| Efficiency Gain | X% | 10%+ | ✅ |

### Scrum Master Notes 📋
[Observations and suggestions for team improvement]
```

---

## 🚨 Critical Issues Escalation

### Definition of Critical Issue

```
CRITICAL if ANY of these are true:

🔴 SECURITY BREACH
   - Data exposed, credentials leaked
   - Unauthorized access achieved
   - Encryption key compromised
   
   Action: STOP ALL WORK
           Call incident commander
           Activate incident response plan
           Notify security team & legal
           ETA: < 1 hour response

🔴 DATA LOSS / CORRUPTION
   - Production data deleted
   - Inconsistent database state
   - Unable to recover transactions
   
   Action: STOP DEPLOYMENT
           Activate rollback procedure
           Restore from backup
           Notify Product Owner
           Root cause analysis required

🔴 SYSTEM DOWN (> 30 min)
   - Service unavailable
   - Database unreachable
   - All users affected
   
   Action: Page on-call engineer
           Activate incident response
           <15 min RTO target
           Post-mortem after recovery

🔴 COMPLIANCE VIOLATION
   - GDPR, VAT, AML violation
   - Regulatory fine risk
   - Legal deadline missed
   
   Action: Notify legal team immediately
           Escalate to Product Owner
           Halt related feature work
           Audit required

🟡 HIGH SEVERITY (but not critical)
   - 50%+ users affected
   - Data correctness issue
   - Performance degradation (>5s)
   
   Action: Escalate to Lead Developer
           Create incident ticket
           Allocate 1 developer
           Continue other work

🟢 NORMAL ISSUE (continue work)
   - < 10% users affected
   - Feature not broken
   - Performance acceptable
   
   Action: Log as bug
           Prioritize in backlog
           Continue feature work
```

### Critical Issue Response Protocol

```
┌─────────────────────────────────────────┐
│ CRITICAL ISSUE DETECTED                 │
└──────────────┬──────────────────────────┘
               ↓
      [STOP CURRENT WORK]
               ↓
    Notify On-Call Engineer
    └─ Lead Developer if technical
    └─ Product Owner if business
    └─ DevOps if infrastructure
               ↓
      [INCIDENT COMMANDER ASSIGNED]
               ↓
    1. ASSESS (5 min)
       - Impact scope
       - Root cause hypothesis
       - Rollback plan ready?
               ↓
    2. CONTAIN (15 min)
       - Rollback if needed
       - Kill problematic service
       - Switch to failover
               ↓
    3. DIAGNOSE (30 min)
       - Root cause analysis
       - Log review
       - Affected data scan
               ↓
    4. RESOLVE (< 60 min total)
       - Fix deployed
       - Tests verify fix
       - Monitoring confirms recovery
               ↓
    5. POST-MORTEM (24h)
       - Document incident
       - Root cause report
       - 3 preventive actions
               ↓
    ✅ INCIDENT CLOSED
```

### On-Call Rotation

```
Week 1:  Backend Lead + DevOps
Week 2:  Frontend Lead + QA
Week 3:  Architect + Documentation
Week 4:  (Rotate)

SLA for On-Call Response:
├─ Page received: 2 min
├─ Response time: 5 min (ack incident)
├─ Incident commander: 10 min
└─ Resolution: < 60 min (target)
```

---

## 🛠️ Tools & Templates

### Required Tools

| Tool | Purpose | Team |
|------|---------|------|
| **GitHub Issues** | Task tracking, questions, blockers | All |
| **GitHub Discussions** | Q&A board, async conversations | All |
| **PR Comments** | Code review, feedback, agent changes | Dev + Lead |
| **Slack Threads** | Urgent escalations, 24/7 alerts | All |
| **Spreadsheet (Metrics)** | Sprint metrics, efficiency tracking | Scrum Master |
| **Wiki/Docs** | Living documentation, runbooks, guides | Documentation |

### Question Template (GitHub Discussions)

```markdown
## [QUESTION] [Brief Title]

**From**: @role-name  
**To**: @target-role-name  
**Issue**: #XXX  
**Priority**: Critical / High / Medium / Low  
**Created**: YYYY-MM-DD HH:MM CET  

### Context
[Explain the situation, what you're working on]

### Question
[Clear, specific question]

### What I've Tried
- [Attempt 1]
- [Attempt 2]
- [What didn't work]

### Acceptance Criteria for Answer
- [ ] Criterion 1
- [ ] Criterion 2
- [ ] Criterion 3

---

**Status**: ⏳ Waiting  
**SLA**: [Calculated based on priority]  
**Assigned To**: [Will fill]  
**Response Time**: [Will calculate]
```

### Code Review Template

```markdown
## Code Review: [PR Title] (#XXX)

### Quality Checks ✅
- [ ] Builds without warnings: `dotnet build`
- [ ] Tests pass locally: `dotnet test`
- [ ] Test coverage >= 80%
- [ ] No hardcoded secrets/config
- [ ] Follows Wolverine pattern (if backend)
- [ ] WCAG AA compliant (if frontend)

### Agent-Generated Code ✅
- [ ] All changes marked with `// 🤖 AGENT-GENERATED:`
- [ ] Explanation comments present
- [ ] Issue reference: #XXX
- [ ] Contact info for modifications clear
- [ ] Human review stamp: `// ✅ Reviewed by: @name`

### Documentation ✅
- [ ] API documented (Swagger/XML comments)
- [ ] Architecture decision recorded (if new pattern)
- [ ] Examples provided (at least 2)
- [ ] Changelog updated (if user-facing)

### Security ✅
- [ ] No SQL injection risks
- [ ] PII encrypted (Email, Phone, Name, Address, DOB)
- [ ] No credentials hardcoded
- [ ] Multi-tenant isolation (TenantId filter)
- [ ] Input validation present

### Performance ✅
- [ ] No N+1 database queries
- [ ] Caching strategy clear
- [ ] Acceptable latency (< 500ms for APIs)
- [ ] Memory leak risks assessed

### Decision
- [ ] **APPROVED** - Ready to merge
- [ ] **REQUEST CHANGES** - Issues above must be fixed
- [ ] **COMMENT** - Suggestions, no blocking

### Reviewer
- Name: @reviewer-name
- Role: Lead Developer / QA / Architect / etc.
- Date: YYYY-MM-DD HH:MM
- Time Spent: X minutes
```

### Testing Checklist Template

```markdown
## Testing Checklist: [Feature Name] (#XXX)

### Unit Tests ✅
- [ ] All happy paths tested
- [ ] All error cases tested
- [ ] Edge cases covered (null, empty, invalid)
- [ ] Coverage >= 80%
- [ ] Tests run in < 5 seconds

### Integration Tests ✅
- [ ] Database integration verified
- [ ] External API mocked correctly
- [ ] Cache behavior tested
- [ ] Transaction rollback verified

### E2E Tests (if frontend)
- [ ] User workflows tested
- [ ] All browsers tested (Chrome, Firefox, Safari, Edge)
- [ ] Mobile responsive verified
- [ ] Keyboard navigation works (accessibility)
- [ ] Screen reader tested (NVDA/VoiceOver)

### Compliance Tests (if P0.x)
- [ ] P0.6 (E-Commerce): [ ] passed
- [ ] P0.7 (AI Act): [ ] passed
- [ ] P0.8 (Accessibility): [ ] passed
- [ ] P0.9 (E-Rechnung): [ ] passed

### Manual Testing ✅
- [ ] Feature works as described
- [ ] Error messages helpful
- [ ] Performance acceptable
- [ ] No console errors

### Signed Off By
- QA Engineer: @name → Date: YYYY-MM-DD
- Lead Developer: @name → Date: YYYY-MM-DD
```

---

## 📊 Sample Sprint Report

```markdown
# Sprint 15 Report

**Sprint Duration**: Dec 20 - Dec 29, 2025  
**Team**: 7 roles, 100% capacity  
**Status**: ✅ COMPLETED

## Delivery

| Feature | Status | PR | Tests | Docs |
|---------|--------|----|---------|----|
| Price Calculation (#30) | ✅ MERGED | #245 | 42 tests (87%) | Complete |
| VAT ID Validation (#31) | ✅ MERGED | #246 | 38 tests (85%) | Complete |
| German Locale Fix (#29) | ✅ MERGED | #244 | 12 tests (90%) | N/A |
| Frontend Accessibility | ✅ MERGED | #247 | E2E + axe | Complete |
| **TOTAL** | **4/4** | - | **92 tests (87%)** | **100%** |

## Quality Metrics

```
Test Coverage: 87% (↑ from 82%)
Build Success: 98% (↑ from 96%)
Production Incidents: 0 (↓ from 2)
Regressions: 1 (↓ from 3)
Compliance Tests: 52/52 ✅

PR Review Time: 2.1h (↓ from 3.5h)
Question SLA: 95% (target: 100%)
Deployment Success: 100% (↑ from 95%)
```

## Efficiency Gains

- ✅ Automated 4 manual tests → 2h saved
- ✅ Agent-change marking → code review 20% faster
- ✅ Async question protocol → unblocked 5 decisions
- ✅ **Total: 8h saved (10% efficiency gain)**

## Action Items for Sprint 16

1. **Architect SLA**: Bring backup architect up to speed (Owner: Lead Dev)
2. **API Design Review**: Mandatory before dev starts (Owner: Product Owner)
3. **E2E Flakiness**: Fix macOS CI tests (Owner: DevOps)

## Team Feedback

> "Question protocol is a game-changer. Used to wait 5 days, now 4h. Love it!" 
> — Backend Developer

> "Agent-change marking makes reviews 20% faster. Clear what's auto-generated."
> — Lead Developer

> "Compliance testing requires 4h of test time. Can we parallelize?"
> — QA Engineer
```

---

## 🎯 Next Steps

1. **Setup**: Implement this framework at next sprint planning
2. **Training**: 1h team workshop on roles & protocols
3. **Templates**: Copy templates to GitHub Discussions
4. **Tools**: Configure GitHub Actions for quality gates
5. **Monitoring**: Track metrics weekly
6. **Iterate**: Refine after first sprint based on feedback

---

**Document Version**: 1.0  
**Last Updated**: 29. Dezember 2025  
**Maintained By**: Scrum Master  
**Review Cycle**: End of each sprint
