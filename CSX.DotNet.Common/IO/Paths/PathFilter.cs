using Microsoft.Extensions.FileSystemGlobbing;
using System.Collections.Generic;

namespace CSX.DotNet.Common.IO.Paths;

public sealed partial class PathFilter
{
    private readonly Matcher _whiteMatcher = new();
    private readonly Matcher _greyMatcher = new();

    public PathFilter(PathFilterConfiguration config)
    {
        // Precompile the patterns once for efficiency
        _whiteMatcher.AddIncludePatterns(config.Whitelist);
        _greyMatcher.AddIncludePatterns(config.Greylist);
        _greyMatcher.AddExcludePatterns(config.Blacklist);
    }

    public bool Validate(string relativePath)
    {
        // Check whitelist first (whitelist overrides blacklist)
        if (_whiteMatcher.Match(relativePath).HasMatches)
            return true;

        // Check greylist + blacklist
        return _greyMatcher.Match(relativePath).HasMatches;
    }

    public IEnumerable<string> Validate(IEnumerable<string> relativePaths)
    {
        foreach (string relativePath in relativePaths)
        {
            if (Validate(relativePath))
                yield return relativePath;
        }
    }
}