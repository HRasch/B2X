# CMS-Verwaltung – Admin Guide

> **Modul**: CMS (Content Management System)  
> **Zielgruppe**: Content Editor, Administratoren  
> **Version**: 1.0

---

## Übersicht

Das CMS ermöglicht die Verwaltung von Seiteninhalten, Templates und Medien ohne Programmierkenntnisse.

## Module

| Modul | Beschreibung | Guide-Abschnitt |
|-------|--------------|-----------------|
| **Seiten** | Statische Inhaltsseiten | [Seiten](#seiten) |
| **Templates** | Seitenlayouts | [Templates](#templates) |
| **Mediathek** | Bilder und Dokumente | [Mediathek](#mediathek) |

---

## Seiten

### Navigation

**Menü**: CMS → Seiten

### Seitenliste

| Spalte | Beschreibung |
|--------|--------------|
| **Titel** | Seitenname |
| **Slug** | URL-Pfad |
| **Template** | Verwendetes Layout |
| **Status** | Veröffentlicht/Entwurf |
| **Geändert** | Letzte Änderung |

### Seite erstellen

1. Klicken Sie auf **+ Neue Seite**
2. Füllen Sie die Felder aus:

| Feld | Pflicht | Beschreibung |
|------|---------|--------------|
| **Titel** | ✅ | Seitenüberschrift |
| **Slug** | ✅ | URL-Pfad (auto-generiert) |
| **Template** | ✅ | Layout auswählen |
| **Inhalt** | ❌ | Seiteninhalt (Editor) |

### Content-Editor

Der WYSIWYG-Editor bietet:

**Formatierung**:
- Überschriften (H1-H6)
- Fett, Kursiv, Unterstrichen
- Listen (nummeriert, Punkte)
- Zitate

**Medien**:
- Bilder einfügen
- Videos einbetten
- Dokumente verlinken

**Struktur**:
- Tabellen
- Spalten-Layout
- Trennlinien

**Code**:
- HTML-Ansicht
- Code-Blöcke

### SEO-Einstellungen

| Feld | Beschreibung |
|------|--------------|
| **Meta-Titel** | Browser-Tab (max. 60 Zeichen) |
| **Meta-Beschreibung** | Google-Snippet (max. 160 Zeichen) |
| **Canonical URL** | Falls abweichend |
| **Robots** | Index/NoIndex, Follow/NoFollow |

### Veröffentlichen

- **Als Entwurf speichern**: Nicht öffentlich sichtbar
- **Veröffentlichen**: Sofort online
- **Planen**: Zu bestimmtem Zeitpunkt veröffentlichen

### Seitenversionen

Das CMS speichert automatisch Versionen:

1. Öffnen Sie eine Seite
2. Klicken Sie auf **Versionen**
3. Sehen Sie alle Änderungen
4. Klicken Sie auf **Wiederherstellen** für eine ältere Version

---

## Templates

### Navigation

**Menü**: CMS → Templates

### Verfügbare Templates

| Template | Verwendung |
|----------|------------|
| **Standard** | Normale Inhaltsseiten |
| **Landing Page** | Marketing-Seiten mit Sektionen |
| **Blog** | Artikel mit Seitenleiste |
| **Kontakt** | Seite mit Formular |
| **Datenschutz** | Rechtliche Texte |

### Template-Bereiche

Jedes Template definiert Bereiche:

```
┌─────────────────────────────────────┐
│             Header                  │
├─────────────────────────────────────┤
│                                     │
│         Haupt-Content               │
│                                     │
├─────────────────────────────────────┤
│  Sidebar (optional)                 │
├─────────────────────────────────────┤
│             Footer                  │
└─────────────────────────────────────┘
```

### Template bearbeiten

⚠️ **Hinweis**: Template-Änderungen betreffen alle Seiten mit diesem Template!

1. Öffnen Sie ein Template
2. Bearbeiten Sie die Bereiche
3. Speichern Sie die Änderungen

---

## Mediathek

### Navigation

**Menü**: CMS → Mediathek

### Unterstützte Formate

| Typ | Formate | Max. Größe |
|-----|---------|------------|
| **Bilder** | JPG, PNG, WebP, GIF, SVG | 10 MB |
| **Dokumente** | PDF, DOC, DOCX, XLS, XLSX | 25 MB |
| **Videos** | MP4, WebM | 100 MB |

### Dateien hochladen

1. Klicken Sie auf **+ Hochladen**
2. Ziehen Sie Dateien per Drag & Drop, oder
3. Klicken Sie zum Auswählen
4. Warten Sie auf den Upload-Fortschritt

### Ordnerstruktur

Organisieren Sie Medien in Ordnern:

```
📁 Mediathek
├── 📁 Produkte
│   ├── 📁 Kategorie-A
│   └── 📁 Kategorie-B
├── 📁 Blog
├── 📁 Banner
└── 📁 Dokumente
```

### Ordner erstellen

1. Klicken Sie auf **+ Neuer Ordner**
2. Geben Sie einen Namen ein
3. Klicken Sie auf **Erstellen**

### Bild bearbeiten

Basis-Bildbearbeitung direkt in der Mediathek:

- **Zuschneiden**: Bildausschnitt wählen
- **Drehen**: 90° links/rechts
- **Größe ändern**: Pixel-Maße anpassen

1. Wählen Sie ein Bild
2. Klicken Sie auf **Bearbeiten**
3. Nehmen Sie Änderungen vor
4. **Als Kopie speichern** oder **Überschreiben**

### Bild verwenden

So fügen Sie ein Bild auf einer Seite ein:

1. Öffnen Sie die Seite im Editor
2. Positionieren Sie den Cursor
3. Klicken Sie auf **Bild einfügen**
4. Wählen Sie aus der Mediathek oder laden Sie neu hoch
5. Fügen Sie Alt-Text hinzu (wichtig für Barrierefreiheit!)

### Dateien löschen

⚠️ **Achtung**: Prüfen Sie vor dem Löschen, ob die Datei verwendet wird!

1. Wählen Sie eine oder mehrere Dateien
2. Klicken Sie auf **Löschen**
3. Bestätigen Sie die Löschung

---

## Häufige Fragen

### Wie erstelle ich eine FAQ-Seite?

1. Erstellen Sie neue Seite mit Template "Standard"
2. Fügen Sie FAQ-Blöcke im Editor ein (Accordion-Format)
3. Fügen Sie strukturierte Daten hinzu (Schema.org FAQ)

### Wie optimiere ich Bilder für SEO?

1. **Dateiname**: Beschreibend (z.B. `laptop-dell-xps-15.jpg`)
2. **Alt-Text**: Beschreibung des Bildinhalts
3. **Größe**: Max. 1920px Breite für Web
4. **Format**: WebP für beste Kompression

### Wie kann ich eine Seite zeitgesteuert veröffentlichen?

1. Erstellen Sie die Seite als Entwurf
2. Klicken Sie auf **Planen**
3. Wählen Sie Datum und Uhrzeit
4. Bestätigen Sie die Planung

Die Seite wird automatisch zum geplanten Zeitpunkt veröffentlicht.

### Wie richte ich Redirects ein?

1. Gehen Sie zu **CMS → Einstellungen → Redirects**
2. Klicken Sie auf **+ Redirect hinzufügen**
3. Geben Sie die alte und neue URL ein
4. Wählen Sie den Typ (301 permanent / 302 temporär)

---

*Zurück zur [Admin-Übersicht](README.md)*
