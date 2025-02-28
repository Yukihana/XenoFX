using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace CSX.Common.Data.Primitives;

public readonly partial struct Byte16 :
    IMinMaxValue<Byte16>,
    IEquatable<Byte16>,
    IComparable<Byte16>,
    IComparable
{
    // Overrides

    public override int GetHashCode()
        => HashCode.Combine(High, Low);

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is not Byte16 other)
            return false;
        return High == other.High && Low == other.Low;
    }

    // IMinMaxValue<T>

    public static Byte16 MinValue => new(ulong.MinValue, ulong.MinValue);
    public static Byte16 MaxValue => new(ulong.MaxValue, ulong.MaxValue);

    // IEquatable <T>

    public bool Equals(Byte16 other)
        => High == other.High && Low == other.Low;

    // IComparable<T>

    public int CompareTo(Byte16 other)
    {
        if (High < other.High)
            return -1;
        else if (High > other.High)
            return 1;
        else if (Low < other.Low)
            return -1;
        else if (Low > other.Low)
            return 1;
        else
            return 0;
    }

    // IComparable

    public int CompareTo(object? obj)
    {
        if (obj is null)
            return 1;

        if (obj is Byte16 other)
        {
            if (High < other.High)
                return -1;
            else if (High > other.High)
                return 1;
            else if (Low < other.Low)
                return -1;
            else if (Low > other.Low)
                return 1;
            else
                return 0;
        }

        throw new ArgumentException("Unsupported comparison target.");
    }
}