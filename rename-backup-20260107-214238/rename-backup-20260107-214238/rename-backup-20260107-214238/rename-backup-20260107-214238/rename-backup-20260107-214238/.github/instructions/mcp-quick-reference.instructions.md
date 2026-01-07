---
docid: MCP-QUICK-REF
title: MCP Tools Quick Reference
applyTo: "**/*"
---

# 🚀 MCP Tools - Quick Reference

**Size**: ~2 KB | **Status**: Phase 2 Replacement  
For detailed docs → Use `kb-mcp/search_knowledge_base` or see [KB-055] Security MCP Best Practices

---

## 📚 KB Access (ALWAYS USE)

**Search Knowledge Base**
```
kb-mcp/search_knowledge_base
  query: string (natural language)
  max_results: 5 (default)
  category: tools|patterns|architecture|best-practices
```

**Get Article**
```
kb-mcp/get_article
  docid: string (KB-053, ADR-001)
  section: string (optional)
```

**Browse Category**
```
kb-mcp/list_by_category
  category: tools|patterns|architecture|best-practices
```

---

## 🔧 Always Enabled MCPs

| MCP | Purpose | When to Use |
|-----|---------|-------------|
| **security-mcp** | XSS, SQL injection, vulnerabilities | Every change |
| **typescript-mcp** | Type checking, symbol search | Frontend/TypeScript |
| **vue-mcp** | Component analysis, i18n, responsive | Vue components |
| **database-mcp** | Schema, queries, migrations | Database changes |
| **docker-mcp** | Container security, K8s validation | DevOps changes |
| **b2connect-mcp** | Domain validation, catalog checks | Business logic |
| **htmlcss-mcp** | Accessibility, CSS optimization | Frontend styling |
| **git-mcp** | Commit validation, branch strategy | Pre-commit |

---

## 🔌 Optional MCPs (Enable on Demand)

| MCP | Enable When | Status |
|-----|-------------|--------|
| **roslyn-mcp** | Large C# refactoring | Disabled |
| **wolverine-mcp** | CQRS handler work | Disabled |
| **chrome-devtools-mcp** | E2E visual testing | Disabled |

---

## 🎯 Quick Decision Matrix

| Task | Tool | Example |
|------|------|---------|
| Learn about topic | `kb-mcp/search_knowledge_base` | "Vue composition patterns" |
| Find specific doc | `kb-mcp/get_article` | docid: "KB-053" |
| Browse tools | `kb-mcp/list_by_category` | category: "tools" |
| Type checking | `typescript-mcp/analyze_types` | frontend path |
| Security audit | `security-mcp/scan_vulnerabilities` | workspacePath: "." |
| Component analysis | `vue-mcp/validate_i18n_keys` | component path |
| Database checks | `database-mcp/validate_schema` | SQL validation |
| Accessibility | `htmlcss-mcp/check_html_accessibility` | WCAG compliance |

---

## ⚡ Pre-Commit Flow

```bash
1. kb-mcp/search_knowledge_base "my topic"  # Learn
2. typescript-mcp/analyze_types             # Type check
3. vue-mcp/validate_i18n_keys              # i18n check
4. security-mcp/scan_xss_vulnerabilities   # Security
5. htmlcss-mcp/check_html_accessibility    # A11y
6. git-mcp/validate_commit_messages        # Commit format
→ COMMIT READY ✓
```

---

## 📋 Tool Categories

### Backend Security
- `security-mcp/check_sql_injection`
- `security-mcp/validate_input_sanitization`
- `security-mcp/check_authentication`

### Frontend Quality
- `typescript-mcp/analyze_types`
- `vue-mcp/validate_i18n_keys`
- `htmlcss-mcp/check_html_accessibility`

### Infrastructure
- `docker-mcp/check_container_security`
- `docker-mcp/validate_kubernetes_manifests`
- `database-mcp/validate_schema`

---

## 🔗 Full Documentation

Detailed tool references available via KB-MCP:

```
kb-mcp/search_knowledge_base query:"MCP tools" category:"tools"
→ Returns: KB-055, KB-053, KB-054, KB-056, KB-057
```

Or search for specific topics:
- TypeScript: `kb-mcp/search_knowledge_base query:"TypeScript MCP"`
- Vue: `kb-mcp/search_knowledge_base query:"Vue MCP"`
- Security: `kb-mcp/search_knowledge_base query:"Security MCP"`

---

**For Complete MCP Documentation**: See [KB-055] Security MCP Best Practices or MCP Operations Guide in KB

