# 🎨 Theme System - Visual Guide

## 📍 Wo findet man das Theme Toggle?

```
┌────────────────────────────────────────────────────────────┐
│                      Admin Frontend                         │
├─────────────────┬──────────────────────────────────────────┤
│                 │  ☰  Dashboard                            │
│   B2 Connect    │                                          │
│     Admin       │                                          │
│                 │                                          │
│ ─────────────── │                                          │
│                 │                                          │
│ Dashboard       │         MAIN CONTENT                    │
│ CMS             │                                          │
│ Shop            │                                          │
│ Jobs            │                                          │
│                 │                                          │
│ ─────────────── │                                          │
│                 │                                          │
│ ☀️  Dark Mode   │  ← THEME TOGGLE (Settings Section)     │
│ [Light | Dark]  │                                          │
│                 │                                          │
└─────────────────┴──────────────────────────────────────────┘
```

## 🎛️ Theme Toggle Varianten

### 1. Icon Only (Default)

```
☀️  (Click to toggle between Light/Dark)
```

### 2. With Label

```
☀️  Light    (Shows current theme mode)
```

### 3. With Menu

```
☀️  [Light | Dark | Auto]  (Choose specific mode)
```

### 4. Combined

```
☀️  Light [Light | Dark | Auto]  (Label + Menu)
```

## 🌓 Visual Theme Comparison

### Light Mode (Default)

```
┌────────────────────────────────────────┐
│ Light Background (#f8f9fa)             │
│ Dark Text (#495057)                    │
│ Light Borders (#e9ecef)                │
│ ☀️ Sun Icon (shows Dark mode available)│
└────────────────────────────────────────┘
```

### Dark Mode

```
┌────────────────────────────────────────┐
│ Dark Background (#1a1a1a)              │
│ Light Text (#e4e4e7)                   │
│ Dark Borders (#404040)                 │
│ 🌙 Moon Icon (shows Light mode available)
└────────────────────────────────────────┘
```

## 🔄 Theme Selection Flow

```
User Clicks Theme Toggle
         ↓
Is Dark mode active?
    ↙       ↘
  Yes        No
   ↓         ↓
Switch    Switch
 to        to
Light     Dark
   ↓         ↓
Update    Update
 State     State
   ↓         ↓
Save to   Save to
LocalStore LocalStore
   ↓         ↓
Apply DOM Apply DOM
Changes   Changes
   ↓         ↓
Update     Update
UI with    UI with
Transition Transition
```

## 💾 Data Flow

```
┌──────────────────────────────────┐
│     useThemeStore (Pinia)        │
├──────────────────────────────────┤
│ State:                           │
│ - theme: 'light'|'dark'|'auto'   │
│ - effectiveTheme: 'light'|'dark' │
├──────────────────────────────────┤
│ Methods:                         │
│ - setTheme()                     │
│ - toggleTheme()                  │
│ - initializeTheme()              │
└──────────────────────────────────┘
         ↓        ↓
    ┌────┴─────────┴────┐
    ↓                   ↓
localStorage         DOM (.dark class)
   theme: 'dark'    <html class="dark">
                           ↓
                      Tailwind CSS
                   dark: modifiers active
                           ↓
                      UI aktualisiert
```

## 🎨 Color Changes bei Theme Wechsel

| Element    | Light Mode           | Dark Mode                  |
| ---------- | -------------------- | -------------------------- |
| Background | `#f8f9fa` (soft-50)  | `#1a1a1a` (soft-900)       |
| Text       | `#495057` (soft-700) | `#e4e4e7` (white/soft-100) |
| Sidebar    | `#ffffff` (white)    | `#2a2a2a` (soft-800)       |
| Borders    | `#e9ecef` (soft-100) | `#404040` (soft-700)       |
| Hover BG   | `#f0f2f5` (soft-100) | `#3a3a3a` (soft-700)       |
| Primary    | `#0284c7` (blue-600) | `#3b82f6` (blue-500)       |

## 📱 Responsive Behavior

### Mobile (< 768px)

```
Sidebar im Overlay
Theme Toggle Icon Only
```

### Desktop (≥ 768px)

```
Sidebar Sticky
Theme Toggle Icon oder Icon + Menu
```

## 🔌 Integrationsbeispiel

### In einer Vue Komponente

```vue
<template>
  <div
    :class="{
      'light-mode': themeStore.effectiveTheme === 'light',
      'dark-mode': themeStore.effectiveTheme === 'dark',
    }"
  >
    <!-- Content -->
  </div>
</template>

<script setup>
import { useThemeStore } from '@/stores/theme';
const themeStore = useThemeStore();
</script>
```

### In einem Store

```typescript
import { useThemeStore } from '@/stores/theme';

// In einer anderen Pinia Action
const themeStore = useThemeStore();

if (themeStore.effectiveTheme === 'dark') {
  // Dark mode specific logic
}
```

## ⚙️ Auto Mode Erklärt

```
User wählt "Auto" Mode
         ↓
System Preference wird gelesen
         ↓
  ╔═══════════════════════╗
  ║ prefers-color-scheme? ║
  ╚═╤═════════════════════╝
    ├─ dark → effectiveTheme = 'dark'
    └─ light → effectiveTheme = 'light'
         ↓
  System Preference ändert sich?
  ↓ Ja: Update effectiveTheme
  ↓ Nein: Alles bleibt gleich
```

## 🎬 User Journey

### Scenario: Benutzer wechselt zu Dark Mode

1. **Start**: Light Mode aktiv (Standard)
   - `theme = 'auto'`
   - `effectiveTheme = 'light'` (von System)

2. **Benutzer klickt Theme Toggle**
   - Im Menü wählt "Dark"

3. **Store Update**

   ```
   setTheme('dark')
   → theme = 'dark'
   → effectiveTheme = 'dark'
   → localStorage.setItem('theme', 'dark')
   ```

4. **DOM Update**

   ```
   applyTheme()
   → document.documentElement.classList.add('dark')
   → <html class="dark">
   ```

5. **Tailwind Aktivierung**

   ```
   dark: prefixes werden aktiv
   Alle dark: modifizierer werden angewendet
   ```

6. **Visual Update**

   ```
   Alle Farben ändern sich mit 300ms transition
   User sieht sanften Übergang
   ```

7. **Persistierung**
   ```
   Beim Browser Refresh:
   → localStorage wird gelesen
   → 'dark' wird geladen
   → Theme wird sofort gesetzt
   → Kein Flashing (Light→Dark)
   ```

## 🧩 Integration in bestehende Komponenten

### Vorher

```vue
<div class="bg-white text-soft-900 border border-soft-100">
  Content
</div>
```

### Nachher (Dark Mode ready)

```vue
<div
  class="bg-white dark:bg-soft-800 text-soft-900 dark:text-white border border-soft-100 dark:border-soft-700 transition-colors duration-300"
>
  Content
</div>
```

## 📚 Files at a Glance

```
frontend-admin/
├── src/
│   ├── stores/
│   │   └── theme.ts              ← Theme Logic
│   ├── components/
│   │   └── common/
│   │       └── ThemeToggle.vue    ← Toggle Component
│   │       └── MainLayout.vue     ← Dark Mode Integrated
│   ├── App.vue                    ← Theme Init
│   └── main.css                   ← Dark Mode Styles
├── THEME_IMPLEMENTATION.md        ← Developer Docs
├── THEME_SETUP_COMPLETE.md       ← Summary
└── README.md                      ← Updated
```

---

**Status**: ✅ Fully Implemented and Ready for Use
