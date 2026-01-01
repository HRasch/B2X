# Dependency Updates & External Modules

## Übersicht

Dieser Ordner enthält umfassende Dokumentation über externe Module, Bibliotheken und deren Updates. Hier finden Sie Informationen zu neuen Softwareversionen, Migration-Guidelines, HowTos und Best Practices für alle Dependencies des B2Connect-Projekts.

## Inhaltsverzeichnis

### 📋 **Übersichts-Dokumente**
- **[dependency-updates-2025-12-31.md](dependency-updates-2025-12-31.md)** - Umfassende Übersicht aller Dependency-Updates und Migrationspläne
  - Kritische Updates (Swashbuckle, Quartz, RabbitMQ)
  - Frontend Updates (Vue, Vite, Axios, Playwright)
  - Sicherheits- und Kompatibilitätsanalysen

### 🔧 **.NET Backend Dependencies**

#### Core Frameworks & Libraries
- **[WolverineFx.md](WolverineFx.md)** - CQRS/Message Framework (v5.9.2)
- **[Microsoft.EntityFrameworkCore.md](Microsoft.EntityFrameworkCore.md)** - ORM Framework
- **[Serilog.md](Serilog.md)** - Logging Framework
- **[Polly.md](Polly.md)** - Resilience Framework
- **[FluentValidation.md](FluentValidation.md)** - Validation Framework
- **[AutoMapper.md](AutoMapper.md)** - Object Mapping

#### Infrastructure & Services
- **[Quartz.md](Quartz.md)** - Job Scheduling (3.11.0 → 3.15.1)
- **[RabbitMQ.Client.md](RabbitMQ.Client.md)** - Message Queue (7.1.2 → 7.2.0)
- **[Swashbuckle.AspNetCore.md](Swashbuckle.AspNetCore.md)** - Swagger/OpenAPI (6.8.0 → 10.1.0)
- **[Yarp.ReverseProxy.md](Yarp.ReverseProxy.md)** - Reverse Proxy
- **[Microsoft.Extensions.Http.Resilience.md](Microsoft.Extensions.Http.Resilience.md)** - HTTP Resilience

#### Cloud & Identity
- **[Azure.Identity.md](Azure.Identity.md)** - Azure Authentication
- **[AWSSDK.Core.md](AWSSDK.Core.md)** - AWS SDK Core
- **[OpenTelemetry.md](OpenTelemetry.md)** - Observability Framework

#### Legacy & Migration
- **[Elastic.Clients.Elasticsearch.md](Elastic.Clients.Elasticsearch.md)** - Elasticsearch Client
- **[Elastic.Clients.Elasticsearch-legacy.md](Elastic.Clients.Elasticsearch-legacy.md)** - Legacy Client
- **[NEST_deprecation.md](NEST_deprecation.md)** - NEST Deprecation Guide

### 🎨 **Frontend Dependencies**

#### Core Frameworks
- **[vue.md](vue.md)** - Vue.js Framework (3.5.13 → 3.5.26)
- **[vite.md](vite.md)** - Build Tool (6.0.5 → 7.3.0)
- **[typescript.md](typescript.md)** - TypeScript Compiler

#### UI & Styling
- **[daisyui.md](daisyui.md)** - DaisyUI Component Library
- **[axios.md](axios.md)** - HTTP Client (1.7.7 → 1.13.2)

#### Testing & Quality
- **[Playwright.md](Playwright.md)** - E2E Testing Framework
- **[vuetest-deps.md](vuetest-deps.md)** - Vue Testing Dependencies

### 🛠️ **Development Tools**
- **[Tooling-Notes.md](Tooling-Notes.md)** - Development Tooling Notes

## 📊 **Update-Prioritäten**

### 🔴 **Kritisch** (Sofortige Aufmerksamkeit)
- **Swashbuckle.AspNetCore**: Major Version Jump (6.x → 10.x)
- **Vite**: Major Version Jump (6.x → 7.x)
- **Axios**: Security Updates und API Changes

### 🟡 **Hoch** (Bald angehen)
- **Quartz**: Minor Updates mit Bug Fixes
- **RabbitMQ.Client**: Stability Improvements
- **Playwright**: Testing Framework Updates

### 🟢 **Mittel/Niedrig** (Nach Bedarf)
- **Vue.js**: Patch Updates
- **TypeScript**: Minor Updates
- **Übrige Libraries**: Routine Maintenance

## 🔄 **Migrations-Strategien**

### Phase 1: Research & Planning
- ✅ Abgeschlossen: Comprehensive dependency analysis
- ✅ Abgeschlossen: Breaking changes identification
- ✅ Abgeschlossen: Compatibility testing requirements

### Phase 2: Implementation (Laufend)
- 🔄 **Backend**: Swashbuckle, Quartz, RabbitMQ Updates
- 🔄 **Frontend**: Vite, Axios, Playwright Updates
- 🔄 **Testing**: Integration tests für alle Updates

### Phase 3: Validation & Rollout
- ⏳ **QA**: Comprehensive testing across all environments
- ⏳ **Staging**: Pre-production validation
- ⏳ **Production**: Phased rollout mit Rollback-Plan

## 📖 **How-To Guides**

### Neue Dependencies hinzufügen
1. Research in `.ai/architecture/dependencies/` durchführen
2. License-Kompatibilität mit @Legal prüfen
3. Security-Audit mit @Security durchführen
4. Dokumentation in diesem Ordner erstellen
5. CI/CD Pipeline Tests hinzufügen

### Dependency Updates durchführen
1. Breaking Changes in der Dokumentation prüfen
2. Test Coverage für betroffenen Code sicherstellen
3. Update in Staging-Environment testen
4. Rollback-Plan vorbereiten
5. Production Deployment mit Monitoring

### Migration von Legacy Libraries
- Siehe spezifische Migrations-Guides (z.B. `NEST_deprecation.md`)
- Breaking Changes Dokumentation beachten
- Alternative Implementierungen prüfen

## 🔍 **Navigation & Suche**

### Nach Technologie filtern
- **.NET**: Suche nach ".NET", "C#", "NuGet"
- **Frontend**: Suche nach "Vue", "JavaScript", "npm"
- **Infrastructure**: Suche nach "Azure", "AWS", "Docker"

### Nach Update-Priorität
- **Critical**: "breaking changes", "security"
- **High**: "major version", "API changes"
- **Medium/Low**: "patch updates", "bug fixes"

### Nach Status
- **Pending**: Neue Research erforderlich
- **In Progress**: Updates in Arbeit
- **Completed**: Successfully migriert

## 📞 **Support & Kontakt**

Bei Fragen zu spezifischen Dependencies:
- **@Backend**: .NET Libraries und Frameworks
- **@Frontend**: JavaScript/Vue.js Dependencies
- **@DevOps**: Infrastructure und Cloud Services
- **@Security**: Security-related Updates
- **@Legal**: License und Compliance Fragen

---

**Letzte Aktualisierung**: 1. Januar 2026
**Maintainer**: @Backend, @Frontend, @DevOps
**Quelle**: `.ai/architecture/dependencies/` und Research Reports