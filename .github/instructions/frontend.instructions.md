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

### Security Analysis (MCP)

**Reference**: See [KB-055] Security MCP Best Practices.

#### XSS Vulnerability Scanning
```typescript
// Scan frontend code for XSS vulnerabilities
security-mcp/scan_xss_vulnerabilities workspacePath="frontend/Store"
```
- Check output encoding and sanitization
- Validate CSP (Content Security Policy) usage
- Detect unsafe DOM manipulation

#### Input Validation
```typescript
// Validate input sanitization patterns
security-mcp/validate_input_sanitization workspacePath="frontend/Store"
```
- Check form validation patterns
- Ensure proper encoding of user inputs
- Validate allowlists vs denylists

### MCP-Powered Pre-Commit Checklist

Before committing frontend code, run:
- [ ] `typescript-mcp/analyze_types` - Zero type errors
- [ ] `vue-mcp/validate_i18n_keys` - No hardcoded strings
- [ ] `vue-mcp/check_responsive_design` - Responsive patterns valid
- [ ] `htmlcss-mcp/check_html_accessibility` - WCAG compliant
- [ ] `security-mcp/scan_xss_vulnerabilities` - No XSS issues

### Code Review Integration
- Run all MCP tools on modified files before requesting review
- MCP validation must pass before human review begins
- Document any MCP exceptions with justification in PR description
- Use MCP symbol search for component discovery and pattern validation

## Multilingual Support (i18n)

**Reference**: See [GL-042] Token-Optimized i18n Strategy for AI-efficient translation workflows.

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

