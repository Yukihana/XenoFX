using System;

namespace CSX.DotNet.Common.Platform;

public static partial class EnumMappings
{
    public static string GetTag(
        this OperatingSystems operatingSystem)
    {
        return operatingSystem switch
        {
            OperatingSystems.Windows => "win",
            OperatingSystems.Linux => "lin",
            OperatingSystems.MacOS => "osx",
            OperatingSystems.Android => "and",
            _ => throw new PlatformNotSupportedException(DebugMessages.UnsupportedPlatform),
        };
    }
}