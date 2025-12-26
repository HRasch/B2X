# Documentation Index

Quick navigation to all B2Connect documentation.

## 🚀 Start Here (Choose Your Path)

| Your Goal | Read This | Time |
|-----------|-----------|------|
| **I'm new to B2Connect** | [GETTING_STARTED.md](../GETTING_STARTED.md) | 5 min |
| **I want to understand the project** | [README.md](../README.md) | 10 min |
| **I'm a developer** | [DEVELOPMENT.md](../DEVELOPMENT.md) | 10 min |
| **I need to debug something** | [guides/DEBUG_QUICK_REFERENCE.md](guides/DEBUG_QUICK_REFERENCE.md) | 3 min |
| **I'm writing tests** | [guides/TESTING_GUIDE.md](guides/TESTING_GUIDE.md) | 15 min |
| **I'm implementing a feature** | Pick one in [features/](features/) | varies |

## 📂 Documentation Structure

### Root Level (4 Essential Files)
- **[README.md](../README.md)** — Project overview & architecture
- **[GETTING_STARTED.md](../GETTING_STARTED.md)** — First-time setup (5 min)
- **[DEVELOPMENT.md](../DEVELOPMENT.md)** — Development workflow & guidelines
- **[BUSINESS_REQUIREMENTS.md](../BUSINESS_REQUIREMENTS.md)** — Features & roadmap

### Architecture (`docs/architecture/`)
- **[ASPIRE_GUIDE.md](architecture/ASPIRE_GUIDE.md)** — Microservices orchestration
- **[VSCODE_ASPIRE_CONFIG.md](architecture/VSCODE_ASPIRE_CONFIG.md)** — VS Code debug config
- **[ARCHITECTURE_RESTRUCTURING_PLAN.md](architecture/ARCHITECTURE_RESTRUCTURING_PLAN.md)** — Design decisions

### Features (`docs/features/`)
- **[CATALOG_IMPLEMENTATION.md](features/CATALOG_IMPLEMENTATION.md)** — Product catalog
- **[AOP_VALIDATION_IMPLEMENTATION.md](features/AOP_VALIDATION_IMPLEMENTATION.md)** — Input validation
- **[EVENT_VALIDATION_IMPLEMENTATION.md](features/EVENT_VALIDATION_IMPLEMENTATION.md)** — Event validation
- **[ELASTICSEARCH_IMPLEMENTATION.md](features/ELASTICSEARCH_IMPLEMENTATION.md)** — Full-text search
- **[LOCALIZATION_IMPLEMENTATION.md](features/LOCALIZATION_IMPLEMENTATION.md)** — i18n & languages
- **[ADMIN_FRONTEND_IMPLEMENTATION.md](features/ADMIN_FRONTEND_IMPLEMENTATION.md)** — Admin UI

### Guides (`docs/guides/`)
- **[DEBUG_QUICK_REFERENCE.md](guides/DEBUG_QUICK_REFERENCE.md)** — Debugging quick tips
- **[TESTING_GUIDE.md](guides/TESTING_GUIDE.md)** — Testing approach
- **[VERIFICATION.md](guides/VERIFICATION.md)** — How to verify implementations
- **[BASH_MODERNIZATION_COMPLETED.md](guides/BASH_MODERNIZATION_COMPLETED.md)** — Bash scripts
- **[PROJECT_NAMING_MAPPING.md](guides/PROJECT_NAMING_MAPPING.md)** — Naming conventions

## 🔍 Quick Links by Topic

**Setup & Architecture** → [GETTING_STARTED.md](../GETTING_STARTED.md), [ASPIRE_GUIDE.md](architecture/ASPIRE_GUIDE.md)

**Backend Coding** → [AOP_VALIDATION_IMPLEMENTATION.md](features/AOP_VALIDATION_IMPLEMENTATION.md), [EVENT_VALIDATION_IMPLEMENTATION.md](features/EVENT_VALIDATION_IMPLEMENTATION.md)

**Frontend Coding** → [ADMIN_FRONTEND_IMPLEMENTATION.md](features/ADMIN_FRONTEND_IMPLEMENTATION.md), [LOCALIZATION_IMPLEMENTATION.md](features/LOCALIZATION_IMPLEMENTATION.md)

**Testing** → [TESTING_GUIDE.md](guides/TESTING_GUIDE.md)

**Search** → [ELASTICSEARCH_IMPLEMENTATION.md](features/ELASTICSEARCH_IMPLEMENTATION.md)

**Debugging** → [DEBUG_QUICK_REFERENCE.md](guides/DEBUG_QUICK_REFERENCE.md)

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
