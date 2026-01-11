# Database MCP Server

A Model Context Protocol (MCP) server for database analysis, query optimization, and schema validation. This server provides AI-assisted tools for working with PostgreSQL, MySQL, and Elasticsearch databases.

## Features

### SQL Query Analysis
- **Performance Analysis**: Identifies slow queries, missing indexes, and optimization opportunities
- **Security Scanning**: Detects SQL injection vulnerabilities and unsafe patterns
- **Best Practices**: Enforces SQL coding standards and naming conventions

### Database Schema Validation
- **Consistency Checks**: Validates table structures, relationships, and constraints
- **Naming Conventions**: Ensures consistent naming across tables and columns
- **Migration Safety**: Analyzes schema changes for potential issues

### Index Analysis
- **Index Optimization**: Reviews index definitions and usage patterns
- **Performance Recommendations**: Suggests indexes for frequently queried columns
- **Maintenance Checks**: Identifies unused or redundant indexes

### Elasticsearch Mapping Validation
- **Mapping Structure**: Validates index mappings and field definitions
- **Analyzer Configuration**: Checks text analysis settings
- **Best Practices**: Ensures optimal Elasticsearch configurations

### Migration Analysis
- **Safety Checks**: Identifies potentially destructive operations
- **Rollback Capability**: Verifies migration rollback plans
- **Performance Impact**: Analyzes migration performance implications

## Installation

```bash
cd tools/DatabaseMCP
npm install
npm run build
```

## Configuration

Add to your `.vscode/mcp.json`:

```json
{
  "mcpServers": {
    "database-mcp": {
      "command": "node",
      "args": [
        "tools/DatabaseMCP/dist/index.js"
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

### SQL Query Analysis

```typescript
// Analyze SQL queries in your workspace
database-mcp/analyze_sql_queries workspacePath="backend" databaseType="postgresql"

// Analyze specific queries
database-mcp/analyze_sql_queries workspacePath="backend" sqlQueries="SELECT * FROM users WHERE id = $1"
```

### Schema Validation

```typescript
// Validate database schemas
database-mcp/validate_database_schema workspacePath="backend" databaseType="postgresql"

// Validate specific schema file
database-mcp/validate_database_schema workspacePath="backend" schemaFile="migrations/001_initial_schema.sql"
```

### Index Analysis

```typescript
// Analyze index definitions
database-mcp/analyze_indexes workspacePath="backend" databaseType="postgresql"
```

### Elasticsearch Validation

```typescript
// Validate Elasticsearch mappings
database-mcp/validate_elasticsearch_mapping workspacePath="backend" indexName="products"
```

### Migration Analysis

```typescript
// Analyze database migrations
database-mcp/analyze_database_migrations workspacePath="backend" databaseType="postgresql"
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

## Supported Databases

- **PostgreSQL**: Full support with PostgreSQL-specific optimizations
- **MySQL**: Standard SQL analysis and validation
- **SQL Server**: T-SQL analysis and validation
- **Elasticsearch**: Index mapping validation and search optimization

## Integration with B2X

This MCP server integrates with the B2X development workflow to provide:

- **Pre-commit validation**: Automatic SQL analysis before commits
- **Migration reviews**: Safety checks for database schema changes
- **Performance monitoring**: Query optimization recommendations
- **Security scanning**: SQL injection prevention

## Contributing

1. Follow the existing code patterns in other MCP servers
2. Add comprehensive tests for new features
3. Update this README with new capabilities
4. Ensure TypeScript strict mode compliance

## License

MIT