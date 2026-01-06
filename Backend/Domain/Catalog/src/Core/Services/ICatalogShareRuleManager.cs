using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using B2Connect.Catalog.Core.Entities;

namespace B2Connect.Catalog.Core.Services
{
    public interface ICatalogShareRuleManager
    {
        /// <summary>
        /// Resolve effective rules for a tenant/customer context.
        /// Returns a cached EffectiveRuleSet or empty set when none defined.
        /// </summary>
        Task<EffectiveRuleSet> ResolveAsync(Guid tenantId, Guid? customerId = null);

        /// <summary>
        /// Simple in-memory invalidation hook for cache/event-driven updates.
        /// </summary>
        Task InvalidateAsync(Guid? tenantId = null);
    }

    public class EffectiveRuleSet
    {
        public Guid TenantId { get; set; }
        public Guid? CustomerId { get; set; }

        // Raw JSON rule payloads as stored in DB; parsing/compilation into queries
        // will be done by the search integration layer.
        public List<CatalogShareRule> Rules { get; set; } = new List<CatalogShareRule>();
    }
}
