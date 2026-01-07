using System.Text.Json;
using B2Connect.Identity.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace B2Connect.Identity.Services;

/// <summary>
/// Mock/Demo ERP Service für Entwicklung und Testing
/// Produktiv: würde gegen SAP OData API gehen
/// </summary>
public class ErpCustomerService : IErpCustomerService
{
    private readonly HttpClient _httpClient;
    private readonly IDistributedCache _cache;
    private readonly ILogger<ErpCustomerService> _logger;
    private readonly string _erpBaseUrl;
    private readonly string _erpApiKey;
    private const string CacheKeyPrefix = "erp:customer:";
    private const int CacheDurationMinutes = 60;

    public ErpCustomerService(
        HttpClient httpClient,
        IDistributedCache cache,
        ILogger<ErpCustomerService> logger,
        string erpBaseUrl = "http://localhost:8100",
        string erpApiKey = "demo-key")
    {
        _httpClient = httpClient;
        _cache = cache;
        _logger = logger;
        _erpBaseUrl = erpBaseUrl;
        _erpApiKey = erpApiKey;
    }

    public async Task<ErpCustomerDto?> GetCustomerByNumberAsync(string customerNumber, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(customerNumber))
        {
            return null;
        }

        var cacheKey = $"{CacheKeyPrefix}number:{customerNumber}";
        var cached = await _cache.GetStringAsync(cacheKey, ct).ConfigureAwait(false);
        if (cached != null)
        {
            _logger.LogDebug("ERP Cache HIT für Kundennummer: {CustomerNumber}", customerNumber);
            return JsonSerializer.Deserialize<ErpCustomerDto>(cached);
        }

        try
        {
            _logger.LogDebug("ERP Lookup: Kundennummer {CustomerNumber}", customerNumber);

            // OData Query beispielsweise: /Customers?$filter=CustomerNumber eq '12345'
            using var request = new HttpRequestMessage(HttpMethod.Get,
                $"{_erpBaseUrl}/api/customers?$filter=CustomerNumber eq '{Uri.EscapeDataString(customerNumber)}'");
            request.Headers.Add("Authorization", $"Bearer {_erpApiKey}");

            using var response = await _httpClient.SendAsync(request, ct).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("ERP API Error: {StatusCode} für Kundennummer {CustomerNumber}",
                    response.StatusCode, customerNumber);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
            var customers = JsonSerializer.Deserialize<List<ErpCustomerDto>>(content) ?? new();
            var customer = customers.FirstOrDefault();

            if (customer != null)
            {
                // Cache für 60 Min
                await _cache.SetStringAsync(
                    cacheKey,
                    JsonSerializer.Serialize(customer),
                    new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheDurationMinutes) },
                    ct).ConfigureAwait(false);

                _logger.LogInformation("ERP Kunde gefunden: {CustomerNumber} -> {CustomerName}", customerNumber, customer.CustomerName);
            }

            return customer;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ERP Service Error bei GetCustomerByNumberAsync: {CustomerNumber}", customerNumber);
            return null;
        }
    }

    public async Task<ErpCustomerDto?> GetCustomerByEmailAsync(string email, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return null;
        }

        var cacheKey = $"{CacheKeyPrefix}email:{email.ToLower(System.Globalization.CultureInfo.CurrentCulture)}";
        var cached = await _cache.GetStringAsync(cacheKey, ct).ConfigureAwait(false);
        if (cached != null)
        {
            _logger.LogDebug("ERP Cache HIT für E-Mail: {Email}", email);
            return JsonSerializer.Deserialize<ErpCustomerDto>(cached);
        }

        try
        {
            _logger.LogDebug("ERP Lookup: E-Mail {Email}", email);

            using var request = new HttpRequestMessage(HttpMethod.Get,
                $"{_erpBaseUrl}/api/customers?$filter=Email eq '{Uri.EscapeDataString(email)}'");
            request.Headers.Add("Authorization", $"Bearer {_erpApiKey}");

            using var response = await _httpClient.SendAsync(request, ct).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
            var customers = JsonSerializer.Deserialize<List<ErpCustomerDto>>(content) ?? new();
            var customer = customers.FirstOrDefault();

            if (customer != null)
            {
                await _cache.SetStringAsync(
                    cacheKey,
                    JsonSerializer.Serialize(customer),
                    new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheDurationMinutes) },
                    ct).ConfigureAwait(false);

                _logger.LogInformation("ERP Kunde gefunden: {Email} -> {CustomerName}", email, customer.CustomerName);
            }

            return customer;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ERP Service Error bei GetCustomerByEmailAsync: {Email}", email);
            return null;
        }
    }

    public async Task<ErpCustomerDto?> GetCustomerByCompanyNameAsync(string companyName, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(companyName))
        {
            return null;
        }

        var cacheKey = $"{CacheKeyPrefix}company:{companyName.ToLower(System.Globalization.CultureInfo.CurrentCulture)}";
        var cached = await _cache.GetStringAsync(cacheKey, ct).ConfigureAwait(false);
        if (cached != null)
        {
            _logger.LogDebug("ERP Cache HIT für Firmenname: {CompanyName}", companyName);
            return JsonSerializer.Deserialize<ErpCustomerDto>(cached);
        }

        try
        {
            _logger.LogDebug("ERP Lookup: Firmenname {CompanyName}", companyName);

            using var request = new HttpRequestMessage(HttpMethod.Get,
                $"{_erpBaseUrl}/api/customers?$filter=contains(CustomerName, '{Uri.EscapeDataString(companyName)}')");
            request.Headers.Add("Authorization", $"Bearer {_erpApiKey}");

            using var response = await _httpClient.SendAsync(request, ct).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
            var customers = JsonSerializer.Deserialize<List<ErpCustomerDto>>(content) ?? new();
            var customer = customers.FirstOrDefault();

            if (customer != null)
            {
                await _cache.SetStringAsync(
                    cacheKey,
                    JsonSerializer.Serialize(customer),
                    new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheDurationMinutes) },
                    ct).ConfigureAwait(false);

                _logger.LogInformation("ERP Kunde gefunden: {CompanyName} -> {CustomerNumber}", companyName, customer.CustomerNumber);
            }

            return customer;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ERP Service Error bei GetCustomerByCompanyNameAsync: {CompanyName}", companyName);
            return null;
        }
    }

    public async Task<bool> IsAvailableAsync(CancellationToken ct = default)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, $"{_erpBaseUrl}/health");
            request.Headers.Add("Authorization", $"Bearer {_erpApiKey}");

            using var response = await _httpClient.SendAsync(request, ct).ConfigureAwait(false);
            var available = response.IsSuccessStatusCode;

            _logger.LogInformation("ERP Health Check: {Status}", available ? "OK" : "FAILED");
            return available;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ERP Health Check fehlgeschlagen");
            return false;
        }
    }

    public async Task<ErpSyncStatusDto> GetSyncStatusAsync(CancellationToken ct = default)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, $"{_erpBaseUrl}/api/sync-status");
            request.Headers.Add("Authorization", $"Bearer {_erpApiKey}");

            using var response = await _httpClient.SendAsync(request, ct).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                return new ErpSyncStatusDto
                {
                    IsConnected = false,
                    Message = "ERP nicht erreichbar"
                };
            }

            var content = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
            var status = JsonSerializer.Deserialize<ErpSyncStatusDto>(content) ?? new();

            return status;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error beim Abrufen des ERP Sync Status");
            return new ErpSyncStatusDto
            {
                IsConnected = false,
                Message = $"Fehler: {ex.Message}"
            };
        }
    }
}
