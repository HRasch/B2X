using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace B2Connect.Identity.Tests.Integration;

/// <summary>
/// Integration tests for Authentication API endpoints
/// Tests actual HTTP requests/responses with real dependency chain
/// </summary>
[Collection("Integration Tests")]
public class AuthenticationIntegrationTests : IAsyncLifetime
{
    private readonly IntegrationTestFixture _fixture;

    public AuthenticationIntegrationTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    public Task InitializeAsync() => Task.CompletedTask;
    public Task DisposeAsync() => Task.CompletedTask;

    #region Login Endpoint Tests

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkWithToken()
    {
        // Arrange
        var loginRequest = new
        {
            email = "test@example.com",
            password = "TestPassword123!"
        };

        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Post,
            "/api/auth/login");
        request.Content = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(loginRequest),
            System.Text.Encoding.UTF8,
            "application/json");

        // Act
        var response = await _fixture.Client.SendAsync(request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/json");
    }

    [Fact]
    public async Task Login_WithInvalidEmail_ReturnsBadRequest()
    {
        // Arrange
        var loginRequest = new
        {
            email = "",
            password = "TestPassword123!"
        };

        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Post,
            "/api/auth/login");
        request.Content = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(loginRequest),
            System.Text.Encoding.UTF8,
            "application/json");

        // Act
        var response = await _fixture.Client.SendAsync(request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new
        {
            email = "test@example.com",
            password = "WrongPassword123!"
        };

        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Post,
            "/api/auth/login");
        request.Content = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(loginRequest),
            System.Text.Encoding.UTF8,
            "application/json");

        // Act
        var response = await _fixture.Client.SendAsync(request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithMissingPassword_ReturnsBadRequest()
    {
        // Arrange
        var loginRequest = new { email = "test@example.com" };

        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Post,
            "/api/auth/login");
        request.Content = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(loginRequest),
            System.Text.Encoding.UTF8,
            "application/json");

        // Act
        var response = await _fixture.Client.SendAsync(request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Register Endpoint Tests

    [Fact]
    public async Task Register_WithValidInput_ReturnsCreated()
    {
        // Arrange
        var registerRequest = new
        {
            email = "newuser@example.com",
            password = "NewPassword123!",
            firstName = "John",
            lastName = "Doe"
        };

        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Post,
            "/api/auth/register");
        request.Content = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(registerRequest),
            System.Text.Encoding.UTF8,
            "application/json");

        // Act
        var response = await _fixture.Client.SendAsync(request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Register_WithExistingEmail_ReturnsConflict()
    {
        // Arrange
        var registerRequest = new
        {
            email = "existing@example.com",
            password = "NewPassword123!",
            firstName = "John",
            lastName = "Doe"
        };

        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Post,
            "/api/auth/register");
        request.Content = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(registerRequest),
            System.Text.Encoding.UTF8,
            "application/json");

        // Act
        var response = await _fixture.Client.SendAsync(request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Register_WithWeakPassword_ReturnsBadRequest()
    {
        // Arrange
        var registerRequest = new
        {
            email = "weakpass@example.com",
            password = "weak",
            firstName = "John",
            lastName = "Doe"
        };

        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Post,
            "/api/auth/register");
        request.Content = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(registerRequest),
            System.Text.Encoding.UTF8,
            "application/json");

        // Act
        var response = await _fixture.Client.SendAsync(request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_WithInvalidEmail_ReturnsBadRequest()
    {
        // Arrange
        var registerRequest = new
        {
            email = "invalid-email",
            password = "ValidPassword123!",
            firstName = "John",
            lastName = "Doe"
        };

        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Post,
            "/api/auth/register");
        request.Content = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(registerRequest),
            System.Text.Encoding.UTF8,
            "application/json");

        // Act
        var response = await _fixture.Client.SendAsync(request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    #endregion

    #region RefreshToken Endpoint Tests

    [Fact]
    public async Task RefreshToken_WithValidRefreshToken_ReturnsNewToken()
    {
        // Arrange
        var refreshTokenRequest = new
        {
            refreshToken = "valid-refresh-token-123"
        };

        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Post,
            "/api/auth/refresh");
        request.Content = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(refreshTokenRequest),
            System.Text.Encoding.UTF8,
            "application/json");

        // Act
        var response = await _fixture.Client.SendAsync(request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RefreshToken_WithExpiredRefreshToken_ReturnsUnauthorized()
    {
        // Arrange
        var refreshTokenRequest = new
        {
            refreshToken = "expired-refresh-token"
        };

        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Post,
            "/api/auth/refresh");
        request.Content = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(refreshTokenRequest),
            System.Text.Encoding.UTF8,
            "application/json");

        // Act
        var response = await _fixture.Client.SendAsync(request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RefreshToken_WithEmptyToken_ReturnsBadRequest()
    {
        // Arrange
        var refreshTokenRequest = new { refreshToken = "" };

        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Post,
            "/api/auth/refresh");
        request.Content = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(refreshTokenRequest),
            System.Text.Encoding.UTF8,
            "application/json");

        // Act
        var response = await _fixture.Client.SendAsync(request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Health Check Tests

    [Fact]
    public async Task HealthCheck_ReturnsOk()
    {
        // Act
        var response = await _fixture.Client.GetAsync("/health");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    #endregion

    #region Multi-Tenant Isolation Tests

    [Fact]
    public async Task Login_WithDifferentTenantIds_IsolatesResponses()
    {
        // Arrange
        var tenantId1 = Guid.NewGuid().ToString();
        var tenantId2 = Guid.NewGuid().ToString();

        var loginRequest = new
        {
            email = "test@example.com",
            password = "TestPassword123!"
        };

        var request1 = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Post,
            "/api/auth/login",
            tenantId: tenantId1);
        request1.Content = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(loginRequest),
            System.Text.Encoding.UTF8,
            "application/json");

        var request2 = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Post,
            "/api/auth/login",
            tenantId: tenantId2);
        request2.Content = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(loginRequest),
            System.Text.Encoding.UTF8,
            "application/json");

        // Act
        var response1 = await _fixture.Client.SendAsync(request1);
        var response2 = await _fixture.Client.SendAsync(request2);

        // Assert
        response1.StatusCode.ShouldBe(HttpStatusCode.OK);
        response2.StatusCode.ShouldBe(HttpStatusCode.OK);
        // Responses should be isolated per tenant
        response1.Headers.GetValues("X-Tenant-ID").ShouldContain(tenantId1);
        response2.Headers.GetValues("X-Tenant-ID").ShouldContain(tenantId2);
    }

    #endregion

    #region Error Response Format Tests

    [Fact]
    public async Task ErrorResponse_HasStandardFormat()
    {
        // Arrange
        var loginRequest = new { email = "", password = "" };

        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Post,
            "/api/auth/login");
        request.Content = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(loginRequest),
            System.Text.Encoding.UTF8,
            "application/json");

        // Act
        var response = await _fixture.Client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        content.ShouldContain("error") // Should contain error info
            .Or.Contain("errors")
            .Or.Contain("message");
    }

    #endregion
}
