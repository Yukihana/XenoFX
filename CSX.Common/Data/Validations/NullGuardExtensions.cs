using System;
using System.Diagnostics.CodeAnalysis;

namespace CSX.Common.Data.Validations;

public static partial class NullGuardExtensions
{
    /// <summary>
    /// Throws an ArgumentNullException if the reference is null.
    /// </summary>
    public static T EnsureNotNull<T>(
        [NotNull] this T? obj,
        string? paramName = null,
        string? message = null)
        where T : class
    {
        return obj ?? throw new ArgumentNullException(
            paramName ?? nameof(obj),
            message);
    }

    /// <summary>
    /// Throws an ArgumentNullException if the struct is null.
    /// </summary>
    public static T EnsureNotNull<T>(
        [NotNull] this T? obj,
        string? paramName = null,
        string? message = null)
        where T : struct
    {
        return obj ?? throw new ArgumentNullException(
            paramName ?? nameof(obj),
            message);
    }
}