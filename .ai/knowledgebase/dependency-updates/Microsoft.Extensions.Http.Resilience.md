---
docid: KB-092
title: Microsoft.Extensions.Http.Resilience
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

---
title: Microsoft.Extensions.Http.Resilience
current_version: 10.1.0
source_files:
  - Directory.Packages.props
status: reviewed
created_by: SARAH
created_at: 2025-12-30

summary: |
  Microsoft.Extensions.Http.Resilience provides pre-built resilience/hedging handlers for `HttpClient` built on Polly strategies. It supports `AddStandardResilienceHandler`, hedging, and custom handler composition to compose retry, timeout, circuit breaker, concurrency limiter, and fallback strategies.

findings: |
  - NuGet current stable: `Microsoft.Extensions.Http.Resilience` 10.1.0
  - Known compatibility notes:
    - `Grpc.Net.ClientFactory` <= 2.63.0 may throw when enabling standard handlers; upgrade to 2.64.0+ or suppress the build-time check.
    - Application Insights: registering resilience before AI (<=2.22.0) can prevent telemetry; ensure AI >=2.23.0 or register AI first.
  - Release notes / repo: https://github.com/dotnet/extensions (see releases and issues for `Http.Resilience`)
  - The package exposes `AddStandardResilienceHandler`, `AddStandardHedgingHandler` and `AddResilienceHandler(name,...)` for custom pipelines.
  - Known caveats (from NuGet docs):
    - `Grpc.Net.ClientFactory` compatibility: if using gRPC clients, ensure `Grpc.Net.ClientFactory` >= 2.64.0 to avoid runtime errors when using the standard handlers.
    - Application Insights ordering: if AppInsights SDK < 2.23.0 is used, register AI services before resilience handlers or upgrade AI.

actions: |
  - PR checklist for upgrades or enabling resilience:
    1. Inventory projects using gRPC and confirm `Grpc.Net.ClientFactory` version; plan upgrade to 2.64.0+ if necessary.
    2. Check Application Insights SDK version; upgrade to >=2.23.0 or ensure AI is registered before resilience handlers.
    3. Prefer named `HttpClient` registrations and centralize resilience pipeline configuration (use `ResilienceExtensions` in repo).
    4. Add unit/integration tests that create named HttpClients and verify retry/hedging behavior against a test handler.
    5. Add a runtime smoke-test to validate AI telemetry presence after enabling resilience.
  - Quick verification steps (local/CI):
    - Create a test host registering AI and resilience in both orders to assert telemetry is not dropped.
    - Start a local gRPC test server and create a client registered with `AddStandardResilienceHandler()` to assert no runtime exceptions.
  - SARAH: include links to known issues and PR text examples for repo maintainers.
---
