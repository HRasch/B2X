using B2Connect.Domain.Support.Commands;
using B2Connect.Domain.Support.Models;
using B2Connect.Domain.Support.Services;
using Microsoft.Extensions.Logging;

namespace B2Connect.Domain.Support.Handlers;

/// <summary>
/// Handler for creating feedback
/// </summary>
public class CreateFeedbackHandler : ICommandHandler<CreateFeedbackCommand, FeedbackResult>
{
    private readonly IDataAnonymizer _anonymizer;
    private readonly IGitHubService _gitHubService;
    private readonly IFeedbackRepository _repository;
    private readonly ILogger<CreateFeedbackHandler> _logger;

    public CreateFeedbackHandler(
        IDataAnonymizer anonymizer,
        IGitHubService gitHubService,
        IFeedbackRepository repository,
        ILogger<CreateFeedbackHandler> logger)
    {
        _anonymizer = anonymizer ?? throw new ArgumentNullException(nameof(anonymizer));
        _gitHubService = gitHubService ?? throw new ArgumentNullException(nameof(gitHubService));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<FeedbackResult> HandleAsync(CreateFeedbackCommand command)
    {
        _logger.LogInformation("Creating feedback with correlation ID {CorrelationId}",
            command.CorrelationId);

        try
        {
            // Step 1: Anonymize the context data
            var anonymizedContext = await _anonymizer.AnonymizeAsync(command.Context);

            // Step 2: Create feedback entity
            var feedback = new Feedback(
                command.Category,
                command.Description,
                anonymizedContext,
                command.Attachments,
                command.CorrelationId);

            // Step 3: Save to repository
            await _repository.SaveAsync(feedback);

            // Step 4: Create GitHub issue
            var gitHubIssue = await _gitHubService.CreateIssueAsync(
                anonymizedContext,
                command.Category,
                command.Description,
                command.Attachments);

            // Step 5: Update feedback with GitHub issue URL
            feedback.MarkAsSubmitted(gitHubIssue.HtmlUrl);
            await _repository.UpdateAsync(feedback);

            _logger.LogInformation("Successfully created feedback and GitHub issue {IssueNumber} for correlation {CorrelationId}",
                gitHubIssue.Number, command.CorrelationId);

            return new FeedbackResult
            {
                CorrelationId = command.CorrelationId,
                IssueUrl = gitHubIssue.HtmlUrl,
                Status = "submitted"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create feedback for correlation {CorrelationId}",
                command.CorrelationId);

            // Still save the feedback even if GitHub issue creation fails
            try
            {
                var anonymizedContext = await _anonymizer.AnonymizeAsync(command.Context);
                var feedback = new Feedback(
                    command.Category,
                    command.Description,
                    anonymizedContext,
                    command.Attachments,
                    command.CorrelationId);

                await _repository.SaveAsync(feedback);

                _logger.LogWarning("Saved feedback locally despite GitHub issue creation failure for correlation {CorrelationId}",
                    command.CorrelationId);
            }
            catch (Exception saveEx)
            {
                _logger.LogError(saveEx, "Failed to save feedback locally for correlation {CorrelationId}",
                    command.CorrelationId);
                throw;
            }

            throw;
        }
    }
}

/// <summary>
/// Handler for updating feedback status
/// </summary>
public class UpdateFeedbackStatusHandler : ICommandHandler<UpdateFeedbackStatusCommand, Unit>
{
    private readonly IFeedbackRepository _repository;
    private readonly ILogger<UpdateFeedbackStatusHandler> _logger;

    public UpdateFeedbackStatusHandler(
        IFeedbackRepository repository,
        ILogger<UpdateFeedbackStatusHandler> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Unit> HandleAsync(UpdateFeedbackStatusCommand command)
    {
        _logger.LogInformation("Updating feedback status for correlation {CorrelationId} to {Status}",
            command.CorrelationId, command.Status);

        var feedback = await _repository.GetByCorrelationIdAsync(command.CorrelationId);
        if (feedback == null)
        {
            throw new FeedbackNotFoundException(command.CorrelationId);
        }

        // Update status based on command
        switch (command.Status)
        {
            case FeedbackStatus.Resolved:
                feedback.MarkAsResolved();
                break;
            default:
                // For other status updates, we might need additional logic
                _logger.LogWarning("Status update to {Status} not implemented for correlation {CorrelationId}",
                    command.Status, command.CorrelationId);
                break;
        }

        await _repository.UpdateAsync(feedback);

        _logger.LogInformation("Successfully updated feedback status for correlation {CorrelationId}",
            command.CorrelationId);

        return Unit.Value;
    }
}

/// <summary>
/// Marker interface for command handlers
/// </summary>
public interface ICommandHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    Task<TResult> HandleAsync(TCommand command);
}

/// <summary>
/// Repository interface for feedback data access
/// </summary>
public interface IFeedbackRepository
{
    Task SaveAsync(Feedback feedback);
    Task UpdateAsync(Feedback feedback);
    Task<Feedback?> GetByIdAsync(Guid id);
    Task<Feedback?> GetByCorrelationIdAsync(Guid correlationId);
    Task<IReadOnlyList<Feedback>> GetByStatusAsync(FeedbackStatus status, int limit = 100);
    Task<IReadOnlyList<Feedback>> GetByDateRangeAsync(DateTime from, DateTime to);
}

/// <summary>
/// Exception thrown when feedback is not found
/// </summary>
public class FeedbackNotFoundException : Exception
{
    public Guid CorrelationId { get; }

    public FeedbackNotFoundException(Guid correlationId)
        : base($"Feedback with correlation ID {correlationId} not found")
    {
        CorrelationId = correlationId;
    }
}</content>
<parameter name="filePath">/Users/holger/Documents/Projekte/B2Connect/backend/Domain/Support/src/Handlers/FeedbackCommandHandlers.cs