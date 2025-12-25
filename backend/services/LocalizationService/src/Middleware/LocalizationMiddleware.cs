namespace B2Connect.LocalizationService.Middleware;

/// <summary>
/// Middleware for detecting and setting the current language from HTTP requests
/// </summary>
public class LocalizationMiddleware
{
    private readonly RequestDelegate _next;
    private const string DEFAULT_LANGUAGE = "en";

    public LocalizationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Invokes the middleware to detect and set language
    /// Priority: 1. Query param, 2. Header, 3. Cookie, 4. Default
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        // Priority order for language detection
        var language =
            context.Request.Query["lang"].FirstOrDefault() ??
            context.Request.Headers["Accept-Language"].FirstOrDefault() ??
            context.Request.Cookies["locale"] ??
            DEFAULT_LANGUAGE;

        // Normalize to 2-letter code (e.g., "de" from "de-DE")
        language = language.Substring(0, Math.Min(2, language.Length)).ToLower();

        // Store in HttpContext for access in services
        context.Items["Language"] = language;

        // Set response header to indicate language
        context.Response.Headers["Content-Language"] = language;

        // Set culture for the request
        System.Globalization.CultureInfo.CurrentCulture = new System.Globalization.CultureInfo(language);
        System.Globalization.CultureInfo.CurrentUICulture = new System.Globalization.CultureInfo(language);

        await _next(context);
    }
}
