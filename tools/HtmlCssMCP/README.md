# HTML/CSS MCP Server

Model Context Protocol (MCP) server for HTML and CSS analysis in the B2Connect project.

## Features

### HTML Analysis
- **Structure Analysis**: Extract headings, links, images, forms, semantic elements, and metadata
- **Accessibility Checking**: Run axe-core accessibility tests to identify WCAG violations
- **Semantic Validation**: Validate HTML semantics and best practices
- **File Discovery**: Find all HTML files in the workspace

### CSS Analysis
- **Structure Analysis**: Analyze rules, selectors, properties, media queries, and variables
- **Issue Detection**: Find duplicate selectors, !important usage, unused variables
- **Color & Font Extraction**: Extract all colors and fonts used in stylesheets
- **Optimization Suggestions**: Get recommendations for improving CSS
- **File Discovery**: Find all CSS files in the workspace

## Related MCP Servers

This project includes several MCP servers for comprehensive frontend development:

### Playwright MCP (Local) ⭐ Recommended
**Official Microsoft browser automation and E2E testing:**
- **Repository**: [microsoft/playwright-mcp](https://github.com/microsoft/playwright-mcp)
- **Package**: `@playwright/mcp` (25,000+ stars)
- **Location**: `tools/PlaywrightMCP/`
- **Features**:
  - Multi-browser support (Chrome, Firefox, Safari/WebKit)
  - Browser automation and interaction
  - Screenshot and PDF generation
  - Network and console monitoring
  - E2E testing workflows
  - Performance profiling
  - Cross-browser compatibility testing

### Chrome DevTools MCP (External)
For Chrome-specific debugging and performance testing:
- **Repository**: [ChromeDevTools/chrome-devtools-mcp](https://github.com/ChromeDevTools/chrome-devtools-mcp)
- **Package**: `chrome-devtools-mcp` on npm
- **Features**:
  - Full Chrome DevTools Protocol access
  - Chrome-specific debugging features
  - Advanced performance profiling
  - Element inspection and manipulation
  - *Note: Use Playwright MCP for cross-browser automation*

### TypeScript MCP (Local)
For TypeScript code analysis:
- Symbol search and type analysis
- Usage tracking and refactoring support
- See `tools/TypeScriptMCP/`

### Vue MCP (Local)
For Vue.js component analysis:
- Component structure analysis
- Composition API support
- See `tools/VueMCP/`

### Security MCP (Local)
For security analysis:
- Vulnerability scanning
- Security best practices validation
- See `tools/SecurityMCP/`

## Installation

```bash
cd tools/HtmlCssMCP
npm install
npm run build
```

## Usage

The server is automatically registered in `.vscode/mcp.json` and can be used through GitHub Copilot.

### Available Tools

#### `analyze_html_structure`
Analyze HTML document structure.

```typescript
{
  workspacePath: "frontend/Store",
  filePath: "frontend/Store/pages/index.html"
}
```

Returns:
- Document type and language
- Page title and meta tags
- All headings with levels
- Links (internal and external)
- Images with alt text status
- Forms with input counts
- Semantic HTML5 elements used

#### `check_html_accessibility`
Run accessibility checks using axe-core.

```typescript
{
  workspacePath: "frontend/Store",
  filePath: "frontend/Store/pages/index.html"
}
```

Returns:
- Accessibility violations with severity levels
- WCAG compliance status
- Detailed remediation guidance

#### `validate_html_semantics`
Validate HTML semantic correctness.

```typescript
{
  workspacePath: "frontend/Store",
  filePath: "frontend/Store/pages/index.html"
}
```

Returns:
- Semantic issues (missing lang, multiple h1, etc.)
- Recommendations for semantic improvements

#### `analyze_css`
Analyze CSS file structure.

```typescript
{
  workspacePath: "frontend/Store",
  filePath: "frontend/Store/assets/styles/main.css"
}
```

Returns:
- Total rules count
- All selectors used
- Property usage statistics
- Media queries
- CSS variables
- Import statements
- Colors and fonts used

#### `detect_css_issues`
Detect CSS issues and get optimization suggestions.

```typescript
{
  workspacePath: "frontend/Store",
  filePath: "frontend/Store/assets/styles/main.css"
}
```

Returns:
- Issues (duplicates, !important, unused variables)
- Optimization suggestions
- Best practice recommendations

#### `find_html_files`
Find all HTML files in workspace.

```typescript
{
  workspacePath: "frontend/Store",
  includePattern: "**/*.html" // optional
}
```

#### `find_css_files`
Find all CSS files in workspace.

```typescript
{
  workspacePath: "frontend/Store",
  includePattern: "**/*.css" // optional
}
```

## Example Workflows

### Accessibility Audit
1. Find all HTML files
2. Run accessibility checks on each
3. Generate compliance report

### CSS Optimization
1. Find all CSS files
2. Analyze each for issues
3. Get optimization suggestions
4. Refactor based on recommendations

### Semantic HTML Review
1. Analyze HTML structure
2. Validate semantics
3. Implement recommended improvements

## Integration with GitHub Copilot

This MCP server enhances GitHub Copilot's capabilities for:
- Frontend code reviews
- Accessibility compliance
- CSS refactoring
- HTML best practices validation

## Dependencies

- `@modelcontextprotocol/sdk`: MCP protocol implementation
- `htmlparser2`: Fast HTML parsing
- `parse5`: HTML5 compliant parser
- `css-tree`: CSS parser and analyzer
- `postcss`: CSS transformation toolkit
- `jsdom`: DOM implementation for Node.js
- `axe-core`: Accessibility testing engine

## Development

```bash
# Build
npm run build

# Run in development mode
npm run dev

# Run tests
npm test
```

## Security

- Input validation prevents path traversal attacks
- Files are restricted to workspace directory
- String inputs have length limits
- Proper error handling for malformed HTML/CSS

## License

MIT
