using System.Collections.Generic;
using System.Threading.Tasks;
using B2Connect.SmartDataIntegration.Models;

namespace B2Connect.SmartDataIntegration.Services
{
    /// <summary>
    /// Interface for AI-powered mapping engine that analyzes data structures and suggests mappings
    /// </summary>
    public interface IMappingEngine
    {
        /// <summary>
        /// Analyzes source and target data fields and generates mapping suggestions
        /// </summary>
        Task<IEnumerable<MappingSuggestion>> AnalyzeAndSuggestMappingsAsync(
            IEnumerable<DataField> sourceFields,
            IEnumerable<DataField> targetFields,
            MappingContext context);

        /// <summary>
        /// Learns from user feedback on mapping suggestions to improve future suggestions
        /// </summary>
        Task LearnFromFeedbackAsync(
            MappingSuggestion suggestion,
            bool wasAccepted,
            string? userFeedback = null);

        /// <summary>
        /// Calculates confidence score for a potential field mapping
        /// </summary>
        Task<double> CalculateMappingConfidenceAsync(
            DataField sourceField,
            DataField targetField);

        /// <summary>
        /// Suggests transformation type for mapping between two fields
        /// </summary>
        Task<MappingTransformationType> SuggestTransformationTypeAsync(
            DataField sourceField,
            DataField targetField);

        /// <summary>
        /// Generates transformation parameters for a suggested mapping
        /// </summary>
        Task<string?> GenerateTransformationParametersAsync(
            DataField sourceField,
            DataField targetField,
            MappingTransformationType transformationType);

        /// <summary>
        /// Analyzes historical mapping data to identify patterns
        /// </summary>
        Task<IDictionary<string, double>> AnalyzeMappingPatternsAsync(
            IEnumerable<DataMappingConfiguration> historicalMappings);
    }

    /// <summary>
    /// Context information for mapping analysis
    /// </summary>
    public class MappingContext
    {
        /// <summary>
        /// Source system identifier
        /// </summary>
        public string SourceSystem { get; set; } = string.Empty;

        /// <summary>
        /// Target system identifier
        /// </summary>
        public string TargetSystem { get; set; } = string.Empty;

        /// <summary>
        /// Business domain context (e.g., "ecommerce", "erp", "crm")
        /// </summary>
        public string BusinessDomain { get; set; } = string.Empty;

        /// <summary>
        /// Specific use case (e.g., "product_sync", "customer_import")
        /// </summary>
        public string UseCase { get; set; } = string.Empty;

        /// <summary>
        /// Tenant identifier for context-specific learning
        /// </summary>
        public string TenantId { get; set; } = string.Empty;

        /// <summary>
        /// Historical mapping configurations for pattern learning
        /// </summary>
        public IEnumerable<DataMappingConfiguration>? HistoricalMappings { get; set; }

        /// <summary>
        /// Sample data for analysis (optional)
        /// </summary>
        public object? SampleData { get; set; }
    }
}
