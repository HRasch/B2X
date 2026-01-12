using System.Net.Http.Json;
using B2X.AppHost.Configuration;

namespace B2X.AppHost.Services;

/// <summary>
/// Utilities for seeding CMS data in test environments.
/// </summary>
public static class CmsSeedingUtilities
{
    /// <summary>
    /// Seeds CMS data for all tenants.
    /// </summary>
    public static async Task SeedCmsAsync(
        HttpClient cmsServiceClient,
        TestingConfiguration testingConfig,
        CancellationToken cancellationToken = default)
    {
        if (cmsServiceClient == null)
            throw new ArgumentNullException(nameof(cmsServiceClient));
        if (testingConfig == null)
            throw new ArgumentNullException(nameof(testingConfig));

        if (!testingConfig.SeedData.IncludeCmsDemo)
        {
            return; // Skip CMS seeding
        }

        var pageCount = testingConfig.SeedData.SamplePageCount;
        var templateCount = testingConfig.SeedData.SampleTemplateCount;

        foreach (var tenantId in TestDataContext.Current.GetAllTenantIds())
        {
            await SeedCmsForTenantAsync(cmsServiceClient, tenantId, pageCount, templateCount, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Seeds CMS data for a specific tenant.
    /// </summary>
    private static async Task SeedCmsForTenantAsync(
        HttpClient client,
        Guid tenantId,
        int pageCount,
        int templateCount,
        CancellationToken cancellationToken)
    {
        var request = new CmsSeedingRequest
        {
            TenantId = tenantId,
            PageCount = pageCount,
            TemplateCount = templateCount,
            IncludeContentBlocks = true,
            IncludeMediaAssets = true,
            IncludeNavigation = true,
            IsTestData = true,
            CreatedBy = "TestDataOrchestrator"
        };

        var response = await client.PostAsJsonAsync("/api/cms/seed", request, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            throw new InvalidOperationException(
                $"Failed to seed CMS for tenant {tenantId}: {response.StatusCode} - {errorContent}");
        }

        var result = await response.Content.ReadFromJsonAsync<CmsSeedingResponse>(cancellationToken: cancellationToken).ConfigureAwait(false);
        if (result != null && result.PageIds != null)
        {
            // Store page IDs for later reference
            foreach (var pageId in result.PageIds)
            {
                TestDataContext.Current.AddPageToTenant(tenantId, pageId);
            }
        }
    }

    /// <summary>
    /// Resets CMS data for all tenants.
    /// </summary>
    public static async Task ResetCmsAsync(
        HttpClient cmsServiceClient,
        CancellationToken cancellationToken = default)
    {
        if (cmsServiceClient == null)
            throw new ArgumentNullException(nameof(cmsServiceClient));

        foreach (var tenantId in TestDataContext.Current.GetAllTenantIds())
        {
            await ResetCmsForTenantAsync(cmsServiceClient, tenantId, cancellationToken).ConfigureAwait(false);
        }

        // Clear page references
        foreach (var tenantId in TestDataContext.Current.GetAllTenantIds())
        {
            // Note: This would need to be implemented in TestDataContext if we want to clear pages
        }
    }

    /// <summary>
    /// Resets CMS data for a specific tenant.
    /// </summary>
    private static async Task ResetCmsForTenantAsync(
        HttpClient client,
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await client.DeleteAsync($"/api/cms/seed/{tenantId}", cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.NotFound)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                // Log warning but don't fail - CMS might not exist
                Console.WriteLine($"Warning: Failed to reset CMS for tenant {tenantId}: {response.StatusCode} - {errorContent}");
            }
        }
        catch (Exception ex)
        {
            // Log but don't fail - CMS reset is not critical
            Console.WriteLine($"Warning: Exception during CMS reset for tenant {tenantId}: {ex.Message}");
        }
    }
}

/// <summary>
/// Request model for CMS seeding.
/// </summary>
public class CmsSeedingRequest
{
    public Guid TenantId { get; set; }
    public int PageCount { get; set; } = 20;
    public int TemplateCount { get; set; } = 5;
    public bool IncludeContentBlocks { get; set; } = true;
    public bool IncludeMediaAssets { get; set; } = true;
    public bool IncludeNavigation { get; set; } = true;
    public bool IsTestData { get; set; }
    public string? CreatedBy { get; set; }
}

/// <summary>
/// Response model for CMS seeding.
/// </summary>
public class CmsSeedingResponse
{
    public Guid TenantId { get; set; }
    public int TemplatesCreated { get; set; }
    public int PagesCreated { get; set; }
    public int ContentBlocksCreated { get; set; }
    public int MediaAssetsCreated { get; set; }
    public int NavigationItemsCreated { get; set; }
    public List<Guid>? PageIds { get; set; }
    public List<Guid>? TemplateIds { get; set; }
    public bool Success { get; set; }
    public List<string>? Messages { get; set; }
}
