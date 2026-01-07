# Documentation MCP Server

A Model Context Protocol (MCP) server for automated documentation generation, validation, and maintenance. This server helps maintain high-quality documentation across the development lifecycle.

## Features

### API Documentation Generation
- **Code Analysis**: Extracts documentation from JSDoc/TSDoc comments
- **Method Signatures**: Generates API references from code
- **Multiple Formats**: Supports Markdown, HTML, and JSON output
- **Coverage Reports**: Identifies undocumented code elements

### README Validation
- **Completeness Checks**: Validates required sections (description, installation, usage, etc.)
- **Best Practices**: Enforces documentation standards
- **Structure Analysis**: Checks for proper heading hierarchy and formatting
- **Content Quality**: Suggests improvements for clarity and completeness

### Documentation Updates
- **Change Detection**: Identifies documentation that needs updating based on code changes
- **Automated Updates**: Suggests documentation updates for new features
- **Consistency Checks**: Ensures documentation matches implementation

### Coverage Analysis
- **Documentation Metrics**: Calculates documentation coverage percentages
- **Gap Identification**: Finds undocumented functions, classes, and modules
- **Progress Tracking**: Monitors documentation completeness over time

### Change Log Generation
- **Git Integration**: Generates change logs from commit messages
- **Conventional Commits**: Parses semantic versioning commit messages
- **Multiple Formats**: Supports Markdown and JSON output
- **Version Tracking**: Creates release notes between tags

## Installation

```bash
cd tools/DocumentationMCP
npm install
npm run build
```

## Configuration

Add to your `.vscode/mcp.json`:

```json
{
  "mcpServers": {
    "documentation-mcp": {
      "command": "node",
      "args": [
        "tools/DocumentationMCP/dist/index.js"
      ],
      "env": {
        "NODE_ENV": "production"
      },
      "disabled": false
    }
  }
}
```

## Usage

### Generate API Documentation

```typescript
// Generate API docs from source code
documentation-mcp/generate_api_docs workspacePath="backend" outputFormat="markdown"

// Include private methods
documentation-mcp/generate_api_docs workspacePath="backend" includePrivate=true

// Generate from specific directory
documentation-mcp/generate_api_docs workspacePath="backend" sourcePath="src/api"
```

### Validate README Files

```typescript
// Validate README completeness
documentation-mcp/validate_readme workspacePath="."

// Check specific sections
documentation-mcp/validate_readme workspacePath="." checkSections=["description", "installation", "usage"]

// Validate specific README file
documentation-mcp/validate_readme workspacePath="." readmePath="docs/README.md"
```

### Update Documentation

```typescript
// Update docs based on changed files
documentation-mcp/update_documentation workspacePath="." changedFiles=["src/api/user.ts", "src/components/Login.vue"] docType="api"
```

### Check Documentation Coverage

```typescript
// Check overall documentation coverage
documentation-mcp/check_doc_coverage workspacePath="backend" coverageType="all"

// Check function documentation only
documentation-mcp/check_doc_coverage workspacePath="backend" coverageType="functions"
```

### Generate Change Logs

```typescript
// Generate change log from recent commits
documentation-mcp/generate_change_log workspacePath="." outputFormat="markdown"

// Generate from specific tag
documentation-mcp/generate_change_log workspacePath="." sinceTag="v1.0.0"
```

## Development

```bash
# Development mode
npm run dev

# Build for production
npm run build

# Run tests
npm run test

# Lint code
npm run lint
```

## Supported Languages

- **TypeScript/JavaScript**: Full JSDoc and TSDoc support
- **C#**: XML documentation comments
- **Python**: Docstring extraction
- **Multiple Output Formats**: Markdown, HTML, JSON

## Integration with B2Connect

This MCP server integrates with the B2Connect development workflow to provide:

- **Pre-commit validation**: Automatic README and documentation checks
- **PR documentation**: Ensures documentation is updated with code changes
- **Release automation**: Generates change logs for releases
- **Quality gates**: Documentation coverage requirements

## Documentation Standards

### JSDoc Comments
```javascript
/**
 * Calculates the total price including tax
 * @param {number} price - The base price
 * @param {number} taxRate - The tax rate as decimal (e.g., 0.08 for 8%)
 * @returns {number} The total price including tax
 */
function calculateTotal(price, taxRate) {
  return price * (1 + taxRate);
}
```

### README Structure
- **Title**: Project name and brief description
- **Badges**: Build status, version, license
- **Description**: What the project does
- **Installation**: How to install and set up
- **Usage**: Basic usage examples
- **API**: API reference (if applicable)
- **Contributing**: How to contribute
- **License**: License information

## Contributing

1. Follow the existing code patterns in other MCP servers
2. Add comprehensive tests for new features
3. Update this README with new capabilities
4. Ensure TypeScript strict mode compliance

## License

MIT