using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using B2Connect.SmartDataIntegration.Models;

namespace B2Connect.SmartDataIntegration.Services
{
    /// <summary>
    /// Main service interface for Smart Data Integration functionality
    /// </summary>
    public interface ISmartDataIntegrationService
    {
        /// <summary>
        /// Creates a new data mapping configuration
        /// </summary>
        Task<DataMappingConfiguration> CreateMappingConfigurationAsync(
            string tenantId,
            string name,
            string description,
            string sourceSystem,
            string targetSystem,
            string createdBy);

        /// <summary>
        /// Gets a mapping configuration by ID
        /// </summary>
        Task<DataMappingConfiguration?> GetMappingConfigurationAsync(Guid id);

        /// <summary>
        /// Gets all mapping configurations for a tenant
        /// </summary>
        Task<IEnumerable<DataMappingConfiguration>> GetMappingConfigurationsAsync(string tenantId);

        /// <summary>
        /// Updates a mapping configuration
        /// </summary>
        Task<DataMappingConfiguration> UpdateMappingConfigurationAsync(
            Guid id,
            string name,
            string description,
            string modifiedBy);

        /// <summary>
        /// Deletes a mapping configuration
        /// </summary>
        Task DeleteMappingConfigurationAsync(Guid id);

        /// <summary>
        /// Analyzes source and target data structures and generates AI-powered mapping suggestions
        /// </summary>
        Task<IEnumerable<MappingSuggestion>> GenerateMappingSuggestionsAsync(
            string sourceSystem,
            IEnumerable<DataField> sourceFields,
            string targetSystem,
            IEnumerable<DataField> targetFields);

        /// <summary>
        /// Applies AI-generated suggestions to create mapping rules
        /// </summary>
        Task<IEnumerable<MappingRule>> ApplyMappingSuggestionsAsync(
            Guid configurationId,
            IEnumerable<MappingSuggestion> suggestions,
            string appliedBy);

        /// <summary>
        /// Validates a mapping configuration
        /// </summary>
        Task<MappingValidationResult> ValidateMappingConfigurationAsync(
            Guid configurationId,
            string? sampleData = null);

        /// <summary>
        /// Executes a data mapping transformation
        /// </summary>
        Task<MappingExecutionResult> ExecuteMappingAsync(
            Guid configurationId,
            object sourceData);

        /// <summary>
        /// Gets mapping execution history
        /// </summary>
        Task<IEnumerable<MappingExecutionResult>> GetMappingExecutionHistoryAsync(
            Guid configurationId,
            DateTime? fromDate = null,
            DateTime? toDate = null);
    }
}
