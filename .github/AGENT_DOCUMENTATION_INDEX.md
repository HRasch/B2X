# 📚 AI Agent Documentation Index

**Complete Navigation for B2Connect AI Agent Ecosystem**  
**Last Updated**: 29. Dezember 2025

---

## 🚀 Getting Started (Choose Your Path)

### I'm New to the Team
👉 **Start Here**: [AGENT_QUICK_START.md](AGENT_QUICK_START.md) (15 minutes)
- Agent roster with quick lookup
- "Who to ask for what" guide
- Visual escalation hierarchy
- 3 real-world workflow examples

### I Need Integration Details
👉 **Read This**: [AGENT_WORKFLOW_INTEGRATION.md](AGENT_WORKFLOW_INTEGRATION.md) (25 minutes)
- Complete hierarchy & escalation paths
- Collaboration patterns between agents
- Decision-making matrix
- Implementation checklist

### I Want the Summary
👉 **Quick Overview**: [AGENT_INTEGRATION_SUMMARY.md](AGENT_INTEGRATION_SUMMARY.md) (10 minutes)
- What was created and updated
- Model assignments
- Integration points
- Benefits & success indicators

---

## 📋 Agent Definitions (In `.github/agents/`)

### Authority Agents (Claude Sonnet 4.5)
These make system-wide decisions affecting architecture and direction.

| Agent | File | Responsibility |
|-------|------|-----------------|
| 🏛️ Software Architect | [software-architect.agent.md](agents/software-architect.agent.md) | System design, service boundaries, compliance |
| 👔 Tech Lead | [tech-lead.agent.md](agents/tech-lead.agent.md) | Code quality, patterns, technical direction |

### Development Agents (Claude Haiku 4.5)
Backend, frontend, and CLI developers who implement features.

| Agent | File | Focus |
|-------|------|-------|
| 💻 Backend Developer | [backend-developer.agent.md](agents/backend-developer.agent.md) | C#/.NET Wolverine microservices |
| 💼 Backend Admin | [backend-admin.agent.md](agents/backend-admin.agent.md) | Admin API, tenancy, user management |
| 🛍️ Backend Store | [backend-store.agent.md](agents/backend-store.agent.md) | Catalog, cart, checkout, orders |
| 🎨 Frontend Developer | [frontend-developer.agent.md](agents/frontend-developer.agent.md) | Vue.js 3, Tailwind CSS, accessibility |
| 📊 Frontend Admin | [frontend-admin.agent.md](agents/frontend-admin.agent.md) | Admin dashboard, tables, forms, analytics |
| 🛒 Frontend Store | [frontend-store.agent.md](agents/frontend-store.agent.md) | E-commerce UI, products, checkout |

### CLI & Infrastructure
| Agent | File | Focus |
|-------|------|-------|
| 🖥️ CLI Developer | [cli-developer.agent.md](agents/cli-developer.agent.md) | DevOps automation, microservice operations |
| ⚙️ DevOps Engineer | [devops-engineer.agent.md](agents/devops-engineer.agent.md) | Infrastructure, CI/CD, monitoring |

### Testing & Quality
| Agent | File | Focus |
|-------|------|-------|
| 🧪 QA Engineer | [qa-engineer.agent.md](agents/qa-engineer.agent.md) | xUnit, Playwright, compliance testing |
| 🎯 QA Frontend | [qa-frontend.agent.md](agents/qa-frontend.agent.md) | E2E, responsive design, accessibility |
| 🔒 QA Pentester | [qa-pentesting.agent.md](agents/qa-pentesting.agent.md) | Security testing, OWASP, vulnerabilities |
| ⚡ QA Performance | [qa-performance.agent.md](agents/qa-performance.agent.md) | Load testing, scalability, bottlenecks |
| ✅ QA Reviewer | [qa-reviewer.agent.md](agents/qa-reviewer.agent.md) | Quality assurance coordination |

### Security & Compliance
| Agent | File | Focus |
|-------|------|-------|
| 🔐 Security Engineer | [security-engineer.agent.md](agents/security-engineer.agent.md) | Encryption, audit logging, incident response |
| 🏛️ Legal Compliance | [legal-compliance.agent.md](agents/legal-compliance.agent.md) | GDPR, NIS2, AI Act, E-Commerce law |

### Leadership & Process
| Agent | File | Focus |
|-------|------|-------|
| 📋 Product Owner | [product-owner.agent.md](agents/product-owner.agent.md) | Roadmap, prioritization, stakeholder comms |
| 👨‍💼 Scrum Master | [scrum-master.agent.md](agents/scrum-master.agent.md) | Process, coordination, retrospectives |

### Specialists
| Agent | File | Focus |
|-------|------|-------|
| 🎨 UI Expert | [ui-expert.agent.md](agents/ui-expert.agent.md) | User experience, design consistency |
| 👤 UX Expert | [ux-expert.agent.md](agents/ux-expert.agent.md) | User research, journey mapping |
| 📚 Documentation | [documentation-developer.agent.md](agents/documentation-developer.agent.md) | Technical docs, user guides |
| 👨‍💼 Stakeholders | [stakeholder-*.agent.md](agents/) | Business integrations (ERP, CRM, PIM, etc.) |

---

## 🏗️ Architecture & Patterns

### Development Guides
| Topic | File | Updated |
|-------|------|---------|
| Copilot Instructions Main | [copilot-instructions.md](copilot-instructions.md) | Comprehensive reference |
| Backend Developer Guide | [copilot-instructions-backend.md](copilot-instructions-backend.md) | Wolverine, EF Core, patterns |
| Frontend Developer Guide | [copilot-instructions-frontend.md](copilot-instructions-frontend.md) | Vue.js, Tailwind, accessibility |
| DevOps Engineer Guide | [copilot-instructions-devops.md](copilot-instructions-devops.md) | Aspire, infrastructure |
| QA Engineer Guide | [copilot-instructions-qa.md](copilot-instructions-qa.md) | Testing, compliance (52 tests) |
| Security Engineer Guide | [copilot-instructions-security.md](copilot-instructions-security.md) | Encryption, audit logging |

### Integration & Coordination
| Document | Purpose | Updated |
|----------|---------|---------|
| **AGENT_QUICK_START.md** | Quick reference for all agents | ✅ NEW |
| **AGENT_WORKFLOW_INTEGRATION.md** | Complete integration guide | ✅ NEW |
| **AGENT_INTEGRATION_SUMMARY.md** | What was done & why | ✅ NEW |
| **AGENT_DOCUMENTATION_INDEX.md** | This file (navigation) | ✅ NEW |

---

## 🎯 When to Use Each Document

### "I'm building a feature"
1. Your specialist agent (backend-developer, frontend-developer, etc.)
2. If complex: [copilot-instructions-backend.md](copilot-instructions-backend.md) or [copilot-instructions-frontend.md](copilot-instructions-frontend.md)
3. If stuck: Ask @tech-lead

### "I'm designing architecture"
1. [AGENT_WORKFLOW_INTEGRATION.md](AGENT_WORKFLOW_INTEGRATION.md) - decision matrix
2. [software-architect.agent.md](agents/software-architect.agent.md) - full architect guide
3. Ask @software-architect directly

### "I need to know about escalation"
1. [AGENT_QUICK_START.md](AGENT_QUICK_START.md) - hierarchy diagram
2. [AGENT_WORKFLOW_INTEGRATION.md](AGENT_WORKFLOW_INTEGRATION.md) - escalation paths
3. [scrum-master.agent.md](agents/scrum-master.agent.md) - conflict resolution

### "I'm new and need to get oriented"
1. [AGENT_QUICK_START.md](AGENT_QUICK_START.md) - 15 min overview
2. [AGENT_INTEGRATION_SUMMARY.md](AGENT_INTEGRATION_SUMMARY.md) - 10 min summary
3. [AGENT_WORKFLOW_INTEGRATION.md](AGENT_WORKFLOW_INTEGRATION.md) - detailed workflows
4. Read your specific agent definition in [agents/](agents/)

### "I have a compliance question"
1. [copilot-instructions-security.md](copilot-instructions-security.md) - security compliance
2. [legal-compliance.agent.md](agents/legal-compliance.agent.md) - legal framework
3. Ask @security-engineer or @legal-compliance

### "I need testing strategy"
1. [copilot-instructions-qa.md](copilot-instructions-qa.md) - 52 compliance tests
2. [qa-engineer.agent.md](agents/qa-engineer.agent.md) - test automation
3. [qa-frontend.agent.md](agents/qa-frontend.agent.md) - E2E testing
4. Ask @qa-engineer

---

## 📊 Agent Capabilities Matrix

| Capability | Who | Reference |
|-----------|-----|-----------|
| System Architecture | @software-architect | [software-architect.agent.md](agents/software-architect.agent.md) |
| Code Patterns | @tech-lead | [tech-lead.agent.md](agents/tech-lead.agent.md) |
| Backend Implementation | @backend-developer | [backend-developer.agent.md](agents/backend-developer.agent.md) |
| Frontend Implementation | @frontend-developer | [frontend-developer.agent.md](agents/frontend-developer.agent.md) |
| CLI Tool Building | @cli-developer | [cli-developer.agent.md](agents/cli-developer.agent.md) |
| Test Automation | @qa-engineer | [qa-engineer.agent.md](agents/qa-engineer.agent.md) |
| Security Testing | @qa-pentester | [qa-pentesting.agent.md](agents/qa-pentesting.agent.md) |
| Performance Testing | @qa-performance | [qa-performance.agent.md](agents/qa-performance.agent.md) |
| Encryption & Compliance | @security-engineer | [security-engineer.agent.md](agents/security-engineer.agent.md) |
| Legal Framework | @legal-compliance | [legal-compliance.agent.md](agents/legal-compliance.agent.md) |
| UX/Accessibility | @ux-expert, @ui-expert | [ux-expert.agent.md](agents/ux-expert.agent.md) |
| Infrastructure | @devops-engineer | [devops-engineer.agent.md](agents/devops-engineer.agent.md) |
| Process Coordination | @scrum-master | [scrum-master.agent.md](agents/scrum-master.agent.md) |
| Prioritization | @product-owner | [product-owner.agent.md](agents/product-owner.agent.md) |

---

## 🔗 Quick Links

### Latest Additions
- 🆕 [cli-developer.agent.md](agents/cli-developer.agent.md) - DevOps CLI tool
- 🆕 [software-architect.agent.md](agents/software-architect.agent.md) - System authority
- 🆕 [AGENT_QUICK_START.md](AGENT_QUICK_START.md) - Quick reference
- 🆕 [AGENT_WORKFLOW_INTEGRATION.md](AGENT_WORKFLOW_INTEGRATION.md) - Integration guide

### Critical for Every Developer
- [AGENT_QUICK_START.md](AGENT_QUICK_START.md) - Who to ask for what
- [agents/](agents/) - All agent definitions
- Your role's copilot-instructions (backend/frontend/qa/security)

### For Decision-Makers
- [AGENT_WORKFLOW_INTEGRATION.md](AGENT_WORKFLOW_INTEGRATION.md) - Governance matrix
- [software-architect.agent.md](agents/software-architect.agent.md) - Authority framework
- [product-owner.agent.md](agents/product-owner.agent.md) - Prioritization

### For Process Improvement
- [scrum-master.agent.md](agents/scrum-master.agent.md) - Process authority
- [AGENT_INTEGRATION_SUMMARY.md](AGENT_INTEGRATION_SUMMARY.md) - What changed & why

---

## 📈 Document Statistics

| Type | Count | Location |
|------|-------|----------|
| Agent Definitions | 25+ | `.github/agents/*.agent.md` |
| Integration Guides | 3 | `.github/AGENT_*.md` |
| Role-Specific Instructions | 6 | `.github/copilot-instructions-*.md` |
| Main Reference | 1 | `.github/copilot-instructions.md` |
| **Total Documentation** | **35+** | `.github/` |

---

## ✅ Navigation Checklist

- [ ] Read [AGENT_QUICK_START.md](AGENT_QUICK_START.md) (15 min)
- [ ] Skim [AGENT_WORKFLOW_INTEGRATION.md](AGENT_WORKFLOW_INTEGRATION.md) (25 min)
- [ ] Read your role's agent definition in [agents/](agents/)
- [ ] Bookmark this index for future reference
- [ ] Share with team: "Agents are ready for use"

---

## 🚀 Ready to Go!

You now have:
- ✅ 25+ specialized AI agents
- ✅ Clear hierarchy and escalation paths
- ✅ Integration guide with examples
- ✅ Quick start for new team members
- ✅ Complete documentation

**Next Step**: Ask your agent a question using `@agent-name` in Copilot Chat!

---

**Questions?** Check the appropriate document above or ask @scrum-master for process clarification.

