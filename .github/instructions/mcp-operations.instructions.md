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
| Domain validation | B2Connect MCP | `b2connect-mcp/validate_tenant_config` |
| Catalog validation | B2Connect MCP | `b2connect-mcp/validate_catalog_structure` |
| ERP integration | B2Connect MCP | `b2connect-mcp/check_erp_integration` |
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
| Domain analysis | B2Connect MCP | `b2connect-mcp/analyze_domain_models` | Always |
| Lifecycle validation | B2Connect MCP | `b2connect-mcp/validate_lifecycle_stages` | Always |
| SQL injection | Security MCP | `security-mcp/check_sql_injection` | Always |
| Input validation | Security MCP | `security-mcp/validate_input_sanitization` | Always |
| Auth patterns | Security MCP | `security-mcp/check_authentication` | Always |

*Enable in `.vscode/mcp.json` when needed

---

### Database Development

**File Types**: `.sql`, `*.cs` (Entity Framework), `*.json` (Elasticsearch mappings)

| Task | MCP Server | Command | Status |
|------|-----------|---------|--------|
| Schema validation | Database MCP | `database-mcp/validate_schema` | Always |
| Query optimization | Database MCP | `database-mcp/analyze_queries` | Always |
| Migration analysis | Database MCP | `database-mcp/check_migrations` | Always |
| Connection monitoring | Database MCP | `database-mcp/monitor_connections` | Always |
| Index optimization | Database MCP | `database-mcp/optimize_indexes` | Always |
| Multi-tenant validation | Database MCP | `database-mcp/validate_multitenancy` | Always |
| Elasticsearch mapping | Database MCP | `database-mcp/validate_elasticsearch_mappings` | Always |

---

### Testing & Quality Assurance

**File Types**: `*.test.*`, `*.spec.*`, `**/tests/**`, `**/TestResults/**`

| Task | MCP Server | Command | Status |
|------|-----------|---------|--------|
| Test execution | Testing MCP | `testing-mcp/run_tests` | Always |
| Coverage analysis | Testing MCP | `testing-mcp/analyze_coverage` | Always |
| Test quality validation | Testing MCP | `testing-mcp/validate_tests` | Always |
| Performance testing | Testing MCP | `testing-mcp/run_performance_tests` | Always |
| Integration testing | Testing MCP | `testing-mcp/run_integration_tests` | Always |
| Test data generation | Testing MCP | `testing-mcp/generate_test_data` | Optional* |
| Test reporting | Testing MCP | `testing-mcp/generate_reports` | Always |

*Enable for AI-assisted test data generation

---

### API Documentation

**File Types**: `*.cs` (Controllers), `*.yaml`, `*.json` (OpenAPI specs)

| Task | MCP Server | Command | Status |
|------|-----------|---------|--------|
| OpenAPI validation | API Documentation MCP | `api-mcp/validate_openapi` | Always |
| API contract checking | API Documentation MCP | `api-mcp/validate_contracts` | Always |
| Breaking change detection | API Documentation MCP | `api-mcp/check_breaking_changes` | Always |
| Documentation completeness | API Documentation MCP | `api-mcp/check_documentation` | Always |
| Schema validation | API Documentation MCP | `api-mcp/validate_schemas` | Always |
| API versioning analysis | API Documentation MCP | `api-mcp/analyze_versioning` | Always |
| Client SDK generation | API Documentation MCP | `api-mcp/generate_sdk` | Optional* |

*Enable for automated SDK generation

---

### Internationalization (i18n)

**File Types**: `*.json` (locale files), `*.vue`, `*.ts`, `*.cs`

| Task | MCP Server | Command | Status |
|------|-----------|---------|--------|
| Translation key validation | i18n MCP | `i18n-mcp/validate_translation_keys` | Always |
| Missing translation detection | i18n MCP | `i18n-mcp/check_missing_translations` | Always |
| Translation consistency | i18n MCP | `i18n-mcp/validate_consistency` | Always |
| Locale file analysis | i18n MCP | `i18n-mcp/analyze_locale_files` | Always |
| Pluralization validation | i18n MCP | `i18n-mcp/check_pluralization` | Always |
| Interpolation validation | i18n MCP | `i18n-mcp/validate_interpolation` | Always |
| RTL language support | i18n MCP | `i18n-mcp/check_rtl_support` | Optional* |
| Translation extraction | i18n MCP | `i18n-mcp/extract_translations` | Optional* |

*Enable for advanced i18n features

---

### Monitoring & Observability

**File Types**: `*.cs`, `*.ts`, `*.vue`, `*.yaml`, `*.json` (metrics configs)

| Task | MCP Server | Command | Status |
|------|-----------|---------|--------|
| Application metrics | Monitoring MCP | `monitoring-mcp/collect_application_metrics` | Always |
| System performance | Monitoring MCP | `monitoring-mcp/monitor_system_performance` | Always |
| Error tracking | Monitoring MCP | `monitoring-mcp/track_errors` | Always |
| Log analysis | Monitoring MCP | `monitoring-mcp/analyze_logs` | Always |
| Health checks | Monitoring MCP | `monitoring-mcp/validate_health_checks` | Always |
| Alert configuration | Monitoring MCP | `monitoring-mcp/configure_alerts` | Always |
| Performance profiling | Monitoring MCP | `monitoring-mcp/profile_performance` | Optional* |
| Resource monitoring | Monitoring MCP | `monitoring-mcp/monitor_resources` | Optional* |

*Enable for advanced monitoring features

---

### Documentation

**File Types**: `*.md`, `*.rst`, `*.txt`, `docs/**`, `*.adoc`

| Task | MCP Server | Command | Status |
|------|-----------|---------|--------|
| Documentation validation | Documentation MCP | `docs-mcp/validate_documentation` | Always |
| Link checking | Documentation MCP | `docs-mcp/check_links` | Always |
| Content quality analysis | Documentation MCP | `docs-mcp/analyze_content_quality` | Always |
| Structure validation | Documentation MCP | `docs-mcp/validate_structure` | Always |
| Accessibility compliance | Documentation MCP | `docs-mcp/check_accessibility` | Always |
| SEO optimization | Documentation MCP | `docs-mcp/optimize_seo` | Optional* |
| Translation sync | Documentation MCP | `docs-mcp/sync_translations` | Optional* |

*Enable for advanced documentation features

---

### DevOps & Infrastructure

**File Types**: `Dockerfile`, `docker-compose.yml`, `*.yaml`, `*.yml`

| Task | MCP Server | Command | Status |
|------|-----------|---------|--------|
| Dockerfile analysis | Docker MCP | `docker-mcp/analyze_dockerfile` | Always |
| K8s manifest validation | Docker MCP | `docker-mcp/validate_kubernetes_manifests` | Always |
| Container security | Docker MCP | `docker-mcp/check_container_security` | Always |
| Compose configuration | Docker MCP | `docker-mcp/analyze_docker_compose` | Always |
| Container monitoring | Docker MCP | `docker-mcp/monitor_container_health` | Always |

---

### Quality Assurance & Testing

**Context**: All test files, QA workflows

| Task | MCP Server | Command | Status |
|------|-----------|---------|--------|
| Type safety | TypeScript/Roslyn | `*-mcp/analyze_types` | Always |
| Accessibility | Vue/HTML MCP | `*-mcp/check_accessibility` | Always |
| Security | Security MCP | All tools | Always |
| Database integrity | Database MCP | `database-mcp/validate_schema` | Always |
| Query performance | Database MCP | `database-mcp/analyze_queries` | Always |
| Data migration | Database MCP | `database-mcp/check_migrations` | Always |
| Test execution | Testing MCP | `testing-mcp/run_tests` | Always |
| Test coverage | Testing MCP | `testing-mcp/analyze_coverage` | Always |
| Test quality | Testing MCP | `testing-mcp/validate_tests` | Always |
| Performance testing | Testing MCP | `testing-mcp/run_performance_tests` | Always |
| Integration testing | Testing MCP | `testing-mcp/run_integration_tests` | Always |
| API documentation | API Documentation MCP | `api-mcp/validate_openapi` | Always |
| API contracts | API Documentation MCP | `api-mcp/validate_contracts` | Always |
| Breaking changes | API Documentation MCP | `api-mcp/check_breaking_changes` | Always |
| i18n compliance | i18n MCP | `i18n-mcp/validate_translation_keys` | Always |
| Translation completeness | i18n MCP | `i18n-mcp/check_missing_translations` | Always |
| Translation consistency | i18n MCP | `i18n-mcp/validate_consistency` | Always |
| Application monitoring | Monitoring MCP | `monitoring-mcp/collect_application_metrics` | Always |
| System performance | Monitoring MCP | `monitoring-mcp/monitor_system_performance` | Always |
| Error tracking | Monitoring MCP | `monitoring-mcp/track_errors` | Always |
| Documentation quality | Documentation MCP | `docs-mcp/validate_documentation` | Always |
| Link validation | Documentation MCP | `docs-mcp/check_links` | Always |
| Content quality | Documentation MCP | `docs-mcp/analyze_content_quality` | Always |
| Performance analysis | Performance MCP | `performance-mcp/analyze_code_performance` | Always |
| Memory profiling | Performance MCP | `performance-mcp/profile_memory_usage` | Always |
| Bundle optimization | Performance MCP | `performance-mcp/check_bundle_size` | Always |
| Commit quality | Git MCP | `git-mcp/validate_commit_messages` | Always |
| Branch strategy | Git MCP | `git-mcp/check_branch_strategy` | Always |
| Code churn | Git MCP | `git-mcp/analyze_code_churn` | Always |
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

### Complete Database PR Review

```bash
#!/bin/bash
echo "🗄️ Running Database MCP Analysis..."

# 1. Schema validation
database-mcp/validate_schema workspacePath="backend"
if [ $? -ne 0 ]; then echo "❌ Schema validation failed"; exit 1; fi

# 2. Query optimization analysis
database-mcp/analyze_queries workspacePath="backend/Domain"
if [ $? -ne 0 ]; then echo "❌ Query optimization issues found"; exit 1; fi

# 3. Migration validation
database-mcp/check_migrations workspacePath="backend"
if [ $? -ne 0 ]; then echo "❌ Migration issues detected"; exit 1; fi

# 4. Multi-tenant validation
database-mcp/validate_multitenancy workspacePath="backend"
if [ $? -ne 0 ]; then echo "❌ Multi-tenant configuration issues"; exit 1; fi

# 5. Elasticsearch mapping validation
database-mcp/validate_elasticsearch_mappings workspacePath="backend/Domain/Search"

# 6. Index optimization
database-mcp/optimize_indexes workspacePath="backend"

echo "✅ All database MCP checks passed"
```

---

### Complete Testing PR Review

```bash
#!/bin/bash
echo "🧪 Running Testing MCP Analysis..."

# 1. Test quality validation
testing-mcp/validate_tests workspacePath="."
if [ $? -ne 0 ]; then echo "❌ Test quality issues found"; exit 1; fi

# 2. Coverage analysis
testing-mcp/analyze_coverage workspacePath="." minCoverage="80"
if [ $? -ne 0 ]; then echo "❌ Coverage below threshold"; exit 1; fi

# 3. Run unit tests
testing-mcp/run_tests workspacePath="." testType="unit"
if [ $? -ne 0 ]; then echo "❌ Unit tests failed"; exit 1; fi

# 4. Run integration tests
testing-mcp/run_integration_tests workspacePath="." environment="test"
if [ $? -ne 0 ]; then echo "❌ Integration tests failed"; exit 1; fi

# 5. Performance testing
testing-mcp/run_performance_tests workspacePath="." baseline="previous"
if [ $? -ne 0 ]; then echo "❌ Performance regression detected"; exit 1; fi

# 6. Generate test report
testing-mcp/generate_reports workspacePath="." format="html,json"

echo "✅ All testing MCP checks passed"
```

---

### Complete API Documentation PR Review

```bash
#!/bin/bash
echo "📚 Running API Documentation MCP Analysis..."

# 1. OpenAPI specification validation
api-mcp/validate_openapi workspacePath="backend"
if [ $? -ne 0 ]; then echo "❌ OpenAPI validation failed"; exit 1; fi

# 2. API contract validation
api-mcp/validate_contracts workspacePath="backend"
if [ $? -ne 0 ]; then echo "❌ API contract validation failed"; exit 1; fi

# 3. Breaking change detection
api-mcp/check_breaking_changes workspacePath="backend" baseline="main"
if [ $? -ne 0 ]; then echo "❌ Breaking changes detected"; exit 1; fi

# 4. Documentation completeness check
api-mcp/check_documentation workspacePath="backend"
if [ $? -ne 0 ]; then echo "❌ Documentation incomplete"; exit 1; fi

# 5. Schema validation
api-mcp/validate_schemas workspacePath="backend"
if [ $? -ne 0 ]; then echo "❌ Schema validation failed"; exit 1; fi

# 6. API versioning analysis
api-mcp/analyze_versioning workspacePath="backend"

echo "✅ All API documentation MCP checks passed"
```

---

### Complete i18n PR Review

```bash
#!/bin/bash
echo "🌍 Running i18n MCP Analysis..."

# 1. Translation key validation (MANDATORY - zero missing keys)
i18n-mcp/validate_translation_keys workspacePath="."
if [ $? -ne 0 ]; then echo "❌ Missing translation keys found"; exit 1; fi

# 2. Missing translation detection
i18n-mcp/check_missing_translations workspacePath="." languages="en,de,fr,es,it,pt,nl,pl"
if [ $? -ne 0 ]; then echo "❌ Missing translations detected"; exit 1; fi

# 3. Translation consistency validation
i18n-mcp/validate_consistency workspacePath="."
if [ $? -ne 0 ]; then echo "❌ Translation inconsistencies found"; exit 1; fi

# 4. Locale file analysis
i18n-mcp/analyze_locale_files workspacePath="."

# 5. Pluralization validation
i18n-mcp/check_pluralization workspacePath="."

# 6. Interpolation validation
i18n-mcp/validate_interpolation workspacePath="."
if [ $? -ne 0 ]; then echo "❌ Interpolation issues found"; exit 1; fi

# Optional: RTL language support (for Arabic, Hebrew)
# i18n-mcp/check_rtl_support workspacePath="."

echo "✅ All i18n MCP checks passed"
```

---

### Complete Monitoring PR Review

```bash
#!/bin/bash
echo "📊 Running Monitoring MCP Analysis..."

# 1. Application metrics collection
monitoring-mcp/collect_application_metrics workspacePath="."
if [ $? -ne 0 ]; then echo "❌ Application metrics collection failed"; exit 1; fi

# 2. System performance monitoring
monitoring-mcp/monitor_system_performance workspacePath="."
if [ $? -ne 0 ]; then echo "❌ System performance issues detected"; exit 1; fi

# 3. Error tracking validation
monitoring-mcp/track_errors workspacePath="."
if [ $? -ne 0 ]; then echo "❌ Error tracking issues found"; exit 1; fi

# 4. Log analysis
monitoring-mcp/analyze_logs workspacePath="." logPath="logs/"
if [ $? -ne 0 ]; then echo "❌ Log analysis issues detected"; exit 1; fi

# 5. Health checks validation
monitoring-mcp/validate_health_checks workspacePath="."
if [ $? -ne 0 ]; then echo "❌ Health check validation failed"; exit 1; fi

# 6. Alert configuration validation
monitoring-mcp/configure_alerts workspacePath="." configPath="monitoring/alerts.yml"
if [ $? -ne 0 ]; then echo "❌ Alert configuration issues found"; exit 1; fi

# Optional: Advanced monitoring features
# monitoring-mcp/profile_performance workspacePath="." duration="60s"
# monitoring-mcp/monitor_resources workspacePath="." threshold="80%"

echo "✅ All monitoring MCP checks passed"
```

---

### Complete Documentation PR Review

```bash
#!/bin/bash
echo "📚 Running Documentation MCP Analysis..."

# 1. Documentation validation
docs-mcp/validate_documentation workspacePath="." docsPath="docs/"
if [ $? -ne 0 ]; then echo "❌ Documentation validation failed"; exit 1; fi

# 2. Link checking
docs-mcp/check_links workspacePath="." docsPath="docs/"
if [ $? -ne 0 ]; then echo "❌ Broken links found"; exit 1; fi

# 3. Content quality analysis
docs-mcp/analyze_content_quality workspacePath="." docsPath="docs/"
if [ $? -ne 0 ]; then echo "❌ Content quality issues found"; exit 1; fi

# 4. Structure validation
docs-mcp/validate_structure workspacePath="." docsPath="docs/"
if [ $? -ne 0 ]; then echo "❌ Documentation structure issues found"; exit 1; fi

# 5. Accessibility compliance
docs-mcp/check_accessibility workspacePath="." docsPath="docs/"
if [ $? -ne 0 ]; then echo "❌ Accessibility compliance issues found"; exit 1; fi

# Optional: Advanced documentation features
# docs-mcp/optimize_seo workspacePath="." docsPath="docs/"
# docs-mcp/sync_translations workspacePath="." docsPath="docs/" sourceLang="en"

echo "✅ All documentation MCP checks passed"
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

# 3. Database security
database-mcp/validate_schema workspacePath="backend"
database-mcp/analyze_queries workspacePath="backend"

# 4. Frontend security
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
    "b2connect-mcp": { "disabled": false },
    "performance-mcp": { "disabled": false },
    "git-mcp": { "disabled": false },
    "docker-mcp": { "disabled": false },
    "database-mcp": { "disabled": false },
    "testing-mcp": { "disabled": false },
    "api-mcp": { "disabled": false },
    "i18n-mcp": { "disabled": false },
    "monitoring-mcp": { "disabled": false },
    "docs-mcp": { "disabled": false }
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
    *.sql) database-mcp/validate_schema workspacePath="backend" ;;
    *Entity*.cs) database-mcp/validate_multitenancy workspacePath="backend" ;;
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
- Performance MCP
- Git MCP

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
- [KB-057] Database MCP Usage Guide
- [KB-058] Testing MCP Usage Guide
- [KB-059] API Documentation MCP Usage Guide
- [KB-060] i18n MCP Usage Guide
- [KB-061] Monitoring MCP Usage Guide
- [KB-062] Documentation MCP Usage Guide
- [KB-052] Roslyn MCP Server (optional)
- [KB-063] Wolverine MCP Server (optional)
- [KB-064] Chrome DevTools MCP Server (optional)
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
