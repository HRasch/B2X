using System;
using System.Collections.Generic;

namespace B2X.SmartDataIntegration.Models
{
    /// <summary>
    /// Represents an AI-generated suggestion for data mapping
    /// </summary>
    public class MappingSuggestion
    {
        /// <summary>
        /// Unique identifier for the suggestion
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Foreign key to the mapping configuration
        /// </summary>
        public Guid DataMappingConfigurationId { get; set; }

        /// <summary>
        /// Source field that this suggestion applies to
        /// </summary>
        public DataField SourceField { get; set; } = null!;

        /// <summary>
        /// Suggested target field
        /// </summary>
        public DataField SuggestedTargetField { get; set; } = null!;

        /// <summary>
        /// Confidence score of the AI suggestion (0-100)
        /// </summary>
        public double ConfidenceScore { get; set; }

        /// <summary>
        /// Type of transformation suggested
        /// </summary>
        public MappingTransformationType SuggestedTransformation { get; set; }

        /// <summary>
        /// Parameters for the suggested transformation
        /// </summary>
        public string? TransformationParameters { get; set; }

        /// <summary>
        /// Reasoning provided by the AI for this suggestion
        /// </summary>
        public string Reasoning { get; set; } = string.Empty;

        /// <summary>
        /// Alternative suggestions with lower confidence
        /// </summary>
        public ICollection<MappingSuggestion> Alternatives { get; set; } = new List<MappingSuggestion>();

        /// <summary>
        /// Whether this suggestion has been accepted by the user
        /// </summary>
        public bool IsAccepted { get; set; }

        /// <summary>
        /// Whether this suggestion has been rejected by the user
        /// </summary>
        public bool IsRejected { get; set; }

        /// <summary>
        /// User feedback on the suggestion
        /// </summary>
        public string? UserFeedback { get; set; }

        /// <summary>
        /// When this suggestion was generated
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// When this suggestion was last updated
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Creates a mapping rule from this accepted suggestion
        /// </summary>
        public MappingRule ToMappingRule(Guid configurationId)
        {
            return new MappingRule
            {
                Id = Guid.NewGuid(),
                DataMappingConfigurationId = configurationId,
                SourceField = SourceField,
                TargetField = SuggestedTargetField,
                TransformationType = SuggestedTransformation,
                TransformationParameters = TransformationParameters,
                AiConfidenceScore = ConfidenceScore,
                IsActive = true,
                Priority = 0,
                ErrorHandlingStrategy = MappingErrorHandlingStrategy.Warn,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };
        }
    }
}
