using System;

namespace CSX.DotNet.Common.Data.Collections;

public static partial class ElementShiftExtensions
{
    public static void ShiftLeftFrom<T>(
        this T[] array, int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, array.Length);

        var span = array.AsSpan();
        span[(index + 1)..].CopyTo(span[index..]);
    }
}