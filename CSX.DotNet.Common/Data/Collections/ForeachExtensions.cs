using System;
using System.Collections.Generic;

namespace CSX.DotNet.Common.Data.Collections;

public static partial class ForeachExtensions
{
    public static void ForEach<T>(
        this IEnumerable<T> source,
        Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(action);

        foreach (T item in source)
            action(item);
    }
}