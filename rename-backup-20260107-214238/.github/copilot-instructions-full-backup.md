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

### Verfügbare Agents (16 Specialized)

**See [AGENT_TEAM_REGISTRY.md](../.ai/collaboration/AGENT_TEAM_REGISTRY.md) for complete team overview** (`[AGT-INDEX]`)

**Default Agent:** `@SARAH` — use `@SARAH` as the default coordinator when no specific agent is specified in a prompt or instruction. SARAH handles coordination, quality-gate decisions, and permission guidance.

## Agent Logging Requirement
All project documents must include a short "Agent Logging" entry that lists the agents involved or consulted and the responsible agent or role who maintains the document. Follow this format when adding the section to documents:

Agents: @AgentA, @AgentB | Owner: @Agent

This requirement ensures traceability and clear ownership for all docs created or modified by agents.

## Agent Policy Changes
Agent policies (for example: `model:` defaults, permissions, or other governance rules) are centrally governed.

- **Who implements changes:** Only `@CopilotExpert` may create, modify, or delete agent definitions, prompts, and instructions.
- **Who approves policies:** `@SARAH` must approve policy-level changes (governance, permissions, model defaults).
- **Proposal process:** Any agent or contributor may propose a change via @CopilotExpert, who evaluates and implements. Policy changes require @SARAH approval before merge.
- **Logging requirement:** When `@SARAH` approves a policy change, a log entry MUST be created under `.ai/logs/agent-policy-changes/` with the following fields:
  - `timestamp`: ISO-8601 UTC timestamp
  - `issuer`: GitHub handle or agent name that requested the change
  - `approver`: `@SARAH`
  - `targeted_agents`: list of affected agent DocIDs or names
  - `summary`: short description of the change
  - `pr`: link to the PR or issue that contains the change

Example file name: `.ai/logs/agent-policy-changes/2025-12-31T15-30-00Z_policy_change_docmaintainer-model.md`

The `DocMaintainer` is responsible for verifying that the log entry exists after `@SARAH` approves a change; it must not independently apply policy changes.

Note: Routine documentation edits (for example: content fixes, link repairs, reorganization within `.ai/` that do not change agent policies or governance) do NOT require entries under `.ai/logs/agent-policy-changes/`. The logging requirement described above applies only to agent policy changes (changes that alter agent governance, defaults, permissions, or similar policy-level rules) where `@SARAH` approval is mandatory.

IMPORTANT: Any change that affects agent policies (including but not limited to changes to model defaults, permission rules, agent roles, or governance processes) is considered a policy-level change and MUST NOT be merged without explicit written approval from `@SARAH`. After `@SARAH` approval, the approver or issuer must create a log entry under `.ai/logs/agent-policy-changes/` containing the required metadata (`timestamp`, `issuer`, `approver`, `targeted_agents`, `summary`, `pr`) before or at merge time.

## Code Change Permissions (ENFORCED)

**Only developer agents may modify program code files.**

### Authorized Code Editors
| Agent | Allowed File Types | Domain |
|-------|-------------------|--------|
| `@Backend` | `.cs`, `.csproj`, `.slnx`, `appsettings.json` | Backend/API code |
| `@Frontend` | `.ts`, `.vue`, `.css`, `.scss`, `.html` | Frontend code |
| `@QA` | `*.test.*`, `*.spec.*`, test fixtures | Test code only |
| `@DevOps` | `Dockerfile`, `.yml`, `.yaml`, CI/CD scripts | Infrastructure code |

### NOT Authorized for Code Changes
| Agent | Role | Can Request Via |
|-------|------|-----------------|
| `@SARAH` | Coordination | → Request `@Backend` or `@Frontend` |
| `@Architect` | Design only | → Request `@Backend` or `@Frontend` |
| `@TechLead` | Standards only | → Maintain coding styles, StyleCop rules, linter rules, lessons learned + internet research for best practices |
| `@ProductOwner` | Requirements | → Create specs for developers |
| `@Security` | Audit only | → Report issues to developers |
| `@Legal` | Compliance | → Flag issues for developers |
| `@UX`/`@UI` | Design | → Create specs for `@Frontend` |
| `@DocMaintainer` | Docs only | → `.md` files in `.ai/` only |
| `@CopilotExpert` | Config only | → `.github/` Copilot files only |

### Enforcement
- Non-developer agents attempting code changes MUST delegate to authorized agents
- Code review required before merge (`@TechLead` approval)
- Violations should be reported to `@SARAH`

## Software Architecture Changes
Changes to software architecture (for example: Architecture Decision Records, major service boundary changes, or other system-design decisions) MUST be approved by both `@Architect` and `@TechLead` before being merged. `@Architect` and `@TechLead` may consult and use support from other agents (for example `@Security`, `@DevOps`, `@Backend`, `@Frontend`, or others) as needed to evaluate the change. Record approvals and rationale in the related ADR or PR so the decision trail is auditable.

## Dependency Approval
Introducing new third-party dependencies (libraries, SDKs, external services, or major infrastructure components) into the project MUST receive explicit approval from `@Legal`, `@Architect`, and `@TechLead` before they are added to the repository, build pipelines, or production environments. Proposals must include:

- **License & legal review** (handled by `@Legal`) — license compatibility and any contractual obligations.
- **Security assessment** (coordinated with `@Security`) — known vulnerabilities, CVE checks, and supply-chain risks.
- **Architecture impact analysis** (handled by `@Architect` and `@TechLead`) — fit with existing architecture, service boundaries, and maintenance implications.

Record approvals, the evaluation summary, and any mitigating actions in the related PR and, where appropriate, in `.ai/decisions/` or an ADR so the decision trail is auditable. For high-risk or commercial dependencies, also consult `@DevOps` and `@Security` before approval.

**Legal constraints:** `@Legal` may only approve dependencies that are explicitly free to use for commercial software (permissive or compatible licenses) or that are explicitly whitelisted by documented policy rules. Any dependency with restrictive, unclear, or commercial-only license terms must be escalated and documented; the PR must include license findings and any required mitigation (e.g., alternative libraries, procurement steps, or whitelist justification).

**Security verification:** Before `@DevOps` adds or updates dependencies in build pipelines or production environments, they MUST obtain written approval from `@Security` that the dependency is free of known security issues. `@Security` will validate dependencies using authoritative internet sources (CVE databases, vendor advisories, package registries, supply-chain scanners, and other available sources) and document the verification in the related PR. If any known vulnerabilities are identified, the PR must include mitigation steps, a timeline for remediation, or approved alternatives.

## Architect Responsibilities for New Dependencies
If any new dependency is proposed, `@Architect` MUST:

- Check the latest released version information from authoritative sources on the internet (official package registry, release notes, or vendor page).
- If the dependency has changed since the version known to the project's default LLM context (gpt-5-mini), ask `@SARAH` to perform targeted internet research and extend the `.ai/knowledgebase/` with a concise summary of breaking changes between the LLM-known version and the latest release. The summary should include migration notes, API diffs, and links to official changelogs or migration guides.
- For dependencies or APIs that are not covered or are unknown to the LLM, coordinate the creation of the following artifacts and add them to `.ai/knowledgebase/` (or `.ai/decisions/` when appropriate):
  - **API How-To** — minimal examples showing integration patterns used by B2X.
  - **Summary** — key behaviors, configuration points, and known limitations.
  - **Migration Guide** — step-by-step instructions to migrate from the currently referenced version to the target version, including code snippets and test/verification steps.

Record the Architect's findings and the `@SARAH` knowledgebase updates in the related PR and/or ADR. `@Architect` may consult `@Backend`, `@DevOps`, `@Security`, or other agents to validate integration concerns, test requirements, or operational impacts.
This Architect → `@SARAH` KB update process also explicitly applies to updates of dependency versions. When proposing or performing a dependency version update, `@Architect` MUST check the latest released version information, determine whether the change includes breaking changes relative to the version known to the project's default LLM context, and ask `@SARAH` to extend the `.ai/knowledgebase/` with a concise summary of breaking changes, migration notes, API diffs, and official changelog links. Treat dependency-version updates with the same documentation and KB update requirements as introducing a new dependency.



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
- **Guidelines**: `.ai/guidelines/` (coding standards, StyleCop, linter rules) → **Managed by @TechLead**
**Knowledgebase**: `.ai/knowledgebase/` (documentation) → **Managed by GitHub Copilot**
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
| @CopilotExpert | `.github/agents/`, `.github/prompts/`, `.github/instructions/` | **EXCLUSIVE:** Agent definitions, prompts, instructions, MCP config |
| Issue Owner | `issues/{issue-id}/` | Issue-specific collaboration, progress notes, blockers, decisions |

| @DocMaintainer | `.ai/` (docs only) | Enforce DocID naming conventions, extend naming for new use cases, update and manage existing documents, fix broken links, and keep registry references up-to-date |

**Authority:** `@DocMaintainer` is empowered to update, rename, archive, and fix documentation files under `.ai/` to maintain accuracy and link integrity. Doc-only changes may be committed with clear messages (audit logs should be created under `.ai/logs/documentation/`). For policy-level naming or retention decisions, `@DocMaintainer` must open an issue and notify `@SARAH` for final approval.

**Note:** `.github/agents/`, `.github/prompts/`, `.github/instructions/` are **exclusively managed by @CopilotExpert**. @DocMaintainer may NOT modify these files.

**Key Principle**: Agents own the organization and updates of `.ai/` artifacts related to their domain expertise.

## Anforderungsanalyse Workflow (Agent-Driven)
Bei neuen Anforderungen folge diesem Ablauf:
1. `@ProductOwner` → Erstellt `.ai/requirements/{feature}/` mit initiale Erfassung
2. `@Backend`, `@Frontend`, `@Security`, etc. → Erstellen Domain-Analysen in `.ai/decisions/`, `.ai/requirements/`
3. `@Architect` → Erstellt ADR in `.ai/decisions/`
4. `@SARAH` → Konsolidiert in `.ai/collaboration/` und stellt Konflikte auf
5. `@ProductOwner` → Finalisiert Spezifikation in `.ai/requirements/`

Siehe [AGENT_COORDINATION.md](../.ai/collaboration/AGENT_COORDINATION.md) für Details.

## File Creation Rules in `.ai/` (MANDATORY)

**CRITICAL**: These rules prevent duplicate files and maintain folder hygiene.

### Before Creating ANY File in `.ai/`
1. **CHECK EXISTENCE FIRST** - Always search for existing file with similar name before creating
2. **CHECK DOCUMENT_REGISTRY.md** - Verify DocID is available and not already assigned
3. **USE EXACT PATHS** - Never let OS create " 2", " 3" variants (e.g., `file 2.md`)
4. **UPDATE REGISTRY** - Add new DocID entry immediately after creating file

### Prohibited Patterns (ENFORCED)
- ❌ Files ending with ` 2`, ` 3`, ` 4`, etc. (e.g., `README 2.md`)
- ❌ Folders ending with ` 2`, ` 3`, ` 4`, etc. (e.g., `architecture 2/`)
- ❌ Creating new files without checking existence first
- ❌ Copying files without renaming appropriately
- ❌ Using drag-and-drop that creates macOS " 2" duplicates

### On Conflict Detection
If a file with similar name already exists:
1. **READ** the existing file first
2. **MERGE** content if needed (don't create new variant)
3. **UPDATE** existing file (append, modify, or replace)
4. **NEVER** create " 2" variant - always resolve the conflict

### Validation Command
Run periodically to check for violations:
```bash
find .ai -name "* [0-9]*" 2>/dev/null
# Should return empty - any results indicate policy violation
```

### Consequence
Duplicate files will be **deleted without merge** during cleanup. The original (without number suffix) is always considered authoritative.

---

## AI Behavior Guidelines
- **Conciseness**: Provide direct answers with code examples.
- **No verbose status reports**: Skip summaries after operations - just confirm completion briefly.
- **Immediate Execution**: AI-Agent tasks are executed immediately - no scheduling required.
- **Log to files**: Detailed logs/reports → `.ai/logs/` (not in chat).
- **Context**: Always consider the surrounding code and project structure.
- **Safety**: Avoid suggesting insecure patterns or hardcoded secrets.
- **Agent Responsibility**: If you are not the responsible agent for a question, hand it over to `@SARAH` for coordination. Verify your domain ownership before proceeding with complex tasks.
- **Coordination**: Bei Unklarheiten @SARAH für Guidance nutzen.
- **Token Optimization** (Prevent rate limiting - see [GL-006]):
  - Agent files: Max 3 KB - link to docs, don't embed
  - Use `read_file` with specific line ranges, not entire files
  - Reference `[DocID]` instead of copying content inline
  - Batch multiple changes into single requests
  - Archive status files older than 7 days to `.ai/archive/`
  - Prefer `run_task`/`runTests` tools over verbose terminal commands
- **Lessons Learned**: Before starting implementation tasks, **always check** `.ai/knowledgebase/lessons.md` for relevant lessons from past work. This prevents repeating known mistakes (e.g., ESLint 9.x flat config, Tailwind v4 class changes, API breaking changes).
- **Test Failures**: When tests fail, **consider whether the tests themselves are invalid or outdated** before assuming the implementation is wrong. Tests may need updating due to: changed requirements, API changes, deprecated patterns, or incorrect original assumptions.
- **Error Reports**: When a user reports an error, **run smoke tests** to verify system health and **evaluate if test coverage needs extending** to catch similar issues in the future. Add missing test cases to prevent regression.
- **Structural Changes**: When making structural changes (renaming, moving files, changing signatures, modifying DTOs/models, updating API contracts), **always update corresponding tests**. Check for affected unit tests, integration tests, and E2E tests.
 - **Product Vision Alignment (Architect & TechLead)**: `@Architect` and `@TechLead` must always keep the ProductVision in mind when designing or approving features. Be critical: verify assumptions against authoritative internet sources when unsure, or ask for clarifying information. When proposing ideas or design options, present them as clearly numbered multiple-choice options (1., 2., 3., ...) with concise pros/cons and a recommended option.
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
- Guidelines und Permissions
- Quality-Gate für kritische Änderungen
- Konfliktlösung zwischen Agents
- Policy-Genehmigungen (nach @CopilotExpert Vorschlag)

Bei Fragen zu Prozessen, Zuständigkeiten oder Konflikten → `@SARAH`

## CopilotExpert Exclusive Authority
**@CopilotExpert (`AGT-018`) has EXCLUSIVE authority** over all GitHub Copilot configuration:

### Exclusive Ownership
- ✅ Agent definitions (`.github/agents/*.agent.md`)
- ✅ Custom instructions (`.github/instructions/*.instructions.md`)
- ✅ Prompt files (`.github/prompts/*.prompt.md`)
- ✅ Repository-wide instructions (`.github/copilot-instructions.md` - technical content)
- ✅ MCP server configuration (`.vscode/mcp.json`)

### Rules (ENFORCED)
- ❌ **NO OTHER AGENT** may create, modify, or delete agent definitions
- ❌ **NO OTHER AGENT** may create, modify, or delete prompt files
- ❌ **NO OTHER AGENT** may create, modify, or delete instruction files
- ✅ Other agents may **REQUEST** changes via @CopilotExpert
- ✅ @SARAH approves policy-level changes proposed by @CopilotExpert

### Process for Changes
1. Agent requests change → @CopilotExpert
2. @CopilotExpert evaluates and implements
3. Policy changes require @SARAH approval
4. @CopilotExpert commits with clear message

📖 **See**: [KB-022] GitHub Copilot Customization Guide

## SARAH Commit & Prompt Tracking
`@SARAH` is responsible for tracking which prompts produced which file changes. For any agent-driven or prompt-triggered modification, `@SARAH` must ensure:

- A clear mapping exists between the triggering prompt (or PR/issue) and the resulting file changes (list of files, commit SHA, and PR if applicable).
 - A clear mapping exists between the triggering prompt (or PR/issue) and the resulting file changes (list of files, commit SHA, and PR if applicable).
 - Commits are clean and targeted (only touch files relevant to the change) and include a concise commit message.
 - Detailed documentation, rationale, and migration notes belong in the PR description (not in commit messages). The PR description should link to the originating prompt or request and to any KB/`.ai/` artifacts.
 - The mapping and a short rationale are recorded in the related issue/PR or in `.ai/logs/operations/` or `.ai/issues/{id}/progress.md` for traceability.

This accountability helps auditing, reviewing, and rolling back changes when necessary.
