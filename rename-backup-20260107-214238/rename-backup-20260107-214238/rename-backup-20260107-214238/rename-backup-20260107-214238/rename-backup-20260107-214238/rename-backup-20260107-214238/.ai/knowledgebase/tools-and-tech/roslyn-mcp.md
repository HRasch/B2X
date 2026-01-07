---
docid: KB-052
title: Roslyn MCP Server for Code Analysis
owner: GitHub Copilot
status: Active
---

# Roslyn MCP Server for Code Analysis

**DocID**: `KB-052`

The Roslyn MCP Server provides Model Context Protocol (MCP) integration for Roslyn-powered C# code analysis in VS Code. It enables AI-assisted development with advanced code insights, dependency analysis, and refactoring suggestions.

## Overview

- **Technology**: .NET 10, Roslyn 4.14.0, Model Context Protocol
- **Location**: `tools/RoslynMCP/`
- **Configuration**: `.vscode/mcp.json` (server: `roslyn-code-navigator`)
- **Status**: MVP available, running in background

## Available Tools

### Symbol Search Tools
- **SearchSymbols**: Find classes, methods, properties using wildcard patterns (e.g., `*Service`, `Get*User`)
- **GetSymbolInfo**: Get detailed information about a specific symbol
- **FindUsages**: Find all usages of a type or member across the solution
- **AnalyzeComplexity**: Identify high-complexity methods (cyclomatic complexity > 15)

### Dependency Analysis Tools
- **AnalyzeDependencies**: Analyze project dependencies and find circular references
- **FindNamespaceUsages**: Find which namespaces are used across projects
- **AnalyzeLayerViolations**: Check for architectural layer violations (ADR-025 compliance)

### Integrated Analyzers
The MCP server can leverage additional Roslyn analyzers for enhanced analysis:
- **SonarAnalyzer.CSharp**: Advanced code quality and security analysis
- **Meziantou.Analyzer**: Enforces C# best practices
- **xunit.analyzers**: Testing framework analysis
- **SerilogAnalyzer**: Logging library best practices

## Usage Examples

### In VS Code Chat
```
Analyze dependencies for the B2Connect solution
Find all usages of TenantContext
Search for classes ending with 'Service'
Analyze complexity of methods in B2Connect.Catalog
```

### Integration Benefits
- **Faster Development**: Real-time code analysis reduces debugging time
- **Architecture Enforcement**: Automatic detection of layer violations
- **Quality Assurance**: Complexity analysis and dependency checks
- **Knowledge Sharing**: Export analysis results for documentation

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

## Best Practices

### Development Workflow Integration
1. **Pre-Commit Analysis**: Run dependency checks before committing changes
   - Command: "Analyze dependencies for B2Connect"
   - Ensures no circular references are introduced

2. **Code Review Enhancement**: Use during PR reviews
   - Check complexity: "Analyze complexity of methods in [ProjectName]"
   - Verify architecture: "Find namespace usages across projects"

3. **Refactoring Support**: Analyze impact before large changes
   - Find usages: "Find all usages of [ClassName]"
   - Check dependencies: "Analyze layer violations"

### Performance Optimization
4. **Targeted Analysis**: Focus on high-risk areas
   - Domain layer: Critical business logic
   - Shared libraries: Cross-cutting concerns
   - Gateway projects: External integrations

5. **Caching Strategy**: Leverage server-side caching
   - Analysis results cached for 5 minutes
   - Reduces repeated computation overhead
   - Clear cache on major structural changes

### Quality Assurance
6. **Complexity Thresholds**: Monitor method complexity
   - Flag methods with cyclomatic complexity > 15
   - Review and refactor complex methods
   - Use as indicator for potential bugs

7. **Dependency Hygiene**: Regular dependency audits
   - Weekly: "Analyze dependencies for the solution"
   - Check for unused imports and circular refs
   - Maintain clean architecture boundaries

### CI/CD Integration
8. **Automated Gates**: Include in build pipelines
   - Fail builds on circular dependencies
   - Warn on high complexity methods
   - Generate dependency reports

9. **Incremental Analysis**: Use for large solutions
   - Analyze changed projects only
   - Parallel processing for multiple projects
   - Cache results across pipeline runs

### Team Collaboration
10. **Knowledge Sharing**: Document analysis findings
    - Export results to `.ai/knowledgebase/`
    - Share patterns and anti-patterns
    - Update lessons learned

11. **Onboarding**: Use for new developer training
    - Demonstrate architecture analysis
    - Show dependency visualization
    - Explain complexity metrics

### Troubleshooting
12. **Server Health**: Monitor MCP server performance
    - Check logs for analysis errors
    - Restart server on solution changes
    - Update Roslyn packages regularly

13. **Query Optimization**: Craft efficient analysis queries
    - Use specific project names
    - Combine related analyses
    - Avoid broad searches on large codebases

## Future Enhancements

- Integrate additional analyzers (SonarAnalyzer, Meziantou, xunit, Serilog) into MCP tools
- Custom analyzers for B2Connect patterns
- Code generation tools
- Integration with GitHub Copilot for suggestions
- Performance profiling integration

---

**Last Updated**: 6. Januar 2026
**Next Review**: 15. Februar 2026