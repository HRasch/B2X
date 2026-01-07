# Playwright MCP Server

Official Microsoft Playwright MCP server integration for B2X project.

## Overview

This server provides comprehensive browser automation capabilities using the official `@playwright/mcp` package from Microsoft.

## Features

### Browser Automation
- **Multi-Browser Support**: Chromium, Firefox, WebKit (Safari)
- **Navigation**: Navigate to URLs, click elements, fill forms
- **Interaction**: Type text, select options, upload files
- **Screenshots**: Capture full page or element screenshots
- **PDF Generation**: Generate PDFs from web pages

### Testing & Debugging
- **Console Monitoring**: Capture browser console logs
- **Network Monitoring**: Track network requests and responses
- **Element Inspection**: Query and interact with DOM elements
- **JavaScript Execution**: Run custom JavaScript in browser context

### Performance
- **Performance Profiling**: Monitor page load times
- **Resource Tracking**: Track resource loading and timing
- **Metrics Collection**: Gather performance metrics

## Installation

```bash
cd tools/PlaywrightMCP
npm install
npm run build
```

## Usage

The server is automatically configured in `.vscode/mcp.json` and can be used with:
- GitHub Copilot
- Claude Desktop
- Cursor IDE
- Any MCP-compatible AI assistant

### Available Tools

#### Navigation
- `browser_navigate` - Navigate to a URL
- `browser_click` - Click an element
- `browser_fill` - Fill form fields
- `browser_screenshot` - Take screenshots

#### Content Extraction
- `browser_extract_text` - Extract text content
- `browser_extract_links` - Extract all links
- `browser_evaluate` - Execute JavaScript

#### Testing
- `browser_wait_for` - Wait for elements or conditions
- `browser_assert` - Assert element states
- `browser_network` - Monitor network activity

## Configuration

Edit `.vscode/mcp.json` to enable/disable the server:

```json
{
  "mcpServers": {
    "playwright-mcp": {
      "command": "node",
      "args": ["tools/PlaywrightMCP/dist/index.js"],
      "disabled": false
    }
  }
}
```

## Browser Support

| Browser | Engine | Supported |
|---------|--------|-----------|
| Chrome | Chromium | ✅ |
| Edge | Chromium | ✅ |
| Firefox | Firefox | ✅ |
| Safari | WebKit | ✅ (macOS only) |

## Example Workflows

### E2E Testing
Ask Copilot: "Test the login flow on localhost:3000"

### Accessibility Audit
Ask Copilot: "Check accessibility on the homepage"

### Screenshot Generation
Ask Copilot: "Take a screenshot of the product page"

### Performance Analysis
Ask Copilot: "Measure page load performance"

## Related MCP Servers

- **Chrome DevTools MCP**: Chrome-specific debugging features
- **HTML/CSS MCP**: Static HTML/CSS analysis
- **Vue MCP**: Vue.js component analysis

## Documentation

- [Official Playwright MCP](https://github.com/microsoft/playwright-mcp)
- [Playwright Documentation](https://playwright.dev/)
- [Model Context Protocol](https://modelcontextprotocol.io/)

## Troubleshooting

### Browser Not Found
```bash
npx playwright install
```

### Permission Issues (macOS Safari)
Enable "Allow Remote Automation" in Safari's Develop menu.

### Port Conflicts
The server uses dynamic port allocation, no manual configuration needed.

## License

MIT - See root LICENSE file
