---
docid: GL-077
title: GL 014 PRE RELEASE DEVELOPMENT PHASE
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

﻿---
docid: GL-014
title: Pre-Release Development Phase Policy
owner: "@SARAH"
status: Active
created: 2025-01-05
effective-until: v1.0.0 Release
part-of: ADR-037 Lifecycle Stages Framework
---

# GL-014: Pre-Release Development Phase Policy

**DocID**: `GL-014`  
**Status**: ✅ Active (until v1.0.0 release)  
**Owner**: @SARAH  
**Approved by**: @Architect, @TechLead  
**Part of**: [ADR-037] Lifecycle Stages Framework

> 📋 This guideline defines the **Pre-Release stage** within the broader [ADR-037 Lifecycle Stages Framework](./../decisions/ADR-037-lifecycle-stages-framework.md). See ADR-037 for all 7 stages (Alpha → Pre-Release → RC → Stable → LTS → Maintenance → Deprecated).

---

## 1. Purpose

This guideline establishes that **B2X is in pre-release development** (version 0.x) and therefore:

1. **No backwards compatibility guarantees** are required
2. **Breaking changes are permitted** without deprecation cycles
3. **API instability is expected** and acceptable

Per [Semantic Versioning 2.0.0](https://semver.org/):
> "Major version zero (0.y.z) is for initial development. **Anything MAY change at any time.** The public API SHOULD NOT be considered stable."

---

## 2. Scope

This policy applies to **ALL components** until v1.0.0 release:

| Component | Pre-Release Status | Stability |
|-----------|-------------------|-----------|
| Backend APIs | 0.x | ⚠️ Unstable |
| Frontend Components | 0.x | ⚠️ Unstable |
| CLI Commands | 0.x | ⚠️ Unstable |
| Database Schema | 0.x | ⚠️ Unstable |
| Configuration Format | 0.x | ⚠️ Unstable |
| Plugin/Connector APIs | 0.x | ⚠️ Unstable |

---

## 3. What This Means in Practice

### ✅ ALLOWED During Pre-Release

| Action | Example | Approval Needed |
|--------|---------|-----------------|
| Rename APIs | `GetUser()` → `FetchUser()` | Code review only |
| Remove endpoints | Delete unused `/api/v1/legacy` | Code review only |
| Change DB schema | Rename columns, change types | Migration script |
| Restructure CLI | `create-tenant` → `tenant create` | Code review only |
| Change config format | YAML → JSON, restructure | Documentation |
| Refactor interfaces | Split/merge service contracts | @TechLead review |

### ❌ NOT Required During Pre-Release

| Requirement | Status | Re-enabled |
|-------------|--------|------------|
| Deprecation warnings | Skip | Post-v1.0 |
| Migration guides for API changes | Optional | Post-v1.0 |
| Backwards compatibility testing | Skip | Post-v1.0 |
| "Backward Compatibility" PR checklist | Skip | Post-v1.0 |
| Version compatibility matrix | Skip | Post-v1.0 |

---

## 4. Versioning During Pre-Release

### Version Format: `0.MINOR.PATCH`

```
0.1.0  - Initial development
0.2.0  - New features, may break 0.1.x
0.2.1  - Bug fixes for 0.2.0
0.3.0  - More features, may break 0.2.x
...
1.0.0  - First stable release (GL-014 expires)
```

### Version Increment Rules (Pre-Release)

| Change Type | Version Bump | Example |
|-------------|--------------|---------|
| Breaking changes | MINOR | 0.1.0 → 0.2.0 |
| New features | MINOR | 0.2.0 → 0.3.0 |
| Bug fixes | PATCH | 0.3.0 → 0.3.1 |
| Documentation | PATCH | 0.3.1 → 0.3.2 |

---

## 5. Transition to v1.0.0

### Pre-Conditions for v1.0.0 Release

Before declaring v1.0.0, the following must be complete:

- [ ] **API Freeze**: Core APIs documented and stable
- [ ] **Schema Freeze**: Database schema finalized
- [ ] **CLI Freeze**: Command structure finalized
- [ ] **Documentation**: Public API documentation complete
- [ ] **Testing**: Comprehensive test coverage
- [ ] **Security Audit**: @Security sign-off

### Post-v1.0.0 Changes

Once v1.0.0 is released, this guideline **expires** and is replaced by:

| Requirement | Policy |
|-------------|--------|
| Breaking changes | MAJOR version bump required |
| Deprecation | Minimum 1 MINOR version warning |
| Migration guides | Required for breaking changes |
| Backwards compatibility | Required for MINOR/PATCH |

---

## 6. Communication

### Internal Team
- All team members understand we're in 0.x development
- Breaking changes don't require extensive justification
- Focus on building the right solution, not preserving the wrong one

### External (if any early adopters)
- Clearly communicate 0.x status
- Set expectations: "APIs will change"
- Recommend pinning to specific versions

---

## 7. Affected Documents

The following documents reference backwards compatibility and should be interpreted in context of this guideline:

| Document | Adjustment |
|----------|------------|
| `.ai/templates/github-change-request.md` | "Backward Compatibility" field → "N/A (pre-release)" |
| `ADR-032` CLI Auto-Update | Pre-release phase noted |
| `ADR-034` Multi-ERP Connector | Pre-release phase noted |
| PR review checklists | Skip compat checks until v1.0 |

---

## 8. Review Schedule

| Milestone | Action |
|-----------|--------|
| Monthly | Review if ready for v1.0 |
| Pre-v1.0 | Create GL-015 (Post-Release Stability Policy) |
| v1.0.0 Release | Archive GL-014, activate GL-015 |

---

## 9. Quick Reference

```
┌─────────────────────────────────────────────────────────┐
│  B2X Version: 0.x (PRE-RELEASE)                   │
│                                                         │
│  ✅ Breaking changes: ALLOWED                           │
│  ✅ API refactoring: ALLOWED                            │
│  ✅ Schema changes: ALLOWED                             │
│  ❌ Backwards compatibility: NOT REQUIRED               │
│  ❌ Deprecation cycles: NOT REQUIRED                    │
│                                                         │
│  This policy expires at v1.0.0 release                  │
│  See: ADR-037 for full lifecycle framework              │
└─────────────────────────────────────────────────────────┘
```

---

## 10. Component Stages

> Component-level tracking defined in `.ai/config/lifecycle.yml`

| Component | Stage | Notes |
|-----------|-------|-------|
| Core API | 🟠 Pre-Release | This guideline applies |
| Store Frontend | 🟠 Pre-Release | This guideline applies |
| Admin Frontend | 🟠 Pre-Release | This guideline applies |
| CLI | 🔴 Alpha | Even fewer constraints |
| ERP Connectors | 🔴 Alpha | Even fewer constraints |

---

**Maintained by**: @SARAH  
**Expires**: v1.0.0 Release  
**Related**: [GL-013] Dependency Management, [ADR-032], [ADR-034], [ADR-037] Lifecycle Stages
