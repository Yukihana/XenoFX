namespace CSX.Common.IO;

public static partial class PathFilterConfigurationExtensions
{
    public static PathFilterConfiguration Copy(this PathFilterConfiguration source) => new()
    {
        Whitelist = [.. source.Whitelist],
        Greylist = [.. source.Greylist],
        Blacklist = [.. source.Blacklist]
    };
}