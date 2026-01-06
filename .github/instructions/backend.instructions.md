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
**Reference**: See Wolverine MCP documentation in `tools/WolverineMCP/`

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

### Security MCP Integration (Always Enabled)

**Reference**: See [KB-055] Security MCP Best Practices

**Mandatory Pre-Commit Checks**:
```bash
# 1. SQL injection detection
security-mcp/check_sql_injection workspacePath="backend"

# 2. Input validation
security-mcp/validate_input_sanitization workspacePath="backend"

# 3. Authentication patterns
security-mcp/check_authentication workspacePath="backend"

# 4. Dependency vulnerabilities
security-mcp/scan_vulnerabilities workspacePath="."
```

**Integration with Development**:
- Run security MCP on all data access code
- Validate authentication/authorization patterns
- Check for SQL injection vulnerabilities
- Ensure parameterized queries only

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

# Security validation (always)
security-mcp/check_sql_injection workspacePath="backend/Domain/Catalog"
security-mcp/validate_input_sanitization workspacePath="backend/Domain/Catalog"
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

