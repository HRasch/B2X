# REQ-007 Backend Analysis: Email WYSIWYG Builder

**DocID**: `REQ-007-BACKEND-ANALYSIS`  
**Owner**: @Backend  
**Created**: 2026-01-07  
**Status**: Draft  

## Executive Summary

Die Implementierung eines Email WYSIWYG Builders erfordert moderate Backend-Änderungen mit Fokus auf erweiterte Template-Management und Widget-System. Die bestehende Email-Domain bietet eine solide Basis, erfordert jedoch neue Entities und Services für Drag&Drop-Funktionalität.

**T-Shirt Size Estimate**: M (Medium) - 12-16 Stunden  
**Risiko Level**: Medium  
**Komplexität**: Moderate Business Logic + Neue Datenmodelle  

---

## Betroffene Services/APIs

### Bestehende Services (zu erweitern)
- **EmailTemplateService**: Erweitern um Widget-Management und Customization-Logik
- **ThemeService**: Integration für Asset-Management (CSS, Bilder, Fonts)

### Neue Services erforderlich
- **EmailWidgetService**: Verwaltung von wiederverwendbaren Email-Widgets (Text, Bilder, Buttons, etc.)
- **EmailBuilderService**: Business Logic für Drag&Drop-Operationen und Template-Rendering
- **EmailPreviewService**: Server-side Rendering für Live-Preview

### API-Änderungen
```csharp
// Neue Endpoints (3 Stück wie spezifiziert)
POST /api/email/widgets - Widget erstellen/bearbeiten
GET  /api/email/templates/{id}/builder - Builder-Daten laden
POST /api/email/templates/{id}/preview - Live-Preview generieren
```

---

## Datenmodell-Änderungen

### Neue Entities

#### EmailTemplateWidget
```csharp
public class EmailTemplateWidget
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Type { get; set; } // "text", "image", "button", "divider"
    public string Name { get; set; }
    public JsonDocument Configuration { get; set; } // Flexible Widget-Konfiguration
    public bool IsSystemWidget { get; set; } // Vordefinierte vs. Custom Widgets
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
}
```

#### UserEmailCustomization
```csharp
public class UserEmailCustomization
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid TemplateId { get; set; }
    public JsonDocument WidgetPlacements { get; set; } // Drag&Drop Positionen/Layout
    public JsonDocument CustomStyles { get; set; } // Benutzerdefinierte CSS-Overrides
    public DateTime LastModified { get; set; }
}
```

### Erweiterte Entities

#### EmailTemplate (Erweiterung)
```csharp
public class EmailTemplate
{
    // Bestehende Properties...
    public bool IsBuilderEnabled { get; set; } = false; // Opt-in für WYSIWYG
    public JsonDocument BuilderMetadata { get; set; } // Layout-Info, verfügbare Widgets
    public string? ThumbnailUrl { get; set; } // Vorschau-Bild
}
```

---

## Business Logic Komplexität

### Drag&Drop Logic
- **Position Management**: Berechnung von Widget-Positionen in responsivem Layout
- **Collision Detection**: Vermeidung von Überlappungen bei Drop-Operationen
- **Responsive Adaptation**: Automatische Anpassung für Mobile/Desktop Views

### Template Rendering
- **Server-side Preview**: HTML/CSS Generierung ohne Client-Abhängigkeiten
- **Variable Substitution**: Integration mit bestehendem Variable-System
- **Email Client Compatibility**: Sicherstellung korrekter Darstellung in verschiedenen Email-Clients

### Widget System
- **Widget Registry**: Zentrales Repository für verfügbare Widgets
- **Configuration Validation**: Sicherstellung valider Widget-Konfigurationen
- **Versioning**: Änderungsverfolgung für Custom Widgets

**Komplexität Score**: Medium-High  
**Haupt-Herausforderungen**:
- Responsive Email-Design Constraints
- Email-Client Rendering Inconsistencies
- Performance bei Live-Preview

---

## Integration Points

### Monaco Editor Integration
- **Code-View Toggle**: Möglichkeit, zwischen WYSIWYG und Code-Ansicht zu wechseln
- **Syntax Highlighting**: HTML/CSS Support für Custom-Code-Blöcke
- **Change Synchronization**: Bidirektionale Synchronisation zwischen Visual und Code

### Asset Management Integration
- **ThemeService Integration**: Zugriff auf zentrales Asset-Repository
- **Image Upload/Storage**: Integration mit bestehendem File-Storage
- **Font Management**: Verfügbare Fonts aus Theme-System laden

### Weitere Integrationen
- **Search Integration**: Widgets und Templates durchsuchbar machen
- **Localization**: Widget-Labels und Standardtexte lokalisierbar
- **Audit Logging**: Änderungen an Templates/Customizations tracken

---

## Aufwandsschätzung (T-Shirt Sizes)

### Breakdown by Component

| Component | Size | Hours | Rationale |
|-----------|------|-------|-----------|
| Datenmodell + Migrationen | S | 2-3 | Neue Entities, JSON-Handling |
| EmailWidgetService | M | 4-6 | CRUD + Validation Logic |
| EmailBuilderService | M | 4-6 | Drag&Drop Logic + Rendering |
| EmailPreviewService | S | 2-3 | HTML Generation |
| API Endpoints | S | 2-3 | 3 neue REST Endpoints |
| Integration Tests | M | 4-6 | Unit + Integration Coverage |
| **GESAMT** | **M** | **12-16** | Moderate Komplexität |

### Risiken & Contingency
- **Email Rendering Complexity**: +2h für Cross-Client Testing
- **JSON Schema Evolution**: +1h für Versionierung
- **Performance bei Preview**: +2h für Caching/Optimization

---

## Implementation Roadmap

### Phase 1: Foundation (4-6h)
- Datenmodell implementieren
- Basis-Services erstellen
- API Endpoints scaffolden

### Phase 2: Core Logic (6-8h)
- Drag&Drop Business Logic
- Widget-System implementieren
- Preview-Generierung

### Phase 3: Integration & Testing (2-4h)
- Monaco Editor Integration
- Asset Management Anbindung
- Vollständige Test-Coverage

---

## Dependencies & Prerequisites

### Technische Dependencies
- PostgreSQL JSONB Support (bereits verfügbar)
- Wolverine CQRS (für neue Commands/Queries)
- Elasticsearch (für Widget-Suche)

### Projekt Dependencies
- REQ-003 Email Template System ✅ (bereits implementiert)
- ThemeService Asset Management ⚠️ (zu verifizieren)

---

## Security Considerations

- **XSS Prevention**: Widget-Content sanitization
- **Template Injection**: Variable-Substitution absichern
- **Access Control**: Tenant/User-Isolierung für Customizations
- **Audit Trail**: Änderungen an Templates loggen

---

## Monitoring & Observability

- **Performance Metrics**: Preview-Generierung Response Times
- **Usage Analytics**: Widget-Nutzung tracken
- **Error Tracking**: Template-Rendering Fehler monitoren

---

**Next Steps**:  
1. Datenmodell Review mit @Architect  
2. API Design Approval mit @TechLead  
3. Integration Testing Plan mit @QA</content>
<parameter name="filePath">/Users/holger/Documents/Projekte/B2X/REQ-007-backend-analysis.md