# License Compliance Review - 2025-12-31

## Executive Summary

This document summarizes the license compliance review of all project dependencies conducted by @Legal. The review covers .NET NuGet packages and npm packages across all frontend applications (Admin, Management, Store).

**Overall Status: COMPLIANT**

All dependencies are compatible with project requirements for GDPR, NIS2, BITV 2.0 compliance. No GPL or copyleft licenses that could contaminate the codebase were found.

## Review Scope

### .NET Dependencies
- **Source**: Directory.Packages.props and transitive dependencies from AppHost project
- **Total Packages**: 85+ (including transitive)
- **License Types**: MIT, Apache-2.0, BSD variants, ISC

### npm Dependencies
- **Sources**: 
  - frontend/Admin/package.json
  - frontend/Management/package.json  
  - frontend/Store/package.json
- **Total Packages**: 200+ (including transitive across all frontends)
- **License Types**: MIT, Apache-2.0, BSD variants, ISC, BlueOak-1.0.0

## License Inventory

### .NET Packages (Top-Level)

| Package | Version | License | Compliance Status |
|---------|---------|---------|-------------------|
| Microsoft.AspNetCore.Identity.EntityFrameworkCore | 10.0.1 | MIT | ✅ Compliant |
| Microsoft.AspNetCore.OpenApi | 10.0.1 | MIT | ✅ Compliant |
| Microsoft.EntityFrameworkCore.Sqlite | 10.0.1 | MIT | ✅ Compliant |
| Swashbuckle.AspNetCore | 6.8.0 | MIT | ✅ Compliant |
| Aspire.Hosting | 13.1.0 | Apache-2.0 | ✅ Compliant |
| Aspire.Hosting.AppHost | 13.1.0 | Apache-2.0 | ✅ Compliant |
| WolverineFx | 5.9.2 | Apache-2.0 | ✅ Compliant |
| Microsoft.EntityFrameworkCore | 10.0.1 | MIT | ✅ Compliant |
| Npgsql.EntityFrameworkCore.PostgreSQL | 10.0.0 | PostgreSQL License | ✅ Compliant |
| Elastic.Clients.Elasticsearch | 8.15.0 | Apache-2.0 | ✅ Compliant |
| RabbitMQ.Client | 7.1.2 | Apache-2.0 | ✅ Compliant |
| Quartz | 3.11.0 | Apache-2.0 | ✅ Compliant |
| Polly | 8.4.0 | BSD-3-Clause | ✅ Compliant |
| xunit | 2.7.1 | Apache-2.0 | ✅ Compliant |
| FluentAssertions | 6.12.1 | Apache-2.0 | ✅ Compliant |
| MailKit | 4.7.1 | MIT | ✅ Compliant |

### npm Packages (Top-Level - Admin Frontend)

| Package | Version | License | Compliance Status |
|---------|---------|---------|-------------------|
| axios | ^1.7.7 | MIT | ✅ Compliant |
| vue | ^3.5.13 | MIT | ✅ Compliant |
| vue-router | ^4.4.5 | MIT | ✅ Compliant |
| pinia | ^2.2.6 | MIT | ✅ Compliant |
| vue-i18n | ^10.0.2 | MIT | ✅ Compliant |
| date-fns | ^3.6.0 | MIT | ✅ Compliant |
| tailwindcss | ^3.4.15 | MIT | ✅ Compliant |
| vite | ^7.3.0 | MIT | ✅ Compliant |
| typescript | ^5.9.3 | Apache-2.0 | ✅ Compliant |
| eslint | ^9.15.0 | MIT | ✅ Compliant |
| prettier | ^3.3.3 | MIT | ✅ Compliant |
| vitest | ^4.0.16 | MIT | ✅ Compliant |
| @playwright/test | ^1.48.2 | Apache-2.0 | ✅ Compliant |

### npm Packages (Top-Level - Management Frontend)

| Package | Version | License | Compliance Status |
|---------|---------|---------|-------------------|
| axios | ^1.7.7 | MIT | ✅ Compliant |
| vue | ^3.5.13 | MIT | ✅ Compliant |
| vue-router | ^4.4.5 | MIT | ✅ Compliant |
| pinia | ^2.2.6 | MIT | ✅ Compliant |
| typescript | ^5.9.3 | Apache-2.0 | ✅ Compliant |
| tailwindcss | ^3.4.15 | MIT | ✅ Compliant |
| vite | ^6.0.5 | MIT | ✅ Compliant |
| eslint | ^9.15.0 | MIT | ✅ Compliant |
| vitest | ^2.1.8 | MIT | ✅ Compliant |

### npm Packages (Top-Level - Store Frontend)

| Package | Version | License | Compliance Status |
|---------|---------|---------|-------------------|
| axios | ^1.7.7 | MIT | ✅ Compliant |
| vue | ^3.5.13 | MIT | ✅ Compliant |
| vue-router | ^4.4.5 | MIT | ✅ Compliant |
| pinia | ^2.2.6 | MIT | ✅ Compliant |
| vue-i18n | ^10.0.2 | MIT | ✅ Compliant |
| date-fns | ^3.6.0 | MIT | ✅ Compliant |
| tailwindcss | ^4.1.18 | MIT | ✅ Compliant |
| daisyui | ^5.5.14 | MIT | ✅ Compliant |
| vite | ^6.0.5 | MIT | ✅ Compliant |
| typescript | ^5.9.3 | Apache-2.0 | ✅ Compliant |
| eslint | ^9.15.0 | MIT | ✅ Compliant |
| vitest | ^4.0.16 | MIT | ✅ Compliant |
| @playwright/test | ^1.48.2 | Apache-2.0 | ✅ Compliant |

## Compliance Assessment

### GDPR Compliance
- ✅ All dependencies use permissive licenses that don't restrict data handling
- ✅ No GPL licenses that could impose copyleft requirements on user data
- ✅ Compatible with open-source distribution requirements

### NIS2 Compliance  
- ✅ No dependencies with restrictive licensing that could impact security requirements
- ✅ All licenses allow commercial use and modification
- ✅ Compatible with EU cybersecurity framework requirements

### BITV 2.0 Compliance
- ✅ Accessibility tools (@axe-core/cli) use MIT license
- ✅ No licensing conflicts with German accessibility standards
- ✅ Compatible with public sector software requirements

## Risk Assessment

### Severity Levels
- **Critical**: GPL/Copyleft contamination - NONE FOUND
- **High**: Proprietary license conflicts - NONE FOUND  
- **Medium**: License version conflicts - NONE FOUND
- **Low**: Outdated license information - NONE FOUND

### Identified Issues
None. All dependencies use standard permissive licenses.

## Recommendations

1. **Continue Monitoring**: Set up automated license scanning in CI/CD pipeline
2. **Dependency Updates**: Regular review of dependency licenses during updates
3. **Third-Party Audit**: Annual comprehensive audit of all dependencies
4. **License Documentation**: Maintain this inventory as part of compliance records

## Review Methodology

- Used `license-checker` npm package for JavaScript dependencies
- Used `dotnet list package --include-transitive` for .NET dependencies  
- Cross-referenced licenses with official package repositories
- Verified compatibility with project license (MIT) and regulatory requirements

## Sign-off

**@Legal Review Completed**: 2025-12-31
**Next Review Due**: 2026-06-30
**Reviewer**: @Legal Agent