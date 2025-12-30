---
title: Elastic.Clients.Elasticsearch
current_version: 9.2.2
source_files:
  - backend/Directory.Packages.props
  - Directory.Packages.props
status: reviewed
created_by: SARAH
created_at: 2025-12-30

summary: |
  The official Elasticsearch .NET client is versioned in lockstep with Elasticsearch server versions; minor/major client versions may contain breaking changes. Repositories using the client must verify server compatibility before upgrades.

findings: |
  - NuGet current stable: `Elastic.Clients.Elasticsearch` 9.2.2
  - Release notes: https://github.com/elastic/elasticsearch-net/releases (see tags for per-release changelogs)
  - Documentation: https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/index.html
  - Important: the client major/minor parts are dictated by the Elasticsearch server version. Minor/patch releases may still introduce breaking changes — consult the breaking-changes policy: https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/breaking-changes-policy.html
  - The client delegates transport concerns to `Elastic.Transport`; verify the transitive `Elastic.Transport` version and test HTTP/serialization paths after any upgrade.
  - Observed breaking-changes highlights (representative):
    - `9.0.0` introduced container type changes and removal of certain descriptors/constructors — these can be high impact for code using generated descriptors.
    - `9.1.x` series included descriptor/aggregation usability changes (e.g., CompositeAggregation sources -> KeyValuePair) — review any code that constructs fluent aggregation descriptors.

actions: |
  - Do NOT upgrade the client without confirming Elasticsearch server compatibility. If upgrading, test index/ingest/search flows against a staging cluster running the target Elasticsearch version.
  - Add integration smoke tests that exercise basic indexing, bulk ingest and search to detect runtime API or serialization mismatches early.
  - Suggested PR checklist for safe upgrades:
    1. Confirm Elasticsearch server version(s) in environments (dev/stage/prod).
    2. Verify compatibility using the client's compatibility guidance and release notes (see links above).
    3. Run smoke integration tests against a staging cluster (see quick steps below).
    4. Monitor requests and error rates in staging for at least one full ingestion cycle.
    5. Roll out gradually (canary) if possible; rollback plan documented.
  - Quick local smoke-test (dev machine or CI job):
    - Start local elastic/kibana: `curl -fsSL https://elastic.co/start-local | sh` (runs ES on localhost:9200)
    - Run a minimal test program that creates an index, indexes one document and searches it (see `getting-started-net` docs linked above).
  - SARAH: collect an explicit mapping between our package versions in `Directory.Packages.props` and the recommended compatible Elasticsearch server versions; prepare example PR text and a staging test plan.

PR template suggestions for Elastic client upgrades:
- Branch: `deps/elastic-client-x.y.z` where `x.y.z` is the target client version.
- PR title: `chore(deps): bump Elastic.Clients.Elasticsearch to x.y.z`
- PR body checklist:
  - Update `Directory.Packages.props` with new client version.
  - Confirm `Elastic.Transport` transitive version changes (list them).
  - Run integration smoke tests against local ES `x.y.z`:
    ```bash
    curl -fsSL https://elastic.co/start-local | sh
    dotnet test tests/ElasticIntegrationTests --filter "Category=ElasticSmoke" -v minimal
    ```
  - Verified compatibility notes (link to client release notes and breaking-changes page).
  - Rollout plan: staging canary (10% traffic), monitor error rates and search/index latencies for 24h.
  - Rollback steps documented.
---
