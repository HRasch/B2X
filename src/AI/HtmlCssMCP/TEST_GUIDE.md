# HTML/CSS MCP Server - Test & Usage Guide

## Quick Test

To verify the MCP server is working, you can test it with the included sample files.

### Test Files
- `test-sample.html` - Sample HTML with semantic elements, forms, and accessibility features
- `test-sample.css` - Sample CSS with variables, media queries, and intentional issues for testing

### Running Tests

#### 1. Test HTML Structure Analysis
```typescript
// Use GitHub Copilot to run:
htmlcss-mcp/analyze_html_structure
{
  workspacePath: "tools/HtmlCssMCP",
  filePath: "tools/HtmlCssMCP/test-sample.html"
}
```

Expected output:
- Document type: HTML5
- Language: en
- Title, headings, links, images
- Semantic elements: header, nav, main, section, footer

#### 2. Test HTML Accessibility
```typescript
htmlcss-mcp/check_html_accessibility
{
  workspacePath: "tools/HtmlCssMCP",
  filePath: "tools/HtmlCssMCP/test-sample.html"
}
```

Expected output:
- Accessibility violations (if any)
- WCAG compliance status
- Remediation suggestions

#### 3. Test HTML Semantic Validation
```typescript
htmlcss-mcp/validate_html_semantics
{
  workspacePath: "tools/HtmlCssMCP",
  filePath: "tools/HtmlCssMCP/test-sample.html"
}
```

Expected output:
- Semantic issues
- Best practice recommendations

#### 4. Test CSS Analysis
```typescript
htmlcss-mcp/analyze_css
{
  workspacePath: "tools/HtmlCssMCP",
  filePath: "tools/HtmlCssMCP/test-sample.css"
}
```

Expected output:
- Total rules: ~15
- CSS variables: --primary-color, --secondary-color, --unused-variable, --spacing
- Colors used: #007bff, #6c757d, #ff0000, etc.
- Media queries: @media (max-width: 768px)

#### 5. Test CSS Issue Detection
```typescript
htmlcss-mcp/detect_css_issues
{
  workspacePath: "tools/HtmlCssMCP",
  filePath: "tools/HtmlCssMCP/test-sample.css"
}
```

Expected issues:
- Duplicate selector: `header` appears twice
- !important usage on `header` color
- Unused variable: `--unused-variable`

## Usage in Real Projects

### Frontend Store Analysis
```typescript
// Find all HTML files
htmlcss-mcp/find_html_files
{
  workspacePath: "frontend/Store"
}

// Analyze specific component
htmlcss-mcp/analyze_html_structure
{
  workspacePath: "frontend/Store",
  filePath: "frontend/Store/pages/index.html"
}

// Check accessibility
htmlcss-mcp/check_html_accessibility
{
  workspacePath: "frontend/Store",
  filePath: "frontend/Store/pages/checkout.html"
}
```

### CSS Optimization Workflow
```typescript
// 1. Find all CSS files
htmlcss-mcp/find_css_files
{
  workspacePath: "frontend/Store",
  includePattern: "**/*.css"
}

// 2. Analyze each file
htmlcss-mcp/analyze_css
{
  workspacePath: "frontend/Store",
  filePath: "frontend/Store/assets/styles/main.css"
}

// 3. Detect issues
htmlcss-mcp/detect_css_issues
{
  workspacePath: "frontend/Store",
  filePath: "frontend/Store/assets/styles/main.css"
}
```

## Integration with Code Reviews

Use the MCP server during code reviews to:

1. **Accessibility Compliance**: Run accessibility checks on all changed HTML files
2. **Semantic HTML**: Validate proper use of semantic elements
3. **CSS Quality**: Check for duplicate selectors, unused variables, and optimization opportunities
4. **Best Practices**: Get recommendations aligned with modern web standards

## Troubleshooting

### Server Not Starting
```bash
# Check if build succeeded
cd tools/HtmlCssMCP
npm run build

# Check for errors in logs
# The server logs to stderr
```

### Path Issues
- Ensure workspace paths are relative (no leading `/`)
- File paths must be within the workspace directory
- No `..` path traversal allowed

### Parse Errors
- Verify HTML/CSS files are well-formed
- Check file encoding (should be UTF-8)
- Look for syntax errors in CSS

## Performance Tips

1. **Batch Operations**: Analyze multiple files in sequence rather than individually
2. **Focused Scans**: Use `includePattern` to limit scope when finding files
3. **Cache Results**: The MCP server doesn't cache, so consider storing analysis results

## Security Notes

- All file paths are validated to prevent directory traversal
- Input strings have length limits
- Files must be within workspace boundaries
- No arbitrary code execution

## Next Steps

1. Test the server with sample files
2. Run on real project files
3. Integrate into your workflow
4. Report any issues or suggestions

Enjoy analyzing HTML and CSS with AI assistance! 🚀
