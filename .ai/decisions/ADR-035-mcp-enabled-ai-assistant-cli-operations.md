---
docid: ADR-035
title: MCP-Enabled AI Assistant with CLI Operations Access
status: Proposed
owner: @Backend
date: 2026-01-05
---

# ADR-035: MCP-Enabled AI Assistant with CLI Operations Access

## Context

The B2Connect administration requires intelligent troubleshooting capabilities. Previous brainstorming explored three approaches:

1. **Direct CLI Integration**: Embed AI directly in CLI commands
2. **Admin Frontend Integration**: Use existing AI assistant in admin UI
3. **MCP-Enabled Assistant**: Connect AI assistant via MCP to invoke CLI operations

This ADR proposes the third approach as the optimal solution.

## Decision

**Implement MCP-enabled AI assistant that can invoke CLI operations for system diagnostics and troubleshooting.**

### Implementation Details

#### MCP Server Enhancement
- Extend existing `B2Connect.Admin.MCP` server with CLI operation tools
- Add `SystemHealthAnalysisTool` that executes `b2connect-admin health check` commands
- Enable AI assistant to call CLI operations programmatically via Process.Start

#### CLI Health Commands
- Add `health check` command to administration CLI
- Support component-specific checks (gateway, database, elasticsearch)
- Provide structured output for AI analysis

#### AI Assistant Integration
- AI assistant available when MCP connection established
- Uses CLI operations for real-time system checks
- Provides intelligent analysis and recommendations

## Consequences

### Positive
- **Modular Architecture**: AI logic separate from CLI operations
- **Reusability**: CLI commands remain CLI-first but accessible to AI
- **Extensibility**: Easy to add new CLI operations as MCP tools
- **Standards Compliance**: Uses MCP protocol for tool calling
- **Security**: MCP handles authentication and authorization

### Negative
- **Dependency Chain**: Requires MCP server + CLI availability
- **Process Overhead**: Spawning CLI processes for each check
- **Setup Complexity**: MCP server configuration and tool registration

### Risks
- **Performance**: CLI startup time may impact response times
- **Error Handling**: Need robust error handling for CLI failures
- **Version Compatibility**: CLI and MCP server must stay synchronized

## Alternatives Considered

### Direct CLI AI Integration
- **Rejected**: Would bloat CLI with AI dependencies and complicate command-line usage

### Admin Frontend AI Enhancement
- **Deferred**: Good for interactive UI-based troubleshooting, but CLI approach better for automation and headless environments

## Implementation Plan

1. **Phase 1**: Add health check commands to administration CLI
2. **Phase 2**: Enhance SystemHealthAnalysisTool to invoke CLI operations
3. **Phase 3**: Test MCP tool integration and AI assistant workflows
4. **Phase 4**: Add additional CLI operations as MCP tools (logs, diagnostics)

## Success Metrics

- AI assistant can successfully execute CLI health checks
- Response time < 10 seconds for typical health analysis
- Accurate diagnosis and actionable recommendations
- No security vulnerabilities introduced

## Related Documents

- [ADR-030](../decisions/ADR-030-cms-tenant-template-overrides-architecture.md) - MCP server for AI integration
- [GL-008](../guidelines/GL-008-GOVERNANCE-POLICIES.md) - Governance policies for MCP configuration</content>
<parameter name="filePath">/Users/holger/Documents/Projekte/B2Connect/.ai/decisions/ADR-035-mcp-enabled-ai-assistant-cli-operations.md