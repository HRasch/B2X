# 🚨 DOCUMENTATION LOCATION ENFORCEMENT - NOTICE TO ALL AGENTS

**Date**: 30. Dezember 2025  
**Status**: 🔴 **ACTIVE ENFORCEMENT**  
**Authority**: @process-assistant  
**Applies To**: All agents (Backend, Frontend, QA, Security, DevOps, Product Owner, Tech Lead, Scrum Master)

---

## ⚠️ Issue Identified

Documentation for issues (#30, #31, #53, etc.) and sprints (SPRINT_1, PHASE_3, etc.) has been created in the **project root** instead of the organized `collaborate/` folder structure.

**Current State** (WRONG):
```
B2Connect/ ← ROOT
├── ISSUE_30_IMPLEMENTATION_COMPLETE.md
├── ISSUE_31_IMPLEMENTATION_COMPLETE.md
├── ISSUE_53_CONTINUATION_GUIDE.md
├── ISSUE_53_DEVELOPMENT_PLAN.md
├── ISSUE_53_PHASE_3_EXECUTION_COMPLETE.md
├── PHASE_3_COMPLETE_SUMMARY.md
├── SPRINT_1_KICKOFF.md
└── ... (15+ more files)
```

**This is NOT acceptable.** All of these files violate the governance structure.

---

## ✅ New Rule (Effective Immediately)

All issue and sprint documentation **MUST** be placed in the `collaborate/` folder with proper organization:

```
B2Connect/collaborate/ ← CORRECT
├── sprint/1/
│   ├── execution/
│   │   ├── ISSUE_30_IMPLEMENTATION_COMPLETE.md
│   │   ├── ISSUE_31_IMPLEMENTATION_COMPLETE.md
│   │   ├── ISSUE_53_PHASE_3_EXECUTION_COMPLETE.md
│   │   └── index.md
│   ├── planning/
│   │   └── SPRINT_1_KICKOFF.md
│   └── retrospective/
│       └── PHASE_3_COMPLETE_SUMMARY.md
```

---

## 📋 What This Means For You

### **Backend Developers**
- When documenting issue implementation → Use `collaborate/sprint/{N}/execution/`
- Example: `B2Connect/collaborate/sprint/1/execution/ISSUE_30_IMPLEMENTATION_COMPLETE.md`

### **Frontend Developers**
- When documenting feature changes → Use `collaborate/sprint/{N}/execution/`
- Example: `B2Connect/collaborate/sprint/1/execution/ISSUE_31_IMPLEMENTATION_COMPLETE.md`

### **QA Engineers**
- When documenting test results → Use `collaborate/sprint/{N}/execution/`
- When creating test reports → Use `collaborate/lessons-learned/`

### **Security Engineers**
- When documenting security reviews → Use `collaborate/pr/{PR_NUM}/review-feedback/`
- When capturing security learnings → Use `collaborate/lessons-learned/`

### **DevOps Engineers**
- When documenting deployment → Use `collaborate/sprint/{N}/execution/` (if issue-related)
- When documenting infrastructure learnings → Use `collaborate/lessons-learned/`

### **Product Owner**
- When planning sprints → Use `collaborate/sprint/{N}/planning/`
- Example: `B2Connect/collaborate/sprint/1/planning/SPRINT_1_KICKOFF.md`

### **Tech Lead**
- When documenting architecture decisions → Use `collaborate/pr/{PR_NUM}/design-decisions/`
- When capturing technical learnings → Use `collaborate/lessons-learned/`

### **Scrum Master**
- ENFORCE this rule
- Move any root-level issue docs to proper location
- Update GitHub issues with new documentation links
- Create and maintain index files

---

## 🔧 Required Actions (Do This Now!)

### Step 1: Read the Reference
Read: [DOCUMENTATION_LOCATION_ENFORCEMENT.md](./.github/DOCUMENTATION_LOCATION_ENFORCEMENT.md)

### Step 2: Bookmark the Quick Card
Bookmark: [DOCUMENTATION_LOCATION_QUICK_REFERENCE.md](./DOCUMENTATION_LOCATION_QUICK_REFERENCE.md)

### Step 3: Update Your Practices
- When creating issue docs → Use `collaborate/sprint/{N}/execution/`
- Update index files when adding new documentation
- Link GitHub issues to proper documentation location
- Never create documentation in project root

### Step 4: Check Your Current Work
If you're currently working on an issue:
- Create documentation in `collaborate/` folder
- Update GitHub issue with proper link
- Follow the naming convention: `ISSUE_{NUM}_DESCRIPTION.md`

---

## 🚨 What Will Happen If You Violate This

### **@process-assistant Will**:

1. **Monitor** for new documentation in project root
   - Daily check of git diffs
   - Alert if violations found

2. **Move files** to proper location automatically
   - Files moved from root to `collaborate/`
   - Proper folder structure created if needed

3. **Update GitHub** issues to reference new location
   - Issue comment with new documentation link
   - Old links remain (won't break anything)

4. **Educate** the agent
   - Message explaining the move
   - Link to DOCUMENTATION_LOCATION_ENFORCEMENT.md
   - No penalties, just organization

5. **Implement git hooks** (future phase)
   - Automatically prevent root-level issue docs
   - Block commits with `ISSUE_*.md` in root
   - Provide helpful error message with correct path

---

## 📞 Questions?

| Question | Answer |
|----------|--------|
| **Where should I put issue documentation?** | `B2Connect/collaborate/sprint/{N}/execution/` |
| **What's the file naming?** | `ISSUE_{NUM}_DESCRIPTION.md` |
| **Do I need to create an index?** | Yes - update `collaborate/sprint/{N}/execution/index.md` |
| **What if I already created docs in root?** | @process-assistant will move them. You don't need to do anything. |
| **Can I still create feature docs?** | Yes, those go in `docs/` folder (not `collaborate/`). |
| **Who enforces this rule?** | @process-assistant (exclusive authority). @scrum-master verifies weekly. |
| **What if I make a mistake?** | No penalties. Just follow the rule next time! |

---

## 🔗 Reference Documents

- **Full Enforcement Guide**: [.github/DOCUMENTATION_LOCATION_ENFORCEMENT.md](./.github/DOCUMENTATION_LOCATION_ENFORCEMENT.md)
- **Quick Reference Card**: [DOCUMENTATION_LOCATION_QUICK_REFERENCE.md](./DOCUMENTATION_LOCATION_QUICK_REFERENCE.md)
- **Scrum Master Instructions**: [.github/agents/scrum-master.agent.md](./.github/agents/scrum-master.agent.md)
- **Main Instructions**: [.github/copilot-instructions.md](./.github/copilot-instructions.md) §Documentation Location Rule
- **Governance Rules**: [.github/docs/processes/GOVERNANCE/GOVERNANCE_RULES.md](./.github/docs/processes/GOVERNANCE/GOVERNANCE_RULES.md)

---

## ✅ Confirmation Checklist

Before starting your next task:

- [ ] Read: [DOCUMENTATION_LOCATION_ENFORCEMENT.md](./.github/DOCUMENTATION_LOCATION_ENFORCEMENT.md)
- [ ] Bookmark: [DOCUMENTATION_LOCATION_QUICK_REFERENCE.md](./DOCUMENTATION_LOCATION_QUICK_REFERENCE.md)
- [ ] Understand: Issue docs go in `collaborate/`, NOT project root
- [ ] Know: Scrum master enforces this, not you
- [ ] Remember: No penalties, just staying organized

---

## 📊 Current Violations (Will Be Fixed)

The following files are in the project root and WILL be moved to `collaborate/`:

```
❌ ISSUE_30_IMPLEMENTATION_COMPLETE.md → collaborate/sprint/1/execution/
❌ ISSUE_30_READY_FOR_REVIEW.md → collaborate/sprint/1/execution/
❌ ISSUE_31_IMPLEMENTATION_COMPLETE.md → collaborate/sprint/1/execution/
❌ ISSUE_53_CONTINUATION_GUIDE.md → collaborate/sprint/1/execution/
❌ ISSUE_53_DEVELOPMENT_PLAN.md → collaborate/sprint/1/execution/
❌ ISSUE_53_DOCUMENTATION_INDEX.md → collaborate/sprint/1/execution/
❌ ISSUE_53_EXECUTIVE_SUMMARY.md → collaborate/sprint/1/execution/
❌ ISSUE_53_PHASE_1_2_COMPLETION.md → collaborate/sprint/1/execution/
❌ ISSUE_53_PHASE_3_EXECUTION_COMPLETE.md → collaborate/sprint/1/execution/
❌ ISSUE_53_PHASE_3_EXECUTION_GUIDE.md → collaborate/sprint/1/execution/
❌ ISSUE_53_PHASE_3_QUICK_REFERENCE.md → collaborate/sprint/1/execution/
❌ ISSUE_53_REFACTORING_LOG.md → collaborate/sprint/1/execution/
❌ ISSUE_53_SESSION_COMPLETE_SUMMARY.md → collaborate/sprint/1/execution/
❌ ISSUE_53_SESSION_SUMMARY.md → collaborate/sprint/1/execution/
❌ PHASE_3_COMPLETE_SUMMARY.md → collaborate/sprint/1/retrospective/
❌ PHASE_3_HANDOFF.md → collaborate/sprint/1/retrospective/
❌ SPRINT_1_KICKOFF.md → collaborate/sprint/1/planning/
❌ SPRINT_1_PHASE_B_COMPLETE.md → collaborate/sprint/1/retrospective/
```

**Status**: Will be moved on next @process-assistant action.

---

**Authority**: @process-assistant  
**Enforcement**: Active  
**Questions**: See [DOCUMENTATION_LOCATION_ENFORCEMENT.md](./.github/DOCUMENTATION_LOCATION_ENFORCEMENT.md)  
**Last Updated**: 30. Dezember 2025
