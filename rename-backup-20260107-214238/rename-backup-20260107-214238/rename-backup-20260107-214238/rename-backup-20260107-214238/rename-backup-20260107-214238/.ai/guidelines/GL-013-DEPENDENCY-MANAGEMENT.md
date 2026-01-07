---
docid: GL-013
title: Dependency Management Policy
owner: "@TechLead"
status: Active
---

# GL-013: Dependency Management Policy

**DocID**: `GL-013`  
**Last Updated**: 5. Januar 2026  
**Owner**: @TechLead

---

## Purpose

Establish consistent dependency management across B2X to prevent version drift, security vulnerabilities, and breaking changes.

---

## Architecture

### Frontend Monorepo Structure

```
frontend/
├── package.json          # Workspace root (npm workspaces)
├── Store/package.json    # Customer-facing store
├── Admin/package.json    # Admin backend portal
└── Management/package.json # Tenant management
```

**Benefits:**
- Single `node_modules` with hoisted dependencies
- Unified versioning across projects
- Single `npm install` command
- Easier dependency auditing

### Commands

```bash
# From frontend/ directory:
npm install                    # Install all workspaces
npm run dev:store             # Dev server for Store
npm run dev:admin             # Dev server for Admin
npm run dev:management        # Dev server for Management
npm run type-check:all        # Type-check all projects
npm run test:all              # Test all projects
npm outdated                  # Check for updates
```

---

## Update Categories

### 🟢 Safe Updates (Auto-Merge Allowed)

| Type | Description | Review Required |
|------|-------------|-----------------|
| Patch | Bug fixes (x.x.1 → x.x.2) | CI only |
| Minor | New features, backward compatible (x.1.x → x.2.x) | CI only |

**Auto-merge criteria:**
- All CI checks pass
- No security vulnerabilities introduced
- Grouped by ecosystem (Vue, testing, etc.)

### 🟡 Cautious Updates (Review Required)

| Package | Current | Latest | Risk | Reviewer |
|---------|---------|--------|------|----------|
| pinia | 2.x | 3.x | Breaking API changes | @Frontend |
| vue-i18n | 10.x | 11.x | Composition API changes | @Frontend |
| date-fns | 3.x | 4.x | Tree-shaking changes | @Frontend |

**Review process:**
1. Create feature branch
2. Apply update in isolation
3. Run full test suite
4. Document migration steps
5. @TechLead approval required

### 🔴 Critical Updates (Immediate Action)

| Type | Action | Responsibility |
|------|--------|----------------|
| Security vulnerability | Patch within 24h | @Security + @DevOps |
| Breaking production | Hotfix immediately | @TechLead |

---

## Automation: Dependabot

### Configuration Location
`.github/dependabot.yml`

### Schedule
- **When**: Weekly on Mondays at 06:00 CET
- **Limit**: 10 open PRs maximum

### Grouping Strategy

| Group | Packages | Rationale |
|-------|----------|-----------|
| vue-ecosystem | vue, vue-router, pinia, vue-i18n | Related changes |
| build-tools | vite, typescript, vue-tsc | Build pipeline |
| testing | vitest, playwright, test-utils | Test infrastructure |
| code-quality | eslint, prettier, typescript-eslint | Linting tools |
| styling | tailwindcss, daisyui, postcss | CSS framework |

### Ignored Major Updates
These require manual migration planning:
- `pinia` major versions
- `vue-i18n` major versions
- `date-fns` major versions

---

## Workflow

### Weekly Dependency Review

```mermaid
graph LR
    A[Dependabot PR] --> B{CI Passes?}
    B -->|No| C[Fix or Close]
    B -->|Yes| D{Major Version?}
    D -->|No| E[Auto-merge]
    D -->|Yes| F[Manual Review]
    F --> G[@TechLead Approval]
    G --> H[Merge with Migration Notes]
```

### Monthly Audit (First Monday)

1. **Run security audit**: `npm audit`
2. **Check outdated**: `npm outdated`
3. **Review ignored updates**: Evaluate major version upgrades
4. **Update documentation**: Record decisions in ADRs

---

## Security

### Vulnerability Response

| Severity | Response Time | Action |
|----------|---------------|--------|
| Critical | 4 hours | Emergency patch |
| High | 24 hours | Priority fix |
| Medium | 1 week | Next sprint |
| Low | 1 month | Backlog |

### Audit Commands

```bash
# Frontend
cd frontend && npm audit

# Backend
dotnet list package --vulnerable

# Full report
npm audit --json > audit-report.json
```

---

## Responsibilities

| Role | Responsibility |
|------|----------------|
| @Frontend | Vue ecosystem updates, migration testing |
| @Backend | .NET/NuGet updates |
| @DevOps | Dependabot config, CI integration |
| @Security | Vulnerability assessment |
| @TechLead | Major version approval, policy updates |
| @SARAH | Quality-gate coordination |

---

## Related Documents

- [INS-002] Frontend Instructions
- [ADR-INDEX] Architecture Decision Records
- [KB-LESSONS] Lessons Learned

---

**Maintained by**: @TechLead  
**Review Cycle**: Monthly
