using System;
using System.Collections.Generic;

namespace B2X.SmartDataIntegration.Models
{
    /// <summary>
    /// Result of validating a data mapping configuration
    /// </summary>
    public class MappingValidationResult
    {
        /// <summary>
        /// Unique identifier for the validation result
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Foreign key to the mapping configuration
        /// </summary>
        public Guid DataMappingConfigurationId { get; set; }

        /// <summary>
        /// Whether the validation passed
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Overall validation score (0-100)
        /// </summary>
        public double ValidationScore { get; set; }

        /// <summary>
        /// Collection of validation errors
        /// </summary>
        public ICollection<ValidationError> Errors { get; set; } = new List<ValidationError>();

        /// <summary>
        /// Collection of validation warnings
        /// </summary>
        public ICollection<ValidationWarning> Warnings { get; set; } = new List<ValidationWarning>();

        /// <summary>
        /// Performance metrics from the validation
        /// </summary>
        public ValidationPerformanceMetrics PerformanceMetrics { get; set; } = new ValidationPerformanceMetrics();

        /// <summary>
        /// Sample data used for validation
        /// </summary>
        public string? SampleData { get; set; }

        /// <summary>
        /// When the validation was performed
        /// </summary>
        public DateTime ValidatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// User who performed the validation
        /// </summary>
        public string ValidatedBy { get; set; } = string.Empty;

        /// <summary>
        /// Adds an error to the validation result
        /// </summary>
        public void AddError(string field, string message, ValidationErrorSeverity severity = ValidationErrorSeverity.Error)
        {
            Errors.Add(new ValidationError
            {
                Field = field,
                Message = message,
                Severity = severity
            });
            IsValid = false;
        }

        /// <summary>
        /// Adds a warning to the validation result
        /// </summary>
        public void AddWarning(string field, string message)
        {
            Warnings.Add(new ValidationWarning
            {
                Field = field,
                Message = message
            });
        }
    }

    /// <summary>
    /// Represents a validation error
    /// </summary>
    public class ValidationError
    {
        /// <summary>
        /// Field that caused the error
        /// </summary>
        public string Field { get; set; } = string.Empty;

        /// <summary>
        /// Error message
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Severity of the error
        /// </summary>
        public ValidationErrorSeverity Severity { get; set; }
    }

    /// <summary>
    /// Represents a validation warning
    /// </summary>
    public class ValidationWarning
    {
        /// <summary>
        /// Field that caused the warning
        /// </summary>
        public string Field { get; set; } = string.Empty;

        /// <summary>
        /// Warning message
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// Performance metrics from validation
    /// </summary>
    public class ValidationPerformanceMetrics
    {
        /// <summary>
        /// Time taken to validate (in milliseconds)
        /// </summary>
        public long ValidationTimeMs { get; set; }

        /// <summary>
        /// Memory usage during validation (in bytes)
        /// </summary>
        public long MemoryUsageBytes { get; set; }

        /// <summary>
        /// Number of records processed
        /// </summary>
        public int RecordsProcessed { get; set; }

        /// <summary>
        /// Number of records that failed validation
        /// </summary>
        public int RecordsFailed { get; set; }
    }

    /// <summary>
    /// Severity levels for validation errors
    /// </summary>
    public enum ValidationErrorSeverity
    {
        /// <summary>
        /// Critical error that prevents mapping execution
        /// </summary>
        Critical = 0,

        /// <summary>
        /// Standard error
        /// </summary>
        Error = 1,

        /// <summary>
        /// Warning that should be addressed
        /// </summary>
        Warning = 2
    }
}
