# Dependency Migration Plan

**Date:** December 30, 2025  
**Owner:** @Backend  
**Phase 1 Scope:** Polly patch update (8.4.0 → 8.6.5)  
**Deferred:** FluentValidation major bump (Phase 2)

---

## Migration Strategy

### Update Sequence

**Order:** Shared-first, then dependents
1. B2X.Shared.Infrastructure (6 projects depend on it)
2. B2X.Customer.API (independent update)

**Timing:** Sequential to catch any cross-project issues

---

## Breaking Changes Assessment

### Polly 8.4.0 → 8.6.5 (Patch Update)

**Change Type:** Patch version (8.4.0 → 8.6.5)

**Compatibility:** ✅ Fully backwards compatible
- No breaking changes in patch releases per semantic versioning
- Bug fixes and minor features only
- API surface unchanged
- Configuration format unchanged

**Testing Plan:**
1. Update package reference
2. Run full test suite: `dotnet test`
3. Spot-check retry/circuit-breaker policies
4. Deploy to integration environment (if available)

### FluentValidation 11.11.0 → 12.1.1 (Major Update)

**Change Type:** Major version (11.x → 12.x)

**Status:** ⚠️ DEFERRED TO PHASE 2

**Why Deferred:**
- Major version bump requires careful review
- Only used in B2X.Customer.API (isolated impact)
- Patch update (Polly) is priority
- No blocking security issues identified

**Phase 2 Plan:**
1. Research FluentValidation 12.x migration guide
2. Review B2X.Customer.API validation code
3. Identify required changes in validation rules
4. Test comprehensively before update
5. Update and verify

---

## Project Update Map

### B2X.Shared.Infrastructure
**Current State:** Polly 8.4.0  
**Target State:** Polly 8.6.5  
**Risk:** 🟢 Low (patch update)  
**Status:** Ready to update in Phase 1  

**Update Process:**
```bash
cd backend/shared/B2X.Shared.Infrastructure
# Update project file or use:
dotnet add package Polly --version 8.6.5
```

**Files Affected:**
- B2X.Shared.Infrastructure.csproj

**Dependencies:** 6 projects depend on this (update first)

**Testing:**
- Unit tests for Infrastructure module
- Integration tests with dependent projects
- Retry/circuit-breaker scenario tests

### B2X.Customer.API
**Current State:** FluentValidation 11.11.0 + Polly 8.4.0  
**Target State (Phase 1):** Polly 8.6.5 only  
**Target State (Phase 2):** FluentValidation 12.1.1  
**Risk:** 🟢 Low for Polly, 🟡 Medium for FluentValidation  
**Status:** Polly ready, FluentValidation deferred  

**Update Process (Phase 1):**
```bash
cd backend/BoundedContexts/Customer/API
# Update Polly only
dotnet add package Polly --version 8.6.5
```

**Testing (Phase 1):**
- Run B2X.Customer.Tests
- Verify retry logic still works
- Verify circuit-breaker patterns function

---

## Compatibility Matrix

| Project | Polly | FluentValidation | Status |
|---------|-------|------------------|--------|
| B2X.Shared.Infrastructure | 8.4.0 → **8.6.5** | — | ✅ Phase 1 |
| B2X.Customer.API | 8.4.0 → **8.6.5** | 11.11.0 (defer) | ✅ Phase 1 |
| B2X.Shared.Core | — | — | ✅ Current |
| B2X.Identity.API | — | — | ✅ Current |
| All others | — | — | ✅ Current |

---

## Risk Mitigation

### High-Risk Points
None identified for Phase 1 (patch update only)

### Medium-Risk Points
FluentValidation 12.x bump (deferred to Phase 2)

### Testing Strategy
1. **Unit tests:** Run full test suite before/after each update
2. **Integration tests:** Verify cross-project dependencies
3. **Spot checks:** Retry/circuit-breaker policies work correctly
4. **Build verification:** `dotnet build` passes without warnings

### Rollback Plan
If issues encountered:
1. Revert package update
2. Investigate root cause
3. Document findings
4. Plan alternative approach in Phase 2

---

## Timeline

**Phase 1 (Current Sprint):**
- ✅ Update Polly in B2X.Shared.Infrastructure (2 SP estimated)
- ✅ Update Polly in B2X.Customer.API (1 SP estimated)
- ✅ Test and verify no regressions

**Phase 2 (Next Sprint):**
- ⏳ Research FluentValidation 12.x
- ⏳ Plan migration strategy
- ⏳ Execute update with comprehensive testing

---

## Success Criteria (Phase 1)

✅ Polly updated to 8.6.5 in both projects  
✅ All tests passing (`dotnet test` returns 0 failures)  
✅ No build warnings  
✅ Regression testing complete  
✅ Changes committed and documented  

---

## Supporting Documentation

- [DEPENDENCY_AUDIT.md](DEPENDENCY_AUDIT.md) - Detailed audit findings
- Upcoming: DEPENDENCY_NOTES.md (breaking changes during updates)

---

**Migration Plan Completed:** Dec 30, 2025  
**Prepared By:** @Backend  
**Next Step:** Begin update execution (Task 3)
