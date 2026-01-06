# 📚 Document Registry

**Version:** 1.0  
**Last Updated:** 6. Januar 2026  
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
| `FH` | Feature Handovers | `.ai/handovers/` | @ProductOwner |

---

## Registry: Architecture Decision Records (ADR-*)

| DocID | Title | File Path | Status |
|-------|-------|-----------|--------|
| `ADR-001` | Wolverine over MediatR | `.ai/decisions/ADR-001-wolverine-over-mediatr.md` | Accepted |
| `ADR-002` | Onion Architecture | `.ai/decisions/ADR-002-onion-architecture.md` | Accepted |
| `ADR-003` | Aspire Orchestration | `.ai/decisions/ADR-003-aspire-orchestration.md` | Accepted |
| `ADR-004` | PostgreSQL Multitenancy | `.ai/decisions/ADR-004-postgresql-multitenancy.md` | Accepted |
| `ADR-020` | PR Quality Gate | `.ai/decisions/ADR-020-pr-quality-gate.md` | Accepted |
| `ADR-021` | ArchUnitNET Architecture Testing | `.ai/decisions/ADR-021-archunitnet-architecture-testing.md` | Accepted |
| `ADR-022` | Multi-Tenant Domain Management | `.ai/decisions/ADR-022-multi-tenant-domain-management.md` | Proposed |
| `ADR-023` | ERP Plugin Architecture | `.ai/decisions/ADR-023-erp-plugin-architecture.md` | Proposed |
| `ADR-024` | Scheduler Job Monitoring | `.ai/decisions/ADR-024-scheduler-job-monitoring.md` | Accepted |
| `ADR-025` | Gateway-Service Communication Strategy | `.ai/decisions/ADR-025-gateway-service-communication-strategy.md` | Accepted |
| `ADR-026` | BMEcat Catalog Import Architecture | `.ai/decisions/ADR-026-bmecat-catalog-import-architecture.md` | Proposed |
| `ADR-027` | Email Template Engine Selection | `.ai/decisions/ADR-027-email-template-engine.md` | Accepted |
| `ADR-028` | ERP API Integration Architecture | `.ai/decisions/ADR-028-erp-bidirectional-integration.md` | Accepted |
| `ADR-029` | Multi-Format Punchout Integration for Craft Software | `.ai/decisions/ADR-029-multi-format-punchout-integration.md` | Proposed |
| `ADR-029-BPA` | Business Process Analysis - Punchout Integration | `.ai/decisions/ADR-029-business-process-analysis.md` | Active |
| `ADR-029-ICAS` | IDS Connect Adapter Specification | `.ai/decisions/ADR-029-ids-connect-adapter-spec.md` | Active |
| `ADR-030` | Vue-i18n v10 to v11 Migration | `.ai/decisions/ADR-030-vue-i18n-v11-migration.md` | Accepted |
| `ADR-031` | CLI Architecture Split - Operations vs. Administration | `.ai/decisions/ADR-031-cli-architecture-split.md` | Proposed |
| `ADR-032` | CLI Auto-Update Functionality | `.ai/decisions/ADR-032-cli-auto-update-brainstorm.md` | Brainstorm |
| `ADR-033` | Tenant-Admin Download for ERP-Connector and Administration-CLI Coupled to CLI | `.ai/decisions/ADR-033-tenant-admin-download-erp-connector-cli.md` | Approved |
| `ADR-034` | Multi-ERP Connector Architecture | `.ai/decisions/ADR-034-multi-erp-connector-architecture.md` | Approved with Conditions |
| `ADR-035` | MCP-Enabled AI Assistant with CLI Operations Access | `.ai/decisions/ADR-035-mcp-enabled-ai-assistant-cli-operations.md` | Proposed |
| `ADR-036` | Shared ERP Project Architecture | `.ai/decisions/ADR-036-shared-erp-project-architecture.md` | Accepted |
| `ADR-037` | Lifecycle Stages Framework | `.ai/decisions/ADR-037-lifecycle-stages-framework.md` | Accepted |
| `ADR-038` | Customer Integration Stages Framework | `.ai/decisions/ADR-038-customer-integration-stages.md` | Accepted |
| `ADR-039` | Agent Instruction Protection Strategy | `.ai/decisions/ADR-039-instruction-protection.md` | Accepted |
| `ADR-040` | Tenant-Customizable Language Resources | `.ai/decisions/ADR-040-tenant-customizable-language-resources.md` | Proposed |
| `ADR-041` | Figma-based Tenant Design Integration | `.ai/decisions/ADR-041-figma-based-tenant-design-integration.md` | Accepted |
| `ADR-042` | Internationalization Strategy for ESLint Error Reduction | `.ai/decisions/ADR-042-internationalization-strategy.md` | Proposed |
| `ADR-043` | Paid Services Infrastructure | `.ai/decisions/ADR-043-paid-services-infrastructure.md` | Proposed |
| `ADR-044` | Floating Labels vs Traditional Labels Ruleset | `.ai/decisions/ADR-044-floating-labels-ruleset.md` | Accepted |
| `ADR-045` | Unified Layout System | `.ai/decisions/ADR-045-unified-layout-system.md` | Proposed |
| `ADR-046` | Unified Category Navigation Architecture | `.ai/decisions/ADR-046-unified-category-navigation.md` | Proposed |
| `ADR-047` | Multishop / Shared Catalogs Architecture | `.ai/decisions/ADR-047-multishop-shared-catalog.md` | Proposed |
| `ADR-048` | Tenant-Level Include/Exclude Rules for Shared Catalogs | `.ai/decisions/ADR-048-tenant-level-include-exclude.md` | Proposed |

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
| `KB-018` | Local LLM Models 2025 | `.ai/knowledgebase/tools-and-tech/local-llm-models-2025.md` | Active |
| `KB-019` | StyleCop Analyzers | `.ai/knowledgebase/tools-and-tech/stylecop-analyzers.md` | Active |
| `KB-020` | ArchUnitNET | `.ai/knowledgebase/tools-and-tech/archunitnet.md` | Active |
| `KB-021` | enventa Trade ERP | `.ai/knowledgebase/enventa-trade-erp.md` | Active |
| `KB-022` | GitHub Copilot Customization | `.ai/knowledgebase/tools-and-tech/github-copilot-customization.md` | Active |
| `KB-023` | Email Template Best Practices | `.ai/knowledgebase/best-practices/email-templates.md` | Active |
| `KB-024` | Microsoft.Extensions.AI | `.ai/knowledgebase/dependency-updates/Microsoft.Extensions.AI.md` | Active |
| `KB-025` | Global Local AI Mode Configuration | `.ai/knowledgebase/tools-and-tech/local-ai-fallback-configuration.md` | Active |
| `KB-026` | Monaco Editor Vue Integration | `.ai/knowledgebase/tools-and-tech/monaco-editor-vue.md` | Active |
| `KB-027` | Email Dark Mode Best Practices | `.ai/knowledgebase/best-practices/email-dark-mode.md` | Active |
| `KB-028` | GrapesJS Email Builder Integration | `.ai/knowledgebase/tools-and-tech/grapesjs-email-builder.md` | Active |
| `KB-029` | CSS Functions | `.ai/knowledgebase/css-functions.md` | Active |
| `KB-030` | SVG and CSS Animations | `.ai/knowledgebase/svg-css-animations.md` | Active |
| `KB-031` | Shouldly Assertion Framework | `.ai/knowledgebase/tools-and-tech/shouldly.md` | Active |
| `KB-AGT-GIT` | Git Management SubAgent | `.ai/knowledgebase/agents/git-management-subagent.md` | Active |
| `KB-LESSONS` | Lessons Learned | `.ai/knowledgebase/lessons.md` | Active |

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
| `GL-006` | Token Optimization Strategy | `.ai/guidelines/GL-006-TOKEN-OPTIMIZATION-STRATEGY.md` | Active |
| `GL-007` | Lessons Learned Maintenance Strategy | `.ai/guidelines/GL-007-lessons-maintenance-strategy.md` | Active |
| `GL-008` | Governance Policies | `.ai/guidelines/GL-008-GOVERNANCE-POLICIES.md` | Active |
| `GL-009` | AI Behavior Guidelines | `.ai/guidelines/GL-009-AI-BEHAVIOR.md` | Active |
| `GL-010` | Agent & Artifact Organization | `.ai/guidelines/GL-010-AGENT-ARTIFACT-ORGANIZATION.md` | Active |
| `GL-011` | Guard Clauses - Coding Style Guidelines | `.ai/guidelines/GL-011-coding-style-guard-clauses.md` | Active |
| `GL-012` | Frontend Quality Standards | `.ai/guidelines/GL-012-FRONTEND-QUALITY-STANDARDS.md` | Active |
| `GL-013` | Dependency Management Policy | `.ai/guidelines/GL-013-DEPENDENCY-MANAGEMENT.md` | Active |
| `GL-014` | Pre-Release Development Phase | `.ai/guidelines/GL-014-PRE-RELEASE-DEVELOPMENT-PHASE.md` | Active (until v1.0) |
| `GL-042` | Token-Optimized i18n Strategy | `.ai/guidelines/GL-042-TOKEN-OPTIMIZED-I18N-STRATEGY.md` | Active |

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

## Registry: Requirements (REQ-*)

| DocID | Title | File Path | Status |
|-------|-------|-----------|--------|
| `REQ-001` | Monitoring for Scheduler Jobs | `.ai/requirements/REQ-001-monitoring-scheduler-jobs.md` | Draft |
| `REQ-002` | BMEcat Katalog Import | `.ai/requirements/REQ-002-bmecat-import.md` | Draft |
| `REQ-003` | Email Template System | `.ai/requirements/REQ-003-email-template-system.md` | Draft |
| `REQ-005` | Phase 5 Enterprise ERP Connectors | `.ai/requirements/REQ-005-phase-5-enterprise-connectors.md` | Active |
| `REQ-006` | CLI Customer Integration Commands | `.ai/requirements/REQ-006-cli-customer-integration-commands.md` | Specification |
| `REQ-007` | Email WYSIWYG Builder | `.ai/requirements/REQ-007-email-wysiwyg-builder.md` | Draft |

---

## Registry: Feature Handovers (FH-*)

| DocID | Title | File Path | Status |
|-------|-------|-----------|--------|
| `FH-030` | ADR-030 CMS Tenant Template Overrides Handover | `.ai/handovers/ADR-030-cms-tenant-template-overrides-handover.md` | Complete |

---

## Registry: Sprint Documents (SPR-*)

| DocID | Title | File Path | Status |
|-------|-------|-----------|--------|
| `SPR-001` | Sprint / Iteration Template | `.ai/sprint/SPR-001-iteration-template.md` | Active |

---

## Registry: Templates (TPL-*)

| DocID | Title | File Path | Status |
|-------|-------|-----------|--------|
| `TPL-001` | Customer Integration Tracker | `.ai/templates/customer-integration-tracker.yml` | Active |
| `TPL-002` | Customer Integration Checklist | `.ai/templates/customer-integration-checklist.md` | Active |

---

## Registry: Logs (LOG-*)

| DocID | Title | File Path | Status |
|-------|-------|-----------|--------|
| `LOG-001` | Instructions Update Log (2025-12-31) | `.ai/logs/instructions/INSTRUCTIONS_UPDATE_2025_12_31.md` | Active |

---

## Registry: Prompts (PRM-*)

| DocID | Title | File Path | Command |
|-------|-------|-----------|---------|
| `PRM-INDEX` | Prompt Index | `.ai/collaboration/PROMPTS_INDEX.md` | - |
| `PRM-001` | Start Feature | `.ai/prompts/start-feature.prompt.md` | `/start-feature` |
| `PRM-002` | Code Review | `.ai/prompts/code-review.prompt.md` | `/code-review` |
| `PRM-003` | Run Tests | `.ai/prompts/run-tests.prompt.md` | `/run-tests` |
| `PRM-004` | Deploy | `.ai/prompts/deploy.prompt.md` | `/deploy` |
| `PRM-005` | Security Audit | `.ai/prompts/security-audit.prompt.md` | `/security-audit` |
| `PRM-006` | ADR Create | `.ai/prompts/adr-create.prompt.md` | `/adr-create` |
| `PRM-007` | Iteration Cycle | `.ai/prompts/iteration-cycle.prompt.md` | `/iteration-cycle` |
| `PRM-008` | Bug Analysis | `.ai/prompts/bug-analysis.prompt.md` | `/bug-analysis` |
| `PRM-009` | Feature Handover | `.ai/prompts/feature-handover.prompt.md` | `/feature-handover` |
| `PRM-010` | Requirements Analysis | `.ai/prompts/requirements-analysis.prompt.md` | `/requirements-analysis` |
| `PRM-011` | Agent Removal | `.ai/prompts/agent-removal.prompt.md` | `/agent-removal` |
| `PRM-012` | Agent Creation | `.ai/prompts/agent-creation.prompt.md` | `/agent-creation` |
| `PRM-013` | Context Optimization | `.ai/prompts/context-optimization.prompt.md` | `/context-optimization` |
| `PRM-014` | Subagent Delegation | `.ai/prompts/subagent-delegation.prompt.md` | `/subagent-delegation` |
| `PRM-015` | Project Cleanup | `.ai/prompts/project-cleanup.prompt.md` | `/project-cleanup` |

---

## Registry: Instructions (INS-*)

| DocID | Title | File Path | Applies To |
|-------|-------|-----------|------------|
| `INS-001` | Backend Instructions | `.github/instructions/backend.instructions.md` | `src/api/**`, `src/services/**` |
| `INS-002` | Frontend Instructions | `.github/instructions/frontend.instructions.md` | `src/components/**`, `src/pages/**` |
| `INS-003` | Testing Instructions | `.github/instructions/testing.instructions.md` | `**/*.test.*`, `**/*.spec.*` |
| `INS-004` | DevOps Instructions | `.github/instructions/devops.instructions.md` | `.github/**`, `Dockerfile` |
| `INS-005` | Security Instructions | `.github/instructions/security.instructions.md` | `**/*` |
| `INS-006` | AI Mode Switching Instructions | `.github/instructions/ai-mode-switching.instructions.md` | `**/*` |

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
| `AGT-017` | Enventa | `.github/agents/Enventa.agent.md` | enventa Trade ERP Integration |
| `AGT-018` | CopilotExpert | `.github/agents/CopilotExpert.agent.md` | Copilot Configuration |

**CopilotExpert (AGT-018) EXCLUSIVE Authority:**
- **ONLY @CopilotExpert** may create, modify, or delete agent definitions (`.github/agents/`)
- **ONLY @CopilotExpert** may create, modify, or delete prompt files (`.github/prompts/`)
- **ONLY @CopilotExpert** may create, modify, or delete instruction files (`.github/instructions/`)
- **ONLY @CopilotExpert** may modify MCP configuration (`.vscode/mcp.json`)
- Other agents must REQUEST changes via @CopilotExpert
- Policy-level changes require @SARAH approval

**DocMaintainer (AGT-016) Responsibilities:**
- Enforce and extend the DocID naming conventions (`GL-`, `WF-`, `CMP-`, `SPR-`, etc.) when new document types are introduced.
- Maintain `.ai/DOCUMENT_REGISTRY.md` entries when docs are added, renamed, or archived.
- Run periodic link checks and create audit logs under `.ai/logs/documentation/`.
- Update cross-references across `.ai/` to ensure links stay current.
- When policy decisions are needed (naming, retention), open an issue and notify `@SARAH`.
- **Note:** @DocMaintainer may NOT modify `.github/agents/`, `.github/prompts/`, `.github/instructions/` (owned by @CopilotExpert)

**Default Agent**: `@SARAH` — Coordinator and primary default for unspecified agent prompts. SARAH is the authoritative coordinator for agent workflows and quality gates.

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
