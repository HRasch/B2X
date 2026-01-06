# Roslyn MCP Server for B2Connect

A custom Model Context Protocol (MCP) server providing Roslyn-powered C# code analysis tools for AI-assisted development.

## Features

### Symbol Search Tools
- **SearchSymbols** - Find classes, methods, properties using wildcard patterns (`*Service`, `Get*User`)
- **GetSymbolInfo** - Get detailed information about a specific symbol
- **FindUsages** - Find all usages of a type or member across the solution
- **AnalyzeComplexity** - Identify high-complexity methods (cyclomatic complexity)
- **ListProjects** - List all projects in a solution

### Dependency Analysis Tools
- **AnalyzeDependencies** - Analyze project dependencies and circular references
- **FindNamespaceUsages** - Find which namespaces are used across projects
- **AnalyzeLayerViolations** - Check for architectural layer violations

## Usage

The MCP server is configured in `.vscode/mcp.json` and integrates with GitHub Copilot in VSCode.

### Example Queries

```
Search for all classes ending with 'Service' in B2Connect.slnx
```

```
Find all usages of TenantContext in the solution
```

```
Analyze dependencies for the solution
```

```
Find methods with complexity higher than 15
```

## Building

```bash
cd tools/RoslynMCP
dotnet build
```

## Configuration

The server is configured in `.vscode/mcp.json`:

```json
{
  "mcpServers": {
    "roslyn-code-navigator": {
      "command": "dotnet",
      "args": ["run", "--project", "tools/RoslynMCP/RoslynMCP.csproj"],
      "env": {
        "DOTNET_ENVIRONMENT": "Production",
        "LOG_LEVEL": "Information"
      }
    }
  }
}
```

## Architecture

- **CodeAnalysisService** - Manages Roslyn workspace and solution loading with caching
- **SymbolSearchTools** - MCP tools for symbol search and analysis
- **DependencyTools** - MCP tools for dependency and architecture analysis

## Requirements

- .NET 9.0 SDK
- MSBuild (included with .NET SDK)
