using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace B2X.Identity.Tests.Integration;

/// <summary>
/// Base class for integration tests with WebApplicationFactory setup
/// </summary>
public class IntegrationTestFixture : IAsyncLifetime
{
    protected WebApplicationFactory<Program> Factory { get; private set; }
    protected HttpClient Client { get; private set; }

    public async Task InitializeAsync()
    {
        Factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                // Configure test environment
                builder.UseSetting("Environment", "Testing");
            });

        Client = Factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            HandleCookies = false
        });

        // Wait for service startup
        await Task.Delay(100);
    }

    public async Task DisposeAsync()
    {
        Client?.Dispose();
        Factory?.Dispose();
        await Task.CompletedTask;
    }

    /// <summary>
    /// Helper to make authenticated requests with JWT token
    /// </summary>
    protected HttpRequestMessage CreateAuthenticatedRequest(
        HttpMethod method,
        string uri,
        string token = null,
        string tenantId = null)
    {
        var request = new HttpRequestMessage(method, uri);

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Add("Authorization", $"Bearer {token}");
        }

        if (!string.IsNullOrEmpty(tenantId))
        {
            request.Headers.Add("X-Tenant-ID", tenantId);
        }

        request.Headers.Add("X-Request-ID", Guid.NewGuid().ToString());

        return request;
    }

    /// <summary>
    /// Helper to make POST request with JSON body
    /// </summary>
    protected async Task<T> PostAsJsonAsync<T>(
        string uri,
        object content,
        string token = null,
        string tenantId = null)
    {
        using var request = CreateAuthenticatedRequest(HttpMethod.Post, uri, token, tenantId);
        request.Content = JsonContent.Create(content);

        using var response = await Client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<T>();
        return result!;
    }

    /// <summary>
    /// Helper to make GET request
    /// </summary>
    protected async Task<T> GetAsync<T>(
        string uri,
        string token = null,
        string tenantId = null)
    {
        using var request = CreateAuthenticatedRequest(HttpMethod.Get, uri, token, tenantId);
        using var response = await Client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<T>();
        return result!;
    }
}

/// <summary>
/// Collection fixture for sharing WebApplicationFactory across tests
/// </summary>
[CollectionDefinition("Integration Tests")]
public class IntegrationTestCollection : ICollectionFixture<IntegrationTestFixture>
{
    // This class has no code, it just defines the collection
}
