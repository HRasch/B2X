# GitHub Copilot Instructions

**DocID**: `INS-000` (Master Instructions)

## Project Context
- **Project**: B2Connect - Multi-Agent Development Framework
- **Tech Stack**: .NET 10, Vue.js 3, Wolverine CQRS, PostgreSQL, Elasticsearch
- **Coordinator**: @SARAH handles coordination, quality-gates, permissions

## Document Reference System

Use stable DocIDs for cross-referencing. See [DOCUMENT_REGISTRY.md](../.ai/DOCUMENT_REGISTRY.md).

| Prefix | Category | Prefix | Category |
|--------|----------|--------|----------|
| `ADR-*` | Architecture Decisions | `KB-*` | Knowledgebase |
| `GL-*` | Guidelines | `WF-*` | Workflows |
| `PRM-*` | Prompts | `INS-*` | Instructions |
| `AGT-*` | Agent Definitions | `DOC-*` | Documentation |

---

## Agent System

**See [GL-010] for complete agent list with DocIDs and responsibilities.**

**Default Agent:** `@SARAH` — Coordinator for quality-gate decisions and permission guidance.

### Core Agents (Quick Reference)
| Agent | Domain | Agent | Domain |
|-------|--------|-------|--------|
| @SARAH | Coordination | @Backend | .NET/Wolverine |
| @Frontend | Vue.js 3 | @QA | Testing |
| @Architect | System Design | @TechLead | Code Quality |
| @Security | Security | @DevOps | Infrastructure |
| @CopilotExpert | Copilot Config | @DocMaintainer | Docs |

---

## Governance & Permissions

**Full details in [GL-008] Governance Policies.**

### Code Change Permissions (ENFORCED)
| Agent | Allowed Files |
|-------|---------------|
| @Backend | `.cs`, `.csproj`, `.slnx`, `appsettings.json` |
| @Frontend | `.ts`, `.vue`, `.css`, `.scss`, `.html` |
| @QA | `*.test.*`, `*.spec.*`, test fixtures |
| @DevOps | `Dockerfile`, `.yml`, `.yaml`, CI/CD |

Non-developer agents MUST delegate code changes to authorized agents.

### Key Policies
- **Agent Policy Changes**: @CopilotExpert implements, @SARAH approves → See [GL-008]
- **Dependency Approval**: @Legal + @Architect + @TechLead → See [GL-008]
- **Architecture Changes**: @Architect + @TechLead approval → See [GL-008]

---

## File Structure & Artifacts

**Full details in [GL-010] Agent & Artifact Organization.**

```
.github/
├── copilot-instructions.md     ← This file (INS-000)
├── agents/*.agent.md           ← Agent definitions
├── instructions/*.instructions.md
└── prompts/*.prompt.md

.ai/
├── DOCUMENT_REGISTRY.md        ← DocID registry
├── decisions/                  ← ADRs (@Architect)
├── guidelines/                 ← Guidelines (@TechLead)
├── knowledgebase/              ← KB (GitHub Copilot)
├── requirements/               ← Specs (@ProductOwner)
└── [see GL-010 for complete structure]
```

---

## Path-specific Instructions

Applied automatically based on file path:
- `src/api/**` → [backend.instructions.md](instructions/backend.instructions.md)
- `src/components/**` → [frontend.instructions.md](instructions/frontend.instructions.md)
- `**/*.test.*` → [testing.instructions.md](instructions/testing.instructions.md)
- `.github/**` → [devops.instructions.md](instructions/devops.instructions.md)
- `**/*` → [security.instructions.md](instructions/security.instructions.md)

---

## Key Prompts

See [PROMPTS_INDEX.md](../.ai/collaboration/PROMPTS_INDEX.md) for complete reference.

| Command | Purpose | Agent |
|---------|---------|-------|
| `/start-feature` | New feature | @SARAH |
| `/code-review` | Quality gate | @TechLead |
| `/run-tests` | Testing | @QA |
| `/deploy` | Deployment | @DevOps |
| `/security-audit` | Security | @Security |

---

## AI Behavior Guidelines

**Full details in [GL-009] AI Behavior Guidelines.**

### Quick Rules
1. **Conciseness**: Direct answers with code examples
2. **Immediate Execution**: AI tasks execute immediately, no scheduling
3. **Token Optimization**: See [GL-006] - Max 3 KB agent files, reference don't embed
4. **Before coding**: Check `.ai/knowledgebase/lessons.md`
5. **Delegation**: If not your domain → @SARAH
6. **Commits**: Small, focused, meaningful messages

### Completion Signal
```
✅ Done: [Operation]
📁 Files: [changed files]
➡️ Next: @[Agent] for [Task]
```

---

## Code Style

- Write clean, idiomatic code
- Use descriptive names
- Document complex logic
- Code in English, docs as requested

---

## Key Guidelines Reference

| DocID | Topic | When to Use |
|-------|-------|-------------|
| [GL-006] | Token Optimization | Prevent rate limiting |
| [GL-008] | Governance Policies | Policy changes, permissions, dependencies |
| [GL-009] | AI Behavior | Implementation guidelines, fallback procedures |
| [GL-010] | Agent & Artifact Organization | File structure, agent responsibilities |
| [GL-014] | Pre-Release Phase | Breaking changes allowed (v0.x) |

---

## Authority Summary

- **@SARAH**: Guidelines, permissions, quality-gate, conflict resolution
- **@CopilotExpert**: EXCLUSIVE over `.github/agents/`, prompts, instructions
- **@Architect + @TechLead**: Architecture changes approval

For processes, ownership, conflicts → `@SARAH`

---
**Maintained by**: @SARAH | **Size target**: <10 KB
