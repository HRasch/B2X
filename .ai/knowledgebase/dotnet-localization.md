
Title: ASP.NET Core Localization & Globalization — summary
Source: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/localization

Summary:
- ASP.NET Core supports globalization and localization with services and middleware: `IStringLocalizer`, `IHtmlLocalizer`, resource (.resx) files, and `RequestLocalizationMiddleware` to set culture per request.

Key concepts & tasks:
- Make content localizable: use `IStringLocalizer<T>` or `IStringLocalizer` to avoid hard-coded strings.
- Resource files: store translations in `.resx` files following the naming convention `Resources.{culture}.resx` or use class-based resource organization.
- Culture selection: configure `RequestLocalizationOptions` with supported cultures and providers (query string, cookie, Accept-Language header, or route data).
- Formatting: use `CultureInfo` to format dates, numbers, currencies according to the current culture.

Best practices / Actionables:
- Provide a fallback culture and an explicit list of supported cultures.
- Avoid concatenating localized strings; use format placeholders to preserve grammar and pluralization.
- Test Right-to-Left languages (RTL) and UI layout for languages with different text direction.
- Keep translation files up-to-date; consider a translation workflow or a service (Crowdin, Lokalise) for larger teams.
- Use caching and resource preloading prudently to avoid performance issues.

Integration notes:
- For Blazor, consult Blazor-specific localization guidance (Blazor globalization/localization docs).
- For client-side frameworks (Vue), separate the server-side/localization pipeline from frontend i18n libraries; provide translation exports or localization API endpoints.

References:
- Localization docs: https://learn.microsoft.com/aspnet/core/fundamentals/localization

