using System;

namespace CSX.DotNet.Common.Platform;

public static partial class EnumMappings
{
    public static string GetLabel(
        this OperatingSystems operatingSystem)
    {
        return operatingSystem switch
        {
            OperatingSystems.Windows => "windows",
            OperatingSystems.Linux => "linux",
            OperatingSystems.MacOS => "macosx",
            OperatingSystems.Android => "android",
            _ => throw new PlatformNotSupportedException(DebugMessages.UnsupportedPlatform),
        };
    }
}