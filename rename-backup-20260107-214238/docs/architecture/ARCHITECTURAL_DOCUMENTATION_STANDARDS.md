# Architectural Documentation Standards

**Owner**: @software-architect  
**Last Updated**: 30. Dezember 2025  
**Status**: Active - All architecture documentation cleaned & verified (startup scale)

---

## 🎯 Purpose

Establish clear, up-to-date architectural documentation for B2X covering:
- **Software Definition**: What the system is, its purpose, constraints
- **Design Decisions**: Why architectural choices were made
- **Estimations**: Resource requirements, capacity, scalability
- **ADRs (Architecture Decision Records)**: Formal decision tracking

This ensures the team has a single source of truth for architecture across all layers.

---

## 📍 Documentation Location

All architectural documentation lives in: **`docs/architecture/`**

### Directory Structure

```
docs/architecture/
├── README.md (INDEX - start here)
├── ARCHITECTURAL_DOCUMENTATION_STANDARDS.md (this file)
├── SOFTWARE_DEFINITION.md
├── DESIGN_DECISIONS.md
├── ESTIMATIONS_AND_CAPACITY.md
├── DDD_BOUNDED_CONTEXTS.md
├── ONION_ARCHITECTURE.md
├── ADR/ (Architecture Decision Records)
│   ├── ADR-001-WOLVERINE_OVER_MEDIATR.md
│   ├── ADR-002-ONION_ARCHITECTURE.md
│   ├── ADR-003-ASPIRE_ORCHESTRATION.md
│   ├── ADR-004-POSTGRESQL_MULTITENANCY.md
│   └── ADR-NNN-[decision-name].md
├── diagrams/
│   ├── system-context.md
│   ├── microservice-topology.md
│   ├── data-flow.md
│   └── deployment-architecture.md
└── MAINTENANCE_LOG.md
```

---

## 📋 Core Documents (@software-architect owns these)

### 1. SOFTWARE_DEFINITION.md

**Purpose**: Describe what B2X IS

**Must Include**:
- [ ] **Vision**: One sentence (what problem does it solve?)
- [ ] **Scope**: What's IN scope, what's OUT of scope
- [ ] **Core Functions**: Main features/capabilities (2-3 sentences each)
- [ ] **Constraints**: Technical, business, legal constraints
- [ ] **Assumptions**: What we assume to be true
- [ ] **Dependencies**: External systems, standards, regulations
- [ ] **Non-Functional Requirements**: Performance, security, availability, scalability targets

**Example Format**:
```markdown
# Software Definition - B2X

## Vision
B2X is a multi-tenant SaaS e-commerce platform enabling EU businesses to sell online with full compliance.

## Core Functions
- **Multi-tenant Store**: Each shop operates independently with isolated data
- **Product Management**: SKU-based catalog with variants, pricing, inventory
- **Checkout & Orders**: EU-compliant payment processing, VAT handling, invoicing
- **Admin Dashboard**: Real-time analytics, customer management, reporting

## Technical Constraints
- PostgreSQL only (no document databases)
- .NET 10+ (no legacy frameworks)
- Wolverine messaging (no MediatR)
- DDD bounded contexts (no monolith)
```

**Update Frequency**: Quarterly (or when scope changes)  
**Maintainer**: @software-architect + @product-owner

---

### 2. DESIGN_DECISIONS.md

**Purpose**: Document WHY architectural choices were made

**Must Include**:
- [ ] **Decision**: What was decided
- [ ] **Context**: What problem needed solving
- [ ] **Options Considered**: Alternatives that were rejected
- [ ] **Decision**: Which option was chosen
- [ ] **Rationale**: Why this option
- [ ] **Consequences**: What this enables/constrains
- [ ] **Date**: When decided
- [ ] **Owner**: Who made the decision

**Example**:
```markdown
## Microservices Architecture (DDD)

**Context**: B2X needs independent scaling of product catalog vs. checkout

**Options Considered**:
- Monolith (all features in one service)
- Microservices with shared database (loose coupling)
- Microservices with separate databases (full isolation)

**Decision**: Microservices with separate databases per bounded context

**Rationale**:
- Each shop has different catalog sizes (10-10,000 products)
- Checkout must scale independently during sales peaks
- Catalog changes shouldn't affect order processing
- Enables independent deployment & scaling

**Consequences**:
- ✅ Independent scaling
- ✅ Independent deployment
- ❌ Distributed transactions (eventual consistency required)
- ❌ More complex observability
```

**Update Frequency**: When major decisions made (quarterly review)  
**Maintainer**: @software-architect + @tech-lead

---

### 3. ESTIMATIONS_AND_CAPACITY.md

**Purpose**: Quantify system capabilities and resource needs

**Must Include**:
- [ ] **Scale Targets**: How many users, shops, products
- [ ] **Performance Targets**: P95 response times, throughput
- [ ] **Capacity Planning**: Storage, compute, network needs
- [ ] **Growth Projections**: 1-year, 3-year, 5-year capacity
- [ ] **Cost Estimates**: Infrastructure, operations
- [ ] **Assumptions**: What these estimates assume

**Example Format**:
```markdown
# Estimations & Capacity

## Current Targets (Year 1)

**Shops**: 100 active shops  
**Users**: 1,000 total (10 per shop avg)  
**Products**: 10,000 total (100 per shop avg)  
**Orders**: 1,000/month (30/day)  
**Peak Traffic**: 2x during Black Friday

## Performance Targets

| Metric | Target | P95 |
|--------|--------|-----|
| Product List | < 200ms | < 500ms |
| Checkout | < 500ms | < 1s |
| Admin Report | < 2s | < 5s |
| Search | < 100ms | < 300ms |

## Capacity Estimates

**Database**: 50 GB (growth: 10 GB/year)  
**Search Index**: 5 GB (Elasticsearch)  
**Cache**: 1 GB (Redis)  
**Compute**: 4 cores (CPU), 8 GB RAM (memory)  
**Network**: 10 Mbps baseline, 100 Mbps peak

## Growth Projections

| Year | Shops | Users | Products | Storage |
|------|-------|-------|----------|---------|
| Y1 | 50 | 150-200 | 5,000-50,000 | 5-10 GB |
| Y2 | 250 | 400-500 | 25,000-250,000 | 30-50 GB |
| Y3 | 500 | 800-1,000 | 50,000-500,000 | 50-100 GB |
```

**Update Frequency**: Quarterly (track actuals vs estimates)  
**Maintainer**: @software-architect + @devops-engineer

---

### 4. ARCHITECTURE DECISION RECORDS (ADR/)

**Purpose**: Formal record of significant architectural decisions

**File Format**: `ADR-NNN-[decision-name].md` (e.g., ADR-001-WOLVERINE_OVER_MEDIATR.md)

**Template**:
```markdown
# ADR-NNN: [Decision Title]

**Status**: Accepted / Pending / Superseded  
**Date**: YYYY-MM-DD  
**Deciders**: @role1, @role2  

## Problem
Describe the architectural issue or question that needed resolution.

## Options Considered
1. **Option A**: Description, pros/cons
2. **Option B**: Description, pros/cons
3. **Option C**: Description, pros/cons

## Decision
We chose **Option B** because...

## Rationale
Detailed explanation of why this option is superior.

## Consequences
**Positive**:
- Enables independent scaling
- Simpler deployment

**Negative**:
- Distributed transactions required
- Increased operational complexity

## Related Decisions
- ADR-NNN: [Related decision]

## References
- [Link to documentation]
- [Link to decision discussion]
```

**Examples Included**:
- ADR-001: Wolverine over MediatR
- ADR-002: Onion Architecture
- ADR-003: Aspire for Orchestration
- ADR-004: PostgreSQL with per-service databases

**Update Frequency**: Immediately (when decision made)  
**Maintainer**: @software-architect

---

## 🔄 Maintenance & Currency

### How to Keep Docs Up-to-Date

#### 1. **Create/Update on Every Architectural Change**
When making a significant decision:
1. Create ADR (or update existing)
2. Update DESIGN_DECISIONS.md reference
3. Update SOFTWARE_DEFINITION.md if scope changed
4. Update ESTIMATIONS_AND_CAPACITY.md if new constraints discovered

#### 2. **Quarterly Review Cycle**
Every quarter:
1. @software-architect reviews all architecture docs
2. Compare documented decisions with actual implementation
3. Update with new learnings
4. Add new ADRs for decisions made last quarter
5. Update MAINTENANCE_LOG.md with review status

#### 3. **Automated Check-In**
When PRs touch architecture:
1. If backend/Domain/ changes significantly → ADR required
2. If infrastructure changes → Update ESTIMATIONS_AND_CAPACITY.md
3. If scope changes → Update SOFTWARE_DEFINITION.md

#### 4. **MAINTENANCE_LOG.md**
Track all updates:
```markdown
# Maintenance Log

## 2025-Q4 (Sep - Dec 2025)
- [2025-12-29] Created ADR-005: Phase 4 Velocity-Based Development
- [2025-12-20] Updated ESTIMATIONS_AND_CAPACITY.md: Year 2 projections
- [2025-12-15] Reviewed all ADRs: No changes needed

## 2025-Q3 (Jul - Sep 2025)
- [2025-09-30] Reviewed DESIGN_DECISIONS.md: All current
- ...
```

---

## 📋 Responsibilities by Role

| Role | Responsibility | Documents |
|------|-----------------|-----------|
| **@software-architect** | Owner of all architecture documentation | All docs in /architecture |
| **@tech-lead** | Review & approve major ADRs | ADR/ folder |
| **@devops-engineer** | Provide input on infrastructure decisions | ESTIMATIONS_AND_CAPACITY.md |
| **@product-owner** | Input on scope & business constraints | SOFTWARE_DEFINITION.md |
| **All agents** | Reference documentation for decisions | Read-only access |

---

## 🚀 Getting Started (@software-architect)

### Week 1: Create Core Documents
1. [ ] Create SOFTWARE_DEFINITION.md
2. [ ] Create DESIGN_DECISIONS.md
3. [ ] Create ESTIMATIONS_AND_CAPACITY.md
4. [ ] Create ADR-NNN for each major decision (4-6 ADRs)
5. [ ] Create MAINTENANCE_LOG.md

### Week 2: Document Current State
1. [ ] Review codebase, extract architectural decisions
2. [ ] Document in ADR format
3. [ ] Update DESIGN_DECISIONS.md with summaries
4. [ ] Create diagrams in diagrams/ folder

### Ongoing: Maintain Currency
1. [ ] Every architectural decision → Create ADR immediately
2. [ ] Every quarter → Full review & update
3. [ ] Every PR with architecture impact → Update relevant docs
4. [ ] Every month → Update MAINTENANCE_LOG.md

---

## ✅ Quality Standards

Each document must have:

- [ ] **Clear Title**: Unambiguous topic
- [ ] **Purpose Section**: Why this doc exists
- [ ] **Scope**: What's covered, what's not
- [ ] **Date Last Updated**: When was this verified current
- [ ] **Owner**: Who maintains this
- [ ] **Table of Contents** (if >500 words)
- [ ] **Examples**: Concrete illustrations (not just abstract)
- [ ] **Cross-References**: Links to related docs
- [ ] **Grammar Review**: No spelling/grammar errors
- [ ] **Up-to-Date**: Reflects current implementation

---

## 📊 Document Relationship Map

```
SOFTWARE_DEFINITION.md
    ↓ (What the system does)
DESIGN_DECISIONS.md
    ↓ (Why architectural choices were made)
ADR/ (Architecture Decision Records)
    ↓ (Formal record of each decision)
ESTIMATIONS_AND_CAPACITY.md
    ↓ (Resource implications of decisions)
DDD_BOUNDED_CONTEXTS.md
    ↓ (How decisions manifest in code)
ONION_ARCHITECTURE.md
    ↓ (Layer structure from decisions)
diagrams/ (Visual representations)
```

---

## 🔒 Authority & Governance

### Who Can Modify What?

**ONLY @software-architect** can modify:
- ✅ SOFTWARE_DEFINITION.md (scope changes)
- ✅ DESIGN_DECISIONS.md (architectural decisions)
- ✅ ESTIMATIONS_AND_CAPACITY.md (projections, costs, timelines)
- ✅ ADRs (Architecture Decision Records)

**All other agents**: Read-only access

### When Must Changes Happen?

**Estimations changes** (e.g., timeline, cost, capacity):
- 🔴 **REQUIRED**: During issue review when development starts
- 📋 Process: Issue created → Product Owner reviews → @software-architect validates estimations → Starts development
- ❌ NOT ALLOWED: Mid-sprint estimation changes (locks in at start)
- ❌ NOT ALLOWED: Without issue review

**Design decision changes** (new ADR needed):
- 🔴 **REQUIRED**: Before implementation starts
- 📋 Process: Issue created → @software-architect creates/reviews ADR → @tech-lead approves → Development can start
- ❌ NOT ALLOWED: Making architectural changes without ADR
- ❌ NOT ALLOWED: After code written (must be planned)

**Scope changes** (SOFTWARE_DEFINITION.md):
- 🔴 **REQUIRED**: During quarterly reviews or when scope shifts
- 📋 Process: Change identified → @product-owner confirms → @software-architect updates → Communicates impact
- ❌ NOT ALLOWED: Scope creep without documentation

### Enforcement

**@process-assistant** enforces:
- ✅ Architectural docs stay in docs/architecture/
- ✅ ADR format is consistent
- ✅ Only @software-architect can commit to these files
- ✅ Docs are referenced in PRs affecting architecture
- ✅ Quarterly review happens
- 🔴 **NEW**: Estimation changes only during issue review
- 🔴 **NEW**: Design decisions have ADRs before implementation
- 🔴 **NEW**: Scope changes logged in SOFTWARE_DEFINITION.md

### All Agents Must

- ✅ Reference architecture docs in decisions
- ✅ Follow documented architectural patterns
- ✅ Report architectural issues to @software-architect
- ✅ Request estimations/design changes during issue review (not mid-sprint)
- ✅ Never bypass @software-architect authority
- ✅ Never modify estimations without approval

---

## 📚 Required Reading for All

**First Time**:
1. Read: SOFTWARE_DEFINITION.md (5 min) - Understand scope
2. Read: DESIGN_DECISIONS.md (10 min) - Know the WHY
3. Skim: ADR/ folder (5 min) - See decision format

**Quarterly**:
1. Read: MAINTENANCE_LOG.md (5 min) - What changed
2. Review: Relevant ADRs (10 min) - Any decisions affecting your work

**Before Major Changes**:
1. Read: Relevant ADRs (10 min) - Ensure alignment
2. Check: ESTIMATIONS_AND_CAPACITY.md (5 min) - Resource implications
3. Propose: ADR for new decision (if needed)

---

## 🎯 Success Criteria

**Documentation is successful when**:

✅ Team refers to docs when making architectural decisions  
✅ New team members onboard faster (docs explain the WHY)  
✅ Architecture stays consistent across all services  
✅ Estimations match reality (within 20%)  
✅ No "tribal knowledge" (everything documented)  
✅ ADRs prevent repeated debates  
✅ Quarterly reviews catch drift early  

---

## 📞 Questions & Updates

**Question**: "Why did we choose architecture X?"  
**Answer**: Check DESIGN_DECISIONS.md or relevant ADR

**Question**: "Will the system scale to 1 million users?"  
**Answer**: Check ESTIMATIONS_AND_CAPACITY.md

**Need to propose new decision**: Create ADR following template  
**Found documentation drift**: Alert @software-architect  
**Quarterly review**: Schedule with @software-architect + @tech-lead  

---

## 📚 Related Standards & Guides

See also:
- [AGENT_ROLE_DOCUMENTATION_GUIDELINES.md](../AGENT_ROLE_DOCUMENTATION_GUIDELINES.md) - How to create agent instruction documentation
- [AGENT_ROLE_DOCUMENTATION_QUICK_REFERENCE.md](../AGENT_ROLE_DOCUMENTATION_QUICK_REFERENCE.md) - Quick template for creating agent docs
- [copilot-instructions.md](../../.github/copilot-instructions.md) - Agent instructions (must follow architecture standards)
- [TESTING_FRAMEWORK_GUIDE.md](../archive/architecture-docs/TESTING_FRAMEWORK_GUIDE.md) - Testing patterns & standards
- [APPLICATION_SPECIFICATIONS.md](../APPLICATION_SPECIFICATIONS.md) - Feature specification format

---

## 🔄 Version History

| Date | Change | Owner |
|------|--------|-------|
| 2025-12-29 | Created initial standards | @software-architect |

---

**Last Updated**: 29. Dezember 2025  
**Next Review**: 2026-03-29 (Q1)  
**Owner**: @software-architect  
**Authority**: @process-assistant enforces standards
