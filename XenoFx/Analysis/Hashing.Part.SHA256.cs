using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace XenoFx.Analysis;

public static partial class Hashing
{
    public const long DefaultSha256BlockSize = 16 * 1024;

    public async static Task<byte[]> HashFileUsingSha256Async(Stream stream, bool hashFromStart = true, CancellationToken ctoken = default)
    {
        if (hashFromStart && stream.CanSeek)
            stream.Position = 0;
        byte[] hash = await SHA256.HashDataAsync(stream, ctoken);
        return hash;
    }

    // Figure out if hash-by-blocks actually helps
    // If so, make a composite hashing strategy, where the same read yields both full file as well as a hash-array for blocks. Also make a model for the result.

    public async static Task<Dictionary<long, byte[]>> HashBlocksUsingSha256Async(Stream stream, long blockSize = DefaultSha256BlockSize, bool hashFromStart = true, CancellationToken ctoken = default)
    {
        await Task.Yield();
        throw new NotImplementedException();
    }
}