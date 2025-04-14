using System;
using System.ComponentModel.DataAnnotations;

namespace XenoFx.Database.AssetsDb.Models;

public class AssetIntegrityData
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Hashes and recovery

    public byte[] SHA256 { get; set; } = [];
    public byte[] MD5 { get; set; } = [];
    public Guid SolomonReedId { get; set; } = Guid.Empty;
    public Guid Hamming { get; set; } = Guid.Empty;

    // Other

    public byte[] Crumbs { get; set; } = [];
}