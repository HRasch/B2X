# 🏗️ Architecture Review Request: Domain vs BoundedContexts

**To**: @Architect  
**Date**: 30. Dezember 2025  
**Priority**: HIGH - Structural decision affects KB files and codebase

---

## Issue

The Knowledge Base Integration Phase 1 was completed with 6 files referencing the current structure:

```
backend/BoundedContexts/     ← Current structure (as implemented)
├── Store/
├── Admin/
└── Shared/
```

**Proposed Change**:
```
backend/Domain/              ← Proposed structure (DDD terminology)
├── Store/
├── Admin/
└── Shared/
```

---

## Context

### Current State (from KB files)
- All new KB files use `backend/BoundedContexts/` references
- Documentation aligned with current project structure
- **Status**: ✅ Phase 1 complete (2,150 lines)

### Proposed State
- Rename `BoundedContexts` → `Domain` (more standard DDD naming)
- Updates needed:
  1. Directory rename (git mv)
  2. Project file paths in `.slnx`
  3. 6 KB files (WOLVERINE, DDD, ERROR, VUE3, ASPIRE, FEATURES)
  4. Getting started guides
  5. Developer guide
  6. Task definitions (tasks.json)
  7. Code namespaces (if desired)

---

## Questions for @Architect

1. **Is `backend/Domain/` the correct structure?**
   - Yes → Proceed with rename
   - No → Keep `backend/BoundedContexts/`
   - Other → What's the standard?

2. **Timing**: When should this rename happen?
   - Now (before KB goes live)
   - Later (post-Phase 1)
   - Never (current structure is fine)

3. **Namespace changes**: Should we also rename namespaces?
   - `B2Connect.Store.Catalog` → Keep as-is
   - `B2Connect.Domain.Store.Catalog` → Add Domain layer
   - Something else?

4. **Related**: Any other structural decisions pending?
   - Tests directory organization?
   - Shared libraries location?

---

## Impact Analysis

| Change | Scope | Effort |
|--------|-------|--------|
| Directory rename | 1 dir | 5 min (git mv) |
| .slnx update | 8 projects | 15 min |
| KB files update | 6 files | 30 min |
| Guides update | 4 files | 20 min |
| tasks.json | 1 file | 10 min |
| Code namespaces | Optional | 2-3 hours |

**Total**: ~1.5 hours (documentation only), ~4 hours (with namespaces)

---

## Recommendation

**Decide now** (before Phase 1 deployment to team):
- If `Domain/` is correct → Rename now, update KB, deploy clean
- If `BoundedContexts/` is correct → Deploy KB as-is, proceed with team

**Risk**: Changing structure after team starts using KB = confusion + rework

---

## Next Steps

1. @Architect reviews and decides
2. If "rename": I execute (git mv, update all refs, commit)
3. If "keep": Proceed with Phase 1 deployment
4. Create GitHub issue tracking decision

---

**Status**: ⏳ Awaiting @Architect guidance  
**Blocker**: None - can proceed with either option  
**Cost of change**: Low (1.5 hours documentation)

