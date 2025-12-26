# B2Connect - Mehrsprachigkeits-Analyse nach Service

## Übersicht
Detaillierte Analyse der Entitäten in verschiedenen Services und deren Mehrsprachigkeits-Unterstützung.

---

## 1. CATALOG SERVICE

### Verzeichnis
`backend/services/Catalog/src/Models/`

### Entitäten

#### `Product` (Main Entity)
**Status**: ⚠️ **NICHT MEHRSPRACHIG - SOLLTE ABER SEIN**

```csharp
public class Product
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public required string Sku { get; set; }           // ❌ NICHT lokalisiert
    public required string Name { get; set; }          // ❌ SOLLTE lokalisiert sein
    public string? Description { get; set; }           // ❌ SOLLTE lokalisiert sein
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; }
    public List<string> Categories { get; set; }       // ⚠️ Referenzen zu Kategorien (keine Lokalisierung)
    public string? BrandName { get; set; }             // ⚠️ KÖNNTE lokalisiert sein
    public List<string> Tags { get; set; }             // ⚠️ KÖNNTE lokalisiert sein
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsAvailable => StockQuantity > 0 && IsActive;
}
```

**Mehrsprachig erforderlich**: 
- ✅ `Name` - Produktname
- ✅ `Description` - Produktbeschreibung
- ⚠️ `BrandName` - Optional/empfohlen

**Fehlende Entität**: Keine separate `ProductTranslation` oder `CategoryTranslation` Entität vorhanden!

---

## 2. THEMING SERVICE (Design/Theme)

### Verzeichnis
`backend/services/Theming/src/`

### Entitäten

#### `Theme` (Main Entity)
**Status**: ⚠️ **TEILWEISE MEHRSPRACHIG - SOLLTE ÜBERPRÜFT WERDEN**

```csharp
public class Theme
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Name { get; set; }                    // ❌ NICHT lokalisiert
    public string Description { get; set; }             // ❌ NICHT lokalisiert
    public string PrimaryColor { get; set; }
    public string SecondaryColor { get; set; }
    public string TertiaryColor { get; set; }
    public List<DesignVariable> Variables { get; set; }
    public List<ThemeVariant> Variants { get; set; }
    public bool IsActive { get; set; }
    public DateTime? PublishedAt { get; set; }
    public int Version { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
}
```

**Mehrsprachig erforderlich**: 
- ⚠️ `Name` - Theme-Name
- ⚠️ `Description` - Theme-Beschreibung

#### `DesignVariable` (Child)
**Status**: ⚠️ **TEILWEISE MEHRSPRACHIG - SOLLTE ÜBERPRÜFT WERDEN**

```csharp
public class DesignVariable
{
    public Guid Id { get; set; }
    public Guid ThemeId { get; set; }
    public string Name { get; set; }                    // ❌ NICHT lokalisiert
    public string Value { get; set; }                   // 🟢 Nicht erforderlich (Design-Token)
    public string Category { get; set; }
    public string Description { get; set; }             // ❌ NICHT lokalisiert
    public VariableType Type { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

**Mehrsprachig erforderlich**: 
- ⚠️ `Name` - Variable-Bezeichnung
- ⚠️ `Description` - Erklärung der Variable

#### `ThemeVariant` (Child)
**Status**: ⚠️ **NICHT MEHRSPRACHIG**

```csharp
public class ThemeVariant
{
    public Guid Id { get; set; }
    public Guid ThemeId { get; set; }
    public string Name { get; set; }                    // ❌ NICHT lokalisiert
    public string Description { get; set; }             // ❌ NICHT lokalisiert
    public Dictionary<string, string> VariableOverrides { get; set; }
    public bool IsEnabled { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

**Mehrsprachig erforderlich**: 
- ⚠️ `Name` - z.B. "Dark Mode", "High Contrast"
- ⚠️ `Description` - Beschreibung der Variante

---

## 3. THEMING SERVICE (Layout/CMS Pages)

### Verzeichnis
`backend/services/Theming/Layout/src/`

### Entitäten

#### `CmsPage` (Main Entity)
**Status**: ❌ **NICHT MEHRSPRACHIG - SOLLTE SEIN**

```csharp
public class CmsPage
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Title { get; set; }                   // ❌ NICHT lokalisiert
    public string Slug { get; set; }                    // ❌ NICHT lokalisiert (URL)
    public string Description { get; set; }             // ❌ NICHT lokalisiert
    public List<CmsSection> Sections { get; set; }
    public PageVisibility Visibility { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime? ScheduledPublishAt { get; set; }
    public int Version { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
}
```

**Mehrsprachig erforderlich**: 
- ✅ `Title` - Seiten-Titel
- ✅ `Slug` - URL-Slug (sprachspezifisch!)
- ✅ `Description` - Meta-Beschreibung

#### `CmsSection` (Child)
**Status**: ❌ **NICHT MEHRSPRACHIG - SOLLTE SEIN**

```csharp
public class CmsSection
{
    public Guid Id { get; set; }
    public Guid PageId { get; set; }
    public string Type { get; set; }
    public int Order { get; set; }
    public SectionLayout Layout { get; set; }
    public List<CmsComponent> Components { get; set; }
    public Dictionary<string, object> Settings { get; set; }
    public Dictionary<string, string> Styling { get; set; }
    public bool IsVisible { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

**Mehrsprachig erforderlich**: 
- ⚠️ Potentiell in `Settings` (abhängig vom Inhalt)

#### `CmsComponent` (Child)
**Status**: ❌ **NICHT MEHRSPRACHIG - SOLLTE SEIN**

```csharp
public class CmsComponent
{
    public Guid Id { get; set; }
    public Guid SectionId { get; set; }
    public string Type { get; set; }
    public string Content { get; set; }                 // ❌ SOLLTE lokalisiert sein (Text!)
    public List<ComponentVariable> Variables { get; set; }
    public Dictionary<string, string> Styling { get; set; }
    public ComponentDataBinding? DataBinding { get; set; }
    public bool IsVisible { get; set; }
    public int Order { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

**Mehrsprachig erforderlich**: 
- ✅ `Content` - Text-Inhalt der Komponente
- ⚠️ `Variables` - Texte in Component-Props

#### `ComponentVariable` (Child)
**Status**: ⚠️ **NICHT MEHRSPRACHIG - KÖNNTE SEIN**

```csharp
public class ComponentVariable
{
    public string Name { get; set; }
    public object Value { get; set; }                   // ❌ Texte SOLLTEN lokalisiert sein
    public string Type { get; set; }
    public string Description { get; set; }
}
```

**Mehrsprachig erforderlich**: 
- ⚠️ `Value` - Falls Textwert

#### `ComponentDefinition` (Support Entity)
**Status**: ⚠️ **NICHT MEHRSPRACHIG**

```csharp
public class ComponentDefinition
{
    public string ComponentType { get; set; }
    public string DisplayName { get; set; }             // ❌ NICHT lokalisiert
    public string Description { get; set; }             // ❌ NICHT lokalisiert
    public string Category { get; set; }
    public string Icon { get; set; }
    public List<ComponentProp> Props { get; set; }
    public List<ComponentSlot> Slots { get; set; }
    public List<ComponentPreset> PresetVariants { get; set; }
}
```

**Mehrsprachig erforderlich**: 
- ⚠️ `DisplayName` - Komponenten-Bezeichnung
- ⚠️ `Description` - Komponenten-Beschreibung

---

## 4. IDENTITY SERVICE

### Verzeichnis
`backend/services/Identity/src/`

### Entitäten

#### `AppUser`
**Status**: ⚠️ **NICHT MEHRSPRACHIG - OPTIONAL**

```csharp
public class AppUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? TenantId { get; set; }
    public bool IsTwoFactorRequired { get; set; }
    public bool IsActive { get; set; } = true;
}
```

**Mehrsprachig erforderlich**: 
- ❌ NICHT erforderlich (Benutzerdaten sind nicht-lokalisierbar)

#### `AppRole`
**Status**: ⚠️ **NICHT MEHRSPRACHIG - OPTIONAL**

```csharp
public class AppRole : IdentityRole
{
    public string? Description { get; set; }             // ⚠️ KÖNNTE lokalisiert sein
}
```

**Mehrsprachig erforderlich**: 
- ⚠️ `Description` - Rollenbeschreibung (optional)

---

## 5. TENANCY SERVICE

### Verzeichnis
`backend/services/Tenancy/`

### Status
❌ **KEINE DOMAIN-ENTITÄTEN VORHANDEN** - Nur `Program.cs`

Die Tenancy-Service scheint nur Orchestrierungs-Funktionalität zu haben und keine eigenen Domain-Entitäten zu besitzen.

---

## 6. LOCALIZATION SERVICE (Eigene Mehrsprachigkeits-Implementierung)

### Verzeichnis
`backend/services/Localization/src/`

### Entitäten

#### `LocalizedString` (Main Entity)
**Status**: 🟢 **BEREITS MEHRSPRACHIG**

```csharp
public class LocalizedString
{
    public Guid Id { get; set; }
    public string Key { get; set; }                     // 🟢 Translation-Key
    public string Category { get; set; }                // 🟢 Translation-Kategorie
    public Dictionary<string, string> Translations { get; set; }  // 🟢 Language -> Text
    public string DefaultValue { get; set; }            // 🟢 Fallback (EN)
    public Guid? TenantId { get; set; }                 // 🟢 Tenant-spezifisch
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

**Mehrsprachigkeit**: 
- ✅ `Translations` - Dictionary mit Language Code Keys (en, de, fr, es, it, pt, nl, pl)
- ✅ `DefaultValue` - Englischer Fallback
- ✅ `TenantId` - Tenant-spezifische Übersetzungen möglich

**Mechan ismus**:
- 🟢 Zentrale Key-Value Lokalisierung für UI-Strings
- 🟢 `ILocalizationService` - Service für Translation-Abruf
- 🟢 `IEntityLocalizationService` - Service für Entity-Property-Übersetzungen
- 🟢 MemoryCache mit 60-Minuten-TTL

---

## 7. SHARED KERNEL - Localization Types

### Verzeichnis
`backend/shared/kernel/`

### Entitäten

#### `LocalizedContent`
**Status**: 🟢 **BEREITS IMPLEMENTIERT - KANN VERWENDET WERDEN**

```csharp
public class LocalizedContent
{
    public Dictionary<string, string> Translations { get; set; } = new();
    public string DefaultLanguage { get; set; } = "en";
    
    // Methods:
    public string Get(string languageCode)
    public string GetDefault()
    public LocalizedContent Set(string languageCode, string value)
    public LocalizedContent SetMany(Dictionary<string, string> translations)
    // ... weitere Utility-Methoden
}
```

**Use Case**: 
- 🟢 JSON-Serialisierung in Entity-Properties
- 🟢 Keine separaten Translation-Tables nötig
- 🟢 Kann direkt in `Product.Name`, `CmsPage.Title`, etc. verwendet werden

#### `LocalizedContent` Usage Patterns (aus LocalizableEntities.cs):

```csharp
// Beispiel 1: Product mit Lokalisierung
public class Product : Entity
{
    public LocalizedContent Name { get; set; } = new();
    public LocalizedContent Description { get; set; } = new();
    public required string Sku { get; set; };
    public decimal Price { get; set; };
    public Guid TenantId { get; set; };
    public ProductStatus Status { get; set; };
}

// Beispiel 2: ContentPage
public class ContentPage : Entity
{
    public LocalizedContent Title { get; set; } = new();
    public LocalizedContent Description { get; set; } = new();
    public LocalizedContent Content { get; set; } = new();
    public required string Slug { get; set; };
    public Guid TenantId { get; set; };
    public bool IsPublished { get; set; };
}

// Beispiel 3: MenuItem
public class MenuItem : Entity
{
    public LocalizedContent Label { get; set; } = new();
    // ...
}
```

---

## 8. CMS SERVICE (Alternative zu Layout Service)

### Verzeichnis
`backend/services/CMS/Core/Domain/`

### Entitäten

#### `PageDefinition`
**Status**: ❌ **NICHT MEHRSPRACHIG - SOLLTE SEIN**

```csharp
public class PageDefinition : AggregateRoot
{
    public string TenantId { get; set; }
    public string PageType { get; set; }
    public string PagePath { get; set; }                // ❌ NICHT lokalisiert
    public string PageTitle { get; set; }               // ❌ NICHT lokalisiert
    public string PageDescription { get; set; }         // ❌ NICHT lokalisiert
    public string MetaKeywords { get; set; }            // ❌ NICHT lokalisiert
    public string TemplateLayout { get; set; }
    public List<PageRegion> Regions { get; set; }
    public Dictionary<string, object> GlobalSettings { get; set; }
    public bool IsPublished { get; set; }
    public DateTime PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int Version { get; set; }
}
```

**Mehrsprachig erforderlich**: 
- ✅ `PageTitle` - Seiten-Titel
- ✅ `PageDescription` - Meta-Beschreibung
- ✅ `MetaKeywords` - SEO-Keywords
- ✅ `PagePath` - URL-Slug (sprachspezifisch!)

#### `PageRegion`
**Status**: ⚠️ **KEINE TEXTINHALTE - NICHT RELEVANT**

```csharp
public class PageRegion
{
    public string Id { get; set; }
    public string PageDefinitionId { get; set; }
    public string Name { get; set; }
    public int Order { get; set; }
    public int MaxWidgets { get; set; }
    public List<WidgetInstance> Widgets { get; set; }
    public Dictionary<string, object> RegionSettings { get; set; }
}
```

#### `WidgetDefinition`
**Status**: ⚠️ **NICHT MEHRSPRACHIG - OPTIONAL**

```csharp
public class WidgetDefinition
{
    public string Id { get; set; }
    public string DisplayName { get; set; }             // ⚠️ KÖNNTE lokalisiert sein
    public string Description { get; set; }             // ⚠️ KÖNNTE lokalisiert sein
    public string ComponentPath { get; set; }
    public string Category { get; set; }
    public string ThumbnailUrl { get; set; }
    public int PreviewWidth { get; set; }
    public int PreviewHeight { get; set; }
    public List<WidgetSetting> DefaultSettings { get; set; }
    public List<string> AllowedPageTypes { get; set; }
    public bool IsEnabled { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

---

## ZUSAMMENFASSUNG

### 🟢 BEREITS MEHRSPRACHIG
1. **Localization Service** - `LocalizedString` Entity (zentrale UI-Übersetzungen)
2. **Shared Kernel** - `LocalizedContent` (zur Verwendung in Entities)

### ❌ NICHT MEHRSPRACHIG - SOLLTEN ABER SEIN

| Service | Entität | Felder | Priorität |
|---------|---------|--------|-----------|
| **Catalog** | `Product` | Name, Description, BrandName | 🔴 HOCH |
| **Theming** | `Theme` | Name, Description | 🔴 HOCH |
| **Theming** | `DesignVariable` | Name, Description | 🟡 MITTEL |
| **Theming** | `ThemeVariant` | Name, Description | 🟡 MITTEL |
| **Layout** | `CmsPage` | Title, Slug, Description | 🔴 HOCH |
| **Layout** | `CmsComponent` | Content, Variables.Value | 🔴 HOCH |
| **Layout** | `ComponentDefinition` | DisplayName, Description | 🟡 MITTEL |
| **CMS** | `PageDefinition` | PageTitle, PagePath, PageDescription, MetaKeywords | 🔴 HOCH |
| **Identity** | `AppRole` | Description | 🟢 NIEDRIG |

### ⚠️ OPTIONAL
- **Identity Service** - `AppUser` (nicht-lokalierbare Benutzerdaten)
- **Tenancy Service** - Keine Domain-Entitäten vorhanden

---

## IMPLEMENTIERUNGS-EMPFEHLUNGEN

### Option 1: Mit `LocalizedContent` (JSON in DB)
```csharp
// Einfach, keine zusätzlichen Tables
public class Product
{
    public LocalizedContent Name { get; set; } = new();
    public LocalizedContent Description { get; set; } = new();
    // ...
}
```

### Option 2: Mit separaten Translation-Entitäten
```csharp
// Bessere Normalisierung, komplexer
public class Product
{
    public string NameKey { get; set; }  // Reference zu LocalizedString
    public string DescriptionKey { get; set; }
    // ...
}
```

### Option 3: Hybrid-Ansatz
```csharp
// Kurze Texte mit LocalizedContent, lange mit LocalizedString-Reference
public class CmsPage
{
    public LocalizedContent Title { get; set; } = new();
    public string ContentKey { get; set; }  // Reference zu LocalizedString
    // ...
}
```

### Unterstützte Sprachen (aus LocalizationService)
- en, de, fr, es, it, pt, nl, pl

---

## NÄCHSTE SCHRITTE
1. Entscheidung: `LocalizedContent` vs. `LocalizedString` Ansatz
2. Migration der oben genannten Entitäten
3. Update DTOs für Lokalisierung
4. Integration mit `IEntityLocalizationService`
5. Frontend-Integration für Sprachenwechsel
