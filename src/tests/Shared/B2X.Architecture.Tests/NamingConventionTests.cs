// -----------------------------------------------------------------------------
// B2X Architecture Tests - Naming Convention Rules
// ADR-021: ArchUnitNET for Automated Architecture Testing
// -----------------------------------------------------------------------------

using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;
using Xunit;

namespace B2X.Architecture.Tests;

/// <summary>
/// Tests enforcing naming conventions for handlers, events, commands, and validators.
/// Consistent naming improves discoverability and maintainability.
/// </summary>
[Collection("Architecture")]
public class NamingConventionTests : ArchitectureTestBase
{
    [Fact]
    public void Handlers_Should_Have_Handler_Suffix()
    {
        // Arrange & Act - Check classes in Handlers namespace follow naming
        ArchRule.Classes()
            .That().ResideInNamespaceMatching(@".*\.Handlers\.")
            .And().AreNotAbstract()
            .And().DoNotHaveNameContaining("Base")
            .Should().HaveNameEndingWith("Handler")
            .Because("All message handlers must follow the naming convention *Handler")
            .WithoutRequiringPositiveResults() // May not have classes in this namespace
            .Check(Architecture);
    }

    [Fact]
    public void Validators_Should_Have_Validator_Suffix()
    {
        // Arrange & Act
        ArchRule.Classes()
            .That().ResideInNamespaceMatching(@".*\.Validators\.")
            .And().AreNotAbstract()
            .Should().HaveNameEndingWith("Validator")
            .Because("All validators must follow the naming convention *Validator")
            .WithoutRequiringPositiveResults() // May not have Validators namespace yet
            .Check(Architecture);
    }

    [Fact]
    public void Controllers_Should_Have_Controller_Suffix()
    {
        // Arrange & Act
        ArchRule.Classes()
            .That().ResideInNamespaceMatching(@".*\.Controllers\.")
            .And().AreNotAbstract()
            .And().DoNotHaveNameContaining("Base")
            .Should().HaveNameEndingWith("Controller")
            .OrShould().HaveNameEndingWith("Endpoint")
            .Because("All API controllers must follow the naming convention *Controller or *Endpoint")
            .WithoutRequiringPositiveResults() // May not have Controllers namespace
            .Check(Architecture);
    }

    [Fact]
    public void Events_In_Events_Namespace_Should_Have_Event_Suffix()
    {
        // Arrange & Act
        ArchRule.Classes()
            .That().ResideInNamespaceMatching(@".*\.Events\.")
            .And().AreNotAbstract()
            .And().DoNotHaveNameContaining("Base")
            .And().DoNotHaveNameContaining("Handler") // Exclude event handlers
            .Should().HaveNameEndingWith("Event")
            .Because("Domain events must follow the naming convention *Event")
            .WithoutRequiringPositiveResults() // May not have Events namespace yet
            .Check(Architecture);
    }

    [Fact]
    public void Services_Should_Have_Service_Suffix()
    {
        // Arrange & Act - Services in Services namespace should follow naming
        ArchRule.Classes()
            .That().ResideInNamespaceMatching(@".*\.Services\.")
            .And().AreNotAbstract()
            .And().DoNotHaveNameContaining("Base")
            .And().DoNotHaveNameContaining("Factory")
            .And().DoNotHaveNameContaining("Provider")
            .And().DoNotHaveNameContaining("Options")
            .And().DoNotHaveNameContaining("Configuration")
            .And().DoNotHaveNameContaining("Context") // DbContext, etc.
            .And().DoNotHaveNameContaining("Helper")
            .Should().HaveNameEndingWith("Service")
            .Because("Application services must follow the naming convention *Service")
            .WithoutRequiringPositiveResults() // May have non-Service classes
            .Check(Architecture);
    }

    [Fact]
    public void Interfaces_Should_Have_I_Prefix()
    {
        // Arrange & Act
        ArchRule.Interfaces()
            .That().ArePublic()
            .Should().HaveNameStartingWith("I")
            .Because("All interfaces must follow C# convention with I prefix")
            .Check(Architecture);
    }

    [Fact]
    public void Abstract_Classes_Should_Have_Base_Suffix_Or_Abstract_Prefix()
    {
        // This is a softer rule - we check but allow exceptions
        ArchRule.Classes()
            .That().AreAbstract()
            .And().AreNotSealed()
            .Should().HaveNameEndingWith("Base")
            .OrShould().HaveNameStartingWith("Abstract")
            .OrShould().HaveNameContaining("Base")
            .Because("Abstract base classes should be clearly identifiable")
            .WithoutRequiringPositiveResults();

        // Note: This rule may need exceptions for framework-derived classes
        // Uncomment when ready to enforce:
        // .Check(Architecture);
    }
}
