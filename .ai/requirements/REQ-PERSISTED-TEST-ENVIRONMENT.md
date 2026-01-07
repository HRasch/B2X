---
docid: REQ-PERSISTED-TEST-ENV
title: Persisted Test Environment
owner: @ProductOwner
status: Requirements Gathering
priority: P2
created: 2026-01-07
---

# Persisted Test Environment - Feature Specification

## 📋 Overview
Configure B2Connect services to support both **persisted storage** (for realistic test scenarios) and **temporary test environments** (for unit/integration tests), with initial seeding for Management-Frontend services and frontend UI for creating new tenants.

---

## 🎯 User Story
As a developer, I want services configured to work with persisted storages for testing, so that I can run realistic tests with data persistence, while also having temporary test options available for unit tests, integration tests, etc. Initially, only Management-Frontend services are seeded, and the frontend offers options to create a new tenant.

---

## ✅ Acceptance Criteria

### Configuration & Setup
- [ ] Services can be configured to use persisted storage (PostgreSQL) for testing
- [ ] Services can be configured to use temporary storage (in-memory) for unit/integration tests
- [ ] Configuration switchable via environment variables or appsettings
- [ ] Initial seed data populated for Management-Frontend services on startup

### Frontend Tenant Management
- [ ] Management frontend displays list of available test tenants
- [ ] "Create New Tenant" button/form available in Management UI
- [ ] New tenants can be created with custom configuration
- [ ] Tenant list is refreshable and reflects new tenants immediately

### Temporary Test Environment
- [ ] Unit tests can run with temporary in-memory storage
- [ ] Integration tests can run with temporary in-memory or persisted storage
- [ ] No data persistence between test runs (optional cleanup)
- [ ] Minimal setup/teardown overhead for tests

### Persisted Test Environment
- [ ] Test data persists across multiple test runs
- [ ] Easy access to test databases for verification
- [ ] Data can be reset/recreated when needed
- [ ] Supports realistic multi-tenant scenarios

---

## 📊 Scope

### In Scope
1. **Backend Configuration**
   - Persisted storage option (PostgreSQL for testing)
   - Temporary storage option (in-memory for unit tests)
   - Environment-based configuration
   - Initial seed data for Management-Frontend

2. **Frontend Enhancement**
   - Tenant management UI in Management frontend
   - Create tenant form/modal
   - List and display existing test tenants
   - Refresh/reload functionality

3. **Database/Storage**
   - Test database schema creation
   - Seed data for initial setup
   - Data isolation between tenants

### Out of Scope
- Production environment testing
- Performance/load testing infrastructure
- Advanced monitoring/observability for tests
- CI/CD integration (can be addressed in follow-up)

---

## 🔗 Dependencies

### Architectural
- Multi-tenant architecture (already implemented)
- Existing seed data infrastructure
- Gateway/API services configuration
- Identity/Authentication services

### Technical
- PostgreSQL for persisted testing
- In-memory database/store for temporary tests
- Entity Framework/database layer
- Wolverine CQRS handlers
- Vue 3 Management frontend

### Organizational
- QA team needs access to test tenants
- Developers need clear documentation on setup
- CI/CD pipeline may need integration (future)

---

## 🏗️ Design Considerations

### Configuration Strategy
```
Environment Variables:
- TEST_STORAGE_MODE: "persisted" | "temporary"
- TEST_ENVIRONMENT: "development" | "testing"
- TEST_SEED_DATA: true | false
- TEST_DATABASE_URL: (for persisted)
```

### Storage Options
1. **Persisted** → PostgreSQL test database
2. **Temporary** → In-memory repositories/stores

### Multi-Tenant Seeding
- Create default "test-tenant" or "management-tenant"
- Allow creating additional tenants from UI
- Each tenant has isolated data

---

## 🔒 Security & Data Protection

- [ ] Test data is marked as non-production
- [ ] No sensitive production data in test seeds
- [ ] Test databases are isolated from production
- [ ] Authentication works same way in test environments
- [ ] Tenant isolation properly enforced in test scenarios

---

## 📚 Initial Implementation Notes

### Management Frontend Seeding
- Create "Management" tenant automatically
- Seed with sample users and configurations
- Pre-populate with sample data for testing

### Tenant Creation
- UI form allows entering tenant name, configuration
- Backend validates and creates tenant
- Returns tenant ID/credentials for testing
- Stored in test database (persisted) or memory (temporary)

### Service Configuration
- Gateway (Store/Admin) accepts test configuration
- Domain services (Catalog, CMS, etc.) respect storage mode
- Repositories use appropriate storage backend

---

## 📝 Acceptance Criteria Details

### Backend
1. Configuration abstraction layer for storage selection
2. Service startup respects configuration
3. Seed data applied on initialization
4. Multi-tenant context properly set in tests

### Frontend
1. Management UI displays current test tenants
2. Form to create new tenant
3. Validation on tenant creation
4. Feedback messages on success/error

### Testing
1. Unit tests run with temporary storage (no DB required)
2. Integration tests can use either storage option
3. Test data isolation verified
4. Cleanup between test runs validated

---

## 🎬 Next Steps
1. **@Backend** - Analyze storage configuration and implementation approach
2. **@Frontend** - Design tenant management UI
3. **@Security** - Review test data protection and isolation
4. **@Architect** - Assess integration with multi-tenant architecture
5. **@SARAH** - Consolidate analyses and create task breakdown

---

## 📞 Questions for Analysis

### Backend
- How to abstract storage layer for easy switching?
- Where should seed data be defined?
- How to manage test database migrations?

### Frontend
- Where should tenant management UI be placed?
- What information should be displayed for each tenant?
- How to handle tenant creation workflow?

### Security
- What constitutes safe test data?
- How to ensure tenant isolation in tests?
- What access controls needed for test tenants?

### Architecture
- Should test tenants differ from production tenants structurally?
- How to integrate with existing identity/auth?
- Any service boundary considerations?

---

**Status**: Awaiting multi-agent analysis  
**Created**: 2026-01-07  
**Next Review**: After agent analyses completed
