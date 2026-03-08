using System;

namespace CSX.DotNet.Common.IO.Integrity.Files;

public record FileSignature :
    IFileHeuristicsSignature,
    IFileContentSignature,
    IFileHashSignature
{
    // IFileHeuristicsSignature

    public string? Location { get; set; } = null;
    public DateTimeOffset? CreatedUtc { get; set; } = null;
    public DateTimeOffset? ModifiedUtc { get; set; } = null;
    public long? Size { get; set; } = null;

    // IFileContentSignature

    public byte[]? Crumbs { get; set; } = null;

    // IFileHashSignature

    public byte[]? SHA256 { get; set; } = null;
    public byte[]? Blake3 { get; set; } = null;
}