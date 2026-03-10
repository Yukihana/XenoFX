using CSX.DotNet.Modules.FileIndexing.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace XenoFx.Bridges.FileIndexing.ControlPanel;

public sealed partial class ControlPanelBridge :
    IControlPanelBridge
{
    // Infrastructure

    private readonly IFileIndexingControlPanel _controlPanel;
    private readonly ILogger<ControlPanelBridge> _logger;

    // Lifecycle

    public ControlPanelBridge(
        IFileIndexingControlPanel controlPanel,
        ILogger<ControlPanelBridge> logger)
    {
        _controlPanel = controlPanel;
        _logger = logger;
    }

    // Generic API

    public async Task<bool> StartTenantAsync(
        string tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _controlPanel
            .StartTenantAsync(tenantId, cancellationToken)
            .ConfigureAwait(false);
    }

    public Task<bool> StartTenantAsync(
        string tenantId,
        object configuration,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> StopTenantAsync(
        string tenantId,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    // Bootstrap Pre-Initialization API

    public async Task<bool> VerifyModuleSetupAsync(
        CancellationToken cancellationToken = default)
    {
        return await StartTenantAsync(
            InteropSettings.AssetsTenantId,
            cancellationToken)
            .ConfigureAwait(false);

        // If not active just start it. Do not set it up.
        // It should've been set up by the main module bootstrap.
        // If it wasn't set up, log and bail out.
    }
}