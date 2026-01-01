# SoftUI Admin Theme

## Übersicht

Das SoftUI Admin Theme ist ein modernes, zugängliches Design-System für die B2Connect Admin-Oberfläche, das auf Tailwind CSS basiert und sanfte, benutzerfreundliche UI-Komponenten bereitstellt.

## Architektur

### Design-Prinzipien
- **Sanfte Ästhetik**: Abgerundete Ecken, weiche Schatten und subtile Farbverläufe
- **Barrierefreiheit**: WCAG 2.1 AA konform mit hohem Kontrast und klarer Typografie
- **Responsive Design**: Mobile-first Ansatz mit flexiblen Layouts
- **Dark Mode Support**: Vollständige Unterstützung für hell/dunkel/auto Themes

### Technische Implementierung

#### Tailwind CSS Konfiguration
```typescript
// tailwind.config.ts
{
  theme: {
    extend: {
      colors: {
        soft: {
          50: "#f8f9fa",   // Hellster Farbton
          100: "#f0f2f5",  // Sehr hell
          200: "#e9ecef",  // Hell
          300: "#dee2e6",  // Mittel-hell
          400: "#ced4da",  // Mittel
          500: "#adb5bd",  // Basis
          600: "#6c757d",  // Dunkel
          700: "#495057",  // Sehr dunkel
          800: "#343a40", // Fast schwarz
          900: "#212529"  // Dunkelster Farbton
        }
      },
      borderRadius: {
        soft: "0.75rem",      // Standard abgerundet
        "soft-lg": "1rem",    // Groß abgerundet
        "soft-xl": "1.5rem",  // Extra groß
        "soft-2xl": "2rem"    // Maximum
      },
      boxShadow: {
        "soft-xs": "0 2px 4px rgba(52, 71, 103, 0.1)",
        "soft-sm": "0 4px 6px rgba(52, 71, 103, 0.1)",
        "soft": "0 1px 3px 0 rgb(0 0 0 / 0.1)",
        "soft-md": "0 6px 12px rgba(52, 71, 103, 0.1)",
        "soft-lg": "0 8px 16px rgba(52, 71, 103, 0.12)",
        "soft-xl": "0 12px 24px rgba(52, 71, 103, 0.15)",
        "soft-2xl": "0 16px 32px rgba(52, 71, 103, 0.18)",
        "soft-inner": "inset 0 2px 4px rgba(255, 255, 255, 0.5)",
        "soft-bottom": "0 8px 16px rgba(52, 71, 103, 0.15), inset 0 -2px 0 rgba(255, 255, 255, 0.5)"
      },
      backgroundImage: {
        "gradient-soft-cyan": "linear-gradient(135deg, #06b6d4 0%, #0891b2 100%)",
        "gradient-soft-blue": "linear-gradient(135deg, #3b82f6 0%, #1d4ed8 100%)",
        "gradient-soft-purple": "linear-gradient(135deg, #8b5cf6 0%, #7c3aed 100%)",
        "gradient-soft-green": "linear-gradient(135deg, #10b981 0%, #059669 100%)",
        "gradient-soft-orange": "linear-gradient(135deg, #f59e0b 0%, #d97706 100%)",
        "gradient-soft-red": "linear-gradient(135deg, #ef4444 0%, #dc2626 100%)"
      }
    }
  }
}
```

## Komponenten

### SoftButton
```vue
<template>
  <button :class="buttonClasses">
    <slot />
  </button>
</template>

<script setup lang="ts">
interface Props {
  variant?: "primary" | "secondary" | "danger" | "success" | "ghost";
  size?: "sm" | "md" | "lg";
  disabled?: boolean;
  loading?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  variant: "primary",
  size: "md",
  disabled: false,
  loading: false
});

const buttonClasses = computed(() => {
  const variants = {
    primary: "bg-gradient-soft-blue text-white shadow-soft-md hover:shadow-soft-lg",
    secondary: "bg-soft-100 text-soft-700 hover:bg-soft-200 shadow-soft-sm",
    danger: "bg-danger-500 text-white shadow-soft-md hover:shadow-soft-lg",
    success: "bg-success-500 text-white shadow-soft-md hover:shadow-soft-lg",
    ghost: "bg-transparent text-soft-700 hover:bg-soft-100"
  };

  const sizes = {
    sm: "px-3 py-1.5 text-xs",
    md: "px-5 py-2.5 text-sm",
    lg: "px-6 py-3 text-base"
  };

  return `inline-flex items-center justify-center px-5 py-2.5 rounded-soft font-medium text-sm transition-all duration-200 focus:outline-none focus:ring-2 focus:ring-offset-2 disabled:opacity-50 disabled:cursor-not-allowed transform hover:scale-105 active:scale-95 ${variants[props.variant]} ${sizes[props.size]}`;
});
</script>
```

### SoftCard
```vue
<template>
  <div :class="cardClasses">
    <slot />
  </div>
</template>

<script setup lang="ts">
interface Props {
  variant?: "default" | "gradient";
  class?: string;
}

const props = withDefaults(defineProps<Props>(), {
  variant: "default",
  class: ""
});

const cardClasses = computed(() => {
  const base = "rounded-soft-lg px-safe py-safe shadow-soft-md transition-all duration-200 bg-white dark:bg-soft-800 hover:shadow-soft-lg border border-soft-100 dark:border-soft-700";

  const variants = {
    default: "",
    gradient: "bg-gradient-to-br from-soft-50 dark:from-soft-700"
  };

  return `${base} ${variants[props.variant]} ${props.class}`;
});
</script>
```

### SoftInput
```vue
<template>
  <div class="space-y-1">
    <label v-if="label" :for="inputId" class="block text-sm font-medium text-soft-700 dark:text-soft-300">
      {{ label }}
    </label>
    <input
      :id="inputId"
      :type="type"
      :value="modelValue"
      :placeholder="placeholder"
      :required="required"
      :disabled="disabled"
      @input="$emit('update:modelValue', $event.target.value)"
      :class="inputClasses"
    />
  </div>
</template>

<script setup lang="ts">
interface Props {
  modelValue: string;
  type?: string;
  label?: string;
  placeholder?: string;
  required?: boolean;
  disabled?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  type: "text",
  required: false,
  disabled: false
});

const emit = defineEmits<{
  "update:modelValue": [value: string]
}>();

const inputId = computed(() => `input-${Math.random().toString(36).substr(2, 9)}`);

const inputClasses = computed(() => {
  return "w-full px-3 py-2 border border-soft-200 rounded-soft shadow-soft-sm focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-primary-500 disabled:opacity-50 disabled:cursor-not-allowed transition-all duration-200 bg-white dark:bg-soft-800 text-soft-900 dark:text-soft-100 placeholder-soft-400";
});
</script>
```

## Theme-System

### Theme Store (Pinia)
```typescript
// stores/theme.ts
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export const useThemeStore = defineStore('theme', () => {
  const theme = ref<'light' | 'dark' | 'auto'>('auto')

  const effectiveTheme = computed(() => {
    if (theme.value === 'auto') {
      return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light'
    }
    return theme.value
  })

  const setTheme = (newTheme: 'light' | 'dark' | 'auto') => {
    theme.value = newTheme
    localStorage.setItem('theme', newTheme)
    updateDocumentClass()
  }

  const updateDocumentClass = () => {
    const root = document.documentElement
    root.classList.remove('light', 'dark')
    root.classList.add(effectiveTheme.value)
  }

  // Initialize theme
  const savedTheme = localStorage.getItem('theme') as 'light' | 'dark' | 'auto' | null
  if (savedTheme) {
    theme.value = savedTheme
  }
  updateDocumentClass()

  return {
    theme,
    effectiveTheme,
    setTheme
  }
})
```

### Theme Toggle Component
```vue
<template>
  <div class="flex items-center gap-1 p-1 bg-soft-100 dark:bg-soft-800 rounded-soft-lg">
    <button
      @click="themeStore.setTheme('light')"
      :class="buttonClasses('light')"
      title="Light Mode"
    >
      ☀️
    </button>
    <button
      @click="themeStore.setTheme('dark')"
      :class="buttonClasses('dark')"
      title="Dark Mode"
    >
      🌙
    </button>
    <button
      @click="themeStore.setTheme('auto')"
      :class="buttonClasses('auto')"
      title="Auto (Follow System)"
    >
      💻
    </button>
  </div>
</template>

<script setup lang="ts">
import { useThemeStore } from "@/stores/theme"
import { computed } from "vue"

const themeStore = useThemeStore()

const buttonClasses = (mode: string) => {
  const base = "px-2 py-1 text-xs rounded transition-colors"
  const isActive = themeStore.theme === mode

  if (isActive) {
    return `${base} bg-primary-100 text-primary-600 dark:bg-primary-900 dark:text-primary-300`
  } else {
    return `${base} text-soft-600 dark:text-soft-400 hover:bg-soft-100 dark:hover:bg-soft-800`
  }
}
</script>
```

## Barrierefreiheit

### WCAG 2.1 AA Konformität
- **Kontrastverhältnis**: Minimum 4.5:1 für normalen Text, 3:1 für großen Text
- **Fokus-Indikatoren**: Klare Fokus-Ringe für alle interaktiven Elemente
- **Tastatur-Navigation**: Vollständige Unterstützung für Tab-Navigation
- **Screen Reader**: Korrekte ARIA-Labels und semantische HTML-Struktur

### Implementierungsdetails
```css
/* Fokus-Indikatoren */
.focus-ring {
  @apply focus:outline-none focus:ring-2 focus:ring-primary-500 focus:ring-offset-2;
}

/* Hoher Kontrast für Dark Mode */
.dark .high-contrast {
  @apply text-white border-white;
}

/* Reduzierte Bewegungen für Benutzer mit Vestibularstörungen */
@media (prefers-reduced-motion: reduce) {
  * {
    animation-duration: 0.01ms !important;
    animation-iteration-count: 1 !important;
    transition-duration: 0.01ms !important;
  }
}
```

## Performance

### CSS Optimierungen
- **Tailwind JIT**: Just-in-Time Kompilierung für minimale CSS-Größe
- **Safelist**: Sicherstellung, dass alle benötigten Klassen generiert werden
- **Purge**: Entfernung ungenutzter CSS im Production-Build

### Bundle-Größe
- **Hauptkomponenten**: ~36KB CSS (gezippt: ~7.8KB)
- **Vue Runtime**: ~102KB JS (gezippt: ~39.7KB)
- **Gesamt**: ~139KB (gezippt: ~47.5KB)

## Verwendung

### Grundlegende Implementierung
```vue
<template>
  <div class="min-h-screen bg-gradient-soft-blue">
    <soft-card class="max-w-md mx-auto mt-8">
      <h1 class="heading-lg text-soft-900 dark:text-white">
        SoftUI Admin Theme
      </h1>

      <soft-input
        v-model="email"
        label="Email"
        type="email"
        placeholder="user@example.com"
      />

      <soft-button variant="primary" class="w-full mt-4">
        Submit
      </soft-button>
    </soft-card>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import SoftCard from '@/components/soft-ui/SoftCard.vue'
import SoftInput from '@/components/soft-ui/SoftInput.vue'
import SoftButton from '@/components/soft-ui/SoftButton.vue'

const email = ref('')
</script>
```

## Troubleshooting

### Häufige Probleme

#### Gradient-Klassen werden nicht angewendet
**Problem**: `bg-gradient-soft-blue` wird nicht in CSS generiert
**Lösung**: Sicherstellen, dass die Klasse in der Tailwind Safelist steht
```typescript
// tailwind.config.ts
safelist: ["bg-gradient-soft-blue", /* ... */]
```

#### Schatten-Klassen fehlen
**Problem**: `shadow-soft-md` wird nicht angewendet
**Lösung**: Box-Shadow-Definitionen in der Tailwind-Konfiguration prüfen
```typescript
// tailwind.config.ts
boxShadow: {
  "soft-md": "0 6px 12px rgba(52, 71, 103, 0.1)",
}
```

#### Dark Mode funktioniert nicht
**Problem**: Dark Mode Klassen werden nicht angewendet
**Lösung**: Sicherstellen, dass `dark:` Klassen in der Safelist stehen
```typescript
safelist: ["dark:bg-soft-800", "dark:text-white", /* ... */]
```

## Erweiterte Features

### Responsive Breakpoints
```css
/* Mobile-first Design */
.soft-container {
  @apply px-safe py-safe;
}

@media (min-width: 640px) {
  .soft-container {
    @apply px-safe-lg py-safe-lg;
  }
}
```

### Animationen
```css
/* Sanfte Übergänge */
.soft-transition {
  @apply transition-all duration-200 ease-out;
}

/* Hover-Effekte */
.soft-hover {
  @apply transform hover:scale-105 active:scale-95;
}
```

## Migration von anderen Themes

### Von Bootstrap zu SoftUI
```typescript
// Vorher (Bootstrap)
<button class="btn btn-primary">Button</button>

// Nachher (SoftUI)
<soft-button variant="primary">Button</soft-button>
```

### Von Material Design zu SoftUI
```typescript
// Vorher (Material)
<div class="mdc-card">Content</div>

// Nachher (SoftUI)
<soft-card>Content</soft-card>
```

## Testing

### Komponenten-Tests
```typescript
// SoftButton.test.ts
import { describe, it, expect } from 'vitest'
import { mount } from '@vue/test-utils'
import SoftButton from './SoftButton.vue'

describe('SoftButton', () => {
  it('renders primary variant correctly', () => {
    const wrapper = mount(SoftButton, {
      props: { variant: 'primary' }
    })
    expect(wrapper.classes()).toContain('bg-gradient-soft-blue')
  })
})
```

### Visuelle Regressionstests
```typescript
// Verwende Playwright für visuelle Tests
test('SoftUI components look correct', async ({ page }) => {
  await page.goto('/components-preview')
  await expect(page).toHaveScreenshot('softui-components.png')
})
```

## Roadmap

### Geplante Features
- **SoftUI Icons**: Dediziertes Icon-System
- **SoftUI Charts**: Diagramm-Komponenten mit SoftUI Styling
- **SoftUI Forms**: Erweiterte Formular-Komponenten
- **SoftUI Tables**: Datentabellen mit SoftUI Design
- **SoftUI Notifications**: Toast-Benachrichtigungen

### Performance-Optimierungen
- **CSS-in-JS**: Optionale CSS-in-JS Implementierung
- **Tree Shaking**: Bessere Bundle-Optimierung
- **Lazy Loading**: Komponenten nach Bedarf laden

---

**Aktualisiert**: 1. Januar 2026
**Version**: 1.0.0
**Status**: ✅ Production Ready