using CSX.DotNet.Common.FileCompression;
using CSX.DotNet.Common.IO.DirectoryUtilities;
using CSX.DotNet.Common.IO.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Modules.FFMpeg.Provisioning.Features.Acquisition;

public partial class AcquisitionService
{
    private async Task GetOsxX64FromEvermeetAsync(
        CancellationToken ctoken = default)
    {
        string targetDirectory = FFMpegDirectory;
        var tasks = UnixFileList.Select(x =>
        {
            return DownloadAndExtractEvermeetBinaryAsync(
                binaryName: x,
                targetDirectory: targetDirectory,
                ctoken: ctoken);
        });

        await Task.WhenAll(tasks);
    }

    private async Task DownloadAndExtractEvermeetBinaryAsync(
        string binaryName,
        string targetDirectory,
        CancellationToken ctoken = default)
    {
        // Create Workspace and HttpClient
        await using var workspace = UniqueWorkspace.Create(
            parentDirectory: _configuration.SharedCacheDirectory,
            clearContentsOnDispose: true);

        // Fetch
        string url = BuildEvermeetUrl(binaryName);
        _logger.LogInformation("Attempting to fetch from Evermeet: {Url}", url);
        string downloadedPath = await _download.DownloadToWorkspaceAsync(
            url: url,
            workspace: workspace,
            relativePath: "",                       // unique dir, use workspace root
            preferredFileName: "",                  // prefer filename reported by server
            resolution: ConflictResolution.None,    // unique dir, don't auto-resolve conflict
            ctoken: ctoken);

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

            // Locate binary
            _logger.LogInformation(
                "Locating {binaryName} inside: {extractedRoot}",
                binaryName,
                extractedRoot);
            string binaryPath = FindEvermeetBinary(extractedRoot, binaryName);

            // Move file to where needed
            _logger.LogInformation(
                "Moving {binaryName} from {extracted} → {TargetDirectory}",
                binaryName,
                binaryPath,
                targetDirectory);
            Directory.CreateDirectory(targetDirectory);
            string finalPath = Path.Combine(targetDirectory, binaryName);
            File.Copy(binaryPath, finalPath, overwrite: true);
        }, ctoken);
    }

    private static string BuildEvermeetUrl(
        string binaryName)
    {
        /* API Instructions on-site:
         *
         * There's an easy way to download the latest binaries by using a download API:
         * https://evermeet.cx/ffmpeg/get[release][/(ffmpeg|ffprobe|ffplay|ffserver)][/(7z|zip)][/sig]
         * while the yellow paths are the default.
         *
         * To download the latest
         * ffmpeg snapshot as .7z: https://evermeet.cx/ffmpeg/get
         * ffmpeg snapshot as .zip: https://evermeet.cx/ffmpeg/get/zip
         * ffprobe snapshot as .7z: https://evermeet.cx/ffmpeg/get/ffprobe
         * ffprobe release as zip: https://evermeet.cx/ffmpeg/getrelease/ffprobe/zip
         * ffmpeg release as zip: https://evermeet.cx/ffmpeg/getrelease/zip
         */

        return binaryName switch
        {
            "ffmpeg" => "https://evermeet.cx/ffmpeg/getrelease",
            "ffprobe" => "https://evermeet.cx/ffmpeg/getrelease/ffprobe",
            "ffplay" => "https://evermeet.cx/ffmpeg/getrelease/ffplay",
            _ => throw new ArgumentException("Unsupported ff binary")
        };
    }

    private static string FindEvermeetBinary(
        string rootDirectory,
        string binaryName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rootDirectory, nameof(rootDirectory));
        ArgumentException.ThrowIfNullOrWhiteSpace(binaryName, nameof(binaryName));

        if (!Directory.Exists(rootDirectory))
            throw new DirectoryNotFoundException($"Root directory not found: {rootDirectory}");

        // === FAST PATH ===
        // Evermeet usually has just a single file inside,
        // But extraction may result in single-sibling directories that require traversal
        string traversalResult = DirectoryTraversal.TraverseUntilBranchOutOrDeadEnd(
            rootDirectory: rootDirectory);

        string traversedBinaryPath = Path.Combine(traversalResult, binaryName);
        if (File.Exists(traversedBinaryPath))
            return traversedBinaryPath;

        // === FALLBACK: BROAD SEARCH ===
        var filesEnumerator = Directory.EnumerateFiles(
            path: rootDirectory,
            searchPattern: binaryName,
            searchOption: SearchOption.AllDirectories);

        if (filesEnumerator.Any())
            return filesEnumerator.First();

        throw new FileNotFoundException("No file found matching the binary name.");
    }
}