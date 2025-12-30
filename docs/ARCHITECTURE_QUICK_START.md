# Architectural Documentation - Quick Start for All Roles

**Last Updated**: 29. Dezember 2025  
**Purpose**: Get your bearings on architecture docs in 5 minutes

---

## 🎯 Where Are Architecture Docs?

**Location**: `/docs/architecture/`

**Start here**: [INDEX.md](./docs/architecture/INDEX.md) (5 min read)

---

## ⚡ By Role - What to Read

### I'm a Backend Developer

**Must read** (30 min):
1. [SOFTWARE_DEFINITION.md](./docs/architecture/SOFTWARE_DEFINITION.md) - Understand scope (5 min)
2. [DESIGN_DECISIONS.md](./docs/architecture/DESIGN_DECISIONS.md) - Understand WHY (15 min)
3. [Relevant DDD Bounded Context](./docs/architecture/DDD_BOUNDED_CONTEXTS.md) - Understand your service (10 min)

**When making a change**:
- Check DESIGN_DECISIONS.md to understand existing patterns
- If proposing new architecture, create ADR with @software-architect

---

### I'm a Frontend Developer

**Must read** (15 min):
1. [SOFTWARE_DEFINITION.md](./docs/architecture/SOFTWARE_DEFINITION.md) - What we're building (5 min)
2. [DESIGN_DECISIONS.md](./docs/architecture/DESIGN_DECISIONS.md) - Skim "Multi-tenancy" & "Event-driven" sections (10 min)

**Reference**:
- [SHARED_AUTHENTICATION.md](./docs/architecture/SHARED_AUTHENTICATION.md) - How auth works

---

### I'm a DevOps Engineer

**Must read** (20 min):
1. [ESTIMATIONS_AND_CAPACITY.md](./docs/architecture/ESTIMATIONS_AND_CAPACITY.md) - Infrastructure planning (10 min)
2. [DESIGN_DECISIONS.md](./docs/architecture/DESIGN_DECISIONS.md) - Aspire & PostgreSQL sections (10 min)

**Reference**:
- [ASPIRE_GUIDE.md](./docs/architecture/ASPIRE_GUIDE.md) - Local orchestration
- [DEPLOYMENT_ARCHITECTURE.md](./docs/architecture/diagrams/deployment-architecture.md) - Production setup

---

### I'm a Product Manager

**Must read** (10 min):
1. [SOFTWARE_DEFINITION.md](./docs/architecture/SOFTWARE_DEFINITION.md) - Scope & features (5 min)
2. [ESTIMATIONS_AND_CAPACITY.md](./docs/architecture/ESTIMATIONS_AND_CAPACITY.md) - Costs & timelines (5 min)

---

### I'm a Tech Lead / Architect

**Must read** (everything):
- All 5 core documents in `/docs/architecture/`
- All ADRs as they're created
- Quarterly full review

**Your responsibilities**:
- Approve major ADRs with @software-architect
- Update architecture docs when scope changes
- Maintain consistency across services

---

### I'm a QA Engineer

**Should read** (15 min):
1. [SOFTWARE_DEFINITION.md](./docs/architecture/SOFTWARE_DEFINITION.md) - Understand scope (5 min)
2. [ESTIMATIONS_AND_CAPACITY.md](./docs/architecture/ESTIMATIONS_AND_CAPACITY.md) - Performance targets (10 min)

---

## 📊 The 5-Minute Overview

### What is B2Connect?

**Vision**: Multi-tenant EU e-commerce platform (100+ independent shops)

**Scope**:
- ✅ Product catalogs, shopping carts, checkout
- ✅ Orders, invoicing, shipping integration
- ✅ EU compliance (VAT, GDPR, ZUGFeRD)
- ❌ Not: Subscriptions, native mobile, AI/ML

### Why This Architecture?

**Microservices** (not monolith):
- Each shop can scale independently
- Identity service vs Catalog service can scale differently

**Wolverine** (not MediatR):
- Auto-discovers HTTP endpoints from methods
- Built-in event streaming (not add-on)
- Better for distributed systems

**Onion Architecture** (per service):
- Business logic (Core) with zero framework dependencies
- Easy to test, easy to refactor

**PostgreSQL per service**:
- Each service owns its schema
- Catalog doesn't block when Identity grows

**Aspire** (for local dev):
- One command starts all 5 services
- Automatic service discovery

---

## 🔍 Finding Specific Information

### "What's the technical vision?"
→ [SOFTWARE_DEFINITION.md](./docs/architecture/SOFTWARE_DEFINITION.md)

### "Why did we choose X instead of Y?"
→ [DESIGN_DECISIONS.md](./docs/architecture/DESIGN_DECISIONS.md)

### "What's the cost?"
→ [ESTIMATIONS_AND_CAPACITY.md](./docs/architecture/ESTIMATIONS_AND_CAPACITY.md)

### "How does authentication work?"
→ [SHARED_AUTHENTICATION.md](./docs/architecture/SHARED_AUTHENTICATION.md)

### "What's the production setup?"
→ [diagrams/deployment-architecture.md](./docs/architecture/diagrams/deployment-architecture.md)

### "How do I run locally?"
→ [ASPIRE_GUIDE.md](./docs/architecture/ASPIRE_GUIDE.md)

### "What's a specific architectural decision?"
→ [ADR-001 through ADR-005](./docs/architecture/ADR/) (check INDEX.md for list)

---

## 📝 What to Do When...

### ...You're Making an Architectural Decision

1. Check [DESIGN_DECISIONS.md](./docs/architecture/DESIGN_DECISIONS.md) - Is this already decided?
2. Check [ADRs](./docs/architecture/ADR/) - Has this been formalized?
3. Propose new ADR with @software-architect
4. Get @tech-lead approval
5. Once approved, update DESIGN_DECISIONS.md + SOFTWARE_DEFINITION.md

### ...You're Proposing a Feature

1. Check [SOFTWARE_DEFINITION.md](./docs/architecture/SOFTWARE_DEFINITION.md) - Is it in scope?
2. Check relevant [DDD Bounded Context](./docs/architecture/DDD_BOUNDED_CONTEXTS.md) - Which service owns this?
3. Check [DESIGN_DECISIONS.md](./docs/architecture/DESIGN_DECISIONS.md) - What patterns apply?

### ...You Found an Error in Architecture Docs

1. Alert @software-architect immediately
2. They'll fix and update MAINTENANCE_LOG.md
3. No need to wait for quarterly review (fix immediately)

### ...You Need Infrastructure Estimates

1. See [ESTIMATIONS_AND_CAPACITY.md](./docs/architecture/ESTIMATIONS_AND_CAPACITY.md)
2. Year 1 baseline: $960/month, 4.5 engineers
3. Year 3 projection: $3,350/month, 8+ engineers

---

## 🚀 Key Metrics & Targets

### Performance

| Metric | Target |
|--------|--------|
| Product List (P95) | < 500ms |
| Checkout (P95) | < 1s |
| Search (P95) | < 100ms |
| Admin Dashboard (P95) | < 2s |

### Scale

| Metric | Year 1 | Year 2 | Year 3 |
|--------|--------|--------|--------|
| Shops | 100 | 500 | 1,000 |
| Users | 1,000 | 5,000 | 10,000 |
| Products | 10,000 | 50,000 | 100,000 |
| Storage | 50 GB | 100 GB | 200 GB |

### Availability

- **SLA Uptime**: 99.9% (44 min/year downtime)
- **MTTR**: < 5 minutes
- **Monthly Downtime**: < 2 hours

---

## 📚 Reading List by Time Available

**I have 5 minutes**:
- Read this document (INDEX.md)
- Check [SOFTWARE_DEFINITION.md](./docs/architecture/SOFTWARE_DEFINITION.md) Vision section

**I have 15 minutes**:
- Read [SOFTWARE_DEFINITION.md](./docs/architecture/SOFTWARE_DEFINITION.md) completely

**I have 30 minutes**:
- Read [SOFTWARE_DEFINITION.md](./docs/architecture/SOFTWARE_DEFINITION.md)
- Skim [DESIGN_DECISIONS.md](./docs/architecture/DESIGN_DECISIONS.md)

**I have 1 hour**:
- Read [SOFTWARE_DEFINITION.md](./docs/architecture/SOFTWARE_DEFINITION.md)
- Read [DESIGN_DECISIONS.md](./docs/architecture/DESIGN_DECISIONS.md)
- Check relevant section of [ESTIMATIONS_AND_CAPACITY.md](./docs/architecture/ESTIMATIONS_AND_CAPACITY.md)

**I have 2+ hours**:
- Read all 4 core documents completely
- Check relevant ADRs
- Review diagrams

---

## ✅ Quality Standards

All architecture docs meet these standards:

✅ **Clear Purpose**: Why the doc exists  
✅ **Scope**: What's covered & what's not  
✅ **Examples**: Concrete, not abstract  
✅ **Current**: Updated regularly (see dates)  
✅ **Owned**: Clear owner & review frequency  
✅ **Linked**: References to related docs  
✅ **Accurate**: Matches implementation  

---

## 🤝 Contributing to Architecture Docs

**Want to propose a change?**
1. Contact @software-architect
2. Explain what's wrong or missing
3. They'll evaluate and update

**Want to create a new decision?**
1. Create ADR-NNN-[decision-name].md
2. Get @tech-lead approval
3. Update [DESIGN_DECISIONS.md](./docs/architecture/DESIGN_DECISIONS.md) with summary

**Want to flag an error?**
1. Alert @software-architect immediately
2. No need to wait for scheduled review
3. Will be fixed ASAP

---

## 📞 Questions?

| Question | Answer Location |
|----------|-----------------|
| What's the vision? | SOFTWARE_DEFINITION.md |
| Why this architecture? | DESIGN_DECISIONS.md |
| Cost & timeline? | ESTIMATIONS_AND_CAPACITY.md |
| Performance targets? | ESTIMATIONS_AND_CAPACITY.md |
| How to set up locally? | ASPIRE_GUIDE.md |
| How's auth handled? | SHARED_AUTHENTICATION.md |
| Architecture decision format? | ADR/ folder or ARCHITECTURAL_DOCUMENTATION_STANDARDS.md |

**Can't find answer?**
→ Check [INDEX.md](./docs/architecture/INDEX.md) (full navigation)  
→ Ask @software-architect  

---

**Version**: 1.0  
**Last Updated**: 29. Dezember 2025  
**Owner**: @software-architect  
**Maintained By**: @process-assistant  
