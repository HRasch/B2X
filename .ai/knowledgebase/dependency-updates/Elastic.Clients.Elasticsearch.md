---
title: Elastic.Clients.Elasticsearch
current_version: 9.2.2
source_files:
  - backend/Directory.Packages.props
  - Directory.Packages.props
status: reviewed
created_by: SARAH
created_at: 2025-12-30
last_updated: 2026-01-01
research_date: 2026-01-01

summary: |
  The official Elasticsearch .NET client is versioned in lockstep with Elasticsearch server versions; minor/major client versions may contain breaking changes. Repositories using the client must verify server compatibility before upgrades.

findings: |
  - NuGet current stable: `Elastic.Clients.Elasticsearch` 9.2.2 (Nov 24, 2025)
  - Latest 8.x LTS: `8.19.13` (Nov 24, 2025)
  - Release notes: https://github.com/elastic/elasticsearch-net/releases (see tags for per-release changelogs)
  - Documentation: https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/index.html
  - Important: the client major/minor parts are dictated by the Elasticsearch server version. Minor/patch releases may still introduce breaking changes — consult the breaking-changes policy: https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/breaking-changes-policy.html
  - The client delegates transport concerns to `Elastic.Transport`; verify the transitive `Elastic.Transport` version and test HTTP/serialization paths after any upgrade.
  - Observed breaking-changes highlights (representative):
    - `9.0.0` introduced container type changes and removal of certain descriptors/constructors — these can be high impact for code using generated descriptors.
    - `9.1.x` series included descriptor/aggregation usability changes (e.g., CompositeAggregation sources -> KeyValuePair) — review any code that constructs fluent aggregation descriptors.

## Versionshistorie seit 01.01.2024

### Aktuelle Version: 9.2.2 (24. November 2025)
**Neue Features:**
- Regenerierter Client mit korrigierten Endpunkten (z.B. `Security.PutUser`)
- Performance-Optimierung: Vermeidung von `HasFlag` in Hot-Paths für .NET Framework

### 9.2.1 (03. November 2025)
**Bug Fixes:**
- Fix für Deserialisierung von `SqlRow` Werten
- Regenerierter Client

### 9.2.0 (23. Oktober 2025)
**Neue Features:**
- Initiale 9.2 Release
- Neue Elasticsearch 9.2 Server Features

### 9.1.12 (23. Oktober 2025)
**Maintenance:**
- Regenerierter Client

### 9.1.11 (16. Oktober 2025)
**Neue Features:**
- Modifikation von `RequestConfiguration` für `BulkRequest` möglich
- Fix für Floating Point Serialisierung in NETFRAMEWORK Builds
- Implizite Konvertierungsoperatoren für handcrafted Descriptors
- Regenerierter Client mit Keyed Buckets für `FiltersAggregation` und `ApiFiltersAggregation`

### 9.1.10 (07. Oktober 2025)
**Neue Features:**
- `Esql.QueryAsObjects()` funktioniert jetzt mit nested Types

## Breaking Changes seit 01.01.2024

### 9.1.8 (Breaking Changes - Impact: Low)
**CompositeAggregation Usability Improvements:**
- `Sources` Property Type geändert von `ICollection<IDictionary<string, CompositeAggregationSource>>` zu `ICollection<KeyValuePair<string, CompositeAggregationSource>>`
- `CompositeAggregationSource` ist jetzt ein Container (ähnlich wie `Aggregation`, `Query`, etc.)
- Neue Syntax für Object Initializer:

```csharp
// Vor 9.1.8
new SearchRequest
{
    Aggregations = new Dictionary<string, Aggregation>
    {
        { "my_composite", new CompositeAggregation
        {
            Sources = new List<IDictionary<string, CompositeAggregationSource>>
            {
                new Dictionary<string, CompositeAggregationSource>
                {
                    { "my_terms", new CompositeAggregationSource
                    {
                        Terms = new CompositeTermsAggregation { /* ... */ }
                    }}
                }
            }
        }}
    }
};

// Ab 9.1.8
new SearchRequest
{
    Aggregations = new Dictionary<string, Aggregation>
    {
        { "my_composite", new CompositeAggregation
        {
            Sources = new List<KeyValuePair<string, CompositeAggregationSource>>
            {
                new KeyValuePair<string, CompositeAggregationSource>(
                    "my_terms",
                    new CompositeTermsAggregation { /* ... */ }
                )
            }
        }}
    }
};
```

**Fluent Syntax:**
```csharp
// Neue optimierte Fluent Syntax
.Sources(sources => sources
    .Add("my_terms", x => x.Terms(/* ... */))
    .Add("my_histo", x => x.Histogram(/* ... */))
)
```

### 9.1.1 (Breaking Changes - Impact: Low)
**Percentiles Aggregation Results:**
- `Values` Property Type geändert von `Percentiles` Union zu einfacher `PercentilesItem` Collection
- Betroffene Klassen: `HdrPercentilesAggregate`, `HdrPercentileRanksAggregate`, `TDigestPercentilesAggregate`, `TDigestPercentileRanksAggregate`, `PercentilesBucketAggregate`
- `Percentiles` Union Type entfernt

### 9.0.0 (Breaking Changes - Impact: High)
**Container Types:**
- Container Types verwenden jetzt reguläre Properties statt statischer Factory Methods
- Neue Syntax für Query und Aggregation:

```csharp
// Vor 9.0.0
Query.MatchAll(new MatchAllQuery { })

// Ab 9.0.0
new Query { MatchAll = new MatchAllQuery { } }
```

**Removal of Generic Request Descriptors:**
- Generische Versionen bestimmter Request Descriptors entfernt
- Betroffen: `AsyncSearchStatusRequestDescriptor<TDocument>`, `DeleteAsyncRequestDescriptor<TDocument>`, etc.
- Migration: Generischen Type Parameter entfernen

**Descriptor Constructors:**
- `(TDocument, IndexName, Id)` Constructor Overloads entfernt wegen Mehrdeutigkeiten
- Alternative: Explizite Konvertierung verwenden

**Date/Time/Duration Values:**
- `long`/`double` durch `DateTimeOffset`/`TimeSpan` ersetzt wo angemessen

**ExtendedBounds:**
- `ExtendedBoundsDate`/`ExtendedBoundsFloat` entfernt
- Ersetzt durch `ExtendedBounds<T>`

**Field Semantics:**
- `Field`/`Fields` werfen jetzt Exceptions statt nullable References zurückzugeben

**FieldSort:**
- Parameterloser Constructor entfernt
- Neuer Constructor: `new FieldSort(Elastic.Clients.Elasticsearch.Field field)`

**Descriptor Types:**
- Alle Descriptor Types sind jetzt `struct` statt `class`

## Neue Features seit 01.01.2024

### Performance & Usability
- **Container Type Improvements**: Besseres Pattern Matching und einfachere Inspektion
- **Fluent API Enhancements**: Optimierte Syntax für Aggregations
- **Implicit Conversions**: Automatische Konvertierung für Aggregation Types
- **Hot Path Optimizations**: Performance-Verbesserungen für .NET Framework

### API Erweiterungen
- **ESQL Support**: Verbesserte Unterstützung für Elasticsearch SQL mit nested Types
- **Bulk Request Configuration**: Modifikation von `RequestConfiguration` möglich
- **Filters Aggregation**: Keyed Buckets für `FiltersAggregation` und `ApiFiltersAggregation`

### Bug Fixes & Stability
- **SqlRow Deserialization**: Fix für SQL Row Wert-Deserialisierung
- **Floating Point Serialization**: Korrektur für .NET Framework Builds
- **Endpoint Generation**: Korrektur falsch generierter Endpunkte (z.B. Security.PutUser)

## Migrations-Guide

### Von 8.x auf 9.x
1. **Container Types aktualisieren:**
   ```csharp
   // Old
   Query.MatchAll(new MatchAllQuery())

   // New
   new Query { MatchAll = new MatchAllQuery() }
   ```

2. **Generic Descriptors korrigieren:**
   ```csharp
   // Old
   new AsyncSearchStatusRequestDescriptor<MyDocument>()

   // New
   new AsyncSearchStatusRequestDescriptor()
   ```

3. **CompositeAggregation Syntax:**
   ```csharp
   // Old
   Sources = new List<IDictionary<string, CompositeAggregationSource>> { ... }

   // New
   Sources = new List<KeyValuePair<string, CompositeAggregationSource>> { ... }
   ```

### Von 9.0.x auf 9.1.x
- Percentiles Aggregation Results: `Values` Property Type prüfen
- CompositeAggregation: Neue Fluent Syntax verwenden

### Von 9.1.x auf 9.2.x
- Keine Breaking Changes - direkte Kompatibilität
- Neue ESQL Features für nested Types nutzen

## Kompatibilitäts-Matrix

| Client Version | Elasticsearch Server | .NET Version | Status |
|----------------|----------------------|--------------|---------|
| 9.2.x | 9.2.x | .NET 6+ | Aktuell |
| 9.1.x | 9.1.x | .NET 6+ | Unterstützt |
| 9.0.x | 9.0.x | .NET 6+ | Unterstützt |
| 8.19.x | 8.19.x | .NET Standard 2.1+ | LTS |

## Empfehlungen

### Upgrade-Strategie
- **Für neue Projekte**: Direkt 9.2.x verwenden
- **Für bestehende Projekte**: Stufenweises Upgrade mit Tests
- **Produktionsumgebungen**: Staging-Tests vor Rollout

### Testing Checklist
- [ ] Container Type Syntax aktualisiert
- [ ] Aggregation Descriptors überprüft
- [ ] Integration Tests gegen Staging ES Cluster
- [ ] Performance Monitoring für 24h nach Deployment
- [ ] Rollback-Plan dokumentiert

### Monitoring
- Request/Error Rates nach Upgrade überwachen
- Such-/Index-Latenzen tracken
- Client-spezifische Logs aktivieren
  - Rollback steps documented.
---

  Migration snippets and practical notes
  ------------------------------------

  Example: per-tenant client creation (recommended pattern for this repo)

  ```csharp
  using Elastic.Clients.Elasticsearch;

  ElasticsearchClient CreateClientForTenant(Uri baseUri, string? username, string? password, string defaultIndex)
  {
      var settings = new ElasticsearchClientSettings(baseUri)
          .DefaultIndex(defaultIndex);

      if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
          settings = settings.Authentication(new BasicAuthentication(username, password));

      return new ElasticsearchClient(settings);
  }
  ```

  Example: bulk-seed with `IndexMany` (seeders / CatalogIndexer)

  ```csharp
  var client = CreateClientForTenant(uri, user, pass, indexName);
  var bulk = await client.BulkAsync(b => b.IndexMany(products, indexName));
  if (bulk.Errors)
  {
      // log and inspect per-item failures: bulk.Items
  }
  ```

  Practical ElasticService migration tips
  --------------------------------------
  - Keep the per-tenant client-caching pattern you already have, but switch the cached
    type to `ElasticsearchClient`.
  - Move index naming and tenant+language logic unchanged; only the client API calls need
    to be updated (Index/IndexMany/Bulk/Search/Get).
  - When inspecting responses, use `response.Errors` and `response.Items` for bulk
    operations; for search check `searchResponse.Hits` and `searchResponse.Total`.

  CI / integration test notes
  -------------------------
  - Use the client major that matches your ES server. For CI, pick a matrix that
    runs the integration suite against the targeted server major (for example,
    ES 8.x in staging). Use Testcontainers to provision transient test clusters.

