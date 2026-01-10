using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace B2X.Shared.Infrastructure.ServiceClients;

/// <summary>
/// Client for communication with the Identity/Auth service
/// Uses Aspire Service Discovery to resolve "http://auth-service" automatically
/// </summary>
public interface IIdentityServiceClient
{
    Task<UserDto?> GetUserByIdAsync(Guid userId, CancellationToken ct = default);
    Task<bool> ValidateTokenAsync(string token, CancellationToken ct = default);
}

public class IdentityServiceClient : IIdentityServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<IdentityServiceClient> _logger;

    public IdentityServiceClient(HttpClient httpClient, ILogger<IdentityServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid userId, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/users/{userId}", ct).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<UserDto>(cancellationToken: ct).ConfigureAwait(false);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to get user {UserId} from Identity service", userId);
            return null;
        }
    }

    public async Task<bool> ValidateTokenAsync(string token, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/auth/validate", new { Token = token }, ct).ConfigureAwait(false);
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to validate token with Identity service");
            return false;
        }
    }
}

public record UserDto(Guid Id, string Email, string? FirstName, string? LastName);
