using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using B2X.Shared.Core;

namespace B2X.Shared.Compliance
{
    /// <summary>
    /// Compliance Template Adaptation Framework
    /// Implements adaptive template systems with regional customization
    /// Sprint 2026-23: COMPLIANCE-ADAPT-001
    /// </summary>
    public class ComplianceTemplateAdapter
    {
        private readonly ITemplateRepository _templateRepository;
        private readonly IRegionalCustomizer _regionalCustomizer;
        private readonly IComplianceValidator _complianceValidator;

        public ComplianceTemplateAdapter(
            ITemplateRepository templateRepository,
            IRegionalCustomizer regionalCustomizer,
            IComplianceValidator complianceValidator)
        {
            _templateRepository = templateRepository;
            _regionalCustomizer = regionalCustomizer;
            _complianceValidator = complianceValidator;
        }

        /// <summary>
        /// Adapts compliance templates for specific regions
        /// </summary>
        public async Task<AdaptedTemplate> AdaptTemplateAsync(
            string templateId,
            Region region,
            AdaptationContext context)
        {
            // Retrieve base template
            var baseTemplate = await _templateRepository.GetTemplateAsync(templateId);

            // Apply regional customizations
            var customizedTemplate = await _regionalCustomizer.CustomizeAsync(
                baseTemplate,
                region,
                context);

            // Validate compliance
            var validationResult = await _complianceValidator.ValidateAsync(
                customizedTemplate,
                region);

            if (!validationResult.IsCompliant)
            {
                throw new ComplianceException(
                    $"Template adaptation failed compliance validation: {string.Join(", ", validationResult.Issues)}");
            }

            return customizedTemplate;
        }

        /// <summary>
        /// Validates template compliance across regions
        /// </summary>
        public async Task<ComplianceValidationResult> ValidateComplianceAsync(
            AdaptedTemplate template,
            IEnumerable<Region> regions)
        {
            var results = new List<ComplianceValidationResult>();

            foreach (var region in regions)
            {
                var result = await _complianceValidator.ValidateAsync(template, region);
                results.Add(result);
            }

            return new ComplianceValidationResult
            {
                IsCompliant = results.All(r => r.IsCompliant),
                Issues = results.SelectMany(r => r.Issues).ToList(),
                RegionalResults = results
            };
        }
    }

    public interface ITemplateRepository
    {
        Task<Template> GetTemplateAsync(string templateId);
        Task SaveTemplateAsync(Template template);
    }

    public interface IRegionalCustomizer
    {
        Task<AdaptedTemplate> CustomizeAsync(Template template, Region region, AdaptationContext context);
    }

    public interface IComplianceValidator
    {
        Task<ComplianceValidationResult> ValidateAsync(Template template, Region region);
    }

    public class Template
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public Dictionary<string, object> Metadata { get; set; }
    }

    public class AdaptedTemplate : Template
    {
        public Region TargetRegion { get; set; }
        public List<Customization> AppliedCustomizations { get; set; }
    }

    public class Region
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public Dictionary<string, object> ComplianceRules { get; set; }
    }

    public class AdaptationContext
    {
        public string BusinessUnit { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
    }

    public class ComplianceValidationResult
    {
        public bool IsCompliant { get; set; }
        public List<string> Issues { get; set; }
        public List<ComplianceValidationResult> RegionalResults { get; set; }
    }

    public class Customization
    {
        public string Type { get; set; }
        public string Description { get; set; }
        public DateTime AppliedAt { get; set; }
    }

    public class ComplianceException : Exception
    {
        public ComplianceException(string message) : base(message) { }
    }
}