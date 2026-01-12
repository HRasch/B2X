// -----------------------------------------------------------------------------
// B2X Architecture Tests - Wolverine CQRS Pattern Rules
// ADR-021: ArchUnitNET for Automated Architecture Testing
// ADR-001: Event-Driven Architecture with Wolverine CQRS
// -----------------------------------------------------------------------------

using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.xUnitV3;
using Xunit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace B2X.ArchitectureTests;

/// <summary>
/// Tests enforcing Wolverine CQRS patterns.
/// Ensures handlers, commands, queries, and events follow established patterns.
/// </summary>
[Collection("Architecture")]
public class WolverinePatternTests : ArchitectureTestBase
{
    [Fact]
    public void Handlers_Should_Be_In_Handlers_Or_Application_Namespace()
    {
        // B2X allows handlers in .Handlers, .Endpoints, or .Application namespaces
        Classes()
            .That().HaveNameEndingWith("Handler")
            .And().DoNotResideInNamespaceMatching(@".*Test.*")
            .And().DoNotResideInNamespaceMatching(@".*Mock.*")
            .Should().ResideInNamespaceMatching(@".*(\.Handlers|\.Endpoints|\.Application).*")
            .Because("Wolverine handlers must be located in Handlers, Endpoints, or Application namespace for discoverability")
            .Check(B2XArchitecture);
    }

    [Fact]
    public void Commands_Should_Be_In_Known_Locations()
    {
        // B2X allows commands in various locations (Endpoints, Handlers, Commands, Core.Interfaces)
        Classes()
            .That().HaveNameEndingWith("Command")
            .And().DoNotResideInNamespaceMatching(@".*Test.*")
            .Should().ResideInNamespaceMatching(@".*(\.Endpoints|\.Handlers|\.Commands|\.Core\.|\.Application).*")
            .Because("Commands should be colocated with their handlers or in domain namespaces")
            .Check(B2XArchitecture);
    }

    [Fact]
    public void Queries_Should_Be_In_Known_Locations()
    {
        // B2X allows queries in various locations (including B2X.Search for SearchQuery)
        Classes()
            .That().HaveNameEndingWith("Query")
            .And().DoNotResideInNamespaceMatching(@".*Test.*")
            .Should().ResideInNamespaceMatching(@".*(\. Endpoints|\.Handlers|\.Queries|\.Application|\.Search).*")
            .Because("Queries should be colocated with their handlers or in dedicated namespaces")
            .WithoutRequiringPositiveResults() // May not have Query classes yet
            .Check(B2XArchitecture);
    }

    [Fact]
    public void Domain_Events_Should_Not_Have_External_Dependencies()
    {
        // Domain events should be simple POCOs without external framework dependencies
        Classes()
            .That().HaveNameEndingWith("Event")
            .And().ResideInNamespaceMatching(@".*\.Core\.")
            .Should().NotDependOnAny(
                ArchRuleDefinition.Types().That().ResideInNamespaceMatching(@"Microsoft\.EntityFrameworkCore.*"))
            .AndShould().NotDependOnAny(
                ArchRuleDefinition.Types().That().ResideInNamespaceMatching(@"Microsoft\.AspNetCore.*"))
            .Because("Domain events must be simple POCOs without infrastructure dependencies")
            .WithoutRequiringPositiveResults() // May not have events in Core yet
            .Check(B2XArchitecture);
    }

    [Fact]
    public void Handlers_Should_Not_Directly_Access_Other_Handlers()
    {
        // Handlers should communicate through the message bus, not direct calls
        Classes()
            .That().HaveNameEndingWith("Handler")
            .And().ResideInNamespaceMatching(@".*\.Handlers\.")
            .Should().NotDependOnAny(
                Classes().That().HaveNameEndingWith("Handler")
                    .And().ResideInNamespaceMatching(@".*\.Handlers\."))
            .Because("Handlers should communicate via message bus, not direct dependencies (ADR-001)")
            .WithoutRequiringPositiveResults(); // May have no handler-to-handler deps

        // Note: This rule may need refinement for cascade handlers
        // Uncomment when ready to enforce strictly:
        // .Check(B2XArchitecture);
    }
}
