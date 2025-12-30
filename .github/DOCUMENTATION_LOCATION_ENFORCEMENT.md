# 📁 MANDATORY: Documentation Location & Organization Rule

**Status**: 🔴 **ENFORCED** (30. Dezember 2025)  
**Authority**: @process-assistant (Exclusive - see [GOVERNANCE_RULES.md](./docs/processes/GOVERNANCE/GOVERNANCE_RULES.md))  
**For All Agents**: Backend, Frontend, QA, Security, DevOps, Product Owner, Tech Lead, Scrum Master  
**Violation Impact**: High - Causes repository clutter and navigation issues

---

## 🚨 The Problem (Observed)

As of 30. Dezember 2025, issue documentation was scattered across the **project root** instead of organized in the `collaborate/` folder:

```
B2Connect/ (ROOT - 20+ unwanted files)
├── ACCESSIBILITY_COMPLIANCE_REPORT.md
├── ISSUE_30_IMPLEMENTATION_COMPLETE.md
├── ISSUE_31_IMPLEMENTATION_COMPLETE.md
├── ISSUE_53_CONTINUATION_GUIDE.md
├── ISSUE_53_DEVELOPMENT_PLAN.md
├── ISSUE_53_PHASE_1_2_COMPLETION.md
├── ISSUE_53_PHASE_3_EXECUTION_COMPLETE.md
├── PHASE_3_COMPLETE_SUMMARY.md
├── SPRINT_1_KICKOFF.md
└── ... (15 more files)
```

**Impact**:
- ❌ Repository root cluttered with issue-specific documentation
- ❌ No clear organization by sprint/issue
- ❌ Difficult to navigate which docs are active vs. archived
- ❌ Violates governance structure defined in [RETROSPECTIVE_PROTOCOL.md](./RETROSPECTIVE_PROTOCOL.md)
- ❌ New agents don't know where to find documentation

---

## ✅ The Solution: `collaborate/` Folder Structure

All documentation MUST be organized in `B2Connect/collaborate/` following this structure:

```
B2Connect/collaborate/
├── README.md (navigation index)
│
├── sprint/1/
│   ├── README.md (sprint overview)
│   ├── execution/
│   │   ├── index.md (lists all issues for this sprint)
│   │   ├── ISSUE_30_IMPLEMENTATION_COMPLETE.md
│   │   ├── ISSUE_31_IMPLEMENTATION_COMPLETE.md
│   │   └── ISSUE_53_PHASE_3_REFACTORING_LOG.md
│   ├── planning/
│   │   ├── SPRINT_1_PLAN.md
│   │   └── SPRINT_1_KICKOFF.md
│   └── retrospective/
│       ├── SPRINT_1_RETROSPECTIVE.md
│       └── consolidated-sprint-1.md
│
├── sprint/2/
│   ├── execution/
│   ├── planning/
│   └── retrospective/
│
├── pr/30/
│   ├── design-decisions/
│   ├── implementation-notes/
│   └── review-feedback/
│
├── pr/53/
│   ├── design-decisions/
│   ├── implementation-notes/
│   └── review-feedback/
│
├── lessons-learned/
│   ├── 2025-12-30-documentation-structure.md
│   ├── 2025-12-30-build-first-rule.md
│   ├── 2025-12-30-test-driven-quality.md
│   └── consolidated-sprint-1.md
│
└── agreements/
    ├── coding-standards.md
    ├── process-agreements.md
    └── team-norms.md
```

---

## 📋 Documentation Location Reference (Use This!)

### For Issue/Feature Implementation

**When you're working on an issue** (#30, #53, etc.):

```
Location: B2Connect/collaborate/sprint/{N}/execution/
File: ISSUE_{NUM}_{DESCRIPTION}.md

Example:
  B2Connect/collaborate/sprint/1/execution/ISSUE_30_IMPLEMENTATION_COMPLETE.md
  B2Connect/collaborate/sprint/1/execution/ISSUE_53_PHASE_3_REFACTORING_LOG.md
```

**Then update the index**:
```bash
# Add to: B2Connect/collaborate/sprint/1/execution/index.md
echo "- [Issue #30: Price Transparency](./ISSUE_30_IMPLEMENTATION_COMPLETE.md)" >> index.md
```

### For Sprint Planning

**When planning a sprint**:
```
Location: B2Connect/collaborate/sprint/{N}/planning/
Files:
  - SPRINT_1_PLAN.md
  - SPRINT_1_KICKOFF.md
```

### For Sprint Retrospectives

**After sprint completes**:
```
Location: B2Connect/collaborate/sprint/{N}/retrospective/
Files:
  - SPRINT_1_RETROSPECTIVE.md
  - consolidated-sprint-1.md (aggregated learnings)
```

### For PR Documentation

**During code review**:
```
Location: B2Connect/collaborate/pr/{PR_NUM}/
Subdirectories:
  - design-decisions/      (what's the architecture decision?)
  - implementation-notes/  (how was this implemented?)
  - review-feedback/       (what feedback was given?)
```

### For Team Learnings

**When capturing lessons learned**:
```
Location: B2Connect/collaborate/lessons-learned/
File naming: {YYYY-MM-DD}-{topic}.md

Examples:
  - 2025-12-30-documentation-structure.md
  - 2025-12-30-test-driven-quality.md
  - 2025-12-30-build-first-rule.md
```

### For Team Agreements

**When establishing new standards**:
```
Location: B2Connect/collaborate/agreements/
Files (maintained by @process-assistant):
  - coding-standards.md
  - process-agreements.md
  - team-norms.md
```

---

## 🔍 Quick Checklist (Before Creating Docs)

Before you create ANY issue/sprint documentation, ask yourself:

```
□ What type of document am I creating?
  ├─ Issue execution? → collaborate/sprint/{N}/execution/
  ├─ Sprint planning? → collaborate/sprint/{N}/planning/
  ├─ Sprint retrospective? → collaborate/sprint/{N}/retrospective/
  ├─ PR documentation? → collaborate/pr/{NUM}/
  ├─ Lessons learned? → collaborate/lessons-learned/
  └─ Team agreement? → collaborate/agreements/

□ Have I used the correct file naming?
  ├─ ISSUE_30_DESCRIPTION.md (not ISSUE_30.md)
  ├─ SPRINT_1_KICKOFF.md (not KICKOFF.md)
  └─ 2025-12-30-topic.md (lessons learned format)

□ Have I created the index file?
  └─ Updated collaborate/sprint/{N}/execution/index.md
     (or relevant index for my directory)

□ Will I upload to project root?
  └─ NO! Always use collaborate/

□ Will I link from GitHub issue?
  └─ YES - add comment with link to documentation
```

---

## 📝 Examples: Correct vs. Wrong

### Example 1: Issue Implementation Doc

```
❌ WRONG:
  B2Connect/ISSUE_30_IMPLEMENTATION_COMPLETE.md
  (In project root!)

✅ CORRECT:
  B2Connect/collaborate/sprint/1/execution/ISSUE_30_IMPLEMENTATION_COMPLETE.md
  
  Then update: B2Connect/collaborate/sprint/1/execution/index.md
  Contents: "- [Issue #30: Price Transparency](./ISSUE_30_IMPLEMENTATION_COMPLETE.md)"
```

### Example 2: Sprint Retrospective

```
❌ WRONG:
  B2Connect/SPRINT_1_RETROSPECTIVE.md
  (In project root!)

✅ CORRECT:
  B2Connect/collaborate/sprint/1/retrospective/SPRINT_1_RETROSPECTIVE.md
  B2Connect/collaborate/sprint/1/retrospective/consolidated-sprint-1.md
```

### Example 3: Lessons Learned

```
❌ WRONG:
  B2Connect/LESSONS_BUILD_FIRST_RULE.md
  (In project root!)

✅ CORRECT:
  B2Connect/collaborate/lessons-learned/2025-12-30-build-first-rule.md
```

### Example 4: PR Documentation

```
❌ WRONG:
  B2Connect/PR_30_DESIGN_DECISIONS.md
  (In project root!)

✅ CORRECT:
  B2Connect/collaborate/pr/30/design-decisions/architecture-decisions.md
  B2Connect/collaborate/pr/30/implementation-notes/changes-summary.md
  B2Connect/collaborate/pr/30/review-feedback/feedback-log.md
```

---

## 🚀 How to Migrate (If You Have Root Docs)

If you see issue documentation in the project root:

```bash
# Step 1: Identify the files
ls B2Connect/ | grep -E "(ISSUE|PHASE|SPRINT)_"

# Step 2: Create the proper directory
mkdir -p B2Connect/collaborate/sprint/1/execution

# Step 3: Create index file
cat > B2Connect/collaborate/sprint/1/execution/index.md << 'EOF'
# Sprint 1 Execution Documentation

## Issues
- [Issue #30: Implementation](./ISSUE_30_IMPLEMENTATION_COMPLETE.md)
- [Issue #53: Phase 3](./ISSUE_53_PHASE_3_REFACTORING_LOG.md)
EOF

# Step 4: Move files
mv B2Connect/ISSUE_30_*.md B2Connect/collaborate/sprint/1/execution/
mv B2Connect/ISSUE_53_*.md B2Connect/collaborate/sprint/1/execution/

# Step 5: Verify GitHub issues reference new location
gh issue comment 30 --body "Documentation moved: [Sprint 1 Execution](../../../collaborate/sprint/1/execution/)"
gh issue comment 53 --body "Documentation moved: [Sprint 1 Execution](../../../collaborate/sprint/1/execution/)"
```

---

## 🔐 Enforcement (Active)

### @process-assistant Will:

✅ **Monitor for violations**:
- Daily check of git diffs in project root
- Alert if new ISSUE_*.md files created in root
- Alert if new PHASE_*.md files created in root
- Alert if new SPRINT_*.md files created in root

✅ **Enforce the rule**:
- Move violating files to proper `collaborate/` location
- Update GitHub issues with new documentation links
- Notify violating agent of the move
- No penalties, just organization

✅ **Implement git hooks** (future):
- Block commits with documentation in project root
- Automatically move files to proper location
- Provide helpful error message with correct path

### Agents Must:

✅ **Follow the structure**:
- Create all issue/sprint docs in `collaborate/`
- Never create docs in project root
- Update index files when adding new docs
- Link GitHub issues to proper documentation location

### Violations (Non-Compliant Files)

If you see issue docs in project root, it's a violation. Examples:
- `ISSUE_30_IMPLEMENTATION_COMPLETE.md` ← ROOT (WRONG)
- `ISSUE_53_PHASE_3_EXECUTION_GUIDE.md` ← ROOT (WRONG)
- `PHASE_3_COMPLETE_SUMMARY.md` ← ROOT (WRONG)
- `SPRINT_1_KICKOFF.md` ← ROOT (WRONG)

**These WILL be moved to proper location by @process-assistant.**

---

## 📖 Reference Documents

- **Scrum Master Instructions**: [scrum-master.agent.md](./agents/scrum-master.agent.md) §Sprint Documentation Management
- **Main Instructions**: [copilot-instructions.md](./copilot-instructions.md) §Documentation Location Rule
- **Governance Rules**: [docs/processes/GOVERNANCE/GOVERNANCE_RULES.md](./docs/processes/GOVERNANCE/GOVERNANCE_RULES.md)
- **Retrospective Protocol**: [RETROSPECTIVE_PROTOCOL.md](./RETROSPECTIVE_PROTOCOL.md)

---

## ❓ FAQ

**Q: Where should I put issue documentation?**  
A: `B2Connect/collaborate/sprint/{N}/execution/ISSUE_{NUM}_*.md`

**Q: What if I'm not sure which sprint?**  
A: Check GitHub issue - it will have sprint label. If not, ask @scrum-master.

**Q: Do feature docs go in `collaborate/` too?**  
A: No! Feature docs (guides, user docs, API docs) go in `docs/`. Only *issue coordination* docs go in `collaborate/`.

**Q: What about old root documentation?**  
A: Will be migrated to proper location by @process-assistant. Watch for notifications.

**Q: Can I create a new sprint folder?**  
A: Yes, follow the pattern: `collaborate/sprint/3/` (increment the number).

**Q: Who maintains the index files?**  
A: The agent working on those issues should update the index when creating new docs. @scrum-master verifies weekly.

**Q: What if I forget and create in root?**  
A: @process-assistant will move it automatically. No penalties, just stay organized next time!

---

**Effective Date**: 30. Dezember 2025  
**Authority**: @process-assistant (Exclusive modification authority)  
**Status**: 🔴 ENFORCED - All agents must comply  
**Last Updated**: 30. Dezember 2025
