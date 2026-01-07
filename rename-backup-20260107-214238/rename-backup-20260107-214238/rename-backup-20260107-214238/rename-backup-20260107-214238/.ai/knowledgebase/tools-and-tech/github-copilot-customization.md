# GitHub Copilot Customization Guide

**DocID**: `KB-022`  
**Version**: 1.1  
**Last Updated**: 3. Januar 2026  
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

Model Context Protocol (MCP) extends Copilot with external tools. In B2X, all MCP servers must enforce tenant isolation at every level to prevent cross-tenant data access.

### Configuration File

**File**: `.vscode/mcp.json`

```json
{
  "servers": {
    "database": {
      "command": "npx",
      "args": ["-y", "@modelcontextprotocol/server-postgres"],
      "env": {
        "DATABASE_URL": "${env:DATABASE_URL}",
        "TENANT_ID": "${env:TENANT_ID}"
      }
    },
    "filesystem": {
      "command": "npx",
      "args": ["-y", "@modelcontextprotocol/server-filesystem", "/path/to/tenant-data"],
      "env": {
        "TENANT_ISOLATION_ENABLED": "true",
        "TENANT_ID": "${env:TENANT_ID}"
      }
    }
  }
}
```

### Security Requirements for MCP Servers

#### Mandatory Tenant Context
Every MCP tool invocation must include tenant validation:
- **Header Injection**: All requests include `X-Tenant-ID` header
- **Environment Variables**: `TENANT_ID` must be set and validated
- **Context Propagation**: Tenant context flows through all tool chains

#### Security Layers
1. **Authentication**: JWT token validation with tenant claims
2. **Authorization**: Role-based access control per tenant
3. **Data Filtering**: Row-level security at database/query level
4. **Audit Logging**: All operations logged with tenant context

#### Failure Modes
- **Missing Tenant ID**: Deny access with error code `TENANT_ISOLATION_VIOLATION`
- **Invalid Tenant ID**: Log security event, deny access
- **Cross-Tenant Access Attempt**: Immediate alert, access denied, user session terminated

#### Audit & Monitoring
- **Comprehensive Logging**: All tenant-related operations logged
- **Real-time Alerts**: Automated alerts for isolation violations
- **Audit Trails**: Immutable logs for compliance (GDPR, NIS2)

#### Implementation Guards
- **Zero-Trust Defaults**: Assume tenant context is invalid unless proven
- **Fail-Safe Behavior**: Deny access on any validation failure
- **Performance Optimized**: Efficient validation without bottlenecks

### Code Patterns for Tenant Validation

#### MCP Tool Wrapper Pattern
```typescript
// Example: Database query tool with tenant isolation
class TenantIsolatedDatabaseTool {
  async execute(query: string, tenantId: string) {
    // 1. Validate tenant context
    if (!tenantId || !this.isValidTenant(tenantId)) {
      throw new Error('TENANT_ISOLATION_VIOLATION: Invalid or missing tenant ID');
    }
    
    // 2. Inject tenant filter into query
    const isolatedQuery = this.injectTenantFilter(query, tenantId);
    
    // 3. Log operation
    this.auditLog('database_query', { tenantId, query: isolatedQuery });
    
    // 4. Execute with isolation
    return await this.database.execute(isolatedQuery);
  }
  
  private injectTenantFilter(query: string, tenantId: string): string {
    // Add WHERE tenant_id = ? clause to all queries
    return query.replace(/WHERE/i, `WHERE tenant_id = '${tenantId}' AND `);
  }
}
```

#### File System Access Pattern
```typescript
class TenantIsolatedFileSystemTool {
  async readFile(path: string, tenantId: string) {
    // 1. Validate tenant access to path
    if (!this.isTenantAllowedPath(tenantId, path)) {
      this.alertSecurityViolation('file_access_denied', { tenantId, path });
      throw new Error('TENANT_ISOLATION_VIOLATION: Access denied');
    }
    
    // 2. Resolve tenant-specific path
    const tenantPath = this.resolveTenantPath(path, tenantId);
    
    // 3. Audit and read
    this.auditLog('file_read', { tenantId, path: tenantPath });
    return await fs.readFile(tenantPath);
  }
}
```

### Monitoring and Alerting Strategy

#### Metrics to Monitor
- **Isolation Violations**: Count of denied cross-tenant attempts
- **Validation Latency**: Time taken for tenant validation
- **Audit Log Volume**: Rate of logged operations
- **Error Rates**: Failures in tenant validation

#### Alerting Rules
```yaml
# Prometheus alerting rules
groups:
  - name: tenant_isolation
    rules:
      - alert: TenantIsolationViolation
        expr: rate(tenant_isolation_violations_total[5m]) > 0
        labels:
          severity: critical
        annotations:
          summary: "Tenant isolation violation detected"
          
      - alert: TenantValidationLatencyHigh
        expr: histogram_quantile(0.95, rate(tenant_validation_duration_seconds[5m])) > 0.1
        labels:
          severity: warning
        annotations:
          summary: "Tenant validation latency is high"
```

#### Dashboard Panels
- Real-time violation count
- Top violating tenants/IPs
- Validation performance trends
- Audit log health checks

### Risk Assessment and Mitigation

#### High-Risk Scenarios
1. **MCP Server Compromise**: Attacker gains access to MCP server
   - **Mitigation**: Network isolation, regular security scans, zero-trust authentication
   
2. **Tenant ID Injection Attack**: Malicious tenant ID in requests
   - **Mitigation**: Strict validation, allowlist of valid tenant IDs, input sanitization
   
3. **Performance Degradation**: Validation overhead slows operations
   - **Mitigation**: Caching of validation results, async validation, optimized queries

#### Compliance Requirements
- **GDPR**: Data isolation, audit trails, right to erasure
- **NIS2**: Security incident reporting, resilience measures
- **BITV 2.0**: Accessibility in monitoring interfaces

#### Backup and Recovery
- **Audit Log Backup**: Encrypted, immutable backups
- **Incident Response**: Automated playbooks for violations
- **Forensic Analysis**: Tools for investigating breaches

### Using MCP Tools in Agents

```yaml
---
tools: ['database/*', 'filesystem/*', 'search']
mcp-servers: ['tenant-database', 'tenant-filesystem']
---
```

**Security Note**: Only use MCP tools that implement tenant isolation. All custom MCP servers must pass security review before deployment.

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

## 8. B2X Configuration

### Current Structure

```
.github/
├── copilot-instructions.md      # Repository-wide (INS-000)
├── agents/
│   ├── sarah.agent.md           # Coordinator
│   ├── backend.agent.md         # .NET development
│   ├── frontend.agent.md        # Vue.js development
│   ├── CopilotExpert.agent.md  # This specialist
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
| 2026-01-03 | 1.1 | Enhanced MCP Servers section with comprehensive tenant isolation design, security layers, monitoring, and risk assessment |
| 2026-01-02 | 1.0 | Initial documentation |

---

**Next Review**: 2026-04-03 (Quarterly)
