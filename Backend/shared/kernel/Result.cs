namespace B2Connect.Types;

/// <summary>
/// Result type for operations that can succeed or fail without returning a value.
/// Used for operations where the outcome (success/failure) is important,
/// but no specific value is returned.
/// </summary>
public abstract record Result
{
    /// <summary>
    /// Represents a successful operation result.
    /// </summary>
    public sealed record Success(string Message = "") : Result;

    /// <summary>
    /// Represents a failed operation result.
    /// </summary>
    /// <param name="Code">Error code for programmatic handling (e.g., "NotFound", "InvalidInput")</param>
    /// <param name="Message">Human-readable error message for logging/display</param>
    /// <param name="Exception">Optional inner exception for debugging</param>
    public sealed record Failure(string Code, string Message, Exception? Exception = null) : Result;

    /// <summary>
    /// Matches on the result type and applies appropriate function.
    /// Explicit handling enforces success/failure path awareness.
    /// </summary>
    /// <example>
    /// var result = await _service.DoSomethingAsync();
    /// var message = result.Match(
    ///     onSuccess: (msg) => $"✅ {msg}",
    ///     onFailure: (code, msg) => $"❌ [{code}] {msg}"
    /// );
    /// </example>
    public TResult Match<TResult>(
        Func<string, TResult> onSuccess,
        Func<string, string, TResult> onFailure)
    {
        return this switch
        {
            Success s => onSuccess(s.Message),
            Failure f => onFailure(f.Code, f.Message),
            _ => throw new InvalidOperationException("Unknown result type")
        };
    }

    /// <summary>
    /// Folds (reduces) the result into a single value.
    /// Similar to Match but with different semantics (accumulation vs. branching).
    /// </summary>
    public TResult Fold<TResult>(
        TResult onSuccess,
        Func<string, string, TResult> onFailure)
    {
        return this switch
        {
            Success => onSuccess,
            Failure f => onFailure(f.Code, f.Message),
            _ => throw new InvalidOperationException("Unknown result type")
        };
    }
}

/// <summary>
/// Result type for operations that return a value on success or failure.
/// Used for most operations that retrieve/process data.
/// </summary>
/// <typeparam name="T">Type of value returned on success</typeparam>
/// <example>
/// public Result&lt;User&gt; GetUser(int id)
/// {
///     if (id &lt;= 0)
///         return new Result&lt;User&gt;.Failure("InvalidId", "ID must be positive");
///
///     var user = _repository.Find(id);
///     return user == null
///         ? new Result&lt;User&gt;.Failure("NotFound", $"User {id} not found")
///         : new Result&lt;User&gt;.Success(user, "User loaded successfully");
/// }
/// </example>
public abstract record Result<T> : Result
{
    /// <summary>
    /// Represents a successful operation with a value.
    /// </summary>
    /// <param name="Value">The resulting value from the operation</param>
    /// <param name="Message">Optional success message</param>
    public new sealed record Success(T Value, string Message = "") : Result<T>;

    /// <summary>
    /// Represents a failed operation.
    /// </summary>
    /// <param name="Code">Error code for programmatic handling</param>
    /// <param name="Message">Human-readable error message</param>
    /// <param name="Exception">Optional inner exception for debugging</param>
    public new sealed record Failure(string Code, string Message, Exception? Exception = null) : Result<T>;

    /// <summary>
    /// Matches on the result type and applies appropriate function.
    /// Enforces explicit handling of success and failure cases.
    /// </summary>
    /// <example>
    /// var result = await _userService.GetUserAsync(42);
    /// var response = result.Match(
    ///     onSuccess: (user, msg) => Ok(new { data = user, message = msg }),
    ///     onFailure: (code, msg) => code switch
    ///     {
    ///         "NotFound" => NotFound(new { error = code, message = msg }),
    ///         "InvalidId" => BadRequest(new { error = code, message = msg }),
    ///         _ => StatusCode(500, new { error = code, message = msg })
    ///     }
    /// );
    /// </example>
    public TResult Match<TResult>(
        Func<T, string, TResult> onSuccess,
        Func<string, string, TResult> onFailure)
    {
        return this switch
        {
            Success s => onSuccess(s.Value, s.Message),
            Failure f => onFailure(f.Code, f.Message),
            _ => throw new InvalidOperationException("Unknown result type")
        };
    }

    /// <summary>
    /// Folds the result into a single value.
    /// </summary>
    public TResult Fold<TResult>(
        Func<T, TResult> onSuccess,
        Func<string, string, TResult> onFailure)
    {
        return this switch
        {
            Success s => onSuccess(s.Value),
            Failure f => onFailure(f.Code, f.Message),
            _ => throw new InvalidOperationException("Unknown result type")
        };
    }

    /// <summary>
    /// Gets the value or a default if the result is a failure.
    /// </summary>
    public T GetValueOrDefault(T defaultValue) =>
        this switch
        {
            Success s => s.Value,
            Failure => defaultValue,
            _ => defaultValue
        };

    /// <summary>
    /// Gets the value or null if the result is a failure.
    /// Only works with nullable T.
    /// </summary>
    public T? GetValueOrNull() =>
        this switch
        {
            Success s => s.Value,
            Failure => default,
            _ => default
        };

    /// <summary>
    /// Checks if the result is a success.
    /// </summary>
    public bool IsSuccess => this is Success;

    /// <summary>
    /// Checks if the result is a failure.
    /// </summary>
    public bool IsFailure => this is Failure;
}
