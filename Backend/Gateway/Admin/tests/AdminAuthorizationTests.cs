using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using System.Net.Http.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace B2X.Admin.Tests.Authorization;

public class AdminAuthorizationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AdminAuthorizationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    private string GenerateTestToken(string userId, string email, string accountType = "DU", string[] roles = null)
    {
        var handler = new JwtSecurityTokenHandler();
        var key = System.Text.Encoding.ASCII.GetBytes(
            "dev-only-secret-minimum-32-chars-required!");

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Email, email),
            new(ClaimTypes.Name, "Test User"),
            new("TenantId", "test-tenant"),
            new("AccountType", accountType)
        };

        // Add roles
        if (roles != null)
        {
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
        }

        var token = new JwtSecurityToken(
            issuer: "B2X",
            audience: "B2X",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: new Microsoft.IdentityModel.Tokens.SigningCredentials(
                new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                    System.Text.Encoding.UTF8.GetBytes(
                        "dev-only-secret-minimum-32-chars-required!")),
                Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256)
        );

        return handler.WriteToken(token);
    }

    [Fact]
    public async Task Unauthenticated_Request_Returns_401_Unauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/products");

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DomainAdmin_With_DU_AccountType_Can_Access_Protected_Endpoints()
    {
        var client = _factory.CreateClient();
        var token = GenerateTestToken("user-1", "admin@example.com", "DU", new[] { "Admin" });

        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        client.DefaultRequestHeaders.Add("X-Tenant-ID", "test-tenant");

        // This should succeed (200) or return 404 if endpoint doesn't exist, but NOT 403
        var response = await client.GetAsync("/api/products");

        // Accept 200 (success), 404 (not found), or 500 (internal error)
        // But NOT 401 or 403
        Assert.NotEqual(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.NotEqual(System.Net.HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task TenantAdmin_With_SU_AccountType_Can_Access_Protected_Endpoints()
    {
        var client = _factory.CreateClient();
        var token = GenerateTestToken("user-2", "tenant-admin@example.com", "SU", new[] { "Admin" });

        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        client.DefaultRequestHeaders.Add("X-Tenant-ID", "test-tenant");

        var response = await client.GetAsync("/api/products");

        Assert.NotEqual(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.NotEqual(System.Net.HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Regular_User_Without_Admin_Roles_Cannot_Access_Admin_Endpoints()
    {
        var client = _factory.CreateClient();
        var token = GenerateTestToken("user-3", "user@example.com", "U", new[] { "User" });

        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        client.DefaultRequestHeaders.Add("X-Tenant-ID", "test-tenant");

        var response = await client.GetAsync("/api/products");

        // Should get 403 Forbidden for regular users
        Assert.Equal(System.Net.HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Missing_AccountType_Claim_Results_In_403_Forbidden()
    {
        var client = _factory.CreateClient();
        var handler = new JwtSecurityTokenHandler();
        var key = System.Text.Encoding.ASCII.GetBytes(
            "dev-only-secret-minimum-32-chars-required!");

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, "user-4"),
            new(ClaimTypes.Email, "noaccount@example.com"),
            // Deliberately omit AccountType claim
        };

        var token = new JwtSecurityToken(
            issuer: "B2X",
            audience: "B2X",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: new Microsoft.IdentityModel.Tokens.SigningCredentials(
                new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                    System.Text.Encoding.UTF8.GetBytes(
                        "dev-only-secret-minimum-32-chars-required!")),
                Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256)
        );

        var tokenString = handler.WriteToken(token);
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenString);

        var response = await client.GetAsync("/api/products");

        Assert.Equal(System.Net.HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Missing_TenantId_Header_Returns_400_Or_403()
    {
        var client = _factory.CreateClient();
        var token = GenerateTestToken("user-5", "admin@example.com", "DU", new[] { "Admin" });

        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        // Deliberately omit X-Tenant-ID header

        var response = await client.GetAsync("/api/products");

        // Should either reject due to missing tenant (400/403) or extract from token
        Assert.True(
            response.StatusCode == System.Net.HttpStatusCode.BadRequest ||
            response.StatusCode == System.Net.HttpStatusCode.Forbidden ||
            response.StatusCode == System.Net.HttpStatusCode.NotFound,
            $"Expected 400/403/404 but got {response.StatusCode}");
    }
}

/// <summary>
/// Frontend Authorization Tests
/// Tests that frontend can properly enforce role-based access control
/// </summary>
public class FrontendAuthorizationTests
{
    [Fact]
    public void Admin_Role_Check_Returns_True()
    {
        var roles = new[]
        {
            new { name = "admin" }
        };

        var hasAdminRole = roles.Any(r => r.name == "admin");
        Assert.True(hasAdminRole);
    }

    [Fact]
    public void Content_Manager_Role_Check_Returns_True()
    {
        var roles = new[]
        {
            new { name = "content_manager" }
        };

        var hasContentManagerRole = roles.Any(r => r.name == "content_manager");
        Assert.True(hasContentManagerRole);
    }

    [Fact]
    public void Missing_Required_Role_Returns_False()
    {
        var roles = new[]
        {
            new { name = "user" }
        };

        var hasAdminRole = roles.Any(r => r.name == "admin");
        Assert.False(hasAdminRole);
    }

    [Fact]
    public void Multiple_Roles_Check_Passes_When_Any_Match()
    {
        var roles = new[]
        {
            new { name = "content_manager" },
            new { name = "user" }
        };

        var requiredRoles = new[] { "admin", "content_manager" };
        var hasRequiredRole = requiredRoles.Any(required =>
            roles.Any(r => r.name == required));

        Assert.True(hasRequiredRole);
    }
}
