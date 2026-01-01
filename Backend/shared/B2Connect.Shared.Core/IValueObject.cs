namespace B2Connect.Shared.Core;

/// <summary>
/// Marker interface for value objects in Domain-Driven Design
/// Value objects are immutable and defined by their values, not by identity
/// </summary>
public interface IValueObject
{
    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// Value objects should implement equality based on their constituent values.
    /// </summary>
    bool Equals(object? obj);

    /// <summary>
    /// Serves as the default hash function.
    /// Value objects should implement GetHashCode based on their constituent values.
    /// </summary>
    int GetHashCode();
}