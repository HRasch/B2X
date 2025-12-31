# Project Naming Convention Mapping

## Services - Layer-Based Naming
Format: `B2Connect.[BoundedContext].[Layer].csproj`

### Identity Service
- OLD: `B2Connect.AuthService.csproj`
- NEW: `B2Connect.Identity.API.csproj`
- Tests: `AuthService.Tests.csproj` → `B2Connect.Identity.Tests.csproj`

### Tenancy Service
- OLD: `B2Connect.TenantService.csproj`
- NEW: `B2Connect.Tenancy.API.csproj`

### Catalog Service
- OLD: `B2Connect.CatalogService.csproj`
- NEW: `B2Connect.Catalog.API.csproj`
- Tests: `CatalogService.Tests.csproj` → `B2Connect.Catalog.Tests.csproj`

### Theming Service (includes Theme + Layout)
- OLD: `B2Connect.ThemeService.csproj`
- NEW: `B2Connect.Theming.API.csproj`
- OLD: `B2Connect.LayoutService.csproj`
- NEW: `B2Connect.Theming.Layout.csproj` (or merge into Theming.API)

### Localization Service
- OLD: `B2Connect.LocalizationService.csproj`
- NEW: `B2Connect.Localization.API.csproj`
- Tests: `B2Connect.LocalizationService.Tests.csproj` → `B2Connect.Localization.Tests.csproj`

### API Gateway
- OLD: `B2Connect.ApiGateway.csproj`
- NEW: `B2Connect.Gateway.csproj`

### Orchestration (Aspire)
- OLD: `B2Connect.AppHost.csproj`
- NEW: `B2Connect.Orchestration.csproj`

### Service Defaults
- KEEP: `B2Connect.ServiceDefaults.csproj`

---

## Shared Libraries - Functional Naming
Format: `B2Connect.Shared.[Function].csproj`

### Core/Kernel
- OLD: `B2Connect.Types.csproj`
- NEW: `B2Connect.Shared.Kernel.csproj`

### Infrastructure/Data
- OLD: `B2Connect.Shared.Data.csproj`
- NEW: `B2Connect.Shared.Infrastructure.csproj`

### Keep As-Is (Good Names)
- ✓ `B2Connect.Shared.Core.csproj`
- ✓ `B2Connect.Shared.Search.csproj`
- ✓ `B2Connect.Shared.Messaging.csproj`
- ✓ `B2Connect.Shared.Middleware.csproj`

### Tools/Utilities
- OLD: `B2Connect.Utils.csproj`
- NEW: `B2Connect.Shared.Tools.csproj`
- OLD: `B2Connect.Shared.Extensions.csproj`
- NEW: `B2Connect.Shared.Tools.csproj` (merge or keep separate as B2Connect.Shared.Extensions.csproj)

### Validation (if separate)
- CREATE: `B2Connect.Shared.Validation.csproj` (for fluent validation rules)

### Tests
- OLD: `SearchService.Tests.csproj`
- NEW: `B2Connect.Shared.Search.Tests.csproj`

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
├── B2Connect.Shared.Data/      → B2Connect.Shared.Infrastructure/
├── utils/                      → tools/
└── B2Connect.Shared.Extensions/ → (merge with tools or keep as extensions/)
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
