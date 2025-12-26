using B2Connect.CatalogService.Providers;
using Microsoft.AspNetCore.Mvc;

namespace B2Connect.CatalogService.Controllers;

/// <summary>
/// Product Provider Management API
/// Diagnostics and health checks for product data sources
/// </summary>
[ApiController]
[Route("api/v2/[controller]")]
[Produces("application/json")]
public class ProvidersController : ControllerBase
{
    private readonly IProductProviderRegistry _registry;
    private readonly IProductProviderResolver _resolver;
    private readonly ILogger<ProvidersController> _logger;

    public ProvidersController(
        IProductProviderRegistry registry,
        IProductProviderResolver resolver,
        ILogger<ProvidersController> logger)
    {
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Get health status of all product providers
    /// GET /api/v2/providers/health
    /// </summary>
    [HttpGet("health")]
    [ProducesResponseType(typeof(ProviderHealthResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProvidersHealth(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Fetching provider health status");

            var metadata = await _registry.GetAllMetadataAsync(cancellationToken);
            var primaryProvider = await _registry.GetPrimaryProviderAsync(cancellationToken);

            var response = new ProviderHealthResponse
            {
                Timestamp = DateTime.UtcNow,
                PrimaryProvider = primaryProvider?.ProviderName,
                Providers = metadata
                    .Select(kvp => new ProviderStatus
                    {
                        Name = kvp.Key,
                        IsConnected = kvp.Value.IsConnected,
                        Version = kvp.Value.Version,
                        LastSyncTime = kvp.Value.LastSyncTime,
                        Capabilities = kvp.Value.Capabilities
                    })
                    .ToList()
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking provider health");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get metadata for specific provider
    /// GET /api/v2/providers/{providerName}
    /// </summary>
    [HttpGet("{providerName}")]
    [ProducesResponseType(typeof(ProviderStatus), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProviderMetadata(
        string providerName,
        CancellationToken cancellationToken)
    {
        try
        {
            var provider = _registry.GetProvider(providerName);
            if (provider == null)
            {
                return NotFound(new { error = $"Provider '{providerName}' not found" });
            }

            var metadata = await provider.GetMetadataAsync(cancellationToken);
            var status = new ProviderStatus
            {
                Name = providerName,
                IsConnected = metadata.IsConnected,
                Version = metadata.Version,
                LastSyncTime = metadata.LastSyncTime,
                Capabilities = metadata.Capabilities
            };

            return Ok(status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching metadata for provider '{ProviderName}'", providerName);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { error = ex.Message });
        }
    }

    /// <summary>
    /// List all available providers
    /// GET /api/v2/providers
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListProviders(CancellationToken cancellationToken)
    {
        try
        {
            var providers = await _registry.GetProvidersInPriorityOrderAsync(cancellationToken);
            var providerNames = providers.Select(p => p.ProviderName).ToList();

            return Ok(new { providers = providerNames });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing providers");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { error = ex.Message });
        }
    }

    /// <summary>
    /// Test provider connectivity
    /// POST /api/v2/providers/{providerName}/test
    /// </summary>
    [HttpPost("{providerName}/test")]
    [ProducesResponseType(typeof(ConnectivityTestResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> TestProviderConnectivity(
        string providerName,
        CancellationToken cancellationToken)
    {
        try
        {
            var provider = _registry.GetProvider(providerName);
            if (provider == null)
            {
                return NotFound(new { error = $"Provider '{providerName}' not found" });
            }

            var sw = System.Diagnostics.Stopwatch.StartNew();
            var isConnected = await provider.VerifyConnectivityAsync(cancellationToken);
            sw.Stop();

            var result = new ConnectivityTestResult
            {
                Provider = providerName,
                IsConnected = isConnected,
                ResponseTimeMs = sw.ElapsedMilliseconds,
                TestedAt = DateTime.UtcNow
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error testing connectivity for provider '{ProviderName}'", providerName);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { error = ex.Message });
        }
    }
}

/// <summary>
/// Provider health response
/// </summary>
public class ProviderHealthResponse
{
    public DateTime Timestamp { get; set; }
    public string? PrimaryProvider { get; set; }
    public List<ProviderStatus> Providers { get; set; } = new();
}

/// <summary>
/// Provider status
/// </summary>
public class ProviderStatus
{
    public string Name { get; set; } = string.Empty;
    public bool IsConnected { get; set; }
    public string Version { get; set; } = string.Empty;
    public DateTime? LastSyncTime { get; set; }
    public ProviderCapabilities? Capabilities { get; set; }
}

/// <summary>
/// Connectivity test result
/// </summary>
public class ConnectivityTestResult
{
    public string Provider { get; set; } = string.Empty;
    public bool IsConnected { get; set; }
    public long ResponseTimeMs { get; set; }
    public DateTime TestedAt { get; set; }
}
