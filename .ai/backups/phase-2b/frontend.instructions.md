---
docid: UNKNOWN-120
title: Frontend.Instructions
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

---
applyTo: "src/components/**,src/pages/**,src/hooks/**,src/ui/**,**/frontend/**"
---

# Frontend Development Instructions

## Component Design
- Use functional components with hooks
- Keep components small and focused (single responsibility)
- Extract reusable logic into custom hooks
- Use proper TypeScript types for props

## State Management
- Use local state for component-specific data
- Use context/global state sparingly
- Avoid prop drilling beyond 2 levels
- Implement proper loading and error states

## Styling
- Follow project's styling conventions
- Use consistent spacing and sizing
- Ensure responsive design (mobile-first)
- Maintain accessibility standards (WCAG)

## Performance
- Implement lazy loading for routes/components
- Memoize expensive computations
- Optimize re-renders with React.memo/useMemo
- Use proper image optimization

## UX
- Provide immediate feedback on user actions
- Handle loading states gracefully
- Display meaningful error messages
- Implement proper form validation

## Testing
- Write unit tests for utility functions
- Write component tests for user interactions
- Test accessibility with automated tools
- **E2E Testing Considerations**:
  - Use `data-testid` attributes for reliable element selection in e2e tests
  - Ensure components render consistently for visual regression testing
  - Test loading states and error conditions that affect user experience
  - Validate responsive behavior across breakpoints

## MCP-Enhanced Development Tools

### TypeScript Analysis (MCP)

**Reference**: See [KB-053] TypeScript MCP Integration Guide for comprehensive usage.

#### MCP Tools for Development
- **Symbol Search**: `typescript-mcp/search_symbols` for finding components, interfaces, types
- **Type Analysis**: `typescript-mcp/analyze_types` for automated type checking
- **Usage Tracking**: `typescript-mcp/find_usages` for refactoring impact assessment
- **Symbol Details**: `typescript-mcp/get_symbol_info` for detailed type information

#### Development Workflow
```typescript
// Before implementing new components
typescript-mcp/search_symbols pattern="*Component" workspacePath="frontend/Store"

// During development - type validation
typescript-mcp/analyze_types workspacePath="frontend/Store" filePath="src/components/NewComponent.vue"

// Before refactoring - usage analysis
typescript-mcp/find_usages symbolName="UserProfile" workspacePath="frontend/Store" filePath="src/types/user.ts"
```

### Vue.js Component Analysis (MCP)

**Reference**: See [KB-054] Vue MCP Integration Guide for detailed workflows.

#### Component Development Workflow
- **Before creating**: Use `vue-mcp/analyze_vue_component` to study similar patterns
- **Structure validation**: Run `vue-mcp/find_component_usage` to check component dependencies
- **Composition API**: Analyze reactive variables, computed properties, and methods
- **Template analysis**: Validate directives and element structure

#### i18n Compliance (MANDATORY)
```typescript
// Validate all components for hardcoded strings
vue-mcp/validate_i18n_keys workspacePath="frontend/Store" componentPath="src/components/UserProfile.vue"
```
- **ZERO hardcoded strings allowed** - all text must use translation keys
- Run validation before every commit
- Following [GL-042] token-optimized i18n strategy
- Supported languages: en, de, fr, es, it, pt, nl, pl

#### Responsive Design Validation
```typescript
// Check responsive patterns and breakpoint usage
vue-mcp/check_responsive_design filePath="src/components/ProductCard.vue" workspacePath="frontend/Store"
```
- Validates Tailwind CSS responsive classes
- Ensures mobile-first design patterns
- Checks breakpoint consistency

#### Pinia Store Analysis
```typescript
// Analyze store structure and patterns
vue-mcp/analyze_pinia_store filePath="src/stores/user.ts" workspacePath="frontend/Store"
```
- Validates state, getters, and actions structure
- Checks composition patterns
- Ensures proper store organization

### HTML/CSS Quality Analysis (MCP)

**Reference**: See [KB-056] HTML/CSS MCP Usage Guide.

#### Semantic HTML Validation
```typescript
// Analyze HTML structure and semantics
htmlcss-mcp/analyze_html_structure workspacePath="frontend/Store" filePath="pages/index.html"
```
- Document type and meta tags validation
- Heading hierarchy checks
- Semantic HTML5 element usage
- Link and image alt text validation

#### Accessibility Checks (WCAG Compliance)
```typescript
// Run axe-core accessibility audits
htmlcss-mcp/check_html_accessibility workspacePath="frontend/Store" filePath="pages/index.html"
```
- **Target: WCAG 2.1 Level AAA compliance**
- Automated violation detection
- Severity-based reporting
- Must pass before PR approval

#### CSS Optimization
```typescript
// Analyze CSS structure and find issues
htmlcss-mcp/analyze_css_structure workspacePath="frontend/Store" filePath="styles/main.css"
```
- Detect duplicate selectors
- Find unnecessary !important usage
- Identify unused CSS variables
- Extract color and font palettes
- Get optimization recommendations

### i18n MCP Integration (Always Enabled)

**Reference**: See [KB-060] i18n MCP Usage Guide.

**Translation Validation**:
```typescript
// Validate translation keys across all locales
i18n-mcp/validate_translation_keys workspacePath="frontend/Store" localePath="locales"

// Check for missing translations
i18n-mcp/check_missing_translations workspacePath="frontend/Store" baseLocale="en"

// Validate translation consistency
i18n-mcp/validate_consistency workspacePath="frontend/Store"

// Analyze locale file structure
i18n-mcp/analyze_locale_files workspacePath="frontend/Store"

// Check pluralization rules
i18n-mcp/check_pluralization workspacePath="frontend/Store" locale="de"

// Validate interpolation syntax
i18n-mcp/validate_interpolation workspacePath="frontend/Store"
```

### Performance MCP for Frontend Optimization

**Reference**: See Performance MCP tools in MCP Operations Guide.

**Frontend Performance Analysis**:
```typescript
// Analyze code performance
performance-mcp/analyze_code_performance workspacePath="frontend/Store"

// Profile memory usage
performance-mcp/profile_memory_usage workspacePath="frontend/Store"

// Check bundle size
performance-mcp/check_bundle_size workspacePath="frontend/Store" buildOutput="dist"
```

### Git MCP for Frontend Development

**Reference**: See Git MCP tools in MCP Operations Guide.

**Code Quality Validation**:
```typescript
// Validate commit messages
git-mcp/validate_commit_messages workspacePath="frontend/Store" count=10

// Analyze code churn
git-mcp/analyze_code_churn workspacePath="frontend/Store" since="last-release"

// Check branch strategy
git-mcp/check_branch_strategy workspacePath="frontend/Store" branchName="feature/new-component"
```

### Chrome DevTools MCP for Frontend Testing (Optional)

**Reference**: See [KB-064] Chrome DevTools MCP Server.

**E2E Testing and Debugging**:
```typescript
// Launch browser for testing
chrome-devtools-mcp/launch browser="chrome"

// Navigate to application
chrome-devtools-mcp/navigate url="http://localhost:3000"

// Run Lighthouse audit
chrome-devtools-mcp/lighthouse url="http://localhost:3000"

// Monitor network performance
chrome-devtools-mcp/network-monitor enable=true

// Capture visual regression screenshots
chrome-devtools-mcp/screenshot path="tests/visual/homepage.png"
```

### Monitoring MCP for Frontend Observability

**Reference**: See [KB-061] Monitoring MCP Usage Guide.

**Frontend Monitoring Setup**:
```typescript
// Collect application metrics
monitoring-mcp/collect_application_metrics serviceName="store-frontend"

// Monitor system performance
monitoring-mcp/monitor_system_performance hostName="frontend-server-01"

// Track frontend errors
monitoring-mcp/track_errors serviceName="store-ui"

// Analyze client-side logs
monitoring-mcp/analyze_logs filePath="logs/frontend.log"

// Validate health checks
monitoring-mcp/validate_health_checks serviceName="frontend-services"
```

### Documentation MCP for Frontend Documentation

**Reference**: See [KB-062] Documentation MCP Usage Guide.

**Frontend Documentation Validation**:
```typescript
// Validate component documentation
docs-mcp/validate_documentation filePath="docs/components/Button.md"

// Check documentation links
docs-mcp/check_links workspacePath="docs/frontend"

// Analyze content quality
docs-mcp/analyze_content_quality filePath="README.md"

// Validate structure
docs-mcp/validate_structure workspacePath="docs"
```

### MCP-Powered Pre-Commit Checklist

Before committing frontend code, run:
- [ ] `typescript-mcp/analyze_types` - Zero type errors
- [ ] `vue-mcp/validate_i18n_keys` - No hardcoded strings
- [ ] `vue-mcp/check_responsive_design` - Responsive patterns valid
- [ ] `htmlcss-mcp/check_html_accessibility` - WCAG compliant
- [ ] `security-mcp/scan_xss_vulnerabilities` - No XSS issues
- [ ] `i18n-mcp/validate_translation_keys` - Translation keys valid
- [ ] `performance-mcp/analyze_code_performance` - Performance optimized
- [ ] `git-mcp/validate_commit_messages` - Conventional commit format
- [ ] `monitoring-mcp/validate_health_checks` - Health checks configured
- [ ] `docs-mcp/validate_documentation` - Documentation complete

### Code Review Integration
- Run all MCP tools on modified files before requesting review
- MCP validation must pass before human review begins
- Document any MCP exceptions with justification in PR description
- Use MCP symbol search for component discovery and pattern validation

## Multilingual Support (i18n)

**Reference**: See [GL-042] Token-Optimized i18n Strategy for AI-efficient translation workflows.

### Pre-Flight Checklist (REQUIRED)
Before implementing ANY user-facing text:
- [ ] **Key defined** in `locales/en.json` (English = source of truth)
- [ ] **$t() call used** - NEVER hardcoded strings
- [ ] **Namespace follows** pattern: `module.section.key`
- [ ] **Key documented** if complex interpolation

### i18n Rules
- **Never use hardcoded strings** - always use translation keys
- **English first**: Define keys in `en.json` as source of truth
- **Batch translations**: Request translations for multiple keys in single AI requests
- **Use vue-i18n properly**:
  - Import `useI18n` composable for script usage
  - Use `$t()` in templates for translation calls
  - Follow key naming: `namespace.section.key` (e.g., `auth.login.title`)
- **Supported languages**: en, de, fr, es, it, pt, nl, pl
- **Token efficiency**: Don't load all language files - work with keys only
- **Validation**: Use `scripts/i18n-check.sh` for completeness checks

### Common Mistakes to Avoid
```vue
<!-- ❌ WRONG: Hardcoded text -->
<button>Submit Order</button>

<!-- ✅ CORRECT: Translation key -->
<button>{{ $t('checkout.submit_order') }}</button>
```

```typescript
// ❌ WRONG: String in validation
return 'Email is required'

// ✅ CORRECT: Translation key
return t('validation.email_required')
```

