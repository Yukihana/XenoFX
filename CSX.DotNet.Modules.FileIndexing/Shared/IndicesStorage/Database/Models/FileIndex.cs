using CSX.DotNet.Common.EFC.Abstractions;
using System;

namespace CSX.DotNet.Modules.FileIndexing.Shared.IndicesStorage.Database.Models;

public class FileIndex : AuditableEntityBase
{
    public Guid Id { get; set; } = Guid.Empty;

    // Heuristic - Fast Resync

    public string Location { get; set; } = string.Empty;
    public long Length { get; set; } = 0;
    public DateTimeOffset FileModifiedAt { get; set; } = DateTimeOffset.MinValue;

    // For modified events, use this as reference point:

    public DateTimeOffset FileCreatedAt { get; set; } = DateTimeOffset.MinValue;

    // Partial - Fast resync

    public byte[]? Crumbs { get; set; } = null;

    // Exact - Full resync

    public byte[]? SHA256 { get; set; } = null;
    public byte[]? Blake3 { get; set; } = null;

    // Recovery (probably should move this to file system recovery utilities instead)
    // public Guid SolomonReedId { get; set; } = Guid.Empty;
    // public Guid Hamming { get; set; } = Guid.Empty;
}