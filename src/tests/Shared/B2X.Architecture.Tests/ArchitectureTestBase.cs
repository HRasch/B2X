// -----------------------------------------------------------------------------
// B2X Architecture Tests - Shared Test Base
// ADR-021: ArchUnitNET for Automated Architecture Testing
// -----------------------------------------------------------------------------

using ArchUnitNET;
using ArchUnitDomain = ArchUnitNET.Domain;

namespace B2X.Architecture.Tests;

/// <summary>
/// Base class providing shared architecture loading for all architecture tests.
/// Loading the architecture once and reusing it across tests improves performance.
/// </summary>
public abstract class ArchitectureTestBase
{
    /// <summary>
    /// The loaded architecture containing all B2X domain assemblies.
    /// Loaded once per test collection for performance.
    /// </summary>
    protected static readonly ArchUnitDomain.Architecture Architecture = new ArchLoader()
        .LoadAssemblies(
            // Domain assemblies - using marker types from each domain
            typeof(Catalog.Core.Entities.TaxRate).Assembly,                     // Catalog
            typeof(CMS.Core.Domain.Pages.PageDefinition).Assembly,              // CMS
            typeof(B2X.AuthService.Data.AppUser).Assembly,                // Identity (AuthService namespace)
            typeof(LocalizationService.Models.LocalizedString).Assembly,        // Localization
            typeof(Domain.Search.Models.ProductDocument).Assembly,              // Search
            typeof(B2X.Types.Domain.Entity).Assembly)                     // Shared Kernel
        .Build();

    /// <summary>
    /// Bounded context namespaces for isolation testing.
    /// </summary>
    protected static class BoundedContexts
    {
        public const string Catalog = "B2X.Catalog";
        public const string CMS = "B2X.CMS";
        public const string Identity = "B2X.AuthService";
        public const string Localization = "B2X.LocalizationService";
        public const string Search = "B2X.Domain.Search";
    }

    /// <summary>
    /// Layer namespaces for clean architecture testing.
    /// </summary>
    protected static class Layers
    {
        public const string Core = ".Core";
        public const string Application = ".Application";
        public const string Infrastructure = ".Infrastructure";
        public const string Handlers = ".Handlers";
        public const string Controllers = ".Controllers";
        public const string Validators = ".Validators";
        public const string Events = ".Events";
        public const string Services = ".Services";
    }
}
