---
docid: KB-154
title: Semantic Search Elasticsearch
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

# Semantic Search with Elasticsearch — Summary

This note summarizes a practical approach to apply semantic (vector) search using Elasticsearch (ES 8+). It focuses on architecture, index design, queries, and operational considerations.

- **Goal:** Combine semantic similarity (vector embeddings) with traditional text relevance (BM25) to improve search quality for natural-language queries.

- **Embeddings:** Generate dense vector embeddings for documents and queries using an embedding model (cloud or local). Store per-document embeddings in the index as a `dense_vector` field (or in external vector stores if preferred).

- **Index mapping (recommended):**

  - Use `dense_vector` with HNSW for approximate k-NN (Elasticsearch 8+): set `index.knn` and provide HNSW params.

  Example mapping (JSON):

  ```json
  {
    "mappings": {
      "properties": {
        "content": { "type": "text" },
        "embedding": { "type": "dense_vector", "dims": 1536, "index": true, "method": { "name": "hnsw", "engine": "nmslib", "space_type": "cosinesimil", "parameters": { "m": 16, "ef_construction": 200 } } }
      }
    },
    "settings": {
      "index": { "knn": true }
    }
  }
  ```

- **Indexing strategy:**
  - Chunk long documents into passages with metadata (source, position).
  - Compute embeddings at ingest time and store them with the chunk.
  - Keep `content` (text) and `embedding` (vector) together to allow hybrid ranking.

- **Query approaches:**
  - k-NN API (preferred when available): use ES `knn` query to return nearest vectors efficiently.
  - Script-scored hybrid query: combine vector similarity (cosine/dot) with BM25 `match` scores via `script_score` or weighted `bool` `should` clauses.

  Example hybrid search (conceptual):

  ```json
  {
    "query": {
      "script_score": {
        "query": { "match": { "content": "user query text" } },
        "script": {
          "source": "0.6 * _score + 0.4 * cosineSimilarity(params.query_vector, 'embedding')",
          "params": { "query_vector": [ ...embedding values...] }
        }
      }
    }
  }
  ```

- **Practical tips:**
  - Normalize embeddings as required by your similarity function (e.g., L2-normalize for dot-product cosine equivalence).
  - Tune the BM25/vector weighting for your use case (recall vs precision trade-offs).
  - Use passage-level retrieval + reranking: retrieve top-K by vector, then rerank with a cross-encoder or exact scoring for final top results.
  - Store useful metadata (`url`, `title`, `source`, `chunk_id`) for result snippets and faceting.

- **Performance & ops:**
  - HNSW index parameters (`m`, `ef_construction`) affect index size and recall — tune for latency and quality.
  - Use `refresh_interval` and bulk indexing to control ingestion performance.
  - Monitor heap usage and node sizing (vectors increase memory pressure). Consider dedicated data nodes for vector indices.
  - Snapshot/backup strategy must include vector indices; reindexing may be required when changing dims or method.

- **Security & governance:**
  - Never store raw user inputs if they are sensitive; consider encryption/PII handling.
  - Avoid embedding secrets into documents or logs.

- **When to use ES vectors vs external vector DB:**
  - Use ES if you want tight integration with full-text features, aggregations, and ingest pipelines.
  - Consider specialized vector DBs if you need very large scale, specialized ANN algorithms, or cheaper GPU-based indexing.

References & next steps:

- Prototype a small index: create mapping, index sample chunks, and test `knn` + hybrid `script_score` queries.
- Add a small reranker model if you need best-in-class ranking for the top 10 results.
