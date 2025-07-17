using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CSX.Common.Data.DataGenerators;

public static class Int32Generators
{
    public static int GenerateDailySeed(params string[] seedParts)
    {
        // Build using current day and provided seed parts
        var day = DateTime.UtcNow.ToString("yyyy-MM-dd");
        StringBuilder seedBuilder = new(day);
        foreach (var part in seedParts.Where(x => !string.IsNullOrWhiteSpace(x)))
            seedBuilder.Append($"-{part}");
        var seedBase = seedBuilder.ToString();

        // Hash and fold to 32 bits
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(seedBase));
        var compacted = HashCompactors.FoldHash(hash, 4);   // Int32 is 4 bytes
        return BitConverter.ToInt32(compacted, 0);
    }

    public static int CreateHourSeed(string partialSeed)
    {
        var timeSlice = DateTime.UtcNow.ToString("yyyy-MM-dd-HH");
        var seedBase = $"{partialSeed}-{timeSlice}";

        // Hash and truncate to 32 bits
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(seedBase));
        return BitConverter.ToInt32(hash, 0);
    }
}