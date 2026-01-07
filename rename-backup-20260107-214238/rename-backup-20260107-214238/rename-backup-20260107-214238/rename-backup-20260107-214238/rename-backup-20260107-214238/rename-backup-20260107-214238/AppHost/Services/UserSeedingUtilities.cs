using B2Connect.AppHost.Configuration;
using B2Connect.Shared.Core;
using Bogus;
using System.Net.Http.Json;

namespace B2Connect.AppHost.Services;

/// <summary>
/// Utilities for seeding user data in test environments.
/// </summary>
public static class UserSeedingUtilities
{
    /// <summary>
    /// Seeds users for all tenants in the test data context.
    /// </summary>
    public static async Task SeedUsersAsync(
        HttpClient authServiceClient,
        TestingConfiguration testingConfig,
        CancellationToken cancellationToken = default)
    {
        if (authServiceClient == null)
            throw new ArgumentNullException(nameof(authServiceClient));
        if (testingConfig == null)
            throw new ArgumentNullException(nameof(testingConfig));

        var faker = new Faker();
        var usersPerTenant = testingConfig.SeedData.UsersPerTenant;

        foreach (var tenantId in TestDataContext.Current.GetAllTenantIds())
        {
            await SeedUsersForTenantAsync(authServiceClient, tenantId, usersPerTenant, faker, cancellationToken);
        }

        // Ensure admin user exists for the default tenant
        await EnsureAdminUserAsync(authServiceClient, cancellationToken);
    }

    /// <summary>
    /// Seeds users for a specific tenant.
    /// </summary>
    private static async Task SeedUsersForTenantAsync(
        HttpClient client,
        Guid tenantId,
        int userCount,
        Faker faker,
        CancellationToken cancellationToken)
    {
        for (int i = 0; i < userCount; i++)
        {
            var userRequest = CreateUserRequest(faker, tenantId, i);
            await CreateUserAsync(client, userRequest, cancellationToken);
        }
    }

    /// <summary>
    /// Creates a user creation request with test data.
    /// </summary>
    private static UserCreationRequest CreateUserRequest(Faker faker, Guid tenantId, int index)
    {
        var firstName = faker.Name.FirstName();
        var lastName = faker.Name.LastName();
        var email = index == 0 ? $"user{index}@{GetTenantSlug(tenantId)}.example.com" : faker.Internet.Email();

        return new UserCreationRequest
        {
            Email = email,
            UserName = email,
            FirstName = firstName,
            LastName = lastName,
            Password = "Password123!", // Standard test password
            ConfirmPassword = "Password123!",
            TenantId = tenantId,
            Roles = index == 0 ? ["Admin"] : ["User"], // First user is admin
            IsActive = true,
            EmailConfirmed = true,
            PhoneNumber = faker.Phone.PhoneNumber(),
            // Mark as test data
            IsTestData = true,
            CreatedBy = "TestDataOrchestrator"
        };
    }

    /// <summary>
    /// Sends the user creation request to the auth service.
    /// </summary>
    private static async Task CreateUserAsync(
        HttpClient client,
        UserCreationRequest request,
        CancellationToken cancellationToken)
    {
        var response = await client.PostAsJsonAsync("/api/users", request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException(
                $"Failed to create user '{request.Email}': {response.StatusCode} - {errorContent}");
        }

        var createdUser = await response.Content.ReadFromJsonAsync<UserResponse>(cancellationToken: cancellationToken);
        if (createdUser != null)
        {
            // Store user ID for later reference
            TestDataContext.Current.AddUserToTenant(request.TenantId, createdUser.Id);
        }
    }

    /// <summary>
    /// Ensures the default admin user exists.
    /// </summary>
    private static async Task EnsureAdminUserAsync(
        HttpClient client,
        CancellationToken cancellationToken)
    {
        var adminRequest = new UserCreationRequest
        {
            Email = SeedConstants.DefaultAdminEmail,
            UserName = SeedConstants.DefaultAdminEmail,
            FirstName = "Admin",
            LastName = "User",
            Password = "AdminPassword123!",
            ConfirmPassword = "AdminPassword123!",
            TenantId = SeedConstants.DefaultTenantId,
            Roles = ["SuperAdmin", "Admin"],
            IsActive = true,
            EmailConfirmed = true,
            IsTestData = true,
            CreatedBy = "TestDataOrchestrator"
        };

        try
        {
            var response = await client.PostAsJsonAsync("/api/users", adminRequest, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var createdUser = await response.Content.ReadFromJsonAsync<UserResponse>(cancellationToken: cancellationToken);
                if (createdUser != null)
                {
                    TestDataContext.Current.AddUserToTenant(SeedConstants.DefaultTenantId, createdUser.Id);
                }
            }
            // If user already exists (409 Conflict), that's fine
        }
        catch (Exception ex)
        {
            // Log but don't fail - admin user might already exist
            Console.WriteLine($"Warning: Could not ensure admin user exists: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets the tenant slug for a tenant ID.
    /// </summary>
    private static string GetTenantSlug(Guid tenantId)
    {
        // For now, use a simple mapping. In a real implementation,
        // this would query the tenant service or use the TestDataContext
        if (tenantId == SeedConstants.DefaultTenantId)
        {
            return SeedConstants.DefaultTenantSlug;
        }

        return $"tenant-{tenantId.ToString().Substring(0, 8)}";
    }

    /// <summary>
    /// Resets all test users (for cleanup between test runs).
    /// </summary>
    public static async Task ResetUsersAsync(
        HttpClient authServiceClient,
        CancellationToken cancellationToken = default)
    {
        if (authServiceClient == null)
            throw new ArgumentNullException(nameof(authServiceClient));

        // Get all test users
        var response = await authServiceClient.GetAsync("/api/users?testDataOnly=true", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            // If endpoint doesn't exist or fails, skip reset
            return;
        }

        var users = await response.Content.ReadFromJsonAsync<List<UserResponse>>(cancellationToken: cancellationToken);

        if (users != null)
        {
            foreach (var user in users)
            {
                await authServiceClient.DeleteAsync($"/api/users/{user.Id}", cancellationToken);
            }
        }
    }
}

/// <summary>
/// Request model for user creation.
/// </summary>
public class UserCreationRequest
{
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public Guid TenantId { get; set; }
    public List<string> Roles { get; set; } = new();
    public bool IsActive { get; set; } = true;
    public bool EmailConfirmed { get; set; } = true;
    public string? PhoneNumber { get; set; }
    public bool IsTestData { get; set; }
    public string? CreatedBy { get; set; }
}

/// <summary>
/// Response model for user.
/// </summary>
public class UserResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Guid TenantId { get; set; }
    public bool IsActive { get; set; }
    public List<string> Roles { get; set; } = new();
}
