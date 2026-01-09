namespace B2X.AppHost.Services;

/// <summary>
/// Exception thrown when seeding operations fail.
/// Provides structured error information for better error handling and debugging.
/// </summary>
public class SeedingException : Exception
{
    /// <summary>
    /// Error code for categorizing the type of seeding failure.
    /// </summary>
    public string ErrorCode { get; }

    /// <summary>
    /// The phase or service where the error occurred.
    /// </summary>
    public string? FailedPhase { get; }

    /// <summary>
    /// Additional context about the error.
    /// </summary>
    public IReadOnlyDictionary<string, object>? Context { get; }

    public SeedingException(string errorCode, string message)
        : base(message)
    {
        ErrorCode = errorCode ?? throw new ArgumentNullException(nameof(errorCode));
    }

    public SeedingException(string errorCode, string message, Exception innerException)
        : base(message, innerException)
    {
        ErrorCode = errorCode ?? throw new ArgumentNullException(nameof(errorCode));
    }

    public SeedingException(string errorCode, string message, string failedPhase)
        : base(message)
    {
        ErrorCode = errorCode ?? throw new ArgumentNullException(nameof(errorCode));
        FailedPhase = failedPhase;
    }

    public SeedingException(string errorCode, string message, string failedPhase, Exception innerException)
        : base(message, innerException)
    {
        ErrorCode = errorCode ?? throw new ArgumentNullException(nameof(errorCode));
        FailedPhase = failedPhase;
    }

    public SeedingException(string errorCode, string message, string failedPhase, IReadOnlyDictionary<string, object> context)
        : base(message)
    {
        ErrorCode = errorCode ?? throw new ArgumentNullException(nameof(errorCode));
        FailedPhase = failedPhase;
        Context = context;
    }

    public SeedingException(string errorCode, string message, IReadOnlyDictionary<string, object> context)
        : base(message)
    {
        ErrorCode = errorCode ?? throw new ArgumentNullException(nameof(errorCode));
        Context = context;
    }

    public SeedingException(string errorCode, string message, Exception innerException, IReadOnlyDictionary<string, object> context)
        : base(message, innerException)
    {
        ErrorCode = errorCode ?? throw new ArgumentNullException(nameof(errorCode));
        Context = context;
    }

    /// <summary>
    /// Creates a seeding exception for service-specific failures.
    /// </summary>
    public static SeedingException ServiceFailure(string serviceName, string operation, Exception innerException)
    {
        var message = $"Service '{serviceName}' failed during '{operation}' operation";
        var context = new Dictionary<string, object>
(StringComparer.Ordinal)
        {
            ["Service"] = serviceName,
            ["Operation"] = operation,
            ["ErrorType"] = innerException.GetType().Name
        };

        return new SeedingException("SERVICE_FAILURE", message, innerException, context);
    }

    /// <summary>
    /// Creates a seeding exception for service-specific failures (overload for phase-based errors).
    /// </summary>
    public static SeedingException ServiceFailure(string failedPhase, string serviceName)
    {
        var message = $"Service '{serviceName}' failed during seeding phase '{failedPhase}'";
        var context = new Dictionary<string, object>
(StringComparer.Ordinal)
        {
            ["FailedPhase"] = failedPhase,
            ["Service"] = serviceName
        };

        return new SeedingException("SERVICE_FAILURE", message, failedPhase, context);
    }

    /// <summary>
    /// Creates a seeding exception for network/communication failures.
    /// </summary>
    public static SeedingException NetworkFailure(string serviceName, string endpoint, Exception innerException)
    {
        var message = $"Network failure communicating with '{serviceName}' at '{endpoint}'";
        var context = new Dictionary<string, object>
(StringComparer.Ordinal)
        {
            ["Service"] = serviceName,
            ["Endpoint"] = endpoint,
            ["ErrorType"] = "NetworkError"
        };

        return new SeedingException("NETWORK_FAILURE", message, innerException, context);
    }

    /// <summary>
    /// Creates a seeding exception for network/communication failures (overload for phase-based errors).
    /// </summary>
    public static SeedingException NetworkFailure(string failedPhase, string endpoint)
    {
        var message = $"Network failure during seeding phase '{failedPhase}' at '{endpoint}'";
        var context = new Dictionary<string, object>
(StringComparer.Ordinal)
        {
            ["FailedPhase"] = failedPhase,
            ["Endpoint"] = endpoint
        };

        return new SeedingException("NETWORK_FAILURE", message, failedPhase, context);
    }

    /// <summary>
    /// Creates a seeding exception for validation failures.
    /// </summary>
    public static SeedingException ValidationFailure(string entityType, string validationErrors)
    {
        var message = $"Validation failed for '{entityType}': {validationErrors}";
        var context = new Dictionary<string, object>
(StringComparer.Ordinal)
        {
            ["EntityType"] = entityType,
            ["ValidationErrors"] = validationErrors
        };

        return new SeedingException("VALIDATION_FAILURE", message, context);
    }

    /// <summary>
    /// Creates a seeding exception for timeout failures.
    /// </summary>
    public static SeedingException TimeoutFailure(string operation, TimeSpan timeout)
    {
        var message = $"Operation '{operation}' timed out after {timeout.TotalSeconds} seconds";
        var context = new Dictionary<string, object>
(StringComparer.Ordinal)
        {
            ["Operation"] = operation,
            ["TimeoutSeconds"] = timeout.TotalSeconds
        };

        return new SeedingException("TIMEOUT_FAILURE", message, context);
    }

    /// <summary>
    /// Creates a seeding exception for timeout failures (overload for phase-based errors).
    /// </summary>
    public static SeedingException TimeoutFailure(string failedPhase, int timeoutSeconds)
    {
        var timeout = TimeSpan.FromSeconds(timeoutSeconds);
        var message = $"Operation timed out during seeding phase '{failedPhase}' after {timeout.TotalSeconds} seconds";
        var context = new Dictionary<string, object>
(StringComparer.Ordinal)
        {
            ["FailedPhase"] = failedPhase,
            ["TimeoutSeconds"] = timeout.TotalSeconds
        };

        return new SeedingException("TIMEOUT_FAILURE", message, failedPhase, context);
    }

    /// <summary>
    /// Creates a seeding exception for configuration errors.
    /// </summary>
    public static SeedingException ConfigurationError(string configKey, string issue)
    {
        var message = $"Configuration error for '{configKey}': {issue}";
        var context = new Dictionary<string, object>
(StringComparer.Ordinal)
        {
            ["ConfigKey"] = configKey,
            ["Issue"] = issue
        };

        return new SeedingException("CONFIGURATION_ERROR", message, context);
    }

    /// <summary>
    /// Creates a seeding exception for configuration errors (overload for simple messages).
    /// </summary>
    public static SeedingException ConfigurationError(string issue)
    {
        var message = $"Configuration error: {issue}";
        var context = new Dictionary<string, object>
(StringComparer.Ordinal)
        {
            ["Issue"] = issue
        };

        return new SeedingException("CONFIGURATION_ERROR", message, context);
    }
}
