using System;

namespace CSX.DotNet.Common.Platform;

public static partial class SystemInformation
{
    // OperatingSystem

    private static OperatingSystems? _os = null;

    public static OperatingSystems OS
        => _os ??= GetCurrentOperatingSystem();

    public static OperatingSystems GetCurrentOperatingSystem()
    {
        if (OperatingSystem.IsWindows())
            return OperatingSystems.Windows;
        else if (OperatingSystem.IsMacOS())
            return OperatingSystems.MacOS;
        else if (OperatingSystem.IsLinux())
            return OperatingSystems.Linux;
        else if (OperatingSystem.IsFreeBSD())
            return OperatingSystems.FreeBSD;
        else if (OperatingSystem.IsAndroid())
            return OperatingSystems.Android;
        else if (OperatingSystem.IsIOS())
            return OperatingSystems.iOS;
        else if (OperatingSystem.IsTvOS())
            return OperatingSystems.tvOS;
        else if (OperatingSystem.IsWatchOS())
            return OperatingSystems.watchOS;
        else if (OperatingSystem.IsBrowser())
            return OperatingSystems.Browser;
        else if (OperatingSystem.IsMacCatalyst())
            return OperatingSystems.MacCatalyst;
        else
            throw new PlatformNotSupportedException("Unsupported or unmapped OS platform.");
    }
}