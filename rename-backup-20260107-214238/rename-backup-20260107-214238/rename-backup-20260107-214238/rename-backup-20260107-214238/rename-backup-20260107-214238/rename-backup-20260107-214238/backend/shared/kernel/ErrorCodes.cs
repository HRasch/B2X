namespace B2Connect.Types;

/// <summary>
/// Standard error codes used throughout the application.
/// Replaces magic strings with type-safe constants.
/// </summary>
public static class ErrorCodes
{
    // Authentication & Authorization
    public const string InvalidCredentials = nameof(InvalidCredentials);
    public const string UserInactive = nameof(UserInactive);
    public const string InvalidToken = nameof(InvalidToken);
    public const string UserNotFound = nameof(UserNotFound);
    public const string Unauthorized = nameof(Unauthorized);
    public const string AccessDenied = nameof(AccessDenied);
    public const string TokenExpired = nameof(TokenExpired);

    // Data Operations
    public const string NotFound = nameof(NotFound);
    public const string AlreadyExists = nameof(AlreadyExists);
    public const string InvalidId = nameof(InvalidId);
    public const string Duplicate = nameof(Duplicate);
    public const string InvalidFormat = nameof(InvalidFormat);

    // Validation
    public const string EmptyKey = nameof(EmptyKey);
    public const string EmptyValue = nameof(EmptyValue);
    public const string EmptyCategory = nameof(EmptyCategory);
    public const string OutOfRange = nameof(OutOfRange);
    public const string InvalidInput = nameof(InvalidInput);

    // Operations
    public const string OperationFailed = nameof(OperationFailed);
    public const string ProviderError = nameof(ProviderError);
    public const string SyncFailed = nameof(SyncFailed);
    public const string TimeoutError = nameof(TimeoutError);
    public const string ConflictError = nameof(ConflictError);
    public const string NoProvidersFound = nameof(NoProvidersFound);

    // Service-Specific
    public const string TwoFactorRequired = nameof(TwoFactorRequired);
    public const string TwoFactorEnabled = nameof(TwoFactorEnabled);
}

/// <summary>
/// HTTP status code mapping for error codes.
/// Enables consistent error response formatting.
/// </summary>
public static class ErrorCodeStatusMap
{
    private static readonly Dictionary<string, int> StatusCodeMap = new()
    {
        // 400 Bad Request
        { ErrorCodes.InvalidToken, 401 },
        { ErrorCodes.InvalidId, 400 },
        { ErrorCodes.InvalidInput, 400 },
        { ErrorCodes.InvalidFormat, 400 },
        { ErrorCodes.EmptyKey, 400 },
        { ErrorCodes.EmptyValue, 400 },
        { ErrorCodes.EmptyCategory, 400 },
        { ErrorCodes.OutOfRange, 400 },

        // 401 Unauthorized
        { ErrorCodes.InvalidCredentials, 401 },
        { ErrorCodes.Unauthorized, 401 },
        { ErrorCodes.TokenExpired, 401 },
        { ErrorCodes.UserInactive, 400 }, // Client's responsibility to fix
        { ErrorCodes.TwoFactorRequired, 400 },

        // 403 Forbidden
        { ErrorCodes.AccessDenied, 403 },

        // 404 Not Found
        { ErrorCodes.NotFound, 404 },
        { ErrorCodes.UserNotFound, 404 },

        // 409 Conflict
        { ErrorCodes.AlreadyExists, 409 },
        { ErrorCodes.Duplicate, 409 },
        { ErrorCodes.ConflictError, 409 },

        // 500 Server Error (default)
        { ErrorCodes.OperationFailed, 500 },
        { ErrorCodes.ProviderError, 500 },
        { ErrorCodes.SyncFailed, 500 },
        { ErrorCodes.TimeoutError, 500 },
        { ErrorCodes.NoProvidersFound, 500 },
    };

    /// <summary>
    /// Get HTTP status code for error code.
    /// Defaults to 500 if not found.
    /// </summary>
    public static int GetStatusCode(string errorCode)
    {
        return StatusCodeMap.TryGetValue(errorCode, out var statusCode) ? statusCode : 500;
    }
}

/// <summary>
/// Extension methods for working with error codes.
/// </summary>
public static class ErrorCodeExtensions
{
    /// <summary>
    /// Gets human-readable error message for error code.
    /// </summary>
    public static string ToMessage(this string errorCode) =>
        errorCode switch
        {
            ErrorCodes.InvalidCredentials => "Invalid email or password",
            ErrorCodes.UserInactive => "User account is inactive",
            ErrorCodes.InvalidToken => "Invalid or expired token",
            ErrorCodes.UserNotFound => "User not found",
            ErrorCodes.Unauthorized => "You are not authorized to perform this action",
            ErrorCodes.AccessDenied => "Access denied",
            ErrorCodes.TokenExpired => "Your token has expired. Please log in again",

            ErrorCodes.NotFound => "Resource not found",
            ErrorCodes.AlreadyExists => "Resource already exists",
            ErrorCodes.InvalidId => "Invalid ID format",
            ErrorCodes.Duplicate => "Duplicate entry",
            ErrorCodes.InvalidFormat => "Invalid format",

            ErrorCodes.EmptyKey => "Key cannot be empty",
            ErrorCodes.EmptyValue => "Value cannot be empty",
            ErrorCodes.EmptyCategory => "Category cannot be empty",
            ErrorCodes.OutOfRange => "Value is out of range",
            ErrorCodes.InvalidInput => "Invalid input provided",

            ErrorCodes.OperationFailed => "Operation failed",
            ErrorCodes.ProviderError => "External provider error",
            ErrorCodes.SyncFailed => "Synchronization failed",
            ErrorCodes.TimeoutError => "Operation timed out",
            ErrorCodes.ConflictError => "Conflict error",
            ErrorCodes.NoProvidersFound => "No providers found",

            ErrorCodes.TwoFactorRequired => "Two-factor authentication required",
            ErrorCodes.TwoFactorEnabled => "Two-factor authentication enabled",

            _ => "An error occurred"
        };

    /// <summary>
    /// Gets HTTP status code for error code.
    /// </summary>
    public static int GetStatusCode(this string errorCode) =>
        ErrorCodeStatusMap.GetStatusCode(errorCode);
}
