# Documentation Index

Quick navigation to all B2Connect documentation.

## 🚀 Start Here (Choose Your Path)

| Your Goal | Read This | Time |
|-----------|-----------|------|
| **I'm new to B2Connect** | [guides/GETTING_STARTED.md](guides/GETTING_STARTED.md) | 5 min |
| **I want to understand the project** | [README.md](../README.md) | 10 min |
| **I'm a developer** | [guides/DEVELOPMENT.md](guides/DEVELOPMENT.md) | 10 min |
| **I need to debug something** | [guides/DEBUG_QUICK_REFERENCE.md](guides/DEBUG_QUICK_REFERENCE.md) | 3 min |
| **I'm writing tests** | [guides/TESTING_GUIDE.md](guides/TESTING_GUIDE.md) | 15 min |
| **I'm implementing a feature** | Pick one in [features/](features/) | varies |

## 📂 Documentation Structure

### Root Level
- **[README.md](../README.md)** — Project overview & quick start
- **[B2Connect.slnx](../B2Connect.slnx)** — Solution file

### Architecture (`docs/architecture/`)
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
