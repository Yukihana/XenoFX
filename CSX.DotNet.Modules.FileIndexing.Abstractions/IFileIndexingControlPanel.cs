namespace CSX.DotNet.Modules.FileIndexing.Abstractions;

public interface IFileIndexingControlPanel
{
    Task<bool> StartTenantAsync(
        string tenantId,
        CancellationToken cancellationToken = default);
}