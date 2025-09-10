using CSX.DotNet.Modules.FileIndexing.Shared.Configuration;
using Microsoft.Extensions.Logging;

namespace CSX.DotNet.Modules.FileIndexing.Shared.PathValidation;

public class PathValidationService : IPathValidationService
{
    // Infrastructure

    private readonly IConfigurationService _configuration;
    private readonly ILogger<PathValidationService> _logger;

    // Lifecycle

    public PathValidationService(
        IConfigurationService configuration,
        ILogger<PathValidationService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }
}