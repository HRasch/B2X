# NEST (.NET) Deprecation Notice

Summary
-------
This document explains why projects should prefer `Elastic.Clients.Elasticsearch` for Elasticsearch 8.x+ and provides migration guidance. It has been updated to reflect that the repository has started a pilot migration to the modern client.

Why migrate
-----------
- The maintained modern .NET client is `Elastic.Clients.Elasticsearch` (8.x+), which offers a redesigned, strongly-typed API and compatibility with Elasticsearch 8+.

Impact on this repository
------------------------
- The codebase contains a pilot migration replacing legacy NEST usage with `Elastic.Clients.Elasticsearch`. Search-related code under `backend/Domain/Search` and host services were updated to the new client.
- Integration tests and CI workflows should target an ES 8.x-compatible instance to validate behavior after migration.

Recommended actions
-------------------
1. Confirm the Aspire-managed Elasticsearch major version (8.x vs 9.x) to pin the client version appropriately.
2. Continue migrating code that uses NEST types (`ConnectionSettings`, `ElasticClient`, and `Nest` query objects) to `Elastic.Clients.Elasticsearch` idioms.
3. Prefer central package management: add `PackageVersion` entries to `Directory.Packages.props` and remove per-project `PackageReference` lines for the old client.
4. Add/upgrade integration tests using Testcontainers or a dev Aspire instance running ES 8.x to validate indexing and search behavior.
5. Keep `.ai/knowledgebase` updated with concrete before/after snippets and notes for uncommon query patterns.

References
----------
- Official .NET client: https://www.elastic.co/guide/en/elasticsearch/client/dotnet/current/index.html
- Migration examples: see `.ai/knowledgebase/dependency-updates/Elastic.Clients.Elasticsearch.md` for code snippets and notes.

Owner/Next step
---------------
- Owner: @Backend to continue migration work and remove any remaining `NEST` references.
- Action: Create a PR for the migration pilot and run CI against an ES 8.x instance.
