---
docid: KB-053
title: TypeScript MCP Integration Guide
owner: @CopilotExpert
status: Active
---

# TypeScript MCP Integration Guide

**DocID**: `KB-053`
**Last Updated**: 6. Januar 2026
**Owner**: @CopilotExpert

## Overview

The TypeScript MCP server provides AI-assisted code analysis tools for TypeScript/Vue.js development, complementing the existing RoslynMCP for C#.

## MCP Server Configuration

**Location**: `tools/TypeScriptMCP/`
**MCP Config**: `.vscode/mcp.json` (already configured)

```json
{
  "mcpServers": {
    "typescript-mcp": {
      "command": "node",
      "args": ["tools/TypeScriptMCP/dist/index.js"],
      "env": { "NODE_ENV": "production" },
      "disabled": false
    }
  }
}
```

## Available Tools

### 1. search_symbols
**Purpose**: Find TypeScript symbols using patterns
**Use Cases**:
- Component discovery: `pattern="*Component"`
- Interface finding: `pattern="*Interface"`
- Service location: `pattern="*Service"`

**Example**:
```typescript
// Find all Vue components
pattern: "*Component"
workspacePath: "frontend/Store"

// Find interfaces
pattern: "*VM"
workspacePath: "frontend/Store/src"
```

### 2. get_symbol_info
**Purpose**: Detailed symbol analysis
**Use Cases**:
- Type definition inspection
- Property/method analysis
- Documentation extraction

**Example**:
```typescript
symbolName: "UserProfile"
workspacePath: "frontend/Store"
filePath: "src/types/user.ts"
```

### 3. find_usages
**Purpose**: Cross-file usage tracking
**Use Cases**:
- Refactoring impact assessment
- Dependency analysis
- Dead code detection

**Example**:
```typescript
symbolName: "ProductCard"
workspacePath: "frontend/Store"
filePath: "src/components/ProductCard.vue"
```

### 4. analyze_types
**Purpose**: TypeScript compilation and error analysis
**Use Cases**:
- Pre-commit validation
- Type safety verification
- Error detection

**Example**:
```typescript
workspacePath: "frontend/Store"
filePath: "src/components/NewComponent.vue"
```

## Agent Integration

### @Frontend Agent
**Tools**: `typescript-mcp/*`
**Usage Patterns**:
- Symbol search during development
- Type analysis in code reviews
- Usage tracking for refactoring

### @TechLead Agent
**Enhanced Code Review**:
- Automated type checking
- Symbol usage validation
- Import analysis

## Workflow Integration

### Development Workflow
1. **Symbol Discovery**: Use `search_symbols` to find existing components
2. **Type Analysis**: Run `analyze_types` on new/modified files
3. **Usage Tracking**: Check `find_usages` before refactoring
4. **Code Review**: Automated analysis with `get_symbol_info`

### Code Review Process
```bash
# TypeScript review command
@TechLead: /typescript-review
Component: frontend
Scope: src/components/UserProfile.vue
Focus: types
```

## Best Practices

### Performance Optimization
- **Selective Analysis**: Target specific files rather than entire workspace
- **Cached Results**: Leverage MCP server caching for repeated queries
- **Incremental Checks**: Use file-specific analysis for faster feedback

### Error Handling
- **Path Validation**: Ensure workspace paths are correct
- **Symbol Existence**: Verify symbols exist before usage analysis
- **Type Errors**: Address compilation errors before advanced analysis

### Integration Patterns
- **Pre-commit Hooks**: Run type analysis before commits
- **CI/CD Integration**: Include MCP checks in build pipeline
- **IDE Integration**: Leverage VS Code MCP extension capabilities

## Troubleshooting

### Common Issues

**MCP Server Not Responding**
```bash
# Check server status
cd tools/TypeScriptMCP
npm run build
npm start
```

**Invalid Workspace Path**
- Ensure paths are relative to project root
- Use forward slashes (/) consistently
- Avoid `..` in paths

**Symbol Not Found**
- Check spelling and case sensitivity
- Verify file is included in tsconfig.json
- Ensure symbol is exported

### Debug Commands
```bash
# Test MCP connection
node tools/TypeScriptMCP/dist/index.js

# Validate TypeScript config
npx tsc --noEmit --project frontend/Store/tsconfig.json
```

## Success Metrics

- **Adoption Rate**: >50% of frontend tasks use MCP tools
- **Error Reduction**: 30% fewer type-related bugs
- **Review Time**: 25% faster code reviews
- **Developer Satisfaction**: Positive feedback on tool usefulness

## Related Documentation

- [PRM-016] TypeScript Review Prompt
- [INS-002] Frontend Instructions
- [GL-012] Frontend Quality Standards
- [KB-052] Roslyn MCP Integration (C# equivalent)

## Future Enhancements

- **Vue SFC Support**: Enhanced analysis for .vue files
- **Performance Metrics**: Response time optimization
- **Custom Rules**: Project-specific type checking rules
- **Integration Testing**: MCP tool validation in CI/CD

---

**Status**: ✅ Active and integrated
**Next Review**: March 2026