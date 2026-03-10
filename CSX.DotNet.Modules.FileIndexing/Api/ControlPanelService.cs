using CSX.DotNet.Modules.FileIndexing.Abstractions;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Modules.FileIndexing.Api;

public sealed partial class ControlPanelService :
    IFileIndexingControlPanel
{
    private readonly ILogger<ControlPanelService> _logger;

    public ControlPanelService(
        ILogger<ControlPanelService> logger)
    {
        _logger = logger;
    }

    public async Task<bool> StartTenantAsync(
        string tenantId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting tenant with ID: {TenantId}", tenantId);
        // TODO

        return true;
    }
}