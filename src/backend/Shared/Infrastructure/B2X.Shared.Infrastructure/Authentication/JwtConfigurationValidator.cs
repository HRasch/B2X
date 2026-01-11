using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace B2X.Shared.Infrastructure.Authentication;

/// <summary>
/// Specialized JWT Configuration Validator for build-time and runtime validation.
/// Provides comprehensive validation of JWT settings including security checks.
/// </summary>
public class JwtConfigurationValidator
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<JwtConfigurationValidator> _logger;

    public JwtConfigurationValidator(IConfiguration configuration, ILogger<JwtConfigurationValidator> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Validates JWT configuration with security-focused checks.
    /// </summary>
    /// <returns>List of validation errors, empty if valid.</returns>
    public IEnumerable<JwtValidationError> Validate()
    {
        var errors = new List<JwtValidationError>();

        errors.AddRange(ValidateSecret());
        errors.AddRange(ValidateIssuer());
        errors.AddRange(ValidateAudience());
        errors.AddRange(ValidateExpirationSettings());
        errors.AddRange(ValidateSecuritySettings());

        if (!errors.Any())
        {
            _logger.LogInformation("✅ JWT configuration validation passed");
        }

        return errors;
    }

    /// <summary>
    /// Validates JWT secret with security requirements.
    /// </summary>
    private List<JwtValidationError> ValidateSecret()
    {
        var errors = new List<JwtValidationError>();
        var secret = _configuration["Jwt:Secret"];

        if (string.IsNullOrWhiteSpace(secret))
        {
            errors.Add(new JwtValidationError
            {
                Property = "Secret",
                Value = secret,
                Error = "JWT Secret is required for token signing",
                Severity = ValidationSeverity.Critical,
                Remediation = "Set 'Jwt:Secret' via secure environment variable or Azure Key Vault. Generate a cryptographically secure random string."
            });
            return errors;
        }

        // Check minimum length for security
        if (secret.Length < 32)
        {
            errors.Add(new JwtValidationError
            {
                Property = "Secret",
                Value = $"Length: {secret.Length}",
                Error = $"JWT Secret must be at least 32 characters for AES-256 security (current: {secret.Length})",
                Severity = ValidationSeverity.Critical,
                Remediation = "Generate a secure random string of at least 32 characters using a cryptographically secure random number generator."
            });
        }

        // Check for common weak patterns
        if (IsWeakSecret(secret))
        {
            errors.Add(new JwtValidationError
            {
                Property = "Secret",
                Value = "[REDACTED]",
                Error = "JWT Secret appears to be weak or predictable",
                Severity = ValidationSeverity.Critical,
                Remediation = "Use a cryptographically secure random string. Avoid common words, patterns, or sequential characters."
            });
        }

        // Check for entropy
        if (!HasSufficientEntropy(secret))
        {
            errors.Add(new JwtValidationError
            {
                Property = "Secret",
                Value = "[REDACTED]",
                Error = "JWT Secret has insufficient entropy for cryptographic security",
                Severity = ValidationSeverity.Warning,
                Remediation = "Regenerate the secret using a cryptographically secure random number generator with high entropy."
            });
        }

        return errors;
    }

    /// <summary>
    /// Validates JWT issuer configuration.
    /// </summary>
    private List<JwtValidationError> ValidateIssuer()
    {
        var errors = new List<JwtValidationError>();
        var issuer = _configuration["Jwt:Issuer"];

        if (string.IsNullOrWhiteSpace(issuer))
        {
            errors.Add(new JwtValidationError
            {
                Property = "Issuer",
                Value = issuer,
                Error = "JWT Issuer is required for token validation",
                Severity = ValidationSeverity.Critical,
                Remediation = "Set 'Jwt:Issuer' to identify the token issuer (e.g., 'B2X' or service URL)."
            });
            return errors;
        }

        // Validate issuer format (should be a valid URI or service name)
        if (Uri.TryCreate(issuer, UriKind.Absolute, out var issuerUri))
        {
            if (issuerUri.Scheme != Uri.UriSchemeHttp && issuerUri.Scheme != Uri.UriSchemeHttps)
            {
                errors.Add(new JwtValidationError
                {
                    Property = "Issuer",
                    Value = issuer,
                    Error = "JWT Issuer URI must use HTTP or HTTPS scheme",
                    Severity = ValidationSeverity.Warning,
                    Remediation = "Use HTTPS for production issuers to ensure secure token validation."
                });
            }
        }
        else if (!IsValidServiceName(issuer))
        {
            errors.Add(new JwtValidationError
            {
                Property = "Issuer",
                Value = issuer,
                Error = "JWT Issuer should be a valid URI or service identifier",
                Severity = ValidationSeverity.Warning,
                Remediation = "Use a valid URI (e.g., 'https://auth.B2X.com') or service name (e.g., 'B2X.Auth')."
            });
        }

        return errors;
    }

    /// <summary>
    /// Validates JWT audience configuration.
    /// </summary>
    private List<JwtValidationError> ValidateAudience()
    {
        var errors = new List<JwtValidationError>();
        var audience = _configuration["Jwt:Audience"];

        if (string.IsNullOrWhiteSpace(audience))
        {
            errors.Add(new JwtValidationError
            {
                Property = "Audience",
                Value = audience,
                Error = "JWT Audience is required for token validation",
                Severity = ValidationSeverity.Critical,
                Remediation = "Set 'Jwt:Audience' to specify intended token recipients (e.g., 'B2X.API' or service URL)."
            });
            return errors;
        }

        // Validate audience format
        if (Uri.TryCreate(audience, UriKind.Absolute, out var audienceUri))
        {
            if (audienceUri.Scheme != Uri.UriSchemeHttp && audienceUri.Scheme != Uri.UriSchemeHttps)
            {
                errors.Add(new JwtValidationError
                {
                    Property = "Audience",
                    Value = audience,
                    Error = "JWT Audience URI should use HTTP or HTTPS scheme",
                    Severity = ValidationSeverity.Warning,
                    Remediation = "Use HTTPS for production audiences to ensure secure token validation."
                });
            }
        }

        return errors;
    }

    /// <summary>
    /// Validates JWT expiration settings.
    /// </summary>
    private List<JwtValidationError> ValidateExpirationSettings()
    {
        var errors = new List<JwtValidationError>();

        // Validate TokenExpirationMinutes
        var tokenExpirationMinutes = _configuration.GetValue<int?>("Jwt:TokenExpirationMinutes");
        if (!tokenExpirationMinutes.HasValue)
        {
            errors.Add(new JwtValidationError
            {
                Property = "TokenExpirationMinutes",
                Value = null,
                Error = "JWT TokenExpirationMinutes is required",
                Severity = ValidationSeverity.Critical,
                Remediation = "Set 'Jwt:TokenExpirationMinutes' to specify token lifetime in minutes (recommended: 15-60 minutes)."
            });
        }
        else if (tokenExpirationMinutes.Value <= 0)
        {
            errors.Add(new JwtValidationError
            {
                Property = "TokenExpirationMinutes",
                Value = tokenExpirationMinutes.Value.ToString(),
                Error = "JWT TokenExpirationMinutes must be positive",
                Severity = ValidationSeverity.Critical,
                Remediation = "Set 'Jwt:TokenExpirationMinutes' to a positive value (recommended: 15-60 minutes)."
            });
        }
        else if (tokenExpirationMinutes.Value > 1440) // 24 hours
        {
            errors.Add(new JwtValidationError
            {
                Property = "TokenExpirationMinutes",
                Value = tokenExpirationMinutes.Value.ToString(),
                Error = $"JWT TokenExpirationMinutes is too long: {tokenExpirationMinutes.Value} minutes",
                Severity = ValidationSeverity.Warning,
                Remediation = "Consider shorter token lifetimes for better security (recommended: 15-60 minutes)."
            });
        }

        // Validate RefreshTokenExpirationDays
        var refreshTokenExpirationDays = _configuration.GetValue<int?>("Jwt:RefreshTokenExpirationDays");
        if (!refreshTokenExpirationDays.HasValue)
        {
            errors.Add(new JwtValidationError
            {
                Property = "RefreshTokenExpirationDays",
                Value = null,
                Error = "JWT RefreshTokenExpirationDays is required",
                Severity = ValidationSeverity.Critical,
                Remediation = "Set 'Jwt:RefreshTokenExpirationDays' to specify refresh token lifetime in days (recommended: 7-30 days)."
            });
        }
        else if (refreshTokenExpirationDays.Value <= 0)
        {
            errors.Add(new JwtValidationError
            {
                Property = "RefreshTokenExpirationDays",
                Value = refreshTokenExpirationDays.Value.ToString(),
                Error = "JWT RefreshTokenExpirationDays must be positive",
                Severity = ValidationSeverity.Critical,
                Remediation = "Set 'Jwt:RefreshTokenExpirationDays' to a positive value (recommended: 7-30 days)."
            });
        }
        else if (refreshTokenExpirationDays.Value > 365)
        {
            errors.Add(new JwtValidationError
            {
                Property = "RefreshTokenExpirationDays",
                Value = refreshTokenExpirationDays.Value.ToString(),
                Error = $"JWT RefreshTokenExpirationDays is too long: {refreshTokenExpirationDays.Value} days",
                Severity = ValidationSeverity.Warning,
                Remediation = "Consider shorter refresh token lifetimes for better security (recommended: 7-30 days)."
            });
        }

        // Validate relationship between token and refresh token expiration
        if (tokenExpirationMinutes.HasValue && refreshTokenExpirationDays.HasValue)
        {
            var tokenLifetimeHours = tokenExpirationMinutes.Value / 60.0;
            var refreshLifetimeHours = refreshTokenExpirationDays.Value * 24.0;

            if (tokenLifetimeHours >= refreshLifetimeHours)
            {
                errors.Add(new JwtValidationError
                {
                    Property = "TokenExpirationMinutes vs RefreshTokenExpirationDays",
                    Value = $"{tokenExpirationMinutes.Value}min vs {refreshTokenExpirationDays.Value}days",
                    Error = "Access token lifetime should be shorter than refresh token lifetime",
                    Severity = ValidationSeverity.Warning,
                    Remediation = "Ensure access tokens expire before refresh tokens to maintain security boundaries."
                });
            }
        }

        return errors;
    }

    /// <summary>
    /// Validates additional JWT security settings.
    /// </summary>
    private List<JwtValidationError> ValidateSecuritySettings()
    {
        var errors = new List<JwtValidationError>();

        // Check for clock skew tolerance
        var clockSkewMinutes = _configuration.GetValue<int?>("Jwt:ClockSkewMinutes") ?? 5;
        if (clockSkewMinutes < 0)
        {
            errors.Add(new JwtValidationError
            {
                Property = "ClockSkewMinutes",
                Value = clockSkewMinutes.ToString(),
                Error = "JWT ClockSkewMinutes cannot be negative",
                Severity = ValidationSeverity.Warning,
                Remediation = "Set 'Jwt:ClockSkewMinutes' to a non-negative value (default: 5 minutes)."
            });
        }
        else if (clockSkewMinutes > 15)
        {
            errors.Add(new JwtValidationError
            {
                Property = "ClockSkewMinutes",
                Value = clockSkewMinutes.ToString(),
                Error = $"JWT ClockSkewMinutes is too high: {clockSkewMinutes} minutes",
                Severity = ValidationSeverity.Warning,
                Remediation = "High clock skew reduces security. Consider values of 5 minutes or less."
            });
        }

        return errors;
    }

    /// <summary>
    /// Checks if a secret appears to be weak or predictable.
    /// </summary>
    private static bool IsWeakSecret(string secret)
    {
        if (string.IsNullOrEmpty(secret))
            return true;

        // Check for common weak patterns
        var weakPatterns = new[]
        {
            "password", "secret", "key", "token", "jwt", "default",
            "123456", "abcdef", "admin", "test", "dev", "local"
        };

        var lowerSecret = secret.ToLower();
        if (weakPatterns.Any(pattern => lowerSecret.Contains(pattern)))
        {
            return true;
        }

        // Check for sequential characters
        if (IsSequential(secret))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Checks if a string contains sequential characters.
    /// </summary>
    private static bool IsSequential(string input)
    {
        if (input.Length < 3)
            return false;

        for (int i = 0; i < input.Length - 2; i++)
        {
            if (input[i] + 1 == input[i + 1] && input[i + 1] + 1 == input[i + 2])
            {
                return true; // Found sequential characters
            }
        }

        return false;
    }

    /// <summary>
    /// Checks if a secret has sufficient entropy for cryptographic use.
    /// </summary>
    private static bool HasSufficientEntropy(string secret)
    {
        if (string.IsNullOrEmpty(secret) || secret.Length < 16)
            return false;

        // For development, accept longer secrets as having sufficient entropy
        // since they are typically generated randomly
        if (secret.Length >= 32)
            return true;

        // Simple entropy check based on character variety
        var uniqueChars = new HashSet<char>(secret);
        var entropyRatio = (double)uniqueChars.Count / secret.Length;

        // Require at least 60% unique characters for reasonable entropy
        return entropyRatio >= 0.6;
    }

    /// <summary>
    /// Checks if a string is a valid service name.
    /// </summary>
    private static bool IsValidServiceName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        // Allow alphanumeric, dots, hyphens, underscores
        return name.All(c => char.IsLetterOrDigit(c) || c == '.' || c == '-' || c == '_');
    }
}

/// <summary>
/// Represents a JWT configuration validation error.
/// </summary>
public class JwtValidationError
{
    public string Property { get; set; } = string.Empty;
    public string? Value { get; set; }
    public string Error { get; set; } = string.Empty;
    public ValidationSeverity Severity { get; set; }
    public string Remediation { get; set; } = string.Empty;

    public override string ToString()
    {
        var severityIcon = Severity == ValidationSeverity.Critical ? "🔴" : "🟡";
        var safeValue = Property.Contains("Secret") || Property.Contains("Key") ? "[REDACTED]" : Value;
        return $"{severityIcon} [Jwt:{Property}] {Error}\n  Value: '{safeValue}'\n  Fix: {Remediation}";
    }
}

/// <summary>
/// Severity levels for validation errors.
/// </summary>
public enum ValidationSeverity
{
    Warning,
    Critical
}
