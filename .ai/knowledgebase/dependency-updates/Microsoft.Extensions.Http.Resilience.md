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

actions: |
  - Verify backend projects that register resilience handlers are not creating gRPC client runtime errors; test gRPC clients if present.
  - Ensure Application Insights registration ordering or upgrade AI if present.
  - Prefer named `HttpClient` registrations and central pipeline configuration; wire the repo's `ResilienceExtensions` accordingly and run CI.
---
