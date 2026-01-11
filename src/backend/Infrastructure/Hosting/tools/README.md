# MCP Servers for B2X

This directory contains Model Context Protocol (MCP) servers that enhance GitHub Copilot and other AI coding assistants with specialized analysis capabilities for the B2X project.

## Code Style Compliance

All C# tools in this directory follow the same coding standards as the main backend:

- **Configuration Inheritance**: `Directory.Build.props` imports backend standards
- **StyleCop Rules**: `stylecop.json` enforces consistent code style
- **EditorConfig**: Root `.editorconfig` applies to all C# files
- **Automatic Formatting**: Pre-commit hooks format C# code automatically

### TypeScript MCP Tools

All TypeScript-based MCP servers (TypeScriptMCP, VueMCP, HtmlCssMCP, SecurityMCP, etc.) follow consistent code style:

- **Prettier**: `tools/.prettierrc` - Consistent formatting
- **ESLint**: `tools/.eslintrc.js` - TypeScript linting rules
- **Automatic Formatting**: Pre-commit hooks format TypeScript files
- **Scripts**: Each tool has `format`, `format:check`, `lint`, `lint:fix` scripts

### Development Workflow

**Before Committing:**
```bash
# Format all code (frontend + backend + tools)
npm run format:all

# Check formatting without changes
npm run check:all
```

**Manual Formatting:**
```bash
# Format specific tool project
dotnet format tools/RoslynMCP/RoslynMCP.csproj

# Format TypeScript MCP tools
cd tools/TypeScriptMCP && npm run format

# Format all tools
npm run format:tools
```

**Pre-commit Hooks:**
Git pre-commit hooks automatically format staged files:
- ✅ C# files (`.cs`) with `dotnet format`
- ✅ TypeScript files (`.ts`, `.js`, `.json`) with Prettier
- ✅ Frontend files with Prettier + ESLint

---

## Available MCP Servers

### Local Servers (Custom Built)

#### 1. **HTML/CSS MCP** (`tools/HtmlCssMCP/`)
Analyze HTML structure, CSS styling, and accessibility.

**Status**: ✅ Active  
**Language**: TypeScript  
**Features**:
- HTML structure analysis (headings, links, images, forms)
- CSS analysis (selectors, properties, variables, media queries)
- Accessibility checking with axe-core
- Semantic HTML validation
- CSS issue detection (duplicates, !important, unused vars)

**Tools**:
- `analyze_html_structure`
- `check_html_accessibility`
- `validate_html_semantics`
- `analyze_css`
- `detect_css_issues`
- `find_html_files`
- `find_css_files`

---

#### 2. **TypeScript MCP** (`tools/TypeScriptMCP/`)
TypeScript code analysis and type checking.

**Status**: ✅ Active  
**Language**: TypeScript  
**Features**:
- Symbol search across workspace
- Type analysis and validation
- Usage tracking for refactoring
- Symbol details and documentation

**Tools**:
- `search_symbols`
- `analyze_types`
- `find_usages`
- `get_symbol_info`

---

#### 3. **Vue MCP** (`tools/VueMCP/`)
Vue.js component analysis.

**Status**: ✅ Active  
**Language**: TypeScript  
**Features**:
- Component structure analysis
- Composition API support
- Props and emits detection
- Template and script parsing

---

#### 4. **Security MCP** (`tools/SecurityMCP/`)
Security vulnerability scanning and analysis.

**Status**: ✅ Active  
**Language**: TypeScript  
**Features**:
- Dependency vulnerability scanning
- Security best practices validation
- OWASP compliance checking

---

#### 5. **Wolverine MCP** (`tools/WolverineMCP/`)
Wolverine CQRS pattern analysis for .NET backend.

**Status**: 🔄 Development  
**Language**: C# (.NET)  
**Features**:
- Handler analysis
- Query optimization
- Dependency injection validation

---

#### 6. **Roslyn MCP** (`tools/RoslynMCP/`)
C# and .NET code analysis using Roslyn.

**Status**: ⏸️ Disabled  
**Language**: C# (.NET)  
**Features**:
- C# code navigation
- Symbol analysis
- Syntax tree inspection

---

### External Servers (NPM Packages)

#### 7. **Chrome DevTools MCP** (External Package)
Official Chrome DevTools Protocol server for browser automation and testing.

**Status**: 🔄 Available (disabled by default)  
**Package**: [`chrome-devtools-mcp`](https://www.npmjs.com/package/chrome-devtools-mcp)  
**Repository**: [ChromeDevTools/chrome-devtools-mcp](https://github.com/ChromeDevTools/chrome-devtools-mcp)  
**Version**: Latest from npm  

**Features**:
- 🌐 Browser automation and control
- 🐛 Full Chrome DevTools Protocol access
- ⚡ Performance profiling and traces
- 🔍 Element inspection and manipulation
- 📸 Screenshots and PDF generation
- 🌍 Network monitoring and debugging
- ♿ Accessibility audits
- 📱 Device emulation and responsive testing
- 🔐 Security analysis

**Key Tools** (40+ total):
- Navigation: `navigate_page`, `new_page`, `close_page`, `list_pages`
- Input: `click`, `fill`, `hover`, `press_key`, `drag`
- Analysis: `snapshot_page`, `get_console_logs`, `get_accessibility_tree`
- Performance: `record_trace`, `get_web_vitals`, `analyze_performance_insights`
- Network: `intercept_network`, `set_cookies`, `clear_cache`
- Debugging: `evaluate_js`, `get_element_details`, `highlight_element`

**Use Cases**:
- E2E testing automation
- Performance debugging
- Accessibility compliance testing
- Visual regression testing
- Network traffic analysis
- Cross-browser testing preparation

**Installation**:
The server is pre-configured in `.vscode/mcp.json`. To enable it:
```json
{
  "chrome-devtools": {
    "disabled": false  // Change to false to enable
  }
}
```

**Usage Example**:
```typescript
// Check page performance
chrome-devtools/record_trace
{
  url: "https://localhost:3000",
  deviceType: "desktop"
}

// Test accessibility
chrome-devtools/snapshot_page
{
  includeAccessibilityTree: true
}

// Automate testing
chrome-devtools/click
{
  selector: "button.submit",
  waitForNavigation: true
}
```

**Documentation**: See [Chrome DevTools MCP Docs](https://github.com/ChromeDevTools/chrome-devtools-mcp#readme)

---

## Configuration

All MCP servers are configured in [.vscode/mcp.json](.vscode/mcp.json).

### Enable/Disable Servers

Set `"disabled": true` to disable a server, or `"disabled": false` to enable it.

### Current Configuration

```json
{
  "mcpServers": {
    "B2X-admin": { "disabled": false },
    "roslyn-code-navigator": { "disabled": true },
    "typescript-mcp": { "disabled": false },
    "vue-mcp": { "disabled": false },
    "wolverine-mcp": { "disabled": true },
    "security-mcp": { "disabled": false },
    "htmlcss-mcp": { "disabled": false },
    "chrome-devtools": { "disabled": true }
  }
}
```

## Usage with GitHub Copilot

### In VS Code

1. Ensure MCP servers are enabled in `.vscode/mcp.json`
2. GitHub Copilot will automatically detect and use available tools
3. Use natural language to request analysis:
   - "Check accessibility of this HTML file"
   - "Analyze CSS for optimization opportunities"
   - "Test performance of the store page"
   - "Find all TypeScript type errors"

### Example Prompts

#### HTML/CSS Analysis
```
Analyze the accessibility of frontend/Store/pages/index.html
Find all CSS variables in main.css and check for unused ones
Validate semantic HTML structure in the checkout page
```

#### Browser Testing (Chrome DevTools)
```
Test the performance of http://localhost:3000
Take a screenshot of the mobile view
Check Web Vitals for the product page
Audit accessibility compliance
```

#### TypeScript Analysis
```
Find all usages of the UserProfile type
Analyze type safety in the auth module
Search for components using the useStore composable
```

#### Security Scanning
```
Scan dependencies for vulnerabilities
Check OWASP compliance in authentication code
Validate input sanitization
```

## Development

### Building Local Servers

#### TypeScript Servers
```bash
cd tools/[ServerName]
npm install
npm run build
```

#### .NET Servers
```bash
cd tools/[ServerName]
dotnet build
```

### Testing

Use the MCP Inspector to test servers:
```bash
npx @modelcontextprotocol/inspector node tools/[ServerName]/dist/index.js
```

### Adding a New Server

1. Create directory in `tools/[NewServerName]/`
2. Implement MCP protocol (see existing servers as examples)
3. Add configuration to `.vscode/mcp.json`
4. Document features and tools
5. Update this README

## Architecture

### MCP Protocol

MCP (Model Context Protocol) allows AI assistants to:
- Discover available tools dynamically
- Call tools with typed parameters
- Receive structured responses
- Maintain context across calls

### Server Types

**Stdio Servers** (Most common):
- Communicate via stdin/stdout
- Launched by AI assistant on-demand
- Examples: TypeScript, Vue, HTML/CSS, Chrome DevTools

**.NET Servers**:
- HTTP-based communication
- Persistent processes
- Examples: Wolverine, Roslyn, B2X Admin

## Performance Considerations

### Token Optimization

MCP servers are designed to be token-efficient:
- Tools return structured, minimal data
- Support filtering and pagination
- Avoid unnecessary context

### Resource Management

- Servers start on-demand
- Automatic cleanup after use
- Chrome DevTools: Reuses browser instances
- TypeScript: Incremental compilation

## Security

### Validation

All servers implement:
- Input validation
- Path traversal protection
- Length limits on inputs
- Workspace boundary enforcement

### Isolation

- Servers run in separate processes
- Limited file system access
- No network access (except Chrome DevTools)

## Troubleshooting

### Server Won't Start

1. Check logs in VS Code Output panel
2. Verify dependencies are installed
3. Ensure correct Node.js/npm versions
4. Try rebuilding: `npm run build`

### Tool Not Found

1. Verify server is enabled in `.vscode/mcp.json`
2. Restart VS Code
3. Check server logs for errors

### Chrome DevTools Issues

1. Ensure Chrome is installed
2. Check Chrome version (stable recommended)
3. Try `npx chrome-devtools-mcp@latest --help` in terminal
4. See [Chrome DevTools troubleshooting](https://github.com/ChromeDevTools/chrome-devtools-mcp/blob/main/docs/troubleshooting.md)

## References

- [Model Context Protocol Specification](https://spec.modelcontextprotocol.io/)
- [MCP SDK for TypeScript](https://github.com/modelcontextprotocol/typescript-sdk)
- [Chrome DevTools Protocol](https://chromedevtools.github.io/devtools-protocol/)
- [GitHub Copilot MCP Documentation](https://code.visualstudio.com/docs/copilot/chat/mcp-servers)

## Maintainers

- **@Frontend**: TypeScript, Vue, HTML/CSS, Chrome DevTools servers
- **@Backend**: Wolverine, Roslyn servers
- **@Security**: Security MCP server
- **@CopilotExpert**: MCP configuration and integration

## License

See individual server directories for license information.
- Local servers: Project license applies
- Chrome DevTools MCP: Apache 2.0
