using System.Threading;
using System.Threading.Tasks;

namespace XenoFx.Bridges.FileIndexing.ControlPanel;

public interface IControlPanelBridge
{
    /// <summary>
    /// Attempts to start a tenant with existing configuration.
    /// Returns false if the tenant configuration is invalid or missing.
    /// </summary>
    Task<bool> StartTenantAsync(
        string tenantId,
        CancellationToken cancellationToken = default);

    Task<bool> StartTenantAsync(
        string tenantId,
        // TODO : change this to:
        // Func<ITenantProfile, bool> configureTenant,
        // after moving the interface to abstractions
        object configuration,
        CancellationToken cancellationToken = default);

    Task<bool> StopTenantAsync(
        string tenantId,
        CancellationToken cancellationToken = default);

    // Technical notes:
    // We don't really need a tenancy control for this project.
    // Provide the API, but also targeted/test API for:
    // Ensuring the required tenant has booted up correctly.

    /// <summary>
    /// Call this after pre-initialization to ensure the required tenant has been started successfully.
    /// </summary>
    Task<bool> VerifyModuleSetupAsync(
        CancellationToken cancellationToken = default);
}