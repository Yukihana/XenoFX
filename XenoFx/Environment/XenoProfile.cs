using System.Collections.Generic;

namespace XenoFx.Environment;

public partial class XenoProfile
{
    public string DatabasePath { get; set; } = string.Empty;
    public string CachePath { get; set; } = string.Empty;
    public List<string> AssetPaths { get; } = [];
    private List<string> WhiteListPatterns { get; } = [];
    public List<string> BlackListPatterns { get; } = [];
}