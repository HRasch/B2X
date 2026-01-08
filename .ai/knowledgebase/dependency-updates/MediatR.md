---
docid: KB-089
title: MediatR
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

---
title: MediatR
current_version: "14.0.0"
source_files:
  - backend/Directory.Packages.props
status: reviewed
created_by: SARAH
created_at: 2025-12-30

summary: |
  MediatR is an in-process mediator library used for request/response and notification patterns. SARAH checked NuGet metadata and release notes for the latest stable release.

findings: |
  - NuGet current stable: `MediatR` 14.0.0
  - Notable: `MediatR` 14 targets .NET 8/modern runtimes and continues to provide a lightweight in-process mediator with registration helpers for `IServiceCollection`.
  - There is a contracts-only `MediatR.Contracts` package useful for splitting contracts and handlers into separate projects.
  - No breaking changes documented in the high-level NuGet description for a simple upgrade from prior 13.x line, but always review the project's GitHub releases/changelog for detailed notes when bumping major versions.

actions: |
  - If repo currently references `MediatR` older than `14.0.0`, plan a patch/minor bump to `14.0.0` and run tests.
  - Verify DI registration: `services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly...())` continues to work; run a smoke test that resolves `IMediator` and dispatches a simple request.
  - If using `MediatR.Contracts` split, ensure projects reference the correct contracts package to avoid runtime binding issues.
  - SARAH: add changelog link and any release-specific notes to this doc if a full migration PR is required.
---
