using FluentAssertions;
using System.Net;
using Xunit;

namespace B2Connect.Tests.Integration;

/// <summary>
/// Integration tests that use the Aspire test host with automatic port discovery.
/// </summary>
[Collection("AspireApp")]
public class GatewayHealthTests(AspireAppFixture fixture)
{
    [Fact]
    public async Task StoreGateway_IsRunning()
    {
        // Arrange - HttpClient is pre-configured with correct port by Aspire
        using var client = fixture.StoreGatewayClient;

        // Act & Assert - Gateway should be reachable (may return any status code, but not connection error)
        try
        {
            var response = await client.GetAsync("/");
            // If we get here, the gateway is running and responded with some status code
            Assert.True(true, "Store Gateway is running and responded");
        }
        catch (HttpRequestException ex) when (ex.Message.Contains("Connection refused"))
        {
            // Gateway is not running
            Assert.Fail("Store Gateway is not running - connection refused");
        }
        catch (TaskCanceledException)
        {
            // Timeout - gateway is not responding
            Assert.Fail("Store Gateway is not responding - timeout");
        }
    }

    [Fact]
    public async Task AdminGateway_IsRunning()
    {
        // Arrange - Admin gateway has tenant resolution middleware
        using var client = fixture.AdminGatewayClient;

        // Act & Assert - Gateway should be reachable
        try
        {
            var response = await client.GetAsync("/");
            // If we get here, the gateway is running
            Assert.True(true, "Admin Gateway is running and responded");
        }
        catch (HttpRequestException ex) when (ex.Message.Contains("Connection refused"))
        {
            // Gateway is not running
            Assert.Fail("Admin Gateway is not running - connection refused");
        }
        catch (TaskCanceledException)
        {
            // Timeout - gateway is not responding
            Assert.Fail("Admin Gateway is not responding - timeout");
        }
    }

    [Fact]
    public async Task CatalogService_IsRunning()
    {
        // Arrange
        using var client = fixture.CatalogServiceClient;

        // Act
        var response = await client.GetAsync("/health");

        // Assert - Service should respond with health endpoint
        response.StatusCode.Should().Be(HttpStatusCode.OK, "Catalog service should have /health endpoint");
    }
}
