# Produktverwaltung – Admin Guide

> **Modul**: Katalog → Produkte  
> **Zielgruppe**: Produktmanager, Administratoren  
> **Version**: 1.0

---

## Übersicht

Die Produktverwaltung ermöglicht das Erstellen, Bearbeiten und Verwalten aller Produkte im B2Connect-Katalog.

## Produktliste

### Navigation

**Menü**: Katalog → Produkte

### Spalten

| Spalte | Beschreibung |
|--------|--------------|
| **Bild** | Produkt-Thumbnail |
| **Name** | Produktname (klickbar) |
| **SKU** | Artikelnummer |
| **Preis** | Netto-Preis |
| **Bestand** | Lagerbestand |
| **Status** | Aktiv/Entwurf/Archiviert |
| **Aktionen** | Bearbeiten/Löschen |

### Filter

- **Suche**: Name, SKU, Beschreibung
- **Kategorie**: Nach Kategorie filtern
- **Marke**: Nach Marke filtern
- **Status**: Aktiv, Entwurf, Archiviert
- **Preis**: Min-Max Bereich

### Sortierung

Klicken Sie auf Spaltenüberschriften zum Sortieren:
- Name (A-Z / Z-A)
- Preis (aufsteigend/absteigend)
- Bestand (niedrig/hoch)
- Erstellungsdatum

## Produkt erstellen

### Schritt 1: Grunddaten

1. Klicken Sie auf **+ Neues Produkt**
2. Füllen Sie die Pflichtfelder aus:

| Feld | Pflicht | Beschreibung |
|------|---------|--------------|
| **Name** | ✅ | Produktbezeichnung |
| **SKU** | ✅ | Eindeutige Artikelnummer |
| **Slug** | ✅ | URL-freundlicher Name |
| **Beschreibung** | ❌ | HTML-formatierte Beschreibung |
| **Kurzbeschreibung** | ❌ | Für Produktlisten |

### Schritt 2: Preise

| Feld | Beschreibung |
|------|--------------|
| **Nettopreis** | Preis ohne MwSt. |
| **Bruttopreis** | Automatisch berechnet |
| **UVP** | Unverbindliche Preisempfehlung |
| **Steuersatz** | 19% / 7% / 0% |

### Schritt 3: Kategorien & Marke

- **Kategorien**: Eine oder mehrere Kategorien zuweisen
- **Marke**: Marke auswählen (optional)

### Schritt 4: Bilder

1. Klicken Sie auf **Bilder hinzufügen**
2. Ziehen Sie Dateien per Drag & Drop oder klicken Sie zum Auswählen
3. **Formate**: JPG, PNG, WebP (max. 5 MB)
4. **Hauptbild**: Erstes Bild wird als Hauptbild verwendet

### Schritt 5: Bestand

| Feld | Beschreibung |
|------|--------------|
| **Bestandsmenge** | Verfügbare Stückzahl |
| **Mindestbestand** | Warnung bei Unterschreitung |
| **Bestandsverfolgung** | Ein/Aus |
| **Verfügbarkeit** | Auf Lager / Vorbestellung / Nicht verfügbar |

### Schritt 6: Speichern

- **Als Entwurf speichern**: Noch nicht im Shop sichtbar
- **Veröffentlichen**: Sofort im Shop sichtbar

## Produkt bearbeiten

1. Klicken Sie auf den Produktnamen oder das Bearbeiten-Icon
2. Ändern Sie die gewünschten Felder
3. Klicken Sie auf **Speichern**

## Produkt löschen

⚠️ **Achtung**: Gelöschte Produkte können nicht wiederhergestellt werden!

1. Klicken Sie auf das Löschen-Icon (🗑️)
2. Bestätigen Sie die Löschung im Dialog
3. **Alternative**: Archivieren Sie das Produkt statt es zu löschen

## Massenaktionen

1. Wählen Sie Produkte über die Checkboxen aus
2. Klicken Sie auf **Massenaktionen**
3. Wählen Sie eine Aktion:
   - **Aktivieren**: Produkte veröffentlichen
   - **Deaktivieren**: Als Entwurf setzen
   - **Archivieren**: In Archiv verschieben
   - **Löschen**: Dauerhaft entfernen
   - **Kategorie zuweisen**: Zu Kategorie hinzufügen

## Import/Export

### Export

1. Klicken Sie auf **Export**
2. Wählen Sie das Format:
   - CSV (für Excel)
   - JSON (für Entwickler)
3. Wählen Sie die Felder aus
4. Klicken Sie auf **Exportieren**

### Import

1. Klicken Sie auf **Import**
2. Laden Sie eine CSV-Datei hoch
3. Ordnen Sie die Spalten zu
4. Prüfen Sie die Vorschau
5. Klicken Sie auf **Importieren**

## Häufige Fragen

### Wie kann ich Produktvarianten erstellen?

1. Erstellen Sie das Hauptprodukt
2. Wechseln Sie zum Tab **Varianten**
3. Definieren Sie Attribute (z.B. Größe, Farbe)
4. Erstellen Sie Varianten

### Warum wird mein Produkt nicht im Shop angezeigt?

Prüfen Sie:
1. **Status**: Muss "Aktiv" sein
2. **Bestand**: Bei Bestandsverfolgung muss Bestand > 0 sein
3. **Kategorie**: Muss einer aktiven Kategorie zugeordnet sein
4. **Cache**: Shop-Cache kann verzögert sein (bis 5 Min.)

### Wie ändere ich Preise für viele Produkte?

Nutzen Sie den **Massen-Import**:
1. Exportieren Sie alle Produkte als CSV
2. Ändern Sie Preise in Excel
3. Importieren Sie die CSV erneut

---

*Nächster Guide: [Kategorien verwalten](categories.md)*
