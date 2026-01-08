---
docid: GL-073
title: GL 010 AGENT ARTIFACT ORGANIZATION
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

# GL-010: Agent & Artifact Organization

**DocID**: `GL-010`  
**Status**: Active | **Owner**: @SARAH  
**Created**: 2026-01-05

## Agent Team (16 Specialized)

See [AGENT_TEAM_REGISTRY.md](../collaboration/AGENT_TEAM_REGISTRY.md) for complete details.

| Agent | DocID | Spezialisierung | Aufgabe |
|---|---|---|---|
| `@SARAH` | `AGT-001` | Coordination | Koordination, Quality-Gate, Guidelines, Permissions |
| `@Backend` | `AGT-002` | .NET/Wolverine | APIs, Microservices, Database, Business Logic |
| `@Frontend` | `AGT-003` | Vue.js 3 | UI Components, State, Accessibility, Styling |
| `@QA` | `AGT-004` | Test Coordination | Unit/Integration Tests, Compliance, Test Delegation |
| `@Architect` | `AGT-005` | System Design | Service Architecture, Patterns, ADRs, Design Decisions |
| `@TechLead` | `AGT-006` | Code Quality | Mentoring, Code Reviews, Complex Problems |
| `@Security` | `AGT-007` | Security/Auth | Vulnerabilities, Encryption, Compliance Verification |
| `@DevOps` | `AGT-008` | Infrastructure | CI/CD, Deployment, Monitoring, Kubernetes |
| `@ScrumMaster` | `AGT-009` | Process | Iteration Management, Velocity, Blockers |
| `@ProductOwner` | `AGT-010` | Requirements | User Stories, Prioritization, Acceptance Criteria |
| `@Legal` | `AGT-011` | Compliance | GDPR, NIS2, BITV 2.0, AI Act |
| `@UX` | `AGT-012` | Design | User Research, Information Architecture, Flows |
| `@UI` | `AGT-013` | Components | Design Systems, Accessibility, Visual Consistency |
| `@SEO` | `AGT-014` | Search | Meta Tags, Structured Data, Search Optimization |
| `@GitManager` | `AGT-015` | Git Workflow | Branching, Code Review, Repository Management |
| `@DocMaintainer` | `AGT-016` | Documentation | Maintain doc quality, enforce DocID rules, link checks |
| `@Enventa` | `AGT-017` | ERP Integration | enventa Trade ERP, Provider Architecture, Actor Pattern |
| `@CopilotExpert` | `AGT-018` | Copilot Config | **EXCLUSIVE:** Agent definitions, prompts, instructions |

### Specialist Agents (Coming Soon)
- @QA-Frontend (E2E, UI Testing, Playwright)
- @QA-Pentesting (Security Testing, OWASP)
- @QA-Performance (Load Testing, Scalability)

---

## File Structure

```
.github/
├── copilot-instructions.md     ← Master Instructions (INS-000)
├── agents/*.agent.md           ← Agent Definitionen (AGT-*)
├── instructions/*.instructions.md  ← Path-specific Instructions (INS-*)
└── prompts/*.prompt.md         ← Wiederverwendbare Prompts (PRM-*)

.ai/
├── DOCUMENT_REGISTRY.md        ← DocID Registry (Master Reference)
├── collaboration/              ← Coordination Framework
├── config/                     ← Konfigurationsdateien (CFG-*)
├── decisions/                  ← Architecture Decision Records (ADR-*)
├── guidelines/                 ← Coding & Process Guidelines (GL-*)
├── handovers/                  ← Feature Handover Documents
├── issues/{issue-id}/          ← Issue-spezifische Collaboration
├── knowledgebase/              ← Wissensdatenbank (KB-*)
├── logs/                       ← Agent Logs (archival-only, ignore in prompts)
├── permissions/                ← Agent Permissions
├── requirements/               ← Anforderungsanalysen (REQ-*)
├── sprint/                     ← Iteration Planning & Tracking (SPR-*)
├── status/                     ← Task Completion Tracking
├── templates/                  ← GitHub Issue & PR Templates (TPL-*)
└── workflows/                  ← Development Workflows (WF-*)
```

---

## Artifact Storage Locations

| Location | Content | Managed by |
|----------|---------|------------|
| `.github/prompts/` | Workflow definitions | @CopilotExpert |
| `.github/instructions/` | Coding guidelines | @CopilotExpert |
| `.github/agents/` | Agent definitions | @CopilotExpert |
| `.ai/requirements/` | Feature specs | @ProductOwner |
| `.ai/decisions/` | ADRs | @Architect |
| `.ai/logs/` | Execution logs | Responsible agent |
| `.ai/sprint/` | Sprint planning | @ScrumMaster |
| `.ai/issues/` | Issue collaboration | Issue owner |
| `.ai/handovers/` | Feature docs | @ProductOwner |
| `.ai/compliance/` | Compliance tracking | @Security, @Legal |
| `.ai/collaboration/` | Coordination framework | @SARAH |
| `.ai/config/` | Configuration | @DevOps |
| `.ai/guidelines/` | Coding standards | @TechLead |
| `.ai/knowledgebase/` | Documentation | GitHub Copilot |
| `.ai/status/` | Task tracking | @ScrumMaster |
| `.ai/templates/` | GitHub templates | @SARAH |
| `.ai/workflows/` | Dev workflows | @SARAH |
| `.ai/archive/` | Historical documents | @DocMaintainer |
| `.ai/cleanup-logs/` | Cleanup audit logs | @DocMaintainer |

### Archive & Cleanup Policy

**Root-Level File Policy**:
- ✅ **Keep at root**: README.md, QUICK_START_GUIDE.md, CONTRIBUTING.md, GOVERNANCE.md, SECURITY.md, LICENSE, `*.slnx`, `docker-compose.yml`, `Directory.Packages.props`
- ❌ **Never at root**: Analysis documents, reports, logs, temporary files, test data, duplicates

**Document Lifecycle**:
1. **Active**: In appropriate `.ai/` subdirectory (requirements, decisions, logs, etc.)
2. **Superseded**: Move to `.ai/archive/` when no longer maintained
3. **Old**: Auto-archive documents > 90 days without updates
4. **Expired**: Delete from archive after 6 months (coordinate with @SARAH)

**Duplicate Prevention**:
- Use [DOCUMENT_REGISTRY.md](../DOCUMENT_REGISTRY.md) DocID system for all documents
- Search archive before creating new docs
- Name files consistently (e.g., `REQ-###-*.md`, not `REQ-007-backend-analysis 2.md`)

See [BS-PROJECT-CLEANLINESS-STRATEGY.md](../brainstorm/BS-PROJECT-CLEANLINESS-STRATEGY.md) for detailed cleanup governance.

### Folder Rules

**`.github/` folder** — Framework & process definitions
- Owner: @SARAH (governance) + @CopilotExpert (config)
- Purpose: Reusable workflow triggers, agent definitions
- Scope: Global process definitions, ALL teams must follow

**`.ai/` folder** — Project artifacts & domain-specific documentation
- Owner: Responsible agent (see table)
- Purpose: Feature specs, decisions, logs, sprint tracking
- Scope: Specific to current project work

---

## Agent Responsibility for `.ai/` Folder

| Agent | `.ai/` Responsibility | Artifacts |
|-------|----------------------|-----------|
| @ProductOwner | `requirements/`, `handovers/` | Feature specs, user stories, handovers |
| @Architect | `decisions/` | ADRs, design patterns, system design docs |
| @ScrumMaster | `sprint/`, `status/` | Sprint plans, velocity, retrospectives |
| @Security | `compliance/` | Security audits, vulnerability reports |
| @Legal | `compliance/` | GDPR reviews, contractual analysis |
| @TechLead | `guidelines/`, `knowledgebase/` | Coding styles, best practices |
| @Backend | `decisions/`, `knowledgebase/` | API docs, data model docs |
| @Frontend | `decisions/`, `knowledgebase/` | Component docs, state management |
| @DevOps | `config/`, `logs/` | Infrastructure config, deployment logs |
| @SARAH | `collaboration/`, `templates/`, `workflows/` | Coordination, templates |
| @CopilotExpert | `.github/` (exclusive) | Agents, prompts, instructions |
| @DocMaintainer | `.ai/` (docs only) | DocID enforcement, link checks |
| Issue Owner | `issues/{issue-id}/` | Progress notes, blockers, decisions |

### DocMaintainer Authority
@DocMaintainer is empowered to update, rename, archive, and fix documentation files under `.ai/`. For policy-level decisions, open an issue and notify @SARAH.

**Note:** @DocMaintainer may NOT modify `.github/agents/`, `.github/prompts/`, `.github/instructions/` (owned by @CopilotExpert).

**Key Principle**: Agents own the organization and updates of `.ai/` artifacts related to their domain expertise.

---

**Maintained by**: @SARAH
