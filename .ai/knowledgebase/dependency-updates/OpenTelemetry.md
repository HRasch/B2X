---
title: OpenTelemetry (core/collectors/exporters)
current_version: "1.14.0 (SDK and OTLP exporter snapshot)"
source_files:
  - Directory.Packages.props
  - backend/Directory.Packages.props
status: reviewed
created_by: SARAH
created_at: 2025-12-30

summary: |
  Multiple OpenTelemetry .NET packages are used across the repository (SDK, OTLP exporter, instrumentations). SARAH researched NuGet package pages and verified current stable versions for the core packages.

findings: |
  - `OpenTelemetry` SDK: 1.14.0 (NuGet)
  - `OpenTelemetry.Exporter.OpenTelemetryProtocol` (OTLP exporter): 1.14.0 (NuGet)
  - Key notes:
    - `UseOtlpExporter()` convenience method enables exporters for all signals but must be called only once; mixing it with signal-specific `AddOtlpExporter` calls can throw `NotSupportedException`.
    - OTLP exporter supports both gRPC and HTTP/protobuf transports; when using `HttpProtobuf` you can supply a custom `HttpClient` via an `HttpClientFactory` option.
    - The exporter exposes many environment-variable configuration knobs (`OTEL_*`) for endpoints, headers, timeouts, batching and attribute limits.
    - Experimental features (in-memory/disk retry) exist; exercise caution and validate stability when enabling.

actions: |
  - If repo references older OpenTelemetry packages, update to `1.14.0` for SDK/exporter where safe and run CI.
  - For any upgrade, verify initialization order (Application Insights vs OpenTelemetry), test OTLP endpoint connectivity, and confirm `UseOtlpExporter` usage does not conflict.
  - Add a small integration test that configures OTLP exporter with a test `HttpClient` and asserts startup/configuration does not throw.
  - SARAH: capture changelog links and breaking-change notes into this doc for each package before opening PRs.
---

References:

- https://www.nuget.org/packages/OpenTelemetry
- https://www.nuget.org/packages/OpenTelemetry.Exporter.OpenTelemetryProtocol

Status: reviewed (research snapshot)
---
title: OpenTelemetry (core/collectors/exporters)
current_version: "various (1.9.0, 1.14.0, 1.11.2, etc.)"
source_files:
  - Directory.Packages.props
  - backend/Directory.Packages.props
status: pending-research
created_by: SARAH
created_at: 2025-12-30

summary: |
  Multiple OpenTelemetry packages are used. Consolidated doc created; SARAH should enumerate each package and research current stable versions and compatibility.

findings: |
  - pending

actions: |
  - pending SARAH research
---
