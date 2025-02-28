using System;
using System.ComponentModel.DataAnnotations;

namespace XenoFx.Database.Models;

public partial class AssetRecord
{
    // Db Core

    [Key]
    public int Id { get; set; }

    public DateTime RecordCreatedOn { get; set; } = DateTime.UtcNow;
    public DateTime RecordModifiedOn { get; set; } = DateTime.UtcNow;

    // Quick Verification Metadata

    public DateTime FileFirstCreatedOn { get; set; } = DateTime.UtcNow;
    public DateTime FileLastModifiedOn { get; set; } = DateTime.UtcNow;
    public long FileSize { get; set; } = 0;

    // Integrity Metadata : SHA256

    public byte[] FileSha256 { get; set; } = [];
    public long BlockSizeSha256 { get; set; } = 0;
    public byte[][] BlocksSha256 { get; set; } = [];

    // Recovery Metadata

    public Guid SolomonReedId { get; set; } = Guid.Empty;
    public Guid Hamming { get; set; } = Guid.Empty;
}