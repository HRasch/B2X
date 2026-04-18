# Wolverine MCP Server

A Model Context Protocol (MCP) server for .NET/Wolverine CQRS analysis in the B2X project. This server provides AI-powered analysis for Wolverine message handlers, dependency injection, and PostgreSQL queries.

## Features

### 🏗️ Handler Analysis
- Parse Wolverine message handlers
- Analyze CQRS patterns and message routing
- Validate handler structure and conventions

### 🔗 Dependency Injection Validation
- Check DI container setup in handlers
- Validate service registrations
- Analyze injection patterns

### 🗄️ Query Analysis
- Analyze PostgreSQL queries in handlers
- Check for SQL injection vulnerabilities
- Validate query performance patterns

## Installation

```bash
cd tools/WolverineMCP
npm install
npm run build
```

## Usage

### Development
```bash
npm run dev
```

### Production
```bash
npm run build
npm start
```

## MCP Tools

### `analyze_handlers`
Analyzes Wolverine message handlers for CQRS patterns.

**Parameters:**
- `workspacePath`: Workspace root directory

### `validate_di`
Validates dependency injection setup in Wolverine handlers.

**Parameters:**
- `workspacePath`: Workspace root directory

### `analyze_queries`
Analyzes PostgreSQL queries in Wolverine handlers.

**Parameters:**
- `workspacePath`: Workspace root directory

## Integration with B2X

This MCP server is designed to work with the B2X backend architecture:

- **.NET 8+** with C# 12
- **Wolverine** CQRS framework
- **PostgreSQL** database
- **Onion Architecture** ([ADR-002])

## Development Status

This is a **basic skeleton implementation**. Full features need to be implemented in future iterations.

## Next Steps

1. **Handler Parsing**: Implement C# AST analysis for Wolverine handlers
2. **CQRS Validation**: Deep validation of command/query separation
3. **Query Optimization**: Performance analysis and suggestions
4. **Architecture Testing**: Integration with [ADR-021] ArchUnitNET
5. **Security Checks**: SQL injection and authorization validation

## Contributing

Follow B2X development guidelines and coordinate with @Backend agent for feature additions.