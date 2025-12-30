# Copilot Agent Definition Reference

**Purpose**: Correct YAML frontmatter format for `.agent.md` files  
**Location**: `.github/agents/{agent-name}.agent.md`  
**Reference**: [VS Code Agent Documentation](https://code.visualstudio.com/docs/copilot/customization/custom-agents)

---

## Agent Types

VS Code supports three types of agents:

| Type | Description | Runs In |
|------|-------------|---------|
| **Local Agent** | Operates in VS Code editor context | VS Code |
| **Background Agent** | Runs autonomously via CLI | Terminal (isolated) |
| **Cloud Agent** | Runs on GitHub infrastructure | GitHub |

---

## Required Format (Local Agents)

```yaml
---
description: 'Brief description of agent purpose'
tools: ['tool1', 'tool2']
model: 'gpt-4o'
infer: true
---
```

---

## Available Properties

| Property | Type | Required | Description |
|----------|------|----------|-------------|
| `description` | string | ✅ | Brief description shown in agent picker |
| `tools` | array | ✅ | List of tools the agent can use |
| `model` | string | ❌ | LLM model (default: gpt-4o) |
| `infer` | boolean | ❌ | Enable inference (default: true) |
| `background` | boolean | ❌ | Run as background task (default: false) |
| `schedule` | string | ❌ | Cron-like schedule for background agents |

---

## Available Tools

### Core Tools
| Tool | Description |
|------|-------------|
| `agent` | Invoke other agents |
| `execute` | Run shell commands |
| `vscode` | VS Code operations |
| `read` | Read files |
| `search` | Search codebase |
| `create_file` | Create new files |
| `replace_string_in_file` | Edit existing files |

### Specialized Tools
| Tool | Description |
|------|-------------|
| `gitkraken/*` | Git operations |
| `github/*` | GitHub API operations |
| `terminal` | Terminal commands |

---

## Available Models

| Model | Use Case |
|-------|----------|
| `gpt-4o` | General purpose (recommended) |
| `gpt-4o-mini` | Faster, cheaper tasks |
| `claude-opus-4.5` | Complex reasoning |
| `claude-sonnet-4` | Balanced performance |

---

## Examples

### Minimal Agent

```yaml
---
description: 'Backend developer for API endpoints'
tools: ['read', 'search', 'create_file', 'replace_string_in_file']
model: 'gpt-4o'
infer: true
---

## Mission

Build backend API endpoints following Wolverine patterns.
```

### Background Watcher Agent

```yaml
---
description: 'Monitors collaboration folders and triggers processing'
tools: ['read', 'search']
model: 'gpt-4o'
infer: true
background: true
schedule: 'daily'
---

## Mission

Scan folders and trigger @team-assistant when messages found.
```

### Full-Featured Agent

```yaml
---
description: 'Tech Lead for architecture decisions and code review'
tools: ['agent', 'execute', 'vscode', 'read', 'search', 'create_file', 'replace_string_in_file', 'gitkraken/*']
model: 'gpt-4o'
infer: true
---

## Mission

Lead technical decisions and maintain code quality.

## Responsibilities

1. Architecture review
2. Code review
3. Mentoring
```

---

## Common Mistakes

### ❌ Wrong: Missing Quotes

```yaml
---
description: Backend developer  # WRONG - needs quotes
tools: [read, search]           # WRONG - strings need quotes
---
```

### ✅ Correct: Quoted Strings

```yaml
---
description: 'Backend developer'
tools: ['read', 'search']
---
```

### ❌ Wrong: Invalid Model

```yaml
---
model: 'gpt-5'  # WRONG - doesn't exist
---
```

### ✅ Correct: Valid Model

```yaml
---
model: 'gpt-4o'
---
```

### ❌ Wrong: Tools as Object

```yaml
---
tools:
  - read
  - search
---
```

### ✅ Correct: Tools as Array

```yaml
---
tools: ['read', 'search']
---
```

---

## Agent File Structure

```markdown
---
description: '...'
tools: ['...']
model: 'gpt-4o'
infer: true
---

## Mission

One sentence describing the agent's purpose.

---

## Responsibilities

1. First responsibility
2. Second responsibility
3. Third responsibility

---

## What You Do

✅ Task 1
✅ Task 2

## What You Do NOT Do

❌ Anti-task 1
❌ Anti-task 2

---

## Execution

[Specific instructions for the agent]

---

**Last Updated**: YYYY-MM-DD
**Status**: ✅ Active
```

---

## Validation Checklist

Before committing an agent file:

- [ ] YAML frontmatter has `---` delimiters
- [ ] `description` is quoted string
- [ ] `tools` is array with quoted strings
- [ ] `model` is valid model name
- [ ] File named `{agent-name}.agent.md`
- [ ] File in `.github/agents/` folder
- [ ] Mission section present
- [ ] No duplicate agents with same purpose

---

## Background Agents

> **Source**: [VS Code Background Agents Documentation](https://code.visualstudio.com/docs/copilot/agents/background-agents)

### What Are Background Agents?

Background agents are **CLI-based agents** that run independently on your local machine. Unlike local agents that operate in VS Code's editor context, background agents run autonomously while you continue other work.

**Key Characteristics**:
- Run via command-line interfaces (CLIs)
- Operate autonomously without user interaction
- Can use Git worktrees for isolation
- Cannot access VS Code built-in tools or MCP servers
- Limited to CLI-available models

### When to Use Background Agents

✅ **Well-suited for**:
- Tasks with well-defined scope
- Implementing features from a plan
- Creating multiple variants of a proof of concept
- Implementing clearly defined fixes or features
- Long-running tasks that don't need editor context

❌ **Not suited for**:
- Tasks requiring VS Code runtime context
- Tasks needing MCP servers or extension tools
- Interactive debugging
- Tasks requiring user approvals frequently

### Background Agent Types in VS Code

| Agent | Description | Setup |
|-------|-------------|-------|
| **Copilot CLI** | GitHub Copilot via CLI | `npm install -g @github/copilot` |
| **OpenAI Codex** | OpenAI's Codex for coding | Install Codex extension |

### Starting Background Agent Sessions

**From VS Code Chat View**:
1. Open Chat view (`⌃⌘I`)
2. Select New Chat dropdown → "New Background Agent"

**From Local Chat Session**:
- Type `@cli <task description>` in chat input
- Or select "Continue In" → "Background Agent"

**Via Command Palette**:
- Run `Chat: New Background Agent` (`⇧⌘P`)

### Git Worktree Isolation (Experimental)

Background agents can use Git worktrees to isolate changes:

1. Start a new background agent session
2. Select "Worktree" for isolation mode
3. VS Code creates a separate folder for the session
4. All changes are applied to the worktree, not main workspace
5. Review and merge changes back when complete

**Benefits**:
- Prevents conflicts with active work
- Changes can be reviewed before merging
- Multiple background sessions can run in parallel

### Custom Agents in Background Sessions

To use custom agents with background agents:

1. Enable: `github.copilot.chat.cli.customAgents.enabled` setting
2. Create custom agent in workspace
3. Start background session
4. Select custom agent from Agents dropdown

**Note**: Only workspace-defined custom agents are available for background sessions.

### Limitations of Background Agents

| Feature | Local Agent | Background Agent |
|---------|-------------|------------------|
| VS Code context | ✅ | ❌ |
| MCP servers | ✅ | ❌ |
| Extension tools | ✅ | ❌ |
| Text selections | ✅ | ❌ |
| Failed test access | ✅ | ❌ |
| Terminal commands | ✅ | ✅ |
| Git worktrees | ❌ | ✅ |
| Autonomous execution | Limited | ✅ |

---

## B2Connect Agent Registry

| Agent | Model | Tools | Background |
|-------|-------|-------|------------|
| backend-developer | gpt-4o | read, search, create_file, replace_string_in_file | ❌ |
| frontend-developer | gpt-4o | read, search, create_file, replace_string_in_file | ❌ |
| tech-lead | gpt-4o | agent, execute, vscode, read, search, ... | ❌ |
| process-assistant | gpt-4o | vscode, execute, search, read | ❌ |
| collaboration-monitor | gpt-4o | read, search | ✅ (triggers @team-assistant) |
| team-assistant | gpt-4o | read, search, create_file | ❌ |

---

**Last Updated**: 30. Dezember 2025  
**Maintained By**: @process-assistant
