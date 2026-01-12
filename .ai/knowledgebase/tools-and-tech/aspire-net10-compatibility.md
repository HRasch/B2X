---
docid: KB-070
title: Aspire .NET 10 Compatibility
owner: @Backend
status: Resolved
created: 2026-01-10
updated: 2026-01-11
---

# Aspire .NET 10 Compatibility Guide

**DocID**: `KB-070`  
**Last Updated**: 11. Januar 2026  
**Status**: ✅ **RESOLVED** - Workarounds required for Aspire 13.1.0 + .NET 10

---

## ✅ Final Resolution (11. Januar 2026)

**Aspire 13.1.0 works with .NET 10, but requires specific MSBuild properties because `Aspire.Hosting` and its transitive dependencies (KubernetesClient, etc.) only ship `net8.0`/`net9.0` assemblies.**

### Required AppHost.csproj Configuration

```xml
<Project Sdk="Aspire.AppHost.Sdk/13.1.0">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    
    <!-- REQUIRED: Allow NuGet to use net8.0/net9.0 packages when net10.0 unavailable -->
    <AssetTargetFallback>$(AssetTargetFallback);net9.0;net8.0</AssetTargetFallback>
    
    <!-- REQUIRED: Allow runtime to load older assemblies -->
    <RollForward>LatestMajor</RollForward>
    
    <!-- REQUIRED: Copy ALL transitive dependencies to output folder -->
    <CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>
    
    <!-- REQUIRED: Enable Aspire host behaviors -->
    <IsAspireHost>true</IsAspireHost>
    
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <UserSecretsId>B2X-apphost</UserSecretsId>
  </PropertyGroup>
  
  <ItemGroup>
    <!-- REQUIRED: Explicit reference despite SDK -->
    <PackageReference Include="Aspire.Hosting.AppHost" />
    <!-- Other Aspire packages... -->
  </ItemGroup>
</Project>
```

### Required Directory.Packages.props Entries

```xml
<!-- Both must be present for central package management -->
<PackageVersion Include="Aspire.Hosting" Version="13.1.0" />
<PackageVersion Include="Aspire.Hosting.AppHost" Version="13.1.0" />
```

### Why Each Property is Required

| Property | Purpose | Without It |
|----------|---------|------------|
| `AssetTargetFallback` | Allows NuGet to use net8.0/net9.0 packages | Restore fails or assemblies not copied |
| `RollForward: LatestMajor` | Allows .NET 10 runtime to load older assemblies | `FileNotFoundException` at runtime |
| `CopyLocalLockFileAssemblies` | Copies ALL transitive deps (KubernetesClient, etc.) to output | `FileNotFoundException` for transitive deps |
| `IsAspireHost: true` | Enables Aspire-specific MSBuild behaviors | May miss SDK features |
| Explicit `Aspire.Hosting.AppHost` | Ensures package is properly resolved | Assembly loading failures |

### Root Cause Analysis

**Package Framework Support in Aspire 13.1.0:**

| Package | net8.0 | net9.0 | net10.0 |
|---------|--------|--------|---------|
| `Aspire.Hosting.AppHost` | ✅ | ✅ | ✅ |
| `Aspire.Hosting` | ✅ | ❌ | ❌ |
| `Aspire.Hosting.PostgreSQL` | ✅ | ❌ | ❌ |
| `Aspire.Hosting.Redis` | ✅ | ❌ | ❌ |
| `KubernetesClient` (transitive) | ✅ | ✅ | ❌ |

The SDK wrapper supports net10.0, but core runtime libraries don't.

---

## Historical Context (Before 11. Januar 2026)

### Original Problem

### Symptom

AppHost fails to start with:
```
System.IO.FileNotFoundException: Could not load file or assembly 
'Aspire.Hosting, Version=13.1.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'
```

### Root Cause

**Framework Target Mismatch** in Aspire 13.1.0 packages:

| Package | net10.0 Support | Status |
|---------|-----------------|--------|
| `Aspire.Hosting.AppHost` (SDK) | ✅ Yes | Works |
| `Aspire.Hosting` (runtime) | ❌ No (net8.0 only) | **FAILS** |
| `Aspire.Hosting.Redis` | ❌ No (net8.0 only) | **FAILS** |
| `Aspire.Hosting.PostgreSQL` | ❌ No (net8.0 only) | **FAILS** |

The `Aspire.AppHost.Sdk` successfully builds projects targeting `net10.0`, but the runtime libraries only ship `net8.0` assemblies.

### Why Build Succeeds But Runtime Fails

1. **NuGet Restore**: Uses `AssetTargetFallback` to accept `net8.0` packages
2. **MSBuild**: Successfully compiles against the SDK
3. **Runtime**: .NET 10 runtime cannot load `net8.0` assemblies by default

---

## Solution

### Applied Workaround

Add these properties to `B2X.AppHost.csproj`:

```xml
<PropertyGroup>
  <TargetFramework>net10.0</TargetFramework>
  
  <!-- WORKAROUND: Aspire 13.1.0 hosting packages only ship net8.0 assemblies -->
  <!-- Force copy ALL NuGet dependencies to output directory -->
  <CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>
  
  <!-- Allow NuGet to use net8.0 packages as fallback -->
  <AssetTargetFallback>$(AssetTargetFallback);net8.0</AssetTargetFallback>
</PropertyGroup>
```

### How It Works

1. **`CopyLocalLockFileAssemblies`**: Forces ALL transitive dependencies to be copied to `bin/Debug/net10.0/`, including `net8.0` fallback assemblies
2. **`AssetTargetFallback`**: Allows NuGet restore to accept `net8.0` packages when `net10.0` isn't available
3. **Runtime Probing**: With assemblies in the output directory, the runtime can find and load them

---

## Diagnostic Commands

### Check Package Framework Targets

```powershell
# See what frameworks a package supports
Get-ChildItem "$env:USERPROFILE\.nuget\packages\aspire.hosting\13.1.0\lib" | Select-Object Name
# Output: net8.0 (no net10.0!)

# Check AppHost SDK targets
Get-ChildItem "$env:USERPROFILE\.nuget\packages\aspire.hosting.apphost\13.1.0\lib" | Select-Object Name
# Output: net10.0, net9.0, net8.0 ✓
```

### Verify Output Directory

```powershell
# Check if Aspire assemblies are copied to output
Get-ChildItem "src/backend/Infrastructure/Hosting/AppHost/bin/Debug/net10.0/" -Filter "Aspire*.dll"
# Should list: Aspire.Hosting.dll, Aspire.Hosting.Redis.dll, etc.
```

### Inspect deps.json

```powershell
# Check assembly paths in deps.json
Select-String -Path "bin/Debug/net10.0/B2X.AppHost.deps.json" -Pattern "lib/net8.0"
# If matches found AND CopyLocalLockFileAssemblies=false, runtime will fail
```

---

## GitHub References

| Issue/PR | Description | Status |
|----------|-------------|--------|
| [#13611](https://github.com/dotnet/aspire/issues/13611) | Aspire.Hosting doesn't support net10.0 | Open |
| [#12500](https://github.com/dotnet/aspire/pull/12500) | Target net10.0 in client integrations | Merged (partial) |
| Aspire 13.2 | Expected to include full net10.0 hosting support | Pending |

---

## Alternative Approaches (Not Recommended)

### 1. Downgrade AppHost to net9.0

```xml
<TargetFramework>net9.0</TargetFramework>
```

**Problem**: All referenced projects target `net10.0`, causing `NU1201` errors.

### 2. RollForward Policy

```xml
<RollForward>LatestMajor</RollForward>
```

**Problem**: Doesn't help with assembly loading; only affects runtime version selection.

### 3. Manual Assembly Copy

```powershell
Copy-Item "$env:USERPROFILE\.nuget\packages\aspire.hosting\13.1.0\lib\net8.0\*.dll" -Destination "bin/Debug/net10.0/"
```

**Problem**: Tedious, doesn't handle transitive dependencies, breaks on clean builds.

---

## Monitoring for Fix

### Check for Aspire Updates

```powershell
# Check latest Aspire version
dotnet package search "Aspire.Hosting" --take 5

# Check aspire-daily feed for previews
dotnet package search "Aspire.Hosting" --source "https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet-aspire-daily/nuget/v3/index.json" --prerelease
```

### When to Remove Workaround

Remove the workaround when:
1. Aspire 13.2+ is released
2. `Aspire.Hosting` package contains `lib/net10.0/` folder
3. Verify with: `Get-ChildItem "$env:USERPROFILE\.nuget\packages\aspire.hosting\13.2.0\lib"`

---

## Troubleshooting Common Issues

### Issue 1: Aspire Exits Immediately in VS Code Terminal

**Symptom**: Aspire starts, prints "Distributed application started", then exits with code 1.

**Cause**: VS Code terminal redirects stdin, making Aspire detect non-interactive environment.

**Solution**: Run Aspire in a separate PowerShell window:
```powershell
Start-Process pwsh -ArgumentList '-NoExit', '-Command', 'cd ''path/to/AppHost''; aspire run'
```

See [KB-LESSONS](../lessons.md) - "Aspire Exits Immediately in VS Code Terminal" for details.

### Issue 2: File Lock Errors (MSB3021)

**Symptom**: Build fails with "Cannot copy file... being used by another process".

**Cause**: VBCSCompiler or previous dotnet processes holding file locks.

**Solution**:
```powershell
# Kill all dotnet/compiler processes
taskkill /F /IM dotnet.exe
taskkill /F /IM VBCSCompiler.exe

# Clean and rebuild
dotnet clean B2X.slnx
dotnet build src/backend/Infrastructure/Hosting/AppHost/B2X.AppHost.csproj
```

### Issue 3: Dashboard Shows No Resources

**Symptom**: Aspire dashboard on http://localhost:15500 shows empty.

**Cause**: Services not yet orchestrated or still starting.

**Solution**: Wait 30-60 seconds for DCP to orchestrate all services, or check stderr logs:
```powershell
Get-Content "$env:TEMP\aspire-err.log" -Tail 40
```

---

## Related Documentation

- [KB-LESSONS](../lessons.md) - Session 11. Januar 2026
- [Aspire Orchestration Specs](../../src/docs/aspire-orchestration-specs.md)
- [ADR-003](../../.ai/decisions/ADR-003-aspire-orchestration.md) - Aspire Orchestration Decision

---

## Change Log

| Date | Change | Author |
|------|--------|--------|
| 2026-01-10 | Initial documentation of .NET 10 compatibility issue | @Backend |
| 2026-01-11 | Added troubleshooting section for VS Code stdin, file locks | @SARAH |
