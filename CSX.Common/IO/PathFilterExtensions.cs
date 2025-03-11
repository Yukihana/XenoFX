using CSX.Common.Platform;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace CSX.Common.IO;

public static partial class PathFilterExtensions
{
    // PathFilter

    // Legacy Filter

    public static string[] GetFiles(this PathFilterConfiguration config, string path, CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        return Directory
            .GetFiles(path, "*.*", SearchOption.AllDirectories)
            .Distinct(FilenameNormalization.FilenameComparer)
            .AsParallel().AsOrdered()
            .Where(config.Validate)
            .ToArray();
    }

    public static bool Validate(this PathFilterConfiguration config, string path)
    {
        // see if the algo can be improved based on this library class
        // https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.filesystemglobbing.matcher?view=dotnet-plat-ext-7.0

        ReadOnlySpan<char> pathSpan = path.AsSpan();

        foreach (string pattern in config.Whitelist)
        {
            if (pathSpan.MatchFilenameByPattern(pattern.AsSpan()))
                return true;
        }

        foreach (string pattern in config.Blacklist)
        {
            if (pathSpan.MatchFilenameByPattern(pattern.AsSpan()))
                return false;
        }

        foreach (string pattern in config.Greylist)
        {
            if (pathSpan.MatchFilenameByPattern(pattern.AsSpan()))
                return true;
        }

        return false;
    }
}