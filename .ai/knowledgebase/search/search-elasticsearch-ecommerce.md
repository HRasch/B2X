# Elasticsearch for E‑commerce Search & Recommendations — Best Practices

Purpose
 - Short, practical guidance for modeling product search and blended recommendations using Elasticsearch (ES).

Summary
 - Use ES as a flexible, horizontally scalable search engine and lightweight vector store for hybrid search/recs.
 - Model documents to favor read/query-time performance: denormalize product view for search, keep separate change-log streams for ETL.
 - Combine traditional inverted-index retrieval with vector/embedding-based ranking for personalization and semantic relevance.

1) Data modeling (documents & granularity)
 - One primary `product` document for search results that contains precomputed, denormalized fields used in ranking and display: title, description, sku, price, currency, availability, category path, attributes, brand, promotions, rating aggregates.
 - Model variants (size/color) as nested objects inside the product if variant-level search/filtering is required. Use `nested` type to preserve per-variant fields and filter efficiently.
 - Avoid frequent cross-index joins at query-time. If relational data must be kept normalized, maintain a denormalized search index updated via CDC or an ingest pipeline.
 - Use a separate `events`/`user_actions` index for clicks, views, add-to-cart to drive offline training and online ranking features.

2) Mappings & analyzers
 - Choose analyzers per field: use `standard`/language-specific analyzers for full-text, `keyword` for exact matches, `lowercase` normalizers for case-insensitive filtering.
 - Use `edge_ngram` or `search_as_you_type` for autocomplete; prefer `search_as_you_type` where available for simpler scoring and fewer custom components.
 - Configure synonyms carefully (managed synonyms index or API): maintain separate synonyms lists for search and suggestion pipelines to avoid over-broad matching.
 - For multilingual sites, index language-specific fields (e.g., `title_en`, `title_de`) or use language analyzers and route queries by language.

3) Indexing strategy and pipelines
 - Use bulk indexing for initial load and incremental bulk updates for daily/near-real-time refreshes. Tune `refresh_interval` during heavy indexing (e.g., set to `-1` then re-enable once done).
 - Use ingest pipelines for lightweight enrichment: category normalization, SKU tokenization, attribute extraction, stemming tweaks.
 - Implement versioning or sequence numbers in documents to enable safe upserts and avoid stale overwrites from asynchronous pipelines.
 - Prefer idempotent bulk updates and include `routing` if you use application-level routing for performance.

4) Relevance and ranking
 - Start with BM25 and field-level boosts (title^4, sku^8, exact_brand^2, description^0.75). Keep boosts data-driven and track query-level performance.
 - Use `function_score` for business signals: recency boost, popularity (sales, CTR), stock availability, margin/promotions. Combine multiplicatively or additively depending on desired effect.
 - Implement query-time rescoring or second-pass re-ranking: retrieve a broad candidate set with inverted index, then re-score top-N with richer features (behavioral signals, ML model scores, vector similarity).

5) Personalization & recommendations
 - Separate concerns: use ES for candidate retrieval (textual + attribute filters + vector kNN) and a ranking/re-ranking service for personalization models.
 - Options:
   - Collaborative filtering offline: produce item-to-item similarity matrices and store top-K neighbors per item in ES as a `recommendations` field.
   - Session-based or sequence models: generate candidate lists externally and store them or re-rank retrieved candidates at query time.
   - Embeddings & hybrid search: store dense vectors (e.g., `dense_vector`) and use kNN (approximate) to find semantically similar products; combine vector scores with BM25 via hybrid scoring.
 - Be aware of feature availability: Elasticsearch vector search may rely on commercial or specific plugins; evaluate OpenSearch or dedicated vector engines (Milvus, Pinecone) for scale/cost tradeoffs.

6) Blending search + recommendations
 - Pipeline: produce candidates from (A) inverted-index full-text search, (B) vector similarity, (C) item-to-item neighbors, then merge/deduplicate and re-rank.
 - Use business rules to guarantee constraints (in-stock, allowed promotions) and filter before final ranking.
 - Apply diversity heuristics (recency/brand/category diversification) to avoid narrow repeats.

7) Performance, scaling & sharding
 - Shard sizing: keep shard count moderate; avoid many tiny shards. Aim for shard sizes 10s of GB depending on hardware and query patterns.
 - Routing: use custom routing when you can isolate traffic to subsets of data (e.g., per-tenant). Otherwise let ES balance shards.
 - Hot-warm architecture: put frequently searched indices on hot nodes, older/frozen indices on warm/cold nodes and use searchable snapshots for very large historical data.
 - Caching: rely on query cache for repeat queries; tune `request_cache` for expensive aggregations, not for frequently changing indices.

8) Sizing & Architecture for Large Catalogs (5M products / 3M variants / 50M attributes)

 - Scope & document counts: decide whether variants are nested inside the product document or modeled as separate documents. With variants as nested objects you will have ~5M product documents; modeling variants as separate documents yields ~8M documents (5M products + 3M variants). Attribute leaf values (50M entries) drive mapping choices: use `flattened` if attribute keys are high-cardinality or unknown.

 - Estimation workflow (quick formula):
   - Estimate `avg_doc_source_bytes` (JSON size before compression). Example: 1.5–3 KB for denormalized product+variants metadata.
   - Raw size = `doc_count * avg_doc_source_bytes`.
   - Lucene on-disk size ≈ raw_size * (1.5–4) depending on stored fields, doc_values, norms, and vectors.
   - Vector storage (if using `dense_vector`): approx `dims * 4 bytes * vector_docs` (float32) before index-specific compression/quantization. Quantized indexes (int8/int4/bbq) reduce this substantially; check your ES version's quantization docs.

 - Shard sizing guidance:
   - Target a moderate shard size (typical recommendation: 20–50 GB primary shard size for general-purpose hardware). Too-small shards increase overhead; too-large shards increase recovery and GC risks.
   - Determine `number_of_primaries = ceil(estimated_index_size / target_shard_size)` at index creation time. Set `index.number_of_routing_shards` if you plan to split later.
   - Example: with 8M docs, avg 2 KB → raw ~16GB; with overhead (×3) ~48GB → choose 2 primary shards (≈24GB each) or 3 shards for extra headroom.

 - Node sizing & JVM heap:
   - Keep JVM heap <= 32 GB and prefer leaving >50% of machine RAM for filesystem cache. Typical data node sizes: 64–256 GB RAM depending on workload; choose more RAM and CPU for high-concurrency search workloads.
   - Provision NVMe SSDs or fast local SSD; prefer local disks to avoid page-cache thrashing from remote storage.

 - Topology & roles:
   - Use dedicated master-eligible nodes (3), dedicated data nodes (hot / warm / cold), and optionally ingest/coordinating-only nodes. For heavy vector or aggregation workloads, dedicate nodes for vector-heavy indices.
   - Consider a follower-search cluster (CCR) to isolate indexing from search if indexing load competes with queries.

 - Attributes modeling tradeoffs (50M attribute values):
   - If attribute keys are unpredictable or numerous, use the `flattened` type to avoid mapping explosion. `flattened` stores all leaf values as keywords and avoids per-key mappings but limits numeric range queries and some aggregations.
   - If attribute keys are well-known and limited, model them as explicit fields (keyword or numeric as appropriate) to enable fast aggregations and sorting.
   - Alternative: store attributes in a separate `attributes` index with documents {product_id, key, value} when you need heavy attribute-level analytics or highly dynamic attribute sets; query with `terms` or use a join at application level.

 - Index settings & query-time tuning for large indices:
   - Use `refresh_interval` relaxation during bulk loads (`-1`) and restore to a suitable value (e.g., `5s` or `1s`) after indexing.
   - Start with `index.number_of_replicas=1` for availability; tune replicas based on throughput/throughput-per-node trade-offs.
   - Use `eager_global_ordinals` for fields used widely in aggregations to avoid high-latency first-aggregation spikes (watch heap usage).
   - Use `index.sort` for extremely common conjunctions to speed specific query patterns when applicable (cost: slightly slower indexing).

 - Vector/HNSW scaling notes (if using embeddings at scale):
   - Expect vector storage and HNSW graph memory to dominate for large vector counts. For N millions of vectors, evaluate quantized `int8_hnsw` / `bbq_hnsw` or specialized vector DBs.
   - For 8M vectors at 768 dims (float32) raw vector bytes ≈ 8M * 768 * 4 ≈ 24.6 GB before indexing overhead. HNSW index memory is additional (graph links, metadata); empirically validate memory needs in a staging cluster.
   - If QPS or vector count grows, prefer dedicated vector nodes or an external vector engine (Milvus / Pinecone / Faiss) and merge/ re-rank results in ES.
   - When using quantization, use oversampling + re-rank to recover recall: query more candidates and re-score with higher-precision vectors for the final top-K.

 - Operational recommendations:
   - Monitor shard count per node and ensure shards per node remain moderate (<20–30 active shards typical depending on node size).
   - Test recoveries and size down/up operations (snapshot & restore, allocation awareness) in pre-prod.
   - Use autoscaling policies (if available) or scripted capacity procedures to add data nodes before shard sizes or I/O approach limits.

 - Summary recommendation for the requested catalog size:
   - Start with a single index for search documents with nested variants (if acceptable) and `flattened` attributes for high-cardinality attributes. Estimate index size with realistic sample data to pick 2–6 primary shards targeting 20–50GB/shard.
   - Use dedicated hot data nodes (fast NVMe, 64–128GB RAM), dedicated masters (3), and optionally dedicated ingest/coordinating nodes. Keep heap <=32GB and leave disk cache for OS.
   - Evaluate vector storage separately: run a staging run to measure vector index memory; if vectors add substantial overhead, move vectors to dedicated vector nodes or an external vector DB and merge at query time.

 - Useful docs:
   - Size your shards: https://www.elastic.co/docs/deploy-manage/production-guidance/optimize-performance/size-shards
   - Tune for search speed: https://www.elastic.co/docs/deploy-manage/production-guidance/optimize-performance/search-speed
   - Important settings (heap, path, discovery): https://www.elastic.co/guide/en/elasticsearch/reference/current/important-settings.html

9) Index lifecycle and operational concerns
 - Use ILM (Index Lifecycle Management) to roll indices: hot → warm → cold → delete. Keep rollups or downsampled indices for analytics.
 - Optimize refresh intervals: near-real-time for admin pages; relaxed refresh for bulk ingestion pipelines.
 - Snapshots & backups: schedule snapshots and test restores periodically.
 - Controlled reindexing: use alias switching to reindex with zero-downtime — build new index, reindex, switch alias.

9) Observability & evaluation
 - Track A/B metrics: CTR, add-to-cart rate, conversion, revenue per search, zero-result rate. Use these to tune relevance.
 - Log query inputs, top-k results, and user interactions for offline analysis and model training. Respect privacy rules.
 - Benchmarks: load-test candidate retrieval and re-ranking separately. Measure P99 latency and tail behavior.

10) Security, privacy & compliance
 - Secure ES cluster: TLS, authentication (native/SSO), IP allowlists, and role-based access control. Avoid exposing ES endpoints publicly.
 - Mask or avoid storing PII in indices. If storing user-related signals, use aggregated/hashed identifiers and document retention policies.

11) Testing & quality
 - Maintain a relevance test-suite: sample queries, expected top results, and automated checks to detect regressions.
 - Use synthetic data and production sampling for performance testing.

12) Practical tradeoffs & recommendations
 - Start simple: inverted index with good analyzers + field boosts + business rule boosts. Add vector search when semantic queries or personalization needs justify complexity.
 - Keep heavy ML models outside ES for re-ranking; ES is best at candidate retrieval and fast scoring.
 - Consider cost: Elasticsearch managed offerings may charge for enterprise features (vectors, kNN) — evaluate OpenSearch or separate vector DB for embeddings.

Hybrid search (text + vector) — best practices
 - Purpose: combine lexical relevance (BM25) and semantic relevance (vector similarity) to handle both keyword and intent queries.
 - Candidate retrieval + re-ranking: always retrieve a candidate set via inverted-index queries (multi-match, filters) and augment with vector kNN results; merge and deduplicate before re-ranking.
 - Score calibration: normalize BM25 scores and vector similarity to comparable ranges before blending. Use min-max normalization or learned weights from offline data.
 - Blending strategies:
   - Linear blend: score = alpha * normalized_bm25 + (1 - alpha) * normalized_vector
   - Two-pass: use BM25 to fetch broad candidates, then re-rank top-N with a ML model that includes vector score as a feature
   - Function score: use `script_score` to combine factors at query time for small top-k rescoring
 - Vector configuration:
   - Use `dense_vector` with dimensionality matching your embedding model (e.g., 768).
   - Choose an ANN algorithm (HNSW recommended) and tune `ef_construction`/`ef_search` for recall vs latency.
   - Consider a separate vector-only index for high-throughput kNN to reduce interference with text indexing.
 - Query-time performance:
   - Limit kNN calls to top candidate sets (e.g., expand BM25 by 100, then vector re-rank top 50) when vectors are expensive.
   - Cache popular query embeddings and precomputed candidate lists where possible.
 - Practical hybrid query example (conceptual):

```json
{
  "size": 10,
  "query": {
    "script_score": {
      "query": { "bool": { "must": [ { "multi_match": { "query": "wireless headphones", "fields": ["title^3","description"] } } ], "filter": [ { "term": { "in_stock": true } } ] } },
      "script": {
        "source": "double bm25 = _score; double vec = params.cosine; return params.alpha * (bm25 / params.bm25_max) + (1-params.alpha) * vec;",
        "params": { "alpha": 0.6, "bm25_max": 10.0, "cosine": 0.82 }
      }
    }
  }
}
```

 - Mapping snippet (conceptual):

```json
{
  "mappings": {
    "properties": {
      "title": { "type": "text", "analyzer": "standard" },
      "description": { "type": "text", "analyzer": "standard" },
      "sku": { "type": "keyword" },
      "embedding": { "type": "dense_vector", "dims": 768 }
    }
  }
}
```

 - Calibration & offline tuning:
   - Use an offline dataset of queries and labeled relevance to grid-search `alpha` (blend weight) and rescoring thresholds.
   - Track business KPIs (CTR, conversion) not only IR metrics (NDCG, MAP) when choosing defaults.

 - Fallbacks & cold start:
   - If embeddings are missing or vector search fails, fall back to pure BM25 ranking.
   - For new items without interaction data, rely more on textual signals and catalog attributes.

 - When to use a separate vector engine
   - If your vectors and kNN workload grows fast, consider using a specialized vector DB (Milvus, Faiss, Pinecone) and then join/merge candidate lists with ES results.

    References (Vectors & kNN)
    - kNN / vector search in Elasticsearch: https://www.elastic.co/docs/solutions/search/vector/knn
    - `dense_vector` field type: https://www.elastic.co/guide/en/elasticsearch/reference/current/dense-vector.html
    - Re-ranking / oversample & rescore guidance: https://www.elastic.co/guide/en/elasticsearch/reference/current/re-ranking-overview.html

Image / Picture Search — Best Practices
 - Use case: visual similarity search, find-near-duplicate, visual product search (user uploads image), and visual recommendations.
 - Architecture: separate object storage (S3/CDN) for images; store metadata and embeddings in ES. Never store raw binary blobs in ES.

1) Feature extraction
 - Use a pre-trained image embedding model (CLIP, ViT, ResNet variants, or task-specific image encoders) to produce fixed-size vectors (e.g., 512, 768, 1024 dims).
 - Extract embeddings in an offline/ingest service (model-server) and write to ES as `dense_vector` fields during indexing. Optionally compute a perceptual hash (pHash) for cheap dedupe checks.
 - Consider using augmentation pipelines to normalize images (resize, center-crop, color normalization) before embedding for consistent vectors.

2) Mappings & storage
 - Store image vectors as `dense_vector` (dims matching embedding model). Keep `image_phash` as a `keyword` or `long` for fast exact/near-duplicate detection.
 - Keep rich metadata: `url`, `width`, `height`, `mime_type`, `color_histogram` (optional), `dominant_color`, `brand`, `category`, `sku`, `variant_id`.
 - Example mapping (conceptual):

```json
{
  "mappings": {
    "properties": {
      "sku": { "type": "keyword" },
      "image_url": { "type": "keyword" },
      "image_phash": { "type": "keyword" },
      "image_embedding": { "type": "dense_vector", "dims": 512 }
    }
  }
}
```

3) Indexing & pipelines
 - Extract embeddings in a scalable model service (GPU/CPU depending on throughput). Use batching for inference and store vectors via bulk API.
 - Compute and store `pHash` for quick duplicate filtering before expensive vector queries.
 - Maintain versioning/sequence numbers for images to avoid stale updates.

4) Query patterns
 - Visual-only search: kNN on `image_embedding` (HNSW/ANN) to return nearest neighbors.
 - Visual + filters: combine kNN with attribute filters (e.g., category, color, price range) using the vector query within a `bool` query.
 - Text + visual hybrid: retrieve candidates via text filters or BM25, then re-rank using image similarity (or vice versa). Merge candidate sets and deduplicate.
 - Example hybrid query (conceptual):

```json
{
  "size": 12,
  "query": {
    "bool": {
      "filter": [ { "term": { "category": "headphones" } } ],
      "should": [
        { "knn": { "image_embedding": { "vector": [/*image vector*/], "k": 50 } } },
        { "multi_match": { "query": "wireless", "fields": ["title^3","description"] } }
      ]
    }
  }
}
```

5) Deduplication & near-duplicates
 - Use `pHash` + thresholded Hamming distance to identify near-duplicates cheaply, then filter before returning results.
 - For strict dedupe, maintain a canonical `image_id` or `primary_image` field and collapse duplicates at result merge time.

6) Performance & scaling
 - Separate heavy vector workloads: put image-heavy indices on dedicated nodes or use a separate vector engine for scale.
 - Tune HNSW parameters: `ef_construction` and `ef_search` control recall vs latency; increase `ef_search` for higher recall on critical queries.
 - Precompute candidate sets for common visual queries (e.g., top neighbors for popular SKUs) to reduce per-query computation.

7) Evaluate & monitor
 - Measure visual relevance with human-labeled similarity sets (precision@k, recall@k). Also track business KPIs (CTR, conversion) for image-driven flows.
 - Monitor vector query latency, memory usage, and ANN index build times.

8) Security & privacy
 - Strip or hash any user-identifying metadata before indexing. If users upload images, confirm consent and retention rules.
 - Protect model endpoints and ES cluster with auth and TLS.

9) When to use specialized vector engines
 - If image vector index grows very large or requires ultra-low latency at high QPS, use dedicated vector DBs (Milvus, Faiss, Pinecone) and merge results with ES metadata.

10) Practical tips
 - Cache image embeddings for repeated uploads (e.g., same file content) using checksum-based keys.
 - Provide thumbnail URLs in search results and lazy-load full-resolution assets from CDN.
 - Keep image-processing and model inference out of request-critical critical path where possible (async ingestion + precompute).


Facets, Rule-based Filtering & Category Graph
---------------------------------------------

Facets & Aggregations — Best practices
 - Index-time: store facetable fields as `keyword` (or numeric) with `doc_values=true` for fast aggregations. Avoid enabling `fielddata` on `text` fields in production.
 - Prefer `terms` aggregations for low-to-moderate cardinality; use `composite` aggregation for paginated or very-high-cardinality faceting to avoid memory spikes and to paginate results reliably.
 - Use `post_filter` to apply user-selected facet filters so aggregations remain computed over the original query result set (if you need facet counts unaffected by filtering), or apply filters in the main `query` when both results and counts should reflect filters.
 - Tune `size` and `shard_size` on `terms` aggs; raise `shard_size` (cheaper than `size`) to improve accuracy for skewed distributions. Consider `shard_min_doc_count` to reduce noise from low-frequency terms.
 - Use `eager_global_ordinals` on heavily-used keyword fields to avoid first-request latency spikes at the cost of memory during indexing.
 - For very large unique value sets, use `partition` or `composite` with `after_key` to iterate buckets across requests rather than one monolithic aggregation.
 - Use `collect_mode: breadth_first` for deep aggregation trees where top-level pruning should occur first (default heuristics usually suffice).
 - Cache expensive aggregation results or precompute popular facet buckets offline where latency matters (e.g., category landing pages).

  References (Aggregations & Facets)
  - Terms aggregation: https://www.elastic.co/guide/en/elasticsearch/reference/current/search-aggregations-bucket-terms-aggregation.html
  - Composite aggregation (paged faceting): https://www.elastic.co/guide/en/elasticsearch/reference/current/search-aggregations-bucket-composite-aggregation.html
  - Filter & Filters aggregation: https://www.elastic.co/guide/en/elasticsearch/reference/current/search-aggregations-bucket-filter-aggregation.html
  - Aggregations overview: https://www.elastic.co/guide/en/elasticsearch/reference/current/search-aggregations.html

Rule-based Filtering & Merchandising (promotions, pins, business rules)
 - Keep a canonical rules store outside ES (database or rules service). At query time, translate active rules into ES query constructs rather than hardcoding rules into mappings.
 - For a small set of promoted documents use `pinned` query to guarantee ordering of specific docs.
 - For boosts and business-rule signals, prefer `function_score` (filter-based weights) or `script_score` for complex logic. Example pattern:

```json
{
  "query": {
    "function_score": {
      "query": { "match": { "title": "sneakers" } },
      "functions": [
        { "filter": { "term": { "is_on_promo": true } }, "weight": 1.5 },
        { "filter": { "term": { "brand": "preferred" } }, "weight": 1.2 }
      ],
      "score_mode": "multiply"
    }
  }
}
```

 - For complex merchandising (business rules with priority, availability windows, geo or price constraints) build the rule decisions in the application layer and emit an optimized ES query (or use `pinned` + `organic` hybrid). Keep the rules engine testable and versioned.
 - Use a separate alias or index for seasonal campaigns so you can safely change boosting without reindexing the entire catalog.
 - For frequent, low-latency rule changes, consider applying rules in a post-retrieval re-ranking microservice (fetch candidates from ES, apply rules and ML score, then return final ordered results).

  References (Rule-based / Merchandising)
  - Pinned query (promote specific docs): https://www.elastic.co/guide/en/elasticsearch/reference/current/query-dsl-pinned-query.html
  - Function score query: https://www.elastic.co/guide/en/elasticsearch/reference/current/query-dsl-function-score-query.html
  - Script score query: https://www.elastic.co/guide/en/elasticsearch/reference/current/query-dsl-script-score-query.html

Category Graph & Taxonomy Modeling
 - Denormalize category paths into product documents for fast filtering and faceting: store `category_ids` (array of ints/keywords) and `category_path` (materialized path string).
 - Use the `path_hierarchy` tokenizer on `category_path` for prefix-based filtering and efficient ancestor matching. Example analyzer usage: index `category_path` as a `text` subfield with `path_hierarchy` to support "show all products under /Electronics/Audio" queries.
 - Store both `category_ids` and `category_path` so you can do exact `term` filters on ids and hierarchical text matches on paths when required.
 - Maintain a separate `categories` index that holds the taxonomy graph (id, parent_id, path, metadata). Use this index for category browsing, breadcrumb resolution, and upstream consistency checks.
 - Graph/traversal choices:
   - Materialized Path: easy to index and query (best for read-heavy category lookups). Use `path_hierarchy` for matching ancestors.
   - Adjacency List: store parent pointers and compute traversals in application or via a dedicated graph DB for complex queries.
   - Parent-Child join in ES: avoid unless you must keep many-to-one write/update semantics; parent/child has performance and complexity costs.
 - For category-based aggregations, prefer aggregating over `category_ids` (keyword/doc_values) and use `composite` when returning very large category lists.
 - Keep category metadata (visibility, merchandising flags, canonical landing page) in the `categories` index and join/merge at query time via application logic or by denormalizing necessary flags into product docs.


Next steps for this project
 - Create an offline pipeline that ingests product catalog changes → builds search documents → bulk indexes to ES using alias switch.
 - Add a small re-ranking microservice that accepts candidates and enriches them with behavioral features and model scores.
 - Implement a relevance test-suite and baseline A/B experiment infrastructure.

Maintainers
 - @Backend, @Search, @Architect

References & Further Reading
 - Elastic: kNN search in Elasticsearch — dense_vector, HNSW, quantization, rescore/oversample: https://www.elastic.co/docs/solutions/search/vector/knn
 - Elastic: dense_vector field type (mapping parameters, quantization): https://www.elastic.co/guide/en/elasticsearch/reference/current/dense-vector.html
 - Elastic: script_score & function_score (combining signals, custom scoring): https://www.elastic.co/guide/en/elasticsearch/reference/current/query-dsl-script-score-query.html
 - Elastic: ingest pipelines (enrichment, normalization): https://www.elastic.co/guide/en/elasticsearch/reference/current/ingest.html
 - Elastic: search_as_you_type (autocomplete patterns): https://www.elastic.co/guide/en/elasticsearch/reference/current/search-as-you-type.html
 - Elastic: index lifecycle management (ILM): https://www.elastic.co/guide/en/elasticsearch/reference/current/index-lifecycle-management.html
 - Pinecone: hybrid search overview and motivation (sparse+dense fusion): https://www.pinecone.io/learn/hybrid-search/
 - Milvus: vector database overview (when to use dedicated vector engines): https://milvus.io/docs/v2.2.9/overview.md
 - OpenSearch: k-NN plugin docs (alternative to ES vector search): https://docs.opensearch.org/latest/search-plugins/knn/
 - CLIP paper (image+text embeddings for visual search): https://arxiv.org/abs/2103.00020

Verification notes
 - The recommendations in this article are aligned with official Elastic documentation (dense vectors, kNN, script_score, ingest pipelines, ILM) and third-party hybrid-search guidance (Pinecone). Use the linked docs for API/syntax details and production tuning guidance.
