// -----------------------------------------------------------------------------
// B2X Architecture Tests - Layer Dependency Rules
// ADR-021: ArchUnitNET for Automated Architecture Testing
// ADR-002: Onion/Clean Architecture
// -----------------------------------------------------------------------------

using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;
using Xunit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace B2X.Architecture.Tests;

/// <summary>
/// Tests enforcing Clean/Onion Architecture layer dependencies.
/// Domain Core must not depend on Infrastructure or external frameworks.
/// </summary>
[Collection("Architecture")]
public class LayerDependencyTests : ArchitectureTestBase
{
    [Fact]
    public void Domain_Core_Should_Not_Depend_On_Infrastructure()
    {
        // Arrange & Act
        var rule = ArchRuleDefinition.Types()
            .That().ResideInNamespaceMatching(@".*\.Core\.")
            .Should.NotDependOnAny(
                ArchRuleDefinition.Types().That().ResideInNamespaceMatching(@".*\.Infrastructure\."))
            .Because("Domain Core must be independent of infrastructure concerns (ADR-002)");

        // Assert
        rule.Check(Architecture);
    }

    [Fact]
    public void Domain_Core_Should_Not_Depend_On_EntityFramework()
    {
        // Arrange & Act
        var rule = ArchRuleDefinition.Types()
            .That().ResideInNamespaceMatching(@".*\.Core\.")
            .Should.NotDependOnAny(
                ArchRuleDefinition.Types().That().ResideInNamespaceMatching(@"Microsoft\.EntityFrameworkCore.*"))
            .Because("Domain Core must be persistence-ignorant - no EF Core dependencies allowed");

        // Assert
        rule.Check(Architecture);
    }

    [Fact]
    public void Domain_Core_Should_Not_Depend_On_AspNetCore()
    {
        // Arrange & Act
        var rule = ArchRuleDefinition.Types()
            .That().ResideInNamespaceMatching(@".*\.Core\.")
            .Should.NotDependOnAny(
                ArchRuleDefinition.Types().That().ResideInNamespaceMatching(@"Microsoft\.AspNetCore.*"))
            .Because("Domain Core must not depend on web framework concerns");

        // Assert
        rule.Check(Architecture);
    }

    [Fact]
    public void Handlers_Should_Not_Depend_On_Controllers()
    {
        // Arrange & Act
        var rule = ArchRuleDefinition.Types()
            .That().ResideInNamespaceMatching(@".*\.Handlers\.")
            .Should.NotDependOnAny(
                ArchRuleDefinition.Types().That().ResideInNamespaceMatching(@".*\.Controllers\."))
            .Because("Handlers are application layer - must not depend on presentation layer");

        // Assert
        rule.Check(Architecture);
    }

    [Fact]
    public void Infrastructure_Should_Not_Depend_On_Controllers()
    {
        // Arrange & Act
        var rule = ArchRuleDefinition.Types()
            .That().ResideInNamespaceMatching(@".*\.Infrastructure\.")
            .Should.NotDependOnAny(
                ArchRuleDefinition.Types().That().ResideInNamespaceMatching(@".*\.Controllers\."))
            .Because("Infrastructure layer must not depend on presentation layer");

        // Assert
        rule.Check(Architecture);
    }
}
