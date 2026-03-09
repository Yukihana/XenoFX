using CSX.DotNet.Common.Abstractions;
using CSX.DotNet.Common.Data.Text.Sanitization;
using CSX.DotNet.Common.Platform.Processes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Services.Utility.Configuration;

namespace XenoFx.Services.Processing.VideoTranscode;

public partial class VideoTranscodeService : IVideoTranscodeService
{
	// Infrastructure

	private readonly IFFMpegProvider _ffmpegProvider;
	private readonly IConfigurationService _configuration;
	private readonly ILogger<VideoTranscodeService> _logger;

	// Lifecycle

	public VideoTranscodeService(
		IFFMpegProvider ffmpegProvider,
		IConfigurationService configuration,
		ILogger<VideoTranscodeService> logger)
	{
		_ffmpegProvider = ffmpegProvider;
		_configuration = configuration;
		_logger = logger;
	}

	// Parameters

	public string TranscodeDirectory
		=> _configuration.TranscodeDirectory;

	// API

	public async Task<string> TranscodeForWebAsync(
		string sourcePath,
		string id,
		CancellationToken ctoken = default)
	{
		// temporary: Sanitize path
		id = Path.GetFileNameWithoutExtension(id);
		id = FileNameSanitizer.Sanitize(id);

		// Determine filename and return early if it already exists
		string targetPath = Path.Combine(
			TranscodeDirectory,
			$"{id}_web.mp4");

		if (File.Exists(targetPath))
			return targetPath; // Already transcoded

		// If not, acquire and run ffmpeg
		string ffmpegPath = await _ffmpegProvider.AcquireFFMpegAsync(ctoken);
		Directory.CreateDirectory(TranscodeDirectory);

		// Run FFMpeg
		string arguments = $"-i \"{sourcePath}\" -c:v libx264 -preset veryfast -crf 23 -c:a aac -movflags +faststart \"{targetPath}\"";
		await ProcessExecution.RunProcessAsync(ffmpegPath, arguments, ctoken);

		// Verify thumbnail was created
		if (!File.Exists(targetPath))
			throw new Exception($"Video transcode failed for file: {sourcePath}");

		return targetPath;
	}

	public async Task<bool> ValidateForWebAsync(
		string sourcePath,
		CancellationToken ctoken = default)
	{
		ctoken.ThrowIfCancellationRequested();

		// Detection args:
		string videoArgs = $"-v error -select_streams v:0 -show_entries stream=codec_name -of default=nk=1:nw=1 \"{sourcePath}\"";
		string audioArgs = $"-v error -select_streams a:0 -show_entries stream=codec_name -of default=nk=1:nw=1 \"{sourcePath}\"";
		string formatArgs = $"-v error -show_entries format=format_name -of default=nk=1:nw=1 \"{sourcePath}\"";

		// Run FFProbe
		string ffprobePath = await _ffmpegProvider.AcquireFFProbeAsync(ctoken);

		string videoCodec = await ProcessExecution.RunAndReadAsync(ffprobePath, videoArgs, ctoken);
		string audioCodec = await ProcessExecution.RunAndReadAsync(ffprobePath, audioArgs, ctoken);
		string container = await ProcessExecution.RunAndReadAsync(ffprobePath, formatArgs, ctoken);

		// Fix FFProbe output
		videoCodec = videoCodec.Trim().ToLowerInvariant();
		audioCodec = audioCodec.Trim().ToLowerInvariant();
		container = container.Trim().ToLowerInvariant();

		// Verdict : Browser-safe codecs
		bool videoOk = videoCodec is "h264" or "vp8" or "vp9" or "av1";
		bool audioOk = string.IsNullOrWhiteSpace(audioCodec) || audioCodec is "aac" or "mp3" or "opus" or "vorbis";
		bool containerOk = container.Contains("mp4") || container.Contains("webm");

		if (videoOk && audioOk && containerOk)
			return true;

		// Log reason for transcode requirement
		StringBuilder sb = new();
		sb.Append(videoOk ? "" : $" Video codec: {videoCodec}.");
		sb.Append(audioOk ? "" : $" Audio codec: {audioCodec}.");
		sb.Append(containerOk ? "" : $" Container: {container}.");
		string reason = sb.ToString().Trim();

		LogTranscodeRequired(sourcePath, reason);

		return false;
	}

	[LoggerMessage(
		Level = LogLevel.Information,
		Message = "Transcode required for {path}. Reason: {reason}")]
	private partial void LogTranscodeRequired(
		string path, string reason);
}