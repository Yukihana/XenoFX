namespace CSX.DotNet.Common.Platform;

/// <summary>
/// A list of operating system labels that's
/// more exhaustive in scope than provided by:
/// System.Runtime.InteropServices.RuntimeInformation.OSPlatform.
/// </summary>
public enum OperatingSystems
{
    Unknown = 0,

    // Windows

    Windows,
    Win,
    Win32,
    Win64,
    WindowsNT,
    WindowsServer,

    // macOS / OS X

    MacOS,
    MacOSX,
    MacCatalyst, // macOS on iPad hardware
    OSX,
    Darwin,     // By kernel name

    // Linux distributions / variants

    Linux,
    GNU_Linux,
    Lin,
    Ubuntu,
    Debian,
    Fedora,
    RedHat,
    CentOS,
    ArchLinux,
    Alpine,
    OpenSUSE,

    // BSD variants

    FreeBSD,
    OpenBSD,
    NetBSD,
    DragonFlyBSD,

    // Unix / POSIX general

    Solaris,
    AIX,
    HPUX,
    IRIX,
    Unix,
    HP_UX,
    SunOS,

    // Mobile / embedded

    Android,
    iOS,
    tvOS,
    watchOS,

    // Virtualization / special

    WSL,        // Windows Subsystem for Linux
    Emscripten, // WebAssembly
    Browser,    // Web browsers (e.g., Chrome, Firefox)

    // Legacy / uncommon

    DOS,
    AmigaOS,
    BeOS,
    OS2,
}