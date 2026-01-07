# REQ-007: Email WYSIWYG Builder - Frontend Analysis

**DocID**: `REQ-007-FRONTEND-ANALYSIS`  
**Owner**: @Frontend  
**Created**: 2026-01-07  
**Framework**: PRM-010 v2.0 - Domain-spezifische Analyse für @Frontend  
**Status**: Draft  

## Executive Summary

Die Implementierung eines Email WYSIWYG Builders erfordert signifikante Frontend-Entwicklung mit Fokus auf eine komplexe Drag&Drop-Oberfläche, State-Management für Template-Zustände und responsive Email-Previews. Vue.js 3 bietet eine solide Basis mit Composition API und modernen UI-Libraries, erfordert jedoch neue Komponenten und State-Management-Patterns.

**T-Shirt Size Estimate**: L (Large) - 20-28 Stunden  
**Risiko Level**: Medium-High  
**Komplexität**: Komplexe UI-Interaktionen + Neue Component-Architektur  

---

## UI/UX Implikationen

### Drag&Drop Interface
- **Canvas-basierter Editor**: Hauptbereich mit visueller Email-Vorschau
- **Widget-Sidebar**: Kategorisierte Bibliothek mit Drag-fähigen Widgets
- **Drop-Zonen**: Visuelle Indikatoren für gültige Drop-Positionen
- **Ghost-Elemente**: Transparente Vorschau beim Draggen
- **Multi-Select**: Strg/Cmd-Click für mehrere Widgets

### WYSIWYG Editor Features
- **Inline-Editing**: Doppelklick auf Text-Widgets öffnet Rich-Text-Editor
- **Property-Panel**: Rechtsseitiger Panel für Widget-Eigenschaften
- **Live-Preview**: Sofortige Aktualisierung bei Änderungen
- **Responsive-Toggle**: Desktop/Mobile/Tablet-Ansichten
- **Zoom-Controls**: 25%/50%/75%/100%/150% für Detailarbeit

### User Experience Flow
1. **Template-Auswahl**: Grid-View mit Template-Thumbnails
2. **Builder-Modus**: Vollbild-Editor mit Canvas und Sidebars
3. **Widget-Anpassung**: Property-Panel für detaillierte Konfiguration
4. **Preview-Modus**: Vollbild-Email-Vorschau mit Device-Simulation
5. **Save/Publish**: Modal mit Validierung und Export-Optionen

---

## Component Impact

### Neue Core-Komponenten

#### EmailBuilder.vue (Haupt-Container)
```vue
<template>
  <div class="email-builder">
    <BuilderToolbar />
    <div class="builder-workspace">
      <WidgetSidebar />
      <EmailCanvas />
      <PropertyPanel />
    </div>
    <PreviewModal />
  </div>
</template>
```

#### EmailCanvas.vue (Drag&Drop Canvas)
- **HTML5 Drag&Drop API**: Native Browser-Support mit Fallbacks
- **Canvas-Grid**: Visuelle Hilfslinien für Alignment
- **Widget-Rendering**: Dynamische Komponenten basierend auf Widget-Type
- **Selection-Handling**: Click/Drag-Selection mit Resize-Handles

#### WidgetLibrary.vue (Template-Sidebar)
- **Kategorien**: Header, Content, CTA, Footer, Custom
- **Drag-Preview**: Miniatur-Vorschau beim Draggen
- **Search/Filter**: Schnelle Widget-Findung
- **Custom Widgets**: User-erstellte Widgets speichern

### Erweiterte Bestehende Komponenten

#### EmailTemplateEditor.vue (Integration)
- **Modus-Toggle**: Code-Editor ↔ WYSIWYG-Modus
- **Hybrid-Editing**: Gleichzeitige Anzeige von Visual und Code
- **Template-Import**: Bestehende HTML-Templates parsen

#### MediaLibrary.vue (Erweiterung)
- **Email-Optimized Uploads**: Automatische Komprimierung für Email
- **Alt-Text Generation**: Accessibility-Verbesserungen
- **Bulk-Upload**: Mehrere Bilder gleichzeitig

---

## State Management

### Template State Structure
```typescript
interface EmailTemplateState {
  id: string;
  name: string;
  widgets: WidgetInstance[];
  globalStyles: GlobalEmailStyles;
  metadata: TemplateMetadata;
  undoStack: TemplateSnapshot[];
  redoStack: TemplateSnapshot[];
}

interface WidgetInstance {
  id: string;
  type: WidgetType;
  position: { x: number; y: number };
  size: { width: number; height: number };
  properties: Record<string, any>;
  styles: WidgetStyles;
}
```

### State Management Pattern
- **Pinia Store**: `useEmailBuilderStore()` für Template-State
- **Reactive Widgets**: Vue Composition API für Widget-Updates
- **Undo/Redo System**: Command-Pattern für State-Changes
- **Auto-Save**: Debounced Persistence zu Backend

### Performance Optimizations
- **Virtual Scrolling**: Für große Widget-Libraries
- **Lazy Loading**: Widgets on-demand laden
- **Memoization**: Expensive Computations cachen
- **Web Workers**: HTML-Preview Rendering auslagern

---

## Responsive & Accessibility

### Responsive Design
- **Email-First Approach**: Mobile-First für Email-Clients
- **Device Previews**: iPhone, iPad, Desktop, Outlook-Simulation
- **Breakpoint Management**: Automatische Anpassung bei Canvas-Resize
- **Fluid Layouts**: Prozent-basierte statt Fixed-Widths

### Accessibility (WCAG 2.1 AA)
- **Keyboard Navigation**: Tab-Navigation durch Widgets
- **Screen Reader Support**: ARIA-Labels für alle interaktiven Elemente
- **Color Contrast**: Automatische Validierung für Text/Background
- **Focus Management**: Visuelle Focus-Indikatoren
- **Alternative Text**: Mandatory für alle Images

### Email Client Compatibility
- **HTML Sanitization**: XSS-Prevention bei User-Content
- **CSS Inlining**: Automatische Konvertierung für Email-Clients
- **Fallback Rendering**: Graceful Degradation für ältere Clients
- **Testing Suite**: Automated Checks gegen gängige Email-Clients

---

## Aufwandsschätzung (T-Shirt Sizes)

### Component Development (L)
- EmailCanvas.vue: 8h (Drag&Drop, Rendering, Selection)
- Widget Components (8 Types): 12h (Reusable, Configurable)
- PropertyPanel.vue: 4h (Dynamic Forms, Validation)
- WidgetSidebar.vue: 4h (Library, Search, Drag-Init)

### State Management (M)
- Pinia Store Setup: 4h (Template State, Actions, Getters)
- Undo/Redo System: 6h (Command Pattern, Snapshots)
- Auto-Save Integration: 2h (Debounced API Calls)

### UI/UX Implementation (M)
- Responsive Previews: 6h (Device Simulation, Breakpoints)
- Accessibility Features: 4h (ARIA, Keyboard Nav, Contrast)
- Drag&Drop Polish: 4h (Visual Feedback, Animations)

### Integration & Testing (S)
- Backend API Integration: 4h (Widget CRUD, Template Save)
- Unit Tests: 4h (Component Tests, State Logic)
- E2E Tests: 4h (Drag&Drop Flows, Preview)

**Gesamt**: 28h (Large)  
**Aufgeteilt**: 16h Development + 8h Testing + 4h Integration  

---

## Technische Dependencies

### Neue Libraries Erforderlich
- **@dnd-kit/core**: Modern Drag&Drop für Vue (8kB gzipped)
- **tiptap**: Rich-Text Editor für Text-Widgets (150kB gzipped)
- **html2canvas**: Email-Preview Screenshots (200kB gzipped)
- **juice**: CSS Inlining für Email-Export (50kB gzipped)

### Bestehende Stack Integration
- **Vue 3 Composition API**: Reactive State für Widgets
- **Pinia**: Global State Management
- **VueUse**: Utility Functions (useDraggable, useLocalStorage)
- **Tailwind CSS**: Utility-First Styling für Components

---

## Risiken & Mitigation

### Performance Risiken
- **Canvas Re-Rendering**: Bei vielen Widgets → Virtual Scrolling + Memoization
- **Drag Performance**: HTML5 API + Web Animations API
- **Memory Leaks**: Proper Cleanup in unmounted Components

### UX Risiken
- **Learning Curve**: Progressive Disclosure, Tooltips, Tutorials
- **Browser Compatibility**: Fallback zu Sortable.js für IE11
- **Mobile Editing**: Touch-Optimized Drag&Drop

### Integration Risiken
- **Backend Sync**: Optimistic Updates + Conflict Resolution
- **Template Parsing**: Robust HTML-to-Widget Conversion
- **Email Rendering**: Server-side Preview für Consistency

---

## Empfehlung

**PROCEED** mit moderater Priorität nach UX-Review.  
Die Frontend-Implementierung ist komplex aber manageable mit Vue.js 3. Fokussieren auf Core Drag&Drop zuerst, dann schrittweise Features hinzufügen. Enge Zusammenarbeit mit @Backend für State-Sync und @QA für Cross-Browser Testing.

## Nächste Schritte
1. UX Wireframes für Builder-Interface erstellen
2. Prototype mit grundlegendem Drag&Drop entwickeln
3. Widget-Library Design und Implementierung
4. Integration mit bestehendem EmailService testen</content>
<parameter name="filePath">/Users/holger/Documents/Projekte/B2Connect/REQ-007-frontend-analysis.md