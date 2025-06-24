namespace CSX.Common.Data.Placeholders;

/// <summary>
/// Represents a unit type, which is a type that has only one value.
/// This is often used as a placeholder in generic types or methods
/// </summary>
public readonly struct Unit
{
    public static readonly Unit Value = new();
}