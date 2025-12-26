# 🎨 Soft UI Dashboard - Admin Frontend Design System

## Übersicht

Das Admin-Frontend wurde mit dem **Soft UI Dashboard** Design System von Creative Tim neu gestaltet. Es bietet ein modernes, professionelles und visuell ansprechendes Interface mit sanften Schatten, weichen Übergängen und einer konsistenten Farbpalette.

## 🎯 Hauptmerkmale

✅ **Modernes Design** - Soft Shadows & Soft Corners
✅ **Responsive Layout** - Funktioniert auf allen Geräten
✅ **Dark Mode Support** - Toggle im User Menu
✅ **Smooth Animations** - Sanfte Übergänge
✅ **Accessible Components** - WCAG konforme Komponenten
✅ **Tailwind CSS** - Vollständig mit Tailwind konfiguriert
✅ **Vue 3 Kompatibel** - Moderne Vue 3 Syntax

---

## 🎨 Design System

### Farbpalette

#### Primary Colors (Corporate Blue)
- **Primary 50-900**: Von hellblau (#f0f9ff) bis dunkelblau (#0c2d57)
- Verwendet für: Hauptaktionen, Links, Fokus-Status

#### Success Colors (Grün)
- Verwendet für: Erfolgs-Status, positive Aktionen, Bestätigungen

#### Warning Colors (Gelb)
- Verwendet für: Warnungen, Hinweise, Informationen

#### Danger Colors (Rot)
- Verwendet für: Fehler, Löschen, kritische Aktionen

#### Info Colors (Indigo)
- Verwendet für: Informationen, sekundäre Aktionen

#### Soft Colors
- **Soft 50-900**: Grauskala für Text, Hintergründe, Borders
- Neutral für: Texte, Hintergründe, Divider

### Shadow System

```css
/* Soft Shadows - 3 Größen */
--soft-shadow-sm: 0 4px 6px rgba(52, 71, 103, 0.1);
--soft-shadow-md: 0 6px 12px rgba(52, 71, 103, 0.1);
--soft-shadow-lg: 0 8px 16px rgba(52, 71, 103, 0.12);
```

**Verwendung:**
- `shadow-soft-sm`: Kleine Elemente (Cards in Listen)
- `shadow-soft-md`: Standard Cards, Panels
- `shadow-soft-lg`: Hover-States, Modals

### Border Radius

- `rounded-soft` (0.75rem): Standard für Input, Buttons
- `rounded-soft-lg` (1rem): Größere Elemente
- `rounded-soft-xl` (1.5rem): Cards, Panels
- `rounded-soft-2xl` (2rem): Große Container

### Spacing

- `safe` (1.5rem): Standard Padding/Margin
- `safe-lg` (2.5rem): Größere Abstände

---

## 📦 Komponenten

### 1. **SoftCard** - Basis Container

```vue
<template>
  <soft-card variant="default">
    <!-- Content -->
  </soft-card>
</template>

<script setup>
import SoftCard from '@/components/soft-ui/SoftCard.vue'
</script>
```

**Props:**
- `variant`: 'default' | 'gradient' (optional)
- `className`: Custom CSS Classes (optional)

**Beispiel:**
```vue
<!-- Standard Card -->
<soft-card>
  <h3>My Content</h3>
</soft-card>

<!-- Mit Gradient Hintergrund -->
<soft-card variant="gradient">
  <h3>Highlighted Content</h3>
</soft-card>
```

---

### 2. **SoftButton** - Interaktive Buttons

```vue
<soft-button 
  variant="primary" 
  size="md" 
  :loading="isLoading"
  @click="handleClick"
>
  Click Me
</soft-button>
```

**Props:**
- `variant`: 'primary' | 'secondary' | 'danger' | 'success' | 'ghost'
- `size`: 'sm' | 'md' | 'lg'
- `disabled`: Boolean
- `loading`: Boolean (zeigt Spinner)

**Varianten:**

```vue
<!-- Primary Button -->
<soft-button variant="primary">Save</soft-button>

<!-- Secondary Button -->
<soft-button variant="secondary">Cancel</soft-button>

<!-- Danger Button -->
<soft-button variant="danger">Delete</soft-button>

<!-- Success Button -->
<soft-button variant="success">Confirm</soft-button>

<!-- Ghost Button -->
<soft-button variant="ghost">More Options</soft-button>

<!-- Loading State -->
<soft-button :loading="true">Processing...</soft-button>

<!-- Disabled State -->
<soft-button disabled>Unavailable</soft-button>
```

---

### 3. **SoftBadge** - Status Badges

```vue
<soft-badge variant="success">Active</soft-badge>
<soft-badge variant="warning">Pending</soft-badge>
<soft-badge variant="danger">Failed</soft-badge>
```

**Props:**
- `variant`: 'success' | 'warning' | 'danger' | 'info' | 'default'

---

### 4. **SoftPanel** - Erweiterte Container

```vue
<soft-panel title="Users" description="Active users list">
  <!-- Header Slot (optional) -->
  <template #header>
    <!-- Custom Header Content -->
  </template>

  <!-- Main Content -->
  <div>Content goes here</div>

  <!-- Footer Slot (optional) -->
  <template #footer>
    <!-- Footer Actions -->
  </template>
</soft-panel>
```

**Props:**
- `title`: String (optional)
- `description`: String (optional)

**Slots:**
- `header`: Custom Header Content
- `default`: Main Content
- `footer`: Footer Actions

---

### 5. **SoftInput** - Form Input

```vue
<soft-input 
  v-model="email"
  type="email"
  label="Email Address"
  placeholder="your@email.com"
  :error="emailError"
  required
/>
```

**Props:**
- `modelValue`: String | Number (required)
- `type`: String (default: 'text')
- `label`: String (optional)
- `placeholder`: String
- `error`: String (error message)
- `required`: Boolean
- `disabled`: Boolean

**Emits:**
- `update:modelValue`: Aktualisiert v-model

---

## 🎨 Layout System

### MainLayout - Neue Navigation

Die neue `MainLayout.vue` bietet:

#### Features:
- **Sidebar Navigation**: Links mit aktiven States
- **Top Bar**: Mit Benachrichtigungen und User Menu
- **Mobile Responsive**: Collapsible Sidebar
- **Dark Mode Toggle**: Im User Menu
- **User Dropdown**: Profile, Settings, Logout
- **Notifications**: Mit roten Badge

#### Navigation Items:
```vue
const navItems = [
  { path: '/dashboard', label: 'Dashboard' },
  { path: '/cms/pages', label: 'CMS' },
  { path: '/shop/products', label: 'Shop' },
  { path: '/jobs/queue', label: 'Jobs' },
]
```

---

## 📐 Grid & Layout Utilities

### Responsive Grids

```vue
<!-- 1 Column (Mobile), 2 Columns (Desktop) -->
<div class="grid-soft-cols-2">
  <soft-card>Card 1</soft-card>
  <soft-card>Card 2</soft-card>
</div>

<!-- 3 Columns auf großen Screens -->
<div class="grid-soft-cols-3">
  <soft-card>Card 1</soft-card>
  <soft-card>Card 2</soft-card>
  <soft-card>Card 3</soft-card>
</div>

<!-- 4 Columns auf großen Screens -->
<div class="grid-soft-cols-4">
  <soft-card>Card 1</soft-card>
  <soft-card>Card 2</soft-card>
  <soft-card>Card 3</soft-card>
  <soft-card>Card 4</soft-card>
</div>
```

---

## 🎬 Animations

### Verfügbare Animationen

```css
/* CSS Klassen für Animations */
.fade-in          /* Sanftes Einblenden */
.slide-in-up      /* Von unten hochfahren */
.slide-in-down    /* Von oben hinunterfahren */
.slide-in-left    /* Von links hereinfahren */
.slide-in-right   /* Von rechts hereinfahren */
```

### Verwendung

```vue
<div class="fade-in">
  <soft-card>Animierte Card</soft-card>
</div>

<transition 
  enter-active-class="slide-in-up"
  leave-active-class="fade-out"
>
  <soft-panel v-if="isVisible">
    Content
  </soft-panel>
</transition>
```

---

## 📊 Dashboard Beispiel

Die `DashboardView.vue` zeigt die Best Practices:

### Struktur:
1. **Header** - Seiten-Überschrift & Beschreibung
2. **Stats Grid** - 4-Column Statistiken Cards
3. **Content Grid** - 2-Column Layout mit Charts & Activity
4. **Data Table** - Benutzer-Tabelle mit Status Badges
5. **Action Buttons** - Primary, Secondary, Ghost Buttons

### Komponenten im Einsatz:
```vue
<!-- Stats Cards -->
<soft-card variant="gradient">
  <h3>{{ stat.label }}</h3>
  <p>{{ stat.value }}</p>
</soft-card>

<!-- Activity List -->
<soft-panel title="Recent Activity">
  <soft-badge variant="success">{{ activity.status }}</soft-badge>
</soft-panel>

<!-- Data Table -->
<table>
  <tr>
    <td>
      <soft-badge :variant="user.status">
        {{ user.statusLabel }}
      </soft-badge>
    </td>
  </tr>
</table>

<!-- Actions -->
<soft-button variant="primary">Create New</soft-button>
```

---

## 🎯 Best Practices

### 1. Konsistente Spacing
```vue
<!-- Verwende safe/safe-lg statt magischen Nummern -->
<div class="p-safe">                <!-- 1.5rem -->
  <div class="space-y-safe-lg">     <!-- 2.5rem gaps -->
    <!-- Content -->
  </div>
</div>
```

### 2. Farbpalette nutzen
```vue
<!-- Primary Actions -->
<soft-button variant="primary">Save</soft-button>

<!-- Destructive Actions -->
<soft-button variant="danger">Delete</soft-button>

<!-- Success Feedback -->
<soft-badge variant="success">Completed</soft-badge>
```

### 3. Komponenten kombinieren
```vue
<soft-panel title="User Management">
  <div class="grid-soft-cols-2">
    <soft-card>
      <soft-input v-model="name" label="Name" />
    </soft-card>
    <soft-card>
      <soft-input v-model="email" label="Email" />
    </soft-card>
  </div>

  <template #footer>
    <soft-button variant="primary">Save</soft-button>
  </template>
</soft-panel>
```

---

## 🚀 Migration Guide

### Alte Struktur → Neue Struktur

```vue
<!-- VORHER -->
<div class="bg-white shadow rounded-lg p-4">
  <button class="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700">
    Click
  </button>
</div>

<!-- NACHHER -->
<soft-card>
  <soft-button variant="primary">Click</soft-button>
</soft-card>
```

---

## 📝 Tailwind Config

### Neue Custom Colors

```js
// tailwind.config.js
colors: {
  primary: { 50-900 },      // Brand Blue
  success: { 50-900 },      // Success Green
  warning: { 50-900 },      // Warning Yellow
  danger: { 50-900 },       // Danger Red
  info: { 50-900 },         // Info Indigo
  soft: { 50-900 },         // Neutral Gray
}

boxShadow: {
  'soft-xs': '...',
  'soft-sm': '...',
  'soft-md': '...',
  'soft-lg': '...',
}
```

---

## ✨ Weitere Features

### Dark Mode
- Toggle im User Menu
- Automatische Anpassung aller Komponenten
- CSS Custom Properties für einfache Anpassung

### Responsive Design
- Mobile First Approach
- Breakpoints: sm, md, lg, xl, 2xl
- Sidebar collapsible auf Mobile

### Accessibility
- Keyboard Navigation
- Focus States auf allen interaktiven Elementen
- ARIA Labels wo nötig
- Semantisches HTML

---

## 📚 Verwendete Ressourcen

- **Design System**: [Soft UI Dashboard by Creative Tim](https://www.creative-tim.com/product/soft-ui-dashboard-tailwind)
- **CSS Framework**: Tailwind CSS v3+
- **Vue Version**: Vue 3 mit Composition API
- **Icons**: SVG Icons (können durch Hero Icons erweitert werden)

---

## 🔄 Weitere Anpassungen

Um das Design noch weiter zu personalisieren:

1. **Custom Colors** → `tailwind.config.js` erweitern
2. **Neue Icons** → `hero-icons` oder `lucide-vue-next` installieren
3. **Animationen** → Keyframes in `main.css` hinzufügen
4. **Komponenten** → In `src/components/soft-ui/` neue Komponenten erstellen

---

## 📞 Support

Bei Fragen oder Erweiterungswünschen:
- Siehe Creative Tim Dokumentation
- Tailwind CSS Docs
- Vue 3 Composition API Guide
