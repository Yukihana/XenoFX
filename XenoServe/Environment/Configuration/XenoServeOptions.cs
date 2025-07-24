namespace XenoServe.Environment.Configuration;

/// <summary>
/// Data-model for configuration extracted from appsettings.json
/// </summary>
public class XenoServeOptions
{
    public const string SectionTitle = nameof(XenoServeOptions);

    public string StartupProfilePath { get; set; } = string.Empty;
    public bool UseCommandLine { get; set; } = true;
}