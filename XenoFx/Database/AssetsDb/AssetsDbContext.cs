using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using XenoFx.Database.AssetsDb.Models;
using XenoFx.Database.CacheDb;
using XenoFx.Database.CacheDb.Models;
using XenoFx.Database.ValueConverters;

namespace XenoFx.Database.AssetsDb;

/// <summary>
/// Database storage model for integrity and recovery metadata of a file repository.
/// </summary>
/// <param name="options">Database Connection and Runtime Options</param>
public class AssetsDbContext : DbContext
{
    // Size, Location, Created, Modified, etc.
    public DbSet<AssetIndentifier> Metadata { get; set; }

    // Hash, Recovery, etc.
    public DbSet<AssetIntegrityData> Integrity { get; set; }

    // Media type, length, resolution, codec, etc.
    public DbSet<AssetMediaInfo> Media { get; set; }

    // Playback settings, trimming, chapters, segments, etc.
    public DbSet<AssetPresentation> Presentation { get; set; }

    // Title, Description, Tags, associated urls, etc.
    public DbSet<AssetDescriptor> Description { get; set; }

    // Tags
    public DbSet<AssetTag> Tags { get; set; }

    // Lifetime

    public AssetsDbContext(DbContextOptions<AssetsDbContext> options) : base(options)
    { }

    // Encoding/Decoding setup

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Conversion: UInt128 ←→ byte[16]

        // Register unique columns

        modelBuilder.Entity<AssetTag>()
            .HasIndex(e => e.TagId)
            .IsUnique();
    }
}