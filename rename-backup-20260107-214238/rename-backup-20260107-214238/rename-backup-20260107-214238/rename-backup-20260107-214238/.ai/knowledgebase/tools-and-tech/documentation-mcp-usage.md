# Documentation MCP Usage Guide

**DocID**: `KB-062`  
**Title**: Documentation MCP Usage Guide  
**Owner**: @DocMaintainer  
**Status**: Active  
**Last Updated**: 6. Januar 2026

---

## Overview

The Documentation MCP (Model Context Protocol) server provides comprehensive documentation validation and quality assurance for B2X. It ensures documentation accuracy, completeness, accessibility, and maintainability across all documentation formats and languages.

**Documentation Scope**: Markdown, reStructuredText, plain text, AsciiDoc, and other documentation formats

---

## Core Features

### Documentation Validation
Validates documentation structure, syntax, and compliance with standards.

```bash
# Validate all documentation
docs-mcp/validate_documentation workspacePath="." docsPath="docs/"

# Validate specific documentation file
docs-mcp/validate_documentation workspacePath="." docsPath="docs/guides/getting-started.md"
```

### Link Checking
Verifies all internal and external links are valid and accessible.

```bash
# Check all links in documentation
docs-mcp/check_links workspacePath="." docsPath="docs/"

# Check links in specific file
docs-mcp/check_links workspacePath="." docsPath="README.md"
```

### Content Quality Analysis
Analyzes documentation content for clarity, completeness, and quality.

```bash
# Analyze content quality
docs-mcp/analyze_content_quality workspacePath="." docsPath="docs/"

# Analyze specific section
docs-mcp/analyze_content_quality workspacePath="." docsPath="docs/api/" minQualityScore="80"
```

### Structure Validation
Validates documentation structure and organization.

```bash
# Validate documentation structure
docs-mcp/validate_structure workspacePath="." docsPath="docs/"

# Validate with custom rules
docs-mcp/validate_structure workspacePath="." docsPath="docs/" rulesFile="docs/.docstructure.yml"
```

### Accessibility Compliance
Ensures documentation meets accessibility standards for all users.

```bash
# Check accessibility compliance
docs-mcp/check_accessibility workspacePath="." docsPath="docs/"

# Check specific accessibility guidelines
docs-mcp/check_accessibility workspacePath="." docsPath="docs/" standard="WCAG2.1"
```

### SEO Optimization (Optional)
Optimizes documentation for search engine discoverability.

```bash
# Optimize for SEO
docs-mcp/optimize_seo workspacePath="." docsPath="docs/"

# Generate SEO report
docs-mcp/optimize_seo workspacePath="." docsPath="docs/" generateReport=true
```

### Translation Sync (Optional)
Synchronizes documentation translations with source content.

```bash
# Sync documentation translations
docs-mcp/sync_translations workspacePath="." docsPath="docs/" sourceLang="en"

# Sync specific language
docs-mcp/sync_translations workspacePath="." docsPath="docs/" sourceLang="en" targetLang="de"
```

---

## Integration with Development Workflow

### Pre-Commit Documentation Validation

```bash
#!/bin/bash
# Run before committing documentation changes

# 1. Documentation validation (MANDATORY)
docs-mcp/validate_documentation workspacePath="." docsPath="docs/"
if [ $? -ne 0 ]; then
  echo "❌ Documentation validation failed"
  exit 1
fi

# 2. Link checking
docs-mcp/check_links workspacePath="." docsPath="docs/"
if [ $? -ne 0 ]; then
  echo "❌ Broken links found"
  exit 1
fi

# 3. Content quality analysis
docs-mcp/analyze_content_quality workspacePath="." docsPath="docs/"
if [ $? -ne 0 ]; then
  echo "❌ Content quality issues found"
  exit 1
fi

# 4. Structure validation
docs-mcp/validate_structure workspacePath="." docsPath="docs/"
if [ $? -ne 0 ]; then
  echo "❌ Documentation structure issues found"
  exit 1
fi

# 5. Accessibility compliance
docs-mcp/check_accessibility workspacePath="." docsPath="docs/"
if [ $? -ne 0 ]; then
  echo "❌ Accessibility compliance issues found"
  exit 1
fi

echo "✅ Documentation validation passed"
```

### CI/CD Integration

```yaml
# .github/workflows/docs-validation.yml
name: Documentation Validation

on:
  pull_request:
    paths:
      - 'docs/**'
      - 'README.md'
      - '**/*.md'

jobs:
  docs-check:
    runs-on: ubuntu-latest
    steps:
      - name: Checkout code
        uses: actions/checkout@v4

      - name: Run documentation MCP validation
        run: |
          docs-mcp/validate_documentation workspacePath="." docsPath="docs/"
          docs-mcp/check_links workspacePath="." docsPath="docs/"
          docs-mcp/analyze_content_quality workspacePath="." docsPath="docs/"
          docs-mcp/validate_structure workspacePath="." docsPath="docs/"
          docs-mcp/check_accessibility workspacePath="." docsPath="docs/"
```

### Documentation Standards

#### File Naming Conventions

```bash
# Good naming patterns
docs/
├── guides/
│   ├── getting-started.md
│   ├── installation.md
│   └── configuration.md
├── api/
│   ├── authentication.md
│   ├── endpoints.md
│   └── examples.md
└── architecture/
    ├── overview.md
    └── components.md
```

#### Content Structure Standards

```markdown
# Document Title

**DocID**: [Unique identifier]  
**Status**: [Draft | Review | Approved | Deprecated]  
**Last Updated**: [Date]  
**Owner**: [@Team or person]

---

## Overview

Brief description of the document's purpose and scope.

## Sections

### Subsections

Detailed content with proper heading hierarchy.

## References

Links to related documentation and resources.

---

**Maintained by**: [@Owner]  
**Last Review**: [Date]  
**Next Review**: [Date]
```

---

## Quality Metrics and Standards

### Content Quality Scores

| Metric | Target | Description |
|--------|--------|-------------|
| Readability | >70 | Flesch Reading Ease score |
| Completeness | 100% | All required sections present |
| Accuracy | 100% | Technical information verified |
| Consistency | >90% | Style and terminology consistent |
| Accessibility | WCAG 2.1 AA | Meets accessibility standards |

### Link Validation Rules

- **Internal links**: Must resolve to existing files
- **External links**: Must return HTTP 200-299 status codes
- **Anchors**: Must exist in target documents
- **Images**: Must be accessible and have alt text
- **References**: Must be up-to-date and accurate

### Structure Validation Rules

```yaml
# docs/.docstructure.yml
rules:
  - name: "Required sections"
    required: ["Overview", "References"]
    level: "error"

  - name: "Heading hierarchy"
    maxDepth: 4
    level: "warning"

  - name: "File naming"
    pattern: "^[a-z0-9-]+\\.md$"
    level: "error"

  - name: "Metadata completeness"
    requiredFields: ["DocID", "Status", "Last Updated", "Owner"]
    level: "error"
```

---

## Best Practices

### Documentation Writing Guidelines

#### Clear and Concise Language

```markdown
<!-- ✅ GOOD: Clear and direct -->
To install the package, run the following command:

```bash
npm install B2X
```

<!-- ❌ BAD: Unclear and verbose -->
In order to get the package installed on your system, you will need to execute the installation command that is provided below. This command should be run in your terminal application.
```

#### Consistent Terminology

```markdown
<!-- Use consistent terms throughout -->
- Use "user" instead of "customer", "client", "person"
- Use "component" instead of "module", "part", "element"
- Use "configure" instead of "setup", "initialize", "prepare"
```

#### Proper Code Examples

```markdown
<!-- ✅ GOOD: Complete, runnable examples -->
```typescript
// src/services/api.ts
import { ApiClient } from '@B2X/core'

export class ProductService {
  private client: ApiClient

  constructor(baseUrl: string) {
    this.client = new ApiClient(baseUrl)
  }

  async getProducts(category?: string): Promise<Product[]> {
    const response = await this.client.get('/products', { category })
    return response.data
  }
}
```

<!-- ❌ BAD: Incomplete or incorrect examples -->
```typescript
// Incomplete example
const products = api.get('/products')
```
```

### Documentation Maintenance

#### Regular Reviews

```bash
# Monthly documentation review
docs-mcp/validate_documentation workspacePath="." docsPath="docs/" reviewCycle="monthly"

# Check for outdated content
docs-mcp/analyze_content_quality workspacePath="." docsPath="docs/" checkFreshness=true
```

#### Version Synchronization

```bash
# Sync documentation with code changes
docs-mcp/sync_translations workspacePath="." docsPath="docs/" sourceLang="en"

# Update API documentation
docs-mcp/validate_documentation workspacePath="." docsPath="docs/api/" syncWithCode=true
```

### Accessibility Best Practices

#### Semantic HTML in Markdown

```markdown
<!-- ✅ GOOD: Semantic structure -->
# Main Heading

## Section Heading

### Subsection

**Bold text** for emphasis.
*Italic text* for stress.

- Unordered list item
- Another item

1. Ordered list item
2. Another item

> Blockquote for important notes

```code
Code blocks for examples
```

<!-- ❌ BAD: Poor structure -->
**EVERYTHING IN BOLD**

- item
- item

> note
```

#### Alt Text for Images

```markdown
<!-- ✅ GOOD: Descriptive alt text -->
![B2X architecture diagram showing frontend, backend, and database layers](docs/images/architecture.png)

<!-- ❌ BAD: Missing or poor alt text -->
![Image](diagram.png)
![](architecture.png)
```

#### Color and Contrast

- Use sufficient color contrast (WCAG AA: 4.5:1 minimum)
- Don't rely on color alone for conveying information
- Provide text alternatives for color-coded information

---

## Troubleshooting

### Common Documentation Issues

#### Broken Links
```bash
# Find broken links
docs-mcp/check_links workspacePath="." docsPath="docs/" verbose=true

# Output shows which links are broken and why
```

#### Content Quality Issues
```bash
# Analyze content quality with details
docs-mcp/analyze_content_quality workspacePath="." docsPath="docs/" detailedReport=true

# Check specific quality metrics
docs-mcp/analyze_content_quality workspacePath="." docsPath="docs/" metrics="readability,completeness"
```

#### Structure Problems
```bash
# Validate structure with suggestions
docs-mcp/validate_structure workspacePath="." docsPath="docs/" showSuggestions=true

# Check against custom rules
docs-mcp/validate_structure workspacePath="." docsPath="docs/" rulesFile="docs/.docstructure.yml"
```

#### Accessibility Violations
```bash
# Detailed accessibility report
docs-mcp/check_accessibility workspacePath="." docsPath="docs/" reportFormat="detailed"

# Check specific guidelines
docs-mcp/check_accessibility workspacePath="." docsPath="docs/" guidelines="WCAG2.1,Section508"
```

### Performance Optimization

#### Link Checking Optimization

```bash
# Cache external link checks
docs-mcp/check_links workspacePath="." docsPath="docs/" cacheResults=true cacheExpiry="24h"

# Parallel link checking
docs-mcp/check_links workspacePath="." docsPath="docs/" parallel=true maxWorkers="10"
```

#### Content Analysis Optimization

```bash
# Incremental quality analysis
docs-mcp/analyze_content_quality workspacePath="." docsPath="docs/" incremental=true

# Focus on changed files
docs-mcp/analyze_content_quality workspacePath="." docsPath="docs/" changedOnly=true
```

---

## Configuration

### MCP Server Configuration

```json
// .vscode/mcp.json
{
  "mcpServers": {
    "docs-mcp": {
      "disabled": false,
      "config": {
        "docsPath": "docs/",
        "supportedFormats": ["md", "rst", "txt", "adoc"],
        "qualityThreshold": 80,
        "linkCheckTimeout": "30s",
        "accessibilityStandard": "WCAG2.1",
        "seoOptimization": false,
        "translationSync": false
      }
    }
  }
}
```

### Documentation Configuration

```yaml
# docs/.docsconfig.yml
documentation:
  standards:
    markdown: true
    accessibility: "WCAG2.1"
    seo: false

  quality:
    minReadability: 70
    requireMetadata: true
    checkLinks: true
    validateStructure: true

  paths:
    guides: "docs/guides/"
    api: "docs/api/"
    architecture: "docs/architecture/"
    examples: "docs/examples/"

  languages:
    - en
    - de
    - fr
    - es
```

### CI/CD Configuration

```yaml
# .github/workflows/docs-pr.yml
name: Documentation PR Checks

on:
  pull_request:
    paths:
      - 'docs/**'
      - '**/*.md'

jobs:
  docs-validation:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - name: Validate Documentation
        run: docs-mcp/validate_documentation workspacePath="." docsPath="docs/"

      - name: Check Links
        run: docs-mcp/check_links workspacePath="." docsPath="docs/"

      - name: Analyze Content Quality
        run: docs-mcp/analyze_content_quality workspacePath="." docsPath="docs/"

      - name: Validate Structure
        run: docs-mcp/validate_structure workspacePath="." docsPath="docs/"

      - name: Check Accessibility
        run: docs-mcp/check_accessibility workspacePath="." docsPath="docs/"
```

---

## Integration Examples

### MkDocs Integration

```yaml
# mkdocs.yml
site_name: B2X Documentation
docs_dir: docs/

plugins:
  - search
  - mermaid2
  - macros

theme:
  name: material
  features:
    - navigation.tabs
    - navigation.sections
    - toc.integrate
    - search.suggest
    - search.highlight

extra:
  docs-mcp:
    validation: true
    quality_check: true
    link_checking: true
```

### Docusaurus Integration

```javascript
// docusaurus.config.js
module.exports = {
  title: 'B2X',
  url: 'https://docs.B2X.com',

  plugins: [
    [
      '@docusaurus/plugin-content-docs',
      {
        id: 'docs',
        path: 'docs',
        routeBasePath: 'docs',
        editUrl: 'https://github.com/B2X/docs/edit/main/',
        sidebarPath: require.resolve('./sidebars.js'),

        // Documentation MCP integration
        docsMcp: {
          validateOnBuild: true,
          checkLinks: true,
          analyzeQuality: true,
          accessibilityCheck: true
        }
      }
    ]
  ]
}
```

### Custom Validation Scripts

```bash
#!/bin/bash
# docs/scripts/validate-docs.sh

set -e

echo "🔍 Validating documentation..."

# Basic validation
docs-mcp/validate_documentation workspacePath="." docsPath="docs/"

# Link checking with retry
max_attempts=3
attempt=1
while [ $attempt -le $max_attempts ]; do
  if docs-mcp/check_links workspacePath="." docsPath="docs/"; then
    break
  else
    echo "Link check failed (attempt $attempt/$max_attempts)"
    attempt=$((attempt + 1))
    sleep 5
  fi
done

if [ $attempt -gt $max_attempts ]; then
  echo "❌ Link validation failed after $max_attempts attempts"
  exit 1
fi

# Quality analysis
docs-mcp/analyze_content_quality workspacePath="." docsPath="docs/" minQualityScore="75"

echo "✅ Documentation validation complete"
```

---

## Related Documentation

- [KB-060] i18n MCP Usage Guide
- [KB-061] Monitoring MCP Usage Guide
- [GL-008] Governance Policies
- [DOC-001] Quick Start Guide

---

**Maintained by**: @DocMaintainer  
**Last Review**: 6. Januar 2026  
**Next Review**: 6. Februar 2026</content>
<parameter name="filePath">/Users/holger/Documents/Projekte/B2X/.ai/knowledgebase/tools-and-tech/documentation-mcp-usage.md