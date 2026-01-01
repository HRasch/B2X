# Benutzerverwaltung – Admin Guide

> **Modul**: Benutzer  
> **Zielgruppe**: Administratoren, User Manager  
> **Version**: 1.0

---

## Übersicht

Die Benutzerverwaltung ermöglicht das Erstellen, Bearbeiten und Verwalten von Benutzerkonten und deren Berechtigungen.

## Benutzerliste

### Navigation

**Menü**: Benutzer

### Spalten

| Spalte | Beschreibung |
|--------|--------------|
| **Avatar** | Benutzer-Bild |
| **Name** | Vor- und Nachname |
| **E-Mail** | Anmelde-Adresse |
| **Rollen** | Zugewiesene Rollen |
| **Status** | Aktiv/Inaktiv/Ausstehend |
| **Erstellt** | Registrierungsdatum |
| **Aktionen** | Bearbeiten/Löschen |

### Filter

- **Suche**: Name, E-Mail
- **Rolle**: Nach Rolle filtern
- **Status**: Aktiv, Inaktiv, Ausstehend
- **Zeitraum**: Registrierungszeitraum

## Benutzer erstellen

### Schritt 1: Grunddaten

1. Klicken Sie auf **+ Neuer Benutzer**
2. Füllen Sie die Felder aus:

| Feld | Pflicht | Beschreibung |
|------|---------|--------------|
| **E-Mail** | ✅ | Eindeutige E-Mail-Adresse |
| **Vorname** | ❌ | Vorname des Benutzers |
| **Nachname** | ❌ | Nachname des Benutzers |
| **Passwort** | ❌ | Initial-Passwort (oder Einladung per E-Mail) |

### Schritt 2: Rollen

Weisen Sie eine oder mehrere Rollen zu:

| Rolle | Beschreibung |
|-------|--------------|
| **Admin** | Vollzugriff auf das Admin Portal |
| **Product Manager** | Produkte und Kategorien verwalten |
| **Content Editor** | CMS-Inhalte bearbeiten |
| **User Manager** | Nur Benutzerverwaltung |
| **Viewer** | Nur lesender Zugriff |

### Schritt 3: Speichern

- **Speichern**: Konto ohne E-Mail-Benachrichtigung erstellen
- **Speichern & Einladen**: E-Mail zur Passwort-Erstellung senden

## Benutzer bearbeiten

1. Klicken Sie auf den Benutzernamen oder das Bearbeiten-Icon
2. Ändern Sie die gewünschten Felder
3. Klicken Sie auf **Speichern**

### Passwort zurücksetzen

1. Öffnen Sie den Benutzer
2. Klicken Sie auf **Passwort zurücksetzen**
3. Wählen Sie eine Option:
   - **Neues Passwort setzen**: Sie geben ein temporäres Passwort ein
   - **Link senden**: Benutzer erhält E-Mail zum Zurücksetzen

## Benutzer deaktivieren

⚠️ **Empfohlen statt Löschen**: Deaktivierte Benutzer können sich nicht anmelden, ihre Daten bleiben aber für die Historie erhalten.

1. Öffnen Sie den Benutzer
2. Setzen Sie **Status** auf "Inaktiv"
3. Klicken Sie auf **Speichern**

## Benutzer löschen

⚠️ **Achtung**: Diese Aktion kann nicht rückgängig gemacht werden!

1. Klicken Sie auf das Löschen-Icon (🗑️)
2. Bestätigen Sie die Löschung

**Hinweis**: Benutzer mit zugeordneten Daten (Bestellungen, Aktivitäten) können nur deaktiviert werden.

## Rollenverwaltung

### Verfügbare Rollen

| Rolle | Produkte | Kategorien | CMS | Benutzer | Einstellungen |
|-------|----------|------------|-----|----------|---------------|
| **Admin** | ✅ CRUD | ✅ CRUD | ✅ CRUD | ✅ CRUD | ✅ |
| **Product Manager** | ✅ CRUD | ✅ CRUD | ❌ | ❌ | ❌ |
| **Content Editor** | 👁️ Nur lesen | 👁️ Nur lesen | ✅ CRUD | ❌ | ❌ |
| **User Manager** | ❌ | ❌ | ❌ | ✅ CRUD | ❌ |
| **Viewer** | 👁️ Nur lesen | 👁️ Nur lesen | 👁️ Nur lesen | 👁️ Nur lesen | ❌ |

### Mehrere Rollen

Ein Benutzer kann mehrere Rollen haben. Die Berechtigungen werden additiv kombiniert.

**Beispiel**: Product Manager + Content Editor = Kann Produkte UND CMS bearbeiten

## API-Endpunkte

Für Entwickler stehen folgende API-Endpunkte zur Verfügung:

| Methode | Endpunkt | Beschreibung |
|---------|----------|--------------|
| `GET` | `/api/admin/users` | Alle Benutzer des Tenants |
| `GET` | `/api/admin/users/{userId}` | Benutzer nach ID |
| `POST` | `/api/admin/users` | Neuen Benutzer erstellen |
| `PUT` | `/api/admin/users/{userId}` | Benutzer aktualisieren |
| `DELETE` | `/api/admin/users/{userId}` | Benutzer löschen |

### Header-Anforderung

```
X-Tenant-ID: {tenant-uuid}
Authorization: Bearer {token}
```

### Request-Body (Erstellen)

```json
{
  "email": "user@example.com",
  "firstName": "Max",
  "lastName": "Mustermann",
  "password": "securePassword123",
  "roles": ["Admin", "Product Manager"]
}
```

## Aktivitätsprotokoll

Für jeden Benutzer werden alle Aktionen protokolliert:

- Login/Logout
- Änderungen an Produkten
- Änderungen an Kategorien
- CMS-Bearbeitungen
- Benutzeraktionen

**Ansehen**: Benutzer öffnen → Tab "Aktivitäten"

## Sicherheitsrichtlinien

### Passwortanforderungen

- Mindestens 8 Zeichen
- Mindestens 1 Großbuchstabe
- Mindestens 1 Kleinbuchstabe
- Mindestens 1 Zahl
- Mindestens 1 Sonderzeichen

### Sitzungsverwaltung

- Automatische Abmeldung nach 30 Minuten Inaktivität
- Maximale Sitzungsdauer: 8 Stunden
- Bei Passwortänderung werden alle aktiven Sitzungen beendet

### Fehlgeschlagene Anmeldungen

- Nach 5 fehlgeschlagenen Versuchen: 15 Minuten Sperre
- Nach 10 Versuchen: Konto wird deaktiviert (Admin-Eingriff erforderlich)

## Häufige Fragen

### Wie lade ich mehrere Benutzer gleichzeitig ein?

1. Bereiten Sie eine CSV-Datei mit E-Mail-Adressen vor
2. Klicken Sie auf **Import**
3. Laden Sie die CSV hoch
4. Wählen Sie Standard-Rollen
5. Alle Benutzer erhalten Einladungs-E-Mails

### Kann ein Benutzer mehreren Tenants angehören?

Ja, dieselbe E-Mail kann in verschiedenen Tenants registriert sein. Der Benutzer wählt beim Login den Tenant aus.

### Wie kann ich Berechtigungen prüfen?

1. Öffnen Sie den Benutzer
2. Tab **Berechtigungen** zeigt alle effektiven Rechte
3. Die Ansicht zeigt, welche Rolle welche Berechtigung gewährt

---

*Nächster Guide: [CMS-Verwaltung](cms.md)*
