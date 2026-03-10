using CSX.DotNet.Modules.FileIndexing.Abstractions;
using Microsoft.Extensions.Logging;

namespace XenoFx.Bridges.FileIndexing.FileCatalogue;

// Break this up. Catalogue is scoped, CP is singleton.
public sealed partial class FileCatalogueBridge :
    IFileCatalogueBridge
{
    private readonly IFileIndexingCatalogue _fileCatalogue;
    private readonly ILogger<FileCatalogueBridge> _logger;

    public FileCatalogueBridge(
        IFileIndexingCatalogue fileCatalogue,
        ILogger<FileCatalogueBridge> logger)
    {
        _fileCatalogue = fileCatalogue;
        _logger = logger;
    }
}