using CSX.DotNet.Common.EFC.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using XenoFx.Database.AssetsDb.Models;

namespace XenoFx.Database.AssetsDb;

/// <summary>
/// Database storage model for integrity and recovery metadata of a file repository.
/// </summary>
/// <param name="options">Database Connection and Runtime Options</param>
public class AssetsDbContext : BaseDbContext<AssetsDbContext>
{
    // Identity hashes and metadata
    public DbSet<AssetIdentity> Identities { get; set; }

    // Media type, length, resolution, codec, etc.
    public DbSet<AssetMediaInfo> Media { get; set; }

    // Playback settings, trimming, chapters, segments, etc.
    public DbSet<AssetPresentation> Presentation { get; set; }

    // Title, Description, Tags, associated urls, etc.
    public DbSet<AssetDescriptor> Description { get; set; }

    // Tags
    public DbSet<AssetTag> Tags { get; set; }

    // Lifetime

    public AssetsDbContext(
        DbContextOptions<AssetsDbContext> options,
        ILogger<AssetsDbContext> logger)
        : base(options, logger)
    { }

    // Encoding/Decoding setup

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.SpecifyAssetIdentity();

        // Conversion: UInt128 ←→ byte[16]

        // Register primary keys

        // Register unique columns

        modelBuilder.Entity<AssetTag>()
            .HasIndex(e => e.TagId)
            .IsUnique();
    }
}