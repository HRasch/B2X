# Tailwind CSS Design System Planning

**Date:** December 30, 2025  
**Owner:** @Frontend  
**Phase 1 Task:** Tailwind Planning (2 SP)  
**Status:** Complete  

---

## Design System Overview

Tailwind CSS design system for B2X Store frontend. Built on modern utility-first principles with comprehensive theming.

---

## Color Palette Definition

### Primary Colors
```
Primary Blue (actions, CTAs):
  - primary-50:  #f0f9ff  (lightest)
  - primary-100: #e0f2fe
  - primary-200: #bae6fd
  - primary-400: #60a5fa  (light usage)
  - primary-500: #3b82f6  (main brand - use for buttons, links)
  - primary-600: #2563eb  (hover state)
  - primary-700: #1d4ed8  (active state)
  - primary-900: #1e3a8a  (darkest, high contrast)
```

### Secondary Colors
```
Success (confirmations):
  - success-500: #10b981  (green, positive actions)
  - success-600: #059669  (hover)

Warning (alerts):
  - warning-500: #f59e0b  (amber, caution)
  - warning-600: #d97706  (hover)

Error (destructive):
  - error-500:   #ef4444  (red, errors, delete)
  - error-600:   #dc2626  (hover)

Neutral (UI):
  - neutral-50:  #f9fafb  (lightest background)
  - neutral-100: #f3f4f6
  - neutral-200: #e5e7eb  (borders, dividers)
  - neutral-500: #6b7280  (secondary text)
  - neutral-700: #374151  (main text)
  - neutral-900: #111827  (darkest text)
```

### Semantic Color Mapping

| Usage | Tailwind Color | Application |
|-------|----------------|-------------|
| Primary buttons | `bg-primary-500` | CTAs, actions |
| Hover state | `bg-primary-600` | Interactive feedback |
| Active state | `bg-primary-700` | Selected/active items |
| Links | `text-primary-600` | Hyperlinks |
| Link hover | `text-primary-700` | Link interaction |
| Success message | `text-success-600` | Confirmations |
| Warning message | `text-warning-600` | Alerts, cautions |
| Error message | `text-error-600` | Errors, validation |
| Main text | `text-neutral-900` | Body text, headers |
| Secondary text | `text-neutral-600` | Descriptions, hints |
| Borders | `border-neutral-200` | All borders |
| Backgrounds | `bg-neutral-50` | Page backgrounds |

---

## Typography Scale

### Font Families
```
Primary (Headers, Branding):
  - font-family: 'Segoe UI', Tahoma, Geneva, sans-serif
  - Weight: 600-700 (bold)

Body (Content):
  - font-family: 'Segoe UI', Tahoma, Geneva, sans-serif
  - Weight: 400 (normal)

Monospace (Code):
  - font-family: 'Courier New', monospace
  - Weight: 400
```

### Size Scale

| Scale | Size | Usage |
|-------|------|-------|
| `text-xs` | 12px | Labels, captions, helper text |
| `text-sm` | 14px | Secondary text, small content |
| `text-base` | 16px | Body text (default) |
| `text-lg` | 18px | Larger content, emphasis |
| `text-xl` | 20px | Section headers |
| `text-2xl` | 24px | Page headers |
| `text-3xl` | 30px | Hero headers |
| `text-4xl` | 36px | Large titles |

### Weight Scale

| Weight | Value | Usage |
|--------|-------|-------|
| Light | 300 | Deemphasis |
| Normal | 400 | Body text |
| Medium | 500 | Emphasis, labels |
| Semibold | 600 | Headers, buttons |
| Bold | 700 | Strong emphasis |

---

## Spacing Scale (Matches Tailwind default)

```
0 = 0px          sm = 0.5rem (8px)
1 = 0.25rem (4px)   md = 1rem (16px)
2 = 0.5rem (8px)    lg = 1.5rem (24px)
3 = 0.75rem (12px)  xl = 2rem (32px)
4 = 1rem (16px)     2xl = 3rem (48px)
6 = 1.5rem (24px)   3xl = 4rem (64px)
8 = 2rem (32px)     4xl = 6rem (96px)
```

### Common Combinations
- Small gap: `gap-2` (8px)
- Medium gap: `gap-4` (16px)
- Large gap: `gap-6` (24px)
- Section separation: `mb-8` (32px)

---

## Responsive Breakpoints

| Breakpoint | Width | Usage |
|-----------|-------|-------|
| (mobile-first default) | < 640px | Mobile phones |
| `sm:` | ≥ 640px | Small tablets |
| `md:` | ≥ 768px | Tablets, small laptops |
| `lg:` | ≥ 1024px | Laptops, desktops |
| `xl:` | ≥ 1280px | Large desktops |
| `2xl:` | ≥ 1536px | Ultra-wide |

### Mobile-First Strategy
1. Design for mobile first (no prefix)
2. Add `sm:` for tablet improvements
3. Add `md:` for desktop layout
4. Add `lg:` for wide features
5. Add `xl:` for premium spacing

### Example Structure
```vue
<!-- Mobile: full width, stacked -->
<div class="w-full">
  <!-- sm: side by side -->
  <div class="sm:flex sm:gap-4">
    <!-- md: larger spacing, better layout -->
    <div class="md:w-1/2 md:gap-6">
```

---

## Component Patterns

### Buttons

**Primary Button (CTA)**
```vue
<button class="
  px-4 py-2
  bg-primary-500 text-white
  font-semibold
  rounded-lg
  hover:bg-primary-600
  active:bg-primary-700
  focus:outline-none focus:ring-2 focus:ring-primary-400
  transition-colors duration-200
">
  Click me
</button>
```

**Secondary Button**
```vue
<button class="
  px-4 py-2
  border-2 border-primary-500 text-primary-600
  font-semibold
  rounded-lg
  hover:bg-primary-50
  focus:outline-none focus:ring-2 focus:ring-primary-400
  transition-colors duration-200
">
  Secondary
</button>
```

**Disabled Button**
```vue
<button class="
  px-4 py-2
  bg-neutral-300 text-neutral-500
  font-semibold
  rounded-lg
  cursor-not-allowed
  opacity-50
"
disabled>
  Disabled
</button>
```

### Form Inputs

**Text Input**
```vue
<input class="
  w-full
  px-4 py-2
  border border-neutral-200
  rounded-lg
  focus:outline-none
  focus:ring-2 focus:ring-primary-500
  focus:border-transparent
  transition-all duration-200
  placeholder-neutral-500
"
placeholder="Enter text...">
```

**Input with Label**
```vue
<label class="block">
  <span class="text-sm font-medium text-neutral-700 mb-2 block">
    Label
  </span>
  <input class="
    w-full px-4 py-2
    border border-neutral-200
    rounded-lg
    focus:ring-2 focus:ring-primary-500
  ">
</label>
```

### Cards

**Basic Card**
```vue
<div class="
  bg-white
  rounded-lg
  border border-neutral-200
  shadow-sm
  p-6
">
  <!-- Card content -->
</div>
```

**Elevated Card (hover effect)**
```vue
<div class="
  bg-white
  rounded-lg
  shadow-sm
  hover:shadow-md
  p-6
  transition-shadow duration-200
">
  <!-- Card content -->
</div>
```

---

## Layout Components

### Container (max-width wrapper)
```vue
<div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
  <!-- Content -->
</div>
```

### Grid (responsive)
```vue
<!-- Auto-responsive grid -->
<div class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-6">
  <!-- Items -->
</div>
```

### Flexbox (alignment)
```vue
<!-- Center alignment -->
<div class="flex items-center justify-center">
  <!-- Content -->
</div>

<!-- Space between -->
<div class="flex items-center justify-between">
  <!-- Content -->
</div>
```

---

## Accessibility Considerations

### Focus States
- All interactive elements must have visible focus rings
- Use `focus:outline-none focus:ring-2 focus:ring-offset-2`
- Focus color: primary color for consistency

### Color Contrast
- Text on background: WCAG AA minimum (4.5:1)
- Large text on background: WCAG AA minimum (3:1)
- Verify with: WebAIM Contrast Checker

### Typography
- Line height: 1.5 minimum for body text
- Font size: 14px minimum for body text
- Letter spacing: adequate for readability

### Interactive Elements
- Minimum touch target: 44x44px (mobile)
- Visual feedback: hover, active, focus states
- Keyboard navigation: all elements accessible

---

## Design System Implementation

### Tailwind Configuration File (`tailwind.config.ts`)
```typescript
/** @type {import('tailwindcss').Config} */
export default {
  content: ['./index.html', './src/**/*.{vue,js,ts,jsx,tsx}'],
  theme: {
    extend: {
      colors: {
        primary: {
          50: '#f0f9ff',
          100: '#e0f2fe',
          200: '#bae6fd',
          400: '#60a5fa',
          500: '#3b82f6',
          600: '#2563eb',
          700: '#1d4ed8',
          900: '#1e3a8a',
        },
        success: {
          500: '#10b981',
          600: '#059669',
        },
        warning: {
          500: '#f59e0b',
          600: '#d97706',
        },
        error: {
          500: '#ef4444',
          600: '#dc2626',
        },
        neutral: {
          50: '#f9fafb',
          100: '#f3f4f6',
          200: '#e5e7eb',
          500: '#6b7280',
          600: '#4b5563',
          700: '#374151',
          900: '#111827',
        },
      },
      fontFamily: {
        sans: ['Segoe UI', 'Tahoma', 'Geneva', 'sans-serif'],
        mono: ['Courier New', 'monospace'],
      },
    },
  },
  plugins: [],
}
```

---

## Documentation Links

- Official Tailwind Docs: https://tailwindcss.com/docs
- Color Customization: https://tailwindcss.com/docs/customizing-colors
- Responsive Design: https://tailwindcss.com/docs/responsive-design
- Component Examples: https://tailwindui.com

---

## Next Steps (Task 3 - Design System Setup)

1. Create `tailwind.config.ts` with above configuration
2. Set up Tailwind CSS in build process
3. Create 3-5 base component patterns
4. Document component guidelines
5. Setup component review process

---

## Summary

**Status:** ✅ COMPLETE

Comprehensive Tailwind CSS design system planned. Color palette defined, typography scale established, responsive breakpoints set, component patterns documented, accessibility guidelines specified.

**Ready for:** Implementation (next task)

---

**Planned By:** @Frontend  
**Date:** Dec 30, 2025  
**For Code Review:** @TechLead  
