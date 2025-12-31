# GitHub Copilot Instructions

Diese Datei definiert projekt-weite Anweisungen für alle GitHub Copilot Agents.

**DocID**: `INS-000` (Master Instructions)

## Project Context
- **Description**: AI-DEV - Multi-Agent Development Framework
- **Architecture**: Agent-basierte Entwicklung mit SARAH als Coordinator
- **Tech Stack**: .NET 10, Vue.js 3, Wolverine CQRS, PostgreSQL, Elasticsearch

## Document Reference System

**All documents use stable DocIDs for cross-referencing.** See [DOCUMENT_REGISTRY.md](../.ai/DOCUMENT_REGISTRY.md) for the complete registry.

### Quick Reference Prefixes
| Prefix | Category | Example |
|--------|----------|---------|
| `ADR-*` | Architecture Decisions | `[ADR-001]` Wolverine over MediatR |
| `KB-*` | Knowledgebase | `[KB-006]` Wolverine Patterns |
| `GL-*` | Guidelines | `[GL-002]` Subagent Delegation |
| `WF-*` | Workflows | `[WF-001]` Context Optimization |
| `PRM-*` | Prompts | `[PRM-001]` Start Feature |
| `INS-*` | Instructions | `[INS-001]` Backend Instructions |
| `AGT-*` | Agent Definitions | `[AGT-001]` SARAH |
| `DOC-*` | Documentation | `[DOC-001]` Quick Start Guide |

### Usage
```markdown
See [ADR-001] for architecture rationale.
Follow [KB-006] for implementation patterns.
```

## Agent System

### Verfügbare Agents (15 Specialized)

**See [AGENT_TEAM_REGISTRY.md](../.ai/collaboration/AGENT_TEAM_REGISTRY.md) for complete team overview** (`[AGT-INDEX]`)

**Default Agent:** `@SARAH` — use `@SARAH` as the default coordinator when no specific agent is specified in a prompt or instruction. SARAH handles coordination, quality-gate decisions, and permission guidance.

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

**Specialist Agents (Coming Soon)**:
- @QA-Frontend (E2E, UI Testing, Playwright)
- @QA-Pentesting (Security Testing, OWASP)
- @QA-Performance (Load Testing, Scalability)

### Dateien-Struktur
```
.github/
├── copilot-instructions.md     ← Du bist hier (INS-000)
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
├── logs/                       ← Agent Logs
Note: Agents MUST ignore `.ai/logs/` when building prompt/context inputs; logs are archival-only and must not be included in agent prompt contexts.
├── permissions/                ← Agent Permissions
├── requirements/               ← Anforderungsanalysen (REQ-*)
├── sprint/                     ← Iteration Planning & Tracking (SPR-*)
├── status/                     ← Task Completion Tracking
├── templates/                  ← GitHub Issue & PR Templates (TPL-*)
└── workflows/                  ← Development Workflows (WF-*)
```

## Code Style & Conventions
- **General**: Write clean, idiomatic code. Prefer readability over cleverness.
- **Naming**: Use descriptive variable and function names.
- **Comments**: Document complex logic and public APIs.
- **Language**: Code and technical docs in English, User docs as requested.

## Path-specific Instructions
Copilot wendet automatisch zusätzliche Instructions an basierend auf dem Dateipfad:

- `src/api/**, src/services/**` → [backend.instructions.md](instructions/backend.instructions.md)
- `src/components/**, src/pages/**` → [frontend.instructions.md](instructions/frontend.instructions.md)
- `**/*.test.*, **/*.spec.*` → [testing.instructions.md](instructions/testing.instructions.md)
- `.github/**, Dockerfile` → [devops.instructions.md](instructions/devops.instructions.md)
- `**/*` (Security Context) → [security.instructions.md](instructions/security.instructions.md)

## Prompt Files

**⚠️ IMPORTANT: All prompts are stored in `.github/prompts/` directory**
- Prompts define reusable workflow triggers for development cycle
- Each prompt file follows naming: `{command-name}.prompt.md`
- See [PROMPTS_INDEX.md](../.ai/collaboration/PROMPTS_INDEX.md) for complete prompt reference

### Development Cycle Prompts
**Location**: `.github/prompts/`

#### Feature Development
- `/start-feature` → New feature initiation & coordination (@SARAH)
- `/requirements-analysis` → Multi-agent requirement analysis (Domain teams)
- `/iteration-cycle` → Iteration planning, execution, retrospective (@ScrumMaster)

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

| @DocMaintainer | `.ai/` (docs + prompts) | Enforce DocID naming conventions, extend naming for new use cases, update and manage existing documents, fix broken links, and keep registry references up-to-date |

**Authority:** `@DocMaintainer` is empowered to update, rename, archive, and fix documentation files under `.ai/` and `.github/prompts/` to maintain accuracy and link integrity. Doc-only changes may be committed with clear messages (audit logs should be created under `.ai/logs/documentation/`). For policy-level naming or retention decisions, `@DocMaintainer` must open an issue and notify `@SARAH` for final approval.

**Key Principle**: Agents own the organization and updates of `.ai/` artifacts related to their domain expertise.

## Anforderungsanalyse Workflow (Agent-Driven)
Bei neuen Anforderungen folge diesem Ablauf:
1. `@ProductOwner` → Erstellt `.ai/requirements/{feature}/` mit initiale Erfassung
2. `@Backend`, `@Frontend`, `@Security`, etc. → Erstellen Domain-Analysen in `.ai/decisions/`, `.ai/requirements/`
3. `@Architect` → Erstellt ADR in `.ai/decisions/`
4. `@SARAH` → Konsolidiert in `.ai/collaboration/` und stellt Konflikte auf
5. `@ProductOwner` → Finalisiert Spezifikation in `.ai/requirements/`

Siehe [AGENT_COORDINATION.md](../.ai/collaboration/AGENT_COORDINATION.md) für Details.

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
  - 📖 See [AI_KNOWLEDGEBASE_RESPONSIBILITY.md](../.ai/collaboration/AI_KNOWLEDGEBASE_RESPONSIBILITY.md) for complete guidelines
- **Completion Signal**: Nach Operationen kurz bestätigen:
  ```
  ✅ Done: [Operation]
  📁 Files: [geänderte Files]
  ➡️ Next: @[Agent] für [Task]
  ```

  - **Commit After Steps**: After each successful implementation-step, create a repository commit with a clear, meaningful message describing the change (for example: "feat(search): wire Elasticsearch config from Aspire"). Keep commits small and focused; avoid bundling unrelated changes to simplify review and rollback.

## Agent Fallback Procedure
When an agent encounters an unexpected dependency, API mismatch, or other information gap, follow this procedure:

- **1) Quick local check:** Search the workspace and `.ai/knowledgebase/` for the dependency or API notes. Confirm local package versions (`package.json`, `Directory.Packages.props`, `obj/project.assets.json`, etc.).
- **2) Validate LLM knowledge:** If the dependency or API is not documented in `.ai/knowledgebase/` or the information seems outdated, escalate the investigation to `@SARAH` for external research.
- **3) SARAH research step:** `@SARAH` performs targeted internet research (official docs, changelogs, release notes) and records findings in `.ai/knowledgebase/dependency-updates/{dependency-name}.md` including: current stable versions, relevant breaking changes, authoritative links, and a minimal repro or usage example.
- **4) Update knowledgebase:** The researching agent commits the new/updated doc to `.ai/knowledgebase/` and adds a short summary to the issue or todo that triggered the investigation.
- **5) Switch back & retry:** The original agent resumes the task, applying the updated guidance or code examples. If code changes are required, open a PR and mark the change with the dependency update note.
- **6) Notify stakeholders:** If the change impacts security, license, or legal compliance, notify `@Security` and `@Legal` before merging.
- **7) Record and learn:** Add a one-line lesson to `.ai/knowledgebase/lessons.md` so future agents can avoid the same gap.

Rules and constraints:
- Never hardcode credentials or secrets during research or repros; use environment variables or mocks.
- Always reference authoritative sources (docs, changelogs) with URLs in the knowledgebase entry.
- Keep the knowledgebase entry concise (summary + links + minimal example) so agents can quickly consume it.

## SARAH Authority
SARAH hat exklusive Autorität über:
- Agent Definitionen und Modifikationen
- Agent Erstellung und Entfernung
- Guidelines und Permissions
- Quality-Gate für kritische Änderungen
- Konfliktlösung zwischen Agents

Bei Fragen zu Prozessen, Zuständigkeiten oder Konflikten → `@SARAH`
