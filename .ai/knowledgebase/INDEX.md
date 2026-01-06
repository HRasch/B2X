---
docid: KB-INDEX
title: Knowledgebase Index
owner: GitHub Copilot (AI)
status: Active
---

# Knowledgebase Index

**DocID**: `KB-INDEX`

This index lists curated knowledgebase articles to help agents and developers implement best practices for the project.

## Quick Reference by DocID

| DocID | Article | Description |
|-------|---------|-------------|
| `KB-001` | [csharp-14.md](csharp-14.md) | C# 14 language highlights |
| `KB-002` | [dotnet-features.md](dotnet-features.md) | .NET runtime, SDK highlights (.NET 10) |
| `KB-003` | [dotnet-breaking-changes.md](dotnet-breaking-changes.md) | Compatibility rules, breaking changes |
| `KB-004` | [dotnet-identity.md](dotnet-identity.md) | ASP.NET Core Identity best practices |
| `KB-005` | [dotnet-localization.md](dotnet-localization.md) | Globalization/localization patterns |
| `KB-006` | [wolverine.md](wolverine.md) | Wolverine messaging & CQRS |
| `KB-007` | [vue.md](vue.md) | Vue.js 3 overview |
| `KB-008` | [pinia.md](pinia.md) | Pinia state management |
| `KB-009` | [vite.md](vite.md) | Vite tooling notes |
| `KB-010` | [owasp-top-ten.md](owasp-top-ten.md) | OWASP Top Ten security |
| `KB-011` | [patterns-antipatterns.md](patterns-antipatterns.md) | Design patterns & antipatterns |
| `KB-012` | [repo-mapping.md](repo-mapping.md) | Repository technology mapping |
| `KB-013` | [governance.md](governance.md) | Governance & P0 Playbook |
| `KB-014` | [git-commit-strategy.md](git-commit-strategy.md) | Git commit conventions |
| `KB-015` | [tools-and-tech/github-copilot-models.md](tools-and-tech/github-copilot-models.md) | GitHub Copilot AI models & pricing |
| `KB-016` | [tools-and-tech/ai-cost-optimization.md](tools-and-tech/ai-cost-optimization.md) | AI cost monitoring & optimization |
| `KB-018` | [tools-and-tech/local-llm-models-2025.md](tools-and-tech/local-llm-models-2025.md) | Beste lokale LLM-Modelle für Entwicklung |
| `KB-020` | [tools-and-tech/archunitnet.md](tools-and-tech/archunitnet.md) | ArchUnitNET architecture testing framework |
| `KB-031` | [tools-and-tech/shouldly.md](tools-and-tech/shouldly.md) | Shouldly assertion framework for .NET testing |
| `KB-052` | [tools-and-tech/roslyn-mcp.md](tools-and-tech/roslyn-mcp.md) | Roslyn MCP Server for code analysis |
| `KB-021` | [enventa-trade-erp.md](enventa-trade-erp.md) | enventa Trade ERP integration guide |
| `KB-026` | [tools-and-tech/monaco-editor-vue.md](tools-and-tech/monaco-editor-vue.md) | Monaco Editor Vue.js integration guide |
| `KB-027` | [best-practices/email-dark-mode.md](best-practices/email-dark-mode.md) | Email Dark Mode Best Practices |
| `KB-PILOT` | [code-quality-pilot-migration.md](code-quality-pilot-migration.md) | Code quality pilot migration results & patterns |
| `KB-LESSONS` | [lessons.md](lessons.md) | Lessons learned from development work |
| `KB-TS-ANY-ELIMINATION` | [typescript-any-elimination-success-story.md](typescript-any-elimination-success-story.md) | TypeScript any type elimination success story |

## Subdirectories

| DocID | Directory | Contents |
|-------|-----------|----------|
| `KB-ARCH` | [architecture/](architecture/) | System architecture patterns |
| `KB-PAT` | [patterns/](patterns/) | Design & integration patterns |
| `KB-BP` | [best-practices/](best-practices/) | Coding standards, guidelines |
| `KB-TOOL` | [tools-and-tech/](tools-and-tech/) | Software versions, dependencies |
| `KB-SW` | [software/](software/) | Dependency update logs |
| `KB-OPS` | [operations/](operations/) | Operations guides |
| `KB-SEARCH` | [search/](search/) | Search/Elasticsearch guides |

## Purpose
Zentrale Übersicht aller dokumentierten Software-Versionen, Änderungen und Best Practices. Ermöglicht schnelle Recherche über alle Agenten.

## Struktur

```
.ai/knowledgebase/
├── INDEX.md (du bist hier)
├── README.md
├── architecture/           # System Architecture Patterns
├── best-practices/         # Coding Standards, Guidelines
├── domain-knowledge/       # Domain-specific Knowledge
├── patterns/               # Design & Integration Patterns
├── tools-and-tech/         # Software Versions, Dependencies
│   ├── nodejs/
│   ├── python/
│   ├── frameworks/
│   └── [more software]
└── software/               # NEW: Dependency Update Logs
    ├── {software-name}/
    │   ├── v1.0.md
    │   ├── v2.0.md         # {old-version} → {new-version}
    │   └── VERSIONS.md     # Overview & Links
    └── [more software]
```

## Software Versions Inventory

### Active Dependencies

| Software | Current | Latest | Updated | Tags | Notes |
|----------|---------|--------|---------|------|-------|
| Node.js | TBD | 20.x | - | runtime, core | Define based on project |
| React | TBD | 18.x | - | frontend, ui | - |
| PostgreSQL | TBD | 16.x | - | database, backend | - |
| Docker | TBD | 25.x | - | devops, infra | - |
| TypeScript | TBD | 5.x | - | tooling, build | - |

## Search by Tag

### 🔴 Breaking Changes
- [Node.js v16→v18](./software/nodejs/v16-to-v18.md)
- [React v17→v18](./software/react/v17-to-v18.md)

### 🟢 Security Fixes
- [PostgreSQL 15.2→15.3](./software/postgresql/v15.2-to-v15.3.md)

### ⚡ Performance
- [Node.js v18→v20](./software/nodejs/v18-to-v20.md)

### 🛠️ New Features
- [TypeScript 5.0](./software/typescript/v4-to-v5.md)

## Recently Updated
- 05.01.2026: Added Email Dark Mode Best Practices guide ([best-practices/email-dark-mode.md](./best-practices/email-dark-mode.md))
- 05.01.2026: Added Monaco Editor Vue.js integration guide ([tools-and-tech/monaco-editor-vue.md](./tools-and-tech/monaco-editor-vue.md))
- 03.01.2026: Added Email Template Best Practices guide ([best-practices/email-templates.md](./best-practices/email-templates.md))
- 02.01.2026: Updated enventa Trade ERP integration guide with resilience pipeline, transaction scopes, and production fixes ([enventa-trade-erp.md](./enventa-trade-erp.md))
- 02.01.2026: Added ERP architecture review lessons to lessons learned ([lessons.md](./lessons.md))
- 02.01.2026: Added ArchUnitNET architecture testing guide ([tools-and-tech/archunitnet.md](./tools-and-tech/archunitnet.md))
- 02.01.2026: Added AI Cost Monitoring & Optimization guide ([tools-and-tech/ai-cost-optimization.md](./tools-and-tech/ai-cost-optimization.md))
- 02.01.2026: Added GitHub Copilot AI Models & Pricing guide ([tools-and-tech/github-copilot-models.md](./tools-and-tech/github-copilot-models.md))
- 31.12.2025: Added Elasticsearch e‑commerce search & recommendations article (search-elasticsearch-ecommerce.md)
 - 31.12.2025: Restored key dependency guidance (OpenTelemetry, Elastic.Clients.Elasticsearch, EFCore, Serilog, Polly, Playwright) into `dependency-updates/`.
 - 31.12.2025: Restored additional dependency guidance (AutoMapper, Azure.Identity, Yarp.ReverseProxy, Swashbuckle.AspNetCore, axios, typescript, FluentValidation) into `dependency-updates/`.

## How Agents Use This Index

### @Architect
→ `architecture/` für System Design Patterns
→ `software/*/breaking-changes` für Integration Planning

### @Backend
→ `software/nodejs/`, `software/postgresql/` für API/Data Changes
→ `patterns/` für Integration Patterns

### @Frontend
→ `software/react/`, `software/typescript/` für Component Updates
→ `best-practices/` für Code Standards

### @Security
→ All `software/*/security-fixes` entries
→ `best-practices/security/`

### @DevOps
→ `software/docker/`, `software/kubernetes/` für Deployment
→ `tools-and-tech/` für Infrastructure

### @TechLead
→ `best-practices/code-quality/`
→ `software/*/migration-checklists`

## Adding New Software Version

1. Create folder: `.ai/knowledgebase/software/{software-name}/`
2. Create summary: `.ai/knowledgebase/software/{software-name}/v{old}--to--v{new}.md`
3. Update this INDEX.md:
   - Add row to "Software Versions Inventory"
   - Add link to appropriate tag section
   - Update "Recently Updated" timestamp
4. SARAH validates and updates

## File Naming Convention

```
{software-name}/{version-old}--to--{version-new}.md

Examples:
- nodejs/16--to--18.md
- react/17--to--18.md
- postgres/15.2--to--15.3.md
```

## Content Guidelines

✓ **DO:**
- Use bullets for readability
- Link to official docs
- Focus on impact & action items
- Include migration checklists
- Separate by agent-perspective

✗ **DON'T:**
- Copy entire official documentation
- Include outdated information
- Duplicate content
- Write prose when bullets work

## Tag System

| Tag | Meaning | Priority |
|-----|---------|----------|
| breaking-changes | API/Architecture Changes | 🔴 High |
| security | CVE Fixes, Security Updates | 🔴 High |
| performance | Speed Improvements | 🟡 Medium |
| new-features | New Capabilities | 🟡 Medium |
| deprecation | Features to Remove | 🟡 Medium |
| minor-update | Patch/Minor Updates | 🟢 Low |
| migration-required | Active Migration Needed | 🔴 High |

## Version Format

- Semantic Versioning: `v{major}.{minor}.{patch}`
- Range Notation: `v1.0--to--v2.0` (folder structure)
- Include Minor: `v1.0.5--to--v1.2.0` when relevant

---

**Last Updated:** 02.01.2026
**Maintained by:** @SARAH, @Architect
**Access Level:** All Agents
