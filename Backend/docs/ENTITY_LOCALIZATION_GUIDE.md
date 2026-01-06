# Entity-basierte Übersetzungen (JSON)

Ein umfassendes System zum Speichern von Übersetzungen direkt in Entitäten als JSON.

## Übersicht

Statt Übersetzungen nur in einer zentralen `LocalizedString`-Tabelle zu speichern, können Entitäten jetzt Übersetzungen direkt als JSON-Eigenschaften speichern:

```csharp
// Statt:
var translation = await localizationService.GetStringAsync("product_name_123", "de");

// Jetzt auch:
var product = await dbContext.Products.FindAsync(productId);
string germanName = product.Name.Get("de");
```

## Core Klassen

### 1. **LocalizedContent**
Die zentrale Klasse für multi-sprachige Inhalte:

```csharp
public class LocalizedContent
{
    public Dictionary<string, string> Translations { get; set; }
    public string DefaultLanguage { get; set; } = "en";
    
    // Fluent API
    public LocalizedContent Set(string lang, string value)
    public string Get(string languageCode)
    public LocalizedContent SetMany(Dictionary<string, string> translations)
    public Dictionary<string, string> GetMany(params string[] languages)
    public bool HasTranslation(string language)
    public IEnumerable<string> GetAvailableLanguages()
}
```

### 2. **LocalizationExtensions**
Extension-Methoden für Entity-Klassen:

```csharp
// Auf Entity zugreifen
product.SetTranslation("Name", "de", "Produktname");
string name = product.GetTranslation("Name", "de");

// Alle Sprachen für eine Property abrufen
var translations = product.GetAllTranslations("Name");

// Verfügbare Sprachen ermitteln
var languages = product.GetAvailableLanguagesForProperty("Name");

// Validierung
bool hasAll = product.HasAllRequiredTranslations("en", "de", "fr");
var missing = product.GetMissingTranslations("en", "de", "fr");
```

### 3. **LocalizationJsonUtility**
Utility für JSON-Operationen:

```csharp
// Serialisierung
string json = LocalizationJsonUtility.Serialize(content);
string prettyJson = LocalizationJsonUtility.Serialize(content, pretty: true);

// Deserialisierung
var content = LocalizationJsonUtility.Deserialize(json);

// Sichere Operationen
if (LocalizationJsonUtility.TryDeserialize(json, out var result))
{
    // success
}

// JSON-Transformationen
var extracted = LocalizationJsonUtility.ExtractLanguages(json, "de", "fr");
var filled = LocalizationJsonUtility.FillMissingLanguages(json, new[] { "en", "de" }, "default");
var merged = LocalizationJsonUtility.MergeJsonStrings(json1, json2);

// Statistiken
var stats = LocalizationJsonUtility.GetStats(json);
Console.WriteLine($"Languages: {stats.TotalLanguages}, Chars: {stats.TotalCharacters}");
```

## Praktische Beispiele

### Beispiel 1: Product mit lokalisierten Feldern

```csharp
var product = new Product
{
    Sku = "PROD-001",
    Price = 99.99m,
    TenantId = tenantId,
    Status = ProductStatus.Active
};

// Name in mehreren Sprachen setzen
product.Name
    .Set("en", "Laptop Computer")
    .Set("de", "Laptop-Computer")
    .Set("fr", "Ordinateur Portable")
    .Set("es", "Ordenador Portátil");

// Description setzen
product.Description.SetMany(new Dictionary<string, string>
{
    { "en", "High performance laptop with 16GB RAM" },
    { "de", "Leistungsstarker Laptop mit 16GB RAM" },
    { "fr", "Ordinateur portable haute performance avec 16GB RAM" },
    { "es", "Portátil de alto rendimiento con 16GB de RAM" }
});

await dbContext.Products.AddAsync(product);
await dbContext.SaveChangesAsync();

// Später: Daten auslesen
var savedProduct = await dbContext.Products.FindAsync(productId);
string germanName = savedProduct.Name.Get("de");           // "Laptop-Computer"
string spanishDesc = savedProduct.Description.Get("es");  // "Portátil de alto rendimiento..."
```

### Beispiel 2: Tenant mit lokalisierten Beschreibungen

```csharp
var tenant = new Tenant
{
    Name = "TechCorp",
    Slug = "techcorp"
};

// Localized description
tenant.LocalizedDescription
    .Set("en", "Leading technology company")
    .Set("de", "Führendes Technologieunternehmen")
    .Set("fr", "Entreprise technologique leader");

await dbContext.Tenants.AddAsync(tenant);
await dbContext.SaveChangesAsync();
```

### Beispiel 3: ContentPage für mehrsprachige Website

```csharp
var aboutPage = new ContentPage
{
    TenantId = tenantId,
    Slug = "about-us",
    IsPublished = true,
    PublishedAt = DateTime.UtcNow
};

// Vollständige Übersetzung des gesamten Page-Inhalts
aboutPage.Title.SetMany(new Dictionary<string, string>
{
    { "en", "About Us" },
    { "de", "Über uns" },
    { "fr", "À propos de nous" },
    { "es", "Acerca de nosotros" },
    { "it", "Chi siamo" }
});

aboutPage.Description.SetMany(new Dictionary<string, string>
{
    { "en", "Learn about our company history and mission" },
    { "de", "Erfahren Sie mehr über die Unternehmensgeschichte und Mission" },
    { "fr", "Découvrez l'historique et la mission de notre entreprise" },
    { "es", "Conozca la historia y la misión de nuestra empresa" },
    { "it", "Scopri la storia e la missione della nostra azienda" }
});

aboutPage.Content.SetMany(new Dictionary<string, string>
{
    { "en", "Founded in 2020..." },
    { "de", "Gegründet im Jahr 2020..." },
    { "fr", "Fondée en 2020..." },
    { "es", "Fundada en 2020..." },
    { "it", "Fondata nel 2020..." }
});

await dbContext.ContentPages.AddAsync(aboutPage);
await dbContext.SaveChangesAsync();
```

### Beispiel 4: Menu-Items mit lokalisierten Labels

```csharp
var mainMenu = new MenuItem
{
    TenantId = tenantId,
    Url = "/",
    Order = 1,
    IsVisible = true
};

mainMenu.Label.SetMany(new Dictionary<string, string>
{
    { "en", "Home" },
    { "de", "Startseite" },
    { "fr", "Accueil" },
    { "es", "Inicio" }
});

var aboutMenu = new MenuItem
{
    ParentId = mainMenu.Id,
    TenantId = tenantId,
    Url = "/about",
    Order = 2
};

aboutMenu.Label.SetMany(new Dictionary<string, string>
{
    { "en", "About" },
    { "de", "Über" },
    { "fr", "À propos" },
    { "es", "Acerca de" }
});

await dbContext.MenuItems.AddRangeAsync(mainMenu, aboutMenu);
await dbContext.SaveChangesAsync();
```

### Beispiel 5: FAQ mit vollständigen Übersetzungen

```csharp
var faq = new FaqEntry
{
    TenantId = tenantId,
    Category = "General",
    Order = 1,
    IsPublished = true
};

faq.Question.SetMany(new Dictionary<string, string>
{
    { "en", "What is your return policy?" },
    { "de", "Was ist Ihre Rückgabepolitik?" },
    { "fr", "Quelle est votre politique de retour?" },
    { "es", "¿Cuál es su política de devolución?" },
    { "it", "Qual è la vostra politica di reso?" }
});

faq.Answer.SetMany(new Dictionary<string, string>
{
    { "en", "We accept returns within 30 days..." },
    { "de", "Wir akzeptieren Rückgaben innerhalb von 30 Tagen..." },
    { "fr", "Nous acceptons les retours dans les 30 jours..." },
    { "es", "Aceptamos devoluciones dentro de 30 días..." },
    { "it", "Accettiamo resi entro 30 giorni..." }
});

await dbContext.FaqEntries.AddAsync(faq);
await dbContext.SaveChangesAsync();
```

## Datenbankschema

LocalizedContent wird als JSON in der Datenbank gespeichert:

```sql
-- Beispiel: Products Tabelle mit JSON-Spalten
CREATE TABLE Products (
    Id UUID PRIMARY KEY,
    Sku VARCHAR(50) NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    TenantId UUID NOT NULL,
    
    -- LocalizedContent als JSON
    Name JSONB NOT NULL DEFAULT '{}'::jsonb,
    Description JSONB NOT NULL DEFAULT '{}'::jsonb,
    
    Status INT NOT NULL,
    CreatedAt TIMESTAMP NOT NULL,
    UpdatedAt TIMESTAMP NOT NULL
);

-- Beispiel JSON-Struktur in der Spalte 'Name':
{
  "translations": {
    "en": "Laptop Computer",
    "de": "Laptop-Computer",
    "fr": "Ordinateur Portable",
    "es": "Ordenador Portátil"
  },
  "defaultLanguage": "en"
}
```

## EF Core Konfiguration

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Configure Product
    modelBuilder.Entity<Product>(entity =>
    {
        entity.HasKey(e => e.Id);
        
        // LocalizedContent Properties als JSON
        entity.Property(e => e.Name)
            .HasColumnType("jsonb")
            .HasConversion(
                v => v.ToJson(),
                v => LocalizedContent.FromJson(v)
            );

        entity.Property(e => e.Description)
            .HasColumnType("jsonb")
            .HasConversion(
                v => v.ToJson(),
                v => LocalizedContent.FromJson(v)
            );
    });

    // Configure ContentPage
    modelBuilder.Entity<ContentPage>(entity =>
    {
        entity.Property(e => e.Title).HasColumnType("jsonb");
        entity.Property(e => e.Description).HasColumnType("jsonb");
        entity.Property(e => e.Content).HasColumnType("jsonb");
    });

    // Configure MenuItem
    modelBuilder.Entity<MenuItem>(entity =>
    {
        entity.Property(e => e.Label).HasColumnType("jsonb");
    });

    // Configure FaqEntry
    modelBuilder.Entity<FaqEntry>(entity =>
    {
        entity.Property(e => e.Question).HasColumnType("jsonb");
        entity.Property(e => e.Answer).HasColumnType("jsonb");
    });
}
```

## Best Practices

### 1. **Validierung beim Speichern**

```csharp
public async Task<bool> SaveProductAsync(Product product)
{
    // Validiere, dass alle erforderlichen Sprachen übersetzt sind
    var requiredLanguages = new[] { "en", "de", "fr" };
    if (!product.HasAllRequiredTranslations(requiredLanguages))
    {
        var missing = product.GetMissingTranslations(requiredLanguages);
        throw new ValidationException($"Missing translations: {string.Join(", ", missing.Keys)}");
    }

    dbContext.Products.Update(product);
    await dbContext.SaveChangesAsync();
    return true;
}
```

### 2. **Standardwerte setzen**

```csharp
// Wenn nur Englisch vorhanden, nutze es als Fallback
var content = new LocalizedContent();
content.Set("en", "Default Name");
content.DefaultLanguage = "en";

// Später: Fallback auf English wenn Sprache nicht vorhanden
string germanName = content.Get("de");  // Returns English, wenn "de" nicht existiert
```

### 3. **Batch-Operationen**

```csharp
var products = new List<Product>
{
    new Product { Sku = "P001" },
    new Product { Sku = "P002" },
    new Product { Sku = "P003" }
};

foreach (var product in products)
{
    product.Name.SetMany(new Dictionary<string, string>
    {
        { "en", $"Product {product.Sku}" },
        { "de", $"Produkt {product.Sku}" }
    });
}

await dbContext.Products.AddRangeAsync(products);
await dbContext.SaveChangesAsync();
```

### 4. **Daten-Export mit Übersetzungen**

```csharp
public async Task<object> ExportProductWithTranslationsAsync(Guid productId, string[] languages)
{
    var product = await dbContext.Products.FindAsync(productId);
    
    return new
    {
        product.Id,
        product.Sku,
        product.Price,
        Name = product.Name.GetMany(languages),
        Description = product.Description.GetMany(languages),
        AvailableLanguages = product.Name.GetAvailableLanguages()
    };
}
```

## Integration mit LocalizationService

Das System integriert sich mit dem bestehenden `LocalizationService`:

```csharp
// Zentraler Service (für UI-Strings, Fehlermeldungen)
var loginButton = await localizationService.GetStringAsync("auth", "login_button", "de");

// Entity-Service (für Produkte, Seiten, etc.)
var product = await dbContext.Products.FindAsync(productId);
string name = product.Name.Get("de");  // Direkt aus Entity
```

## Vorteile

✅ **Performance**: Keine Joins nötig, alles in einer Entity
✅ **Flexibilität**: Jede Entity kann ihre eigene Übersetzungslogik haben
✅ **Wartbarkeit**: Übersetzungen bleiben mit den Daten zusammen
✅ **Skalierbarkeit**: Kein Bottleneck durch zentrale Translations-Tabelle
✅ **JSONB-Features**: Datenbankfeatures wie Index und Query auf JSON
✅ **Type-Safe**: Strongly-typed über LocalizedContent-Klasse

## Limitierungen und Workarounds

| Problem | Lösung |
|---------|--------|
| Keine globalen Übersetzungsupdates | Nutze LocalizationService für System-Strings, LocalizedContent für Entity-Daten |
| Schwierig searchbar | Erstelle denormalisierte Search-Spalten oder verwende PostgreSQL Full-Text Search auf JSON |
| Keine zentrale Versionskontrolle | Nutze Entity's CreatedAt/UpdatedAt, dazu Audit-Logging |
| Large JSON kann Query langsam machen | Begrenzen Sie Übersetzungsgröße, nutzen Sie Pagination |

## Zusammenfassung

Das System bietet:

1. **LocalizedContent** - Core-Klasse für multi-sprachige Inhalte
2. **LocalizationExtensions** - Einfache API zum Arbeiten mit Übersetzungen in Entities
3. **LocalizationJsonUtility** - Fortgeschrittene JSON-Operationen
4. **Vorbereitete Entities** - Product, ContentPage, MenuItem, FaqEntry, Feature
5. **Flexible Integration** - Funktioniert mit oder ohne zentrale LocalizationService

Nutzen Sie diese Lösung für Anwendungen, die viele lokalisierte Entitäten benötigen!
