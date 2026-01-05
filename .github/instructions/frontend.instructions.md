---
applyTo: "src/components/**,src/pages/**,src/hooks/**,src/ui/**,**/frontend/**"
---

# Frontend Development Instructions

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

## Testing
- Write unit tests for utility functions
- Write component tests for user interactions
- Test accessibility with automated tools

## Multilingual Support (i18n)
- **Always consider multilingualism** in all frontend development
- **Never use hardcoded strings** in components - always use translation keys
- **Keep translations current** - update all language files when adding new UI text
- **Use vue-i18n properly**:
  - Import `useI18n` composable for script usage
  - Use `$t()` in templates for translation calls
  - Follow consistent key naming: `feature.section.key`
  - Add translation keys to ALL supported languages simultaneously
- **Supported languages**: English (en), German (de), French (fr), Spanish (es), Italian (it), Portuguese (pt), Dutch (nl), Polish (pl)
- **Test translations**: Verify all languages display correctly before committing
- **Backend integration**: Ensure localization API calls work for dynamic content

