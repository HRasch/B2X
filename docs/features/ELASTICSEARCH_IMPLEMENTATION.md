# Elasticsearch Integration & Full-Text Search

Comprehensive guide to Elasticsearch integration for product search, indexing, and multilingual support.

## Overview

Elasticsearch provides fast, scalable full-text search across all catalog entities:

- **Product Search** - Search by name, description, SKU with relevance ranking
- **Multilingual Support** - Search in any configured language
- **Autocomplete** - Suggest products as users type
- **Filters** - Filter by category, brand, price range, tags
- **Aggregations** - Category counts, brand popularity, price ranges

## Architecture

```
Product Created/Updated
    ↓
Event Published (RabbitMQ)
    ↓
Search Service Listens
    ↓
Index Product in Elasticsearch
    ↓
User Searches
    ↓
Query Elasticsearch
    ↓
Return Results with Relevance Score
```

## Configuration

### Environment Variables

```bash
ELASTICSEARCH_ENABLED=true
ELASTICSEARCH_URL=http://localhost:9200
ELASTICSEARCH_INDEX_PREFIX=B2X
ELASTICSEARCH_DEFAULT_LANGUAGE=en
```

### Docker Compose

```yaml
elasticsearch:
  image: docker.elastic.co/elasticsearch/elasticsearch:8.x
  environment:
    - discovery.type=single-node
    - xpack.security.enabled=false
  ports:
    - "9200:9200"
  volumes:
    - elasticsearch_data:/usr/share/elasticsearch/data
```

## Index Mapping

### Product Index

```json
{
  "settings": {
    "number_of_shards": 1,
    "number_of_replicas": 0,
    "analysis": {
      "analyzer": {
        "multilingual": {
          "type": "standard",
          "stopwords": "_english_,_german_,_french_"
        }
      }
    }
  },
  "mappings": {
    "properties": {
      "id": { "type": "keyword" },
      "sku": { "type": "keyword" },
      "name": {
        "type": "object",
        "properties": {
          "en": {
            "type": "text",
            "analyzer": "multilingual",
            "fields": {
              "keyword": { "type": "keyword" }
            }
          },
          "de": {
            "type": "text",
            "analyzer": "multilingual"
          }
        }
      },
      "description": {
        "type": "text",
        "analyzer": "multilingual"
      },
      "price": { "type": "scaled_float", "scaling_factor": 100 },
      "stockQuantity": { "type": "integer" },
      "categoryId": { "type": "keyword" },
      "categoryName": { "type": "keyword" },
      "brandId": { "type": "keyword" },
      "brandName": { "type": "keyword" },
      "tags": { "type": "keyword" },
      "imageUrls": { "type": "keyword" },
      "isActive": { "type": "boolean" },
      "tenantId": { "type": "keyword" },
      "createdAt": { "type": "date" },
      "updatedAt": { "type": "date" },
      "relevanceScore": { "type": "float" }
    }
  }
}
```

## Indexing

### Automatic Indexing on Events

When `ProductCreatedEvent` is published:

```csharp
public class ProductSearchIndexer : IEventHandler<ProductCreatedEvent>
{
    private readonly IElasticsearchService _elasticsearchService;
    
    public async Task HandleAsync(ProductCreatedEvent @event)
    {
        var document = new ProductSearchDocument(
            Id: @event.ProductId,
            Sku: @event.Sku,
            Name: @event.Name.Values,
            Description: @event.Description.Values,
            Price: @event.Price,
            StockQuantity: @event.StockQuantity,
            Tags: @event.Tags,
            TenantId: @event.TenantId,
            CreatedAt: DateTime.UtcNow
        );
        
        await _elasticsearchService.IndexProductAsync(document, @event.TenantId);
    }
}
```

### Bulk Indexing

For initial setup or re-indexing:

```csharp
public async Task ReindexAllProductsAsync(Guid tenantId)
{
    var products = await _productRepository.GetAllAsync(tenantId);
    
    var documents = products.Select(p => new ProductSearchDocument
    {
        Id = p.Id,
        Sku = p.Sku,
        Name = p.Name.Values,
        // ... map other fields
    });
    
    await _elasticsearchService.BulkIndexAsync(documents, tenantId);
}
```

## Search Queries

### Simple Text Search

```csharp
public async Task<SearchResults> SearchProductsAsync(
    string query,
    Guid tenantId,
    int page = 1,
    int pageSize = 20)
{
    var searchRequest = new SearchRequest<ProductSearchDocument>
    {
        Query = new MultiMatchQuery
        {
            Query = query,
            Fields = new[] { "name.en^2", "name.de", "description", "sku" },
            Operator = Operator.Or,
            Type = TextQueryType.BestFields
        },
        Filter = new TermQuery { Field = "tenantId", Value = tenantId.ToString() },
        Skip = (page - 1) * pageSize,
        Size = pageSize,
        Highlight = new Highlight
        {
            Fields = new Dictionary<Field, HighlightField>
            {
                { "name.en", new HighlightField() },
                { "description", new HighlightField() }
            }
        }
    };
    
    var response = await _client.SearchAsync<ProductSearchDocument>(searchRequest);
    
    return new SearchResults
    {
        Total = response.Total,
        Products = response.Documents.Select(d => new SearchResult
        {
            Id = d.Id,
            Name = d.Name,
            RelevanceScore = response.Hits.First(h => h.Id == d.Id.ToString()).Score ?? 0,
            Highlights = response.Hits.First(h => h.Id == d.Id.ToString()).Highlights
        }).ToList(),
        Page = page,
        PageSize = pageSize
    };
}
```

### Filtered Search

```csharp
public async Task<SearchResults> SearchWithFiltersAsync(
    string query,
    ProductFilter filter,
    Guid tenantId)
{
    var boolQuery = new BoolQuery();
    
    // Base query
    if (!string.IsNullOrEmpty(query))
    {
        boolQuery.Must.Add(new MultiMatchQuery
        {
            Query = query,
            Fields = new[] { "name.en", "description" }
        });
    }
    
    // Category filter
    if (filter.CategoryId.HasValue)
    {
        boolQuery.Filter.Add(new TermQuery
        {
            Field = "categoryId",
            Value = filter.CategoryId.Value.ToString()
        });
    }
    
    // Price range
    if (filter.MinPrice.HasValue || filter.MaxPrice.HasValue)
    {
        boolQuery.Filter.Add(new RangeQuery
        {
            Field = "price",
            GreaterThanOrEqualTo = filter.MinPrice,
            LessThanOrEqualTo = filter.MaxPrice
        });
    }
    
    // Brand filter
    if (filter.BrandIds?.Any() == true)
    {
        boolQuery.Filter.Add(new TermsQuery
        {
            Field = "brandId",
            Terms = filter.BrandIds.Cast<object>().ToList()
        });
    }
    
    // Stock availability
    if (filter.InStockOnly)
    {
        boolQuery.Filter.Add(new RangeQuery
        {
            Field = "stockQuantity",
            GreaterThan = 0
        });
    }
    
    boolQuery.Filter.Add(new TermQuery
    {
        Field = "tenantId",
        Value = tenantId.ToString()
    });
    
    var searchRequest = new SearchRequest<ProductSearchDocument>
    {
        Query = boolQuery,
        Size = 20
    };
    
    return await _client.SearchAsync<ProductSearchDocument>(searchRequest);
}
```

### Autocomplete/Suggestions

```csharp
public async Task<List<string>> GetSuggestionsAsync(
    string prefix,
    Guid tenantId)
{
    var request = new SearchRequest<ProductSearchDocument>
    {
        Query = new BoolQuery
        {
            Must = new List<QueryContainer>
            {
                new MatchQuery
                {
                    Field = "name.en",
                    Query = prefix,
                    Operator = Operator.And
                }
            },
            Filter = new List<QueryContainer>
            {
                new TermQuery { Field = "tenantId", Value = tenantId.ToString() }
            }
        },
        Size = 10,
        Source = new SourceFilter { Includes = new[] { "name" } }
    };
    
    var response = await _client.SearchAsync<ProductSearchDocument>(request);
    
    return response.Documents
        .Select(d => d.Name.Values.FirstOrDefault().Value)
        .Distinct()
        .ToList();
}
```

## Frontend Integration

### Search Component (Vue)

```typescript
// frontend-admin/src/services/api/catalog.ts
export const searchProducts = async (
  query: string,
  filters?: ProductFilter,
  tenantId?: string
): Promise<SearchResults> => {
  const response = await api.get<SearchResults>('/api/v1/products/search', {
    params: {
      q: query,
      ...filters,
      tenantId
    }
  });
  return response.data;
};
```

```vue
<template>
  <div class="search-container">
    <input
      v-model="searchQuery"
      type="text"
      placeholder="Search products..."
      @input="handleSearch"
    />
    
    <div v-if="results.length" class="results">
      <div
        v-for="product in results"
        :key="product.id"
        class="result-item"
      >
        <h3>{{ product.name }}</h3>
        <p class="relevance">Score: {{ product.relevanceScore }}</p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { searchProducts } from '@/services/api/catalog';

const searchQuery = ref('');
const results = ref([]);

const handleSearch = async () => {
  if (!searchQuery.value.trim()) {
    results.value = [];
    return;
  }
  
  const data = await searchProducts(searchQuery.value);
  results.value = data.products;
};
</script>
```

## Multilingual Search

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
    var fieldName = $"name.{language}";
    
    var searchRequest = new SearchRequest<ProductSearchDocument>
    {
        Query = new MatchQuery
        {
            Field = fieldName,
            Query = query,
            Analyzer = $"{language}_analyzer"
        },
        Filter = new TermQuery { Field = "tenantId", Value = tenantId.ToString() }
    };
    
    return await _client.SearchAsync<ProductSearchDocument>(searchRequest);
}
```

## Monitoring & Maintenance

### Check Index Health

```bash
curl -X GET "localhost:9200/B2X-products/_stats"
```

### Reindex with New Mapping

```csharp
public async Task ReindexWithNewMappingAsync(Guid tenantId)
{
    // Create new index with updated mapping
    var newIndexName = $"B2X-products-{DateTime.Now:yyyyMMdd}";
    await _client.CreateIndexAsync(newIndexName);
    
    // Copy data from old index
    var reindexRequest = new ReindexOnServerRequest
    {
        Source = new ReindexSource
        {
            Index = "B2X-products"
        },
        Destination = new ReindexDestination
        {
            Index = newIndexName
        }
    };
    
    await _client.ReindexAsync(reindexRequest);
    
    // Switch alias
    await _client.AliasAsync(a => a
        .Remove(r => r.Index("B2X-products").Alias("products"))
        .Add(add => add.Index(newIndexName).Alias("products"))
    );
}
```

## Best Practices

**DO:**
- Keep Elasticsearch in sync with database (via events)
- Use multilingual analyzers for language-specific search
- Monitor index size and performance
- Reindex periodically for optimal performance
- Test search queries with real data

**DON'T:**
- Query database directly for search (use Elasticsearch)
- Index sensitive customer data
- Forget to handle Elasticsearch downtime gracefully
- Use Elasticsearch as primary storage

## Troubleshooting

### Products not appearing in search
- Check `ELASTICSEARCH_ENABLED=true`
- Verify Elasticsearch is running: `curl localhost:9200`
- Check index exists: `curl localhost:9200/_cat/indices`
- Run reindex: `POST /api/v1/catalog/reindex`

### Slow search queries
- Check index size: `GET /B2X-products/_stats`
- Consider adding more shards
- Profile queries: `GET /B2X-products/_search/profile`

## References

- [Elasticsearch Documentation](https://www.elastic.co/guide/en/elasticsearch/reference/current/index.html)
- `.copilot-specs.md` Section 20 (Search architecture)
- `CATALOG_IMPLEMENTATION.md` (Catalog events)
- `EVENT_VALIDATION_IMPLEMENTATION.md` (Event handling)
