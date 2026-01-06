using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using B2Connect.CMS.Core.Domain.Pages;
using Wolverine;

namespace B2Connect.CMS.Application.Pages
{
    /// <summary>
    /// Query to get template overrides for a tenant (ADR-030)
    /// </summary>
    public class GetTemplateOverridesQuery
    {
        public string TenantId { get; init; } = string.Empty;
        public string? TemplateKey { get; init; } // Optional filter
    }

    /// <summary>
    /// Handler for getting template overrides
    /// </summary>
    public class GetTemplateOverridesQueryHandler
    {
        private readonly ITemplateRepository _templateRepository;

        public GetTemplateOverridesQueryHandler(ITemplateRepository templateRepository)
        {
            _templateRepository = templateRepository;
        }

        public async Task<List<TemplateOverrideDto>> Handle(
            GetTemplateOverridesQuery query,
            CancellationToken cancellationToken)
        {
            var overrides = await _templateRepository.GetTenantOverridesAsync(
                query.TenantId, cancellationToken);

            if (!string.IsNullOrEmpty(query.TemplateKey))
            {
                overrides = overrides.Where(o => o.BaseTemplateKey == query.TemplateKey).ToList();
            }

            return overrides.Select(MapToDto).ToList();
        }

        private TemplateOverrideDto MapToDto(PageDefinition pageDefinition)
        {
            return new TemplateOverrideDto
            {
                Id = Guid.NewGuid(), // PageDefinition doesn't have ID, generate one
                TenantId = Guid.Parse(pageDefinition.TenantId),
                TemplateKey = pageDefinition.BaseTemplateKey ?? string.Empty,
                BaseTemplateKey = pageDefinition.BaseTemplateKey,
                OverrideSections = new Dictionary<string, string>(), // PageDefinition doesn't have sections
                Metadata = pageDefinition.OverrideMetadata != null ? new TemplateOverrideMetadataDto
                {
                    CreatedAt = pageDefinition.OverrideMetadata.CreatedAt,
                    CreatedBy = pageDefinition.OverrideMetadata.CreatedBy ?? string.Empty,
                    LastModified = pageDefinition.OverrideMetadata.LastModifiedAt,
                    OverrideVersion = pageDefinition.OverrideMetadata.Version,
                    ValidationResults = pageDefinition.OverrideMetadata.ValidationResults,
                    IsValidated = pageDefinition.OverrideMetadata.IsValidated,
                    ValidationStatus = pageDefinition.OverrideMetadata.ValidationStatus.ToString()
                } : new TemplateOverrideMetadataDto(),
                CreatedAt = pageDefinition.CreatedAt,
                UpdatedAt = pageDefinition.UpdatedAt
            };
        }
    }

    /// <summary>
    /// DTO for template override
    /// </summary>
    public class TemplateOverrideDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string TemplateKey { get; set; } = null!;
        public string? BaseTemplateKey { get; set; }
        public Dictionary<string, string> OverrideSections { get; set; } = new();
        public TemplateOverrideMetadataDto Metadata { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO for template override metadata
    /// </summary>
    public class TemplateOverrideMetadataDto
    {
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime LastModified { get; set; }
        public int OverrideVersion { get; set; }
        public List<string> ValidationResults { get; set; } = new();
        public bool IsValidated { get; set; }
        public string? ValidationStatus { get; set; }
    }
}
