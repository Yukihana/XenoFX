using System;
using System.Numerics;

namespace CSX.DotNet.Common.IO.Integrity.Strategies.CrumbsSampling;

public static partial class CrumbsCartography
{
    public const int OutputSize = 32;

    public static int MapOffsets<T>(
        T sourceLength,
        Span<T> indexBuffer)
        where T : IBinaryInteger<T>
    {
        if (sourceLength <= T.Zero || indexBuffer.IsEmpty)
            return 0;

        // count will always fit in int (Span<T>.Length is int)
        int count = int.CreateChecked(T.Min(sourceLength, T.CreateChecked(indexBuffer.Length)));

        // If the source is smaller than the number of samples, clamp to length
        if (sourceLength <= T.CreateChecked(count))
        {
            for (int i = 0; i < count; i++)
                indexBuffer[i] = T.CreateChecked(i);
            return count;
        }

        // Map the indices evenly from 0 to length - 1
        double step = double.CreateChecked(sourceLength - T.One) / (count - 1);

        for (int i = 0; i < count; i++)
            indexBuffer[i] = T.CreateChecked(Math.Round(i * step));

        return count;
    }

    public static T[] CreateOffsets<T>(T sourceLength)
        where T : IBinaryInteger<T>
    {
        T[] buffer = new T[OutputSize];
        _ = MapOffsets(sourceLength, buffer);
        return buffer;
    }
}