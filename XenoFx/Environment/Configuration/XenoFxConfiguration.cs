using CSX.Common.IO;
using System.IO;

namespace XenoFx.Environment.Configuration;

public sealed partial class XenoFxConfiguration
{
    // Infrastructure

    private readonly XenoFxProfile _profile;
    private readonly IXenoFxOptions _options;

    // Lifecycle

    public XenoFxConfiguration(
        XenoFxProfile profile,
        IXenoFxOptions options)
    {
        _profile = profile;
        _options = options;
    }

    // Profile; Not needed but keep anyway

    public string ProfilePath
        => XenoFxProfile.GetFilePath(_options.DataDirectory); //

    // Assets Database

    public string AssetsDatabasePath => Path.Combine(
        _options.DataDirectory,
        _profile.DatabasesDirectory,
        _profile.AssetsDatabasePath);

    public string AssetsDatabaseType
        => _profile.AssetsDatabaseType;

    // Cache Database

    public string CacheDatabasePath => Path.Combine(
        _options.DataDirectory,
        _profile.DatabasesDirectory,
        _profile.CacheDatabasePath);

    public string CacheDatabaseType
        => _profile.CacheDatabaseType;

    // Directories

    public string UploadCacheDirectory => Path.Combine(
        _options.DataDirectory,
        _profile.UploadCacheDirectory);

    public string AssetsUploadDirectory => Path.Combine(
        _options.DataDirectory,
        _profile.AssetsUploadDirectory);

    // Parameters

    public PathFilterConfiguration AssetFilterConfig
        => _profile.AssetFilterConfig;

    public ulong AssetEnumerationIntervalSeconds
        => _profile.AssetEnumerationIntervalSeconds;

    // Shared (Should be pre-resolved; Forward only)

    public string AssetsDirectory
        => _options.AssetsDirectory; // PathExtensions.ResolveCombine(dataDirectory, _configuration.AssetsDirectory);
}