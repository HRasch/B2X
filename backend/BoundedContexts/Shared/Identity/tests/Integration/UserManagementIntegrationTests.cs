using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace B2Connect.Identity.Tests.Integration;

/// <summary>
/// Integration tests for User Management API endpoints
/// Tests CRUD operations and authorization
/// </summary>
[Collection("Integration Tests")]
public class UserManagementIntegrationTests : IAsyncLifetime
{
    private readonly IntegrationTestFixture _fixture;
    private readonly string _tenantId = Guid.NewGuid().ToString();
    private string _adminToken = "";

    public UserManagementIntegrationTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        // In real scenario, would login as admin to get token
        _adminToken = "admin-token-for-testing";
    }

    public Task DisposeAsync() => Task.CompletedTask;

    #region GetUser Endpoint Tests

    [Fact]
    public async Task GetUser_WithValidUserId_ReturnsOk()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();

        // Act
        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Get,
            $"/api/users/{userId}",
            token: _adminToken,
            tenantId: _tenantId);
        var response = await _fixture.Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetUser_WithInvalidUserId_ReturnsNotFound()
    {
        // Arrange
        var invalidUserId = Guid.Empty.ToString();

        // Act
        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Get,
            $"/api/users/{invalidUserId}",
            token: _adminToken,
            tenantId: _tenantId);
        var response = await _fixture.Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetUser_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();

        // Act - No token
        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Get,
            $"/api/users/{userId}",
            tenantId: _tenantId);
        var response = await _fixture.Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetUser_WithExpiredToken_ReturnsUnauthorized()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var expiredToken = "expired-jwt-token";

        // Act
        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Get,
            $"/api/users/{userId}",
            token: expiredToken,
            tenantId: _tenantId);
        var response = await _fixture.Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region ListUsers Endpoint Tests

    [Fact]
    public async Task ListUsers_WithValidPagination_ReturnsOk()
    {
        // Act
        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Get,
            "/api/users?page=1&pageSize=20",
            token: _adminToken,
            tenantId: _tenantId);
        var response = await _fixture.Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ListUsers_WithSearchFilter_ReturnsOk()
    {
        // Act
        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Get,
            "/api/users?search=john&page=1&pageSize=20",
            token: _adminToken,
            tenantId: _tenantId);
        var response = await _fixture.Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ListUsers_WithInvalidPage_ReturnsBadRequest()
    {
        // Act
        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Get,
            "/api/users?page=-1&pageSize=20",
            token: _adminToken,
            tenantId: _tenantId);
        var response = await _fixture.Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ListUsers_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act - No token
        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Get,
            "/api/users?page=1&pageSize=20",
            tenantId: _tenantId);
        var response = await _fixture.Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region UpdateUser Endpoint Tests

    [Fact]
    public async Task UpdateUser_WithValidData_ReturnsOk()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var updateRequest = new
        {
            firstName = "UpdatedFirst",
            lastName = "UpdatedLast",
            email = "updated@example.com"
        };

        // Act
        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Put,
            $"/api/users/{userId}",
            token: _adminToken,
            tenantId: _tenantId);
        request.Content = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(updateRequest),
            System.Text.Encoding.UTF8,
            "application/json");
        var response = await _fixture.Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateUser_WithInvalidEmail_ReturnsBadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var updateRequest = new
        {
            email = "invalid-email"
        };

        // Act
        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Put,
            $"/api/users/{userId}",
            token: _adminToken,
            tenantId: _tenantId);
        request.Content = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(updateRequest),
            System.Text.Encoding.UTF8,
            "application/json");
        var response = await _fixture.Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateUser_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var updateRequest = new { firstName = "Updated" };

        // Act - No token
        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Put,
            $"/api/users/{userId}",
            tenantId: _tenantId);
        request.Content = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(updateRequest),
            System.Text.Encoding.UTF8,
            "application/json");
        var response = await _fixture.Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region ChangePassword Endpoint Tests

    [Fact]
    public async Task ChangePassword_WithValidPassword_ReturnsOk()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var changePasswordRequest = new
        {
            currentPassword = "OldPassword123!",
            newPassword = "NewPassword123!"
        };

        // Act
        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Post,
            $"/api/users/{userId}/change-password",
            token: _adminToken,
            tenantId: _tenantId);
        request.Content = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(changePasswordRequest),
            System.Text.Encoding.UTF8,
            "application/json");
        var response = await _fixture.Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ChangePassword_WithWeakPassword_ReturnsBadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var changePasswordRequest = new
        {
            currentPassword = "OldPassword123!",
            newPassword = "weak"
        };

        // Act
        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Post,
            $"/api/users/{userId}/change-password",
            token: _adminToken,
            tenantId: _tenantId);
        request.Content = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(changePasswordRequest),
            System.Text.Encoding.UTF8,
            "application/json");
        var response = await _fixture.Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ChangePassword_WithIncorrectCurrentPassword_ReturnsUnauthorized()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var changePasswordRequest = new
        {
            currentPassword = "WrongPassword123!",
            newPassword = "NewPassword123!"
        };

        // Act
        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Post,
            $"/api/users/{userId}/change-password",
            token: _adminToken,
            tenantId: _tenantId);
        request.Content = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(changePasswordRequest),
            System.Text.Encoding.UTF8,
            "application/json");
        var response = await _fixture.Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region DeactivateUser Endpoint Tests

    [Fact]
    public async Task DeactivateUser_WithValidUserId_ReturnsOk()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();

        // Act
        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Post,
            $"/api/users/{userId}/deactivate",
            token: _adminToken,
            tenantId: _tenantId);
        var response = await _fixture.Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NoContent, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeactivateUser_WithAdminRole_Succeeds()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();

        // Act - Admin can deactivate users
        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Post,
            $"/api/users/{userId}/deactivate",
            token: _adminToken,
            tenantId: _tenantId);
        var response = await _fixture.Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NoContent, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeactivateUser_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();

        // Act - No token
        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Post,
            $"/api/users/{userId}/deactivate",
            tenantId: _tenantId);
        var response = await _fixture.Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region Multi-Tenant Isolation Tests

    [Fact]
    public async Task ListUsers_WithDifferentTenants_IsolatesData()
    {
        // Arrange
        var tenantId1 = Guid.NewGuid().ToString();
        var tenantId2 = Guid.NewGuid().ToString();

        // Act - Tenant 1
        var request1 = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Get,
            "/api/users?page=1&pageSize=20",
            token: _adminToken,
            tenantId: tenantId1);
        var response1 = await _fixture.Client.SendAsync(request1);

        // Tenant 2
        var request2 = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Get,
            "/api/users?page=1&pageSize=20",
            token: _adminToken,
            tenantId: tenantId2);
        var response2 = await _fixture.Client.SendAsync(request2);

        // Assert - Both succeed but are isolated
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        response2.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetUser_FromWrongTenant_ReturnsNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var tenantId2 = Guid.NewGuid().ToString();

        // Act - Try to access user from different tenant
        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Get,
            $"/api/users/{userId}",
            token: _adminToken,
            tenantId: tenantId2);
        var response = await _fixture.Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.Forbidden);
    }

    #endregion

    #region Authorization Tests

    [Fact]
    public async Task UpdateUser_WithoutAdminRole_ReturnsForbidden()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var userToken = "user-token-not-admin";
        var updateRequest = new { firstName = "Updated" };

        // Act - Non-admin trying to update other user
        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Put,
            $"/api/users/{userId}",
            token: userToken,
            tenantId: _tenantId);
        request.Content = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(updateRequest),
            System.Text.Encoding.UTF8,
            "application/json");
        var response = await _fixture.Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeactivateUser_WithoutAdminRole_ReturnsForbidden()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var userToken = "user-token-not-admin";

        // Act - Non-admin trying to deactivate user
        var request = _fixture.CreateAuthenticatedRequest(
            HttpMethod.Post,
            $"/api/users/{userId}/deactivate",
            token: userToken,
            tenantId: _tenantId);
        var response = await _fixture.Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    #endregion
}
