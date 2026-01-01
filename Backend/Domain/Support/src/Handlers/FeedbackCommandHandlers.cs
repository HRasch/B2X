using B2Connect.Domain.Support.Commands;
using B2Connect.Domain.Support.Models;
using B2Connect.Domain.Support.Services;
using Microsoft.Extensions.Logging;

namespace B2Connect.Domain.Support.Handlers;

/// <summary>
/// Handler for validating feedback before processing
/// </summary>
public class ValidateFeedbackHandler : ICommandHandler<ValidateFeedbackCommand, ValidationResult>
{
    private readonly IFeedbackValidator _validator;
    private readonly ILogger<ValidateFeedbackHandler> _logger;

    public ValidateFeedbackHandler(
        IFeedbackValidator validator,
        ILogger<ValidateFeedbackHandler> logger)
    {
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ValidationResult> HandleAsync(ValidateFeedbackCommand command)
    {
        _logger.LogInformation("Validating feedback with correlation ID {CorrelationId}",
            command.CorrelationId);

        try
        {
            // Convert to CreateFeedbackCommand for validation
            var createCommand = new CreateFeedbackCommand
            {
                Category = command.Category,
                Description = command.Description,
                Context = command.Context,
                Attachments = command.Attachments,
                CorrelationId = command.CorrelationId
            };

            var result = await _validator.ValidateAsync(createCommand);

            _logger.LogInformation("Validation completed for feedback {CorrelationId}: {Status}",
                command.CorrelationId, result.Status);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to validate feedback for correlation {CorrelationId}",
                command.CorrelationId);

            return new ValidationResult
            {
                IsValid = false,
                Status = ValidationStatus.Rejected,
                Message = "Validation failed due to an internal error. Please try again later.",
                Reasons = new[] { "Internal validation error" },
                Severity = ValidationSeverity.Critical
            };
        }
    }
}

/// <summary>
/// Handler for creating feedback
/// </summary>
public class CreateFeedbackHandler : ICommandHandler<CreateFeedbackCommand, FeedbackResult>
{
    private readonly IDataAnonymizer _anonymizer;
    private readonly IGitHubService _gitHubService;
    private readonly IFeedbackRepository _repository;
    private readonly IFeedbackValidator _validator;
    private readonly ILogger<CreateFeedbackHandler> _logger;

    public CreateFeedbackHandler(
        IDataAnonymizer anonymizer,
        IGitHubService gitHubService,
        IFeedbackRepository repository,
        IFeedbackValidator validator,
        ILogger<CreateFeedbackHandler> logger)
    {
        _anonymizer = anonymizer ?? throw new ArgumentNullException(nameof(anonymizer));
        _gitHubService = gitHubService ?? throw new ArgumentNullException(nameof(gitHubService));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<FeedbackResult> HandleAsync(CreateFeedbackCommand command)
    {
        _logger.LogInformation("Creating feedback with correlation ID {CorrelationId}",
            command.CorrelationId);

        try
        {
            // Step 1: Validate feedback before processing
            var validationResult = await _validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Feedback validation failed for correlation {CorrelationId}: {Reasons}",
                    command.CorrelationId, string.Join("; ", validationResult.Reasons));

                throw new ValidationException(validationResult.Message, validationResult.Reasons);
            }

            // Step 2: Anonymize the context data
            var anonymizedContext = await _anonymizer.AnonymizeAsync(command.Context);

            // Step 3: Create feedback entity
            var feedback = new Feedback(
                command.Category,
                command.Description,
                anonymizedContext,
                command.Attachments,
                command.CorrelationId);

            // Step 4: Save to repository
            await _repository.SaveAsync(feedback);

            // Step 5: Create GitHub issue
            var gitHubIssue = await _gitHubService.CreateIssueAsync(
                anonymizedContext,
                command.Category,
                command.Description,
                command.Attachments);

            // Step 6: Update feedback with GitHub issue URL
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
        catch (ValidationException ex)
        {
            _logger.LogWarning("Feedback rejected due to validation for correlation {CorrelationId}: {Message}",
                command.CorrelationId, ex.Message);

            // Don't create feedback or GitHub issue for validation failures
            throw;
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
/// Exception thrown when feedback validation fails
/// </summary>
public class ValidationException : Exception
{
    public IReadOnlyList<string> ValidationErrors { get; }

    public ValidationException(string message, IReadOnlyList<string> errors)
        : base(message)
    {
        ValidationErrors = errors;
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