# Architecture Documentation Index

**Owner**: @software-architect  
**Last Updated**: 10. Januar 2026  
**Purpose**: Single source of truth for B2X system architecture (Startup Scale)

---

## 📚 Core Documents (Start Here)

### 1. [SOFTWARE_DEFINITION.md](./SOFTWARE_DEFINITION.md)
**What is B2X?**
- Vision & purpose
- Scope (what's in/out)
- Core functions
- Constraints & assumptions
- Non-functional requirements

**Read this if**: You're new to the project, need to understand the big picture

**Time**: 15 minutes

---

### 2. [DESIGN_DECISIONS.md](./DESIGN_DECISIONS.md)
**Why did we build it this way?**
- Microservices (not monolith)
- Wolverine (not MediatR)
- Onion Architecture
- PostgreSQL per service
- Aspire for orchestration
- Multi-tenancy strategy
- Event-driven processing
- Encryption approach
- Velocity-based development

**Read this if**: Making a decision, need to understand architectural trade-offs

**Time**: 30 minutes (skim as needed)

---

### 3. [ESTIMATIONS_AND_CAPACITY.md](./ESTIMATIONS_AND_CAPACITY.md)
**What are the resource requirements?**
- Current scale targets (100 shops, 1K users, 10K products)
- Growth projections (5x Year 1→2, 2x Year 2→3)
- Performance targets (P95 latencies)
- Infrastructure costs ($960/month baseline)
- Team staffing (4-6 engineers)
- Database performance estimates
- Scaling strategy

**Read this if**: Planning infrastructure, estimating costs, capacity planning

**Time**: 20 minutes

---

## 📋 Architecture Decision Records (ADR)

Formal records of major decisions. Each follows standard format:
- Problem statement
- Options considered
- Decision made
- Consequences (positive & negative)
- Related decisions

### Completed ADRs

| ADR | Title | Deciders | Date |
|-----|-------|----------|------|
| [ADR-001](../../.ai/decisions/ADR-001-wolverine-over-mediatr.md) | Wolverine over MediatR | @tech-lead, @backend-dev | 2025-09 |
| [ADR-002](../../.ai/decisions/ADR-002-onion-architecture.md) | Onion Architecture pattern | @tech-lead, @architect | 2025-09 |
| [ADR-003](../../.ai/decisions/ADR-003-aspire-orchestration.md) | Aspire for local orchestration | @devops-engineer | 2025-10 |
| [ADR-004](../../.ai/decisions/ADR-004-postgresql-multitenancy.md) | PostgreSQL per service | @architect, @devops | 2025-10 |
| [ADR-005](./ADR/ADR-005-PHASE_4_VELOCITY_BASED.md) | Velocity-based development | @scrum-master | 2025-12 |
| [ADR-020](../../.ai/decisions/ADR-020-pr-quality-gate.md) | PR Quality Gate | @tech-lead, @architect | 2025-12 |
| [ADR-021](../../.ai/decisions/ADR-021-archunitnet-architecture-testing.md) | ArchUnitNET Architecture Testing | @tech-lead, @qa | 2025-12 |
| [ADR-022](../../.ai/decisions/ADR-022-multi-tenant-domain-management.md) | Multi-Tenant Domain Management | @architect, @backend | 2025-12 |
| [ADR-023](../../.ai/decisions/ADR-023-erp-plugin-architecture.md) | ERP Plugin Architecture | @architect, @backend | 2025-12 |
| [ADR-024](../../.ai/decisions/ADR-024-scheduler-job-monitoring.md) | Scheduler Job Monitoring | @architect, @backend | 2025-12 |
| [ADR-025](../../.ai/decisions/ADR-025-gateway-service-communication-strategy.md) | Gateway-Service Communication Strategy | @architect, @backend | 2025-12 |
| [ADR-026](../../.ai/decisions/ADR-026-bmecat-catalog-import-architecture.md) | BMEcat Catalog Import Architecture | @architect, @backend | 2025-12 |
| [ADR-027](../../.ai/decisions/ADR-027-email-template-engine.md) | Email Template Engine Selection | @architect, @backend | 2025-12 |
| [ADR-028](../../.ai/decisions/ADR-028-erp-bidirectional-integration.md) | ERP API Integration Architecture | @architect, @backend | 2025-12 |
| [ADR-029](../../.ai/decisions/ADR-029-multi-format-punchout-integration.md) | Multi-Format Punchout Integration | @architect, @backend | 2025-12 |
| [ADR-030](../../.ai/decisions/ADR-030-vue-i18n-v11-migration.md) | Vue-i18n v10 to v11 Migration | @frontend, @architect | 2025-12 |
| [ADR-031](../../.ai/decisions/ADR-031-cli-architecture-split.md) | CLI Architecture Split | @architect, @backend | 2025-12 |
| [ADR-032](../../.ai/decisions/ADR-032-cli-auto-update-brainstorm.md) | CLI Auto-Update Functionality | @architect, @backend | 2025-12 |
| [ADR-033](../../.ai/decisions/ADR-033-tenant-admin-download-erp-connector-cli.md) | Tenant-Admin Download for ERP-Connector | @architect, @backend | 2025-12 |
| [ADR-034](../../.ai/decisions/ADR-034-multi-erp-connector-architecture.md) | Multi-ERP Connector Architecture | @architect, @backend | 2025-12 |
| [ADR-035](../../.ai/decisions/ADR-035-mcp-enabled-ai-assistant-cli-operations.md) | MCP-Enabled AI Assistant | @architect, @backend | 2025-12 |
| [ADR-036](../../.ai/decisions/ADR-036-shared-erp-project-architecture.md) | Shared ERP Project Architecture | @architect, @backend | 2025-12 |
| [ADR-037](../../.ai/decisions/ADR-037-lifecycle-stages-framework.md) | Lifecycle Stages Framework | @architect, @backend | 2025-12 |
| [ADR-038](../../.ai/decisions/ADR-038-customer-integration-stages.md) | Customer Integration Stages Framework | @architect, @backend | 2025-12 |
| [ADR-039](../../.ai/decisions/ADR-039-instruction-protection.md) | Agent Instruction Protection Strategy | @architect, @backend | 2025-12 |
| [ADR-040](../../.ai/decisions/ADR-040-tenant-customizable-language-resources.md) | Tenant-Customizable Language Resources | @architect, @frontend | 2025-12 |
| [ADR-041](../../.ai/decisions/ADR-041-figma-based-tenant-design-integration.md) | Figma-based Tenant Design Integration | @architect, @frontend | 2025-12 |
| [ADR-042](../../.ai/decisions/ADR-042-internationalization-strategy.md) | Internationalization Strategy | @architect, @frontend | 2025-12 |
| [ADR-043](../../.ai/decisions/ADR-043-paid-services-infrastructure.md) | Paid Services Infrastructure | @architect, @backend | 2025-12 |
| [ADR-044](../../.ai/decisions/ADR-044-floating-labels-ruleset.md) | Floating Labels vs Traditional Labels | @architect, @frontend | 2025-12 |
| [ADR-045](../../.ai/decisions/ADR-045-unified-layout-system.md) | Unified Layout System | @architect, @frontend | 2025-12 |
| [ADR-046](../../.ai/decisions/ADR-046-unified-category-navigation.md) | Unified Category Navigation Architecture | @architect, @frontend | 2025-12 |
| [ADR-047](../../.ai/decisions/ADR-047-multishop-shared-catalog.md) | Multishop / Shared Catalogs Architecture | @architect, @backend | 2025-12 |
| [ADR-048](../../.ai/decisions/ADR-048-tenant-level-include-exclude.md) | Tenant-Level Include/Exclude Rules | @architect, @backend | 2025-12 |
| [ADR-049](../../.ai/decisions/ADR-049-plan-act-control.md) | Plan-Act-Control Engineering Loop | @architect, @backend | 2025-12 |
| [ADR-050](../../.ai/decisions/ADR-050-typescript-mcp-server.md) | TypeScript MCP Server | @architect, @frontend | 2025-12 |
| [ADR-051](../../.ai/decisions/ADR-051-rename-B2X-to-b2xgate.md) | Rename B2X to B2XGate | @architect, @backend | 2025-12 |
| [ADR-052](../../.ai/decisions/ADR-052-mcp-enhanced-bugfixing.md) | MCP-Enhanced Bugfixing Workflow | @architect, @backend | 2025-12 |
| [ADR-053](../../.ai/decisions/ADR-053-realtime-debug-architecture.md) | Realtime Debug Architecture | @architect, @backend | 2026-01 |
| [ADR-054](../../.ai/decisions/ADR-054-realtime-debug-strategy.md) | Realtime Debug Strategy | @architect, @backend | 2026-01 |

### Create New ADR

Use template: [ADR-NNN-[decision-name].md]()

Format:
```markdown
# ADR-NNN: [Title]

**Status**: Accepted / Pending  
**Date**: YYYY-MM-DD  
**Deciders**: @role1, @role2

## Problem
Describe the architectural issue.

## Options Considered
1. Option A: pros/cons
2. Option B: pros/cons

## Decision
We chose **Option X** because...

## Consequences
**Positive**: 
- ...
**Negative**:
- ...

## Related ADRs
- ADR-NNN: ...
```

---

## 🏗️ System Design Documents

### [DDD_BOUNDED_CONTEXTS.md](./DDD_BOUNDED_CONTEXTS.md)
How the system is organized into domain-driven design contexts

### [ONION_ARCHITECTURE.md](../archive/architecture-docs/ONION_ARCHITECTURE.md)
Layer structure for each microservice

### [ASPIRE_GUIDE.md](./ASPIRE_GUIDE.md)
How to run services locally with .NET Aspire

### [SHARED_AUTHENTICATION.md](./SHARED_AUTHENTICATION.md)
JWT-based authentication across all services

### [GATEWAY_SEPARATION.md](../guides/GATEWAY_SEPARATION.md)
Store gateway vs Admin gateway (different auth, different routes)

---

## 📊 Diagrams & Visualizations

Visual representations of architecture (created in Mermaid, PlantUML, or Lucidchart):

- **system-context.md** - High-level system context (B2X box + external systems)
- **microservice-topology.md** - Services + communication paths
- **data-flow.md** - How data flows through system
- **deployment-architecture.md** - Production infrastructure layout

---

## 🔄 Maintenance & Currency

### Who Maintains What?

| Document | Owner | Review Frequency |
|----------|-------|------------------|
| SOFTWARE_DEFINITION.md | @software-architect | Quarterly |
| DESIGN_DECISIONS.md | @software-architect | When decisions made |
| ESTIMATIONS_AND_CAPACITY.md | @software-architect + @devops | Monthly (update actuals) |
| ADR/ folder | @software-architect | As decisions made |
| DDD_BOUNDED_CONTEXTS.md | @software-architect | Quarterly |
| ONION_ARCHITECTURE.md | @tech-lead | Quarterly |

### Update Process

**When architectural decision is made**:
1. Create ADR immediately (ADR-NNN-[name].md)
2. Summarize in DESIGN_DECISIONS.md
3. Update SOFTWARE_DEFINITION.md if scope changes
4. Update ESTIMATIONS_AND_CAPACITY.md if resource implications

**Quarterly review**:
1. @software-architect reviews all documents
2. Verify against current implementation
3. Update with new learnings
4. Update MAINTENANCE_LOG.md

### MAINTENANCE_LOG.md

Track all updates:

```markdown
# Maintenance Log

## 2026-Q1
- [2026-01-10] Updated DDD_BOUNDED_CONTEXTS.md: Added Monitoring bounded context to reflect Phase 2 realtime debug implementation
- [2026-01-10] Updated ADR table: Added all recent ADRs (ADR-020 through ADR-054) including realtime debug strategy ADRs
- [2026-01-10] Updated last modified date to reflect current documentation status

## 2025-Q4
- [2025-12-29] Created ADR-005: Velocity-based development
- [2025-12-20] Updated ESTIMATIONS_AND_CAPACITY.md: Year 2 projections

## 2025-Q3
- [2025-09-30] Created DESIGN_DECISIONS.md (initial)
- [2025-09-15] Created SOFTWARE_DEFINITION.md (initial)
```

---

## 👥 Document Audience

### For Developers (Backend/Frontend)

**Must Read**:
1. SOFTWARE_DEFINITION.md (what we're building)
2. Relevant DDD bounded context doc
3. ONION_ARCHITECTURE.md (how to structure code)

**Skim When Needed**:
- ADRs (understand decisions affecting your work)
- DESIGN_DECISIONS.md (context for architectural choices)

### For DevOps Engineers

**Must Read**:
1. ESTIMATIONS_AND_CAPACITY.md (infrastructure planning)
2. ASPIRE_GUIDE.md (local orchestration)
3. SHARED_AUTHENTICATION.md (auth infrastructure)

**Skim When Needed**:
- DESIGN_DECISIONS.md (understand infrastructure decisions)
- ADRs (relevant to infrastructure choices)

### For Product Managers

**Must Read**:
1. SOFTWARE_DEFINITION.md (scope & features)
2. ESTIMATIONS_AND_CAPACITY.md (timelines & costs)

**Optional**:
- DESIGN_DECISIONS.md (understand technical trade-offs)

### For Tech Leads / Architects

**Must Read**: ALL documents

**Maintain**: All documents (quarterly reviews)

---

## 🎯 Quick Reference

### "How do I...?"

| Question | Document |
|----------|----------|
| Understand what B2X is? | SOFTWARE_DEFINITION.md |
| Understand why we chose architecture X? | DESIGN_DECISIONS.md or ADR-NNN |
| Estimate infrastructure costs? | ESTIMATIONS_AND_CAPACITY.md |
| Set up locally? | ASPIRE_GUIDE.md |
| Understand data flow? | diagrams/data-flow.md |
| Make a new architectural decision? | Create ADR-NNN-[name].md |

---

## 📈 Growth Timeline

```
Y1 (Current)
├── 100 shops
├── 50 GB database
├── 4.5 engineers
└── ~$960/month cost

Y2 (Projected)
├── 500 shops
├── 100 GB database
├── 6 engineers
└── ~$3,500/month cost

Y3+ (Long-term)
├── 1,000+ shops
├── 200+ GB database
├── 8+ engineers
└── Enterprise infrastructure
```

---

## ✅ Quality Standards

All architecture documents must have:

- [ ] Clear title & purpose
- [ ] Date last updated
- [ ] Owner & maintainer
- [ ] Scope (what's covered)
- [ ] Examples (not just abstract theory)
- [ ] Related documents (cross-references)
- [ ] Review frequency

---

## 🔗 Related Documentation

**Within B2X**:
- [Backend Instructions](.../../.github/copilot-instructions-backend.md) - Coding standards
- [Tech Lead Guide](../by-role/TECH_LEAD.md) - Architecture review process
- [Testing Framework](../archive/architecture-docs/TESTING_FRAMEWORK_GUIDE.md) - Testing strategy

**External**:
- [.NET 10 Documentation](https://learn.microsoft.com/dotnet/)
- [PostgreSQL 16 Docs](https://www.postgresql.org/docs/16/)
- [Wolverine Documentation](https://wolverinefx.io)
- [DDD Fundamentals](https://martinfowler.com/bliki/DomainDrivenDesign.html)

---

## 📞 Questions?

**Architecture question?** → Contact @software-architect  
**Need to propose decision?** → Create ADR, tag @software-architect for review  
**Infrastructure question?** → Contact @devops-engineer  
**Team coordination?** → Contact @scrum-master  

---

**Last Updated**: 29. Dezember 2025  
**Owner**: @software-architect  
**Governance**: @process-assistant (maintains structure & standards)  

---

## 📋 Document Checklist

Before marking doc as "complete", verify:

- [ ] Scope clearly defined (what's IN, what's OUT)
- [ ] Examples provided (not just theory)
- [ ] Date & owner documented
- [ ] Review frequency specified
- [ ] Cross-references to related docs
- [ ] Grammar review (no typos)
- [ ] Technical accuracy verified
- [ ] Tested in practice (not theoretical only)
