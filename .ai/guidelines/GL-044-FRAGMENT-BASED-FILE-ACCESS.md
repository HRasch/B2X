---
docid: GL-080
title: GL 044 FRAGMENT BASED FILE ACCESS
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

---
docid: GL-044
title: Fragment-Based File Access Strategy
owner: "@SARAH"
status: Active
created: "2026-01-07"
---

# GL-044: Fragment-Based File Access Strategy

**Estimate**: 40% Token Savings | **Effort**: ⭐ Small

## Purpose

Minimize token consumption by reading **specific file fragments** instead of loading entire files, and choosing the right tool for each access pattern.

---

## 🎯 Core Rule

**BEFORE (Wasteful)**:
```bash
read_file("backend/Domain/Catalog/Application/UseCases/ProductQuery.cs", 1, 500)
# Loads: All 500 lines even if only need lines 50-100
```

**AFTER (Optimized)**:
```bash
# Option 1: Grep for specific section
grep_search("public class ProductQuery", includePattern="UseCases/**")

# Option 2: Read only needed lines
read_file("...ProductQuery.cs", startLine: 45, endLine: 65)

# Option 3: Semantic search for concept
semantic_search("ProductQuery handler pattern")
```

---

## 📊 Tool Selection Matrix

| Situation | Tool | Reason | Token Savings |
|---|---|---|---|
| **Looking for specific function/class** | `grep_search` | Finds location, no full file load | **60%** |
| **Need definition + context** | `read_file` with targeted lines | Precise range | **50%** |
| **Searching concept across files** | `semantic_search` | One-shot, no file iteration | **70%** |
| **Analyzing large file structure** | `grep_search` with multiple patterns | Pattern-based, fast | **65%** |
| **Need exact line numbers** | `list_code_usages` | Find all references | **75%** |
| **Full file needed** (rare) | `read_file` full range | Only when truly necessary | **0%** |

---

## 🔍 Pattern Examples

### ❌ ANTI-PATTERN: Reading Entire File

```bash
# DON'T: Load entire 200-line file
read_file("src/services/CatalogService.cs", 1, 200)
# Cost: ~66 tokens per line × 200 = ~13,200 tokens
```

### ✅ PATTERN 1: Grep for Specific Function

```bash
# DO: Find function location first
grep_search("CreateProduct", includePattern="**/*Service.cs", isRegexp: false)
# Cost: ~500 tokens, returns line numbers

# Then read only that function
# Assume result: lines 45-80
read_file("src/services/CatalogService.cs", 45, 80)
# Cost: ~1,200 tokens
# Total: ~1,700 tokens instead of 13,200 (87% savings!)
```

### ✅ PATTERN 2: Semantic Search for Concept

```bash
# DO: Search for pattern concept
semantic_search("product creation command handler pattern")
# Cost: ~1,000 tokens, returns relevant snippets
# No file reading needed!

# Result: Already has code context
# Total: ~1,000 tokens (vs. multiple file reads = 90% savings!)
```

### ✅ PATTERN 3: List Code Usages

```bash
# DO: Find where function is used
list_code_usages("CreateProductCommand", filePaths: ["backend/Domain/Catalog"])
# Cost: ~800 tokens, returns all usages with context

# No need to read entire files!
# Total: ~800 tokens vs. 5+ full file reads (90% savings!)
```

### ✅ PATTERN 4: Grep + Targeted Read

```bash
# DO: Multi-step targeted approach
# Step 1: Find all class definitions
grep_search("public (class|record|interface)", includePattern="**/*.cs", isRegexp: true)
# Cost: ~600 tokens, returns all locations

# Step 2: Read only relevant ones
read_file("file1.cs", 10, 30)  # Constructor
read_file("file1.cs", 45, 65)  # Main method
# Cost: ~1,200 tokens
# Total: ~1,800 tokens vs. reading all files (80% savings!)
```

---

## 📋 Decision Tree

```
START: Need to access file content?
│
├─ Looking for specific NAME (function/class/variable)?
│  └─ Use: grep_search()
│     → Returns line numbers, read only those lines
│     → Pattern: grep_search → read_file (targeted)
│
├─ Need concept/pattern across codebase?
│  └─ Use: semantic_search()
│     → Returns relevant snippets with context
│     → Pattern: semantic_search → (maybe no read_file needed!)
│
├─ Need all usages/references of symbol?
│  └─ Use: list_code_usages()
│     → Returns all references with context
│     → Pattern: list_code_usages → (maybe no read_file needed!)
│
├─ Analyzing file structure (imports, class hierarchy)?
│  └─ Use: grep_search() with patterns
│     → Pattern: grep_search for patterns
│
└─ Need exact context in specific line range?
   └─ Use: read_file(startLine, endLine)
      → Specific range only
      → Pattern: grep_search to find range, then read_file
```

---

## 🚀 Quick Reference Patterns

### Pattern A: "Find and Read"
```bash
# Goal: Understand a specific function

# Step 1: Locate
grep_search("ProductQueryHandler", isRegexp: false)
# Returns: Lines 42-95

# Step 2: Read context
read_file("Application/Handlers/ProductQueryHandler.cs", 35, 100)
# Includes 5 lines before for context, 5 lines after

# Total tokens: grep (~500) + read (~2,000) = 2,500
# vs. full file read (15,000+) = 83% savings
```

### Pattern B: "Concept Search"
```bash
# Goal: Find all handler patterns

# One-shot: Semantic search
semantic_search("Wolverine handler with validation pattern")
# Returns: All relevant handlers with line numbers and snippets

# Total tokens: ~1,500
# vs. grepping + reading 10 files (20,000+) = 93% savings
```

### Pattern C: "Analyze Structure"
```bash
# Goal: Find all public methods in class

# Use grep with pattern
grep_search("public (async )?.*\(", includePattern="MyClass.cs", isRegexp: true)
# Returns: All method signatures with line numbers

# Then read each if needed
# Total tokens: grep (~600) + selective reads (~3,000) = 3,600
# vs. full file read (10,000+) = 64% savings
```

### Pattern D: "Find Usage"
```bash
# Goal: See how function is used elsewhere

# Direct: List usages
list_code_usages("ProductQuery", filePaths: ["backend/Domain/Catalog"])
# Returns: All usages with context

# Total tokens: ~1,000
# vs. grep + read multiple files (5,000+) = 80% savings
```

---

## 📏 When to Use read_file() Full Range

| Scenario | Example | Tokens | Note |
|---|---|---|---|
| **Small file** | <50 lines | <2,000 | OK to read full |
| **Need overview** | Architecture decision file | OK | One-time context |
| **Critical section** | Security validation code | OK | Must be complete |
| **Other cases** | (rare) | Use fragment methods | (90% of situations) |

---

## 🧠 Common Mistakes to Avoid

### ❌ Mistake 1: Reading Too Much
```bash
# WRONG: "Give me lines 1-500 to understand the handler"
read_file("EventHandler.cs", 1, 500)

# RIGHT: "Where is CreateEventHandler defined?"
grep_search("CreateEventHandler", isRegexp: false)
# Then: "Show me that function"
read_file("EventHandler.cs", 42, 68)
```

### ❌ Mistake 2: Ignoring semantic_search
```bash
# WRONG: Multiple grep searches + file reads
grep_search("pattern1")
grep_search("pattern2")
grep_search("pattern3")
read_file(file1, ...)
read_file(file2, ...)
# Total: ~8,000 tokens

# RIGHT: One semantic search
semantic_search("Find all places where pattern is implemented")
# Total: ~1,500 tokens (82% savings!)
```

### ❌ Mistake 3: Not using list_code_usages
```bash
# WRONG: Grep for each reference manually
grep_search("MyFunction", isRegexp: false)
grep_search("MyFunction", includePattern: "handlers/**", isRegexp: false)
grep_search("MyFunction", includePattern: "tests/**", isRegexp: false)

# RIGHT: One list_code_usages call
list_code_usages("MyFunction", filePaths: ["backend"])
# All results with context, no file reading needed!
```

### ❌ Mistake 4: Over-specifying read ranges
```bash
# WRONG: Include entire 200-line file for one method
read_file("Service.cs", 1, 200)

# RIGHT: Target the range
# Grep first: "public Create" found at line 42
read_file("Service.cs", 37, 62)  # 5 lines before/after for context
```

---

## ✅ Checklist: Fragment-Based Access

Before using `read_file()` full range, ask:

- [ ] Do I need the **entire file**, or specific parts?
- [ ] Have I tried `grep_search()` to **locate** first?
- [ ] Could `semantic_search()` **answer directly** without file reading?
- [ ] Does `list_code_usages()` **find all references** I need?
- [ ] Is the file < 50 lines? (If yes, full read OK)
- [ ] Am I reading **only the targeted range**?

---

## 📊 Token Savings Examples

### Example 1: Find Bug in Large Service

**Scenario**: Need to trace ProductQuery issue

**Old Way** (Read full file):
```
read_file("ProductQueryService.cs", 1, 250)
Tokens: ~8,300
```

**New Way** (Fragment-based):
```
grep_search("ProductQuery", isRegexp: false)
# Returns: Lines 42-95, 156-180, 210-235

read_file("ProductQueryService.cs", 40, 100)   # 2,000 tokens
read_file("ProductQueryService.cs", 150, 190)  # 1,300 tokens
read_file("ProductQueryService.cs", 205, 240)  # 1,200 tokens

Total: 4,500 tokens
Savings: 46% ✅
```

### Example 2: Understand Handler Pattern

**Scenario**: Need to understand Wolverine handler pattern

**Old Way** (Multiple file reads):
```
read_file("Handler1.cs", 1, 100)   # 3,300 tokens
read_file("Handler2.cs", 1, 100)   # 3,300 tokens
read_file("Handler3.cs", 1, 100)   # 3,300 tokens
Total: 9,900 tokens
```

**New Way** (Semantic search):
```
semantic_search("Wolverine handler with IResult return pattern")
# Returns: All handlers with snippets and line references
Total: 1,500 tokens
Savings: 85% ✅
```

### Example 3: Find All Method Usages

**Scenario**: Need to see where `CreateProduct` is called

**Old Way** (Grep + multiple reads):
```
grep_search("CreateProduct")
# Returns: 15 different locations

read_file(...) × 15 locations  # ~25,000 tokens
```

**New Way** (list_code_usages):
```
list_code_usages("CreateProduct", filePaths: ["backend"])
# Returns: All usages with context
Total: 1,200 tokens
Savings: 95% ✅
```

---

## 🔄 Integration with Other Guidelines

- **GL-043 (Smart Attachments)**: Reduces initial context
- **GL-044 (Fragment Access)**: Reduces per-query file reads
- **GL-006 (Token Optimization)**: Overall strategy umbrella

Combined effect: **60-80% token reduction** possible! 🚀

---

## 📈 Implementation Timeline

**Week 1 (Jan 7-13)**:
- [ ] All agents adopt fragment-based access for new work
- [ ] Prefer `grep_search()` → `read_file(targeted)` pattern

**Week 2 (Jan 14-20)**:
- [ ] Update agent behavior guidelines to emphasize tools
- [ ] Monitor pattern usage in conversations

**By Jan 27**:
- [ ] **40% average token reduction** from file access optimization
- [ ] **Combined 80%+ reduction** with GL-043 + GL-044

---

## 🎯 Tool-Specific Tips

### `grep_search()`
- Use `isRegexp: true` for complex patterns
- Include `includePattern` to narrow scope
- Returns line numbers automatically

### `semantic_search()`
- Best for "find where this pattern is used"
- Returns code snippets + context (no additional read_file needed!)
- Slower than grep but finds conceptual matches

### `list_code_usages()`
- ALWAYS use for finding function/variable references
- Returns all usages with context (complete answer)
- Replaces multiple grep_search calls

### `read_file(startLine, endLine)`
- Always specify exact line range
- Include 3-5 lines context before/after
- Minimize to targeted range only

---

**Maintained by**: @SARAH  
**Last Updated**: 7. Januar 2026  
**Expected Benefit**: 40% token reduction from optimized file access
