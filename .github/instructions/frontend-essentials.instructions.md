---
applyTo: "src/components/**,src/pages/**,src/hooks/**,src/ui/**,**/frontend/**"
---

# Frontend Development Instructions (Essentials)

## Component Design
- Use functional components with hooks
- Keep components small and focused (single responsibility)
- Extract reusable logic into custom hooks
- Use proper TypeScript types for props

## State Management
- Use local state for component-specific data
- Use context/global state sparingly
- Avoid prop drilling beyond 2 levels
- Implement proper loading and error states

## Styling
- Follow project's styling conventions
- Use consistent spacing and sizing
- Ensure responsive design (mobile-first)
- Maintain accessibility standards (WCAG)

## Performance
- Implement lazy loading for routes/components
- Memoize expensive computations
- Optimize re-renders with React.memo/useMemo
- Use proper image optimization

## UX
- Provide immediate feedback on user actions
- Handle loading states gracefully
- Display meaningful error messages
- Implement proper form validation

## i18n (MANDATORY)
- **ZERO hardcoded strings** - all text must use translation keys
- Use `$t()` in templates or `useI18n()` in scripts
- Key pattern: `module.section.key` (e.g., `auth.login.title`)
- English (`en`) is source of truth
- Supported languages: en, de, fr, es, it, pt, nl, pl

## MCP Tools

For detailed frontend-specific MCP tools documentation:

```
kb-mcp/search_knowledge_base
  query: "TypeScript MCP" OR "Vue MCP" OR "Accessibility"
```

See [KB-053] TypeScript MCP, [KB-054] Vue MCP, [KB-056] HTML/CSS MCP

---

**Full documentation**: Use `kb-mcp/get_article` or search Knowledge Base  
**Size**: 1.1 KB (optimized from 5+ KB)
