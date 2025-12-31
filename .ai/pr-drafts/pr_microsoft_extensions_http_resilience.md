---
title: "chore(deps): bump Microsoft.Extensions.Http.Resilience to 10.1.0"
branch: chore/pr-microsoft-extensions-http-resilience-10.1.0
labels: [dependencies, infra, needs-review]
assignees: []
---

Summary
-------

This PR updates `Microsoft.Extensions.Http.Resilience` to `10.1.0` across the repository where referenced.

Why
---

- Aligns with the latest stable resilience primitives from `dotnet/extensions`.
- Provides the `AddStandardResilienceHandler` and hedging improvements and bug fixes.

What changed
------------

- Update `Directory.Packages.props` (and any project-level references) to `10.1.0`.

Compatibility notes / known issues
--------------------------------

- gRPC clients: If the repo uses `Grpc.Net.ClientFactory`, ensure it is >= `2.64.0` to avoid runtime exceptions when using standard handlers. If upgrading `Grpc.Net.ClientFactory` is not possible, consider suppressing the build-time check as documented in the package notes.
- Application Insights: if using `Microsoft.ApplicationInsights` SDK <= `2.22.0`, register AI before resilience handlers or upgrade AI to >= `2.23.0` to avoid missing telemetry.

Verification checklist (local / CI)
--------------------------------

1. Run unit tests: `dotnet test -v minimal` (watch for test failures related to transient-handling expectations)
2. Run integration smoke tests for HTTP clients that use resilience handlers (see `tests/Http.Resilience.Smoke` or equivalent):

```bash
# Start any local dependencies required by the tests (e.g., test servers)
dotnet test tests/Http.Resilience.Smoke --filter "Category=ResilienceSmoke" -v minimal
```

3. If repo uses gRPC, run a small runtime verification against a local gRPC test server to confirm no `InvalidOperationException` when registering gRPC clients with the standard handler.
4. Verify Application Insights telemetry is present in staging when resilience is enabled (start app with staging config and assert AI metrics/logs exist).

PR body template
---------------

This PR updates `Microsoft.Extensions.Http.Resilience` to `10.1.0`.

Changes:
- Bumped package version in `Directory.Packages.props` (and checked project references).

Compatibility notes:
- gRPC: ensure `Grpc.Net.ClientFactory` >= `2.64.0` or avoid `AddStandardResilienceHandler()` on gRPC clients.
- Application Insights: register AI before resilience or upgrade to >= `2.23.0`.

Verification steps performed:
- Unit tests run: [CI link / local run results here]
- Resilience smoke tests: [link or results]

Rollout plan:
- Merge to `develop` (or main branch) and deploy to staging with canary rollout (10%). Monitor error rates and telemetry for 24 hours.
- If errors increase, revert via documented rollback plan.

Reviewers: @Backend, @DevOps, @Security
