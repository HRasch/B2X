---
docid: UNKNOWN-126
title: DOCUMENT_REGISTRY
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

﻿# 📚 Document Registry

**Version:** 1.1  
**Last Updated:** 7. Januar 2026  
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
| `BS` | Brainstorm & Strategy | `.ai/brainstorm/` | @SARAH |
| `COMM` | Communications & Templates | `.ai/brainstorm/` | @SARAH |

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
| `ADR-049` | Plan-Act-Control Engineering Loop | `.ai/decisions/ADR-049-plan-act-control.md` | Proposed |
| `ADR-050` | TypeScript MCP Server for AI-Assisted Development | `.ai/decisions/ADR-050-typescript-mcp-server.md` | Accepted |
| `ADR-051` | Rename B2X to B2XGate | `.ai/decisions/ADR-051-rename-B2X-to-b2xgate.md` | Proposed |
| `ADR-052` | MCP-Enhanced Bugfixing Workflow | `.ai/decisions/ADR-052-mcp-enhanced-bugfixing.md` | Accepted |
| `ADR-053` | Realtime Debug Architecture | `.ai/decisions/ADR-053-realtime-debug-architecture.md` | Proposed |

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
| `KB-052` | Roslyn MCP Server | `.ai/knowledgebase/tools-and-tech/roslyn-mcp.md` | Active |
| `KB-053` | TypeScript MCP Integration | `.ai/knowledgebase/tools-and-tech/typescript-mcp-integration.md` | Active |
| `KB-054` | GitHub Copilot Agent Skills | `.ai/knowledgebase/tools-and-tech/github-copilot-agent-skills.md` | Active |
| `KB-050` | Plan-Act-Control Practical Guide | `.ai/knowledgebase/KB-050-plan-act-control.md` | Active |
| `KB-051` | External Knowledge Sync Procedure | `.ai/knowledgebase/KB-051-external-knowledge-sync.md` | Active |
| `KB-052` | Roslyn MCP Server | `.ai/knowledgebase/tools-and-tech/roslyn-mcp.md` | Active |
| `KB-053` | TypeScript MCP Integration | `.ai/knowledgebase/tools-and-tech/typescript-mcp-integration.md` | Active |
| `KB-054` | Vue MCP Integration Guide | `.ai/knowledgebase/tools-and-tech/vue-mcp-integration.md` | Active |
| `KB-055` | Security MCP Best Practices | `.ai/knowledgebase/tools-and-tech/security-mcp-best-practices.md` | Active |
| `KB-056` | HTML/CSS MCP Usage Guide | `.ai/knowledgebase/tools-and-tech/htmlcss-mcp-usage.md` | Active |
| `KB-057` | Database MCP Usage Guide | `.ai/knowledgebase/tools-and-tech/database-mcp-usage.md` | Active |
| `KB-058` | Testing MCP Usage Guide | `.ai/knowledgebase/tools-and-tech/testing-mcp-usage.md` | Active |
| `KB-059` | API Documentation MCP Usage Guide | `.ai/knowledgebase/tools-and-tech/api-documentation-mcp-usage.md` | Active |
| `KB-060` | i18n MCP Usage Guide | `.ai/knowledgebase/tools-and-tech/i18n-mcp-usage.md` | Active |
| `KB-061` | Monitoring MCP Usage Guide | `.ai/knowledgebase/tools-and-tech/monitoring-mcp-usage.md` | Active |
| `KB-062` | Documentation MCP Usage Guide | `.ai/knowledgebase/tools-and-tech/documentation-mcp-usage.md` | Active |
| `KB-063` | Wolverine MCP Server | `.ai/knowledgebase/tools-and-tech/wolverine-mcp.md` | Active |
| `KB-064` | Chrome DevTools MCP Server | `.ai/knowledgebase/tools-and-tech/chrome-devtools-mcp.md` | Active |
| `KB-065` | Nuxt 4 Monorepo Configuration | `.ai/knowledgebase/tools-and-tech/nuxt4-monorepo-config.md` | Active |
| `KB-066` | npm Package Updates Guide | `.ai/knowledgebase/dependency-updates/npm-package-updates.md` | Active |
| `KB-067` | VS Code Agent Sessions & Subagents | `.ai/knowledgebase/tools-and-tech/vscode-agent-sessions.md` | Active |
| `KB-068` | Vue to Nuxt Migration Guide | `.ai/knowledgebase/tools-and-tech/vue-to-nuxt-migration.md` | Active |
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
| `KB-DEP` | Dependency Updates | `.ai/knowledgebase/dependency-updates/` |

---

## Registry: Guidelines (GL-*)

| DocID | Title | File Path | Status |
|-------|-------|-----------|--------|
| `GL-INDEX` | Guidelines Index | `.ai/guidelines/README.md` | Active |
| `QS-001` | Token Optimization Quick Start | `.ai/guidelines/QS-001-TOKEN-OPTIMIZATION-QUICK-START.md` | **ACTIVE NOW** |
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
| `GL-043` | Multi-Language Fragment Editing Strategy | `.ai/guidelines/GL-043-multi-language-fragment-editing-strategy.md` | Active |
| `GL-043` | Smart Attachment Strategy | `.ai/guidelines/GL-043-SMART-ATTACHMENT-STRATEGY.md` | Active |
| `GL-044` | Markdown Fragment Editing Strategy | `.ai/guidelines/GL-044-markdown-fragment-editing.md` | Active |
| `GL-044` | Fragment-Based File Access Strategy | `.ai/guidelines/GL-044-FRAGMENT-BASED-FILE-ACCESS.md` | Active |
| `GL-045` | KB-MCP Query Strategy | `.ai/guidelines/GL-045-KB-MCP-QUERY-STRATEGY.md` | Active |
| `GL-046` | Token Audit & Analysis Framework | `.ai/guidelines/GL-046-TOKEN-AUDIT-FRAMEWORK.md` | Active |
| `GL-047` | MCP-Orchestration Layer | `.ai/guidelines/GL-047-MCP-ORCHESTRATION-LAYER.md` | Active |
| `GL-048` | Instruction File Consolidation | `.ai/guidelines/GL-048-INSTRUCTION-FILE-CONSOLIDATION.md` | Active |
| `GL-049` | Prompt Compression Prototype | `.ai/guidelines/GL-049-PROMPT-COMPRESSION-PROTOTYPE.md` | Active |
| `GL-050` | Project Documentation Structure in docs/ | `.ai/guidelines/GL-050-PROJECT-DOCS-STRUCTURE.md` | Active |
| `GL-051` | AI-Ready Documentation Integration Guide | `.ai/guidelines/GL-051-AI-READY-DOCUMENTATION-INTEGRATION.md` | Active |
| `GL-052` | Role-Based Documentation Access & Audience Segregation | `.ai/guidelines/GL-052-ROLE-BASED-DOCUMENTATION-ACCESS.md` | Active |
| `GL-053` | Multi-Language Fragment Editing Strategy | `.ai/guidelines/GL-053-multi-language-fragment-editing.md` | Active |
| `GL-054` | Documentation Trust Strategy | `.ai/guidelines/GL-054-DOCUMENTATION-TRUST-STRATEGY.md` | Active |
| `GL-070` | runSubagent Delegation Strategy | `.ai/guidelines/GL-070-RUNSUBAGENT-DELEGATION-STRATEGY.md` | Active |

---

## Registry: Architecture (ARCH-*)

| DocID | Title | File Path | Status |
|-------|-------|-----------|--------|
| `ARCH-INDEX` | Architecture Documentation Index | `docs/architecture/INDEX.md` | Active |
| `ARCH-001` | Project Structure - Verified | `docs/architecture/components/PROJECT_STRUCTURE.md` | **AUTHORITATIVE** |
| `ARCH-002` | Store Gateway | `docs/architecture/components/STORE_GATEWAY.md` | Active |
| `ARCH-003` | Admin Gateway | `docs/architecture/components/ADMIN_GATEWAY.md` | Active |
| `ARCH-004` | Management Gateway | `docs/architecture/components/MANAGEMENT_GATEWAY.md` | Active |
| `ARCH-DEF` | Software Definition | `docs/architecture/SOFTWARE_DEFINITION.md` | Active |
| `ARCH-DEC` | Design Decisions | `docs/architecture/DESIGN_DECISIONS.md` | Active |
| `ARCH-DDD` | DDD Bounded Contexts | `docs/architecture/DDD_BOUNDED_CONTEXTS.md` | Active |
| `ARCH-CAP` | Estimations and Capacity | `docs/architecture/ESTIMATIONS_AND_CAPACITY.md` | Active |
| `ARCH-STD` | Architectural Documentation Standards | `docs/architecture/ARCHITECTURAL_DOCUMENTATION_STANDARDS.md` | Active |
| `ARCH-PAT-001` | CQRS Wolverine Patterns | `docs/architecture/patterns/CQRS_WOLVERINE.md` | Active |
| `ARCH-PAT-002` | Multitenancy Patterns | `docs/architecture/patterns/MULTITENANCY.md` | Active |
| `ARCH-PAT-003` | Localization Patterns | `docs/architecture/patterns/LOCALIZATION.md` | Active |
| `ARCH-PAT-004` | Communication Patterns | `docs/architecture/patterns/COMMUNICATION.md` | Active |
| `ARCH-005` | ERP Connectors | `docs/architecture/components/ERP_CONNECTORS.md` | Active |
| `ARCH-006` | CLI Tools | `docs/architecture/components/CLI_TOOLS.md` | Active |
| `ARCH-007` | MCP Integration | `docs/architecture/components/MCP_INTEGRATION.md` | Active |
| `ARCH-008` | Frontend Architecture | `docs/architecture/components/FRONTEND_ARCHITECTURE.md` | Active |
| `ARCH-009` | Database & Search | `docs/architecture/components/DATABASE_SEARCH.md` | Active |
| `ARCH-010` | Testing Infrastructure | `docs/architecture/components/TESTING_INFRASTRUCTURE.md` | Active |
| `ARCH-011` | Hosting Infrastructure | `docs/architecture/components/HOSTING_INFRASTRUCTURE.md` | Active |
| `ARCH-PAT-005` | Database Providers | `docs/architecture/patterns/DATABASE_PROVIDERS.md` | Active |
| `ARCH-PAT-006` | Cloud Providers | `docs/architecture/patterns/CLOUD_PROVIDERS.md` | Active |
| `ARCH-FEAT-001` | Catalog Feature | `docs/architecture/features/CATALOG.md` | Active |
| `ARCH-FEAT-002` | Orders Feature | `docs/architecture/features/ORDERS.md` | Active |
| `ARCH-FEAT-003` | CMS Feature | `docs/architecture/features/CMS.md` | Active |
| `ARCH-FEAT-004` | Email Editor | `docs/architecture/features/EMAIL_EDITOR.md` | Active |
| `ARCH-FEAT-005` | Media Management | `docs/architecture/features/MEDIA_MANAGEMENT.md` | Active |

---

## Registry: Workflows (WF-*)

| DocID | Title | File Path | Status |
|-------|-------|-----------|--------|
| `WF-001` | Context Optimization | `.ai/workflows/WF-001-context-optimization.workflow.md` | Active |
| `WF-002` | Subagent Delegation | `.ai/workflows/WF-002-subagent-delegation.workflow.md` | Active |
| `WF-003` | Dependency Upgrade | `.ai/workflows/WF-003-dependency-upgrade.workflow.md` | Active |
| `WF-009` | Token Optimization Execution Plan | `.ai/workflows/WF-009-TOKEN-OPTIMIZATION-EXECUTION.md` | Active |
| `WF-004` | GitHub CLI Quick Reference | `.ai/workflows/WF-004-GITHUB_CLI_QUICK_REFERENCE.md` | Active |
| `WF-005` | GitHub CLI Implementation | `.ai/workflows/WF-005-GITHUB_CLI_IMPLEMENTATION.md` | Active |
| `WF-006` | GitHub CLI Sprint How-To | `.ai/workflows/WF-006-GITHUB_CLI_SPRINT_HOWTO.md` | Active |
| `WF-007` | GitHub Scrum/Kanban Workflow | `.ai/workflows/WF-007-GITHUB_SCRUM_KANBAN_WORKFLOW.md` | Active |
| `WF-008` | Update GitHub Issues Sprint | `.ai/workflows/WF-008-update-github-issues-sprint.md` | Active |
| `WF-010` | Documentation Maintenance & Quality | `.ai/workflows/WF-010-DOCUMENTATION-MAINTENANCE.md` | Active |
| `WF-015` | Bugfix Workflow with runSubagent | `.ai/workflows/WF-015-bugfix-workflow.md` | Active |
| `WF-CLEANUP-SETUP` | Project Cleanup Setup Guide | `.ai/workflows/WF-CLEANUP-SETUP.md` | Active |

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
| `REQ-008` | Realtime Debug Functionality | `.ai/requirements/REQ-008-realtime-debug.md` | Draft |
| `REQ-SMART-DATA-INTEGRATION` | Smart Data Integration Assistant | `.ai/requirements/REQ-SMART-DATA-INTEGRATION.md` | Active |
| `ANALYSIS-SMART-DATA-INTEGRATION-BACKEND` | Smart Data Integration Assistant - Backend Analysis | `.ai/requirements/ANALYSIS-SMART-DATA-INTEGRATION-BACKEND.md` | Analysis Complete |
| `ANALYSIS-SMART-DATA-INTEGRATION-FRONTEND` | Smart Data Integration Assistant - Frontend Analysis | `.ai/requirements/ANALYSIS-SMART-DATA-INTEGRATION-FRONTEND.md` | Analysis Complete |
| `ANALYSIS-SMART-DATA-INTEGRATION-SECURITY` | Smart Data Integration Assistant - Security Analysis | `.ai/requirements/ANALYSIS-SMART-DATA-INTEGRATION-SECURITY.md` | Analysis Complete |
| `ANALYSIS-SMART-DATA-INTEGRATION-ARCHITECT` | Smart Data Integration Assistant - Architecture Analysis | `.ai/requirements/ANALYSIS-SMART-DATA-INTEGRATION-ARCHITECT.md` | Analysis Complete |
| `CONSOLIDATION-SMART-DATA-INTEGRATION` | Smart Data Integration Assistant - Consolidated Analysis | `.ai/requirements/CONSOLIDATION-SMART-DATA-INTEGRATION.md` | Consolidation Complete |

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
| `TPL-DEVDOC-001` | Developer Documentation Template | `.ai/templates/TPL-DEVDOC-001-DEVELOPER-DOCS-TEMPLATE.md` | Active |
| `TPL-USERDOC-001` | User Documentation Template | `.ai/templates/TPL-USERDOC-001-USER-DOCS-TEMPLATE.md` | Active |
| `TPL-001` | Customer Integration Tracker | `.ai/templates/customer-integration-tracker.yml` | Active |
| `TPL-002` | Customer Integration Checklist | `.ai/templates/customer-integration-checklist.md` | Active |
| `TPL-SALES-001` | Sales Enablement Documentation Template | `.ai/templates/TPL-SALES-001-SALES-ENABLEMENT-TEMPLATE.md` | Active |

---

## Registry: Sales Documentation (DOCS-SALES-*)

| DocID | Title | File Path | Status |
|-------|-------|-----------|--------|
| `DOCS-SALES-001` | B2XGate Platform Overview: Features, Pricing & ROI | `.ai/sales/DOCS-SALES-001-B2XGATE-OVERVIEW.md` | Active |

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
| `PRM-016` | TypeScript Review | `.github/prompts/typescript-review.prompt.md` | `/typescript-review` |
| `PRM-017` | Bug Null Check | `.ai/prompts/bug-null-check.prompt.md` | `/bug-null-check` |
| `PRM-018` | Bug Async Race | `.ai/prompts/bug-async-race.prompt.md` | `/bug-async-race` |
| `PRM-019` | Bug Type Mismatch | `.ai/prompts/bug-type-mismatch.prompt.md` | `/bug-type-mismatch` |
| `PRM-020` | Bug i18n Missing | `.ai/prompts/bug-i18n-missing.prompt.md` | `/bug-i18n-missing` |
| `PRM-021` | Bug Lint Fix | `.ai/prompts/bug-lint-fix.prompt.md` | `/bug-lint-fix` |
| `PRM-QUICK-BUG` | Quick Bug-Fix Starter Set | `.github/prompts/bug-quick-starter.prompt.md` | - |
| `PRM-QBF-NULL` | Quick Null Reference Fix | `.github/prompts/bug-quick-null-check.prompt.md` | `/bug-null-check` |
| `PRM-QBF-ASYNC` | Quick Async Race Fix | `.github/prompts/bug-quick-async-race.prompt.md` | `/bug-async-race` |
| `PRM-QBF-TYPE` | Quick Type Mismatch Fix | `.github/prompts/bug-quick-type-mismatch.prompt.md` | `/bug-type-mismatch` |
| `PRM-QBF-I18N` | Quick i18n Key Fix | `.github/prompts/bug-quick-i18n-missing.prompt.md` | `/bug-i18n-missing` |
| `PRM-QBF-LINT` | Quick Linting Fix | `.github/prompts/bug-quick-lint-fix.prompt.md` | `/bug-lint-fix` |
| `PRM-022` | Auto Lessons Learned | `.ai/prompts/auto-lessons-learned.prompt.md` | `/auto-lessons-learned` |
| `PRM-023` | Cleanup Workspace | `.github/prompts/cleanup-workspace.prompt.md` | `/cleanup-workspace` |

---

## Registry: Brainstorm & Strategy (BS-*)

| DocID | Title | File Path | Status |
|-------|-------|-----------|--------|
| `BS-REFACTOR-001` | Refactoring Efficiency Strategy | `.ai/brainstorm/REFACTORING-EFFICIENCY-STRATEGY.md` | Active |
| `BS-SPRINT-EXECUTION-TRACKING` | GitHub-Based Sprint Execution Tracking | `.ai/brainstorm/BS-SPRINT-EXECUTION-TRACKING.md` | Brainstorm |
| `BS-PROJECT-CLEANLINESS` | Project Cleanliness & Long-Term Organization Strategy | `.ai/brainstorm/BS-PROJECT-CLEANLINESS-STRATEGY.md` | Active |
| `BS-DOCUMENTATION-CLEANUP-STRATEGY` | Documentation Cleanup Strategy | `.ai/brainstorm/BS-DOCUMENTATION-CLEANUP-STRATEGY.md` | Brainstorm |
| `BS-REALTIME-DEBUG` | Realtime Debug Functionality Brainstorm | `.ai/brainstorm/BS-REALTIME-DEBUG.md` | Brainstorm |

---

## Registry: Status & Dashboards (STATUS-*)

| DocID | Title | File Path | Status |
|-------|-------|-----------|--------|
| `STATUS-REFACTOR-STRATEGY` | Status Dashboard - Refactoring Strategy | `.ai/status/STATUS-REFACTOR-STRATEGY.md` | Active |
| `STATUS-PROJECT-CLEANUP-PHASE-2` | Project Cleanup - Phase 2 Documentation Complete | `.ai/status/STATUS-PROJECT-CLEANUP-PHASE-2.md` | Complete |
| `STATUS-011` | B2X Project Restructuring Completion | `.ai/status/restructuring-completion-status.md` | Completed |
| `STATUS-AUDIT-001` | Complete System Review / Audit | `.ai/status/system-audit-2026-01-10.md` | Complete |
| `STATUS-012` | runSubagent Strategy Demonstration Complete | `.ai/status/runsubagent-strategy-demo-complete.md` | Complete |

---

## Registry: Collaboration (COLLAB-*)

| DocID | Title | File Path | Status |
|-------|-------|-----------|--------|
| `COLLAB-REFACTOR-ANALYSIS` | Refactoring Strategy - Multi-Agent Analysis | `.ai/collaboration/REFACTOR-ANALYSIS.md` | Active |

---

## Registry: Communications (COMM-*)

| DocID | Title | File Path | Status |
|-------|-------|-----------|--------|
| `COMM-REFACTOR-001` | Launch Communication - Refactoring Strategy | `.ai/brainstorm/COMM-REFACTOR-001.md` | Ready |

---

## Registry: Reviews (REV-*)

| DocID | Title | File Path | Status |
|-------|-------|-----------|--------|
| `REV-REFACTOR-001` | Review Request - Refactoring Strategy | `.ai/brainstorm/REVIEW-REQUEST-REFACTORING-STRATEGY.md` | In Review |

---

## Registry: Planning (PLAN-*)

| DocID | Title | File Path | Status |
|-------|-------|-----------|--------|
| `PLAN-PILOT-001` | Pilot Refactoring Candidates | `.ai/brainstorm/PILOT-REFACTORING-CANDIDATES.md` | Planning |

---

## Registry: Quick Start (QS-*)

| DocID | Title | File Path | Status |
|-------|-------|-----------|--------|
| `QS-REFACTOR` | Quick Start - Refactoring Implementation | `.ai/brainstorm/QUICKSTART-REFACTOR.md` | Active |

---

## Registry: Status & Dashboards (STATUS-*)

| DocID | Title | File Path | Status |
|-------|-------|-----------|--------|
| `STATUS-REFACTOR-STRATEGY` | Status Dashboard - Refactoring Strategy | `.ai/brainstorm/STATUS-REFACTOR-STRATEGY.md` | Active |
| `STATUS-PROJECT-CLEANUP-PHASE-2` | Project Cleanup - Phase 2 Documentation Complete | `.ai/logs/STATUS-PROJECT-CLEANUP-PHASE-2.md` | Complete |

---

## Registry: Indexes (INDEX-*)

| DocID | Title | File Path | Status |
|-------|-------|-----------|--------|
| `REFACTOR-INDEX` | Index - Refactoring Efficiency Strategy Suite | `.ai/brainstorm/REFACTOR-INDEX.md` | Active |

---

## Registry: Instructions (INS-*)

| DocID | Title | File Path | Applies To |
|-------|-------|-----------|------------|
| `INS-001` | Backend Instructions | `.github/instructions/backend-essentials.instructions.md` | `src/api/**`, `src/services/**` |
| `INS-002` | Frontend Instructions | `.github/instructions/frontend-essentials.instructions.md` | `src/components/**`, `src/pages/**` |
| `INS-003` | Testing Instructions | `.github/instructions/testing.instructions.md` | `**/*.test.*`, `**/*.spec.*` |
| `INS-004` | DevOps Instructions | `.github/instructions/devops.instructions.md` | `.github/**`, `Dockerfile` |
| `INS-005` | Security Instructions | `.github/instructions/security.instructions.md` | `**/*` |
| `INS-006` | AI Mode Switching Instructions | `.github/instructions/ai-mode-switching.instructions.md` | `**/*` |
| `INS-007` | Dependency Management Instructions | `.github/instructions/dependency-management.instructions.md` | `Directory.Packages.props,**/*.csproj,**/*.fsproj,**/*.vbproj` |

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
| `AGT-016` | DocMaintainer | `.github/agents/DocMaintainer.agent.md` | Documentation Steward - Quality, Registry, Compliance |
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

## Registry: Developer Documentation (DEVDOC-*)

**Location**: `docs/developer/`

| DocID | Category | Title | File Path | Status |
|-------|----------|-------|-----------|--------|
| `DEVDOC-ARCH-*` | Architecture | System architecture, design patterns | `docs/developer/architecture/` | TBD |
| `DEVDOC-API-*` | API | API reference, endpoints, contracts | `docs/developer/api/` | TBD |
| `DEVDOC-GUIDE-*` | Developer Guides | How-to for developers, patterns | `docs/developer/guides/` | TBD |
| `DEVDOC-FEAT-*` | Features | Feature technical documentation | `docs/developer/features/` | TBD |
| `DEVDOC-HOW-*` | How-To | Technical step-by-step guides | `docs/developer/howto/` | TBD |
| `DEVDOC-FAQ-*` | FAQ | Technical FAQ | `docs/developer/faq/` | TBD |

---

## Registry: User Documentation (USERDOC-*)

**Location**: `docs/user/`

| DocID | Category | Title | File Path | Status |
|-------|----------|-------|-----------|--------|
| `USERDOC-START-*` | Getting Started | Installation, onboarding, setup | `docs/user/getting-started/` | TBD |
| `USERDOC-FEAT-*` | Features | Feature descriptions (user view) | `docs/user/features/` | TBD |
| `USERDOC-HOW-*` | How-To | Step-by-step user guides | `docs/user/howto/` | TBD |
| `USERDOC-SYS-*` | System Overview | System description, workflows | `docs/user/system-overview/` | TBD |
| `USERDOC-PROC-*` | Process Guides | Business process guides | `docs/user/process-guides/` | TBD |
| `USERDOC-SCREEN-*` | Screen Guide | Page/screen explanations | `docs/user/screen-explanations/` | TBD |
| `USERDOC-FAQ-*` | FAQ | User FAQ | `docs/user/faq/` | TBD |

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
| `DOC-009` | Copilot Specifications | `.copilot-specs.md` | Active |
| `DOC-APPHOST-SPEC` | AppHost Specifications | `APPHOST_SPECIFICATIONS.md` | Active |
| `DOC-APPHOST-QUICKSTART` | AppHost Quick Start Guide | `APPHOST_QUICKSTART.md` | Active |
| `DOC-CMS-OVERVIEW` | CMS Business Overview | `CMS_OVERVIEW.md` | Active |
| `DOC-CMS-IMPLEMENTATION` | CMS Technical Implementation Details | `CMS_IMPLEMENTATION_UPDATE.md` | Active |

---

## Registry: Tasks (TASK-*)

| DocID | Title | File Path | Status |
|-------|-------|-----------|--------|
| `TASK-001` | Active Tasks | `.ai/tasks/ACTIVE_TASKS.md` | Active |
| `TASK-002` | Brief Template | `.ai/tasks/BRIEF_TEMPLATE.md` | Active |
| `TASK-003` | Progress Template | `.ai/tasks/PROGRESS_TEMPLATE.md` | Active |
| `TASK-004` | Documentation Improvements Brief | `.ai/tasks/task-001-documentation-improvements/brief.md` | Active |
| `TASK-005` | Documentation Improvements Progress | `.ai/tasks/task-001-documentation-improvements/progress.md` | Active |
| `TASK-006` | First Operational Dispatch Brief | `.ai/tasks/task-002-first-operational-dispatch/brief.md` | Active |
| `TASK-007` | First Operational Dispatch Progress | `.ai/tasks/task-002-first-operational-dispatch/progress.md` | Active |

---

## Registry: Proposals (PROP-*)

| DocID | Title | File Path | Status |
|-------|-------|-----------|--------|
| `PROP-001` | Backend Instructions Proposal | `.ai/proposals/instruction-updates/backend-instructions-proposal.md` | Active |
| `PROP-002` | DevOps Instructions Proposal | `.ai/proposals/instruction-updates/devops-instructions-proposal.md` | Active |
| `PROP-003` | Frontend Instructions Proposal | `.ai/proposals/instruction-updates/frontend-instructions-proposal.md` | Active |
| `PROP-004` | PR Ready for CopilotExpert | `.ai/proposals/instruction-updates/PR_READY_FOR_COPILOTEXPERT.md` | Active |
| `PROP-005` | Instruction Updates README | `.ai/proposals/instruction-updates/README.md` | Active |
| `PROP-006` | Security Instructions Proposal | `.ai/proposals/instruction-updates/security-instructions-proposal.md` | Active |
| `PROP-007` | Testing Instructions Proposal | `.ai/proposals/instruction-updates/testing-instructions-proposal.md` | Active |
| `PROP-008` | Updated Backend Instructions | `.ai/proposals/instruction-updates/updated/backend.instructions.md` | Active |
| `PROP-009` | Updated DevOps Instructions | `.ai/proposals/instruction-updates/updated/devops.instructions.md` | Active |
| `PROP-010` | Updated Frontend Instructions | `.ai/proposals/instruction-updates/updated/frontend.instructions.md` | Active |
| `PROP-011` | Updated Security Instructions | `.ai/proposals/instruction-updates/updated/security.instructions.md` | Active |
| `PROP-012` | Updated Testing Instructions | `.ai/proposals/instruction-updates/updated/testing.instructions.md` | Active |
| `PROP-013` | SARAH Agent Definition - runSubagent Integration | `.ai/proposals/agent-updates/SARAH-runSubagent-integration-proposal.md` | Pending @CopilotExpert Review |

---

## Registry: Reviews (REV-*)

| DocID | Title | File Path | Status |
|-------|-------|-----------|--------|
| `REV-001` | ADR-032 Review Request | `.ai/reviews/ADR-032-review-request.md` | Active |
| `REV-002` | eGate Adoption Review 2026-01-03 | `.ai/reviews/eGate-adoption-review-2026-01-03.md` | Active |
| `REV-003` | ERP Architecture Review 2026-01-02 | `.ai/reviews/ERP-ARCHITECTURE-REVIEW-2026-01-02.md` | Active |

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
