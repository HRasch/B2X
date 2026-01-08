# Localization & i18n Implementation

Complete guide to multi-language support with LocalizedContent, translation management, and frontend localization.

## Overview

B2X supports unlimited languages through the `LocalizedContent` pattern:

- **Flexible Language Support** - Add any language without code changes
- **Database-Backed** - Translations stored with entities
- **Frontend Localization** - i18n support in Vue components
- **Search Integration** - Elasticsearch multilingual search
- **API Responses** - Language-specific content in API responses

## Architecture

### LocalizedContent Pattern

Every user-facing text field uses LocalizedContent:

```csharp
public class LocalizedContent
{
    public Dictionary<string, string> Values { get; set; }
    // { "en": "English text", "de": "German text", "fr": "French text" }
}
```

**Used in:**
- Product Name, Description
- Category Name, Description
- Brand Name, Description
- Error Messages (localized)

### Supported Languages

Configure in database seed or configuration:

```csharp
public static class SupportedLanguages
{
    public static readonly string[] All = new[] { "en", "de", "fr", "es", "it" };
    public static readonly string Default = "en";
}
```

## Entity Models

### Product with Localization

```csharp
public class Product
{
    public Guid Id { get; set; }
    public string Sku { get; set; }
    
    [Column(TypeName = "jsonb")]
    public LocalizedContent Name { get; set; } = new();
    
    [Column(TypeName = "jsonb")]
    public LocalizedContent Description { get; set; } = new();
    
    public decimal Price { get; set; }
    // ... other properties
    
    public Guid TenantId { get; set; }
}
```

**Database Migration:**

```sql
CREATE TABLE Products (
    Id UUID PRIMARY KEY,
    Sku VARCHAR(20) NOT NULL,
    Name JSONB NOT NULL,  -- {"en": "...", "de": "..."}
    Description JSONB NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    TenantId UUID NOT NULL,
    CreatedAt TIMESTAMP NOT NULL,
    UpdatedAt TIMESTAMP NOT NULL
);

CREATE INDEX idx_products_tenant ON Products(TenantId);
CREATE INDEX idx_products_sku ON Products(Sku, TenantId);
```

## Backend Localization

### Creating Localized Entities

```csharp
[HttpPost]
[ValidateModel]
public async Task<ActionResult<ProductDto>> CreateProduct(
    [FromBody] CreateProductRequest request)
{
    var product = new Product
    {
        Sku = request.Sku,
        Name = new LocalizedContent
        {
            Values = new Dictionary<string, string>
            {
                { "en", request.Name["en"] },
                { "de", request.Name["de"] },
                { "fr", request.Name.Get("fr", "")  }  // Optional
            }
        },
        Description = new LocalizedContent
        {
            Values = request.Description  // Already a dict
        },
        Price = request.Price,
        TenantId = _tenantContext.TenantId
    };
    
    await _context.Products.AddAsync(product);
    await _context.SaveChangesAsync();
    
    // Publish event (includes all language versions)
    await _eventPublisher.PublishAsync(new ProductCreatedEvent(...));
    
    return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
}
```

### Querying by Language

```csharp
public async Task<ProductDto> GetProductAsync(Guid id, string language = "en")
{
    var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
    
    return new ProductDto(
        Id: product.Id,
        Sku: product.Sku,
        Name: product.Name.Values.ContainsKey(language)
            ? product.Name.Values[language]
            : product.Name.Values.First().Value,
        // ... map other fields
    );
}
```

### Fallback Language Logic

```csharp
public static class LocalizedContentExtensions
{
    public static string GetValue(
        this LocalizedContent content,
        string language,
        string fallbackLanguage = "en")
    {
        if (content?.Values.TryGetValue(language, out var value) == true)
            return value;
        
        if (content?.Values.TryGetValue(fallbackLanguage, out var fallback) == true)
            return fallback;
        
        return content?.Values.Values.FirstOrDefault() ?? string.Empty;
    }
}
```

## Frontend Localization

### i18n Setup (Vue 3)

```typescript
// frontend-admin/src/i18n.ts
import { createI18n } from 'vue-i18n';

const messages = {
  en: {
    catalog: {
      products: 'Products',
      categories: 'Categories',
      brands: 'Brands',
      create: 'Create New',
      edit: 'Edit',
      delete: 'Delete',
      search: 'Search products...'
    },
    errors: {
      validation: 'Validation failed',
      notFound: 'Resource not found',
      unauthorized: 'Access denied'
    }
  },
  de: {
    catalog: {
      products: 'Produkte',
      categories: 'Kategorien',
      brands: 'Marken',
      create: 'Neu erstellen',
      edit: 'Bearbeiten',
      delete: 'Löschen',
      search: 'Produkte suchen...'
    },
    errors: {
      validation: 'Validierung fehlgeschlagen',
      notFound: 'Ressource nicht gefunden',
      unauthorized: 'Zugriff verweigert'
    }
  }
};

export default createI18n({
  legacy: false,
  locale: localStorage.getItem('language') || 'en',
  fallbackLocale: 'en',
  messages
});
```

### Using i18n in Components

```vue
<template>
  <div class="products-container">
    <h1>{{ $t('catalog.products') }}</h1>
    <button class="btn">{{ $t('catalog.create') }}</button>
    
    <input :placeholder="$t('catalog.search')" />
  </div>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n';

const { t, locale } = useI18n();

// Change language
const changeLanguage = (lang: string) => {
  locale.value = lang;
  localStorage.setItem('language', lang);
};
</script>
```

### Multi-Language Input Component

For managing LocalizedContent in forms:

```vue
<template>
  <div class="language-input">
    <div class="tabs">
      <button
        v-for="lang in languages"
        :key="lang"
        :class="{ active: activeLanguage === lang }"
        @click="activeLanguage = lang"
      >
        {{ lang.toUpperCase() }}
      </button>
    </div>
    
    <div class="input-group">
      <label>{{ label }} ({{ activeLanguage }})</label>
      <textarea
        :value="modelValue.values[activeLanguage] || ''"
        @input="updateValue"
        rows="4"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { LocalizedContent } from '@/types/catalog';

interface Props {
  modelValue: LocalizedContent;
  label: string;
  languages: string[];
}

const props = defineProps<Props>();
const emit = defineEmits<{ 'update:modelValue': [LocalizedContent] }>();

const activeLanguage = ref(props.languages[0]);

const updateValue = (event: Event) => {
  const target = event.target as HTMLTextAreaElement;
  const updated = {
    values: {
      ...props.modelValue.values,
      [activeLanguage.value]: target.value
    }
  };
  emit('update:modelValue', updated);
};
</script>
```

## Elasticsearch Multilingual Search

### Language-Specific Analyzers

```json
{
  "settings": {
    "analysis": {
      "analyzer": {
        "english_analyzer": {
          "type": "standard",
          "stopwords": "_english_"
        },
        "german_analyzer": {
          "type": "standard",
          "stopwords": "_german_"
        },
        "french_analyzer": {
          "type": "standard",
          "stopwords": "_french_"
        }
      }
    }
  },
  "mappings": {
    "properties": {
      "name": {
        "type": "object",
        "properties": {
          "en": {
            "type": "text",
            "analyzer": "english_analyzer"
          },
          "de": {
            "type": "text",
            "analyzer": "german_analyzer"
          },
          "fr": {
            "type": "text",
            "analyzer": "french_analyzer"
          }
        }
      }
    }
  }
}
```

### Search by Language

```csharp
public async Task<SearchResults> SearchByLanguageAsync(
    string query,
    string language,
    Guid tenantId)
{
    var searchRequest = new SearchRequest<ProductDocument>
    {
        Query = new BoolQuery
        {
            Must = new List<QueryContainer>
            {
                new MatchQuery
                {
                    Field = $"name.{language}",
                    Query = query,
                    Analyzer = $"{language}_analyzer"
                }
            },
            Filter = new List<QueryContainer>
            {
                new TermQuery { Field = "tenantId", Value = tenantId.ToString() }
            }
        }
    };
    
    return await _client.SearchAsync<ProductDocument>(searchRequest);
}
```

## Translation Management

### Export/Import Translations

```csharp
// Export all translations to JSON
public async Task<Stream> ExportTranslationsAsync(Guid tenantId)
{
    var products = await _context.Products
        .Where(p => p.TenantId == tenantId)
        .ToListAsync();
    
    var translations = new
    {
        products = products.Select(p => new
        {
            id = p.Id,
            sku = p.Sku,
            name = p.Name.Values,
            description = p.Description.Values
        })
    };
    
    var json = JsonSerializer.Serialize(translations, new JsonSerializerOptions { WriteIndented = true });
    return new MemoryStream(Encoding.UTF8.GetBytes(json));
}

// Import translations from JSON
public async Task ImportTranslationsAsync(Guid tenantId, Stream jsonStream)
{
    using var reader = new StreamReader(jsonStream);
    var json = await reader.ReadToEndAsync();
    var data = JsonSerializer.Deserialize<ImportData>(json);
    
    foreach (var item in data.Products)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == item.Id);
        if (product != null)
        {
            product.Name.Values = item.Name;
            product.Description.Values = item.Description;
        }
    }
    
    await _context.SaveChangesAsync();
}
```

## API Endpoints

### Get Product (Language-Specific)

```http
GET /api/v1/products/{id}?language=de
```

Returns product with German text for Name and Description.

### Get All Translations

```http
GET /api/v1/products/{id}/translations
```

Returns:
```json
{
  "name": {
    "en": "Product Name",
    "de": "Produktname",
    "fr": "Nom du produit"
  },
  "description": {
    "en": "...",
    "de": "...",
    "fr": "..."
  }
}
```

## Best Practices

**DO:**
- Always include "en" as fallback language
- Validate all language codes before saving
- Use LocalizedContent for all user-facing text
- Test search in multiple languages
- Provide language selector in UI

**DON'T:**
- Store machine-translated content
- Forget fallback language in queries
- Exceed reasonable language count (>20)
- Store HTML in LocalizedContent (use plain text)
- Change language codes after launch

## Troubleshooting

### Translations appearing as null
- Ensure all configured languages have values
- Check LocalizedContent.Values dictionary
- Use fallback language logic

### Elasticsearch multilingual search not working
- Verify analyzers are defined in index mapping
- Check language code matches index field name
- Test with `GET /index/_analyze`

## References

- `.copilot-specs.md` Section 8 (i18n guidelines)
- `CATALOG_IMPLEMENTATION.md` (LocalizedContent usage)
- `ELASTICSEARCH_IMPLEMENTATION.md` (Multilingual search)
- [Vue i18n Docs](https://vue-i18n.intlify.dev/)
