using System.ComponentModel.DataAnnotations;
using System;

namespace XenoFx.Database.AssetsDb.Models;

public class AssetPresentation
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}