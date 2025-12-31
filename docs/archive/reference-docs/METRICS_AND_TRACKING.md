# Development Process Metrics & Tracking

**Date**: 29. Dezember 2025  
**Purpose**: Track process effectiveness and identify improvement areas

---

## 📊 Core Metrics (Track Weekly)

### 1. Definition of Ready (DoR) Compliance

**Metric**: % of issues with complete DoR before development starts  
**Target**: 100%  
**Calculation**: Issues with "status:ready-for-dev" / Total issues started

```
Weekly Goal: 100% of pulled issues have DoR
If < 100%:
  ├─ Which issues pulled without DoR?
  ├─ Who pulled them?
  └─ Action: Reinforce Phase 0 (Pull DoR issue)

Tracking:
  Week 1: 85% (3 of 3.5 issues)
  Week 2: 95% (19 of 20)
  Week 3: 100% (25 of 25) ✅
```

---

### 2. Build Success Rate

**Metric**: % of pushes that pass CI build gates  
**Target**: >= 98%  
**Calculation**: Successful CI builds / Total builds attempted

```
Weekly Goal: 98%+ of CI runs succeed on first attempt
If < 98%:
  ├─ What percentage of builds fail initially?
  ├─ Most common failures? (compile, test, lint)
  ├─ Are developers testing locally before push?
  └─ Action: Remind about Gate #2 (pre-push verification)

Tracking:
  Week 1: 94% (47 of 50 builds passed first time)
         MISSED: 3 failed due to coverage < 80%
  Week 2: 96% (96 of 100)
         MISSED: 4 failed due to lint errors
  Week 3: 99% (198 of 200) ✅
         MISSED: 2 flaky tests (not code issue)
```

---

### 3. Code Review Cycle Time

**Metric**: Hours from PR opened to all approvals received  
**Target**: < 24 hours  
**Calculation**: Time from PR submission to 3rd approval (Lead Dev, QA, Docs)

```
Weekly Average: [Calculate average]
If > 24 hours:
  ├─ Which role delayed most? (Lead Dev, QA, Docs?)
  ├─ Are reviewers on vacation?
  ├─ Are changes too complex to review?
  └─ Action: Assign backup reviewer, break down PRs

Tracking:
  Week 1: Avg 8h (good)
  Week 2: Avg 12h (still good)
  Week 3: Avg 6h (excellent, team got faster)
```

---

### 4. Code Ownership Violations

**Metric**: % of PRs where code was pushed by non-owner  
**Target**: 0%  
**Calculation**: PRs with non-owner commits / Total PRs

```
Weekly Goal: ZERO violations
If > 0:
  ├─ Which developer pushed to another's branch?
  ├─ Was it emergency? (pair programming? refactoring?)
  ├─ Did reviewer accidentally push?
  └─ Action: Reinforce ownership rule, educate team

Tracking:
  Week 1: 0% ✅ (0 violations)
  Week 2: 2% (1 of 50 PRs, reviewer pushed 'quick fix')
         ACTION: Discussed with Lead Dev
  Week 3: 0% ✅ (0 violations, team learned)
```

---

### 5. QA Bug Loop - Bugs Found & Fixed

**Metric 1**: Bugs found per feature  
**Target**: <= 2 bugs per feature  
**Calculation**: Total bugs / Total features deployed

**Metric 2**: Time from bug found to fix deployed  
**Target**: <= 24 hours  
**Calculation**: Time from QA issue creation to developer pushes fix

```
Weekly Tracking:
  Feature #30 (Price Calc)
    ├─ Bugs found: 1 (good, <= 2)
    ├─ Critical? No
    ├─ Time to fix: 4h (excellent, < 24h)
    └─ Root cause: Missing edge case test

  Feature #31 (VAT Validation)
    ├─ Bugs found: 3 (bad, > 2)
    ├─ Critical? 1 (cross-tenant data leak) ✅✅✅
    ├─ Time to fix: 2h (excellent)
    └─ Root cause: Insufficient DoR testing scenarios

Analysis:
  - Feature #30: Acceptable (1 bug, fixed quickly)
  - Feature #31: Requires post-mortem (3 bugs, one critical)
  - Action: Improve DoR for complex features
```

---

### 6. Test Coverage

**Metric**: % of code covered by tests (on changed files)  
**Target**: >= 80%  
**Calculation**: Covered lines / Total lines changed

```
Weekly Tracking:
  Week 1: 78% (MISSED - below 80%, required more tests)
  Week 2: 82% ✅ (target met)
  Week 3: 85% ✅ (exceeding target)

Per service:
  Backend (Catalog): 84% ✅
  Backend (Identity): 80% ✅
  Backend (CMS): 76% ⚠️ (below target)
  Frontend (Store): 88% ✅
  
  Action: CMS team needs to add more tests
```

---

### 7. Documentation Completeness

**Metric**: % of PRs with complete API documentation before review  
**Target**: 100%  
**Calculation**: PRs with API docs / Total PRs

```
Weekly Tracking:
  Week 1: 92% (4 of 43 PRs missing docs)
          ISSUES:
            ├─ PR #240: No Swagger comments
            ├─ PR #241: No examples provided
            ├─ PR #242: No architecture decision recorded
            └─ PR #243: Missing changelog
  
  Week 2: 98% (1 of 50 missing)
  Week 3: 100% ✅ (all 35 PRs complete)
  
  Action: Gate 3 now blocks PRs without doc review approval
```

---

### 8. Agent-Change Transparency

**Metric**: % of AI-generated code marked with 🤖  
**Target**: 100%  
**Calculation**: Marked agent code / Total agent-generated code

```
Weekly Tracking:
  Week 1: 100% ✅ (all agent changes properly marked)
  Week 2: 100% ✅
  Week 3: 100% ✅
  
  Breakdown:
    ├─ Total agent changes: 127
    ├─ Marked with 🤖: 127 (100%)
    ├─ With explanation comments: 127 (100%)
    ├─ With issue reference: 127 (100%)
    └─ ✅ Perfect score
```

---

### 9. Question Response SLA Compliance

**Metric**: % of questions answered within SLA  
**Target**: 100%  
**Calculation**: Questions answered on time / Total questions asked

```
Weekly Tracking:
  Week 1: 85% (85 of 100 questions answered in time)
          MISSED (15):
            ├─ 8 to Architect (only 1 architect, overloaded)
            ├─ 5 to Lead Dev (vacation)
            └─ 2 to QA (meeting overload)
  
  Week 2: 92% (92 of 100)
  Week 3: 98% ✅ (98 of 100)
          MISSED (2):
            └─ 2 to Architect (legitimate blocker)
  
  Action Week 1: Train backup architect, assign alternates
  Success: SLA now at 98% (almost 100%)
```

---

### 10. Efficiency Gains from Automation

**Metric**: Hours saved per sprint from automation  
**Target**: >= 10% improvement each sprint  
**Calculation**: (Manual hours last sprint - automation hours this sprint) / Last sprint total

```
Tracking (in hours saved):

Week 1 (Sprint 1):
  ├─ CI automated: 4h saved
  ├─ DoR template reuse: 2h saved
  ├─ Agent-change marking: 1h saved (faster review)
  └─ Total: 7h saved (8% improvement)

Week 2 (Sprint 2):
  ├─ CI even faster (parallel tests): 5h saved
  ├─ GitHub actions auto-review hints: 1h saved
  ├─ Agent-marking became routine: 0.5h saved
  └─ Total: 6.5h saved (7% improvement)

Week 3 (Sprint 3):
  ├─ Full pipeline optimization: 8h saved
  ├─ DoR reviews faster: 2h saved
  ├─ Developer habits improved: 1h saved
  └─ Total: 11h saved (12% improvement) ✅
```

---

## 📈 Dashboard Summary (Report Weekly)

```
┌──────────────────────────────────────────────────────────┐
│ DEVELOPMENT PROCESS METRICS - WEEK 3                      │
├──────────────────────────────────────────────────────────┤
│                                                           │
│ Definition of Ready Compliance .................. 100% ✅ │
│ Build Success Rate .............................. 99% ✅  │
│ Code Review Cycle Time ........................... 6h ✅  │
│ Code Ownership Violations ........................ 0% ✅  │
│ Bugs per Feature ................................. 1.5 ✅ │
│ Test Coverage .................................... 85% ✅ │
│ Documentation Completeness ....................... 100% ✅ │
│ Agent-Change Transparency ........................ 100% ✅ │
│ Question Response SLA ............................ 98% ✅  │
│ Efficiency Gains .................................. 12% ✅  │
│                                                           │
│ OVERALL SCORE: 9.8/10 EXCELLENT                          │
│                                                           │
│ Trend: ↑ Improving (↑4% from week 1)                    │
│ Team Health: 👍 Excellent                               │
└──────────────────────────────────────────────────────────┘
```

---

## 🎯 Monthly Retrospective Metrics

### Sprint Velocity
```
Sprint 1: 32 points
Sprint 2: 35 points
Sprint 3: 38 points
Trend: ↑ Increasing (team improving)
```

### Critical Bugs in Production
```
Sprint 1: 1 critical bug (VAT calculation)
Sprint 2: 0 critical bugs
Sprint 3: 0 critical bugs
Target: 0-1 per sprint
Status: ✅ Meeting target
```

### Technical Debt
```
New tech debt added this sprint: 2 items
Tech debt resolved: 1 item
Net change: +1 (acceptable)
Target: <= +2 per sprint
Status: ✅ Within bounds
```

### Team Satisfaction
```
DoR process helpful?
  Week 1: 65% (learning curve)
  Week 2: 80%
  Week 3: 92% ✅

Build gates preventing bugs?
  Week 1: 70%
  Week 2: 85%
  Week 3: 95% ✅

Code ownership clarity?
  Week 1: 75%
  Week 2: 90%
  Week 3: 98% ✅
```

---

## 📋 Action Items Based on Metrics

### If DoR < 100%
```
Action: 
  1. List which issues violated
  2. Train violators on Phase 0
  3. Add reminder in GitHub issue template
  4. Measure again next week
```

### If Build Success < 98%
```
Action:
  1. Analyze failed builds (compile? test? lint?)
  2. Identify if same person/service recurring
  3. Pair programming session for problem area
  4. Ensure Gate #2 (pre-push test) is being done
```

### If Code Review > 24h
```
Action:
  1. Identify blocking reviewer (Lead, QA, Docs?)
  2. Is reviewer on vacation?
  3. Assign backup reviewer
  4. Consider async review (comment-based)
  5. Break down complex PRs
```

### If Code Ownership Violations > 0
```
Action:
  1. Talk with violator (emergency? mistake?)
  2. Reinforce rule in team meeting
  3. If pattern: pair with Lead Dev
  4. Emphasize branch protection rules
```

### If Bugs/Feature > 2
```
Action:
  1. Post-mortem with developer + QA
  2. What scenarios missed in DoR?
  3. What edge cases not tested?
  4. Improve DoR for similar features
  5. Add regression tests
```

### If Test Coverage < 80%
```
Action:
  1. Which service has issue?
  2. Pair developer with QA for test design
  3. Review tricky areas (boundary conditions?)
  4. Gate blocks PR until coverage meets target
```

---

## 🔍 Weekly Metrics Check

**Every Monday Morning**:

1. ✅ DoR Compliance: 100%? → If no, why?
2. ✅ Build Success: >= 98%? → If no, help failing developers
3. ✅ Review Cycle: < 24h avg? → If no, assign backups
4. ✅ Ownership: 0 violations? → If no, discuss
5. ✅ Bug Loop: <= 2/feature? → If no, improve DoR
6. ✅ Coverage: >= 80%? → If no, add tests
7. ✅ Documentation: 100%? → If no, gate blocks merge
8. ✅ Agent marks: 100%? → If no, mark before commit
9. ✅ Questions: >= 95% SLA? → If no, add backup
10. ✅ Efficiency: Track hours saved

**Report to Scrum Master**: Green/Yellow/Red status + actions

---

## 📞 Metrics Owner

**Scrum Master** maintains metrics dashboard  
**Reports**: Every Monday (5 min standup)  
**Full retrospective**: End of each sprint (2h)

**Questions about metrics?**  
→ Ask Scrum Master  
→ Check [DEVELOPMENT_PROCESS_FRAMEWORK.md](../../guides/DEVELOPMENT_PROCESS_FRAMEWORK.md)

---

**Version**: 1.0  
**Last Updated**: 29. Dezember 2025  
**Next Review**: End of Week 1 implementation
