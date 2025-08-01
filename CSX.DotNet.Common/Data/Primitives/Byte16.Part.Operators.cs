namespace CSX.Common.Data.Primitives;

public readonly partial struct Byte16
{
    // Equality Operators

    public static bool operator ==(Byte16 left, Byte16 right) => left.Equals(right);

    public static bool operator !=(Byte16 left, Byte16 right) => !left.Equals(right);

    // Comparison Operators

    public static bool operator <(Byte16 left, Byte16 right) => left.CompareTo(right) < 0;

    public static bool operator >(Byte16 left, Byte16 right) => left.CompareTo(right) > 0;

    public static bool operator <=(Byte16 left, Byte16 right) => left.CompareTo(right) <= 0;

    public static bool operator >=(Byte16 left, Byte16 right) => left.CompareTo(right) >= 0;
}