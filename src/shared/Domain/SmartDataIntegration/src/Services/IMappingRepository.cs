using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using B2X.SmartDataIntegration.Models;

namespace B2X.SmartDataIntegration.Services
{
    /// <summary>
    /// Repository interface for data mapping configurations and related entities
    /// </summary>
    public interface IMappingRepository
    {
        // DataMappingConfiguration operations
        Task<DataMappingConfiguration?> GetByIdAsync(Guid id);
        Task<IEnumerable<DataMappingConfiguration>> GetByTenantAsync(string tenantId);
        Task<IEnumerable<DataMappingConfiguration>> GetActiveByTenantAsync(string tenantId);
        Task<DataMappingConfiguration> AddAsync(DataMappingConfiguration configuration);
        Task<DataMappingConfiguration> UpdateAsync(DataMappingConfiguration configuration);
        Task DeleteAsync(Guid id);

        // MappingRule operations
        Task<MappingRule?> GetRuleByIdAsync(Guid id);
        Task<IEnumerable<MappingRule>> GetRulesByConfigurationAsync(Guid configurationId);
        Task<IEnumerable<MappingRule>> GetActiveRulesByConfigurationAsync(Guid configurationId);
        Task<MappingRule> AddRuleAsync(MappingRule rule);
        Task<MappingRule> UpdateRuleAsync(MappingRule rule);
        Task DeleteRuleAsync(Guid id);

        // MappingSuggestion operations
        Task<IEnumerable<MappingSuggestion>> GetSuggestionsByConfigurationAsync(Guid configurationId);
        Task<MappingSuggestion> AddSuggestionAsync(MappingSuggestion suggestion);
        Task UpdateSuggestionAsync(MappingSuggestion suggestion);
        Task DeleteSuggestionAsync(Guid id);

        // MappingValidationResult operations
        Task<IEnumerable<MappingValidationResult>> GetValidationResultsAsync(Guid configurationId);
        Task<MappingValidationResult> AddValidationResultAsync(MappingValidationResult result);

        // MappingExecutionResult operations
        Task<IEnumerable<MappingExecutionResult>> GetExecutionResultsAsync(
            Guid configurationId,
            DateTime? fromDate = null,
            DateTime? toDate = null);
        Task<MappingExecutionResult> AddExecutionResultAsync(MappingExecutionResult result);

        // Analytics and reporting
        Task<IDictionary<string, int>> GetMappingStatisticsAsync(string tenantId);
        Task<IEnumerable<DataMappingConfiguration>> GetConfigurationsBySystemAsync(
            string sourceSystem,
            string targetSystem);
        Task<double> GetAverageConfidenceScoreAsync(string tenantId);
    }
}
