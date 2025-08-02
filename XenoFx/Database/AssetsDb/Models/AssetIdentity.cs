using CSX.DotNet.EFC.Common.Abstractions;
using System;

namespace XenoFx.Database.AssetsDb.Models;

public class AssetIdentity : IAuditable
{
    public Guid Id { get; set; }

    // FileInfo

    public long FileSize { get; set; } = 0;
    public DateTimeOffset FileCreatedAt { get; set; } = DateTimeOffset.MinValue;
    public DateTimeOffset FileModifiedAt { get; set; } = DateTimeOffset.MinValue;

    // Integrity

    public byte[] SHA256 { get; set; } = [];
    public byte[] Blake3 { get; set; } = [];
    public byte[] Crumbs { get; set; } = [];

    // Recovery (probably should move this to file system recovery utilities instead)
    // public Guid SolomonReedId { get; set; } = Guid.Empty;
    // public Guid Hamming { get; set; } = Guid.Empty;

    // Metadata

    public DateTimeOffset RecordCreatedAt { get; set; } = DateTimeOffset.MinValue;
    public DateTimeOffset RecordUpdatedAt { get; set; } = DateTimeOffset.MinValue;
    public bool IsRecordDeleted { get; set; } = false;
    public DateTimeOffset? RecordDeletedAt { get; set; } = DateTimeOffset.MinValue;
}