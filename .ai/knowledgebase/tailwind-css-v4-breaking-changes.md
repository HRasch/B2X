# Tailwind CSS v4 Breaking Changes

## Overview
Tailwind CSS v4.0 introduces significant breaking changes from v3.x, particularly around configuration, CSS modules, and Vue/Svelte component styling.

## Key Breaking Changes

### @apply in Vue/Svelte Components Requires @reference
**Issue**: In Tailwind CSS v4, `@apply` directives in Vue `<style scoped>` blocks or CSS modules fail with "unknown utility class" errors.

**Solution**: Add `@reference` directive at the top of `<style>` blocks to import theme variables and utilities.

**Examples**:
```vue
<style scoped>
@reference "../../app.css"; /* For custom theme */
@reference "tailwindcss";   /* For default theme only */

.my-class {
  @apply text-xl font-bold bg-primary/20;
}
</style>
```

**Affected Classes in B2Connect**:
- `bg-primary/20` - opacity modifiers
- `rounded-box` - DaisyUI utilities
- `ring-blue-500` - focus ring colors

### Import Syntax Changes
**Before (v3)**:
```css
@tailwind base;
@tailwind components;
@tailwind utilities;
```

**After (v4)**:
```css
@import "tailwindcss";
```

### Configuration Migration
- JavaScript config files → CSS-based configuration with `@theme` directive
- `tailwind.config.js` → CSS variables in main stylesheet
- PostCSS plugin → `@tailwindcss/postcss` package

### Deprecated Utilities Removed
| Removed | Replacement |
|---------|-------------|
| `bg-opacity-*` | `bg-black/50` (opacity modifiers) |
| `text-opacity-*` | `text-black/50` |
| `ring-opacity-*` | `ring-black/50` |
| `flex-shrink-*` | `shrink-*` |
| `flex-grow-*` | `grow-*` |

### Renamed Utilities
| v3 | v4 |
|----|----|
| `shadow-sm` | `shadow-xs` |
| `shadow` | `shadow-sm` |
| `ring` | `ring-3` |
| `outline-none` | `outline-hidden` |

### Browser Requirements
- **Minimum**: Safari 16.4, Chrome 111, Firefox 128
- **Note**: Older browsers require staying on v3.4

## Migration Steps for B2Connect

### 1. Update Vue Components
Add `@reference` to all `<style scoped>` blocks using `@apply`:

```vue
<style scoped>
@reference "../../src/main.css";

.my-component {
  @apply rounded-box bg-primary/20 ring-blue-500;
}
</style>
```

### 2. Update Main CSS File
```css
/* Before */
@tailwind base;
@tailwind components;
@tailwind utilities;

/* After */
@import "tailwindcss";
@import "daisyui/daisyui.css";
```

### 3. Update Package Dependencies
```json
{
  "devDependencies": {
    "tailwindcss": "^4.1.18",
    "@tailwindcss/postcss": "^4.1.18"
  }
}
```

### 4. Update Build Configuration
**Vite**: Use `@tailwindcss/vite` plugin
**PostCSS**: Use `@tailwindcss/postcss` plugin

## Common Issues and Solutions

### Issue: "Cannot apply unknown utility class"
**Cause**: Missing `@reference` in component styles
**Solution**: Add `@reference "tailwindcss";` or `@reference "../../app.css";`

### Issue: DaisyUI classes not working
**Cause**: DaisyUI v5 requires proper import order
**Solution**:
```css
@import "tailwindcss";
@import "daisyui/daisyui.css";
```

### Issue: Opacity modifiers not working
**Cause**: `bg-primary/20` syntax changed in v4
**Solution**: Ensure proper theme variable definitions in `@theme` block

## Version Compatibility
- **Tailwind CSS v4.1.18**: Current version in B2Connect
- **Breaking changes**: Major version upgrade required
- **Migration tool**: `npx @tailwindcss/upgrade` (automates most changes)

## References
- [Tailwind CSS v4 Upgrade Guide](https://tailwindcss.com/docs/upgrade-guide)
- [@reference Directive Documentation](https://tailwindcss.com/docs/functions-and-directives#reference-directive)
- [Tailwind CSS v4.0 Release Notes](https://github.com/tailwindlabs/tailwindcss/releases/tag/v4.0.0)

## Last Updated
2026-01-01 - Initial documentation based on B2Connect migration issues