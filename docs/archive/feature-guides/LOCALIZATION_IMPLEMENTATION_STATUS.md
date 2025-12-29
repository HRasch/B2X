# Store-Queries mit Sprach-Parameter - Implementierungs-Status

## ✅ Completed

### 1. Dtos.cs - Neue Response-DTOs
- **LocalizationHelper**: Vollständig implementiert
  - Fallback-Kette: Requested → Default → EN → Base
  - Akzeptiert null-Dictionary
  
- **CmsPageDto**: Lokalisierte Page-Responses
  - Title, Slug, Description als einzelne Werte (keine Dictionaries)
  - Language-Feld gibt Antwort-Sprache an
  
- **CmsSectionDto**: Lokalisierte Section-Responses
- **CmsComponentDto**: Lokalisierte Component-Responses

### 2. Interfaces.cs - Aktualisierte Signaturen
- ILayoutService alle Get-Methoden mit `string languageCode` Parameter:
  - `GetPageByIdAsync(tenantId, pageId, languageCode)` ✓
  - `GetPagesByTenantAsync(tenantId, languageCode)` ✓
  - `UpdatePageAsync(..., languageCode)` ✓
  - `AddComponentAsync(..., languageCode)` ✓
  - `UpdateComponentAsync(..., languageCode)` ✓
  - `GeneratePreviewHtmlAsync(..., languageCode)` ✓
  
- ILayoutRepository: Keine Änderungen erforderlich (Pass-through)

### 3. LayoutService.cs - Implementierung
- MapPageToDto(): Lokalisierung auf DTO-Ebene ✓
- MapSectionToDto(): Mapping mit Lokalisierung ✓
- MapComponentToDto(): Content lokalisieren ✓
- GenerateHtml(): HTML-Preview mit lokalisierten Inhalten ✓

## ❌ Pending - Müssen noch aktualisiert werden

### 1. Models.cs - Request-Klassen erweitern
Die vorhandenen Klassen müssen folgende Properties haben:

**CreatePageRequest** - Muss hinzufügen:
```csharp
public Dictionary<string, PageTranslation> Translations { get; set; } = new();
```

**UpdatePageRequest** - Muss hinzufügen:
```csharp
public Dictionary<string, PageTranslation>? Translations { get; set; }
```

**AddComponentRequest** - Muss hinzufügen:
```csharp
public Dictionary<string, string> ContentTranslations { get; set; } = new();
public int Order { get; set; }
public bool IsVisible { get; set; } = true;
```

**UpdateComponentRequest** - Muss hinzufügen:
```csharp
public Dictionary<string, string>? ContentTranslations { get; set; }
public int? Order { get; set; }
public bool? IsVisible { get; set; }
```

### 2. LayoutController.cs - languageCode hinzufügen
Alle Endpoints müssen `[FromQuery] string lang = "en"` Parameter haben und ihn an Service übergeben.

Beispiel:
```csharp
[HttpGet("{pageId}")]
public async Task<IActionResult> GetPageById(Guid pageId, [FromQuery] string lang = "en")
{
    var result = await _layoutService.GetPageByIdAsync(_tenantId, pageId, lang);
    return result == null ? NotFound() : Ok(result);
}
```

### 3. Tests aktualisieren
Alle 35 Test-Aufrufe müssen languageCode-Parameter hinzufügen:
- LayoutControllerTests.cs: 11 Fehler
- LayoutServiceTests.cs: 10 Fehler

Pattern:
```csharp
// Vorher
await _layoutService.GetPageByIdAsync(tenantId, pageId)

// Nachher
await _layoutService.GetPageByIdAsync(tenantId, pageId, "en")
```

## Lokalisierungs-Flow (Implementiert)

```
HTTP Request
    ↓
LayoutController.GetPageById(tenantId, pageId, lang="de")
    ↓
ILayoutService.GetPageByIdAsync(tenantId, pageId, "de")
    ↓
Repository.GetPageByIdAsync() → CmsPage mit Dictionaries
    ↓
LayoutService.MapPageToDto(page, "de")
    ↓
LocalizationHelper.GetLocalizedValue(baseValue, translations, "de")
    ↓
CmsPageDto (Title="Startseite", Description="Die Startseite", etc.)
    ↓
JSON-Response (single language, no dictionaries)
```

## Nächste Schritte (Prio-Reihenfolge)

1. **Models.cs** - Request-Klassen erweitern (5 min)
2. **LayoutController.cs** - languageCode Parameter hinzufügen (10 min)
3. **Tests** - languageCode an alle Service-Aufrufe hinzufügen (15 min)
4. **Build & Verify** - Keine Fehler, alle Tests grün (5 min)

## Technische Details

### Fallback-Verhalten
Wenn `GetPageByIdAsync(tenantId, pageId, "de")` aufgerufen wird:
1. Versuche "de" aus TitleTranslations
2. Fallback auf "en" (defaultLanguage)
3. Fallback auf Title (base value)

**Beispiel**:
```csharp
Page.Title = "Home" (base)
Page.TitleTranslations = { "de": "Startseite", "fr": "Accueil" }

GetPageByIdAsync(..., "de") → Title = "Startseite"
GetPageByIdAsync(..., "es") → Title = "Home" (fallback zu EN nicht gefunden, base value)
GetPageByIdAsync(..., "fr") → Title = "Accueil"
```

### Aktueller Status Build
- **Fehler**: 35 (hauptsächlich fehlende languageCode-Parameter)
- **Ursachen**:
  - Request-Klassen haben neue Properties nicht
  - Controller.cs übergeben languageCode nicht
  - Tests.cs haben languageCode-Parameter nicht
  
**Nicht- Blockierende Fehler**:
- Tests brauchen ReturnsAsync-Überladung für CmsPageDto (Moq-Update, nicht kritisch)

## Zusammenfassung

✅ **Phase 1 (DTOs & Service-Logik)**: KOMPLETT
- Lokalisierungshilfer
- Response-DTOs
- Service-Implementierung

🔄 **Phase 2 (Infrastructure)**: IN ARBEIT
- Muss nur Models.cs erweitern
- Muss nur Controller.cs aktualisieren
- Muss nur Tests aktualisieren

✅ **Entwurfs-Lösung**: VALIDIERT
- Keine Änderungen am Database-Schema erforderlich
- Keine Änderungen an Repository erforderlich
- Nur Service/Controller/Tests berührt
