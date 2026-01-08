---
docid: STATUS-016
title: KB MCP PHASE 2 PLAN
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

---
docid: KB-MCP-PHASE-2
title: Knowledge Base MCP - Phase 2 Attachment Removal
date: 2026-01-07
status: 🚀 IN PROGRESS
---

# Phase 2: KB Attachment Removal & KB-MCP Migration

**Date**: 7. Januar 2026  
**Status**: 🚀 IN PROGRESS  
**Goal**: Remove KB attachments, migrate fully to KB-MCP

---

## 📋 Phase 2 Checklist

### Pre-Removal Validation
- [ ] KB-MCP server tested and working
- [ ] All 104 documents searchable
- [ ] Search performance <500ms
- [ ] Fallback mechanism verified

### Attachment Removal Strategy
- [ ] Remove KB-* attachments from copilot context
- [ ] Remove GL-*/WF-*/CMP-* from instructions (large ones)
- [ ] Update context loading logic
- [ ] Update prompts to use KB-MCP tools

### Documentation Updates
- [ ] Update .github/copilot-instructions.md
- [ ] Create KB-MCP transition guide
- [ ] Update agent instructions
- [ ] Update DOCUMENT_REGISTRY

### Validation
- [ ] Test Copilot context size reduction
- [ ] Verify KB searches still work
- [ ] Check token usage metrics
- [ ] Monitor for any regressions

---

## 🎯 Attachment Removal Plan

### Highest Priority (Remove First)
| File | Size | Priority | Reason |
|------|------|----------|--------|
| mcp-operations.instructions.md | ~8 KB | 🔴 HIGH | Large, referenced via KB-MCP |
| backend.instructions.md | ~5 KB | 🟡 MED | Keep minimal version |
| frontend.instructions.md | ~5 KB | 🟡 MED | Keep minimal version |

### Medium Priority
| Document | Size | Action |
|----------|------|--------|
| GL-008 (Governance) | ~3 KB | Compress to summary |
| GL-010 (Agent Org) | ~2 KB | Link to KB-MCP |
| DOCUMENT_REGISTRY | ~8 KB | Keep (needed) |

### Keep (Essential)
- copilot-instructions.md (master instructions)
- security.instructions.md (always needed)
- testing.instructions.md (always needed)
- DOCUMENT_REGISTRY.md (index)

---

## 📊 Expected Savings

### Current Context Size
- KB Articles: ~15 KB
- Instructions: ~18 KB
- Guidelines: ~8 KB
- **Total Overhead**: ~41 KB per request

### After Phase 2
- KB-MCP Query: ~0.3-2 KB
- Essential Instructions: ~8 KB
- Guidelines Summary: ~2 KB
- **Total Overhead**: ~10-12 KB per request

### Combined Savings
- Phase 1 (KB-MCP): 92% KB reduction
- Phase 2 (Attachment removal): Additional 70% overhead reduction
- **Total**: ~95% context reduction for typical KB queries 🎯

---

## 🔄 Implementation Steps

### Step 1: Create Compressed Versions
- [ ] mcp-operations → Quick Reference (2 KB)
- [ ] backend.instructions → Essentials only (1 KB)
- [ ] frontend.instructions → Essentials only (1 KB)

### Step 2: Update Copilot Instructions
- [ ] Add KB-MCP tool references
- [ ] Create tool usage examples
- [ ] Update context loading rules

### Step 3: Verify & Test
- [ ] Test KB searches
- [ ] Check context sizes
- [ ] Monitor token usage
- [ ] Validate fallback behavior

### Step 4: Document Migration
- [ ] Create transition guide
- [ ] Update DOCUMENT_REGISTRY
- [ ] Update agent instructions
- [ ] Archive old context

---

## 🛠️ Implementation Details

### 1. MCP Operations → Quick Reference

**Current**: `.github/instructions/mcp-operations.instructions.md` (8 KB)

**New**: Create compact version with tool matrix only
```markdown
# MCP Quick Reference

## Always Enabled
- security-mcp
- typescript-mcp
- vue-mcp
- database-mcp
- docker-mcp

## Tool Matrix
[Compact table of MCP tools]
[Link to KB-MCP for detailed docs]
```

**Benefit**: 
- 75% size reduction (8 KB → 2 KB)
- Full details available via `kb-mcp/search_knowledge_base query:"MCP tools"`

### 2. Update Copilot Instructions

Add to `.github/copilot-instructions.md`:

```markdown
## Knowledge Base Access

**Use KB-MCP Tools** instead of attachments:

- `kb-mcp/search_knowledge_base` - Find KB articles
- `kb-mcp/get_article` - Retrieve by DocID
- `kb-mcp/list_by_category` - Browse by topic

Example:
  kb-mcp/search_knowledge_base
    query: "Vue MCP integration"
    max_results: 5
```

### 3. Create Smart Context Loading

Update instructions to use:
```
IF kb_topic_mentioned:
  USE kb-mcp/search_knowledge_base
ELSE:
  USE local essentials only
```

---

## 📈 Metrics Before & After

### Token Usage per Request

**Before (All Attachments)**
```
Context: 41 KB
  • KB Articles: 15 KB
  • Instructions: 18 KB
  • Guidelines: 8 KB
  
Per-request overhead: 41 KB
```

**After (KB-MCP + Compressed)**
```
Context: 10-12 KB
  • KB-MCP Query: 0.3-2 KB
  • Essential Instructions: 6 KB
  • Guidelines: 4 KB
  
Per-request overhead: 10-12 KB
```

**Savings**: 75% reduction additional to Phase 1

---

## 🔐 Safety Measures

### Fallback Strategy
```python
IF kb_mcp_server_down:
  # Read directly from .ai/knowledgebase/
  load_markdown_file(docid)
```

### Validation Before Removal
- [ ] Test all 5 KB-MCP tools
- [ ] Verify <500ms response time
- [ ] Check search accuracy
- [ ] Validate category filtering

### Rollback Plan
- [ ] Keep old attachments in git history
- [ ] Version control all changes
- [ ] Can restore if needed

---

## 📅 Timeline

| Phase | Task | Duration | Status |
|-------|------|----------|--------|
| 1 | KB-MCP Setup | ✅ Done | Complete |
| 2a | Validation | 3-5 days | 🚀 Now |
| 2b | Remove Attachments | 1-2 days | Queued |
| 2c | Test & Verify | 2-3 days | Queued |
| 3 | Monitor & Optimize | Ongoing | Queued |

---

## 🎯 Success Criteria

Phase 2 is successful when:
- [ ] KB-MCP tools working reliably
- [ ] Attachments reduced by 75%
- [ ] All searches accurate
- [ ] Response time <500ms
- [ ] Zero regressions
- [ ] Team trained on KB-MCP tools

---

## 📝 Transition Guide

### For Users
1. Use `kb-mcp/search_knowledge_base` for KB lookups
2. Reference articles by DocID (KB-053, ADR-001)
3. Browse categories with `list_by_category`

### For Agents
1. Update tool usage in prompts
2. Reference KB-MCP instead of attachments
3. Use quick searches instead of full article loads

### For Developers
1. Keep .ai/knowledgebase/ updated
2. Run `python3 tools/KnowledgeBaseMCP/scripts/build_index.py` after updates
3. Monitor KB-MCP server status

---

## 📊 Phase 2 Status Dashboard

```
Validation:      ████░░░░░░ 40%
Removal:         ░░░░░░░░░░  0%
Testing:         ░░░░░░░░░░  0%
Documentation:   ░░░░░░░░░░  0%
─────────────────────────────
Overall:         ████░░░░░░ 10%
```

---

## 🔗 Related Documents

- [KB-MCP-PHASE-1-STATUS.md](KB-MCP-PHASE-1-STATUS.md) - Phase 1 Summary
- [.github/copilot-instructions.md](../../.github/copilot-instructions.md) - Master Instructions
- [tools/KnowledgeBaseMCP/README.md](../../tools/KnowledgeBaseMCP/README.md) - KB-MCP Docs

---

**Started**: 7. Januar 2026  
**Expected Completion**: 14. Januar 2026  
**Owner**: @CopilotExpert

