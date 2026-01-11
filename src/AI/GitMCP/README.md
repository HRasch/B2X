# Git MCP Server

A Model Context Protocol (MCP) server that provides AI-assisted analysis of Git repositories, focusing on version control best practices, commit quality, and development workflow optimization.

## Features

### 🔍 Commit History Analysis
- Analyze commit frequency and patterns
- Evaluate team collaboration metrics
- Identify commit quality issues
- Provide recommendations for improvement

### 🌿 Branch Strategy Validation
- Validate branch naming conventions
- Check compliance with branching strategies (GitHub Flow, Git Flow, Trunk-based)
- Identify stale branches
- Recommend branch management practices

### 📝 Commit Message Validation
- Check conventional commit format compliance
- Validate message length and clarity
- Identify poorly formatted commits
- Provide formatting guidelines

### 📊 Code Churn Analysis
- Analyze code change patterns over time
- Identify high-churn files that may indicate problems
- Calculate churn rates and trends
- Recommend focused development practices

### ⚡ Merge Conflict Detection
- Identify files prone to merge conflicts
- Analyze contributor patterns
- Recommend conflict prevention strategies
- Suggest architectural improvements

## Installation

```bash
cd tools/GitMCP
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

### Testing
```bash
npm test
```

## MCP Configuration

Add to your `.vscode/mcp.json`:

```json
{
  "mcpServers": {
    "git-mcp": {
      "command": "node",
      "args": ["tools/GitMCP/dist/index.js"],
      "env": {
        "NODE_ENV": "production"
      },
      "disabled": false
    }
  }
}
```

## Available Tools

### `analyze_commit_history`
Analyzes commit patterns and quality metrics.

**Parameters:**
- `workspacePath` (string, required): Path to the git repository
- `days` (number, optional): Number of days to analyze (default: 30)

### `check_branch_strategy`
Validates branch naming and workflow compliance.

**Parameters:**
- `workspacePath` (string, required): Path to the git repository
- `strategy` (string, optional): Branching strategy ('github-flow', 'git-flow', 'trunk-based')

### `validate_commit_messages`
Checks commit message quality and conventional commit compliance.

**Parameters:**
- `workspacePath` (string, required): Path to the git repository
- `count` (number, optional): Number of commits to validate (default: 50)

### `analyze_code_churn`
Analyzes code change patterns and identifies high-churn areas.

**Parameters:**
- `workspacePath` (string, required): Path to the git repository
- `days` (number, optional): Number of days to analyze (default: 30)

### `detect_merge_conflicts`
Identifies files prone to merge conflicts based on contributor patterns.

**Parameters:**
- `workspacePath` (string, required): Path to the git repository
- `threshold` (number, optional): Minimum contributors to flag (default: 3)

## Example Usage

```bash
# Analyze recent commit history
git-mcp/analyze_commit_history workspacePath="/path/to/repo" days=30

# Validate branch strategy
git-mcp/check_branch_strategy workspacePath="/path/to/repo" strategy="github-flow"

# Check commit message quality
git-mcp/validate_commit_messages workspacePath="/path/to/repo" count=100

# Analyze code churn patterns
git-mcp/analyze_code_churn workspacePath="/path/to/repo" days=14

# Detect merge conflict hotspots
git-mcp/detect_merge_conflicts workspacePath="/path/to/repo" threshold=5
```

## Dependencies

- `@modelcontextprotocol/sdk`: MCP protocol implementation
- `simple-git`: Git operations wrapper
- `fs-extra`: Enhanced file system operations
- `glob`: File pattern matching

## Development

### Project Structure
```
tools/GitMCP/
├── src/
│   ├── index.ts          # Main server implementation
│   └── index.test.ts     # Test suite
├── package.json          # Dependencies and scripts
├── tsconfig.json         # TypeScript configuration
├── jest.config.js        # Test configuration
└── README.md            # This file
```

### Adding New Tools

1. Define the tool in the `ListToolsRequestSchema` handler
2. Implement the tool logic in a private method
3. Add the tool case to the `CallToolRequestSchema` handler
4. Add tests for the new functionality
5. Update this README

## Contributing

1. Follow the existing code style and patterns
2. Add comprehensive tests for new features
3. Update documentation for any new tools
4. Ensure all tests pass before submitting

## License

MIT