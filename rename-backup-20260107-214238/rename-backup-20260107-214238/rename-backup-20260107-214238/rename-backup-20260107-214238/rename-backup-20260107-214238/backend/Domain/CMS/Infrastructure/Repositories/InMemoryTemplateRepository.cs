using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using B2Connect.CMS.Application.Pages;
using B2Connect.CMS.Core.Domain.Pages;

namespace B2Connect.CMS.Infrastructure.Repositories
{
    /// <summary>
    /// In-memory template repository implementation (ADR-030)
    /// TODO: Replace with database implementation
    /// </summary>
    public class InMemoryTemplateRepository : ITemplateRepository
    {
        private readonly ConcurrentDictionary<string, TemplateOverride> _overrides = new(StringComparer.Ordinal);
        private readonly ConcurrentDictionary<string, List<TemplateOverride>> _history = new(StringComparer.Ordinal);

        public Task<PageDefinition?> GetTenantOverrideAsync(
            string tenantId,
            string templateKey,
            CancellationToken cancellationToken = default)
        {
            var key = $"{tenantId}:{templateKey}";
            if (_overrides.TryGetValue(key, out var templateOverride))
            {
                return Task.FromResult<PageDefinition?>(new PageDefinition
                {
                    TenantId = tenantId,
                    PageType = "template",
                    PagePath = $"/templates/{templateKey}",
                    PageTitle = templateKey,
                    PageDescription = $"Template override for {templateKey}",
                    MetaKeywords = "",
                    TemplateLayout = templateOverride.TemplateContent,
                    IsTemplateOverride = true,
                    BaseTemplateKey = templateKey
                });
            }
            return Task.FromResult<PageDefinition?>(null);
        }

        public Task<PageDefinition?> GetBaseTemplateAsync(
            string templateKey,
            CancellationToken cancellationToken = default)
        {
            // Base templates are system defaults, not stored in overrides
            return Task.FromResult<PageDefinition?>(null);
        }

        public Task<PageDefinition?> GetResolvedTemplateAsync(
            string templateKey,
            string tenantId,
            CancellationToken cancellationToken = default)
        {
            var key = $"{tenantId}:{templateKey}";
            if (_overrides.TryGetValue(key, out var templateOverride) && templateOverride.IsPublished)
            {
                return Task.FromResult<PageDefinition?>(new PageDefinition
                {
                    TenantId = tenantId,
                    PageType = "template",
                    PagePath = $"/templates/{templateKey}",
                    PageTitle = templateKey,
                    PageDescription = $"Resolved template for {templateKey}",
                    MetaKeywords = "",
                    TemplateLayout = templateOverride.TemplateContent,
                    IsTemplateOverride = true,
                    BaseTemplateKey = templateKey,
                    IsPublished = true,
                    PublishedAt = templateOverride.PublishedAt ?? DateTime.UtcNow
                });
            }
            return Task.FromResult<PageDefinition?>(null);
        }

        public Task<PageDefinition> SaveOverrideAsync(
            PageDefinition pageDefinition,
            CancellationToken cancellationToken = default)
        {
            var key = $"{pageDefinition.TenantId}:{pageDefinition.BaseTemplateKey ?? "unknown"}";
            var templateOverride = new TemplateOverride
            {
                TenantId = Guid.Parse(pageDefinition.TenantId),
                TemplateKey = pageDefinition.BaseTemplateKey ?? "unknown",
                TemplateContent = pageDefinition.TemplateLayout,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "system",
                Version = pageDefinition.Version
            };

            _overrides[key] = templateOverride;

            // Track in history
            var historyKey = $"{pageDefinition.TenantId}:{pageDefinition.BaseTemplateKey ?? "unknown"}";
            _history.AddOrUpdate(historyKey,
                new List<TemplateOverride> { templateOverride },
                (_, versions) =>
                {
                    versions.Add(templateOverride);
                    return versions;
                });

            return Task.FromResult(pageDefinition);
        }

        public Task PublishOverrideAsync(
            string tenantId,
            string templateKey,
            CancellationToken cancellationToken = default)
        {
            var key = $"{tenantId}:{templateKey}";
            if (_overrides.TryGetValue(key, out var templateOverride))
            {
                templateOverride.Publish("system");
            }
            return Task.CompletedTask;
        }

        public Task<List<PageDefinition>> GetOverrideHistoryAsync(
            string tenantId,
            string templateKey,
            int maxVersions = 10,
            CancellationToken cancellationToken = default)
        {
            var key = $"{tenantId}:{templateKey}";
            if (_history.TryGetValue(key, out var versions))
            {
                var pageDefinitions = versions
                    .TakeLast(maxVersions)
                    .Select(v => new PageDefinition
                    {
                        TenantId = tenantId,
                        PageType = "template",
                        PagePath = $"/templates/{templateKey}",
                        PageTitle = templateKey,
                        PageDescription = $"Template override version for {templateKey}",
                        MetaKeywords = "",
                        TemplateLayout = v.TemplateContent,
                        IsTemplateOverride = true,
                        BaseTemplateKey = templateKey,
                        Version = v.Version,
                        CreatedAt = v.CreatedAt,
                        UpdatedAt = v.CreatedAt
                    })
                    .ToList();
                return Task.FromResult(pageDefinitions);
            }
            return Task.FromResult(new List<PageDefinition>());
        }

        public Task RevertToVersionAsync(
            string tenantId,
            string templateKey,
            int versionNumber,
            CancellationToken cancellationToken = default)
        {
            var key = $"{tenantId}:{templateKey}";
            var historyKey = $"{tenantId}:{templateKey}";

            if (_history.TryGetValue(historyKey, out var versions))
            {
                var targetVersion = versions.FirstOrDefault(v => v.Version == versionNumber);
                if (targetVersion != null)
                {
                    _overrides[key] = targetVersion;
                }
            }
            return Task.CompletedTask;
        }

        public Task<List<PageDefinition>> GetTenantOverridesAsync(
            string tenantId,
            CancellationToken cancellationToken = default)
        {
            var tenantOverrides = _overrides
                .Where(kvp => kvp.Key.StartsWith($"{tenantId}:", StringComparison.Ordinal))
                .Select(kvp => new PageDefinition
                {
                    TenantId = tenantId,
                    PageType = "template",
                    PagePath = $"/templates/{kvp.Value.TemplateKey}",
                    PageTitle = kvp.Value.TemplateKey,
                    PageDescription = $"Template override for {kvp.Value.TemplateKey}",
                    MetaKeywords = "",
                    TemplateLayout = kvp.Value.TemplateContent,
                    IsTemplateOverride = true,
                    BaseTemplateKey = kvp.Value.TemplateKey,
                    Version = kvp.Value.Version,
                    CreatedAt = kvp.Value.CreatedAt,
                    UpdatedAt = kvp.Value.UpdatedAt ?? kvp.Value.CreatedAt
                })
                .ToList();

            return Task.FromResult(tenantOverrides);
        }

        public Task DeleteOverrideAsync(
            string tenantId,
            string templateKey,
            CancellationToken cancellationToken = default)
        {
            var key = $"{tenantId}:{templateKey}";
            _overrides.TryRemove(key, out _);
            return Task.CompletedTask;
        }
    }
}
