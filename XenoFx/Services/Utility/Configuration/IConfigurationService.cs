using CSX.DotNet.Common.IO.Paths;
using System.Collections;
using System.Collections.Generic;
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
    string TranscodeDirectory { get; }
    string AssetsUploadDirectory { get; }

    // Parameters

    ImmutableArray<string> AllowedAssetExtensions { get; }
    PathFilterConfiguration AssetPathFilterConfiguration { get; }
    ImmutableArray<string> AllowedAssetUploadExtensions { get; }
    ulong AssetEnumerationIntervalSeconds { get; }
}