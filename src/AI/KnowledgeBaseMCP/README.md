# Knowledge Base MCP Server

Fast, efficient knowledge base access via MCP protocol. Reduces token usage by ~90% compared to loading full KB articles as attachments.

## Features

- **Semantic Search**: Full-text search with relevance scoring
- **Document Retrieval**: Get full articles by DocID (KB-053, ADR-001, etc.)
- **Category Filtering**: Browse by tools, patterns, architecture, best-practices
- **Quick References**: Get key points with links to detailed docs
- **Lessons Learned**: Search for solutions to past problems
- **Efficient**: SQLite backend with zero-copy queries

## Installation

```bash
cd tools/KnowledgeBaseMCP

# Install dependencies
npm install

# Build TypeScript
npm run build

# Index knowledge base
npm run index
```

## Usage

### Quick Start

```bash
# Start the MCP server
npm start

# Or development mode with auto-reload
npm run dev
```

### Available Tools

#### 1. Search Knowledge Base
```json
{
  "tool": "search_knowledge_base",
  "arguments": {
    "query": "Vue MCP integration",
    "max_results": 5,
    "category": "tools"
  }
}
```

Returns: Top matching articles with relevance scores

#### 2. Get Article
```json
{
  "tool": "get_article",
  "arguments": {
    "docid": "KB-053",
    "section": "Available Tools"
  }
}
```

Returns: Full article content or specific section

#### 3. List by Category
```json
{
  "tool": "list_by_category",
  "arguments": {
    "category": "tools"
  }
}
```

Returns: All articles in category

#### 4. Get Quick Reference
```json
{
  "tool": "get_quick_reference",
  "arguments": {
    "topic": "mcp-tools"
  }
}
```

Returns: Key points with links to full articles

#### 5. Search Lessons Learned
```json
{
  "tool": "search_lessons_learned",
  "arguments": {
    "pattern": "type error",
    "limit": 3
  }
}
```

Returns: Relevant lessons from KB

## Configuration

### VS Code Integration

Add to `.vscode/mcp.json`:

```json
{
  "mcpServers": {
    "knowledge-base-mcp": {
      "command": "npm",
      "args": ["start"],
      "cwd": "tools/KnowledgeBaseMCP",
      "disabled": false,
      "env": {
        "NODE_ENV": "production"
      }
    }
  }
}
```

## Database Schema

### documents table
- `docid` (TEXT, PRIMARY KEY) - Document identifier (KB-053, ADR-001)
- `title` (TEXT) - Document title
- `category` (TEXT) - Category: tools, patterns, architecture, best-practices
- `content` (TEXT) - Full markdown content
- `summary` (TEXT) - First 200 characters
- `tags` (TEXT) - JSON array of tags
- `size_kb` (REAL) - File size in KB
- `updated_at` (TIMESTAMP) - Last update
- `file_path` (TEXT) - Source file path

### kb_search table
- Full-text search index on content + title

## Performance

- **Index Build**: ~1-2 seconds for 200+ documents
- **Search Query**: <100ms for typical queries
- **Memory**: ~50 KB index + SQLite overhead
- **Token Usage**: ~90% reduction vs. attachment-based approach

## Rebuilding Index

After adding/updating KB articles:

```bash
npm run index
```

This will:
1. Scan `.ai/knowledgebase/` directory
2. Parse markdown files (extract docid, title, category)
3. Rebuild SQLite FTS index
4. Report statistics

## Fallback Behavior

If MCP server is unavailable, Copilot can:
- Fall back to direct file reads from `.ai/knowledgebase/`
- Cache results locally
- Use compressed knowledge summaries

## Development

### File Structure
```
tools/KnowledgeBaseMCP/
├── src/
│   └── server.ts          # MCP server implementation
├── scripts/
│   └── build-index.ts     # Index builder
├── package.json
├── tsconfig.json
└── README.md
```

### Building
```bash
npm run build          # Compile TypeScript
npm run index          # Rebuild KB index
npm run rebuild        # Both
npm run dev            # Development mode
npm run start          # Production mode
```

## Logging

Enable debug logging:

```bash
DEBUG=kb-mcp npm start
```

## Error Handling

- Missing articles return error with docid
- Invalid searches return empty results
- Database errors are logged to stderr
- Graceful fallback to file system if needed

## Future Enhancements

- [ ] Elasticsearch backend for scale
- [ ] Cross-reference detection
- [ ] Automatic KB maintenance tasks
- [ ] Analytics on KB usage patterns
- [ ] Change detection and automatic reindexing
- [ ] KB version control integration

## License

B2X Project

---

**Last Updated**: 7. Januar 2026  
**Status**: Phase 1 - Core Implementation
