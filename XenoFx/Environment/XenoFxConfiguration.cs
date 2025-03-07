namespace XenoFx.Environment;

public sealed partial class XenoFxConfiguration
{
    public XenoFxConfiguration(
        string startupPath,
        XenoFxProfile profile,
        XenoFxOptions options)
    {
        StartupPath = startupPath;
        Profile = profile;
        Options = options;
    }

    public string StartupPath { get; }
    public XenoFxProfile Profile { get; }
    public XenoFxOptions Options { get; }
}