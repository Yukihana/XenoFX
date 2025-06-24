using System;
using System.Diagnostics.CodeAnalysis;

namespace CSX.Common.Extensions.Validations;

public static partial class NullGuardExtensions
{
    /// <summary>
    /// Throws an ArgumentNullException if the reference is null.
    /// </summary>
    public static T EnsureNotNull<T>(
        [NotNull] this T? obj,
        string? paramName = null)
        where T : class
    {
        return obj
            ?? throw new ArgumentNullException(paramName ?? nameof(obj));
    }

    /// <summary>
    /// Throws an ArgumentNullException if the struct is null.
    /// </summary>
    public static T EnsureNotNull<T>(
        [NotNull] this T? obj,
        string? paramName = null)
        where T : struct
    {
        return obj
            ?? throw new ArgumentNullException(paramName ?? nameof(obj));
    }
}