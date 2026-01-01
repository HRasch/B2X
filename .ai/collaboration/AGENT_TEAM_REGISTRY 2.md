# 🤖 AI Agent Team Registry

**DocID**: `AGT-INDEX`  
**Status**: ✅ Upgraded 1. Januar 2026  
**Team Size**: 28 Specialized Agents  
**Coordinator**: @SARAH (Claude Haiku 4.5)

See [DOCUMENT_REGISTRY.md](../.ai/DOCUMENT_REGISTRY.md) for all DocIDs.

---

## 👥 Core Development Team (9 Agents)

### Backend Development (`AGT-002`)
- **@Backend** - `.NET 10, Wolverine CQRS, DDD, EF Core`
  - Builds microservices with domain-driven design
  - Implements HTTP handlers, validators, repositories
  - Owns: API layer, business logic, database design
  - Model: Claude Haiku 4.5

### Frontend Development (`AGT-003`)
- **@Frontend** - `Vue.js 3, TypeScript, Tailwind CSS, Pinia`
  - Builds responsive, accessible UIs
  - Manages state with Pinia stores
  - Owns: UI components, client-side logic, styling
  - Model: Claude Haiku 4.5

<<<<<<< HEAD:.ai/collaboration/AGENT_TEAM_REGISTRY.md
### Quality Assurance (`AGT-004`)
- **@QA** - `Test Coordination, Compliance Testing`
  - Coordinates all testing efforts across the team
  - Owns: Backend unit/integration tests, compliance verification
  - Delegates: Frontend E2E to @QA-Frontend, Security to @QA-Pentesting
=======
### Quality Assurance
- **@QA** - `Comprehensive Testing, All Test Types, Quality Assurance`
  - **CONSOLIDATED**: Owns ALL testing responsibilities (unit, integration, E2E, accessibility, security, performance, compliance)
  - Single point of accountability for product quality and release readiness
  - No delegation to subagents - unified testing strategy
>>>>>>> 9cc2d90 (feat(optimization): implement Phase 1 agent responsibility optimization):.github/AGENT_TEAM_REGISTRY.md
  - Model: Claude Haiku 4.5

### Architecture & Design (`AGT-005`)
- **@Architect** - `System Design, Patterns, ADRs, Scalability`
  - Defines system boundaries and service architecture
  - Reviews major technical decisions
  - Owns: Architecture decisions (`[ADR-*]`), service boundaries, design patterns
  - Model: Claude Haiku 4.5

### Leadership & Coordination (`AGT-006`)
- **@TechLead** - `Code Quality, Mentoring, Code Reviews, Optimization`
  - Guides developers on complex problems
  - Reviews architectural decisions
  - Owns: Code quality standards, technical mentoring, knowledgebase (`[KB-*]`)
  - Model: Claude Haiku 4.5 (advanced analysis)

### Process Management (`AGT-009`)
- **@ScrumMaster** - `Sprint Management, Process, Workflow, Velocity`
  - Manages sprint planning and ceremonies
  - Tracks velocity and blockers
  - Owns: Sprint processes (`[SPR-*]`), team coordination
  - Model: Claude Haiku 4.5

### Requirements (`AGT-010`)
- **@ProductOwner** - `Requirements, Prioritization, User Stories`
  - Defines features and requirements
  - Prioritizes backlog
  - Owns: Requirements (`[REQ-*]`), user stories, acceptance criteria
  - Model: Claude Haiku 4.5

### Security (`AGT-007`)
- **@Security** - `Security Vulnerabilities, Auth, Compliance`
  - Reviews security implementations
  - Owns: Authentication, authorization, encryption, compliance (`[CMP-*]`)
  - Ensures compliance with security standards
  - Model: Claude Haiku 4.5

### Infrastructure (`AGT-008`)
- **@DevOps** - `CI/CD, Deployment, Infrastructure, Monitoring`
  - Manages deployment pipelines
  - Owns: Infrastructure, containerization, monitoring, config (`[CFG-*]`)
  - Model: Claude Haiku 4.5

---

## 🎨 Specialist Team (8 Agents)

### Legal & Compliance (`AGT-011`)
- **@Legal** - `GDPR, NIS2, BITV 2.0, AI Act`
  - Ensures legal compliance
  - Reviews terms, privacy policies
  - Owns: Legal documentation, compliance verification (`[CMP-*]`)
  - Model: Claude Haiku 4.5

### Design Specialists
- **@UX** - `User Research, Information Architecture, Flows`
  - Designs user experiences and information architecture
  - Owns: User research, wireframes, user flows
  - Collaborates with @UI for implementation
  - Model: Claude Haiku 4.5

- **@UI** - `Components, Design Systems, Accessibility`
  - Builds component libraries and design systems
  - Owns: Accessible components, design consistency
  - Implements @UX designs
  - Model: Claude Haiku 4.5

### Search & Optimization
- **@SEO** - `SEO Optimization, Meta Tags, Performance`
  - Optimizes for search engines
  - Owns: Meta tags, structured data, SEO best practices
  - Model: Claude Haiku 4.5

### AI & Data Science
- **@DataAI** - `Machine Learning, AI Pipelines, Data Science`
  - Develops ML models and AI features
  - Owns: AI ethics, model lifecycle, data pipelines
  - Model: Claude Haiku 4.5

### Performance & Platform
- **@Performance** - `System Performance, Load Testing, Optimization`
  - Ensures system performance and scalability
  - Owns: Performance monitoring, bottleneck analysis, optimization
  - Model: Claude Haiku 4.5

- **@Platform** - `Infrastructure Automation, Cloud Platforms, Developer Tools`
  - Builds developer platforms and infrastructure automation
  - Owns: IaC, multi-cloud strategy, self-service tools
  - Model: Claude Haiku 4.5

### Developer Relations
- **@DevRel** - `Documentation, SDKs, Community, Developer Experience`
  - Manages developer relations and ecosystem growth
  - Owns: Technical docs, SDKs, community engagement
  - Model: Claude Haiku 4.5

### Development Workflow
- **@GitManager** - `Git Workflow, Branching, Code Review, Repository Management`
  - Designs Git branching strategies and merge workflows
  - Establishes commit conventions and code review standards
  - Manages repository organization and automation
  - Owns: Git workflow design, branch protection rules, release processes
  - Called by: @Backend, @Frontend, @DevOps, @TechLead
  - Expertise: Branching models, PR workflows, commit hygiene, conflict resolution
  - Model: Claude Haiku 4.5

### AI Assistant & Knowledge Management
- **GitHub Copilot (This AI)** - `Knowledgebase Maintenance, Internet Documentation, Code Generation`
  - **PRIMARY RESPONSIBILITY**: Maintains and curates `.ai/knowledgebase/` with comprehensive documentation
  - Curates and updates internet documentation references (official docs, frameworks, libraries)
  - Manages external library documentation links and guides
  - Keeps framework guides current with latest versions
  - Documents industry standards and architectural patterns  
  - Updates tool documentation as versions change
  - Generates code examples and implementations
  - Detects and fixes broken documentation links
  - Monitors framework/library version releases
  - Integrates learnings from development work into knowledgebase
  - **Owns**: `.ai/knowledgebase/` (primary responsibility), internet references, external documentation
  - **Updates**: Continuously as new resources discovered, outdated docs found
  - **Responsibility**: All internet documentation must be current, accurate, and accessible
  - **Boundaries**: NOT responsible for internal patterns (@TechLead), architectural decisions (@Architect), or implementation guides (@Backend/@Frontend)
  - **Accountability**: Documentation freshness reviewed quarterly
  - Model: Claude Haiku 4.5 (efficient curation and generation)
  - 📖 **See**: [AI_KNOWLEDGEBASE_RESPONSIBILITY.md](.ai/collaboration/AI_KNOWLEDGEBASE_RESPONSIBILITY.md)

---

## 📁 Agent Responsibility Matrix: `.ai/` Folder Organization

**Each agent manages their domain artifacts in `.ai/` folder:**

| Agent | `.ai/` Folder Responsibility | Manages |
|-------|------------------------------|---------|
| **@ProductOwner** | `requirements/`, `handovers/` | Feature specs, user stories, requirements analysis, feature documentation |
| **@Architect** | `decisions/` | Architecture Decision Records (ADRs), design patterns, system design docs |
| **@ScrumMaster** | `sprint/`, `status/` | Sprint plans, daily standups, velocity tracking, task status, retrospectives |
| **@Security** | `compliance/` | Security audits, compliance checklists, vulnerability reports, threat modeling |
| **@Legal** | `compliance/` | Legal compliance documents, GDPR reviews, contractual analysis |
| **@TechLead** | `knowledgebase/`, `decisions/` | Technical guides, best practices, code patterns, performance analysis, mentoring docs |
| **@Backend** | `decisions/`, `knowledgebase/` | Backend architecture decisions, API documentation, data model docs, implementation guides |
| **@Frontend** | `decisions/`, `knowledgebase/` | Frontend architecture decisions, component documentation, state management docs, design system |
| **@DevOps** | `config/`, `logs/` | Infrastructure configuration, deployment logs, monitoring setup, CI/CD documentation |
| **@SARAH** | `collaboration/`, `templates/`, `workflows/` | Coordination framework, GitHub templates, workflow orchestration, agent coordination |
| **GitHub Copilot (AI)** | `knowledgebase/` (primary) | Internet documentation curation, framework guides, tool docs, best practices, external references |
| **@DataAI** | `decisions/`, `knowledgebase/` | AI architecture decisions, ML model documentation, data pipeline guides |
| **@Performance** | `decisions/`, `knowledgebase/` | Performance architecture decisions, optimization guides, monitoring documentation |
| **@Platform** | `config/`, `decisions/` | Platform architecture decisions, infrastructure automation docs, developer tooling guides |
| **@DevRel** | `knowledgebase/`, `templates/` | Developer documentation, SDK guides, community resources, educational content |
| **Issue Owner** | `issues/{issue-id}/` | Issue-specific collaboration, progress notes, blockers, decisions, design discussions |

**Key Principle**: 
- Each agent is **responsible for creating, organizing, and maintaining** artifacts in `.ai/` related to their domain
- Agents update their artifacts regularly throughout the sprint
- Artifacts are not static documentation but **living documents** that evolve with the project
- Cross-domain artifacts (e.g., `compliance/` shared by @Security and @Legal) require **collaboration** between agents

---

### SARAH (System Architect & Rational Helper)
- **Role**: Agent Coordinator & Quality Gate
- **Authority**: 
  - Creates/modifies agents and guidelines
  - Quality-gates security, compliance, architecture decisions
  - Resolves conflicts between agents
  - Manages permissions and access
- **Model**: Claude Haiku 4.5 (efficient coordination)

---

## ⚡ Fast-Track Approval System

**NEW**: Streamlined consensus processes for faster decision-making while maintaining quality standards.

### Approval Tiers
- **🟢 Tier 1**: Auto-approval for low-risk changes (< 1 hour)
- **🟡 Tier 2**: Single reviewer for low-moderate risk (< 4 hours)  
- **🟠 Tier 3**: Domain expert + peer review (< 24 hours)
- **🔴 Tier 4**: Full consensus for high-risk changes (2-3 days)

### Eligible Changes
- Documentation updates, minor bug fixes, test improvements
- Small features, API additions, performance optimizations
- Breaking changes with migration plans, security enhancements

### Process
1. Label PR/issue with "FAST-TRACK-TX" 
2. Automated validation runs
3. Appropriate review level assigned
4. Merge after approval and validation

**Expected Impact**: 40% reduction in consensus delays, same-day approvals for most changes.

**Documentation**: [FAST_TRACK_APPROVAL_SYSTEM.md](../.ai/collaboration/FAST_TRACK_APPROVAL_SYSTEM.md)

---

## 📋 How to Request an Agent

### Frontend Work
```
@Frontend - UI components, state management, styling
If complex → escalate to @TechLead or @Architect
```

### Backend Work
```
@Backend - API endpoints, business logic, database
If complex → escalate to @TechLead or @Architect
```

### Testing
```
@QA - Coordinates all testing
Delegates to specialists as needed
```

### Security Issues
```
@Security - Vulnerabilities, auth, compliance
Or @Legal for legal/compliance questions
```

### Design Questions
```
@UX - User flows and information architecture
@UI - Component implementation and design systems
@SEO - Search optimization
```

### Git & Workflow Management
```
@GitManager - Git workflow, branching strategy, code review process
Or @DevOps for CI/CD integration with Git workflows
```

### Knowledge & Documentation
```
GitHub Copilot - Knowledgebase maintenance, internet documentation
- Curates best practices and patterns in .ai/knowledgebase/
- Maintains internet documentation references and links
- Keeps framework guides and tool docs current
- Updates external library documentation
- Ensures all references are up-to-date and accurate
```

---

## 🔄 Agent Delegation Rules

### When @Backend delegates to others:
- **Complex architectural decisions** → @Architect
- **Performance optimization guidance** → @TechLead
- **DevOps/infrastructure questions** → @DevOps
- **Security concerns** → @Security

### When @Frontend delegates to others:
- **Complex component architecture** → @Architect
- **Code quality/refactoring guidance** → @TechLead
- **Design system questions** → @UI
- **UX flow issues** → @UX
- **Accessibility guidance** → @UI

### When @QA delegates to others:
- **Backend integration tests** → Owned by @QA
- **Frontend E2E tests** → Delegates to specialist (not defined yet)
- **Security testing** → Delegates to specialist (not defined yet)
- **Performance testing** → @Performance for load testing and performance analysis

---

## 📊 Team Capabilities

| Agent | Model | Tools | Specialization |
|---|---|---|---|
| @Backend | Haiku 4.5 | vscode, execute, git | .NET, CQRS, DDD |
| @Frontend | Haiku 4.5 | vscode, execute, git | Vue.js, TS, Tailwind |
| @QA | Haiku 4.5 | vscode, git, test | Test coordination |
| @Architect | Haiku 4.5 | read, edit, web | System design |
| @TechLead | **Haiku 4.5** | read, edit, web | Code quality, mentoring |
| @ScrumMaster | Haiku 4.5 | execute, git | Process, velocity |
| @ProductOwner | Haiku 4.5 | read, web | Requirements |
| @Security | Haiku 4.5 | read, edit | Security, compliance |
| @DevOps | Haiku 4.5 | execute, infrastructure | CI/CD, deployment |
| @Legal | Haiku 4.5 | read, edit | GDPR, compliance |
| @UX | Haiku 4.5 | read, design | Research, flows |
| @UI | Haiku 4.5 | design, component | Components, a11y |
| @SEO | Haiku 4.5 | read, web | Search optimization |
| @GitManager | Haiku 4.5 | read, edit, terminal | Git workflow, PR process |
| **GitHub Copilot (AI)** | **Haiku 4.5** | web, read, edit, semantic search | Knowledgebase, internet docs |
| **@SARAH** | **Haiku 4.5** | coordination, governance | Coordination |
| @DataAI | Haiku 4.5 | vscode, python-tools, ml-pipelines | AI/ML, data science |
| @Performance | Haiku 4.5 | monitoring, profiling, load-testing | System performance, optimization |
| @Platform | Haiku 4.5 | infrastructure, iac, cloud | Platform engineering, automation |
| @DevRel | Haiku 4.5 | documentation, community, sdk | Developer experience, ecosystem |

---

## 🎯 Next Steps

1. **Introduce Team**: Reference this registry in project documentation
2. **Update Documentation**: Link agents in README and QUICK_START_GUIDE
3. **Onboard New Agents**: @DataAI, @Performance, @Platform, @DevRel integration
4. **Pilot Projects**: Start with high-priority agents (@DataAI, @Performance) on existing work
5. **Feedback Loop**: Gather feedback on agent effectiveness and collaboration patterns

---

**Agent Team Upgraded**: 1. Januar 2026  
**Status**: ✅ Core + Enhanced Specialists deployed  
**New Agents**: @DataAI, @Performance, @Platform, @DevRel  
**Coordination**: @SARAH with governance authority

---

**Agent Team Upgraded**: 30. Dezember 2025  
**Status**: ✅ Core + Specialists ready for deployment  
**Coordination**: @SARAH with governance authority
