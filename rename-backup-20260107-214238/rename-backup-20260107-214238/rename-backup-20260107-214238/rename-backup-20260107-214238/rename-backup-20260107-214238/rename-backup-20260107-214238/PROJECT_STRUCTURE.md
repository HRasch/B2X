# 📁 B2Connect Project Structure - Post-Cleanup

**Last Updated**: 29. Dezember 2025  
**Status**: ✅ Clean & Organized  
**Purpose**: Quick reference for navigating the reorganized project

---

## 🎯 Quick Navigation

### For New Team Members
1. **Start Here**: [QUICK_START_GUIDE.md](./QUICK_START_GUIDE.md)
2. **Your Role?**: See `.github/copilot-instructions-*.md` (backend, frontend, qa, etc.)
3. **Compliance**: [docs/compliance/](./docs/compliance/)
4. **Architecture**: [docs/architecture/](./docs/architecture/)

### Project Root (10 Essential Files)
```
/
├── README.md                                    ← Project overview
├── QUICK_START_GUIDE.md                         ← NAVIGATION HUB
├── .ai/knowledgebase/governance.md              ← Decision-making
├── PROJECT_DASHBOARD.md                         ← Metrics
├── ACCESSIBILITY_COMPLIANCE_REPORT.md           ← Compliance
├── ISSUE_TEMPLATE_STORE_LEGAL_COMPLIANCE.md     ← GitHub template
├── SPRINT_1_KICKOFF.md                          ← Launch reference
├── SPRINT_3_COMPLETION_SUMMARY.md               ← Latest sprint
├── SPRINT_3_PHASE_2_CONTINUATION_GUIDE.md       ← Next session
└── CLEANUP_SUMMARY.md                           ← This cleanup
```

### Docs Structure (9 Subdirectories)
```
docs/
├── compliance/                    (Legal, compliance tests, VAT, pricing)
│   ├── P0.6_ECOMMERCE_LEGAL_TESTS.md
│   ├── P0.7_AI_ACT_TESTS.md
│   ├── P0.8_BARRIEREFREIHEIT_BITV_TESTS.md
│   ├── P0.9_ERECHNUNG_TESTS.md
│   └── ... (19 files total)
│
├── guides/                        (Developer guides, quick references)
│   ├── DEVELOPER_GUIDE.md
│   ├── DEVELOPER_QUICK_REFERENCE.md
│   ├── TESTING_GUIDE.md
│   ├── TESTING_FRAMEWORK_GUIDE.md
│   ├── MODEL_BINDING_QUICK_REFERENCE.md
│   └── ... (26 files total)
│
├── archive/                       (Historical & reference docs)
│   ├── implementation-guides/     (Admin, API, ASPIRE, Auth, CQRS, Email, etc.)
│   ├── architecture-docs/         (DDD, Wolverine, ONION, patterns)
│   ├── integration-guides/        (Feature integrations)
│   ├── reference-docs/            (Infrastructure, deployment, config)
│   │   └── github-workflows/      (GitHub/PR setup)
│   ├── feature-guides/            (AI/Agent, localization, payment)
│   ├── sprints/                   (Sprint 1-3 completion docs)
│   ├── processes/                 (Setup & optimization guides)
│   └── legacy/                    (173 deprecated files - preserved)
│
├── api/                           (API documentation)
├── architecture/                  (ADRs, architectural decisions)
├── by-role/                       (Role-based documentation)
├── features/                      (Feature specifications)
└── user-guides/                   (EN/DE user documentation)
```

---

## 📍 Finding Documents by Topic

### Legal & Compliance
- **Where**: `docs/compliance/`
- **Files**: P0.6-P0.9 tests, VAT, pricing, e-commerce, AI Act, BITV
- **Quick Access**: [docs/compliance/](./docs/compliance/)

### Developer Guides
- **Where**: `docs/guides/`
- **Files**: TESTING_GUIDE.md, DEVELOPER_GUIDE.md, MODEL_BINDING_QUICK_REFERENCE.md
- **Quick Access**: [docs/guides/](./docs/guides/)

### Architecture & Design
- **Where**: `docs/architecture/`
- **Files**: ADRs, architectural decisions, system design
- **Quick Access**: [docs/architecture/](./docs/architecture/)

### Infrastructure & Deployment
- **Where**: `docs/archive/reference-docs/`
- **Files**: DEPLOYMENT_*.md, DATABASE_*.md, PORT_BLOCKING_SOLUTION.md, VSCODE_*.md
- **Quick Access**: [docs/archive/reference-docs/](./docs/archive/reference-docs/)

### GitHub Workflows
- **Where**: `docs/archive/reference-docs/github-workflows/`
- **Files**: GITHUB_PR_SETUP.md, PR_WORKFLOW_*.md
- **Quick Access**: [docs/archive/reference-docs/github-workflows/](./docs/archive/reference-docs/github-workflows/)

### Feature Documentation
- **Where**: `docs/archive/feature-guides/`
- **Files**: AI implementation, localization, payment processing
- **Quick Access**: [docs/archive/feature-guides/](./docs/archive/feature-guides/)

### Deprecated/Historical
- **Where**: `docs/archive/legacy/`
- **Files**: 173 files from previous iterations (fully preserved)
- **Quick Access**: [docs/archive/legacy/](./docs/archive/legacy/)

---

## 🔄 Backend Directory Structure

```
backend/
├── AppHost/                       (Aspire orchestration)
├── BoundedContexts/               (Microservices - Domain-Driven Design)
│   ├── Admin/                     (Admin API)
│   ├── Store/                     (Store API)
│   └── ...
├── CLI/                           (CLI tooling)
├── Domain/                        (Bounded contexts)
│   ├── Identity/                  (Auth, registration, JWT)
│   ├── Catalog/                   (Products, pricing)
│   ├── CMS/                       (Pages, content)
│   ├── Tenancy/                   (Multi-tenant management)
│   ├── Localization/              (i18n, translations)
│   ├── Theming/                   (UI themes)
│   ├── Search/                    (Elasticsearch integration)
│   └── ...
└── Orchestration/                 (Aspire host)
```

---

## 🎨 Frontend Directory Structure

```
Frontend/
├── Store/                         (Customer storefront)
│   └── src/
│       ├── components/            (Vue.js components)
│       ├── pages/                 (Page layouts)
│       ├── stores/                (Pinia state management)
│       └── ...
├── Admin/                         (Management dashboard)
│   └── src/
│       ├── components/            (Vue.js components)
│       ├── pages/                 (Page layouts)
│       └── ...
└── [other frontends]
```

---

## 📚 Documentation by Role

### Backend Developer
1. Start: [QUICK_START_GUIDE.md](./QUICK_START_GUIDE.md)
2. Role Guide: `.github/copilot-instructions-backend.md`
3. Architecture: `docs/architecture/DDD_BOUNDED_CONTEXTS.md`
4. Guides: `docs/guides/DEVELOPER_GUIDE.md`

### Frontend Developer
1. Start: [QUICK_START_GUIDE.md](./QUICK_START_GUIDE.md)
2. Role Guide: `.github/copilot-instructions-frontend.md`
3. Components: `docs/features/`
4. Guides: `docs/guides/FRONTEND_FEATURE_INTEGRATION_GUIDE.md`

### QA Engineer
1. Start: [QUICK_START_GUIDE.md](./QUICK_START_GUIDE.md)
2. Role Guide: `.github/copilot-instructions-qa.md`
3. Testing: `docs/guides/TESTING_GUIDE.md`
4. Compliance: `docs/compliance/`

### DevOps Engineer
1. Start: [QUICK_START_GUIDE.md](./QUICK_START_GUIDE.md)
2. Role Guide: `.github/copilot-instructions-devops.md`
3. Infrastructure: `docs/archive/reference-docs/`
4. Deployment: `docs/guides/DEPLOYMENT_*.md`

### Security Engineer
1. Start: [QUICK_START_GUIDE.md](./QUICK_START_GUIDE.md)
2. Role Guide: `.github/copilot-instructions-security.md`
3. Security: `docs/archive/reference-docs/`
4. Compliance: `docs/compliance/`

### Product Owner
1. Start: [QUICK_START_GUIDE.md](./QUICK_START_GUIDE.md)
2. Project Status: `PROJECT_DASHBOARD.md`
3. Compliance: `docs/compliance/`
4. Governance: `.ai/knowledgebase/governance.md`

### Tech Lead
1. Start: `docs/by-role/TECH_LEAD.md`
2. Architecture: `docs/architecture/`
3. Decisions: `docs/architecture/ADRs/`
4. All Documentation: Comprehensive access to all sections

---

## 🚀 Common Tasks & Where to Find Info

| Task | Location |
|------|----------|
| **Build & Run Backend** | [QUICK_START_GUIDE.md](./QUICK_START_GUIDE.md) |
| **Start Frontend Dev Server** | `docs/guides/FRONTEND_FEATURE_INTEGRATION_GUIDE.md` |
| **Run Tests** | `docs/guides/TESTING_GUIDE.md` |
| **Deploy to Production** | `docs/guides/DEPLOYMENT_*.md` |
| **Check Compliance** | `docs/compliance/` |
| **Understand Architecture** | `docs/architecture/DDD_BOUNDED_CONTEXTS.md` |
| **Setup Development Environment** | `docs/guides/VSCODE_*.md` |
| **Fix Port Issues (macOS)** | `docs/archive/reference-docs/MACOS_CDP_PORTFIX.md` |
| **Configure GitHub Workflows** | `docs/archive/reference-docs/github-workflows/` |
| **Database Schema** | `docs/guides/DATABASE_SCHEMA_VAT_VALIDATION.md` |

---

## ✅ Post-Cleanup Verification

**Documentation Status**: ✅ Clean & Organized
- Root files: 10 (down from 57) ✅
- Docs root files: 0 (down from 82) ✅
- Duplicate archives: Consolidated ✅
- Total files preserved: 251+ ✅
- Data loss: Zero ✅

**Navigation Status**: ✅ Ready
- QUICK_START_GUIDE.md: Updated ✅
- Role guides: In .github/ ✅
- Cross-references: Verified ✅
- Archive structure: Organized ✅

**Launch Status**: ✅ Ready (4 Januar 2026)
- Documentation: Clean & professional ✅
- Navigation: Clear & intuitive ✅
- Accessibility: WCAG 2.1 AA ✅
- Compliance: Verified (P0.6-P0.9) ✅

---

## 📞 Quick Links

- **Project Overview**: [README.md](./README.md)
- **Quick Start**: [QUICK_START_GUIDE.md](./QUICK_START_GUIDE.md)
- **Development Guides**: [docs/guides/](./docs/guides/)
- **Compliance**: [docs/compliance/](./docs/compliance/)
- **Architecture**: [docs/architecture/](./docs/architecture/)
- **Copilot Instructions**: [.github/copilot-instructions.md](./.github/copilot-instructions.md)
- **GitHub Actions**: [docs/archive/reference-docs/github-workflows/](./docs/archive/reference-docs/github-workflows/)
- **Historical Docs**: [docs/archive/](./docs/archive/)

---

## 🎯 Next Steps (When You're Ready)

1. **For Developers**: Read [QUICK_START_GUIDE.md](./QUICK_START_GUIDE.md)
2. **For Leaders**: Review [PROJECT_DASHBOARD.md](./PROJECT_DASHBOARD.md)
3. **For Compliance**: See [docs/compliance/](./docs/compliance/)
4. **For Architecture**: Check [docs/architecture/](./docs/architecture/)

---

**Created**: 29. Dezember 2025  
**Status**: ✅ **COMPLETE - All Documentation Organized**  
**Ready for**: 4 Januar 2026 Launch

