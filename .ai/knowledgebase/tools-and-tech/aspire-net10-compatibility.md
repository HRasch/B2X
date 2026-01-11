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
**Status**: ✅ **RESOLVED** - Upgraded to Aspire 13.1.0 with SDK-based approach

---

## ✅ Resolution (11. Januar 2026)

**The compatibility issues with Aspire and .NET 10 have been resolved by upgrading to Aspire 13.1.0 with the modern SDK-based approach.**

### Changes Made

1. **Upgraded from Aspire 8.2.0 → 13.1.0**:
   - Changed from deprecated workload model to SDK-based approach
   - Updated `B2X.AppHost.csproj` to use `Sdk="Aspire.AppHost.Sdk/13.1.0"`
   - Removed `<IsAspireHost>true</IsAspireHost>` property (no longer needed)

2. **Package Updates**:
   - All Aspire packages updated to 13.1.0 (except Elasticsearch at 13.0.0)
   - Removed explicit `Aspire.Hosting.AppHost` reference (implicitly provided by SDK)
   - Updated `nuget.config` to allow Aspire packages from nuget.org

3. **Eliminated Workarounds**:
   - No longer need `AssetTargetFallback` properties
   - No longer need `RollForward` to `LatestMajor`
   - No longer need `CopyLocalLockFileAssemblies`

### New AppHost.csproj Structure

```xml
<Project Sdk="Aspire.AppHost.Sdk/13.1.0">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <UserSecretsId>B2X-apphost</UserSecretsId>
  </PropertyGroup>
  <!-- Aspire.Hosting.AppHost automatically included by SDK -->
</Project>
```

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

## Related Documentation

- [KB-LESSONS](../lessons.md) - Session 10. Januar 2026
- [Aspire Orchestration Specs](../../src/docs/aspire-orchestration-specs.md)
- [ADR-003](../../.ai/decisions/ADR-003-aspire-orchestration.md) - Aspire Orchestration Decision

---

## Change Log

| Date | Change | Author |
|------|--------|--------|
| 2026-01-10 | Initial documentation of .NET 10 compatibility issue | @Backend |
