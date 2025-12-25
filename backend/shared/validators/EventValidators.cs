using FluentValidation;
using B2Connect.Types;

namespace B2Connect.Shared.Validators;

/// <summary>
/// Base validator for domain events
/// Provides common validation rules for all event types
/// </summary>
public abstract class DomainEventValidator<TEvent> : AbstractValidator<TEvent>
    where TEvent : DomainEvent
{
    protected DomainEventValidator()
    {
        // Common validation rules for all events
        RuleFor(e => e.EventId)
            .NotEmpty()
            .WithMessage("EventId must not be empty");

        RuleFor(e => e.Timestamp)
            .NotEqual(default(DateTime))
            .WithMessage("Timestamp must be set")
            .LessThanOrEqualTo(DateTime.UtcNow.AddSeconds(1))
            .WithMessage("Timestamp cannot be in the future");

        RuleFor(e => e.AggregateId)
            .NotEqual(Guid.Empty)
            .WithMessage("AggregateId must not be empty");

        RuleFor(e => e.AggregateType)
            .NotEmpty()
            .WithMessage("AggregateType must not be empty")
            .MaximumLength(100)
            .WithMessage("AggregateType must not exceed 100 characters");

        RuleFor(e => e.EventType)
            .NotEmpty()
            .WithMessage("EventType must not be empty")
            .MaximumLength(100)
            .WithMessage("EventType must not exceed 100 characters")
            .Matches(@"^[a-z]+(\.[a-z]+)*$")
            .WithMessage("EventType must be lowercase with dot notation (e.g., 'product.created')");

        RuleFor(e => e.Version)
            .GreaterThan(0)
            .WithMessage("Version must be greater than 0");
    }
}

/// <summary>
/// Validator for Product Domain Events
/// </summary>
/// 
public class ProductEventValidator : DomainEventValidator<dynamic>
{
    public ProductEventValidator()
    {
        // Base validation is inherited
    }
}

/// <summary>
/// Validator for User Domain Events
/// </summary>
public class UserEventValidator : DomainEventValidator<dynamic>
{
    public UserEventValidator()
    {
        // Base validation is inherited
    }
}

/// <summary>
/// Validator for Order Domain Events
/// </summary>
public class OrderEventValidator : DomainEventValidator<dynamic>
{
    public OrderEventValidator()
    {
        // Base validation is inherited
    }
}

/// <summary>
/// Validator for Tenant Domain Events
/// </summary>
public class TenantEventValidator : DomainEventValidator<dynamic>
{
    public TenantEventValidator()
    {
        // Base validation is inherited
    }
}

/// <summary>
/// Generic Event Validator Factory
/// Creates appropriate validators based on event type
/// </summary>
public class EventValidatorFactory
{
    private readonly IValidator<DomainEvent>[] _validators;

    public EventValidatorFactory(params IValidator<DomainEvent>[] validators)
    {
        _validators = validators;
    }

    /// <summary>
    /// Validates a domain event before publishing
    /// </summary>
    public async Task<bool> ValidateEventAsync(DomainEvent @event)
    {
        foreach (var validator in _validators)
        {
            var result = await validator.ValidateAsync(@event);
            if (!result.IsValid)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Validates and returns validation result with errors
    /// </summary>
    public async Task<ValidationResult> ValidateEventWithResultAsync(DomainEvent @event)
    {
        var errors = new List<ValidationFailure>();

        foreach (var validator in _validators)
        {
            var result = await validator.ValidateAsync(@event);
            if (!result.IsValid)
            {
                errors.AddRange(result.Errors);
            }
        }

        return new ValidationResult(errors.Count == 0, errors);
    }
}
