using System.Net.Http.Json;
using B2X.AppHost.Configuration;
using B2X.Shared.Core;
using Bogus;

namespace B2X.AppHost.Services;

/// <summary>
/// Utilities for seeding tenant data in test environments.
/// </summary>
public static class TenantSeedingUtilities
{
    /// <summary>
    /// Seeds the specified number of test tenants.
    /// </summary>
    public static async Task SeedTenantsAsync(
        HttpClient tenantServiceClient,
        TestingConfiguration testingConfig,
        CancellationToken cancellationToken = default)
    {
        if (tenantServiceClient == null)
            throw new ArgumentNullException(nameof(tenantServiceClient));
        if (testingConfig == null)
            throw new ArgumentNullException(nameof(testingConfig));

        var faker = new Faker();
        var tenantCount = testingConfig.SeedData.DefaultTenantCount;

        for (int i = 0; i < tenantCount; i++)
        {
            var tenantRequest = CreateTenantRequest(faker, i);
            await CreateTenantAsync(tenantServiceClient, tenantRequest, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Creates a tenant creation request with test data.
    /// </summary>
    private static TenantCreationRequest CreateTenantRequest(Faker faker, int index)
    {
        var companyName = index == 0 ? "Demo Company" : faker.Company.CompanyName();
        var slug = index == 0 ? SeedConstants.DefaultTenantSlug : GenerateSlug(companyName);

        return new TenantCreationRequest
        {
            Name = companyName,
            Slug = slug,
            Domain = index == 0 ? SeedConstants.DefaultTenantDomain : faker.Internet.DomainName(),
            Description = faker.Company.Bs(),
            ContactEmail = faker.Internet.Email(),
            ContactPhone = faker.Phone.PhoneNumber(),
            Address = new AddressRequest
            {
                Street = faker.Address.StreetAddress(),
                City = faker.Address.City(),
                State = faker.Address.State(),
                PostalCode = faker.Address.ZipCode(),
                Country = faker.Address.Country()
            },
            Settings = new TenantSettingsRequest
            {
                TimeZone = "UTC",
                Locale = "en-US",
                Currency = "USD",
                IsActive = true,
                MaxUsers = 100,
                StorageLimitGb = 10
            },
            // Mark as test data
            IsTestData = true,
            CreatedBy = "TestDataOrchestrator"
        };
    }

    /// <summary>
    /// Sends the tenant creation request to the tenant service.
    /// </summary>
    private static async Task CreateTenantAsync(
        HttpClient client,
        TenantCreationRequest request,
        CancellationToken cancellationToken)
    {
        var response = await client.PostAsJsonAsync("/api/tenants", request, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            throw new InvalidOperationException(
                $"Failed to create tenant '{request.Name}': {response.StatusCode} - {errorContent}");
        }

        var createdTenant = await response.Content.ReadFromJsonAsync<TenantResponse>(cancellationToken: cancellationToken).ConfigureAwait(false);
        if (createdTenant != null)
        {
            // Store tenant ID for later use in other seeding operations
            TestDataContext.Current.AddTenant(createdTenant.Id, createdTenant.Slug);
        }
    }

    /// <summary>
    /// Generates a URL-safe slug from a company name.
    /// </summary>
    private static string GenerateSlug(string companyName)
    {
        return companyName
            .ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("&", "and")
            .Replace("'", "")
            .Replace("\"", "")
            .Replace(".", "")
            .Replace(",", "")
            .Replace("(", "")
            .Replace(")", "")
            .Replace("[", "")
            .Replace("]", "")
            .Replace("{", "")
            .Replace("}", "")
            .Replace("/", "-")
            .Replace("\\", "-")
            .Replace("---", "-")
            .Replace("--", "-")
            .Trim('-');
    }

    /// <summary>
    /// Resets all test tenants (for cleanup between test runs).
    /// </summary>
    public static async Task ResetTenantsAsync(
        HttpClient tenantServiceClient,
        CancellationToken cancellationToken = default)
    {
        if (tenantServiceClient == null)
            throw new ArgumentNullException(nameof(tenantServiceClient));

        // Get all test tenants
        var response = await tenantServiceClient.GetAsync("/api/tenants?testDataOnly=true", cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            // If endpoint doesn't exist or fails, skip reset
            return;
        }

        var tenants = await response.Content.ReadFromJsonAsync<List<TenantResponse>>(cancellationToken: cancellationToken).ConfigureAwait(false);

        if (tenants != null)
        {
            foreach (var tenant in tenants)
            {
                await tenantServiceClient.DeleteAsync($"/api/tenants/{tenant.Id}", cancellationToken).ConfigureAwait(false);
            }
        }

        TestDataContext.Current.ClearTenants();
    }
}

/// <summary>
/// Request model for tenant creation.
/// </summary>
public class TenantCreationRequest
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ContactEmail { get; set; } = string.Empty;
    public string? ContactPhone { get; set; }
    public AddressRequest Address { get; set; } = new();
    public TenantSettingsRequest Settings { get; set; } = new();
    public bool IsTestData { get; set; }
    public string? CreatedBy { get; set; }
}

/// <summary>
/// Address information for tenant.
/// </summary>
public class AddressRequest
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? State { get; set; }
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}

/// <summary>
/// Settings for tenant.
/// </summary>
public class TenantSettingsRequest
{
    public string TimeZone { get; set; } = "UTC";
    public string Locale { get; set; } = "en-US";
    public string Currency { get; set; } = "USD";
    public bool IsActive { get; set; } = true;
    public int MaxUsers { get; set; } = 100;
    public int StorageLimitGb { get; set; } = 10;
}

/// <summary>
/// Response model for tenant.
/// </summary>
public class TenantResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
