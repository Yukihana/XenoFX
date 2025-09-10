using CSX.DotNet.Common.EFC.Abstractions;
using System;

namespace XenoFx.Database.AssetsDb.Models;

public class AssetIdentity : Auditable
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
}