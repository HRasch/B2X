---
docid: KB-056
title: HTML/CSS MCP Usage Guide
owner: @Frontend
status: Active
---

# HTML/CSS MCP Usage Guide

**DocID**: `KB-056`  
**Last Updated**: 6. Januar 2026  
**Owner**: @Frontend

## Overview

The HTML/CSS MCP server provides automated analysis tools for HTML structure validation, CSS optimization, accessibility auditing, and semantic HTML compliance. This guide covers integration into frontend development workflows.

## MCP Server Configuration

**Location**: `tools/HtmlCssMCP/`  
**MCP Config**: `.vscode/mcp.json` (enabled by default)

```json
{
  "mcpServers": {
    "htmlcss-mcp": {
      "command": "node",
      "args": ["tools/HtmlCssMCP/dist/index.js"],
      "env": { "NODE_ENV": "production" },
      "disabled": false
    }
  }
}
```

## Available Tools

### 1. analyze_html_structure
**Purpose**: Analyze HTML document structure and semantics  
**Use Cases**:
- Validate HTML5 semantic elements
- Check heading hierarchy
- Audit meta tags and document structure
- Verify link and image attributes

**Example**:
```typescript
{
  workspacePath: "frontend/Store",
  filePath: "frontend/Store/pages/index.html"
}
```

**Returns**:
- Document type and language
- Page title and meta tags
- Heading hierarchy (h1-h6) with levels
- Links (internal vs external)
- Images with alt text status
- Forms with input counts
- Semantic HTML5 elements used
- Validation warnings

**Best Practices**:
```html
<!-- ✅ CORRECT - Semantic HTML5 -->
<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>B2Connect - Product Catalog</title>
  <meta name="description" content="Browse our product catalog">
</head>
<body>
  <header>
    <nav>...</nav>
  </header>
  
  <main>
    <article>
      <h1>Main Heading</h1>
      <section>
        <h2>Section Heading</h2>
        <p>Content...</p>
      </section>
    </article>
  </main>
  
  <footer>...</footer>
</body>
</html>

<!-- ❌ WRONG - Non-semantic -->
<div class="header">
  <div class="nav">...</div>
</div>
<div class="content">
  <div class="title">Main Heading</div>
</div>
```

---

### 2. check_html_accessibility
**Purpose**: Run axe-core accessibility audits for WCAG compliance  
**Use Cases**:
- **Pre-commit validation** - catch accessibility issues early
- WCAG 2.1 Level AAA compliance verification
- Automated violation detection
- Accessibility scoring

**Example**:
```typescript
{
  workspacePath: "frontend/Store",
  filePath: "frontend/Store/pages/product-detail.html"
}
```

**Returns**:
- Accessibility violations by severity (critical, serious, moderate, minor)
- WCAG success criteria violations
- Element selectors for each issue
- Remediation suggestions
- Accessibility score

**Target**: **WCAG 2.1 Level AAA compliance**

**Common Violations**:
```html
<!-- ❌ CRITICAL - Missing alt text -->
<img src="product.jpg">

<!-- ✅ CORRECT - Descriptive alt text -->
<img src="product.jpg" alt="Blue wireless headphones">

<!-- ❌ CRITICAL - Non-descriptive link -->
<a href="/products">Click here</a>

<!-- ✅ CORRECT - Descriptive link -->
<a href="/products">View all products</a>

<!-- ❌ SERIOUS - Missing form labels -->
<input type="text" name="email">

<!-- ✅ CORRECT - Labeled form input -->
<label for="email">Email Address</label>
<input type="email" id="email" name="email" required>

<!-- ❌ SERIOUS - Low color contrast -->
<p style="color: #ccc; background: #fff;">Text</p>

<!-- ✅ CORRECT - Sufficient contrast (4.5:1 minimum) -->
<p style="color: #333; background: #fff;">Text</p>

<!-- ❌ MODERATE - Skipped heading level -->
<h1>Main Title</h1>
<h3>Subsection</h3> <!-- Skipped h2 -->

<!-- ✅ CORRECT - Proper heading hierarchy -->
<h1>Main Title</h1>
<h2>Section</h2>
<h3>Subsection</h3>
```

---

### 3. analyze_css_structure
**Purpose**: Analyze CSS for optimization opportunities and issues  
**Use Cases**:
- Detect duplicate selectors
- Find unnecessary !important usage
- Identify unused CSS variables
- Extract color and font palettes
- Get optimization recommendations

**Example**:
```typescript
{
  workspacePath: "frontend/Store",
  filePath: "frontend/Store/styles/main.css"
}
```

**Returns**:
- Total rules, selectors, and properties
- Media queries analysis
- CSS variables defined
- Duplicate selectors
- !important usage count
- Unused CSS variables
- Color palette extraction
- Font families used
- Optimization suggestions

**Optimization Patterns**:
```css
/* ❌ DUPLICATE SELECTORS */
.button { color: blue; }
.button { background: white; } /* Merge with above */

/* ✅ MERGED */
.button { 
  color: blue;
  background: white;
}

/* ❌ OVERUSE OF !important */
.text { color: red !important; }
.text { font-size: 14px !important; }

/* ✅ FIX SPECIFICITY INSTEAD */
.component .text { 
  color: red;
  font-size: 14px;
}

/* ❌ UNUSED CSS VARIABLES */
:root {
  --unused-color: #ff0000; /* Never referenced */
  --primary-color: #0066cc; /* Used throughout */
}

/* ✅ REMOVE UNUSED */
:root {
  --primary-color: #0066cc;
}

/* ❌ INCONSISTENT COLORS */
.header { background: #0066cc; }
.button { background: #0066CC; } /* Same color, different case */
.footer { background: #0067cd; } /* Almost same color */

/* ✅ USE CSS VARIABLES */
:root {
  --primary-color: #0066cc;
}
.header { background: var(--primary-color); }
.button { background: var(--primary-color); }
.footer { background: var(--primary-color); }
```

---

### 4. find_html_files
**Purpose**: Discover all HTML files in workspace  
**Use Cases**:
- Batch analysis of all HTML files
- Project-wide accessibility audits
- Structure validation across entire site

**Example**:
```typescript
{
  workspacePath: "frontend/Store"
}
```

**Returns**: List of HTML file paths

---

### 5. find_css_files
**Purpose**: Discover all CSS files in workspace  
**Use Cases**:
- Batch CSS optimization
- Consistency checks across stylesheets
- Color palette extraction

**Example**:
```typescript
{
  workspacePath: "frontend/Store"
}
```

**Returns**: List of CSS file paths

---

## Development Workflows

### HTML Development Workflow

```bash
# Step 1: Create HTML structure
# [Write HTML code]

# Step 2: Validate structure
htmlcss-mcp/analyze_html_structure \
  workspacePath="frontend/Store" \
  filePath="pages/new-page.html"

# Step 3: Check accessibility (MANDATORY)
htmlcss-mcp/check_html_accessibility \
  workspacePath="frontend/Store" \
  filePath="pages/new-page.html"

# Step 4: Fix violations
# [Address accessibility issues]

# Step 5: Re-validate until zero critical violations
```

---

### CSS Development Workflow

```bash
# Step 1: Write CSS
# [Create stylesheet]

# Step 2: Analyze structure
htmlcss-mcp/analyze_css_structure \
  workspacePath="frontend/Store" \
  filePath="styles/components.css"

# Step 3: Review findings
# - Duplicate selectors → Merge
# - Excessive !important → Fix specificity
# - Unused variables → Remove

# Step 4: Optimize
# [Refactor CSS based on recommendations]

# Step 5: Re-analyze
htmlcss-mcp/analyze_css_structure \
  workspacePath="frontend/Store" \
  filePath="styles/components.css"
```

---

### Pre-Commit Checklist

Before committing HTML/CSS changes:

```bash
# 1. HTML structure validation
htmlcss-mcp/analyze_html_structure workspacePath="frontend/Store" filePath="[file]"
# ✅ Must validate: Semantic HTML5, proper heading hierarchy

# 2. Accessibility audit (CRITICAL)
htmlcss-mcp/check_html_accessibility workspacePath="frontend/Store" filePath="[file]"
# ✅ Must pass: Zero CRITICAL violations
# ⚠️ Acceptable: MODERATE/MINOR with documented plan

# 3. CSS optimization
htmlcss-mcp/analyze_css_structure workspacePath="frontend/Store" filePath="[file]"
# ✅ Must validate: No duplicates, minimal !important
```

---

### Project-Wide Accessibility Audit

```bash
# Step 1: Find all HTML files
htmlcss-mcp/find_html_files workspacePath="frontend/Store"

# Step 2: Audit each file
for file in $(htmlcss-mcp/find_html_files workspacePath="frontend/Store"); do
  echo "Auditing: $file"
  htmlcss-mcp/check_html_accessibility \
    workspacePath="frontend/Store" \
    filePath="$file"
done

# Step 3: Generate compliance report
# Save results to .ai/compliance/accessibility-audit-$(date +%Y%m%d).md
```

---

### CSS Consistency Audit

```bash
# Step 1: Find all CSS files
htmlcss-mcp/find_css_files workspacePath="frontend/Store"

# Step 2: Extract color palettes from each
for file in $(htmlcss-mcp/find_css_files workspacePath="frontend/Store"); do
  htmlcss-mcp/analyze_css_structure \
    workspacePath="frontend/Store" \
    filePath="$file"
done

# Step 3: Consolidate colors
# Create unified color palette in CSS variables
# Replace hardcoded colors with variables
```

---

## Integration with Code Review (PRM-002)

### Automated Gates

HTML/CSS changes must pass MCP validation before review:

```bash
# Pre-review validation
echo "Running HTML/CSS MCP validation..."

# 1. Structure validation
htmlcss-mcp/analyze_html_structure workspacePath="frontend/Store" filePath="[changed-file]"

# 2. Accessibility check
htmlcss-mcp/check_html_accessibility workspacePath="frontend/Store" filePath="[changed-file]"

# 3. CSS optimization
htmlcss-mcp/analyze_css_structure workspacePath="frontend/Store" filePath="[changed-file]"

echo "✅ HTML/CSS validation passed"
```

**Policy**:
- ❌ **BLOCK PR** on CRITICAL accessibility violations
- ⚠️ **REQUIRE JUSTIFICATION** for SERIOUS violations
- ✅ **DOCUMENT** plan for MODERATE violations
- ℹ️ **TRACK** MINOR violations for future cleanup

---

## WCAG 2.1 Level AAA Compliance

### Compliance Targets

| Level | Description | B2Connect Target |
|-------|-------------|------------------|
| **A** | Minimum accessibility | ✅ 100% compliance |
| **AA** | Enhanced accessibility | ✅ 100% compliance |
| **AAA** | Highest accessibility | 🎯 Target (best effort) |

**Reference**: See [KB-027] Email Dark Mode Best Practices for accessibility patterns

---

### Common WCAG Violations and Fixes

#### 1. Color Contrast (WCAG 1.4.3 - AA, 1.4.6 - AAA)

```html
<!-- ❌ FAIL - Contrast ratio 2.5:1 (minimum 4.5:1 for AA) -->
<p style="color: #999; background: #fff;">Low contrast text</p>

<!-- ✅ PASS AA - Contrast ratio 4.5:1 -->
<p style="color: #666; background: #fff;">Acceptable contrast</p>

<!-- ✅ PASS AAA - Contrast ratio 7:1 -->
<p style="color: #333; background: #fff;">High contrast</p>
```

**Tool**: Use contrast checker in browser DevTools or online tools

---

#### 2. Alt Text for Images (WCAG 1.1.1 - A)

```html
<!-- ❌ FAIL - Missing alt -->
<img src="product.jpg">

<!-- ❌ FAIL - Generic alt -->
<img src="product.jpg" alt="image">

<!-- ✅ PASS - Descriptive alt -->
<img src="product.jpg" alt="Blue wireless headphones with noise cancellation">

<!-- ✅ PASS - Decorative image -->
<img src="decoration.svg" alt="" role="presentation">
```

---

#### 3. Form Labels (WCAG 1.3.1, 3.3.2 - A)

```html
<!-- ❌ FAIL - No label -->
<input type="email" name="email">

<!-- ❌ FAIL - Placeholder as label (not accessible) -->
<input type="email" placeholder="Email">

<!-- ✅ PASS - Explicit label -->
<label for="email">Email Address</label>
<input type="email" id="email" name="email" required>

<!-- ✅ PASS - aria-label for icon-only buttons -->
<button aria-label="Close dialog">
  <svg>...</svg>
</button>
```

---

#### 4. Heading Hierarchy (WCAG 1.3.1 - A)

```html
<!-- ❌ FAIL - Skipped heading level -->
<h1>Page Title</h1>
<h3>Subsection</h3> <!-- Skipped h2 -->

<!-- ✅ PASS - Proper hierarchy -->
<h1>Page Title</h1>
<h2>Section</h2>
<h3>Subsection</h3>
```

---

#### 5. Link Purpose (WCAG 2.4.4 - A, 2.4.9 - AAA)

```html
<!-- ❌ FAIL - Non-descriptive -->
<a href="/products">Click here</a>
<a href="/download">Read more</a>

<!-- ✅ PASS - Descriptive -->
<a href="/products">View all products</a>
<a href="/download">Download user manual (PDF, 2MB)</a>
```

---

## CSS Optimization Patterns

### Reduce Specificity, Avoid !important

```css
/* ❌ HIGH SPECIFICITY + !important */
div.container div.content p.text {
  color: blue !important;
}

/* ✅ LOWER SPECIFICITY, NO !important */
.content-text {
  color: blue;
}
```

---

### Use CSS Variables for Consistency

```css
/* ❌ HARDCODED COLORS */
.button-primary { background: #0066cc; }
.link-primary { color: #0066cc; }
.border-primary { border-color: #0066cc; }

/* ✅ CSS VARIABLES */
:root {
  --color-primary: #0066cc;
  --color-secondary: #ff6600;
  --color-success: #00cc66;
  --color-error: #cc0000;
}

.button-primary { background: var(--color-primary); }
.link-primary { color: var(--color-primary); }
.border-primary { border-color: var(--color-primary); }
```

---

### Mobile-First Responsive CSS

```css
/* ✅ MOBILE-FIRST */
.container {
  width: 100%; /* Mobile */
  padding: 1rem;
}

@media (min-width: 768px) {
  .container {
    width: 750px; /* Tablet */
    padding: 1.5rem;
  }
}

@media (min-width: 1024px) {
  .container {
    width: 960px; /* Desktop */
    padding: 2rem;
  }
}

/* ❌ DESKTOP-FIRST (avoid) */
.container {
  width: 960px; /* Desktop */
}

@media (max-width: 1024px) {
  .container { width: 750px; }
}
```

---

## Performance Considerations

### Targeted Analysis

```typescript
// ✅ EFFICIENT - Analyze specific file
htmlcss-mcp/analyze_css_structure \
  workspacePath="frontend/Store" \
  filePath="styles/components.css"

// ❌ SLOW - Find all files then analyze
// Only use when necessary for batch operations
```

---

### Incremental Validation

```bash
# During development - validate changed files
git diff --name-only | grep '.html$' | xargs -I {} \
  htmlcss-mcp/check_html_accessibility workspacePath="frontend/Store" filePath="{}"

# Pre-PR - full validation
htmlcss-mcp/check_html_accessibility workspacePath="frontend/Store"
```

---

## Integration with Vue.js (via Vue MCP)

HTML/CSS MCP complements Vue MCP for complete frontend validation:

```bash
# 1. Vue component structure (Vue MCP)
vue-mcp/analyze_vue_component filePath="src/components/ProductCard.vue"

# 2. Template HTML accessibility (HTML/CSS MCP)
# Extract template to temp HTML file and validate
htmlcss-mcp/check_html_accessibility workspacePath="frontend/Store" filePath="temp.html"

# 3. Component styles (HTML/CSS MCP)
# Extract <style> section and validate
htmlcss-mcp/analyze_css_structure workspacePath="frontend/Store" filePath="temp.css"

# 4. Responsive design (Vue MCP)
vue-mcp/check_responsive_design filePath="src/components/ProductCard.vue"
```

---

## Related Documentation

- [KB-027] - Email Dark Mode Best Practices
- [KB-053] - TypeScript MCP Integration
- [KB-054] - Vue MCP Integration Guide
- [KB-055] - Security MCP Best Practices
- [GL-042] - Token-Optimized i18n Strategy

---

## Troubleshooting

### Accessibility False Positives

```html
<!-- If MCP flags intentional design decisions: -->
<!-- Document with aria-label or hidden attribute -->

<!-- Example: Decorative icon marked as presentation -->
<svg role="presentation" aria-hidden="true">
  <path d="..."/>
</svg>

<!-- Example: Skip link for keyboard navigation -->
<a href="#main-content" class="sr-only">Skip to main content</a>
```

---

### CSS Variable Not Found

```css
/* If MCP reports unused variables but they're used via JavaScript: */
/* Document in CSS file */

:root {
  /* Used dynamically via JavaScript theme switcher */
  --theme-primary: #0066cc;
  --theme-secondary: #ff6600;
}
```

---

**Maintained by**: @Frontend  
**Last Review**: 6. Januar 2026  
**Next Review**: 6. Februar 2026
