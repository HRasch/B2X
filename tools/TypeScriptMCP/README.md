# TypeScript MCP Server for B2Connect

A Model Context Protocol (MCP) server providing TypeScript code analysis tools for AI-assisted development, complementing the existing RoslynMCP for C#.

## Status: ✅ Fully Functional

All MCP tools are implemented and tested:
- ✅ **search_symbols** - Find TypeScript symbols using patterns
- ✅ **get_symbol_info** - Get detailed symbol information
- ✅ **find_usages** - Find all symbol references
- ✅ **analyze_types** - Type checking and error analysis

## Features

### Symbol Analysis Tools
- **search_symbols** - Find TypeScript symbols (classes, interfaces, functions) using wildcard patterns
- **get_symbol_info** - Get detailed information about a specific symbol including type, location, and documentation
- **find_usages** - Find all usages/references of a symbol across the codebase
- **analyze_types** - Perform type checking and report errors in TypeScript files

## Setup

1. Install dependencies:
```bash
cd tools/TypeScriptMCP
npm install
```

2. Build the project:
```bash
npm run build
```

3. Start the server:
```bash
npm start
```

## Configuration

Add to `.vscode/mcp.json`:

```json
{
  "mcpServers": {
    "typescript-mcp": {
      "command": "node",
      "args": ["/path/to/tools/TypeScriptMCP/dist/index.js"]
    }
  }
}
```

## Usage

The MCP server integrates with GitHub Copilot in VS Code, providing TypeScript-specific tools for AI assistance.

### Example Queries

```
Search for all interfaces ending with 'Props' in frontend/
```

```
Find all usages of useAuth hook
```

```
Analyze types in src/components/UserProfile.vue
```

```
Get symbol info for LoginForm component
```

## Testing

Run the test scripts to verify functionality:

```bash
# Test symbol search
node test-search.js

# Test symbol info retrieval
node test-symbol-info.js

# Test finding usages
node test-find-usages.js

# Test type analysis
node test-analyze-types.js
```

## Implementation Details

### Language Service Integration
- Uses TypeScript's official language services API
- Automatically discovers `tsconfig.json` for project configuration
- Supports incremental analysis and caching
- Handles TypeScript's AST for symbol resolution

### Tool Capabilities

#### search_symbols
- Pattern matching with wildcards (*, ?)
- Searches across classes, interfaces, functions, methods, properties, variables
- Supports file-specific or project-wide searches

#### get_symbol_info
- Provides quick info (type, documentation, modifiers)
- Supports position-based or name-based lookup
- Returns location information

#### find_usages
- Finds all references to a symbol across the codebase
- Uses TypeScript's reference resolution
- Returns file locations with line/character positions

#### analyze_types
- Reports syntactic and semantic diagnostics
- Includes type errors, unused variables, etc.
- Provides detailed error messages with locations

## Development

- **Source**: `src/index.ts`
- **Build output**: `dist/`
- **Language Service**: Automatically initialized per project
- **Error Handling**: Graceful degradation with informative error messages

## Architecture

Built using:
- **@modelcontextprotocol/sdk**: MCP protocol implementation
- **typescript**: Compiler and language services
- **Node.js**: Runtime with ES modules

Follows the same architectural pattern as RoslynMCP but adapted for TypeScript's language services API.