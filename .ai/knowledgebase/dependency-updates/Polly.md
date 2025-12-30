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
  - Documentation & changelog on GitHub: https://github.com/App-vNext/Polly
  - Polly remains stable; Microsoft.Extensions.Http.Resilience builds on Polly-style strategies—ensure versions are compatible when composing custom handlers.

actions: |
  - If upgrading Polly, ensure `Microsoft.Extensions.*` packages that depend on Polly are compatible; prefer aligning Polly and Microsoft.Extensions.Http.Resilience versions.
  - Run unit/integration tests that cover retry and circuit-breaker scenarios to detect semantic changes.
  - SARAH: include direct changelog links for the exact versions used in the repo when preparing PR descriptions.
---
