---
docid: INS-009
title: Dependency Management.Instructions
owner: @CopilotExpert
status: Active
created: 2026-01-08
---

---
docid: INS-007
title: Dependency Management Instructions
owner: "@TechLead"
status: Active
---

---
applyTo: "Directory.Packages.props,**/*.csproj,**/*.fsproj,**/*.vbproj"
---

# Dependency Management Instructions

## Overview
This instruction governs dependency version management across the B2X project using Central Package Management (CPM).

## Core Principles

### 1. Single Source of Truth
- `Directory.Packages.props` is the **ONLY** file for package versions
- No hardcoded versions in individual `.csproj` files
- No additional `Directory.Packages.props` files in subfolders

### 2. No Downgrades Policy 🚨
**NEVER downgrade package versions without explicit approval and documentation!**

#### Reasons for Current Version Locks:
- **Microsoft.AspNetCore.Mvc.Core**: `2.3.9` (latest available, `10.0.1` doesn't exist on NuGet)
- **Entity Framework Core**: `10.0.1` (required for .NET 10.0 compatibility)
- **Aspire packages**: `13.1.0` (latest stable, all Aspire packages must stay in sync)
- **OpenTelemetry**: `1.14.0` (latest stable, all OTEL packages must stay in sync)
- **Wolverine**: `5.9.2` (latest stable, all Wolverine packages must stay in sync)

### 3. Version Compatibility Rules
| Package Group | Rule |
|---------------|------|
| Microsoft.EntityFrameworkCore.* | Must all be same version (10.0.1) |
| Npgsql.EntityFrameworkCore.PostgreSQL | Must match EF Core major (10.0.x) |
| Aspire.Hosting.* | Keep all at same version (13.1.0) |
| OpenTelemetry.* | Keep all at same version (1.14.0) |
| WolverineFx.* | Keep all at same version (5.9.2) |
| Microsoft.Extensions.* | Keep at .NET version (10.0.x) |

## Process for Version Changes

### Before Changing ANY Version:
1. **Check NuGet.org** for package availability
2. **Test build compatibility** across all target frameworks (net10.0, net8.0, netstandard2.1, net48)
3. **Run full test suite** (332+ tests)
4. **Document change reason** in commit message
5. **Update documentation** in `Directory.Packages.props` header

### Required Approvals:
- **Major version changes**: @Architect + @TechLead approval
- **Breaking changes**: Full team review
- **Security updates**: Immediate application allowed

### Multi-Framework Considerations:
- Projects targeting older frameworks (netstandard2.1, net48) may have different versions
- These exceptions must be documented in project files with clear comments
- CPM is disabled for such projects (`<ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>`)

## Validation

### Automated Checks:
- Pre-commit hook validates CPM compliance
- CI runs `scripts/ci-validate-dependencies.sh`
- Build fails on hardcoded versions in `.csproj` files

### Manual Verification:
```bash
# Check for hardcoded versions
grep -r "PackageReference.*Version=" --include="*.csproj" .

# Validate package availability
dotnet list package --outdated

# Test build across frameworks
dotnet build B2X.slnx
```

## Exceptions

### Legitimate Exceptions:
1. **Multi-targeting projects** with older framework requirements
2. **Tool projects** in `/tools/` folder
3. **Test projects** with specific version requirements

### Documentation Requirements:
All exceptions must include:
- Clear comment explaining the exception
- Link to issue or ADR justifying the exception
- Regular review schedule

## Enforcement

### Pre-commit Hook:
- Blocks commits with hardcoded versions
- Validates CPM compliance
- Can be bypassed with `--no-verify` for legitimate exceptions

### CI Validation:
- Runs on every PR
- Fails builds with version violations
- Reports detailed error messages

## Maintenance

### Regular Tasks:
- Monthly review of outdated packages
- Quarterly security audit
- Annual dependency cleanup

### Documentation Updates:
- Update this file when policies change
- Update `Directory.Packages.props` header when versions change
- Maintain changelog of version changes

---

**Last Updated**: 8. Januar 2026
**Owner**: @TechLead
**Review Cycle**: Monthly</content>
<parameter name="filePath">c:\Users\Holge\repos\B2Connect\.github\instructions\dependency-management.instructions.md