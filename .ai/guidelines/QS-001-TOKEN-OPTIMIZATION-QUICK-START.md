---
docid: GL-092
title: QS 001 TOKEN OPTIMIZATION QUICK START
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

---
docid: QS-001
title: Token Optimization Quick Start - For Agents NOW
owner: "@SARAH"
status: Active
created: "2026-01-07"
---

# ⚡ Quick Start: Token Optimization (Read This First!)

**Time to Read**: 5 minutes  
**Time to Implement**: 0 minutes (start immediately!)  
**Impact**: 70-80% token reduction per interaction

---

## 🎯 TL;DR - Do This NOW

### Rule 1: Load ONLY Path-Specific Instructions (GL-043)

**BEFORE** ❌:
```
Attach ALL 6 instruction files (18 KB = 6,000 tokens)
```

**NOW** ✅:
```
Working on: src/components/ProductCard.vue
Attach: frontend-essentials.md + security.md ONLY (3.1 KB = 1,000 tokens)

Working on: backend/Domain/Catalog/...
Attach: backend-essentials.md + security.md ONLY (3.2 KB = 1,000 tokens)

Working on: **/*.test.*
Attach: testing.md + security.md ONLY (10 KB = 3,300 tokens)

Working on: .github/**
Attach: devops.md + security.md ONLY (3.5 KB = 1,150 tokens)
```

**Savings**: ~5,000 tokens per interaction

---

### Rule 2: Find Before You Read (GL-044)

**BEFORE** ❌:
```
read_file("LargeFile.cs", 1, 250)
= 8,300 tokens (whole file!)
```

**NOW** ✅:
```
Step 1: Find the location
  grep_search("CreateProduct", isRegexp: false)
  → Returns: lines 42-95 exist

Step 2: Read ONLY those lines + context
  read_file("LargeFile.cs", 37, 100)
  = 2,100 tokens
  
SAVINGS: 6,200 tokens (75%)
```

**Pattern**: `grep → read(targeted)` instead of `read(full)`

---

### Rule 3: Query KB, Don't Attach (GL-045)

**BEFORE** ❌:
```
Attach KB-006 (8 KB) + KB-053 (7 KB) = 5,000 tokens
"Just in case I need patterns"
```

**NOW** ✅:
```
When you need knowledge, QUERY:
  kb-mcp/search_knowledge_base
    query: "Wolverine handler with validation pattern"
    category: "patterns"
  = 1,500 tokens
  
Then use the answer immediately!
NO pre-loading!
```

**Savings**: 3,500 tokens per interaction

---

## 🚀 START TODAY - 3 Simple Steps

### Step 1: Path-Specific Attachments (Takes 5 seconds)

Before you start work, check your file path:

```
IF path contains "components/" OR "frontend/"
  → Attach: frontend-essentials.md + security.md

ELSE IF path contains "api/" OR "backend/"
  → Attach: backend-essentials.md + security.md

ELSE IF path contains "test" OR ".spec" OR ".test"
  → Attach: testing.md + security.md

ELSE IF path contains ".github/" OR "Dockerfile"
  → Attach: devops.md + security.md

ALWAYS attach: security.md
```

**That's it. Start doing this now.**

---

### Step 2: Grep First, Read Targeted (Takes 10 seconds)

When you need to read code:

```
1. Search for what you need:
   grep_search("FunctionName", isRegexp: false)
   
2. See the line numbers in results
   → Let's say lines 42-95

3. Read ONLY that section + context:
   read_file("file.cs", 37, 100)
   
DON'T: read_file("file.cs", 1, 500)
```

**That's it. Do this from now on.**

---

### Step 3: Query KB When Needed (Takes 10 seconds)

When you need knowledge:

```
NEED: "How to implement Wolverine handler pattern?"

DO THIS:
  kb-mcp/search_knowledge_base
    query: "Wolverine handler pattern with IResult"
    category: "patterns"
    max_results: 3

DON'T: "Let me attach KB-006 just to be safe"
```

**That's it. Query instead of attach.**

---

## ✅ Checklist for Every Task

- [ ] **File Path**: What am I working on?
  - Check path → Load ONLY relevant instructions
  - Always load security.md
  
- [ ] **Code Reading**: Do I need to read code?
  - Use grep_search first
  - Read only targeted range
  - Include ±5 lines context
  
- [ ] **Knowledge**: Do I need external knowledge?
  - Query kb-mcp instead of attaching
  - Specific questions only
  - Use query, don't pre-load

- [ ] **Attachments**: What am I really attaching?
  - frontend-essentials + security? (3.1 KB)
  - backend-essentials + security? (3.2 KB)
  - testing + security? (10 KB)
  - devops + security? (3.5 KB)
  - NO KB articles pre-loaded!

---

## 📊 Expected Results (Per Interaction)

### Before (Old Way)
```
Instruction files:    6,000 tokens
KB articles:          3,000 tokens
File reads:           3,000 tokens
Overhead:            12,000 tokens ❌
```

### After (New Way)
```
Instruction files:    1,000 tokens (path-specific)
KB queries:           1,500 tokens (on-demand)
File reads:           1,000 tokens (grep + fragment)
Overhead:             3,500 tokens ✅
SAVINGS:              8,500 tokens (71%) 🎉
```

---

## 🎯 Common Scenarios

### Scenario 1: "Fix bug in backend service"

```
1. File path: backend/Domain/Catalog/...
   → Attach: backend-essentials.md + security.md

2. Find bug location:
   → grep_search("ProductQuery")
   → Returns lines 42-95

3. Read code:
   → read_file("Service.cs", 37, 100)

4. Need Wolverine pattern?
   → kb-mcp/search_knowledge_base "handler pattern"

✓ Done efficiently!
```

---

### Scenario 2: "Build Vue component"

```
1. File path: src/components/ProductCard.vue
   → Attach: frontend-essentials.md + security.md

2. Need pattern?
   → kb-mcp/search_knowledge_base "Vue composition patterns"
   → NOT: Attach KB-007 (wastes tokens!)

3. Understand responsive design?
   → semantic_search("responsive Vue patterns")
   → NO file reads needed!

✓ Done efficiently!
```

---

### Scenario 3: "Security review of API"

```
1. File path: backend/Gateway/Controllers/OrderController.cs
   → Attach: backend-essentials.md + security.md

2. Find validation code:
   → grep_search("Validate", isRegexp: false)
   → Returns lines 50-80, 120-140

3. Read both sections:
   → read_file("Controller.cs", 45, 85)
   → read_file("Controller.cs", 115, 145)

4. Need OWASP guidance?
   → kb-mcp/search_knowledge_base "OWASP input validation"

✓ Done efficiently!
```

---

## 🚨 IMPORTANT: What NOT To Do

❌ **DON'T**:
```
- Attach all 6 instruction files (wastes 6,000 tokens)
- Read entire large files (wastes 5,000+ tokens)
- Pre-load KB articles "just in case" (wastes 3,000+ tokens)
- Use read_file(1, 500) for big files (inefficient!)
- Load mcp-quick-reference unless discussing MCPs
```

✅ **DO**:
```
- Attach ONLY path-specific instructions (saves 5,000 tokens)
- grep → targeted read (saves 6,200 tokens)
- Query KB on-demand (saves 3,500 tokens)
- Use semantic_search for patterns (saves 8,000 tokens)
- Keep attachments minimal and focused
```

---

## 📚 Read These If You Want Details

| Document | When | Time |
|----------|------|------|
| [GL-043] Smart Attachments | Understand path-specific loading | 10 min |
| [GL-044] Fragment-Based Access | Learn grep + targeted read patterns | 10 min |
| [GL-045] KB-MCP Queries | Understand query strategy | 10 min |
| [GL-047] MCP-Orchestration | See full decision tree | 15 min |
| [WF-009] Execution Plan | Understand full timeline | 15 min |

**But you don't need to read them to START!** Just follow the 3 rules above.

---

## 🎯 YOUR MISSION

**Starting NOW (Jan 7, 2026)**:

1. ✅ Read this Quick Start (5 min)
2. ✅ Apply Rule 1: Path-specific attachments (immediate)
3. ✅ Apply Rule 2: Grep before read (immediate)
4. ✅ Apply Rule 3: Query KB on-demand (immediate)
5. ✅ Keep checklist handy for every task

**That's all you need to do!**

---

## 💬 Quick Reference Card

Print this and keep it handy:

```
╔════════════════════════════════════════════════════╗
║ TOKEN OPTIMIZATION QUICK RULES                    ║
╠════════════════════════════════════════════════════╣
║ RULE 1: Path-Specific Attachments                ║
║  → frontend → frontend-essentials + security     ║
║  → backend → backend-essentials + security       ║
║  → test → testing + security                     ║
║  → devops → devops + security                    ║
╠════════════════════════════════════════════════════╣
║ RULE 2: Grep Before Read                         ║
║  → grep_search("term") → find location           ║
║  → read_file(targetLine, targetLine+60)          ║
║  → Always include ±5 lines context               ║
╠════════════════════════════════════════════════════╣
║ RULE 3: Query KB, Don't Attach                   ║
║  → kb-mcp/search_knowledge_base query: "topic"   ║
║  → NO pre-loading articles                       ║
║  → Query when you need knowledge                 ║
╚════════════════════════════════════════════════════╝
```

---

## ✨ Expected Impact (Your Work)

- **Faster responses** (smaller context = faster processing)
- **More budget** (70-80% tokens freed up for other work)
- **Better quality** (focused context = better answers)
- **No rate limiting** (token usage under control)

---

## 📞 Questions?

- **How do I do grep_search?** → See [GL-044]
- **How do I query KB?** → See [GL-045]
- **Why path-specific?** → See [GL-043]
- **What's the full plan?** → See [WF-009]
- **Unsure?** → Ask @SARAH

---

## 🎉 You're Ready!

Everything you need to know to **start immediately** is above.

**Go implement these 3 rules NOW. Don't wait for Jan 9.**

The sooner you start, the sooner we get 80% token reduction! 🚀

---

**Created**: 7. Januar 2026  
**For**: All Agents  
**Status**: Ready to Use NOW  
**Next Review**: Jan 10, 2026
