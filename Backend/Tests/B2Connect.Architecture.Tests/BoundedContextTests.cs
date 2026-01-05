// -----------------------------------------------------------------------------
// B2Connect Architecture Tests - Bounded Context Isolation Rules
// ADR-021: ArchUnitNET for Automated Architecture Testing
// ADR-001: Event-Driven Architecture with Bounded Contexts
// -----------------------------------------------------------------------------

using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;
using Xunit;

namespace B2Connect.Architecture.Tests;

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
        var rule = ArchRuleDefinition.Types()
            .That().ResideInNamespaceMatching($@"{BoundedContexts.Catalog}\..*")
            .Should.NotDependOnAny(
                ArchRuleDefinition.Types().That().ResideInNamespaceMatching($@"{BoundedContexts.CMS}\..*"))
            .Because("Bounded contexts must be isolated - Catalog cannot directly depend on CMS (ADR-001)");

        // Assert
        rule.Check(Architecture);
    }

    [Fact]
    public void Catalog_Should_Not_Depend_On_Identity()
    {
        // Arrange & Act
        var rule = ArchRuleDefinition.Types()
            .That().ResideInNamespaceMatching($@"{BoundedContexts.Catalog}\..*")
            .Should.NotDependOnAny(
                ArchRuleDefinition.Types().That().ResideInNamespaceMatching($@"{BoundedContexts.Identity}\..*"))
            .Because("Bounded contexts must be isolated - Catalog cannot directly depend on Identity (ADR-001)");

        // Assert
        rule.Check(Architecture);
    }

    [Fact]
    public void CMS_Should_Not_Depend_On_Catalog()
    {
        // Arrange & Act
        var rule = ArchRuleDefinition.Types()
            .That().ResideInNamespaceMatching($@"{BoundedContexts.CMS}\..*")
            .Should.NotDependOnAny(
                ArchRuleDefinition.Types().That().ResideInNamespaceMatching($@"{BoundedContexts.Catalog}\..*"))
            .Because("Bounded contexts must be isolated - CMS cannot directly depend on Catalog (ADR-001)");

        // Assert
        rule.Check(Architecture);
    }

    [Fact]
    public void CMS_Should_Not_Depend_On_Identity()
    {
        // Arrange & Act
        var rule = ArchRuleDefinition.Types()
            .That().ResideInNamespaceMatching($@"{BoundedContexts.CMS}\..*")
            .Should.NotDependOnAny(
                ArchRuleDefinition.Types().That().ResideInNamespaceMatching($@"{BoundedContexts.Identity}\..*"))
            .Because("Bounded contexts must be isolated - CMS cannot directly depend on Identity (ADR-001)");

        // Assert
        rule.Check(Architecture);
    }

    [Fact]
    public void Identity_Should_Not_Depend_On_Catalog()
    {
        // Arrange & Act
        var rule = ArchRuleDefinition.Types()
            .That().ResideInNamespaceMatching($@"{BoundedContexts.Identity}\..*")
            .Should.NotDependOnAny(
                ArchRuleDefinition.Types().That().ResideInNamespaceMatching($@"{BoundedContexts.Catalog}\..*"))
            .Because("Bounded contexts must be isolated - Identity cannot directly depend on Catalog (ADR-001)");

        // Assert
        rule.Check(Architecture);
    }

    [Fact]
    public void Identity_Should_Not_Depend_On_CMS()
    {
        // Arrange & Act
        var rule = ArchRuleDefinition.Types()
            .That().ResideInNamespaceMatching($@"{BoundedContexts.Identity}\..*")
            .Should.NotDependOnAny(
                ArchRuleDefinition.Types().That().ResideInNamespaceMatching($@"{BoundedContexts.CMS}\..*"))
            .Because("Bounded contexts must be isolated - Identity cannot directly depend on CMS (ADR-001)");

        // Assert
        rule.Check(Architecture);
    }

    [Fact]
    public void Localization_Should_Not_Depend_On_Catalog()
    {
        // Arrange & Act
        var rule = ArchRuleDefinition.Types()
            .That().ResideInNamespaceMatching($@"{BoundedContexts.Localization}\..*")
            .Should.NotDependOnAny(
                ArchRuleDefinition.Types().That().ResideInNamespaceMatching($@"{BoundedContexts.Catalog}\..*"))
            .Because("Bounded contexts must be isolated - Localization cannot directly depend on Catalog (ADR-001)");

        // Assert
        rule.Check(Architecture);
    }

    [Fact]
    public void Search_Should_Not_Depend_On_Identity()
    {
        // Arrange & Act
        var rule = ArchRuleDefinition.Types()
            .That().ResideInNamespaceMatching($@"{BoundedContexts.Search}\..*")
            .Should.NotDependOnAny(
                ArchRuleDefinition.Types().That().ResideInNamespaceMatching($@"{BoundedContexts.Identity}\..*"))
            .Because("Bounded contexts must be isolated - Search cannot directly depend on Identity (ADR-001)");

        // Assert
        rule.Check(Architecture);
    }

    // Note: Shared.Kernel and B2Connect.Types are allowed as shared dependencies
    // They contain value objects and base classes used across all bounded contexts
}
