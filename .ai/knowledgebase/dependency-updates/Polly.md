---
title: Polly
current_version: 8.6.5
source_files:
  - Directory.Packages.props
status: reviewed
created_by: SARAH
created_at: 2025-12-30

summary: |
  Polly is a mature .NET resilience and transient-fault-handling library that provides Retry, Circuit Breaker, Timeout, Rate-limiting, Hedging and Fallback policies. It is widely used under Microsoft.Extensions.Http.Resilience pipelines.

findings: |
  - NuGet current stable: `Polly` 8.6.5
  - Documentation & changelog: https://github.com/App-vNext/Polly (see `CHANGELOG.md` for per-release notes)
  - Polly is mature and backward-compatible in most cases, but changes to defaults (timeouts/retry jitter) can affect behavior—test policies under load.
  - `Microsoft.Extensions.Http.Resilience` builds on Polly-style strategies; ensure version alignment when composing handlers.

actions: |
  - When preparing a PR to bump Polly:
    1. Verify dependent packages (e.g., `Microsoft.Extensions.Http.Resilience`) are compatible with the target Polly version.
    2. Add/execute unit tests that simulate transient failures (timeouts, 500s) to assert retry and circuit-breaker semantics.
    3. Run a short load test to observe retry/backoff behaviour and guard for increased downstream load.
  - CI checks: include a job that runs the resilience unit tests and a small integration scenario that hits a stubbed failing endpoint.
  - SARAH: link to the specific Polly changelog entries for the repo's current and target versions to include in PR descriptions.
---
