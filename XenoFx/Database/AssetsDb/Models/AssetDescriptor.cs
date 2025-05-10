using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace XenoFx.Database.AssetsDb.Models;

public class AssetDescriptor
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Descriptions

    public string Title { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = [];

    // Historical Data

    public string OriginalTitle { get; set; } = string.Empty;
    public List<string> PageUrls { get; set; } = [];
    public List<string> DataUrls { get; set; } = [];
}