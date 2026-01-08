---
docid: UNKNOWN-125
title: CatalogShareRulesAPI
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

---
title: Catalog Share Rules - API Design
owner: @SARAH
---

Purpose
-------
Design the HTTP API surface and server-side resolution flow for CatalogShare rules (tenant/customer/customer-group scoped). Includes DTOs, request flow, example Elasticsearch queries and caching guidance.

API Endpoints
-------------
- GET /api/catalogs/{catalogId}/shares/{shareId}/rules
  - Permission: catalog owner (read) or consuming tenant admin (read) depending on share permissions
  - Response: list of `CatalogShareRuleDto`

- POST /api/catalogs/{catalogId}/shares/{shareId}/rules
  - Create a scoped rule (owner only or permitted consumer)
  - Body: `CreateCatalogShareRuleRequest`
  - Response: `CatalogShareRuleDto`

- PUT /api/catalogs/{catalogId}/shares/{shareId}/rules/{ruleId}
  - Update rule (owner or permitted consumer)

- DELETE /api/catalogs/{catalogId}/shares/{shareId}/rules/{ruleId}
  - Delete rule (owner or permitted consumer)

- GET /api/search/products
  - Existing search endpoint extended with context parameters:
    - `includeShared=true|false` (default true for storefronts that consume shared catalogs)
    - `catalogShareIds=guid[,guid...]` (optional explicit share ids to include)
    - `customerId=guid` (optional, for customer-scoped resolution)
    - other existing search parameters (q, facets, page, size, sort...)

DTOs / Request Models (C#)
--------------------------
public record CatalogShareRuleDto
{
    public Guid Id { get; init; }
    public Guid CatalogShareId { get; init; }
    public string TargetType { get; init; } // Tenant | Customer | CustomerGroup
    public Guid TargetId { get; init; }
    public int Priority { get; init; }
    public bool Active { get; init; }
    public string[] IncludedCategorySlugs { get; init; }
    public string[] ExcludedCategorySlugs { get; init; }
    public Guid[] IncludedBrandIds { get; init; }
    public Guid[] ExcludedBrandIds { get; init; }
    public Guid[] IncludedProductIds { get; init; }
    public Guid[] ExcludedProductIds { get; init; }
    public bool ApplyToAggregations { get; init; } = true;
}

public record CreateCatalogShareRuleRequest
(
    string TargetType,
    Guid TargetId,
    int Priority,
    bool Active,
    string[] IncludedCategorySlugs,
    string[] ExcludedCategorySlugs,
    Guid[] IncludedBrandIds,
    Guid[] ExcludedBrandIds,
    Guid[] IncludedProductIds,
    Guid[] ExcludedProductIds,
    bool ApplyToAggregations
);

Search API extension (effective-rule resolution)
---------------------------------------------
Server-side flow for a store request with `includeShared=true`:

1. Authenticate request and resolve `tenantId` and optional `customerId`.
2. Resolve `sharedCatalogIds` available to `tenantId` (catalog shares where tenant is target).
3. If client supplied `catalogShareIds`, intersect with allowed `sharedCatalogIds`.
4. Fetch all `CatalogShareRule` records for these `catalogShareIds`.
   - If `customerId` present: fetch rules for `TargetType=Customer` with `TargetId=customerId`.
   - Fetch `CustomerGroup` rules for groups the customer belongs to.
   - Fetch `Tenant` scoped rules for the tenant.
5. Merge rule-sets by precedence: Customer -> CustomerGroup -> Tenant -> OwnerDefaults. Apply `Active` and `Priority`.
6. Compose the effective ES query (bool):
   - Must: `(tenantId == currentTenant) OR (sourceCatalogId IN sharedCatalogIds)`
   - MustNot: any `excludedProductIds` OR `categories.keyword IN excludedCategorySlugs` OR `brandId IN excludedBrandIds`
   - If `includedCategorySlugs` present: add Must clause `categories.keyword IN includedCategorySlugs`
7. Run search and aggregations with the same filter set. For variant indexes, apply filters before aggregations and use `cardinality` on `originProductId` to de-duplicate.

Example Elasticsearch query (variant-oriented) - simplified
---------------------------------------------------------
{
  "query": {
    "bool": {
      "must": [
        { "term": { "tenantId": "tenant-123" }},
        { "terms": { "sourceCatalogId": ["share-guid-1","share-guid-2"] }}
      ],
      "must_not": [
        { "terms": { "productId": ["excluded-prod-1"] }},
        { "terms": { "categories.keyword": ["cat-slug-1"] }}
      ]
    }
  },
  "aggs": {
    "by_category": {
      "terms": { "field": "categories.keyword" },
      "aggs": {
        "unique_products": { "cardinality": { "field": "originProductId" }}
      }
    }
  }
}

Effective Rule Fingerprint & Caching
------------------------------------
- Compute fingerprint: SHA256(tenantId + customerId + sorted(sharedCatalogIds) + sorted(ruleIds + priorities + active flags))
- Use fingerprint in cache key for search results and aggregations.

Precomputed Filter Aliases (performance)
----------------------------------------
- For customers with large/exactly-known rule sets, precompute an ES filtered alias or stored query that represents the effective filter and use it in queries to reduce bool complexity.

Security & Authorization
------------------------
- Only allow consumers to view rules when `CatalogShare.permissions` permit it.
- Admin endpoints require owner-level or appropriate role checks.

Errors & Edge Cases
-------------------
- If `sharedCatalogIds` is empty, behave as local-only catalog.
- If include and exclude lists conflict, `exclude` wins unless `AllowOverride=true` on higher-priority rule.

Next Steps
----------
- Implement `CatalogShareRule` entity and EF migration.
- Add service `ICatalogShareRuleService` to resolve and merge rules by context.
- Extend `SearchIndexService` / `ProductSearchController` to call rule resolution when `includeShared=true`.
