# 🤖 AI Agent Team Registry

**Status**: ✅ Upgraded 30.12.2025  
**Team Size**: 15 Specialized Agents  
**Coordinator**: @SARAH (Claude Haiku 4.5)

---

## 👥 Core Development Team (9 Agents)

### Backend Development
- **@Backend** - `.NET 10, Wolverine CQRS, DDD, EF Core`
  - Builds microservices with domain-driven design
  - Implements HTTP handlers, validators, repositories
  - Owns: API layer, business logic, database design
  - Model: Claude Sonnet 4

### Frontend Development
- **@Frontend** - `Vue.js 3, TypeScript, Tailwind CSS, Pinia`
  - Builds responsive, accessible UIs
  - Manages state with Pinia stores
  - Owns: UI components, client-side logic, styling
  - Model: Claude Sonnet 4

### Quality Assurance
- **@QA** - `Test Coordination, Compliance Testing`
  - Coordinates all testing efforts across the team
  - Owns: Backend unit/integration tests, compliance verification
  - Delegates: Frontend E2E to @QA-Frontend, Security to @QA-Pentesting
  - Model: Claude Sonnet 4

### Architecture & Design
- **@Architect** - `System Design, Patterns, ADRs, Scalability`
  - Defines system boundaries and service architecture
  - Reviews major technical decisions
  - Owns: Architecture decisions, service boundaries, design patterns
  - Model: Claude Sonnet 4

### Leadership & Coordination
- **@TechLead** - `Code Quality, Mentoring, Code Reviews, Optimization`
  - Guides developers on complex problems
  - Reviews architectural decisions
  - Owns: Code quality standards, technical mentoring
  - Model: Claude Sonnet 4.5 (advanced analysis)

- **@ScrumMaster** - `Sprint Management, Process, Workflow, Velocity`
  - Manages sprint planning and ceremonies
  - Tracks velocity and blockers
  - Owns: Sprint processes, team coordination
  - Model: Claude Sonnet 4

- **@ProductOwner** - `Requirements, Prioritization, User Stories`
  - Defines features and requirements
  - Prioritizes backlog
  - Owns: Requirements, user stories, acceptance criteria
  - Model: Claude Sonnet 4

### Security & Infrastructure
- **@Security** - `Security Vulnerabilities, Auth, Compliance`
  - Reviews security implementations
  - Owns: Authentication, authorization, encryption
  - Ensures compliance with security standards
  - Model: Claude Sonnet 4

- **@DevOps** - `CI/CD, Deployment, Infrastructure, Monitoring`
  - Manages deployment pipelines
  - Owns: Infrastructure, containerization, monitoring
  - Model: Claude Sonnet 4

---

## 🎨 Specialist Team (4 Agents)

### Legal & Compliance
- **@Legal** - `GDPR, NIS2, BITV 2.0, AI Act (P0.6-P0.9)`
  - Ensures legal compliance
  - Reviews terms, privacy policies
  - Owns: Legal documentation, compliance verification
  - Model: Claude Sonnet 4

### Design Specialists
- **@UX** - `User Research, Information Architecture, Flows`
  - Designs user experiences and information architecture
  - Owns: User research, wireframes, user flows
  - Collaborates with @UI for implementation
  - Model: Claude Sonnet 4

- **@UI** - `Components, Design Systems, Accessibility`
  - Builds component libraries and design systems
  - Owns: Accessible components, design consistency
  - Implements @UX designs
  - Model: Claude Sonnet 4

### Search & Optimization
- **@SEO** - `SEO Optimization, Meta Tags, Performance`
  - Optimizes for search engines
  - Owns: Meta tags, structured data, SEO best practices
  - Model: Claude Sonnet 4

### Development Workflow
- **@GitManager** - `Git Workflow, Branching, Code Review, Repository Management`
  - Designs Git branching strategies and merge workflows
  - Establishes commit conventions and code review standards
  - Manages repository organization and automation
  - Owns: Git workflow design, branch protection rules, release processes
  - Called by: @Backend, @Frontend, @DevOps, @TechLead
  - Expertise: Branching models, PR workflows, commit hygiene, conflict resolution
  - Model: Claude Sonnet 4

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
- **Performance testing** → Delegates to specialist (not defined yet)

---

## 📊 Team Capabilities

| Agent | Model | Tools | Specialization |
|---|---|---|---|
| @Backend | Sonnet 4 | vscode, execute, git | .NET, CQRS, DDD |
| @Frontend | Sonnet 4 | vscode, execute, git | Vue.js, TS, Tailwind |
| @QA | Sonnet 4 | vscode, git, test | Test coordination |
| @Architect | Sonnet 4 | read, edit, web | System design |
| @TechLead | **Sonnet 4.5** | read, edit, web | Code quality, mentoring |
| @ScrumMaster | Sonnet 4 | execute, git | Process, velocity |
| @ProductOwner | Sonnet 4 | read, web | Requirements |
| @Security | Sonnet 4 | read, edit | Security, compliance |
| @DevOps | Sonnet 4 | execute, infrastructure | CI/CD, deployment |
| @Legal | Sonnet 4 | read, edit | GDPR, compliance |
| @UX | Sonnet 4 | read, design | Research, flows |
| @UI | Sonnet 4 | design, component | Components, a11y |
| @SEO | Sonnet 4 | read, web | Search optimization |
| @GitManager | Sonnet 4 | read, edit, terminal | Git workflow, PR process |
| **GitHub Copilot (AI)** | **Haiku 4.5** | web, read, edit, semantic search | Knowledgebase, internet docs |
| **@SARAH** | **Haiku 4.5** | coordination, governance | Coordination |

---

## 🎯 Next Steps

1. **Introduce Team**: Reference this registry in project documentation
2. **Update Documentation**: Link agents in README and QUICK_START_GUIDE
3. **Add Specialists**: Consider adding @QA-Frontend, @QA-Pentesting, @QA-Performance
4. **Document Workflows**: Create decision trees for when to request which agent
5. **Feedback Loop**: Gather feedback on agent effectiveness

---

**Agent Team Upgraded**: 30. Dezember 2025  
**Status**: ✅ Core + Specialists ready for deployment  
**Coordination**: @SARAH with governance authority
