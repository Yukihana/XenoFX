using System;

namespace CSX.DotNet.Common.Data.Collections;

public static partial class ResolverUtility
{
    /// <summary>
    /// Resolves a possibly negative index to a non-negative index relative to length.
    /// Supports negative indices similar to Python/C# ^ syntax.
    /// </summary>
    /// <param name="index">Index to resolve (can be negative).</param>
    /// <param name="length">Logical length of the collection.</param>
    /// <returns>Resolved non-negative index.</returns>
    public static int ResolveIndex(int index, int length)
    {
        int actualIndex = index >= 0 ? index : length + index;
        return ValidateIndex(actualIndex, length);
    }

    /// <summary>
    /// Validates that an index is within [0, length).
    /// </summary>
    public static int ValidateIndex(int index, int length)
    {
        if ((uint)index >= (uint)length)
            throw new ArgumentOutOfRangeException(nameof(index),
                $"Index {index} is out of range for length {length}.");
        return index;
    }

    /// <summary>
    /// Validates that a length is within [0, maxLength].
    /// </summary>
    public static int ValidateLength(
        int length,
        int maxLength,
        string? errorMessage = null)
    {
        if ((uint)length > (uint)maxLength)
        {
            throw new ArgumentOutOfRangeException(nameof(length),
                errorMessage ?? $"Length {length} is out of range (max {maxLength}).");
        }

        return length;
    }
}