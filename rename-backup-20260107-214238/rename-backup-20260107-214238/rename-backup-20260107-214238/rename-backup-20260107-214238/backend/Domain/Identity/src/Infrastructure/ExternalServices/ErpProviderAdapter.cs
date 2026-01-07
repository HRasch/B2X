using B2Connect.Identity.Interfaces;
using B2Connect.Identity.Models;

namespace B2Connect.Identity.Infrastructure.ExternalServices;

/// <summary>
/// Adapter zwischen IErpProvider und IErpCustomerService
/// Ermöglicht es, neue IErpProvider-Implementierungen mit bestehendem Code zu verwenden
/// ohne breaking changes zu verursachen
/// </summary>
public class ErpProviderAdapter : IErpCustomerService
{
    private readonly IErpProvider _provider;
    private readonly ILogger<ErpProviderAdapter> _logger;

    public ErpProviderAdapter(IErpProvider provider, ILogger<ErpProviderAdapter> logger)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        _logger = logger;
    }

    public Task<ErpCustomerDto?> GetCustomerByNumberAsync(string customerNumber, CancellationToken ct = default)
    {
        _logger.LogInformation("GetCustomerByNumber via provider {Provider}: {CustomerNumber}",
            _provider.ProviderName, customerNumber);
        return _provider.GetCustomerByNumberAsync(customerNumber, ct);
    }

    public Task<ErpCustomerDto?> GetCustomerByEmailAsync(string email, CancellationToken ct = default)
    {
        _logger.LogInformation("GetCustomerByEmail via provider {Provider}: {Email}",
            _provider.ProviderName, email);
        return _provider.GetCustomerByEmailAsync(email, ct);
    }

    public Task<ErpCustomerDto?> GetCustomerByCompanyNameAsync(string companyName, CancellationToken ct = default)
    {
        _logger.LogInformation("GetCustomerByCompanyName via provider {Provider}: {CompanyName}",
            _provider.ProviderName, companyName);
        return _provider.GetCustomerByCompanyNameAsync(companyName, ct);
    }

    public Task<bool> IsAvailableAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("IsAvailable check via provider {Provider}", _provider.ProviderName);
        return _provider.IsAvailableAsync(ct);
    }

    public Task<ErpSyncStatusDto> GetSyncStatusAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("GetSyncStatus via provider {Provider}", _provider.ProviderName);
        return _provider.GetSyncStatusAsync(ct);
    }
}
