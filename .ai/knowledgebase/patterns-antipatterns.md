Title: Patterns & Antipatterns — cross-cutting guidance
Source: curated from official docs and best practices (Microsoft, OWASP, Vue, Pinia, JasperFx)

Overview
- Short, actionable patterns and antipatterns for the main technologies used in this repo: message-driven (.NET/Wolverine), frontend (Vue/Pinia/Vite), authentication (ASP.NET Identity), localization, and .NET library design.

Messaging (Wolverine & CQRS)
- Patterns:
  - Use explicit message contracts (DTOs) and version them.
  - Prefer idempotent handlers; make handlers safe to retry.
  - Use Outbox pattern when coupling DB changes and messages to guarantee at-least-once delivery without duplicates.
  - Separate commands (write intent) from events (notifications) and queries (read-only).
- Antipatterns:
  - Tight coupling between message handlers and HTTP controllers or UI code.
  - Performing long-running synchronous work inside request handlers instead of scheduling background work.
  - Relying on eventual consistency without compensating or reconciliation strategies.

Frontend (Vue, Pinia, Vite)
- Patterns:
  - Composition API + composables for reusable logic and testability.
  - Domain stores in Pinia (one store per domain concept) and keep actions small and pure where possible.
  - Lazy-load routes and code-split heavy components to reduce initial bundle size.
  - Use Vite dev server and plugin ecosystem; configure `optimizeDeps` for problematic packages.
- Antipatterns:
  - Storing secrets or large binary blobs in client-side state or persisted stores.
  - Putting heavy business logic inside components instead of stores/composables.
  - Mutating shared state directly from multiple places without clear ownership.

Authentication & Identity (ASP.NET Core Identity)
- Patterns:
  - Enforce confirmed accounts, strong password policies, and MFA for sensitive flows.
  - Use cookie hardening (`HttpOnly`, `Secure`, `SameSite`) and rotate data-protection keys in multi-instance deployments.
  - Prefer delegating auth to proven providers (Entra/Azure AD, IdentityServer/Duende) for complex SSO scenarios.
- Antipatterns:
  - Rolling your own cryptography or token formats instead of using platform-provided APIs.
  - Exposing verbose error messages from authentication endpoints that enable username enumeration.

Localization & Globalization
- Patterns:
  - Use `IStringLocalizer`/resource files for server-side strings; separate client translations and export as needed.
  - Determine culture per request via route/cookie/Accept-Language and provide explicit fallbacks.
  - Use format placeholders and avoid concatenating localized fragments.
- Antipatterns:
  - Keeping translations inline in code or concatenating localized pieces (breaks grammar and pluralization).
  - Ignoring RTL/LTR differences and failing to test layout for RTL languages.

.NET library design & compatibility
- Patterns:
  - Favor additive changes (new APIs) over changing existing signatures; use `[Obsolete]` and migration helpers.
  - Run API-compatibility checks and document breaking changes clearly in release notes.
  - Use TFMs and `SupportedOSPlatformVersion` properly; include platform guards for platform-specific APIs.
- Antipatterns:
  - Renaming or removing public types/members without providing compatibility shims or migration guidance.
  - Changing synchronous APIs to async without a migration plan or compatibility wrapper.

How to use this file
- Use this document as a quick checklist when proposing changes, reviewing PRs, or designing new features. Add project-specific examples and link to the detailed knowledgebase pages for each technology.

References
- Wolverine README & docs, Vue docs, Pinia docs, Vite docs, ASP.NET Core Identity docs, .NET compatibility docs, OWASP Top Ten.
