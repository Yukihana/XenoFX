using System;

namespace CSX.DotNet.Common.Data.Events;

public class FileUploadedEventArgs(string temporaryFileFullPath) : EventArgs
{
    // Required

    public string TemporaryFileFullPath { get; }
        = temporaryFileFullPath
        ?? throw new ArgumentNullException(nameof(temporaryFileFullPath));

    public string ReportedFilename { get; init; } = string.Empty;
    public string ReportedMimeType { get; init; } = string.Empty;

    // Metadata

    public string Title { get; init; } = string.Empty;
    public string PageUrl { get; init; } = string.Empty;
    public string DataUrl { get; init; } = string.Empty;

    // Optional

    public string PreferredFilename { get; init; } = string.Empty;
    public string ExtraDataRaw { get; init; } = string.Empty;
}