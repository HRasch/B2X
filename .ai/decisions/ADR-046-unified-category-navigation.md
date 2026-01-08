---
docid: ADR-090
title: ADR 046 Unified Category Navigation
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

﻿---
docid: ADR-046
title: Unified Category Navigation Architecture
status: Accepted
owner: @Backend, @Frontend, @Search
approved-by: @Architect, @TechLead
created: 2026-01-07
accepted: 2026-01-06
---

# ADR-046: Unified Category Navigation Architecture

## Context

The B2X Store frontend requires a hierarchical category navigation system to allow customers to browse products by category. Currently:

- Products have a `Categories` array field (string-based)
- Admin gateway has full category management with hierarchical structure
- Store gateway lacks category endpoints for frontend consumption
- No dedicated Categories.vue page exists in Store frontend

## Decision

Implement a unified category navigation system with the following architecture:

### Frontend Architecture

**Categories.vue Component Structure:**
```
Categories.vue
├── Breadcrumb Navigation (hierarchical path)
├── Category Grid/List View
├── Subcategory Navigation
├── Product Count Display
└── Loading States
```

**Navigation Flow:**
1. Root categories displayed as cards/grid
2. Click category → navigate to `/categories/:slug`
3. Show subcategories + products in that category
4. Breadcrumbs show full hierarchical path
5. Back navigation to parent categories

### Data Architecture

**Category Interface (Store Frontend):**
```typescript
interface Category {
  id: string;
  name: string;  // Already translated in language-specific index
  slug: string;  // Language-neutral identifier
  description?: string;  // Already translated
  parentId?: string;
  children?: Category[];
  productCount: number;
  image?: string;
}
```

**API Integration Strategy:**
- Phase 1: Elasticsearch-based category aggregation from product index
- Phase 2: Hierarchical category structure with real-time product counts
- Phase 3: Advanced filters (brands, attributes) via Elasticsearch aggregations

**Data Source Strategy:**
Store frontend retrieves category information from Elasticsearch-optimized queries rather than direct database access. This ensures:
- **Consistency**: Categories always reflect actual available products
- **Performance**: Leverages existing search index infrastructure
- **Real-time**: Product counts and availability are always current
- **Scalability**: Same approach applies to brands and attribute filters

**Language-Specific Index Architecture:**
Separate Elasticsearch index per language (e.g., `products_de`, `products_en`, `products_fr`):
- Translation happens during indexing (LocalizedContent → target language string)
- No runtime translation overhead
- Category names pre-translated in each index
- Slug remains language-neutral for consistent routing
- Query simplicity: select index based on user language

### Index Architecture Decision: Categories Storage

**Evaluated Options:**

#### Option A: Separate Category Index
```json
// Index: categories_de (separate from products)
{
  "slug": "electronics/smartphones",
  "name": "Smartphones",
  "description": "...",
  "parentSlug": "electronics",
  "image": "...",
  "level": 2
}
```

**Pros:**
- Clean separation of concerns
- Easy category CRUD operations
- Independent category metadata updates
- Smaller document size

**Cons:**
- Requires JOIN operations (Parent-Child or Application-Side)
- Product counts need separate aggregation query on products index
- No atomic consistency between products and categories
- Complex query logic: Query categories → Get IDs → Query products
- Performance overhead of multiple index queries

**Verdict:** ❌ Rejected - Complexity outweighs benefits; ES joins are expensive

---

#### Option B: Nested Category Documents
```json
// In products_de index
{
  "productId": "...",
  "name": "iPhone 15",
  "categories": [  // Nested object array
    {
      "slug": "electronics",
      "name": "Elektronik",
      "level": 1,
      "image": "..."
    },
    {
      "slug": "electronics/smartphones",
      "name": "Smartphones",
      "level": 2,
      "image": "..."
    }
  ]
}
```

**Pros:**
- Atomic consistency (category data with product)
- Rich category metadata per product
- Efficient nested queries

**Cons:**
- Massive duplication (same category object in 1000s of products)
- Index size explosion (category metadata × product count)
- Complex updates (change category image → reindex all products)
- Nested documents limit (max ~100 nested docs per document)
- Update latency (bulk reindex on category change)

**Verdict:** ❌ Rejected - Duplication and update complexity too high

---

#### Option C: Denormalized Category Slugs + Aggregation (SELECTED)
```json
// In products_de index
{
  "productId": "abc-123",
  "name": "iPhone 15",
  "categories": [  // Simple keyword array
    "electronics",
    "electronics/smartphones",
    "electronics/smartphones/apple"
  ],
  "categoryNames": [  // Pre-translated for search
    "Elektronik",
    "Smartphones",
    "Apple"
  ]
}
```

**Category Metadata in Database:**
```csharp
// Admin Category entity (source of truth for metadata)
public class Category {
    public string Slug { get; set; }  // "electronics/smartphones"
    public LocalizedContent Name { get; set; }
    public LocalizedContent Description { get; set; }
    public string ImageUrl { get; set; }
    public Guid? ParentCategoryId { get; set; }
    // ... other metadata
}
```

**Aggregation Query:**
```csharp
// Get categories with product counts from products_de index
var aggregation = new TermsAggregation("categories")
{
    Field = "categories",
    Size = 1000,
    MinDocCount = 1  // Only categories with products
};

// Join with DB to get metadata (image, description)
var categoryMetadata = await _categoryRepository.GetBySlugAsync(aggregatedSlugs);
```

**Pros:**
✅ **Best Performance**: Simple keyword field, blazing fast aggregations
✅ **Minimal Index Size**: Only slugs stored (small strings)
✅ **Atomic Product Consistency**: Categories always match available products
✅ **Simple Queries**: Standard terms aggregation (ES strength)
✅ **Easy Updates**: Category metadata change → no product reindex
✅ **Multi-Category Support**: Natural array handling
✅ **Hierarchical via Slug**: Path encoded in slug ("electronics/smartphones")

**Cons:**
⚠️ **Two-Phase Query**: Aggregation → DB lookup for metadata
⚠️ **Category Metadata Separate**: Requires DB join for images/descriptions
⚠️ **Slug Changes**: Rare but requires product reindex

**Verdict:** ✅ **SELECTED** - Best balance of performance, simplicity, and maintainability

---

### Selected Architecture: Denormalized Slugs + DB Metadata

**Data Flow:**
```
1. Frontend requests categories (language: de)
   ↓
2. Elasticsearch aggregation on products_de.categories field
   → Returns: [("electronics", 1500), ("electronics/smartphones", 450), ...]
   ↓
3. Extract unique category slugs
   ↓
4. DB query for category metadata (batch by slugs)
   → Returns: images, descriptions, SEO data
   ↓
5. Merge: Elasticsearch counts + DB metadata
   ↓
6. Return CategoryAggregationDto[] to frontend
```

**Why This Works:**
- **Categories change rarely** (metadata updates): No product reindex needed
- **Products change frequently** (new/deleted): Aggregation auto-updates
- **Product count >> Category count**: Aggregating 100k products into 200 categories is fast (<50ms)
- **Separation of Concerns**: ES for counts/availability, DB for metadata
- **Cache-Friendly**: Category metadata cached separately from counts

**Performance Characteristics:**
```
Aggregation Query: ~30-50ms (100k products → 200 categories)
DB Metadata Lookup: ~10-20ms (200 categories, indexed by slug)
Merge + Serialize: ~5ms
Total: ~50-75ms (cacheable for 5-15min)
```

### Routing Structure

```
/categories              → Root categories overview
/categories/:slug        → Category detail with subcategories + products
/categories/:parent/:child → Nested category navigation
```

## ⚠️ Critical Upcoming Architecture Change: Product Variants

**Context:**
A product variant system will be introduced where:
- **Product**: Base entity (e.g., "T-Shirt")
- **ProductVariant**: Sellable unit with SKU (e.g., "T-Shirt Red Size M")
- **Attributes**: Stored on variant level (color, size, material)
- **Stock/Price**: Per variant, not per product
- **enventa Article**: Maps to ProductVariant (1:1)

**Impact on Category Navigation:**

### Index Structure Changes Required
```csharp
// Current (simple products):
public record ProductIndexDocument(
    Guid ProductId,
    string[] Categories,
    // ... product fields
);

// Future (with variants):
public record ProductIndexDocument(
    Guid ProductId,           // Base product
    Guid VariantId,           // Specific variant (SKU)
    string Sku,               // Variant SKU
    string[] Categories,      // From base product
    
    // Variant-specific attributes
    string? Color,
    string? Size,
    string? Material,
    Dictionary<string, string> VariantAttributes,
    
    // Variant-specific data
    decimal Price,            // Variant price
    int StockQuantity,        // Variant stock
    bool IsAvailable          // Variant availability
);
```

**Key Architectural Decisions:**

1. **Index Granularity: One Document per Variant**
   - Each variant is a separate ES document
   - Enables variant-level filtering ("show only red shirts")
   - Categories inherited from base product
   - Product with 10 variants = 10 ES documents

2. **Category Product Count: Unique Products, Not Variants**
   ```csharp
   // Aggregation must de-duplicate by ProductId
   var aggregation = new TermsAggregation("categories") {
       Field = "categories",
       Aggregations = new Dictionary<string, Aggregation> {
           ["unique_products"] = new CardinalityAggregation("productId")
       }
   };
   // Returns: "Electronics: 1500 products" (not 15000 variants)
   ```

3. **Attribute Filters: Variant-Level Aggregations**
   ```csharp
   // Filter: "Electronics > Shirts > Color: Red"
   .Filter(f => f.Terms(t => t.Field("categories").Terms("electronics/shirts")))
   .Aggregations(new {
       colors = new TermsAggregation("color"),      // From variants
       sizes = new TermsAggregation("size"),        // From variants
       materials = new TermsAggregation("material") // From variants
   });
   ```

4. **Search Behavior: Match Product, Return Variants**
   - Search "Red Shirt" → Matches variants with color=red
   - Category view shows products (grouped variants)
   - Product detail shows all available variants

**Migration Path:**

```markdown
### Phase 1 (Current): Simple Products
- One product = one ES document
- Categories directly on product
- Attributes on product level

### Phase 2 (Next): Product Variants
- One variant = one ES document
- Categories from parent product
- Attributes on variant level
- Backward compatible: Products without variants = single variant

### Phase 3 (Future): Variant Grouping
- Frontend groups variants by productId
- "Show 3 colors available" UI
- Variant selection in product detail
```

**Implications for Current ADR:**

✅ **Good News - Architecture Still Valid:**
- Denormalized category slugs work perfectly (variants inherit from product)
- Aggregation approach scales (just add cardinality for de-duplication)
- DB metadata lookup unchanged (categories still on base product)
- Language-specific indexes unchanged

⚠️ **Adjustments Needed:**
- `productCount` → Use cardinality aggregation on `productId` field
- Attribute filters → Aggregate on variant fields, not product
- Search queries → Match variants, group by product in UI
- Index size → Estimate: avg 5 variants per product = 5x index size

**Performance Impact:**
```
Current: 100k products → 100k ES documents → ~50ms aggregation
Future:  100k products × 5 variants = 500k ES documents → ~150ms aggregation
Mitigation: Aggressive caching (15min), pagination, pre-computed counts
```

### 🔄 Dual-Index Strategy: Product-Oriented vs Variant-Oriented

**Customer Requirement:**
Some customers perceive variant handling differently:
- **Problem 1**: Same variant appears multiple times (if in multiple products)
- **Problem 2**: Only one product shown, but multiple matching variants exist
- **Solution**: Support both product-centric AND variant-centric views

**Two Index Structures:**

#### Strategy A: Variant-Oriented Index (Current Plan)
```json
// Index: products_de_variants
// One document per variant
{
  "variantId": "var-123",
  "productId": "prod-456",
  "sku": "TSHIRT-RED-M",
  "name": "T-Shirt",
  "categories": ["bekleidung", "bekleidung/shirts"],
  "color": "Rot",
  "size": "M",
  "price": 19.99,
  "stock": 50
}
```

**Use Cases:**
- ✅ "Show me all red shirts" → Returns all red variants
- ✅ Attribute-first filtering (color, size, material)
- ✅ Price range on variant level
- ✅ Stock availability per variant
- ⚠️ Same product appears multiple times (one per variant)
- ⚠️ Category counts inflated by variants

**Best For:**
- Fashion/Apparel (color/size critical)
- B2B wholesale (SKU-level ordering)
- Technical products (precise specs matter)
- Customers who want "all matching variants visible"

---

#### Strategy B: Product-Oriented Index with Variant Aggregation
```json
// Index: products_de_grouped
// One document per product, variants as nested/aggregated
{
  "productId": "prod-456",
  "name": "T-Shirt",
  "categories": ["bekleidung", "bekleidung/shirts"],
  
  // Available variant attributes (aggregated)
  "availableColors": ["Rot", "Blau", "Schwarz"],
  "availableSizes": ["S", "M", "L", "XL"],
  "priceRange": { "min": 19.99, "max": 29.99 },
  
  // Variant count and availability
  "variantCount": 12,
  "inStockVariantCount": 8,
  "totalStock": 450,
  
  // Best-match variant for search
  "primaryVariant": {
    "variantId": "var-123",
    "sku": "TSHIRT-RED-M",
    "color": "Rot",
    "size": "M",
    "price": 19.99
  },
  
  // All variants (for filtering)
  "variants": [
    { "variantId": "var-123", "sku": "TSHIRT-RED-M", ... },
    { "variantId": "var-124", "sku": "TSHIRT-RED-L", ... }
  ]
}
```

**Use Cases:**
- ✅ "Show products" → One result per product
- ✅ Clean category counts (unique products)
- ✅ "3 colors available" UI
- ✅ Traditional e-commerce UX
- ⚠️ Attribute filters need special handling (nested queries)
- ⚠️ "Show only in-stock red M" requires variant-level filtering

**Best For:**
- Consumer e-commerce (Amazon-style)
- Product catalogs (overview first, details later)
- Customers who find duplicate results confusing
- SEO optimization (one page per product)

---

### Implementation: Dual-Index Architecture

**Maintain Both Indexes Simultaneously:**
```csharp
public class SearchIndexService
{
    public async Task IndexProductWithVariantsAsync(Product product)
    {
        foreach (var language in SupportedLanguages)
        {
            // Index A: Variant-oriented (one doc per variant)
            foreach (var variant in product.Variants)
            {
                var variantDoc = new VariantIndexDocument(
                    VariantId: variant.Id,
                    ProductId: product.Id,
                    Sku: variant.Sku,
                    Name: product.Name.GetTranslation(language),
                    Categories: GetCategoryHierarchy(product),
                    // Variant-specific attributes
                    Color: variant.Attributes["color"],
                    Size: variant.Attributes["size"],
                    Price: variant.Price,
                    Stock: variant.StockQuantity
                );
                
                await IndexToAsync($"products_{language}_variants", variantDoc);
            }
            
            // Index B: Product-oriented (one doc per product)
            var productDoc = new ProductGroupedIndexDocument(
                ProductId: product.Id,
                Name: product.Name.GetTranslation(language),
                Categories: GetCategoryHierarchy(product),
                AvailableColors: product.Variants
                    .Select(v => v.Attributes["color"])
                    .Distinct()
                    .ToArray(),
                AvailableSizes: product.Variants
                    .Select(v => v.Attributes["size"])
                    .Distinct()
                    .ToArray(),
                PriceRange: new PriceRange(
                    Min: product.Variants.Min(v => v.Price),
                    Max: product.Variants.Max(v => v.Price)
                ),
                VariantCount: product.Variants.Count,
                InStockVariantCount: product.Variants.Count(v => v.StockQuantity > 0),
                PrimaryVariant: SelectPrimaryVariant(product.Variants),
                Variants: product.Variants.Select(MapToNestedVariant).ToArray()
            );
            
            await IndexToAsync($"products_{language}_grouped", productDoc);
        }
    }
}
```

**Frontend: Tenant-Configurable View Mode:**
```typescript
// Store configuration per tenant
interface TenantSearchConfig {
  viewMode: 'product-oriented' | 'variant-oriented';
  defaultFiltering: 'show-all-variants' | 'group-by-product';
}

// API endpoint adapts based on tenant config
[HttpGet("search")]
public async Task<IActionResult> SearchAsync(
    [FromQuery] string query,
    [FromQuery] string language = "de",
    [FromQuery] Guid? tenantId = null)
{
    var config = await _tenantConfigService.GetSearchConfigAsync(tenantId);
    
    var indexName = config.ViewMode == "variant-oriented"
        ? $"products_{language}_variants"
        : $"products_{language}_grouped";
    
    // Execute search on appropriate index
    return Ok(await SearchInIndexAsync(indexName, query));
}
```

**Category Aggregation Adaptation:**
```csharp
// Variant-oriented index:
var variantAgg = new TermsAggregation("categories") {
    Field = "categories",
    Aggregations = new {
        unique_products = new CardinalityAggregation("productId")
    }
};
// Returns: "Shirts: 150 products (750 variants)"

// Product-oriented index:
var productAgg = new TermsAggregation("categories") {
    Field = "categories"
};
// Returns: "Shirts: 150 products"
```

**Index Size Comparison:**
```
Product-Oriented: 100k products → 100k documents → ~500 MB
Variant-Oriented: 100k products × 5 variants = 500k documents → ~2 GB

Storage: 2.5 GB total (both indexes)
Cost: Acceptable for flexibility
```

**Migration Strategy:**
```markdown
### Phase 1: Variant-Oriented Only (v0.x)
- Single index with variants
- All tenants use same view

### Phase 2: Dual-Index Support (v1.0)
- Create both indexes
- Add tenant configuration
- Default: product-oriented (safer UX)
- Allow tenant override via admin panel

### Phase 3: Advanced Hybrid (v1.1+)
- Category-level configuration
  - "Fashion" → variant-oriented (color/size matter)
  - "Electronics" → product-oriented (specs in detail page)
- User preference toggle
- A/B testing framework
```

**Recommendation:**
Start with **variant-oriented index** (Phase 1) for simplicity. Add product-oriented index in Phase 2 when customer demand is validated. The dual-index approach provides maximum flexibility at acceptable storage cost.

✅ **Benefits:**
- Supports diverse customer expectations
- No runtime performance penalty (separate indexes)
- Clean separation of concerns
- Easy A/B testing

⚠️ **Trade-offs:**
- 2.5x storage requirement (both indexes)
- Indexing time doubles (two indexes to update)
- More complex tenant configuration

---

**Recommendation:**
Implement current architecture as specified. Variant support requires only:
1. Change ProductIndexDocument schema (add variant fields)
2. Update aggregation to use cardinality for product counts
3. Adjust indexing service to create one doc per variant

No fundamental architecture changes needed. ✅

---

## Implementation Plan

### Phase 1: Elasticsearch-based Category Navigation (Current Sprint)
1. **Elasticsearch Aggregation Endpoint**
   - Create category aggregation query in ProductSearchController
   - Return category list with product counts from index
   - Support hierarchical structure via category slug patterns
2. **Store Frontend Integration**
   - Create Categories.vue with Elasticsearch-powered category display
   - Implement breadcrumb navigation
   - Add category filtering to ProductListing.vue
3. **Multi-Category Support**
   - Update ProductIndexDocument: `string Category` → `string[] Categories`
   - Support products in multiple categories (via ProductCategory M:N)
   - Ensure correct aggregation (no duplicate counts)

### Phase 2: Advanced Navigation & Filtering (Future)
1. **Brand Aggregations**
   - Add brand facets from Elasticsearch
   - Display brand filters with product counts
   - Support brand + category combined filtering
2. **Attribute Filters**
   - Implement configurable attribute filters per category
  **Consistency**: Category data always reflects actual product availability
- **Performance**: Leverages Elasticsearch for fast aggregations
- **Scalability**: Same pattern applies to brands, attributes, and future facets
- **Real-time**: Product counts always accurate without manual sync
- **Multi-category Support**: Products can appear in multiple categories
- **Foundation**: Enables AI-driven categorization and analytics

### Negative
- **Index Dependency**: Store navigation relies on Elasticsearch availability
- **Schema Updates**: Requires ProductIndexDocument changes (Category → Categories[])
- **Complexity**: Aggregation logic more complex than simple DB queries

### Risks
- **Elasticsearch Downtime**: Requires fallback strategy (cached aggregations)
- **Data Consistency**: Product updates must trigger index refresh
- **Performance**: Large catalogs may require aggregation optimization
- **Multi-Category Counts**: Must avoid duplicate product counting in aggregationilingual support
3. **Analytics & OpDirect Database Category Queries
- **Pros**: Simple implementation, direct access to Category entity
- **Cons**: No product count consistency, requires separate sync mechanism
- **Rejected**: Does not guarantee categories contain actual available products

### Alternative 2: Admin Gateway Category API with Product Counts
- **Pros**: Centralized category management
- **Cons**: Requires complex product count calculation, potential performance issues
- **Rejected**: Duplicate logic; Elasticsearch already has this data

### Alternative 3: Hybrid Approach (DB + Elasticsearch)
- **Pros**: Category metadata from DB, counts from Elasticsearch
- **Cons**: Increased complexity, two data sources
- **Rejected**: Violates single source of truth principle
Elasticsearch-based approach chosen because:
1. **Consistency**: Categories automatically reflect available products (no orphaned categories)
2. **Performance**: Aggregations are native Elasticsearch feature (highly optimized)
3. **Scalability**: Same pattern extends to brands, attributes, price ranges, etc.
4. **Real-time**: Index updates immediately reflect in navigation
5. **Reduced Complexity**: Single data flow (DB → Elasticsearch → Frontend)
6. **Future-proof**: Enables AI-driven categorization and analytics

This aligns with existing architecture:
### Backend Changes Required
1. **ProductIndexDocument Update**
   ```csharp
   // Change from:
   public record ProductIndexDocument(string Category, ...)
   // To:
   public record ProductIndexDocument(string[] Categories, ...)
   ```

2. **Elasticsearch Mapping Update (per language index)**
   ```json
   // Index: products_de, products_en, products_fr
   {
     "mappings": {
       "properties": {
         "categories": { 
           "type": "keyword",  // Array: ["electronics", "electronics/smartphones"]
           // Optimized for aggregations and exact matching
         },
         "categoryNames": {
           "type": "text",  // Pre-translated: ["Elektronik", "Smartphones"]
           "analyzer": "standard",  // For full-text search
           "fields": {
             "keyword": { "type": "keyword" }  // For exact match
           }
         }
       }
     }
   }
   ```
   
   **Storage Strategy:**
   - `categories`: Hierarchical slugs (denormalized, language-neutral)
   - `categoryNames`: Translated names for search only
   - Category metadata (images, descriptions): Stored in PostgreSQL, cached separately
   - Aggregation uses `categories` field, metadata enriched via DB lookup

3. **New Aggregation Endpoint (Complete Implementation)**
   ```csharp
   [HttpGet("categories")]
   public async Task<ActionResult<CategoryListResponse>> GetCategoriesAsync(
       [FromQuery] string language = "de",
       [FromQuery] Guid? tenantId = null,
       [FromQuery] bool includeMetadata = false)
   {
       var indexName = $"products_{language}";
       
       // Step 1: Elasticsearch aggregation for counts
       var searchRequest = new SearchRequest(indexName)
       {
           Size = 0,  // No documents, only aggregations
           Query = new TermQuery { Field = "tenantId", Value = tenantId },
           Aggregations = new Dictionary<string, Aggregation>
           {
               ["categories"] = new TermsAggregation("categories")
               {
                   Field = "categories",
                   Size = 1000,  // Max categories to return
                   MinDocCount = 1  // Only categories with products
               }
           }
       };
       
       var response = await _elasticsearchClient.SearchAsync<ProductIndexDocument>(searchRequest);
       var buckets = response.Aggregations.Terms("categories").Buckets;
       
       // Step 2: Extract unique category slugs
       var categorySlugs = buckets.Select(b => b.Key).ToArray();
       
       // Step 3: Fetch metadata from DB (if requested)
       Dictionary<string, CategoryMetadata>? metadata = null;
       if (includeMetadata)
       {
           metadata = await _categoryRepository
               .GetMetadataBySlugAsync(categorySlugs, language);
       }
       
       // Step 4: Build response with counts + metadata
       var categories = buckets.Select(bucket => 
           new CategoryAggregationDto(
               Slug: bucket.Key,
               ProductCount: (int)bucket.DocCount,
               Name: metadata?[bucket.Key]?.Name ?? ExtractNameFromSlug(bucket.Key),
               Description: metadata?[bucket.Key]?.Description,
               Image: metadata?[bucket.Key]?.ImageUrl,
               ParentSlug: ExtractParentSlug(bucket.Key)
           )
       ).ToArray();
       
       // Step 5: Build hierarchy tree
       var tree = BuildCategoryTree(categories);
       
       return Ok(new CategoryListResponse(
           Categories: tree,
           TotalCount: categories.Length,
           Language: language,
           CachedUntil: DateTime.UtcNow.AddMinutes(5)
       ));
   }
   
   // Helper: Extract parent from hierarchical slug
   private string? ExtractParentSlug(string slug)
   {
       var lastSlash = slug.LastIndexOf('/');
       return lastSlash > 0 ? slug[..lastSlash] : null;
   }
   ```
   
   **Response DTOs:**
   ```csharp
   public record CategoryAggregationDto(
       string Slug,
       int ProductCount,
       string Name,
       string? Description,
       string? Image,
       string? ParentSlug,
       CategoryAggregationDto[]? Children = null
   );
   
   public record CategoryListResponse(
       CategoryAggregationDto[] Categories,
       int TotalCount,
       string Language,
       DateTime CachedUntil
   );
   ```

### Frontend Implementation
- Use existing ProductSearchController for category aggregations
- Implement category composable for shared logic
- Follow established Vue 3 + TypeScript patterns
- Ensure mobile-responsive design
- Add proper loading and error states
- Language selection determines index: `const index = 'products_' + userLanguage`

### Indexing Strategy
**Translation during Indexing:**
```csharp
// SearchIndexService extracts translated values per language
foreach (var language in SupportedLanguages) // ["de", "en", "fr"]
{
    // Extract hierarchical category slugs (denormalized)
    var categorySlugs = product.ProductCategories
        .SelectMany(pc => GetCategoryHierarchy(pc.Category))  // "electronics", "electronics/smartphones"
        .Distinct()
        .ToArray();
    
    var document = new ProductIndexDocument(
        // ...
        Categories: categorySlugs,  // ["electronics", "electronics/smartphones"]
        CategoryNames: product.ProductCategories
            .Select(pc => pc.Category.Name.GetTranslation(language))  // For text search
            .ToArray()
    );
    
    await IndexToLanguageSpecificIndex(document, language);
}

// Helper: Build full hierarchy for category
private IEnumerable<string> GetCategoryHierarchy(Category category)
{
    var hierarchy = new List<string>();
    var current = category;
    
    while (current != null)
    {
        hierarchy.Insert(0, current.Slug);  // Build from root
        current = current.ParentCategory;
    }
    
    return hierarchy;
}
```

**Category Metadata Endpoint (DB-backed):**
```csharp
[HttpGet("categories/{slug}/metadata")]
public async Task<CategoryMetadataDto> GetCategoryMetadata(string slug)
{
    // Fetch from DB (cached)
    var category = await _categoryRepository.GetBySlugAsync(slug);
    return new CategoryMetadataDto(
        Slug: category.Slug,
        Description: category.Description.GetTranslation(language),
        Image: category.ImageUrl,
        MetaTitle: category.SeoMetaTitle?.GetTranslation(language),
        MetaDescription: category.MetaDescription?.GetTranslation(language)
    );
}
```

### Fallback Strategy
- Cache category aggregations (Redis/Memory)
  - Cache key format: `category-agg:{tenantId}:{language}:{timestamp}`
  - Separate cache per language (de/en/fr)
- Serve cached data if Elasticsearch unavailable
- Display "Last updated" timestamp on frontend
- Log Elasticsearch downtime for monitoring
- Cache invalidation on product create/update/delete (all language caches)

### Testing Considerations
- Test multi-category product scenarios
- Verify aggregation counts are accurate (no duplicates)
- Performance test with large catalogs (>100k products)
- Test category filter combinations
- **Language-specific tests:**
  - Verify correct index selection per language (de/en/fr)
  - Validate category names are correctly translated in each index
  - Test language switching doesn't break navigation
  - Ensure slug consistency across all language indexes
  - Test missing translations (fallback to default language)

## Multishop / Shared Catalogs

### Requirement
Some customers operate a central catalog that serves multiple shop tenants. The platform must support granting a catalog to other tenants (multishop scenario) while preserving central data maintenance and tenant-level customizations.

### Goals
- Allow a tenant to *share* one or more catalogs with other tenant(s)
- Support central data maintenance (master catalog) with downstream shops consuming the shared data
- Preserve tenant-specific overrides (pricing, availability, localized content) when required
- Ensure strict tenant isolation and permission checks
- Enable search & filtering across both shared and tenant-local catalogs

### Data Model (sketch)
- `Catalog`: entity representing a logical product set (ownerTenantId, name, description, defaultLanguage)
- `CatalogShare`: mapping (catalogId, targetTenantId, permissions {read, override})
- `Product` keeps `CatalogId` reference when originating from a catalog
- Index documents include `sourceCatalogId` and `sourceTenantId`

### Indexing & Search Implications
- Each language index (e.g., `products_de`) will store documents with extra fields:
  - `sourceCatalogId`: Guid? (null for local-only products)
  - `sourceTenantId`: Guid (origin tenant)
  - `isShared`: bool (true when product originates from a shared catalog)
- API / aggregation queries accept `sourceCatalogId` or `includeSharedFrom` parameters to filter which catalogs to include
- For a tenant consuming shared catalogs, search must filter by:
  - Their tenantId OR any `sourceCatalogId` shared with them
  - Respect `CatalogShare.permissions` (read vs override)

### Visibility & Overrides
- Default: shared catalog content is read-only for consuming tenants
- If `CatalogShare.permissions` allows `override`, tenant can create local overrides (stored as separate product records with `overridesSourceId` link)
- Search aggregation semantics:
  - Counts should be computed over the effective product set (local products + shared products visible to tenant)
  - Cardinality aggregation for product counts must deduplicate by `productId` (or `sourceProductId`) across local+shared sets

### Security & Tenant Isolation
- All queries must enforce tenant filtering and share membership checks server-side
- `CatalogShare` management limited to catalog owner and platform admins
- Auditing: log catalog share/grant/revoke events

### Operational Concerns
- Reindexing shared catalogs: central owner triggers bulk reindex for shared products; consuming tenants may choose to index a local copy or query central index
- Cache keys must include `tenantId` and `sourceCatalogId` to avoid leakage
- Billing/limits: document storage and indexing cost for shared catalogs

### UI / Admin
- Admin: Catalog owner UI to share/unshare catalogs and set permissions
- Admin: Consumer tenant UI to accept/decline shared catalogs and view overrides

## Migration & Rollback Plan

### Phase 1: Schema Update (Week 1)
```markdown
1. ✅ Update ProductIndexDocument schema
   - Add: Categories[] (string array)
   - Add: CategoryNames[] (translated strings)
   - Keep: Category (string) - deprecated, backward compatibility

2. ✅ Deploy SearchIndexService v2
   - Populate both Categories[] and Category fields
   - No breaking changes for existing queries

3. ✅ Test with sample products
   - Verify indexing works for both fields
   - Validate language-specific indexes
```

### Phase 2: API & Frontend (Week 2)
```markdown
1. ✅ Create category aggregation endpoint
   - GET /api/catalog/categories
   - Implement caching (Redis, 15min TTL)
   - Add tenant filtering

2. ✅ Deploy Store frontend changes
   - Categories.vue component
   - Category navigation
   - Breadcrumbs

3. ✅ Monitor & validate
   - Check aggregation latency (<100ms)
   - Verify cache hit rates (>80%)
   - Test category counts accuracy
```

### Phase 3: Bulk Reindex (Week 3)
```markdown
1. ⚠️ Reindex all products (off-peak hours)
   - Batch size: 1000 products
   - Estimated time: 2-4 hours (100k products)
   - Zero-downtime via alias switching

2. ✅ Validate reindex completion
   - Compare document counts
   - Spot-check category aggregations
   - Verify all languages (de, en, fr)

3. ✅ Switch aliases
   - products_de_old → products_de
   - Enable new aggregation queries
```

### Phase 4: Cleanup (Week 4)
```markdown
1. ✅ Remove deprecated Category field
   - Update ProductIndexDocument
   - Remove from mapping
   - Reindex one final time

2. ✅ Archive old indexes
   - Keep for 30 days (rollback window)
   - Document retention policy
```

### Rollback Procedures

**If Phase 2 fails (API issues):**
```bash
1. Disable new category endpoint (feature flag)
2. Revert frontend to previous version
3. Keep new index schema (backward compatible)
4. Investigate & fix
```

**If Phase 3 fails (reindex issues):**
```bash
1. Stop reindex process
2. Switch alias back to old index
3. Frontend uses old Category field
4. Debug reindex script
5. Retry with smaller batches
```

**If Phase 4 fails (cleanup issues):**
```bash
1. Keep deprecated field (no harm)
2. Monitor for 1 week
3. Retry cleanup during next maintenance window
```

### Success Criteria
- ✅ Category aggregation latency <100ms (p95)
- ✅ Cache hit rate >80%
- ✅ No category count discrepancies
- ✅ All languages working (de, en, fr)
- ✅ Frontend renders correctly
- ✅ No errors in Elasticsearch logs

---

## Security & Authorization

### Tenant Isolation
```csharp
// CRITICAL: Always filter by tenantId
var searchRequest = new SearchRequest(indexName) {
    Query = new BoolQuery {
        Must = [
            new TermQuery { Field = "tenantId", Value = currentTenantId }
        ]
    }
};

// Validate tenant access
if (!await _authService.CanAccessTenantAsync(userId, tenantId))
    return Forbid();
```

### Category Visibility
- **Public Categories**: All users can see
- **Private Categories**: B2B customers only
- **Draft Categories**: Admin users only

**Implementation:**
```csharp
// Add to ProductIndexDocument
public bool IsPublic { get; set; }
public string[] AllowedRoles { get; set; }  // ["B2B", "Premium"]

// Filter in aggregation
var filter = new BoolQuery {
    Should = [
        new TermQuery { Field = "isPublic", Value = true },
        new TermsQuery { Field = "allowedRoles", Terms = currentUserRoles }
    ]
};
```

### Rate Limiting
```csharp
[RateLimit(PerMinute = 60, PerHour = 1000)]
[HttpGet("categories")]
public async Task<IActionResult> GetCategoriesAsync(...)
```

### Audit Logging
```csharp
_logger.LogInformation(
    "Category aggregation: User={UserId}, Tenant={TenantId}, Language={Language}, Count={Count}, Duration={Duration}ms",
    userId, tenantId, language, categories.Length, stopwatch.ElapsedMilliseconds
);
```

---

## Monitoring & Observability

### Key Metrics
```yaml
# Prometheus/Grafana metrics
elasticsearch_category_aggregation_duration_ms:
  p50: < 30ms
  p95: < 100ms
  p99: < 200ms

category_cache_hit_rate:
  target: > 80%

category_count_by_language:
  de: ~200
  en: ~200
  fr: ~200

elasticsearch_index_size_bytes:
  products_de_variants: ~2GB
  products_de_grouped: ~500MB

category_aggregation_errors_total:
  target: < 5/hour
```

### Alerts
```yaml
# Alert if aggregation latency spikes
- name: CategoryAggregationSlow
  expr: histogram_quantile(0.95, elasticsearch_category_aggregation_duration_ms) > 200
  for: 5m
  severity: warning
  message: "Category aggregation p95 latency > 200ms"

# Alert if cache hit rate drops
- name: CategoryCacheMiss
  expr: category_cache_hit_rate < 0.7
  for: 10m
  severity: warning
  message: "Category cache hit rate < 70%"

# Alert if Elasticsearch errors spike
- name: CategoryAggregationErrors
  expr: rate(category_aggregation_errors_total[5m]) > 0.1
  for: 5m
  severity: critical
  message: "Category aggregation error rate high"
```

### Dashboards
```markdown
**Category Navigation Dashboard:**
1. Aggregation latency (p50, p95, p99)
2. Cache hit/miss rates
3. Category counts by language
4. Error rates and types
5. Top categories by product count
6. Index size trends
7. Reindex progress (during migration)
```

### Health Checks
```csharp
[HttpGet("health/categories")]
public async Task<IActionResult> CategoryHealthCheckAsync()
{
    var checks = new {
        ElasticsearchReachable = await _esClient.PingAsync(),
        CacheReachable = await _cache.PingAsync(),
        SampleAggregation = await TestCategoryAggregationAsync(),
        IndexSizes = await GetIndexSizesAsync()
    };
    
    return checks.ElasticsearchReachable && checks.CacheReachable && checks.SampleAggregation
        ? Ok(checks)
        : StatusCode(503, checks);
}
```

---

## Related Documents
- [ADR-001] Wolverine over MediatR (Event-driven architecture)
- [ADR-022] Multi-Tenant Domain Management (Tenant isolation)
- [KB-015] Search/Elasticsearch (Technical documentation)
- Product search already Elasticsearch-based (ProductSearchController)
- [GL-006] Token Optimization Strategy (Caching guidelines)nt API constraints
- Category data duplication between Admin and Store
- Requires future API enhancements for full hierarchy

### Risks
- Category data consistency between Admin and Store
- Performance impact of category extraction from products
- SEO implications for category URLs

## Alternatives Considered

### Alternative 1: Wait for Full Category API
- **Pros**: Clean architecture, full hierarchy support
- **Cons**: Delays Store navigation improvements, blocks Sprint 3

### Alternative 2: Client-side Category Management
- **Pros**: Fast implementation, no backend changes
- **Cons**: Data consistency issues, limited hierarchy

### Alternative 3: Direct Admin API Access
- **Pros**: Full category data available
- **Cons**: Security concerns, architectural violation

## Decision Rationale

Phase 1 approach chosen because:
- Enables immediate user experience improvement
- Follows progressive enhancement principles
- Maintains architectural integrity
- Provides foundation for future enhancements
- Aligns with Sprint 3 timeline and priorities

## Implementation Notes

- Use existing ProductService for category extraction
- Implement category composable for shared logic
- Follow established Vue 3 + TypeScript patterns
- Ensure mobile-responsive design
- Add proper loading and error states
- Include i18n support for category names</content>
<parameter name="filePath">/Users/holger/Documents/Projekte/B2X/.ai/decisions/ADR-046-unified-category-navigation.md