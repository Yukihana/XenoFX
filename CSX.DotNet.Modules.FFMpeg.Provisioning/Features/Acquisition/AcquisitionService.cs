using CSX.DotNet.Common.Platform;
using CSX.DotNet.Common.Platform.Unix;
using CSX.DotNet.Modules.FFMpeg.Provisioning.Features.Configuration;
using CSX.DotNet.Modules.FFMpeg.Provisioning.Features.Decompression;
using CSX.DotNet.Modules.FFMpeg.Provisioning.Features.Download;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Modules.FFMpeg.Provisioning.Features.Acquisition;

public partial class AcquisitionService : IAcquisitionService
{
    // Defaults

    public const string FfmpegDirectoryName = "FFmpeg"; // Non-config until required

    private readonly string[] UnixFileList =
    [
        "ffmpeg",
        "ffprobe",
        "ffplay"
    ];

    private IEnumerable<string> WinFileList
        => UnixFileList.Select(f => $"{f}.exe");

    // Infrastructure

    private readonly IDownloadService _download;
    private readonly IDecompressionService _decompression;
    private readonly IConfigurationService _configuration;
    private readonly ILogger<AcquisitionService> _logger;

    private readonly SemaphoreSlim _initLock = new(1);

    // Data

    private readonly bool _platformIsWindows;
    private readonly string _platformTag;

    // Lifecycle

    public AcquisitionService(
        IDownloadService download,
        IDecompressionService decompression,
        IConfigurationService configuration,
        ILogger<AcquisitionService> logger)
    {
        _download = download;
        _decompression = decompression;
        _configuration = configuration;
        _logger = logger;

        // Cache parameters ahead based on platform

        _platformTag = PlatformTag.GetCurrent("{os}_{arch}");
        _platformIsWindows = _platformTag.StartsWith("win"); // already normalized

        FFMpegDirectory = Path.Combine(
            _configuration.BinariesDirectory,
            FfmpegDirectoryName,
            _platformTag);

        string ext
            = _platformIsWindows
            ? ".exe"
            : string.Empty;

        // Set these up with platform specific filenames
        FFMpegBinaryPath = Path.Combine(FFMpegDirectory, $"ffmpeg{ext}");
        FFProbeBinaryPath = Path.Combine(FFMpegDirectory, $"ffprobe{ext}");
        FFPlayBinaryPath = Path.Combine(FFMpegDirectory, $"ffplay{ext}");
    }

    // FFMPEG Directory

    public string FFMpegDirectory { get; }
    public string FFMpegBinaryPath { get; }
    public string FFProbeBinaryPath { get; }
    public string FFPlayBinaryPath { get; }

    // Public API

    public async Task PreInitializeFFMpegAsync(
        CancellationToken ctoken = default)
    {
        await EnsureBinariesAsync(ctoken);
    }

    public async Task<string> AcquireFFMpegAsync(
        CancellationToken ctoken = default)
    {
        await EnsureBinariesAsync(ctoken);
        return FFMpegBinaryPath;
    }

    public async Task<string> AcquireFFProbeAsync(
        CancellationToken ctoken = default)
    {
        await EnsureBinariesAsync(ctoken);
        return FFProbeBinaryPath;
    }

    public async Task<string> AcquireFFPlayAsync(
        CancellationToken ctoken = default)
    {
        await EnsureBinariesAsync(ctoken);
        return FFPlayBinaryPath;
    }

    // Internal Shared

    private async Task EnsureBinariesAsync(
        CancellationToken ctoken = default)
    {
        await _initLock.WaitAsync(ctoken);
        try
        {
            // If the binaries can be confirmed, bail early
            try
            {
                EnsureBinaryPaths();
                return;
            }
            catch { }

            // Route based on platform tag
            string platformTag = _platformTag;
            _logger.LogInformation("Acquiring FFMpeg for platform: {PlatformTag}", platformTag);

            // Router
            switch (platformTag)
            {
                case "win_x64":
                    await GetWinX64FromBtbNAsync(ctoken);
                    break;

                case "lin_x64":
                    await GetLinuxX64FromBtbNAsync(ctoken);
                    break;

                case "lin_arm64":
                    await GetLinuxArm64FromBtbNAsync(ctoken);
                    break;

                case "osx_x64":
                    await GetOsxX64FromEvermeetAsync(ctoken);
                    break;

                default:
                    _logger.LogError("Unsupported platform tag: {PlatformTag}", platformTag);
                    throw new NotSupportedException($"FFMpeg support unavailable for platform: '{platformTag}'");
            }

            // Ensure binaries are available and permissions are set
            EnsureBinaryPaths();
            await SetBinaryPermissionsAsync(ctoken);
        }
        finally { _initLock.Release(); }
    }

    private void EnsureBinaryPaths()
    {
        EnsureBinaryPath(FFMpegBinaryPath);
        EnsureBinaryPath(FFProbeBinaryPath);
        EnsureBinaryPath(FFPlayBinaryPath);
    }

    private static void EnsureBinaryPath(string path)
    {
        if (!File.Exists(path))
            throw new InvalidOperationException($"Binary missing from: {path}");
    }

    private async Task SetBinaryPermissionsAsync(
        CancellationToken ctoken = default)
    {
        // Set permissions for relevant platforms
        if (!_platformIsWindows)
        {
            await UnixExecutableUtility.SetUnixExecuteBitAsync(FFMpegBinaryPath, ctoken);
            await UnixExecutableUtility.SetUnixExecuteBitAsync(FFProbeBinaryPath, ctoken);
            await UnixExecutableUtility.SetUnixExecuteBitAsync(FFPlayBinaryPath, ctoken);
        }
    }
}