using System.Text.Json;
using System.Text.Json.Serialization;

namespace B2Connect.CLI.Services;

/// <summary>
/// HTTP Client für CLI-Commands zur Kommunikation mit Microservices
/// </summary>
public sealed class CliHttpClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private bool _disposed;

    public CliHttpClient(string baseUrl)
    {
        _httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public void AddHeader(string name, string value)
    {
        _httpClient.DefaultRequestHeaders.Add(name, value);
    }

    public void SetAuthorizationToken(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<ApiResponse<T>> GetAsync<T>(string endpoint) where T : class
    {
        try
        {
            var response = await _httpClient.GetAsync(endpoint);
            return await HandleResponse<T>(response);
        }
        catch (Exception ex)
        {
            return ApiResponse<T>.CreateError($"Request failed: {ex.Message}");
        }
    }

    public async Task<ApiResponse<T>> PostAsync<T>(string endpoint, object? payload = null) where T : class
    {
        try
        {
            var content = payload == null
                ? null
                : new StringContent(
                    JsonSerializer.Serialize(payload, _jsonOptions),
                    System.Text.Encoding.UTF8,
                    "application/json");

            var response = await _httpClient.PostAsync(endpoint, content);
            return await HandleResponse<T>(response);
        }
        catch (Exception ex)
        {
            return ApiResponse<T>.CreateError($"Request failed: {ex.Message}");
        }
    }

    public async Task<ApiResponse<string>> PostAsync(string endpoint, object? payload = null)
    {
        try
        {
            var content = payload == null
                ? null
                : new StringContent(
                    JsonSerializer.Serialize(payload, _jsonOptions),
                    System.Text.Encoding.UTF8,
                    "application/json");

            var response = await _httpClient.PostAsync(endpoint, content);
            var body = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return ApiResponse<string>.CreateSuccess(body);
            }

            return ApiResponse<string>.CreateError($"Error: {response.StatusCode} - {body}");
        }
        catch (Exception ex)
        {
            return ApiResponse<string>.CreateError($"Request failed: {ex.Message}");
        }
    }

    public async Task<ApiResponse<T>> PutAsync<T>(string endpoint, object payload) where T : class
    {
        try
        {
            var content = new StringContent(
                JsonSerializer.Serialize(payload, _jsonOptions),
                System.Text.Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PutAsync(endpoint, content);
            return await HandleResponse<T>(response);
        }
        catch (Exception ex)
        {
            return ApiResponse<T>.CreateError($"Request failed: {ex.Message}");
        }
    }

    public async Task<ApiResponse<string>> DeleteAsync(string endpoint)
    {
        try
        {
            var response = await _httpClient.DeleteAsync(endpoint);
            var body = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return ApiResponse<string>.CreateSuccess(body);
            }

            return ApiResponse<string>.CreateError($"Error: {response.StatusCode} - {body}");
        }
        catch (Exception ex)
        {
            return ApiResponse<string>.CreateError($"Request failed: {ex.Message}");
        }
    }

    private async Task<ApiResponse<T>> HandleResponse<T>(HttpResponseMessage response) where T : class
    {
        var body = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            try
            {
                var data = JsonSerializer.Deserialize<T>(body, _jsonOptions);
                return ApiResponse<T>.CreateSuccess(data ?? throw new InvalidOperationException("Empty response"));
            }
            catch (JsonException ex)
            {
                return ApiResponse<T>.CreateError($"Failed to parse response: {ex.Message}");
            }
        }

        return ApiResponse<T>.CreateError($"Error: {response.StatusCode} - {body}");
    }
    /// <summary>
    /// Disposes of the HTTP client resources.
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            _httpClient.Dispose();
            _disposed = true;
        }
    }}

/// <summary>
/// Standardisierte API Response Struktur
/// </summary>
public class ApiResponse<T> where T : class
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("data")]
    public T? Data { get; set; }

    [JsonPropertyName("error")]
    public string? Error { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    public static ApiResponse<T> CreateSuccess(T data) => new()
    {
        Success = true,
        Data = data
    };

    public static ApiResponse<T> CreateError(string error, string? message = null) => new()
    {
        Success = false,
        Error = error,
        Message = message
    };
}
