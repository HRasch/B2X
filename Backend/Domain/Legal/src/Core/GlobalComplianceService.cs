using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using B2X.Shared.Core;

namespace B2X.Legal.Core
{
    /// <summary>
    /// Global Compliance Framework Service
    /// Implements multi-jurisdictional compliance validation and customization
    /// Sprint 2026-24: COMPLIANCE-GLOBAL-001
    /// </summary>
    public class GlobalComplianceService
    {
        private readonly ILogger<GlobalComplianceService> _logger;
        private readonly IComplianceRuleEngine _ruleEngine;
        private readonly IJurisdictionManager _jurisdictionManager;
        private readonly IComplianceValidator _validator;

        public GlobalComplianceService(
            ILogger<GlobalComplianceService> logger,
            IComplianceRuleEngine ruleEngine,
            IJurisdictionManager jurisdictionManager,
            IComplianceValidator validator)
        {
            _logger = logger;
            _ruleEngine = ruleEngine;
            _jurisdictionManager = jurisdictionManager;
            _validator = validator;
        }

        /// <summary>
        /// Validates compliance for a given operation across multiple jurisdictions
        /// </summary>
        public async Task<ComplianceValidationResult> ValidateComplianceAsync(
            ComplianceRequest request,
            IEnumerable<string> jurisdictions)
        {
            _logger.LogInformation("Validating compliance for operation {OperationId} across {JurisdictionCount} jurisdictions",
                request.OperationId, jurisdictions.Count());

            var results = new List<JurisdictionComplianceResult>();

            foreach (var jurisdiction in jurisdictions)
            {
                var jurisdictionRules = await _jurisdictionManager.GetRulesForJurisdictionAsync(jurisdiction);
                var result = await _validator.ValidateAsync(request, jurisdictionRules);
                results.Add(result);
            }

            var overallResult = new ComplianceValidationResult
            {
                OperationId = request.OperationId,
                IsCompliant = results.All(r => r.IsCompliant),
                JurisdictionResults = results,
                ValidationTimestamp = DateTime.UtcNow
            };

            if (!overallResult.IsCompliant)
            {
                _logger.LogWarning("Compliance validation failed for operation {OperationId}", request.OperationId);
            }

            return overallResult;
        }

        /// <summary>
        /// Customizes compliance rules for a specific jurisdiction
        /// </summary>
        public async Task CustomizeJurisdictionRulesAsync(
            string jurisdiction,
            ComplianceCustomization customization)
        {
            _logger.LogInformation("Customizing rules for jurisdiction {Jurisdiction}", jurisdiction);

            await _jurisdictionManager.UpdateRulesForJurisdictionAsync(jurisdiction, customization);
        }

        /// <summary>
        /// Gets supported jurisdictions
        /// </summary>
        public async Task<IEnumerable<JurisdictionInfo>> GetSupportedJurisdictionsAsync()
        {
            return await _jurisdictionManager.GetSupportedJurisdictionsAsync();
        }
    }

    /// <summary>
    /// Compliance rule engine interface
    /// </summary>
    public interface IComplianceRuleEngine
    {
        Task<IEnumerable<ComplianceRule>> GetRulesForOperationAsync(string operationType);
    }

    /// <summary>
    /// Jurisdiction manager interface
    /// </summary>
    public interface IJurisdictionManager
    {
        Task<IEnumerable<ComplianceRule>> GetRulesForJurisdictionAsync(string jurisdiction);
        Task UpdateRulesForJurisdictionAsync(string jurisdiction, ComplianceCustomization customization);
        Task<IEnumerable<JurisdictionInfo>> GetSupportedJurisdictionsAsync();
    }

    /// <summary>
    /// Compliance validator interface
    /// </summary>
    public interface IComplianceValidator
    {
        Task<JurisdictionComplianceResult> ValidateAsync(ComplianceRequest request, IEnumerable<ComplianceRule> rules);
    }

    /// <summary>
    /// Compliance request
    /// </summary>
    public class ComplianceRequest
    {
        public string OperationId { get; set; }
        public string OperationType { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
        public string UserId { get; set; }
        public DateTime Timestamp { get; set; }
    }

    /// <summary>
    /// Compliance validation result
    /// </summary>
    public class ComplianceValidationResult
    {
        public string OperationId { get; set; }
        public bool IsCompliant { get; set; }
        public IEnumerable<JurisdictionComplianceResult> JurisdictionResults { get; set; }
        public DateTime ValidationTimestamp { get; set; }
    }

    /// <summary>
    /// Jurisdiction compliance result
    /// </summary>
    public class JurisdictionComplianceResult
    {
        public string Jurisdiction { get; set; }
        public bool IsCompliant { get; set; }
        public IEnumerable<string> Violations { get; set; }
        public IEnumerable<string> Warnings { get; set; }
    }

    /// <summary>
    /// Compliance rule
    /// </summary>
    public class ComplianceRule
    {
        public string RuleId { get; set; }
        public string Jurisdiction { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public RuleSeverity Severity { get; set; }
        public Dictionary<string, object> Conditions { get; set; }
    }

    /// <summary>
    /// Compliance customization
    /// </summary>
    public class ComplianceCustomization
    {
        public string Jurisdiction { get; set; }
        public IEnumerable<ComplianceRule> CustomRules { get; set; }
        public Dictionary<string, object> Overrides { get; set; }
    }

    /// <summary>
    /// Jurisdiction info
    /// </summary>
    public class JurisdictionInfo
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Region { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Rule severity levels
    /// </summary>
    public enum RuleSeverity
    {
        Low,
        Medium,
        High,
        Critical
    }
}