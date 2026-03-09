using CSX.DotNet.Common.FileCompression.Abstractions;
using CSX.DotNet.Common.FileCompression.Shared;
using SharpCompress.Common;
using SharpCompress.Readers;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Common.FileCompression.Formats.Rar;

public sealed class RarUtility : ArchiveUtilityBase
{
	protected override async Task ExtractInternalAsync(
		string archivePath,
		string decompressionPath,
		ExtractionOverwriteMode overwriteMode = ExtractionOverwriteMode.Abort,
		CancellationToken ctoken = default)
	{
		ctoken.ThrowIfCancellationRequested();

		await using var reader = await ReaderFactory
			.OpenAsyncReader(archivePath, cancellationToken: ctoken);

		while (await reader.MoveToNextEntryAsync(ctoken))
		{
			var entry = reader.Entry;

			if (entry.IsDirectory ||
				string.IsNullOrWhiteSpace(entry.Key))
				continue;

			string combinedPath = Path.Combine(
				decompressionPath,
				entry.Key!); // Safe due to check above

			// Prevent directory traversal attacks by ensuring the entry is within the target directory
			if (!combinedPath.StartsWith(Path.GetFullPath(decompressionPath), StringComparison.OrdinalIgnoreCase))
				throw new IOException($"Entry is outside the target directory: {entry.Key}");

			string originalExtractionPath = combinedPath;

			// Ensure directory exists before extraction
			Directory.CreateDirectory(Path.GetDirectoryName(originalExtractionPath)!);

			// Get the source file's last modified time if available
			DateTime? sourceLastWriteTimeUtc = entry.LastModifiedTime?.ToUniversalTime();

			// Decide on final extraction path based on overwrite mode
			if (!FileSystemUtilities.TryGetExtractionPath(
					originalExtractionPath,
					overwriteMode,
					sourceLastWriteTimeUtc,
					out string finalExtractionPath))
			{
				// Skip extraction if TryGetExtractionPath says so
				continue;
			}

			// Safety-net in case TryGetExtractionPath fails to respect the overwrite policy
			bool overwrite =
				overwriteMode == ExtractionOverwriteMode.Always ||
				overwriteMode == ExtractionOverwriteMode.IfNewer;

			// Perform the actual extraction
			await reader.WriteEntryToFileAsync(finalExtractionPath, new ExtractionOptions
			{
				ExtractFullPath = true,
				Overwrite = overwrite,
			}, ctoken);
		}
	}
}