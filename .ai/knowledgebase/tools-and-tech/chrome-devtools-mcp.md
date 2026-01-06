---
docid: KB-064
title: Chrome DevTools MCP Server for E2E Testing
owner: GitHub Copilot
status: Active
---

# Chrome DevTools MCP Server for E2E Testing

**DocID**: `KB-064`
**Last Updated**: 6. Januar 2026
**Owner**: GitHub Copilot

## Overview

The Chrome DevTools MCP Server provides browser automation capabilities for end-to-end testing, visual regression testing, performance profiling, and accessibility auditing within the B2Connect frontend testing workflow.

## Status & Configuration

**Status**: Optional - Disabled by default
**Enable in**: `.vscode/mcp.json`

```json
{
  "mcpServers": {
    "chrome-devtools": {
      "disabled": false  // Change from true
    }
  }
}
```

## Available Tools

### Browser Launch (`chrome-devtools-mcp/launch`)

Launches a Chrome browser instance with DevTools protocol enabled for automation.

**Parameters**:
- `browser`: Browser type (default: `"chrome"`)
- Additional options for headless mode, window size, etc.

**Example**:
```bash
chrome-devtools-mcp/launch browser="chrome"
```

**Output**:
- Browser session ID
- WebSocket endpoint for DevTools protocol
- Browser capabilities confirmation

### Page Navigation (`chrome-devtools-mcp/navigate`)

Navigates to a specific URL in the browser.

**Parameters**:
- `url`: Target URL to navigate to
- `waitUntil`: Wait condition (`"load"`, `"domcontentloaded"`, `"networkidle"`)

**Example**:
```bash
chrome-devtools-mcp/navigate url="http://localhost:3000" waitUntil="networkidle"
```

**Output**:
- Navigation status
- Page load metrics
- Console errors/warnings

### Screenshot Capture (`chrome-devtools-mcp/screenshot`)

Captures screenshots of the current page or specific elements for visual regression testing.

**Parameters**:
- `path`: Output file path for screenshot
- `selector`: Optional CSS selector for element screenshot
- `fullPage`: Boolean for full page vs viewport screenshot

**Example**:
```bash
# Full page screenshot
chrome-devtools-mcp/screenshot path="tests/screenshots/homepage.png" fullPage=true

# Element screenshot
chrome-devtools-mcp/screenshot path="tests/screenshots/login-form.png" selector="#login-form"
```

**Output**:
- Screenshot file saved confirmation
- Image dimensions and file size
- Capture timestamp

### Lighthouse Audit (`chrome-devtools-mcp/lighthouse`)

Runs Google Lighthouse performance, accessibility, and SEO audits.

**Parameters**:
- `url`: Target URL for audit
- `categories`: Array of audit categories (`["performance", "accessibility", "seo", "best-practices"]`)

**Example**:
```bash
chrome-devtools-mcp/lighthouse url="http://localhost:3000" categories="[\"performance\",\"accessibility\"]"
```

**Output**:
- Performance score (0-100)
- Accessibility score
- SEO recommendations
- Detailed audit results

### Network Monitoring (`chrome-devtools-mcp/network-monitor`)

Monitors network requests and responses for API validation and performance analysis.

**Parameters**:
- `enable`: Boolean to start/stop monitoring
- `filter`: Optional URL pattern filter

**Example**:
```bash
# Start monitoring
chrome-devtools-mcp/network-monitor enable=true

# Monitor specific API endpoints
chrome-devtools-mcp/network-monitor enable=true filter="**/api/**"
```

**Output**:
- Request/response logs
- Response times and status codes
- Failed request analysis

## Integration with Testing Workflow

### Visual Regression Testing

```bash
#!/bin/bash
echo "📸 Visual Regression Testing..."

# Launch browser
chrome-devtools-mcp/launch browser="chrome"

# Navigate to page
chrome-devtools-mcp/navigate url="http://localhost:3000/login"

# Capture baseline screenshot
chrome-devtools-mcp/screenshot path="tests/baseline/login-page.png" fullPage=true

# Run tests that modify UI
npm run test:e2e

# Capture comparison screenshot
chrome-devtools-mcp/navigate url="http://localhost:3000/login"
chrome-devtools-mcp/screenshot path="tests/current/login-page.png" fullPage=true

# Compare screenshots (external tool)
# pixelmatch tests/baseline/login-page.png tests/current/login-page.png diff.png 0.1
```

### Performance Testing

```bash
#!/bin/bash
echo "⚡ Performance Testing..."

# Launch browser
chrome-devtools-mcp/launch browser="chrome"

# Run Lighthouse audit
chrome-devtools-mcp/lighthouse url="http://localhost:3000" categories="[\"performance\"]"

# Check scores against thresholds
# Performance >= 90
# First Contentful Paint < 1.8s
# Largest Contentful Paint < 2.5s
```

### E2E Test Preparation

```bash
#!/bin/bash
echo "🧪 E2E Test Environment Setup..."

# Start development server
npm run dev &

# Wait for server
sleep 10

# Launch browser for testing
chrome-devtools-mcp/launch browser="chrome"

# Pre-flight checks
chrome-devtools-mcp/navigate url="http://localhost:3000"
chrome-devtools-mcp/screenshot path="tests/health-check.png"

# Run test suite
npm run test:e2e

# Generate test report
chrome-devtools-mcp/lighthouse url="http://localhost:3000" categories="[\"accessibility\"]"
```

## Accessibility Testing

### Automated Accessibility Audits

```bash
#!/bin/bash
echo "♿ Accessibility Testing..."

# Launch browser
chrome-devtools-mcp/launch browser="chrome"

# Navigate to page
chrome-devtools-mcp/navigate url="http://localhost:3000"

# Run accessibility audit
chrome-devtools-mcp/lighthouse url="http://localhost:3000" categories="[\"accessibility\"]"

# Target: WCAG 2.1 Level AAA compliance
# Score threshold: >= 95
```

### Visual Accessibility Checks

```bash
# Capture screenshots for manual review
chrome-devtools-mcp/screenshot path="tests/accessibility/page-focus-states.png"

# Test keyboard navigation
# (Requires additional scripting for full keyboard navigation testing)
```

## API Integration Testing

### Network Request Validation

```bash
#!/bin/bash
echo "🔗 API Integration Testing..."

# Start network monitoring
chrome-devtools-mcp/network-monitor enable=true filter="**/api/**"

# Navigate and trigger API calls
chrome-devtools-mcp/navigate url="http://localhost:3000/products"

# Check network logs for:
# - Correct API endpoints called
# - Proper HTTP status codes
# - Response times within limits
# - Authentication headers present

# Stop monitoring
chrome-devtools-mcp/network-monitor enable=false
```

### End-to-End API Flows

```bash
# Test complete user journeys
chrome-devtools-mcp/navigate url="http://localhost:3000/login"
# Simulate user login actions
chrome-devtools-mcp/navigate url="http://localhost:3000/dashboard"
# Verify dashboard API calls succeed
```

## Performance Profiling

### Runtime Performance Analysis

```bash
#!/bin/bash
echo "📊 Performance Profiling..."

# Launch browser with performance monitoring
chrome-devtools-mcp/launch browser="chrome"

# Navigate to application
chrome-devtools-mcp/navigate url="http://localhost:3000"

# Run performance audit
chrome-devtools-mcp/lighthouse url="http://localhost:3000" categories="[\"performance\"]"

# Key metrics to monitor:
# - First Contentful Paint (FCP)
# - Largest Contentful Paint (LCP)
# - First Input Delay (FID)
# - Cumulative Layout Shift (CLS)
```

### Bundle Size Analysis

```bash
# Analyze JavaScript bundle sizes
# (Integrate with webpack-bundle-analyzer or similar)
npm run build:analyze

# Cross-reference with Lighthouse performance scores
chrome-devtools-mcp/lighthouse url="http://localhost:3000"
```

## Integration with MCP Ecosystem

### Combined Testing Workflow

```bash
#!/bin/bash
echo "🔬 Complete Testing Suite..."

# 1. Type safety validation (TypeScript MCP)
typescript-mcp/analyze_types workspacePath="frontend/Store"

# 2. Component validation (Vue MCP)
vue-mcp/validate_i18n_keys workspacePath="frontend/Store"
vue-mcp/check_responsive_design filePath="src/components/LoginForm.vue"

# 3. Security validation (Security MCP)
security-mcp/scan_xss_vulnerabilities workspacePath="frontend/Store"

# 4. Accessibility pre-check (HTML/CSS MCP)
htmlcss-mcp/check_html_accessibility workspacePath="frontend/Store" filePath="pages/index.html"

# 5. E2E testing (Chrome DevTools MCP)
chrome-devtools-mcp/launch browser="chrome"
chrome-devtools-mcp/navigate url="http://localhost:3000"
chrome-devtools-mcp/lighthouse url="http://localhost:3000"
chrome-devtools-mcp/screenshot path="tests/e2e-results.png"

echo "✅ All MCP validations passed"
```

## Troubleshooting

### Browser Launch Issues

**Chrome not found**:
- Ensure Chrome/Chromium is installed
- Check PATH environment variable
- Use absolute path to Chrome executable

**Port conflicts**:
- Kill existing Chrome processes
- Use different port for DevTools protocol
- Check for firewall blocking

### Screenshot Issues

**Blank screenshots**:
- Wait for page load completion
- Check for overlay elements blocking content
- Ensure page is fully rendered

**Element not found**:
- Verify CSS selector accuracy
- Wait for dynamic content to load
- Check for iframe context

### Performance Variations

**Inconsistent scores**:
- Run multiple times and average results
- Ensure consistent network conditions
- Disable browser extensions that affect performance
- Use clean browser profile

## Best Practices

### Test Environment Setup

- Use consistent browser versions across environments
- Configure viewport sizes for responsive testing
- Disable browser features that affect performance (extensions, caching variations)
- Use headless mode for CI/CD pipelines

### Screenshot Management

- Store baseline screenshots in version control
- Use semantic naming conventions (`login-form-empty.png`, `dashboard-loaded.png`)
- Implement pixel difference thresholds (0.1 = 10% tolerance)
- Archive historical screenshots for trend analysis

### Performance Baselines

- Establish performance budgets for each page
- Monitor trends over time, not just absolute scores
- Account for network variability in thresholds
- Document performance regression causes

## Related Documentation

- [KB-058] Testing MCP Usage Guide - Core testing framework integration
- [KB-053] TypeScript MCP Integration - Type safety validation
- [KB-054] Vue MCP Integration - Component analysis
- [KB-056] HTML/CSS MCP Usage Guide - Accessibility validation
- [GL-012] Frontend Quality Standards - Frontend testing requirements

---

**Maintained by**: GitHub Copilot
**Last Review**: 6. Januar 2026
**Next Review**: 6. Februar 2026