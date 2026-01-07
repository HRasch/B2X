---
issue_id: gh-issue-persisted-test-env
title: Persisted Test Environment - Sprint Planning
created: 2026-01-07
related_docs:
  - REQ-PERSISTED-TEST-ENVIRONMENT.md
  - CONSOLIDATION-PERSISTED-TEST-ENV.md
---

# GitHub Issue Template: Persisted Test Environment

## 📋 Issue Title
**Persisted Test Environment: Configuration-Driven Storage & Management Frontend Seeding**

## 🎯 Feature Overview

Enable B2Connect to support both **persisted test environments** (PostgreSQL with persistent data) and **temporary test environments** (in-memory for unit/integration tests), with initial seeding for Management-Frontend services and tenant management UI.

**Priority**: P2 (Medium)  
**Effort**: 1-2 weeks (6-8 developer-days)  
**Risk**: MEDIUM (manageable)

## 📄 Acceptance Criteria

### Configuration & Storage
- [ ] Services support PostgreSQL for persisted testing
- [ ] Services support in-memory storage for temporary testing
- [ ] `Testing:Mode` configuration controls storage selection
- [ ] Configuration switchable via environment variables
- [ ] Startup validation fails if testing enabled in production

### Frontend Features
- [ ] Test tenant management page at `/test-tenants`
- [ ] Tenant list displays all test tenants (sortable, filterable)
- [ ] Create new tenant form with configuration options
- [ ] Delete tenant with confirmation dialog
- [ ] Reset tenant data (persisted mode only)
- [ ] Search by name/ID
- [ ] Mobile responsive design
- [ ] Full WCAG 2.1 AA accessibility

### Backend Features
- [ ] API endpoints for test tenant operations (admin-only)
- [ ] Test data seeding on startup (configurable)
- [ ] Management tenant pre-seeded with default admin user
- [ ] Tenant creation with custom configuration
- [ ] Audit logging of all test operations
- [ ] Comprehensive test data protection

### Security & Testing
- [ ] Test endpoints protected by `[EnvironmentRestriction]`
- [ ] 404 response for test endpoints in production
- [ ] All test data marked with `IsTestData = true`
- [ ] No real PII in seed data
- [ ] Test data excluded from production backups
- [ ] Unit tests pass with temporary storage
- [ ] Integration tests verify tenant isolation
- [ ] Audit trail captures user/timestamp/details

## 📊 Implementation Breakdown

### Phase 1: Backend Infrastructure (3-4 days)

**Week 1, Days 1-2: Configuration & Service Registration**
- [ ] Add `TestingConfiguration` schema to appsettings
- [ ] Implement conditional DbContext registration in ServiceDefaults
- [ ] Add startup validation (fails in production if testing enabled)
- [ ] Update AppHost for in-memory database support
- [ ] Documentation: Configuration guide

**Files**:
- `appsettings.json`, `appsettings.Development.json`, `appsettings.Testing.json`
- `ServiceDefaults/Extensions.cs`
- `AppHost/Program.cs`

**Week 1, Days 3-4: Seeding Infrastructure**
- [ ] Create `ITestDataOrchestrator` interface
- [ ] Implement `ManagementTenantSeeder`
- [ ] Create seed data JSON files (`test-data/auth/`, `test-data/tenant/`)
- [ ] Wire up orchestrator DI registration
- [ ] Create test HTTP endpoint for manual seeding

**Files**:
- `backend/shared/TestData/ITestDataOrchestrator.cs`
- `backend/shared/TestData/ManagementTenantSeeder.cs`
- `test-data/auth/users.json`
- `test-data/tenant/tenants.json`

**Week 1, Day 5: API & Security**
- [ ] Add test endpoints to Admin Gateway (POST, GET, DELETE, POST reset)
- [ ] Create `[EnvironmentRestriction]` attribute and filter
- [ ] Implement RBAC authorization (SuperAdmin required)
- [ ] Create `AuditLog` entity and audit service
- [ ] Add rate limiting on sensitive operations

**Files**:
- `backend/Gateway/Admin/src/Controllers/TestTenantController.cs`
- `backend/shared/Filters/EnvironmentRestrictionFilter.cs`
- `backend/shared/Audit/AuditLog.cs`
- `backend/shared/Audit/AuditService.cs`

### Phase 2: Frontend (2-3 days) - *Parallel with Phase 1*

**Week 1-2, Days 1-2: Components & State Management**
- [ ] Create `TestTenantsPage.vue` (main page)
- [ ] Create `TenantList.vue` (list with search/filter)
- [ ] Create `CreateTenantModal.vue` (form)
- [ ] Create `useTenantStore.ts` (Pinia store)
- [ ] Add routing `/test-tenants`
- [ ] Add navigation menu item

**Files**:
- `frontend/Management/src/pages/TestTenantsPage.vue`
- `frontend/Management/src/components/TenantList.vue`
- `frontend/Management/src/components/CreateTenantModal.vue`
- `frontend/Management/src/stores/tenant.ts`
- `frontend/Management/src/router.ts`

**Week 2, Days 3-4: Features & Styling**
- [ ] Implement search/filter functionality
- [ ] Add delete with confirmation
- [ ] Add reset functionality
- [ ] Responsive design (mobile-friendly)
- [ ] Loading states & error handling
- [ ] i18n translations (en, de, fr, es, it, pt, nl, pl)

**Files**:
- `frontend/Management/src/i18n/locales/tenants.json` (all languages)
- Component updates with styles

### Phase 3: Testing & Integration (2 days) - *Parallel*

**Week 2, Days 1-3: Testing**
- [ ] Unit tests for Pinia store
- [ ] Unit tests for API service
- [ ] Integration tests for tenant creation
- [ ] Integration tests for tenant isolation
- [ ] E2E tests for complete workflow
- [ ] Accessibility audit

**Files**:
- `backend/Tests/TenantManagement.Tests.cs`
- `frontend/Management/src/stores/__tests__/tenant.spec.ts`
- `frontend/Management/src/components/__tests__/TenantList.spec.ts`

### Phase 4: Documentation & Release (1 day)

**Week 2, Day 5: Documentation**
- [ ] Update testing documentation
- [ ] Create developer guide
- [ ] API documentation (OpenAPI/Swagger)
- [ ] Troubleshooting guide
- [ ] Code review & final polish

**Files**:
- `docs/testing/PERSISTED_TEST_ENVIRONMENT.md`
- `docs/guides/TESTING_PATTERNS.md`

---

## 🔧 Technical Details

### Configuration Example

**Development with Persisted Storage**:
```json
{
  "Database": {
    "Provider": "postgres"
  },
  "Testing": {
    "Mode": "persisted",
    "Environment": "development",
    "SeedOnStartup": true,
    "SeedDataPath": "./test-data/"
  }
}
```

### API Endpoints

```
POST   /api/admin/test-tenants
GET    /api/admin/test-tenants
GET    /api/admin/test-tenants/{id}
DELETE /api/admin/test-tenants/{id}
POST   /api/admin/test-tenants/{id}/reset
```

### Data Model

```csharp
public class Tenant
{
    public string Id { get; set; }
    public string Name { get; set; }
    public bool IsTestTenant { get; set; }
    public string StorageMode { get; set; } // persisted|temporary
    public string DataProfile { get; set; }  // basic|full|custom
    public bool IsTestData { get; set; }
}
```

---

## 📚 Reference Documents

- **Feature Specification**: [REQ-PERSISTED-TEST-ENVIRONMENT.md](../../.ai/requirements/REQ-PERSISTED-TEST-ENVIRONMENT.md)
- **Consolidated Analysis**: [CONSOLIDATION-PERSISTED-TEST-ENV.md](../../.ai/requirements/CONSOLIDATION-PERSISTED-TEST-ENV.md)
- **Backend Analysis**: [ANALYSIS-PERSISTED-TEST-ENV-BACKEND.md](../../.ai/requirements/ANALYSIS-PERSISTED-TEST-ENV-BACKEND.md)
- **Frontend Analysis**: [ANALYSIS-PERSISTED-TEST-ENV-FRONTEND.md](../../.ai/requirements/ANALYSIS-PERSISTED-TEST-ENV-FRONTEND.md)
- **Security Analysis**: [ANALYSIS-PERSISTED-TEST-ENV-SECURITY.md](../../.ai/requirements/ANALYSIS-PERSISTED-TEST-ENV-SECURITY.md)
- **Architecture Analysis**: [ANALYSIS-PERSISTED-TEST-ENV-ARCHITECT.md](../../.ai/requirements/ANALYSIS-PERSISTED-TEST-ENV-ARCHITECT.md)

---

## 👥 Team Assignments

| Component | Owner | Duration | Status |
|-----------|-------|----------|--------|
| Configuration & Registration | @Backend | 2 days | Ready |
| Seeding Infrastructure | @Backend | 2 days | Ready |
| API & Security | @Backend + @Security | 1 day | Ready |
| Frontend UI | @Frontend | 2 days | Ready |
| Testing | @QA | 2 days | Ready |
| Documentation | @DocMaintainer | 1 day | Ready |

---

## ⚠️ Risk Assessment

| Risk | Mitigation |
|------|-----------|
| Test code in production | Compile-time exclusion + startup validation |
| Test data leaks | Explicit flagging + backup exclusion |
| Tenant isolation breach | Integration tests + query validation |
| Unauthorized access | RBAC + MFA + audit logging |
| Performance issues | In-memory defaults + async seeding |

---

## ✅ Definition of Done

- [ ] All acceptance criteria met
- [ ] Code reviewed and approved
- [ ] Unit tests passing (>80% coverage)
- [ ] Integration tests passing
- [ ] E2E tests passing
- [ ] Accessibility audit passed (WCAG 2.1 AA)
- [ ] Documentation complete
- [ ] Security review passed
- [ ] No breaking changes to existing APIs
- [ ] Ready for production deployment

---

## 📞 Questions & Decisions Needed

1. **Seed Data Timing**: Auto-seed on startup or expose endpoint?
   - **Decision**: Both - API endpoint for manual, config flag for auto

2. **Storage Mode Default**: Persisted or temporary for testing?
   - **Decision**: Persisted (more realistic), temporary available as option

3. **Retention Period**: How long to keep persisted test data?
   - **Decision**: 30 days, auto-cleanup

4. **MFA Requirement**: Required for all admin operations?
   - **Decision**: Yes, for SuperAdmin create/delete

---

## 🚀 Success Metrics

- Tenant creation response time < 1 second
- Seed data populates in < 5 seconds
- Frontend page load < 2 seconds
- 100% test isolation verified
- 100% audit log capture
- Zero production security issues

---

## 📅 Timeline

**Start**: 2026-01-09 (next sprint)  
**End**: 2026-01-23 (2 weeks)  
**Milestones**:
- End of Week 1: Backend infrastructure complete
- End of Week 2: Frontend complete, testing underway
- Week 2 End: Full feature complete & tested

---

**Prepared by**: @SARAH (Coordinator)  
**Date**: 2026-01-07  
**Status**: Ready for Sprint Planning Meeting  
**Next**: Schedule standup with team leads
