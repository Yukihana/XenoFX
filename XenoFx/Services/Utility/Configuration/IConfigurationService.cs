using CSX.Common.IO;
using System;
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
    ulong AssetEnumerationIntervalSeconds { get; }
}