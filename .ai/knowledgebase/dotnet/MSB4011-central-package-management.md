# MSB4011 Warning with Central Package Management (CPM)

**Category**: .NET Build  
**Tags**: MSBuild, NuGet, Central Package Management, Warnings  
**Created**: 2026-01-01  
**Author**: @TechLead

## Problem

When using Central Package Management (CPM) with `Directory.Packages.props`, you may see many MSB4011 warnings:

```
warning MSB4011: "Directory.Packages.props" cannot be imported again. It was already imported.
```

This occurs when the file is imported twice:
1. **Automatically** by the .NET SDK when `ManagePackageVersionsCentrally=true`
2. **Manually** via `<Import Project="..\Directory.Packages.props" />` in `Directory.Build.props`

## Root Cause

The .NET SDK automatically imports `Directory.Packages.props` from the project directory or parent directories when Central Package Management is enabled. A manual import causes a duplicate import.

## Solution

**Remove the manual import** from `Directory.Build.props`. The SDK handles it automatically.

### ❌ Wrong (causes MSB4011)

```xml
<!-- Directory.Build.props -->
<Project>
  <Import Project="..\Directory.Packages.props" />  <!-- REMOVE THIS -->
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>
</Project>
```

### ✅ Correct

```xml
<!-- Directory.Build.props -->
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>
  <!-- No manual import needed - SDK auto-imports Directory.Packages.props -->
</Project>
```

## Alternative: Suppress the Warning

If you must keep the manual import for specific reasons, suppress MSB4011:

```xml
<PropertyGroup>
  <MSBuildWarningsAsMessages>$(MSBuildWarningsAsMessages);MSB4011</MSBuildWarningsAsMessages>
</PropertyGroup>
```

**Note**: `NoWarn` does NOT work for MSB4011 because it's an MSBuild warning, not a compiler warning.

## References

- [Microsoft Docs: Central Package Management](https://learn.microsoft.com/en-us/nuget/consume-packages/central-package-management)
- [Microsoft Docs: Customize Build by Directory](https://learn.microsoft.com/en-us/visualstudio/msbuild/customize-by-directory)

## Project Application

Applied in B2Connect on 2026-01-01:
- Removed manual import from `backend/Directory.Build.props`
- Result: 118 warnings → 0 warnings
