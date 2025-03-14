using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using XenoFx.Database.Cache.Models;

namespace XenoFx.Database.Cache;

public class CacheDbContext : DbContext
{
    // Data

    public DbSet<AssetPresenceInfo> AssetPresences { get; set; }

    // Lifetime

    public CacheDbContext(DbContextOptions<CacheDbContext> options) : base(options)
    { }

    // Encoding/Decoding setup
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // UInt128 Property <-> byte[16] Column
        var uint128Converter = new ValueConverter<UInt128, byte[]>(
            v => BitConverter.GetBytes(v),
            v => BitConverter.ToUInt128(v)
        );

        // Register conversions
        modelBuilder.Entity<AssetPresenceInfo>()
            .Property(e => e.AssetId)
            .HasConversion(uint128Converter);

        // Register unique columns
        modelBuilder.Entity<AssetPresenceInfo>()
            .HasIndex(e => e.RelativePath)
            .IsUnique();
    }
}