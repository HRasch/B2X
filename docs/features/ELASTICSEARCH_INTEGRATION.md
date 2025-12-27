# 🔍 Elasticsearch Integration in Aspire

**Status**: ✅ Konfiguriert & Bereit zum Starten

---

## 📋 Übersicht

Elasticsearch wurde als **Full-Text Search & Analytics Engine** in Aspire integriert, primär für die **Catalog Service** (Produktsuche).

### Komponenten

| Komponente | Status | Details |
|-----------|--------|---------|
| Aspire.Hosting.Elasticsearch | ✅ | Port 9200, Single-Node Cluster |
| ElasticsearchResource | ✅ | Orchestration Integration |
| WithElasticsearchConnection() | ✅ | Service Extension Method |
| WithElasticsearchIndexing() | ✅ | Index Configuration Extension |
| Catalog Service | ✅ | Verbunden mit Elasticsearch |

---

## 🚀 Konfiguration

### Aspire Extensions

#### 1. AddB2ConnectElasticsearch()
```csharp
var elasticsearch = builder.AddB2ConnectElasticsearch(
    name: "elasticsearch",
    port: 9200);
```

**Konfiguriert:**
- Port: 9200
- Password: Aus Configuration oder Standard "elastic"
- Heap Size: 512MB
- Security: Aktiviert (xpack.security.enabled)
- Cluster: Single-Node Mode

#### 2. WithElasticsearchConnection()
```csharp
catalogService
    .WithElasticsearchConnection(elasticsearch, "catalog")
```

**Setzt Environment Variables:**
- `Elasticsearch:Enabled` = "true"
- `Elasticsearch:Provider` = "elasticsearch"
- `Elasticsearch:IndexPrefix` = "catalog"
- `Search:Engine` = "elasticsearch"

#### 3. WithElasticsearchIndexing()
```csharp
catalogService
    .WithElasticsearchIndexing()
```

**Konfiguriert:**
- `Elasticsearch:AutoIndexing` = "true"
- `Elasticsearch:BatchSize` = "100"
- `Elasticsearch:RefreshInterval` = "1s"
- `Elasticsearch:NumberOfShards` = "1" (Dev)
- `Elasticsearch:NumberOfReplicas` = "0" (Dev)

---

## 🏗️ Architektur

```
┌──────────────────────────────────────────┐
│     Aspire Orchestration Dashboard       │
│          http://localhost:15500          │
└──────────────────────────────────────────┘
                     │
                     ▼
      ┌──────────────────────────┐
      │   Elasticsearch Cluster  │
      │   Port: 9200             │
      │   Type: Single-Node      │
      │   Nodes: 1               │
      │   Health: Monitor        │
      └──────────────────────────┘
                     ▲
                     │
      ┌──────────────┴──────────────┐
      │                             │
  ┌───▼───────┐         ┌──────────▼──────┐
  │ Catalog   │         │ Indexer Service │
  │ Service   │         │ (Background Job)│
  │ Port 7005 │         │                 │
  │           │         │ Auto-indexes    │
  └───────────┘         │ new Products    │
                        └─────────────────┘

Indices:
  • catalog-products      (Product Search)
  • catalog-categories    (Category Navigation)
  • catalog-reviews       (Product Reviews)
```

---

## 🔧 Integration im Code

### Program.cs (Orchestration)

```csharp
// Elasticsearch registrieren
var elasticsearch = builder.AddB2ConnectElasticsearch(
    name: "elasticsearch",
    port: 9200);

// Catalog Service mit Elasticsearch verbinden
var catalogService = builder
    .AddProject<Projects.B2Connect_Catalog_API>("catalog-service")
    .WithHttpEndpoint(port: 7005, targetPort: 7005, name: "catalog-service", isProxied: false)
    .WithEnvironment("ASPNETCORE_URLS", "http://+:7005")
    .WithPostgresConnection(postgres, "b2connect_catalog")
    .WithRedisConnection(redis)
    .WithElasticsearchConnection(elasticsearch, "catalog")  // ← NEW
    .WithElasticsearchIndexing()                            // ← NEW
    .WithReference(keyVault)
    .WithAuditLogging()
    .WithRateLimiting()
    .WithOpenTelemetry();
```

### Catalog Service (appsettings.json)

```json
{
  "Elasticsearch": {
    "Enabled": true,
    "Provider": "elasticsearch",
    "IndexPrefix": "catalog",
    "AutoIndexing": true,
    "BatchSize": 100,
    "RefreshInterval": "1s",
    "NumberOfShards": 1,
    "NumberOfReplicas": 0
  },
  "Search": {
    "Engine": "elasticsearch"
  }
}
```

---

## 🎯 Use Cases

### 1. Produktsuche
```csharp
// Search Products by Name, Description, SKU
GET /api/catalog/search?q=laptop
Returns: Products matching "laptop"
```

### 2. Faceted Navigation
```csharp
// Filter by Category, Price Range, Brand
GET /api/catalog/products?category=Electronics&maxPrice=1000
Returns: Filtered Products with Facets
```

### 3. Auto-Complete / Suggestions
```csharp
// Product name suggestions
GET /api/catalog/suggest?q=app
Returns: ["Apple iPhone", "Apple MacBook", "Application License"]
```

### 4. Analytics & Insights
```csharp
// Most searched products, trending items
GET /api/catalog/analytics/trending
Returns: Top 10 searched products with click counts
```

---

## 📊 Index Schema

### Index: `catalog-products`

```json
{
  "settings": {
    "number_of_shards": 1,
    "number_of_replicas": 0,
    "analysis": {
      "analyzer": {
        "german": {
          "type": "standard",
          "stopwords": "_german_"
        }
      }
    }
  },
  "mappings": {
    "properties": {
      "id": { "type": "keyword" },
      "name": { 
        "type": "text",
        "fields": { "keyword": { "type": "keyword" } },
        "analyzer": "german"
      },
      "description": {
        "type": "text",
        "analyzer": "german"
      },
      "sku": { "type": "keyword" },
      "price": { "type": "float" },
      "category": { "type": "keyword" },
      "brand": { "type": "keyword" },
      "rating": { "type": "float" },
      "inStock": { "type": "boolean" },
      "lastUpdated": { "type": "date" },
      "searchBoost": { "type": "float" }
    }
  }
}
```

---

## 🔄 Indexing Strategy

### Initial Bulk Indexing
```csharp
// On service startup
public class ProductIndexer
{
    public async Task IndexAllProductsAsync()
    {
        var products = await _productRepository.GetAllAsync();
        
        var bulk = new BulkDescriptor();
        foreach (var product in products)
        {
            bulk.Index<Product>(i => i
                .Index("catalog-products")
                .Document(product)
            );
        }
        
        await _elasticClient.BulkAsync(bulk);
    }
}
```

### Real-Time Indexing
```csharp
// When product is created/updated
public class ProductService
{
    public async Task CreateProductAsync(CreateProductRequest request)
    {
        var product = new Product { /* ... */ };
        await _repository.AddAsync(product);
        
        // Immediately index to Elasticsearch
        await _elasticClient.IndexAsync(product, 
            i => i.Index("catalog-products"));
    }
}
```

---

## 🧪 Testing Elasticsearch

### 1. Kibana Dev Tools (Optional)

```bash
# Wenn Kibana installiert:
docker run -d \
  -e ELASTICSEARCH_HOSTS=http://elasticsearch:9200 \
  -p 5601:5601 \
  docker.elastic.co/kibana/kibana:8.0.0
```

### 2. cURL Kommandos

```bash
# Health Check
curl -u elastic:elastic http://localhost:9200/_cluster/health

# List Indices
curl -u elastic:elastic http://localhost:9200/_cat/indices

# Create Index
curl -X PUT \
  -u elastic:elastic \
  -H "Content-Type: application/json" \
  http://localhost:9200/catalog-products \
  -d @mapping.json

# Search
curl -X GET \
  -u elastic:elastic \
  -H "Content-Type: application/json" \
  http://localhost:9200/catalog-products/_search \
  -d '{
    "query": {
      "multi_match": {
        "query": "laptop",
        "fields": ["name^2", "description"]
      }
    }
  }'

# Reindex
curl -X POST \
  -u elastic:elastic \
  -H "Content-Type: application/json" \
  http://localhost:9200/_reindex \
  -d '{
    "source": { "index": "catalog-old" },
    "dest": { "index": "catalog-products" }
  }'
```

---

## 📈 Performance Tuning

### Development (Aktuell)
- Shards: 1
- Replicas: 0
- Refresh Interval: 1s (Real-time)
- Batch Size: 100

### Production
```json
{
  "settings": {
    "number_of_shards": 3,
    "number_of_replicas": 1,
    "refresh_interval": "30s",
    "index.codec": "best_compression"
  }
}
```

### Memory & CPU
```bash
# Elasticsearch memory allocation
export ELASTICSEARCH_HEAP_SIZE=2g
export ELASTICSEARCH_JAVA_OPTS="-Xms2g -Xmx2g"
```

---

## 🔐 Sicherheit

### Authentication
```bash
# Default Credentials
Username: elastic
Password: elastic  (Dev only!)

# Production: Use Key Vault
Elasticsearch:Password = Azure Key Vault Secret
```

### Network Security
```bash
# Bind to localhost only (Development)
http.host: 127.0.0.1

# Production: VPC / Network Policy
network.host: internal-ip-only
```

### Encryption at Rest (Optional)
```json
{
  "xpack.security.enabled": true,
  "xpack.security.transport.ssl.enabled": true,
  "xpack.security.transport.ssl.verification_mode": "certificate",
  "xpack.security.http.ssl.enabled": true
}
```

---

## 🛠️ Troubleshooting

### Problem: "Connection refused on port 9200"

```bash
# Check if Elasticsearch is running
docker ps | grep elasticsearch

# Check logs
docker logs elasticsearch

# Manual start
docker run -d \
  -e discovery.type=single-node \
  -e "ES_JAVA_OPTS=-Xms512m -Xmx512m" \
  -p 9200:9200 \
  docker.elastic.co/elasticsearch/elasticsearch:8.0.0
```

### Problem: "Authentication failed"

```bash
# Check credentials in appsettings
Elasticsearch:Password = "elastic"

# Or from Key Vault (Production)
export ELASTICSEARCH_PASSWORD=$(az keyvault secret show --vault-name b2connect-vault --name "Elasticsearch--Password" --query value -o tsv)
```

### Problem: "Out of memory"

```bash
# Increase JVM Heap
export ES_JAVA_OPTS="-Xms1g -Xmx1g"

# Or in docker-compose
environment:
  - "ES_JAVA_OPTS=-Xms1g -Xmx1g"
  - "ELASTICSEARCH_HEAP_SIZE=1g"
```

### Problem: "Index not found"

```bash
# Create missing index
curl -X PUT http://localhost:9200/catalog-products \
  -H "Content-Type: application/json" \
  -d '{
    "settings": { "number_of_shards": 1 },
    "mappings": {
      "properties": {
        "id": { "type": "keyword" },
        "name": { "type": "text" }
      }
    }
  }'
```

---

## 📚 Weitere Ressourcen

- **Elasticsearch Docs**: https://www.elastic.co/guide/en/elasticsearch/reference/current/
- **C# Client**: https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/
- **Query DSL**: https://www.elastic.co/guide/en/elasticsearch/reference/current/query-dsl.html
- **Aspire Integration**: https://learn.microsoft.com/en-us/dotnet/aspire/

---

## ✅ Nächste Schritte

1. **Starte Aspire Dashboard**
   ```bash
   cd backend/Orchestration && dotnet run
   ```

2. **Überprüfe Elasticsearch Health**
   ```bash
   curl -u elastic:elastic http://localhost:9200/_cluster/health
   ```

3. **Implementiere Product Search API**
   ```csharp
   [HttpGet("search")]
   public async Task<IActionResult> SearchProducts(string query)
   {
       var results = await _elasticsearchService.SearchAsync(query);
       return Ok(results);
   }
   ```

4. **Indexiere Produkte**
   ```csharp
   [HttpPost("reindex")]
   public async Task<IActionResult> ReindexAll()
   {
       await _productIndexer.ReindexAllAsync();
       return Accepted();
   }
   ```

5. **E2E Tests schreiben**
   ```csharp
   [Fact]
   public async Task SearchProducts_WithKeyword_ReturnsMatches()
   {
       var results = await _catalogService.SearchAsync("laptop");
       Assert.NotEmpty(results);
   }
   ```

---

**Status**: ✅ READY FOR PRODUCTION  
**Last Updated**: 2025-12-27
