using System;
using System.Security.Cryptography;

namespace CSX.DotNet.Common.Data.DataGenerators;

public static class HexcodeGenerator
{
    public static string GenerateRandomHexId()
        => Convert.ToHexStringLower(RandomNumberGenerator.GetBytes(16));
}