---
docid: KB-111
title: Dotnet Features
owner: @DocMaintainer
status: Active
created: 2026-01-08
updated: 2026-01-09
---

Title: .NET features (summary focused on .NET 10)
Source: https://learn.microsoft.com/dotnet/core/whats-new

Overview:
- This file summarizes notable runtime, library, and SDK changes introduced across recent .NET releases with focus on .NET 10 (LTS).
- Latest version: .NET 10.0.101 (released December 9, 2025)

Highlights (NET 10):
- Runtime: JIT inlining and method devirtualization improvements, AVX10.2 support, better codegen for struct arguments, improved loop inversion, and NativeAOT enhancements.
- Libraries: new and improved APIs in cryptography (post-quantum crypto primitives and CNG improvements), globalization, serialization (JSON strict options, duplicate-property handling, PipeReader support), networking (WebSocketStream, TLS improvements on macOS), and diagnostics.
- ASP.NET Core: Minimal API improvements, Blazor performance and WebAssembly preloading, form validation enhancements, Passkey support in identity flows, and improved diagnostics.
- EF Core: LINQ improvements, performance work, Azure Cosmos DB enhancements, named query filters.
- SDK & tooling: `dotnet` CLI updates, improved `dotnet test` with Microsoft.Testing.Platform integration, container image publishing improvements, native AOT workflows, CLI inspection flags (`--cli-schema`), and improved tooling for file-based apps.

Actionables for projects:
- Pick an appropriate TargetFrameworkMoniker (TFM) such as `net10.0` to access new APIs.
- Test native AOT build paths and container image outputs if using AOT or containerized deployments.
- Review JSON serializer options if strict serialization or duplicate-property handling matters for your interop/clients.
- For ASP.NET projects, validate Passkey and identity changes against your auth flows and test WebAssembly preloading for Blazor workloads.
- **Package Dependencies**: Ensure complete EF Core references - include both `Microsoft.EntityFrameworkCore` (runtime) and `Microsoft.EntityFrameworkCore.Relational` (migrations) plus provider packages like `Npgsql.EntityFrameworkCore.PostgreSQL`
- **Extension Packages**: Explicitly reference `Microsoft.Extensions.Logging` and `Microsoft.Extensions.Configuration` even in ASP.NET Core projects for ILogger<> and IConfiguration usage
- **Project References**: Use consistent relative paths with `/src/` prefixes in solution folder structures to prevent "project does not exist" compilation errors

References:
- .NET whats-new: https://learn.microsoft.com/dotnet/core/whats-new
- .NET 10 overview: https://learn.microsoft.com/dotnet/core/whats-new/dotnet-10/overview
- ASP.NET Core 10 notes: https://learn.microsoft.com/aspnet/core/release-notes/aspnetcore-10.0
- Latest download: https://dotnet.microsoft.com/en-us/download/dotnet/10.0
