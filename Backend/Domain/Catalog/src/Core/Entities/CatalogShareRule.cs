using System;
using System.Collections.Generic;

namespace B2Connect.Catalog.Core.Entities
{
    public enum CatalogShareRuleTargetType
    {
        Tenant = 0,
        Customer = 1,
        CustomerGroup = 2
    }

    public class CatalogShareRule
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CatalogShareId { get; set; }
        public CatalogShareRuleTargetType TargetType { get; set; }
        public Guid TargetId { get; set; }
        public int Priority { get; set; } = 0;
        public bool Active { get; set; } = true;
        public bool AllowOverride { get; set; } = false;
        public bool ApplyToAggregations { get; set; } = true;

        // JSON-serialized lists for flexible storage; EF Core value conversions map these to JSON
        public string? IncludedCategorySlugsJson { get; set; }
        public string? ExcludedCategorySlugsJson { get; set; }
        public string? IncludedBrandIdsJson { get; set; }
        public string? ExcludedBrandIdsJson { get; set; }
        public string? IncludedProductIdsJson { get; set; }
        public string? ExcludedProductIdsJson { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
