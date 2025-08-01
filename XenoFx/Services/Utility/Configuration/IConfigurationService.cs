using CSX.DotNet.Common.IO;
using System.Collections.Immutable;
using XenoFx.Services.Utility.Configuration.Models;

namespace XenoFx.Services.Utility.Configuration;

public interface IConfigurationService
{
    // Runtime

    RuntimeContext RuntimeContext { get; }

    // Core

    PathFilterConfiguration AssetPathFilterConfiguration { get; }

    // Directories

    string AssetsDirectory { get; }
    string AssetsUploadDirectory { get; }

    string MetadataDirectory { get; }
    string UploadsDirectory { get; }

    // Parameters

    ImmutableArray<string> AllowedAssetExtensions { get; }
    ulong AssetEnumerationIntervalSeconds { get; }
}