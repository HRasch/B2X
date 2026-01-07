using B2X.AppHost.Configuration;
using System.Net.Http.Json;

namespace B2X.AppHost.Services;

/// <summary>
/// Utilities for seeding catalog data in test environments.
/// </summary>
public static class CatalogSeedingUtilities
{
    /// <summary>
    /// Seeds catalog data for all tenants.
    /// </summary>
    public static async Task SeedCatalogAsync(
        HttpClient catalogServiceClient,
        TestingConfiguration testingConfig,
        CancellationToken cancellationToken = default)
    {
        if (catalogServiceClient == null)
            throw new ArgumentNullException(nameof(catalogServiceClient));
        if (testingConfig == null)
            throw new ArgumentNullException(nameof(testingConfig));

        if (!testingConfig.SeedData.IncludeCatalogDemo)
        {
            return; // Skip catalog seeding
        }

        var productCount = testingConfig.SeedData.SampleProductCount;

        foreach (var tenantId in TestDataContext.Current.GetAllTenantIds())
        {
            await SeedCatalogForTenantAsync(catalogServiceClient, tenantId, productCount, cancellationToken);
        }
    }

    /// <summary>
    /// Seeds catalog data for a specific tenant.
    /// </summary>
    private static async Task SeedCatalogForTenantAsync(
        HttpClient client,
        Guid tenantId,
        int productCount,
        CancellationToken cancellationToken)
    {
        var request = new CatalogSeedingRequest
        {
            TenantId = tenantId,
            ProductCount = productCount,
            IncludeCategories = true,
            IncludeBrands = true,
            IncludeVariants = true,
            IncludeImages = true,
            IsTestData = true,
            CreatedBy = "TestDataOrchestrator"
        };

        var response = await client.PostAsJsonAsync("/api/catalog/seed", request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException(
                $"Failed to seed catalog for tenant {tenantId}: {response.StatusCode} - {errorContent}");
        }

        var result = await response.Content.ReadFromJsonAsync<CatalogSeedingResponse>(cancellationToken: cancellationToken);
        if (result != null && result.ProductIds != null)
        {
            // Store product IDs for later reference
            foreach (var productId in result.ProductIds)
            {
                TestDataContext.Current.AddProductToTenant(tenantId, productId);
            }
        }
    }

    /// <summary>
    /// Resets catalog data for all tenants.
    /// </summary>
    public static async Task ResetCatalogAsync(
        HttpClient catalogServiceClient,
        CancellationToken cancellationToken = default)
    {
        if (catalogServiceClient == null)
            throw new ArgumentNullException(nameof(catalogServiceClient));

        foreach (var tenantId in TestDataContext.Current.GetAllTenantIds())
        {
            await ResetCatalogForTenantAsync(catalogServiceClient, tenantId, cancellationToken);
        }

        // Clear product references
        foreach (var tenantId in TestDataContext.Current.GetAllTenantIds())
        {
            // Note: This would need to be implemented in TestDataContext if we want to clear products
        }
    }

    /// <summary>
    /// Resets catalog data for a specific tenant.
    /// </summary>
    private static async Task ResetCatalogForTenantAsync(
        HttpClient client,
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await client.DeleteAsync($"/api/catalog/seed/{tenantId}", cancellationToken);

            if (!response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.NotFound)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                // Log warning but don't fail - catalog might not exist
                Console.WriteLine($"Warning: Failed to reset catalog for tenant {tenantId}: {response.StatusCode} - {errorContent}");
            }
        }
        catch (Exception ex)
        {
            // Log but don't fail - catalog reset is not critical
            Console.WriteLine($"Warning: Exception during catalog reset for tenant {tenantId}: {ex.Message}");
        }
    }
}

/// <summary>
/// Request model for catalog seeding.
/// </summary>
public class CatalogSeedingRequest
{
    public Guid TenantId { get; set; }
    public int ProductCount { get; set; } = 50;
    public bool IncludeCategories { get; set; } = true;
    public bool IncludeBrands { get; set; } = true;
    public bool IncludeVariants { get; set; } = true;
    public bool IncludeImages { get; set; } = true;
    public bool IsTestData { get; set; }
    public string? CreatedBy { get; set; }
}

/// <summary>
/// Response model for catalog seeding.
/// </summary>
public class CatalogSeedingResponse
{
    public Guid TenantId { get; set; }
    public int CategoriesCreated { get; set; }
    public int BrandsCreated { get; set; }
    public int ProductsCreated { get; set; }
    public int VariantsCreated { get; set; }
    public int ImagesCreated { get; set; }
    public List<Guid>? ProductIds { get; set; }
    public bool Success { get; set; }
    public List<string>? Messages { get; set; }
}
