# Architecture Documentation Index

**Owner**: @software-architect  
**Last Updated**: 30. Dezember 2025  
**Purpose**: Single source of truth for B2Connect system architecture (Startup Scale)

---

## 📚 Core Documents (Start Here)

### 1. [SOFTWARE_DEFINITION.md](./SOFTWARE_DEFINITION.md)
**What is B2Connect?**
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
| [ADR-001](./ADR/ADR-001-WOLVERINE_OVER_MEDIATR.md) | Wolverine over MediatR | @tech-lead, @backend-dev | 2025-09 |
| [ADR-002](./ADR/ADR-002-ONION_ARCHITECTURE.md) | Onion Architecture pattern | @tech-lead, @architect | 2025-09 |
| [ADR-003](./ADR/ADR-003-ASPIRE_ORCHESTRATION.md) | Aspire for local orchestration | @devops-engineer | 2025-10 |
| [ADR-004](./ADR/ADR-004-POSTGRESQL_MULTITENANCY.md) | PostgreSQL per service | @architect, @devops | 2025-10 |
| [ADR-005](./ADR/ADR-005-PHASE_4_VELOCITY_BASED.md) | Velocity-based development | @scrum-master | 2025-12 |

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

### [ONION_ARCHITECTURE.md](./ONION_ARCHITECTURE.md)
Layer structure for each microservice

### [ASPIRE_GUIDE.md](./ASPIRE_GUIDE.md)
How to run services locally with .NET Aspire

### [SHARED_AUTHENTICATION.md](./SHARED_AUTHENTICATION.md)
JWT-based authentication across all services

### [GATEWAY_SEPARATION.md](./GATEWAY_SEPARATION.md)
Store gateway vs Admin gateway (different auth, different routes)

---

## 📊 Diagrams & Visualizations

Visual representations of architecture (created in Mermaid, PlantUML, or Lucidchart):

- **system-context.md** - High-level system context (B2Connect box + external systems)
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
| Understand what B2Connect is? | SOFTWARE_DEFINITION.md |
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

**Within B2Connect**:
- [Backend Instructions](.../../.github/copilot-instructions-backend.md) - Coding standards
- [Tech Lead Guide](../by-role/TECH_LEAD.md) - Architecture review process
- [Testing Framework](../TESTING_FRAMEWORK_GUIDE.md) - Testing strategy

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
