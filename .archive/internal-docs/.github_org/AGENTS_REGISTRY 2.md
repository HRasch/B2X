# B2Connect Agent Registry

**Status**: Complete (28 agents created)  
**Last Updated**: 28. Dezember 2025  
**Purpose**: Centralized directory of all AI coding agents

---

## Core Development (3 agents)

### 1. Backend Developer
**File**: `agents/backend-developer.agent.md`  
**Focus**: API endpoints, services, databases  
**Key Tech**: .NET 10, Wolverine, EF Core, FluentValidation  
**Primary Tasks**:
- Implement microservices (Identity, Catalog, CMS, etc.)
- Create CQRS handlers
- Build repositories and validators
- Write 80%+ coverage unit tests

**Success Criteria**: 
- Wolverine pattern used (NOT MediatR)
- All CRUD operations audit logged
- Tests passing locally before commit
- Code coverage >= 80%

---

### 2. Frontend Developer
**File**: `agents/frontend-developer.agent.md`  
**Focus**: Vue.js components, UX, accessibility  
**Key Tech**: Vue.js 3, TypeScript, Tailwind CSS, Vite  
**Primary Tasks**:
- Build responsive components
- Implement multi-language support (i18n)
- Ensure WCAG 2.1 AA compliance (P0.8!)
- Write end-to-end tests (Playwright)

**Success Criteria**:
- Lighthouse Accessibility >= 90
- Mobile-first responsive design
- Dark mode support
- Keyboard navigation working

---

### 3. QA Engineer
**File**: `agents/qa-engineer.agent.md`  
**Focus**: Testing, automation, compliance verification  
**Key Tech**: xUnit, Playwright, axe DevTools, k6  
**Primary Tasks**:
- Execute 52 compliance tests (P0.6-P0.9)
- Automate test suites
- Generate test reports
- Verify security scenarios

**Success Criteria**:
- 52/52 compliance tests passing
- >80% test coverage
- 0 critical bugs in production
- Accessibility tests automated

---

## Specialized Roles (5 agents)

### 4. Security Engineer
**File**: `agents/security-engineer.agent.md`  
**Focus**: Encryption, audit logging, key management  
**Primary P0 Components**: P0.1, P0.2, P0.3, P0.5, P0.7

| Component | Owner | Duration | Status |
|-----------|-------|----------|--------|
| P0.1: Audit Logging | Security | Week 1-2 | Design |
| P0.2: Encryption (AES-256) | Security | Week 2-3 | Design |
| P0.3: Incident Response | Security + DevOps | Week 3-4 | Design |
| P0.5: Key Management | Security + DevOps | Week 4-5 | Design |
| P0.7: AI Act Compliance | Security | Week 5-6 | Design |

---

### 5. DevOps Engineer
**File**: `agents/devops-engineer.agent.md`  
**Focus**: Infrastructure, CI/CD, scaling  
**Primary P0 Components**: P0.3, P0.4, P0.5

| Component | Owner | Duration | Status |
|-----------|-------|----------|--------|
| P0.3: Incident Response (Infra) | DevOps | Week 3-4 | Design |
| P0.4: Network Segmentation | DevOps | Week 4-5 | Design |
| P0.5: Key Management (Infra) | DevOps | Week 5-6 | Design |

---

### 6. Tech Lead
**File**: `agents/tech-lead.agent.md`  
**Focus**: Architecture, code review, standards  
**Responsibilities**:
- Review all PRs for architecture compliance
- Enforce DDD and Onion Architecture
- Resolve technical blockers
- Make go/no-go decisions

---

### 7. Product Owner
**File**: `agents/product-owner.agent.md`  
**Focus**: Requirements, roadmap, prioritization  
**Responsibilities**:
- Define user stories
- Manage backlog
- Make phase gate decisions
- Communicate with stakeholders

---

### 8. Legal/Compliance Officer
**File**: `agents/legal-compliance.agent.md`  
**Focus**: Regulatory compliance, legal review  
**Responsibilities**:
- Interpret EU regulations (NIS2, GDPR, AI Act, BITV)
- Review features for legal compliance
- Manage vendor contracts (DPAs)
- Support audits

---

## Service-Specific Roles (4 agents)

### 9. Backend Store Microservice
**File**: `agents/backend-store.agent.md`  
**Focus**: Store public APIs (Catalog, CMS, Theming, Search)  
**Key Services**: Catalog, CMS, Theming, Localization, Search  
**Port**: 8000

---

### 10. Backend Admin Microservice
**File**: `agents/backend-admin.agent.md`  
**Focus**: Admin operations (CRUD, management)  
**Key Services**: Admin API, Shared (Identity, Tenancy)  
**Port**: 8080

---

### 11. Frontend Store
**File**: `agents/frontend-store.agent.md`  
**Focus**: Public storefront UI  
**Key Features**: Browse products, search, cart, checkout  
**Port**: 5173

---

### 12. Frontend Admin
**File**: `agents/frontend-admin.agent.md`  
**Focus**: Admin dashboard  
**Key Features**: User management, product management, analytics  
**Port**: 5174

---

## QA Specializations (4 agents)

### 13. QA Frontend
**File**: `agents/qa-frontend.agent.md`  
**Focus**: Frontend testing (components, E2E)  
**Tools**: Playwright, Vue Test Utils, Lighthouse

---

### 14. QA Pentesting
**File**: `agents/qa-pentesting.agent.md`  
**Focus**: Security testing, vulnerability scanning  
**Tools**: OWASP ZAP, SonarQube, Dependency Check

---

### 15. QA Performance
**File**: `agents/qa-performance.agent.md`  
**Focus**: Load testing, stress testing, scalability  
**Tools**: k6, Grafana, Apache JMeter

---

### 23. Documentation (End-User)
**File**: `agents/documentation-enduser.agent.md`  
**Focus**: GitHub Pages, user guides, feature documentation  
**Key Tech**: Jekyll, Markdown, GitHub Pages  
**Primary Tasks**:
- Document user-visible features on GitHub Pages
- Create step-by-step guides (non-technical)
- Capture screenshots and GIFs
- Maintain FAQ and troubleshooting sections
- Optimize for search and discoverability

**Success Criteria**: 
- Documentation written for end-users (no jargon)
- Screenshots/GIFs included (HD quality)
- Mobile-friendly and responsive
- All links working
- SEO keywords optimized

---

### 24. Documentation (Developer)
**File**: `agents/documentation-developer.agent.md`  
**Focus**: API docs, release notes, breaking changes, migrations  
**Key Tech**: API documentation, version management, release automation  
**Primary Tasks**:
- Document code changes in release notes
- Identify and highlight breaking changes
- Create migration guides for major updates
- Maintain API documentation with examples
- Track version history and compatibility

**Success Criteria**:
- Release notes complete before release
- Breaking changes clearly documented
- Migration guides created (if needed)
- Code examples tested and working
- Version history accurate

---

### 27. QA-Reviewer
**File**: `agents/qa-reviewer.agent.md`  
**Focus**: Code quality, code-smells, architectural consistency  
**Key Tech**: Code analysis, architecture validation, project structure  
**Primary Tasks**:
- Detect code-smells (complexity, duplication, large methods)
- Verify functional consistency with requirements
- Validate Onion Architecture and project structure
- Conduct code reviews with structured checklist
- Provide feedback on code quality issues

**Success Criteria**:
- Code smells identified and documented
- Architectural consistency verified
- Security issues caught before merge
- Feedback constructive and actionable
- Test coverage verified >= 80%

---

### 28. Scrum-Master
**File**: `agents/scrum-master.agent.md`  
**Focus**: Team coordination, process optimization, conflict resolution  
**Key Tech**: Team dynamics, process management, retrospectives  
**Primary Tasks**:
- Facilitate sprint retrospectives
- Coordinate team activities and resolve blockers
- Optimize development processes
- Resolve disagreements using majority voting
- Update copilot-instructions.md with proven improvements
- Track metrics and KPIs

**Success Criteria**:
- Retrospectives completed every sprint
- Disagreements resolved constructively
- Process improvements documented
- Team velocity stable or improving
- All agents aligned on decisions

**Special Authority**: Can update copilot-instructions.md when majority agrees on process improvements

---

## Agent Relationships & Hierarchy

```
Leadership Layer:
├── Tech-Lead (Architecture & Standards)
├── Product-Owner (Prioritization)
└── Scrum-Master (Process & Team Coordination)

Development Layer:
├── Backend-Developer / Frontend-Developer
├── DevOps-Engineer
├── Security-Engineer
└── QA-Reviewer (Code Quality)

Specialization Layer:
├── Role-Specific (backend-store, frontend-admin, etc.)
├── Quality Assurance (qa-engineer, qa-frontend, qa-performance, qa-pentesting)
├── Experts (ai-specialist, ui-expert, ux-expert)
└── Documentation (documentation-enduser, documentation-developer - BILINGUAL)

Support Layer:
├── Stakeholders (erp, pim, crm, bi, reseller)
└── Support (support-triage)
```
**File**: `agents/support-triage.agent.md`  
**Focus**: GitHub issue automation, triage  
**Tools**: GitHub CLI, automation workflows

---

## Expert Roles (3 agents)

### 17. AI Specialist
**File**: `agents/ai-specialist.agent.md`  
**Focus**: Fraud detection, recommendations, AI Act compliance  
**Expertise**: ML models, bias testing, performance monitoring

---

### 18. UI Expert
**File**: `agents/ui-expert.agent.md`  
**Focus**: Design systems, component libraries  
**Expertise**: Tailwind CSS, design tokens, theming

---

### 19. UX Expert
**File**: `agents/ux-expert.agent.md`  
**Focus**: User experience, accessibility, interaction design  
**Expertise**: WCAG 2.1 AA, usability testing, information architecture

---

## Stakeholder Roles (5 agents)

### 20. CRM Stakeholder
**File**: `agents/stakeholder-crm.agent.md`  
**Focus**: CRM integration requirements  
**Integrations**: Salesforce, HubSpot, Pipedrive

---

### 21. ERP Stakeholder
**File**: `agents/stakeholder-erp.agent.md`  
**Focus**: ERP integration requirements  
**Integrations**: SAP, Oracle, NetSuite, Dynamics

---

### 22. PIM Stakeholder
**File**: `agents/stakeholder-pim.agent.md`  
**Focus**: PIM/Master data requirements  
**Integrations**: Nexmart Datacloud, Oxomi, custom providers

---

### 23. BI Stakeholder
**File**: `agents/stakeholder-bi.agent.md`  
**Focus**: Analytics and business intelligence  
**Integrations**: PowerBI, Tableau, Google Analytics

---

### 24. Reseller Program
**File**: `agents/stakeholder-reseller.agent.md`  
**Focus**: Reseller/partner program requirements  
**Features**: White-label config, licensing, partner portal

---

## 🎯 Using Agents

### Quick Start
```bash
# For any task, include the agent context:
# "You are the [Role] agent. Context: [agent file]. Task: [description]"

# Example:
# You are the Backend Developer agent. Context: .github/agents/backend-developer.agent.md
# Task: Implement audit logging for all CRUD operations in Product service
```

### Common Workflows

**Implementing a Feature**:
1. Backend Developer reads requirements
2. Creates service, repository, validators
3. QA Engineer writes tests
4. Tech Lead reviews for architecture compliance
5. Frontend Developer implements UI
6. QA Frontend tests E2E
7. Product Owner approves and merges

**Security Fix**:
1. Security Engineer identifies vulnerability
2. Backend Developer implements fix
3. QA Pentesting verifies fix
4. DevOps Engineer deploys hotfix
5. Legal/Compliance Officer documents incident

**Compliance Implementation**:
1. Legal/Compliance Officer interprets regulation
2. Backend Developer implements feature
3. QA Engineer executes compliance tests
4. Security Engineer verifies encryption/audit
5. Product Owner manages go/no-go gate

---

## 📊 Agent Matrix

| Agent | Primary Role | Secondary Roles | Time to Mastery |
|-------|-------------|-----------------|-----------------|
| Backend Developer | Development | Security, Testing | 2 weeks |
| Frontend Developer | Development | Accessibility, Testing | 2 weeks |
| QA Engineer | Testing | Compliance, Security | 2 weeks |
| Security Engineer | Compliance | Testing, Architecture | 3 weeks |
| DevOps Engineer | Infrastructure | Compliance, Monitoring | 3 weeks |
| Tech Lead | Oversight | All | 4 weeks |
| Product Owner | Management | Compliance, Requirements | 2 weeks |
| Legal/Compliance | Compliance | Risk Management | 3 weeks |

---

## ⚡ Performance Metrics

**Agents are optimized for**:
- Response time: < 5 seconds per query
- Context retention: Across 10+ message turns
- Code quality: 80%+ test coverage, 0 security issues
- Compliance: 100% requirement adherence

---

## 🔄 Agent Updates

### When to Update an Agent
- New framework version released
- New compliance requirement added
- New security vulnerability discovered
- Process improvement identified
- Team feedback suggests change

### How to Update
1. Edit `agents/[role].agent.md`
2. Run agent-focused tests
3. Get Tech Lead approval
4. Commit with comment: `chore(agents): update [role] context`
5. Announce to team

---

## 📞 Agent Support

**Questions about an agent?**
1. Check `.github/agents/[role].agent.md`
2. Check `.github/AGENTS_QUICK_REFERENCE.md`
3. Ask Tech Lead or corresponding specialist

**Need a new agent?**
1. Tech Lead assesses need
2. New agent template created
3. Specialist trains agent
4. Add to registry and announce

---

## 📚 Related Documentation

- [COPILOT_SETUP.md](./COPILOT_SETUP.md) - Setup guide
- [copilot-instructions.md](./copilot-instructions.md) - Main guidelines
- `.github/agents/` - All agent files
- [AGENTS_QUICK_REFERENCE.md](./AGENTS_QUICK_REFERENCE.md) - Quick lookup

---

**Owner**: Tech Lead  
**Last Reviewed**: 28. Dezember 2025  
**Next Review**: 15. Januar 2026
