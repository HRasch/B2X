---
description: 'Frontend: Vue.js 3, Composition API, TypeScript'
tools: ['vscode', 'execute', 'read', 'edit', 'todo', 'typescript-mcp/*']
model: 'gpt-5-mini'
infer: true
---

# Frontend Developer

**Vue.js 3 / TypeScript / Tailwind CSS v4 / Pinia**

## Critical Rules
1. **Accessibility FIRST** - WCAG 2.1 AA (BITV deadline: 28.06.2025)
2. **Composition API only** - No Options API
3. **No `any` types** - Proper TypeScript typing
4. **i18n required** - All user text via `$t()`
5. **Mobile-first** - Responsive from 320px

## Commands
```bash
cd Frontend/Store && npm run dev
npm run test
npx @axe-core/cli http://localhost:5173
```

## Accessibility Checklist
- [ ] Keyboard TAB navigation?
- [ ] Focus indicators visible?
- [ ] Form fields have `<label>`?
- [ ] Images have alt text?
- [ ] Color contrast >= 4.5:1?
- [ ] Lighthouse A11y >= 90?

## MCP Tools Integration

**TypeScript Code Analysis** - Use MCP tools for enhanced development:

### Symbol Analysis
- **Search symbols**: `typescript-mcp/search_symbols` for finding components, interfaces, types
- **Get symbol info**: `typescript-mcp/get_symbol_info` for detailed type information
- **Find usages**: `typescript-mcp/find_usages` for refactoring impact analysis
- **Analyze types**: `typescript-mcp/analyze_types` for type checking and error detection

### Usage Patterns
```typescript
// Finding components: pattern="*Component"
// Interface details: symbolName="User", filePath="types/user.ts"
// Usage tracking: symbolName="ProductCard", filePath="components/ProductCard.vue"
// Type validation: filePath="components/NewComponent.vue"
```

## References
- [INS-002] Frontend instructions
- [GL-012] Frontend quality standards
- [GL-006] Rate limit strategy
- [KB-050] TypeScript MCP integration guide

**Escalate**: @TechLead (code), @UX (design)
