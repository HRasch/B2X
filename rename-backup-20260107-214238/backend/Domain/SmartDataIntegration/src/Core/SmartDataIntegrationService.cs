using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using B2X.SmartDataIntegration.Models;
using B2X.SmartDataIntegration.Services;
using Microsoft.Extensions.Logging;

namespace B2X.SmartDataIntegration.Core;

/// <summary>
/// Core implementation of the Smart Data Integration service
/// </summary>
public class SmartDataIntegrationService : ISmartDataIntegrationService
{
    private readonly IMappingRepository _repository;
    private readonly IMappingEngine _mappingEngine;
    private readonly IMappingValidator _validator;
    private readonly ILogger<SmartDataIntegrationService> _logger;

    public SmartDataIntegrationService(
        IMappingRepository repository,
        IMappingEngine mappingEngine,
        IMappingValidator validator,
        ILogger<SmartDataIntegrationService> logger)
    {
        _repository = repository;
        _mappingEngine = mappingEngine;
        _validator = validator;
        _logger = logger;
    }

    public async Task<DataMappingConfiguration> CreateMappingConfigurationAsync(
        string tenantId,
        string name,
        string description,
        string sourceSystem,
        string targetSystem,
        string createdBy)
    {
        if (!Guid.TryParse(tenantId, out var tenantGuid))
        {
            throw new ArgumentException("Invalid tenant ID format", nameof(tenantId));
        }

        _logger.LogInformation("Creating mapping configuration for tenant {TenantId}: {Name}",
            tenantId, name);

        var configuration = new DataMappingConfiguration
        {
            Id = Guid.NewGuid(),
            TenantId = tenantGuid,
            Name = name,
            Description = description,
            SourceSystem = sourceSystem,
            TargetSystem = targetSystem,
            CreatedBy = Guid.TryParse(createdBy, out var createdByGuid) ? createdByGuid : null,
            ModifiedBy = Guid.TryParse(createdBy, out var modifiedByGuid) ? modifiedByGuid : null
        };

        var result = await _repository.AddAsync(configuration);
        _logger.LogInformation("Created mapping configuration {Id}", result.Id);

        return result;
    }

    public async Task<DataMappingConfiguration?> GetMappingConfigurationAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<DataMappingConfiguration>> GetMappingConfigurationsAsync(string tenantId)
    {
        return await _repository.GetByTenantAsync(tenantId);
    }

    public async Task<DataMappingConfiguration> UpdateMappingConfigurationAsync(
        Guid id,
        string name,
        string description,
        string modifiedBy)
    {
        var configuration = await _repository.GetByIdAsync(id);
        if (configuration == null)
        {
            throw new KeyNotFoundException($"Mapping configuration {id} not found");
        }

        configuration.Name = name;
        configuration.Description = description;
        configuration.ModifiedBy = Guid.TryParse(modifiedBy, out var modifiedByGuid) ? modifiedByGuid : null;

        var result = await _repository.UpdateAsync(configuration);
        _logger.LogInformation("Updated mapping configuration {Id}", result.Id);

        return result;
    }

    public async Task DeleteMappingConfigurationAsync(Guid id)
    {
        _logger.LogInformation("Deleting mapping configuration {Id}", id);
        await _repository.DeleteAsync(id);
    }

    public async Task<IEnumerable<MappingSuggestion>> GenerateMappingSuggestionsAsync(
        string sourceSystem,
        IEnumerable<DataField> sourceFields,
        string targetSystem,
        IEnumerable<DataField> targetFields)
    {
        _logger.LogInformation("Generating mapping suggestions from {SourceSystem} to {TargetSystem}",
            sourceSystem, targetSystem);

        var context = new MappingContext
        {
            SourceSystem = sourceSystem,
            TargetSystem = targetSystem,
            BusinessDomain = "integration",
            UseCase = "data_mapping"
        };

        var suggestions = await _mappingEngine.AnalyzeAndSuggestMappingsAsync(
            sourceFields,
            targetFields,
            context);

        _logger.LogInformation("Generated {Count} mapping suggestions", suggestions.Count());
        return suggestions;
    }

    public async Task<IEnumerable<MappingRule>> ApplyMappingSuggestionsAsync(
        Guid configurationId,
        IEnumerable<MappingSuggestion> suggestions,
        string appliedBy)
    {
        var suggestionsList = suggestions.ToList();
        _logger.LogInformation("Applying {Count} mapping suggestions to configuration {ConfigurationId}",
            suggestionsList.Count, configurationId);

        var acceptedSuggestions = suggestionsList.Where(s => s.IsAccepted);
        var rules = new List<MappingRule>();

        foreach (var suggestion in acceptedSuggestions)
        {
            var rule = suggestion.ToMappingRule(configurationId);
            var addedRule = await _repository.AddRuleAsync(rule);
            rules.Add(addedRule);

            // Learn from the accepted suggestion
            await _mappingEngine.LearnFromFeedbackAsync(suggestion, true);
        }

        // Update the configuration's AI confidence score
        await UpdateConfigurationConfidenceScoreAsync(configurationId);

        _logger.LogInformation("Applied {Count} mapping rules", rules.Count);
        return rules;
    }

    public async Task<MappingValidationResult> ValidateMappingConfigurationAsync(
        Guid configurationId,
        string? sampleData = null)
    {
        _logger.LogInformation("Validating mapping configuration {ConfigurationId}", configurationId);

        var configuration = await _repository.GetByIdAsync(configurationId);
        if (configuration == null)
        {
            throw new KeyNotFoundException($"Mapping configuration {configurationId} not found");
        }

        var result = await _validator.ValidateConfigurationAsync(configuration, sampleData);

        // Save validation result
        await _repository.AddValidationResultAsync(result);

        // Update configuration's last validated timestamp
        configuration.LastValidatedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(configuration);

        _logger.LogInformation("Validation completed for configuration {ConfigurationId}: {IsValid}",
            configurationId, result.IsValid);

        return result;
    }

    public async Task<MappingExecutionResult> ExecuteMappingAsync(
        Guid configurationId,
        object sourceData)
    {
        _logger.LogInformation("Executing mapping for configuration {ConfigurationId}", configurationId);

        var configuration = await _repository.GetByIdAsync(configurationId);
        if (configuration == null)
        {
            throw new KeyNotFoundException($"Mapping configuration {configurationId} not found");
        }

        var startedAt = DateTime.UtcNow;
        var result = new MappingExecutionResult
        {
            Id = Guid.NewGuid(),
            DataMappingConfigurationId = configurationId,
            StartedAt = startedAt,
            ExecutedBy = "system" // TODO: Get from current user context
        };

        try
        {
            // Apply mapping rules to transform the data
            var rules = await _repository.GetActiveRulesByConfigurationAsync(configurationId);
            var transformedData = await ApplyMappingRulesAsync(sourceData, rules);

            result.OutputData = transformedData;
            result.IsSuccessful = true;
            result.CompletedAt = DateTime.UtcNow;

            _logger.LogInformation("Mapping execution successful for configuration {ConfigurationId}",
                configurationId);
        }
        catch (Exception ex)
        {
            result.IsSuccessful = false;
            result.CompletedAt = DateTime.UtcNow;
            result.AddError("execution", $"Mapping execution failed: {ex.Message}");

            _logger.LogError(ex, "Mapping execution failed for configuration {ConfigurationId}",
                configurationId);
        }

        // Save execution result
        await _repository.AddExecutionResultAsync(result);

        return result;
    }

    public async Task<IEnumerable<MappingExecutionResult>> GetMappingExecutionHistoryAsync(
        Guid configurationId,
        DateTime? fromDate = null,
        DateTime? toDate = null)
    {
        return await _repository.GetExecutionResultsAsync(configurationId, fromDate, toDate);
    }

    private async Task<object> ApplyMappingRulesAsync(object sourceData, IEnumerable<MappingRule> rules)
    {
        // TODO: Implement actual transformation logic
        // This is a placeholder for the transformation engine
        // In a real implementation, this would apply each mapping rule to transform the data

        _logger.LogDebug("Applying {Count} mapping rules", rules.Count());

        // For now, return the source data unchanged
        // This will be implemented in Phase 2
        return sourceData;
    }

    private async Task UpdateConfigurationConfidenceScoreAsync(Guid configurationId)
    {
        var rules = await _repository.GetActiveRulesByConfigurationAsync(configurationId);
        if (rules.Any())
        {
            var averageConfidence = rules.Average(r => r.AiConfidenceScore);
            var configuration = await _repository.GetByIdAsync(configurationId);
            if (configuration != null)
            {
                configuration.AiConfidenceScore = averageConfidence;
                await _repository.UpdateAsync(configuration);
            }
        }
    }
}
