# Issue: Standardize Backend Structure - Domain vs BoundedContexts

**Title**: Standardize Backend Directory Structure: Align with DDD Terminology  
**Type**: Architecture Decision  
**Priority**: HIGH  
**Status**: 🔴 BLOCKED - Awaiting @Architect Decision

---

## Problem

Current backend structure uses `backend/BoundedContexts/`:

```
backend/
├── BoundedContexts/        ← Inconsistent with DDD standard naming
│   ├── Store/
│   ├── Admin/
│   └── Shared/
```

DDD best practice is `backend/Domain/`:

```
backend/
├── Domain/                 ← Standard DDD naming
│   ├── Store/
│   ├── Admin/
│   └── Shared/
```

---

## Impact

**Just completed**: Knowledge Base Phase 1 (6 files, 2,150 lines)
- All KB files reference `backend/BoundedContexts/`
- Guides updated with current structure
- If structure changes → KB must be re-updated

**Decision needed**: BEFORE deploying to team

---

## Acceptance Criteria

- [ ] @Architect confirms preferred structure
- [ ] Structure renamed (if decision is "Domain/")
- [ ] All references updated (KB, guides, configs)
- [ ] .slnx project paths corrected
- [ ] tasks.json updated
- [ ] Team notified

---

## Scope of Changes (if renaming to Domain/)

1. **Directory**: `git mv backend/BoundedContexts backend/Domain`
2. **Project files**: Update `.slnx` (8 project paths)
3. **KB files**: Update 6 files (WOLVERINE, DDD, ERROR, VUE3, ASPIRE, FEATURES)
4. **Guides**: Update GETTING_STARTED.md, DEVELOPER_GUIDE.md
5. **Tasks**: Update tasks.json paths
6. **Optional**: Rename namespaces `B2Connect.Store.X` → `B2Connect.Domain.Store.X`

**Effort**: 1.5 hours (docs only) or 4 hours (with namespaces)

---

## Decision Options

**Option A: Rename to `backend/Domain/`**
- Pro: Standard DDD naming, aligns with industry best practice
- Con: Requires 1.5h of updates now
- Status: Clean structure from day 1

**Option B: Keep `backend/BoundedContexts/`**
- Pro: No changes needed, KB already references this
- Con: Non-standard naming, less recognizable as DDD
- Status: Deploy immediately with current KB

**Option C: Other Structure**
- Suggest alternative naming/organization

---

## Recommendation

**@Architect**: Please decide within 24 hours

**If "Domain/"**: Rename immediately (before KB Phase 1 team activation)  
**If "BoundedContexts/"**: Deploy KB as-is Monday 2. Januar  
**If "Other"**: Specify structure + I'll update everything

---

## Related Issues

- #54: PR Creation Process (currently using BoundedContexts)
- KB Integration Phase 1 (awaiting deployment decision)

---

**Created**: 30. Dezember 2025  
**Author**: @SARAH (Coordinator)  
**Assigned To**: @Architect

