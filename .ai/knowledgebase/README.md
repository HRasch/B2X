# B2Connect Knowledge Base

## Übersicht

Diese Knowledge Base enthält umfassende Dokumentation und Referenzmaterialien für das B2Connect-Projekt, einschließlich Architektur, Technologien, Prozesse und Best Practices.

## Inhaltsverzeichnis

### 🎨 Frontend & UI
- **[SoftUI Admin Theme](softui-admin-theme.md)** - Umfassende Dokumentation des SoftUI Design-Systems für die Admin-Oberfläche
  - Komponenten-Architektur und Implementierung
  - Tailwind CSS Konfiguration
  - Theme-System und Dark Mode
  - Barrierefreiheit und Performance
  - Troubleshooting und Best Practices

### 🏗️ Architektur & Entscheidungen
- **[Architecture Decisions](/Users/holger/Documents/Projekte/B2Connect/.ai/architecture/decisions/)** - ADRs und Design-Entscheidungen
  - Event-Driven Architecture, Service Boundaries, Domain Services
  - Email Provider Selection, DaisyUI Framework, Migration Plans
  - Component Inventory, Design System Setup, Dependency Audits
- **[Architecture Guides](/Users/holger/Documents/Projekte/B2Connect/.ai/architecture/guides/)** - Technische Leitfäden
- **[Architecture Patterns](/Users/holger/Documents/Projekte/B2Connect/.ai/architecture/patterns/)** - Entwurfsmuster
- **[Architecture Reviews](/Users/holger/Documents/Projekte/B2Connect/.ai/architecture/reviews/)** - Code-Reviews und Audits

### 🔧 Technologien & Frameworks
- **Vue.js 3** - Composition API, TypeScript Integration
- **Tailwind CSS** - Utility-First CSS Framework
- **Vite** - Build-Tool und Development Server
- **Pinia** - State Management
- **Vue Router** - Client-side Routing
- **.NET 10** - Backend Framework mit Wolverine CQRS
- **Wolverine** - Command Query Responsibility Segregation
- **Entity Framework Core** - ORM und Datenbankzugriff
### 📚 **Externe Module & Updates**
- **[Dependency Updates](/Users/holger/Documents/Projekte/B2Connect/.ai/knowledgebase/dependency-updates/)** - Umfassende Dokumentation aller externen Bibliotheken
  - .NET Packages (Wolverine, EF Core, Quartz, RabbitMQ, Swashbuckle)
  - Frontend Dependencies (Vue.js, Vite, Axios, Playwright, TypeScript)
  - Migration-Guidelines und Breaking Changes
  - How-To Guides für Updates und neue Dependencies
  - Security- und License-Informationen
### 📋 Projekte & Features
- **[Project Documentation](/Users/holger/Documents/Projekte/B2Connect/.ai/projects/)** - Feature-Spezifikationen und Projektpläne
  - MVP Backlog, Sprint Planning, Phase 1-2 Reviews
  - Component Inventory, UI Modernization, Dependency Updates
  - Tenant Theming, Search Enhancements, Email Integration
- **[Requirements](/Users/holger/Documents/Projekte/B2Connect/.ai/projects/requirements/)** - Anforderungsanalysen
- **[Sprints](/Users/holger/Documents/Projekte/B2Connect/.ai/projects/sprints/)** - Sprint-Planung und Tracking
- **[Status](/Users/holger/Documents/Projekte/B2Connect/.ai/projects/status/)** - Projektstatus und Fortschritt

### ⚙️ Operations & DevOps
- **[Operations Guide](/Users/holger/Documents/Projekte/B2Connect/.ai/operations/)** - Betriebs- und Wartungsdokumentation
  - Compliance Adoption, Scalability Guide, Deployment
  - Monitoring, Maintenance, Incident Reports
  - Cost Optimization, License Inventory, Security Audits
- **[Deployment](/Users/holger/Documents/Projekte/B2Connect/.ai/operations/deployment/)** - Deployment-Strategien
- **[Monitoring](/Users/holger/Documents/Projekte/B2Connect/.ai/operations/monitoring/)** - Überwachung und Metriken
- **[Maintenance](/Users/holger/Documents/Projekte/B2Connect/.ai/operations/maintenance/)** - Wartungspläne

### 🤖 Agent System & Prozesse
- **[Agent Processes](/Users/holger/Documents/Projekte/B2Connect/.ai/processes/)** - Agent-Koordination und Governance
  - Subagent Delegation, Communication Patterns, Context Optimization
  - Agent Removal Guide, Governance Metrics, Learning System
- **[Communication](/Users/holger/Documents/Projekte/B2Connect/.ai/processes/communication/)** - Kommunikationsprotokolle
- **[Governance](/Users/holger/Documents/Projekte/B2Connect/.ai/processes/governance/)** - Governance-Frameworks
- **[Workflows](/Users/holger/Documents/Projekte/B2Connect/.ai/processes/workflows/)** - Entwicklungs-Workflows

### 🧪 Testing & Quality Assurance
- **Unit Tests** - Vitest, Jest, xUnit
- **E2E Tests** - Playwright
- **Component Tests** - Vue Test Utils
- **Performance Tests** - Lighthouse, Web Vitals
- **Integration Tests** - API und Service-Tests

### 🚀 DevOps & Deployment
- **CI/CD** - GitHub Actions
- **Containerization** - Docker, Kubernetes
- **Infrastructure** - Azure, Cloud-Deployment
- **Monitoring** - Application Insights, Health Checks

### 🔒 Security & Compliance
- **Authentication** - JWT, OAuth, Multi-Factor
- **Authorization** - Role-Based Access Control
- **Data Protection** - GDPR, NIS2, AI Act Compliance
- **Security Best Practices** - OWASP Guidelines, Vulnerability Scanning

## Suchen & Navigation

### Nach Thema suchen
- Verwende die Suchfunktion deiner IDE (Cmd+Shift+F)
- Suche nach Schlüsselwörtern wie "SoftUI", "DDD", "CQRS", "ADR", "Vue.js", etc.

### Nach Dateityp filtern
- `*.md` - Markdown Dokumentation
- `*.vue` - Vue Komponenten
- `*.ts` - TypeScript Dateien
- `*.config.*` - Konfigurationsdateien

### Wichtige Ordner-Struktur
```
.ai/
├── knowledgebase/          # ← Du bist hier (zentraler Index)
├── architecture/           # Architektur-Entscheidungen und -Guides
├── projects/               # Projekt-Dokumentation und -Status
├── operations/             # Betrieb, Deployment, Monitoring
├── processes/              # Agent-System und Workflows
└── [weitere spezialisierte Ordner]
```

### Schnellzugriff auf häufig gesuchte Themen
- **Architektur-Entscheidungen**: `.ai/architecture/decisions/`
- **Projekt-Status**: `.ai/projects/status/`
- **Deployment-Guides**: `.ai/operations/deployment/`
- **Agent-Dokumentation**: `.ai/processes/`
- **Compliance**: `.ai/operations/` (Suche nach "compliance")

## Beitragende Richtlinien

### Neue Einträge hinzufügen
1. Erstelle eine neue Markdown-Datei im entsprechenden Unterordner
2. Verwende klare, beschreibende Dateinamen (z.B. `softui-admin-theme.md`)
3. Folge dem etablierten Format und Stil
4. Aktualisiere diesen Index

### Bestehende Einträge aktualisieren
1. Suche nach relevanten Dateien
2. Aktualisiere Inhalte mit neuen Informationen
3. Füge Versions- und Datumsangaben hinzu
4. Stelle sicher, dass Links und Referenzen korrekt sind

### Qualitätsstandards
- **Vollständigkeit**: Umfassende, aber fokussierte Informationen
- **Aktualität**: Regelmäßige Überprüfung und Aktualisierung
- **Konsistenz**: Einheitlicher Stil und Formatierung
- **Barrierefreiheit**: Klare Sprache und Struktur

## Versionshistorie

### v1.2.0 (1. Januar 2026)
- ✅ Dependency Updates Bereich hinzugefügt
- ✅ Umfassende Dokumentation externer Module integriert
- ✅ Migration-Guidelines und How-To Guides verfügbar
- ✅ Vollständige Übersicht aller .NET und Frontend Dependencies
- ✅ Update-Prioritäten und Migrations-Strategien dokumentiert

### v1.1.0 (1. Januar 2026)
- ✅ Erweiterte Knowledge Base Index mit vollständiger .ai/ Struktur
- ✅ Architektur-Dokumentation hinzugefügt (ADRs, Guides, Patterns)
- ✅ Projekt-Dokumentation integriert (Requirements, Sprints, Status)
- ✅ Operations & DevOps Dokumentation hinzugefügt
- ✅ Agent-System und Prozess-Dokumentation integriert
- ✅ Vollständige Navigation und Suchhilfen

### v1.0.0 (1. Januar 2026)
- ✅ Initiale Knowledge Base Struktur
- ✅ SoftUI Admin Theme Dokumentation
- ✅ Index und Navigation
- ✅ Beitragende Richtlinien

---

**Letzte Aktualisierung**: 1. Januar 2026
**Maintainer**: GitHub Copilot (AI Assistant)
**Kontakt**: [Issues und Verbesserungsvorschläge](https://github.com/your-org/b2connect/issues)