# 👥 Role-Based Documentation Map

**Purpose:** Strukturierte Zuweisung von Dokumentation zu Teamrollen  
**Last Updated:** 28. Dezember 2025  
**Total Documents:** 60+ | **Roles:** 8

---

## 🎯 Team Roles Overview

| Role | Primary Focus | Secondary Focus | P0 Components |
|------|--------------|-----------------|---------------|
| **🔐 Security Engineer** | Encryption, Auth, Keys | Incident Response, Compliance | P0.1, P0.2, P0.3, P0.5, P0.7 |
| **⚙️ DevOps Engineer** | Infrastructure, CI/CD | Monitoring, Scaling | P0.3, P0.4, P0.5 |
| **💻 Backend Developer** | APIs, Business Logic | Database, Testing | P0.1, P0.6, P0.7, P0.9 |
| **🎨 Frontend Developer** | Vue.js, UX | Accessibility, i18n | P0.6, P0.8 |
| **🧪 QA Engineer** | Testing, Automation | Compliance Verification | All P0 Tests |
| **📋 Product Owner** | Requirements, Roadmap | Compliance Decisions | Executive Summaries |
| **⚖️ Legal/Compliance Officer** | Regulatory Compliance | Risk Assessment | P0.6, P0.7, P0.8, P0.9 |
| **👔 Tech Lead/Architect** | Architecture, Patterns | Code Review, Standards | All Documentation |

---

## 🔐 Security Engineer

### Must-Read Documentation (P0 Critical)

#### Core Security
| Document | Path | Priority | Purpose |
|----------|------|----------|---------|
| **Encryption Implementation** | `docs/AUDIT_LOGGING_IMPLEMENTATION.md` | 🔴 P0 | AES-256 encryption patterns |
| **Security Checklist** | `.github/copilot-instructions.md` §Security | 🔴 P0 | Security guidelines |
| **Application Specs** | `docs/APPLICATION_SPECIFICATIONS.md` §Security | 🔴 P0 | Security requirements |
| **Pentester Review** | `docs/PENTESTER_REVIEW.md` | 🔴 P0 | Security audit findings |

#### Compliance Documents
| Document | Path | Priority | P0 Component |
|----------|------|----------|--------------|
| **EU Compliance Roadmap** | `docs/EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md` | 🔴 P0 | P0.1-P0.5 |
| **AI Act Compliance** | `docs/P0.7_AI_ACT_TESTS.md` | 🔴 P0 | P0.7 |
| **AI Act Overview** | `docs/AI_ACT_OVERVIEW.md` | 🟡 P1 | P0.7 |
| **Compliance Quick Start** | `docs/COMPLIANCE_QUICK_START_CHECKLIST.md` | 🟡 P1 | All |

#### Authentication & Authorization
| Document | Path | Priority |
|----------|------|----------|
| **Shared Authentication** | `docs/architecture/SHARED_AUTHENTICATION.md` | 🔴 P0 |
| **Identity Service** | `backend/Domain/Identity/README.md` | 🟡 P1 |

#### Test Specifications
| Document | Priority | Tests |
|----------|----------|-------|
| `docs/P0.7_AI_ACT_TESTS.md` | 🔴 P0 | 15 AI Act tests |
| `docs/COMPLIANCE_TESTING_EXAMPLES.md` | 🟡 P1 | Security tests |

### Onboarding Path (Security Engineer)
```
Week 1: Security Foundation
├── 📄 APPLICATION_SPECIFICATIONS.md §Security Requirements
├── 📄 copilot-instructions.md §Security Checklist
├── 📄 PENTESTER_REVIEW.md
└── 📄 SHARED_AUTHENTICATION.md

Week 2: Compliance Deep Dive
├── 📄 EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md (P0.1-P0.5)
├── 📄 P0.7_AI_ACT_TESTS.md
├── 📄 AI_ACT_OVERVIEW.md
└── 📄 AUDIT_LOGGING_IMPLEMENTATION.md

Week 3: Implementation
├── 📄 COMPLIANCE_TESTING_EXAMPLES.md
├── 📄 COMPLIANCE_QUICK_START_CHECKLIST.md
└── 🔧 Begin P0.1 (Audit Logging) implementation
```

---

## ⚙️ DevOps Engineer

### Must-Read Documentation (P0 Critical)

#### Infrastructure
| Document | Path | Priority | Purpose |
|----------|------|----------|---------|
| **Aspire Guide** | `docs/architecture/ASPIRE_GUIDE.md` | 🔴 P0 | Service orchestration |
| **Aspire Quick Fix** | `docs/ASPIRE_QUICK_FIX.md` | 🔴 P0 | Troubleshooting |
| **Port Blocking** | `docs/PORT_BLOCKING_SOLUTION.md` | 🔴 P0 | DCP port issues |
| **macOS CDP Fix** | `docs/MACOS_CDP_PORTFIX.md` | 🟡 P1 | macOS-specific |
| **VS Code Config** | `docs/architecture/VSCODE_ASPIRE_CONFIG.md` | 🟡 P1 | VS Code setup |

#### Compliance Infrastructure
| Document | Path | Priority | P0 Component |
|----------|------|----------|--------------|
| **EU Compliance Roadmap** | `docs/EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md` | 🔴 P0 | P0.3, P0.4, P0.5 |
| **Network Segmentation** | §P0.4 in EU Roadmap | 🔴 P0 | P0.4 |
| **Incident Response** | §P0.3 in EU Roadmap | 🔴 P0 | P0.3 |
| **Key Management** | §P0.5 in EU Roadmap | 🔴 P0 | P0.5 |

#### CI/CD & Deployment
| Document | Path | Priority |
|----------|------|----------|
| **GitHub Workflows** | `docs/GITHUB_WORKFLOWS.md` | 🟡 P1 |
| **GitHub Pages** | `docs/GITHUB_PAGES_SETUP.md` | 🟢 P2 |
| **Service Discovery** | `docs/SERVICE_DISCOVERY.md` | 🟡 P1 |

#### Troubleshooting
| Document | Path | Priority |
|----------|------|----------|
| **Aspire Dashboard Troubleshooting** | `docs/ASPIRE_DASHBOARD_TROUBLESHOOTING.md` | 🔴 P0 |
| **InMemory Quick Ref** | `docs/INMEMORY_QUICKREF.md` | 🟡 P1 |
| **VS Code InMemory** | `docs/VSCODE_INMEMORY_SETUP.md` | 🟡 P1 |

### Onboarding Path (DevOps Engineer)
```
Week 1: Infrastructure Setup
├── 📄 architecture/ASPIRE_GUIDE.md
├── 📄 PORT_BLOCKING_SOLUTION.md
├── 📄 ASPIRE_DASHBOARD_TROUBLESHOOTING.md
└── 📄 SERVICE_DISCOVERY.md

Week 2: Compliance Infrastructure
├── 📄 EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md (P0.3, P0.4, P0.5)
├── 📄 Focus: Network Segmentation (P0.4)
├── 📄 Focus: Incident Response Infrastructure (P0.3)
└── 📄 Focus: Key Management (P0.5)

Week 3: CI/CD & Monitoring
├── 📄 GITHUB_WORKFLOWS.md
├── 📄 GITHUB_PAGES_SETUP.md
└── 🔧 Begin P0.4 (Network Segmentation) implementation
```

---

## 💻 Backend Developer

### Must-Read Documentation (P0 Critical)

#### Architecture & Patterns
| Document | Path | Priority | Purpose |
|----------|------|----------|---------|
| **Copilot Instructions** | `.github/copilot-instructions.md` | 🔴 P0 | ALL patterns |
| **Wolverine Patterns** | `docs/WOLVERINE_HTTP_ENDPOINTS.md` | 🔴 P0 | HTTP handlers |
| **CQRS Pattern** | `docs/CQRS_WOLVERINE_PATTERN.md` | 🔴 P0 | CQRS implementation |
| **Onion Architecture** | `docs/ONION_ARCHITECTURE.md` | 🔴 P0 | Layer structure |
| **DDD Bounded Contexts** | `docs/architecture/DDD_BOUNDED_CONTEXTS.md` | 🔴 P0 | Domain separation |

#### API Development
| Document | Path | Priority |
|----------|------|----------|
| **Admin API Guide** | `docs/ADMIN_API_IMPLEMENTATION_GUIDE.md` | 🟡 P1 |
| **Gateway Separation** | `docs/GATEWAY_SEPARATION.md` | 🟡 P1 |
| **Model Binding** | `docs/MODEL_BINDING_QUICK_REFERENCE.md` | 🟡 P1 |
| **Controller Filters** | `docs/CONTROLLER_FILTER_REFACTORING.md` | 🟢 P2 |

#### Compliance Implementation
| Document | Path | Priority | P0 Component |
|----------|------|----------|--------------|
| **EU Compliance Roadmap** | `docs/EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md` | 🔴 P0 | P0.1, P0.6, P0.9 |
| **E-Commerce Legal Tests** | `docs/P0.6_ECOMMERCE_LEGAL_TESTS.md` | 🔴 P0 | P0.6 |
| **E-Rechnung Tests** | `docs/P0.9_ERECHNUNG_TESTS.md` | 🔴 P0 | P0.9 |
| **E-Commerce Overview** | `docs/ECOMMERCE_LEGAL_OVERVIEW.md` | 🟡 P1 | P0.6 |
| **Audit Logging** | `docs/AUDIT_LOGGING_IMPLEMENTATION.md` | 🔴 P0 | P0.1 |

#### Database & Persistence
| Document | Path | Priority |
|----------|------|----------|
| **Entity Extensions** | `docs/ENTITY_EXTENSIONS_IMPLEMENTATION.md` | 🟡 P1 |
| **Localization** | `docs/LOCALIZATION_IMPLEMENTATION_COMPLETE.md` | 🟡 P1 |

#### Testing
| Document | Path | Priority |
|----------|------|----------|
| **Testing Framework** | `docs/TESTING_FRAMEWORK_GUIDE.md` | 🔴 P0 |
| **Testing Guide** | `docs/guides/TESTING_GUIDE.md` | 🟡 P1 |
| **Compliance Testing** | `docs/COMPLIANCE_TESTING_EXAMPLES.md` | 🟡 P1 |

### Onboarding Path (Backend Developer)
```
Week 1: Core Patterns
├── 📄 copilot-instructions.md (FULL document!)
├── 📄 WOLVERINE_HTTP_ENDPOINTS.md
├── 📄 CQRS_WOLVERINE_PATTERN.md
├── 📄 ONION_ARCHITECTURE.md
└── 📄 architecture/DDD_BOUNDED_CONTEXTS.md

Week 2: API & Testing
├── 📄 ADMIN_API_IMPLEMENTATION_GUIDE.md
├── 📄 GATEWAY_SEPARATION.md
├── 📄 TESTING_FRAMEWORK_GUIDE.md
└── 📄 guides/TESTING_GUIDE.md

Week 3: Compliance Features
├── 📄 EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md (P0.1, P0.6, P0.9)
├── 📄 P0.6_ECOMMERCE_LEGAL_TESTS.md
├── 📄 P0.9_ERECHNUNG_TESTS.md
├── 📄 AUDIT_LOGGING_IMPLEMENTATION.md
└── 🔧 Begin P0.1 (Audit Logging) implementation
```

---

## 🎨 Frontend Developer

### Must-Read Documentation (P0 Critical)

#### Frontend Architecture
| Document | Path | Priority | Purpose |
|----------|------|----------|---------|
| **Frontend Feature Guide** | `docs/FRONTEND_FEATURE_INTEGRATION_GUIDE.md` | 🔴 P0 | Component patterns |
| **Admin Frontend Guide** | `docs/ADMIN_FRONTEND_FEATURE_INTEGRATION_GUIDE.md` | 🔴 P0 | Admin panel |
| **Frontend Tenant Setup** | `docs/FRONTEND_TENANT_SETUP.md` | 🔴 P0 | Multi-tenancy |
| **Aspire Frontend** | `docs/ASPIRE_FRONTEND_INTEGRATION.md` | 🟡 P1 | Service integration |

#### Accessibility (CRITICAL - P0.8)
| Document | Path | Priority | P0 Component |
|----------|------|----------|--------------|
| **BITV Tests** | `docs/P0.8_BARRIEREFREIHEIT_BITV_TESTS.md` | 🔴 P0 | P0.8 |
| **EU Compliance** | `docs/EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md` §P0.8 | 🔴 P0 | P0.8 |

#### E-Commerce UI
| Document | Path | Priority | P0 Component |
|----------|------|----------|--------------|
| **E-Commerce Legal** | `docs/P0.6_ECOMMERCE_LEGAL_TESTS.md` | 🔴 P0 | P0.6 |
| **E-Commerce Overview** | `docs/ECOMMERCE_LEGAL_OVERVIEW.md` | 🟡 P1 | P0.6 |

#### Localization
| Document | Path | Priority |
|----------|------|----------|
| **Localization Complete** | `docs/LOCALIZATION_IMPLEMENTATION_COMPLETE.md` | 🟡 P1 |
| **Localization Status** | `docs/LOCALIZATION_IMPLEMENTATION_STATUS.md` | 🟢 P2 |

### Onboarding Path (Frontend Developer)
```
Week 1: Frontend Foundation
├── 📄 FRONTEND_FEATURE_INTEGRATION_GUIDE.md
├── 📄 ADMIN_FRONTEND_FEATURE_INTEGRATION_GUIDE.md
├── 📄 FRONTEND_TENANT_SETUP.md
└── 📄 ASPIRE_FRONTEND_INTEGRATION.md

Week 2: Accessibility (P0.8 - CRITICAL!)
├── 📄 P0.8_BARRIEREFREIHEIT_BITV_TESTS.md (ALL 12 tests!)
├── 📄 EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md §P0.8
├── 🔧 Install axe DevTools, NVDA
└── 🔧 Run Lighthouse accessibility audit

Week 3: E-Commerce UI
├── 📄 P0.6_ECOMMERCE_LEGAL_TESTS.md (UI-relevant tests)
├── 📄 ECOMMERCE_LEGAL_OVERVIEW.md
├── 📄 LOCALIZATION_IMPLEMENTATION_COMPLETE.md
└── 🔧 Begin P0.8 (BITV) implementation
```

---

## 🧪 QA Engineer

### Must-Read Documentation (P0 Critical)

#### Testing Framework
| Document | Path | Priority | Purpose |
|----------|------|----------|---------|
| **Testing Framework** | `docs/TESTING_FRAMEWORK_GUIDE.md` | 🔴 P0 | Test patterns |
| **Testing Guide** | `docs/guides/TESTING_GUIDE.md` | 🔴 P0 | Test execution |
| **Debugging Guide** | `docs/guides/DEBUGGING_GUIDE.md` | 🟡 P1 | Issue debugging |
| **Debug Quick Ref** | `docs/guides/DEBUG_QUICK_REFERENCE.md` | 🟡 P1 | Quick lookup |

#### Compliance Test Specifications (ALL!)
| Document | Path | Priority | Tests |
|----------|------|----------|-------|
| **E-Commerce Tests** | `docs/P0.6_ECOMMERCE_LEGAL_TESTS.md` | 🔴 P0 | 15 tests |
| **AI Act Tests** | `docs/P0.7_AI_ACT_TESTS.md` | 🔴 P0 | 15 tests |
| **BITV Tests** | `docs/P0.8_BARRIEREFREIHEIT_BITV_TESTS.md` | 🔴 P0 | 12 tests |
| **E-Rechnung Tests** | `docs/P0.9_ERECHNUNG_TESTS.md` | 🔴 P0 | 10 tests |
| **Compliance Examples** | `docs/COMPLIANCE_TESTING_EXAMPLES.md` | 🔴 P0 | Patterns |

#### Verification
| Document | Path | Priority |
|----------|------|----------|
| **Verification Guide** | `docs/guides/VERIFICATION.md` | 🔴 P0 |
| **Review Summary** | `docs/REVIEW_COMPLETION_SUMMARY.md` | 🟡 P1 |

### Test Execution Matrix
| Component | Test File | Test Count | Automation |
|-----------|-----------|------------|------------|
| P0.6 E-Commerce | `P0.6_ECOMMERCE_LEGAL_TESTS.md` | 15 | xUnit |
| P0.7 AI Act | `P0.7_AI_ACT_TESTS.md` | 15 | xUnit |
| P0.8 BITV | `P0.8_BARRIEREFREIHEIT_BITV_TESTS.md` | 12 | Playwright + axe |
| P0.9 E-Rechnung | `P0.9_ERECHNUNG_TESTS.md` | 10 | xUnit |
| **TOTAL** | **4 files** | **52 tests** | Mixed |

### Onboarding Path (QA Engineer)
```
Week 1: Testing Foundation
├── 📄 TESTING_FRAMEWORK_GUIDE.md
├── 📄 guides/TESTING_GUIDE.md
├── 📄 guides/DEBUGGING_GUIDE.md
└── 📄 COMPLIANCE_TESTING_EXAMPLES.md

Week 2: Compliance Tests Setup
├── 📄 P0.6_ECOMMERCE_LEGAL_TESTS.md
├── 📄 P0.7_AI_ACT_TESTS.md
├── 📄 P0.8_BARRIEREFREIHEIT_BITV_TESTS.md
├── 📄 P0.9_ERECHNUNG_TESTS.md
└── 🔧 Create test automation scripts

Week 3: Execute & Verify
├── 📄 guides/VERIFICATION.md
├── 🔧 Run all 52 compliance tests
├── 🔧 Create test reports
└── 🔧 Identify gaps
```

---

## 📋 Product Owner

### Must-Read Documentation (Executive Level)

#### Strategic Documents
| Document | Path | Priority | Purpose |
|----------|------|----------|---------|
| **Application Specs** | `docs/APPLICATION_SPECIFICATIONS.md` | 🔴 P0 | Full requirements |
| **Business Requirements** | `docs/guides/BUSINESS_REQUIREMENTS.md` | 🔴 P0 | Business goals |
| **User Guide** | `docs/USER_GUIDE.md` | 🟡 P1 | User perspective |

#### Compliance Executive Summaries
| Document | Path | Priority | Purpose |
|----------|------|----------|---------|
| **AI Act Executive Summary** | `docs/AI_ACT_EXECUTIVE_SUMMARY.md` | 🔴 P0 | Leadership decision |
| **EU Compliance Roadmap** | `docs/EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md` | 🔴 P0 | Full roadmap |
| **Final Compliance Status** | `FINAL_COMPLIANCE_STATUS.md` | 🔴 P0 | Current status |

#### Development Status
| Document | Path | Priority |
|----------|------|----------|
| **Documentation Index** | `docs/DOCUMENTATION_INDEX.md` | 🟡 P1 |
| **Review Summary** | `docs/REVIEW_COMPLETION_SUMMARY.md` | 🟡 P1 |

### Decision Documents (Require PO Approval)
| Decision | Document | Impact |
|----------|----------|--------|
| Phase 0 Timeline (12 weeks) | `FINAL_COMPLIANCE_STATUS.md` | 6-week delay |
| Phase 0 Budget (€77K) | `FINAL_COMPLIANCE_STATUS.md` | +€27K |
| AI Act Compliance | `AI_ACT_EXECUTIVE_SUMMARY.md` | €30M risk mitigation |
| BITV Accessibility | `P0.8_BARRIEREFREIHEIT_BITV_TESTS.md` | €100K risk mitigation |

---

## ⚖️ Legal/Compliance Officer

### Must-Read Documentation (Regulatory)

#### EU Regulations
| Document | Path | Priority | Regulations |
|----------|------|----------|-------------|
| **EU Compliance Roadmap** | `docs/EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md` | 🔴 P0 | NIS2, GDPR, DORA, eIDAS, AI Act |
| **E-Commerce Legal** | `docs/ECOMMERCE_LEGAL_OVERVIEW.md` | 🔴 P0 | PAngV, VVVG, TMG |
| **AI Act Overview** | `docs/AI_ACT_OVERVIEW.md` | 🔴 P0 | EU AI Act |
| **Application Specs §Security** | `docs/APPLICATION_SPECIFICATIONS.md` | 🔴 P0 | Security requirements |

#### Test Specifications (Compliance Verification)
| Document | Path | Priority | Tests |
|----------|------|----------|-------|
| **E-Commerce Tests** | `docs/P0.6_ECOMMERCE_LEGAL_TESTS.md` | 🔴 P0 | Legal compliance |
| **AI Act Tests** | `docs/P0.7_AI_ACT_TESTS.md` | 🔴 P0 | AI governance |
| **BITV Tests** | `docs/P0.8_BARRIEREFREIHEIT_BITV_TESTS.md` | 🔴 P0 | Accessibility |
| **E-Rechnung Tests** | `docs/P0.9_ERECHNUNG_TESTS.md` | 🔴 P0 | E-Invoicing |

#### Risk Assessment
| Document | Path | Priority |
|----------|------|----------|
| **Pentester Review** | `docs/PENTESTER_REVIEW.md` | 🔴 P0 |
| **AI Act Executive Summary** | `docs/AI_ACT_EXECUTIVE_SUMMARY.md` | 🔴 P0 |

### Regulatory Checklist (Legal Review Required)
| Regulation | Document | Review Status |
|------------|----------|---------------|
| GDPR Art. 32 | `EU_SAAS_COMPLIANCE_...` §P0.2 | ⏳ Pending |
| NIS2 Art. 21-23 | `EU_SAAS_COMPLIANCE_...` §P0.1-P0.5 | ⏳ Pending |
| AI Act Art. 6, 22 | `P0.7_AI_ACT_TESTS.md` | ⏳ Pending |
| BITV 2.0 | `P0.8_BARRIEREFREIHEIT_BITV_TESTS.md` | ⏳ Pending |
| ERechnungsVO | `P0.9_ERECHNUNG_TESTS.md` | ⏳ Pending |
| PAngV, VVVG, TMG | `P0.6_ECOMMERCE_LEGAL_TESTS.md` | ⏳ Pending |

---

## 👔 Tech Lead / Architect

### Must-Read Documentation (ALL)

#### Architecture Foundation
| Document | Path | Priority |
|----------|------|----------|
| **Copilot Instructions** | `.github/copilot-instructions.md` | 🔴 P0 |
| **DDD Bounded Contexts** | `docs/architecture/DDD_BOUNDED_CONTEXTS.md` | 🔴 P0 |
| **Onion Architecture** | `docs/ONION_ARCHITECTURE.md` | 🔴 P0 |
| **Store Separation** | `docs/architecture/STORE_SEPARATION_STRUCTURE.md` | 🔴 P0 |
| **Architecture Restructuring** | `docs/architecture/ARCHITECTURE_RESTRUCTURING_PLAN.md` | 🟡 P1 |

#### Patterns & Standards
| Document | Path | Priority |
|----------|------|----------|
| **Wolverine HTTP** | `docs/WOLVERINE_HTTP_ENDPOINTS.md` | 🔴 P0 |
| **CQRS Pattern** | `docs/CQRS_WOLVERINE_PATTERN.md` | 🔴 P0 |
| **CQRS Quick Start** | `docs/CQRS_QUICKSTART.md` | 🟡 P1 |
| **Gateway Separation** | `docs/GATEWAY_SEPARATION.md` | 🟡 P1 |

#### Compliance (Review All)
| Document | Path | Priority |
|----------|------|----------|
| **EU Compliance Roadmap** | `docs/EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md` | 🔴 P0 |
| **All P0.x Test Files** | `docs/P0.*.md` | 🔴 P0 |
| **Application Specs** | `docs/APPLICATION_SPECIFICATIONS.md` | 🔴 P0 |

#### Code Quality
| Document | Path | Priority |
|----------|------|----------|
| **AI Development Guidelines** | `docs/AI_DEVELOPMENT_GUIDELINES.md` | 🔴 P0 |
| **Testing Framework** | `docs/TESTING_FRAMEWORK_GUIDE.md` | 🔴 P0 |
| **Pentester Review** | `docs/PENTESTER_REVIEW.md` | 🔴 P0 |

### Architecture Review Checklist
| Area | Documents | Review Status |
|------|-----------|---------------|
| Service Architecture | `DDD_BOUNDED_CONTEXTS.md`, `ONION_ARCHITECTURE.md` | ⏳ |
| API Patterns | `WOLVERINE_HTTP_ENDPOINTS.md`, `CQRS_*.md` | ⏳ |
| Security Architecture | `copilot-instructions.md §Security`, `PENTESTER_REVIEW.md` | ⏳ |
| Compliance Architecture | All `P0.*.md` files | ⏳ |
| Infrastructure | `ASPIRE_GUIDE.md`, `SERVICE_DISCOVERY.md` | ⏳ |

---

## 📊 Documentation Priority Matrix

### P0 Documents (Must Read Before Development)

| Document | Security | DevOps | Backend | Frontend | QA | PO | Legal | Architect |
|----------|:--------:|:------:|:-------:|:--------:|:--:|:--:|:-----:|:---------:|
| `copilot-instructions.md` | ✅ | ✅ | ✅ | ✅ | ✅ | - | - | ✅ |
| `EU_SAAS_COMPLIANCE_...` | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| `APPLICATION_SPECIFICATIONS.md` | ✅ | - | ✅ | - | ✅ | ✅ | ✅ | ✅ |
| `P0.6_ECOMMERCE_...` | - | - | ✅ | ✅ | ✅ | - | ✅ | ✅ |
| `P0.7_AI_ACT_...` | ✅ | - | ✅ | - | ✅ | - | ✅ | ✅ |
| `P0.8_BARRIEREFREIHEIT_...` | - | - | - | ✅ | ✅ | - | ✅ | ✅ |
| `P0.9_ERECHNUNG_...` | - | - | ✅ | - | ✅ | - | ✅ | ✅ |
| `WOLVERINE_HTTP_...` | - | - | ✅ | - | - | - | - | ✅ |
| `DDD_BOUNDED_CONTEXTS.md` | - | - | ✅ | - | - | - | - | ✅ |
| `ONION_ARCHITECTURE.md` | - | - | ✅ | - | - | - | - | ✅ |
| `ASPIRE_GUIDE.md` | - | ✅ | - | - | - | - | - | ✅ |
| `TESTING_FRAMEWORK_...` | - | - | ✅ | - | ✅ | - | - | ✅ |

---

## 🗂️ Proposed Folder Restructure

```
docs/
├── 📁 by-role/                         # Role-specific entry points
│   ├── SECURITY_ENGINEER.md
│   ├── DEVOPS_ENGINEER.md
│   ├── BACKEND_DEVELOPER.md
│   ├── FRONTEND_DEVELOPER.md
│   ├── QA_ENGINEER.md
│   ├── PRODUCT_OWNER.md
│   ├── LEGAL_COMPLIANCE.md
│   └── TECH_LEAD.md
│
├── 📁 compliance/                      # All compliance docs
│   ├── EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md
│   ├── P0.6_ECOMMERCE_LEGAL_TESTS.md
│   ├── P0.7_AI_ACT_TESTS.md
│   ├── P0.8_BARRIEREFREIHEIT_BITV_TESTS.md
│   ├── P0.9_ERECHNUNG_TESTS.md
│   ├── AI_ACT_OVERVIEW.md
│   ├── AI_ACT_EXECUTIVE_SUMMARY.md
│   ├── ECOMMERCE_LEGAL_OVERVIEW.md
│   ├── COMPLIANCE_QUICK_START_CHECKLIST.md
│   └── COMPLIANCE_TESTING_EXAMPLES.md
│
├── 📁 architecture/                    # (existing, keep)
│   ├── DDD_BOUNDED_CONTEXTS.md
│   ├── ONION_ARCHITECTURE.md
│   ├── ASPIRE_GUIDE.md
│   └── ...
│
├── 📁 guides/                          # (existing, keep)
│   ├── GETTING_STARTED.md
│   ├── TESTING_GUIDE.md
│   ├── DEBUGGING_GUIDE.md
│   └── ...
│
├── 📁 api/                             # API-specific docs
│   ├── WOLVERINE_HTTP_ENDPOINTS.md
│   ├── CQRS_WOLVERINE_PATTERN.md
│   ├── GATEWAY_SEPARATION.md
│   └── MODEL_BINDING_QUICK_REFERENCE.md
│
├── 📄 DOCUMENTATION_INDEX.md           # Master index
├── 📄 ROLE_BASED_DOCUMENTATION_MAP.md  # This file!
└── 📄 APPLICATION_SPECIFICATIONS.md    # Core specs
```

---

## 🎯 Quick Role Lookup

**I am a [ROLE]. Where do I start?**

| If you are... | Start with... | Then read... |
|---------------|---------------|--------------|
| **Security Engineer** | `copilot-instructions.md §Security` | `EU_SAAS_COMPLIANCE_...` P0.1-P0.5, P0.7 |
| **DevOps Engineer** | `architecture/ASPIRE_GUIDE.md` | `EU_SAAS_COMPLIANCE_...` P0.3-P0.5 |
| **Backend Developer** | `copilot-instructions.md` (FULL) | `WOLVERINE_HTTP_ENDPOINTS.md`, P0.6, P0.9 |
| **Frontend Developer** | `FRONTEND_FEATURE_INTEGRATION_GUIDE.md` | `P0.8_BARRIEREFREIHEIT_BITV_TESTS.md` |
| **QA Engineer** | `TESTING_FRAMEWORK_GUIDE.md` | All `P0.*.md` test files |
| **Product Owner** | `APPLICATION_SPECIFICATIONS.md` | `AI_ACT_EXECUTIVE_SUMMARY.md` |
| **Legal/Compliance** | `EU_SAAS_COMPLIANCE_...` | All `P0.*.md` + `PENTESTER_REVIEW.md` |
| **Tech Lead** | This document | Everything in P0 priority |

---

**Document Owner:** Architecture Team  
**Last Updated:** 28. Dezember 2025  
**Next Review:** 15. Januar 2026
