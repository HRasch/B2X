---
docid: KB-114
title: Central Package Management (CPM) for .NET
owner: @Backend, @TechLead
status: Active
created: 2026-01-11
updated: 2026-01-11
---

# Central Package Management (CPM) for .NET

**DocID**: `KB-114`  
**Focus**: Managing NuGet package versions centrally in .NET projects  
**Status**: Active  
**Last Updated**: January 11, 2026

---

## Overview

Central Package Management (CPM) is a NuGet feature that allows you to manage common package dependencies from a single location in your repository. Introduced in NuGet 5.2 and fully supported in .NET 7+, CPM simplifies dependency management across multi-project solutions.

**Key Benefit**: Instead of specifying versions in each project file, you define all package versions in a central `Directory.Packages.props` file at the repository root.

---

## Quick Start

### Enable CPM

1. **Create or update `Directory.Packages.props`** at your repository root:

```xml
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>

  <ItemGroup>
    <!-- Define all package versions here -->
    <PackageVersion Include="Microsoft.Extensions.Configuration" Version="9.0.0" />
    <PackageVersion Include="Newtonsoft.Json" Version="13.0.3" />
    <PackageVersion Include="xunit" Version="2.6.6" />
    <PackageVersion Include="Shouldly" Version="4.1.0" />
  </ItemGroup>
</Project>
```

2. **Update project files** - Remove `Version` attributes from `PackageReference`:

```xml
<!-- Before CPM -->
<ItemGroup>
  <PackageReference Include="Microsoft.Extensions.Configuration" Version="9.0.0" />
</ItemGroup>

<!-- After CPM -->
<ItemGroup>
  <PackageReference Include="Microsoft.Extensions.Configuration" />
</ItemGroup>
```

### Project Structure

```
Repository/
├── Directory.Packages.props          ← Central version management
├── B2X.slnx
├── src/
│   ├── api/project1.csproj           ← No Version attributes
│   ├── backend/project2.csproj       ← No Version attributes
│   └── frontend/
└── tests/
    ├── unit.tests.csproj             ← No Version attributes
    └── integration.tests.csproj       ← No Version attributes
```

---

## CPM Best Practices

### 1. **Version Pinning Strategy**

```xml
<ItemGroup>
  <!-- Exact versions for critical dependencies -->
  <PackageVersion Include="Microsoft.EntityFrameworkCore" Version="[9.0.0]" />
  
  <!-- Minor version ranges for stable packages -->
  <PackageVersion Include="Wolverine" Version="1.18.0" />
  
  <!-- Patch-only updates for production packages -->
  <PackageVersion Include="System.IO.Abstractions" Version="19.2.63" />
</ItemGroup>
```

### 2. **Organize by Category**

```xml
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>

  <!-- Core Framework -->
  <ItemGroup Label="Microsoft.Extensions">
    <PackageVersion Include="Microsoft.Extensions.Configuration" Version="9.0.0" />
    <PackageVersion Include="Microsoft.Extensions.DependencyInjection" Version="9.0.0" />
    <PackageVersion Include="Microsoft.Extensions.Hosting" Version="9.0.0" />
    <PackageVersion Include="Microsoft.Extensions.Logging" Version="9.0.0" />
    <PackageVersion Include="Microsoft.Extensions.Caching.Hybrid" Version="9.0.0" />
  </ItemGroup>

  <!-- CQRS Framework -->
  <ItemGroup Label="Wolverine">
    <PackageVersion Include="Wolverine" Version="1.18.0" />
    <PackageVersion Include="Wolverine.Postgresql" Version="1.18.0" />
  </ItemGroup>

  <!-- Testing Frameworks -->
  <ItemGroup Label="Testing">
    <PackageVersion Include="xunit" Version="2.6.6" />
    <PackageVersion Include="xunit.runner.visualstudio" Version="2.5.5" />
    <PackageVersion Include="Shouldly" Version="4.1.0" />
    <PackageVersion Include="Moq" Version="4.20.70" />
  </ItemGroup>

  <!-- Data Access -->
  <ItemGroup Label="Data">
    <PackageVersion Include="Npgsql" Version="8.0.1" />
    <PackageVersion Include="System.Data.SqlClient" Version="4.8.6" />
  </ItemGroup>

  <!-- API & Web -->
  <ItemGroup Label="Web">
    <PackageVersion Include="Microsoft.AspNetCore.Mvc" Version="9.0.0" />
    <PackageVersion Include="Swashbuckle.AspNetCore" Version="6.0.0" />
  </ItemGroup>

  <!-- Security & Auth -->
  <ItemGroup Label="Security">
    <PackageVersion Include="System.IdentityModel.Tokens.Jwt" Version="7.4.0" />
    <PackageVersion Include="Azure.Identity" Version="1.12.0" />
  </ItemGroup>

  <!-- Utilities -->
  <ItemGroup Label="Utilities">
    <PackageVersion Include="Polly" Version="8.2.0" />
    <PackageVersion Include="Serilog" Version="3.1.1" />
    <PackageVersion Include="Serilog.Sinks.Console" Version="5.0.1" />
  </ItemGroup>
</Project>
```

### 3. **Version Range Syntax**

| Syntax | Meaning | Use Case |
|--------|---------|----------|
| `1.0` | x ≥ 1.0 (minimum) | Most flexible, allows any newer version |
| `[1.0]` | x = 1.0 (exact) | Critical dependencies, security-sensitive |
| `[1.0,2.0)` | 1.0 ≤ x < 2.0 | Major version stability |
| `1.0.*` | Any 1.0.x version | Minor version flexibility |
| `(1.0,)` | x > 1.0 (exclusive) | Exclude specific problematic versions |

**Recommendation**: Use exact versions `[x.y.z]` for production builds, minor ranges `[x.y,x+1)` for libraries.

---

## Version Management Workflow

### 1. **Dependency Updates**

```bash
# When updating a package version
# 1. Update Directory.Packages.props only
# 2. All projects automatically use the new version
# 3. No need to update individual .csproj files
```

Example:
```xml
<!-- Update this line -->
<PackageVersion Include="Wolverine" Version="1.18.0" />
<!-- to this -->
<PackageVersion Include="Wolverine" Version="1.19.0" />
<!-- All projects automatically get 1.19.0 -->
```

### 2. **Overriding Versions (When Needed)**

If a specific project needs a different version, you can override it:

```xml
<!-- In Directory.Packages.props -->
<ItemGroup>
  <PackageVersion Include="Newtonsoft.Json" Version="13.0.3" />
</ItemGroup>

<!-- In specific project file -->
<ItemGroup>
  <!-- Override with different version if needed -->
  <PackageReference Include="Newtonsoft.Json" Version="12.0.3" />
</ItemGroup>
```

### 3. **Transitive Dependencies**

CPM also manages transitive dependency versions:

```xml
<ItemGroup>
  <!-- Explicitly set versions for transitive dependencies -->
  <PackageVersion Include="System.Reflection.Metadata" Version="7.0.0" />
  <PackageVersion Include="System.Text.Json" Version="9.0.0" />
</ItemGroup>
```

---

## MSBuild Properties

### ManagePackageVersionsCentrally

```xml
<PropertyGroup>
  <!-- Enable CPM in Directory.Packages.props -->
  <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
</PropertyGroup>
```

**Location**: `Directory.Packages.props` (root level)  
**Default**: false  
**Impact**: When true, all `PackageReference` items must omit `Version` attributes

---

## Compatibility & Requirements

| Feature | Requirement | Status |
|---------|-------------|--------|
| Central Package Management | .NET 7.0+ / NuGet 5.2+ | ✅ Stable |
| Directory.Packages.props | .NET 7.0+ | ✅ Stable |
| `ManagePackageVersionsCentrally` | .NET 7.0+ | ✅ Stable |
| SDK-style projects | .NET SDK 7.0.100+ | ✅ Stable |

### Legacy Project Support

CPM works with SDK-style projects only. Non-SDK-style projects must use traditional package management:

```xml
<!-- Legacy (non-SDK) projects still use per-project versions -->
<ItemGroup>
  <PackageReference Include="Package" Version="1.0.0" />
</ItemGroup>
```

---

## Common Issues & Solutions

### Issue 1: "Version not specified in PackageReference"

**Error**: NU1008 - Package version not specified in PackageReference

**Solution**: Add the package to `Directory.Packages.props`:

```xml
<!-- Directory.Packages.props -->
<ItemGroup>
  <PackageVersion Include="PackageName" Version="X.Y.Z" />
</ItemGroup>
```

### Issue 2: Project-specific overrides not working

**Solution**: Specify the full version in the project file:

```xml
<!-- In project .csproj -->
<ItemGroup>
  <!-- When you need a different version -->
  <PackageReference Include="PackageName" Version="A.B.C" />
  <!-- This overrides the central version -->
</ItemGroup>
```

### Issue 3: Transitive dependency version conflicts

**Solution**: Explicitly pin the transitive dependency:

```xml
<!-- Directory.Packages.props -->
<ItemGroup>
  <PackageVersion Include="DependencyPackage" Version="3.0.0" />
  <PackageVersion Include="TransitiveDependency" Version="2.1.0" />
</ItemGroup>
```

---

## Integration with Build Process

### Pre-build Validation

```bash
# Check for version consistency
dotnet restore --no-cache

# Validate CPM configuration
dotnet build --no-restore
```

### CI/CD Pipeline

```yaml
# Example GitHub Actions workflow
- name: Restore packages with CPM
  run: dotnet restore --no-cache

- name: Validate package versions
  run: dotnet list package --outdated
```

---

## Security Considerations

### 1. **Dependency Scanning**

Regularly scan for vulnerabilities:

```bash
# Using dotnet tool
dotnet add package Spectre.Console.Nerdbank.GitVersioning
dotnet list package --vulnerable
```

### 2. **Version Pinning for Security**

```xml
<ItemGroup>
  <!-- Use exact versions for security-critical packages -->
  <PackageVersion Include="System.IdentityModel.Tokens.Jwt" Version="[7.4.0]" />
  <PackageVersion Include="Azure.Identity" Version="[1.12.0]" />
</ItemGroup>
```

### 3. **Restrict Ranges**

Avoid open-ended version ranges:

```xml
<!-- ❌ Too permissive -->
<PackageVersion Include="PackageName" Version="1.0" />

<!-- ✅ Better - restricts to minor versions -->
<PackageVersion Include="PackageName" Version="[1.0,2.0)" />

<!-- ✅ Best for critical packages - exact version -->
<PackageVersion Include="PackageName" Version="[1.2.3]" />
```

---

## Performance Impact

CPM provides **no negative performance impact**. Benefits include:

- **Faster project loads**: Single version source instead of parsing each .csproj
- **Easier analysis**: Single location for all versions
- **Reduced file size**: No version attributes in individual project files

---

## Migration Guide

### Migrating from Traditional to CPM

#### Step 1: Create Directory.Packages.props

```xml
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>

  <ItemGroup>
    <!-- Extract all PackageVersion entries from .csproj files -->
  </ItemGroup>
</Project>
```

#### Step 2: Extract Versions

```bash
# Find all versions in project files
grep -r "PackageReference.*Version" *.csproj
```

#### Step 3: Add to Directory.Packages.props

```xml
<ItemGroup>
  <!-- Add each unique package once -->
  <PackageVersion Include="Package1" Version="X.Y.Z" />
  <PackageVersion Include="Package2" Version="A.B.C" />
</ItemGroup>
```

#### Step 4: Update Project Files

Remove `Version` attributes from all `PackageReference` items:

```xml
<!-- Before -->
<PackageReference Include="Package1" Version="X.Y.Z" />

<!-- After -->
<PackageReference Include="Package1" />
```

#### Step 5: Validate

```bash
dotnet restore
dotnet build
```

---

## Advanced Scenarios

### Monorepo Version Management

For monorepos with multiple solutions:

```
Repository/
├── Directory.Packages.props          ← Shared versions
├── Solution1/
│   ├── Directory.Build.props          ← Solution1 overrides
│   └── projects/
└── Solution2/
    ├── Directory.Build.props          ← Solution2 overrides
    └── projects/
```

### Conditional Versions

```xml
<ItemGroup Condition="'$(Configuration)' == 'Debug'">
  <PackageVersion Include="Debugger" Version="1.0.0" />
</ItemGroup>

<ItemGroup Condition="'$(TargetFramework)' == 'net9.0'">
  <PackageVersion Include="Net9Only" Version="2.0.0" />
</ItemGroup>
```

---

## Recommendations for B2Connect

### Current Setup

```xml
<!-- Directory.Packages.props for B2Connect -->
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>

  <!-- Microsoft.Extensions ecosystem -->
  <ItemGroup>
    <PackageVersion Include="Microsoft.Extensions.Configuration" Version="9.0.0" />
    <PackageVersion Include="Microsoft.Extensions.DependencyInjection" Version="9.0.0" />
    <PackageVersion Include="Microsoft.Extensions.Hosting" Version="9.0.0" />
    <PackageVersion Include="Microsoft.Extensions.Logging" Version="9.0.0" />
    <!-- Add more as needed -->
  </ItemGroup>

  <!-- Wolverine CQRS -->
  <ItemGroup>
    <PackageVersion Include="Wolverine" Version="1.18.0" />
    <PackageVersion Include="Wolverine.Postgresql" Version="1.18.0" />
  </ItemGroup>

  <!-- Testing frameworks -->
  <ItemGroup>
    <PackageVersion Include="xunit" Version="2.6.6" />
    <PackageVersion Include="Shouldly" Version="4.1.0" />
  </ItemGroup>
</Project>
```

### Benefits

1. ✅ **Single source of truth** for all package versions
2. ✅ **Easier updates** - change version in one place
3. ✅ **Reduced conflicts** - no version mismatches across projects
4. ✅ **Cleaner .csproj files** - no Version attributes needed
5. ✅ **Better governance** - enforces consistent versions

---

## References

- [Microsoft Learn: Central Package Management](https://learn.microsoft.com/en-us/nuget/consume/central-package-management)
- [NuGet Package Versioning](https://learn.microsoft.com/en-us/nuget/concepts/package-versioning)
- [MSBuild Reference: ManagePackageVersionsCentrally](https://learn.microsoft.com/en-us/dotnet/core/project-sdk/msbuild-props#managepackageversionscentrally)

---

**Last Updated**: January 11, 2026  
**Next Review**: April 11, 2026  
**Status**: ✅ Production Ready
