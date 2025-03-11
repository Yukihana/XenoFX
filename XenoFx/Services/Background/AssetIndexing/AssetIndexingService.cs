using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Services.Storage.AssetPresence;
using XenoFx.Services.Utility.Configuration;

namespace XenoFx.Services.Background.AssetIndexing;

public sealed partial class AssetIndexingService : IAssetIndexingService
{
    // Infrastructure

    private readonly IAssetPresenceService _assetPresence;
    private readonly IConfigurationService _configuration;
    private readonly ILogger<AssetIndexingService> _logger;

    public AssetIndexingService(
        IAssetPresenceService assetPresence,
        IConfigurationService configuration,
        ILogger<AssetIndexingService> logger)
    {
        _assetPresence = assetPresence;
        _configuration = configuration;
        _logger = logger;
    }

    public Task OnFilesEnumeratedAsync(string[] files, CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        // Legacy Code
        _assetPresence.LegacyRegisterBulk(files);

        return Task.CompletedTask;
    }

    public Task OnFileCreatedAsync(string path, FileSystemEventArgs eventArgs, CancellationToken ctoken = default)
    {
        _logger.LogWarning("Indexing not implemented.");
        return Task.CompletedTask;
    }

    public Task OnFileDeletedAsync(string path, FileSystemEventArgs eventArgs, CancellationToken ctoken = default)
    {
        _logger.LogWarning("Indexing not implemented.");
        return Task.CompletedTask;
    }

    public Task OnFileModifiedAsync(string path, FileSystemEventArgs eventArgs, CancellationToken ctoken = default)
    {
        _logger.LogWarning("Indexing not implemented.");
        return Task.CompletedTask;
    }

    public Task OnFileRenamedAsync(string oldPath, string newPath, RenamedEventArgs e, CancellationToken ctoken = default)
    {
        _logger.LogWarning("Indexing not implemented.");
        return Task.CompletedTask;
    }

    public Task OnFileSystemErrorAsync(ErrorEventArgs e, CancellationToken ctoken = default)
    {
        _logger.LogWarning("Indexing not implemented.");
        return Task.CompletedTask;
    }
}