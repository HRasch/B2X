using System;
using System.Collections.Generic;

namespace B2X.SmartDataIntegration.Models
{
    /// <summary>
    /// Result of executing a data mapping transformation
    /// </summary>
    public class MappingExecutionResult
    {
        /// <summary>
        /// Unique identifier for the execution
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Foreign key to the mapping configuration
        /// </summary>
        public Guid DataMappingConfigurationId { get; set; }

        /// <summary>
        /// Whether the execution was successful
        /// </summary>
        public bool IsSuccessful { get; set; }

        /// <summary>
        /// The transformed output data
        /// </summary>
        public object? OutputData { get; set; }

        /// <summary>
        /// Collection of execution errors
        /// </summary>
        public ICollection<MappingExecutionError> Errors { get; set; } = new List<MappingExecutionError>();

        /// <summary>
        /// Collection of execution warnings
        /// </summary>
        public ICollection<MappingExecutionWarning> Warnings { get; set; } = new List<MappingExecutionWarning>();

        /// <summary>
        /// Performance metrics from the execution
        /// </summary>
        public MappingExecutionMetrics Metrics { get; set; } = new MappingExecutionMetrics();

        /// <summary>
        /// When the execution started
        /// </summary>
        public DateTime StartedAt { get; set; }

        /// <summary>
        /// When the execution completed
        /// </summary>
        public DateTime CompletedAt { get; set; }

        /// <summary>
        /// User who initiated the execution
        /// </summary>
        public string ExecutedBy { get; set; } = string.Empty;

        /// <summary>
        /// Input data that was processed (for auditing)
        /// </summary>
        public string? InputData { get; set; }

        /// <summary>
        /// Adds an error to the execution result
        /// </summary>
        public void AddError(string field, string message, string? ruleId = null)
        {
            Errors.Add(new MappingExecutionError
            {
                Field = field,
                Message = message,
                RuleId = ruleId
            });
            IsSuccessful = false;
        }

        /// <summary>
        /// Adds a warning to the execution result
        /// </summary>
        public void AddWarning(string field, string message, string? ruleId = null)
        {
            Warnings.Add(new MappingExecutionWarning
            {
                Field = field,
                Message = message,
                RuleId = ruleId
            });
        }
    }

    /// <summary>
    /// Represents an error that occurred during mapping execution
    /// </summary>
    public class MappingExecutionError
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
        /// ID of the mapping rule that caused the error
        /// </summary>
        public string? RuleId { get; set; }
    }

    /// <summary>
    /// Represents a warning that occurred during mapping execution
    /// </summary>
    public class MappingExecutionWarning
    {
        /// <summary>
        /// Field that caused the warning
        /// </summary>
        public string Field { get; set; } = string.Empty;

        /// <summary>
        /// Warning message
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// ID of the mapping rule that caused the warning
        /// </summary>
        public string? RuleId { get; set; }
    }

    /// <summary>
    /// Performance metrics from mapping execution
    /// </summary>
    public class MappingExecutionMetrics
    {
        /// <summary>
        /// Total execution time (in milliseconds)
        /// </summary>
        public long TotalExecutionTimeMs { get; set; }

        /// <summary>
        /// Time spent on transformation (in milliseconds)
        /// </summary>
        public long TransformationTimeMs { get; set; }

        /// <summary>
        /// Time spent on validation (in milliseconds)
        /// </summary>
        public long ValidationTimeMs { get; set; }

        /// <summary>
        /// Memory usage during execution (in bytes)
        /// </summary>
        public long MemoryUsageBytes { get; set; }

        /// <summary>
        /// Number of rules executed
        /// </summary>
        public int RulesExecuted { get; set; }

        /// <summary>
        /// Number of transformations applied
        /// </summary>
        public int TransformationsApplied { get; set; }
    }
}
