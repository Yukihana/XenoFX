using CSX.DotNet.Common.FileCompression;
using CSX.DotNet.Common.IO.DirectoryUtilities;
using CSX.DotNet.Common.IO.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Modules.FFMpeg.Provisioning.Features.Acquisition;

public partial class AcquisitionService
{
    // Acquisitors

    private async Task GetWinX64FromBtbNAsync(
        CancellationToken ctoken = default)
    {
        string url = "https://github.com/BtbN/FFmpeg-Builds/releases/download/latest/ffmpeg-master-latest-win64-gpl-shared.zip";

        await DownloadAndExtractFromBtbNAsync(url, FFMpegDirectory, WinFileList, ctoken);
    }

    private async Task GetLinuxX64FromBtbNAsync(
        CancellationToken ctoken = default)
    {
        string url = "https://github.com/BtbN/FFmpeg-Builds/releases/download/latest/ffmpeg-master-latest-linux64-gpl.tar.xz";

        await DownloadAndExtractFromBtbNAsync(url, FFMpegDirectory, UnixFileList, ctoken);
    }

    private async Task GetLinuxArm64FromBtbNAsync(
        CancellationToken ctoken = default)
    {
        string url = "https://github.com/BtbN/FFmpeg-Builds/releases/download/latest/ffmpeg-master-latest-linuxarm64-gpl.tar.xz";

        await DownloadAndExtractFromBtbNAsync(url, FFMpegDirectory, UnixFileList, ctoken);
    }

    // Shared Helpers

    private async Task DownloadAndExtractFromBtbNAsync(
        string url,
        string targetDirectory,
        IEnumerable<string> fileList,
        CancellationToken ctoken = default)
    {
        // Create Workspace and HttpClient
        await using var workspace = UniqueWorkspace.Create(
            parentDirectory: _configuration.SharedCacheDirectory,
            clearContentsOnDispose: true);

        // Fetch
        _logger.LogInformation("Attempting to fetch from BtbN: {Url}", url);
        string downloadedPath = await _download.DownloadToWorkspaceAsync(
            url: url,
            workspace: workspace,
            relativePath: "",                       // unique dir, use workspace root
            preferredFileName: "",                  // prefer filename reported by server
            resolution: ConflictResolution.None,    // unique dir, don't auto-resolve conflict
            ctoken: ctoken);
        _logger.LogDebug("File downloaded to: {path}", downloadedPath);

        await workspace.ExecuteWithLockAsync(async (rootPath, ct) =>
        {
            // Decompress
            _logger.LogInformation(
                "Decompressing archive: {ArchivePath}",
                downloadedPath);
            string extractedRoot = await _decompression.DecompressAsync(
                archivePath: downloadedPath,
                decompressionPath: "", // dir with archive name, in same parent dir
                overwriteMode: ExtractionOverwriteMode.Always,
                deleteZipFile: false,
                ctoken: ct);

            // Locate binaries
            _logger.LogInformation(
                "Locating 'bin' directory in extracted files: {extractedRoot}",
                extractedRoot);
            string binDirectory = FindBtbNBinDirectory(extractedRoot, fileList);

            // Move all files in 'bin' folder to where needed (assume they have dependencies)
            _logger.LogInformation(
                "Moving files: {BinDirectory} → {TargetDirectory}",
                binDirectory, targetDirectory);
            Directory.CreateDirectory(targetDirectory);
            DirectoryContents.MoveTreesRecursive(
                topLevelEntries: Directory.EnumerateFileSystemEntries(binDirectory, "*", SearchOption.TopDirectoryOnly),
                targetRootDirectory: targetDirectory,
                overwrite: true,
                overwriteOnEntryTypeMismatch: true,
                ctoken: ct);
        }, ctoken);

        // Workspace cleanup: Automatic if using:
        // 'using' and 'ClearContentsOnDispose: true'
    }

    private static string FindBtbNBinDirectory(
        string rootDirectory,
        IEnumerable<string> fileList)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rootDirectory, nameof(rootDirectory));

        if (!Directory.Exists(rootDirectory))
            throw new DirectoryNotFoundException($"Root directory not found: {rootDirectory}");

        if (fileList == null || !fileList.Any())
            throw new ArgumentException("File list must contain at least one file.", nameof(fileList));

        // === FAST PATH ===
        // BtbN usually has a single top-level folder containing 'bin',
        // But extraction may result in single-sibling directories that require traversal
        string traversalResult = DirectoryTraversal.TraverseUntilBranchOutOrDeadEnd(
            rootDirectory: rootDirectory,
            stopAtDirectoryName: "bin");

        if (DirectoryContents.EnsureFilesInside(traversalResult, fileList))
            return traversalResult;

        // === FALLBACK: BROAD SEARCH ===
        var directoriesEnumerator = Directory.EnumerateDirectories(
            path: rootDirectory,
            searchPattern: "bin",
            searchOption: SearchOption.AllDirectories);

        foreach (var dir in directoriesEnumerator)
        {
            if (DirectoryContents.EnsureFilesInside(dir, fileList))
                return dir;
        }

        throw new DirectoryNotFoundException("No bin directory containing required files was found.");
    }
}