# Layout Service Localization Specification

## Overview
All store queries for Layout Service must include `languageCode` as a required parameter. DTOs are localized before being returned to the client.

## Query Pattern

### Basic Pattern
```csharp
Task<PageDto?> GetPageByIdAsync(Guid tenantId, Guid pageId, string languageCode)
```

### Localization Flow
1. **Query Layer** (`ILayoutRepository`)
   - Accepts `languageCode` parameter
   - Returns entity with full translation dictionaries
   - Example: `GetPageByIdAsync(tenantId, pageId, "de")`

2. **Service Layer** (`ILayoutService`)
   - Accepts `languageCode` parameter
   - Calls repository with language code
   - Localizes entity using helper methods
   - Returns localized DTO

3. **DTO Layer**
   - Contains only localized values (no dictionaries)
   - Example: `{ title: "Startseite", slug: "startseite", description: "..." }`
   - Single language per response

## Supported Languages
- `en` (English) - default fallback
- `de` (German)
- `fr` (French)
- `es` (Spanish)
- `it` (Italian)
- `pt` (Portuguese)
- `nl` (Dutch)
- `pl` (Polish)

## Default Fallback Behavior
If requested language not found in translations:
1. Try to use default language (from entity)
2. Fall back to `en`
3. Use base property if no translations exist

## Implementation Pattern

### DTO Example
```csharp
public class CmsPageDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    
    // Localized properties (single values, not dictionaries)
    public string Title { get; set; }
    public string Slug { get; set; }
    public string Description { get; set; }
    
    // Non-localized
    public PageVisibility Visibility { get; set; }
    public List<CmsSectionDto> Sections { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CmsComponentDto
{
    public Guid Id { get; set; }
    public Guid SectionId { get; set; }
    public string Type { get; set; }
    
    // Localized content
    public string Content { get; set; }
    
    // Non-localized
    public bool IsVisible { get; set; }
    public int Order { get; set; }
}
```

### Helper Method for Localization
```csharp
public static class LocalizationHelper
{
    public static string GetLocalizedValue(
        string baseValue, 
        Dictionary<string, string> translations, 
        string languageCode,
        string defaultLanguage = "en")
    {
        // Try requested language
        if (translations.TryGetValue(languageCode, out var value))
            return value;
        
        // Fall back to default language
        if (translations.TryGetValue(defaultLanguage, out var defaultValue))
            return defaultValue;
        
        // Return base value
        return baseValue;
    }
}
```

## Repository Interface
```csharp
public interface ILayoutRepository
{
    // All queries include languageCode
    Task<CmsPage?> GetPageByIdAsync(Guid tenantId, Guid pageId, string languageCode);
    Task<List<CmsPage>> GetPagesByTenantAsync(Guid tenantId, string languageCode);
    Task<CmsComponent?> GetComponentAsync(Guid componentId, string languageCode);
    
    // Creation/update still works with dictionaries
    Task<CmsPage> CreatePageAsync(Guid tenantId, CmsPage page);
    Task<CmsPage> UpdatePageAsync(Guid tenantId, CmsPage page);
}
```

## Service Interface
```csharp
public interface ILayoutService
{
    // All queries include languageCode
    Task<CmsPageDto?> GetPageByIdAsync(Guid tenantId, Guid pageId, string languageCode);
    Task<List<CmsPageDto>> GetPagesByTenantAsync(Guid tenantId, string languageCode);
    
    // DTOs are returned with localized values
}
```

## HTTP Endpoint Example
```
GET /api/pages/{pageId}?lang=de
GET /api/pages/{pageId}?lang=en
GET /api/pages/{pageId}  // defaults to 'en'
```

## Migration Path
1. Phase 1: Create DTOs with localization helpers ✓
2. Phase 2: Update repository queries to accept languageCode
3. Phase 3: Update service layer to localize DTOs
4. Phase 4: Update controllers with language parameter
5. Phase 5: Remove old methods without language parameter
