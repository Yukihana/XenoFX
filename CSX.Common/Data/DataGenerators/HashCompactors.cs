using System;

namespace CSX.Common.Data.DataGenerators;

public static class HashCompactors
{
    public static byte[] FoldHash(byte[] hash, int length)
    {
        if (length <= 0 || length > hash.Length)
            throw new ArgumentOutOfRangeException(nameof(length), "Length must be > 0 and <= hash length");

        byte[] result = new byte[length];
        for (int i = 0; i < hash.Length; i++)
            result[i % length] ^= hash[i];

        return result;
    }
}