# GitHub Copilot Customization Guide

**DocID**: `KB-022`  
**Version**: 1.0  
**Last Updated**: 2. Januar 2026  
**Maintained By**: @CopilotExpert  
**Status**: ✅ Current

---

## Overview

GitHub Copilot in VS Code supports extensive customization through custom agents, instructions, prompt files, and MCP tools. This guide covers all customization options and best practices.

## Official Documentation

| Resource | URL | Purpose |
|----------|-----|---------|
| Customization Overview | https://code.visualstudio.com/docs/copilot/customization/overview | Main entry point |
| Custom Agents | https://code.visualstudio.com/docs/copilot/customization/custom-agents | Agent configuration |
| Custom Instructions | https://code.visualstudio.com/docs/copilot/customization/custom-instructions | Coding guidelines |
| Prompt Files | https://code.visualstudio.com/docs/copilot/customization/prompt-files | Reusable prompts |
| MCP Servers | https://code.visualstudio.com/docs/copilot/customization/mcp-servers | External tools |
| Language Models | https://code.visualstudio.com/docs/copilot/customization/language-models | Model selection |
| GitHub Docs | https://docs.github.com/en/copilot/customizing-copilot | GitHub-side config |

---

## 1. Custom Agents (`.agent.md`)

Custom agents create specialized AI personas for specific development tasks.

### File Location
- **Workspace**: `.github/agents/*.agent.md`
- **User Profile**: VS Code profile folder

### File Structure

```yaml
---
description: Generate an implementation plan
name: Planner
argument-hint: Describe what you want to plan
tools: ['fetch', 'githubRepo', 'search', 'usages']
model: Claude Sonnet 4
infer: true  # Allow as subagent (default: true)
target: vscode  # or github-copilot
handoffs:
  - label: Start Implementation
    agent: agent
    prompt: Implement the plan above.
    send: false
---
# Planning Instructions

You are in planning mode. Generate implementation plans without making code edits.

Use #tool:search to find relevant code.
Use #tool:githubRepo to analyze repository structure.
```

### YAML Frontmatter Fields

| Field | Description | Required |
|-------|-------------|----------|
| `description` | Shown as placeholder in chat input | No |
| `name` | Display name (default: filename) | No |
| `argument-hint` | Guidance text for users | No |
| `tools` | Available tools list | No |
| `model` | AI model to use | No |
| `infer` | Allow as subagent (default: true) | No |
| `target` | Environment: `vscode` or `github-copilot` | No |
| `mcp-servers` | MCP server configs (for github-copilot target) | No |
| `handoffs` | Workflow transitions | No |

### Handoffs Configuration

```yaml
handoffs:
  - label: "Button Text"      # Display text
    agent: "target-agent"     # Agent to switch to
    prompt: "Next step text"  # Pre-filled prompt
    send: false               # Auto-submit (default: false)
```

### Built-in Tools

| Tool | Purpose |
|------|---------|
| `search` | Search workspace files |
| `fetch` | Fetch web content |
| `githubRepo` | Analyze GitHub repositories |
| `usages` | Find code usages |
| `editFiles` | Modify files |
| `terminalLastCommand` | Get terminal output |

### Tool Reference Syntax
In the body, reference tools with: `#tool:toolName`

---

## 2. Custom Instructions (`.instructions.md`)

Instructions provide coding guidelines and project-specific rules.

### Types of Instructions

| Type | Location | Scope |
|------|----------|-------|
| Repository-wide | `.github/copilot-instructions.md` | All requests |
| Path-specific | `.github/instructions/*.instructions.md` | Matching files |
| Agent instructions | `AGENTS.md` (any directory) | Nearest file wins |

### Repository-Wide Instructions

**File**: `.github/copilot-instructions.md`

```markdown
# Project Instructions

## Code Standards
- Use async/await for all asynchronous operations
- Follow SOLID principles
- Write unit tests for business logic

## Architecture
- Backend: .NET 10 with Wolverine CQRS
- Frontend: Vue.js 3 with TypeScript
- Database: PostgreSQL with EF Core

## Security
- Never commit secrets
- Validate all user inputs
- Use parameterized queries
```

### Path-Specific Instructions

**File**: `.github/instructions/backend.instructions.md`

```yaml
---
applyTo: "src/api/**,src/services/**,**/*.cs"
excludeAgent: "code-review"  # Optional
---

# Backend Development Instructions

- Use async/await for database operations
- Implement proper error handling
- Follow repository pattern
- Add XML documentation for public APIs
```

### Glob Pattern Examples

| Pattern | Matches |
|---------|---------|
| `*` | All files in current directory |
| `**` or `**/*` | All files recursively |
| `*.py` | Python files in current directory |
| `**/*.py` | All Python files recursively |
| `src/**/*.ts` | TypeScript files under src/ |
| `**/*.test.*` | All test files |

---

## 3. Prompt Files (`.prompt.md`)

Reusable prompts for common development tasks.

### File Location
- **Workspace**: `.github/prompts/*.prompt.md`
- **User Profile**: VS Code profile folder

### File Structure

```yaml
---
description: Generate a React component with TypeScript
name: create-react-component
agent: agent
model: Claude Sonnet 4
tools: ['editFiles', 'search']
---

# Create React Component

Generate a new React functional component with TypeScript.

Component name: ${input:componentName:MyComponent}
Location: ${input:location:src/components}

## Requirements
- Use functional component with hooks
- Include TypeScript interfaces for props
- Add JSDoc documentation
- Create corresponding test file

Reference existing components: [components](../src/components/)
```

### Variables

| Variable | Description |
|----------|-------------|
| `${workspaceFolder}` | Workspace root path |
| `${workspaceFolderBasename}` | Workspace folder name |
| `${selection}` | Current editor selection |
| `${selectedText}` | Selected text content |
| `${file}` | Current file path |
| `${fileBasename}` | Current filename |
| `${fileDirname}` | Current file directory |
| `${input:name}` | User input variable |
| `${input:name:default}` | Input with default value |

### Running Prompts
1. Type `/prompt-name` in chat
2. Use Command Palette: `Chat: Run Prompt`
3. Click play button in prompt file editor

---

## 4. MCP Servers Configuration

Model Context Protocol (MCP) extends Copilot with external tools.

### Configuration File

**File**: `.vscode/mcp.json`

```json
{
  "servers": {
    "database": {
      "command": "npx",
      "args": ["-y", "@modelcontextprotocol/server-postgres"],
      "env": {
        "DATABASE_URL": "${env:DATABASE_URL}"
      }
    },
    "filesystem": {
      "command": "npx",
      "args": ["-y", "@modelcontextprotocol/server-filesystem", "/path/to/data"]
    }
  }
}
```

### Using MCP Tools in Agents

```yaml
---
tools: ['database/*', 'filesystem/*', 'search']
---
```

---

## 5. Language Models

### Available Models (as of Jan 2026)

| Model | Best For |
|-------|----------|
| Claude Sonnet 4 | Balanced performance |
| Claude Haiku 4.5 | Fast, efficient tasks |
| GPT-4o | Complex reasoning |
| GPT-4o mini | Quick suggestions |

### Model Selection in Files

```yaml
---
model: Claude Sonnet 4
---
```

### VS Code Settings

```json
{
  "github.copilot.chat.defaultModel": "claude-sonnet-4"
}
```

---

## 6. Tool Priority Order

When using prompt files with agents:

1. Tools specified in prompt file
2. Tools from referenced agent
3. Default tools for selected agent

---

## 7. Best Practices

### Instructions
- ✅ Keep under 2 pages
- ✅ Be specific and actionable
- ✅ Use examples where helpful
- ✅ Reference files via links
- ❌ Don't duplicate content
- ❌ Don't include task-specific details

### Agents
- ✅ Define clear persona and scope
- ✅ Limit tools to what's needed
- ✅ Use handoffs for workflows
- ✅ Test iteratively
- ❌ Don't give all tools to every agent
- ❌ Don't make instructions too long

### Prompts
- ✅ Use clear variable names
- ✅ Provide examples of expected output
- ✅ Reference instructions via links
- ✅ Use input variables for flexibility
- ❌ Don't hardcode values
- ❌ Don't repeat instruction content

---

## 8. B2Connect Configuration

### Current Structure

```
.github/
├── copilot-instructions.md      # Repository-wide (INS-000)
├── agents/
│   ├── sarah.agent.md           # Coordinator
│   ├── backend.agent.md         # .NET development
│   ├── frontend.agent.md        # Vue.js development
│   ├── copilot-expert.agent.md  # This specialist
│   └── ...
├── instructions/
│   ├── backend.instructions.md  # src/api/**, src/services/**
│   ├── frontend.instructions.md # src/components/**, src/pages/**
│   ├── testing.instructions.md  # **/*.test.*, **/*.spec.*
│   ├── devops.instructions.md   # .github/**, Dockerfile
│   └── security.instructions.md # **/*
└── prompts/
    ├── start-feature.prompt.md
    ├── code-review.prompt.md
    ├── run-tests.prompt.md
    └── ...
```

### Key Settings

```json
{
  "github.copilot.chat.customAgents.showOrganizationAndEnterpriseAgents": true,
  "chat.promptFilesLocations": [".github/prompts"],
  "chat.promptFilesRecommendations": true
}
```

---

## 9. Troubleshooting

### Instructions Not Applied
1. Check file is in correct location
2. Verify `applyTo` glob pattern matches
3. Check VS Code setting `github.copilot.chat.customInstructions` is enabled
4. Restart VS Code

### Agent Not Appearing
1. Verify `.agent.md` extension
2. Check file is in `.github/agents/` folder
3. Look for YAML syntax errors
4. Check VS Code version (requires 1.106+)

### Tools Not Available
1. Verify tool name spelling
2. Check MCP server is running
3. Verify server configuration in `.vscode/mcp.json`
4. Check tool is available for current agent mode

---

## 10. Related Documentation

| DocID | Title | Description |
|-------|-------|-------------|
| [INS-000] | copilot-instructions.md | Global project instructions |
| [GL-006] | Token Optimization Strategy | Cost efficiency guidelines |
| [AGT-INDEX] | Agent Team Registry | All project agents |
| [PRM-INDEX] | Prompts Index | All project prompts |

---

## Changelog

| Date | Version | Changes |
|------|---------|---------|
| 2026-01-02 | 1.0 | Initial documentation |

---

**Next Review**: 2026-04-02 (Quarterly)
