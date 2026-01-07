---
title: CatalogShareRule Refactor Proposal
owner: @SARAH
---

Kurz: Konkreter Vorschlag zur Übernahme des IRuleManager-Patterns (IsVisible/Rules) in B2X für `CatalogShare`-Regeln. Enthält Interface-Spezifikation, Entity-Design, NEST-Beispiele, Migrations- und Testplan.

1) Ziel
- Einheitliche Rule-Engine (`ICatalogShareRuleManager`) die Scoped-Rules (Tenant/Customer/CustomerGroup) auflöst und eine serialisierbare `EffectiveRuleSet` liefert.

2) Wichtige Konzepte
- Rules sind stateless: Datenzugriff (z. B. Rollen, Gruppen) gehört in `RequestContext`/`PermissionService`.
- Regeln liefern ES-Filterfragmente (NEST `QueryContainer`).
- Regeln werden nach Priorität und Scope (Customer→CustomerGroup→Tenant→OwnerDefault) gemerged; `Exclude` gewinnt, `AllowOverride` optional.

3) Interfaces (C#)
```csharp
public interface ICatalogShareRuleManager
{
    Task<EffectiveRuleSet> ResolveAsync(Guid tenantId, Guid? customerId, IEnumerable<Guid> catalogShareIds, CancellationToken ct = default);
    QueryContainer BuildElasticsearchFilter(EffectiveRuleSet rules, QueryContainerDescriptor<dynamic> q);
    string ComputeFingerprint(EffectiveRuleSet rules);
}

public record EffectiveRuleSet
{
    public string[] IncludedCategorySlugs { get; init; } = Array.Empty<string>();
    public string[] ExcludedCategorySlugs { get; init; } = Array.Empty<string>();
    public Guid[] IncludedBrandIds { get; init; } = Array.Empty<Guid>();
    public Guid[] ExcludedBrandIds { get; init; } = Array.Empty<Guid>();
    public Guid[] IncludedProductIds { get; init; } = Array.Empty<Guid>();
    public Guid[] ExcludedProductIds { get; init; } = Array.Empty<Guid>();
    public bool ApplyToAggregations { get; init; } = true;
    public string Fingerprint { get; init; }
}
```

4) `CatalogShareRule` Entity (EF Core)
- Option A (normalized): `CatalogShareRule` table with columns: Id, CatalogShareId, TargetType (Tenant|Customer|CustomerGroup), TargetId, Priority, Active, AllowOverride, IncludedCategorySlugs (json), ExcludedCategorySlugs (json), IncludedBrandIds (json), ExcludedBrandIds (json), IncludedProductIds (json), ExcludedProductIds (json), ApplyToAggregations, CreatedBy, UpdatedAt.
- Option B (inline): JSON array on `CatalogShare` (simpler migration, harder querying).

5) Rule Resolution Flow (server-side)
1. Authenticate → resolve `tenantId`, optional `customerId` and `customerGroupIds`.
2. Resolve `sharedCatalogIds` allowed for this tenant.
3. Load `CatalogShareRule` rows for these `sharedCatalogIds` (filter Active=true).
4. Pick rules matching target scope (customer exact, groups, tenant). Order by Priority desc.
5. Merge rule-sets into single `EffectiveRuleSet` (customer wins; exclude > include).
6. Compute fingerprint and cache the `EffectiveRuleSet` with TTL and invalidation on rule changes.

6) NEST Example: Build filter
```csharp
QueryContainer BuildElasticsearchFilter(EffectiveRuleSet rules, QueryContainerDescriptor<dynamic> q)
{
    return q.Bool(b => b
        .Filter(
            f => f.Term("isDeleted", false),
            f => f.Terms(t => t.Field("categories.keyword").Terms(rules.IncludedCategorySlugs ?? Array.Empty<string>()).MinimumShouldMatch(1))
        )
        .MustNot(mn => mn.Terms(t => t.Field("productId").Terms(rules.ExcludedProductIds)))
    );
}
```

Notes: if `IncludedCategorySlugs` is empty do not add the include-terms; use `cardinality` on `originProductId` for variant indexes when calculating product counts.

7) Example ES JSON Query (variant-oriented)
```json
{ "query": { "bool": { "must": [ {"terms": {"sourceCatalogId": ["..."]}}, {"term":{"tenantId":"tenant-123"}} ], "must_not": [{"terms":{"productId":["excluded-1"]}}, {"terms":{"categories.keyword":["excluded-cat"]}} ] } }, "aggs": { "by_category": { "terms": {"field":"categories.keyword"}, "aggs": { "unique_products": { "cardinality": { "field": "originProductId" } } } } } }
```

8) Caching & Fingerprint
- Fingerprint = SHA256(tenantId + customerId + sorted(sharedCatalogIds) + sorted(ruleIds+priority+active)). Use as cache key for `EffectiveRuleSet` and search result cache.

9) Tests
- Unit: rule merging precedence matrix, fingerprint stability, build ES filter JSON/NEST equivalence.
- Integration: run queries against a test ES cluster with representative docs (variants, shared catalogs) and compare with expected results.

10) Migration & Rollout
- Add `CatalogShareRule` EF entity + migration. Prefer normalized table for easier queries.
- Implement `ICatalogShareRuleManager` skeleton and tests.
- Extend `ProductSearchController` to call `ResolveAsync` when `includeShared=true` and pass resulting filter into search service.
- Rollout in shadow mode: new queries logged & compared; use feature-flag for full activation.

11) Performance & Security
- Use filter clauses (not scripts). Precompute heavy ACLs into index if necessary.
- Limit rule list sizes; support precomputed ES aliases for heavy customer filters.
- Validate incoming `catalogShareIds` against tenant permissions.

12) Known pitfalls
- Large per-user ACLs (scale problem) → prefer role/group-based indexing.
- Frequent rule changes causing cache churn → tune TTL and use event-driven invalidation on rule updates.

13) Next implementation suggestion (small incremental steps)
1. Add EF `CatalogShareRule` entity + migration (normalized).
2. Implement `ICatalogShareRuleManager.ResolveAsync` returning cached `EffectiveRuleSet`.
3. Implement `BuildElasticsearchFilter` and wire into `SearchIndexService`.
4. Add unit & integration tests; run shadow-mode comparison.

14) Reindexing, Aliases and Per-Tenant Indices

- For complex mapping or schema changes, trigger a background reindex job rather than blocking requests. Implement a reindex worker (e.g., a Wolverine message handler / background service) that:
    - Creates a new index name following convention: `products_{tenantId}_{lang}_v{N}`
    - Builds the target mapping/settings (shards, analyzers) and creates the new index
    - Streams documents (full or filtered) from source (DB or existing index) into the new index, applying any transformation (e.g., rule precomputation)
    - Monitors progress, exposes status and supports resume on failure

- Use aliases for zero-downtime cutover:
    - Active alias: `products_{tenantId}_{lang}` → points to current index `products_{tenantId}_{lang}_v{N}`
    - Reindex job writes into `products_{tenantId}_{lang}_v{N+1}` and validates data
    - When ready, atomically switch alias to the new index and optionally delete the old index after retention window

- Per-tenant indices:
    - Each tenant gets isolated indices for better security and per-tenant tuning: `products_{tenantId}_{lang}` aliases and versioned concrete indexes.
    - Pros: tenant isolation, per-tenant analyzers/settings, simpler per-tenant reindexing and rollbacks.
    - Cons: more indices to manage; plan shard sizing and resource usage accordingly.

- Shared catalogs & per-tenant indices:
    - Options:
        1. Copy shared catalog documents into consuming tenant indices during reindex (creates fully tailored tenant index).
        2. Keep shared documents in a central `shared_{lang}` index and run cross-index queries (tenant index + shared index) at runtime — requires query-time include/exclude handling.
    - Recommendation: prefer option 1 for tailored assortments and simpler aggregation semantics (counts follow tenant rules). Use option 2 for extremely large shared catalogs where duplication is prohibitive.

- Operational concerns:
    - Provide reindex status endpoint and dashboard (progress, errors, ETA).
    - Add safe-guards: max concurrent reindexes, rate-limited bulk throughput, and backpressure on elastic cluster.
    - Ensure alias switch is atomic (Elasticsearch supports alias API for atomic updates).
    - Plan shard/replica counts for per-tenant indices; consider index lifecycle policies (ILM) to remove old versions after retention.

- Testing and Rollout:
    - Run reindex in shadow mode first (write to new index, do not alias-swap), compare hit-lists and aggregations against active index.
    - Automate correctness checks (document counts, sample queries, cardinalities) before alias swap.


---
Date: 2026-01-06
