using System;
using System.Runtime.InteropServices;

namespace CSX.DotNet.Common.Platform;

public static partial class EnumMappings
{
    public static string GetTag(
        this Architecture arch)
    {
        return arch switch
        {
            Architecture.X86 => "x86",
            Architecture.X64 => "x64",
            Architecture.Arm => "arm",
            Architecture.Arm64 => "arm64",
            _ => throw new PlatformNotSupportedException(DebugMessages.UnsupportedArchitecture),
        };
    }
}