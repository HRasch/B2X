using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using B2X.CMS.Application.Pages;
using B2X.CMS.Core.Domain.Pages;
using Microsoft.EntityFrameworkCore;

namespace B2X.CMS.Infrastructure.Repositories
{
    /// <summary>
    /// PostgreSQL template repository implementation (ADR-030 Phase 3)
    /// Persists template overrides to database with proper tenant isolation
    /// </summary>
    public class PostgreSqlTemplateRepository : ITemplateRepository
    {
        private readonly CmsDbContext _context;

        public PostgreSqlTemplateRepository(CmsDbContext context)
        {
            _context = context;
        }

        public async Task<PageDefinition?> GetTenantOverrideAsync(
            string tenantId,
            string templateKey,
            CancellationToken cancellationToken = default)
        {
            // Parse tenantId from string to Guid
            if (!Guid.TryParse(tenantId, out var tenantGuid))
                return null;

            var templateOverride = await _context.TemplateOverrides
                .FirstOrDefaultAsync(t =>
                    t.TenantId == tenantGuid &&
                    t.TemplateKey == templateKey &&
                    t.IsPublished,
                    cancellationToken).ConfigureAwait(false);

            if (templateOverride == null)
                return null;

            // Get metadata separately
            var metadata = await _context.TemplateOverrideMetadata
                .FirstOrDefaultAsync(m => m.OverrideId == templateOverride.Id, cancellationToken).ConfigureAwait(false);

            return new PageDefinition
            {
                TenantId = tenantId,
                PageType = "template",
                PagePath = $"/templates/{templateKey}",
                PageTitle = templateKey,
                PageDescription = $"Tenant override for {templateKey}",
                MetaKeywords = "",
                TemplateLayout = templateOverride.TemplateContent,
                IsTemplateOverride = true,
                BaseTemplateKey = templateKey,
                Version = templateOverride.Version,
                CreatedAt = templateOverride.CreatedAt,
                UpdatedAt = templateOverride.UpdatedAt ?? templateOverride.CreatedAt,
                IsPublished = templateOverride.IsPublished,
                PublishedAt = templateOverride.PublishedAt ?? DateTime.MinValue,
                OverrideSections = templateOverride.OverrideSections,
                OverrideMetadata = metadata
            };
        }

        public async Task<PageDefinition?> GetBaseTemplateAsync(
            string templateKey,
            CancellationToken cancellationToken = default)
        {
            // Base templates are system defaults, not stored in overrides
            // This would typically come from a separate base template store
            // For now, return null to indicate base template not found
            return null;
        }

        public async Task<List<PageDefinition>> GetTenantOverridesAsync(
            string tenantId,
            CancellationToken cancellationToken = default)
        {
            // Parse tenantId from string to Guid
            if (!Guid.TryParse(tenantId, out var tenantGuid))
                return new List<PageDefinition>();

            var overrides = await _context.TemplateOverrides
                .Where(t => t.TenantId == tenantGuid && t.IsPublished)
                .ToListAsync(cancellationToken).ConfigureAwait(false);

            var result = new List<PageDefinition>();
            foreach (var templateOverride in overrides)
            {
                // Get metadata separately
                var metadata = await _context.TemplateOverrideMetadata
                    .FirstOrDefaultAsync(m => m.OverrideId == templateOverride.Id, cancellationToken).ConfigureAwait(false);

                result.Add(new PageDefinition
                {
                    TenantId = tenantId,
                    PageType = "template",
                    PagePath = $"/templates/{templateOverride.TemplateKey}",
                    PageTitle = templateOverride.TemplateKey,
                    PageDescription = $"Tenant override for {templateOverride.TemplateKey}",
                    MetaKeywords = "",
                    TemplateLayout = templateOverride.TemplateContent,
                    IsTemplateOverride = true,
                    BaseTemplateKey = templateOverride.TemplateKey,
                    Version = templateOverride.Version,
                    CreatedAt = templateOverride.CreatedAt,
                    UpdatedAt = templateOverride.UpdatedAt ?? templateOverride.CreatedAt,
                    IsPublished = templateOverride.IsPublished,
                    PublishedAt = templateOverride.PublishedAt ?? templateOverride.CreatedAt,
                    OverrideMetadata = metadata != null ? new TemplateOverrideMetadata
                    {
                        OverrideId = metadata.OverrideId,
                        Version = metadata.Version,
                        ValidationStatus = metadata.ValidationStatus,
                        IsValidated = metadata.IsValidated,
                        ValidationResults = metadata.ValidationResults,
                        PreviewErrors = metadata.PreviewErrors,
                        AiSuggestions = metadata.AiSuggestions,
                        IsLive = templateOverride.IsPublished,
                        PublishedAt = templateOverride.PublishedAt,
                        CreatedBy = metadata.CreatedBy,
                        LastModifiedBy = metadata.LastModifiedBy,
                        CreatedAt = metadata.CreatedAt,
                        LastModifiedAt = metadata.LastModifiedAt,
                        ChangeDescription = metadata.ChangeDescription,
                        SecurityMetadata = metadata.SecurityMetadata,
                        PerformanceMetrics = metadata.PerformanceMetrics
                    } : null
                });
            }

            return result;
        }

        public async Task<PageDefinition> SaveOverrideAsync(
            PageDefinition pageDefinition,
            CancellationToken cancellationToken = default)
        {
            // Parse tenantId from string to Guid
            if (!Guid.TryParse(pageDefinition.TenantId, out var tenantGuid))
                throw new ArgumentException("Invalid tenant ID format", nameof(pageDefinition));

            var existingOverride = await _context.TemplateOverrides
                .FirstOrDefaultAsync(t =>
                    t.TenantId == tenantGuid &&
                    t.TemplateKey == pageDefinition.BaseTemplateKey,
                    cancellationToken).ConfigureAwait(false);

            if (existingOverride != null)
            {
                // Update existing override
                existingOverride.TemplateContent = pageDefinition.TemplateLayout;
                existingOverride.OverrideSections = pageDefinition.OverrideSections;
                existingOverride.Version++;
                existingOverride.UpdatedAt = DateTime.UtcNow;
                existingOverride.UpdatedBy = "system"; // TODO: Get from current user context

                if (pageDefinition.OverrideMetadata != null)
                {
                    var existingMetadata = await _context.TemplateOverrideMetadata
                        .FirstOrDefaultAsync(m => m.OverrideId == existingOverride.Id, cancellationToken).ConfigureAwait(false);

                    if (existingMetadata != null)
                    {
                        UpdateMetadataFromPageDefinition(existingMetadata, pageDefinition.OverrideMetadata);
                    }
                    else
                    {
                        var newMetadata = new TemplateOverrideMetadata
                        {
                            OverrideId = existingOverride.Id
                        };
                        UpdateMetadataFromPageDefinition(newMetadata, pageDefinition.OverrideMetadata);
                        _context.TemplateOverrideMetadata.Add(newMetadata);
                    }
                }

                _context.TemplateOverrides.Update(existingOverride);
            }
            else
            {
                // Create new override
                var templateOverride = new TemplateOverride(
                    tenantGuid,
                    pageDefinition.BaseTemplateKey ?? "unknown",
                    null, // baseTemplateKey
                    pageDefinition.TemplateLayout,
                    pageDefinition.OverrideSections,
                    "system" // TODO: Get from current user context
                );

                _context.TemplateOverrides.Add(templateOverride);
                await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false); // Save to get the ID

                if (pageDefinition.OverrideMetadata != null)
                {
                    var metadata = new TemplateOverrideMetadata
                    {
                        OverrideId = templateOverride.Id
                    };
                    UpdateMetadataFromPageDefinition(metadata, pageDefinition.OverrideMetadata);
                    _context.TemplateOverrideMetadata.Add(metadata);
                }
            }

            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            // Return updated page definition
            return pageDefinition;
        }


        public async Task PublishOverrideAsync(
            string tenantId,
            string templateKey,
            CancellationToken cancellationToken = default)
        {
            // Parse tenantId from string to Guid
            if (!Guid.TryParse(tenantId, out var tenantGuid))
                throw new ArgumentException("Invalid tenant ID format", nameof(tenantId));

            var templateOverride = await _context.TemplateOverrides
                .FirstOrDefaultAsync(t =>
                    t.TenantId == tenantGuid &&
                    t.TemplateKey == templateKey,
                    cancellationToken).ConfigureAwait(false);

            if (templateOverride == null)
                throw new InvalidOperationException($"Template override not found for tenant {tenantId} and key {templateKey}");

            templateOverride.IsPublished = true;
            templateOverride.PublishedAt = DateTime.UtcNow;
            templateOverride.PublishedBy = "system"; // TODO: Get from current user context

            // Update metadata separately
            var metadata = await _context.TemplateOverrideMetadata
                .FirstOrDefaultAsync(m => m.OverrideId == templateOverride.Id, cancellationToken).ConfigureAwait(false);

            if (metadata != null)
            {
                metadata.IsLive = true;
                metadata.PublishedAt = templateOverride.PublishedAt;
                metadata.LastModifiedAt = DateTime.UtcNow;
                metadata.LastModifiedBy = "system";
            }

            _context.TemplateOverrides.Update(templateOverride);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        private void UpdateMetadataFromPageDefinition(
            TemplateOverrideMetadata metadata,
            TemplateOverrideMetadata pageMetadata)
        {
            metadata.ValidationStatus = pageMetadata.ValidationStatus;
            metadata.IsValidated = pageMetadata.IsValidated;
            metadata.ValidationResults = pageMetadata.ValidationResults;
            metadata.PreviewErrors = pageMetadata.PreviewErrors;
            metadata.AiSuggestions = pageMetadata.AiSuggestions;
            metadata.LastModifiedAt = DateTime.UtcNow;
            metadata.LastModifiedBy = "system"; // TODO: Get from current user context
            metadata.ChangeDescription = pageMetadata.ChangeDescription;
            metadata.SecurityMetadata = pageMetadata.SecurityMetadata;
            metadata.PerformanceMetrics = pageMetadata.PerformanceMetrics;
        }

        public async Task<PageDefinition?> GetResolvedTemplateAsync(
            string tenantId,
            string templateKey,
            CancellationToken cancellationToken = default)
        {
            // Get tenant override first
            var tenantOverride = await GetTenantOverrideAsync(tenantId, templateKey, cancellationToken).ConfigureAwait(false);
            if (tenantOverride != null)
            {
                return tenantOverride;
            }

            // Fall back to base template
            return await GetBaseTemplateAsync(templateKey, cancellationToken).ConfigureAwait(false);
        }


        public async Task<List<PageDefinition>> GetOverrideHistoryAsync(
            string tenantId,
            string templateKey,
            int limit = 10,
            CancellationToken cancellationToken = default)
        {
            // For now, return current override as history (versioning not fully implemented)
            var currentOverride = await GetTenantOverrideAsync(tenantId, templateKey, cancellationToken).ConfigureAwait(false);
            return currentOverride != null ? new List<PageDefinition> { currentOverride } : new List<PageDefinition>();
        }

        public async Task RevertToVersionAsync(
            string tenantId,
            string templateKey,
            int version,
            CancellationToken cancellationToken = default)
        {
            // Parse tenantId from string to Guid
            if (!Guid.TryParse(tenantId, out var tenantGuid))
                return;

            // For now, just unpublish current override (versioning not fully implemented)
            var templateOverride = await _context.TemplateOverrides
                .FirstOrDefaultAsync(t =>
                    t.TenantId == tenantGuid &&
                    t.TemplateKey == templateKey,
                    cancellationToken).ConfigureAwait(false);

            if (templateOverride != null)
            {
                templateOverride.IsPublished = false;
                templateOverride.UpdatedAt = DateTime.UtcNow;
                templateOverride.UpdatedBy = "system";

                // Update metadata separately
                var metadata = await _context.TemplateOverrideMetadata
                    .FirstOrDefaultAsync(m => m.OverrideId == templateOverride.Id, cancellationToken).ConfigureAwait(false);

                if (metadata != null)
                {
                    metadata.IsLive = false;
                    metadata.LastModifiedAt = DateTime.UtcNow;
                    metadata.LastModifiedBy = "system";
                }

                await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task DeleteOverrideAsync(
            string tenantId,
            string templateKey,
            CancellationToken cancellationToken = default)
        {
            // Parse tenantId from string to Guid
            if (!Guid.TryParse(tenantId, out var tenantGuid))
                return;

            var templateOverride = await _context.TemplateOverrides
                .FirstOrDefaultAsync(t =>
                    t.TenantId == tenantGuid &&
                    t.TemplateKey == templateKey,
                    cancellationToken).ConfigureAwait(false);

            if (templateOverride != null)
            {
                _context.TemplateOverrides.Remove(templateOverride);
                await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
