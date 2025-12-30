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
  - Serilog core remains stable; ensure sinks and enrichment packages used in the repo are compatible with the core version.

actions: |
  - Audit installed Serilog sinks/enrichers listed in `Directory.Packages.props` and verify their compatibility with Serilog 4.x.
  - Run a smoke test that starts the app and verifies logs are emitted to configured sinks (console/file/remote) after upgrade.
  - SARAH: capture release notes for 4.3.0 and any sink-specific migration notes.
---
