namespace B2Connect.Shared.Kernel;

/// <summary>
/// Minimal base class for aggregate roots used by multiple domain projects.
/// </summary>
public abstract class AggregateRoot
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
}
