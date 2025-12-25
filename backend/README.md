# B2Connect Backend

ASP.NET Core microservices platform with .NET Aspire orchestration.

## Quick Start

```bash
cd backend/services/AppHost
dotnet run
```

Open Aspire Dashboard: http://localhost:9000

## Project Structure

```
backend/
├── services/                    # Microservices
│   ├── AppHost/                # Aspire orchestrator
│   ├── CatalogService/         # Products, Categories, Brands
│   ├── AuthService/            # Authentication
│   ├── SearchService/          # Elasticsearch integration
│   └── OrderService/           # Orders management
├── shared/                      # Shared libraries
│   ├── aop/                    # Action filters
│   ├── events/                 # Domain events
│   ├── validators/             # FluentValidation
│   ├── extensions/             # DI extensions
│   ├── middleware/             # HTTP middleware
│   └── types/                  # Common types
├── Tests/                       # Test projects
│   ├── CatalogService.Tests/
│   ├── SearchService.Tests/
│   └── ...
├── infrastructure/              # IaC (Terraform, Kubernetes)
├── kubernetes/                  # K8s configs
├── docs/                        # Documentation
├── B2Connect.sln               # Solution file
└── Directory.Packages.props    # NuGet config
```

## Development

### Build
```bash
dotnet build
```

### Run Tests
```bash
dotnet test
dotnet test --filter Catalog
```

### Debug in VS Code
Press **F5** → Select "AppHost (Debug)"

## Documentation

- [../GETTING_STARTED.md](../GETTING_STARTED.md) - Setup guide
- [../DEVELOPMENT.md](../DEVELOPMENT.md) - Development workflow
- [docs/AOP_FLUENT_VALIDATION_GUIDE.md](docs/AOP_FLUENT_VALIDATION_GUIDE.md) - Validation patterns
- [../docs/features/](../docs/features/) - Feature implementations

## Services

| Service | Port | Type |
|---------|------|------|
| AppHost | 9000 | Aspire Dashboard |
| Catalog | 9001 | REST API |
| Auth | 9002 | REST API |
| Search | 9003 | REST API |
| Order | 9004 | REST API |

## Stack

- **.NET 10** with C# 13
- **ASP.NET Core** for APIs
- **Entity Framework Core** for data access
- **.NET Aspire** for orchestration
- **FluentValidation** for input validation
- **Elasticsearch** for search
- **xUnit** for testing

## Support

See [../docs/](../docs/) for detailed documentation.
