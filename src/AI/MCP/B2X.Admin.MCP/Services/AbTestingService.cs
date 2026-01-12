using B2X.Admin.MCP.Data;
using B2X.Admin.MCP.Middleware;
using Microsoft.EntityFrameworkCore;

namespace B2X.Admin.MCP.Services;

/// <summary>
/// Service for managing A/B testing of AI recommendations
/// </summary>
public class AbTestingService
{
    private readonly McpDbContext _dbContext;
    private readonly TenantContext _tenantContext;
    private readonly ILogger<AbTestingService> _logger;

    public AbTestingService(
        McpDbContext dbContext,
        TenantContext tenantContext,
        ILogger<AbTestingService> logger)
    {
        _dbContext = dbContext;
        _tenantContext = tenantContext;
        _logger = logger;
    }

    /// <summary>
    /// Create a new A/B test
    /// </summary>
    public async Task<AbTest> CreateTestAsync(string name, string description, string toolName, string createdBy)
    {
        var test = new AbTest
        {
            TenantId = _tenantContext.TenantId,
            Name = name,
            Description = description,
            ToolName = toolName,
            Status = "draft",
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.AbTests.Add(test);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Created A/B test {TestId} for tool {ToolName}", test.Id, toolName);
        return test;
    }

    /// <summary>
    /// Add a variant to an A/B test
    /// </summary>
    public async Task<AbTestVariant> AddVariantAsync(int testId, string name, string description,
        string promptTemplate, decimal weight = 1.0m, bool isControl = false)
    {
        var variant = new AbTestVariant
        {
            TestId = testId,
            Name = name,
            Description = description,
            PromptTemplate = promptTemplate,
            Weight = weight,
            IsControl = isControl,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.AbTestVariants.Add(variant);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Added variant {VariantName} to test {TestId}", name, testId);
        return variant;
    }

    /// <summary>
    /// Start an A/B test
    /// </summary>
    public async Task StartTestAsync(int testId)
    {
        var test = await _dbContext.AbTests
            .Include(t => t.Variants)
            .FirstOrDefaultAsync(t => t.Id == testId && t.TenantId == _tenantContext.TenantId);

        if (test == null)
            throw new ArgumentException("Test not found");

        if (!test.Variants.Any())
            throw new InvalidOperationException("Test must have at least one variant");

        test.Status = "active";
        test.StartDate = DateTime.UtcNow;
        test.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Started A/B test {TestId}", testId);
    }

    /// <summary>
    /// Select a variant for a user based on weighted random selection
    /// </summary>
    public async Task<AbTestVariant> SelectVariantAsync(int testId, string userId)
    {
        var test = await _dbContext.AbTests
            .Include(t => t.Variants)
            .FirstOrDefaultAsync(t => t.Id == testId &&
                                    t.TenantId == _tenantContext.TenantId &&
                                    t.Status == "active");

        if (test == null || !test.Variants.Any())
            throw new ArgumentException("Active test with variants not found");

        // Use user ID for consistent variant assignment (deterministic)
        var hash = StringComparer.Ordinal.GetHashCode(userId);
        var random = new Random(hash);
        var totalWeight = test.Variants.Sum(v => v.Weight);
        var randomValue = (decimal)random.NextDouble() * totalWeight;

        decimal cumulativeWeight = 0;
        foreach (var variant in test.Variants.OrderBy(v => v.Id))
        {
            cumulativeWeight += variant.Weight;
            if (randomValue <= cumulativeWeight)
            {
                return variant;
            }
        }

        // Fallback to first variant
        return test.Variants.First();
    }

    /// <summary>
    /// Record a test result
    /// </summary>
    public async Task RecordResultAsync(int testId, int variantId, int conversationId,
        string userId, string metricType, decimal metricValue, string? feedback = null)
    {
        var result = new AbTestResult
        {
            TestId = testId,
            VariantId = variantId,
            ConversationId = conversationId,
            UserId = userId,
            MetricType = metricType,
            MetricValue = metricValue,
            Feedback = feedback,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.AbTestResults.Add(result);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Recorded {MetricType} result for test {TestId}, variant {VariantId}",
            metricType, testId, variantId);
    }

    /// <summary>
    /// Get test results and statistics
    /// </summary>
    public async Task<AbTestStatistics> GetTestStatisticsAsync(int testId)
    {
        var test = await _dbContext.AbTests
            .Include(t => t.Variants)
            .ThenInclude(v => v.Results)
            .FirstOrDefaultAsync(t => t.Id == testId && t.TenantId == _tenantContext.TenantId);

        if (test == null)
            throw new ArgumentException("Test not found");

        var statistics = new AbTestStatistics
        {
            TestId = test.Id,
            TestName = test.Name,
            Status = test.Status,
            StartDate = test.StartDate,
            VariantStatistics = new List<VariantStatistics>()
        };

        foreach (var variant in test.Variants)
        {
            var variantStats = new VariantStatistics
            {
                VariantId = variant.Id,
                VariantName = variant.Name,
                IsControl = variant.IsControl,
                SampleSize = variant.Results.Count,
                Metrics = new Dictionary<string, MetricStatistics>()
            };

            // Group results by metric type
            var resultsByMetric = variant.Results.GroupBy(r => r.MetricType);

            foreach (var metricGroup in resultsByMetric)
            {
                var values = metricGroup.Select(r => r.MetricValue).ToList();
                variantStats.Metrics[metricGroup.Key] = new MetricStatistics
                {
                    Count = values.Count,
                    Mean = values.Average(),
                    Median = CalculateMedian(values),
                    Min = values.Min(),
                    Max = values.Max(),
                    StandardDeviation = CalculateStandardDeviation(values)
                };
            }

            statistics.VariantStatistics.Add(variantStats);
        }

        return statistics;
    }

    /// <summary>
    /// Get active tests for a tool
    /// </summary>
    public async Task<List<AbTest>> GetActiveTestsForToolAsync(string toolName)
    {
        return await _dbContext.AbTests
            .Include(t => t.Variants)
            .Where(t => t.TenantId == _tenantContext.TenantId &&
                       t.ToolName == toolName &&
                       t.Status == "active")
            .ToListAsync();
    }

    /// <summary>
    /// End an A/B test
    /// </summary>
    public async Task EndTestAsync(int testId)
    {
        var test = await _dbContext.AbTests.FindAsync(testId);
        if (test != null && test.TenantId == _tenantContext.TenantId)
        {
            test.Status = "completed";
            test.EndDate = DateTime.UtcNow;
            test.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Ended A/B test {TestId}", testId);
        }
    }

    /// <summary>
    /// Get all tests for the tenant
    /// </summary>
    public async Task<List<AbTest>> GetTestsAsync()
    {
        return await _dbContext.AbTests
            .Include(t => t.Variants)
            .Include(t => t.Results)
            .Where(t => t.TenantId == _tenantContext.TenantId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Get a specific test by ID
    /// </summary>
    public async Task<AbTest?> GetTestAsync(int testId)
    {
        return await _dbContext.AbTests
            .Include(t => t.Variants)
            .Include(t => t.Results)
            .FirstOrDefaultAsync(t => t.Id == testId && t.TenantId == _tenantContext.TenantId);
    }

    /// <summary>
    /// Delete a test
    /// </summary>
    public async Task<bool> DeleteTestAsync(int testId)
    {
        var test = await _dbContext.AbTests
            .FirstOrDefaultAsync(t => t.Id == testId && t.TenantId == _tenantContext.TenantId);

        if (test == null)
            return false;

        _dbContext.AbTests.Remove(test);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Deleted A/B test {TestId}", testId);
        return true;
    }

    /// <summary>
    /// Update test status
    /// </summary>
    public async Task<bool> UpdateTestStatusAsync(int testId, string status)
    {
        var test = await _dbContext.AbTests
            .FirstOrDefaultAsync(t => t.Id == testId && t.TenantId == _tenantContext.TenantId);

        if (test == null)
            return false;

        test.Status = status;
        test.UpdatedAt = DateTime.UtcNow;

        if (status == "active" && test.StartDate == null)
            test.StartDate = DateTime.UtcNow;
        else if (status == "completed" && test.EndDate == null)
            test.EndDate = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Updated A/B test {TestId} status to {Status}", testId, status);
        return true;
    }

    private static decimal CalculateMedian(List<decimal> values)
    {
        if (!values.Any())
            return 0;

        var sorted = values.OrderBy(v => v).ToList();
        var count = sorted.Count;
        var mid = count / 2;

        return count % 2 == 0
            ? (sorted[mid - 1] + sorted[mid]) / 2
            : sorted[mid];
    }

    private static decimal CalculateStandardDeviation(List<decimal> values)
    {
        if (values.Count <= 1)
            return 0;

        var mean = values.Average();
        var sumOfSquares = values.Sum(v => (v - mean) * (v - mean));
        return (decimal)Math.Sqrt((double)(sumOfSquares / (values.Count - 1)));
    }
}

/// <summary>
/// Statistics for an A/B test
/// </summary>
public class AbTestStatistics
{
    public int TestId { get; set; }
    public string TestName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public List<VariantStatistics> VariantStatistics { get; set; } = new();
}

/// <summary>
/// Statistics for a test variant
/// </summary>
public class VariantStatistics
{
    public int VariantId { get; set; }
    public string VariantName { get; set; } = string.Empty;
    public bool IsControl { get; set; }
    public int SampleSize { get; set; }
    public Dictionary<string, MetricStatistics> Metrics { get; set; } = new();
}

/// <summary>
/// Statistics for a metric
/// </summary>
public class MetricStatistics
{
    public int Count { get; set; }
    public decimal Mean { get; set; }
    public decimal Median { get; set; }
    public decimal Min { get; set; }
    public decimal Max { get; set; }
    public decimal StandardDeviation { get; set; }
}
