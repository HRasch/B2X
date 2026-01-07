// <copyright file="ProviderResult.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

namespace B2X.ERP.Core;

/// <summary>
/// Represents the result of an ERP provider operation with standardized error handling.
/// </summary>
/// <typeparam name="T">The type of the result value.</typeparam>
public sealed record ProviderResult<T>
{
    private ProviderResult(bool isSuccess, T? value, ProviderError? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the result value if the operation was successful.
    /// </summary>
    public T? Value { get; }

    /// <summary>
    /// Gets the error details if the operation failed.
    /// </summary>
    public ProviderError? Error { get; }

    /// <summary>
    /// Creates a successful result with the specified value.
    /// </summary>
    public static ProviderResult<T> Success(T value) => new(true, value, null);

    /// <summary>
    /// Creates a failed result with the specified error.
    /// </summary>
    public static ProviderResult<T> Failure(ProviderError error) => new(false, default, error);

    /// <summary>
    /// Creates a failed result with the specified error code and message.
    /// </summary>
    public static ProviderResult<T> Failure(string code, string message)
        => new(false, default, new ProviderError(code, message));

    /// <summary>
    /// Creates a failed result from an exception.
    /// </summary>
    public static ProviderResult<T> FromException(Exception ex)
        => new(false, default, ProviderError.FromException(ex));

    /// <summary>
    /// Gets the data value (alias for Value for convenience).
    /// </summary>
    public T? Data => Value;

    /// <summary>
    /// Gets the error code if the operation failed.
    /// </summary>
    public string? ErrorCode => Error?.Code;

    /// <summary>
    /// Gets the error message if the operation failed.
    /// </summary>
    public string? ErrorMessage => Error?.Message;

    /// <summary>
    /// Gets the exception if the failure was caused by an exception.
    /// </summary>
#pragma warning disable CA2201 // Do not raise reserved exception types
    public Exception? Exception => Error?.Code == "EXCEPTION" ? new Exception(Error.Message) : null;
#pragma warning restore CA2201 // Do not raise reserved exception types

    /// <summary>
    /// Pattern matches on the result, executing the appropriate action.
    /// </summary>
    /// <param name="onSuccess">Action to execute if successful.</param>
    /// <param name="onFailure">Action to execute if failed.</param>
    public void Match(Action<T> onSuccess, Action<string?, string?> onFailure)
    {
        if (IsSuccess && Value != null)
        {
            onSuccess(Value);
        }
        else
        {
            onFailure(ErrorCode, ErrorMessage);
        }
    }

    /// <summary>
    /// Maps the result value to a new type if successful.
    /// </summary>
    /// <typeparam name="TNew">The new result type.</typeparam>
    /// <param name="mapper">Function to transform the value.</param>
    /// <returns>A new result with the mapped value, or the original error.</returns>
    public ProviderResult<TNew> Map<TNew>(Func<T, TNew> mapper)
    {
        if (IsSuccess && Value != null)
        {
            return ProviderResult<TNew>.Success(mapper(Value));
        }

        return Error != null
            ? ProviderResult<TNew>.Failure(Error)
            : ProviderResult<TNew>.Failure("UNKNOWN", "Unknown error");
    }
}

/// <summary>
/// Represents an error from an ERP provider operation.
/// </summary>
public sealed record ProviderError
{
    public ProviderError(string code, string message, string? details = null)
    {
        Code = code;
        Message = message;
        Details = details;
        Timestamp = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Gets the error code for programmatic handling.
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Gets the human-readable error message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets additional error details.
    /// </summary>
    public string? Details { get; }

    /// <summary>
    /// Gets the timestamp when the error occurred.
    /// </summary>
    public DateTimeOffset Timestamp { get; }

    /// <summary>
    /// Creates an error from an exception.
    /// </summary>
    public static ProviderError FromException(Exception ex) => new(ex.GetType().Name, ex.Message, ex.StackTrace);
    // Common error codes
    public static class Codes
    {
        public const string NotFound = "ERP_NOT_FOUND";
        public const string Timeout = "ERP_TIMEOUT";
        public const string ConnectionFailed = "ERP_CONNECTION_FAILED";
        public const string InvalidOperation = "ERP_INVALID_OPERATION";
        public const string ValidationFailed = "ERP_VALIDATION_FAILED";
        public const string Unauthorized = "ERP_UNAUTHORIZED";
        public const string RateLimited = "ERP_RATE_LIMITED";
        public const string InternalError = "ERP_INTERNAL_ERROR";
    }
}

/// <summary>
/// Non-generic result for operations that don't return a value.
/// </summary>
public sealed record ProviderResult
{
    private ProviderResult(bool isSuccess, ProviderError? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public ProviderError? Error { get; }

    public static ProviderResult Success() => new(true, null);
    public static ProviderResult Failure(ProviderError error) => new(false, error);
    public static ProviderResult Failure(string code, string message)
        => new(false, new ProviderError(code, message));
    public static ProviderResult FromException(Exception ex)
        => new(false, ProviderError.FromException(ex));
}
