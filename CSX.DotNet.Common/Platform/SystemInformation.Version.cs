using System;

namespace CSX.DotNet.Common.Platform;

public static partial class SystemInformation
{
    private static Version? _version = GetCurrentVersion();

    public static Version Version
        => _version ??= GetCurrentVersion();

    public static Version GetCurrentVersion()
        => Environment.OSVersion.Version;
}