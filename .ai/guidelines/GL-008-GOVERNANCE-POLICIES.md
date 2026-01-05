# GL-008: Governance Policies

**DocID**: `GL-008`  
**Status**: Active | **Owner**: @SARAH  
**Created**: 2026-01-05

## Agent Logging Requirement

All project documents must include a short "Agent Logging" entry:

```
Agents: @AgentA, @AgentB | Owner: @Agent
```

This ensures traceability and clear ownership for all docs created or modified by agents.

---

## Agent Policy Changes

Agent policies (`model:` defaults, permissions, governance) are centrally governed.

### Authority
- **Implements**: Only `@CopilotExpert` creates/modifies agent definitions, prompts, instructions
- **Approves**: `@SARAH` must approve policy-level changes

### Policy Change Process
1. Propose change via @CopilotExpert
2. @CopilotExpert evaluates and implements
3. Policy changes require @SARAH approval before merge
4. Log entry created under `.ai/logs/agent-policy-changes/`

### Logging Requirement
When @SARAH approves a policy change, create log entry with:
- `timestamp`: ISO-8601 UTC
- `issuer`: Requester name
- `approver`: @SARAH
- `targeted_agents`: Affected agent DocIDs
- `summary`: Short description
- `pr`: Link to PR/issue

**Example filename**: `2025-12-31T15-30-00Z_policy_change_docmaintainer-model.md`

### Exemptions
Routine documentation edits (content fixes, link repairs) do NOT require policy logs.

---

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
| `@TechLead` | Standards only | → Maintain coding styles, linter rules |
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

---

## Software Architecture Changes

ADRs and major service changes require approval from both `@Architect` and `@TechLead`.

### Process
1. Create ADR draft in `.ai/decisions/`
2. @Architect and @TechLead review
3. Consult @Security, @DevOps as needed
4. Record approvals in ADR

---

## Dependency Approval

New dependencies require approval from `@Legal`, `@Architect`, and `@TechLead`.

### Required Reviews
| Reviewer | Checks |
|----------|--------|
| @Legal | License compatibility, contractual obligations |
| @Security | CVE checks, supply-chain risks |
| @Architect | Architecture fit, maintenance implications |
| @TechLead | Code quality impact |

### Process
1. Submit proposal with dependency details
2. @Legal validates license (must be commercial-friendly)
3. @Security scans for vulnerabilities
4. @Architect/@TechLead assess architecture impact
5. Record approval in PR and `.ai/decisions/` if needed

### Legal Constraints
@Legal may only approve dependencies explicitly free for commercial software (permissive/compatible licenses) or whitelisted by policy. Restrictive/unclear licenses must be escalated with findings and mitigation.

### Security Verification
Before @DevOps adds dependencies, @Security MUST verify using CVE databases, vendor advisories, and supply-chain scanners. Document verification in PR.

### Architect Responsibilities for New Dependencies
`@Architect` MUST:
- Check latest version from authoritative sources
- If changed since LLM knowledge cutoff → ask @SARAH for internet research
- Update `.ai/knowledgebase/` with: breaking changes, migration notes, API diffs, changelog links
- Create artifacts for unknown APIs:
  - **API How-To** — integration patterns
  - **Summary** — behaviors, config, limitations
  - **Migration Guide** — step-by-step instructions

This also applies to **dependency version updates**.

---

## CopilotExpert Exclusive Authority

`@CopilotExpert` (`AGT-018`) has EXCLUSIVE authority over:
- Agent definitions (`.github/agents/*.agent.md`)
- Custom instructions (`.github/instructions/*.instructions.md`)
- Prompt files (`.github/prompts/*.prompt.md`)
- Repository-wide instructions (`.github/copilot-instructions.md`)
- MCP server configuration (`.vscode/mcp.json`)

**NO OTHER AGENT** may modify these files. Request changes via @CopilotExpert.

---

## SARAH Authority

@SARAH has exclusive authority over:
- Guidelines and Permissions
- Quality-Gate for critical changes
- Conflict resolution between agents
- Policy approvals (after @CopilotExpert proposal)

---

## SARAH Commit & Prompt Tracking

@SARAH tracks which prompts produced which file changes:
- Clear mapping: prompt/PR → file changes, commit SHA
- Commits clean and targeted
- Detailed docs in PR description (not commit messages)
- Mapping recorded in `.ai/logs/operations/` or `.ai/issues/{id}/progress.md`

---

**Maintained by**: @SARAH
