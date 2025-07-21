using CSX.Common.IO;
using XenoFx.Services.Utility.Configuration.Models;

namespace XenoFx.Services.Utility.Configuration;

public interface IConfigurationService
{
    // Runtime

    RuntimeContext RuntimeContext { get; }

    // Core

    PathFilterConfiguration AssetPathFilterConfiguration { get; }

    // SubPaths : Base

    string AssetsDirectory { get; }

    // SubPaths : Cache

    string UploadCacheDirectory { get; }

    // SubPaths : Assets

    string AssetsUploadDirectory { get; }

    // Parameters

    ulong AssetEnumerationIntervalSeconds { get; }
}