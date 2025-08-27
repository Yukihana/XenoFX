using CSX.DotNet.Common.IO.DirectoryUtilities;
using CSX.DotNet.Common.IO.Storage;
using CSX.DotNet.Modules.FFMpeg.Provisioning.Features.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Modules.FFMpeg.Provisioning.Features.Download;

public class DownloadService : IDownloadService
{
    // Infrastructure

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfigurationService _configuration;
    private readonly ILogger<DownloadService> _logger;

    // Lifecycle

    public DownloadService(
        IHttpClientFactory httpClientFactory,
        IConfigurationService configuration,
        ILogger<DownloadService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    // Public API

    public async Task<string> DownloadToWorkspaceAsync(
        string url,
        IWorkspace workspace,
        string relativePath = "",
        string preferredFileName = "",
        ConflictResolution resolution = ConflictResolution.None,
        CancellationToken ctoken = default)
    {
        try
        {
            using var httpClient = _httpClientFactory.CreateClient();

            return await httpClient.DownloadToWorkspaceAsync(
                url: url,
                workspace: workspace,
                relativePath: relativePath,
                preferredFileName: preferredFileName,
                resolution: resolution,
                ctoken: ctoken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to download from: {Url}", url);
            throw;
        }
    }
}