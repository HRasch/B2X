// <copyright file="ProviderResult.cs" company="NissenVelten">
// Copyright (c) NissenVelten. All rights reserved.
// </copyright>

namespace B2X.ERP.Core;

/// <summary>
/// Represents the result of a provider operation.
/// </summary>
/// <typeparam name="T">The type of the data.</typeparam>
public class ProviderResult<T>
{
    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the data if the operation was successful.
    /// </summary>
    public T? Data { get; }

    /// <summary>
    /// Gets the error code if the operation failed.
    /// </summary>
    public string? ErrorCode { get; }

    /// <summary>
    /// Gets the error message if the operation failed.
    /// </summary>
    public string? ErrorMessage { get; }

    private ProviderResult(bool isSuccess, T? data, string? errorCode, string? errorMessage)
    {
        IsSuccess = isSuccess;
        Data = data;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// Creates a successful result with data.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <returns>A successful result.</returns>
    public static ProviderResult<T> Success(T data)
    {
        return new ProviderResult<T>(true, data, null, null);
    }

    /// <summary>
    /// Creates a failure result with error code and message.
    /// </summary>
    /// <param name="errorCode">The error code.</param>
    /// <param name="errorMessage">The error message.</param>
    /// <returns>A failure result.</returns>
    public static ProviderResult<T> Failure(string errorCode, string errorMessage)
    {
        return new ProviderResult<T>(false, default, errorCode, errorMessage);
    }

    /// <summary>
    /// Creates a failure result from an exception.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <returns>A failure result.</returns>
    public static ProviderResult<T> FromException(Exception exception)
    {
        return new ProviderResult<T>(false, default, exception.GetType().Name, exception.Message);
    }

    /// <summary>
    /// Matches the result and executes the appropriate action.
    /// </summary>
    /// <param name="onSuccess">The action to execute on success.</param>
    /// <param name="onFailure">The action to execute on failure.</param>
    public void Match(Action<T> onSuccess, Action<string, string> onFailure)
    {
        if (IsSuccess)
        {
            onSuccess(Data!);
        }
        else
        {
            onFailure(ErrorCode!, ErrorMessage!);
        }
    }

    /// <summary>
    /// Maps the result to a new type.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="mapper">The mapping function.</param>
    /// <returns>The mapped result.</returns>
    public ProviderResult<TResult> Map<TResult>(Func<T, TResult> mapper)
    {
        return IsSuccess
            ? ProviderResult<TResult>.Success(mapper(Data!))
            : ProviderResult<TResult>.Failure(ErrorCode!, ErrorMessage!);
    }
}
