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

    // SubPaths : Base

    string AssetsDirectory { get; }

    // SubPaths : Cache

    string UploadDirectory { get; }

    // SubPaths : Assets

    string AssetUploadDirectory { get; }

    // Parameters

    ulong AssetEnumerationIntervalSeconds { get; }
}