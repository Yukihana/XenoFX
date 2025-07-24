using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace CSX.Common.IO.Paths;

public static partial class PathPolicyParameters
{
    private static readonly ImmutableArray<char> EnforcedInvalidFileNameCharacters
        = ['/', '\\', ':', '*', '?', '"', '<', '>', '|', '&'];

    public static ImmutableArray<char> ExtendedInvalidFileNameCharacters { get; }
        = [.. Path.GetInvalidFileNameChars().Concat(EnforcedInvalidFileNameCharacters).Distinct()];
}