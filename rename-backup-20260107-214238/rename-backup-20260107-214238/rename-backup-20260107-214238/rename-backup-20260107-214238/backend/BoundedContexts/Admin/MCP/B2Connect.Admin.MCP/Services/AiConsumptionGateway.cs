using System.Collections.Concurrent;
using System.Threading.RateLimiting;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace B2Connect.Admin.MCP.Services;

/// <summary>
/// Exclusive AI Consumption Gateway - MCP Server is the ONLY component allowed to access AI services
/// </summary>
public class AiConsumptionGateway
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<AiConsumptionGateway> _logger;
    private readonly ConcurrentDictionary<string, RateLimiter> _rateLimiters = new();

    public AiConsumptionGateway(
        IDistributedCache cache,
        ILogger<AiConsumptionGateway> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// Execute AI request with consumption monitoring and controls
    /// </summary>
    public async Task<AiConsumptionResult> ExecuteAiRequestAsync(
        string tenantId,
        string toolName,
        string provider,
        Func<Task<AiResponse>> aiRequest,
        CancellationToken cancellationToken = default)
    {
        var requestId = Guid.NewGuid().ToString();
        var startTime = DateTime.UtcNow;

        try
        {
            // Check rate limits
            if (!await CheckRateLimitAsync(tenantId, toolName, cancellationToken))
            {
                throw new AiConsumptionException("Rate limit exceeded", AiConsumptionError.RateLimitExceeded);
            }

            // Check budget
            if (!await CheckBudgetAsync(tenantId, cancellationToken))
            {
                throw new AiConsumptionException("Budget exceeded", AiConsumptionError.BudgetExceeded);
            }

            // Execute AI request
            var response = await aiRequest();

            // Record consumption
            await RecordConsumptionAsync(tenantId, toolName, provider, requestId, startTime, response, cancellationToken);

            return new AiConsumptionResult
            {
                Success = true,
                RequestId = requestId,
                Response = response,
                TokensUsed = response.TokensUsed,
                Cost = CalculateCost(provider, response.TokensUsed)
            };
        }
        catch (Exception ex)
        {
            // Record failed consumption
            await RecordFailedConsumptionAsync(tenantId, toolName, provider, requestId, startTime, ex, cancellationToken);

            return new AiConsumptionResult
            {
                Success = false,
                RequestId = requestId,
                Error = ex.Message,
                ErrorType = ex is AiConsumptionException ace ? ace.ErrorType : AiConsumptionError.Unknown
            };
        }
    }

    private async Task<bool> CheckRateLimitAsync(string tenantId, string toolName, CancellationToken cancellationToken)
    {
        var key = $"ratelimit:{tenantId}:{toolName}";
        var rateLimiter = _rateLimiters.GetOrAdd(key, _ => new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions
        {
            TokenLimit = 100, // 100 requests per minute
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 10,
            ReplenishmentPeriod = TimeSpan.FromMinutes(1),
            TokensPerPeriod = 100,
            AutoReplenishment = true
        }));

        var lease = await rateLimiter.AcquireAsync(1, cancellationToken);
        return lease.IsAcquired;
    }

    private async Task<bool> CheckBudgetAsync(string tenantId, CancellationToken cancellationToken)
    {
        var budgetKey = $"budget:{tenantId}";
        var currentSpend = await _cache.GetStringAsync(budgetKey, cancellationToken);
        var monthlyBudget = 1000m; // $1000/month per tenant

        if (decimal.TryParse(currentSpend, out var spend) && spend >= monthlyBudget)
        {
            _logger.LogWarning("Budget exceeded for tenant {TenantId}: ${Spend}/${Budget}", tenantId, spend, monthlyBudget);
            return false;
        }

        return true;
    }

    private async Task RecordConsumptionAsync(
        string tenantId,
        string toolName,
        string provider,
        string requestId,
        DateTime startTime,
        AiResponse response,
        CancellationToken cancellationToken)
    {
        var consumption = new AiConsumptionRecord
        {
            TenantId = tenantId,
            ToolName = toolName,
            Provider = provider,
            RequestId = requestId,
            Timestamp = startTime,
            Duration = DateTime.UtcNow - startTime,
            TokensUsed = response.TokensUsed,
            Cost = CalculateCost(provider, response.TokensUsed),
            Success = true
        };

        // Store in cache for quick access
        var cacheKey = $"consumption:{tenantId}:{requestId}";
        await _cache.SetStringAsync(cacheKey, System.Text.Json.JsonSerializer.Serialize(consumption),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30) },
            cancellationToken);

        // Update budget
        var budgetKey = $"budget:{tenantId}";
        var currentSpend = await _cache.GetStringAsync(budgetKey, cancellationToken);
        var newSpend = (decimal.TryParse(currentSpend, out var spend) ? spend : 0) + consumption.Cost;
        await _cache.SetStringAsync(budgetKey, newSpend.ToString(),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30) },
            cancellationToken);

        _logger.LogInformation("AI consumption recorded: Tenant={TenantId}, Tool={ToolName}, Cost=${Cost}, Tokens={Tokens}",
            tenantId, toolName, consumption.Cost, consumption.TokensUsed);
    }

    private async Task RecordFailedConsumptionAsync(
        string tenantId,
        string toolName,
        string provider,
        string requestId,
        DateTime startTime,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var consumption = new AiConsumptionRecord
        {
            TenantId = tenantId,
            ToolName = toolName,
            Provider = provider,
            RequestId = requestId,
            Timestamp = startTime,
            Duration = DateTime.UtcNow - startTime,
            TokensUsed = 0,
            Cost = 0,
            Success = false,
            Error = exception.Message
        };

        var cacheKey = $"consumption:{tenantId}:{requestId}";
        await _cache.SetStringAsync(cacheKey, System.Text.Json.JsonSerializer.Serialize(consumption),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30) },
            cancellationToken);

        _logger.LogWarning(exception, "AI consumption failed: Tenant={TenantId}, Tool={ToolName}, Error={Error}",
            tenantId, toolName, exception.Message);
    }

    private decimal CalculateCost(string provider, int tokensUsed)
    {
        // Cost per 1K tokens (approximate)
        return provider.ToLower() switch
        {
            "openai" => tokensUsed * 0.002m / 1000,
            "anthropic" => tokensUsed * 0.008m / 1000,
            "azure" => tokensUsed * 0.002m / 1000,
            _ => tokensUsed * 0.005m / 1000
        };
    }

    /// <summary>
    /// Get consumption metrics for a tenant
    /// </summary>
    public async Task<AiConsumptionMetrics> GetConsumptionMetricsAsync(string tenantId, CancellationToken cancellationToken = default)
    {
        var budgetKey = $"budget:{tenantId}";
        var currentSpend = await _cache.GetStringAsync(budgetKey, cancellationToken);

        return new AiConsumptionMetrics
        {
            TenantId = tenantId,
            CurrentMonthSpend = decimal.TryParse(currentSpend, out var spend) ? spend : 0,
            MonthlyBudget = 1000m,
            RemainingBudget = 1000m - (decimal.TryParse(currentSpend, out var s) ? s : 0)
        };
    }
}

public class AiConsumptionResult
{
    public bool Success { get; set; }
    public string RequestId { get; set; } = string.Empty;
    public AiResponse? Response { get; set; }
    public string? Error { get; set; }
    public AiConsumptionError ErrorType { get; set; }
    public int TokensUsed { get; set; }
    public decimal Cost { get; set; }
}

public class AiConsumptionRecord
{
    public string TenantId { get; set; } = string.Empty;
    public string ToolName { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public string RequestId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public TimeSpan Duration { get; set; }
    public int TokensUsed { get; set; }
    public decimal Cost { get; set; }
    public bool Success { get; set; }
    public string? Error { get; set; }
}

public class AiConsumptionMetrics
{
    public string TenantId { get; set; } = string.Empty;
    public decimal CurrentMonthSpend { get; set; }
    public decimal MonthlyBudget { get; set; }
    public decimal RemainingBudget { get; set; }
}

public class AiResponse
{
    public string Content { get; set; } = string.Empty;
    public int TokensUsed { get; set; }
    public string Model { get; set; } = string.Empty;
}

public enum AiConsumptionError
{
    Unknown,
    RateLimitExceeded,
    BudgetExceeded,
    ProviderError,
    InvalidRequest
}

public class AiConsumptionException : Exception
{
    public AiConsumptionError ErrorType { get; }

    public AiConsumptionException(string message, AiConsumptionError errorType)
        : base(message)
    {
        ErrorType = errorType;
    }
}