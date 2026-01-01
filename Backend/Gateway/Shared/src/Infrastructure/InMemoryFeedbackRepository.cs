using B2Connect.Domain.Support.Handlers;
using B2Connect.Domain.Support.Models;

namespace B2Connect.Gateway.Shared.Infrastructure;

/// <summary>
/// In-memory implementation of feedback repository for development/testing
/// In production, this should be replaced with EF Core implementation
/// </summary>
public class InMemoryFeedbackRepository : IFeedbackRepository
{
    private readonly Dictionary<Guid, Feedback> _feedbacks = new();
    private readonly object _lock = new();

    public Task SaveAsync(Feedback feedback)
    {
        lock (_lock)
        {
            _feedbacks[feedback.Id] = feedback;
        }
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Feedback feedback)
    {
        lock (_lock)
        {
            _feedbacks[feedback.Id] = feedback;
        }
        return Task.CompletedTask;
    }

    public Task<Feedback?> GetByIdAsync(Guid id)
    {
        lock (_lock)
        {
            _feedbacks.TryGetValue(id, out var feedback);
            return Task.FromResult(feedback);
        }
    }

    public Task<Feedback?> GetByCorrelationIdAsync(Guid correlationId)
    {
        lock (_lock)
        {
            var feedback = _feedbacks.Values.FirstOrDefault(f => f.CorrelationId == correlationId);
            return Task.FromResult(feedback);
        }
    }

    public Task<IReadOnlyList<Feedback>> GetByStatusAsync(FeedbackStatus status, int limit = 100)
    {
        lock (_lock)
        {
            var feedbacks = _feedbacks.Values
                .Where(f => f.Status == status)
                .Take(limit)
                .ToList();
            return Task.FromResult<IReadOnlyList<Feedback>>(feedbacks);
        }
    }

    public Task<IReadOnlyList<Feedback>> GetByDateRangeAsync(DateTime from, DateTime to)
    {
        lock (_lock)
        {
            var feedbacks = _feedbacks.Values
                .Where(f => f.CreatedAt >= from && f.CreatedAt <= to)
                .ToList();
            return Task.FromResult<IReadOnlyList<Feedback>>(feedbacks);
        }
    }
}</content>
<parameter name="filePath">/Users/holger/Documents/Projekte/B2Connect/backend/Gateway/Shared/src/Infrastructure/InMemoryFeedbackRepository.cs