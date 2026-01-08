---
docid: KB-095
title: OpenTelemetry
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

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
  - Documentation: https://github.com/open-telemetry/opentelemetry-dotnet and https://opentelemetry.io/
  - Key notes:
    - `UseOtlpExporter()` convenience method enables exporters for all signals but must be called only once; mixing it with signal-specific `AddOtlpExporter` calls can throw `NotSupportedException`.
    - OTLP exporter supports both gRPC and HTTP/protobuf transports; when using `HttpProtobuf` you can provide a custom `HttpClient` via the `HttpClientFactory` option (OTLP options expose `HttpClientFactory`).
    - The exporter exposes many environment-variable configuration knobs (`OTEL_*`) for endpoints, headers, timeouts, batching and attribute limits.
    - Experimental features (in-memory/disk retry) exist (see OTLP exporter docs); test thoroughly before enabling in production.
    - `UseOtlpExporter` will add the exporter as the last processor in the pipeline and can only be called once — ensure application startup calls don't conflict.

actions: |
  - If repo references older OpenTelemetry packages, update to `1.14.0` for SDK/exporter where safe and run CI with focused integration checks.
  - For upgrades, verify initialization order (Application Insights vs OpenTelemetry), confirm `UseOtlpExporter` is not called multiple times, and test OTLP endpoint connectivity (gRPC and HttpProtobuf).
  - Add integration tests:
    1. Startup test ensuring `AddOpenTelemetry().UseOtlpExporter()` does not throw.
    2. OTLP connectivity test using a local collector (or OTLP test double) to assert traces/metrics/logs are transmitted.
    3. Custom `HttpClient` test when using `HttpProtobuf` to confirm `HttpClientFactory` wiring and headers.
  - CI suggestion: add a matrix job that runs OTLP tests using `docker-compose` to start an OpenTelemetry Collector and validates telemetry ingestion.
  - SARAH: include changelog links for `OpenTelemetry` and `OpenTelemetry.Exporter.OpenTelemetryProtocol` and note any breaking changes between the repo's current versions and `1.14.0`.
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
