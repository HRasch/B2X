# 🤖 AI Agent Ecosystem - Quick Start

**B2Connect Development Workflow** - Complete Agent Configuration  
**Last Updated**: 29. Dezember 2025

---

## 🎯 Your Agent Team

### Authority Level (Claude Sonnet 4.5)
These agents make architectural decisions affecting the entire system.

| Agent | Purpose | Use When |
|-------|---------|----------|
| 🏛️ **@software-architect** | System-wide design decisions | Service design, data architecture, compliance integration |
| 👔 **@tech-lead** | Code quality & technical direction | Complex algorithms, design patterns, team mentoring |

### Development Level (Claude Haiku 4.5)
These agents build features. They escalate to Tech Lead and Software Architect as needed.

| Agent | Purpose | Use When |
|-------|---------|----------|
| 💻 **@backend-developer** | .NET/Wolverine microservices | Building API handlers, domain logic |
| 💼 **@backend-admin** | Admin API operations | User/tenant management, configurations |
| 🛍️ **@backend-store** | Public catalog & checkout | Products, carts, orders, invoices |
| 🎨 **@frontend-developer** | Vue.js 3 components | Building accessible, responsive UI |
| 📊 **@frontend-admin** | Admin dashboard UI | Tables, forms, analytics, admin workflows |
| 🛒 **@frontend-store** | E-commerce storefront | Products, checkout, legal compliance |

### Infrastructure & Automation
| Agent | Purpose | Use When |
|-------|---------|----------|
| 🖥️ **@cli-developer** (NEW) | DevOps CLI tool | Microservice operations, bulk data, automation |
| ⚙️ **@devops-engineer** | Cloud infrastructure | Deployment, monitoring, scaling |

### Testing & Quality
| Agent | Purpose | Use When |
|-------|---------|----------|
| 🧪 **@qa-engineer** | Test automation & compliance | xUnit, Playwright, P0.1-P0.9 compliance tests |
| 🎯 **@qa-frontend** | E2E & UI testing | User workflows, responsive design, accessibility |
| 🔒 **@qa-pentester** | Security testing | Vulnerabilities, OWASP top 10 |
| ⚡ **@qa-performance** | Load & performance testing | Scalability, response times, bottlenecks |

### Security & Compliance
| Agent | Purpose | Use When |
|-------|---------|----------|
| 🔐 **@security-engineer** | Encryption & audit logging | PII protection, incident response, NIS2 |

### Leadership & Coordination
| Agent | Purpose | Use When |
|-------|---------|----------|
| 📋 **@product-owner** | Feature prioritization | Roadmap, go/no-go decisions, stakeholder comms |
| 👨‍💼 **@scrum-master** | Process & coordination | Retrospectives, standups, disagreement resolution |

---

## 🚀 How to Use

### "I'm implementing a feature"
1. Start with **@backend-developer** or **@frontend-developer**
2. For complex problems → Ask **@tech-lead**
3. For system impact → **@tech-lead** escalates to **@software-architect**

### "I need to build a CLI tool"
1. Start with **@cli-developer**
2. For command design → Consult **@tech-lead**
3. For integration architecture → Ask **@software-architect**

### "I have a test question"
1. **@qa-engineer** for unit/integration tests
2. **@qa-frontend** for E2E/UI tests
3. **@qa-pentester** for security tests
4. **@qa-performance** for load/scalability
5. For test architecture → Ask **@software-architect**

### "I'm making an architectural decision"
1. Always ask **@software-architect** FIRST
2. Involves code patterns? Include **@tech-lead**
3. Document as ADR (Architecture Decision Record)

### "There's a conflict between agents"
1. Let **@scrum-master** facilitate discussion
2. For technical disagreements → Escalate to **@tech-lead**
3. For architectural disagreements → Escalate to **@software-architect**

---

## 📊 Escalation Hierarchy

```
┌─────────────────────────────────────────┐
│       🏛️ Software Architect             │
│   (System-wide decisions, P0.1-P0.9)    │
│   Uses: Claude Sonnet 4.5              │
└──────────────┬──────────────────────────┘
               │
               │ reports to / consults for major decisions
               ↓
┌──────────────────────────────────────────┐
│         👔 Tech Lead                      │
│   (Code quality, patterns, direction)     │
│   Uses: Claude Sonnet 4.5                │
└──────────────┬───────────────────────────┘
               │
        ┌──────┴──────┬──────────┬────────┬─────────┐
        ↓             ↓          ↓        ↓         ↓
    💻Backend    🎨Frontend   🖥️CLI   ⚙️DevOps  🔐Security
   (Haiku 4.5)  (Haiku 4.5) (Haiku) (Haiku)  (Haiku)
   
    + All QA agents (Haiku 4.5)
    + Product Owner (Haiku 4.5)
    + Scrum Master (Haiku 4.5)
```

---

## 🎬 Example Workflows

### Adding a New Feature (Feature Request → Implementation)

```
Product Owner: "We need bulk CSV product import"
    ↓
Software Architect: "Design the feature:
  - Backend API endpoint (backend-store)
  - CLI command for bulk upload (cli-developer)
  - Admin UI for uploads (frontend-admin)
  - Data validation and error handling
  - Multi-tenant safety"
    ↓
Backend Store Dev: "Build POST /products/import API"
    ↓
CLI Developer: "Build 'b2connect products import' command"
    ↓
Frontend Admin Dev: "Build upload UI in admin dashboard"
    ↓
QA Engineer: "Write tests for import logic"
    ↓
QA Performance: "Load test bulk upload with 100K products"
    ↓
Release!
```

### Solving a Performance Issue

```
QA Performance: "Product listing takes 2 seconds"
    ↓
Software Architect: "Analysis:
  - Root cause: N+1 queries on product categories
  - Solution: Implement Redis caching
  - Add cache invalidation on product updates
  - Update event architecture to handle cache"
    ↓
Tech Lead: "Review implementation pattern"
    ↓
Backend Store Dev: "Implement caching"
    ↓
DevOps Engineer: "Configure Redis, set up monitoring"
    ↓
QA Performance: "Verify <500ms response time"
    ↓
Fixed!
```

### Creating a New Service

```
Developer: "We need a notification service"
    ↓
Software Architect: "Design:
  - Notification bounded context
  - Event-driven (subscribes to OrderCreated, OrderShipped)
  - Database schema (notifications table)
  - Delivery strategies (email, SMS, push)
  - Integration with existing Identity service
  - Multi-tenant isolation requirements"
    ↓
Tech Lead: "Review design patterns"
    ↓
Backend Developer: "Implement service"
    ↓
DevOps Engineer: "Deploy service to Aspire"
    ↓
QA Engineer: "Write integration tests"
    ↓
QA Pentester: "Security testing"
    ↓
New service deployed!
```

---

## 📚 Key Documents

| Document | Location | Purpose |
|----------|----------|---------|
| Agent Workflow Integration | [.github/AGENT_WORKFLOW_INTEGRATION.md](.github/AGENT_WORKFLOW_INTEGRATION.md) | Complete integration guide with examples |
| Software Architect Guide | [.github/agents/software-architect.agent.md](.github/agents/software-architect.agent.md) | Full software architect responsibilities |
| Tech Lead Guide | [.github/agents/tech-lead.agent.md](.github/agents/tech-lead.agent.md) | Tech lead responsibilities & standards |
| Backend Developer | [.github/agents/backend-developer.agent.md](.github/agents/backend-developer.agent.md) | Wolverine, DDD, EF Core patterns |
| CLI Developer | [.github/agents/cli-developer.agent.md](.github/agents/cli-developer.agent.md) | CLI tool development guide |
| All Agents | [.github/agents/](.github/agents/) | Complete agent definitions |

---

## ✅ Quick Reference

### Asking for Help

**"How do I implement X?"**
```
Ask: @backend-developer or @frontend-developer
They'll escalate to @tech-lead if needed
```

**"What's the best architecture for X?"**
```
Ask: @software-architect directly
```

**"How do I build a CLI command for X?"**
```
Ask: @cli-developer
```

**"Is this code quality good?"**
```
Ask: @tech-lead
```

**"Is this secure?"**
```
Ask: @security-engineer
Then review with @software-architect if architectural changes needed
```

### New to the Team?

1. ✅ Read [AGENT_WORKFLOW_INTEGRATION.md](.github/AGENT_WORKFLOW_INTEGRATION.md) (15 min)
2. ✅ Find your agent in `.github/agents/` and read it
3. ✅ Bookmark the escalation hierarchy above
4. ✅ When stuck, ask your agent (they know what to do!)

---

## 🔗 Agent Definitions

All agent definitions live in `.github/agents/`:
- `software-architect.agent.md` - System authority
- `tech-lead.agent.md` - Technical direction
- `backend-developer.agent.md` - C#/.NET APIs
- `cli-developer.agent.md` - DevOps automation
- ... and 20+ more specialized agents

Each includes:
- Role description
- Expertise areas
- Responsibilities
- Escalation points
- Focus areas

---

## 🎯 Success Metrics

Your agent team is working well when:
- ✅ Architectural decisions documented as ADRs
- ✅ Build times < 10 seconds
- ✅ Test coverage > 80%
- ✅ Features deploy weekly
- ✅ P0.1-P0.9 compliance maintained
- ✅ Team velocity steady (or improving)
- ✅ Technical debt managed (not accumulating)

---

**Ready to work?** Pick your agent and start building! 🚀

