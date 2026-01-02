namespace B2Connect.Types;

/// <summary>
/// Extension methods for Result types.
/// Provides functional composition patterns (Map, Bind, etc.).
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Maps the success value to a new type.
    /// If the result is a failure, returns a failure with the same code/message.
    /// </summary>
    /// <example>
    /// var result = await _service.GetUserAsync(id);
    /// var dtoResult = result.Map(user => new UserDto { Id = user.Id, Name = user.Name });
    /// </example>
    public static Result<TNew> Map<T, TNew>(
        this Result<T> result,
        Func<T, TNew> mapper)
    {
        return result switch
        {
            Result<T>.Success s => new Result<TNew>.Success(mapper(s.Value), s.Message),
            Result<T>.Failure f => new Result<TNew>.Failure(f.Code, f.Message, f.Exception),
            _ => throw new InvalidOperationException("Unknown result type")
        };
    }

    /// <summary>
    /// Maps the success value asynchronously to a new type.
    /// If the result is a failure, returns a failure with the same code/message.
    /// </summary>
    public static async Task<Result<TNew>> MapAsync<T, TNew>(
        this Result<T> result,
        Func<T, Task<TNew>> mapper)
    {
        return result switch
        {
            Result<T>.Success s => new Result<TNew>.Success(await mapper(s.Value), s.Message),
            Result<T>.Failure f => new Result<TNew>.Failure(f.Code, f.Message, f.Exception),
            _ => throw new InvalidOperationException("Unknown result type")
        };
    }

    /// <summary>
    /// Binds (flatMaps) to another Result-returning operation.
    /// Chains multiple operations that return results, avoiding nested results.
    /// </summary>
    /// <example>
    /// var result = await _service.GetUserAsync(id)
    ///     .Bind(user => _service.GetUserRoleAsync(user.RoleId));
    /// </example>
    public static Result<TNew> Bind<T, TNew>(
        this Result<T> result,
        Func<T, Result<TNew>> binder)
    {
        return result switch
        {
            Result<T>.Success s => binder(s.Value),
            Result<T>.Failure f => new Result<TNew>.Failure(f.Code, f.Message, f.Exception),
            _ => throw new InvalidOperationException("Unknown result type")
        };
    }

    /// <summary>
    /// Binds asynchronously to another Result-returning operation.
    /// </summary>
    public static async Task<Result<TNew>> BindAsync<T, TNew>(
        this Result<T> result,
        Func<T, Task<Result<TNew>>> binder)
    {
        return result switch
        {
            Result<T>.Success s => await binder(s.Value),
            Result<T>.Failure f => new Result<TNew>.Failure(f.Code, f.Message, f.Exception),
            _ => throw new InvalidOperationException("Unknown result type")
        };
    }

    /// <summary>
    /// Binds a non-generic Result operation.
    /// </summary>
    public static Result Bind<T>(
        this Result<T> result,
        Func<T, Result> binder)
    {
        return result switch
        {
            Result<T>.Success s => binder(s.Value),
            Result<T>.Failure f => new Result.Failure(f.Code, f.Message, f.Exception),
            _ => throw new InvalidOperationException("Unknown result type")
        };
    }

    /// <summary>
    /// Converts a Result&lt;T&gt; to Result by discarding the value.
    /// Useful when you only care about success/failure, not the value.
    /// </summary>
    public static Result ToResult<T>(this Result<T> result) =>
        result switch
        {
            Result<T>.Success s => new Result.Success(s.Message),
            Result<T>.Failure f => new Result.Failure(f.Code, f.Message, f.Exception),
            _ => throw new InvalidOperationException("Unknown result type")
        };

    /// <summary>
    /// Applies a side-effect function if the result is a success.
    /// Returns the original result unchanged for chaining.
    /// </summary>
    /// <example>
    /// var result = await _service.CreateUserAsync(dto)
    ///     .Tap(user => _logger.LogInformation("User created: {UserId}", user.Id));
    /// </example>
    public static Result<T> Tap<T>(
        this Result<T> result,
        Action<T> action)
    {
        if (result is Result<T>.Success s)
        {
            action(s.Value);
        }
        return result;
    }

    /// <summary>
    /// Applies an async side-effect function if the result is a success.
    /// </summary>
    public static async Task<Result<T>> TapAsync<T>(
        this Result<T> result,
        Func<T, Task> action)
    {
        if (result is Result<T>.Success s)
        {
            await action(s.Value);
        }
        return result;
    }

    /// <summary>
    /// Applies a side-effect function if the result is a failure.
    /// Returns the original result unchanged for chaining.
    /// </summary>
    public static Result<T> TapFailure<T>(
        this Result<T> result,
        Action<string, string, Exception?> action)
    {
        if (result is Result<T>.Failure f)
        {
            action(f.Code, f.Message, f.Exception);
        }
        return result;
    }

    /// <summary>
    /// Applies an async side-effect function if the result is a failure.
    /// </summary>
    public static async Task<Result<T>> TapFailureAsync<T>(
        this Result<T> result,
        Func<string, string, Exception?, Task> action)
    {
        if (result is Result<T>.Failure f)
        {
            await action(f.Code, f.Message, f.Exception);
        }
        return result;
    }

    /// <summary>
    /// Returns the failure of the first result, or the second result if the first succeeds.
    /// Useful for combining validation results.
    /// </summary>
    public static Result<T> Or<T>(
        this Result<T> result,
        Result<T> alternative)
    {
        return result switch
        {
            Result<T>.Success => result,
            Result<T>.Failure => alternative,
            _ => throw new InvalidOperationException("Unknown result type")
        };
    }

    /// <summary>
    /// Returns the failure of the first result, or applies a function if it succeeds.
    /// </summary>
    public static Result<T> OrElse<T>(
        this Result<T> result,
        Func<Result<T>> alternative)
    {
        return result switch
        {
            Result<T>.Success => result,
            Result<T>.Failure => alternative(),
            _ => throw new InvalidOperationException("Unknown result type")
        };
    }

    /// <summary>
    /// Converts a task result to an async enumerable that yields on success or failure.
    /// Useful for compatibility with async patterns.
    /// </summary>
    public static async IAsyncEnumerable<T> AsAsyncEnumerable<T>(
        this Task<Result<T>> resultTask)
    {
        var result = await resultTask;
        if (result is Result<T>.Success s)
        {
            yield return s.Value;
        }
    }

    /// <summary>
    /// Combines multiple results into a single result of a collection.
    /// Returns failure if any result fails (first failure wins).
    /// </summary>
    /// <example>
    /// var results = await Task.WhenAll(
    ///     _service.GetUser(1),
    ///     _service.GetUser(2),
    ///     _service.GetUser(3)
    /// );
    /// var combined = results.Combine();
    /// </example>
    public static Result<IEnumerable<T>> Combine<T>(
        this IEnumerable<Result<T>> results)
    {
        var resultList = results.ToList();

        var failures = resultList.OfType<Result<T>.Failure>().FirstOrDefault();
        if (failures != null)
        {
            return new Result<IEnumerable<T>>.Failure(failures.Code, failures.Message, failures.Exception);
        }

        var values = resultList
            .OfType<Result<T>.Success>()
            .Select(s => s.Value)
            .AsEnumerable();

        return new Result<IEnumerable<T>>.Success(values);
    }

    /// <summary>
    /// Converts an exception to a failure result.
    /// Useful for wrapping try-catch blocks at system boundaries.
    /// </summary>
    /// <example>
    /// public async Task&lt;Result&lt;User&gt;&gt; GetUserAsync(int id)
    /// {
    ///     return await ResultExtensions.TryAsync(async () =>
    ///     {
    ///         var user = await _repository.FindAsync(id);
    ///         if (user == null)
    ///             throw new KeyNotFoundException($"User {id} not found");
    ///         return user;
    ///     },
    ///     (ex) => new Result&lt;User&gt;.Failure("DatabaseError", ex.Message, ex));
    /// }
    /// </example>
    public static async Task<Result<T>> TryAsync<T>(
        Func<Task<T>> operation,
        Func<Exception, Result<T>> errorHandler)
    {
        try
        {
            var value = await operation();
            return new Result<T>.Success(value);
        }
        catch (Exception ex)
        {
            return errorHandler(ex);
        }
    }

    /// <summary>
    /// Synchronous version of TryAsync.
    /// </summary>
    public static Result<T> Try<T>(
        Func<T> operation,
        Func<Exception, Result<T>> errorHandler)
    {
        try
        {
            var value = operation();
            return new Result<T>.Success(value);
        }
        catch (Exception ex)
        {
            return errorHandler(ex);
        }
    }
}
