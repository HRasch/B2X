using Xunit;

namespace B2Connect.Tests.Integration;

/// <summary>
/// Collection definition for sharing AspireAppFixture across tests.
/// All tests in this collection share the same Aspire AppHost instance.
/// </summary>
[CollectionDefinition("AspireApp")]
public class AspireAppCollection : ICollectionFixture<AspireAppFixture>
{
    // This class has no code, and is never created.
    // Its purpose is simply to be the place to apply [CollectionDefinition]
}
