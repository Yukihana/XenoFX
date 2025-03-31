using CSX.Common.IO;
using XenoFx.Services.Utility.Configuration.Models;

namespace XenoFx.Services.Utility.Configuration;

public interface IConfigurationService
{
    // Runtime

    RuntimeContext RuntimeContext { get; }

    // Core

    PathFilterConfiguration AssetPathFilterConfiguration { get; }

    // Derived

    string BaseDirectory { get; }
    string AssetsDirectory { get; }
    string AssetUploadDirectory { get; }

    ulong AssetEnumerationIntervalSeconds { get; }
}