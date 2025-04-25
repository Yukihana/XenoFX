namespace XenoFx.Environment;

public sealed partial class XenoFxConfiguration
{
    // Data

    public string StartupPath { get; }
    public XenoFxProfile Profile { get; }
    public XenoFxOptions Options { get; }

    public XenoFxConfiguration(
        string startupPath,
        XenoFxProfile profile,
        XenoFxOptions options)
    {
        StartupPath = startupPath;
        Profile = profile;
        Options = options;
    }
}