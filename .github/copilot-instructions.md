# GitHub Copilot Instructions

Diese Datei definiert projekt-weite Anweisungen für alle GitHub Copilot Agents.

## Project Context
- **Description**: AI-DEV - Multi-Agent Development Framework
- **Architecture**: Agent-basierte Entwicklung mit SARAH als Coordinator
- **Tech Stack**: [Projekt-spezifisch zu definieren]

## Agent System

### Verfügbare Agents (15 Specialized)

**See [AGENT_TEAM_REGISTRY.md](AGENT_TEAM_REGISTRY.md) for complete team overview**

| Agent | Spezialisierung | Aufgabe |
|---|---|---|
| `@SARAH` | Coordination | Koordination, Quality-Gate, Guidelines, Permissions |
| `@Backend` | .NET/Wolverine | APIs, Microservices, Database, Business Logic |
| `@Frontend` | Vue.js 3 | UI Components, State, Accessibility, Styling |
| `@QA` | Test Coordination | Unit/Integration Tests, Compliance, Test Delegation |
| `@Architect` | System Design | Service Architecture, Patterns, ADRs, Design Decisions |
| `@TechLead` | Code Quality | Mentoring, Code Reviews, Complex Problems (Sonnet 4.5) |
| `@Security` | Security/Auth | Vulnerabilities, Encryption, Compliance Verification |
| `@DevOps` | Infrastructure | CI/CD, Deployment, Monitoring, Kubernetes |
| `@ScrumMaster` | Process | Sprint Management, Velocity, Blockers |
| `@ProductOwner` | Requirements | User Stories, Prioritization, Acceptance Criteria |
| `@Legal` | Compliance | GDPR, NIS2, BITV 2.0, AI Act (P0.6-P0.9) |
| `@UX` | Design | User Research, Information Architecture, Flows |
| `@UI` | Components | Design Systems, Accessibility, Visual Consistency |
| `@SEO` | Search | Meta Tags, Structured Data, Search Optimization |

**Specialist Agents (Coming Soon)**:
- @QA-Frontend (E2E, UI Testing, Playwright)
- @QA-Pentesting (Security Testing, OWASP)
- @QA-Performance (Load Testing, Scalability)

### Dateien-Struktur
```
.github/
├── copilot-instructions.md     ← Du bist hier (global)
├── agents/*.agent.md           ← Agent Definitionen
├── instructions/*.instructions.md  ← Path-specific Instructions
└── prompts/*.prompt.md         ← Wiederverwendbare Prompts

.ai/
├── collaboration/              ← Coordination Framework
├── config/                     ← Konfigurationsdateien
├── decisions/                  ← Architecture Decision Records
├── guidelines/                 ← Coding & Process Guidelines
├── handovers/                  ← Feature Handover Documents
├── issues/{issue-id}/          ← Issue-spezifische Collaboration
├── knowledgebase/              ← Wissensdatenbank
├── logs/                       ← Agent Logs
├── permissions/                ← Agent Permissions
├── requirements/               ← Anforderungsanalysen
├── sprint/                     ← Sprint Planning & Tracking
├── status/                     ← Task Completion Tracking
├── templates/                  ← GitHub Issue & PR Templates
└── workflows/                  ← Development Workflows
```

## Code Style & Conventions
- **General**: Write clean, idiomatic code. Prefer readability over cleverness.
- **Naming**: Use descriptive variable and function names.
- **Comments**: Document complex logic and public APIs.
- **Language**: Code and technical docs in English, User docs as requested.

## Path-specific Instructions
Copilot wendet automatisch zusätzliche Instructions an basierend auf dem Dateipfad:

- `src/api/**, src/services/**` → [backend.instructions.md](.github/instructions/backend.instructions.md)
- `src/components/**, src/pages/**` → [frontend.instructions.md](.github/instructions/frontend.instructions.md)
- `**/*.test.*, **/*.spec.*` → [testing.instructions.md](.github/instructions/testing.instructions.md)
- `.github/**, Dockerfile` → [devops.instructions.md](.github/instructions/devops.instructions.md)
- `**/*` (Security Context) → [security.instructions.md](.github/instructions/security.instructions.md)

## Prompt Files

**⚠️ IMPORTANT: All prompts are stored in `.github/prompts/` directory**
- Prompts define reusable workflow triggers for development cycle
- Each prompt file follows naming: `{command-name}.prompt.md`
- See [INDEX.md](.github/prompts/INDEX.md) for complete prompt reference

### Development Cycle Prompts
**Location**: `.github/prompts/`

#### Feature Development
- `/start-feature` → New feature initiation & coordination (@SARAH)
- `/requirements-analysis` → Multi-agent requirement analysis (Domain teams)
- `/sprint-cycle` → Sprint planning, execution, retrospective (@ScrumMaster)

#### Development & QA
- `/code-review` → Code quality gate & security review (@TechLead)
- `/run-tests` → Testing & QA sign-off (@QA)
- `/bug-analysis` → Bug investigation & root cause (@TechLead)

#### Architecture & Security
- `/security-audit` → Security review & compliance (@Security)
- `/adr-create` or `/create-adr` → Architecture decision records (@Architect)

#### Deployment & Release
- `/deploy` → Deployment management & release (@DevOps)
- `/feature-handover` → Feature completion & documentation (@ProductOwner)

### Additional Prompts (Agent Management & Context)
- `/agent-removal` → Agent removal and deactivation (@SARAH)
- `/subagent-delegation` → SubAgent delegation routing (@SARAH)
- `/context-optimization` → Context optimization & token efficiency (@SARAH)
- `/dependency-upgrade-research` → Dependency research & knowledge base (@Backend/@DevOps)
- `/project-cleanup` → Project cleanup (10 dimensions) (@SARAH)

## Artifact Storage Locations

**CRITICAL - Storage Rules**:
- **Prompts**: `.github/prompts/` (workflow definitions)
- **Instructions**: `.github/instructions/` (coding guidelines)
- **Agents**: `.github/agents/` (agent definitions)
- **Requirements**: `.ai/requirements/` (feature specs, analysis documents) → **Managed by @ProductOwner**
- **Decisions**: `.ai/decisions/` (ADRs, architectural decisions) → **Managed by @Architect**
- **Logs**: `.ai/logs/` (execution logs, test reports) → **Managed by responsible agent**
- **Sprint**: `.ai/sprint/` (sprint planning, tracking) → **Managed by @ScrumMaster**
- **Issues**: `.ai/issues/` (issue-specific collaboration) → **Managed by issue owner**
- **Handovers**: `.ai/handovers/` (feature documentation) → **Managed by @ProductOwner**
- **Compliance**: `.ai/compliance/` (compliance tracking, audits) → **Managed by @Security, @Legal**
- **Collaboration**: `.ai/collaboration/` (coordination framework) → **Managed by @SARAH**
- **Config**: `.ai/config/` (configuration) → **Managed by @DevOps**
- **Knowledgebase**: `.ai/knowledgebase/` (documentation) → **Managed by @TechLead**
- **Status**: `.ai/status/` (task completion tracking) → **Managed by @ScrumMaster**
- **Templates**: `.ai/templates/` (GitHub templates) → **Managed by @SARAH**
- **Workflows**: `.ai/workflows/` (development workflows) → **Managed by @SARAH**

**Rule**: 
- **`.github/` folder** → Framework & process definitions (Copilot system, not project artifacts)
  - **Owner**: @SARAH (governance) + @TechLead (guidelines)
  - **Purpose**: Reusable workflow triggers, coding standards, agent definitions
  - **When to use**: Global process definitions, ALL teams must follow
  
- **`.ai/` folder** → Project artifacts & domain-specific documentation
  - **Owner**: Responsible agent (see table above)
  - **Purpose**: Feature specs, decisions, logs, sprint tracking, issue collaboration
  - **When to use**: Specific to current project work, managed by domain expert

## Agent Responsibility for `.ai/` Folder

Each agent is **responsible for creating and organizing** artifacts in the `.ai/` folder related to their domain:

| Agent | `.ai/` Responsibility | Artifacts |
|-------|----------------------|-----------|
| @ProductOwner | `requirements/`, `handovers/` | Feature specs, user stories, requirements analysis, feature handovers |
| @Architect | `decisions/` | Architecture Decision Records (ADRs), design patterns, system design docs |
| @ScrumMaster | `sprint/`, `status/` | Sprint plans, daily standups, velocity tracking, task status, retrospectives |
| @Security | `compliance/` | Security audits, compliance checklists, vulnerability reports, threat modeling |
| @Legal | `compliance/` | Legal compliance documents, GDPR reviews, contractual analysis |
| @TechLead | `knowledgebase/`, `decisions/` | Technical guides, best practices, code patterns, performance analysis |
| @Backend | `decisions/`, `knowledgebase/` | Backend architecture decisions, API documentation, data model docs |
| @Frontend | `decisions/`, `knowledgebase/` | Frontend architecture decisions, component documentation, state management docs |
| @DevOps | `config/`, `logs/` | Infrastructure configuration, deployment logs, monitoring setup |
| @SARAH | `collaboration/`, `templates/`, `workflows/` | Coordination framework, GitHub templates, workflow orchestration |
| Issue Owner | `issues/{issue-id}/` | Issue-specific collaboration, progress notes, blockers, decisions |

**Key Principle**: Agents own the organization and updates of `.ai/` artifacts related to their domain expertise.

## Anforderungsanalyse Workflow (Agent-Driven)
Bei neuen Anforderungen folge diesem Ablauf:
1. `@ProductOwner` → Erstellt `.ai/requirements/{feature}/` mit initiale Erfassung
2. `@Backend`, `@Frontend`, `@Security`, etc. → Erstellen Domain-Analysen in `.ai/decisions/`, `.ai/requirements/`
3. `@Architect` → Erstellt ADR in `.ai/decisions/`
4. `@SARAH` → Konsolidiert in `.ai/collaboration/` und stellt Konflikte auf
5. `@ProductOwner` → Finalisiert Spezifikation in `.ai/requirements/`

Siehe [AGENT_COORDINATION.md](.ai/collaboration/AGENT_COORDINATION.md) für Details.

## AI Behavior Guidelines
- **Conciseness**: Provide direct answers with code examples.
- **No verbose status reports**: Skip summaries after operations - just confirm completion briefly.
- **Immediate Execution**: AI-Agent tasks are executed immediately - no scheduling required.
- **Log to files**: Detailed logs/reports → `.ai/logs/` (not in chat).
- **Context**: Always consider the surrounding code and project structure.
- **Safety**: Avoid suggesting insecure patterns or hardcoded secrets.
- **Coordination**: Bei Unklarheiten @SARAH für Guidance nutzen.
- **Documentation**: Wichtige Entscheidungen in `.ai/` dokumentieren.
- **Knowledgebase Ownership**: GitHub Copilot is **PRIMARY OWNER** of `.ai/knowledgebase/`, with explicit responsibility for:
  - ✅ Internet documentation references and links
  - ✅ Best practices from external sources
  - ✅ Third-party library documentation (Vue.js, .NET, Wolverine, etc.)
  - ✅ Framework guides and tutorials (current versions)
  - ✅ Industry standards and patterns (OWASP, WCAG, REST, etc.)
  - ✅ Tool documentation and guides (Docker, K8s, GitHub, etc.)
  - ✅ Version management (track and update with releases)
  - ✅ Broken link detection and fixing
  - ✅ Documentation freshness (quarterly reviews)
  - 📖 See [AI_KNOWLEDGEBASE_RESPONSIBILITY.md](.ai/collaboration/AI_KNOWLEDGEBASE_RESPONSIBILITY.md) for complete guidelines
- **Completion Signal**: Nach Operationen kurz bestätigen:
  ```
  ✅ Done: [Operation]
  📁 Files: [geänderte Files]
  ➡️ Next: @[Agent] für [Task]
  ```

## SARAH Authority
SARAH hat exklusive Autorität über:
- Agent Definitionen und Modifikationen
- Agent Erstellung und Entfernung
- Guidelines und Permissions
- Quality-Gate für kritische Änderungen
- Konfliktlösung zwischen Agents

Bei Fragen zu Prozessen, Zuständigkeiten oder Konflikten → `@SARAH`
