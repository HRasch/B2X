---
docid: ADR-030
title: CMS Tenant Template Overrides Architecture
owner: @SARAH
status: Proposed
created: 2026-01-03
---

# ADR-030: CMS Tenant Template Overrides Architecture

## Context

B2X requires a CMS system that allows tenants to customize store templates with their branding while maintaining system security and performance. The system must support AI-powered validation and helpful advice, plus Monaco editor integration with AI suggestions.

**Business Requirements:**
- Tenants need brand-specific template customizations
- Non-technical users should edit templates safely
- AI assistance reduces errors and speeds development
- Templates must be tenant-isolated and secure

**Technical Constraints:**
- Existing CMS domain with PageDefinition entity
- Multi-tenant PostgreSQL architecture (ADR-004)
- Wolverine CQRS pattern usage
- MCP server for AI integration (REQ-004)
- Vue.js 3 frontend with Monaco editor

## Decision

**We will implement tenant template overrides by extending the existing CMS bounded context with a hierarchical template resolution system, AI validation service, and Monaco editor integration.**

### Core Architecture Decisions

#### 1. Template Storage & Resolution
- **Extend PageDefinition Entity**: Add override metadata and versioning to existing entity
- **Hierarchical Resolution**: Tenant override → Base template → System fallback
- **Section-based Merging**: Allow partial overrides rather than full template replacement
- **Tenant-scoped Storage**: All overrides isolated by tenant_id

#### 2. AI Integration Architecture
- **MCP Server Extension**: Add TemplateValidationTool to existing MCP server
- **Validation Pipeline**: Syntax → Security → Performance → Best Practices
- **Confidence Scoring**: Rate AI suggestions with confidence levels
- **Feedback Loop**: Learn from user acceptance/rejection of suggestions

#### 3. Frontend Architecture
- **Monaco Editor Integration**: Direct integration with monaco-editor package
- **AI Completion Provider**: Custom language server for template syntax
- **Real-time Validation**: Debounced validation with inline error display
- **Live Preview**: Side-by-side editor and rendered preview

#### 4. Security Architecture
- **Template Sandboxing**: Isolated execution context for template rendering
- **Content Sanitization**: HTML sanitization with strict allowlists
- **Tenant Isolation**: Row-level security and scoped queries
- **Audit Logging**: Comprehensive change tracking

## Implementation Details

### Database Schema
```sql
-- Extend existing cms.page_definitions table
ALTER TABLE cms.page_definitions ADD COLUMN
    override_metadata JSONB;

-- New table for override history
CREATE TABLE cms.template_override_history (
    id UUID PRIMARY KEY,
    tenant_id UUID NOT NULL,
    page_definition_id UUID NOT NULL,
    override_content JSONB NOT NULL,
    version INT NOT NULL,
    created_at TIMESTAMP NOT NULL,
    created_by UUID,
    validation_results JSONB,
    FOREIGN KEY (tenant_id) REFERENCES tenants(id),
    FOREIGN KEY (page_definition_id) REFERENCES cms.page_definitions(id)
);

-- Indexes for performance
CREATE INDEX idx_template_overrides_tenant ON cms.template_override_history(tenant_id);
CREATE INDEX idx_template_overrides_page ON cms.template_override_history(page_definition_id);
```

### CQRS Commands & Events
```csharp
// Commands
public record CreateTemplateOverrideCommand(
    Guid TenantId,
    Guid PageDefinitionId,
    string OverrideContent,
    Dictionary<string, string> OverrideSections
) : ICommand;

public record PublishTemplateOverrideCommand(
    Guid TenantId,
    Guid OverrideId
) : ICommand;

// Events
public record TemplateOverrideCreatedEvent(
    Guid TenantId,
    Guid OverrideId,
    int Version
) : IEvent;

public record TemplateOverridePublishedEvent(
    Guid TenantId,
    Guid OverrideId
) : IEvent;
```

### Template Resolution Service
```csharp
public interface ITemplateResolutionService
{
    Task<string> ResolveTemplateAsync(
        Guid tenantId,
        string templateKey,
        CancellationToken ct = default);
}

public class HierarchicalTemplateResolver : ITemplateResolutionService
{
    private readonly ITemplateRepository _repository;
    private readonly ICacheService _cache;

    public async Task<string> ResolveTemplateAsync(
        Guid tenantId,
        string templateKey,
        CancellationToken ct)
    {
        var cacheKey = $"template:{tenantId}:{templateKey}";

        // Check cache first
        var cached = await _cache.GetAsync<string>(cacheKey);
        if (cached != null) return cached;

        // Resolve hierarchically
        var resolved = await ResolveHierarchicalAsync(tenantId, templateKey, ct);

        // Cache result
        await _cache.SetAsync(cacheKey, resolved, TimeSpan.FromMinutes(15));

        return resolved;
    }

    private async Task<string> ResolveHierarchicalAsync(
        Guid tenantId,
        string templateKey,
        CancellationToken ct)
    {
        // 1. Get tenant override if exists
        var override = await _repository.GetOverrideAsync(tenantId, templateKey, ct);

        // 2. Get base template
        var baseTemplate = await _repository.GetBaseTemplateAsync(templateKey, ct);

        // 3. Merge with override sections
        return override != null
            ? MergeTemplates(baseTemplate, override)
            : baseTemplate;
    }
}
```

### AI Validation Service
```csharp
public interface ITemplateValidationService
{
    Task<ValidationResult> ValidateAsync(
        string templateContent,
        Guid tenantId,
        CancellationToken ct = default);
}

public class AiPoweredTemplateValidator : ITemplateValidationService
{
    private readonly IMcpClient _mcpClient;

    public async Task<ValidationResult> ValidateAsync(
        string templateContent,
        Guid tenantId,
        CancellationToken ct)
    {
        // Call MCP server for AI validation
        var validationRequest = new TemplateValidationRequest
        {
            Content = templateContent,
            TenantId = tenantId,
            ValidationTypes = new[] {
                ValidationType.Syntax,
                ValidationType.Security,
                ValidationType.Performance,
                ValidationType.BestPractices
            }
        };

        return await _mcpClient.ValidateTemplateAsync(validationRequest, ct);
    }
}
```

### Frontend Monaco Integration
```typescript
// MonacoEditor.vue
import * as monaco from 'monaco-editor';

export default {
  name: 'MonacoEditor',
  props: {
    value: String,
    language: {
      type: String,
      default: 'html'
    }
  },
  mounted() {
    this.initEditor();
    this.setupAiCompletion();
    this.setupValidation();
  },
  methods: {
    initEditor() {
      this.editor = monaco.editor.create(this.$refs.editor, {
        value: this.value,
        language: this.language,
        theme: 'vs-dark',
        automaticLayout: true
      });
    },

    async setupAiCompletion() {
      monaco.languages.registerCompletionItemProvider(this.language, {
        provideCompletionItems: async (model, position) => {
          const suggestions = await this.fetchAiSuggestions(
            model.getValue(),
            position
          );
          return { suggestions };
        }
      });
    },

    async setupValidation() {
      // Debounced validation
      this.editor.onDidChangeModelContent(async () => {
        clearTimeout(this.validationTimeout);
        this.validationTimeout = setTimeout(async () => {
          await this.validateContent();
        }, 500);
      });
    }
  }
}
```

## Consequences

### Positive Consequences

#### ✅ **Business Benefits**
- **Tenant Customization**: Enables brand-specific store experiences
- **User Experience**: AI assistance reduces technical barriers
- **Scalability**: Tenant-isolated overrides don't affect others
- **Maintainability**: Hierarchical resolution simplifies management

#### ✅ **Technical Benefits**
- **Incremental Adoption**: Extends existing CMS without disruption
- **Performance**: Caching ensures fast template resolution
- **Security**: Sandboxed execution and tenant isolation
- **Extensibility**: AI service can be enhanced over time

### Negative Consequences

#### ⚠️ **Complexity**
- **Template Resolution**: Hierarchical merging adds complexity
- **AI Integration**: Requires robust error handling for AI failures
- **Monaco Integration**: Large bundle size and browser compatibility

#### ⚠️ **Performance Risks**
- **Cache Management**: Invalidation complexity with multiple tenants
- **AI Latency**: Validation delays could impact user experience
- **Memory Usage**: Monaco editor resource requirements

#### ⚠️ **Security Considerations**
- **Template Injection**: Requires careful sandboxing implementation
- **AI Trust**: Must validate AI outputs don't introduce vulnerabilities
- **Tenant Data**: Strict isolation prevents data leakage

## Alternatives Considered

### Alternative 1: Separate Bounded Context
**Pros**: Clean separation, independent scaling
**Cons**: Cross-cutting concerns, duplicate entities, integration overhead
**Decision**: Rejected - CMS template management is inherently part of CMS domain

### Alternative 2: Full Template Replacement
**Pros**: Simpler implementation
**Cons**: No inheritance, larger storage requirements, harder maintenance
**Decision**: Rejected - Section-based overrides provide better flexibility

### Alternative 3: Client-side AI Only
**Pros**: No server dependency
**Cons**: Limited validation scope, security risks, performance issues
**Decision**: Rejected - Server-side validation required for security

### Alternative 4: Custom Editor Instead of Monaco
**Pros**: Smaller bundle, full control
**Cons**: Development effort, missing features, maintenance burden
**Decision**: Rejected - Monaco provides mature AI integration and features

## Implementation Plan

### Phase 1: Core Template Override System (2 sprints)
- Extend PageDefinition entity with override metadata
- Implement basic template resolution service
- Add CQRS commands for override management
- Create database migrations and indexes

### Phase 2: AI Validation Service (2 sprints)
- Extend MCP server with TemplateValidationTool
- Implement validation pipeline (syntax, security, performance)
- Add confidence scoring and feedback collection
- Integrate with backend validation service

### Phase 3: Monaco Editor Integration (2 sprints)
- Create MonacoEditor Vue component
- Implement AI completion provider
- Add real-time validation feedback
- Integrate live preview functionality

### Phase 4: Advanced Features (1-2 sprints)
- Template versioning and rollback
- A/B testing for template changes
- Analytics and usage tracking
- Performance optimization and monitoring

## Success Metrics

### Technical Metrics
- **Template Resolution**: <100ms average response time
- **AI Validation**: <2 seconds for complex templates
- **Editor Load Time**: <3 seconds initialization
- **Cache Hit Rate**: >95% for resolved templates

### Business Metrics
- **Adoption Rate**: >80% of tenants create at least one override
- **Error Reduction**: <5% template errors after AI validation
- **User Satisfaction**: >4.5/5 rating for editing experience
- **Development Speed**: 50% faster template creation with AI assistance

## Monitoring & Observability

### Application Metrics
- Template resolution latency by tenant
- AI validation success/failure rates
- Cache hit/miss ratios
- Editor load times and errors

### Business Metrics
- Template override creation frequency
- AI suggestion acceptance rates
- Validation error types and frequencies
- User feedback on AI assistance

### Alerting
- Template resolution >500ms
- AI validation failures >5%
- Cache miss rate >20%
- Editor load time >10 seconds

## Security Measures

### Template Security
- **Sandboxing**: Isolated execution environment for template rendering
- **Sanitization**: HTML sanitization with strict allowlists
- **Validation**: Pre-execution syntax and security checks
- **Auditing**: Complete audit trail of template changes

### AI Security
- **Input Filtering**: Sanitize all inputs to AI services
- **Output Validation**: Filter and validate AI suggestions
- **Rate Limiting**: Prevent abuse of AI validation services
- **Logging**: Audit all AI interactions

### Tenant Isolation
- **Database Level**: Row-level security on all template tables
- **Application Level**: Tenant-scoped queries and caching
- **Runtime Checks**: Validate tenant context in all operations

## Migration Strategy

### Database Migration
- Add new columns to existing tables (non-breaking)
- Create new history table
- Update indexes for performance
- Backfill existing templates as base templates

### Application Migration
- Deploy new services alongside existing CMS
- Feature flag for gradual rollout
- Backward compatibility for existing templates
- Rollback plan for each phase

### User Migration
- Documentation and training for new features
- Gradual feature introduction
- Support channels for template editing assistance

## Risks & Mitigations

### High Risk: AI Accuracy
**Impact**: Incorrect suggestions could introduce bugs or security issues
**Mitigation**: Human oversight, confidence scoring, feedback loops, gradual rollout

### High Risk: Performance Degradation
**Impact**: Slow template resolution affects user experience
**Mitigation**: Comprehensive caching, performance monitoring, optimization

### Medium Risk: Security Vulnerabilities
**Impact**: Template injection could compromise tenant data
**Mitigation**: Sandboxing, sanitization, security testing, audit logging

### Medium Risk: Adoption Resistance
**Impact**: Tenants may not use advanced features
**Mitigation**: User training, gradual introduction, feedback collection

## Related Decisions

- **ADR-004**: PostgreSQL Multitenancy - Tenant isolation foundation
- **ADR-022**: Multi-Tenant Domain Management - Tenant context handling
- **ADR-027**: Email Template Engine - Template rendering patterns
- **REQ-004**: MCP Server for Tenant Administrators - AI integration foundation

## Next Steps

1. **Prototype Development**: Build proof-of-concept for template resolution
2. **Security Review**: Conduct threat modeling and penetration testing
3. **Performance Benchmarking**: Establish baseline metrics
4. **Implementation Planning**: Detailed sprint breakdown and resource allocation
5. **Stakeholder Review**: Present ADR to product and development teams

---

**Status**: Proposed - Ready for technical review and implementation planning

**Reviewers**: @Architect, @Backend, @Frontend, @Security, @QA

**Next Review**: 2026-01-10

**Implementation Start**: Phase 1 - Week of 2026-01-13</content>
<parameter name="filePath">/Users/holger/Documents/Projekte/B2X/.ai/decisions/ADR-030-cms-tenant-template-overrides-architecture.md