---
title: Serilog
current_version: 4.3.0
source_files:
  - Directory.Packages.props
  - backend/Directory.Packages.props
status: reviewed
created_by: SARAH
created_at: 2025-12-30

summary: |
  Serilog is the structured logging library used widely in .NET apps. Upgrading may require verifying compatibility of sinks (e.g., Seq, Application Insights, File sinks).

findings: |
  - NuGet current stable: `Serilog` 4.3.0
  - Release notes: https://github.com/serilog/serilog/releases
  - Core changes are rare, but sinks/enrichers may require updates; check `Serilog.AspNetCore`, `Serilog.Sinks.File`, `Serilog.Sinks.Console`, and any cloud sinks used in the repo.

actions: |
  - Audit installed Serilog sinks/enrichers listed in `Directory.Packages.props` and verify compatibility with Serilog 4.x; bump sink versions in a follow-up PR if necessary.
  - Add a smoke test that starts the app and asserts logs are emitted to configured sinks (console/file/remote) and that structured properties are preserved.
  - CI suggestion: add a lightweight test that initializes Serilog with the project's sink configuration and emits test events, asserting no exceptions and that sink-specific formatting is present.
  - SARAH: gather sink-specific migration notes (e.g., Serilog.Sinks.Seq, Serilog.Sinks.ApplicationInsights) and add links to this doc for PR context.
---
