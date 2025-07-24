using CSX.Common.Data.Guids;
using CSX.Common.IO.Paths;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace XenoFx.Services.Processing.AssetIngestion;

public static partial class AssetIngestionExtensions
{
    public static string DetermineFilename(
        this DTOs.AssetUploadMetadata metadata)
    {
        string? filename = metadata
            .GetFilenameCandidates()
            .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));

        // If all fails, generate a guid
        return !string.IsNullOrWhiteSpace(filename)
            ? filename
            : DMC212710Guid.FromUtcNow().ToString();
    }

    public static IEnumerable<string> GetFilenameCandidates(
        this DTOs.AssetUploadMetadata metadata)
    {
        yield return FilenameKebabizer.Sanitize(
            Path.GetFileNameWithoutExtension(metadata.PreferredFilename));
        yield return FilenameKebabizer.Sanitize(
            Path.GetFileNameWithoutExtension(metadata.DeclaredFilename));

        yield return FilenameKebabizer.FromTitle(metadata.Title);
        yield return FilenameKebabizer.FromUrl(metadata.DataUrl);
        yield return FilenameKebabizer.FromUrl(metadata.PageUrl);

        yield return FilenameKebabizer.Sanitize(
            Path.GetFileNameWithoutExtension(metadata.CachedFilename));
    }
}