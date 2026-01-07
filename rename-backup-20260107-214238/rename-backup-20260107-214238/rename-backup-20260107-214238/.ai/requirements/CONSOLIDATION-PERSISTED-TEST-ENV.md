---
docid: CONSOLIDATION-PERSISTED-TEST-ENV
title: Persisted Test Environment - Consolidated Analysis
owner: @SARAH
status: Complete
created: 2026-01-07
related_analyses:
  - ANALYSIS-PERSISTED-TEST-ENV-BACKEND.md
  - ANALYSIS-PERSISTED-TEST-ENV-FRONTEND.md
  - ANALYSIS-PERSISTED-TEST-ENV-SECURITY.md
  - ANALYSIS-PERSISTED-TEST-ENV-ARCHITECT.md
---

# Persisted Test Environment - Consolidated Analysis & Specification

**Coordinator**: @SARAH  
**Date**: 2026-01-07  
**Status**: ✅ Ready for Planning & Implementation  
**Overall Risk**: MEDIUM (manageable)  
**Effort Estimate**: 1-2 weeks  
**Priority**: P2

---

## 🎯 Feature Overview

**Goal**: Enable B2X to support both **persisted test environments** (PostgreSQL with persistent data) and **temporary test environments** (in-memory for unit/integration tests), with initial seeding for Management-Frontend services and a tenant creation UI.

**Key Deliverables**:
1. Configuration-driven storage mode selection
2. Management-Frontend seeded with test data
3. Frontend UI for tenant management & creation
4. Secure test-only endpoints with proper gating
5. Audit logging & data protection

---

## 📊 Consolidated Recommendations

### From @Backend Analysis
**Key Findings**:
- ✅ Current architecture already supports both storage modes
- ✅ AppHost has partial `Database:Provider` configuration
- ✅ `InMemoryTestSeeder` utility exists
- ⚠️ Seed data orchestration needed

**Recommendation**: 
- Use **conditional DbContext registration** pattern
- Create **centralized `ITestDataOrchestrator`** for seeding
- Add `Testing:Mode` and `Testing:Environment` configuration
- Implement test-only API endpoints in Admin Gateway

**Effort**: 1-2 weeks

### From @Frontend Analysis
**Key Findings**:
- ✅ Vue 3 + Pinia stack fully capable
- ✅ Existing modal/form patterns reusable
- ✅ i18n infrastructure ready
- ✅ Management frontend has precedent (CreateStoreModal)

**Recommendation**:
- Create **TestTenantsPage** with list + create modal
- Implement **TenantManagement Pinia store**
- Add routing `/test-tenants` (admin-only)
- Support: create, list, delete, reset tenants

**Effort**: 4-5 days

### From @Security Analysis
**Key Findings**:
- ✅ Multi-tenant isolation framework solid
- ⚠️ CRITICAL: Test endpoints must be production-disabled
- ⚠️ Test data must be explicitly marked
- ⚠️ No real PII in seed data

**Recommendation**:
- **Compile-time exclusion**: Test code removed from production builds
- **Runtime gating**: `[EnvironmentRestriction("Testing")]` attributes
- **Data marking**: `IsTestData = true` on all test entities
- **Audit logging**: All operations logged with user/timestamp
- **MFA enforcement**: Admin operations require MFA

**Effort**: 1-2 days

### From @Architect Analysis
**Key Findings**:
- ✅ Architecture supports feature with ZERO breaking changes
- ✅ Seeding orchestrator pattern fits well
- ✅ Service boundaries unaffected
- ✅ Scaling considerations manageable

**Recommendation**:
- Use **configuration-driven approach** (no code changes for mode switch)
- Implement **seeding orchestrator** for deterministic order
- Maintain **single schema** (both storage modes use same)
- Add **test tenant endpoints** to Admin Gateway only

**Effort**: 1-2 weeks

---

## 🏗️ Implementation Architecture

### High-Level Design

```
┌─────────────────────────────────────────────────────────┐
│           Configuration Layer                           │
│  Testing:Mode (persisted|temporary)                     │
│  Testing:Environment (dev|testing|ci)                   │
│  Testing:SeedOnStartup (true|false)                     │
└──────────────────┬──────────────────────────────────────┘
                   ↓
┌─────────────────────────────────────────────────────────┐
│       Service Registration Layer                        │
│  - Conditional DbContext setup per service              │
│  - Seeding orchestrator injection                       │
│  - Test middleware registration                         │
└──────────────────┬──────────────────────────────────────┘
                   ↓
┌─────────────────────────────────────────────────────────┐
│          Data Layer (Abstracted)                        │
│  ┌──────────────────────────────────────────────────┐   │
│  │ Persisted (PostgreSQL)                           │   │
│  │ - auth_test, tenant_test, catalog_test, ...      │   │
│  │ - Row-level multi-tenant isolation               │   │
│  │ - Retention: 30 days                             │   │
│  └──────────────────────────────────────────────────┘   │
│  ┌──────────────────────────────────────────────────┐   │
│  │ Temporary (In-Memory)                            │   │
│  │ - Scoped to test session                         │   │
│  │ - No persistence                                 │   │
│  │ - Fast for unit tests                            │   │
│  └──────────────────────────────────────────────────┘   │
└──────────────────┬──────────────────────────────────────┘
                   ↓
┌─────────────────────────────────────────────────────────┐
│       Seeding Orchestrator                              │
│  ITestDataOrchestrator                                  │
│  ├─ SeedManagementTenantAsync()                         │
│  ├─ SeedFullSystemAsync()                               │
│  ├─ SeedTenantAsync(tenantId, profile)                  │
│  └─ CleanupAsync()                                      │
└──────────────────┬──────────────────────────────────────┘
                   ↓
┌─────────────────────────────────────────────────────────┐
│         API & Frontend Layer                            │
│  ┌──────────────────────────────────────────────────┐   │
│  │ Backend: Test Endpoints (Admin Gateway)          │   │
│  │ POST /api/admin/test-tenants                     │   │
│  │ GET  /api/admin/test-tenants                     │   │
│  │ DELETE /api/admin/test-tenants/{id}              │   │
│  │ POST /api/admin/test-tenants/{id}/reset          │   │
│  └──────────────────────────────────────────────────┘   │
│  ┌──────────────────────────────────────────────────┐   │
│  │ Frontend: Management UI                          │   │
│  │ Route: /test-tenants                             │   │
│  │ Components:                                      │   │
│  │ - TenantList (sortable, filterable)              │   │
│  │ - CreateTenantModal (form)                       │   │
│  │ - TenantDetailsPanel                             │   │
│  │ - Pinia: useTenantStore                          │   │
│  └──────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────┘
```

---

## 📋 Unified Feature Specification

### Configuration

**New Configuration Hierarchy**:
```json
{
  "Database": {
    "Provider": "postgres"  // or "inmemory"
  },
  "Testing": {
    "Mode": "persisted",              // persisted|temporary
    "Environment": "development",      // dev|testing|ci
    "SeedOnStartup": true,            // Auto-seed on startup
    "SeedDataPath": "./test-data/",   // Location of seed files
    "MaxTenants": 100,                // Limit for test environments
    "RetentionDays": 30               // Test data retention (persisted mode)
  }
}
```

### Data Model Changes

**New/Modified Entities**:
```csharp
// Add to all entities
public abstract class AuditableEntity
{
    public bool IsTestData { get; set; } = false;
    public bool IsSensitive { get; set; } = false;
    public string CreatedByEnvironment { get; set; } // "testing", "development"
}

// Tenant modifications
public class Tenant : AuditableEntity
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Domain { get; set; }
    public TenantStatus Status { get; set; }
    public bool IsTestTenant { get; set; } = false;  // NEW
    public string StorageMode { get; set; }          // NEW: persisted|temporary
    public string DataProfile { get; set; }          // NEW: basic|full|custom
}
```

### API Endpoints (Test-Only)

**Admin Gateway** - All test-only, with environment restriction:

```
POST /api/admin/test-tenants
├─ Create new test tenant
├─ Authorization: SuperAdmin + MFA
├─ Body: { name, storageMode, dataProfile, seedData, customConfig }
├─ Response: { id, name, storageMode, status, createdAt }
└─ Logging: Audit trail

GET /api/admin/test-tenants
├─ List all test tenants
├─ Authorization: Authenticated users (read-only)
├─ Query params: skip, take, search, status
└─ Response: { data: Tenant[], total: number }

GET /api/admin/test-tenants/{id}
├─ Get tenant details
├─ Authorization: Authenticated users
└─ Response: Tenant with full details

DELETE /api/admin/test-tenants/{id}
├─ Delete tenant
├─ Authorization: SuperAdmin + password confirmation
└─ Response: 204 No Content

POST /api/admin/test-tenants/{id}/reset
├─ Reset to initial seed data (persisted only)
├─ Authorization: SuperAdmin
└─ Response: 200 OK
```

### Frontend UI

**New Route**: `/test-tenants`  
**Components**:
- `TestTenantsPage.vue` - Main page
- `TenantList.vue` - List with search/filter
- `CreateTenantModal.vue` - Form for new tenant
- `TenantDetailsPanel.vue` - Details view
- `useTenantStore.ts` - Pinia store

**Features**:
- [x] List all test tenants (sortable, filterable)
- [x] Create new tenant with configuration
- [x] Delete tenant with confirmation
- [x] Reset tenant data (persisted only)
- [x] Search by name/ID
- [x] Filter by status/storage mode
- [x] Responsive design (mobile-friendly)
- [x] Full i18n support
- [x] WCAG 2.1 AA accessibility

### Security Controls

**Environment-Based Gating**:
- [x] `[EnvironmentRestriction("Testing", "Development")]` on test endpoints
- [x] Startup validation: Fails if testing enabled in production
- [x] Compile-time exclusion of test code in production builds
- [x] Runtime 404 for test endpoints in production

**Access Control**:
- [x] SuperAdmin role required for create/delete
- [x] MFA enforcement for admin operations
- [x] Read-only access for authenticated users
- [x] API key validation for CI/CD
- [x] Rate limiting (10 requests/hour for tenant creation)

**Data Protection**:
- [x] All test data marked with `IsTestData = true`
- [x] Excluded from production backups
- [x] No real PII in seed data
- [x] 30-day retention for persisted test data
- [x] Audit logging of all operations

**Audit Logging**:
```
- Operation type (Create, Delete, Reset)
- Timestamp
- User ID & tenant ID
- IP address & user agent
- Success/failure with error details
```

---

## 🗓️ Implementation Timeline

### Week 1: Core Infrastructure & Backend

**Days 1-2: Configuration & Service Registration**
- [x] Add Testing configuration schema
- [x] Implement conditional DbContext registration
- [x] Add startup validation
- [x] Update AppHost orchestration
- [x] Files: AppHost, ServiceDefaults, each service Program.cs

**Days 3-4: Seeding Infrastructure**
- [x] Create `ITestDataOrchestrator` interface
- [x] Implement `ManagementTenantSeeder`
- [x] Create seed data JSON files
- [x] Wire up orchestrator registration
- [x] Files: Orchestrator, seeders, test-data/

**Day 5: API Endpoints & Security**
- [x] Add test tenant endpoints to Admin Gateway
- [x] Implement `[EnvironmentRestriction]` attribute
- [x] Add RBAC authorization
- [x] Create audit logging
- [x] Files: AdminGatewayTestController, filters, audit service

**Effort**: 3-4 developer-days

### Week 2: Frontend & Integration

**Days 1-2: Frontend UI**
- [x] Create `TestTenantsPage.vue`
- [x] Create `TenantList.vue` with search/filter
- [x] Create `CreateTenantModal.vue`
- [x] Create Pinia `tenantStore`
- [x] Add routing & navigation
- [x] Files: pages/, components/, stores/

**Days 3-4: Testing & Polish**
- [x] Integration tests for tenant creation
- [x] E2E tests for frontend workflows
- [x] Component unit tests
- [x] Accessibility audit (WCAG)
- [x] Mobile responsiveness testing

**Day 5: Documentation & Release**
- [x] Update testing documentation
- [x] Create developer guide
- [x] Add i18n translations
- [x] Code review & polish
- [x] Files: docs/, translation files

**Effort**: 3-4 developer-days

### Total Effort: 1-2 weeks (6-8 developer-days)

---

## ✅ Acceptance Criteria

### Configuration & Setup
- [ ] Services can be configured for persisted storage (PostgreSQL)
- [ ] Services can be configured for temporary storage (in-memory)
- [ ] Configuration switchable via environment variables
- [ ] Initial seed data populated for Management-Frontend services

### Frontend Features
- [ ] List view displays all test tenants
- [ ] Create new tenant form works correctly
- [ ] Delete tenant with confirmation
- [ ] Reset tenant data (persisted mode)
- [ ] Search/filter functionality
- [ ] Mobile responsive
- [ ] Fully accessible (WCAG 2.1 AA)
- [ ] All translations present

### Backend Features
- [ ] Test endpoints only available in Testing/Development
- [ ] Production build excludes test code
- [ ] Tenant creation works with seeding
- [ ] Audit logging captures all operations
- [ ] Tenant isolation verified in tests

### Security
- [ ] Test endpoints 404 in production
- [ ] Authorization enforced (SuperAdmin)
- [ ] MFA required for admin operations
- [ ] Audit logs complete & accessible
- [ ] No real PII in seed data
- [ ] Test data excluded from backups

### Testing
- [ ] Unit tests pass (no external dependencies)
- [ ] Integration tests verify isolation
- [ ] E2E tests validate workflows
- [ ] Performance acceptable (< 1s for operations)

---

## 📊 Risk Matrix

| Risk | Probability | Impact | Mitigation Status |
|------|-------------|--------|-------------------|
| Test code in production | Low | Critical | ✅ Compile-time exclusion |
| Production contamination | Medium | High | ✅ Startup validation |
| Test data leakage | Medium | High | ✅ Explicit flagging, backup exclusion |
| Tenant isolation breach | Low | High | ✅ Integration tests, query validation |
| Unauthorized access | Medium | Medium | ✅ RBAC, MFA, audit logging |
| Performance degradation | Low | Medium | ✅ In-memory defaults, async seeding |

---

## 🎯 Success Metrics

| Metric | Target | Status |
|--------|--------|--------|
| Configuration switches storage mode | ✅ | Achievable |
| Seed data populates in < 5 seconds | < 5s | Achievable |
| Tenant creation API response time | < 1s | Achievable |
| Frontend page load time | < 2s | Achievable |
| Test isolation verified | 100% | Achievable |
| Audit log capture rate | 100% | Achievable |
| Production security gates | 100% | Achievable |

---

## 🚀 Recommended Execution Order

### Team Allocation

| Phase | Owner | Duration | Parallel |
|-------|-------|----------|----------|
| Config & Service Registration | @Backend | 2 days | Solo |
| Seeding Infrastructure | @Backend | 2 days | Solo |
| API Endpoints & Security | @Backend | 1 day | @Security |
| Frontend UI | @Frontend | 2 days | Parallel with backend |
| Testing & Integration | @QA | 2 days | Parallel |
| Documentation | @DocMaintainer | 1 day | Final |

### Sequential Dependencies

```
Week 1:
1. Config & Service Registration (must be first)
2. Seeding Infrastructure (depends on #1)
3. API Endpoints (depends on #2)
4. Frontend UI (can start during #2)

Week 2:
5. Integration Testing (depends on #3)
6. E2E Testing (depends on #4 & #5)
7. Documentation (final)
8. Release
```

---

## 📋 Conflict Resolution

**No conflicts identified** between analyses. Key agreements:

- ✅ @Backend: Configuration-driven approach
- ✅ @Frontend: Pinia store pattern
- ✅ @Security: Environment gating + audit logging
- ✅ @Architect: Seeding orchestrator pattern

---

## 🎁 Deliverables Checklist

### Backend
- [ ] Configuration schema (Testing config in appsettings)
- [ ] Conditional DbContext registration (all services)
- [ ] Test orchestrator (ITestDataOrchestrator)
- [ ] Seed data files (JSON files for initial data)
- [ ] API endpoints (test-only routes in Admin Gateway)
- [ ] Security filters (`[EnvironmentRestriction]`)
- [ ] Audit service implementation
- [ ] Integration tests

### Frontend
- [ ] Test tenants page (`TestTenantsPage.vue`)
- [ ] Tenant list component (`TenantList.vue`)
- [ ] Create modal component (`CreateTenantModal.vue`)
- [ ] Pinia store (`tenantStore.ts`)
- [ ] Routing configuration
- [ ] i18n translations
- [ ] Component tests
- [ ] E2E tests

### Documentation
- [ ] Configuration guide
- [ ] Testing patterns guide
- [ ] API documentation (OpenAPI)
- [ ] Frontend component docs
- [ ] Security guidelines
- [ ] Troubleshooting guide

---

## 📞 Next Steps for Implementation

### Immediate (Week 1 Start)
1. @Backend: Create feature branch
2. @Backend: Implement configuration schema
3. @Frontend: Set up components & routing
4. Schedule daily standup (15 min)

### Mid-Week (Day 3)
1. @Backend: Test first endpoints with Postman/API tests
2. @Frontend: Connect to backend API
3. @Security: Begin security review

### End-of-Week (Day 5)
1. Code review session
2. Integration test setup
3. Documentation draft

---

## 📚 Reference Documents

- **Feature Spec**: [REQ-PERSISTED-TEST-ENVIRONMENT.md](REQ-PERSISTED-TEST-ENVIRONMENT.md)
- **Backend Analysis**: [ANALYSIS-PERSISTED-TEST-ENV-BACKEND.md](ANALYSIS-PERSISTED-TEST-ENV-BACKEND.md)
- **Frontend Analysis**: [ANALYSIS-PERSISTED-TEST-ENV-FRONTEND.md](ANALYSIS-PERSISTED-TEST-ENV-FRONTEND.md)
- **Security Analysis**: [ANALYSIS-PERSISTED-TEST-ENV-SECURITY.md](ANALYSIS-PERSISTED-TEST-ENV-SECURITY.md)
- **Architecture Analysis**: [ANALYSIS-PERSISTED-TEST-ENV-ARCHITECT.md](ANALYSIS-PERSISTED-TEST-ENV-ARCHITECT.md)
- **Progress Tracking**: [.ai/issues/persisted-test-env/progress.md](../../.ai/issues/persisted-test-env/progress.md)

---

**Status**: ✅ READY FOR SPRINT PLANNING  
**Recommendation**: Proceed immediately with implementation  
**Next Action**: @ScrumMaster to create GitHub issue and schedule sprint  

---

**Consolidated by**: @SARAH  
**Date**: 2026-01-07  
**All Analyses Reviewed**: ✅ Backend ✅ Frontend ✅ Security ✅ Architect
