---
applyTo: "**/*"
---

# MCP Operations Guide

**Purpose**: Centralized guide for MCP (Model Context Protocol) server usage patterns across all development contexts.

## Overview

B2Connect integrates multiple MCP servers to provide AI-assisted code analysis, security validation, and quality assurance. This guide helps developers understand when and how to use each MCP server effectively.

---

## 🎯 When to Use Which MCP Server

### Frontend Development

**File Types**: `.vue`, `.ts`, `.js`, `.html`, `.css`

| Task | MCP Server | Command |
|------|-----------|---------|
| Type checking | TypeScript MCP | `typescript-mcp/analyze_types` |
| Symbol search | TypeScript MCP | `typescript-mcp/search_symbols` |
| Component analysis | Vue MCP | `vue-mcp/analyze_vue_component` |
| i18n validation | Vue MCP | `vue-mcp/validate_i18n_keys` |
| Responsive design | Vue MCP | `vue-mcp/check_responsive_design` |
| HTML structure | HTML/CSS MCP | `htmlcss-mcp/analyze_html_structure` |
| Accessibility | HTML/CSS MCP | `htmlcss-mcp/check_html_accessibility` |
| CSS optimization | HTML/CSS MCP | `htmlcss-mcp/analyze_css_structure` |
| XSS scanning | Security MCP | `security-mcp/scan_xss_vulnerabilities` |

---

### Backend Development

**File Types**: `.cs`, `.csproj`

| Task | MCP Server | Command | Status |
|------|-----------|---------|--------|
| Symbol search | Roslyn MCP | `roslyn-mcp/search_symbols` | Optional* |
| Type analysis | Roslyn MCP | `roslyn-mcp/analyze_types` | Optional* |
| Handler analysis | Wolverine MCP | `wolverine-mcp/analyze_handlers` | Optional* |
| DI validation | Wolverine MCP | `wolverine-mcp/validate_di` | Optional* |
| SQL injection | Security MCP | `security-mcp/check_sql_injection` | Always |
| Input validation | Security MCP | `security-mcp/validate_input_sanitization` | Always |
| Auth patterns | Security MCP | `security-mcp/check_authentication` | Always |

*Enable in `.vscode/mcp.json` when needed

---

### Quality Assurance & Testing

**Context**: All test files, QA workflows

| Task | MCP Server | Command | Status |
|------|-----------|---------|--------|
| Type safety | TypeScript/Roslyn | `*-mcp/analyze_types` | Always |
| Accessibility | Vue/HTML MCP | `*-mcp/check_accessibility` | Always |
| Security | Security MCP | All tools | Always |
| Visual regression | Chrome DevTools | `chrome-devtools-mcp/screenshot` | Optional* |
| Performance | Chrome DevTools | `chrome-devtools-mcp/lighthouse` | Optional* |

*Enable in `.vscode/mcp.json` for advanced E2E testing

---

### Security Audits

**Context**: Security reviews, compliance checks, pre-deployment

| Task | MCP Server | Command |
|------|-----------|---------|
| Dependency scan | Security MCP | `security-mcp/scan_vulnerabilities` |
| SQL injection | Security MCP | `security-mcp/check_sql_injection` |
| XSS scanning | Security MCP | `security-mcp/scan_xss_vulnerabilities` |
| Input validation | Security MCP | `security-mcp/validate_input_sanitization` |
| Auth review | Security MCP | `security-mcp/check_authentication` |

---

## 🔄 MCP Tool Chaining Examples

### Complete Frontend PR Review

```bash
#!/bin/bash
echo "🔍 Running Frontend MCP Analysis..."

# 1. Type safety
typescript-mcp/analyze_types workspacePath="frontend/Store"
if [ $? -ne 0 ]; then echo "❌ Type errors found"; exit 1; fi

# 2. i18n compliance (MANDATORY - zero hardcoded strings)
vue-mcp/validate_i18n_keys workspacePath="frontend/Store"
if [ $? -ne 0 ]; then echo "❌ Hardcoded strings found"; exit 1; fi

# 3. Responsive design
vue-mcp/check_responsive_design filePath="src/components/ChangedComponent.vue"

# 4. Accessibility (WCAG AAA target)
htmlcss-mcp/check_html_accessibility workspacePath="frontend/Store" filePath="pages/index.html"

# 5. Security (XSS prevention)
security-mcp/scan_xss_vulnerabilities workspacePath="frontend/Store"
if [ $? -ne 0 ]; then echo "❌ XSS vulnerabilities found"; exit 1; fi

echo "✅ All frontend MCP checks passed"
```

---

### Complete Backend PR Review

```bash
#!/bin/bash
echo "🔍 Running Backend MCP Analysis..."

# Optional: Enable Roslyn MCP for large refactoring
# roslyn-mcp/analyze_types workspacePath="backend"

# 1. SQL injection detection (MANDATORY)
security-mcp/check_sql_injection workspacePath="backend"
if [ $? -ne 0 ]; then echo "❌ SQL injection vulnerabilities found"; exit 1; fi

# 2. Input validation
security-mcp/validate_input_sanitization workspacePath="backend"
if [ $? -ne 0 ]; then echo "❌ Input validation issues found"; exit 1; fi

# 3. Authentication patterns
security-mcp/check_authentication workspacePath="backend"

# Optional: Wolverine handler analysis
# wolverine-mcp/analyze_handlers workspacePath="backend/Domain"

echo "✅ All backend MCP checks passed"
```

---

### Pre-Deployment Security Audit

```bash
#!/bin/bash
echo "🔒 Running Comprehensive Security Audit..."

# 1. Dependency vulnerabilities (CRITICAL)
security-mcp/scan_vulnerabilities workspacePath="."
if grep -q "CRITICAL" security-results.json; then
  echo "❌ CRITICAL vulnerabilities detected - deployment blocked"
  exit 1
fi

# 2. Backend security
security-mcp/check_sql_injection workspacePath="backend"
security-mcp/validate_input_sanitization workspacePath="backend"
security-mcp/check_authentication workspacePath="backend"

# 3. Frontend security
security-mcp/scan_xss_vulnerabilities workspacePath="frontend"
security-mcp/validate_input_sanitization workspacePath="frontend"

# 4. Generate audit report
echo "📊 Security audit results saved to .ai/compliance/security-audit-$(date +%Y%m%d).md"
echo "✅ Security audit complete"
```

---

### E2E Test Preparation

```bash
#!/bin/bash
echo "🧪 Preparing for E2E Test Execution..."

# 1. Type safety validation
typescript-mcp/analyze_types workspacePath="frontend/Store"
if [ $? -ne 0 ]; then echo "❌ Fix type errors before E2E tests"; exit 1; fi

# 2. Component structure validation
vue-mcp/analyze_vue_component filePath="src/components/LoginForm.vue"

# 3. Accessibility pre-check
vue-mcp/check_accessibility filePath="src/components/LoginForm.vue"

# 4. Security pre-check
security-mcp/scan_xss_vulnerabilities workspacePath="frontend/Store"

# Optional: Enable Chrome DevTools MCP for visual regression
# chrome-devtools-mcp/launch browser
# chrome-devtools-mcp/screenshot path="tests/baseline/login.png"

echo "✅ E2E test environment validated"
npm run test:e2e
```

---

## 📊 MCP Server Status & Configuration

### Always Enabled (Production Ready)

```json
// .vscode/mcp.json
{
  "mcpServers": {
    "typescript-mcp": { "disabled": false },
    "vue-mcp": { "disabled": false },
    "security-mcp": { "disabled": false },
    "htmlcss-mcp": { "disabled": false },
    "b2connect-admin": { "disabled": false }
  }
}
```

**Use Cases**:
- Daily development workflows
- Pre-commit validation
- Code review automation
- Continuous integration

---

### Enable On-Demand (Optional - For Specific Tasks)

```json
// Enable when needed, disable after use
{
  "mcpServers": {
    "roslyn-code-navigator": { "disabled": true },  // Large C# refactoring
    "wolverine-mcp": { "disabled": true },          // CQRS pattern analysis
    "chrome-devtools": { "disabled": true }         // E2E visual regression
  }
}
```

**When to Enable**:
- **Roslyn MCP**: Large-scale backend refactoring, symbol usage analysis
- **Wolverine MCP**: CQRS handler implementation, DI validation
- **Chrome DevTools MCP**: Visual regression testing, performance profiling

---

## 🎯 Integration with Development Workflows

### Daily Development Routine

```bash
# Morning (5 min)
security-mcp/scan_vulnerabilities workspacePath="."
# Check for new CVEs overnight

# Pre-Commit (2-3 min)
# Run relevant MCP tools based on changed files
git diff --name-only | while read file; do
  case "$file" in
    *.vue) vue-mcp/validate_i18n_keys componentPath="$file" ;;
    *.cs) security-mcp/check_sql_injection workspacePath="backend" ;;
    *.html) htmlcss-mcp/check_html_accessibility filePath="$file" ;;
  esac
done

# Pre-PR (10-15 min)
# Full MCP suite on modified areas
./scripts/mcp-pre-pr-check.sh
```

---

### Code Review Integration (PRM-002)

**Reviewer Checklist**:
1. ✅ All MCP validations passed (see PR description)
2. ✅ No CRITICAL security issues
3. ✅ Zero hardcoded strings (i18n compliance)
4. ✅ WCAG accessibility validated
5. ✅ Type safety confirmed

**Automated PR Comment**:
```markdown
## MCP Analysis Results

### ✅ Passed
- TypeScript type analysis: 0 errors
- Vue i18n validation: 0 hardcoded strings
- Accessibility: WCAG AAA compliant
- Security XSS scan: 0 vulnerabilities

### ⚠️ Warnings
- CSS optimization: 3 duplicate selectors (non-blocking)

**Action Required**: None - PR approved for human review
```

---

### Weekly Security Audit (Automated)

```yaml
# .github/workflows/weekly-security-audit.yml
name: Weekly Security Audit

on:
  schedule:
    - cron: '0 9 * * 1' # Every Monday at 9am

jobs:
  security-audit:
    runs-on: ubuntu-latest
    steps:
      - name: Run MCP Security Suite
        run: |
          security-mcp/scan_vulnerabilities workspacePath="."
          security-mcp/check_sql_injection workspacePath="backend"
          security-mcp/scan_xss_vulnerabilities workspacePath="frontend"
          
      - name: Generate Report
        run: |
          echo "# Security Audit Report - $(date)" > report.md
          cat security-results.json >> report.md
          
      - name: Create Issue if Critical
        if: failure()
        run: |
          gh issue create \
            --title "🚨 Weekly Security Audit: Critical Issues Detected" \
            --label "security,critical" \
            --body-file report.md
```

---

## 🚀 Performance & Resource Management

### Smart MCP Usage

**✅ Efficient**:
```bash
# Targeted analysis on specific file
vue-mcp/validate_i18n_keys componentPath="src/components/UserProfile.vue"

# Directory-scoped analysis
security-mcp/check_sql_injection workspacePath="backend/Domain/Catalog"
```

**❌ Inefficient**:
```bash
# Full workspace scan (slow, high resource usage)
vue-mcp/validate_i18n_keys workspacePath="."
security-mcp/check_sql_injection workspacePath="."
```

---

### MCP Server Lifecycle Management

**Active Development** (keep enabled):
- TypeScript MCP
- Vue MCP
- Security MCP
- HTML/CSS MCP

**Task-Specific** (enable only when needed):
- Roslyn MCP → Backend refactoring sessions
- Wolverine MCP → CQRS implementation
- Chrome DevTools MCP → E2E testing sprints

**Resource Optimization**:
```bash
# Check MCP server resource usage
ps aux | grep -E 'typescript-mcp|vue-mcp|security-mcp'

# Restart MCP server if performance degrades
# VS Code: Developer: Reload Window
```

---

## 🔧 Troubleshooting

### MCP Server Not Responding

```bash
# 1. Check server status in VS Code Output panel
# View → Output → Select "MCP Servers"

# 2. Verify configuration
cat .vscode/mcp.json | grep -A 5 "typescript-mcp"

# 3. Rebuild MCP server
cd tools/TypeScriptMCP
npm run build
npm start

# 4. Restart VS Code
# Command Palette → Developer: Reload Window
```

---

### False Positives

```typescript
// Document exceptions in code
// MCP-EXCEPTION: [Tool name] - [Reason]

// Example: Security MCP false positive
// MCP-EXCEPTION: security-mcp - Dynamic SQL is safe, tableName validated against whitelist
const sql = `SELECT * FROM ${validatedTableName} WHERE Id = @id`;
```

---

### Performance Issues

```bash
# Clear MCP caches
find tools -name "node_modules/.cache" -type d -exec rm -rf {} +

# Use incremental analysis
git diff --name-only | grep '.vue$' | xargs -I {} \
  vue-mcp/validate_i18n_keys componentPath="{}"

# Enable only required MCPs
# Disable unused MCPs in .vscode/mcp.json
```

---

## 📚 Related Documentation

- [KB-053] TypeScript MCP Integration Guide
- [KB-054] Vue MCP Integration Guide
- [KB-055] Security MCP Best Practices
- [KB-056] HTML/CSS MCP Usage Guide
- [KB-052] Roslyn MCP Server (optional)
- [INS-001] Backend Instructions
- [INS-002] Frontend Instructions
- [INS-003] Testing Instructions
- [INS-005] Security Instructions

---

## 💡 Best Practices

1. **Run MCP tools early and often** - Don't wait until PR time
2. **Fix issues immediately** - Smaller corrections are easier
3. **Use MCP for discovery** - Study patterns before implementing
4. **Automate with scripts** - Create npm/bash scripts for common MCP chains
5. **Document exceptions** - If MCP flags false positives, document why
6. **Monitor resource usage** - Disable unused MCPs to reduce overhead
7. **Chain tools strategically** - Run type checking before other validations
8. **Cache results** - Use targeted scans during development, full scans pre-PR

---

**Maintained by**: @CopilotExpert  
**Last Review**: 6. Januar 2026  
**Next Review**: 6. Februar 2026
