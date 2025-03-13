using System;
using System.ComponentModel.DataAnnotations;

namespace XenoFx.Database.Cache.Models;

public sealed class AssetPresenceInfo
{
    [Key]
    public int Id { get; set; } = 0;

    public string RelativePath { get; set; } = string.Empty;
    public UInt128 AssetId { get; set; } = UInt128.Zero;
    public ulong StateIndex { get; set; } = 0;
}