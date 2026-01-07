# Project Naming Convention Mapping

## Services - Layer-Based Naming
Format: `B2X.[BoundedContext].[Layer].csproj`

### Identity Service
- OLD: `B2X.AuthService.csproj`
- NEW: `B2X.Identity.API.csproj`
- Tests: `AuthService.Tests.csproj` → `B2X.Identity.Tests.csproj`

### Tenancy Service
- OLD: `B2X.TenantService.csproj`
- NEW: `B2X.Tenancy.API.csproj`

### Catalog Service
- OLD: `B2X.CatalogService.csproj`
- NEW: `B2X.Catalog.API.csproj`
- Tests: `CatalogService.Tests.csproj` → `B2X.Catalog.Tests.csproj`

### Theming Service (includes Theme + Layout)
- OLD: `B2X.ThemeService.csproj`
- NEW: `B2X.Theming.API.csproj`
- OLD: `B2X.LayoutService.csproj`
- NEW: `B2X.Theming.Layout.csproj` (or merge into Theming.API)

### Localization Service
- OLD: `B2X.LocalizationService.csproj`
- NEW: `B2X.Localization.API.csproj`
- Tests: `B2X.LocalizationService.Tests.csproj` → `B2X.Localization.Tests.csproj`

### API Gateway
- OLD: `B2X.ApiGateway.csproj`
- NEW: `B2X.Gateway.csproj`

### Orchestration (Aspire)
- OLD: `B2X.AppHost.csproj`
- NEW: `B2X.Orchestration.csproj`

### Service Defaults
- KEEP: `B2X.ServiceDefaults.csproj`

---

## Shared Libraries - Functional Naming
Format: `B2X.Shared.[Function].csproj`

### Core/Kernel
- OLD: `B2X.Types.csproj`
- NEW: `B2X.Shared.Kernel.csproj`

### Infrastructure/Data
- OLD: `B2X.Shared.Data.csproj`
- NEW: `B2X.Shared.Infrastructure.csproj`

### Keep As-Is (Good Names)
- ✓ `B2X.Shared.Core.csproj`
- ✓ `B2X.Shared.Search.csproj`
- ✓ `B2X.Shared.Messaging.csproj`
- ✓ `B2X.Shared.Middleware.csproj`

### Tools/Utilities
- OLD: `B2X.Utils.csproj`
- NEW: `B2X.Shared.Tools.csproj`
- OLD: `B2X.Shared.Extensions.csproj`
- NEW: `B2X.Shared.Tools.csproj` (merge or keep separate as B2X.Shared.Extensions.csproj)

### Validation (if separate)
- CREATE: `B2X.Shared.Validation.csproj` (for fluent validation rules)

### Tests
- OLD: `SearchService.Tests.csproj`
- NEW: `B2X.Shared.Search.Tests.csproj`

---

## Directory Renaming

### Services Directories
```
backend/services/
├── AppHost/                    → Orchestration/
├── auth-service/               → Identity/
├── tenant-service/             → Tenancy/
├── CatalogService/             → Catalog/
├── api-gateway/                → Gateway/
├── ThemeService/               → Theming/
└── LocalizationService/        → Localization/

Note: LayoutService could be merged into Theming/ or kept separate
```

### Shared Directories
```
backend/shared/
├── types/                      → kernel/
├── B2X.Shared.Data/      → B2X.Shared.Infrastructure/
├── utils/                      → tools/
└── B2X.Shared.Extensions/ → (merge with tools or keep as extensions/)
```

---

## Summary of Changes

### Project File Renames
- **11 main service/app projects** to be renamed
- **8 shared library projects** to be renamed
- **4 test projects** to be renamed
- **Total: 23 projects** affected

### Directory Renames
- **7 service directories** to be renamed
- **3-4 shared directories** to be renamed

---

## Implementation Order

1. **Update .slnx files** (references to projects)
2. **Rename .csproj files** (filesystem)
3. **Update AssemblyName** in .csproj files
4. **Update project references** between projects
5. **Verify namespaces** match project structure
6. **Test compilation** to ensure no breaking changes
