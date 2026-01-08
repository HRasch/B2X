---
docid: KB-165
title: Database Mcp Usage
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

﻿# Database MCP Usage Guide

**DocID**: `KB-057`
**Title**: Database MCP Usage Guide
**Owner**: @Backend
**Status**: Active
**Last Updated**: 6. Januar 2026

---

## Overview

The Database MCP provides comprehensive analysis and validation tools for PostgreSQL and Elasticsearch databases in the B2X project. It ensures data integrity, query performance, and multi-tenant configuration compliance.

---

## Supported Databases

- **PostgreSQL**: Primary relational database
- **Elasticsearch**: Search and analytics engine
- **Entity Framework**: ORM layer validation

---

## Core Commands

### Schema Validation

```bash
# Validate database schema consistency
database-mcp/validate_schema workspacePath="backend"

# Check specific domain schemas
database-mcp/validate_schema workspacePath="backend/Domain/Catalog"
```

**Validates**:
- Table structure consistency
- Foreign key relationships
- Index definitions
- Constraint definitions
- Multi-tenant isolation

### Query Analysis

```bash
# Analyze query performance
database-mcp/analyze_queries workspacePath="backend/Domain"

# Check specific query patterns
database-mcp/analyze_queries filePath="backend/Domain/Catalog/Queries/GetProductsQuery.cs"
```

**Analyzes**:
- Npgsql query patterns
- Parameter usage (prevents SQL injection)
- Query execution plans
- Performance bottlenecks
- Connection pooling efficiency

### Migration Validation

```bash
# Validate EF Core migrations
database-mcp/check_migrations workspacePath="backend"

# Check migration scripts
database-mcp/check_migrations filePath="backend/Infrastructure/Migrations/"
```

**Validates**:
- Migration script syntax
- Data transformation logic
- Rollback procedures
- Multi-tenant data handling

### Connection Monitoring

```bash
# Monitor database connections
database-mcp/monitor_connections workspacePath="backend"

# Check connection pool health
database-mcp/monitor_connections environment="development"
```

**Monitors**:
- Connection pool utilization
- Connection leaks
- Timeout configurations
- Load balancing

### Index Optimization

```bash
# Analyze and optimize indexes
database-mcp/optimize_indexes workspacePath="backend"

# Check specific table indexes
database-mcp/optimize_indexes tableName="products"
```

**Optimizes**:
- Index usage statistics
- Unused index detection
- Index fragmentation
- Query performance impact

### Multi-Tenant Validation

```bash
# Validate multi-tenant configuration
database-mcp/validate_multitenancy workspacePath="backend"

# Check tenant isolation
database-mcp/validate_multitenancy tenantId="tenant-123"
```

**Validates**:
- Tenant data isolation
- Shared schema integrity
- Row-level security policies
- Cross-tenant data access

### Elasticsearch Mapping Validation

```bash
# Validate Elasticsearch mappings
database-mcp/validate_elasticsearch_mappings workspacePath="backend/Domain/Search"

# Check index mappings
database-mcp/validate_elasticsearch_mappings indexName="products"
```

**Validates**:
- Index mapping definitions
- Field type consistency
- Analyzer configurations
- Search query compatibility

---

## Integration with Development Workflow

### Pre-Commit Checks

```bash
# Run database validation on changed files
git diff --name-only | while read file; do
  case "$file" in
    *.sql) database-mcp/validate_schema workspacePath="backend" ;;
    *Entity*.cs) database-mcp/validate_multitenancy workspacePath="backend" ;;
    *Migration*.cs) database-mcp/check_migrations workspacePath="backend" ;;
  esac
done
```

### CI/CD Integration

```yaml
# .github/workflows/database-validation.yml
name: Database Validation
on: [push, pull_request]

jobs:
  validate:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Run Database MCP
        run: |
          database-mcp/validate_schema workspacePath="backend"
          database-mcp/analyze_queries workspacePath="backend"
          database-mcp/check_migrations workspacePath="backend"
```

### Quality Gates

**Mandatory Checks**:
- Schema validation (blocks deployment on failure)
- Query analysis (warnings for performance issues)
- Migration validation (blocks deployment on failure)
- Multi-tenant validation (blocks deployment on failure)

---

## Configuration

### MCP Server Configuration

```json
// .vscode/mcp.json
{
  "mcpServers": {
    "database-mcp": {
      "disabled": false,
      "config": {
        "postgresql": {
          "connectionString": "${DATABASE_CONNECTION}",
          "validateMigrations": true,
          "checkIndexes": true
        },
        "elasticsearch": {
          "hosts": ["localhost:9200"],
          "validateMappings": true,
          "checkQueries": true
        }
      }
    }
  }
}
```

### Environment Variables

```bash
# Database connection
DATABASE_CONNECTION="Host=localhost;Database=B2X;Username=user;Password=password"

# Elasticsearch configuration
ELASTICSEARCH_HOSTS="http://localhost:9200"

# Multi-tenant settings
TENANT_SCHEMA_PREFIX="tenant_"
DEFAULT_TENANT="public"
```

---

## Best Practices

### Schema Design

1. **Use descriptive names**: Table and column names should be self-documenting
2. **Consistent naming**: Follow project's naming conventions
3. **Proper constraints**: Use appropriate primary keys, foreign keys, and check constraints
4. **Index strategy**: Index foreign keys and frequently queried columns

### Query Optimization

1. **Parameterized queries**: Always use parameters to prevent SQL injection
2. **Efficient joins**: Use appropriate join types and indexes
3. **Pagination**: Implement proper pagination for large result sets
4. **Connection pooling**: Reuse connections efficiently

### Multi-Tenant Architecture

1. **Data isolation**: Ensure tenant data is properly isolated
2. **Shared resources**: Handle shared tables and resources correctly
3. **Performance**: Optimize queries for multi-tenant access patterns
4. **Security**: Implement row-level security policies

### Elasticsearch Integration

1. **Mapping consistency**: Keep mappings synchronized with code models
2. **Index strategy**: Use appropriate index patterns for search requirements
3. **Query optimization**: Use efficient query structures
4. **Monitoring**: Monitor index performance and usage

---

## Troubleshooting

### Common Issues

**Schema Validation Failures**:
```bash
# Check specific schema issues
database-mcp/validate_schema workspacePath="backend" --verbose

# Fix common issues:
# - Add missing foreign keys
# - Remove circular dependencies
# - Fix naming inconsistencies
```

**Query Performance Issues**:
```bash
# Analyze slow queries
database-mcp/analyze_queries workspacePath="backend" --performance

# Solutions:
# - Add missing indexes
# - Rewrite inefficient queries
# - Use query hints appropriately
```

**Migration Errors**:
```bash
# Validate migration scripts
database-mcp/check_migrations workspacePath="backend" --dry-run

# Common fixes:
# - Add transaction boundaries
# - Handle data transformations
# - Create rollback scripts
```

**Multi-Tenant Configuration**:
```bash
# Check tenant isolation
database-mcp/validate_multitenancy workspacePath="backend" --isolation

# Verify:
# - RLS policies are active
# - Tenant context is set
# - Cross-tenant access is blocked
```

---

## Performance Monitoring

### Key Metrics

- **Query Execution Time**: Target <100ms for simple queries
- **Connection Pool Utilization**: Keep <80% utilization
- **Index Hit Ratio**: Target >95% for critical indexes
- **Migration Duration**: Target <30 seconds for schema changes

### Monitoring Commands

```bash
# Real-time monitoring
database-mcp/monitor_connections workspacePath="backend" --watch

# Performance analysis
database-mcp/analyze_queries workspacePath="backend" --metrics

# Index optimization
database-mcp/optimize_indexes workspacePath="backend" --recommend
```

---

## Security Considerations

### Data Protection

1. **Encryption**: Use encrypted connections in production
2. **Access Control**: Implement proper database roles and permissions
3. **Audit Logging**: Enable database audit logging
4. **Backup Security**: Secure database backups

### Query Security

1. **Parameterization**: Always use parameterized queries
2. **Input Validation**: Validate all user inputs before database operations
3. **SQL Injection Prevention**: Use ORM features or prepared statements
4. **Least Privilege**: Grant minimal required permissions

---

## Integration Examples

### Backend Domain Integration

```csharp
// Entity Framework context validation
public class B2XDbContext : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Database MCP will validate this configuration
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(B2XDbContext).Assembly);
    }
}
```

### Repository Pattern

```csharp
public class ProductRepository : IProductRepository
{
    public async Task<IEnumerable<Product>> GetProductsAsync(ProductQuery query)
    {
        // Database MCP analyzes this query for performance
        return await _context.Products
            .Where(p => p.TenantId == _tenantContext.TenantId)
            .Where(p => p.Category == query.Category)
            .ToListAsync();
    }
}
```

### Elasticsearch Service

```csharp
public class ProductSearchService
{
    public async Task<SearchResponse<Product>> SearchAsync(ProductSearchRequest request)
    {
        // Database MCP validates Elasticsearch mappings and queries
        var searchResponse = await _elasticClient.SearchAsync<Product>(s => s
            .Index("products")
            .Query(q => q
                .MultiMatch(m => m
                    .Fields(f => f.Field(p => p.Name).Field(p => p.Description))
                    .Query(request.Query)
                )
            )
        );

        return searchResponse;
    }
}
```

---

## Related Documentation

- [ADR-004] PostgreSQL Multitenancy
- [KB-015] Search/Elasticsearch
- [INS-001] Backend Instructions
- [KB-055] Security MCP Best Practices

---

**Maintained by**: @Backend  
**Last Review**: 6. Januar 2026  
**Next Review**: 6. Februar 2026