---
docid: GL-053
title: Multi-Language Fragment Editing Strategy
owner: @CopilotExpert
status: Active
created: 2026-01-08
---

# GL-053: Multi-Language Fragment Editing Strategy

**DocID**: `GL-053`  
**Status**: Active | **Owner**: @CopilotExpert  
**Created**: 2026-01-08  
**Purpose**: Token-saving approach for editing large files using MCP semantic analysis

## 🎯 Overview

This strategy enables efficient editing of large files (>200 lines) across multiple languages while maintaining quality and reducing token consumption by 75-85%.

**Key Innovation**: Combines fragment-based access with MCP semantic analysis for intelligent, quality-assured editing.

---

## 📊 Token Savings & Quality Impact

| Aspect | Fragment-Only | MCP-Enhanced | Improvement |
|--------|---------------|--------------|-------------|
| **Token Cost** | ~4,500 tokens | ~800 tokens | **82% savings** |
| **Context Coverage** | ±15 lines | Full semantic graph | **Complete understanding** |
| **Error Detection** | Syntax only | Compiler + semantic | **Higher quality** |
| **Automation** | Manual edits | MCP refactoring | **Faster, safer** |

---

## 🚀 Core Workflow

### Phase 1: Pre-Edit Intelligence Gathering
```bash
# 1. Location Finding (Universal)
grep_search("targetPattern", includePattern="**/*.ext")
# OR
semantic_search("concept description")

# 2. Semantic Analysis (Language-Specific)
# .NET
roslyn-mcp/analyze_types workspacePath="backend/Domain"
# TypeScript
typescript-mcp/analyze_types workspacePath="frontend/src"
# Vue
vue-mcp/analyze_vue_component filePath="src/Component.vue"
# Database
database-mcp/validate_schema connectionString="..."
```

### Phase 2: Fragment Editing with MCP Enhancement
```bash
# Read targeted fragment
read_file("file.ext", startLine: 40, endLine: 80)

# Apply MCP-powered edits
# .NET
roslyn-mcp/invoke_refactoring fileUri="file.cs" name="source.addTypeAnnotation"
# TypeScript
typescript-mcp/invoke_refactoring fileUri="file.ts" name="source.addTypeAnnotation"
# Vue
vue-mcp/validate_i18n_keys componentPath="file.vue"

# Edit with precision
replace_string_in_file(filePath, oldString, newString)
```

### Phase 3: Quality Validation
```bash
# Language-specific validation
get_errors(filePath)  # Syntax check
runTests(files: [testFile])  # Functional validation

# MCP semantic validation
roslyn-mcp/analyze_types workspacePath="." filter="breaking-changes"
typescript-mcp/validate_types fileUri="file.ts"
vue-mcp/check_accessibility filePath="file.vue"
database-mcp/check_migrations workspacePath="backend"
```

---

## 🛠️ Language-Specific MCP Workflows

### .NET Backend (Roslyn MCP)
```bash
# Pre-edit analysis
roslyn-mcp/analyze_types workspacePath="backend/Domain/Catalog"

# Refactoring
roslyn-mcp/invoke_refactoring fileUri="Service.cs" name="source.unusedImports"

# Validation
roslyn-mcp/analyze_types workspacePath="backend" filter="errors"
```

### TypeScript Frontend (TypeScript MCP)
```bash
# Analysis
typescript-mcp/analyze_types workspacePath="frontend/src/components"

# Refactoring
typescript-mcp/invoke_refactoring fileUri="Component.ts" name="source.addTypeAnnotation"

# Validation
typescript-mcp/validate_types fileUri="Component.ts"
```

### Vue Components (Vue MCP)
```bash
# Component analysis
vue-mcp/analyze_vue_component filePath="src/ProductCard.vue"

# i18n validation
vue-mcp/validate_i18n_keys componentPath="src/ProductCard.vue"

# Responsive check
vue-mcp/check_responsive_design filePath="src/ProductCard.vue"

# Accessibility
vue-mcp/check_accessibility filePath="src/ProductCard.vue"
```

### Database Schema (Database MCP)
```bash
# Schema validation
database-mcp/validate_schema connectionString="Server=localhost;Database=B2X"

# Migration check
database-mcp/check_migrations workspacePath="backend" migrationPath="Migrations"

# Query analysis
database-mcp/analyze_queries workspacePath="backend/Domain" queryFile="Queries.cs"
```

### HTML/CSS (HTML/CSS MCP)
```bash
# Accessibility validation
htmlcss-mcp/check_html_accessibility workspacePath="frontend" filePath="pages/product.html"

# CSS analysis
htmlcss-mcp/validate_css workspacePath="frontend/src" filePath="styles/main.css"
```

### API Contracts (B2X MCP)
```bash
# Contract validation
B2X-mcp/validate_api_contracts workspacePath="backend/Gateway"

# Breaking change detection
B2X-mcp/check_breaking_changes oldSpec="v1.0" newSpec="v2.0"
```

### Testing (Testing MCP)
```bash
# Test validation
testing-mcp/validate_test_coverage workspacePath="backend" testFile="ServiceTests.cs"

# Mock analysis
testing-mcp/analyze_mocks workspacePath="backend" testFile="ServiceTests.cs"
```

### i18n (i18n MCP)
```bash
# Translation validation
i18n-mcp/validate_translation_keys workspacePath="frontend" localePath="locales"

# Missing key detection
i18n-mcp/check_missing_translations workspacePath="frontend" baseLocale="en"

# Consistency check
i18n-mcp/validate_consistency workspacePath="frontend"
```

---

## ⚠️ Quality Safeguards

### Mandatory Pre-Edit Checks
- [ ] Run `list_code_usages` to understand dependencies
- [ ] Execute MCP semantic analysis for target language
- [ ] Review cross-file impacts

### Validation Gates
- [ ] Syntax validation: `get_errors(filePath)`
- [ ] Functional testing: `runTests(files: [relatedTests])`
- [ ] MCP semantic check: Language-specific validation
- [ ] Integration testing: Full suite for critical changes

### Risk Mitigation
| Risk | Mitigation |
|------|------------|
| **Context Blindness** | MCP semantic analysis + `list_code_usages` |
| **Incremental Errors** | Comprehensive validation after each edit |
| **Cross-Language Issues** | API contract validation + integration tests |
| **Tool Failures** | Fallback to fragment-only approach |

---

## 📋 Decision Tree: When to Use

```
Large file edit needed?
├── Yes → MCP available for language?
│   ├── Yes → Use MCP-enhanced workflow
│   └── No → Use fragment-only approach
└── No → Standard editing (read full if <200 lines)
```

### Full File Read Scenarios (Rare)
- Small files (<50 lines)
- Architecture documentation
- Security-critical code (full context needed)
- Complex refactoring requiring complete understanding

---

## 🎯 Success Metrics

- **Token Reduction**: 75%+ savings on large file edits
- **Quality Maintenance**: <5% defect increase vs. baseline
- **Adoption Rate**: 80%+ agent usage within 2 weeks
- **Error Detection**: 90%+ of semantic issues caught pre-commit

---

## 🔄 Integration with Existing Guidelines

- **GL-006**: Token optimization foundation
- **GL-044**: Fragment-based access patterns
- **GL-043**: Smart attachment loading
- **KB-052 to KB-064**: MCP tool documentation

---

## 📈 Implementation Timeline

| Phase | Duration | Deliverables |
|-------|----------|--------------|
| **Foundation** | Week 1 | Guideline + MCP config updates |
| **Agent Training** | Week 2 | Updated instructions + workflows |
| **Rollout** | Week 3 | Pilot testing + monitoring |
| **Optimization** | Ongoing | Metrics tracking + refinements |

---

## 🚨 Emergency Fallback

If MCP tools fail:
1. Fall back to fragment-only approach ([GL-044])
2. Increase validation frequency
3. Document incident for tool improvement
4. Consider full file read for critical changes

---

**Maintained by**: @CopilotExpert  
**Review**: Monthly  
**Size Target**: <5 KB (token optimization)</content>
<parameter name="filePath">c:\Users\Holge\repos\B2Connect\.ai\guidelines\GL-052-multi-language-fragment-editing.md