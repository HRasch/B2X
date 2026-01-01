using B2Connect.Domain.Support.Queries;
using B2Connect.Domain.Support.Models;
using Microsoft.Extensions.Logging;

namespace B2Connect.Domain.Support.Handlers;

/// <summary>
/// Handler for getting feedback status
/// </summary>
public class GetFeedbackStatusHandler : IQueryHandler<GetFeedbackStatusQuery, FeedbackStatusResult>
{
    private readonly IFeedbackRepository _repository;
    private readonly ILogger<GetFeedbackStatusHandler> _logger;

    public GetFeedbackStatusHandler(
        IFeedbackRepository repository,
        ILogger<GetFeedbackStatusHandler> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<FeedbackStatusResult> HandleAsync(GetFeedbackStatusQuery query)
    {
        _logger.LogInformation("Getting feedback status for correlation {CorrelationId}",
            query.CorrelationId);

        var feedback = await _repository.GetByCorrelationIdAsync(query.CorrelationId);
        if (feedback == null)
        {
            throw new FeedbackNotFoundException(query.CorrelationId);
        }

        return new FeedbackStatusResult
        {
            CorrelationId = feedback.CorrelationId,
            Status = feedback.Status,
            GitHubIssueUrl = feedback.GitHubIssueUrl,
            Resolution = null, // Could be added later if we track resolutions
            CreatedAt = feedback.CreatedAt,
            UpdatedAt = feedback.CreatedAt // Simplified, could track last update separately
        };
    }
}

/// <summary>
/// Handler for getting detailed feedback (admin only)
/// </summary>
public class GetFeedbackByIdHandler : IQueryHandler<GetFeedbackByIdQuery, FeedbackDetailsResult>
{
    private readonly IFeedbackRepository _repository;
    private readonly ILogger<GetFeedbackByIdHandler> _logger;

    public GetFeedbackByIdHandler(
        IFeedbackRepository repository,
        ILogger<GetFeedbackByIdHandler> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<FeedbackDetailsResult> HandleAsync(GetFeedbackByIdQuery query)
    {
        _logger.LogInformation("Getting feedback details for ID {FeedbackId}", query.FeedbackId);

        var feedback = await _repository.GetByIdAsync(query.FeedbackId);
        if (feedback == null)
        {
            throw new FeedbackNotFoundException(query.FeedbackId);
        }

        var attachmentInfos = feedback.Attachments.Select(a => new AttachmentInfo
        {
            Id = a.Id,
            FileName = a.FileName,
            ContentType = a.ContentType,
            Size = a.Size
        }).ToList();

        return new FeedbackDetailsResult
        {
            Id = feedback.Id,
            Category = feedback.Category,
            Description = feedback.Description,
            Context = feedback.Context,
            Attachments = attachmentInfos,
            CorrelationId = feedback.CorrelationId,
            Status = feedback.Status,
            GitHubIssueUrl = feedback.GitHubIssueUrl,
            Resolution = null,
            CreatedAt = feedback.CreatedAt,
            UpdatedAt = feedback.CreatedAt
        };
    }
}

/// <summary>
/// Handler for getting feedback statistics
/// </summary>
public class GetFeedbackStatisticsHandler : IQueryHandler<GetFeedbackStatisticsQuery, FeedbackStatisticsResult>
{
    private readonly IFeedbackRepository _repository;
    private readonly ILogger<GetFeedbackStatisticsHandler> _logger;

    public GetFeedbackStatisticsHandler(
        IFeedbackRepository repository,
        ILogger<GetFeedbackStatisticsHandler> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<FeedbackStatisticsResult> HandleAsync(GetFeedbackStatisticsQuery query)
    {
        _logger.LogInformation("Getting feedback statistics from {FromDate} to {ToDate}",
            query.FromDate, query.ToDate);

        var fromDate = query.FromDate ?? DateTime.UtcNow.AddDays(-30);
        var toDate = query.ToDate ?? DateTime.UtcNow;

        var feedbackItems = await _repository.GetByDateRangeAsync(fromDate, toDate);

        // Filter by category if specified
        if (query.Category.HasValue)
        {
            feedbackItems = feedbackItems.Where(f => f.Category == query.Category.Value).ToList();
        }

        var totalFeedback = feedbackItems.Count;
        var resolvedFeedback = feedbackItems.Count(f => f.Status == FeedbackStatus.Resolved);

        // Calculate average resolution time (simplified - would need proper tracking)
        var averageResolutionTime = 24.0; // Placeholder - would calculate from actual data

        // Category breakdown
        var categoryBreakdown = feedbackItems
            .GroupBy(f => f.Category)
            .ToDictionary(g => g.Key, g => g.Count());

        // Status breakdown
        var statusBreakdown = feedbackItems
            .GroupBy(f => f.Status)
            .ToDictionary(g => g.Key, g => g.Count());

        // Recent items (last 10)
        var recentItems = feedbackItems
            .OrderByDescending(f => f.CreatedAt)
            .Take(10)
            .Select(f => new RecentFeedbackItem
            {
                CorrelationId = f.CorrelationId,
                Category = f.Category,
                Status = f.Status,
                CreatedAt = f.CreatedAt
            })
            .ToList();

        return new FeedbackStatisticsResult
        {
            TotalFeedback = totalFeedback,
            ResolvedFeedback = resolvedFeedback,
            AverageResolutionTimeHours = averageResolutionTime,
            CategoryBreakdown = categoryBreakdown,
            StatusBreakdown = statusBreakdown,
            RecentItems = recentItems
        };
    }
}

/// <summary>
/// Marker interface for query handlers
/// </summary>
public interface IQueryHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    Task<TResult> HandleAsync(TQuery query);
}</content>
<parameter name="filePath">/Users/holger/Documents/Projekte/B2Connect/backend/Domain/Support/src/Handlers/FeedbackQueryHandlers.cs