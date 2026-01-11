using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using B2X.SmartDataIntegration.Models;
using B2X.SmartDataIntegration.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace B2X.SmartDataIntegration.Infrastructure.Data;

/// <summary>
/// Repository implementation for Smart Data Integration operations
/// </summary>
public class SmartDataIntegrationRepository : IMappingRepository
{
    private readonly SmartDataIntegrationDbContext _context;
    private readonly ILogger<SmartDataIntegrationRepository> _logger;

    public SmartDataIntegrationRepository(
        SmartDataIntegrationDbContext context,
        ILogger<SmartDataIntegrationRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    #region DataMappingConfiguration operations

    public async Task<DataMappingConfiguration?> GetByIdAsync(Guid id)
    {
        return await _context.DataMappingConfigurations
            .Include(x => x.MappingRules.Where(r => r.IsActive))
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<DataMappingConfiguration>> GetByTenantAsync(string tenantId)
    {
        if (!Guid.TryParse(tenantId, out var tenantGuid))
        {
            return Enumerable.Empty<DataMappingConfiguration>();
        }

        return await _context.DataMappingConfigurations
            .Where(x => x.TenantId == tenantGuid)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<DataMappingConfiguration>> GetActiveByTenantAsync(string tenantId)
    {
        if (!Guid.TryParse(tenantId, out var tenantGuid))
        {
            return Enumerable.Empty<DataMappingConfiguration>();
        }

        return await _context.DataMappingConfigurations
            .Where(x => x.TenantId == tenantGuid && x.IsActive)
            .Include(x => x.MappingRules.Where(r => r.IsActive))
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<DataMappingConfiguration> AddAsync(DataMappingConfiguration configuration)
    {
        _context.DataMappingConfigurations.Add(configuration);
        await _context.SaveChangesAsync();
        return configuration;
    }

    public async Task<DataMappingConfiguration> UpdateAsync(DataMappingConfiguration configuration)
    {
        configuration.ModifiedAt = DateTime.UtcNow;
        _context.DataMappingConfigurations.Update(configuration);
        await _context.SaveChangesAsync();
        return configuration;
    }

    public async Task DeleteAsync(Guid id)
    {
        var configuration = await GetByIdAsync(id);
        if (configuration != null)
        {
            _context.DataMappingConfigurations.Remove(configuration);
            await _context.SaveChangesAsync();
        }
    }

    #endregion

    #region MappingRule operations

    public async Task<MappingRule?> GetRuleByIdAsync(Guid id)
    {
        return await _context.MappingRules
            .Include(x => x.DataMappingConfiguration)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<MappingRule>> GetRulesByConfigurationAsync(Guid configurationId)
    {
        return await _context.MappingRules
            .Where(x => x.DataMappingConfigurationId == configurationId)
            .OrderBy(x => x.Priority)
            .ThenBy(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<MappingRule>> GetActiveRulesByConfigurationAsync(Guid configurationId)
    {
        return await _context.MappingRules
            .Where(x => x.DataMappingConfigurationId == configurationId && x.IsActive)
            .OrderBy(x => x.Priority)
            .ThenBy(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<MappingRule> AddRuleAsync(MappingRule rule)
    {
        _context.MappingRules.Add(rule);
        await _context.SaveChangesAsync();
        return rule;
    }

    public async Task<MappingRule> UpdateRuleAsync(MappingRule rule)
    {
        rule.ModifiedAt = DateTime.UtcNow;
        _context.MappingRules.Update(rule);
        await _context.SaveChangesAsync();
        return rule;
    }

    public async Task DeleteRuleAsync(Guid id)
    {
        var rule = await GetRuleByIdAsync(id);
        if (rule != null)
        {
            _context.MappingRules.Remove(rule);
            await _context.SaveChangesAsync();
        }
    }

    #endregion

    #region MappingSuggestion operations

    public async Task<IEnumerable<MappingSuggestion>> GetSuggestionsByConfigurationAsync(Guid configurationId)
    {
        return await _context.MappingSuggestions
            .Where(x => x.DataMappingConfigurationId == configurationId)
            .OrderByDescending(x => x.ConfidenceScore)
            .ThenByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<MappingSuggestion> AddSuggestionAsync(MappingSuggestion suggestion)
    {
        _context.MappingSuggestions.Add(suggestion);
        await _context.SaveChangesAsync();
        return suggestion;
    }

    public async Task UpdateSuggestionAsync(MappingSuggestion suggestion)
    {
        suggestion.UpdatedAt = DateTime.UtcNow;
        _context.MappingSuggestions.Update(suggestion);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteSuggestionAsync(Guid id)
    {
        var suggestion = await _context.MappingSuggestions.FindAsync(id);
        if (suggestion != null)
        {
            _context.MappingSuggestions.Remove(suggestion);
            await _context.SaveChangesAsync();
        }
    }

    #endregion

    #region MappingValidationResult operations

    public async Task<IEnumerable<MappingValidationResult>> GetValidationResultsAsync(Guid configurationId)
    {
        return await _context.MappingValidationResults
            .Where(x => x.DataMappingConfigurationId == configurationId)
            .Include(x => x.Errors)
            .Include(x => x.Warnings)
            .OrderByDescending(x => x.ValidatedAt)
            .ToListAsync();
    }

    public async Task<MappingValidationResult> AddValidationResultAsync(MappingValidationResult result)
    {
        _context.MappingValidationResults.Add(result);
        await _context.SaveChangesAsync();
        return result;
    }

    #endregion

    #region MappingExecutionResult operations

    public async Task<IEnumerable<MappingExecutionResult>> GetExecutionResultsAsync(
        Guid configurationId,
        DateTime? fromDate = null,
        DateTime? toDate = null)
    {
        var query = _context.MappingExecutionResults
            .Where(x => x.DataMappingConfigurationId == configurationId);

        if (fromDate.HasValue)
        {
            query = query.Where(x => x.StartedAt >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(x => x.StartedAt <= toDate.Value);
        }

        return await query
            .Include(x => x.Errors)
            .Include(x => x.Warnings)
            .OrderByDescending(x => x.StartedAt)
            .ToListAsync();
    }

    public async Task<MappingExecutionResult> AddExecutionResultAsync(MappingExecutionResult result)
    {
        _context.MappingExecutionResults.Add(result);
        await _context.SaveChangesAsync();
        return result;
    }

    #endregion

    #region Analytics and reporting

    public async Task<IDictionary<string, int>> GetMappingStatisticsAsync(string tenantId)
    {
        if (!Guid.TryParse(tenantId, out var tenantGuid))
        {
            return new Dictionary<string, int>();
        }

        var stats = new Dictionary<string, int>();

        stats["TotalConfigurations"] = await _context.DataMappingConfigurations
            .CountAsync(x => x.TenantId == tenantGuid);

        stats["ActiveConfigurations"] = await _context.DataMappingConfigurations
            .CountAsync(x => x.TenantId == tenantGuid && x.IsActive);

        stats["TotalRules"] = await _context.MappingRules
            .Where(x => x.DataMappingConfiguration!.TenantId == tenantGuid)
            .CountAsync();

        stats["ActiveRules"] = await _context.MappingRules
            .Where(x => x.DataMappingConfiguration!.TenantId == tenantGuid && x.IsActive)
            .CountAsync();

        stats["TotalSuggestions"] = await _context.MappingSuggestions
            .Join(_context.DataMappingConfigurations,
                  s => s.DataMappingConfigurationId,
                  c => c.Id,
                  (s, c) => new { Suggestion = s, Configuration = c })
            .CountAsync(x => x.Configuration.TenantId == tenantGuid);

        stats["AcceptedSuggestions"] = await _context.MappingSuggestions
            .Join(_context.DataMappingConfigurations,
                  s => s.DataMappingConfigurationId,
                  c => c.Id,
                  (s, c) => new { Suggestion = s, Configuration = c })
            .CountAsync(x => x.Configuration.TenantId == tenantGuid && x.Suggestion.IsAccepted);

        stats["TotalExecutions"] = await _context.MappingExecutionResults
            .Join(_context.DataMappingConfigurations,
                  e => e.DataMappingConfigurationId,
                  c => c.Id,
                  (e, c) => new { Execution = e, Configuration = c })
            .CountAsync(x => x.Configuration.TenantId == tenantGuid);

        stats["SuccessfulExecutions"] = await _context.MappingExecutionResults
            .Join(_context.DataMappingConfigurations,
                  e => e.DataMappingConfigurationId,
                  c => c.Id,
                  (e, c) => new { Execution = e, Configuration = c })
            .CountAsync(x => x.Configuration.TenantId == tenantGuid && x.Execution.IsSuccessful);

        return stats;
    }

    public async Task<IEnumerable<DataMappingConfiguration>> GetConfigurationsBySystemAsync(
        string sourceSystem,
        string targetSystem)
    {
        return await _context.DataMappingConfigurations
            .Where(x => x.SourceSystem == sourceSystem && x.TargetSystem == targetSystem && x.IsActive)
            .Include(x => x.MappingRules.Where(r => r.IsActive))
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<double> GetAverageConfidenceScoreAsync(string tenantId)
    {
        if (!Guid.TryParse(tenantId, out var tenantGuid))
        {
            return 0.0;
        }

        return await _context.DataMappingConfigurations
            .Where(x => x.TenantId == tenantGuid && x.IsActive)
            .AverageAsync(x => x.AiConfidenceScore);
    }

    #endregion
}
