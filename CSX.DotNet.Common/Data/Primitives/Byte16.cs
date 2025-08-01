using System;

namespace CSX.Common.Data.Primitives;

[Obsolete("Superceded by UInt128")]
public readonly partial struct Byte16(ulong high, ulong low)
{
    public readonly ulong High = high;
    public readonly ulong Low = low;
}