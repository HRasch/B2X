---
docid: KB-LESSONS
title: Lessons Learned
owner: @DocMaintainer
status: Active
created: 2026-01-08
updated: 2026-01-12
---

# Lessons Learned

**DocID**: `KB-LESSONS`  
**Last Updated**: 12. Januar 2026  
**Maintained By**: GitHub Copilot  
**Size**: Optimized per [GL-007] - Target <1000 lines

---

## Table of Contents

- [.NET Core & Aspire](#net-core--aspire)
- [Entity Framework Core](#entity-framework-core)
- [Build & Compilation](#build--compilation)
- [Testing & Quality](#testing--quality)
- [Type Systems & Language Features](#type-systems--language-features)
- [Development Workflow](#development-workflow)
- [API Design & Serialization](#api-design--serialization)

---

## .NET Core & Aspire

### Aspire CLI Requires Interactive Terminal (stdin Redirection Issue)

**Problem**: Aspire starts successfully but exits immediately with code 1 in VS Code terminal.

**Root Cause**: VS Code's integrated terminal redirects stdin, causing Aspire CLI to detect non-interactive environment via `Console.IsInputRedirected`.

**Solution**: Run Aspire in separate PowerShell window using `Start-Process`:

```jsonc
// .vscode/tasks.json
{
  "label": "backend-start",
  "type": "shell",
  "isBackground": true,
  "command": "Start-Process",
  "args": [
    "pwsh",
    "-ArgumentList",
    "'-NoExit', '-Command', 'cd ''${workspaceFolder}/src/backend/Infrastructure/Hosting/AppHost''; $env:Database__Provider=''inmemory''; aspire run --project B2X.AppHost.csproj'",
  ],
}
```

**Key Insights**:

- VS Code terminals redirect stdin - affects any CLI checking interactivity
- `Start-Process` spawns true interactive window with proper stdin
- `-NoExit` flag keeps window open for Ctrl+C shutdown
- Build success ≠ Runtime success

**Reference**: [dotnet/aspire#12242](https://github.com/dotnet/aspire/pull/12242)

---

### Aspire 13.1.0 on .NET 10 - Runtime Assembly Loading

**Problem**: Aspire compiles but fails at runtime with `FileNotFoundException` for `Aspire.Hosting` assembly.

**Root Cause**: Aspire 13.1.0 ships only `net8.0` assemblies, not `net10.0`. NuGet restores packages but doesn't copy assemblies for mismatched target frameworks.

**Solution**: Add fallback configuration to `B2X.AppHost.csproj`:

```xml
<PropertyGroup>
  <!-- Allow NuGet to accept net8.0/net9.0 as fallback -->
  <AssetTargetFallback>$(AssetTargetFallback);net9.0;net8.0</AssetTargetFallback>

  <!-- Allow runtime to load older assemblies -->
  <RollForward>LatestMajor</RollForward>

  <!-- Force copy ALL transitive dependencies -->
  <CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>

  <!-- Enable Aspire host behaviors -->
  <IsAspireHost>true</IsAspireHost>
</PropertyGroup>
```

**All Four Properties Required**:

- `AssetTargetFallback`: NuGet copies net8.0 assets
- `RollForward`: Runtime accepts older assemblies
- `CopyLocalLockFileAssemblies`: Transitive deps in output folder
- `IsAspireHost`: Aspire MSBuild behaviors

**Reference**: See [KB-070] for complete configuration

---

### Aspire Workload Deprecation - SDK 13.1.0 Migration

**Problem**: Build fails with `NETSDK1228` - workload-based Aspire installation no longer supported.

**Root Cause**: Aspire 8.x workload model deprecated. Aspire 13.x uses SDK-based approach.

**Solution**: Upgrade to SDK format:

```xml
<!-- OLD: Aspire 8.x -->
<Project Sdk="Microsoft.NET.Sdk">
  <Sdk Name="Aspire.AppHost.Sdk" Version="8.2.0-preview.1.24427.3" />
  <PropertyGroup>
    <IsAspireHost>true</IsAspireHost>
  </PropertyGroup>
</Project>

<!-- NEW: Aspire 13.x -->
<Project Sdk="Aspire.AppHost.Sdk/13.1.0">
  <PropertyGroup>
    <!-- No IsAspireHost needed -->
  </PropertyGroup>
</Project>
```

**Key Changes**:

- No separate `<Sdk Name="..." />` element
- SDK version in `Sdk` attribute: `Aspire.AppHost.Sdk/13.1.0`
- Remove `IsAspireHost` property
- Error cannot be suppressed with `NoWarn`

---

## Entity Framework Core

### In-Memory Provider Doesn't Support Migrations

**Problem**: `context.Database.MigrateAsync()` throws `InvalidOperationException` with in-memory provider.

**Root Cause**: Migrations are relational-only operations. In-memory provider has no schema to migrate.

**Solution**: Conditional migration based on provider type:

```csharp
var dbProvider = builder.Configuration["Database:Provider"] ?? "postgres";

if (dbProvider != "inmemory")
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
    await context.Database.MigrateAsync();
}
else
{
    // For in-memory testing
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
    await context.Database.EnsureCreatedAsync();
}
```

**Provider Support Matrix**:
| Method | In-Memory | SQLite | PostgreSQL |
|--------|-----------|--------|------------|
| `MigrateAsync()` | Error | Works | Works |
| `EnsureCreatedAsync()` | Works | Works | Works |

**Key Insights**:

- In-memory is dictionaries, not a database
- Always check provider type before relational methods
- Use `EnsureCreatedAsync()` for testing
- Configuration-driven providers enable flexible testing

---

### JSON Circular References in Navigation Properties

**Problem**: POST API returns HTTP 500 with `JsonException` about circular references:

```
System.Text.Json.JsonException: A possible object cycle was detected.
Path: $.Items.Order.Items.Order.Items.Order...
```

**Root Cause**: Bidirectional navigation properties create cycles:

```csharp
public class Order
{
    public ICollection<OrderItem> Items { get; set; }
}

public class OrderItem
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; }  // Back-reference causes cycle
}
```

**Solution**: Configure `ReferenceHandler.IgnoreCycles`:

```csharp
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });
```

**Alternative Solutions**:
| Option | Pros | Cons |
|--------|------|------|
| `ReferenceHandler.IgnoreCycles` | Simple, no code changes | Silently ignores cycles |
| `ReferenceHandler.Preserve` | Preserves with `$id`/`$ref` | Adds metadata to JSON |
| `[JsonIgnore]` on navigation | Explicit control | Every back-reference |
| DTOs without navigation | Clean separation | More code |

**Best Practice**: Use DTOs for API responses instead of EF entities directly.

---

## Build & Compilation

### VBCSCompiler/MSBuild File Locks (CS2012)

**Problem**: Build fails with "Cannot open 'B2X.Shared.Core.dll' for writing - file is being used by another process."

**Root Cause**: VBCSCompiler shared compilation and MSBuild node reuse keep processes alive that hold file locks.

**Solution**: Add to `Directory.Build.props`:

```xml
<PropertyGroup>
  <!-- Prevent File Locking Issues -->
  <UseSharedCompilation>false</UseSharedCompilation>
  <DisableMSBuildNodeReuse>true</DisableMSBuildNodeReuse>
  <RunAnalyzersDuringBuild>true</RunAnalyzersDuringBuild>
  <RunAnalyzersDuringLiveAnalysis>false</RunAnalyzersDuringLiveAnalysis>
</PropertyGroup>
```

**Emergency Cleanup**:

```powershell
Get-Process -Name "dotnet", "msbuild", "VBCSCompiler", "csc" -ErrorAction SilentlyContinue | Stop-Process -Force
Remove-Item "path/to/project/obj" -Recurse -Force
```

**Key Insights**:

- VBCSCompiler designed for build speed but causes locks
- MSBuild node reuse problematic in large solutions
- Analyzers can lock files during live analysis
- Different issue from Aspire stdin redirection

---

### Central Package Management (CPM) Missing Versions

**Problem**: Build fails with `NU1008` - "Projects using central package version management should not define version on PackageReference."

**Root Cause**: Projects referenced packages without versions in `Directory.Packages.props`.

**Solution**: Add missing package versions to CPM:

```xml
<!-- Directory.Packages.props -->
<ItemGroup>
  <!-- Architecture Testing -->
  <PackageVersion Include="TngTech.ArchUnitNET" Version="0.13.1" />

  <!-- Build Infrastructure -->
  <PackageVersion Include="Microsoft.Build.Tasks.Core" Version="17.14.8" />

  <!-- MEF Composition -->
  <PackageVersion Include="System.ComponentModel.Composition" Version="10.0.1" />
</ItemGroup>
```

**Prevention**:

- Add `NU1008` to CI as blocking error (default)
- Document CPM process in `CONTRIBUTING.md`
- Use `dotnet list package --outdated` to audit

**Key Lessons**:

- CPM is solution-wide: Every `<PackageReference>` needs `<PackageVersion>`
- New projects inherit CPM requirements
- Transitive dependencies may need explicit versions
- Consider version audit script

---

### CopyLocalLockFileAssemblies for NuGet Dependencies

**Problem**: Aspire-orchestrated projects fail at runtime with `FileNotFoundException` for NuGet package DLLs.

**Root Cause**: NuGet default only copies direct project references, not transitive package dependencies.

**Solution**: Add to `src/Directory.Build.props`:

```xml
<PropertyGroup>
  <!-- Force copy ALL NuGet package DLLs to output directory -->
  <CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>
</PropertyGroup>
```

**What This Does**:
| Before | After |
|--------|-------|
| Only project DLLs copied | All DLLs from lock file copied |
| Runtime resolution via NuGet cache | All deps in bin/ folder |
| Smaller output but fragile | Larger but self-contained |

**When to Use**:

- Aspire AppHost projects
- Plugin/extension architectures
- Container deployments needing all DLLs
- xcopy-deployable applications
- NOT library projects (consumers manage deps)

---

### .NET Solution Path Corrections & Dependencies

**Problem**: Widespread compilation failures due to incorrect project reference paths and missing package dependencies.

**Root Cause**: Inconsistent relative path patterns (missing `/src/` prefixes) and incomplete package references for EF, logging, configuration.

**Solution**: Systematic fixes:

1. **Path Pattern**: Standardize 5-level navigation for test projects:

   ```xml
   <!-- Correct -->
   <ProjectReference Include="../../../../src/shared/Domain/Tenancy/B2X.Tenancy.API.csproj" />

   <!-- Incorrect - missing /src/ -->
   <ProjectReference Include="../../../../shared/Domain/Tenancy/B2X.Tenancy.API.csproj" />
   ```

2. **Package Completeness**: Add both runtime and design-time packages:

   ```xml
   <!-- EF Core -->
   <PackageReference Include="Microsoft.EntityFrameworkCore" />
   <PackageReference Include="Microsoft.EntityFrameworkCore.Relational" />
   <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" />

   <!-- Logging -->
   <PackageReference Include="Microsoft.Extensions.Logging" />
   ```

**Key Insights**:

- Path depth matters: Test projects in `tests/` require 5 levels up
- EF needs both runtime and relational packages
- `ILogger<>` requires explicit `Microsoft.Extensions.Logging`
- Build individual projects to isolate issues

**Prevention**:

- Path validation scripts for consistent `/src/` prefixes
- Package dependency templates for common project types
- Require successful individual builds before solution-wide validation

---

## Testing & Quality

### xUnit v3 MTP v2 Migration & Test Suite Verification

**Problem**: Verify xUnit v3 with Microsoft Testing Platform v2 works across entire test suite after migration.

**Root Cause**: xUnit v3 MTP v2 is significant change from traditional xUnit - requires comprehensive verification.

**Solution**: Systematic verification showed 433 tests passing across 19 test projects with MTP v2.

**Required Configuration**:

```xml
<PropertyGroup>
  <!-- Required for MTP v2 test execution -->
  <UseMicrosoftTestingPlatformRunner>true</UseMicrosoftTestingPlatformRunner>
  <CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>
  <OutputType>Exe</OutputType>
</PropertyGroup>

<ItemGroup>
  <!-- Use only MTP v2 package, not metapackage -->
  <PackageReference Include="xunit.v3.mtp-v2" />
  <PackageReference Include="Microsoft.NET.Test.Sdk" />
</ItemGroup>
```

**Key Lessons**:

- Reference only `xunit.v3.mtp-v2`, not metapackage (avoids conflicts)
- MTP v2 requires `UseMicrosoftTestingPlatformRunner` and `CopyLocalLockFileAssemblies`
- For IDisposable fixtures, add `GC.SuppressFinalize(this)` in Dispose (CA1816)
- Suppress CS1591 for documentation warnings in test projects
- Mock implementations need realistic data matching production expectations
- All properly configured projects work seamlessly with MTP v2

---

## Type Systems & Language Features

### Roslyn Type Inference Requires Explicit Cast for IEnumerable Covariance

**Problem**: WolverineMCP build fails with `CS1503` - cannot convert `IEnumerable<ISymbol>` to `IEnumerable<INamedTypeSymbol>`.

**Root Cause**: C# covariance doesn't apply when types aren't directly compatible. Method returns `IEnumerable<ISymbol>` but assigned to `IEnumerable<INamedTypeSymbol>`.

**Solution**: Add explicit `.Cast<T>()`:

```csharp
// Broken - type inference fails
IEnumerable<INamedTypeSymbol> commands = symbolFinder.FindCommands(types);

// Fixed - explicit Cast
var commandSymbols = symbolFinder.FindCommands(types).Cast<INamedTypeSymbol>();
```

**Key Lessons**:

- `IEnumerable<Derived>` is NOT `IEnumerable<Base>` in method returns
- Roslyn symbols often return `ISymbol` needing explicit casting
- Use `.Cast<T>()` from `System.Linq` for safe conversion
- Prefer `var` when adding `.Cast<T>()` to avoid redundancy

---

## Development Workflow

### PowerShell Background Jobs for API Testing

**Problem**: Running `dotnet run` then `curl` in same VS Code terminal terminates server after HTTP request.

**Root Cause**: VS Code terminal executes commands sequentially. `dotnet run` blocks, requiring termination for next command.

**Solution**: Use PowerShell `Start-Job` for background execution:

```powershell
# Start server as background job
Start-Job -ScriptBlock {
    cd "c:\path\to\project"
    $env:Database__Provider = "inmemory"
    dotnet run --project Api.csproj --urls "http://localhost:5001"
} -Name ApiServer

# Wait for startup
Start-Sleep -Seconds 15

# Test API
Invoke-WebRequest -Uri "http://localhost:5001/api/v1/orders" `
    -Headers @{"X-Tenant-ID"="12345678-1234-1234-1234-123456789012"}

# Cleanup
Stop-Job -Name ApiServer
Remove-Job -Name ApiServer
```

**Job Management**:

```powershell
Get-Job -Name ApiServer           # Check status
Receive-Job -Name ApiServer -Keep # View output (keep in buffer)
Stop-Job -Name ApiServer          # Stop
Remove-Job -Name ApiServer        # Clean up
```

**Key Lessons**:

- PowerShell jobs are stateless - set env vars inside script block
- Allow 10-15 seconds startup for .NET + EF Core + Wolverine
- Always clean up jobs - prevent orphaned processes
- Check job output on failure with `Receive-Job -Keep`

---

## API Design & Serialization

### System.Text.Json vs Newtonsoft.Json Defaults

**Key Difference**: System.Text.Json has NO default cycle handling, unlike Newtonsoft.Json which handles cycles automatically.

**When Migrating**: Always configure `ReferenceHandler` explicitly:

```csharp
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // System.Text.Json requires explicit cycle handling
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
```

**Best Practice**: Use DTOs for API boundaries to avoid exposing navigation properties entirely.

---

---

## Frontend Development & Testing

### Nuxt 3 Dev Server Host Binding

**Problem**: Built Nuxt servers only accessible locally, preventing Playwright test connectivity.

**Root Cause**: Default localhost binding prevents external connections from test runners.

**Solution**: Bind to all interfaces for test accessibility:

```bash
# Development
HOST=0.0.0.0 PORT=3000 npm run dev

# Built server
HOST=0.0.0.0 PORT=3000 node .output/server/index.mjs
```

**Playwright Configuration**:

```typescript
export default defineConfig({
  use: {
    baseURL: 'http://localhost:3000', // Must match server binding
  },
});
```

**Key Insights**:

- Default localhost binding blocks containerized/external test runners
- Always use `0.0.0.0` for testing environments
- Validate connectivity with `curl` before running tests

---

### TypeScript Strict Mode Migration

**Problem**: Lax TypeScript settings allowed `any` types and incomplete interfaces.

**Root Cause**: Missing strict compiler options and gradual ESLint rule adoption needed.

**Solution**: Enable strict TypeScript in phases:

```json
// tsconfig.json - Phase 1
{
  "compilerOptions": {
    "strict": true,
    "noImplicitAny": true,
    "strictNullChecks": true
  }
}

// eslint.config.js - Phase 1 (warnings)
{
  '@typescript-eslint/no-explicit-any': 'warn'
}

// Phase 2 (after cleanup - enforce)
{
  '@typescript-eslint/no-explicit-any': 'error'
}
```

**Interface Completeness Pattern**:

```typescript
// Extract from actual usage
interface CustomerTypeSelectionVM {
  id: string;
  name: string;
  type: 'individual' | 'business';
  isSelected: boolean;
}
```

**Key Lessons**:

- Gradual rule enforcement prevents team overwhelm
- Create minimal interfaces from actual usage
- Auth state must be complete (token, userId, email, tenantId)
- Coverage gates prevent regression (75% branches, 80% functions/lines)

---

## Infrastructure & Database

### EF Core Migrations: Never Use AppHost as Startup Project

**Problem**: Running migrations with AppHost as startup project fails with assembly loading errors.

**Root Cause**: AppHost shouldn't reference data projects or include `Microsoft.EntityFrameworkCore.Design`.

**Solution**: Implement `IDesignTimeDbContextFactory<T>` in data project:

```csharp
// backend/shared/Monitoring/Data/MonitoringDbContextFactory.cs
public class MonitoringDbContextFactory : IDesignTimeDbContextFactory<MonitoringDbContext>
{
    public MonitoringDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MonitoringDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=monitoring;");
        return new MonitoringDbContext(optionsBuilder.Options);
    }
}
```

**Migration Commands**:

```bash
# Use data project as startup
dotnet ef migrations add InitialCreate --startup-project backend/Monitoring/Data
dotnet ef database update --startup-project backend/Monitoring/Data
```

**Prevention**:

- Never add `Microsoft.EntityFrameworkCore.Design` to AppHost
- Never add DbContext project references to AppHost
- Always implement `IDesignTimeDbContextFactory<T>` in data projects
- Use data project itself as startup for migrations

---

### SQLite Database Schema Recreation

**Problem**: Authentication fails with 401 despite correct credentials due to missing database columns.

**Root Cause**: Database schema mismatch - old database missing new migration columns.

**Solution**: Delete and recreate database:

```powershell
# Delete old database
Remove-Item auth.db* -Force

# Restart service - EF Core auto-creates with current schema
```

**Prevention**:

- Use EF Core migrations properly for schema changes
- Implement database health checks validating schema integrity
- Document database reset procedures for development
- Use in-memory database for unit tests

---

### Microservice Port Conflicts

**Problem**: Multiple services trying to use same port causing 502 Bad Gateway errors.

**Root Cause**: Hardcoded ports in `launchSettings.json` without coordination.

**Solution**: Environment-specific port assignments:

```json
// launchSettings.json
{
  "applicationUrl": "http://localhost:5001"  // Unique per service
}

// appsettings.json - Gateway configuration
{
  "Clusters": {
    "identity-cluster": {
      "Destinations": {
        "identity-service": {
          "Address": "http://localhost:5001"
        }
      }
    }
  }
}
```

**Prevention**:

- Document port mappings in README
- Use service discovery for production
- docker-compose for local development with proper networking
- Standardized port ranges: Identity (5001), Catalog (5002), Orders (5003), etc.

---

## Code Quality & Patterns

### Token-Optimized File Reading Can Mask Corruption

**Problem**: Reading partial file ranges during reviews can miss structural issues.

**Root Cause**: Optimization focused on token savings missed full document validation.

**Solution**: For structure reviews, always read full file first:

```typescript
// WRONG - Partial read can miss issues
read_file("large-doc.md", startLine: 1, endLine: 100)

// CORRECT - Full read for structure review
read_file("large-doc.md", startLine: 1, endLine: totalLines)
```

**When to Use Full Reads**:

- Document structure reviews
- Format validation
- Duplicate detection
- Cross-reference checks

**When Partial Reads OK**:

- Targeted code edits
- Specific section updates
- Known line ranges

---

## Related Documentation

- **[GL-007]**: Lessons Maintenance Strategy
- **[KB-070]**: Aspire .NET 10 Compatibility
- **[ADR-021]**: ArchUnitNET Architecture Testing
- **[GL-006]**: Token Optimization Strategy
- **[KB-021]**: enventa Trade ERP Integration

---

**File Size**: ~560 lines (within GL-007 target)  
**Last Cleanup**: 12. Januar 2026  
**Next Review**: 12. Februar 2026
