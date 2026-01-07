using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace B2Connect.Shared.Infrastructure.ServiceClients;

/// <summary>
/// Client for communication with the Localization service
/// Uses Aspire Service Discovery to resolve "http://localization-service" automatically
/// </summary>
public interface ILocalizationServiceClient
{
    Task<Dictionary<string, string>?> GetTranslationsAsync(string locale, Guid tenantId, CancellationToken ct = default);
    Task<string?> GetTranslationAsync(string key, string locale, Guid tenantId, CancellationToken ct = default);
}

public class LocalizationServiceClient : ILocalizationServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<LocalizationServiceClient> _logger;

    public LocalizationServiceClient(HttpClient httpClient, ILogger<LocalizationServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<Dictionary<string, string>?> GetTranslationsAsync(string locale, Guid tenantId, CancellationToken ct = default)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Add("X-Tenant-ID", tenantId.ToString());
            var response = await _httpClient.GetAsync($"/api/translations/{locale}", ct);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Dictionary<string, string>>(cancellationToken: ct);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to get translations for locale {Locale}", locale);
            return null;
        }
    }

    public async Task<string?> GetTranslationAsync(string key, string locale, Guid tenantId, CancellationToken ct = default)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Add("X-Tenant-ID", tenantId.ToString());
            var response = await _httpClient.GetAsync($"/api/translations/{locale}/{key}", ct);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<TranslationResult>(cancellationToken: ct);
            return result?.Value;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to get translation for key {Key}, locale {Locale}", key, locale);
            return null;
        }
    }
}

public record TranslationResult(string Key, string Value, string Locale);
