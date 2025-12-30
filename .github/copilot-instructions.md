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
Nutze diese Standard-Prompts für wiederkehrende Tasks:

### Anforderungsanalyse
- `/requirements-analysis` → Multi-Agent Anforderungsanalyse (Domain-spezifisch)
- `/requirements-consolidation` → SARAH Konsolidierung aller Analysen

### Development Workflows
- `/code-review` → Standardisierter Code Review
- `/feature-handover` → Feature Handover Dokumentation
- `/security-audit` → Security Audit Checklist
- `/adr-create` → Architecture Decision Record erstellen
- `/bug-analysis` → Bug Analyse und Root Cause
- `/dependency-upgrade-research` → Software-Version Recherche & Knowledgebase Update
- `/project-cleanup` → Bestehendes Projekt bereinigen (10 Dimensionen)

### Agent Management (SARAH)
- `/agent-removal` → Agent Entfernung und Deaktivierung
- `/subagent-delegation` → SubAgent Delegation Routing

### Context Management
- `/context-optimization` → Agent-Kontexte optimieren & Token sparen
- `/subagent-delegation` → SubAgent Delegation für effiziente Spezialisierung

## Anforderungsanalyse Workflow
Bei neuen Anforderungen folge diesem Ablauf:
1. `@ProductOwner` → Initiale Erfassung in `.ai/requirements/`
2. `@Backend`, `@Frontend`, `@Security`, etc. → Domain-Analysen
3. `@SARAH` → Konsolidierung und Konfliktlösung
4. `@ProductOwner` → Finale Spezifikation

Siehe [AGENT_COORDINATION.md](.ai/collaboration/AGENT_COORDINATION.md) für Details.

## AI Behavior Guidelines
- **Conciseness**: Provide direct answers with code examples.
- **No verbose status reports**: Skip summaries after operations - just confirm completion briefly.
- **Log to files**: Detailed logs/reports → `.ai/logs/` (not in chat).
- **Context**: Always consider the surrounding code and project structure.
- **Safety**: Avoid suggesting insecure patterns or hardcoded secrets.
- **Coordination**: Bei Unklarheiten @SARAH für Guidance nutzen.
- **Documentation**: Wichtige Entscheidungen in `.ai/` dokumentieren.
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
