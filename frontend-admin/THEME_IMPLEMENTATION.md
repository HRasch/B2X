# Light/Dark Theme Implementation - Admin Frontend

## 🎨 Übersicht

Die Admin-Frontend hat nun ein vollständiges Light/Dark Theme System mit den folgenden Features:

- ✅ **Persistent Theme Storage**: Theme-Auswahl wird in localStorage gespeichert
- ✅ **System Preference Detection**: Automatische Erkennung der Systemeinstellung (Auto-Mode)
- ✅ **Three Theme Options**: Light, Dark, Auto (System)
- ✅ **Smooth Transitions**: Fließende Übergänge bei Theme-Wechsel
- ✅ **Complete Styling**: Alle Komponenten sind für Dark Mode optimiert
- ✅ **Responsive Design**: Funktioniert auf allen Bildschirmgrößen

## 📁 Neue/Geänderte Dateien

### Neue Dateien
1. **`src/stores/theme.ts`** - Pinia Store für Theme-Management
2. **`src/components/common/ThemeToggle.vue`** - Wiederverwendbare Theme-Toggle Komponente

### Geänderte Dateien
1. **`src/App.vue`** - Theme-Initialisierung hinzugefügt
2. **`src/components/common/MainLayout.vue`** - Dark Mode Styling und Theme Toggle Integration
3. **`src/main.css`** - Dark Mode CSS Variablen und Styles

## 🚀 Verwendung

### Auto-Initialisierung
Das Theme wird automatisch beim App-Start initialisiert:

```typescript
// In App.vue onMounted
themeStore.initializeTheme()
```

### Theme-Toggle in Komponenten

#### Option 1: Einfacher Toggle Button
```vue
<ThemeToggle />
```

#### Option 2: Mit Label
```vue
<ThemeToggle show-label />
```

#### Option 3: Mit Menü (Light/Dark/Auto)
```vue
<ThemeToggle show-menu />
```

#### Option 4: Kombiniert
```vue
<ThemeToggle show-label show-menu />
```

### Programmatischer Zugriff

```typescript
import { useThemeStore } from '@/stores/theme'

const themeStore = useThemeStore()

// Toggle zwischen Light und Dark
themeStore.toggleTheme()

// Spezifisches Theme setzen
themeStore.setTheme('light')    // Light Mode
themeStore.setTheme('dark')     // Dark Mode
themeStore.setTheme('auto')     // Systemeinstellung folgen

// Aktuelles Theme auslesen
console.log(themeStore.theme)           // 'light' | 'dark' | 'auto'
console.log(themeStore.effectiveTheme)  // 'light' | 'dark' (tatsächlich angewandt)
```

## 🎯 Theme Store API

### State
- `theme`: Gespeicherte Theme-Auswahl ('light' | 'dark' | 'auto')
- `effectiveTheme`: Tatsächlich angewandtes Theme ('light' | 'dark')

### Methods
- `initializeTheme()`: Initialisiert Theme beim App-Start
- `toggleTheme()`: Wechselt zwischen Light und Dark
- `setTheme(newTheme)`: Setzt spezifisches Theme
- `updateEffectiveTheme()`: Aktualisiert effektives Theme basierend auf Systemeinstellung
- `applyTheme()`: Wendet Theme auf DOM an (setzt/entfernt `dark` Klasse)

## 🎨 Dark Mode CSS

### Tailwind Dark Mode
- Nutzt Tailwind's `dark:` Modifier
- Wird aktiviert mit `class` darkMode in tailwind.config.js
- DOM Class: `dark` wird auf `<html>` Element gesetzt/entfernt

### CSS Variablen für Dark Mode
```css
:root {
  --soft-shadow-xs: 0 2px 4px rgba(52, 71, 103, 0.1);
  --soft-shadow-sm: 0 4px 6px rgba(52, 71, 103, 0.1);
  /* ... etc */
}

html.dark :root {
  --soft-shadow-xs: 0 2px 4px rgba(0, 0, 0, 0.3);
  --soft-shadow-sm: 0 4px 6px rgba(0, 0, 0, 0.3);
  /* ... etc */
}
```

## 📱 Responsive Dark Mode

Die Theme-Toggle Komponente ist vollständig responsiv:
- Mobile: Icon-only Button mit optional Label
- Desktop: Icon-only oder mit Menü für Light/Dark/Auto Optionen

## 🔄 System Preference Detection

Wenn `theme === 'auto'`:
1. Liest Systemeinstellung via `window.matchMedia('(prefers-color-scheme: dark)')`
2. Setzt `effectiveTheme` basierend auf Systemeinstellung
3. Lauscht auf System Theme-Änderungen
4. Aktualisiert UI automatisch bei Systemeinstellung-Änderung

## 💾 Persistierung

- Theme-Auswahl wird in localStorage unter `'theme'` gespeichert
- Wird beim App-Start automatisch wiederhergestellt
- Works offline

## ✨ Styling Best Practices

### In Vue Komponenten

```vue
<!-- Tailwind Dark Mode -->
<div class="bg-white dark:bg-soft-800 text-soft-900 dark:text-white">
  Content
</div>

<!-- Mit Transition -->
<div class="transition-colors duration-300 bg-soft-50 dark:bg-soft-900">
  Content
</div>
```

### In CSS

```css
/* Light Mode (Default) */
.my-element {
  background: white;
  color: #333;
}

/* Dark Mode */
html.dark .my-element {
  background: #1a1a1a;
  color: #e4e4e7;
}

/* Mit Transition */
.my-element {
  transition: background-color 0.3s, color 0.3s;
}
```

## 🧪 Testing

```typescript
// In komponenten Tests
import { useThemeStore } from '@/stores/theme'

const themeStore = useThemeStore()

// Test toggle
themeStore.toggleTheme()
expect(themeStore.effectiveTheme).toBe('dark')

// Test setzen
themeStore.setTheme('light')
expect(document.documentElement.classList.contains('dark')).toBe(false)

// Test localStorage
themeStore.setTheme('dark')
expect(localStorage.getItem('theme')).toBe('dark')
```

## 🐛 Troubleshooting

### Theme wird nicht gespeichert
- Prüfe ob localStorage aktiviert ist
- Überprüfe Browser-Konsole auf Fehler

### Dark Mode wird nicht angewendet
- Überprüfe ob `html` Element die `dark` Klasse hat
- Stelle sicher dass Tailwind `darkMode: "class"` in config hat
- Clear Browser Cache

### System Theme wird nicht erkannt
- Überprüfe System Preference (macOS: System Preferences > General > Appearance)
- Browser muss `prefers-color-scheme` Media Query unterstützen

## 📚 Weitere Ressourcen

- [Tailwind Dark Mode](https://tailwindcss.com/docs/dark-mode)
- [CSS Media Queries](https://developer.mozilla.org/en-US/docs/Web/CSS/Media_Queries)
- [localStorage API](https://developer.mozilla.org/en-US/docs/Web/API/Window/localStorage)
