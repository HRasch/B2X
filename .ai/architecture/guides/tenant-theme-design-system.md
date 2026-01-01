# Tenant Theme Design System

**Date:** January 1, 2026  
**Owner:** @UI  
**Status:** Draft  
**Framework:** DaisyUI v5 with CSS Custom Properties  

## Overview

This design system defines the visual foundation for B2Connect's tenant-themed frontend. It builds on DaisyUI components while enabling runtime customization through CSS custom properties for tenant-specific theming.

## CSS Custom Properties (Design Tokens)

### Color Palette

The design system uses CSS custom properties for all color values, allowing runtime customization without SCSS recompilation.

```css
:root {
  /* Primary Colors */
  --b2c-primary: #0b98ff;
  --b2c-primary-focus: #0891ea;
  --b2c-primary-content: #ffffff;

  /* Secondary Colors */
  --b2c-secondary: #8b5cf6;
  --b2c-secondary-focus: #7c3aed;
  --b2c-secondary-content: #ffffff;

  /* Neutral Colors */
  --b2c-neutral: #374151;
  --b2c-neutral-focus: #1f2937;
  --b2c-neutral-content: #ffffff;

  /* Base Colors */
  --b2c-base-100: #ffffff;
  --b2c-base-200: #f9fafb;
  --b2c-base-300: #f3f4f6;
  --b2c-base-content: #111827;

  /* State Colors */
  --b2c-success: #22c55e;
  --b2c-warning: #f59e0b;
  --b2c-error: #ef4444;
  --b2c-info: #3b82f6;
}
```

### Typography

```css
:root {
  /* Font Families */
  --b2c-font-family-sans: 'Inter', system-ui, sans-serif;
  --b2c-font-family-mono: 'JetBrains Mono', monospace;

  /* Font Sizes */
  --b2c-font-size-xs: 0.75rem;
  --b2c-font-size-sm: 0.875rem;
  --b2c-font-size-base: 1rem;
  --b2c-font-size-lg: 1.125rem;
  --b2c-font-size-xl: 1.25rem;
  --b2c-font-size-2xl: 1.5rem;
  --b2c-font-size-3xl: 1.875rem;

  /* Font Weights */
  --b2c-font-weight-light: 300;
  --b2c-font-weight-normal: 400;
  --b2c-font-weight-medium: 500;
  --b2c-font-weight-semibold: 600;
  --b2c-font-weight-bold: 700;
}
```

### Spacing & Layout

```css
:root {
  /* Spacing Scale */
  --b2c-space-1: 0.25rem;
  --b2c-space-2: 0.5rem;
  --b2c-space-3: 0.75rem;
  --b2c-space-4: 1rem;
  --b2c-space-5: 1.25rem;
  --b2c-space-6: 1.5rem;
  --b2c-space-8: 2rem;
  --b2c-space-10: 2.5rem;
  --b2c-space-12: 3rem;

  /* Border Radius */
  --b2c-radius-sm: 0.125rem;
  --b2c-radius-md: 0.375rem;
  --b2c-radius-lg: 0.5rem;
  --b2c-radius-xl: 0.75rem;
  --b2c-radius-2xl: 1rem;

  /* Shadows */
  --b2c-shadow-sm: 0 1px 2px 0 rgb(0 0 0 / 0.05);
  --b2c-shadow-md: 0 4px 6px -1px rgb(0 0 0 / 0.1);
  --b2c-shadow-lg: 0 10px 15px -3px rgb(0 0 0 / 0.1);
}
```

## DaisyUI Integration

DaisyUI components are styled using these custom properties. For tenant theming, override the CSS custom properties at runtime.

### Example: Button Component

```vue
<template>
  <button class="btn btn-primary">
    <slot />
  </button>
</template>

<style scoped>
/* DaisyUI will use the custom properties */
.btn-primary {
  background-color: var(--b2c-primary);
  color: var(--b2c-primary-content);
}
.btn-primary:hover {
  background-color: var(--b2c-primary-focus);
}
</style>
```

## Runtime Customization

Tenants can customize themes by updating CSS custom properties dynamically:

```javascript
// Example: Apply tenant theme
const theme = {
  primary: '#ff6b35',
  secondary: '#f7931e',
  neutral: '#2d3748'
};

Object.entries(theme).forEach(([key, value]) => {
  document.documentElement.style.setProperty(`--b2c-${key}`, value);
});
```

## Design System Guidelines

### Color Usage
- Use semantic color variables (--b2c-primary, --b2c-secondary) instead of hardcoded colors
- Ensure sufficient contrast ratios for accessibility (WCAG 2.1 AA)
- Provide focus states for interactive elements

### Typography
- Use consistent font families defined in custom properties
- Maintain readable font sizes and line heights
- Support responsive typography scaling

### Components
- All components should use DaisyUI classes where possible
- Custom styling should leverage CSS custom properties
- Maintain component consistency across Store, Admin, and Management frontends

## Implementation Notes

- CSS custom properties enable fast runtime theming without SCSS compilation
- For complex theming (gradients, advanced selectors), fall back to SCSS compilation
- Validate theme configurations to prevent invalid CSS
- Cache theme applications for performance
- Ensure tenant isolation in theme storage and application

## Maintenance

- Update this document when adding new design tokens
- Test theme changes across all frontend applications
- Maintain backward compatibility with existing themes
- Review design system annually for consistency