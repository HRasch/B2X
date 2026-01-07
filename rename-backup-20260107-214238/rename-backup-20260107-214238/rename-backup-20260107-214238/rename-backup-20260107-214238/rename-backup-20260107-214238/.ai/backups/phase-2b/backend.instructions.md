---
applyTo: "src/api/**,src/services/**,src/models/**,src/repositories/**,**/backend/**"
---

# Backend Development Instructions

## Code Standards
- Use async/await for all asynchronous operations
- Implement proper error handling with typed exceptions
- Apply input validation on all public endpoints
- Use dependency injection for testability

## API Design
- Follow RESTful conventions
- Use proper HTTP status codes
- Return consistent error response format
- Document endpoints with OpenAPI/Swagger

## Database
- Use parameterized queries (prevent SQL injection)
- Implement proper connection pooling
- Add database migrations for schema changes
- Index frequently queried columns

## Security
- Never expose internal errors to clients
- Sanitize all user inputs
- Implement rate limiting on public endpoints
- Use environment variables for secrets

## Testing
- Write unit tests for business logic
- Write integration tests for API endpoints
- Mock external dependencies
- Aim for >80% code coverage

## Localization Support

**Reference**: See [GL-042] for token-optimized i18n patterns.

- **All backend messages must be translated** - error messages, validation messages, notifications
- Return translation keys (not hardcoded strings) for user-facing messages
- Use `IStringLocalizer<T>` for server-side localization
- Maintain localization API endpoints for frontend i18n support
- Implement caching for localization data to improve performance
- Support languages: en, de, fr, es, it, pt, nl, pl
- English (`en`) is source of truth for all translations
- Validate localization keys exist before deployment

## MCP-Enhanced Development (Optional - Enable When Needed)

### Roslyn MCP for C# Analysis

**Status**: Disabled by default (enable in `.vscode/mcp.json`)  
**Reference**: See [KB-052] Roslyn MCP Server documentation

**Use Cases**:
- Symbol search across C# codebase
- Type analysis and compile-time validation
- Find usages before refactoring
- Code structure analysis

**Enable Roslyn MCP**:
```json
// .vscode/mcp.json
{
  "mcpServers": {
    "roslyn-code-navigator": {
      "disabled": false  // Change from true
    }
  }
}
```

**Available Tools**:
```csharp
// Symbol search
roslyn-mcp/search_symbols pattern="*Handler" workspacePath="backend/Domain"

// Type analysis
roslyn-mcp/analyze_types workspacePath="backend/Domain/Catalog"

// Find usages
roslyn-mcp/find_usages symbolName="CreateProductCommand" workspacePath="backend"

// Get symbol info
roslyn-mcp/get_symbol_info symbolName="ProductRepository" workspacePath="backend"
```

---

### Wolverine MCP for CQRS Analysis

**Status**: Disabled by default (enable in `.vscode/mcp.json`)  
**Reference**: See [KB-063] Wolverine MCP Server.

**Use Cases**:
- Analyze Wolverine message handlers
- Validate CQRS patterns
- Check dependency injection setup
- Analyze PostgreSQL queries in handlers

**Enable Wolverine MCP**:
```json
// .vscode/mcp.json
{
  "mcpServers": {
    "wolverine-mcp": {
      "disabled": false  // Change from true
    }
  }
}
```

**Available Tools**:
```csharp
// Analyze handlers
wolverine-mcp/analyze_handlers workspacePath="backend/Domain"

// Validate DI
wolverine-mcp/validate_di workspacePath="backend/Domain"

// Analyze queries
wolverine-mcp/analyze_queries workspacePath="backend/Domain"
```

---

### Database MCP Integration (Always Enabled)

**Reference**: See [KB-057] Database MCP Usage Guide.

**Schema and Migration Validation**:
```csharp
// Validate database schema
database-mcp/validate_schema connectionString="Server=localhost;Database=B2X"

# Check migration scripts
database-mcp/check_migrations workspacePath="backend/Domain" migrationPath="Migrations"

// Analyze query performance
database-mcp/analyze_queries workspacePath="backend/Domain/Catalog" queryFile="ProductQueries.cs"

// Optimize database indexes
database-mcp/optimize_indexes connectionString="Server=localhost;Database=B2X"

// Validate multi-tenant setup
database-mcp/validate_multitenancy workspacePath="backend" tenantConfig="appsettings.json"
```

### API Documentation MCP Integration (Always Enabled)

**Reference**: See [KB-059] API Documentation MCP Usage Guide.

**OpenAPI and Contract Validation**:
```csharp
// Validate OpenAPI specifications
api-mcp/validate_openapi filePath="backend/Gateway/Store/openapi.yaml"

// Check API contracts
api-mcp/validate_contracts workspacePath="backend/Gateway"

// Detect breaking changes
api-mcp/check_breaking_changes oldSpec="v1.0/openapi.yaml" newSpec="v2.0/openapi.yaml"

// Validate API schemas
api-mcp/validate_schemas workspacePath="backend"

// Analyze API versioning
api-mcp/analyze_versioning workspacePath="backend/Gateway"
```

### Monitoring MCP for Backend Observability

**Reference**: See [KB-061] Monitoring MCP Usage Guide.

**Backend Performance Monitoring**:
```csharp
// Collect application metrics
monitoring-mcp/collect_application_metrics serviceName="catalog-api"

// Monitor system performance
monitoring-mcp/monitor_system_performance hostName="api-server-01"

// Track errors and exceptions
monitoring-mcp/track_errors serviceName="identity-service"

// Analyze application logs
monitoring-mcp/analyze_logs filePath="logs/backend.log"

// Validate health checks
monitoring-mcp/validate_health_checks serviceName="backend-services"
```

### Documentation MCP for Backend Documentation

**Reference**: See [KB-062] Documentation MCP Usage Guide.

**API Documentation Validation**:
```csharp
// Validate API documentation
docs-mcp/validate_documentation filePath="docs/api/catalog-api.md"

// Check documentation links
docs-mcp/check_links workspacePath="docs/backend"

// Analyze content quality
docs-mcp/analyze_content_quality filePath="README.md"

// Validate structure
docs-mcp/validate_structure workspacePath="docs"
```

### Performance MCP for Backend Optimization

**Reference**: See Performance MCP tools in MCP Operations Guide.

**Code Performance Analysis**:
```csharp
// Analyze code performance
performance-mcp/analyze_code_performance workspacePath="backend/Domain"

// Profile memory usage
performance-mcp/profile_memory_usage workspacePath="backend"

// Check bundle size (if applicable)
performance-mcp/check_bundle_size workspacePath="backend"
```

### Git MCP for Backend Development

**Reference**: See Git MCP tools in MCP Operations Guide.

**Code Quality Validation**:
```csharp
// Validate commit messages
git-mcp/validate_commit_messages workspacePath="backend" count=10

// Analyze code churn
git-mcp/analyze_code_churn workspacePath="backend" since="last-sprint"

// Check branch strategy
git-mcp/check_branch_strategy workspacePath="backend" branchName="feature/new-endpoint"
```

### Chrome DevTools MCP for Backend Testing (Optional)

**Reference**: See [KB-064] Chrome DevTools MCP Server.

**API Testing and Debugging**:
```csharp
// Launch browser for API testing
chrome-devtools-mcp/launch browser="chrome"

// Navigate to API documentation
chrome-devtools-mcp/navigate url="http://localhost:8080/swagger"

// Monitor network calls
chrome-devtools-mcp/network-monitor enable=true

// Capture API response screenshots
chrome-devtools-mcp/screenshot path="tests/api-responses/login.png"
```

---

### MCP-Powered Backend Workflow

When Roslyn and Wolverine MCPs are enabled:

```bash
# Before implementing new handler
roslyn-mcp/search_symbols pattern="*Handler" workspacePath="backend/Domain"
wolverine-mcp/analyze_handlers workspacePath="backend/Domain/Catalog"

# During development
roslyn-mcp/analyze_types workspacePath="backend/Domain/Catalog"

# Before refactoring
roslyn-mcp/find_usages symbolName="ProductService" workspacePath="backend"

# Database validation (always)
database-mcp/validate_schema connectionString="Server=localhost;Database=B2X"
database-mcp/analyze_queries workspacePath="backend/Domain/Catalog"

# API documentation (always)
api-mcp/validate_openapi filePath="backend/Gateway/Store/openapi.yaml"

# Security validation (always)
security-mcp/check_sql_injection workspacePath="backend/Domain/Catalog"
security-mcp/validate_input_sanitization workspacePath="backend/Domain/Catalog"

# Monitoring setup (always)
monitoring-mcp/validate_health_checks serviceName="catalog-service"

# Documentation validation (always)
docs-mcp/validate_documentation filePath="docs/api/catalog-api.md"

# Performance analysis (always)
performance-mcp/analyze_code_performance workspacePath="backend/Domain"

# Git validation (always)
git-mcp/validate_commit_messages workspacePath="backend" count=5
```

---

### When to Enable Optional MCPs

**Enable Roslyn MCP when**:
- Large-scale refactoring across multiple projects
- Need to analyze symbol usage patterns
- Working on complex type hierarchies
- Investigating compilation issues

**Enable Wolverine MCP when**:
- Implementing new CQRS handlers
- Validating message routing patterns
- Optimizing query performance
- Checking DI container configuration

**Keep Security MCP always enabled**:
- Continuous security validation
- Pre-commit security checks
- Dependency vulnerability monitoring

**Keep Database MCP always enabled**:
- Schema validation before deployments
- Query performance optimization
- Migration testing

**Keep API Documentation MCP always enabled**:
- OpenAPI specification validation
- Breaking change detection
- API contract verification

**Keep Monitoring MCP always enabled**:
- Health check validation
- Performance baseline establishment
- Error tracking setup

**Keep Documentation MCP always enabled**:
- API documentation completeness
- Link validation
- Content quality assurance

**Keep Performance MCP always enabled**:
- Code performance profiling
- Memory usage analysis
- Optimization recommendations

**Keep Git MCP always enabled**:
- Commit message validation
- Code churn analysis
- Branch strategy compliance

