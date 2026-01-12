# 🎨 Theme System - Quick Reference

## 🚀 One-Liner Setup

```typescript
// In App.vue onMounted hook
themeStore.initializeTheme();
```

## 📊 Theme Store API

### State

```typescript
const themeStore = useThemeStore();

themeStore.theme; // 'light' | 'dark' | 'auto'
themeStore.effectiveTheme; // 'light' | 'dark' (actual)
```

### Methods

```typescript
// Toggle between Light and Dark
themeStore.toggleTheme();

// Set specific theme
themeStore.setTheme('light'); // Light mode
themeStore.setTheme('dark'); // Dark mode
themeStore.setTheme('auto'); // Follow system

// Initialize (called in App.vue)
themeStore.initializeTheme();
```

## 🎨 Using in Vue Components

### Template

```vue
<!-- Check current theme -->
<div v-if="themeStore.effectiveTheme === 'dark'">
  Dark mode is on
</div>

<!-- Conditional classes -->
<div :class="{ dark: themeStore.effectiveTheme === 'dark' }">
  Content
</div>
```

### Script

```typescript
import { useThemeStore } from '@/stores/theme';

const themeStore = useThemeStore();
console.log(themeStore.effectiveTheme);
```

## 🎯 Component Integration

### ThemeToggle Component

```vue
<!-- Icon only -->
<ThemeToggle />

<!-- With label -->
<ThemeToggle show-label />

<!-- With menu (Light/Dark/Auto) -->
<ThemeToggle show-menu />

<!-- Everything -->
<ThemeToggle show-label show-menu />
```

## 🎨 Tailwind Dark Mode Syntax

### Classes

```vue
<!-- Base + Dark -->
<div class="bg-white dark:bg-soft-800">

<!-- Text -->
<p class="text-soft-900 dark:text-white">

<!-- Borders -->
<div class="border border-soft-100 dark:border-soft-700">

<!-- Hover -->
<button class="hover:bg-soft-100 dark:hover:bg-soft-700">

<!-- With transition -->
<div class="transition-colors duration-300 bg-white dark:bg-soft-800">
```

## 💾 localStorage Keys

```javascript
localStorage.getItem('theme'); // 'light' | 'dark' | 'auto'
localStorage.setItem('theme', 'dark');
```

## 📝 Common Patterns

### Check if Dark Mode

```typescript
if (themeStore.effectiveTheme === 'dark') {
  // Dark mode logic
}
```

### React to Theme Changes

```typescript
import { watch } from 'vue';

watch(
  () => themeStore.effectiveTheme,
  newTheme => {
    console.log(`Theme changed to ${newTheme}`);
  }
);
```

### In Computed Property

```typescript
const isDark = computed(() => themeStore.effectiveTheme === 'dark');
```

### In CSS-in-JS

```typescript
const bgColor = computed(() => {
  return themeStore.effectiveTheme === 'dark' ? '#1a1a1a' : '#f8f9fa';
});
```

## 🎯 Common Tailwind Patterns

### Backgrounds

```
Light: bg-white, bg-soft-50
Dark: dark:bg-soft-800, dark:bg-soft-900
```

### Text

```
Light: text-soft-700, text-soft-900
Dark: dark:text-soft-300, dark:text-white
```

### Borders

```
Light: border-soft-100, border-soft-200
Dark: dark:border-soft-700, dark:border-soft-600
```

### Shadows

```
Light: shadow-sm, shadow-md
Dark: dark:shadow-soft-sm (CSS var adjusted)
```

## 🔍 Debugging

### Check Current Theme

```typescript
console.log(useThemeStore().theme); // User selected
console.log(useThemeStore().effectiveTheme); // Actually applied
```

### Check DOM

```javascript
document.documentElement.classList.contains('dark'); // true/false
```

### Check Storage

```javascript
console.log(localStorage.getItem('theme'));
```

### Check System Preference

```javascript
window.matchMedia('(prefers-color-scheme: dark)').matches;
```

## 🎭 CSS Variable Syntax

### In CSS

```css
/* Light mode (default) */
:root {
  --soft-shadow-sm: 0 4px 6px rgba(52, 71, 103, 0.1);
}

/* Dark mode */
html.dark {
  --soft-shadow-sm: 0 4px 6px rgba(0, 0, 0, 0.3);
}
```

### Usage

```css
.element {
  box-shadow: var(--soft-shadow-sm);
  transition: all 0.3s;
}
```

## 🚀 Performance Tips

1. ✅ Wrap localStorage access in try-catch
2. ✅ Check `isBrowser` before DOM access
3. ✅ Use CSS transitions instead of JS animations
4. ✅ Cache `useThemeStore()` reference
5. ✅ Use computed properties for derived state

## 🧪 Testing Template

```typescript
import { useThemeStore } from '@/stores/theme';

describe('Theme Store', () => {
  it('toggles theme', () => {
    const store = useThemeStore();
    store.toggleTheme();
    expect(store.effectiveTheme).toBe('dark');
  });

  it('saves to localStorage', () => {
    const store = useThemeStore();
    store.setTheme('dark');
    expect(localStorage.getItem('theme')).toBe('dark');
  });

  it('applies dark class to HTML', () => {
    const store = useThemeStore();
    store.setTheme('dark');
    expect(document.documentElement.classList.contains('dark')).toBe(true);
  });
});
```

## 🎨 Color Reference

| Type               | Light                | Dark                 |
| ------------------ | -------------------- | -------------------- |
| **Background**     | `#f8f9fa` (soft-50)  | `#1a1a1a` (soft-900) |
| **Surface**        | `#ffffff` (white)    | `#2a2a2a` (soft-800) |
| **Text Primary**   | `#212529` (soft-900) | `#ffffff` (white)    |
| **Text Secondary** | `#6c757d` (soft-600) | `#a1a1a1` (soft-400) |
| **Border**         | `#e9ecef` (soft-100) | `#3a3a3a` (soft-700) |
| **Primary**        | `#0284c7` (blue-600) | `#3b82f6` (blue-500) |

## 📖 Documentation Links

- Full Docs: [THEME_IMPLEMENTATION.md](THEME_IMPLEMENTATION.md)
- Visual Guide: [THEME_VISUAL_GUIDE.md](THEME_VISUAL_GUIDE.md)
- Setup Summary: [THEME_SETUP_COMPLETE.md](THEME_SETUP_COMPLETE.md)

---

**TL;DR**: Import store, call `initializeTheme()`, use `dark:` in Tailwind. Done! 🎉
