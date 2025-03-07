namespace XenoFx.Environment;

public sealed partial class XenoFxOptions
{
    public const string SectionTitle = "XenoFxConfiguration";

    public string StartupPath { get; set; } = "XenoFxWorkspace/Default.xfp";

    public bool UseCommandLine { get; set; } = true;
}