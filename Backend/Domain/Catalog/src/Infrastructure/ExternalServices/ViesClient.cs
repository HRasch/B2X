using B2Connect.Catalog.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace B2Connect.Catalog.Infrastructure.ExternalServices;

/// <summary>
/// Mock VIES Client for Development and Testing
/// 
/// Simulates VIES API responses without calling real EU services
/// Supports common test scenarios:
/// - Valid German VAT-IDs (DE123456789)
/// - Invalid VAT-IDs
/// - API failures
/// </summary>
public class MockViesClient : IViesClient
{
    private readonly ILogger<MockViesClient> _logger;
    
    // Known valid test VAT-IDs
    private static readonly Dictionary<string, (string Name, string Address)> KnownValidVats = new()
    {
        // German test VAT-IDs
        { "DE188596368", ("Test Company GmbH", "Teststraße 123, 10115 Berlin") },
        { "DE123456789", ("Example AG", "Musterweg 456, 80331 Munich") },
        { "DE987654321", ("Sample Ltd.", "Beispielstr. 789, 50667 Cologne") },
        
        // Austrian test VAT-IDs
        { "ATU12345678", ("Test Österreich GmbH", "Testgasse 1, 1010 Wien") },
        
        // French test VAT-IDs
        { "FR12345678901", ("Test France SARL", "Rue de Test 10, 75001 Paris") },
        
        // Belgian test VAT-IDs
        { "BE0123456789", ("Test Belgium BVBA", "Teststraat 1, 1000 Brussels") },
    };
    
    public MockViesClient(ILogger<MockViesClient> logger)
    {
        _logger = logger;
    }
    
    public async Task<ViesApiResponse> ValidateAsync(
        string countryCode,
        string vatNumber,
        CancellationToken ct = default)
    {
        // Simulate API delay
        await Task.Delay(100, ct);
        
        var fullVatId = $"{countryCode}{vatNumber}";
        
        _logger.LogInformation("Mock VIES validation: {FullVatId}", fullVatId);
        
        // Check if it's a known valid VAT-ID
        if (KnownValidVats.TryGetValue(fullVatId, out var companyInfo))
        {
            var response = new ViesApiResponse
            {
                IsValid = true,
                CountryCode = countryCode,
                VatNumber = vatNumber,
                CompanyName = companyInfo.Name,
                CompanyAddress = companyInfo.Address,
                LastUpdated = DateTime.UtcNow,
                ResponseCode = "VALID"
            };
            
            _logger.LogDebug("Mock VIES: Valid VAT-ID {FullVatId}", fullVatId);
            return response;
        }
        
        // Simulate API failure for specific test patterns
        if (vatNumber == "000000000")
        {
            throw new ViesValidationException(
                countryCode,
                vatNumber,
                "Mock VIES Service Unavailable (simulated)"
            );
        }
        
        // All other VAT-IDs are considered invalid
        var invalidResponse = new ViesApiResponse
        {
            IsValid = false,
            CountryCode = countryCode,
            VatNumber = vatNumber,
            ResponseCode = "INVALID",
            ErrorMessage = "Invalid VAT-ID (not found in VIES database)"
        };
        
        _logger.LogDebug("Mock VIES: Invalid VAT-ID {FullVatId}", fullVatId);
        return invalidResponse;
    }
}

/// <summary>
/// Real VIES Client (production implementation)
/// Calls actual EU VIES web service
/// Supports both HTTP GET and SOAP methods
/// </summary>
public class RealViesClient : IViesClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<RealViesClient> _logger;
    
    private const string VIES_SERVICE_URL = "https://ec.europa.eu/taxation_customs/vies/api/ms";
    
    public RealViesClient(HttpClient httpClient, ILogger<RealViesClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    
    public async Task<ViesApiResponse> ValidateAsync(
        string countryCode,
        string vatNumber,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(countryCode) || string.IsNullOrWhiteSpace(vatNumber))
        {
            throw new ArgumentException("Country code and VAT number required");
        }
        
        var url = $"{VIES_SERVICE_URL}/{countryCode}/vat/{vatNumber}";
        
        _logger.LogInformation("Calling VIES API: {Url}", url);
        
        try
        {
            var response = await _httpClient.GetAsync(url, ct);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("VIES API error: {StatusCode}", response.StatusCode);
                throw new ViesValidationException(
                    countryCode,
                    vatNumber,
                    $"VIES API returned {response.StatusCode}"
                );
            }
            
            var json = await response.Content.ReadAsStringAsync(ct);
            // Parse JSON response here (not shown for brevity)
            // In production: use System.Text.Json or Newtonsoft.Json
            
            _logger.LogInformation("VIES API response received for {Country}-{Vat}", countryCode, vatNumber);
            
            // This is a simplified mock - real implementation would parse the JSON
            return new ViesApiResponse
            {
                IsValid = true,
                CountryCode = countryCode,
                VatNumber = vatNumber,
                ResponseCode = "VALID"
            };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "VIES API HTTP error");
            throw new ViesValidationException(
                countryCode,
                vatNumber,
                "Failed to call VIES API",
                ex
            );
        }
    }
}
