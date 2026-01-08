---
docid: KB-186
title: Typescript Mcp Integration
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

---
docid: KB-053
title: TypeScript MCP Integration Guide
owner: @CopilotExpert
status: Active
---

# TypeScript MCP Integration Guide

**DocID**: `KB-053`
**Last Updated**: 6. Januar 2026
**Owner**: @CopilotExpert

## Overview

The TypeScript MCP server provides AI-assisted code analysis tools for TypeScript/Vue.js development, complementing the existing RoslynMCP for C#.

## MCP Server Configuration

**Location**: `tools/TypeScriptMCP/`
**MCP Config**: `.vscode/mcp.json` (already configured)

```json
{
  "mcpServers": {
    "typescript-mcp": {
      "command": "node",
      "args": ["tools/TypeScriptMCP/dist/index.js"],
      "env": { "NODE_ENV": "production" },
      "disabled": false
    }
  }
}
```

## Available Tools

### 1. search_symbols
**Purpose**: Find TypeScript symbols using patterns
**Use Cases**:
- Component discovery: `pattern="*Component"`
- Interface finding: `pattern="*Interface"`
- Service location: `pattern="*Service"`

**Example**:
```typescript
// Find all Vue components
pattern: "*Component"
workspacePath: "frontend/Store"

// Find interfaces
pattern: "*VM"
workspacePath: "frontend/Store/src"
```

### 2. get_symbol_info
**Purpose**: Detailed symbol analysis
**Use Cases**:
- Type definition inspection
- Property/method analysis
- Documentation extraction

**Example**:
```typescript
symbolName: "UserProfile"
workspacePath: "frontend/Store"
filePath: "src/types/user.ts"
```

### 3. find_usages
**Purpose**: Cross-file usage tracking
**Use Cases**:
- Refactoring impact assessment
- Dependency analysis
- Dead code detection

**Example**:
```typescript
symbolName: "ProductCard"
workspacePath: "frontend/Store"
filePath: "src/components/ProductCard.vue"
```

### 4. analyze_types
**Purpose**: TypeScript compilation and error analysis
**Use Cases**:
- Pre-commit validation
- Type safety verification
- Error detection

**Example**:
```typescript
workspacePath: "frontend/Store"
filePath: "src/components/NewComponent.vue"
```

## Agent Integration

### @Frontend Agent
**Tools**: `typescript-mcp/*`
**Usage Patterns**:
- Symbol search during development
- Type analysis in code reviews
- Usage tracking for refactoring

### @TechLead Agent
**Enhanced Code Review**:
- Automated type checking
- Symbol usage validation
- Import analysis

## Workflow Integration

### Development Workflow
1. **Symbol Discovery**: Use `search_symbols` to find existing components
2. **Type Analysis**: Run `analyze_types` on new/modified files
3. **Usage Tracking**: Check `find_usages` before refactoring
4. **Code Review**: Automated analysis with `get_symbol_info`

### Code Review Process
```bash
# TypeScript review command
@TechLead: /typescript-review
Component: frontend
Scope: src/components/UserProfile.vue
Focus: types
```

## Best Practices

### Performance Optimization
- **Selective Analysis**: Target specific files rather than entire workspace
- **Cached Results**: Leverage MCP server caching for repeated queries
- **Incremental Checks**: Use file-specific analysis for faster feedback

### Error Handling
- **Path Validation**: Ensure workspace paths are correct
- **Symbol Existence**: Verify symbols exist before usage analysis
- **Type Errors**: Address compilation errors before advanced analysis

### Integration Patterns
- **Pre-commit Hooks**: Run type analysis before commits
- **CI/CD Integration**: Include MCP checks in build pipeline
- **IDE Integration**: Leverage VS Code MCP extension capabilities

## Vue/Nuxt Testing Integration

### Composable Mocking Strategies

**Critical for Vue Testing**: Proper mocking of Nuxt auto-imported composables is essential for reliable component testing.

#### useCookie Mocking (Vitest)

**Issue**: `useCookie` returns plain objects instead of Vue refs, causing JSON parsing errors.

**Solution**: Use `vi.stubGlobal()` for proper Vue reactivity:

```typescript
// tests/setup.ts - CORRECT approach
import { vi } from 'vitest';
import { ref } from 'vue';

// Proper composable mocking
vi.stubGlobal('useCookie', vi.fn(() => ref(null)));
vi.stubGlobal('useHead', vi.fn());
vi.stubGlobal('navigateTo', vi.fn());

// Auth store compatibility
vi.stubGlobal('useCookie', vi.fn((name: string) => {
  if (name === 'auth') {
    // Return proper ref for auth cookie
    return ref(process.env.NODE_ENV === 'test' ? null : undefined);
  }
  return ref(null);
}));
```

**Benefits**:
- ✅ **Vue Reactivity**: Returns proper `ref()` objects
- ✅ **Type Safety**: Maintains TypeScript compatibility  
- ✅ **Test Reliability**: Eliminates JSON parsing errors
- ✅ **SSR Compatible**: Works in both client and server contexts

#### Common Composable Mock Patterns

```typescript
// tests/setup.ts
vi.stubGlobal('useAsyncData', vi.fn(() => ({
  data: ref(null),
  pending: ref(false),
  error: ref(null),
  refresh: vi.fn()
})));

vi.stubGlobal('useFetch', vi.fn(() => ({
  data: ref(null),
  pending: ref(false),
  error: ref(null)
})));

vi.stubGlobal('$fetch', vi.fn());
```

#### Test Environment Detection

**Dual Persistence Strategy**: Support both production (cookies) and test (localStorage) environments:

```typescript
// src/stores/auth.ts
const useAuthPersistence = () => {
  if (process.env.NODE_ENV === 'test') {
    return {
      get: () => localStorage.getItem('auth'),
      set: (value: string) => localStorage.setItem('auth', value),
      remove: () => localStorage.removeItem('auth'),
    };
  } else {
    const authCookie = useCookie('auth', { 
      default: () => null,
      encode: JSON.stringify,
      decode: JSON.parse,
    });
    return {
      get: () => authCookie.value,
      set: (value: any) => authCookie.value = value,
      remove: () => authCookie.value = null,
    };
  }
};
```

### Component Testing with i18n

**Issue**: Internationalized components fail without proper i18n setup.

**Solution**: Configure Vue i18n plugin in test mounts:

```typescript
// tests/components/Testimonials.spec.ts
import { createI18n } from 'vue-i18n';

const i18n = createI18n({
  legacy: false,
  locale: 'en',
  messages: {
    en: {
      testimonials: {
        title: 'Customer Reviews',
        subtitle: 'What our customers say'
      }
    }
  }
});

const wrapper = mount(Testimonials, {
  global: {
    plugins: [i18n], // Required for $t() usage
  }
});
```

**Policy**: Zero hardcoded strings - all text must use translation keys.

### ESLint Integration for Test Files

**Issue**: ESLint flags unused parameters in mock functions.

**Solution**: Use targeted disable comments:

```typescript
// eslint-disable-next-line @typescript-eslint/no-unused-vars
vi.mock('#app', () => ({
  useCookie: vi.fn((name) => ref(null)), // 'name' intentionally unused
}));
```

### Quality Check Pipeline Integration

**Coverage Configuration**: Ensure vitest finds test files in quality checks:

```typescript
// vitest.config.ts
export default defineConfig({
  test: {
    coverage: {
      include: ['**/*.{test,spec}.{js,mjs,cjs,ts,mts,cts,jsx,tsx}'],
      exclude: ['node_modules/**', '.nuxt/**', 'dist/**'],
    },
  },
});
```

## Troubleshooting

### Common Issues

**MCP Server Not Responding**
```bash
# Check server status
cd tools/TypeScriptMCP
npm run build
npm start
```

**Invalid Workspace Path**
- Ensure paths are relative to project root
- Use forward slashes (/) consistently
- Avoid `..` in paths

**Symbol Not Found**
- Check spelling and case sensitivity
- Verify file is included in tsconfig.json
- Ensure symbol is exported

### Debug Commands
```bash
# Test MCP connection
node tools/TypeScriptMCP/dist/index.js

# Validate TypeScript config
npx tsc --noEmit --project frontend/Store/tsconfig.json
```

## Success Metrics

- **Adoption Rate**: >50% of frontend tasks use MCP tools
- **Error Reduction**: 30% fewer type-related bugs
- **Review Time**: 25% faster code reviews
- **Developer Satisfaction**: Positive feedback on tool usefulness

## Related Documentation

- [PRM-016] TypeScript Review Prompt
- [INS-002] Frontend Instructions
- [GL-012] Frontend Quality Standards
- [KB-052] Roslyn MCP Integration (C# equivalent)

## Future Enhancements

- **Vue SFC Support**: Enhanced analysis for .vue files
- **Performance Metrics**: Response time optimization
- **Custom Rules**: Project-specific type checking rules
- **Integration Testing**: MCP tool validation in CI/CD

---

**Status**: ✅ Active and integrated
**Next Review**: March 2026