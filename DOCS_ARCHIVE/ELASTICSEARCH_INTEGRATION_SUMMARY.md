# Elasticsearch Integration - Quick Reference

**Zusammenfassung:** Das StoreFront ermittelt Katalogdaten aus Elasticsearch. Admin-Änderungen triggern automatische Index-Updates.

## Workflow-Übersicht

```
┌──────────────────────────────────────────────────────────────┐
│ 1. ADMIN AKTION                                               │
│ • Produkt erstellen/bearbeiten/löschen                       │
│ • URL: /admin/shop/products                                  │
└────────────────┬─────────────────────────────────────────────┘
                 │
┌────────────────▼─────────────────────────────────────────────┐
│ 2. API REQUEST                                                │
│ • POST/PUT/DELETE /api/admin/shop/products                   │
│ • Authentication & Validation                                │
│ • Latenz: <100ms                                             │
└────────────────┬─────────────────────────────────────────────┘
                 │
┌────────────────▼─────────────────────────────────────────────┐
│ 3. DATENBANK SPEICHERN                                        │
│ • PostgreSQL Catalog Database                                │
│ • ACID Transaktionen                                         │
│ • Latenz: <50ms                                              │
└────────────────┬─────────────────────────────────────────────┘
                 │
┌────────────────▼─────────────────────────────────────────────┐
│ 4. EVENT PUBLISHING                                           │
│ • Domain Event (ProductCreated/Updated/Deleted)              │
│ • RabbitMQ Channel: product.*                                │
│ • Latenz: <50ms                                              │
└────────────────┬─────────────────────────────────────────────┘
                 │
┌────────────────▼─────────────────────────────────────────────┐
│ 5. SEARCH INDEX SERVICE                                       │
│ • Event Consumer (RabbitMQ Listener)                         │
│ • Mappt zu Index-Dokument                                    │
│ • Latenz: 1-2 Sekunden                                       │
└────────────────┬─────────────────────────────────────────────┘
                 │
┌────────────────▼─────────────────────────────────────────────┐
│ 6. ELASTICSEARCH UPDATE                                       │
│ • Index/Update/Delete Document                               │
│ • Refresh Index                                              │
│ • Latenz: <500ms                                             │
└────────────────┬─────────────────────────────────────────────┘
                 │
┌────────────────▼─────────────────────────────────────────────┐
│ 7. STOREFRONT VERFÜGBAR                                       │
│ • Produkt is searchable                                      │
│ • Total Latenz: 2-3 Sekunden                                 │
│ • StoreFront kann sofort zugreifen                           │
└──────────────────────────────────────────────────────────────┘
```

## Operation Types & Index Updates

| Operation | Admin Action | Event Type | Index Action | Latenz |
|-----------|-------------|-----------|-------------|--------|
| **Create** | POST /products | `product.created` | `POST /_doc/{id}` | 2-3s |
| **Update** | PUT /products/{id} | `product.updated` | `POST /_update/{id}` | 2-3s |
| **Delete** | DELETE /products/{id} | `product.deleted` | `DELETE /_doc/{id}` | 2-3s |
| **Bulk** | POST /bulk-import | `products.bulk-imported` | `POST /_bulk` | 5-10s |

## StoreFront Search Queries

### Example 1: Simple Text Search
```
GET /catalog/products/search?q=wireless
Response: All products mit "wireless" im Name/Description
Quelle: Elasticsearch multi_match query
```

### Example 2: Filtered Search
```
GET /catalog/products/search?
  q=headphones&
  category=Electronics&
  brand=TechBrand&
  minPrice=50&
  maxPrice=150&
  inStock=true

Response: Gefilterte Resultate mit Facets
```

### Example 3: Autocomplete
```
GET /catalog/products/suggestions?q=wire
Response: ["Wireless Headphones", "Wireless Keyboard", ...]
```

## Data Flow: Beispiel Produkt hinzufügen

```
Admin: Fügt "Wireless Headphones" hinzu
  │
  └─> POST /api/admin/shop/products
        {
          "name": "Wireless Headphones",
          "sku": "WH-001",
          "price": 99.99,
          "category": "Electronics"
        }
        │
        └─> Catalog Service speichert in PostgreSQL
              │
              └─> ProductCreated Event publiziert
                    │
                    └─> RabbitMQ routing: product.created
                          │
                          └─> Search Index Service konsumiert
                                │
                                └─> Elasticsearch indexiert
                                      │
                                      └─> GET /catalog/products/search?q=wireless
                                            │
                                            └─> ✅ Produkt gefunden!
```

## Technische Details

### Index Mapping
- **Type:** Full-text search optimized
- **Analyzer:** Standard (mit Stemming)
- **Tokenizer:** Standard
- **Fields:** name (boosted 3x), description (1.5x), tags

### Performance
- **Query Latency:** <100ms (p95)
- **Indexing Speed:** ~1000 docs/sec
- **Cache:** Redis (5 min TTL)
- **Shards:** 3 (Production)
- **Replicas:** 2 (High Availability)

### Event Handling
- **Queue:** RabbitMQ
- **Retry:** Exponential Backoff (3 attempts)
- **Dead Letter Queue:** Bei persistent Errors
- **Monitoring:** Prometheus Metrics

## Admin-Operationen & Index-Updates

### Scenario 1: Einzelnes Produkt erstellen
```
Time: 0ms    → Admin klickt "Create"
Time: 100ms  → API speichert in DB
Time: 150ms  → Event publiziert
Time: 1000ms → Search Service verarbeitet Event
Time: 1500ms → Elasticsearch Update
Time: 2000ms → Produkt in StoreFront suchbar
```

### Scenario 2: Produkt aktualisieren
```
Time: 0ms    → Admin ändert Preis
Time: 100ms  → API updated DB
Time: 150ms  → ProductUpdated Event
Time: 1000ms → Search Service
Time: 1500ms → Elasticsearch partial update
Time: 2000ms → Neue Preis im StoreFront sichtbar
```

### Scenario 3: Bulk-Import (1000 Produkte)
```
Time: 0ms     → Admin startet Import
Time: 500ms   → Alle 1000 in PostgreSQL
Time: 600ms   → ProductsBulkImported Event
Time: 2000ms  → Search Service (parallel indexing)
Time: 5000ms  → Elasticsearch Bulk Indexing done
Time: 5100ms  → Alle 1000 Produkte im StoreFront suchbar
```

## Fehlerbehandlung

### Problem: Index-Update schlägt fehl
```
1. Search Index Service bekommt Error
2. Message geht zu Dead Letter Queue
3. Admin Alert wird gesendet
4. Fallback: Manual Index Rebuild möglich
5. Retry mit exponential backoff: 1s, 10s, 60s
```

### Problem: Index out-of-sync
```
POST /api/admin/system/rebuild-search-index
→ Trigger Full Index Rebuild
→ Zero-Downtime durch Aliases
→ Alte Index wird gelöscht
```

## Monitoring

### Health Check
```bash
GET /elasticsearch/_cluster/health/b2connect-products
Response: {
  "status": "green",
  "number_of_nodes": 3,
  "active_primary_shards": 3,
  "relocating_shards": 0
}
```

### Metrics zu überwachen
- Index Health Status
- Document Count
- Indexing Lag
- Query Latency (p50, p95, p99)
- Error Rate
- Cache Hit Rate

## Best Practices

✅ **Do's:**
- Verwende Bulk API für Batch-Operationen
- Implementiere Retry-Logik
- Monitore kontinuierlich
- Setze Query Timeouts
- Nutze Caching

❌ **Don'ts:**
- Synchrone Indexing im kritischen Pfad
- Zu breite Text Analyzer
- Ignoriere Fehlerquoten
- Unbegrenzte Queries
- Fehlende Backups

---

## Verwandte Dokumentation

- **Shop Platform Specs:** [shop-platform-specs.md](./backend/docs/shop-platform-specs.md)
- **Elasticsearch Integration:** [elasticsearch-integration.md](./backend/docs/elasticsearch-integration.md)
- **API Specifications:** [api-specifications.md](./backend/docs/api-specifications.md)
- **Architecture:** [architecture.md](./backend/docs/architecture.md)

**Last Updated:** 25. Dezember 2025
**Version:** 1.0
