---
docid: KB-112
title: Dotnet Identity
owner: @DocMaintainer
status: Active
created: 2026-01-08
---


Title: ASP.NET Core Identity — summary
Source: https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity

Summary:
- ASP.NET Core Identity provides user and role management, password hashing, token providers, and optional UI scaffolding for web apps. It integrates with EF Core by default but can use custom stores.

Typical configuration (high level):
- Register services in `Program.cs`: add DbContext, `AddIdentity`/`AddDefaultIdentity`, configure `IdentityOptions`, and `ConfigureApplicationCookie` for cookie settings.
- Ensure middleware order: `UseRouting()`, `UseAuthentication()`, `UseAuthorization()`.

Security & operational best practices:
- Enforce strong password policies and require confirmed accounts for production systems.
- Enable account lockout on repeated failed attempts and configure reasonable lockout windows.
- Secure cookies: set `HttpOnly`, `Secure`, and appropriate `SameSite` policy; use HTTPS in production.
- Use data protection keys with key-rotation/backups for token and cookie encryption; consider centralized key storage for multi-instance deployments.
- Protect against username enumeration in auth endpoints and log suspicious activities.
- Consider external providers (Microsoft Entra, Azure AD B2C, Duende IdentityServer) for SSO and OAuth/OIDC scenarios.
- Use MFA/TOTP for higher security; Identity includes token providers to enable TOTP and recovery codes.

Operational notes:
- Scaffold Identity pages if you need to customize registration/login flows.
- Monitor Identity metrics (failed logins, account lockouts) and integrate with logging/alerts.

References:
- Identity docs: https://learn.microsoft.com/aspnet/core/security/authentication/identity
- Scaffold identity: https://learn.microsoft.com/aspnet/core/security/authentication/scaffold-identity

