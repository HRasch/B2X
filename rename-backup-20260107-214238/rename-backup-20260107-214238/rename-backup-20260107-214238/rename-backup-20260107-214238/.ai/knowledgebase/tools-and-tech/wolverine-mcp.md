---
docid: KB-063
title: Wolverine MCP Server for CQRS Analysis
owner: GitHub Copilot
status: Active
---

# Wolverine MCP Server for CQRS Analysis

**DocID**: `KB-063`
**Last Updated**: 6. Januar 2026
**Owner**: GitHub Copilot

## Overview

The Wolverine MCP Server provides specialized analysis tools for Wolverine CQRS patterns, message handlers, dependency injection validation, and PostgreSQL query optimization within the B2X backend architecture.

## Status & Configuration

**Status**: Optional - Disabled by default
**Enable in**: `.vscode/mcp.json`

```json
{
  "mcpServers": {
    "wolverine-mcp": {
      "disabled": false  // Change from true
    }
  }
}
```

## Available Tools

### Handler Analysis (`wolverine-mcp/analyze_handlers`)

Analyzes Wolverine message handlers for CQRS pattern compliance and optimization opportunities.

**Parameters**:
- `workspacePath`: Path to analyze (e.g., `"backend/Domain"`)

**Example**:
```bash
wolverine-mcp/analyze_handlers workspacePath="backend/Domain/Catalog"
```

**Output**:
- Handler structure validation
- Message routing analysis
- CQRS pattern compliance
- Performance optimization suggestions

### Dependency Injection Validation (`wolverine-mcp/validate_di`)

Validates dependency injection setup for Wolverine handlers and services.

**Parameters**:
- `workspacePath`: Path to analyze

**Example**:
```bash
wolverine-mcp/validate_di workspacePath="backend/Domain"
```

**Output**:
- DI container configuration validation
- Service registration checks
- Handler dependency analysis
- Circular dependency detection

### Query Analysis (`wolverine-mcp/analyze_queries`)

Analyzes PostgreSQL queries within Wolverine handlers for performance and security.

**Parameters**:
- `workspacePath`: Path to analyze

**Example**:
```bash
wolverine-mcp/analyze_queries workspacePath="backend/Domain/Catalog"
```

**Output**:
- Query performance analysis
- SQL injection vulnerability checks
- Index usage recommendations
- Query optimization suggestions

## Integration with Development Workflow

### Pre-Implementation Analysis

Before implementing new Wolverine handlers:

```bash
# Analyze existing handler patterns
wolverine-mcp/analyze_handlers workspacePath="backend/Domain/Catalog"

# Validate DI setup
wolverine-mcp/validate_di workspacePath="backend/Domain"
```

### During Development

```bash
# Analyze new handler implementation
wolverine-mcp/analyze_handlers workspacePath="backend/Domain/Catalog/Handlers"

# Check query performance
wolverine-mcp/analyze_queries workspacePath="backend/Domain/Catalog/Handlers"
```

### Before PR Submission

```bash
# Full CQRS validation
wolverine-mcp/analyze_handlers workspacePath="backend"
wolverine-mcp/validate_di workspacePath="backend"
wolverine-mcp/analyze_queries workspacePath="backend"
```

## CQRS Pattern Validation

### Handler Structure Analysis

The MCP validates handlers against Wolverine best practices:

- **Message Handler Interface**: Ensures proper `IHandler<T>` implementation
- **Async/Await Patterns**: Validates asynchronous handler implementations
- **Error Handling**: Checks for proper exception handling in handlers
- **Transaction Management**: Validates transaction scopes and isolation levels

### Message Routing Validation

- **Route Configuration**: Validates message routing setup
- **Handler Discovery**: Ensures handlers are properly registered
- **Message Types**: Validates message contracts and serialization

### Dependency Injection Checks

- **Service Lifetime**: Validates scoped/singleton/transient service usage
- **Handler Dependencies**: Checks for proper dependency injection in handlers
- **Circular Dependencies**: Detects and reports circular dependency issues

## PostgreSQL Query Optimization

### Query Performance Analysis

- **Execution Plan Review**: Analyzes query execution plans
- **Index Usage**: Identifies missing or unused indexes
- **Query Structure**: Validates query syntax and structure
- **Parameter Usage**: Ensures parameterized queries for security

### Security Validation

- **SQL Injection Prevention**: Validates parameterized query usage
- **Input Sanitization**: Checks for proper input validation
- **Access Control**: Validates query authorization patterns

## Integration Examples

### Complete Handler Development Workflow

```bash
#!/bin/bash
echo "🔍 Wolverine MCP Handler Analysis..."

# 1. Analyze existing patterns before implementation
wolverine-mcp/analyze_handlers workspacePath="backend/Domain/Catalog"
if [ $? -ne 0 ]; then echo "❌ Handler pattern issues found"; exit 1; fi

# 2. Validate DI setup
wolverine-mcp/validate_di workspacePath="backend/Domain/Catalog"
if [ $? -ne 0 ]; then echo "❌ DI configuration issues"; exit 1; fi

# 3. Check query implementations
wolverine-mcp/analyze_queries workspacePath="backend/Domain/Catalog"
if [ $? -ne 0 ]; then echo "❌ Query optimization issues"; exit 1; fi

echo "✅ Wolverine handler validation passed"
```

### CQRS Architecture Review

```bash
#!/bin/bash
echo "🏗️ CQRS Architecture Validation..."

# Analyze all domain handlers
wolverine-mcp/analyze_handlers workspacePath="backend/Domain"

# Validate cross-domain DI
wolverine-mcp/validate_di workspacePath="backend"

# Performance audit
wolverine-mcp/analyze_queries workspacePath="backend"

echo "✅ CQRS architecture validated"
```

## Troubleshooting

### Common Issues

**Handler Not Discovered**:
- Check namespace and assembly scanning configuration
- Validate handler implements correct interface
- Ensure proper message routing setup

**DI Resolution Failures**:
- Verify service registrations in DI container
- Check service lifetime configurations
- Validate constructor parameter types

**Query Performance Issues**:
- Review execution plans for missing indexes
- Check query parameter usage
- Validate connection pooling configuration

### Performance Considerations

- **Resource Usage**: Wolverine MCP analysis can be resource-intensive on large codebases
- **Caching**: Results are cached per session to improve performance
- **Targeted Analysis**: Use specific workspace paths instead of full backend analysis when possible

## Related Documentation

- [KB-006] Wolverine Patterns - Core Wolverine implementation patterns
- [ADR-001] Wolverine over MediatR - Architecture decision rationale
- [INS-001] Backend Instructions - Backend development guidelines
- [KB-055] Security MCP Best Practices - Security validation integration

---

**Maintained by**: GitHub Copilot
**Last Review**: 6. Januar 2026
**Next Review**: 6. Februar 2026