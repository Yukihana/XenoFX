using System;

namespace CSX.DotNet.Common.Platform;

public static class PlatformTag
{
    public static string GetCurrent(string format = "")
    {
        // Default to short os and arch if no format is specified
        if (string.IsNullOrWhiteSpace(format))
            format = "{os}_{arch}_{ver}";

        var builder = PlatformTagBuilder.Create(format);

        // Detect placeholders and set the corresponding component
        if (format.Contains("{os}", StringComparison.OrdinalIgnoreCase))
            builder.SetCurrentOSTag();

        if (format.Contains("{OS}", StringComparison.OrdinalIgnoreCase))
            builder.SetCurrentOSLabel();

        if (format.Contains("arch", StringComparison.OrdinalIgnoreCase))
            builder.SetCurrentArch();

        if (format.Contains("ver", StringComparison.OrdinalIgnoreCase))
            builder.SetCurrentVersion();

        // Build and return the formatted tag
        return builder.ToString();
    }
}