---
docid: KB-148
title: Patterns Antipatterns
owner: @DocMaintainer
status: Active
created: 2026-01-08
updated: 2026-01-09
---

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

Nuxt 3 Framework
- Patterns:
  - Leverage file-based routing and auto-imports for clean, convention-over-configuration development.
  - Use server-side rendering (SSR) for SEO-critical pages and static generation (SSG) for content-heavy sites.
  - Create composables for shared logic across pages/components; use Nuxt's auto-import feature extensively.
  - Implement proper error boundaries with Nuxt's error pages and `useError()` composable.
  - Use Nuxt modules for reusable functionality and third-party integrations.
  - Optimize images and assets with Nuxt's built-in image optimization and lazy loading.
  - Use `useHead()` and `useSeoMeta()` for declarative head management instead of manual DOM manipulation.
  - Implement proper data fetching with `useAsyncData()` and `useFetch()` for reactive, cached data.
  - Use `<NuxtLink>` for internal navigation to leverage prefetching and improve performance.
  - Structure components with clear separation: pages for routing, components for UI, composables for logic.
  - Use HOST=0.0.0.0 for built servers in testing environments to ensure external accessibility.
  - Validate server connectivity before running e2e tests to prevent false failures.
  - Access composables through nuxtApp parameter in plugins for proper context access.
- Antipatterns:
  - Mixing client-side and server-side logic inappropriately (e.g., accessing browser APIs in server-side code).
  - Manually managing meta tags and head elements instead of using Nuxt's head composables.
  - Overusing plugins for simple logic that belongs in composables or page components.
  - Ignoring hydration mismatches between server and client rendering.
  - Not leveraging Nuxt's built-in features like auto-imports, leading to verbose import statements.
  - Performing heavy computations in setup functions without proper async handling or lazy loading.
  - Using traditional Vue Router navigation instead of Nuxt's file-based routing system.
  - Not handling loading and error states properly in data fetching composables.
  - Placing business logic directly in components instead of extracting to composables.
  - Ignoring Nuxt's performance optimizations like payload extraction and code splitting.
  - Assuming localhost binding works for external test runners in e2e testing.
  - Ignoring PostCSS warnings that may indicate configuration issues with Tailwind/DaisyUI.

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

Code Quality & Legacy Migration (Pilot Phase 2 - Jan 2026)
- Patterns:
  - Interface-first approach: Create proper TypeScript interfaces for component props/settings before implementation (`{ComponentName}Settings`)
  - Replace `any` types in tests with typed interfaces for better maintainability and IntelliSense
  - Systematic cleanup: Remove unused imports and variables immediately when identified by linters
  - Prettier + ESLint automation: Run formatters and linters in pre-commit hooks to prevent style drift
  - Data-driven prioritization: Use impact analysis scripts to identify most critical files for migration
- Antipatterns:
  - Leaving `any` types in production code or tests - always create proper interfaces
  - Ignoring ESLint warnings in legacy code - establish exceptions only for truly blocking issues
  - Manual style fixes - automate formatting and enforce via CI/CD
  - Uncontrolled technical debt accumulation - implement regular code quality audits

Playwright E2E Testing
- Patterns:
  - Use locators with user-facing attributes (getByRole, getByLabel) over fragile CSS selectors
  - Chain and filter locators for precise element targeting
  - Use web-first assertions (toBeVisible, toHaveText) that auto-wait and retry
  - Test against built/production server for realistic testing environment
  - Bind built servers to 0.0.0.0 for external test runner accessibility
  - Validate server connectivity before running tests to avoid false failures
  - Use setup projects for shared authentication state across tests
  - Isolate tests with proper beforeEach/afterEach hooks and clean contexts
  - Prefer soft assertions for collecting multiple failures without stopping execution
  - Generate locators with codegen tool or VS Code extension for resilient selectors
- Antipatterns:
  - Testing implementation details instead of user-visible behavior
  - Using manual assertions without awaiting expect results
  - Running tests against dev server with hot reload that may cause timing issues
  - Assuming localhost server binding works for external test runners
  - Not validating server health before test execution
  - Using XPath or complex CSS selectors that break with DOM changes
  - Testing third-party dependencies without proper mocking
  - Not isolating test data and state between test runs

Code Organization & Refactoring
- Patterns:
  - Establish file size limits (500 LOC max for complex logic files) and class count limits (1-2 related classes per file).
  - Use systematic extraction for large monolithic files: create separate files first, then remove from original file.
  - Implement base classes for common patterns (e.g., ValidatedBase<T> for validation logic) to eliminate duplication.
  - Maintain build and test validation after each refactoring step to ensure stability.
  - Document refactoring patterns and triggers in team guidelines for consistent application.
- Antipatterns:
  - Allowing files to grow beyond 1000 LOC without splitting into focused components.
  - Multiple classes in single files creating complexity hotspots and maintenance challenges.
  - Copy-paste development leading to duplicate validation or business logic patterns.
  - Large-scale refactoring without incremental validation and testing checkpoints.
  - Lack of clear file organization guidelines leading to inconsistent project structure.

.NET Project Structure & References
- Patterns:
  - Use consistent relative path patterns based on directory depth: test projects in `tests/` require 5 levels up (`../../../../../src/`) to reach project root
  - Include `/src/` prefix in all project reference paths within solution folders (e.g., `../../../../src/shared/Domain/Tenancy/B2X.Tenancy.API.csproj`)
  - Maintain complete package references for frameworks: EF requires both runtime (`Microsoft.EntityFrameworkCore`) and relational (`Microsoft.EntityFrameworkCore.Relational`) packages plus provider packages
  - Explicitly reference extension packages even in ASP.NET Core context: `Microsoft.Extensions.Logging` and `Microsoft.Extensions.Configuration` needed for DI services
  - Validate project references against solution file structure to prevent broken builds
- Antipatterns:
  - Inconsistent path patterns causing "project does not exist" errors (missing `/src/` prefixes)
  - Incomplete package references leading to missing type errors (Migration, ILogger<>, IConfiguration)
  - Assuming ASP.NET Core projects automatically include all extension packages
  - Not validating individual project builds before solution-wide compilation

References
- Wolverine README & docs, Vue docs, Pinia docs, Vite docs, ASP.NET Core Identity docs, .NET compatibility docs, OWASP Top Ten.
