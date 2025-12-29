# AI Agent Workflow Integration Guide

**B2Connect Development Team** - Agent Hierarchy & Collaboration Patterns  
**Last Updated**: 29. Dezember 2025

---

## 🏗️ New Agent Hierarchy

### Level 1: Authority & Architecture
- **🏛️ Software Architect** (`@software-architect`)
  - **Model**: Claude Sonnet 4.5 (advanced reasoning)
  - **Authority**: System-wide structural decisions
  - **Escalation point**: All architectural changes
  - **Key decisions**: Service boundaries, data architecture, event flows, security patterns

- **👔 Tech Lead** (`@tech-lead`)
  - **Model**: Claude Sonnet 4.5 (advanced reasoning)
  - **Authority**: Technical direction, code quality, complex design patterns
  - **Reports to**: Software Architect for system structure
  - **Key decisions**: Code patterns, performance optimization, technology selection

---

### Level 2: Development Teams
All development agents report to Tech Lead for complex issues and to Software Architect for structural changes.

#### Backend Development
- **💻 Backend Developer** → Asks @tech-lead for complex problems → Asks @software-architect for service design
- **💼 Backend Admin** → Multi-service admin workflows → @software-architect
- **🛍️ Backend Store** → Catalog/checkout architecture → @software-architect

#### Frontend Development
- **🎨 Frontend Developer** → Complex UI architecture → @tech-lead → @software-architect
- **📊 Frontend Admin** → Admin state management → @software-architect + @cli-developer collaboration
- **🎨 Frontend Store** → Checkout flow architecture → @software-architect

#### CLI & Infrastructure
- **🖥️ CLI Developer** (NEW)
  - **Model**: Claude Haiku 4.5
  - **Authority**: Microservice operations, DevOps automation, CLI architecture
  - **Reports to**: Tech Lead for complex patterns, @software-architect for CLI architecture
  - **Works with**: @devops-engineer, @frontend-admin (bulk operations), @backend-admin (admin workflows)

- **⚙️ DevOps Engineer** → Infrastructure architecture → @software-architect

#### Security & Quality
- **🔐 Security Engineer** → Security architecture changes → @software-architect
- **🧪 QA Engineer** → Test architecture & CLI testing → @cli-developer + @software-architect
- **🎯 QA Frontend** → Complex E2E scenarios → @software-architect
- **🔒 QA Pentester** → Security architecture → @software-architect
- **⚡ QA Performance** → Performance architecture → @software-architect + @cli-developer

#### Leadership & Coordination
- **📋 Product Owner** → Architecture decisions → @software-architect
- **👨‍💼 Scrum Master** → Escalates architectural disagreements → @software-architect
- **🎬 Scrum Master** → Tech disagreements → @tech-lead
- **🎬 Scrum Master** → CLI design → @cli-developer

---

## 🔄 Collaboration Patterns

### When to Ask Software Architect
✅ **Service Design & Integration**
- Creating new microservices or bounded contexts
- Changing service boundaries
- Designing multi-service communication patterns
- Event schema design

✅ **Data Architecture**
- Database schema changes affecting multiple services
- Multi-tenancy pattern changes
- Replication or synchronization strategies
- Query optimization across services

✅ **System-Wide Concerns**
- Scalability requirements
- Performance optimization strategies
- Encryption/security architecture
- Compliance integration (P0.1-P0.9)

✅ **Complex Technical Decisions**
- Caching strategies
- Message queue design
- API versioning strategies
- State management architecture

### When to Ask Tech Lead
✅ **Code Quality & Patterns**
- Complex async/await patterns
- Performance optimization (single service)
- Architecture patterns within a service
- Testing strategies
- Library/framework selection

✅ **Implementation Guidance**
- How to implement a feature
- Code review feedback
- Design pattern questions
- Technology choice for a feature

### When to Ask CLI Developer
✅ **DevOps Automation**
- CLI commands for operations
- Bulk data operations
- Service management workflows
- Administrative automation
- Database migrations via CLI

✅ **CLI Architecture**
- CLI command structure
- Spectre.Console output design
- Configuration management
- Integration with backend services

---

## 📊 Decision Matrix

| Decision Type | Who Decides | Consultation |
|---|---|---|
| **New Service Creation** | Software Architect | Tech Lead, Product Owner |
| **Service Boundary Change** | Software Architect | Tech Lead, affected developers |
| **Database Schema Change** | Software Architect | Backend Dev, Database Expert |
| **API Contract Change** | Tech Lead | Software Architect (multi-service) |
| **Code Pattern** | Tech Lead | Backend Dev (implementing) |
| **Performance Issue** | Software Architect | QA Performance, affected Dev |
| **Security Issue** | Software Architect | Security Engineer |
| **CLI Design** | CLI Developer | Tech Lead, Software Architect |
| **Feature Implementation** | Backend/Frontend Dev | Tech Lead (if complex) |
| **Bulk Operation** | CLI Developer | Frontend Admin, Backend Admin |
| **Testing Strategy** | QA Engineer | Software Architect (cross-service) |

---

## 🎯 Escalation Workflow

```
Developer
    ↓ (Complex problem)
Tech Lead
    ↓ (Affects multiple services or system structure)
Software Architect
    ↓ (Architectural approval required)
Implementation proceeds with feedback
```

### Examples

**Backend Developer Question**:
> "How do I implement product caching with multi-tenant isolation?"
- **Ask**: @tech-lead for implementation patterns
- **If**: Architecture affects multiple services
- **Then**: @tech-lead consults @software-architect

**Frontend Developer Question**:
> "Should we use Pinia or Context API for checkout state?"
- **Ask**: @tech-lead for technology choice
- **If**: State management involves server communication
- **Then**: @tech-lead may consult @software-architect

**CLI Developer Question**:
> "How should bulk product import work?"
- **Ask**: @software-architect directly (involves service integration)
- **Coordinate**: @backend-store, @frontend-admin for UI integration

**Architectural Question**:
> "Should search be a separate service or embedded in Catalog?"
- **Ask**: @software-architect immediately
- **Include**: Tech Lead, affected developers in discussion
- **Outcome**: Architecture decision record (ADR)

---

## 🔗 Coordination Between New Agents

### Software Architect ↔ Tech Lead
- **Software Architect** designs system-wide patterns
- **Tech Lead** ensures implementation follows patterns
- **Collaboration**: Architecture reviews, pattern validation, team alignment

### Tech Lead ↔ CLI Developer
- **Tech Lead** provides coding patterns
- **CLI Developer** implements DevOps automation
- **Collaboration**: CLI command patterns, service integration, testing

### CLI Developer ↔ Backend Developers
- **Backend Developers** expose APIs
- **CLI Developer** consumes APIs for automation
- **Coordination**: CLI requirements, API contracts

### CLI Developer ↔ Frontend Admins
- **Frontend Admin** builds UI for administrative tasks
- **CLI Developer** provides CLI alternative for bulk operations
- **Coordination**: Feature parity, shared business logic

### QA Engineers ↔ CLI Developer
- **QA Engineers** test system functionality
- **CLI Developer** provides CLI automation testing
- **Coordination**: Test coverage, CLI command testing, automated workflows

### Security Engineer ↔ Software Architect
- **Software Architect** designs security architecture
- **Security Engineer** verifies implementation
- **Coordination**: Encryption strategies, audit logging, incident response

---

## 📋 Agent Reference Card

| Agent | Model | Focus | Escalates To |
|---|---|---|---|
| Backend Developer | Haiku 4.5 | Service implementation | Tech Lead → Software Architect |
| Frontend Developer | Haiku 4.5 | Component & page building | Tech Lead → Software Architect |
| Tech Lead | Sonnet 4.5 | Code quality, patterns | Software Architect |
| Software Architect | Sonnet 4.5 | System design, decisions | — (Final authority) |
| CLI Developer | Haiku 4.5 | DevOps automation | Tech Lead → Software Architect |
| DevOps Engineer | Haiku 4.5 | Infrastructure | Software Architect |
| Security Engineer | Haiku 4.5 | Security implementation | Software Architect |
| QA Engineer | Haiku 4.5 | Testing automation | Software Architect (test architecture) |
| Product Owner | Haiku 4.5 | Feature prioritization | Software Architect (architecture trade-offs) |
| Scrum Master | Haiku 4.5 | Process coordination | Tech Lead / Software Architect (escalations) |

---

## 🚀 Usage Examples

### Example 1: New Service Decision
```
Developer: "We need a new notification service for order updates."

Tech Lead: "Let's check if this fits the roadmap and architecture."
→ Discusses scope, integration points

Software Architect: "Notification Service should be event-driven, 
consuming OrderCreated/OrderShipped events from the event bus. 
It owns the delivery strategy (email, SMS, push). Here's the design..."

Implementation: Backend Dev implements following the architecture
```

### Example 2: Performance Optimization
```
QA Performance: "API response time is >500ms for product listing."

Software Architect: "Let's analyze the issue. Is it:
1. Query optimization (cache product catalog)
2. Service boundary issue (split Catalog service)
3. Database design (add indexes, denormalize)

Recommendation: Implement Redis caching with 5-minute TTL for product listings,
add database index on category_id. Implement cache invalidation on product updates."

Tech Lead: "Ensures implementation follows our async/caching patterns"

Backend Dev: Implements caching solution
```

### Example 3: CLI Feature
```
Product Owner: "We need bulk product import for B2B customers."

CLI Developer: "I'll build a 'products import' CLI command that:
- Reads CSV file
- Validates against schema
- Calls backend API
- Shows progress
- Logs results"

Frontend Admin: "Also builds UI for same operation for non-technical users"

Backend Store: "Ensures API is designed to support both CLI and UI"

Software Architect: "Reviews integration pattern to ensure multi-tenant safety"
```

---

## ✅ Implementation Checklist

For all new agent interactions:
- [ ] Software Architect created and configured with Claude Sonnet 4.5
- [ ] CLI Developer created with focus on DevOps automation
- [ ] Tech Lead updated to reference Software Architect
- [ ] All developers updated to reference Tech Lead and Software Architect appropriately
- [ ] QA agents updated to coordinate with CLI Developer
- [ ] Product Owner and Scrum Master updated for escalation paths
- [ ] Documentation created (this file)
- [ ] Team informed of new agent hierarchy
- [ ] First architecture decision documented as precedent

---

**Questions?** Ask your local agent or consult the agent documentation in `.github/agents/`

