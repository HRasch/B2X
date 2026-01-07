using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using B2X.Catalog.Core.Entities;
using B2X.Catalog.Core.Services;

namespace B2X.Catalog.Infrastructure.Services
{
    /// <summary>
    /// Minimal in-memory implementation of ICatalogShareRuleManager.
    /// Intended as a placeholder until a DB-backed implementation is added.
    /// </summary>
    public class InMemoryCatalogShareRuleManager : ICatalogShareRuleManager
    {
        private readonly ConcurrentDictionary<Guid, EffectiveRuleSet> _cache = new();

        public Task<EffectiveRuleSet> ResolveAsync(Guid tenantId, Guid? customerId = null)
        {
            // Return cached set or empty set
            var key = tenantId;
            if (_cache.TryGetValue(key, out var set))
            {
                return Task.FromResult(set);
            }

            var empty = new EffectiveRuleSet { TenantId = tenantId, CustomerId = customerId };
            return Task.FromResult(empty);
        }

        public Task InvalidateAsync(Guid? tenantId = null)
        {
            if (tenantId.HasValue)
            {
                _cache.TryRemove(tenantId.Value, out _);
            }
            else
            {
                _cache.Clear();
            }

            return Task.CompletedTask;
        }
    }
}
