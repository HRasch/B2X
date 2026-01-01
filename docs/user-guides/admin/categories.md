# Kategorieverwaltung – Admin Guide

> **Modul**: Katalog → Kategorien  
> **Zielgruppe**: Produktmanager, Administratoren  
> **Version**: 1.0

---

## Übersicht

Kategorien strukturieren den Produktkatalog hierarchisch. Eine gut durchdachte Kategoriestruktur verbessert die Navigation und Auffindbarkeit für Kunden.

## Kategorieliste

### Navigation

**Menü**: Katalog → Kategorien

### Ansichten

| Ansicht | Beschreibung |
|---------|--------------|
| **Baumansicht** | Hierarchische Struktur |
| **Listenansicht** | Flache Tabelle |

### Spalten (Listenansicht)

| Spalte | Beschreibung |
|--------|--------------|
| **Name** | Kategoriename |
| **Slug** | URL-Pfad |
| **Eltern** | Übergeordnete Kategorie |
| **Produkte** | Anzahl zugeordneter Produkte |
| **Status** | Aktiv/Inaktiv |
| **Aktionen** | Bearbeiten/Löschen |

## Kategorie erstellen

### Schritt 1: Grunddaten

1. Klicken Sie auf **+ Neue Kategorie**
2. Füllen Sie die Felder aus:

| Feld | Pflicht | Beschreibung |
|------|---------|--------------|
| **Name** | ✅ | Anzeigename |
| **Slug** | ✅ | URL-freundlich (auto-generiert) |
| **Beschreibung** | ❌ | SEO-optimierte Beschreibung |
| **Elternkategorie** | ❌ | Für Unterkategorien |

### Schritt 2: Bild

- **Kategoriebild**: Wird in der Navigation angezeigt
- **Format**: JPG, PNG, WebP (max. 2 MB)
- **Empfohlene Größe**: 400 x 400 px

### Schritt 3: SEO

| Feld | Beschreibung |
|------|--------------|
| **Meta-Titel** | Browser-Tab Titel |
| **Meta-Beschreibung** | Google-Snippet |
| **Canonical URL** | Falls abweichend |

### Schritt 4: Speichern

Klicken Sie auf **Speichern**.

## Kategoriehierarchie

### Struktur

```
📁 Elektronik (Hauptkategorie)
├── 📁 Computer (Unterkategorie)
│   ├── 📁 Laptops (Unter-Unterkategorie)
│   └── 📁 Desktops
├── 📁 Smartphones
└── 📁 Zubehör
```

### Hierarchie erstellen

1. Erstellen Sie zuerst die **Hauptkategorie** (ohne Eltern)
2. Erstellen Sie **Unterkategorien** mit Verweis auf Eltern
3. Max. Tiefe: **3 Ebenen** empfohlen

### Reihenfolge ändern

1. Wechseln Sie zur **Baumansicht**
2. Ziehen Sie Kategorien per **Drag & Drop**
3. Die Sortierung wird automatisch gespeichert

## Kategorie bearbeiten

1. Klicken Sie auf den Kategorienamen oder das Bearbeiten-Icon
2. Ändern Sie die gewünschten Felder
3. Klicken Sie auf **Speichern**

## Kategorie löschen

⚠️ **Wichtige Hinweise**:
- Kategorien mit zugeordneten Produkten können nicht gelöscht werden
- Kategorien mit Unterkategorien können nicht gelöscht werden

### Voraussetzungen zum Löschen

1. Entfernen Sie alle Produkte aus der Kategorie
2. Löschen oder verschieben Sie alle Unterkategorien
3. Dann kann die Kategorie gelöscht werden

### Löschen

1. Klicken Sie auf das Löschen-Icon (🗑️)
2. Bestätigen Sie die Löschung

## API-Endpunkte

Für Entwickler stehen folgende API-Endpunkte zur Verfügung:

| Methode | Endpunkt | Beschreibung |
|---------|----------|--------------|
| `GET` | `/api/categories` | Alle aktiven Kategorien |
| `GET` | `/api/categories/{id}` | Kategorie nach ID |
| `GET` | `/api/categories/slug/{slug}` | Kategorie nach Slug |
| `GET` | `/api/categories/root` | Nur Hauptkategorien |
| `GET` | `/api/categories/{id}/children` | Unterkategorien |
| `GET` | `/api/categories/hierarchy` | Komplette Hierarchie |
| `POST` | `/api/categories` | Neue Kategorie |
| `PUT` | `/api/categories/{id}` | Kategorie aktualisieren |
| `DELETE` | `/api/categories/{id}` | Kategorie löschen |

## Best Practices

### Namensgebung

✅ **Gut**:
- "Laptops & Notebooks"
- "Drucker & Scanner"
- "Bürobedarf"

❌ **Vermeiden**:
- "Kategorie 1"
- "Diverses"
- Zu lange Namen (max. 50 Zeichen)

### Struktur

✅ **Empfohlen**:
- Max. 3 Ebenen Tiefe
- 5-10 Hauptkategorien
- Aussagekräftige Namen

❌ **Vermeiden**:
- Zu tiefe Hierarchien
- Kategorien mit nur 1-2 Produkten
- Überschneidende Kategorien

## Häufige Fragen

### Kann ich eine Kategorie umbenennen?

Ja, bearbeiten Sie einfach den Namen. Der Slug bleibt erhalten, um bestehende URLs nicht zu brechen. Bei Bedarf kann der Slug separat geändert werden.

### Was passiert mit Produkten beim Löschen?

Produkte werden **nicht** gelöscht. Sie müssen vorher:
1. Einer anderen Kategorie zugeordnet werden, oder
2. Die Kategorie-Zuordnung entfernt werden

### Wie erstelle ich eine Aktion-Kategorie?

1. Erstellen Sie eine neue Kategorie (z.B. "Sale")
2. Ordnen Sie Aktionsprodukte dieser Kategorie zu
3. Produkte können mehreren Kategorien zugeordnet sein

---

*Nächster Guide: [Benutzerverwaltung](users.md)*
