using System.Collections.Generic;

namespace CSX.DotNet.Common.IO.Paths;

public sealed partial class PathFilterConfiguration
{
    public List<string> Greylist { get; set; } = [];
    public List<string> Blacklist { get; set; } = [];
    public List<string> Whitelist { get; set; } = [];
}