---
docid: GL-071
title: GL 009 AI BEHAVIOR
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

# GL-009: AI Behavior Guidelines

**DocID**: `GL-009`  
**Status**: Active | **Owner**: @SARAH  
**Created**: 2026-01-05

## Core Principles

- **Conciseness**: Direct answers with code examples
- **No verbose status reports**: Skip summaries after operations
- **Immediate Execution**: AI tasks execute immediately, no scheduling
- **Log to files**: Detailed logs → `.ai/logs/` (not in chat)
- **Context awareness**: Consider surrounding code and project structure
- **Safety**: No insecure patterns or hardcoded secrets
- **Agent Responsibility**: If not responsible agent → hand over to @SARAH
- **Coordination**: Bei Unklarheiten @SARAH für Guidance nutzen

---

## Token Optimization (See [GL-006])

- Agent files: Max 3 KB - link to docs, don't embed
- Use `read_file` with specific line ranges, not entire files
- Reference `[DocID]` instead of copying content inline
- Batch multiple changes into single requests
- Archive status files >7 days to `.ai/archive/`
- Prefer `run_task`/`runTests` over verbose terminal commands

---

## Before Implementation

1. **Check lessons learned**: Always check `.ai/knowledgebase/lessons.md` for relevant lessons
2. Verify domain ownership before complex tasks
3. If not responsible agent → hand over to @SARAH

---

## Test Handling

- **Test failures**: Consider if tests are outdated before assuming implementation is wrong
  - Tests may need updating: changed requirements, API changes, deprecated patterns
- **Error reports**: Run smoke tests, evaluate test coverage extension
  - Add missing test cases to prevent regression
- **Structural changes**: Always update corresponding tests
  - Check unit tests, integration tests, and E2E tests

---

## Product Vision Alignment

`@Architect` and `@TechLead` must:
- Keep ProductVision in mind when designing/approving features
- Be critical: verify assumptions against authoritative internet sources
- Present ideas as numbered multiple-choice options with pros/cons and recommendation

---

## Completion Signal

After operations, confirm briefly:
```
✅ Done: [Operation]
📁 Files: [changed files]
➡️ Next: @[Agent] for [Task]
```

---

## Commit Strategy

- Create commit after each successful step
- Clear, meaningful messages (e.g., `feat(search): wire Elasticsearch config`)
- Keep commits small and focused
- Avoid bundling unrelated changes

---

## Agent Fallback Procedure

When encountering unexpected dependency/API issues:

1. **Local check**: Search workspace and `.ai/knowledgebase/` for notes. Confirm versions in `package.json`, `Directory.Packages.props`
2. **Validate LLM knowledge**: If outdated → escalate to @SARAH
3. **SARAH research**: Documents in `.ai/knowledgebase/dependency-updates/{name}.md`:
   - Current stable versions
   - Breaking changes
   - Authoritative links
   - Minimal repro/usage example
4. **Update**: Commit new doc, add summary to issue
5. **Retry**: Resume task with updated guidance
6. **Notify**: @Security/@Legal if compliance impact
7. **Learn**: Add lesson to `lessons.md`

### Rules
- Never hardcode credentials; use env vars or mocks
- Always reference authoritative sources with URLs
- Keep knowledgebase entries concise

---

## Knowledgebase Ownership

GitHub Copilot is PRIMARY OWNER of `.ai/knowledgebase/`:
- ✅ Internet documentation references and links
- ✅ Best practices from external sources
- ✅ Third-party library documentation (Vue.js, .NET, Wolverine, etc.)
- ✅ Framework guides and tutorials (current versions)
- ✅ Industry standards (OWASP, WCAG, REST, etc.)
- ✅ Tool documentation (Docker, K8s, GitHub, etc.)
- ✅ Version management (track and update with releases)
- ✅ Broken link detection and fixing
- ✅ Documentation freshness (quarterly reviews)

📖 See [AI_KNOWLEDGEBASE_RESPONSIBILITY.md] for complete guidelines.

---

## File Creation Rules in `.ai/` (MANDATORY)

### Before Creating ANY File
1. **CHECK EXISTENCE FIRST** - Search for existing file with similar name
2. **CHECK DOCUMENT_REGISTRY.md** - Verify DocID is available
3. **USE EXACT PATHS** - Never let OS create " 2", " 3" variants
4. **UPDATE REGISTRY** - Add DocID entry immediately after creating

### Prohibited Patterns
- ❌ Files ending with ` 2`, ` 3`, ` 4`
- ❌ Creating files without checking existence first
- ❌ Copying files without renaming

### On Conflict Detection
1. **READ** existing file first
2. **MERGE** content if needed
3. **UPDATE** existing file
4. **NEVER** create " 2" variant

### Validation
```bash
find .ai -name "* [0-9]*" 2>/dev/null
# Should return empty
```

Duplicate files will be **deleted without merge** during cleanup.

---

## Anforderungsanalyse Workflow

Bei neuen Anforderungen:
1. `@ProductOwner` → `.ai/requirements/{feature}/` mit initialer Erfassung
2. `@Backend`, `@Frontend`, `@Security` → Domain-Analysen
3. `@Architect` → ADR in `.ai/decisions/`
4. `@SARAH` → Konsolidiert, stellt Konflikte auf
5. `@ProductOwner` → Finalisiert Spezifikation

Siehe [AGENT_COORDINATION.md] für Details.

---

**Maintained by**: @SARAH
