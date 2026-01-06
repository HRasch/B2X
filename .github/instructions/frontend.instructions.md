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
- **E2E Testing Considerations**:
  - Use `data-testid` attributes for reliable element selection in e2e tests
  - Ensure components render consistently for visual regression testing
  - Test loading states and error conditions that affect user experience
  - Validate responsive behavior across breakpoints

## Frontend Placement Decision Tree

**BEFORE creating ANY component, answer:**
```
├─ Customer-facing feature?
│   └─ YES → frontend/Store/ (Port 5173, Gateway 8000)
│
├─ Admin/internal operations?
│   └─ YES → frontend/Admin/ (Port 5174, Gateway 8080)
│
└─ Tenant configuration/customization?
    └─ YES → frontend/Management/ (Port 5175, Gateway 8090)
```

**Boundary Examples:**
| Feature | Correct Frontend | Why |
|---------|------------------|-----|
| Product listing | Store | Customer browses products |
| Order management | Admin | Staff processes orders |
| Tenant branding | Management | Tenant customizes their store |
| Shopping cart | Store | Customer checkout flow |
| User permissions | Admin | Staff role management |
| Email templates | Management | Tenant configures notifications |

**If unsure:** Ask @SARAH for frontend placement decision.

## Multilingual Support (i18n)

**Reference**: See [GL-042] Token-Optimized i18n Strategy for AI-efficient translation workflows.

### Pre-Flight Checklist (REQUIRED)
Before implementing ANY user-facing text:
- [ ] **Key defined** in `locales/en.json` (English = source of truth)
- [ ] **$t() call used** - NEVER hardcoded strings
- [ ] **Namespace follows** pattern: `module.section.key`
- [ ] **Key documented** if complex interpolation

### i18n Rules
- **Never use hardcoded strings** - always use translation keys
- **English first**: Define keys in `en.json` as source of truth
- **Batch translations**: Request translations for multiple keys in single AI requests
- **Use vue-i18n properly**:
  - Import `useI18n` composable for script usage
  - Use `$t()` in templates for translation calls
  - Follow key naming: `namespace.section.key` (e.g., `auth.login.title`)
- **Supported languages**: en, de, fr, es, it, pt, nl, pl
- **Token efficiency**: Don't load all language files - work with keys only
- **Validation**: Use `scripts/i18n-check.sh` for completeness checks

### Common Mistakes to Avoid
```vue
<!-- ❌ WRONG: Hardcoded text -->
<button>Submit Order</button>

<!-- ✅ CORRECT: Translation key -->
<button>{{ $t('checkout.submit_order') }}</button>
```

```typescript
// ❌ WRONG: String in validation
return 'Email is required'

// ✅ CORRECT: Translation key
return t('validation.email_required')
```

