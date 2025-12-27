using Serilog.Core;
using Serilog.Events;
using System.Collections.Generic;
using System.Linq;

namespace B2Connect.Infrastructure.Logging;

/// <summary>
/// Serilog enricher that redacts sensitive data from log messages and properties.
/// Prevents credential leakage via logs (password, token, email, phone, SSN, credit card numbers).
/// Complies with GDPR and other data protection regulations.
/// </summary>
public class SensitiveDataEnricher : ILogEventEnricher
{
    // List of property names that should be redacted (case-insensitive)
    private static readonly HashSet<string> SensitiveFieldNames = new(StringComparer.OrdinalIgnoreCase)
    {
        // Authentication & Security
        "password", "passwd", "pwd", "secret",
        "token", "accesstoken", "refreshtoken", "bearertoken",
        "apikey", "api_key", "key",
        "hmackey", "encryptionkey",
        
        // Personal Identification
        "email", "e-mail", "emailaddress",
        "phone", "phonenumber", "mobile", "telephone",
        "ssn", "socialsecuritynumber", "taxid", "taxnumber",
        "personalidnumber",
        
        // Financial
        "creditcard", "cardnumber", "cc",
        "bankaccount", "accountnumber",
        "routingnumber", "swift",
        "pin",
        
        // Credentials
        "username", "user_name",
        "clientid", "client_id",
        "clientsecret", "client_secret",
        
        // Sensitive data patterns
        "authorization", "auth",
        "bearer",
        "session",
        "cookie",
        "nonce",
        "signature",
        "checksum"
    };

    /// <summary>
    /// Enriches log events by redacting sensitive data from both message and properties.
    /// </summary>
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        try
        {
            // Redact properties
            var redactedProperties = logEvent.Properties
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => RedactProperty(kvp.Key, kvp.Value, propertyFactory)
                );

            // Clear and rebuild properties with redacted values
            logEvent.RemovePropertyIfPresent("Properties");
            foreach (var kvp in redactedProperties)
            {
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(kvp.Key, kvp.Value));
            }

            // Redact message text
            if (!string.IsNullOrEmpty(logEvent.MessageTemplate.Text))
            {
                var redactedMessage = RedactMessage(logEvent.MessageTemplate.Text);
                if (redactedMessage != logEvent.MessageTemplate.Text)
                {
                    // Update the message template if sensitive data was found
                    var template = new Serilog.Parsing.MessageTemplateParser()
                        .Parse(redactedMessage);
                    logEvent = logEvent.WithMessageTemplate(template);
                }
            }
        }
        catch (Exception ex)
        {
            // If enrichment fails, log the error but don't prevent logging
            System.Diagnostics.Debug.WriteLine($"Error in SensitiveDataEnricher: {ex.Message}");
        }
    }

    /// <summary>
    /// Redacts the value of a property if its name matches sensitive field patterns.
    /// </summary>
    private LogEventPropertyValue RedactProperty(string propertyName, LogEventPropertyValue value, ILogEventPropertyFactory propertyFactory)
    {
        if (IsSensitiveField(propertyName))
        {
            return propertyFactory.CreateProperty("[REDACTED]").Value;
        }

        // Check if it's a scalar value containing sensitive patterns
        if (value is ScalarValue scalarValue && scalarValue.Value is string stringValue)
        {
            if (ContainsSensitivePattern(stringValue))
            {
                return propertyFactory.CreateProperty("[REDACTED]").Value;
            }
        }

        // Recursively check dictionary values
        if (value is DictionaryValue dictValue)
        {
            var redactedDict = new Dictionary<ScalarValue, LogEventPropertyValue>();
            foreach (var kvp in dictValue.Elements)
            {
                var key = kvp.Key as ScalarValue;
                var redactedValue = RedactProperty(key?.Value?.ToString() ?? "", kvp.Value, propertyFactory);
                if (key != null)
                {
                    redactedDict[key] = redactedValue;
                }
            }
            return propertyFactory.CreateProperty(redactedDict).Value;
        }

        return value;
    }

    /// <summary>
    /// Redacts sensitive patterns in plain text messages.
    /// </summary>
    private string RedactMessage(string message)
    {
        if (string.IsNullOrEmpty(message))
            return message;

        // Redact email addresses
        message = System.Text.RegularExpressions.Regex.Replace(
            message,
            @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}",
            "[EMAIL_REDACTED]",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase
        );

        // Redact JWT tokens (Bearer tokens)
        message = System.Text.RegularExpressions.Regex.Replace(
            message,
            @"(Bearer|bearer)\s+[A-Za-z0-9\-_\.]+",
            "$1 [TOKEN_REDACTED]"
        );

        // Redact API keys (generic pattern)
        message = System.Text.RegularExpressions.Regex.Replace(
            message,
            @"(api[_-]?key|apikey)[=:\s]+[A-Za-z0-9\-_\.]+",
            "$1=[KEY_REDACTED]",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase
        );

        // Redact passwords (generic pattern)
        message = System.Text.RegularExpressions.Regex.Replace(
            message,
            @"(password|passwd|pwd)[=:\s]+[^\s]+",
            "$1=[PASSWORD_REDACTED]",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase
        );

        // Redact credit card numbers (basic pattern)
        message = System.Text.RegularExpressions.Regex.Replace(
            message,
            @"\b(?:\d{4}[-\s]?){3}\d{4}\b",
            "[CARD_REDACTED]"
        );

        // Redact SSN pattern (XXX-XX-XXXX)
        message = System.Text.RegularExpressions.Regex.Replace(
            message,
            @"\b\d{3}-\d{2}-\d{4}\b",
            "[SSN_REDACTED]"
        );

        // Redact phone numbers (basic patterns)
        message = System.Text.RegularExpressions.Regex.Replace(
            message,
            @"\b(?:\+?1[-.\s]?)?\(?\d{3}\)?[-.\s]?\d{3}[-.\s]?\d{4}\b",
            "[PHONE_REDACTED]"
        );

        return message;
    }

    /// <summary>
    /// Determines if a property name indicates sensitive data.
    /// </summary>
    private bool IsSensitiveField(string fieldName)
    {
        return SensitiveFieldNames.Contains(fieldName);
    }

    /// <summary>
    /// Checks if a string value contains sensitive patterns that should be redacted.
    /// </summary>
    private bool ContainsSensitivePattern(string value)
    {
        if (string.IsNullOrEmpty(value))
            return false;

        // Check for JWT-like tokens (many dots and alphanumeric/special chars)
        if (System.Text.RegularExpressions.Regex.IsMatch(value, @"^[A-Za-z0-9\-_\.]{50,}$"))
            return true;

        // Check for common credential patterns
        if (System.Text.RegularExpressions.Regex.IsMatch(
            value,
            @"^(Bearer\s+|Basic\s+)[A-Za-z0-9\-_\.]+$",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase))
            return true;

        return false;
    }
}

/// <summary>
/// Extension method to add sensitive data enricher to Serilog configuration.
/// </summary>
public static class SensitiveDataEnricherExtensions
{
    /// <summary>
    /// Adds the SensitiveDataEnricher to the Serilog configuration.
    /// Call this in Program.cs when configuring Serilog.
    /// 
    /// Usage: 
    /// config.Enrich.With(new SensitiveDataEnricher());
    /// </summary>
    /// <param name="enrichmentConfiguration">The enrichment configuration from Serilog</param>
    /// <returns>The enrichment configuration for method chaining</returns>
    public static Serilog.Configuration.LoggerEnrichmentConfiguration WithSensitiveDataRedaction(
        this Serilog.Configuration.LoggerEnrichmentConfiguration enrichmentConfiguration)
    {
        return enrichmentConfiguration.With(new SensitiveDataEnricher());
    }
}
