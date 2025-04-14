using System;
using System.ComponentModel.DataAnnotations;

namespace XenoFx.Database.AssetsDb.Models;

public partial class AssetTag
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Reference

    public string TagId { get; set; } = string.Empty;

    // Description

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}