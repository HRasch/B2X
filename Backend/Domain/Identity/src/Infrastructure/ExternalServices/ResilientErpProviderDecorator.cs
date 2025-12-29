using B2Connect.Identity.Interfaces;
using B2Connect.Identity.Models;

namespace B2Connect.Identity.Infrastructure.ExternalServices;

/// <summary>
/// Decorator für ERP-Provider mit Fehlerbehandlung und Fallback-Logik
/// 
/// Funktioniert wie ein Proxy:
/// 1. Versucht Primary-Provider (z.B. SAP, Oracle)
/// 2. Bei Fehler: Fallback zum Fake-Provider
/// 3. Logged alle Fehler für Monitoring
/// 4. Ermöglicht Graceful Degradation
/// </summary>
public class ResilientErpProviderDecorator : IErpProvider
{
    private readonly IErpProvider _primaryProvider;
    private readonly IErpProvider _fallbackProvider;
    private readonly ILogger<ResilientErpProviderDecorator> _logger;

    public string ProviderName => $"{_primaryProvider.ProviderName} (with {_fallbackProvider.ProviderName} fallback)";

    public ResilientErpProviderDecorator(
        IErpProvider primaryProvider,
        IErpProvider fallbackProvider,
        ILogger<ResilientErpProviderDecorator> logger)
    {
        _primaryProvider = primaryProvider ?? throw new ArgumentNullException(nameof(primaryProvider));
        _fallbackProvider = fallbackProvider ?? throw new ArgumentNullException(nameof(fallbackProvider));
        _logger = logger;
    }

    public async Task<ErpCustomerDto?> GetCustomerByNumberAsync(string customerNumber, CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("[RESILIENT] Attempting GetCustomerByNumber via {Provider}",
                _primaryProvider.ProviderName);

            var result = await _primaryProvider.GetCustomerByNumberAsync(customerNumber, ct);
            if (result != null)
            {
                _logger.LogInformation("[RESILIENT] Successfully found customer via {Provider}",
                    _primaryProvider.ProviderName);
                return result;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "[RESILIENT] Primary provider {Provider} failed, falling back to {Fallback}",
                _primaryProvider.ProviderName, _fallbackProvider.ProviderName);

            try
            {
                return await _fallbackProvider.GetCustomerByNumberAsync(customerNumber, ct);
            }
            catch (Exception fallbackEx)
            {
                _logger.LogError(fallbackEx,
                    "[RESILIENT] Fallback provider {Fallback} also failed",
                    _fallbackProvider.ProviderName);
                throw;
            }
        }
    }

    public async Task<ErpCustomerDto?> GetCustomerByEmailAsync(string email, CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("[RESILIENT] Attempting GetCustomerByEmail via {Provider}",
                _primaryProvider.ProviderName);

            var result = await _primaryProvider.GetCustomerByEmailAsync(email, ct);
            if (result != null)
            {
                _logger.LogInformation("[RESILIENT] Successfully found customer via {Provider}",
                    _primaryProvider.ProviderName);
                return result;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "[RESILIENT] Primary provider {Provider} failed, falling back to {Fallback}",
                _primaryProvider.ProviderName, _fallbackProvider.ProviderName);

            try
            {
                return await _fallbackProvider.GetCustomerByEmailAsync(email, ct);
            }
            catch (Exception fallbackEx)
            {
                _logger.LogError(fallbackEx,
                    "[RESILIENT] Fallback provider {Fallback} also failed",
                    _fallbackProvider.ProviderName);
                throw;
            }
        }
    }

    public async Task<ErpCustomerDto?> GetCustomerByCompanyNameAsync(string companyName, CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("[RESILIENT] Attempting GetCustomerByCompanyName via {Provider}",
                _primaryProvider.ProviderName);

            var result = await _primaryProvider.GetCustomerByCompanyNameAsync(companyName, ct);
            if (result != null)
            {
                _logger.LogInformation("[RESILIENT] Successfully found customer via {Provider}",
                    _primaryProvider.ProviderName);
                return result;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "[RESILIENT] Primary provider {Provider} failed, falling back to {Fallback}",
                _primaryProvider.ProviderName, _fallbackProvider.ProviderName);

            try
            {
                return await _fallbackProvider.GetCustomerByCompanyNameAsync(companyName, ct);
            }
            catch (Exception fallbackEx)
            {
                _logger.LogError(fallbackEx,
                    "[RESILIENT] Fallback provider {Fallback} also failed",
                    _fallbackProvider.ProviderName);
                throw;
            }
        }
    }

    public async Task<bool> IsAvailableAsync(CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("[RESILIENT] Checking availability of {Provider}",
                _primaryProvider.ProviderName);

            var isAvailable = await _primaryProvider.IsAvailableAsync(ct);
            if (isAvailable)
            {
                _logger.LogInformation("[RESILIENT] {Provider} is available", _primaryProvider.ProviderName);
                return true;
            }

            _logger.LogWarning("[RESILIENT] {Provider} is not available", _primaryProvider.ProviderName);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "[RESILIENT] Failed to check availability of {Provider}",
                _primaryProvider.ProviderName);
            return false;
        }
    }

    public async Task<ErpSyncStatusDto> GetSyncStatusAsync(CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("[RESILIENT] Attempting GetSyncStatus via {Provider}",
                _primaryProvider.ProviderName);

            return await _primaryProvider.GetSyncStatusAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "[RESILIENT] Primary provider {Provider} failed getting sync status, using fallback",
                _primaryProvider.ProviderName);

            try
            {
                return await _fallbackProvider.GetSyncStatusAsync(ct);
            }
            catch (Exception fallbackEx)
            {
                _logger.LogError(fallbackEx, "[RESILIENT] Fallback provider also failed getting sync status");
                throw;
            }
        }
    }
}
