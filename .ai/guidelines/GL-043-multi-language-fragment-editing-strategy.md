---
docid: GL-043
title: Multi-Language Fragment Editing Strategy
owner: @CopilotExpert
status: Active
created: 2026-01-08
---

# Multi-Language Fragment Editing Strategy

**DocID**: `GL-043`  
**Owner**: @CopilotExpert  
**Status**: Active  
**Created**: 2026-01-08  

## Overview

This guideline establishes a token-saving approach for editing large files across multiple programming languages (C#, TypeScript, Vue.js, etc.) using Model Context Protocol (MCP) tools. The strategy focuses on fragment-based editing to achieve 75-85% token reduction while maintaining code quality and consistency.

## Core Principles

1. **Fragment-Based Editing**: Edit only relevant code fragments instead of entire files
2. **MCP Tool Integration**: Leverage language-specific MCP tools for precise operations
3. **Token Optimization**: Minimize context by using targeted edits and references
4. **Quality Gates**: Automated validation to ensure correctness

## Language-Specific Strategies

### C# (.NET Backend)

**MCP Tools**: Roslyn MCP Server, Wolverine MCP

**Fragment Patterns**:
- Method-level editing for business logic
- Class fragment updates for domain models
- Interface implementation fragments
- Dependency injection configuration blocks

**Token Savings Target**: 80%

**Integration Pattern**:
```csharp
// Fragment: Business logic method
public async Task<Result<Order>> ProcessOrderAsync(OrderRequest request)
{
    // Edit only this method body, reference external dependencies
    var validation = await _validator.ValidateAsync(request);
    if (!validation.IsValid) return Result.Failure<Order>(validation.Errors);
    
    // ... rest of method
}
```

### TypeScript/JavaScript (Frontend)

**MCP Tools**: TypeScript MCP Server, ESLint MCP

**Fragment Patterns**:
- Component method fragments
- State management slices
- API integration functions
- Type definitions

**Token Savings Target**: 75%

**Integration Pattern**:
```typescript
// Fragment: React component method
const handleSubmit = async (formData: FormData) => {
  try {
    const response = await apiClient.post('/orders', formData);
    setOrders(prev => [...prev, response.data]);
  } catch (error) {
    setError('Submission failed');
  }
};
```

### Vue.js (Frontend Components)

**MCP Tools**: Vue MCP Server, Vite MCP

**Fragment Patterns**:
- Template sections
- Script setup blocks
- Style scoped fragments
- Composition function exports

**Token Savings Target**: 78%

**Integration Pattern**:
```vue
<!-- Fragment: Template section -->
<template>
  <div class="order-form">
    <form @submit.prevent="handleSubmit">
      <!-- Edit only form fields -->
    </form>
  </div>
</template>

<script setup lang="ts">
// Fragment: Composition logic
const handleSubmit = async () => {
  // Implementation
};
</script>
```

### Python (Scripts/Tools)

**MCP Tools**: Pylance MCP, Python Environment MCP

**Fragment Patterns**:
- Function definitions
- Class methods
- Configuration blocks
- Test case implementations

**Token Savings Target**: 82%

## MCP Tool Integration Patterns

### Fragment Extraction
1. Use `read_file` with specific line ranges
2. Apply MCP refactoring tools (e.g., `mcp_pylance_mcp_s_pylanceInvokeRefactoring`)
3. Validate with syntax checkers
4. Commit fragment changes

### Token Optimization Techniques

1. **Reference External Context**: Use DocIDs instead of embedding full content
2. **Incremental Updates**: Edit one fragment at a time
3. **Smart Imports**: Only include necessary using/import statements
4. **Context-Free Edits**: Ensure fragments are self-contained

**Target Metrics**:
- 75-85% token reduction per edit
- <5% error rate in fragment validation
- <10 minute average edit time

## Validation Checklists

### Pre-Edit Checklist
- [ ] Identify target language and MCP tools
- [ ] Determine fragment boundaries (methods, classes, components)
- [ ] Check dependencies and imports
- [ ] Verify syntax with MCP validators

### Post-Edit Checklist
- [ ] Run syntax validation (e.g., `mcp_pylance_mcp_s_pylanceFileSyntaxErrors`)
- [ ] Execute relevant tests
- [ ] Check for breaking changes
- [ ] Update documentation references

### Quality Gates

1. **Syntax Gate**: All fragments pass language-specific syntax checks
2. **Dependency Gate**: No broken imports or references
3. **Test Gate**: Related tests pass (minimum 80% coverage maintained)
4. **Integration Gate**: Build succeeds and services start correctly

## Implementation Workflow

1. **Planning**: Identify edit scope and required fragments
2. **Preparation**: Gather MCP tool configurations
3. **Execution**: Apply fragment edits sequentially
4. **Validation**: Run quality gates
5. **Commit**: Small, focused commits with clear messages

## Monitoring & Metrics

- Track token usage per edit session
- Monitor error rates and rollback frequency
- Measure time-to-completion for large file edits
- Report on adoption rates across agents

## References

- [GL-006] Token Optimization Strategy
- [GL-008] Governance Policies
- [ADR-050] TypeScript MCP Server for AI-Assisted Development

---

**Maintained by**: @CopilotExpert  
**Review Cycle**: Monthly  
**Last Reviewed**: 2026-01-08</content>
<parameter name="filePath">c:\Users\Holge\repos\B2Connect\.ai\guidelines\GL-011-multi-language-fragment-editing-strategy.md