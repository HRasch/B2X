using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Schema;
using B2Connect.Shared.Infrastructure.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace B2Connect.Shared.Infrastructure.Validation;

/// <summary>
/// Configuration Validation System for B2Connect services.
/// Validates environment variables and configuration against JSON schemas.
/// Provides fail-fast validation with clear error messages and remediation steps.
/// </summary>
public class ConfigurationValidator
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ConfigurationValidator> _logger;
    private readonly JwtConfigurationValidator _jwtValidator;
    // private readonly Dictionary<string, JsonSchema> _schemas = new();

    public ConfigurationValidator(IConfiguration configuration, ILogger<ConfigurationValidator> logger, ILoggerFactory loggerFactory)
    {
        _configuration = configuration;
        _logger = logger;
        _jwtValidator = new JwtConfigurationValidator(configuration, loggerFactory.CreateLogger<JwtConfigurationValidator>());
        // LoadSchemas();
    }

    /// <summary>
    /// Validates all required configuration sections.
    /// Throws ConfigurationValidationException with detailed error messages if validation fails.
    /// </summary>
    public void ValidateAll()
    {
        var errors = new List<ConfigurationValidationError>();

        // Validate core configuration sections
        errors.AddRange(ValidateJwtConfiguration());
        errors.AddRange(ValidateDatabaseConfiguration());
        errors.AddRange(ValidateServiceConfiguration());
        errors.AddRange(ValidateCorsConfiguration());
        errors.AddRange(ValidateRateLimitingConfiguration());
        errors.AddRange(ValidatePortConfiguration());

        if (errors.Any())
        {
            throw new ConfigurationValidationException("Configuration validation failed", errors);
        }

        _logger.LogInformation("✅ Configuration validation passed for all sections");
    }

    /// <summary>
    /// Validates JWT configuration (secrets, issuer, audience).
    /// </summary>
    private IEnumerable<ConfigurationValidationError> ValidateJwtConfiguration()
    {
        // Use the specialized JWT validator for comprehensive validation
        var jwtErrors = _jwtValidator.Validate();

        // Convert JWT validation errors to configuration validation errors
        foreach (var jwtError in jwtErrors)
        {
            yield return new ConfigurationValidationError
            {
                Section = "Jwt",
                Key = jwtError.Property,
                Value = jwtError.Value,
                Error = jwtError.Error,
                Remediation = jwtError.Remediation
            };
        }
    }

    /// <summary>
    /// Validates database configuration (provider, connection strings).
    /// </summary>
    private List<ConfigurationValidationError> ValidateDatabaseConfiguration()
    {
        var errors = new List<ConfigurationValidationError>();

        var databaseProvider = _configuration["Database:Provider"] ?? _configuration["Database__Provider"];
        if (string.IsNullOrEmpty(databaseProvider))
        {
            errors.Add(new ConfigurationValidationError
            {
                Section = "Database",
                Key = "Provider",
                Value = databaseProvider,
                Error = "Database Provider is required",
                Remediation = "Set 'Database:Provider' to 'postgres', 'sqlite', or 'inmemory'."
            });
        }
        else
        {
            var validProviders = new[] { "postgres", "sqlite", "inmemory" };
            if (!validProviders.Contains(databaseProvider.ToLower()))
            {
                errors.Add(new ConfigurationValidationError
                {
                    Section = "Database",
                    Key = "Provider",
                    Value = databaseProvider,
                    Error = $"Invalid database provider: {databaseProvider}",
                    Remediation = $"Valid providers: {string.Join(", ", validProviders)}"
                });
            }
        }

        // Validate connection strings if not using in-memory
        if (databaseProvider?.ToLower() != "inmemory")
        {
            var connectionString = _configuration.GetConnectionString("AuthDb") ??
                                   _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                errors.Add(new ConfigurationValidationError
                {
                    Section = "ConnectionStrings",
                    Key = "AuthDb",
                    Value = connectionString,
                    Error = "Database connection string is required when not using in-memory database",
                    Remediation = "Set 'ConnectionStrings:AuthDb' or 'ConnectionStrings:DefaultConnection'."
                });
            }
        }

        return errors;
    }

    /// <summary>
    /// Validates service configuration (ports, URLs, service discovery).
    /// </summary>
    private List<ConfigurationValidationError> ValidateServiceConfiguration()
    {
        var errors = new List<ConfigurationValidationError>();

        // Validate service URLs and ports
        var urls = _configuration["ASPNETCORE_URLS"];
        if (string.IsNullOrEmpty(urls))
        {
            errors.Add(new ConfigurationValidationError
            {
                Section = "ASPNETCORE",
                Key = "URLS",
                Value = urls,
                Error = "ASPNETCORE_URLS is required",
                Remediation = "Set 'ASPNETCORE_URLS' to specify service endpoints (e.g., 'http://localhost:5000')."
            });
        }

        return errors;
    }

    /// <summary>
    /// Validates CORS configuration.
    /// </summary>
    private List<ConfigurationValidationError> ValidateCorsConfiguration()
    {
        var errors = new List<ConfigurationValidationError>();

        var corsOrigins = _configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
        if (corsOrigins == null || corsOrigins.Length == 0)
        {
            errors.Add(new ConfigurationValidationError
            {
                Section = "Cors",
                Key = "AllowedOrigins",
                Value = string.Join(", ", corsOrigins ?? Array.Empty<string>()),
                Error = "CORS AllowedOrigins is required",
                Remediation = "Set 'Cors:AllowedOrigins' array with allowed origin URLs."
            });
        }
        else
        {
            // Validate URL format
            foreach (var origin in corsOrigins)
            {
                if (!Uri.TryCreate(origin, UriKind.Absolute, out _))
                {
                    errors.Add(new ConfigurationValidationError
                    {
                        Section = "Cors",
                        Key = "AllowedOrigins",
                        Value = origin,
                        Error = $"Invalid CORS origin URL format: {origin}",
                        Remediation = "Ensure all CORS origins are valid absolute URLs (e.g., 'https://example.com')."
                    });
                }
            }
        }

        return errors;
    }

    /// <summary>
    /// Validates rate limiting configuration.
    /// </summary>
    private List<ConfigurationValidationError> ValidateRateLimitingConfiguration()
    {
        var errors = new List<ConfigurationValidationError>();

        var generalLimit = _configuration.GetValue<int?>("RateLimiting:GeneralLimit");
        if (!generalLimit.HasValue || generalLimit.Value <= 0)
        {
            errors.Add(new ConfigurationValidationError
            {
                Section = "RateLimiting",
                Key = "GeneralLimit",
                Value = generalLimit?.ToString(),
                Error = "RateLimiting:GeneralLimit must be a positive integer",
                Remediation = "Set 'RateLimiting:GeneralLimit' to a positive integer (e.g., 100)."
            });
        }

        return errors;
    }

    /// <summary>
    /// Validates port configuration and detects conflicts.
    /// </summary>
    private List<ConfigurationValidationError> ValidatePortConfiguration()
    {
        var errors = new List<ConfigurationValidationError>();

        // Check for port conflicts in ASPNETCORE_URLS
        var urls = _configuration["ASPNETCORE_URLS"];
        if (!string.IsNullOrEmpty(urls))
        {
            var ports = new HashSet<int>();
            var urlParts = urls.Split(';');

            foreach (var url in urlParts)
            {
                if (Uri.TryCreate(url.Trim(), UriKind.Absolute, out var uri))
                {
                    if (ports.Contains(uri.Port))
                    {
                        errors.Add(new ConfigurationValidationError
                        {
                            Section = "ASPNETCORE",
                            Key = "URLS",
                            Value = urls,
                            Error = $"Port conflict detected: {uri.Port} is used multiple times",
                            Remediation = "Use unique ports for each service endpoint in ASPNETCORE_URLS."
                        });
                    }
                    else
                    {
                        ports.Add(uri.Port);
                    }
                }
            }
        }

        return errors;
    }

    /// <summary>
    /// Loads JSON schemas for configuration validation.
    /// </summary>
    // private void LoadSchemas()
    // {
    //     try
    //     {
    //         // Load JWT configuration schema
    //         var jwtSchemaPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Schemas", "jwt-configuration.schema.json");
    //         if (File.Exists(jwtSchemaPath))
    //         {
    //             var jwtSchemaJson = File.ReadAllText(jwtSchemaPath);
    //             var jwtSchema = JsonSerializer.Deserialize<JsonSchema>(jwtSchemaJson);
    //             if (jwtSchema != null)
    //             {
    //                 _schemas["jwt"] = jwtSchema;
    //                 _logger.LogInformation("✅ Loaded JWT configuration schema");
    //             }
    //         }
    //         else
    //         {
    //             _logger.LogWarning("⚠️ JWT configuration schema not found at {Path}", jwtSchemaPath);
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "❌ Failed to load configuration schemas");
    //     }
    // }
}

/// <summary>
/// Exception thrown when configuration validation fails.
/// </summary>
public class ConfigurationValidationException : Exception
{
    public IReadOnlyList<ConfigurationValidationError> Errors { get; }

    public ConfigurationValidationException(string message, IEnumerable<ConfigurationValidationError> errors)
        : base(message)
    {
        Errors = errors.ToList().AsReadOnly();
    }
}

/// <summary>
/// Represents a configuration validation error.
/// </summary>
public class ConfigurationValidationError
{
    public string Section { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string? Value { get; set; }
    public string Error { get; set; } = string.Empty;
    public string Remediation { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"[{Section}:{Key}] {Error}\n  Value: '{Value}'\n  Fix: {Remediation}";
    }
}
