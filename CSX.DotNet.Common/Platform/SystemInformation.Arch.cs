using System.Runtime.InteropServices;

namespace CSX.DotNet.Common.Platform;

public static partial class SystemInformation
{
    // Architecture

    private static Architecture? _architecture = null;

    public static Architecture Architecture
        => _architecture ??= RuntimeInformation.OSArchitecture;
}