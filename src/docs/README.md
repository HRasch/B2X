# Backend Documentation

Guides und Referenzen für die Backend-Entwicklung.

## Inhalte

### Architektur & Patterns

**[AOP & FluentValidation Guide](AOP_FLUENT_VALIDATION_GUIDE.md)**
- AOP Filter für Cross-Cutting-Concerns
- Validation Filter mit FluentValidation
- Exception Handling
- Authorization Filter
- Praktische Beispiele

**[AOP & FluentValidation Quick Reference](AOP_FLUENT_VALIDATION_QUICK_REFERENCE.md)**
- Schnelle Übersicht der AOP Implementierung
- Code Snippets
- Häufige Patterns

### Weitere Guides

Siehe auch:
- [../../../docs/features/EVENT_VALIDATION_IMPLEMENTATION.md](../../../docs/features/EVENT_VALIDATION_IMPLEMENTATION.md) - Event System
- [../../../docs/features/ELASTICSEARCH_IMPLEMENTATION.md](../../../docs/features/ELASTICSEARCH_IMPLEMENTATION.md) - Search Integration
- [../../../docs/features/CATALOG_IMPLEMENTATION.md](../../../docs/features/CATALOG_IMPLEMENTATION.md) - Catalog Service

## Entwicklung

```bash
cd backend

# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Run tests
dotnet test

# Run with Aspire
cd services/AppHost
dotnet run
```

## Struktur

```
backend/
├── services/
│   ├── AppHost/              # Aspire orchestrator
│   ├── CatalogService/       # Main catalog API
│   ├── AuthService/
│   ├── SearchService/
│   └── OrderService/
├── shared/                   # Shared libraries
│   ├── aop/
│   ├── events/
│   ├── validators/
│   └── extensions/
├── Tests/                    # Test projects
├── infrastructure/           # IaC
└── docs/ (Sie sind hier)     # Documentation
```

## Tests

```bash
# All tests
dotnet test

# By category
dotnet test --filter "Catalog"
dotnet test --filter "Events"
dotnet test --filter "Validation"

# Watch mode
dotnet watch test
```

## Links

- [Backend README](../README.md) - Projektübersicht
- [Main Documentation](../../../docs/) - Alle Docs
- [Architecture Guides](../../../docs/architecture/)
- [Feature Implementations](../../../docs/features/)
