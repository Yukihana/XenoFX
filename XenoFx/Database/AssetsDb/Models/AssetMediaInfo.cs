using System;
using System.ComponentModel.DataAnnotations;

namespace XenoFx.Database.AssetsDb.Models;

public class AssetMediaInfo
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}