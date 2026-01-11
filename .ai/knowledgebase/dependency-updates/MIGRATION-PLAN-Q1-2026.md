---
docid: KB-116
title: Dependency Migration Plan Q1 2026
owner: @TechLead
status: Active
created: 2026-01-11
---

# Dependency Migration Plan - Q1 2026

**Created**: January 11, 2026  
**Project**: B2Connect / B2X  
**Target Framework**: .NET 10.0

---

## Executive Summary

**Current State**: ✅ Excellent - All major dependencies are at latest stable versions.

The B2Connect project is exceptionally well-maintained with nearly all packages at their latest stable versions. This migration plan focuses on:

1. Minor updates for consistency (Spectre.Console.Json)
2. Monitoring pre-release packages for stable releases
3. Preparing for upcoming major version releases

---

## Migration Priority Matrix

| Priority | Package | Action | Risk | Effort |
|----------|---------|--------|------|--------|
| P3-Low | Spectre.Console.Json | 0.49.1 → 0.54.0 | Low | 1h |
| P4-Monitor | EFCore.NamingConventions | Wait for 10.0.0 stable | None | 0h |
| P4-Monitor | Microsoft.Extensions.AI | Wait for stable | None | 0h |
| P4-Monitor | Aspire.Hosting.Elasticsearch | Wait for 13.1.0 | None | 0h |

---

## Phase 1: Immediate Updates (This Sprint)

### 1.1 Spectre.Console.Json Update

**Current**: 0.49.1 → **Target**: 0.54.0

**Risk Assessment**: Low
- Same major version
- Console output only - no production impact
- Used only in CLI tools

**Steps**:
```xml
<!-- In Directory.Packages.props -->
<PackageVersion Include="Spectre.Console.Json" Version="0.54.0" />
```

**Validation**:
1. Build solution
2. Run CLI tool smoke tests
3. Verify JSON output formatting

---

## Phase 2: Pre-release Monitoring (Ongoing)

### 2.1 EF Core Extension Packages

**Packages to Monitor**:
- `EFCore.NamingConventions` (10.0.0-rc.2 → 10.0.0 stable)
- `EFCore.BulkExtensions.PostgreSql` (10.0.0-rc.2 → 10.0.0 stable)
- `EFCore.BulkExtensions.SqlServer` (10.0.0-rc.2 → 10.0.0 stable)

**Action**: Update when stable releases are available (expected Q1 2026)

**Check Command**:
```powershell
# Check for stable releases
Invoke-RestMethod "https://api.nuget.org/v3-flatcontainer/efcore.namingconventions/index.json" | Select-Object -ExpandProperty versions | Select-Object -Last 5
```

### 2.2 Microsoft.Extensions.AI

**Packages**:
- `Microsoft.Extensions.AI` (10.0.0 - current stable)
- `Microsoft.Extensions.AI.OpenAI` (10.1.1-preview.1)
- `Microsoft.Extensions.AI.AzureAIInference` (10.0.0-preview.1)

**Action**: Monitor for stable releases, update AI preview packages when ready

### 2.3 Aspire.Hosting.Elasticsearch

**Current**: 13.0.0  
**Target**: 13.1.0 (when published)

**Action**: Update immediately when 13.1.0 is published to maintain sync with other Aspire packages

---

## Phase 3: No Action Required

These packages are current and require no updates:

### Core Framework (All Current ✅)
| Package | Version | Status |
|---------|---------|--------|
| Microsoft.EntityFrameworkCore | 10.0.1 | ✅ Latest |
| Npgsql | 10.0.1 | ✅ Latest |
| Microsoft.Extensions.* | 10.0.x | ✅ Latest |

### CQRS & Messaging (All Current ✅)
| Package | Version | Status |
|---------|---------|--------|
| WolverineFx | 5.9.2 | ✅ Latest |
| RabbitMQ.Client | 7.2.0 | ✅ Latest |
| MediatR | 14.0.0 | ✅ Latest |

### Observability (All Current ✅)
| Package | Version | Status |
|---------|---------|--------|
| OpenTelemetry | 1.14.0 | ✅ Latest |
| Serilog | 4.3.0 | ✅ Latest |

### Infrastructure (All Current ✅)
| Package | Version | Status |
|---------|---------|--------|
| Aspire.Hosting | 13.1.0 | ✅ Latest |
| StackExchange.Redis | 2.10.1 | ✅ Latest |
| Elastic.Clients.Elasticsearch | 9.2.2 | ✅ Latest |

### Testing (All Current ✅)
| Package | Version | Status |
|---------|---------|--------|
| xunit.v3 | 3.2.1 | ✅ Latest |
| Testcontainers | 4.10.0 | ✅ Latest |
| FluentAssertions | 8.8.0 | ✅ Latest |

### Validation & Mapping (All Current ✅)
| Package | Version | Status |
|---------|---------|--------|
| FluentValidation | 12.1.1 | ✅ Latest |
| AutoMapper | 16.0.0 | ✅ Latest |
| Polly | 8.6.5 | ✅ Latest |

### API & Documentation (All Current ✅)
| Package | Version | Status |
|---------|---------|--------|
| Swashbuckle.AspNetCore | 10.1.0 | ✅ Latest |

---

## Intentionally Pinned Packages

These packages are intentionally locked and should NOT be updated:

### Microsoft.AspNetCore.Mvc / Mvc.Core
- **Pinned Version**: 2.3.9
- **Reason**: Version 10.0.1 does not exist on NuGet
- **Action**: Review if Microsoft publishes newer version

### Aspire.Hosting.Elasticsearch
- **Pinned Version**: 13.0.0
- **Reason**: Version 13.1.0 not yet published
- **Action**: Update when 13.1.0 is available

---

## Validation Checklist

After any package update:

- [ ] Solution builds successfully
- [ ] All unit tests pass
- [ ] All integration tests pass
- [ ] No new security vulnerabilities introduced
- [ ] No breaking changes in APIs
- [ ] CI/CD pipeline passes
- [ ] Update Directory.Packages.props header documentation

---

## Rollback Procedure

If issues occur after update:

1. **Git revert** the Directory.Packages.props changes
2. **Clear NuGet cache**: `dotnet nuget locals all --clear`
3. **Restore packages**: `dotnet restore`
4. **Rebuild solution**: `dotnet build`

---

## Automated Monitoring

### Weekly Dependency Check (CI/CD)

```yaml
# .github/workflows/dependency-check.yml
name: Weekly Dependency Check
on:
  schedule:
    - cron: '0 8 * * 1'  # Every Monday at 8 AM
jobs:
  check-dependencies:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Check outdated packages
        run: dotnet list package --outdated
      - name: Check vulnerable packages
        run: dotnet list package --vulnerable
```

---

## Timeline

| Week | Action | Owner |
|------|--------|-------|
| W2 2026 | Update Spectre.Console.Json | @Backend |
| Monthly | Monitor pre-release packages | @TechLead |
| On Release | Update EFCore extensions when stable | @Backend |
| On Release | Update Aspire.Hosting.Elasticsearch when 13.1.0 | @Backend |

---

## References

- [KB-DEP-STATUS-2026-01-11] Dependency Version Status Report
- [KB-114] Central Package Management (CPM)
- [GL-013] Dependency Management Policy
- [WF-003] Dependency Upgrade Workflow

---

**Last Updated**: January 11, 2026  
**Next Review**: February 11, 2026  
**Maintained by**: @TechLead
