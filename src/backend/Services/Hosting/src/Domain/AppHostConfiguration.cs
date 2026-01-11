namespace B2X.Hosting.Domain;

/// <summary>
/// Configuration for AppHost services.
/// </summary>
public class AppHostConfiguration
{
    /// <summary>
    /// The base URL for the AppHost service.
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// The timeout for AppHost operations.
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Whether to enable health checks.
    /// </summary>
    public bool EnableHealthChecks { get; set; } = true;
}