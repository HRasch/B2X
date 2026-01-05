namespace B2Connect.CMS.Core.Domain;

/// <summary>
/// Represents a void return type for functional operations (ADR-030)
/// Used when a command handler doesn't return a meaningful value
/// </summary>
public readonly struct Unit : IEquatable<Unit>
{
    public static readonly Unit Value = default;

    public override bool Equals(object? obj) => obj is Unit;

    public bool Equals(Unit other) => true;

    public override int GetHashCode() => 0;

    public override string ToString() => "()";

    public static bool operator ==(Unit left, Unit right) => true;

    public static bool operator !=(Unit left, Unit right) => false;
}
