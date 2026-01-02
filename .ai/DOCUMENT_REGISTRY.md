# 📚 Document Registry

**Version:** 1.0  
**Last Updated:** 31. Dezember 2025  
**Owner:** @SARAH

---

## Purpose

This registry provides **stable document identifiers (DocIDs)** for cross-referencing documentation across the project. Use DocIDs instead of file paths when referencing documents to ensure links remain valid after file moves or renames.

---

## Document Identifier Format

```
[PREFIX]-[NUMBER]-[SHORT_NAME]
```

**Prefixes by Category:**

| Prefix | Category | Location | Owner |
|--------|----------|----------|-------|
| `DOC` | General Documentation | `docs/` | @TechLead |
| `ADR` | Architecture Decision Records | `.ai/decisions/` | @Architect |
| `KB` | Knowledgebase Articles | `.ai/knowledgebase/` | GitHub Copilot |
| `WF` | Workflows | `.ai/workflows/` | @SARAH |
| `GL` | Guidelines | `.ai/guidelines/` | @TechLead |
| `REQ` | Requirements | `.ai/requirements/` | @ProductOwner |
| `SPR` | Sprint Documents | `.ai/sprint/` | @ScrumMaster |
| `TPL` | Templates | `.ai/templates/` | @SARAH |
| `CFG` | Configuration | `.ai/config/` | @DevOps |
| `CMP` | Compliance | `.ai/compliance/` | @Security/@Legal |
| `AGT` | Agent Definitions | `.github/agents/` | @SARAH |
| `PRM` | Prompts | `.github/prompts/` | @SARAH |
| `INS` | Instructions | `.github/instructions/` | @TechLead |

---

## Registry: Architecture Decision Records (ADR-*)

| DocID | Title | File Path | Status |
|-------|-------|-----------|--------|
| `ADR-001` | Wolverine over MediatR | `.ai/decisions/ADR-001-wolverine-over-mediatr.md` | Accepted |
| `ADR-002` | Onion Architecture | `.ai/decisions/ADR-002-onion-architecture.md` | Accepted |
| `ADR-003` | Aspire Orchestration | `.ai/decisions/ADR-003-aspire-orchestration.md` | Accepted |
| `ADR-004` | PostgreSQL Multitenancy | `.ai/decisions/ADR-004-postgresql-multitenancy.md` | Accepted |
| `ADR-INDEX` | Decisions Index | `.ai/decisions/INDEX.md` | Active |

---

## Registry: Knowledgebase (KB-*)

| DocID | Title | File Path | Status |
|-------|-------|-----------|--------|
| `KB-INDEX` | Knowledgebase Index | `.ai/knowledgebase/INDEX.md` | Active |
| `KB-001` | C# 14 Features | `.ai/knowledgebase/csharp-14.md` | Active |
| `KB-002` | .NET Features | `.ai/knowledgebase/dotnet-features.md` | Active |
| `KB-003` | .NET Breaking Changes | `.ai/knowledgebase/dotnet-breaking-changes.md` | Active |
| `KB-004` | .NET Identity | `.ai/knowledgebase/dotnet-identity.md` | Active |
| `KB-005` | .NET Localization | `.ai/knowledgebase/dotnet-localization.md` | Active |
| `KB-006` | Wolverine Patterns | `.ai/knowledgebase/wolverine.md` | Active |
| `KB-007` | Vue.js 3 | `.ai/knowledgebase/vue.md` | Active |
| `KB-008` | Pinia State | `.ai/knowledgebase/pinia.md` | Active |
| `KB-009` | Vite Tooling | `.ai/knowledgebase/vite.md` | Active |
| `KB-010` | OWASP Top Ten | `.ai/knowledgebase/owasp-top-ten.md` | Active |
| `KB-011` | Patterns & Antipatterns | `.ai/knowledgebase/patterns-antipatterns.md` | Active |
| `KB-012` | Repository Mapping | `.ai/knowledgebase/repo-mapping.md` | Active |
| `KB-013` | Governance Playbook | `.ai/knowledgebase/governance.md` | Active |
| `KB-014` | Git Commit Strategy | `.ai/knowledgebase/git-commit-strategy.md` | Active |
| `KB-015` | Search/Elasticsearch | `.ai/knowledgebase/search/` | Active |
| `KB-016` | GitHub Copilot Models | `.ai/knowledgebase/tools-and-tech/github-copilot-models.md` | Active |
| `KB-017` | AI Cost Optimization | `.ai/knowledgebase/tools-and-tech/ai-cost-optimization.md` | Active |

### Knowledgebase Subdirectories

| DocID | Title | File Path |
|-------|-------|-----------|
| `KB-ARCH` | Architecture Patterns | `.ai/knowledgebase/architecture/` |
| `KB-PAT` | Design Patterns | `.ai/knowledgebase/patterns/` |
| `KB-BP` | Best Practices | `.ai/knowledgebase/best-practices/` |
| `KB-TOOL` | Tools & Tech | `.ai/knowledgebase/tools-and-tech/` |
| `KB-SW` | Software Updates | `.ai/knowledgebase/software/` |
| `KB-OPS` | Operations | `.ai/knowledgebase/operations/` |

---

## Registry: Guidelines (GL-*)

| DocID | Title | File Path | Status |
|-------|-------|-----------|--------|
| `GL-INDEX` | Guidelines Index | `.ai/guidelines/README.md` | Active |
| `GL-001` | Communication Overview | `.ai/guidelines/GL-001-COMMUNICATION-OVERVIEW.md` | Active |
| `GL-002` | Subagent Delegation | `.ai/guidelines/GL-002-SUBAGENT_DELEGATION.md` | Active |
| `GL-003` | AI Directory Usage | `.ai/guidelines/GL-003-AI-DIRECTORY-USAGE.md` | Active |
| `GL-004` | Branch Naming & Single-Topic Strategy | `.ai/guidelines/GL-004-BRANCH_NAMING_STRATEGY.md` | Active |
| `GL-005` | SARAH Quality Gate Criteria | `.ai/guidelines/GL-005-SARAH_QUALITY_GATE_CRITERIA.md` | Active |

---

## Registry: Workflows (WF-*)

| DocID | Title | File Path | Status |
|-------|-------|-----------|--------|
| `WF-001` | Context Optimization | `.ai/workflows/WF-001-context-optimization.workflow.md` | Active |
| `WF-002` | Subagent Delegation | `.ai/workflows/WF-002-subagent-delegation.workflow.md` | Active |
| `WF-003` | Dependency Upgrade | `.ai/workflows/WF-003-dependency-upgrade.workflow.md` | Active |
| `WF-004` | GitHub CLI Quick Reference | `.ai/workflows/WF-004-GITHUB_CLI_QUICK_REFERENCE.md` | Active |
| `WF-005` | GitHub CLI Implementation | `.ai/workflows/WF-005-GITHUB_CLI_IMPLEMENTATION.md` | Active |
| `WF-006` | GitHub CLI Sprint How-To | `.ai/workflows/WF-006-GITHUB_CLI_SPRINT_HOWTO.md` | Active |
| `WF-007` | GitHub Scrum/Kanban Workflow | `.ai/workflows/WF-007-GITHUB_SCRUM_KANBAN_WORKFLOW.md` | Active |
| `WF-008` | Update GitHub Issues Sprint | `.ai/workflows/WF-008-update-github-issues-sprint.md` | Active |

---

## Registry: Compliance (CMP-*)

| DocID | Title | File Path | Status |
|-------|-------|-----------|--------|
| `CMP-001` | Compliance Quick Reference | `.ai/compliance/CMP-001-COMPLIANCE_QUICK_REFERENCE.md` | Active |

---

## Registry: Sprint Documents (SPR-*)

| DocID | Title | File Path | Status |
|-------|-------|-----------|--------|
| `SPR-001` | Sprint / Iteration Template | `.ai/sprint/SPR-001-iteration-template.md` | Active |


## Registry: Logs (LOG-*)

| DocID | Title | File Path | Status |
|-------|-------|-----------|--------|
| `LOG-001` | Instructions Update Log (2025-12-31) | `.ai/logs/instructions/INSTRUCTIONS_UPDATE_2025_12_31.md` | Active |

---

## Registry: Prompts (PRM-*)

| DocID | Title | File Path | Command |
|-------|-------|-----------|---------|
| `PRM-INDEX` | Prompt Index | `.ai/collaboration/PROMPTS_INDEX.md` | - |
| `PRM-001` | Start Feature | `.github/prompts/start-feature.prompt.md` | `/start-feature` |
| `PRM-002` | Code Review | `.github/prompts/code-review.prompt.md` | `/code-review` |
| `PRM-003` | Run Tests | `.github/prompts/run-tests.prompt.md` | `/run-tests` |
| `PRM-004` | Deploy | `.github/prompts/deploy.prompt.md` | `/deploy` |
| `PRM-005` | Security Audit | `.github/prompts/security-audit.prompt.md` | `/security-audit` |
| `PRM-006` | ADR Create | `.github/prompts/adr-create.prompt.md` | `/adr-create` |
| `PRM-007` | Iteration Cycle | `.github/prompts/iteration-cycle.prompt.md` | `/iteration-cycle` |
| `PRM-008` | Bug Analysis | `.github/prompts/bug-analysis.prompt.md` | `/bug-analysis` |
| `PRM-009` | Feature Handover | `.github/prompts/feature-handover.prompt.md` | `/feature-handover` |
| `PRM-010` | Requirements Analysis | `.github/prompts/requirements-analysis.prompt.md` | `/requirements-analysis` |
| `PRM-011` | Agent Removal | `.github/prompts/agent-removal.prompt.md` | `/agent-removal` |
| `PRM-012` | Agent Creation | `.github/prompts/agent-creation.prompt.md` | `/agent-creation` |
| `PRM-013` | Context Optimization | `.github/prompts/context-optimization.prompt.md` | `/context-optimization` |
| `PRM-014` | Subagent Delegation | `.github/prompts/subagent-delegation.prompt.md` | `/subagent-delegation` |
| `PRM-015` | Project Cleanup | `.github/prompts/project-cleanup.prompt.md` | `/project-cleanup` |

---

## Registry: Instructions (INS-*)

| DocID | Title | File Path | Applies To |
|-------|-------|-----------|------------|
| `INS-001` | Backend Instructions | `.github/instructions/backend.instructions.md` | `src/api/**`, `src/services/**` |
| `INS-002` | Frontend Instructions | `.github/instructions/frontend.instructions.md` | `src/components/**`, `src/pages/**` |
| `INS-003` | Testing Instructions | `.github/instructions/testing.instructions.md` | `**/*.test.*`, `**/*.spec.*` |
| `INS-004` | DevOps Instructions | `.github/instructions/devops.instructions.md` | `.github/**`, `Dockerfile` |
| `INS-005` | Security Instructions | `.github/instructions/security.instructions.md` | `**/*` |

---

## Registry: Agents (AGT-*)

| DocID | Title | File Path | Role |
|-------|-------|-----------|------|
| `AGT-001` | SARAH | `.github/agents/sarah.agent.md` | Coordinator |
| `AGT-002` | Backend | `.github/agents/backend.agent.md` | .NET/Wolverine |
| `AGT-003` | Frontend | `.github/agents/frontend.agent.md` | Vue.js 3 |
| `AGT-004` | QA | `.github/agents/qa.agent.md` | Testing |
| `AGT-005` | Architect | `.github/agents/architect.agent.md` | System Design |
| `AGT-006` | TechLead | `.github/agents/tech-lead.agent.md` | Code Quality |
| `AGT-007` | Security | `.github/agents/security.agent.md` | Security |
| `AGT-008` | DevOps | `.github/agents/devops.agent.md` | Infrastructure |
| `AGT-009` | ScrumMaster | `.github/agents/scrum-master.agent.md` | Process |
| `AGT-010` | ProductOwner | `.github/agents/product-owner.agent.md` | Requirements |
| `AGT-011` | Legal | `.github/agents/legal.agent.md` | Compliance |
| `AGT-012` | UX | `.github/agents/ux.agent.md` | User Research |
| `AGT-013` | UI | `.github/agents/ui.agent.md` | Components |
| `AGT-014` | SEO | `.github/agents/seo.agent.md` | Search Optimization |
| `AGT-015` | GitManager | `.github/agents/git-manager.agent.md` | Git Workflow |
| `AGT-016` | DocMaintainer | `.github/agents/DocMaintainer.agent.md` | Documentation |

**DocMaintainer (AGT-016) Responsibilities:**
- Enforce and extend the DocID naming conventions (`GL-`, `WF-`, `CMP-`, `SPR-`, etc.) when new document types are introduced.
- Maintain `.ai/DOCUMENT_REGISTRY.md` entries when docs are added, renamed, or archived.
- Run periodic link checks and create audit logs under `.ai/logs/documentation/`.
- Update cross-references across `.ai/` and `.github/prompts/` to ensure links stay current.
- When policy decisions are needed (naming, retention), open an issue and notify `@SARAH`.

**Default Agent**: `@SARAH` — Coordinator and primary default for unspecified agent prompts. SARAH is the authoritative coordinator for agent workflows and quality gates.
| `AGT-016` | DocMaintainer | `.github/agents/DocMaintainer.agent.md` | Documentation |

---

## Registry: Documentation (DOC-*)

| DocID | Title | File Path | Status |
|-------|-------|-----------|--------|
| `DOC-INDEX` | Documentation Index | `docs/guides/index.md` | Active |
| `DOC-001` | Quick Start Guide | `QUICK_START_GUIDE.md` | Active |
| `DOC-002` | Quick Reference | `docs/guides/QUICK_REFERENCE.md` | Active |
| `DOC-003` | Architecture Index | `docs/architecture/INDEX.md` | Active |
| `DOC-004` | Getting Started | `docs/guides/GETTING_STARTED.md` | Active |
| `DOC-005` | Development Guide | `docs/guides/DEVELOPMENT.md` | Active |
| `DOC-006` | Project Dashboard | `PROJECT_DASHBOARD.md` | Active |
| `DOC-007` | Governance | `GOVERNANCE.md` | Active |
| `DOC-008` | UX Guide | `docs/guides/UX_GUIDE.md` | Active |

---

## How to Use Document References

### In Markdown Documents

Use DocID in square brackets for human-readable references:

```markdown
See [ADR-001] for the Wolverine decision rationale.
Refer to [KB-006] for Wolverine patterns.
Follow [GL-002] for subagent delegation rules.
```

### In Code Comments

```csharp
// Implementation follows [ADR-001] Wolverine over MediatR
// See [KB-006] for handler patterns
```

### Cross-Reference Format

When creating new documents, add a header with the DocID:

```markdown
---
docid: KB-016
title: New Knowledgebase Article
owner: @TechLead
status: Active
---
```

---

## Adding New Documents

1. **Choose the appropriate prefix** based on document category
2. **Get the next available number** in that category
3. **Add entry to this registry** with DocID, title, file path
4. **Add docid header** to the new document
5. **Commit both files** together

---

## Maintenance

- **Owner**: @SARAH
- **Review Frequency**: Monthly
- **Last Audit**: 2. Januar 2026
