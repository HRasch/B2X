// -----------------------------------------------------------------------------
// B2X Architecture Tests - Layer Dependency Rules
// ADR-021: ArchUnitNET for Automated Architecture Testing
// ADR-002: Onion/Clean Architecture
// -----------------------------------------------------------------------------

using ArchUnitNET;
using ArchUnitNET.Fluent;
using TngTech.ArchUnitNET.xUnitV3;
using Xunit;
using static TngTech.ArchUnitNET.Fluent.ArchRule;

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
        ArchRule.Types()
            .That().ResideInNamespaceMatching(@".*\.Core\.")
            .Should().NotDependOnAny(
                ArchRule.Types().That().ResideInNamespaceMatching(@".*\.Infrastructure\."))
            .Because("Domain Core must be independent of infrastructure concerns (ADR-002)")
            .Check(Architecture);
    }

    [Fact]
    public void Domain_Core_Should_Not_Depend_On_EntityFramework()
    {
        // Arrange & Act
        ArchRule.Types()
            .That().ResideInNamespaceMatching(@".*\.Core\.")
            .Should().NotDependOnAny(
                ArchRule.Types().That().ResideInNamespaceMatching(@"Microsoft\.EntityFrameworkCore.*"))
            .Because("Domain Core must be persistence-ignorant - no EF Core dependencies allowed")
            .Check(Architecture);
    }

    [Fact]
    public void Domain_Core_Should_Not_Depend_On_AspNetCore()
    {
        // Arrange & Act
        ArchRule.Types()
            .That().ResideInNamespaceMatching(@".*\.Core\.")
            .Should().NotDependOnAny(
                ArchRule.Types().That().ResideInNamespaceMatching(@"Microsoft\.AspNetCore.*"))
            .Because("Domain Core must not depend on web framework concerns")
            .Check(Architecture);
    }

    [Fact]
    public void Handlers_Should_Not_Depend_On_Controllers()
    {
        // Arrange & Act
        ArchRule.Types()
            .That().ResideInNamespaceMatching(@".*\.Handlers\.")
            .Should().NotDependOnAny(
                ArchRule.Types().That().ResideInNamespaceMatching(@".*\.Controllers\."))
            .Because("Handlers are application layer - must not depend on presentation layer")
            .Check(Architecture);
    }

    [Fact]
    public void Infrastructure_Should_Not_Depend_On_Controllers()
    {
        // Arrange & Act
        ArchRule.Types()
            .That().ResideInNamespaceMatching(@".*\.Infrastructure\.")
            .Should().NotDependOnAny(
                ArchRule.Types().That().ResideInNamespaceMatching(@".*\.Controllers\."))
            .Because("Infrastructure layer must not depend on presentation layer")
            .Check(Architecture);
    }
}
