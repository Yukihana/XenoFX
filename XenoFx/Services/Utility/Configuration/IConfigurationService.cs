using CSX.DotNet.Common.IO;
using System.Collections.Immutable;
using XenoFx.Services.Utility.Configuration.Models;

namespace XenoFx.Services.Utility.Configuration;

public interface IConfigurationService
{
    // Runtime

    RuntimeContext RuntimeContext { get; }

    // Shared

    string AssetsDirectory { get; }
    string UploadsDirectory { get; }
    string MetadataDirectory { get; }
    string SharedCacheDirectory { get; }

    // Directories

    string ThumbsDirectory { get; }
    string AssetsUploadDirectory { get; }

    // Parameters

    PathFilterConfiguration AssetPathFilterConfiguration { get; }
    ImmutableArray<string> AllowedAssetExtensions { get; }
    ulong AssetEnumerationIntervalSeconds { get; }
}