---
docid: PRM-055
title: Typescript Review.Prompt
owner: @CopilotExpert
status: Active
created: 2026-01-08
---

# TypeScript Code Review Prompt

**DocID**: `PRM-016`
**Purpose**: Automated TypeScript analysis and review using MCP tools
**Owner**: @TechLead
**Command**: `/typescript-review`

## Usage

```bash
@TechLead: /typescript-review
Component: [frontend | admin | management]
Scope: [file-path | component-name | directory]
Focus: [types | symbols | usage | all]
Severity: [info | warning | error]
```

## Process Flow

### 1. Symbol Analysis
- **search_symbols**: Discover all TypeScript symbols in scope
- **get_symbol_info**: Analyze symbol definitions and types
- **find_usages**: Track symbol usage patterns

### 2. Type Validation
- **analyze_types**: Run TypeScript compiler checks
- Report type errors, unused imports, missing types

### 3. Code Quality Assessment
- Interface compliance with project standards
- Type safety violations
- Import/export consistency

### 4. Recommendations
- Refactoring suggestions based on usage analysis
- Type improvements and optimizations
- Documentation gaps for complex types

## Output Format

```
📋 TypeScript Review: [Component]

🔍 **Symbols Found**: [count]
- [Symbol1]: [type] - [usage count] references
- [Symbol2]: [type] - [usage count] references

⚠️ **Type Issues**: [count]
- [File:Line]: [Error description]
- [File:Line]: [Warning description]

💡 **Recommendations**:
- [Actionable suggestion 1]
- [Actionable suggestion 2]

✅ **Status**: [PASS | NEEDS_ATTENTION | FAIL]
```

## Integration Points

- **Code Review**: Automated pre-review analysis
- **Bug Analysis**: Type-related issue investigation
- **Refactoring**: Impact assessment before changes
- **Documentation**: Symbol usage for API docs

## Success Criteria

- Zero type errors in reviewed code
- All symbols properly typed
- Clear usage patterns identified
- Actionable recommendations provided

## Related Documents

- [INS-002] Frontend instructions
- [GL-012] Frontend quality standards
- [KB-050] TypeScript MCP integration guide