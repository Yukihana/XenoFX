using System;
using System.Numerics;

namespace CSX.DotNet.Common.Data.Validations;

public static partial class NumberGuardExtensions
{
    public static int EnsureNotNegative(
        this int value,
        string? errorMessage = null)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value),
                errorMessage ?? "Value cannot be negative.");
        }

        return value;
    }

    public static long EnsureNotNegative(
        this long value,
        string? errorMessage = null)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value),
                errorMessage ?? "Value cannot be negative.");
        }

        return value;
    }

    public static T EnsurePositive<T>(
        this T value,
        string? argumentName = null,
        string? errorMessage = null)
        where T : INumber<T>
    {
        if (value <= T.Zero)
        {
            throw new ArgumentOutOfRangeException(
                argumentName ?? nameof(value),
                errorMessage ?? "Value cannot be zero or negative.");
        }

        return value;
    }
}