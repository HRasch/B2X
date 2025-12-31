# Documentation Standards - Anti-Bloat Rules

**Authority**: @process-assistant (exclusive enforcement)  
**Effective**: 30. Dezember 2025  
**Purpose**: Prevent documentation bloat while maintaining clarity

---

## 🚫 The 8 Anti-Bloat Rules

### 1. One Document Per Concept (Hard Rule)

**❌ VIOLATION EXAMPLES**:
```
✗ MAILBOX_SYSTEM.md + MAILBOX_DOCUMENTATION_INDEX.md + MAILBOX_CLARIFICATION.md (3 files for 1 concept)
✗ WORKFLOW_SPRINT.md + SPRINT_EXECUTION.md + SPRINT_PROCESS.md (3 files for 1 workflow)
✗ COMMUNICATION_ARCHITECTURE.md + COMMUNICATION_SYSTEMS_ARCHITECTURE.md (duplicates)
```

**✅ CORRECT**:
```
✓ Single MAILBOX_SYSTEM.md (150 lines max)
✓ Single SPRINT_EXECUTION.md (100 lines max)
✓ All related concepts linked from README.md (index)
```

**Enforcement**: @process-assistant blocks duplicate concept documents with explanation.

---

### 2. Index Files Are Anti-Pattern (Hard Rule)

**❌ VIOLATION EXAMPLES**:
```
✗ Create AGENTS_INDEX.md (redundant - already in README)
✗ Create DOCUMENTATION_INDEX.md (redundant - already in README)
✗ Create -INDEX files at all (they become documentation of documentation)
```

**✅ CORRECT**:
```
✓ Table of contents in README.md
✓ Links in relevant sections
✓ Navigation handles discovery
✓ No -INDEX files
```

**Enforcement**: @process-assistant deletes -INDEX files immediately.

---

### 3. Status Documents Don't Persist (Hard Rule)

**❌ VIOLATION EXAMPLES**:
```
✗ IMPLEMENTATION_COMPLETE.md (status at point in time)
✗ SYSTEM_READY_FOR_PRODUCTION.md (status document)
✗ PROCESS_ASSISTANT_IMPLEMENTATION_SUMMARY.md (status summary)
✗ SPRINT_1_KICKOFF.md (historical status)
```

**✅ CORRECT**:
```
✓ Progress tracked in GitHub issues (linked, searchable, timestamped)
✓ Current status visible in issue comments
✓ Historical status in closed issues (git history preserved)
✓ No status files in repo
```

**Enforcement**: @process-assistant archives status files to /collaborate/archive/ immediately.

---

### 4. No Duplicate Explanations (Hard Rule)

**❌ VIOLATION EXAMPLES**:
```
✗ Explain tenant isolation in:
  - copilot-instructions-backend.md
  - ARCHITECTURE_OVERVIEW.md
  - SECURITY_PATTERNS.md
  - MULTI_TENANCY_GUIDE.md
  (Same concept in 4 places = 4x maintenance burden)
```

**✅ CORRECT**:
```
✓ Explain tenant isolation in ONE place:
  - copilot-instructions-backend.md (primary source)
  - SECURITY_PATTERNS.md (links to primary)
  - Other docs (links to primary)
```

**Enforcement**: @process-assistant requires single source of truth on PR review.

---

### 5. Strict Line Limits (Hard Rule)

| Document Type | Max Lines | Behavior |
|---|---|---|
| Process document | 150 | If > 200: split into sub-processes (separate files) |
| Agent instructions | 500 | If > 600: move patterns to role-specific file |
| Quick reference | 50 | If > 80: it's not quick (rename & expand) |
| System overview | 300 | If > 400: split into components |
| Role guide | 200 | If > 250: consolidate into agent instructions |

**Exception**: Main reference documents (copilot-instructions.md) can be 1000+ lines IF:
- Table of contents is first
- All sections are <150 lines each
- Each section links to detail files

**Enforcement**: @process-assistant flags violations in PR, requests trimming.

---

### 6. No "Learning Clutter" (Hard Rule)

**❌ VIOLATION EXAMPLES**:
```
✗ Issue #30 generates 15 "what we learned" documents
✗ Each session creates retrospective files
✗ Each agent adds "quick reference" for their understanding
✗ Multiple formats of same learning
```

**✅ CORRECT**:
```
✓ One file per sprint: /collaborate/sprint/{N}/retrospective/RETRO_{DATE}.md
✓ One file per major lesson: /collaborate/lessons-learned/{DATE}-{TOPIC}.md
✓ Git history + PR comments = source of truth for implementation details
✓ No "implementation summary" files
```

**Enforcement**: @process-assistant consolidates into single retrospective file per sprint.

---

### 7. Configuration Files < Agent Instructions (Rule)

**Purpose**: Keep configuration simple, agent instructions thorough.

**Configuration Files** (simple, <200 lines):
- `.github/agents/{name}.agent.md` - Agent definition only
- `.github/docs/processes/{name}.md` - Process steps only
- `/collaborate/README.md` - System overview only

**Agent Instructions** (comprehensive, <500 lines):
- How agent should behave
- When to use them
- Escalation paths
- Key patterns (with code)

**No New Files For**:
- ❌ Explaining a process (use comments in process file)
- ❌ Describing system architecture (use README + links)
- ❌ Documenting implementation (use git + PRs)

---

### 8. Examples Stay Minimal (Rule)

**❌ VIOLATION**:
```markdown
## Example 1
[10 lines of context]
## Example 2
[10 lines of context]
## Example 3
[10 lines of context]
# Example 4
[10 lines of context]
# Example 5
[10 lines of context]
(Total: 50 lines of examples in a 150-line document = 1/3 bloat)
```

**✅ CORRECT**:
```markdown
## Example (one good example)
[5 lines of code/output]

## Related
[Link to detailed examples if needed]
```

**Rule**: Max 2-3 examples per document, max 5 lines each.

---

## 📋 Pre-Commit Documentation Checklist

Before committing ANY documentation:

- [ ] **One Concept Rule**: This file explains ONE concept only (no duplicates in codebase)
- [ ] **Not Status**: Document doesn't contain words: "complete", "ready", "operational", "summary" (unless part of formal title)
- [ ] **Not Index**: Document isn't a -INDEX file (use README instead)
- [ ] **Line Limit**: Document respects its type's line limit (process: 150, agent: 500, etc.)
- [ ] **Minimal Examples**: Max 2-3 examples, max 5 lines each
- [ ] **One Source**: If explaining existing concept, links to primary source
- [ ] **Links Work**: All internal links point to actual files
- [ ] **Clear Title**: Title unambiguously describes content (no generic names)

---

## 🔄 Quarterly Cleanup Process

**@process-assistant runs quarterly**:

1. **Identify Bloat**:
   ```bash
   find .github/docs/processes/ -name "*.md" -exec wc -l {} \; | awk '$1 > 200'
   find .github/agents/ -name "*.md" -exec wc -l {} \; | awk '$1 > 600'
   ```

2. **Audit for Duplicates**:
   - Check for files explaining same concept
   - Find "index" or "summary" files
   - Look for status documents

3. **Consolidate**:
   - Merge duplicates
   - Remove indexes
   - Archive status documents

4. **Report**: Track metrics (total lines, file count, violations found)

---

## 📊 Metrics Dashboard

**Maintained by @process-assistant:**

| Metric | Target | Current | Status |
|--------|--------|---------|--------|
| Total documentation lines | < 7,000 | 6,650 | ✅ |
| Agent files | 15-20 | 20 | ✅ |
| Copilot instruction files | 5-6 | 5 | ✅ |
| Process files | 4-6 | 4 | ✅ |
| Process file avg lines | < 150 | 100 | ✅ |
| Agent instruction avg lines | < 500 | 325 | ✅ |
| Documentation bloat violations/month | 0 | 0 | ✅ |
| Duplicate concept files found | 0 | 0 | ✅ |

---

## 🛑 When @process-assistant Blocks Files

**@process-assistant rejects on PR** if:

1. ❌ File explains concept already documented elsewhere
2. ❌ File is purely status update (commit to issue instead)
3. ❌ File is an -INDEX variant
4. ❌ File exceeds line limit by >20%
5. ❌ File duplicates another file's purpose

**Response Template**:
```
❌ Documentation Standards Violation

Rule violated: [1-8]
Issue: [specific problem]
Correction: [what to do instead]

Remove from PR or consolidate per suggestions.
```

---

## 💡 Good vs Bad Examples

### Example 1: Process Documentation

**❌ BAD** (bloated):
```
SPRINT_EXECUTION_WORKFLOW.md (450 lines)
├─ What is sprint execution? (50 lines)
├─ Why do sprints? (30 lines)
├─ History of sprints (20 lines)
├─ Step 1 with 5 examples (80 lines)
├─ Step 2 with 4 examples (70 lines)
├─ Decision tree (100 lines)
└─ FAQ (100 lines)
```

**✅ GOOD** (minimal):
```
SPRINT_EXECUTION.md (90 lines)
├─ Title + scope
├─ 5 numbered steps (50 lines)
├─ Success criteria (20 lines)
└─ Links to related docs (20 lines)

[Examples in agent instructions, not here]
[FAQ in GitHub discussions, not here]
[History in git, not here]
```

---

### Example 2: Configuration File

**❌ BAD** (bloated):
```
.github/agents/backend-developer.agent.md (850 lines)
├─ What is backend development (50 lines)
├─ Architecture deep dive (200 lines)
├─ All patterns & anti-patterns (300 lines)
├─ FAQ from 5 retrospectives (200 lines)
├─ Future considerations (100 lines)
```

**✅ GOOD** (minimal):
```
.github/agents/backend-developer.agent.md (380 lines)
├─ Header + mission (10 lines)
├─ Responsibility (5 lines)
├─ 3-5 key patterns with code (200 lines)
├─ Escalation path (20 lines)
└─ Links to detailed guides (145 lines)

[Architecture in copilot-instructions-backend.md, not here]
[Patterns in agent instructions, not here]
```

---

## ✅ Authority Chain

| Who | Can Do | Cannot Do |
|-----|--------|-----------|
| **Any Agent** | Create PR with new doc | Commit docs directly without review |
| **@process-assistant** | Reject docs violating rules | Block good docs that are thorough |
| **@tech-lead** | Suggest documentation | Override anti-bloat rules |
| **@team** | Follow rules, keep feedback | Bypass rules with "just this once" |

---

## 🚀 Enforcement Timeline

**Effective Immediately**:
- ✅ All new PRs subject to anti-bloat rules
- ✅ @process-assistant blocks violations
- ✅ Existing docs have 30 days before enforcement (cleanup phase)

**After 30 Days**:
- ✅ Quarterly audit enforced
- ✅ All metrics tracked
- ✅ Violations in PRs rejected immediately

---

**Last Updated**: 30. Dezember 2025  
**Status**: ACTIVE & ENFORCED  
**Authority**: @process-assistant (exclusive)

---

*These rules exist because 23,432 lines of configuration became 6,650 lines by following them. Documentation should clarify, not obscure.*
