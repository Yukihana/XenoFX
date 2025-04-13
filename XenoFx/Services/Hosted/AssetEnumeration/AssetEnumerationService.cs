using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Services.Background.AssetQueue;
using XenoFx.Services.Utility.Configuration;
using XenoFx.Services.Utility.PathValidator;

namespace XenoFx.Services.Hosted.AssetEnumeration;

public sealed partial class AssetEnumerationService : IAssetEnumerationService
{
    // Infrastructure

    private readonly IAssetQueueService _assetQueue;
    private readonly IPathValidatorService _pathValidator;
    private readonly IConfigurationService _configuration;
    private readonly ILogger<AssetEnumerationService> _logger;

    // Data

    private readonly SemaphoreSlim _lock = new(1);
    private CancellationTokenSource _cts = new();
    private Task? _enumerationTask;

    // Resources

    public AssetEnumerationService(
        IAssetQueueService assetQueue,
        IPathValidatorService pathValidator,
        IConfigurationService configuration,
        ILogger<AssetEnumerationService> logger)
    {
        _assetQueue = assetQueue;
        _pathValidator = pathValidator;
        _configuration = configuration;
        _logger = logger;

        _assetQueue.EnumerateCallback = EnumerateFiles;
    }

    public void Dispose()
    {
        _lock.Wait();
        try
        {
            _cts.Cancel();
            _assetQueue.EnumerateCallback = null;

            _enumerationTask?.Wait();
        }
        catch (AggregateException ex) when (ex.InnerExceptions.All(e => e is TaskCanceledException))
        { }
        finally { _lock.Release(); }
    }

    // Task

    private async Task ExecuteAsync(CancellationToken ctoken = default)
    {
        _logger.LogInformation("Service starting for enumeration root: {path}", _configuration.AssetsDirectory);
        await EnumerateAndIndexAsync(ctoken);

        // Repeat
        TimeSpan interval = TimeSpan.FromSeconds(_configuration.AssetEnumerationIntervalSeconds);
        Stopwatch stopwatch = new();

        while (!ctoken.IsCancellationRequested)
        {
            // Cycle wait
            await Task.Delay(1000, ctoken);
            if (stopwatch.ElapsedMilliseconds < interval.TotalMilliseconds)
                continue;

            // Do work and reset timer.
            await EnumerateAndIndexAsync(ctoken);
            stopwatch.Restart();
        }
    }

    private async Task EnumerateAndIndexAsync(CancellationToken ctoken = default)
    {
        try
        {
            ctoken.ThrowIfCancellationRequested();

            if (_configuration.RuntimeContext.AutoEnumerateAssets)
            {
                string[] paths = GetFiles();
                string[] validated = TruncateAndValidate(paths);
                _logger.LogInformation("Enumerating {count} files succeeded. Validated {count} assets for indexing...", paths.Length, validated.Length);
                await _assetQueue.OnFilesEnumeratedAsync(validated, ctoken);
            }
            else
            {
                _logger.LogInformation(
                    "{parameter} parameter is set to false. Skipping enumeration.",
                    nameof(_configuration.RuntimeContext.AutoEnumerateAssets));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Enumerating assets failed.");
        }
    }

    // Internal

    private string[] GetFiles()
    {
        string path = _configuration.AssetsDirectory;
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        return Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
    }

    private string[] TruncateAndValidate(string[] files)
    {
        List<string> validatedRelatives = [];
        foreach (var item in files)
        {
            if (_pathValidator.TryTruncateAssetPath(item, out string? relativePath))
                validatedRelatives.Add(relativePath);
        }

        return [.. validatedRelatives];
    }

    // API

    public string[] EnumerateFiles()
    {
        string[] allItems = GetFiles();
        return [.. TruncateAndValidate(allItems)];
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
}