# Sprint & Phase Archive

**Location**: `B2Connect/collaborate/archive/`  
**Purpose**: Historical sprint and phase documentation  
**Last Updated**: 30. Dezember 2025

---

## Archive Structure

### Sprints
Historical sprint documentation for reference and learning.

```
archive/sprints/
├── sprint-1-kickoff.md                    # Sprint 1 initial kickoff & planning
├── sprint-3-completion-summary.md         # Sprint 3 completion summary
└── sprint-3-phase-2-continuation-guide.md # Sprint 3 Phase 2 continuation
```

### Phases
Phase-based project milestones and deliverables.

```
archive/phases/
├── phase-3-completion-summary.md      # Phase 3 completion summary
├── phase-3-execution-checklist.md     # Phase 3 execution checklist
├── phase-4-completion-report.md       # Phase 4 completion report
├── phase-4-completion-summary.md      # Phase 4 completion summary
├── phase-4-deliverables.md            # Phase 4 deliverables
├── phase-4-documentation-index.md     # Phase 4 documentation index
└── phase-4-execution-summary.md       # Phase 4 execution summary
```

---

## How to Use This Archive

### For Reference
Use historical documents to understand:
- Previous sprint planning approaches
- Phase completion criteria
- What was delivered and when
- How project evolved

### For Learning
Reference archive when:
- Planning new sprints (see what worked before)
- Setting phase gates (reference previous completions)
- Understanding project history
- Documenting lessons learned from past phases

### For Continuation
Check archive before starting:
- New sprint (understand previous sprint patterns)
- New phase (see how previous phases transitioned)
- Similar work (find lessons from previous implementations)

---

## Current Project Structure

**Active Documentation**: `B2Connect/collaborate/`

```
collaborate/
├── sprint/{sprint-number}/          # Current/active sprint docs
├── pr/{pr-number}/                  # Current/active PR docs
├── lessons-learned/                 # Team learnings & retrospectives
├── agreements/                       # Team norms & standards
├── archive/                          # Historical docs (THIS FOLDER)
│   ├── sprints/                      # Past sprint docs
│   └── phases/                       # Past phase docs
└── request-to/                       # Agent requests
```

**Root Level**: Only active/essential docs  
**Archived**: Sprint/Phase historical documentation  
**Learnings**: Consolidated in `lessons-learned/`  
**Agreements**: Team standards in `agreements/`

---

## Archive Contents Summary

| Type | Count | Purpose |
|------|-------|---------|
| **Sprints** | 3 documents | Historical sprint planning & completion |
| **Phases** | 7 documents | Historical phase execution & deliverables |
| **Total** | 10 documents | Project history & reference |

---

## Guidelines

### ✅ What Goes to Archive
- Completed sprint documentation (after consolidation)
- Completed phase documentation (after completion)
- Historical reference materials
- Previous project milestones

### ❌ What Stays in collaborate/ Root
- Current sprint planning (active work)
- Current PR documentation (active work)
- Lessons learned (team learning)
- Team agreements (current standards)

### For New Sprints
1. Create new `sprint/{sprint-number}/` folder
2. Add planning documentation
3. After completion, consolidate to `lessons-learned/`
4. Archive final summary to `archive/sprints/`

---

## Maintenance

**Archive is Read-Only** (for reference)  
**Updates**: Only add completed sprint/phase documentation  
**Cleanup**: Consolidate learnings to `lessons-learned/` before archiving

---

**Note**: This archive maintains project history. Reference it when planning future work or understanding past decisions.

