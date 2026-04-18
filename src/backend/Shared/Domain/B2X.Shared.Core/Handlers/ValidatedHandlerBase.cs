using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace B2X.Shared.Core.Handlers;

/// <summary>
/// Base class for handlers and services with common validation patterns.
/// Eliminates code duplication across handlers and services that use FluentValidation.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "Protected field is intentional for derived classes")]
public abstract class ValidatedBase
{
    protected readonly ILogger Logger;

    protected ValidatedBase(ILogger logger)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Validates a request using FluentValidation and returns a standardized error response if validation fails.
    /// Returns null if validation succeeds, allowing the caller to continue processing.
    /// </summary>
    /// <typeparam name="TRequest">The request type to validate</typeparam>
    /// <typeparam name="TResponse">The response type that should be returned on validation failure</typeparam>
    /// <param name="request">The request to validate</param>
    /// <param name="validator">The validator to use</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <param name="createErrorResponse">Function to create the error response from validation errors</param>
    /// <returns>Null if validation succeeds, error response if validation fails</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1068:CancellationToken parameters must come last", Justification = "Func parameter is optional and comes last for readability")]
    protected async Task<TResponse?> ValidateRequestAsync<TRequest, TResponse>(
        TRequest request,
        IValidator<TRequest> validator,
        CancellationToken cancellationToken,
        Func<string, TResponse> createErrorResponse)
        where TResponse : class
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken).ConfigureAwait(false);
        if (!validationResult.IsValid)
        {
            var errorMessage = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            Logger.LogWarning("Validation failed: {Errors}", errorMessage);
            return createErrorResponse(errorMessage);
        }

        return null; // Validation succeeded
    }

    /// <summary>
    /// Validates a request and throws ValidationException if validation fails.
    /// Use this when you want validation failures to be handled by middleware.
    /// </summary>
    /// <typeparam name="TRequest">The request type to validate</typeparam>
    /// <param name="request">The request to validate</param>
    /// <param name="validator">The validator to use</param>
    /// <param name="cancellationToken">Cancellation token</param>
    protected async Task ValidateRequestAsync<TRequest>(
        TRequest request,
        IValidator<TRequest> validator,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken).ConfigureAwait(false);
        if (!validationResult.IsValid)
        {
            var errorMessage = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            Logger.LogWarning("Validation failed: {Errors}", errorMessage);
            throw new ValidationException(validationResult.Errors);
        }
    }
}

/// <summary>
/// Base class for Wolverine handlers with common validation patterns.
/// Inherits from ValidatedBase to provide handler-specific functionality.
/// </summary>
public abstract class ValidatedHandlerBase : ValidatedBase
{
    protected ValidatedHandlerBase(ILogger logger) : base(logger) { }
}

/// <summary>
/// Extension methods for common validation response patterns.
/// </summary>
public static class ValidationResponseExtensions
{
    /// <summary>
    /// Creates a standardized validation error response for handlers that return success/message pattern.
    /// </summary>
    /// <typeparam name="TResponse">The response type</typeparam>
    /// <param name="errorMessage">The validation error message</param>
    /// <param name="createResponse">Function to create the response with success=false and message</param>
    /// <returns>The error response</returns>
    public static TResponse CreateValidationErrorResponse<TResponse>(
        string errorMessage,
        Func<string, TResponse> createResponse)
        where TResponse : class
    {
        return createResponse(errorMessage);
    }
}
