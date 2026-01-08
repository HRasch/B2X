# Markdown Fragment Editing Strategy

## Overview
Token-optimized approach for editing large markdown files using intelligent content extraction and fragment-based reading.

## Strategy Components

### 1. Intelligent Content Extraction
- **Frontmatter Preservation**: Complete metadata headers (docid, title, owner, etc.)
- **Structural Headers**: All H1/H2 headers with context lines
- **Content Sampling**: Representative sections from middle of document
- **End Sections**: Recent additions and conclusions

### 2. Token Optimization
- **99%+ Savings**: From ~44K tokens to ~400 tokens for 172KB files
- **Context Preservation**: Maintains document structure and key information
- **Edit Readiness**: Sufficient context for AI-assisted modifications

### 3. Implementation Tools

#### markdown-fragment-reader.sh
```bash
# Usage: Extract fragments from large markdown files
./scripts/markdown-fragment-reader.sh <file-path> [max-lines]

# Example: Process lessons.md with 100 line limit
./scripts/markdown-fragment-reader.sh .ai/knowledgebase/lessons.md 100
```

#### Integration with Existing Workflows
- Compatible with GL-043 Multi-Language Fragment Editing Strategy
- Works alongside Roslyn, TypeScript, Vue MCP tools
- Integrates with documentation quality monitoring

## Usage Guidelines

### When to Apply
- Markdown files >50KB
- Documentation with frontmatter metadata
- Knowledge bases and lesson repositories
- Files with >1000 lines

### File Size Thresholds
- **< 50KB**: Use full file reading (sufficient context)
- **50KB - 200KB**: Fragment reading with 100-150 lines
- **> 200KB**: Fragment reading with 50-100 lines + intelligent sampling

### Content Preservation Rules
1. **Always include**: Frontmatter, major headers, end sections
2. **Sample from**: Middle content sections proportionally
3. **Omit**: Repetitive content, long code blocks, extensive lists
4. **Preserve**: Document structure, key insights, recent additions

## Quality Assurance

### Validation Checks
- Frontmatter integrity maintained
- Major sections represented
- Document structure preserved
- Edit context sufficient

### Performance Metrics
- Token savings: Target 95-99%
- Processing time: <2 seconds for files up to 500KB
- Memory usage: Minimal (bash text processing)

## Integration Points

### With Documentation MCP (NEW)
The markdown fragment extraction is now available as an MCP tool:

```typescript
// AI can directly call MCP tool for fragment extraction
documentation-mcp/extract_markdown_fragment filePath=".ai/knowledgebase/lessons.md" workspacePath="." maxLines=100
```

**MCP Tool Parameters:**
- `filePath`: Path to markdown file (relative to workspace)
- `workspacePath`: Workspace root directory
- `maxLines`: Maximum lines in fragment (default: 100, range: 50-500)
- `includeHeaders`: Include major headers with context (default: true)
- `includeFrontmatter`: Include YAML frontmatter (default: true)
- `sampleContent`: Include representative content samples (default: true)

**Benefits:**
1. **Programmatic Access**: AI agents can call fragment extraction directly
2. **Consistent Interface**: Unified MCP tool interface across all documentation operations
3. **Enhanced Workflows**: Enables advanced AI-assisted editing scenarios
4. **Automatic Token Tracking**: MCP server reports token savings in response

### Manual Script Usage
For standalone usage outside MCP workflows, use the bash script:

```bash
./scripts/markdown-fragment-reader.sh <file-path> [max-lines]
```

### With Quality Gates
- Use fragment content for AI analysis
- Maintain full file access for validation
- Support incremental updates

### With Quality Gates
- Pre-edit: Fragment extraction and backup
- Post-edit: Full file validation and integrity checks
- CI/CD: Automated fragment validation in pipelines

## Example Output

For a 172KB markdown file with 4945 lines:

```
🔍 Markdown Fragment Reader
File: .ai/knowledgebase/lessons.md
Total lines: 4945
Full file tokens: ~43985

📄 Extracted Content:
====================
--- frontmatter ---
## Major Sections:
Line 17: ## Session: Header 1
Line 49: ## Session: Header 2
[... headers with context ...]

## Content Sample:
[... representative content ...]
... [Content continues for X more lines] ...

## End Section:
[... recent additions ...]

💰 Token Savings: 99%
```

## Benefits

- **Massive Token Reduction**: 99%+ savings for large documentation
- **Context Preservation**: Intelligent extraction maintains document value
- **Edit Efficiency**: AI can work with focused, relevant content
- **Scalability**: Handles documentation-heavy projects effectively

## Future Enhancements

- **Semantic Analysis**: AI-powered content importance scoring
- **User Preferences**: Configurable extraction rules
- **Version Control**: Git-aware fragment selection
- **Multi-file**: Cross-document reference preservation