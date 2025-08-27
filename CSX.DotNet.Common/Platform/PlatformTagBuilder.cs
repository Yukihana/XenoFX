using CSX.DotNet.Common.Data.Text.Builders;
using System;
using System.Runtime.InteropServices;

namespace CSX.DotNet.Common.Platform;

/// <summary>
/// A builder for creating platform-specific tags.
/// Note: Variants, Tools, Build and Suffix aren't implemented
/// because their values can vary widely.
/// </summary>
public class PlatformTagBuilder : FormatBuilder
{
    // Lifecycle

    protected PlatformTagBuilder(string format) : base(format)
    {
    }

    // Enforce factory pattern

    public static PlatformTagBuilder Create(string format)
        => new(format);

    // Core overrides

    public override PlatformTagBuilder SetComponent(
        string key,
        string value)
    {
        base.SetComponent(key, value);
        return this;
    }

    public override string ToString()
        => base.ToString();

    // Public API: Short OS

    private PlatformTagBuilder SetOSTag(string tag)
        => SetComponent("os", tag.ToLowerInvariant());

    public PlatformTagBuilder SetOSTag(OperatingSystems operatingSystem)
        => SetOSTag(operatingSystem.GetTag());

    public PlatformTagBuilder SetCurrentOSTag()
        => SetOSTag(SystemInformation.OS);

    // Public API: Long OS

    private PlatformTagBuilder SetOSLabel(string label)
        => SetComponent("OS", label.ToLowerInvariant());

    public PlatformTagBuilder SetOSLabel(OperatingSystems operatingSystem)
        => SetOSLabel(operatingSystem.GetLabel());

    public PlatformTagBuilder SetCurrentOSLabel()
        => SetOSLabel(SystemInformation.OS);

    // Public API: Architecture

    public PlatformTagBuilder SetArch(string arch)
        => SetComponent("arch", arch.ToLowerInvariant());

    public PlatformTagBuilder SetArch(Architecture arch)
        => SetArch(arch.ToString());

    public PlatformTagBuilder SetCurrentArch()
        => SetArch(SystemInformation.Architecture);

    // Public API: Version

    public PlatformTagBuilder SetVersion(string ver)
        => SetComponent("ver", ver.ToLowerInvariant());

    public PlatformTagBuilder SetVersion(int[] ver)
        => SetVersion(string.Join(".", ver));

    public PlatformTagBuilder SetVersion(Version ver)
        => SetVersion(ver.ToString());

    public PlatformTagBuilder SetCurrentVersion()
        => SetVersion(SystemInformation.Version);
}