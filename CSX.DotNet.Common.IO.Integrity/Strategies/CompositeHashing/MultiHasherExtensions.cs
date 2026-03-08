using CSX.DotNet.Common.IO.Integrity.Strategies.CompositeHashing.Adapters;
using System.Security.Cryptography;

namespace CSX.DotNet.Common.IO.Integrity.Strategies.CompositeHashing;

public static partial class MultiHasherExtensions
{
    public static MultiHasher WithSHA256(
        this MultiHasher hasher)
    {
        hasher.Hashers.Add(new IncrementalHasherAdapter(HashAlgorithmName.SHA256));
        return hasher;
    }

    public static MultiHasher WithBlake3(
        this MultiHasher hasher)
    {
        hasher.Hashers.Add(new Blake3Adapter());
        return hasher;
    }

    public static MultiHasher WithCrumbs(
        this MultiHasher hasher,
        long length)
    {
        hasher.Hashers.Add(new CrumbsAdapter(length));
        return hasher;
    }

    public static MultiHasher WithDHD256(
        this MultiHasher hasher)
    {
        hasher.Hashers.Add(new DHD256Adapter());
        return hasher;
    }
}