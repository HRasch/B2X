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
  - Release notes: https://github.com/elastic/elasticsearch-net/releases (check specific tags for patch/minor details)
  - Documentation: https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/index.html
  - Important: the client major/minor parts are dictated by the Elasticsearch server version. Minor or patch releases may include breaking changes — always consult the client's breaking-changes policy: https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/breaking-changes-policy.html
  - The client delegates transport concerns to `Elastic.Transport`; verify the transitive `Elastic.Transport` version and test HTTP/serialization paths after any upgrade.

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
---
