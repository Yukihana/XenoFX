using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Services.Background.AssetIndexing;
using XenoFx.Services.Utility.Configuration;
using XenoFx.Services.Utility.PathValidator;

namespace XenoFx.Services.Hosted.AssetEnumeration;

public sealed partial class AssetEnumerationService : IAssetEnumerationService
{
    // Infrastructure

    private readonly IAssetIndexingService _assetIndexing;
    private readonly IPathValidatorService _pathValidator;
    private readonly IConfigurationService _configuration;
    private readonly ILogger<AssetEnumerationService> _logger;

    // Data

    private readonly SemaphoreSlim _lock = new(1);
    private CancellationTokenSource _cts = new();
    private Task? _enumerationTask;

    // Resources

    public AssetEnumerationService(
        IAssetIndexingService assetIndexing,
        IPathValidatorService pathValidator,
        IConfigurationService configuration,
        ILogger<AssetEnumerationService> logger)
    {
        _assetIndexing = assetIndexing;
        _pathValidator = pathValidator;
        _configuration = configuration;
        _logger = logger;
    }

    // Task

    private async Task ExecuteAsync(CancellationToken ctoken = default)
    {
        TimeSpan interval = TimeSpan.FromSeconds(_configuration.AssetEnumerationIntervalSeconds);
        Stopwatch stopwatch = new();

        while (!ctoken.IsCancellationRequested)
        {
            // Cycle wait
            await Task.Delay(1000, ctoken);
            if (stopwatch.ElapsedMilliseconds < interval.TotalMilliseconds)
                continue;

            // Do work if allowed
            if (_configuration.RuntimeContext.EnableAssetEnumeration)
            {
                await _assetIndexing.OnFilesEnumeratedAsync(ListFiles(), ctoken);
            }

            // Reset timer after task is completed to prevent zero interval edge case.
            stopwatch.Restart();
        }
    }

    // Enumeration API

    public string[] ListFiles()
    {
        string path = _configuration.AssetsDirectory;
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        string[] allItems = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
        List<string> validatedRelatives = [];

        foreach (var item in allItems)
        {
            if (_pathValidator.TryTruncateAssetPath(item, out string? relativePath))
                validatedRelatives.Add(relativePath);
        }

        return [.. validatedRelatives];
    }

    // IHostedService

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            if (_enumerationTask != null && !_enumerationTask.IsCompleted)
                return;

            if (_cts.IsCancellationRequested)
                _cts = new();

            _enumerationTask = Task.Run(() => ExecuteAsync(_cts.Token), cancellationToken);
        }
        finally { _lock.Release(); }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            if (_enumerationTask is not null && !_enumerationTask.IsCompleted)
            {
                await _cts.CancelAsync();
                await _enumerationTask.WaitAsync(cancellationToken).ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
                _enumerationTask = null;
            }
        }
        finally { _lock.Release(); }
    }

    // IDisposable

    public void Dispose()
    {
        _lock.Wait();
        try
        {
            _cts.Cancel();
            _enumerationTask?.Wait();
        }
        catch (AggregateException ex) when (ex.InnerExceptions.All(e => e is TaskCanceledException))
        { }
        finally { _lock.Release(); }
    }
}