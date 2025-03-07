using CSX.Common.IO;

namespace XenoFx.Services.Utility.Configuration;

public interface IConfigurationService
{
    // Core

    PathFilterConfiguration AssetPathFilterConfiguration { get; }

    // Derived

    string BaseDirectory { get; }
    string AssetsDirectory { get; }
}