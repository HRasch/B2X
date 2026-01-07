using System;
using System.Collections.Generic;
using B2X.Shared.Core.Entities;
using B2X.Shared.Kernel;

namespace B2X.SmartDataIntegration.Models
{
    /// <summary>
    /// Represents a data mapping configuration between source and target systems
    /// </summary>
    public class DataMappingConfiguration : BaseEntity
    {
        /// <summary>
        /// Human-readable name for the mapping
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of what this mapping does
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Source system identifier (e.g., "ERP", "Catalog")
        /// </summary>
        public string SourceSystem { get; set; } = string.Empty;

        /// <summary>
        /// Target system identifier
        /// </summary>
        public string TargetSystem { get; set; } = string.Empty;

        /// <summary>
        /// Version of the mapping configuration
        /// </summary>
        public int Version { get; set; } = 1;

        /// <summary>
        /// Whether this mapping is active
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Collection of mapping rules
        /// </summary>
        public ICollection<MappingRule> MappingRules { get; set; } = new List<MappingRule>();

        /// <summary>
        /// AI confidence score for the overall mapping (0-100)
        /// </summary>
        public double AiConfidenceScore { get; set; }

        /// <summary>
        /// Last time the mapping was validated
        /// </summary>
        public DateTime? LastValidatedAt { get; set; }
    }
}
