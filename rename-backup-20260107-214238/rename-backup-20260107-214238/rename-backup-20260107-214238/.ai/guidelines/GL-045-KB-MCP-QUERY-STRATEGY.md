---
docid: GL-045
title: KB-MCP Query Strategy - Dynamic Knowledge Loading
owner: "@SARAH"
status: Active
created: "2026-01-07"
---

# GL-045: KB-MCP Query Strategy - Dynamic Knowledge Loading

**Estimate**: 60% Token Savings | **Effort**: ⭐⭐ Medium

## Purpose

Minimize token consumption by **querying knowledge on-demand via KB-MCP** instead of pre-loading large KB articles as attachments.

---

## 🎯 Core Principle

**BEFORE (Wasteful)**:
```yaml
# Attach entire KB articles "just in case"
attachments:
  - KB-006-wolverine.md              # 8 KB
  - KB-001-csharp-14.md              # 6 KB
  - KB-052-roslyn-mcp.md             # 5 KB
  - KB-053-typescript-mcp.md         # 7 KB
# Total: 26 KB loaded upfront
# Tokens: ~8,600 tokens before work even starts!
```

**AFTER (Optimized)**:
```yaml
# Query knowledge ONLY when needed
workflow:
  1. Start work without KB attachments
  2. When you need info: kb-mcp/search_knowledge_base
  3. Get targeted answer: ~1,500 tokens
  4. Continue work with fresh context
# Total: Only load what you actually need!
```

---

## 📊 Size Comparison

| Scenario | Old Way | New Way | Savings |
|---|---|---|---|
| Working on Wolverine handler | Attach KB-006 (8 KB) | Query kb-mcp/search_knowledge_base | **~2,600 tokens** (85%) |
| TypeScript refactoring | Attach KB-053 (7 KB) | Query kb-mcp/search_knowledge_base | **~2,300 tokens** (82%) |
| Security review | Attach KB-055 (6 KB) | Query kb-mcp/search_knowledge_base | **~2,000 tokens** (80%) |
| Multiple topics | Attach KB-006 + KB-053 + KB-055 (21 KB) | 3× targeted queries (~4,500) | **~16,500 tokens** (79%) |

---

## 🔍 When to Use KB-MCP Queries

### ✅ Use KB-MCP When:
- You're about to do something specific
- You need targeted knowledge (not overview)
- Knowledge might change frequently
- Question is well-defined

**Example**:
```bash
# YOU: "I need to understand how to implement Wolverine handlers with validation"
# QUERY:
kb-mcp/search_knowledge_base
  query: "Wolverine handler with input validation pattern"
  category: "patterns"
  max_results: 3
# RESULT: ~1,500 tokens, exact answer with code examples
```

### ❌ Keep Attachments Only When:
- You'll reference the entire document repeatedly
- Document is fundamental (GL-006, GL-043, GL-044)
- Working in path-specific mode (already covered by GL-043)
- Document size < 2 KB (minimal token cost)

---

## 🔄 Three Strategies for Different Situations

### Strategy 1: Project-Specific Knowledge

**Scenario**: Need B2X-specific patterns

**Query Pattern**:
```bash
kb-mcp/search_knowledge_base
  query: "multi-tenant domain pattern with Wolverine CQRS"
  category: "architecture"

# Returns: KB-012, KB-006, relevant ADRs
# Tokens: ~1,500
```

**vs. Attachment** (20 KB, 6,600 tokens) = **77% savings**

---

### Strategy 2: Technology Deep Dive

**Scenario**: Need to understand feature (e.g., Vue.js i18n)

**Query Pattern**:
```bash
kb-mcp/search_knowledge_base
  query: "Vue.js 3 composition API with i18n integration"
  category: "tools"

# Returns: KB-007, KB-042, code examples
# Tokens: ~1,500
```

**vs. Multiple Attachments** (15 KB, 5,000 tokens) = **70% savings**

---

### Strategy 3: Best Practices Reference

**Scenario**: Need to follow pattern for new feature

**Query Pattern**:
```bash
kb-mcp/search_knowledge_base
  query: "email template best practices with dark mode support"
  category: "best-practices"

# Returns: KB-023, KB-027, implementation guide
# Tokens: ~1,500
```

**vs. Attachment** (8 KB, 2,600 tokens) = **42% savings**

---

## 📋 KB-MCP Query Quick Reference

### Command Structure
```bash
kb-mcp/search_knowledge_base
  query: "[natural language question]"
  category: "tools|patterns|architecture|best-practices"  # optional
  max_results: 3-5  # optional
```

### Query Formulation Tips

**❌ Vague**:
```bash
kb-mcp/search_knowledge_base query: "Wolverine"
# Returns: Too many results, 2,500+ tokens
```

**✅ Specific**:
```bash
kb-mcp/search_knowledge_base
  query: "Wolverine handler with IResult return pattern and validation"
  max_results: 3
# Returns: 3 most relevant KB articles, ~1,500 tokens
```

---

### Common Queries by Domain

#### Backend (.NET/Wolverine)
```bash
# Wolverine Patterns
kb-mcp/search_knowledge_base
  query: "Wolverine CQRS handler pattern with middleware"
  category: "patterns"

# .NET Features
kb-mcp/search_knowledge_base
  query: ".NET 10 C# 14 record types and init-only properties"
  category: "tools"

# Architecture
kb-mcp/search_knowledge_base
  query: "multi-tenant domain isolation with shared catalogs"
  category: "architecture"
```

#### Frontend (Vue.js)
```bash
# Vue.js Patterns
kb-mcp/search_knowledge_base
  query: "Vue 3 composition API with Pinia state management"
  category: "patterns"

# i18n Strategy
kb-mcp/search_knowledge_base
  query: "token-optimized internationalization with fallback"
  category: "best-practices"

# Responsive Design
kb-mcp/search_knowledge_base
  query: "responsive design patterns for mobile-first Vue components"
  category: "patterns"
```

#### Security
```bash
# Input Validation
kb-mcp/search_knowledge_base
  query: "OWASP Top Ten input validation and sanitization patterns"
  category: "best-practices"

# Specific Vulnerabilities
kb-mcp/search_knowledge_base
  query: "SQL injection prevention with parameterized queries"
  category: "best-practices"
```

#### Infrastructure
```bash
# Docker Security
kb-mcp/search_knowledge_base
  query: "Docker container security best practices and hardening"
  category: "best-practices"

# Kubernetes
kb-mcp/search_knowledge_base
  query: "Kubernetes multi-tenant isolation with network policies"
  category: "architecture"
```

---

## 🎯 Integration with GL-043 & GL-044

### Combined Token Optimization Strategy

```
┌─────────────────────────────────────────────┐
│ GL-043: Smart Attachments                   │
│ Load ONLY path-specific instructions        │
│ Saves: 50-70% on instruction files          │
└──────────────────┬──────────────────────────┘
                   │
                   ↓
┌─────────────────────────────────────────────┐
│ GL-044: Fragment-Based File Access          │
│ Read targeted ranges, not full files        │
│ Saves: 40% on file reads                    │
└──────────────────┬──────────────────────────┘
                   │
                   ↓
┌─────────────────────────────────────────────┐
│ GL-045: KB-MCP Query Strategy (THIS)        │
│ Query knowledge on-demand, not pre-loaded   │
│ Saves: 60% on knowledge loading             │
└──────────────────┬──────────────────────────┘
                   │
                   ↓
         TOTAL: 70-80% Token Savings! 🚀
```

---

## 📊 Example Workflow: Complex Feature Implementation

### Scenario: Add new ERP connector feature

**Old Way** (All KB articles as attachments):
```yaml
attachments:
  - KB-012-repo-mapping.md              # 6 KB
  - KB-006-wolverine.md                 # 8 KB
  - KB-028-erp-integration.md           # 10 KB
  - KB-055-security-mcp.md              # 6 KB
  - ADR-034-multi-erp-architecture.md   # 5 KB
Total: 35 KB = 11,600 tokens
Plus: File reads, instruction files, etc.
GRAND TOTAL: ~18,000 tokens before work!
```

**New Way** (KB-MCP queries on-demand):
```yaml
# Phase 1: Architecture (~1,500 tokens)
kb-mcp/search_knowledge_base
  query: "multi-ERP connector architecture with shared components"
  category: "architecture"

# Phase 2: Wolverine patterns (~1,500 tokens)
kb-mcp/search_knowledge_base
  query: "Wolverine handler for external API integration with retry"
  category: "patterns"

# Phase 3: Security (~1,500 tokens)
kb-mcp/search_knowledge_base
  query: "API key management and secrets in connector integrations"
  category: "best-practices"

# Phase 4: ERP-specific (~1,500 tokens)
kb-mcp/search_knowledge_base
  query: "enventa Trade ERP API endpoints and data mapping"
  category: "tools"

Total: ~6,000 tokens = 68% savings! ✅
```

---

## ⚡ Quick Reference: When to Query

| Situation | Query | Tokens |
|---|---|---|
| Need architecture info | kb-mcp/search... "architecture" | 1,500 |
| Need pattern example | kb-mcp/search... "pattern" | 1,500 |
| Need best practice | kb-mcp/search... "best-practices" | 1,500 |
| Need tool reference | kb-mcp/search... "tools" | 1,500 |
| Need specific KB article | kb-mcp/get_article docid:"KB-006" | 2,000 |
| Browse category | kb-mcp/list_by_category category:"patterns" | 1,200 |

---

## 🔧 Implementation Workflow

### For Every New Task:

**Step 1: Prepare** (5 min)
```
✅ Attach ONLY path-specific instructions (GL-043)
✅ NO KB article attachments yet
✅ NO "just in case" documents
```

**Step 2: Gather Knowledge** (5 min)
```
🔍 As questions arise, use kb-mcp/search_knowledge_base
❓ "I need to understand X" → Query instead of reading document
📖 Get targeted answer in <2 minutes
```

**Step 3: Work** (main task)
```
💻 Implementation with just-in-time knowledge
📚 Reference specific KB articles via DocID when needed
🎯 Minimal context overhead
```

**Step 4: Review** (5 min)
```
🔒 Use security-related queries if needed
📋 Reference guidelines as you go
```

---

## 📈 Expected Token Reduction

| Phase | Savings | Notes |
|---|---|---|
| **GL-043** (Smart Attachments) | 50-70% | Path-specific only |
| **GL-044** (Fragment Access) | 40% | Targeted file reads |
| **GL-045** (KB-MCP Queries) | 60% | On-demand knowledge |
| **Combined** | **70-80%** | Applied together! |

---

## ✅ Checklist: KB-MCP Adoption

Before starting work:
- [ ] Do I have all instructions attached? (GL-043 path-specific)
- [ ] Have I attached KB articles? (If yes, reconsider!)
- [ ] Do I know my first question? (If yes, prepare query)
- [ ] Am I ready to use kb-mcp/search_knowledge_base? (If yes, go!)

During work:
- [ ] Do I need knowledge? → Query kb-mcp, don't attach
- [ ] Do I need code context? → Use grep/semantic_search (GL-044)
- [ ] Do I need examples? → Query kb-mcp/search_knowledge_base

---

## 🚀 Migration Timeline

**Week 1 (Jan 7-13)**:
- [ ] All agents stop pre-attaching KB articles
- [ ] Use kb-mcp/search_knowledge_base for queries

**Week 2 (Jan 14-20)**:
- [ ] Update conversation patterns in agent definitions
- [ ] Monitor query effectiveness

**By Jan 27**:
- [ ] **60% average reduction** in knowledge attachment size
- [ ] **Combined 70-80%** with GL-043 + GL-044
- [ ] Significant rate-limit relief

---

## 🎓 Examples by Role

### @Backend Developer
```bash
# Starting new Wolverine handler
kb-mcp/search_knowledge_base
  query: "Wolverine handler with async IResult pattern and error handling"
  category: "patterns"
# Instead of: Attaching KB-006 (8 KB)
# Saves: 2,600 tokens
```

### @Frontend Developer
```bash
# Building new Vue component
kb-mcp/search_knowledge_base
  query: "Vue 3 composition API with reactive state and lifecycle hooks"
  category: "patterns"
# Instead of: Attaching KB-007 (7 KB)
# Saves: 2,300 tokens
```

### @Security Reviewer
```bash
# Reviewing input validation
kb-mcp/search_knowledge_base
  query: "input validation allowlist patterns and SQL injection prevention"
  category: "best-practices"
# Instead of: Attaching KB-010 (6 KB)
# Saves: 2,000 tokens
```

---

## 💾 Knowledge Base Structure (Quick Reference)

**Main categories for querying**:
- `tools` - Technology references (Vue, .NET, Wolverine, etc.)
- `patterns` - Design and architectural patterns
- `architecture` - System design and ADRs
- `best-practices` - Security, performance, coding standards

**Search tips**:
- Be specific in query
- Use technical terms
- Mention patterns or practices
- Include version/technology if relevant

---

**Maintained by**: @SARAH  
**Last Updated**: 7. Januar 2026  
**Expected Benefit**: 60% token reduction from KB attachment optimization
