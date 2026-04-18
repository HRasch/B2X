using System.Threading.Tasks;
using B2X.SmartDataIntegration.Models;

namespace B2X.SmartDataIntegration.Services
{
    /// <summary>
    /// Interface for validating data mapping configurations and executions
    /// </summary>
    public interface IMappingValidator
    {
        /// <summary>
        /// Validates a complete mapping configuration
        /// </summary>
        Task<MappingValidationResult> ValidateConfigurationAsync(
            DataMappingConfiguration configuration,
            object? sampleData = null);

        /// <summary>
        /// Validates an individual mapping rule
        /// </summary>
        Task<MappingValidationResult> ValidateRuleAsync(
            MappingRule rule,
            object? sampleData = null);

        /// <summary>
        /// Validates data transformation before execution
        /// </summary>
        Task<MappingValidationResult> ValidateTransformationAsync(
            object sourceData,
            DataMappingConfiguration configuration);

        /// <summary>
        /// Performs security validation on mapping configuration
        /// </summary>
        Task<MappingValidationResult> ValidateSecurityAsync(
            DataMappingConfiguration configuration);

        /// <summary>
        /// Validates data integrity constraints
        /// </summary>
        Task<MappingValidationResult> ValidateDataIntegrityAsync(
            object data,
            DataMappingConfiguration configuration);

        /// <summary>
        /// Validates performance characteristics of a mapping
        /// </summary>
        Task<MappingValidationResult> ValidatePerformanceAsync(
            DataMappingConfiguration configuration,
            int expectedDataVolume);

        /// <summary>
        /// Creates a validation report with detailed findings
        /// </summary>
        Task<string> GenerateValidationReportAsync(
            MappingValidationResult result);
    }
}
