using System;
using System.ComponentModel.DataAnnotations;

namespace XenoFx.Database.AssetsDb.Models;

public class AssetIndentifier
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Reference

    public string Location { get; set; } = string.Empty;

    // File system

    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime Modified { get; set; } = DateTime.UtcNow;
    public long Size { get; set; } = 0;
}