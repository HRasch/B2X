# ✅ Scrum Master Agent - Sprint Documentation Instructions Added

**Date**: 30. Dezember 2025  
**Status**: COMPLETE  
**Authority**: @process-assistant  
**File Modified**: `.github/agents/scrum-master.agent.md`

---

## Summary

Comprehensive **Sprint Documentation Management** section has been added to the scrum-master agent instructions at `B2Connect/collaborate/`.

---

## What Was Added

### Section 3: Sprint Documentation Management

**Purpose**: Provide clear instructions for organizing, managing, and consolidating sprint documentation to keep the project structure clean and maintainable.

**Key Areas Covered**:

#### 1. Documentation Structure
- Central location: `B2Connect/collaborate/`
- Parallel folder hierarchies:
  - `sprint/{sprint-number}/` - Sprint planning, execution, retrospective
  - `pr/{pr-number}/` - PR-specific design, implementation, feedback
  - `lessons-learned/` - Consolidated learnings and retrospectives
  - `request-to/{agent}/` - Requests to specific agents
  - `agreements/` - Team norms and standards

#### 2. Role Responsibilities

**@scrum-master**:
- Coordinates documentation in sprint folders
- Consolidates outputs immediately after PR merge
- Archives and cleans up folders
- Maintains folder structure integrity

**@product-owner** → advises **@team-assistant**:
- Updates GitHub issues based on sprint progress
- Links issues to consolidated documentation
- Closes resolved items
- Notes blockers for next sprint

**@tech-lead**:
- Documents architecture decisions in PR folders

**All agents**:
- Contribute to lessons-learned throughout sprint

#### 3. Documentation Workflow

Three phases with clear responsibilities:

**During Sprint**:
- Issues created in GitHub
- Documentation created in sprint folders
- Lessons captured daily
- Requests to agents logged
- Agreements updated as needed

**During PR Review**:
- Design decisions documented
- Implementation notes added
- Review feedback captured

**After PR Merge - Consolidation**:
- Archive sprint artifacts
- Create consolidated summary
- Archive completed PR folder
- Clean up redundant entries
- Update agreements if needed
- Product owner updates GitHub issues

#### 4. Consolidation Checklist

Detailed checklist for @scrum-master after each PR:
- [ ] Move sprint artifacts to lessons-learned
- [ ] Create consolidated summary
- [ ] Archive PR folder
- [ ] Remove duplicate entries
- [ ] Update master index
- [ ] Verify agreements reflect standards

Detailed checklist for @product-owner → @team-assistant:
- [ ] Update GitHub issues with status
- [ ] Link issues to documentation
- [ ] Close resolved issues
- [ ] Create new issues for blockers
- [ ] Note lessons in issue comments

#### 5. Documentation Types

| Type | Purpose | Created | Consolidated |
|------|---------|---------|--------------|
| Lessons Learned | Validated learnings, metrics | Throughout sprint | After PR merge |
| Requests to Agents | Tasks requested with context | During sprint | After completion |
| Agreements | Team norms, coding standards | When aligned | After retrospectives |
| Sprint Docs | Planning, execution, retro | Start-end of sprint | To lessons-learned |
| PR Docs | Design, implementation, feedback | During development | After merge |

#### 6. Key Principles

1. **Centralization**: All sprint coordination in `B2Connect/collaborate/`
2. **Parallelization**: Multiple folders enable simultaneous work
3. **Consolidation**: Regular cleanup after each PR
4. **Traceability**: GitHub issues linked to documentation
5. **Accessibility**: Clear folder structure for discovery
6. **Automation**: Product owner coordinates GitHub updates

---

## Benefits

✅ **Clear Documentation Home**: Single location for all sprint artifacts  
✅ **No Orphaned Docs**: Consolidation process archives everything  
✅ **Organized Structure**: Parallel folders for parallel work  
✅ **Discoverable History**: Lessons-learned repository  
✅ **Clean Project**: Regular consolidation keeps folders manageable  
✅ **GitHub Integration**: Product owner updates issues automatically  
✅ **Agent Clarity**: Each agent knows their responsibilities  

---

## Implementation Guide

### Step 1: Create Folder Structure (One-time)

```bash
mkdir -p B2Connect/collaborate/{sprint,pr,lessons-learned,request-to,agreements}
touch B2Connect/collaborate/README.md
```

### Step 2: Start Sprint

```bash
mkdir -p B2Connect/collaborate/sprint/{sprint-number}/{planning,execution,retrospective}
```

### Step 3: Create PR Documentation

```bash
mkdir -p B2Connect/collaborate/pr/{pr-number}/{design-decisions,implementation-notes,review-feedback}
```

### Step 4: Consolidate After PR Merge

- @scrum-master: Run consolidation checklist
- @product-owner: Advise @team-assistant to update GitHub issues
- Result: `collaborate/` stays clean and organized

### Step 5: Next Sprint

- Reference consolidated docs from previous sprint
- Repeat for new sprint

---

## Example Usage

### During Sprint 1

```
collaborate/
└── sprint/1/
    ├── planning/
    │   └── 2025-12-30-sprint-1-kickoff.md
    ├── execution/
    │   ├── 2025-12-30-day-1-notes.md
    │   ├── 2025-12-31-day-2-notes.md
    │   └── 2026-01-01-blockers.md
    └── retrospective/
        └── 2026-01-02-retro-output.md
```

### During PR #1

```
collaborate/
└── pr/1/
    ├── design-decisions/
    │   └── architecture-choice-reason.md
    ├── implementation-notes/
    │   └── implementation-summary.md
    └── review-feedback/
        └── review-comments.md
```

### After PR #1 Merge - Consolidation

```
# @scrum-master does:
contribute/
├── sprint/1/
│   ├── planning/ → stays
│   ├── execution/ → stays
│   └── retrospective/ → stays
├── pr/1/ → ARCHIVED (marked as complete)
└── lessons-learned/
    └── consolidated-1.md (from pr/1 + sprint/1)

# Result: collaborate/ folder is clean
```

---

## Integration with Other Agents

**No changes required**:
- Agents work on their assigned tasks
- They document their work
- Scrum Master coordinates and consolidates

**Coordination needed**:
- Product Owner ↔ Team Assistant (GitHub updates)
- Tech Lead → Scrum Master (architecture docs)
- All agents → Scrum Master (consolidation input)

---

## Next Actions

1. ✅ **Instructions Added**: Scrum-master now has full guidance
2. **Create Folder Structure**: Manual one-time setup (not yet done)
3. **First Sprint**: Test structure with actual sprint documentation
4. **Refine**: Adjust based on team feedback

---

## File Details

**File**: `.github/agents/scrum-master.agent.md`  
**Section**: 3. Sprint Documentation Management  
**Lines Added**: ~260 lines of comprehensive guidance  
**Location in File**: Lines 103-370 (approximately)  
**File Size**: 32K (increased from original)

**Subsections**:
1. Documentation Structure (folder hierarchy diagram)
2. Responsibilities (detailed role assignments)
3. Documentation Types (5 types with purposes)
4. Workflow (3-phase sprint-to-consolidation flow)
5. Consolidation Checklist (detailed task lists)
6. Key Principles (6 core principles)

---

## Validation

✅ Section properly formatted with clear hierarchy  
✅ Role assignments explicit and clear  
✅ Workflow diagram shows complete process  
✅ Consolidation checklist detailed and actionable  
✅ Integrates with product-owner and team-assistant  
✅ Supports @tech-lead documentation requirements  
✅ Maintains project structure cleanliness  

---

**Authority**: @process-assistant (exclusive over agent instructions)  
**Status**: READY FOR USE  
**Effective Date**: 30. Dezember 2025

