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

    public async Task OnFileCreatedAsync(FileSystemEventArgs e, CancellationToken ctoken = default)
    {
        await Task.Yield();
    }

    public async Task OnFileDeletedAsync(FileSystemEventArgs e, CancellationToken ctoken = default)
    {
        await Task.Yield();
    }

    public async Task OnFileModifiedAsync(FileSystemEventArgs e, CancellationToken ctoken = default)
    {
        await Task.Yield();
    }

    public async Task OnFileRenamedAsync(RenamedEventArgs e, CancellationToken ctoken = default)
    {
        await Task.Yield();
    }

    public async Task OnFileSystemErrorAsync(ErrorEventArgs e, CancellationToken ctoken = default)
    {
        await Task.Yield();
    }
}