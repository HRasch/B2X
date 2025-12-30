# Knowledgebase Index

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
- 30.12.2025: Index created and structure established

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

**Last Updated:** 30.12.2025
**Maintained by:** @SARAH, @Architect
**Access Level:** All Agents
