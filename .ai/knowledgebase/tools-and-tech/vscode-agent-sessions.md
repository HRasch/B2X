---
docid: KB-067
title: VS Code Agent Sessions & Subagents
owner: @CopilotExpert
status: Active
created: 2026-01-08
---

# VS Code Agent Sessions & Subagents

**DocID**: `KB-067`  
**Version**: 1.0  
**Last Updated**: 8. Januar 2026  
**Maintained By**: @CopilotExpert  
**Status**: ✅ Current

---

## Overview

VS Code introduced a unified agent experience in November 2025, providing orchestration for multiple coding agents (GitHub Copilot, Copilot coding agent, Copilot CLI, OpenAI Codex) through **Agent Sessions** and **Subagents**. This addresses context confusion and enables isolated task execution.

**Source**: https://code.visualstudio.com/blogs/2025/11/03/unified-agent-experience

---

## Agent Sessions

Agent Sessions provide a centralized view for managing all agents in VS Code.

### Key Features

| Feature | Description |
|---------|-------------|
| **Unified View** | Single sidebar view showing all agent sessions (local/cloud) |
| **Status Monitoring** | Real-time agent status and progress tracking |
| **Session Navigation** | Jump between sessions with a click |
| **Chat Editors** | Tabbed experience for watching agent progress |
| **Mid-Run Correction** | Course-correct agents during execution |
| **Task Delegation** | Delegate any task to any agent from Chat view |

### Access

- Open via sidebar: **Agent Sessions** view
- Documentation: https://code.visualstudio.com/docs/copilot/chat/chat-sessions#_agent-sessions

---

## Subagents (runSubagent)

Subagents solve **Context Confusion**—the problem where accumulated context leads to agent errors.

### Core Mechanism

```text
Main Agent → Creates focused prompt → Subagent executes in isolation → Returns only final result
```

### Key Characteristics

| Aspect | Behavior |
|--------|----------|
| **Context Isolation** | Subagents receive only the specific prompt context |
| **No Inheritance** | No access to main chat history |
| **Tool Access** | Most tools available (file reading, code analysis, etc.) |
| **No User Feedback** | Executes autonomously without pauses |
| **Result Return** | Only final result joins main context |

### Usage

Add `#runSubagent` tool to your prompt:

```text
Analyze the #file:api with #runSubagent and recommend the best authentication strategy for a web client consuming these endpoints.
```

### Visual Indicator

- Subagent progress shows as expandable tool calls below the main agent action
- Example: "Analyze app structure for auth" with nested tool calls visible

### Documentation

https://code.visualstudio.com/docs/copilot/chat/chat-sessions#_contextisolated-subagents

---

## Planning Agent

Built-in agent for creating detailed plans from vague prompts.

### Features

- Asks clarifying questions before proceeding
- Recommends libraries with comparisons
- Quick reply format for answering questions
- **Handoff** feature to proceed or open plan in editor

### Access

- Select "Plan" from agent dropdown in Chat
- Documentation: https://code.visualstudio.com/docs/copilot/chat/chat-planning

### Pro Tips

1. Change `workbench.action.chat.submit` keybinding to `Ctrl+Enter` for multi-line answers
2. Try Claude models for identifying missing context and edge cases
3. View Plan prompt via Command Palette → "Configure Agents" → Plan

---

## Custom Agents

Agents (formerly "chat modes") allow customizing agent behavior.

### Creating Custom Agents

- Use Command Palette → "Configure Agents"
- Reference the Plan agent as a baseline
- Custom agents work with delegation to CLI, coding agent, etc.

### Example: Research Agent

```markdown
A custom agent that recursively does internet research and writes findings.
```

**Community Resource**: https://github.com/github/awesome-copilot

---

## B2X Integration Recommendations

### Align with Existing Agent System

| B2X Agent | Subagent Use Case |
|-----------|-------------------|
| @SARAH | Coordinate subagent delegation for complex features |
| @Backend | Subagent for isolated .NET analysis/refactoring |
| @Frontend | Subagent for Vue.js component research |
| @Security | Subagent for vulnerability deep-dives |

### Token Optimization (GL-006)

Subagents reduce context bloat:
- Main chat remains lean
- Only final results added to context
- Ideal for research, deep-dives, large file analysis

### Plan-Act-Control (ADR-049)

| Phase | Subagent Application |
|-------|---------------------|
| **Plan** | Use Planning Agent for requirements clarification |
| **Act** | Subagent executes isolated implementation tasks |
| **Control** | Main agent receives results for quality gate review |

### Prompt Enhancement Examples

**Feature Analysis with Subagent**:
```text
@SARAH: /start-feature
Use #runSubagent to analyze requirements for [Feature Title] across backend and frontend domains, then consolidate findings.
```

**Bug Investigation with Subagent**:
```text
@TechLead: /bug-analysis
Use #runSubagent to research root cause in #file:problematic-service.cs without polluting main context.
```

### Governance Alignment (GL-008)

- Subagents inherit permission boundaries (e.g., @Backend subagent → `.cs` files only)
- Document subagent usage in PR descriptions for audit trail
- Use for multi-domain tasks to prevent cross-context contamination

---

## Context Engineering Best Practices

### When to Use Subagents

✅ **Use For**:
- Deep research tasks (APIs, authentication strategies)
- Large file analysis (>200 lines)
- Domain-specific investigations
- Avoiding context drift in long sessions

❌ **Avoid For**:
- Simple, quick tasks
- Tasks requiring user feedback mid-execution
- Cross-dependent operations needing shared state

### Context Management Tips

1. **Scope Prompts Tightly**: Include specific files/directories only
2. **Limit Tool Set**: Reference only necessary tools
3. **Clear Handoffs**: Define expected output format
4. **Monitor Progress**: Use chat editors for visibility

---

## Related Documentation

| DocID | Title | Relevance |
|-------|-------|-----------|
| [KB-022] | GitHub Copilot Customization | Custom agents, instructions |
| [GL-002] | Subagent Delegation | B2X delegation patterns |
| [GL-006] | Token Optimization | Context efficiency |
| [ADR-049] | Plan-Act-Control | Engineering loop integration |
| [GL-008] | Governance Policies | Permission boundaries |

---

## Changelog

| Version | Date | Changes |
|---------|------|---------|
| 1.0 | 2026-01-08 | Initial creation from VS Code blog analysis |

---

**Maintained by**: @CopilotExpert  
**Review Frequency**: Quarterly
