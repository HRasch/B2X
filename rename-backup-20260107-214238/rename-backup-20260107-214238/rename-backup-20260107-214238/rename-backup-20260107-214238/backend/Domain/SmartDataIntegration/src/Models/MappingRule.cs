using System;
using B2X.Shared.Core.Entities;
using B2X.Shared.Kernel;

namespace B2X.SmartDataIntegration.Models
{
    /// <summary>
    /// Represents an individual mapping rule within a data mapping configuration
    /// </summary>
    public class MappingRule : BaseEntity
    {
        /// <summary>
        /// Foreign key to the parent mapping configuration
        /// </summary>
        public Guid DataMappingConfigurationId { get; set; }

        /// <summary>
        /// Reference to the parent mapping configuration
        /// </summary>
        public DataMappingConfiguration DataMappingConfiguration { get; set; } = null!;

        /// <summary>
        /// Source field information
        /// </summary>
        public DataField SourceField { get; set; } = null!;

        /// <summary>
        /// Target field information
        /// </summary>
        public DataField TargetField { get; set; } = null!;

        /// <summary>
        /// Type of transformation to apply
        /// </summary>
        public MappingTransformationType TransformationType { get; set; }

        /// <summary>
        /// Transformation parameters (JSON serialized)
        /// </summary>
        public string? TransformationParameters { get; set; }

        /// <summary>
        /// AI confidence score for this specific rule (0-100)
        /// </summary>
        public double AiConfidenceScore { get; set; }

        /// <summary>
        /// Whether this rule is currently active
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Priority order for rule execution (lower numbers execute first)
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Validation rules for this mapping
        /// </summary>
        public string? ValidationRules { get; set; }

        /// <summary>
        /// Error handling strategy
        /// </summary>
        public MappingErrorHandlingStrategy ErrorHandlingStrategy { get; set; }
    }

    /// <summary>
    /// Types of transformations that can be applied to data during mapping
    /// </summary>
    public enum MappingTransformationType
    {
        /// <summary>
        /// Direct field mapping without transformation
        /// </summary>
        Direct = 0,

        /// <summary>
        /// String concatenation
        /// </summary>
        Concatenate = 1,

        /// <summary>
        /// String splitting
        /// </summary>
        Split = 2,

        /// <summary>
        /// Value lookup from a reference table
        /// </summary>
        Lookup = 3,

        /// <summary>
        /// Mathematical calculation
        /// </summary>
        Calculate = 4,

        /// <summary>
        /// Format conversion (date, number, etc.)
        /// </summary>
        Format = 5,

        /// <summary>
        /// Conditional mapping based on rules
        /// </summary>
        Conditional = 6,

        /// <summary>
        /// Custom transformation using AI/ML
        /// </summary>
        AiTransform = 7
    }

    /// <summary>
    /// Error handling strategies for mapping rules
    /// </summary>
    public enum MappingErrorHandlingStrategy
    {
        /// <summary>
        /// Skip the record and continue processing
        /// </summary>
        Skip = 0,

        /// <summary>
        /// Use a default value
        /// </summary>
        DefaultValue = 1,

        /// <summary>
        /// Stop processing and report error
        /// </summary>
        Fail = 2,

        /// <summary>
        /// Log warning and continue
        /// </summary>
        Warn = 3
    }
}
