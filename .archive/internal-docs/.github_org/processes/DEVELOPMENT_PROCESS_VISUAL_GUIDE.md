# Development Process - Visual Summary

**Quick Visual Guide** | **One-Page Reference** | **Share in Team**

---

## 🔄 The Complete Flow (5 Phases)

```
┌─────────────────────────────────────────────────────────────────────────┐
│ PHASE 0️⃣: PULL DoR ISSUE (Mandatory First Step)                        │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│  GitHub → Filter: status:ready-for-dev                                 │
│           ↓                                                             │
│         Read: Acceptance criteria ✅                                    │
│               Dependencies ✅                                           │
│               Test scenarios ✅                                         │
│               Architecture ✅                                           │
│           ↓                                                             │
│         Assign: To yourself → status: "in-progress"                     │
│           ↓                                                             │
│         ✅ ONLY THEN START CODING                                       │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘
                                    ↓
┌─────────────────────────────────────────────────────────────────────────┐
│ PHASE 1️⃣: PLAN (Day 1)                                                  │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│  Confirm Everything:                                                    │
│    ✅ Acceptance criteria clear                                         │
│    ✅ Dependencies resolved                                             │
│    ✅ Test scenarios defined                                            │
│    ✅ Story points assigned                                             │
│    ✅ Architecture validated (if new)                                   │
│                                                                         │
│  Ready? → Create feature branch: feature/#<issue>-<name>               │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘
                                    ↓
┌─────────────────────────────────────────────────────────────────────────┐
│ PHASE 2️⃣: DEVELOP (Days 2-3)                                            │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│  ┌─────────────────────────────────────────────────────────────────┐   │
│  │ BUILD GATE #1: Before Continuing                               │   │
│  │                                                                 │   │
│  │  dotnet build B2Connect.slnx                                    │   │
│  │     ↓ FAIL → Fix immediately (cannot proceed)                  │   │
│  │     ↓ PASS → Continue                                          │   │
│  └─────────────────────────────────────────────────────────────────┘   │
│                                    ↓                                    │
│  Write code + tests (80%+ coverage)                                     │
│    ├─ Implement feature                                               │
│    ├─ Write unit tests                                                │
│    ├─ Write integration tests                                         │
│    └─ Mark agent changes: // 🤖 AGENT-GENERATED                       │
│                                    ↓                                    │
│  ┌─────────────────────────────────────────────────────────────────┐   │
│  │ BUILD GATE #2: All Tests Pass Locally                           │   │
│  │                                                                 │   │
│  │  dotnet test B2Connect.slnx                                     │   │
│  │  Coverage >= 80%                                                │   │
│  │     ↓ FAIL → Fix immediately (cannot push)                     │   │
│  │     ↓ PASS → Push to GitHub                                    │   │
│  └─────────────────────────────────────────────────────────────────┘   │
│                                                                         │
│  RULE: ❌ ONLY YOU modify this code                                    │
│        ❌ No other developer can push to your branch                   │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘
                                    ↓
┌─────────────────────────────────────────────────────────────────────────┐
│ PHASE 3️⃣: TEST & REVIEW (Days 3-4)                                      │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│  ┌─────────────────────────────────────────────────────────────────┐   │
│  │ BUILD GATE #3: GitHub Actions CI (Automatic)                   │   │
│  │                                                                 │   │
│  │  ✅ Build: dotnet build B2Connect.slnx                          │   │
│  │  ✅ Test: dotnet test B2Connect.slnx                            │   │
│  │  ✅ Coverage: >= 75%                                            │   │
│  │  ✅ Lint: StyleCop + ESLint                                     │   │
│  │     ↓ FAIL → Developer fixes immediately                       │   │
│  │     ↓ PASS → Proceed to reviews                                │   │
│  └─────────────────────────────────────────────────────────────────┘   │
│                                    ↓                                    │
│  ┌─────────────────────────────────────────────────────────────────┐   │
│  │ REVIEW #1: Lead Developer (< 24h)                              │   │
│  │                                                                 │   │
│  │  Checks: Pattern ✓ Security ✓ Performance ✓                    │   │
│  │          Tests ✓ Agent marks ✓                                 │   │
│  │                                                                 │   │
│  │  Feedback → YOU implement (reviewer doesn't push code)          │   │
│  │  Approved? → Next reviewer                                      │   │
│  └─────────────────────────────────────────────────────────────────┘   │
│                                    ↓                                    │
│  ┌─────────────────────────────────────────────────────────────────┐   │
│  │ REVIEW #2: QA Engineer (< 24h)                                 │   │
│  │                                                                 │   │
│  │  Checks: Coverage >= 80% ✓ Tests adequate ✓                    │   │
│  │          Edge cases ✓ Compliance tests ✓                       │   │
│  │                                                                 │   │
│  │  Requests → YOU write more tests                                │   │
│  │  Approved? → Next reviewer                                      │   │
│  └─────────────────────────────────────────────────────────────────┘   │
│                                    ↓                                    │
│  ┌─────────────────────────────────────────────────────────────────┐   │
│  │ REVIEW #3: Documentation Engineer (< 24h)                      │   │
│  │                                                                 │   │
│  │  Checks: API docs ✓ Examples ✓ Architecture ✓                  │   │
│  │          Changelog ✓                                            │   │
│  │                                                                 │   │
│  │  Requests → YOU write documentation                             │   │
│  │  Approved? → Ready to merge                                     │   │
│  └─────────────────────────────────────────────────────────────────┘   │
│                                                                         │
│  RULE: ❌ Reviewers do NOT push code                                   │
│        ✅ Reviewers provide feedback in comments                       │
│        ✅ YOU implement all requested changes                          │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘
                                    ↓
┌─────────────────────────────────────────────────────────────────────────┐
│ PHASE 4️⃣: DEPLOY (Days 4-5)                                             │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│  ✅ Merge to main (all 3 reviews approved)                              │
│       ↓                                                                 │
│  ✅ Deploy to staging                                                   │
│       ↓                                                                 │
│  ✅ Deploy to production                                                │
│       ↓                                                                 │
│  ✨ FEATURE LIVE                                                        │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘
```

---

## 🐛 QA Bug Loop

```
┌──────────────────────────────────────────────┐
│ QA Testing in Staging                        │
└────────────────┬─────────────────────────────┘
                 ↓
        QA Finds Bug/Issue
                 ↓
    ┌────────────────────────────────┐
    │ Create Bug Issue in GitHub:    │
    │ - Title: [BUG] Description    │
    │ - Assign to: @original-dev    │
    │ - Steps to reproduce          │
    │ - Priority: Critical/High/...  │
    └────────────────────────────────┘
                 ↓
   Original Developer Notified
         (GitHub + Email)
                 ↓
   ┌──────────────────────────────┐
   │ Developer Fixes Bug:         │
   │ 1. Reproduces locally        │
   │ 2. Writes test for bug       │
   │ 3. Fixes code                │
   │ 4. Verifies test passes      │
   │ 5. Pushes fix                │
   └──────────────────────────────┘
                 ↓
     QA Re-Tests in Staging
                 ↓
          Bug Fixed?
          ↙        ↘
        YES        NO
         ↓         ↓
    Close ✅   Developer
    Issue      Fixes Again
```

**Key Rule**: QA finds bugs → Reports → Developer fixes → QA verifies
              ❌ QA does NOT modify code

---

## 🎯 Code Ownership Rules

```
Developer Pulls Issue
    ↓
Developer = OWNER
    ↓
ONLY Owner Modifies Code
    ↓
┌──────────────────────────────────────────────┐
│ Code Review Process (No Code Pushing)        │
│                                              │
│ Reviewer: Provides feedback in comments      │
│ Owner: Reads feedback                        │
│ Owner: Implements changes                    │
│ Owner: Pushes updated code                   │
│ Reviewer: Approves or requests more changes  │
│                                              │
│ ❌ Reviewer does NOT push code               │
│ ✅ Only Owner makes code changes             │
└──────────────────────────────────────────────┘
```

---

## 📊 Metrics Tracked Weekly

```
DoR Compliance ...................... Target: 100%
Build Success Rate .................. Target: ≥98%
Code Review Cycle Time .............. Target: <24h
Code Ownership Violations ........... Target: 0%
Test Coverage ....................... Target: ≥80%
Bugs per Feature .................... Target: ≤2
QA Bug Resolution Time .............. Target: <24h
Documentation Complete .............. Target: 100%
Question Response SLA ............... Target: 100%
Efficiency Gains .................... Target: ≥10%
```

---

## ✅ Pre-Push Checklist

Before pushing your code:

```
□ Code compiles: dotnet build B2Connect.slnx ✅
□ All tests pass: dotnet test B2Connect.slnx ✅
□ Coverage >= 80% ✅
□ No hardcoded secrets ✅
□ Agent changes marked: // 🤖 ✅
□ API/Interface documented ✅
□ No compiler warnings ✅

✅ ALL CHECKED? → Safe to push to GitHub
❌ ANY NOT CHECKED? → Fix before pushing
```

---

## 🚨 If Build Fails

```
Local Build Fails?
    ↓
Fix immediately → Cannot commit
Re-run build
    ↓
Build passes? → Can commit

CI Build Fails?
    ↓
GitHub notifies you with error
Fix locally
    ↓
Re-push fix
    ↓
CI re-runs automatically
    ↓
Build passes? → Proceed to review
```

---

## 🆘 Questions During Development?

```
Question Type          → Time Limit
═══════════════════════════════════
Critical (blocks you)  → 1 hour
High (needed soon)     → 4 hours
Medium (can wait)      → 8 hours
Low (nice to know)     → 24 hours

Where? → #dev-questions channel (GitHub Discussions)
Provide: Context, what you've tried, acceptance criteria
Get: Answer with explanation + code examples
```

---

## 📈 Success = Following All Rules

```
Rule 1: Pull DoR issue
  ✅ Do this first, always

Rule 2: Build must succeed before next phase
  ✅ Gate #1 (pre-commit)
  ✅ Gate #2 (pre-push)
  ✅ Gate #3 (CI)

Rule 3: Only owner modifies code
  ✅ You implement review feedback
  ❌ Reviewer doesn't push

Rule 4: QA reports bugs, developer fixes
  ✅ QA creates issue
  ✅ Developer fixes
  ✅ QA verifies
  ❌ QA doesn't code

Rule 5: Mark agent changes with 🤖
  ✅ All AI code marked
  ✅ Explanation provided
  ✅ Contact info listed

Follow = Quality ✨ + Speed 🚀 + Clarity 📖
```

---

## 📚 Where to Find Help

```
"What's the process?" 
  → Print: DEVELOPER_QUICK_REFERENCE.md
  → Read: DEVELOPMENT_PROCESS_FRAMEWORK.md

"How do I get started?"
  → Read: IMPLEMENTATION_CHECKLIST.md

"What should we measure?"
  → Check: METRICS_AND_TRACKING.md

"I have a question"
  → Ask: #dev-questions (4-24h SLA)

"This is broken"
  → Tell: Scrum Master
  → Report: #dev-process

"I need help right now"
  → Slack: @lead-dev (urgent)
  → Or: Pair programming with Lead Dev
```

---

## 🎯 Key Takeaways

1. **Pull DoR Issue** - Start with ready-for-dev issue (Phase 0)
2. **Build Success** - Code must compile before every phase transition
3. **Code Ownership** - Only owner modifies code (no reviewer pushing)
4. **QA Bug Loop** - QA reports, developer fixes, QA verifies
5. **Transparency** - Mark agent changes with 🤖
6. **Quality First** - 80%+ test coverage, 100% documentation
7. **Async Collaboration** - Questions answered in 4-24h
8. **Metrics Matter** - Weekly tracking, continuous improvement

---

**Print This Page** | **Put in Team Slack** | **Reference Often**

**Version**: 1.0 | **Last Updated**: 29. Dezember 2025 | **Status**: Ready for Use
