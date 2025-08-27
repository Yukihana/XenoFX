using CSX.DotNet.Common.Data.Guids;
using CSX.DotNet.Common.Data.Text.Sanitization;
using CSX.DotNet.Common.IO.DirectoryUtilities;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Common.IO.Storage;

public static class DownloadUtility
{
    private static HttpClient CreateClient() => new(
        new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(5), // recycle connections
            PooledConnectionIdleTimeout = TimeSpan.FromMinutes(2), // close idle ones
            // No need to cap max connections since this a library function.
            // For custom configurations: use the client based overloads instead,
            // e.g. frequency-capped API: pass a reusable client with the max connection limit baked in.
        },
        disposeHandler: true);

    private static readonly Lazy<HttpClient> _lazyHttpClientWrapper
        = new(() => CreateClient(), isThreadSafe: true);

    private static HttpClient HttpClient
        => _lazyHttpClientWrapper.Value;  // Internal API for non-DI

    // Internal

    private static string RetrieveFileNameFromHeaders(
        this HttpResponseMessage response)
    {
        if (response.Content.Headers.ContentDisposition is { } disposition)
        {
            string? raw = disposition.FileNameStar ?? disposition.FileName;

            if (!string.IsNullOrWhiteSpace(raw))
                return Path.GetFileName(raw.Trim('"'));
        }
        return string.Empty;
    }

    // Public API : Core

    public static async Task<string> DownloadAsync(
        this HttpClient httpClient,
        string url,
        string parentDirectory,
        string preferredFileName = "",
        ConflictResolution resolution = ConflictResolution.None,
        CancellationToken ctoken = default)
    {
        parentDirectory = Path.GetFullPath(parentDirectory);

        // Open connection
        using var response = await httpClient.GetAsync(
            url,
            HttpCompletionOption.ResponseHeadersRead,
            ctoken);
        response.EnsureSuccessStatusCode();

        // Resolve filename
        byte fileNameAttempt = 0;
        string downloadFileName = string.Empty;
        do
        {
            downloadFileName = fileNameAttempt switch
            {
                0 => FileNameSanitizer.FromFileName(preferredFileName),
                1 => FileNameSanitizer.FromFileName(response.RetrieveFileNameFromHeaders()),
                2 => FileNameSanitizer.FromPathAndQuery(url),
                3 => FileNameSanitizer.FromUrl(url),
                _ => $"download_{DMC212710Guid.FromUtcNow():N}.bin",
            };
            unchecked { fileNameAttempt++; }
        }
        while (string.IsNullOrWhiteSpace(downloadFileName));

        // Build the save path and delegate to the save utility
        string savePath = Path.Combine(
            parentDirectory,
            downloadFileName);

        return await SaveUtility.SaveToDiskAsync(
            stream: await response.Content.ReadAsStreamAsync(ctoken),
            savePath: savePath,
            resolution: resolution,
            ctoken: ctoken);
    }

    public static async Task<string> DownloadToWorkspaceAsync(
        this HttpClient httpClient,
        string url,
        IWorkspace workspace,
        string preferredFileName = "",
        string relativePath = "",
        ConflictResolution resolution = ConflictResolution.None,
        CancellationToken ctoken = default)
    {
        return await workspace.ExecuteWithLockAsync(async (rootPath, ct) =>
        {
            string parentDirectory = Path.Combine(
                rootPath,
                relativePath);

            return await DownloadAsync(
                httpClient: httpClient,
                url: url,
                parentDirectory: parentDirectory,
                preferredFileName: preferredFileName,
                resolution: resolution,
                ctoken: ctoken);
        }, ctoken);
    }

    // Thin wrappers

    public static async Task<string> DownloadAsync(
        string url,
        string parentDirectory,
        string preferredFileName = "",
        ConflictResolution resolution = ConflictResolution.None,
        CancellationToken ctoken = default)
    {
        return await DownloadAsync(
            httpClient: HttpClient,
            url: url,
            parentDirectory: parentDirectory,
            preferredFileName: preferredFileName,
            resolution: resolution,
            ctoken: ctoken);
    }

    public static async Task<string> ToWorkspaceAsync(
        string url,
        IWorkspace workspace,
        string preferredFileName = "",
        string relativePath = "",
        ConflictResolution resolution = ConflictResolution.None,
        CancellationToken ctoken = default)
    {
        return await DownloadToWorkspaceAsync(
            httpClient: HttpClient,
            url: url,
            workspace: workspace,
            preferredFileName: preferredFileName,
            relativePath: relativePath,
            resolution: resolution,
            ctoken: ctoken);
    }

    public static async Task<string> DownloadToUniqueDirectoryAsync(
        this HttpClient httpClient,
        string url,
        string parentDirectory,
        string preferredFileName = "",
        CancellationToken ctoken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(url);

        await using UniqueWorkspace workspace = UniqueWorkspace.Create(
            parentDirectory: parentDirectory,   // working directory if empty
            clearContentsOnDispose: false);     // caller owns cleanup

        return await DownloadToWorkspaceAsync(
            httpClient: httpClient,
            url: url,
            workspace: workspace,
            preferredFileName: preferredFileName,
            relativePath: "",
            resolution: ConflictResolution.Overwrite,
            ctoken: ctoken);
    }
}