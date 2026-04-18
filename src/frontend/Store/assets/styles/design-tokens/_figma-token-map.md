# Figma to CSS Token Mapping

This document maps Figma style names to CSS custom properties for consistent design implementation.

## Color Tokens

| Figma Style Name | CSS Variable | Hex Value |
|------------------|--------------|-----------|
| **Brand** |||
| Color / Brand / Primary / 50 | `--color-brand-primary-50` | #FEF2F2 |
| Color / Brand / Primary / 100 | `--color-brand-primary-100` | #FEE2E2 |
| Color / Brand / Primary / 500 | `--color-brand-primary-500` | #C00018 |
| Color / Brand / Primary / 600 | `--color-brand-primary-600` | #9A0013 |
| Color / Brand / Primary / 700 | `--color-brand-primary-700` | #7F0010 |
| Color / Brand / Accent / 500 | `--color-brand-accent-500` | #1D4E89 |
| **Neutral** |||
| Color / Neutral / 0 | `--color-neutral-0` | #FFFFFF |
| Color / Neutral / 50 | `--color-neutral-50` | #F7F7F8 |
| Color / Neutral / 100 | `--color-neutral-100` | #ECEDEF |
| Color / Neutral / 300 | `--color-neutral-300` | #C5C7CC |
| Color / Neutral / 600 | `--color-neutral-600` | #4A4D55 |
| Color / Neutral / 900 | `--color-neutral-900` | #111318 |
| **Feedback** |||
| Color / Success / 500 | `--color-success-500` | #1B8733 |
| Color / Warning / 500 | `--color-warning-500` | #D97706 |
| Color / Danger / 500 | `--color-danger-500` | #DC2626 |
| Color / Info / 500 | `--color-info-500` | #1F6FB2 |
| **Surfaces** |||
| Color / Surface / Default | `--color-surface-default` | #FFFFFF |
| Color / Surface / Muted | `--color-surface-muted` | #F7F7F8 |
| Color / Surface / Inverse | `--color-surface-inverse` | #1F2128 |

## Typography Tokens

| Figma Style Name | CSS Variables | Values |
|------------------|---------------|--------|
| Text / Display / L | `--font-size-display-l` | 48px, semibold |
| Text / Heading / H1 | `--font-size-heading-h1` | 32px, semibold |
| Text / Heading / H2 | `--font-size-heading-h2` | 24px, semibold |
| Text / Heading / H3 | `--font-size-heading-h3` | 20px, medium |
| Text / Body / M | `--font-size-body-m` | 16px, regular |
| Text / Body / S | `--font-size-body-s` | 14px, regular |
| Text / Label / M | `--font-size-label-m` | 14px, medium |
| Text / Caption / XS | `--font-size-caption-xs` | 12px, regular |

## Spacing Tokens

| Figma Token | CSS Variable | Value |
|-------------|--------------|-------|
| Space / 4 | `--space-1` | 4px |
| Space / 8 | `--space-2` | 8px |
| Space / 12 | `--space-3` | 12px |
| Space / 16 | `--space-4` | 16px |
| Space / 24 | `--space-6` | 24px |
| Space / 32 | `--space-8` | 32px |
| Space / 40 | `--space-10` | 40px |
| Space / 64 | `--space-16` | 64px |

## Radius Tokens

| Figma Token | CSS Variable | Value |
|-------------|--------------|-------|
| Radius / XS | `--radius-xs` | 2px |
| Radius / S | `--radius-s` | 4px |
| Radius / M | `--radius-m` | 6px |
| Radius / L | `--radius-l` | 8px |
| Radius / Full | `--radius-full` | 9999px |

## Shadow Tokens

| Figma Style Name | CSS Variable |
|------------------|--------------|
| Shadow / Card | `--shadow-card` |
| Shadow / Card Hover | `--shadow-card-hover` |
| Shadow / Dropdown | `--shadow-dropdown` |
| Shadow / Modal | `--shadow-modal` |

## Component Usage Examples

### Button Primary
```css
.btn-primary {
  height: var(--btn-height-md);
  padding: 0 var(--btn-padding-x-md);
  background: var(--btn-primary-bg);
  color: var(--btn-primary-text);
  border-radius: var(--btn-radius);
  font-size: var(--btn-font-size);
  font-weight: var(--btn-font-weight);
  transition: var(--transition-colors);
}
.btn-primary:hover {
  background: var(--btn-primary-bg-hover);
}
```

### Product Card
```css
.product-card {
  background: var(--card-bg);
  border: var(--border-width-thin) solid var(--card-border);
  border-radius: var(--card-radius);
  padding: var(--card-padding);
  box-shadow: var(--card-shadow);
  transition: var(--transition-shadow);
}
.product-card:hover {
  box-shadow: var(--card-shadow-hover);
}
```

### Price Display
```css
.price-current {
  color: var(--price-current-color);
  font-size: var(--price-current-size);
  font-weight: var(--price-current-weight);
}
.price-old {
  color: var(--price-old-color);
  font-size: var(--price-old-size);
  text-decoration: line-through;
}
.price-discount {
  color: var(--price-discount-color);
  font-weight: var(--font-weight-semibold);
}
```

### Search Bar
```css
.search-bar {
  height: var(--search-height);
  max-width: var(--search-max-width);
  background: var(--search-bg);
  border: var(--border-width-thin) solid var(--search-border);
  border-radius: var(--search-radius);
  padding: 0 var(--space-4);
  transition: var(--transition-all);
}
.search-bar:focus-within {
  background: var(--search-bg-focus);
  border-color: var(--search-border-focus);
}
```

---

## Figma Setup Instructions

### 1. Create Color Styles
In Figma, create local styles following the naming convention:
- `Color / Brand / Primary / 500`
- `Color / Neutral / 600`
- etc.

### 2. Create Text Styles
Create text styles with:
- `Text / Heading / H1` → Inter, 32px, Semibold
- `Text / Body / M` → Inter, 16px, Regular
- etc.

### 3. Create Effect Styles
Create shadow effects:
- `Shadow / Card` → 0 1px 3px rgba(0,0,0,0.1)
- `Shadow / Modal` → 0 20px 25px rgba(0,0,0,0.1)

### 4. Document Grid Systems
Add layout grids to your frames:
- Desktop: 12 columns, 24px gutter, 80px margin
- Tablet: 8 columns, 24px gutter, 40px margin
- Mobile: 4 columns, 16px gutter, 16px margin

---

*Last updated: 2026-01-11*
