---
docid: REQ-004-SPM
title: System Prompt Management for MCP Server
owner: @SARAH
status: Draft
created: 2026-01-03
---

# System Prompt Management Design

## Overview
Tenant-admins can customize system prompts for MCP tools through the Management frontend, allowing them to tailor AI behavior to their specific needs and workflows.

## Database Schema

### Tenant System Prompts Table
```sql
CREATE TABLE tenant_ai.system_prompts (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL,
    tool_category VARCHAR(50) NOT NULL, -- 'cms', 'email', 'health', 'admin'
    tool_name VARCHAR(100) NOT NULL, -- specific tool identifier
    prompt_text TEXT NOT NULL,
    prompt_version INTEGER NOT NULL DEFAULT 1,
    variables JSONB, -- available template variables
    is_active BOOLEAN NOT NULL DEFAULT true,
    created_by UUID NOT NULL, -- admin user ID
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ DEFAULT NOW(),
    
    UNIQUE(tenant_id, tool_category, tool_name, prompt_version)
);

-- Indexes for performance
CREATE INDEX idx_system_prompts_tenant_active 
ON tenant_ai.system_prompts(tenant_id, tool_category, tool_name) 
WHERE is_active = true;
```

### Prompt Version History
```sql
CREATE TABLE tenant_ai.prompt_history (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    prompt_id UUID NOT NULL REFERENCES tenant_ai.system_prompts(id),
    tenant_id UUID NOT NULL,
    previous_version INTEGER,
    new_version INTEGER,
    changes TEXT, -- diff or summary of changes
    changed_by UUID NOT NULL,
    changed_at TIMESTAMPTZ DEFAULT NOW()
);
```

## API Endpoints

### Get System Prompts
```csharp
[HttpGet("api/v1/tenants/{tenantId}/ai/prompts")]
public async Task<ActionResult<List<SystemPromptDto>>> GetPrompts(
    [FromRoute] Guid tenantId,
    [FromQuery] string? category = null)
```

### Get Specific Prompt
```csharp
[HttpGet("api/v1/tenants/{tenantId}/ai/prompts/{toolCategory}/{toolName}")]
public async Task<ActionResult<SystemPromptDto>> GetPrompt(
    [FromRoute] Guid tenantId,
    [FromRoute] string toolCategory,
    [FromRoute] string toolName)
```

### Update Prompt
```csharp
[HttpPut("api/v1/tenants/{tenantId}/ai/prompts/{toolCategory}/{toolName}")]
public async Task<IActionResult> UpdatePrompt(
    [FromRoute] Guid tenantId,
    [FromRoute] string toolCategory,
    [FromRoute] string toolName,
    [FromBody] UpdatePromptRequest request)
```

### Reset to Default
```csharp
[HttpPost("api/v1/tenants/{tenantId}/ai/prompts/{toolCategory}/{toolName}/reset")]
public async Task<IActionResult> ResetToDefault(
    [FromRoute] Guid tenantId,
    [FromRoute] string toolCategory,
    [FromRoute] string toolName)
```

### Get Prompt History
```csharp
[HttpGet("api/v1/tenants/{tenantId}/ai/prompts/{toolCategory}/{toolName}/history")]
public async Task<ActionResult<List<PromptHistoryDto>>> GetPromptHistory(
    [FromRoute] Guid tenantId,
    [FromRoute] string toolCategory,
    [FromRoute] string toolName)
```

## Frontend Components

### Prompt Editor Page
```vue
<template>
  <div class="prompt-editor-page">
    <div class="page-header">
      <h1>System Prompt Editor</h1>
      <p>Customize AI behavior for administrative tasks</p>
    </div>
    
    <div class="tool-selector">
      <select v-model="selectedTool">
        <optgroup label="CMS Tools">
          <option value="cms.list_components">List Components</option>
          <option value="cms.create_page">Create Page</option>
          <option value="cms.preview_page">Preview Page</option>
        </optgroup>
        <optgroup label="Email Tools">
          <option value="email.create_template">Create Template</option>
          <option value="email.preview_template">Preview Template</option>
        </optgroup>
        <!-- More tool options -->
      </select>
    </div>
    
    <PromptEditor 
      v-if="selectedTool"
      :tool="selectedTool"
      :current-prompt="currentPrompt"
      @save="savePrompt"
      @reset="resetToDefault"
      @preview="previewPrompt" />
  </div>
</template>
```

### Prompt Editor Component
```vue
<template>
  <Card title="Prompt Editor">
    <div class="editor-toolbar">
      <button @click="insertVariable" class="btn-secondary">
        Insert Variable
      </button>
      <button @click="validatePrompt" class="btn-secondary">
        Validate
      </button>
      <button @click="previewPrompt" class="btn-secondary">
        Preview
      </button>
    </div>
    
    <div class="editor-container">
      <textarea 
        v-model="promptText"
        class="prompt-textarea"
        placeholder="Enter your custom system prompt..."
        @input="onPromptChange">
      </textarea>
    </div>
    
    <div class="variables-panel">
      <h4>Available Variables</h4>
      <div class="variable-list">
        <div 
          v-for="variable in availableVariables" 
          :key="variable.name"
          class="variable-item"
          @click="insertVariable(variable)">
          <code>{{ variable.placeholder }}</code>
          <span>{{ variable.description }}</span>
        </div>
      </div>
    </div>
    
    <div class="actions">
      <button @click="save" :disabled="!hasChanges" class="btn-primary">
        Save Changes
      </button>
      <button @click="reset" class="btn-secondary">
        Reset to Default
      </button>
    </div>
  </Card>
</template>
```

### Prompt History Component
```vue
<template>
  <Card title="Prompt History">
    <div class="history-list">
      <div 
        v-for="entry in history" 
        :key="entry.id"
        class="history-entry"
        @click="viewDiff(entry)">
        <div class="entry-header">
          <span class="version">v{{ entry.version }}</span>
          <span class="timestamp">{{ formatDate(entry.changedAt) }}</span>
          <span class="author">{{ entry.changedBy }}</span>
        </div>
        <div class="changes-summary">
          {{ entry.changes }}
        </div>
      </div>
    </div>
    
    <div class="history-actions">
      <button @click="rollback" :disabled="!selectedEntry" class="btn-warning">
        Rollback to Selected Version
      </button>
    </div>
  </Card>
</template>
```

## MCP Server Integration

### Prompt Loading Service
```csharp
public class TenantPromptService
{
    private readonly ITenantPromptRepository _repository;
    private readonly IMemoryCache _cache;
    
    public async Task<string> GetPromptForToolAsync(
        Guid tenantId, 
        string toolCategory, 
        string toolName)
    {
        var cacheKey = $"prompt:{tenantId}:{toolCategory}:{toolName}";
        
        return await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
            
            var prompt = await _repository.GetActivePromptAsync(
                tenantId, toolCategory, toolName);
            
            return prompt?.PromptText ?? GetDefaultPrompt(toolCategory, toolName);
        });
    }
    
    private string GetDefaultPrompt(string toolCategory, string toolName)
    {
        // Return built-in default prompts
        return _defaultPrompts[$"{toolCategory}.{toolName}"];
    }
}
```

### Tool Execution with Custom Prompts
```csharp
public class McpToolExecutor
{
    private readonly ITenantPromptService _promptService;
    private readonly IAiProvider _aiProvider;
    
    public async Task<McpToolResult> ExecuteToolAsync(
        string toolName, 
        Dictionary<string, object> parameters,
        HttpContext context)
    {
        var tenantId = context.GetTenantId();
        var toolCategory = GetToolCategory(toolName);
        
        // Load tenant-specific prompt
        var systemPrompt = await _promptService.GetPromptForToolAsync(
            tenantId, toolCategory, toolName);
        
        // Apply variable substitution
        systemPrompt = SubstituteVariables(systemPrompt, parameters);
        
        // Execute with custom prompt
        var result = await _aiProvider.ExecuteWithPromptAsync(
            systemPrompt, parameters);
        
        return result;
    }
}
```

## Security Considerations

### Prompt Validation
```csharp
public class PromptValidator
{
    private readonly List<string> _forbiddenPatterns = new()
    {
        @"system\(", @"exec\(", @"eval\(", @"import\s+os", @"import\s+subprocess"
    };
    
    public ValidationResult ValidatePrompt(string prompt)
    {
        var errors = new List<string>();
        
        foreach (var pattern in _forbiddenPatterns)
        {
            if (Regex.IsMatch(prompt, pattern, RegexOptions.IgnoreCase))
            {
                errors.Add($"Forbidden pattern detected: {pattern}");
            }
        }
        
        // Check prompt length
        if (prompt.Length > 10000)
        {
            errors.Add("Prompt too long (max 10000 characters)");
        }
        
        return new ValidationResult { IsValid = !errors.Any(), Errors = errors };
    }
}
```

### Audit Logging
```csharp
public class PromptAuditService
{
    public async Task LogPromptChangeAsync(
        Guid tenantId, 
        string toolCategory, 
        string toolName,
        Guid adminId,
        string oldPrompt, 
        string newPrompt)
    {
        var diff = GenerateDiff(oldPrompt, newPrompt);
        
        await _auditLog.CreateEntryAsync(new AuditEntry
        {
            TenantId = tenantId,
            AdminId = adminId,
            Action = "prompt_updated",
            Resource = $"{toolCategory}.{toolName}",
            Details = new { Diff = diff },
            Timestamp = DateTime.UtcNow
        });
    }
}
```

## Template Variables

### Available Variables by Tool Category

#### CMS Tools
- `{{tenant_name}}` - Name of the tenant
- `{{admin_name}}` - Name of the admin user
- `{{available_components}}` - List of available CMS components
- `{{page_requirements}}` - Specific page requirements from parameters
- `{{brand_guidelines}}` - Tenant brand guidelines

#### Email Tools
- `{{tenant_name}}` - Name of the tenant
- `{{email_purpose}}` - Purpose of the email (transactional, marketing)
- `{{recipient_type}}` - Type of recipient (customer, admin, etc.)
- `{{brand_tone}}` - Brand tone guidelines
- `{{legal_requirements}}` - Legal requirements for emails

#### Health Monitoring Tools
- `{{tenant_name}}` - Name of the tenant
- `{{system_components}}` - List of monitored components
- `{{alert_thresholds}}` - Current alert thresholds
- `{{support_contact}}` - Tenant support contact information

#### Administrative Tools
- `{{tenant_name}}` - Name of the tenant
- `{{admin_role}}` - Role of the admin user
- `{{user_permissions}}` - Available user permissions
- `{{compliance_requirements}}` - Tenant compliance requirements

## Implementation Phases

### Phase 1: Core Infrastructure (1 week)
- Database schema for prompts
- Basic CRUD API endpoints
- Prompt validation service
- Default prompt definitions

### Phase 2: Frontend Editor (1 week)
- Prompt editor component
- Variable insertion functionality
- Preview and validation
- History viewing

### Phase 3: MCP Integration (1 week)
- Prompt loading service
- Variable substitution
- Caching and performance optimization
- Fallback mechanisms

### Phase 4: Advanced Features (1 week)
- Version control and rollback
- Bulk prompt operations
- Template management
- Advanced validation rules

## Testing Strategy

### Unit Tests
- Prompt validation logic
- Variable substitution
- Cache behavior
- Error handling

### Integration Tests
- Full prompt CRUD cycle
- MCP server prompt loading
- Frontend-backend integration
- Multi-tenant isolation

### Security Tests
- Prompt injection prevention
- Authorization validation
- Audit logging verification
- Performance under load

## Performance Considerations

### Caching Strategy
- In-memory cache for active prompts (30-minute TTL)
- Redis for cross-instance cache sharing
- Cache invalidation on prompt updates

### Database Optimization
- Partitioning by tenant for large deployments
- Read replicas for prompt retrieval
- Connection pooling and query optimization

### API Performance
- Response compression for large prompts
- Pagination for prompt history
- Async operations for non-blocking updates

This design provides tenant-admins with powerful customization capabilities while maintaining security, performance, and usability standards.</content>
<parameter name="filePath">/Users/holger/Documents/Projekte/B2X/.ai/requirements/REQ-004-system-prompt-management.md