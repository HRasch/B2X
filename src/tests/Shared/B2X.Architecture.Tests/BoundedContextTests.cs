// -----------------------------------------------------------------------------
// B2X Architecture Tests - Bounded Context Isolation Rules
// ADR-021: ArchUnitNET for Automated Architecture Testing
// ADR-001: Event-Driven Architecture with Bounded Contexts
// -----------------------------------------------------------------------------

using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.xUnitV3;
using Xunit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace B2X.ArchitectureTests;

/// <summary>
/// Tests enforcing Bounded Context isolation.
/// Each bounded context should be independent and communicate only through events/messages.
/// </summary>
[Collection("Architecture")]
public class BoundedContextTests : ArchitectureTestBase
{
    [Fact]
    public void Catalog_Should_Not_Depend_On_CMS()
    {
        // Arrange & Act
        ArchRuleDefinition.Types()
            .That().ResideInNamespaceMatching($@"{BoundedContexts.Catalog}\..*")
            .Should().NotDependOnAny(
                ArchRuleDefinition.Types().That().ResideInNamespaceMatching($@"{BoundedContexts.CMS}\..*"))
            .Because("Bounded contexts must be isolated - Catalog cannot directly depend on CMS (ADR-001)")
            .Check(B2XArchitecture);
    }

    [Fact]
    public void Catalog_Should_Not_Depend_On_Identity()
    {
        // Arrange & Act
        ArchRuleDefinition.Types()
            .That().ResideInNamespaceMatching($@"{BoundedContexts.Catalog}\..*")
            .Should().NotDependOnAny(
                ArchRuleDefinition.Types().That().ResideInNamespaceMatching($@"{BoundedContexts.Identity}\..*"))
            .Because("Bounded contexts must be isolated - Catalog cannot directly depend on Identity (ADR-001)")
            .Check(B2XArchitecture);
    }

    [Fact]
    public void CMS_Should_Not_Depend_On_Catalog()
    {
        // Arrange & Act
        ArchRuleDefinition.Types()
            .That().ResideInNamespaceMatching($@"{BoundedContexts.CMS}\..*")
            .Should().NotDependOnAny(
                ArchRuleDefinition.Types().That().ResideInNamespaceMatching($@"{BoundedContexts.Catalog}\..*"))
            .Because("Bounded contexts must be isolated - CMS cannot directly depend on Catalog (ADR-001)")
            .Check(B2XArchitecture);
    }

    [Fact]
    public void CMS_Should_Not_Depend_On_Identity()
    {
        // Arrange & Act
        ArchRuleDefinition.Types()
            .That().ResideInNamespaceMatching($@"{BoundedContexts.CMS}\..*")
            .Should().NotDependOnAny(
                ArchRuleDefinition.Types().That().ResideInNamespaceMatching($@"{BoundedContexts.Identity}\..*"))
            .Because("Bounded contexts must be isolated - CMS cannot directly depend on Identity (ADR-001)")
            .Check(B2XArchitecture);
    }

    [Fact]
    public void Identity_Should_Not_Depend_On_Catalog()
    {
        // Arrange & Act
        ArchRuleDefinition.Types()
            .That().ResideInNamespaceMatching($@"{BoundedContexts.Identity}\..*")
            .Should().NotDependOnAny(
                ArchRuleDefinition.Types().That().ResideInNamespaceMatching($@"{BoundedContexts.Catalog}\..*"))
            .Because("Bounded contexts must be isolated - Identity cannot directly depend on Catalog (ADR-001)")
            .Check(B2XArchitecture);
    }

    [Fact]
    public void Identity_Should_Not_Depend_On_CMS()
    {
        // Arrange & Act
        ArchRuleDefinition.Types()
            .That().ResideInNamespaceMatching($@"{BoundedContexts.Identity}\..*")
            .Should().NotDependOnAny(
                ArchRuleDefinition.Types().That().ResideInNamespaceMatching($@"{BoundedContexts.CMS}\..*"))
            .Because("Bounded contexts must be isolated - Identity cannot directly depend on CMS (ADR-001)")
            .Check(B2XArchitecture);
    }

    [Fact]
    public void Localization_Should_Not_Depend_On_Catalog()
    {
        // Arrange & Act
        ArchRuleDefinition.Types()
            .That().ResideInNamespaceMatching($@"{BoundedContexts.Localization}\..*")
            .Should().NotDependOnAny(
                ArchRuleDefinition.Types().That().ResideInNamespaceMatching($@"{BoundedContexts.Catalog}\..*"))
            .Because("Bounded contexts must be isolated - Localization cannot directly depend on Catalog (ADR-001)")
            .Check(B2XArchitecture);
    }

    [Fact]
    public void Search_Should_Not_Depend_On_Identity()
    {
        // Arrange & Act
        ArchRuleDefinition.Types()
            .That().ResideInNamespaceMatching($@"{BoundedContexts.Search}\..*")
            .Should().NotDependOnAny(
                ArchRuleDefinition.Types().That().ResideInNamespaceMatching($@"{BoundedContexts.Identity}\..*"))
            .Because("Bounded contexts must be isolated - Search cannot directly depend on Identity (ADR-001)")
            .WithoutRequiringPositiveResults() // Search namespace may have no types with sub-namespaces
            .Check(B2XArchitecture);
    }

    // Note: Shared.Kernel and B2X.Types are allowed as shared dependencies
    // They contain value objects and base classes used across all bounded contexts
}
