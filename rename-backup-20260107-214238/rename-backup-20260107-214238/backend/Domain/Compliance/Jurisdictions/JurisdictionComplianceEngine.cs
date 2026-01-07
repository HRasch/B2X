// B2X.Compliance.Jurisdictions
// Emerging Market Jurisdiction Expansion
// DocID: SPR-027-COMPLIANCE-JURISDICTIONS

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace B2X.Compliance.Jurisdictions
{
    /// <summary>
    /// Compliance Engine for Emerging Market Jurisdictions
    /// Supports 15+ jurisdictions with automated regulatory updates
    /// </summary>
    public class JurisdictionComplianceEngine
    {
        private readonly IJurisdictionRepository _jurisdictionRepo;
        private readonly IRegulatoryUpdateService _regulatoryService;
        private readonly Dictionary<string, IJurisdictionHandler> _handlers;

        public JurisdictionComplianceEngine(
            IJurisdictionRepository jurisdictionRepo,
            IRegulatoryUpdateService regulatoryService)
        {
            _jurisdictionRepo = jurisdictionRepo;
            _regulatoryService = regulatoryService;
            _handlers = new Dictionary<string, IJurisdictionHandler>();
        }

        /// <summary>
        /// Register jurisdiction handler
        /// </summary>
        public void RegisterHandler(string jurisdictionCode, IJurisdictionHandler handler)
        {
            _handlers[jurisdictionCode] = handler;
        }

        /// <summary>
        /// Assess compliance for transaction in specific jurisdiction
        /// </summary>
        public async Task<ComplianceResult> AssessComplianceAsync(
            string jurisdictionCode,
            ComplianceRequest request)
        {
            if (!_handlers.TryGetValue(jurisdictionCode, out var handler))
            {
                throw new NotSupportedException($"Jurisdiction {jurisdictionCode} not supported");
            }

            var result = await handler.AssessComplianceAsync(request);

            // Check for regulatory updates
            var updates = await _regulatoryService.GetUpdatesAsync(jurisdictionCode);
            if (updates.Any())
            {
                result.RegulatoryNotes = updates;
            }

            return result;
        }

        /// <summary>
        /// Get supported jurisdictions (15+ emerging markets)
        /// </summary>
        public IEnumerable<string> GetSupportedJurisdictions()
        {
            return new[]
            {
                "BR", "MX", "AR", "CO", "CL", "PE", "VE", "EC", // Latin America
                "ZA", "NG", "KE", "GH", "TZ", "UG", "RW", // Africa
                "IN", "ID", "MY", "TH", "VN", "PH", // Asia Pacific
                "TR", "PL", "CZ", "HU", "RO" // Emerging Europe
            };
        }
    }

    public interface IJurisdictionHandler
    {
        Task<ComplianceResult> AssessComplianceAsync(ComplianceRequest request);
    }

    public interface IJurisdictionRepository
    {
        Task<JurisdictionData> GetJurisdictionDataAsync(string code);
    }

    public interface IRegulatoryUpdateService
    {
        Task<IEnumerable<string>> GetUpdatesAsync(string jurisdictionCode);
    }

    public class ComplianceRequest
    {
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string EntityType { get; set; }
        public Dictionary<string, object> AdditionalData { get; set; }
    }

    public class ComplianceResult
    {
        public bool IsCompliant { get; set; }
        public string RiskLevel { get; set; }
        public IEnumerable<string> Violations { get; set; }
        public IEnumerable<string> RegulatoryNotes { get; set; }
        public decimal RiskScore { get; set; }
    }

    public class JurisdictionData
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public Dictionary<string, object> Regulations { get; set; }
    }
}