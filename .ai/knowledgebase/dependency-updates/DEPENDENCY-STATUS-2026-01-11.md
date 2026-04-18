---
docid: KB-115
title: Dependency Status Report Q1 2026
owner: @TechLead
status: Active
created: 2026-01-11
---

# Dependency Version Status Report

**Generated**: January 11, 2026  
**Project**: B2Connect / B2X  
**Framework**: .NET 10.0

---

## Summary

| Status | Count | Action |
|--------|-------|--------|
| ✅ Current | 38 | No action needed |
| 🔄 Update Available | 2 | Review and update |
| ⚠️ Pre-release | 5 | Monitor for stable |
| 📌 Pinned | 3 | Intentionally locked |

---

## Package Version Analysis

### ✅ Current Versions (No Update Needed)

| Package | Current | Latest Stable | Status |
|---------|---------|---------------|--------|
| **Aspire.Hosting** | 13.1.0 | 13.1.0 | ✅ Current |
| **WolverineFx** | 5.9.2 | 5.9.2 | ✅ Current |
| **OpenTelemetry** | 1.14.0 | 1.14.0 | ✅ Current |
| **FluentValidation** | 12.1.1 | 12.1.1 | ✅ Current |
| **Polly** | 8.6.5 | 8.6.5 | ✅ Current |
| **Serilog** | 4.3.0 | 4.3.0 | ✅ Current |
| **Quartz** | 3.15.1 | 3.15.1 | ✅ Current |
| **xunit.v3** | 3.2.1 | 3.2.1 | ✅ Current |
| **Testcontainers** | 4.10.0 | 4.10.0 | ✅ Current |
| **StackExchange.Redis** | 2.10.1 | 2.10.1 | ✅ Current |
| **Elastic.Clients.Elasticsearch** | 9.2.2 | 9.2.2 | ✅ Current |
| **RabbitMQ.Client** | 7.2.0 | 7.2.0 | ✅ Current |
| **Swashbuckle.AspNetCore** | 10.1.0 | 10.1.0 | ✅ Current |
| **AutoMapper** | 16.0.0 | 16.0.0 | ✅ Current |
| **MediatR** | 14.0.0 | 14.0.0 | ✅ Current |
| **Microsoft.EntityFrameworkCore** | 10.0.1 | 10.0.1 | ✅ Current |
| **Npgsql** | 10.0.1 | 10.0.1 | ✅ Current |
| **Microsoft.Extensions.*** | 10.0.1 | 10.0.1 | ✅ Current |

### 🔄 Updates Available

| Package | Current | Latest | Priority | Breaking Changes |
|---------|---------|--------|----------|------------------|
| **Spectre.Console.Json** | 0.49.1 | 0.54.0 | Low | Minor API changes |

### ⚠️ Pre-release Packages (Monitor)

| Package | Current | Status | Notes |
|---------|---------|--------|-------|
| **EFCore.NamingConventions** | 10.0.0-rc.2 | Pre-release | Awaiting stable 10.0.0 |
| **EFCore.BulkExtensions.PostgreSql** | 10.0.0-rc.2 | Pre-release | Awaiting stable 10.0.0 |
| **EFCore.BulkExtensions.SqlServer** | 10.0.0-rc.2 | Pre-release | Awaiting stable 10.0.0 |
| **Microsoft.Extensions.AI.OpenAI** | 10.1.1-preview.1 | Pre-release | AI SDK preview |
| **Microsoft.Extensions.AI.AzureAIInference** | 10.0.0-preview.1 | Pre-release | AI SDK preview |
| **System.CommandLine** | 2.0.0-beta4 | Pre-release | Long-term beta |
| **ModelContextProtocol** | 0.5.0-preview.1 | Pre-release | MCP protocol preview |

### 📌 Intentionally Pinned

| Package | Version | Reason |
|---------|---------|--------|
| **Microsoft.AspNetCore.Mvc** | 2.3.9 | Legacy compatibility - 10.0.1 doesn't exist |
| **Microsoft.AspNetCore.Mvc.Core** | 2.3.9 | Legacy compatibility - 10.0.1 doesn't exist |
| **Aspire.Hosting.Elasticsearch** | 13.0.0 | 13.1.0 not yet published |

---

## Package Group Synchronization Rules

These packages MUST remain synchronized:

### Entity Framework Core Group (10.0.1)
```
Microsoft.EntityFrameworkCore.*  → 10.0.1
Npgsql.EntityFrameworkCore.PostgreSQL → 10.0.0
```

### Aspire Hosting Group (13.1.0)
```
Aspire.Hosting.*  → 13.1.0
Exception: Aspire.Hosting.Elasticsearch → 13.0.0 (awaiting publish)
```

### OpenTelemetry Group (1.14.0)
```
OpenTelemetry.*  → 1.14.0
Exception: OpenTelemetry.Exporter.Prometheus.AspNetCore → 1.14.0-beta.1
Exception: OpenTelemetry.Instrumentation.Quartz → 1.10.0-beta.1
```

### Wolverine Group (5.9.2)
```
WolverineFx.*  → 5.9.2
```

### Microsoft.Extensions Group (10.0.x)
```
Microsoft.Extensions.* → 10.0.1 (base)
Microsoft.Extensions.Http.Resilience → 10.1.0
Microsoft.Extensions.ServiceDiscovery → 10.1.0
```

### xUnit v3 Group (3.2.1)
```
xunit.v3.* → 3.2.1
```

---

## Release Notes & Breaking Changes

### Recent Notable Updates

#### Aspire 13.1.0 (December 2025)
- SDK-based hosting model (Aspire.AppHost.Sdk)
- Improved resource graph orchestration
- Better health check integration
- **Migration**: Removed explicit `Aspire.Hosting.AppHost` reference

#### WolverineFx 5.9.2 (December 2025)
- Performance improvements in message handling
- Better PostgreSQL integration
- No breaking changes from 5.9.x

#### OpenTelemetry 1.14.0 (November 2025)
- Enhanced .NET 10 support
- Improved logging integration
- Self-diagnostics improvements

#### FluentValidation 12.1.1 (December 2025)
- Bug fixes
- No breaking changes from 12.x

---

## Recommendations

### Immediate Actions
1. **None required** - All critical packages are current

### Short-term (Next Sprint)
1. Update `Spectre.Console.Json` from 0.49.1 to 0.54.0

### Monitor
1. Watch for stable releases of EF Core extension packages (10.0.0)
2. Monitor Microsoft.Extensions.AI for stable release
3. Track System.CommandLine for stable release

### Do Not Update
1. `Microsoft.AspNetCore.Mvc*` - Stay on 2.3.9 (10.x not available)
2. `Aspire.Hosting.Elasticsearch` - Stay on 13.0.0 until 13.1.0 published

---

## Version Check Commands

```bash
# Check for outdated packages
dotnet list package --outdated

# Check for vulnerable packages
dotnet list package --vulnerable

# Update specific package
dotnet add package [PackageName] --version [Version]
```

---

## References

- [KB-114] Central Package Management (CPM)
- [GL-013] Dependency Management Policy
- [ADR-001] Wolverine over MediatR
- [ADR-003] Aspire Orchestration

---

**Next Review**: February 11, 2026  
**Maintained by**: @TechLead
