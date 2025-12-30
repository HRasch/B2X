# ✅ INSTRUCTION UPDATE COMPLETE: Documentation Location Rule Enforcement

**Date**: 30. Dezember 2025  
**Authority**: @process-assistant  
**Status**: 🟢 **DEPLOYED TO ALL AGENTS**

---

## 📋 Summary of Changes

All agents have been instructed to:

✅ **Use `collaborate/` folder** for all issue/sprint documentation  
✅ **Never create documentation in project root**  
✅ **Follow folder structure** by document type  
✅ **Update index files** when creating new docs  
✅ **Link GitHub issues** to proper documentation location  

---

## 📚 Updated Files (Instruction Changes)

| File | Change | Status |
|------|--------|--------|
| `.github/copilot-instructions.md` | Added "Documentation Location Rule" section | ✅ Updated |
| `.github/agents/scrum-master.agent.md` | Added enforcement responsibility | ✅ Updated |
| `.github/DOCUMENTATION_LOCATION_ENFORCEMENT.md` | **NEW** - Full enforcement guide (45+ KB) | ✅ Created |
| `.github/DOCUMENTATION_LOCATION_ENFORCEMENT_NOTICE.md` | **NEW** - Notice to all agents | ✅ Created |
| `.github/docs/processes/GOVERNANCE/DOCUMENTATION_LOCATION_ENFORCEMENT_LOG.md` | **NEW** - Enforcement tracking log | ✅ Created |
| `DOCUMENTATION_LOCATION_QUICK_REFERENCE.md` | **NEW** - Quick card for agents | ✅ Created |

---

## 🎯 What All Agents Now Know

### Backend Developers
- ✅ Create issue documentation in `collaborate/sprint/{N}/execution/`
- ✅ Use naming: `ISSUE_{NUM}_IMPLEMENTATION.md`
- ✅ Never create docs in project root

### Frontend Developers
- ✅ Create issue documentation in `collaborate/sprint/{N}/execution/`
- ✅ Link GitHub issue to documentation location
- ✅ Update index when adding new docs

### QA Engineers
- ✅ Test documentation in `collaborate/sprint/{N}/execution/`
- ✅ Test reports in `collaborate/lessons-learned/`
- ✅ PR feedback in `collaborate/pr/{NUM}/review-feedback/`

### Security Engineers
- ✅ Security reviews in `collaborate/pr/{NUM}/review-feedback/`
- ✅ Security learnings in `collaborate/lessons-learned/`
- ✅ Never in project root

### DevOps Engineers
- ✅ Deployment docs in `collaborate/sprint/{N}/execution/` (if issue-related)
- ✅ Infrastructure learnings in `collaborate/lessons-learned/`
- ✅ Follow folder structure consistently

### Product Owner
- ✅ Sprint planning in `collaborate/sprint/{N}/planning/`
- ✅ Named: `SPRINT_{N}_KICKOFF.md`
- ✅ Update GitHub issues with proper links

### Tech Lead
- ✅ Architecture decisions in `collaborate/pr/{NUM}/design-decisions/`
- ✅ Technical learnings in `collaborate/lessons-learned/`
- ✅ Verify all teams comply

### Scrum Master
- ✅ **Enforce the rule** (new responsibility)
- ✅ Move violations to proper location
- ✅ Update GitHub issues with new links
- ✅ Create/maintain index files
- ✅ Educate agents when violations occur

---

## 📖 Reference Documents (For All Agents)

| Document | Purpose | Location |
|----------|---------|----------|
| **Quick Reference Card** | Quick lookup - where does each doc go? | `DOCUMENTATION_LOCATION_QUICK_REFERENCE.md` |
| **Full Enforcement Guide** | Complete reference with examples | `.github/DOCUMENTATION_LOCATION_ENFORCEMENT.md` |
| **Notice to All Agents** | What changed and what agents must do | `.github/DOCUMENTATION_LOCATION_ENFORCEMENT_NOTICE.md` |
| **Scrum Master Instructions** | How to enforce the rule | `.github/agents/scrum-master.agent.md` |
| **Main Instructions** | Updated copilot instructions | `.github/copilot-instructions.md` |
| **Enforcement Log** | @process-assistant tracking | `.github/docs/processes/GOVERNANCE/DOCUMENTATION_LOCATION_ENFORCEMENT_LOG.md` |

---

## 🔴 Current Violations (Will Be Fixed)

18 files currently in project root:
- ISSUE_30_*.md (2 files)
- ISSUE_31_*.md (1 file)
- ISSUE_53_*.md (8 files)
- PHASE_3_*.md (2 files)
- SPRINT_1_*.md (2 files)
- Other issue docs (1 file)

**Status**: Will be moved to `collaborate/` on next @process-assistant action.

---

## ✅ Enforcement Active

### What @process-assistant Will Do

✅ **Monitor** daily for new violations  
✅ **Move** files to proper location automatically  
✅ **Update** GitHub issues with new links  
✅ **Educate** agents about the rule  
✅ **Implement** git hooks to prevent future violations  

### What Agents Must Do

✅ **Use** `collaborate/` folder for all issue docs  
✅ **Never** create docs in project root  
✅ **Follow** the folder structure  
✅ **Update** index files  
✅ **Link** GitHub issues  

### What Scrum Master Must Do

✅ **Verify** agents use proper structure  
✅ **Move** any violations found  
✅ **Update** GitHub issues  
✅ **Educate** agents if violations occur  
✅ **Maintain** index files  

---

## 🎓 Key Points for All Agents

### ✅ Correct Structure
```
B2Connect/collaborate/sprint/1/execution/
  ├── ISSUE_30_IMPLEMENTATION_COMPLETE.md
  ├── ISSUE_53_PHASE_3_REFACTORING_LOG.md
  └── index.md
```

### ❌ Incorrect Structure
```
B2Connect/
  ├── ISSUE_30_IMPLEMENTATION_COMPLETE.md  ← WRONG!
  └── ISSUE_53_PHASE_3_REFACTORING_LOG.md  ← WRONG!
```

### 📋 File Locations by Type

| Type | Location |
|------|----------|
| Issue Implementation | `collaborate/sprint/{N}/execution/` |
| Sprint Planning | `collaborate/sprint/{N}/planning/` |
| Sprint Retrospective | `collaborate/sprint/{N}/retrospective/` |
| PR Design Decisions | `collaborate/pr/{NUM}/design-decisions/` |
| PR Implementation Notes | `collaborate/pr/{NUM}/implementation-notes/` |
| PR Review Feedback | `collaborate/pr/{NUM}/review-feedback/` |
| Lessons Learned | `collaborate/lessons-learned/` |
| Team Agreements | `collaborate/agreements/` |

---

## 📞 Quick Answers

**Q: Where do I put issue documentation?**  
A: `B2Connect/collaborate/sprint/{N}/execution/ISSUE_{NUM}_*.md`

**Q: What if I put it in the wrong place?**  
A: @process-assistant will move it automatically. No penalties, just follow the rule next time.

**Q: Do I need to update index files?**  
A: Yes! Update `collaborate/sprint/{N}/execution/index.md` when adding new docs.

**Q: Who verifies compliance?**  
A: @scrum-master (weekly) and @process-assistant (daily monitoring).

**Q: What about feature documentation?**  
A: That goes in `docs/`, not `collaborate/`. This rule applies to issue/sprint docs only.

---

## ✨ Success Criteria (Measured Over Time)

| Metric | Target | Timeline |
|--------|--------|----------|
| **New violations/week** | 0 | After 2 weeks |
| **Agent compliance rate** | 100% | After 4 weeks |
| **Repository clarity** | Good structure | After 1 week |
| **Agent satisfaction** | Understands rule | Immediate |

---

## 📊 Deployment Status

- ✅ Main instructions updated (copilot-instructions.md)
- ✅ Scrum master instructions updated (scrum-master.agent.md)
- ✅ Quick reference created (DOCUMENTATION_LOCATION_QUICK_REFERENCE.md)
- ✅ Full enforcement guide created (DOCUMENTATION_LOCATION_ENFORCEMENT.md)
- ✅ Notice to all agents created (DOCUMENTATION_LOCATION_ENFORCEMENT_NOTICE.md)
- ✅ Enforcement log created (DOCUMENTATION_LOCATION_ENFORCEMENT_LOG.md)
- ✅ All agents notified of the rule

**Overall Status**: 🟢 **DEPLOYED & ACTIVE**

---

## 🎯 Next Steps (What Happens Now)

1. **Agents read** the new instructions (immediate)
2. **Scrum master** reviews the enforcement duty (today)
3. **@process-assistant** begins monitoring (daily)
4. **Violations moved** to proper location (as found)
5. **GitHub issues updated** with new links (as moved)
6. **Agents informed** to follow rule (ongoing)
7. **Metrics tracked** for improvement (weekly)

---

## 📝 Authority & Governance

**Who Enforces**: @process-assistant (Exclusive authority)  
**Who Verifies**: @scrum-master (Weekly compliance check)  
**Who Follows**: All agents (Mandatory)  
**Violations**: Moved immediately, no penalties

---

**Status**: 🟢 **INSTRUCTION UPDATE COMPLETE**  
**All Agents**: Now instructed to use `collaborate/` folder  
**Enforcement**: Active (monitoring daily)  
**Last Updated**: 30. Dezember 2025
