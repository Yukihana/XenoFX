using CSX.DotNet.Modules.FileIndexing.Abstractions;
using Microsoft.Extensions.Logging;

namespace CSX.DotNet.Modules.FileIndexing.Api;

public sealed partial class FileCatalogueService : IFileIndexingCatalogue
{
    private readonly ILogger<FileCatalogueService> _logger;

    public FileCatalogueService(
        ILogger<FileCatalogueService> logger)
    {
        _logger = logger;

        _logger.LogInformation("FileCatalogueService initialized.");
    }
}