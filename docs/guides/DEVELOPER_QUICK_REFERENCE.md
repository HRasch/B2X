# Development Process Quick Reference Card

**Print This** | **Keep on Desk** | **Follow Always**

---

## 🚀 The 5-Phase Development Process

### Phase 0️⃣ - PULL DoR ISSUE (Day 0)
```
GitHub → Filter: status:ready-for-dev
         Read: Acceptance criteria
         Check: Dependencies resolved
         Assign: To yourself
         Status: "in-progress"
         ✅ ONLY THEN START CODING
```

### Phase 1️⃣ - PLAN (Day 1)
```
Confirm: Acceptance criteria clear
         Dependencies documented
         Test scenarios defined
         Story points assigned
         ✅ READY TO CODE
```

### Phase 2️⃣ - DEVELOP (Days 2-3)
```
BUILD GATE #1: dotnet build B2Connect.slnx
    ↓ FAIL? FIX IMMEDIATELY
    ↓ PASS? Continue

Write code + tests (80%+ coverage)
    ↓
Mark agent changes: 🤖 AGENT-GENERATED
    ↓
BUILD GATE #2: dotnet test B2Connect.slnx
    ↓ FAIL? FIX IMMEDIATELY
    ↓ PASS? Push to GitHub

RULE: ONLY YOU modify this code
      No other developer can push to your branch
```

### Phase 3️⃣ - TEST (Days 3-4)
```
BUILD GATE #3: GitHub Actions CI
    ├─ Build ✅
    ├─ Test ✅
    ├─ Coverage >= 75% ✅
    ├─ Lint ✅
    └─ PASS? Continue to Reviews

Lead Dev Reviews (< 24h)
    └─ Requests changes? YOU implement them
    └─ Approved? Next reviewer

QA Engineer Reviews (< 24h)
    └─ Requests tests? YOU write them
    └─ Approved? Next reviewer

Documentation Engineer Reviews (< 24h)
    └─ Requests docs? YOU write them
    └─ Approved? ✅ READY TO MERGE

RULE: Reviewers provide feedback in comments
      YOU make all code changes
      Reviewers do NOT push code
```

### Phase 4️⃣ - DEPLOY (Days 4-5)
```
Merge to main
    ↓
Deploy to staging
    ↓
Deploy to production
    ↓
✅ FEATURE LIVE
```

---

## ⚙️ Build Gates - THE ABSOLUTE RULES

| Gate | When | Command | If Fail |
|------|------|---------|---------|
| **#1** | Before commit | `dotnet build B2Connect.slnx` | Fix now, cannot commit |
| **#2** | Before push | `dotnet test B2Connect.slnx` | Fix now, cannot push |
| **#3** | PR submitted | CI runs automatically | Developer fixes immediately |

```
❌ NO EXCEPTIONS
❌ NO "I'LL FIX LATER"
✅ FIX IMMEDIATELY BEFORE PROCEEDING
```

---

## 📝 Code Ownership - CRITICAL

```
YOU pulled this issue
    ↓
YOU own this code
    ↓
ONLY YOU can modify it
    ↓
No other developer touches this code
    ↓
If reviewer requests changes:
  - Reviewer writes comment
  - YOU read comment
  - YOU make change
  - Reviewer approves (doesn't push)
```

**Reviewers Cannot Do:**
- ❌ Push changes to your branch
- ❌ Merge PRs
- ❌ Modify your code

**You Must Do:**
- ✅ Address all review feedback
- ✅ Write tests reviewer requests
- ✅ Document APIs properly
- ✅ Push updated code

---

## 🐛 QA Finds a Bug - What Happens

```
QA testing → Finds bug
    ↓
QA creates issue in GitHub
    ↓ Assigns to YOU
    ↓ Links to original issue #XXX
    ↓ Provides clear steps to reproduce
    ↓
YOU get notified
    ↓
YOU reproduce locally
    ↓
YOU write test for bug
    ↓
YOU fix the code
    ↓
YOU push fix
    ↓
QA re-tests
    ↓
Bug fixed? → Closes issue ✅
Still broken? → YOU fix again
```

**Key**: QA does NOT fix your code
         QA reports, YOU fix, QA verifies

---

## 🤖 AI Agent Changes - RED MARK

Every change from an AI agent MUST have:

```csharp
// 🤖 AGENT-GENERATED: [Brief description]
// Issue #30: [What this solves]
// Contact: [Lead Developer name] for modifications

public class MyService { ... }
```

**Lead Developer Reviews**:
- ✅ All agent changes marked with 🤖
- ✅ Explanation comments present
- ✅ Issue references included
- ✅ Can approve

---

## 📋 Checklist Before Pushing PR

- [ ] Code compiles: `dotnet build B2Connect.slnx` ✅
- [ ] All tests pass: `dotnet test B2Connect.slnx` ✅
- [ ] Coverage >= 80%: `--collect:"XPlat Code Coverage"` ✅
- [ ] No hardcoded secrets ✅
- [ ] Agent changes marked: `// 🤖` ✅
- [ ] API/Interface documented ✅
- [ ] No compiler warnings ✅

**Can't push until ALL checked** ✅

---

## 🆘 Questions During Development?

```
Create GitHub Discussion (Discussions tab)
    ↓
Title: [QUESTION] Brief Title
    ↓
To: @architect or @lead-dev
    ↓
Priority: Critical/High/Medium/Low
    ↓
SLA:
  Critical → 1 hour
  High → 4 hours
  Medium → 8 hours
  Low → 24 hours
```

**Don't block waiting** → Push what you have, ask async

---

## 🚨 BUILD FAILURE - WHAT TO DO

### If local build fails:
```
1. dotnet build B2Connect.slnx
2. Read error message carefully
3. Fix the issue
4. Re-run dotnet build
5. Repeat until success
6. THEN you can commit
```

### If CI build fails:
```
1. GitHub notifies you with error link
2. Read the error in CI logs
3. Pull latest code
4. Fix locally
5. Push fixed code
6. CI runs again automatically
7. All tests pass? → Proceed to review
```

---

## 📊 Response Times (SLA)

| Request | Responder | Time | Escalate If |
|---------|-----------|------|-------------|
| Review feedback | Dev reads & acts | SAME DAY | Not done next day |
| Question (Critical) | Lead Dev | 1 hour | Not answered in 2h |
| Question (High) | Lead Dev | 4 hours | Not answered in 8h |
| Question (Medium) | Architect | 8 hours | Not answered in 16h |
| Bug fix (Critical) | You | Same day | Can't reproduce |
| Bug fix (High) | You | Next day | Needs help |

---

## ✅ Success Criteria

Your feature is done when:

```
✅ Issue pulled from "ready-for-dev" list
✅ Code compiles without errors
✅ All tests pass (coverage >= 80%)
✅ Agent changes marked with 🤖
✅ APIs documented
✅ Lead Dev approved code
✅ QA approved tests
✅ Documentation Engineer approved docs
✅ Merged to main
✅ Deployed to staging + production
✅ Working in production ✨
```

---

**Remember**: Follow the process. The process exists to prevent bugs, ensure quality, and keep the team aligned.

**Questions?** → Check [DEVELOPMENT_PROCESS_FRAMEWORK.md](./DEVELOPMENT_PROCESS_FRAMEWORK.md)

**Confused?** → Ask in GitHub Discussions (you have 4-24h SLA depending on priority)

**Build failed?** → Fix immediately, don't wait
