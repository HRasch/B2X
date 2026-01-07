# Dependency upgrade candidates (scan: 2026-01-06)

Summary of candidate updates discovered by scan across the workspace.

NuGet highlights
- Dapper: 2.1.35 -> 2.1.66 (patch)
- EFCore.BulkExtensions: 8.1.3 -> 9.0.2 (major)
- Npgsql: 10.0.0 -> 10.0.1 (patch)
- Microsoft.Extensions.Caching.StackExchangeRedis: 10.0.0 -> 10.0.1 (patch)
- Microsoft.CodeAnalysis.Analyzers: 3.3.4 -> 3.11.0 (major/minor)
- Microsoft.CodeAnalysis.CSharp: 4.14.0 -> 5.0.0 (major)
- Testcontainers.*: 4.9.0 -> 4.10.0 (minor)
- coverlet.collector: 6.0.2 -> 6.0.4 (patch)

Notes:
- Some upgrades are major and may require code changes (e.g., EFCore.BulkExtensions, Roslyn/C# analyzers).
- I recommend applying patch/minor updates first, then major updates in a separate staged branch with CI/test verification.

Frontend (npm) highlights (frontend/):
- nuxt: 3.20.2 -> 4.2.2 (major)
- pinia: 2.3.1 -> 3.0.4 (major)
- tailwindcss: 3.4.19 -> 4.1.18 (major)
- date-fns: 3.6.0 -> 4.1.0 (major)
- @nuxtjs/i18n: 9.5.6 -> 10.2.1 (major)
- OpenTelemetry packages: large major version jumps

Recommendations / next steps
1. Create an isolated branch `deps/upgrade-minor` to apply only patch/minor updates (NuGet patch/minor + npm minor/patch). Run full test suite and e2e.
2. Create a second branch `deps/upgrade-major` for major upgrades (Nuxt 3->4, Tailwind 3->4, EFCore.BulkExtensions 8->9, Roslyn 4->5). Tackle each major upgrade separately, update code as required, and run tests.
3. Open PRs for each branch with CI enabled; review failing tests and fix iteratively.

If you want, I can start by applying patch/minor NuGet updates automatically in `deps/upgrade-minor` and run `dotnet build` + tests.

Generated: 2026-01-06
