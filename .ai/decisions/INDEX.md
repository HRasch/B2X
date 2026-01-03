---
docid: ADR-INDEX
title: Architecture & Knowledge Base Index
owner: "@Architect"
status: Active
---

# Architecture & Knowledge Base Index

**DocID**: `ADR-INDEX`  
**Last Updated:** December 31, 2025  
**Maintained By:** @Architect, @TechLead  
**Status:** ✅ Active

See [DOCUMENT_REGISTRY.md](../DOCUMENT_REGISTRY.md) for all DocIDs.

---

## 📚 Knowledge Base Organization

### Architecture Reviews & Assessments

#### Latest Review (Dec 30, 2025)
- **[Architecture Review 2025-12-30](./ARCHITECTURE_REVIEW_2025_12_30.md)** ⭐ START HERE
  - Comprehensive system assessment
  - Component scorecards (DDD 9/10, Microservices 9/10, etc.)
  - Identified issues & recommendations
  - Scalability analysis with 3-phase roadmap
  - 38 pages of detailed analysis

- **[Architecture Review Summary](./ARCHITECTURE_REVIEW_SUMMARY.md)** 📋 QUICK READ
  - Executive summary
  - Key findings
  - Recommendations by priority
  - Team assignments

---

### Architecture Decision Records (ADRs)

**ADR Index:** System decisions documented for future reference

| DocID | Title | Status | Impact |
|-------|-------|--------|--------|
| `ADR-001` | Wolverine over MediatR | ✅ Accepted | High |
| `ADR-002` | Onion Architecture | ✅ Accepted | High |
| `ADR-003` | Aspire Orchestration | ✅ Accepted | High |
| `ADR-004` | PostgreSQL Multitenancy | ✅ Accepted | High |
| `ADR-028` | AI Consumption Monitoring for MCP | 📋 Proposed | High |

#### ADR-001: Event-Driven Architecture with Wolverine CQRS
- **File:** [ADR-001-event-driven-architecture.md](./ADR-001-event-driven-architecture.md)
- **DocID:** `ADR-001`
- **Status:** ✅ Accepted
- **Impact:** High - Core system communication pattern
- **Summary:**
  - Event-driven architecture chosen for loose coupling
  - Wolverine CQRS integration for command/query separation
  - RabbitMQ for production, local queues for development
  - Consequences: Eventual consistency, resilience, scalability
  - Alternatives considered: REST, GraphQL, shared database

**Key Sections:**
- Problem: Cross-service coordination needed
- Solution: Event-driven with Wolverine
- Implementation: Code examples, configuration
- Monitoring: Metrics & observability
- Related: `[ADR-002]`, `[ADR-003]`, `[ADR-004]`

#### ADR-028: AI Consumption Monitoring and Limiting for MCP Server Operations
- **File:** [ADR-028-ai-consumption-monitoring.md](./ADR-028-ai-consumption-monitoring.md)
- **DocID:** `ADR-028`
- **Status:** 📋 Proposed
- **Impact:** High - AI cost control and security
- **Summary:**
  - Centralized AI service gateway for MCP operations
  - Per-tenant rate limiting and budget controls
  - Real-time cost monitoring with alerts and hard limits
  - Complete audit trail for compliance
  - Security controls: API key isolation, request signing, abuse detection

**Key Sections:**
- Problem: Uncontrolled AI costs and security risks
- Solution: Comprehensive monitoring and limiting system
- Implementation: Gateway architecture, rate limiting, cost control
- Security: API key management, request validation, abuse detection
- Related: `[ADR-022]`, `[KB-017]`, `[CMP-002]`

#### ADR-002: Multi-Database per Bounded Context (Planned)
- **Status:** 📋 Scheduled for Sprint 1
- **Rationale:** Service autonomy, independent scaling, data isolation
- **Expected:** Week of Jan 6

#### ADR-003: API Gateway Pattern (Planned)
- **Status:** 📋 Scheduled for Sprint 1
- **Rationale:** Single entry point, authentication enforcement, versioning
- **Expected:** Week of Jan 6

#### ADR-004: Saga Pattern for Distributed Transactions (Planned)
- **Status:** 📋 Scheduled for Sprint 2
- **Rationale:** Complex multi-service workflows, rollback capability
- **Expected:** Week of Jan 13

---

### Knowledge Base - Architecture

Located: `.ai/knowledgebase/architecture/`

#### Service Communication Guide
- **File:** [SERVICE_COMMUNICATION.md](../knowledgebase/architecture/SERVICE_COMMUNICATION.md)
- **Purpose:** Practical guide to service-to-service communication
- **When to Read:** Starting new inter-service integration

**Contents:**
1. **Synchronous HTTP/REST Pattern**
   - When to use: Request-response immediate
   - Example: API Gateway to Catalog Service
   - Code examples: Calling services
   - Pros/Cons analysis
   - Guidelines: Timeouts, circuit breakers

2. **Asynchronous Events Pattern** (Recommended)
   - When to use: State change notifications
   - Example: Product Inventory Update
   - Code examples: Publishing & handling events
   - Pros/Cons analysis
   - Guidelines: Idempotency, error handling

3. **Saga Pattern** (Complex Workflows)
   - Orchestration-based vs Choreography-based
   - Multi-step process example: Order creation
   - Compensation handlers for rollback
   - Code examples for both patterns
   - Error handling strategies

4. **Event Contracts**
   - Standard event structure
   - Required fields: EventId, AggregateId, Version, CorrelationId
   - Event versioning strategy
   - Backward compatibility approach

5. **Error Handling**
   - Retry strategies with Wolverine
   - Dead letter queue handling
   - Fallback mechanisms
   - Alert & monitoring

6. **Best Practices**
   - ✅ DO: Include context in events
   - ✅ DO: Handle idempotency
   - ✅ DO: Log everything
   - ❌ DON'T: Require service lookups
   - ❌ DON'T: Create circular dependencies
   - ❌ DON'T: Ignore failures

7. **Monitoring & Debugging**
   - Key metrics (throughput, latency, errors)
   - Debugging distributed events
   - Correlation ID tracing

---

### Knowledge Base - Operations

Located: `.ai/knowledgebase/operations/`

#### Scalability & Performance Guide
- **File:** [SCALABILITY_GUIDE.md](../knowledgebase/operations/SCALABILITY_GUIDE.md)
- **Purpose:** Scale from 1K to 50K+ req/sec
- **When to Read:** Planning capacity or scaling work

**Contents:**
1. **Current Capacity Analysis**
   - Baseline metrics (1K-5K req/sec)
   - Optimization targets (5K-10K req/sec)
   - Production goals (10K-50K req/sec)
   - Bottleneck identification matrix

2. **Horizontal Scaling**
   - Stateless service design (already implemented)
   - Kubernetes deployment example
   - Auto-scaling configuration
   - Load balancer setup

3. **Caching Strategy**
   - Current Redis setup (write-through)
   - Strategy 1: Write-through (current)
   - Strategy 2: Write-behind (high-traffic)
   - Strategy 3: Event-driven invalidation
   - Redis cluster for production
   - Code examples: Cached repository

4. **Database Optimization**
   - N+1 query prevention (eager loading)
   - Connection pooling sizing
   - Read replicas for reporting
   - Index strategy with examples
   - Slow query monitoring

5. **Search Optimization (Elasticsearch)**
   - Configuration setup
   - Sharding strategy
   - Index lifecycle (monthly rotation)
   - Performance tuning

6. **Performance Monitoring**
   - Metrics dashboard structure
   - Application Insights integration
   - Monitoring code examples
   - Custom metrics

7. **Load Testing**
   - Test plan (ramp-up, plateau, spike)
   - Target metrics
   - K6 load test script example

8. **Scaling Roadmap**
   - Phase 1 (Now): 1K-5K req/sec ✅
   - Phase 2 (Month 3-6): 5K-10K req/sec
   - Phase 3 (Month 6-12): 10K-50K req/sec
   - Actions per phase

---

## 🎯 Quick Navigation by Use Case

### "I need to understand the overall architecture"
→ Start: [ARCHITECTURE_REVIEW_2025_12_30.md](./ARCHITECTURE_REVIEW_2025_12_30.md)  
→ Then: [ARCHITECTURE_REVIEW_SUMMARY.md](./ARCHITECTURE_REVIEW_SUMMARY.md)

### "I'm implementing service-to-service communication"
→ Start: [SERVICE_COMMUNICATION.md](../knowledgebase/architecture/SERVICE_COMMUNICATION.md)  
→ Reference: [ADR-001](./ADR-001-event-driven-architecture.md)

### "I need to make a new architectural decision"
→ Start: Review existing ADRs [ADR-001](./ADR-001-event-driven-architecture.md)  
→ Template: Use ADR-001 as template  
→ Store: Create ADR-00N in `.ai/decisions/`

### "I need to scale the system"
→ Start: [SCALABILITY_GUIDE.md](../knowledgebase/operations/SCALABILITY_GUIDE.md)  
→ Reference: Current capacity analysis, bottleneck identification

### "I need to optimize database performance"
→ Section: SCALABILITY_GUIDE.md → Database Optimization  
→ Topics: Query patterns, connection pooling, indexing, replication

### "I need to set up observability"
→ Section: SCALABILITY_GUIDE.md → Performance Monitoring  
→ Reference: Metrics dashboard, code examples

### "I need to understand event-driven architecture"
→ Start: [ADR-001](./ADR-001-event-driven-architecture.md)  
→ Deep-dive: [SERVICE_COMMUNICATION.md](../knowledgebase/architecture/SERVICE_COMMUNICATION.md) → Asynchronous Events Pattern

---

## 📊 Document Statistics

| Document | Size | Lines | Type | Status |
|----------|------|-------|------|--------|
| ARCHITECTURE_REVIEW_2025_12_30.md | 21 KB | 850 | Report | ✅ Complete |
| ADR-001-event-driven-architecture.md | 7 KB | 280 | Decision | ✅ Accepted |
| SERVICE_COMMUNICATION.md | 15 KB | 580 | Guide | ✅ Complete |
| SCALABILITY_GUIDE.md | 15 KB | 650 | Guide | ✅ Complete |
| ARCHITECTURE_REVIEW_SUMMARY.md | 11 KB | 400 | Summary | ✅ Complete |
| **TOTAL** | **69 KB** | **2,760** | | ✅ Active |

**Code Examples:** 50+  
**Diagrams:** 12+  
**Recommendations:** 8 (prioritized)

---

## 📅 Maintenance Schedule

### Weekly
- Monitor for new architectural issues
- Track ADR status
- Update if new patterns discovered

### Monthly
- Review knowledge base accuracy
- Ensure links are current
- Add new decision records as needed

### Quarterly (Every 3 Months)
- Full architecture review
- Measure against scorecard
- Update recommendations
- Publish quarterly ADR digest

**Next Review:** March 30, 2026

---

## 🔄 Content Update Checklist

When new architecture decisions are made:

- [ ] Create new ADR document
  - Use [ADR-001](./ADR-001-event-driven-architecture.md) as template
  - Document problem, solution, consequences
  - Include alternatives considered
  - Add related ADRs

- [ ] Update relevant knowledge base pages
  - Link from SERVICE_COMMUNICATION.md if pattern-related
  - Link from SCALABILITY_GUIDE.md if performance-related
  - Add to ADR Index

- [ ] Update this INDEX
  - Add link to new ADR
  - Update "Quick Navigation"
  - Update statistics

- [ ] Notify team
  - Share in sprint planning
  - Reference in code reviews
  - Include in training

---

## 👥 Ownership & Responsibilities

| Role | Responsibility | Review Schedule |
|------|-----------------|-----------------|
| **@Architect** | ADR creation, system design decisions | Ongoing |
| **@TechLead** | Code quality, pattern guidance | Ongoing |
| **@Backend** | Service implementation validation | Quarterly |
| **@DevOps** | Infrastructure & scaling | Quarterly |
| **@Security** | Security architecture | Quarterly |
| **GitHub Copilot** | Knowledge base curation & updates | Monthly |

---

## 📚 Related Documents

### In This Directory (.ai/decisions/)
- [ARCHITECTURE_REVIEW_2025_12_30.md](./ARCHITECTURE_REVIEW_2025_12_30.md)
- [ADR-001-event-driven-architecture.md](./ADR-001-event-driven-architecture.md)
- [ARCHITECTURE_REVIEW_SUMMARY.md](./ARCHITECTURE_REVIEW_SUMMARY.md)

### In Knowledge Base (.ai/knowledgebase/)
- [architecture/SERVICE_COMMUNICATION.md](../knowledgebase/architecture/SERVICE_COMMUNICATION.md)
- [operations/SCALABILITY_GUIDE.md](../knowledgebase/operations/SCALABILITY_GUIDE.md)

### Project Root
- [README.md](../../../README.md) - Project overview
- [QUICK_START_GUIDE.md](../../QUICK_START_GUIDE.md) - Getting started
- [.ai/collaboration/AGENT_TEAM_REGISTRY.md](../../../.ai/collaboration/AGENT_TEAM_REGISTRY.md) - Agent descriptions

---

## 🎓 Learning Path

**For New Team Members:**
1. Read: [ARCHITECTURE_REVIEW_SUMMARY.md](./ARCHITECTURE_REVIEW_SUMMARY.md) (10 min)
2. Read: [Service Communication Guide](../knowledgebase/architecture/SERVICE_COMMUNICATION.md) (30 min)
3. Reference: [ARCHITECTURE_REVIEW_2025_12_30.md](./ARCHITECTURE_REVIEW_2025_12_30.md) (deep dive)
4. Review: [ADR-001](./ADR-001-event-driven-architecture.md) (decision rationale)

**For Scaling Work:**
1. Read: [SCALABILITY_GUIDE.md](../knowledgebase/operations/SCALABILITY_GUIDE.md) (30 min)
2. Identify: Current bottleneck
3. Reference: Relevant scaling phase
4. Implement: Recommended optimization

**For Architecture Decisions:**
1. Review: Existing ADRs
2. Create: New ADR using template
3. Discuss: With @Architect & @TechLead
4. Document: Final decision & rationale

---

**Last Updated:** December 30, 2025  
**Maintained By:** @Architect, @TechLead, GitHub Copilot  
**Next Review:** January 2, 2026

---

*This index is the central hub for all B2Connect architecture documentation.*  
*Keep it current. Link to it from relevant work. Use it to onboard new team members.*
