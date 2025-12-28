# Documentation Index

Quick navigation to all B2Connect documentation.

> **🎯 NEW!** See [../GOVERNANCE.md](../GOVERNANCE.md) for complete overview of requirements & governance documentation

## 🚀 Start Here (Choose Your Path)

| Your Goal | Read This | Time |
|-----------|-----------|------|
| **📚 User Documentation (GitHub Pages)** | [USER_GUIDE.md](USER_GUIDE.md) | 20 min |
| **🔐 Security Assessment (Pentester Review)** | [PENTESTER_REVIEW.md](PENTESTER_REVIEW.md) | 30 min |
| **👨‍💻 Technical Documentation (Developers)** | [SOFTWARE_DOCUMENTATION.md](SOFTWARE_DOCUMENTATION.md) | 25 min |
| **🚀 Deploy Documentation to GitHub Pages** | [GITHUB_PAGES_SETUP.md](GITHUB_PAGES_SETUP.md) | 15 min |
| **This week: P0 Critical Fixes** | [../CRITICAL_ISSUES_ROADMAP.md](../CRITICAL_ISSUES_ROADMAP.md) | 30 min |
| **All Requirements** | [../REQUIREMENTS_SUMMARY.md](../REQUIREMENTS_SUMMARY.md) | 15 min |
| **Setup & Specs** | [APPLICATION_SPECIFICATIONS.md](APPLICATION_SPECIFICATIONS.md) | 15 min |
| **GitHub Workflows** | [GITHUB_WORKFLOWS.md](GITHUB_WORKFLOWS.md) | 15 min |
| **Daily Standup Template** | [../DAILY_STANDUP_TEMPLATE.md](../DAILY_STANDUP_TEMPLATE.md) | 5 min |
| **Quick Start P0** | [../QUICK_START_P0.md](../QUICK_START_P0.md) | 5 min |
| **I'm new to B2Connect** | [guides/GETTING_STARTED.md](guides/GETTING_STARTED.md) | 5 min |
| **I want to understand the project** | [README.md](../README.md) | 10 min |
| **I'm a developer** | [guides/DEVELOPMENT.md](guides/DEVELOPMENT.md) | 10 min |
| **I need to debug something** | [guides/DEBUG_QUICK_REFERENCE.md](guides/DEBUG_QUICK_REFERENCE.md) | 3 min |
| **I'm writing tests** | [guides/TESTING_GUIDE.md](guides/TESTING_GUIDE.md) | 15 min |
| **I'm implementing a feature** | Pick one in [features/](features/) | varies |

## 📂 Documentation Structure

### Root Level (Critical Documents)
- **[GOVERNANCE.md](../GOVERNANCE.md)** — Documentation map & governance overview
- **[REQUIREMENTS_SUMMARY.md](../REQUIREMENTS_SUMMARY.md)** — All requirements for P0 week
- **[README.md](../README.md)** — Project overview & quick start
- **[B2Connect.slnx](../B2Connect.slnx)** — Solution file
- **[CRITICAL_ISSUES_ROADMAP.md](../CRITICAL_ISSUES_ROADMAP.md)** — P0 Week (30.12-03.01)
- **[SECURITY_HARDENING_GUIDE.md](../SECURITY_HARDENING_GUIDE.md)** — Security implementation
- **[DAILY_STANDUP_TEMPLATE.md](../DAILY_STANDUP_TEMPLATE.md)** — Daily team coordination
- **[QUICK_START_P0.md](../QUICK_START_P0.md)** — Quick start guide for P0 week
- **[.github/CONTRIBUTING.md](../.github/CONTRIBUTING.md)** — Contributing guidelines

### Documentation, User Guides & GitHub Pages
- **[USER_GUIDE.md](USER_GUIDE.md)** — Complete user documentation (Customers & Admins)
  - For store customers: shopping, orders, account management
  - For admins: products, customers, orders, settings, reports
- **[PENTESTER_REVIEW.md](PENTESTER_REVIEW.md)** — Security penetration testing assessment
  - Executive summary with CVSS scores
  - 5 CRITICAL vulnerabilities with exploitation scenarios
  - 8 HIGH severity findings
  - OWASP Top 10 mapping
  - Manual testing checklist
- **[SOFTWARE_DOCUMENTATION.md](SOFTWARE_DOCUMENTATION.md)** — Technical documentation for developers
  - Architecture & API specifications
  - Database schema with ERD
  - JWT & authentication patterns
  - RBAC & tenant isolation
  - Deployment guides (Docker, Kubernetes)
  - Testing patterns & examples
  - Troubleshooting guide
- **[GITHUB_PAGES_SETUP.md](GITHUB_PAGES_SETUP.md)** — Deploy documentation to GitHub Pages
  - Step-by-step setup guide
  - GitHub Pages configuration
  - CI/CD pipeline for auto-deployment
  - Custom domain setup
  - Troubleshooting
- **[AI_DEVELOPMENT_GUIDELINES.md](AI_DEVELOPMENT_GUIDELINES.md)** — KI-Assistenten Richtlinien (NEW)
  - KI-Integration Prinzipien
  - Security Checklisten & Templates
  - Architektur-Anforderungen
  - Prompt-Bibliothek mit Templates
  - Code-Review Checklisten
  - Common Mistakes & Best Practices
  - Extracted from all reviews for future AI use

### Specifications & Standards
- **[APPLICATION_SPECIFICATIONS.md](APPLICATION_SPECIFICATIONS.md)** — Complete system specs
  - Core & security requirements
  - Data & API specifications
  - Database schema (P0.3, P0.4)
  - Audit & compliance requirements
  - Performance & deployment specs

- **[GITHUB_WORKFLOWS.md](GITHUB_WORKFLOWS.md)** — Development workflows
  - GitHub project management
  - Branch strategy & naming
  - Commit strategy & conventions
  - Pull request workflow
  - Code review process
  - Release management
  - CI/CD pipelines
- **[ONION_ARCHITECTURE.md](ONION_ARCHITECTURE.md)** — Architekturprinzipien
- **[GATEWAY_SEPARATION.md](GATEWAY_SEPARATION.md)** — Gateway-Trennung
- **[STORE_SEPARATION_STRUCTURE.md](architecture/STORE_SEPARATION_STRUCTURE.md)** — Store-Architektur
- **[STRUCTURE_SEPARATION_STATUS.md](architecture/STRUCTURE_SEPARATION_STATUS.md)** — Trennungsstatus

### Features (`docs/features/`)
- **[CATALOG_IMPLEMENTATION.md](features/CATALOG_IMPLEMENTATION.md)** — Product catalog
- **[AOP_VALIDATION_IMPLEMENTATION.md](features/AOP_VALIDATION_IMPLEMENTATION.md)** — Input validation
- **[EVENT_VALIDATION_IMPLEMENTATION.md](features/EVENT_VALIDATION_IMPLEMENTATION.md)** — Event validation
- **[ELASTICSEARCH_IMPLEMENTATION.md](features/ELASTICSEARCH_IMPLEMENTATION.md)** — Full-text search
- **[LOCALIZATION_IMPLEMENTATION.md](features/LOCALIZATION_IMPLEMENTATION.md)** — i18n & languages
- **[LOCALIZATION_ENTITY_ANALYSIS.md](features/LOCALIZATION_ENTITY_ANALYSIS.md)** — Lokalisierungsanalyse
- **[ADMIN_FRONTEND_IMPLEMENTATION.md](features/ADMIN_FRONTEND_IMPLEMENTATION.md)** — Admin UI
- **[CQRS_E2E_TESTS_SUMMARY.md](features/CQRS_E2E_TESTS_SUMMARY.md)** — CQRS E2E Tests
- **[CQRS_INTEGRATION_POINT1.md](features/CQRS_INTEGRATION_POINT1.md)** — CQRS Integration
- **[CQRS_TODOS_COMPLETED.md](features/CQRS_TODOS_COMPLETED.md)** — CQRS Status
- **[STORE_READ_SERVICES_COMPLETION.md](features/STORE_READ_SERVICES_COMPLETION.md)** — Store Read Services

### Guides (`docs/guides/`)
- **[GETTING_STARTED.md](guides/GETTING_STARTED.md)** — Erste Schritte
- **[DEVELOPMENT.md](guides/DEVELOPMENT.md)** — Entwicklungsworkflow
- **[BUSINESS_REQUIREMENTS.md](guides/BUSINESS_REQUIREMENTS.md)** — Business-Anforderungen
- **[DEBUG_QUICK_REFERENCE.md](guides/DEBUG_QUICK_REFERENCE.md)** — Debugging quick tips
- **[TESTING_GUIDE.md](guides/TESTING_GUIDE.md)** — Testing approach
- **[VERIFICATION.md](guides/VERIFICATION.md)** — How to verify implementations
- **[BASH_MODERNIZATION_COMPLETED.md](guides/BASH_MODERNIZATION_COMPLETED.md)** — Bash scripts
- **[PROJECT_NAMING_MAPPING.md](guides/PROJECT_NAMING_MAPPING.md)** — Naming conventions

## 🔍 Quick Links by Topic

**User Guides & Documentation** → [USER_GUIDE.md](USER_GUIDE.md), [GITHUB_PAGES_SETUP.md](GITHUB_PAGES_SETUP.md)

**Compliance & AI Act** → [EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md](EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md), [AI_ACT_OVERVIEW.md](AI_ACT_OVERVIEW.md), [P0.7_AI_ACT_TESTS.md](P0.7_AI_ACT_TESTS.md), [AI_ACT_INTEGRATION_COMPLETE.md](AI_ACT_INTEGRATION_COMPLETE.md), [AI_ACT_EXECUTIVE_SUMMARY.md](AI_ACT_EXECUTIVE_SUMMARY.md)

**Security & Pentesting** → [PENTESTER_REVIEW.md](PENTESTER_REVIEW.md), [../SECURITY_HARDENING_GUIDE.md](../SECURITY_HARDENING_GUIDE.md)

**Technical Documentation** → [SOFTWARE_DOCUMENTATION.md](SOFTWARE_DOCUMENTATION.md), [APPLICATION_SPECIFICATIONS.md](APPLICATION_SPECIFICATIONS.md)

**This Week (P0)** → [../CRITICAL_ISSUES_ROADMAP.md](../CRITICAL_ISSUES_ROADMAP.md), [../QUICK_START_P0.md](../QUICK_START_P0.md), [../REQUIREMENTS_SUMMARY.md](../REQUIREMENTS_SUMMARY.md)

**Development Workflows** → [GITHUB_WORKFLOWS.md](GITHUB_WORKFLOWS.md), [.github/pull_request_template.md](../.github/pull_request_template.md)

**Specs & Standards** → [APPLICATION_SPECIFICATIONS.md](APPLICATION_SPECIFICATIONS.md), [REQUIREMENTS_SUMMARY.md](../REQUIREMENTS_SUMMARY.md)

**Setup & Architecture** → [guides/GETTING_STARTED.md](guides/GETTING_STARTED.md), [ONION_ARCHITECTURE.md](ONION_ARCHITECTURE.md)

**Backend Coding** → [features/AOP_VALIDATION_IMPLEMENTATION.md](features/AOP_VALIDATION_IMPLEMENTATION.md), [features/EVENT_VALIDATION_IMPLEMENTATION.md](features/EVENT_VALIDATION_IMPLEMENTATION.md)

**Frontend Coding** → [features/ADMIN_FRONTEND_IMPLEMENTATION.md](features/ADMIN_FRONTEND_IMPLEMENTATION.md), [features/LOCALIZATION_IMPLEMENTATION.md](features/LOCALIZATION_IMPLEMENTATION.md)

**Testing** → [guides/TESTING_GUIDE.md](guides/TESTING_GUIDE.md)

**Search** → [features/ELASTICSEARCH_IMPLEMENTATION.md](features/ELASTICSEARCH_IMPLEMENTATION.md)

**Debugging** → [guides/DEBUG_QUICK_REFERENCE.md](guides/DEBUG_QUICK_REFERENCE.md)

**CQRS** → [features/CQRS_E2E_TESTS_SUMMARY.md](features/CQRS_E2E_TESTS_SUMMARY.md), [features/CQRS_INTEGRATION_POINT1.md](features/CQRS_INTEGRATION_POINT1.md)

## 📋 Full Standards

See [.copilot-specs.md](../.copilot-specs.md) for complete development standards (24 sections):
- Frontend architecture (Vue 3, Pinia, Vite)
- TDD & testing philosophy
- Error handling & Result pattern
- Type safety & security
- Performance & optimization
- API & module design
- Wolverine messaging
- Microservices patterns
- AOP & FluentValidation
- Event validation
- Documentation rules
- Bash script standards

## 📊 Project Status

✅ Backend: 65/65 tests | ✅ Frontend: Vue 3 + Pinia | ✅ Infrastructure: K8s ready

## 🐛 Troubleshooting

| Issue | Solution |
|-------|----------|
| Port in use | [DEBUG_QUICK_REFERENCE.md](guides/DEBUG_QUICK_REFERENCE.md) |
| Tests failing | [TESTING_GUIDE.md](guides/TESTING_GUIDE.md) |
| Frontend not loading | [DEVELOPMENT.md](../DEVELOPMENT.md) |

---

**Note:** Historical docs are in [DOCS_ARCHIVE/](../../DOCS_ARCHIVE/)
