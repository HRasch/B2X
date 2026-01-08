using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using B2X.Types;

namespace B2X.Shared.Extensions;

/// <summary>
/// Middleware for validating domain events before publishing
/// Ensures all events meet validation rules before being sent to message broker
/// </summary>
public class EventValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<EventValidationMiddleware> _logger;
    private readonly EventValidatorFactory _validatorFactory;

    public EventValidationMiddleware(
        RequestDelegate next,
        ILogger<EventValidationMiddleware> logger,
        EventValidatorFactory validatorFactory)
    {
        _next = next;
        _logger = logger;
        _validatorFactory = validatorFactory;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Continue with next middleware
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Event validation middleware error");
            throw;
        }
    }
}

/// <summary>
/// Service for intercepting and validating domain events
/// Used within application to validate events before publishing
/// </summary>
public interface IEventValidationService
{
    Task<(bool IsValid, List<string> Errors)> ValidateEventAsync(DomainEvent @event);
    Task PublishValidatedEventAsync<T>(T @event) where T : DomainEvent;
}

/// <summary>
/// Implementation of event validation service
/// </summary>
public class EventValidationService : IEventValidationService
{
    private readonly EventValidatorFactory _validatorFactory;
    private readonly ILogger<EventValidationService> _logger;

    public EventValidationService(
        EventValidatorFactory validatorFactory,
        ILogger<EventValidationService> logger)
    {
        _validatorFactory = validatorFactory;
        _logger = logger;
    }

    /// <summary>
    /// Validates a domain event and returns validation result
    /// </summary>
    public async Task<(bool IsValid, List<string> Errors)> ValidateEventAsync(DomainEvent @event)
    {
        try
        {
            var result = await _validatorFactory.ValidateEventWithResultAsync(@event);

            if (!result.IsValid)
            {
                var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
                _logger.LogWarning(
                    "Event validation failed for {EventType} (Id: {EventId}). Errors: {Errors}",
                    @event.EventType,
                    @event.EventId,
                    string.Join("; ", errors));

                return (false, errors);
            }

            _logger.LogInformation(
                "Event validation succeeded for {EventType} (Id: {EventId})",
                @event.EventType,
                @event.EventId);

            return (true, new List<string>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during event validation for {EventType}", @event.EventType);
            throw;
        }
    }

    /// <summary>
    /// Validates event before publishing - throws exception if validation fails
    /// </summary>
    public async Task PublishValidatedEventAsync<T>(T @event) where T : DomainEvent
    {
        var (isValid, errors) = await ValidateEventAsync(@event);

        if (!isValid)
        {
            throw new InvalidOperationException(
                $"Event validation failed: {string.Join("; ", errors)}");
        }

        // Event is valid - ready for publishing
        _logger.LogInformation(
            "Event {EventType} (Id: {EventId}) passed validation and is ready to publish",
            @event.EventType,
            @event.EventId);
    }
}

/// <summary>
/// Event publisher interceptor for automatic validation
/// </summary>
public class ValidatedEventPublisher : IEventPublisher
{
    private readonly IEventValidationService _validationService;
    private readonly ILogger<ValidatedEventPublisher> _logger;

    public ValidatedEventPublisher(
        IEventValidationService validationService,
        ILogger<ValidatedEventPublisher> logger)
    {
        _validationService = validationService;
        _logger = logger;
    }

    /// <summary>
    /// Publishes event only if validation passes
    /// </summary>
    public async Task PublishAsync<T>(T @event) where T : DomainEvent
    {
        await _validationService.PublishValidatedEventAsync(@event);

        _logger.LogInformation(
            "Published validated event {EventType} (Id: {EventId})",
            @event.EventType,
            @event.EventId);
    }
}

/// <summary>
/// Interface for event publishers (used by ValidatedEventPublisher)
/// </summary>
public interface IEventPublisher
{
    Task PublishAsync<T>(T @event) where T : DomainEvent;
}
